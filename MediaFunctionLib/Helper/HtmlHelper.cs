using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using OpenHtmlToPdf;

namespace MediaFunctionLib.Helper
{
    public static class HtmlHelper
    {
        public static string InsertJsonIntoHtml(string html,dynamic json)
        {
            //construct a dictionary we can use to replace any matching strings
            Dictionary<string, string> properties = GetPropertyKeysForDynamic(json);
            List<KeyValuePair<string, string>> replacePatterns = properties.Select(e => new KeyValuePair<string, string>(e.Key, e.Value)).ToList();


            // 2. replace each key
            var replacedContents = html;
            foreach (var item in replacePatterns)
            {
                replacedContents = replacedContents.Replace("{{" + item.Key + "}}", item.Value.ToString());
            }
            return replacedContents;
        }

        public static Dictionary<string, string> GetPropertyKeysForDynamic(dynamic dynamicToGetPropertiesFor)
        {
            JObject attributesAsJObject = dynamicToGetPropertiesFor;
            Dictionary<string, string> values = attributesAsJObject.ToObject<Dictionary<string, string>>();
            return values;
        }

        public static byte[] ConvertHtmlToPdf(string html)
        {
            return Pdf
            .From(html)
            .Content();
        }
    }
}
