// See https://aka.ms/new-console-template for more information

using CSharp_OpenAI_LangChain;
using LangChain.Chains.LLM;
using LangChain.Chains.Sequentials;
using LangChain.Prompts;
using LangChain.Providers;
using LangChain.Providers.Azure;
using LangChain.Schema;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

var builder = ConsoleApp.CreateBuilder(args);


builder.ConfigureHostConfiguration(c =>
{
    c.AddUserSecrets<Program>();
});

builder.ConfigureServices((ctx, services) =>
{
    
});
var app = builder.Build();
app.AddCommands<DevOpsLangChain>();
app.Run();


public class DevOpsLangChain : ConsoleAppBase
{
    private readonly string apiKey;
    private readonly string endpoint;
    private readonly IConfiguration config;
    private readonly string sqlConnection; 

    public DevOpsLangChain(IConfiguration config)
    {
        
        
        this.endpoint = config.GetValue<string>("AZURE_OPENAI_ENDPOINT") ?? "https://dgopenai-us.openai.azure.com/openai/deployments/gpt4functioncalling";
        this.config = config;
        this.apiKey = GetEnvironmentValue("AZURE_OPENAI_APIKEY");
        this.sqlConnection = GetEnvironmentValue("SQL_CONNECTION");
    }

    [Command("ask", "Ask ChatGPT for assistance")]
    public async Task Run(
        [Option("sys", "System message to prime ChatGPT with")] string? systemMessage = null,
        [Option("msg", "Message to send to ChatGpt")] string? humanMessage = null,
        [Option("project", "The Azure DevOps Project")] string? project = null,        
        [Option("verbose")]bool verbose = false)
    {
        var nugetService = new NugetService();
        var githubService = new GithubService();
        var cveService = new CveLookupService();
        //var res = await nugetService.GetPackageAsync("ardalis.liststartupservices", "1.1.4");
        //Console.WriteLine(res.Metadata.Authors);

        //var res = await githubService.GetRepoInformationAsync("octokit", "octokit.net");
        //Console.WriteLine(res.StargazersCount);
        //Console.ReadLine();

               
        using var httpClient = new HttpClient(new LoggingHandler(new HttpClientHandler(), verbose));



        var model = new AzureModel(apiKey, endpoint, httpClient, "gpt-4");

       // model.AddGlobalFunctions(nugetService.AsFunctions(), nugetService.AsCalls());
      //  model.AddGlobalFunctions(githubService.AsFunctions(), githubService.AsCalls());
      //  model.AddGlobalFunctions(cveService.AsFunctions(), cveService.AsCalls());
        if (project != null)
        {
            Console.WriteLine("Adding Azure DevOps Functions");
            var devopsService = new AzureDevOpsService(new Uri(GetEnvironmentValue("AZURE_DEVOPS_URI")), project, GetEnvironmentValue("AZURE_DEVOPS_PAT"));
            model.AddGlobalFunctions(devopsService.AsFunctions(), devopsService.AsCalls());
        }

        if (!string.IsNullOrEmpty(sqlConnection))
        {
            Console.WriteLine("Adding SQL Functions");
            var sqlService = new SqlService(sqlConnection);
            model.AddGlobalFunctions(sqlService.AsFunctions(), sqlService.AsCalls());
        }

        //List<LangChain.Providers.Message> messages = new List<Message>()
        //{
        //    (systemMessage ?? "You are a company license validator and should ensure that all licenses used are MIT, if you find any license that are not MIT you should reply with STOP BUILD").AsSystemMessage()
        //};

        //messages.AddRange((humanMessage ?? "@For the following nuget packages please find their license   > Ardalis.ListStartupServices                               1.1.3       1.1.3 \n > MediatR.Extensions.Microsoft.DependencyInjection          10.0.1      10.0.1").Split(";;").Select(s => s.AsHumanMessage()));

        //var response = await model.GenerateAsync(new ChatRequest(messages));


        //Console.WriteLine(response.Messages.AsHistory());
        ////var numberOfTokens = model.CountTokens("Hello, World of AI!");


        var firstPrompt = new PromptTemplate(new PromptTemplateInput("List all azure devops workitems items in a markdown table with Title and StoryPoints from the project GPTReviewTest in iterationpath GPTReviewTest\\iteration 1", new List<string> { }));

        var chainOne = new LlmChain(new LlmChainInput(model, firstPrompt)
        {
            OutputKey = "adotable",
        });

        var secondTemplate = "describe the table TimeEntries from TimeReg sql database";
        var secondPrompt = new PromptTemplate(new PromptTemplateInput(secondTemplate, new List<string> { }));

        var chainTwo = new LlmChain(new LlmChainInput(model, secondPrompt) { OutputKey = "sqldescribe", Verbose = true });

        var thridPrompt = new PromptTemplate(new PromptTemplateInput("for each work item in {adotable} find the the number of hours in the timereg sql database with the following definition {sqldescribe} present the result as a markdown table containing Title, StoryPoint and Hours", new List<string> { "sqldescribe", "adotable" }));

        var chainThree = new LlmChain(new LlmChainInput(model, thridPrompt) { OutputKey = "text"});

        var overallChain = new SequentialChain(new SequentialChainInput(new[]
        {
            chainOne,
            chainTwo,
            chainThree
        }, new string[] { }));

        var result = await overallChain.CallAsync(new ChainValues(new Dictionary<string, object>(1)
        {

        }));

        //        var firstTemplate = "What is a good name for a company that makes {product}?";
        //        var firstPrompt = new PromptTemplate(new PromptTemplateInput(firstTemplate, new List<string>(1) { "product" }));

        //        var chainOne = new LlmChain(new LlmChainInput(model, firstPrompt)
        //        {
        //            OutputKey = "company_name"
        //        });

        //        var secondTemplate = "Write a 20 words description for the following company:{company_name}";
        //        var secondPrompt = new PromptTemplate(new PromptTemplateInput(secondTemplate, new List<string>(1) { "company_name" }));

        //        var chainTwo = new LlmChain(new LlmChainInput(model, secondPrompt) { OutputKey = "text"});

        //        var overallChain = new SequentialChain(new SequentialChainInput(new[]
        //        {
        //    chainOne,
        //    chainTwo
        //}, new[] { "product" }, new string[] {}));

        //        var result = await overallChain.CallAsync(new ChainValues(new Dictionary<string, object>(1)
        //{
        //    { "product", "colourful socks" }
        //}));

        Console.WriteLine(result.Value["text"]);
    }

    private string GetEnvironmentValue(string name)
    {
        return config.GetValue<string>(name) ?? throw new InvalidProgramException($"Environment Variable {name} must exists");
    }
}