using Jarvis.db.Models;
using Jarvis.Filter;
using Jarvis.Resource;
using Jarvis.Service.Services;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.Utility;
using Jarvis.ViewModels.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    public class ResultServiceFilter : ResultFilterAttribute
    {
        private Logger _logger;

        public ResultServiceFilter()
        {
            _logger = Logger.GetInstance<ResultServiceFilter>();
        }
        public void OnResultExecuted(ResultExecutedContext context)
        {
            if (context.HttpContext.Response.StatusCode != (int)HttpStatusCode.BadRequest
                && context.HttpContext.Response.ContentType != "application/pdf"
                && context.HttpContext.Response.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                && context.HttpContext.Response.ContentType != "application/octet-stream")
            {
                // comment below line to log every response in file but this will create high cpu load and memory issue on server
                //_logger.LogInformation("Response Model: " + JsonConvert.SerializeObject(context.Result));
              
                
            }
            else
            {
                _logger.LogInformation("Response Error Model: " + JsonConvert.SerializeObject(context.Result));
                //  LogHelper.Log<ResultServiceFilter>(LogLevel.Error, "Response Error Model: " + JsonConvert.SerializeObject(context.Result));
            }
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (GenericRequestModel.store_app_version != null)
            {
                GenericRequestModel.store_app_version = null;
                GenericRequestModel.is_optional_update =false;
            }
        }
    }
}
