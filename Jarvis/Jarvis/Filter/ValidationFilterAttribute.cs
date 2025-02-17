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

namespace Jarvis.ViewModels.Filter
{
    public class ValidationFilterAttribute : ActionFilterAttribute
    {
        private Logger _logger;
        ValidateUser validateuser = null;

        public ValidationFilterAttribute()
        {
            _logger = Logger.GetInstance<ValidationFilterAttribute>();
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string actionName = context.RouteData.Values["Action"].ToString();

            Response_Message result = new Response_Message();
            //GenericRequestModel requestModel = new GenericRequestModel();
            base.OnActionExecuting(context);
            Guid Device_UUID = Guid.Empty;
            Guid Requested_By = Guid.Empty;
            var IP_ADDRESS = context.HttpContext.Request.Headers[RequestHeaderUtils.IP_ADDRESS_KEY].ToString();
            var REQUESTED_BY = context.HttpContext.Request.Headers[RequestHeaderUtils.REQUESTED_BY_KEY].ToString();
            var REQUEST_ID = context.ActionDescriptor.Id.ToString();
            var DEVICE_UUID = context.HttpContext.Request.Headers[RequestHeaderUtils.DEVICE_UUID_KEY].ToString();
            var DEVICE_LATITUDE = context.HttpContext.Request.Headers[RequestHeaderUtils.DEVICE_LATITUDE_KEY].ToString();
            var DEVICE_LONGITUDE = context.HttpContext.Request.Headers[RequestHeaderUtils.DEVICE_LONGITUDE_KEY].ToString();
            var MAC_ADDRESS = context.HttpContext.Request.Headers[RequestHeaderUtils.MAC_ADDRESS_KEY].ToString();
            var DEVICE_BATTERY_PERCENTAGE = context.HttpContext.Request.Headers[RequestHeaderUtils.DEVICE_BATTERY_PERCENTAGE_KEY].ToString();
            var SITE_ID = context.HttpContext.Request.Headers[RequestHeaderUtils.SITE_ID_KEY].ToString();
            var ROLE_ID = context.HttpContext.Request.Headers[RequestHeaderUtils.ROLE_ID_KEY].ToString();
            var TOKEN = context.HttpContext.Request.Headers[RequestHeaderUtils.TOKEN_KEY].ToString();
            var DOMAIN_NAME = context.HttpContext.Request.Headers[RequestHeaderUtils.DoMAIN_NAME_KEY].ToString();
            var APP_VERSION = context.HttpContext.Request.Headers[RequestHeaderUtils.APP_VERSION_KEY].ToString();
            var App_Brand = context.HttpContext.Request.Headers[RequestHeaderUtils.App_Brand].ToString();

            int app_brand = 1; // 1 for sensaii , 2 for conduit  // default will be sensaii. 
            if (!String.IsNullOrEmpty(App_Brand))
            {
                app_brand = int.Parse(App_Brand);
            }
            try
            {
                Device_UUID = Guid.Parse(DEVICE_UUID);
            }
            catch(Exception e)
            {
                //Do Nothing
            }

            try
            {
                Requested_By = Guid.Parse(REQUESTED_BY);
            }
            catch (Exception e)
            {
                //Do Nothing
            }
            GenericRequestModel.device_uuid = Device_UUID;
            GenericRequestModel.device_battery_percentage = DEVICE_BATTERY_PERCENTAGE;
            GenericRequestModel.device_latitude = DEVICE_LATITUDE;
            GenericRequestModel.device_longitude = DEVICE_LONGITUDE;
            GenericRequestModel.mac_address = MAC_ADDRESS;
            GenericRequestModel.requested_by = Requested_By;
            GenericRequestModel.request_id = REQUEST_ID;
            GenericRequestModel.site_id = SITE_ID;
            GenericRequestModel.role_id = ROLE_ID;
            GenericRequestModel.token = TOKEN;
            GenericRequestModel.domain_name = DOMAIN_NAME;
            GenericRequestModel.app_version = APP_VERSION;

            if (!string.IsNullOrEmpty(TOKEN) && !string.IsNullOrEmpty(DOMAIN_NAME))
            {
                VerifyCognitoTokenResponseModel response = VerifyCognitoToken.Verify(TOKEN, DOMAIN_NAME , app_brand);
                if (response.success > 0)
                {
                    // Do nothing;
                }
                else
                {
                    context.Result = new OkObjectResult(response);
                }
            }

            if ((Device_UUID != null && Device_UUID != Guid.Empty) && (Requested_By != null && Requested_By != Guid.Empty))
            {
                _logger.LogInformation("Device UUID: " + GenericRequestModel.device_uuid.ToString());
                _logger.LogInformation("Device Battery Percentage: " + GenericRequestModel.device_battery_percentage);
                _logger.LogInformation("Device Latitude: " + GenericRequestModel.device_latitude);
                _logger.LogInformation("Device Longitude: " + GenericRequestModel.device_longitude);
                _logger.LogInformation("Mac Address: " + GenericRequestModel.mac_address);
                _logger.LogInformation("Requested By: " + GenericRequestModel.requested_by.ToString());
                _logger.LogInformation("Requested ID: " + GenericRequestModel.request_id);
                if (validateuser == null)
                {
                    validateuser = new ValidateUser();
                }
                int response = validateuser.User(REQUESTED_BY, Device_UUID);
                if (response > 0)
                {
                    // Noting to Do
                }
                else
                {
                    if (response == (int)ResponseStatusNumber.NotValidUser)
                    {
                        result.message = "user is not valid";
                    }
                    else if(response == (int)ResponseStatusNumber.DeviceNotAssignToUserCompany)
                    {
                        result.message = ResponseMessages.DeviceNotRegisterToUserCompany;
                    }else if(response == (int)ResponseStatusNumber.UnauthorizedDevice)
                    {
                        result.message = ResponseMessages.DeviceUnaAuthorized;
                    }
                    result.success = response;
                    //(int)ResponseStatusNumber.UnauthorizedDevice;
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    //result.error = apiError;
                    //_logger.LogInformation("Device Validation Error ", "Unthorized Exception");
                    var responseModel = JsonConvert.SerializeObject(result);
                    _logger.LogError("Unauthorize Device or User ", "Response Model: " + responseModel.ToString());
                    //_logger.LogInformation("Unauthorize Device or User ", "Response Model: " + responseModel.ToString());
                    context.Result = new UnauthorizedObjectResult(result);
                }
            }
        }

        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    _logger = Logger.GetInstance<ValidationFilterAttribute>();
        //    Response_Message response = new Response_Message();

        //    _logger.LogInformation("--------------------------  BEGIN REQUEST -------------------------- ", context.ActionDescriptor.Id);

        //    _logger.LogInformation("Method Initialize", context.ActionDescriptor.DisplayName);

        //    //var requestModel = JsonConvert.SerializeObject(context.ActionArguments);

        //    //_logger.LogInformation("Request Model", requestModel);

        //    base.OnActionExecuting(context);
        //    if (!context.ModelState.IsValid)
        //    {
        //        //ApiError apiError = new ApiError(context.ModelState);
        //        //context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //        //response.message = "Bad Request";
        //        //response.data = apiError;
        //        //response.success = (int)ResponseStatusNumber.BadRequest;
        //        //_logger.LogError("Model Validation Error");
        //        //context.Result = new BadRequestObjectResult(response);
        //    }
        //}
    }
}
