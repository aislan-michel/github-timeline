using GitHubTimeline.App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GitHubTimeline.App.Controllers
{
    public class TimelineController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TimelineController> _logger;
        private static IList<GitHubUser> _cache = new List<GitHubUser>();

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
        public async Task<IActionResult> Generate(GitHubUser model)
        {
            try
            {
                if (!await UserNameExists(model.GitHubUserName))
                {
                    ModelState.AddModelError("GitHubUserName", "not found");

                    return View(model);
                }

                var gitHubRepos = await GetGitHubRepos(model.GitHubUserName);

                model.Repositories.AddRange(gitHubRepos);

                _cache.Add(model);

                return View("Repositories", gitHubRepos?.OrderByDescending(x => x.CreatedAt));
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

            return await JsonSerializer.DeserializeAsync<IEnumerable<GitHubRepo>>(contentStream);
        }

        private async Task<bool> UserNameExists(string gitHubUserName)
        {
            var httpClient = _httpClientFactory.CreateClient("GitHub");
            
            var httpResponseMessage = await httpClient.GetAsync($"users/{gitHubUserName}");
            
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }

        public IActionResult Summary(string gitHubUserName)
        {
            var model = _cache.FirstOrDefault(x => x.GitHubUserName == gitHubUserName);

            var summary = model?.Repositories
                .Select(repo =>
                {
                    var createdYear = repo.CreatedAt.Year;
                    var numberOfGitHubReposTallied = model.GetNumberOfGitHubReposTallied(createdYear);

                    return new Summary(createdYear, numberOfGitHubReposTallied);
                });

            return View(summary);
        }
    }
}
