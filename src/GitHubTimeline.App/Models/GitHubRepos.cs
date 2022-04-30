using System.Text.Json.Serialization;

namespace GitHubTimeline.App.Models
{
    public class GitHubRepos
    {
        [JsonPropertyName("name")] 
        public string? Name { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
