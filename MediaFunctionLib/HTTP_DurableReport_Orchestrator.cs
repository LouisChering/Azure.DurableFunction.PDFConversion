using MediaFunctionLib.Models;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MediaFunctionLib
{
    public class HTTP_DurableReport_Orchestrator
    {
        [FunctionName("HTTP_Durable_Report_Orchestrator")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var runContext = context.GetInput<ReportContext>();
            runContext.Date = DateTime.Now;
            var outputs = new List<string>();

            // First task loads html from template in blob storage and updates the content inside
            var updatedHtml = await context.CallActivityAsync<string>("ReplaceHtmlContent", new object[] { runContext.Template, runContext.JSON });

            // convert html into pdf, upload to blob storage and get download link
            var pdf = await context.CallActivityAsync<string>("ProducePdf", new string[] { updatedHtml, runContext.ReportName });
            outputs.Add(pdf);

            var email = new EmailContext
            {
                BCC = runContext.Recipients,
                Subject = $"Report generated {runContext.ReportName} @ {DateTime.Now}",
                body = $"Report Access Link: <hr/> <a href=\"{pdf}\">{pdf}</a>"
            };

            // run the next two tasks at the same time
            var tasks = new List<Task> {
                context.CallActivityAsync("EmailReportNotification", email),
                context.CallActivityAsync("AuditReport", runContext)
            };

            await Task.WhenAll(tasks);

            return outputs;
        }
    }
}
