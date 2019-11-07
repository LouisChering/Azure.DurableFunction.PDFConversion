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
    public class ProducePDF
    {
        [FunctionName("ProducePdf")]
        public async static Task<byte[]> ProducePdf([ActivityTrigger] string[] args, ILogger log, Binder binder)
        {
            log.LogInformation($"Generating pdf document...");

            //add null checks here
            var html = args[0];
            var fileName = args[1];

            // convert string html into pdf
            var pdf = HtmlHelper.ConvertHtmlToPdf(html);

            var attributes = new Attribute[]
            {
                new BlobAttribute($"output/{fileName}",FileAccess.Write),
                new StorageAccountAttribute("AzureWebJobsStorage")
            };

            using (var writer = await binder.BindAsync<Stream>(attributes))
            {
                Stream stream = new MemoryStream(pdf);
                await stream.CopyToAsync(writer);
            }

            // return output
            log.LogInformation($"Generated pdf document.");

            return pdf;
        }
    }
}
