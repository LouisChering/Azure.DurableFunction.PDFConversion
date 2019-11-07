using MediaFunctionLib.Helper;
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
    public class EmailReportNotification
    {
        [FunctionName("EmailReportNotification")]
        public async static Task Run([ActivityTrigger] EmailContext context, ILogger log, Binder binder)
        {
            log.LogInformation($"Sending out email notifications...");

            var mailhelper = new EmailHelper(binder);

            try
            {
                await mailhelper.SendMail(context.Recipients, context.CC, context.BCC, context.Subject, context.body);
            }
            catch (Exception)
            {
                log.LogError("This section is done on purpose to remind us to implement an email strategy");
            }

            // return output
            log.LogInformation($"Sending out email notifications.");
        }
    }
}
