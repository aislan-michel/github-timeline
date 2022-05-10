namespace GitHubTimeline.App.Models
{
    public class Summary
    {
        protected Summary()
        {

        }

        public Summary(int numberOfGitHubReposTallied, int createdYear)
        {
            NumberOfGitHubReposTallied = numberOfGitHubReposTallied;
            CreatedYear = createdYear;
        }

        public int NumberOfGitHubReposTallied { get; set; } 
        public int CreatedYear { get; set; }
    }
}
