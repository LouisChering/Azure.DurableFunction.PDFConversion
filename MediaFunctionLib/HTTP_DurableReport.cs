using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MediaFunctionLib.Helper;
using MediaFunctionLib.Models;
using MediaFunctionMedia.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MediaFunctionLib
{
    public static class HTTP_DurableReport
    {
        [FunctionName("HTTP_Durable_Report")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            //get json from inbound request
            dynamic json = new { };

            // mocked for the moment
            var context = new ReportContext
            {
                Template = "templates/Template.html",
                JSON = new { somevalue = 1 },
                ReportName = "my-report.pdf",
                Recipients = new string[] { "email@email.com" }
            };

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("HTTP_Durable_Report_Orchestrator", context);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        
    }



    
}