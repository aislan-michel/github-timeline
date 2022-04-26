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

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Generate(string gitHubUserName)
        {
            Console.WriteLine(gitHubUserName);

            var httpClient = _httpClientFactory.CreateClient("GitHub");

            var httpResponseMessage = await httpClient.GetAsync($"users/{gitHubUserName}/repos");

            if (!httpResponseMessage.IsSuccessStatusCode)
            { 
                return BadRequest();
            }

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            var githubRepos = await JsonSerializer.DeserializeAsync<IEnumerable<GitHubRepos>>(contentStream);

            throw new NotImplementedException();
        }
    }
}
