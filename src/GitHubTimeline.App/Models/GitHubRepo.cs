using System.Text.Json.Serialization;

namespace GitHubTimeline.App.Models
{
    public class GitHubRepo
    {
        [JsonPropertyName("name")] 
        public string? Name { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.MinValue; 
    }
}
