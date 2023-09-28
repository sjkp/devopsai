using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using tryAGI.OpenAI;

namespace CSharp_OpenAI_LangChain
{

    [OpenAiFunctions]
    public interface ICveLookupService
    { 
        [Description("Get information about software vulnerabilities from a CVE database")]
        Task<string> GetCveDescription(
            [Description("Id for the CVE, in the format ghsa-XXXX-XXXX-XXXX where XXXX is number or letters, the response is returned as html and contains information about affected versions, severity and a description of the vulnerability")]
            string id,            
            CancellationToken cancellationToken);

        
    }
    public class CveLookupService : ICveLookupService
    {

        public async Task<string> GetCveDescription(string id, CancellationToken cancellationToken)
        {
            var client = new HttpClient();

            var res = await client.GetStringAsync($"https://cvelookup.netlify.app/{id}");

            return res;
        }
    }
}
