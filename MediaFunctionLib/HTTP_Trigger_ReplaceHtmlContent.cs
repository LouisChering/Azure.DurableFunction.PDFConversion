using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Linq;
using MediaFunctionLib.Helper;

namespace MediaFunctionLib
{
    public static class ReplaceHtmlContent
    {
        /// <summary>
        /// This function takes in html content and a json object and will output an html string with updated strings
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <param name="html"> todo: replace this with a stream to handle larger html batch sizes</param>
        /// <returns></returns>
        [FunctionName("HTTP_ReplaceHtmlContent")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            [Blob("templates/Template.html", FileAccess.Read)]string html)
        {
            if (req == null)
            {
                log.LogCritical("Poorly formatted request inbound, unable to create HttpRequest object");
                return ErrorMessage("Poorly formatted request", HttpStatusCode.BadRequest);
            } else if (log == null)
            {
                log.LogCritical("Unable to get logger for azure function");
                return ErrorMessage("Unable to get logger for azure function", HttpStatusCode.InternalServerError);
            }

            log.LogInformation("ReplaceHtmlContent processing incoming request");

            //1. read incoming json from body
            string requestBody = new StreamReader(req.Body).ReadToEnd();

            dynamic json = new { };

            //handle bad json for code coverage
            try
            {
                json = JsonConvert.DeserializeObject(requestBody);
            }
            catch (Newtonsoft.Json.JsonReaderException formattingException)
            {
                log.LogError(formattingException.Message);
                var message = "Invalid JSON provided";
                return ErrorMessage(message, HttpStatusCode.BadRequest);
            }

            //error handling for code coverage if the html template or json is missing from this request
            if (string.IsNullOrWhiteSpace(html) || string.IsNullOrWhiteSpace(requestBody) || requestBody.Length <= 2)
            {
                var message = string.IsNullOrWhiteSpace(html) ? "No HTML provided" : "No JSON provided";
                if (string.IsNullOrWhiteSpace(html)) log.LogError("Could not locate html for function.  Possible the template file could not be located");
                if (string.IsNullOrWhiteSpace(requestBody)) log.LogError("Empty request body for json");
                if (string.IsNullOrWhiteSpace(requestBody)) log.LogError("Json body too short");
                return ErrorMessage(message, HttpStatusCode.BadRequest);
            }

            var replacedContents = HtmlHelper.InsertJsonIntoHtml(html,json);

            log.LogInformation("ReplaceHtmlContent processed request");

            //3. return html output
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            if (replacedContents == html)
            {
                response.StatusCode = HttpStatusCode.NotModified;
            }
            response.Content = new StringContent(replacedContents);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        private static HttpResponseMessage ErrorMessage(string message,HttpStatusCode statusCode)
        {
            var badRequestResponse = new HttpResponseMessage(statusCode);
            badRequestResponse.Content = new StringContent(message);
            badRequestResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return badRequestResponse;
        }
    }
}
