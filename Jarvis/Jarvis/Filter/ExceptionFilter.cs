using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.SecurityTokenService;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Filter
{
    public class ExceptionFilter : RequestException , IExceptionFilter 
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private Logger _logger;

        public ExceptionFilter(
            IHostingEnvironment hostingEnvironment,
            IModelMetadataProvider modelMetadataProvider)
        {
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
            _logger = Logger.GetInstance<ExceptionFilter>();
        }

        public void OnException(ExceptionContext context)
        {

            //var urlWithAccessToken = "https://hooks.slack.com/services/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";

            //var client = new SlackClient(urlWithAccessToken);

            //client.PostMessage(username: "kaushal jayswal",
            //           text: "THIS IS A TEST MESSAGE! SQUEEDLYBAMBLYFEEDLYMEEDLsaMOWWWWWWWW!",
            //           channel: "#logtestproj");

            //var exception = context.Exception;

            //if (exception == null)
            //{
            //    // Should never happen.
            //    return;
            //}
            //var badRequestException = context.Exception as BadRequestException;
            ApiError apiError = null;

            if (context.Exception is ApiException)
            {

                var ex = context.Exception as ApiException;
                context.Exception = null;
                apiError = new ApiError(ex.Message);
                apiError.errors = ex.Errors;

                context.HttpContext.Response.StatusCode = ex.StatusCode;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                apiError = new ApiError("Unauthorized Access");
                context.HttpContext.Response.StatusCode = 401;
            }else if (context.Exception is InvalidOperationException)
            {
                apiError = new ApiError("Database operation not allowed");
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
            }else if(context.Exception is NotFoundException)
            {
                apiError = new ApiError("Resource not found");
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }else if (context.Exception is BadRequestException)
            {
                apiError = new ApiError("Bad request");
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else if(context.Exception is InvalidSecurityTokenException)
            {
                apiError = new ApiError("Invalid token");
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            else
            {
                var msg = context.Exception.GetBaseException().Message;
                string stack = context.Exception.StackTrace;

                apiError = new ApiError(msg);
                apiError.detail = stack;

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            //_logger.LogInformation("--------------------------  BEGIN REQUEST -------------------------- ", context.ActionDescriptor.Id);

            //_logger.LogInformation("Method Initialize", context.ActionDescriptor.DisplayName);

            Response_Message response = new Response_Message();
            response.message = apiError.ToString();
            response.data = context.Exception;
            response.success = (int)ResponseStatusNumber.Error;
            var jasonString = Newtonsoft.Json.JsonConvert.SerializeObject(response);

            var jsonobj = new Dictionary<string, string>
            {
                { "--------------------------  BEGIN REQUEST -------------------------- : " , context.ActionDescriptor.Id },
                { "Method Initialize : " , context.ActionDescriptor.DisplayName },
                { "Response Model : " , jasonString.ToString() },
                { "--------------------------  END REQUEST -------------------------- : ",  context.ActionDescriptor.Id}
            };

            var jsonmessage = Newtonsoft.Json.JsonConvert.SerializeObject(jsonobj);
            _logger.LogError("Exception Occured" , jsonmessage.ToString());
            //_logger.LogError("Response Model", jasonString.ToString());
            //_logger.LogInformation("--------------------------  END REQUEST -------------------------- ", context.ActionDescriptor.Id);
            // always return a JSON result
            context.Result = new JsonResult(response);
        }


        public class SlackClient
        {
            private readonly Uri _uri;
            private readonly Encoding _encoding = new UTF8Encoding();

            public SlackClient(string urlWithAccessToken)
            {
                _uri = new Uri(urlWithAccessToken);
            }

            //Post a message using simple strings
            public void PostMessage(string text, string username = null, string channel = null)
            {
                Payload payload = new Payload()
                {
                    Channel = channel,
                    Username = username,
                    Text = text
                };

                PostMessage(payload);
            }

            //Post a message using a Payload object
            public void PostMessage(Payload payload)
            {
                string payloadJson = JsonConvert.SerializeObject(payload);

                using (WebClient client = new WebClient())
                {
                    NameValueCollection data = new NameValueCollection();
                    data["payload"] = payloadJson;

                    var response = client.UploadValues(_uri, "POST", data);

                    //The response text is usually "ok"
                    string responseText = _encoding.GetString(response);
                }
            }
        }

        //This class serializes into the Json payload required by Slack Incoming WebHooks
        public class Payload
        {
            [JsonProperty("channel")]
            public string Channel { get; set; }

            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }
        }

    }
}
