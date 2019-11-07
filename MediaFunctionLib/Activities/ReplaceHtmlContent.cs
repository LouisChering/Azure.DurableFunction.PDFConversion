using MediaFunctionLib.Helper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MediaFunctionLib.Activities
{
    public class ReplaceHtmlContent
    {
        [FunctionName("ReplaceHtmlContent")]
        public async static Task<string> Run([ActivityTrigger] object[] args, ILogger log, Binder binder)
        {
            log.LogInformation($"Replacing html content...");

            //add null checks here
            string templateName = args[0].ToString();
            dynamic json = args[1];

            // retrieve the template from blob storage
            var html = await binder.BindAsync<string>(new BlobAttribute(templateName, FileAccess.Read));

            //update the contents
            var replacedContents = HtmlHelper.InsertJsonIntoHtml(html, json);

            log.LogInformation($"Replaced html content.");

            // return output
            return replacedContents;
        }
    }
}
