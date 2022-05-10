using GitHubTimeline.App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GitHubTimeline.App.Controllers
{
    public class TimelineController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TimelineController> _logger;
        private static readonly IList<GitHubUser> Cache = new List<GitHubUser>();

        public TimelineController(IHttpClientFactory httpClientFactory, ILogger<TimelineController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Generate()
        {
            return View(new GitHubUser(string.Empty));
        }

        [HttpPost]
        public async Task<IActionResult> Generate(GitHubUser gitHubUser)
        {
            try
            {
                if (!await UserNameExists(gitHubUser.GitHubUserName))
                {
                    ModelState.AddModelError("GitHubUserName", "not found");

                    return View(gitHubUser);
                }

                var gitHubRepos = await GetGitHubRepos(gitHubUser.GitHubUserName);

                gitHubUser.Repositories.AddRange(gitHubRepos.OrderByDescending(x => x.CreatedAt));

                Cache.Add(gitHubUser);

                return View("Repositories", gitHubUser);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        private async Task<IEnumerable<GitHubRepo>> GetGitHubRepos(string gitHubUserName)
        {
            var httpClient = _httpClientFactory.CreateClient("GitHub");

            var httpResponseMessage = await httpClient.GetAsync($"users/{gitHubUserName}/repos");

            await using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            var json = await JsonSerializer.DeserializeAsync<IEnumerable<GitHubRepo>>(contentStream);

            if (json == null)
            {
                throw new NullReferenceException(nameof(json));
            }

            return json;
        }

        private async Task<bool> UserNameExists(string gitHubUserName)
        {
            var httpClient = _httpClientFactory.CreateClient("GitHub");
            
            var httpResponseMessage = await httpClient.GetAsync($"users/{gitHubUserName}");
            
            return httpResponseMessage.IsSuccessStatusCode;
        }

        public IActionResult Summary(string gitHubUserName)
        {
            var model = Cache.FirstOrDefault(x => x.GitHubUserName == gitHubUserName);

            var summary = model?.Repositories
                .Select(repo =>
                {
                    var createdYear = repo.CreatedAt.Year;
                    var numberOfGitHubReposTallied = model.GetNumberOfGitHubReposTallied(createdYear);

                    return new Summary(createdYear, numberOfGitHubReposTallied);
                }).Distinct();

            return View(summary);
        }
    }
}
