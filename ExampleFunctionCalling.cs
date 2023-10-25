using Microsoft.VisualStudio.Services.WebApi;
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
    public interface IExampleFunctionCalling
    {
        [Description("What should the function call be able to help with")]
        Task<object> CreateWorkItem(
            [Description("The example of the first input")]
            string input1,                        
            CancellationToken cancellationToken);

       
    }
}
