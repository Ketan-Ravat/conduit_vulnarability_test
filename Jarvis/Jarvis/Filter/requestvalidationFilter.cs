using Jarvis.db.Models;
using Jarvis.Filter;
using Jarvis.Resource;
using Jarvis.Service.Services;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.Utility;
using Jarvis.ViewModels.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Filter
{
    public class requestvalidationFilter : ActionFilterAttribute
    {
        private Logger _logger;


        public requestvalidationFilter()
        {
            _logger = Logger.GetInstance<requestvalidationFilter>();
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("--------------------------  BEGIN REQUEST -------------------------- ", context.ActionDescriptor.Id);

            _logger.LogInformation("Method Initialize", context.ActionDescriptor.DisplayName);

            var header_request = JsonConvert.SerializeObject(context.HttpContext.Request.Headers);
            var requestModel = JsonConvert.SerializeObject(context.ActionArguments);

            _logger.LogInformation("Request Header", header_request);
            _logger.LogInformation("Request Model", requestModel);

        }
    }
}
