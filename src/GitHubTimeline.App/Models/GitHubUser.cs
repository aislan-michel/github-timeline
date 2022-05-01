namespace GitHubTimeline.App.Models;

public class GitHubUser
{
    public GitHubUser()
    {

    }

    public GitHubUser(string gitHubUserName)
    {
        GitHubUserName = gitHubUserName;
    }

    public string GitHubUserName { get; set; } = string.Empty;
    public List<GitHubRepo> Repositories { get; set; } = new List<GitHubRepo>();

    public int GetNumberOfGitHubReposTallied(int createdYear)
    {
        return Repositories.Where(y => y.CreatedAt.Year == createdYear).Count();
    }
}