﻿// See https://aka.ms/new-console-template for more information

using CSharp_OpenAI_LangChain;
using LangChain.Providers;
using LangChain.Providers.Azure;

var app = ConsoleApp.Create(args);
app.AddCommands<DevOpsLangChain>();
app.Run();

#if DEBUG
Console.ReadLine();
#endif

public class DevOpsLangChain : ConsoleAppBase
{
    [Command("ask", "Ask ChatGPT for assistance")]
    public async Task Run([Option("sys", "System message to prime ChatGPT with")] string? systemMessage = null, [Option("msg", "Message to send to ChatGpt")] string? humanMessage =null, [Option("verbose")]bool verbose = false)
    {
        var nugetService = new NugetService();
        var githubService = new GithubService();
        //var res = await nugetService.GetPackageAsync("ardalis.liststartupservices", "1.1.4");
        //Console.WriteLine(res.Metadata.Authors);

        //var res = await githubService.GetRepoInformationAsync("octokit", "octokit.net");
        //Console.WriteLine(res.StargazersCount);
        //Console.ReadLine();



        string apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_APIKEY") ?? throw new InvalidProgramException("Enviroment Variable APIKEY must contain the Azure Open AI apikey to use");
        string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "https://dgopenai-us.openai.azure.com/openai/deployments/gpt4functioncalling";
        using var httpClient = new HttpClient(new LoggingHandler(new HttpClientHandler(), verbose));



        var model = new AzureModel(apiKey, endpoint, httpClient, "gpt-4");

        model.AddGlobalFunctions(nugetService.AsFunctions(), nugetService.AsCalls());
        model.AddGlobalFunctions(githubService.AsFunctions(), githubService.AsCalls());

        var response = await model.GenerateAsync(new ChatRequest(new[] {
    (systemMessage ?? "You are a company license validator and should ensure that all licenses used are MIT, if you find any license that are not MIT you should reply with STOP BUILD").AsSystemMessage(),
    (humanMessage ?? "@For the following nuget packages please find their license   > Ardalis.ListStartupServices                               1.1.3       1.1.3 \n > MediatR.Extensions.Microsoft.DependencyInjection          10.0.1      10.0.1").AsHumanMessage()
}));


        Console.WriteLine(response.Messages.AsHistory());
        //var numberOfTokens = model.CountTokens("Hello, World of AI!");

        
    }
}