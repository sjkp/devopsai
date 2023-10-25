using Microsoft.Extensions.Configuration;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
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
    public interface IAzureDevOpsService
    {
        [Description("Create a workitem in Azure DevOps")]
        Task<WorkItem> CreateWorkItem(
            [Description("The title for the workitem")]
            string title,
            [Description("The description of the workitem")]
            string description,
            [Description("The type of work item, can be Bug, Task, Feature, User Story, Product Backlog Item, or Epic")]
            string type,
            CancellationToken cancellationToken);

        [Description("Get workitems references that are linked to a build")]
        Task<List<ResourceRef>> GetPipelineRun(
            [Description("The build id")]
            int buildId, CancellationToken cancellationToken);

        [Description("Get all workitems based on a Azure DevOps WIQL query")]
        Task<IEnumerable<WorkItem>> QueryWorkItems(
            [Description("The WIQL query that should be executed")]
            string query, CancellationToken cancellationToken);

        [Description("Returns information about a workitem stored in Azure DevOps")]
        Task<WorkItem> GetWorkItemInfo(
            [Description("The workitem id")]
            int workItemId,            
            CancellationToken cancellationToken);
    }
    public class AzureDevOpsService : IAzureDevOpsService
    {
        private readonly VssConnection connection;
        private readonly string project;

        public AzureDevOpsService(IConfiguration configuration, string project) : this(new Uri(configuration["AZURE_DEVOPS_URI"]), project, configuration["AZURE_DEVOPS_PAT"])
        {

        }

        public AzureDevOpsService(Uri orgUrl, string project, string personalAccessToken)
        {
            connection = new VssConnection(orgUrl, new VssBasicCredential(string.Empty, personalAccessToken));
            this.project = project;
        }

        public async Task<WorkItem> GetWorkItemInfo(int workItemId, CancellationToken cancellationToken)
        {
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            var workitem = await witClient.GetWorkItemAsync(workItemId);

            return workitem;
        }

        public async Task<List<WorkItem>> GetWorkItemsInfo(IEnumerable<int> workItemId, CancellationToken cancellationToken)
        {
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            var workitem = await witClient.GetWorkItemsAsync(workItemId);

            return workitem;
        }

        public async Task<IEnumerable<WorkItem>> QueryWorkItems(string query, CancellationToken cancellationToken)
        {
            var witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            var wiql = new Wiql() { Query = query };
            var res = await witClient.QueryByWiqlAsync(wiql);

            return await GetWorkItemsInfo(res.WorkItems.Select(s => s.Id), cancellationToken);
        }

        public async Task<WorkItem> CreateWorkItem(string title, string description, string type, CancellationToken cancellationToken)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            //add fields and their values to your patch document
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = title
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Description",
                    Value = description
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Common.Priority",
                    Value = "1"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Common.Severity",
                    Value = "2 - High"
                }
            );
            
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                WorkItem result = await workItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, project, type);

                Console.WriteLine("Bug Successfully Created: Bug #{0}", result.Id);

                return result;
            }
            catch (AggregateException ex)
            {
                Console.WriteLine("Error creating bug: {0}", ex.InnerException.Message);
                return null;
            }
        }

        public async Task<List<ResourceRef>> GetPipelineRun(int buildId, CancellationToken cancellationToken)
        {
            BuildHttpClient buildClient = connection.GetClient<BuildHttpClient>();

            var refs = await buildClient.GetBuildWorkItemsRefsAsync(project, buildId);
            return refs;
        }
    }
}
