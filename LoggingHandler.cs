using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_OpenAI_LangChain
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly bool verbose;

        public LoggingHandler(HttpMessageHandler innerHandler, bool verbose)
            : base(innerHandler)
        {
            this.verbose = verbose;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (verbose)
            {
                Console.WriteLine("Request:");
                Console.WriteLine(request.ToString());
                if (request.Content != null)
                {
                    Console.WriteLine(await request.Content.ReadAsStringAsync());
                }
                Console.WriteLine();
            }
            request.RequestUri = new Uri(request.RequestUri.ToString() + "?api-version=2023-07-01-preview");
            var auth = request.Headers.Authorization;
            request.Headers.TryAddWithoutValidation("api-key", auth.Parameter);
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            if (verbose)
            {
                Console.WriteLine("Response:");
                Console.WriteLine(response.ToString());
                if (response.Content != null)
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
                Console.WriteLine();
            }

            return response;
        }
    }

}
