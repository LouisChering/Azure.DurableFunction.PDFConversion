using MediaFunctionLib.Models;
using MediaFunctionMedia.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MediaFunctionLib.Activities
{
    public class AuditReport
    {
        [FunctionName("AuditReport")]
        public async static Task Run([ActivityTrigger] ReportContext context, ILogger log, Binder binder)
        {
            log.LogInformation($"Recording audit...");

            var attributes = new Attribute[]
            {
                new StorageAccountAttribute("AzureWebJobsStorage"),
                new TableAttribute("ReportAudit")
            };

            var output = await binder.BindAsync<IAsyncCollector<ReportAudit>>(attributes);
            await output.AddAsync(new ReportAudit
            {
                PartitionKey = context.ReportName,
                Recipients = context.Recipients,
                ReportName = context.ReportName,
                RowKey = context.Date.ToString("hh:mm:ss:fff-dd-MM-yyyy")
            });

            log.LogInformation($"Recored audit...");

        }
    }
}
