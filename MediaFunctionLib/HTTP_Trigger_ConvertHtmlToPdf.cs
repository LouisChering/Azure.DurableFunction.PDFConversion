using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using OpenHtmlToPdf;
using MediaFunctionLib.Helper;
using MediaFunctionLib.Activities;

namespace MediaFunctionLib
{
    public static class HTTP_Trigger_ConvertHtmlToPdf
    {
        [FunctionName("HTTP_ConvertHtmlToPdf")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            Binder binder)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic json = new { };
             json = JsonConvert.DeserializeObject(requestBody);

       
            var pdfBytes = await ProducePDF.ProducePdf(new string[] { json.Template, json.Html  }, log,binder);

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(pdfBytes);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            { FileName = "file.pdf" };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            return result;
        }
    }
}
