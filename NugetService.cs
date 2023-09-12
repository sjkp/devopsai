using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using tryAGI.OpenAI;
using NuGet.Protocol;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Microsoft.Extensions.Logging;

namespace CSharp_OpenAI_LangChain
{
    [OpenAiFunctions]
    public interface INugetService
    {
        [Description("Returns information from the nuspec for any nuget package. Nuspec contains authors, license type, github project url")]
        public Task<IPackageSearchMetadata> GetPackageAsync(
             [Description("The name of the nuget package in in lower case")]
            string packageName,
             [Description("The version number of the nuget package in format MAJOR.MINOR.PATCH-SUFFIX where -SUFFIX is optional")]
            string version,
            CancellationToken cancellationToken = default);
    }
    public class NugetService : INugetService
    {
        public async Task<IPackageSearchMetadata> GetPackageAsync(string packageName, string version, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($" LOOKING UP {packageName} {version}");
            SourceCacheContext cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            PackageMetadataResource resource = await repository.GetResourceAsync<PackageMetadataResource>();



            IEnumerable<IPackageSearchMetadata> packages = await resource.GetMetadataAsync(
    packageName,
    includePrerelease: true,
    includeUnlisted: false,
    cache,
    NullLogger.Instance,
    cancellationToken);

            foreach (var p in packages)
            {

                if (p.Identity.Version == new NuGetVersion(version))
                {
                    return p;
                }
            }
            return null;
        }
    }


}
