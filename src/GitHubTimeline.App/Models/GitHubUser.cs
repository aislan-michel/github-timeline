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
    public List<GitHubRepo> Repositories { get; set; } = new();

    public int GetNumberOfGitHubReposTallied(int createdYear)
    {
        return Repositories.Count(x => x.CreatedAt.Year == createdYear);
    }
}