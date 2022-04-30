using GitHubTimeline.App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GitHubTimeline.App.Controllers
{
    public class TimelineController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TimelineController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Generate()
        {
            return View(new Generate());
        }

        [HttpPost]
        public async Task<IActionResult> Generate(Generate command)
        {
            var httpClient = _httpClientFactory.CreateClient("GitHub");
            
            if (!await UserNameExists(command.GitHubUserName))
            { 
                ModelState.AddModelError("GitHubUserName", "not found");

                return View(command);
            }
            
            var httpResponseMessage = await httpClient.GetAsync($"users/{command.GitHubUserName}/repos");

            await using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            var githubRepos = await JsonSerializer.DeserializeAsync<IEnumerable<GitHubRepos>>(contentStream);

            return View("Repositories", githubRepos);
        }

        private async Task<bool> UserNameExists(string gitHubUserName)
        {
            Console.WriteLine(gitHubUserName);
            
            var httpClient = _httpClientFactory.CreateClient("GitHub");
            
            var httpResponseMessage = await httpClient.GetAsync($"users/{gitHubUserName}");
            
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
    }
}
