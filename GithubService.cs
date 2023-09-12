using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tryAGI.OpenAI;

namespace CSharp_OpenAI_LangChain
{
    [OpenAiFunctions]
    public interface IGithubService
    {
        [Description("Returns information from github about any public repository, returns information such as forks, stars, license and number of issues")]
        Task<Repository> GetRepoInformationAsync(
            [Description("The owner of the github repository, found as the {owner} part of the url https://github.com/{owner}")]
            string repositoryOwner,
            [Description("The name of the github repository, found as the {repositoryName} part of the url https://github.com/{owner}/{repositoryName}")]
            string repositoryName,
            CancellationToken cancellationToken);
    }
    public class GithubService : IGithubService
    {
        public async Task<Repository> GetRepoInformationAsync(string repositoryOwner, string repositoryName, CancellationToken cancellationToken = default)
        {
            var client = new GitHubClient(new ProductHeaderValue("sjkp.devops-openai-langchain"));
            var repo = await client.Repository.Get(repositoryOwner, repositoryName);
            return repo;
        }
    }
}
