using MediaFunctionLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace MediaFunction.Test
{
    public class HTTP_Trigger_ReplaceHtmlContent
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void Should_return_updated_html()
        {
            //ASSEMBLE - fake data including a string template and a valid looking web request
            // construct some values to inject into the method, important to know these values
            // ahead of time so we can later assert if they exist in the output
            var fakeHtmlTemplate = TestFactory.TestHtmlTemplate();
            var fakeJson = new
            {
                value1 = "TestInput1",
                value2 = "TestInput2",
                value3 = DateTime.Now.ToString(),
                value4 = "TestInput4"
            };
            var request = TestFactory.CreateValidHtmlContentRequest(fakeJson);


            //ACT - send our assembled test data into the method we are testing
            var response = await ReplaceHtmlContent.Run(request, logger, fakeHtmlTemplate);


            //ASSERT - check response against expected
            // here we can check if all of the strings we wanted to be injected to the output 
            // are present
            string responseBodyString = await response.Content.ReadAsStringAsync();
            bool string1Present = responseBodyString.Contains(fakeJson.value1);
            bool string2Present = responseBodyString.Contains(fakeJson.value2);
            bool string3Present = responseBodyString.Contains(fakeJson.value3);
            bool string4Present = responseBodyString.Contains(fakeJson.value4);

            Assert.True(string1Present);
            Assert.True(string2Present);
            Assert.True(string3Present);
            Assert.True(string4Present);

        }

        [Fact]
        public async void Should_return_not_modified_html_if_unchanged()
        {
            //ASSEMBLE - instantiate a valid html template and try to update a value that
            // does not exist in it. you should still receive the same html with a not-modified status
            // to help indicate a value provided cant be inserted
            var fakeHtmlTemplate = TestFactory.TestHtmlTemplate();
            var fakeJson = new
            {
                valueThatdoesntExistInTemplate = "TestInput1",
            };
            var request = TestFactory.CreateValidHtmlContentRequest(fakeJson);


            //ACT - send our assembled test data into the method we are testing
            var response = await ReplaceHtmlContent.Run(request, logger, fakeHtmlTemplate);


            //ASSERT - 
            string responseBodyString = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.NotModified,response.StatusCode);
        }

        [Fact]
        public async void Should_ignore_empty_html()
        {
            //ASSEMBLE - empty template but valid json
            var fakeHtmlTemplate = "";
            var fakeJson = new
            {
                value1 = "TestInput1",
                value2 = "TestInput2",
                value3 = DateTime.Now.ToString(),
                value4 = "TestInput4"
            };
            var request = TestFactory.CreateValidHtmlContentRequest(fakeJson);

            //ACT - send our assembled test data into the method we are testing
            var response = await ReplaceHtmlContent.Run(request, logger, fakeHtmlTemplate);

            //ASSERT - check that the response catches this specific scenario and doesnt cause any exceptions along
            // the way
            string responseBodyString = await response.Content.ReadAsStringAsync();

            Assert.Equal("No HTML provided",responseBodyString);
        }

        [Fact]
        public async void Should_handle_null_request()
        {
            //ASSEMBLE - empty template but empty json
            var fakeHtmlTemplate = TestFactory.TestHtmlTemplate();
            var fakeJson = new { };
            var request = TestFactory.CreateValidHtmlContentRequest(fakeJson);

            //ACT - send our assembled test data into the method we are testing
            var response = await ReplaceHtmlContent.Run(null, logger, fakeHtmlTemplate);

            //ASSERT
            string responseBodyString = await response.Content.ReadAsStringAsync();

            Assert.Equal("Poorly formatted request", responseBodyString);
        }

        [Fact]
        public async void Should_handle_null_logger()
        {
            //ASSEMBLE - empty template but empty json
            var fakeHtmlTemplate = TestFactory.TestHtmlTemplate();
            var fakeJson = new { };
            var request = TestFactory.CreateValidHtmlContentRequest(fakeJson);

            try
            {      
                //ACT - send our assembled test data into the method we are testing
                var response = await ReplaceHtmlContent.Run(request, null, fakeHtmlTemplate);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }


        [Fact]
        public async void Should_ignore_empty_json()
        {
            //ASSEMBLE - empty template but empty json
            var fakeHtmlTemplate = TestFactory.TestHtmlTemplate();
            var fakeJson = new{ };
            var request = TestFactory.CreateValidHtmlContentRequest(fakeJson);

            //ACT - send our assembled test data into the method we are testing
            var response = await ReplaceHtmlContent.Run(request, logger, fakeHtmlTemplate);

            //ASSERT - check that the response catches this specific scenario and doesnt cause any exceptions along
            // the way
            string responseBodyString = await response.Content.ReadAsStringAsync();

            Assert.Equal("No JSON provided", responseBodyString);
        }

        [Fact]
        public async void Should_ignore_bad_json()
        {
            //ASSEMBLE - empty template but bad json
            var fakeHtmlTemplate = TestFactory.TestHtmlTemplate();
            var badJson = "  {   {'bad':json }}}} format   }";
            var request = TestFactory.CreateCorruptHtmlContentRequest(badJson);

            //ACT - send our assembled test data into the method we are testing
            var response = await ReplaceHtmlContent.Run(request, logger, fakeHtmlTemplate);

            //ASSERT - check that the response catches this specific scenario and doesnt cause any exceptions along
            // the way
            string responseBodyString = await response.Content.ReadAsStringAsync();

            Assert.Equal("Invalid JSON provided", responseBodyString);
        }

        [Fact]
        public async void Should_ignore_keys_that_do_not_exist_in_html()
        {
            //ASSEMBLE - a normal test except we provide a json value that should not make 
            // it into the final output
            var fakeHtmlTemplate = TestFactory.TestHtmlTemplate();
            var fakeJson = new
            {
                value1 = "TestInput1",
                value2 = "TestInput2",
                value3 = DateTime.Now.ToString(),
                value999999 = "TestInput4"
            };
            var request = TestFactory.CreateValidHtmlContentRequest(fakeJson);


            //ACT - send our assembled test data into the method we are testing
            var response = await ReplaceHtmlContent.Run(request, logger, fakeHtmlTemplate);


            //ASSERT - check response against expected
            // here we can check if all of the strings we wanted to be injected to the output 
            // are present
            string responseBodyString = await response.Content.ReadAsStringAsync();
            bool string1Present = responseBodyString.Contains(fakeJson.value1);
            bool string2Present = responseBodyString.Contains(fakeJson.value2);
            bool string3Present = responseBodyString.Contains(fakeJson.value3);
            bool string4Present = responseBodyString.Contains(fakeJson.value999999);

            Assert.True(string1Present);
            Assert.True(string2Present);
            Assert.True(string3Present);
            Assert.False(string4Present);
        }

    }
}
