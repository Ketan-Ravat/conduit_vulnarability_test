using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Jarvis.Resource;
using Jarvis.Service.Abstract;
using Jarvis.Service.Services;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using static Jarvis.Service.Concrete.AssetService;

namespace Jarvis.Filter
{
    public class AuthenticationFilter : IAuthorizationFilter
    {
        private Logger _logger;
        ValidateUser validateuser = null;
        public IJwtHandler jwtHandler;
        public IUserService userService;
        public List<string> AccesibleRoles = new List<string>();
        public JWTExternalClientsKeyDetails _options { get; set; }

        public AuthenticationFilter(string roles, IUserService userService, IJwtHandler _jwtHandler, IOptions<JWTExternalClientsKeyDetails> options)
        {
            _logger = Logger.GetInstance<AuthenticationFilter>();
            this._options = options.Value;
            this.jwtHandler = _jwtHandler;
            this.userService = userService;
            this.AccesibleRoles = roles.Split(',').Select(p => p.Trim()).ToList();
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string actionName = context.RouteData.Values["Action"].ToString();
            bool isLoginAction = false;
            if (actionName == "Login")
            {
                isLoginAction = true;
            }
            Response_Message result = new Response_Message();
            Guid Device_UUID = Guid.Empty;
            Guid Requested_By = Guid.Empty;
            var IP_ADDRESS = context.HttpContext.Request.Headers[RequestHeaderUtils.IP_ADDRESS_KEY].ToString();
            var REQUEST_ID = context.ActionDescriptor.Id.ToString();
            var DEVICE_UUID = context.HttpContext.Request.Headers[RequestHeaderUtils.DEVICE_UUID_KEY].ToString();
            var DEVICE_LATITUDE = context.HttpContext.Request.Headers[RequestHeaderUtils.DEVICE_LATITUDE_KEY].ToString();
            var DEVICE_LONGITUDE = context.HttpContext.Request.Headers[RequestHeaderUtils.DEVICE_LONGITUDE_KEY].ToString();
            var MAC_ADDRESS = context.HttpContext.Request.Headers[RequestHeaderUtils.MAC_ADDRESS_KEY].ToString();
            var DEVICE_BATTERY_PERCENTAGE = context.HttpContext.Request.Headers[RequestHeaderUtils.DEVICE_BATTERY_PERCENTAGE_KEY].ToString();
            var TOKEN = context.HttpContext.Request.Headers[RequestHeaderUtils.TOKEN_KEY].ToString();
            var DOMAIN_NAME = context.HttpContext.Request.Headers[RequestHeaderUtils.DoMAIN_NAME_KEY].ToString();
            var DEVICE_BRAND = context.HttpContext.Request.Headers[RequestHeaderUtils.Device_Brand].ToString(); // 1 for android / 2 for Ios 

            var SITE_ID = context.HttpContext.Request.Headers[RequestHeaderUtils.SITE_ID_KEY].ToString();

            var ROLE_ID = context.HttpContext.Request.Headers[RequestHeaderUtils.ROLE_ID_KEY].ToString();

            var AUTH_TYPE = context.HttpContext.Request.Headers[RequestHeaderUtils.AUTH_TYPE].ToString();

            var PLATFORM_TYPE = context.HttpContext.Request.Headers[RequestHeaderUtils.PLATFORM_TYPE].ToString();

            var REQUESTED_BY = context.HttpContext.Request.Headers[RequestHeaderUtils.REQUESTED_BY_KEY].ToString();
            var APP_VERSION = context.HttpContext.Request.Headers[RequestHeaderUtils.APP_VERSION_KEY].ToString();

            var App_Brand = context.HttpContext.Request.Headers[RequestHeaderUtils.App_Brand].ToString();

            var domain_refer = context.HttpContext.Request.Headers[RequestHeaderUtils.Referer].ToString();
           
            var USER_SESSION_ID = context.HttpContext.Request.Headers[RequestHeaderUtils.USER_SESSION_ID].ToString();

            int app_brand = 1; // 1 for sensaii , 2 for conduit  // default will be sensaii. 
            int device_brand = 0;
            int requeted_app_version = 0;
            if (!String.IsNullOrEmpty(App_Brand))
            {
                app_brand = int.Parse(App_Brand);
            }
            if (!String.IsNullOrEmpty(DEVICE_BRAND))
            {
                device_brand = int.Parse(DEVICE_BRAND);
            }
            if (!String.IsNullOrEmpty(APP_VERSION))
            {
                try
                {
                    requeted_app_version = int.Parse(APP_VERSION.Replace(".", ""));
                }
                catch (Exception ex)
                {

                }
            }
            try
            {
                Device_UUID = Guid.Parse(DEVICE_UUID);
            }
            catch (Exception e)
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

            if (ROLE_ID == "[object Object]")
            {
                ROLE_ID = Guid.Empty.ToString();
            }

            if (SITE_ID == "[object Object]")
            {
                SITE_ID = Guid.Empty.ToString();
            }

            string role_name_ = null;
            if (!String.IsNullOrEmpty(ROLE_ID))
            {
                role_name_ = userService.GetRoleNameById(Guid.Parse(ROLE_ID));
            }
            string site_name = null;
            if (!String.IsNullOrEmpty(SITE_ID))
            {
                site_name = userService.GetSiteNameById(Guid.Parse(SITE_ID));
            }

            GenericRequestModel.device_uuid = Device_UUID;
            GenericRequestModel.device_battery_percentage = DEVICE_BATTERY_PERCENTAGE;
            GenericRequestModel.device_latitude = DEVICE_LATITUDE;
            GenericRequestModel.device_longitude = DEVICE_LONGITUDE;
            GenericRequestModel.mac_address = MAC_ADDRESS;
            GenericRequestModel.requested_by = Requested_By;
            GenericRequestModel.request_id = REQUEST_ID;
            GenericRequestModel.token = TOKEN;
            GenericRequestModel.domain_name = DOMAIN_NAME;
            GenericRequestModel.site_status = (int)Status.Active;
            GenericRequestModel.company_status = (int)Status.Active;
            GenericRequestModel.app_version = APP_VERSION;
            GenericRequestModel.domain_refer = domain_refer;
            GenericRequestModel.site_id = SITE_ID;
            GenericRequestModel.role_id = ROLE_ID;
            GenericRequestModel.role_name = role_name_;
            GenericRequestModel.site_name = site_name;

            var RequestInfo = new RequestInfo();

            RequestInfo.device_uuid = Device_UUID;
            RequestInfo.device_battery_percentage = DEVICE_BATTERY_PERCENTAGE;
            RequestInfo.device_latitude = DEVICE_LATITUDE;
            RequestInfo.device_longitude = DEVICE_LONGITUDE;
            RequestInfo.mac_address = MAC_ADDRESS;
            RequestInfo.requested_by = Requested_By;
            RequestInfo.request_id = REQUEST_ID;
            RequestInfo.token = TOKEN;
            RequestInfo.domain_name = DOMAIN_NAME;
            RequestInfo.site_status = (int)Status.Active;
            RequestInfo.company_status = (int)Status.Active;
            RequestInfo.app_version = APP_VERSION;
            RequestInfo.platform_type = PLATFORM_TYPE;
            RequestInfo.site_name = site_name;
            //RequestInfo.site_id = SITE_ID;

            if (!string.IsNullOrEmpty(AUTH_TYPE) && !string.IsNullOrEmpty(AUTH_TYPE))
            {
                if (AUTH_TYPE.ToLower() == AuthenticationConstants.QRAuthType)
                {
                    // this is the mobile device 
                    // get the userid from the token
                    if (!string.IsNullOrEmpty(TOKEN))
                    {
                        Response_Message response = new Response_Message();
                        var isValid = jwtHandler.ValidateToken(_options.RsaPublicKey, TOKEN);
                        if (isValid)
                        {
                            // get the user details from token
                            var handler = new JwtSecurityTokenHandler();
                            var jwtToken = handler.ReadToken(TOKEN) as JwtSecurityToken;

                            var user_id = jwtToken.Claims.First(claim => claim.Type == "user_id").Value;
                            if (!String.IsNullOrEmpty(user_id))
                            {

                                GenericRequestModel.requested_by = Guid.Parse(user_id);
                                Requested_By = Guid.Parse(user_id);
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
                                    int DeviceValid = validateuser.User(Requested_By.ToString(), Device_UUID);
                                    if (DeviceValid > 0)
                                    {
                                        var userdetails = userService.GetUserSessionByUserIDAsync(Requested_By.ToString());
                                        if (userdetails != null)
                                        {
                                            
                                            GenericRequestModel.site_status = userdetails.active_site_status;
                                            GenericRequestModel.company_status = userdetails.active_company_status;
                                            RequestInfo.site_status = userdetails.active_site_status;
                                            RequestInfo.company_status = userdetails.active_company_status;
                                            var isSuperAdmin = false;
                                            if (!String.IsNullOrEmpty(userdetails.active_rolename_app))
                                            {
                                                var activeRoleCheck = userdetails.Userroles.Where(x => x.role_id.ToString() == userdetails.active_rolename_app && x.status == (int)Status.Active).FirstOrDefault();
                                                if (activeRoleCheck == null && isLoginAction == false)
                                                {
                                                    // return unauthorized
                                                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                                    response.success = (int)ResponseStatusNumber.AccessRevoke;
                                                    response.message = ResponseMessages.RoleAccessRevoke;
                                                    var responseModel = JsonConvert.SerializeObject(result);
                                                    _logger.LogError("Unauthorize Access Revoked ", "Response Model: " + responseModel.ToString());
                                                    context.Result = new UnauthorizedObjectResult(response);
                                                }
                                                else
                                                {
                                                    if (activeRoleCheck?.role_name == GlobalConstants.Admin)
                                                    {
                                                        isSuperAdmin = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var defaultRoleCheck = userdetails.Userroles.Where(x => x.role_id.ToString() == userdetails.default_rolename_app && x.status == (int)Status.Active).FirstOrDefault();
                                                if (defaultRoleCheck == null)
                                                {
                                                    // return unauthorized
                                                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                                    response.success = (int)ResponseStatusNumber.AccessRevoke;
                                                    response.message = ResponseMessages.RoleAccessRevoke;
                                                    var responseModel = JsonConvert.SerializeObject(result);
                                                    _logger.LogError("Unauthorize Access Revoked ", "Response Model: " + responseModel.ToString());
                                                    context.Result = new UnauthorizedObjectResult(response);
                                                }
                                                else
                                                {
                                                    if (defaultRoleCheck.role_name == GlobalConstants.Admin)
                                                    {
                                                        isSuperAdmin = true;
                                                    }
                                                }
                                            }
                                            if (!String.IsNullOrEmpty(userdetails.active_site_id))
                                            {
                                                var activeSiteCheck = userdetails.Usersites.Where(x => x.site_id.ToString() == userdetails.active_site_id && x.status == (int)Status.Active).FirstOrDefault();
                                                if (activeSiteCheck == null && userdetails.active_site_status != (int)Status.AllSiteType && isSuperAdmin != true && isLoginAction == false)
                                                {
                                                    // return unauthorized
                                                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                                    response.success = (int)ResponseStatusNumber.AccessRevoke;
                                                    response.message = ResponseMessages.SiteAccessRevoked;
                                                    var responseModel = JsonConvert.SerializeObject(result);
                                                    _logger.LogError("Unauthorize Access Revoked ", "Response Model: " + responseModel.ToString());
                                                    context.Result = new UnauthorizedObjectResult(response);
                                                }
                                                else
                                                {
                                                    GenericRequestModel.site_status = userdetails.active_site_status;
                                                    GenericRequestModel.company_status = userdetails.active_company_status;
                                                    RequestInfo.site_status = userdetails.active_site_status;
                                                    RequestInfo.company_status = userdetails.active_company_status;
                                                }
                                            }
                                            else
                                            {
                                                var defaultSiteCheck = userdetails.Usersites.Where(x => x.site_id.ToString() == userdetails.default_site_id && x.status == (int)Status.Active).FirstOrDefault();
                                                if (defaultSiteCheck == null && isSuperAdmin != true && userdetails.default_site_status != (int)Status.AllSiteType)
                                                {
                                                    // return unauthorized
                                                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                                    response.success = (int)ResponseStatusNumber.AccessRevoke;
                                                    response.message = ResponseMessages.SiteAccessRevoked;
                                                    var responseModel = JsonConvert.SerializeObject(result);
                                                    _logger.LogError("Unauthorize Access Revoked ", "Response Model: " + responseModel.ToString());
                                                    context.Result = new UnauthorizedObjectResult(response);
                                                }
                                                else
                                                {
                                                    GenericRequestModel.site_status = userdetails.default_site_status;
                                                    GenericRequestModel.company_status = userdetails.default_company_status;
                                                    RequestInfo.site_status = userdetails.default_site_status;
                                                    RequestInfo.company_status = userdetails.default_company_status;
                                                }
                                            }
                                            var activeRole = userdetails.active_rolename_app_name;
                                            if (activeRole == null)
                                            {
                                                activeRole = userdetails.default_rolename_app_name;
                                            }
                                            if (AccesibleRoles.Contains(activeRole) || isLoginAction == true)
                                            {
                                                if (!String.IsNullOrEmpty(SITE_ID) && SITE_ID != Guid.Empty.ToString())
                                                {
                                                    GenericRequestModel.site_id = SITE_ID;
                                                    RequestInfo.site_id = SITE_ID;
                                                }
                                                else
                                                {
                                                    GenericRequestModel.site_id = userdetails.active_site_id;
                                                    RequestInfo.site_id = userdetails.active_site_id;
                                                }
                                                if (!String.IsNullOrEmpty(ROLE_ID) && ROLE_ID != Guid.Empty.ToString())
                                                {
                                                    GenericRequestModel.role_id = ROLE_ID;//!string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                                                    GenericRequestModel.role_name = role_name_;
                                                    RequestInfo.role_id = ROLE_ID;
                                                    RequestInfo.role_name = role_name_;
                                                }
                                                else
                                                {
                                                    GenericRequestModel.role_id = !string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                                                    GenericRequestModel.role_name = !string.IsNullOrEmpty(userdetails.active_rolename_app_name) ? userdetails.active_rolename_app_name : userdetails.default_rolename_app_name;
                                                    RequestInfo.role_id = !string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                                                    RequestInfo.role_name = !string.IsNullOrEmpty(userdetails.active_rolename_app_name) ? userdetails.active_rolename_app_name : userdetails.default_rolename_app_name;
                                                }

                                                GenericRequestModel.company_id = userdetails.active_company_id;
                                               
                                                                                           //RequestInfo.site_id = userdetails.active_site_id;
                                                RequestInfo.company_id = userdetails.active_company_id;
                                                //RequestInfo.role_id = !string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                                                //RequestInfo.role_name = !string.IsNullOrEmpty(userdetails.active_rolename_app_name) ? userdetails.active_rolename_app_name : userdetails.default_rolename_app_name;

                                                GenericRequestModel.company_id = userdetails.active_company_id;

                                                //RequestInfo.site_id = userdetails.active_site_id;
                                                RequestInfo.company_id = userdetails.active_company_id;
                                                //RequestInfo.role_id = !string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                                                //RequestInfo.role_name = !string.IsNullOrEmpty(userdetails.active_rolename_app_name) ? userdetails.active_rolename_app_name : userdetails.default_rolename_app_name;

                                            }
                                            else
                                            {
                                                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                                result.message = "Access Forbidden";
                                                result.success = (int)ResponseStatusNumber.Forbidden;
                                                var responseModel = JsonConvert.SerializeObject(result);
                                                _logger.LogError("Access Forbidden ", "Response Model: " + responseModel.ToString());
                                                context.Result = new ObjectResult(responseModel) { StatusCode = (int)HttpStatusCode.Forbidden };
                                            }

                                        }
                                        else
                                        {
                                            response.success = (int)ResponseStatusNumber.NotFound;
                                            response.message = ResponseMessages.UserNotFound;
                                        }
                                    }
                                    else
                                    {
                                        if (DeviceValid == (int)ResponseStatusNumber.NotValidUser)
                                        {
                                            result.message = "user is not valid";
                                        }
                                        else if (DeviceValid == (int)ResponseStatusNumber.DeviceNotAssignToUserCompany)
                                        {
                                            result.message = ResponseMessages.DeviceNotRegisterToUserCompany;
                                        }
                                        else if (DeviceValid == (int)ResponseStatusNumber.UnauthorizedDevice)
                                        {
                                            result.message = ResponseMessages.DeviceUnaAuthorized;
                                        }
                                        result.success = DeviceValid;
                                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                        var responseModel = JsonConvert.SerializeObject(result);
                                        _logger.LogError("Unauthorize Device or User ", "Response Model: " + responseModel.ToString());
                                        //_logger.LogInformation("Unauthorize Device or User ", "Response Model: " + responseModel.ToString());
                                        context.Result = new UnauthorizedObjectResult(result);
                                    }
                                }
                                else
                                {
                                    result.message = "DeviceID is required";
                                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                    var responseModel = JsonConvert.SerializeObject(result);
                                    _logger.LogError("Unauthorize Device or User ", "Response Model: " + responseModel.ToString());
                                    context.Result = new BadRequestObjectResult(result);
                                }
                            }
                            else
                            {
                                response.success = (int)ResponseStatusNumber.NotFound;
                                response.message = ResponseMessages.UserNotFound;
                            }
                        }
                        else
                        {
                            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            response.success = (int)ResponseStatusNumber.Unauthorized;
                            response.message = ResponseMessages.InvalidToken;
                            var responseModel = JsonConvert.SerializeObject(result);
                            _logger.LogError("Unauthorize Invalid JWT Token ", "Response Model: " + responseModel.ToString());
                            context.Result = new UnauthorizedObjectResult(response);
                        }
                    }
                    else
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        result.message = "Token not provided";
                        result.success = (int)ResponseStatusNumber.InvalidToken;
                        var responseModel = JsonConvert.SerializeObject(result);
                        _logger.LogError("Unauthorize Invalid JWT Token ", "Response Model: " + responseModel.ToString());
                        context.Result = new UnauthorizedObjectResult(result);
                    }
                }
                else if (AUTH_TYPE.ToLower() == AuthenticationConstants.CredentialsAuthType)
                {
                    // this contains mobile and web both platform
                    // check cognito token

                    /// check if request is from mobile and app version 
                    bool is_force_update = false;
                    int store_app_version = 0;
                    string db_store_app_version = null;
                    if (!String.IsNullOrEmpty(DEVICE_BRAND))
                    {
                        var get_app_version = userService.MobileAppVersion(device_brand);

                        store_app_version = int.Parse(get_app_version.store_app_version.Replace(".", ""));
                        db_store_app_version = get_app_version.store_app_version;
                        if (get_app_version != null)
                        {

                            int force_update_version = int.Parse(get_app_version.force_to_update_app_version.Replace(".", ""));
                            if (force_update_version > requeted_app_version) /// force to update an app
                            {
                                is_force_update = true;
                                //GenericRequestModel.store_app_version = store_app_version.ToString();
                            }
                            else if (store_app_version > requeted_app_version) /// optional update an app
                            {
                                GenericRequestModel.store_app_version = db_store_app_version;
                                GenericRequestModel.is_optional_update = true;
                                RequestInfo.store_app_version = db_store_app_version;
                                RequestInfo.is_optional_update = true;
                            }
                        }

                    }
                    if (is_force_update)
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        result.success = (int)ResponseStatusNumber.app_force_update;
                        result.message = "Force update is required New Version is " + db_store_app_version;
                        result.store_app_version = db_store_app_version.ToString();
                        var responseModel = JsonConvert.SerializeObject(result);
                        _logger.LogError("force Update is required ", "Response Model: " + responseModel.ToString());
                        //  context.Result = new UnauthorizedObjectResult(result);
                        context.Result = new ObjectResult(result) { StatusCode = (int)HttpStatusCode.NotAcceptable };
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(TOKEN) && !string.IsNullOrEmpty(DOMAIN_NAME))
                        {
                            VerifyCognitoTokenResponseModel response = VerifyCognitoToken.Verify(TOKEN, DOMAIN_NAME, app_brand);
                            //Response_Message response = VerifyCognitoToken.Verify(TOKEN, DOMAIN_NAME);
                            if (response.success > 0 && response.data != null)
                            {
                                // check device_uuid validation
                                Requested_By = Guid.Parse(response.data.uuid);
                                GenericRequestModel.requested_by = Guid.Parse(response.data.uuid);
                                RequestInfo.requested_by = Guid.Parse(response.data.uuid);
                                // set the generic req data here
                                if (!string.IsNullOrEmpty(PLATFORM_TYPE))
                                {
                                    if (PLATFORM_TYPE.ToLower() == AuthenticationConstants.MobilePlatform)
                                    {
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
                                            int DeviceValid = validateuser.User(Requested_By.ToString(), Device_UUID);
                                            if (DeviceValid > 0)
                                            {
                                                var userdetails = userService.GetUserSessionByUserIDAsyncOptimized(Requested_By.ToString());
                                                if (userdetails != null)
                                                {
                                                    GenericRequestModel.site_status = userdetails.active_site_status;
                                                    GenericRequestModel.company_status = userdetails.active_company_status;
                                                    RequestInfo.site_status = userdetails.active_site_status;
                                                    RequestInfo.company_status = userdetails.active_company_status;
                                                    bool isSuperAdmin = false;
                                                    if (!String.IsNullOrEmpty(userdetails.active_rolename_app))
                                                    {
                                                        var activeRoleCheck = userService.activeRoleCheck(userdetails.uuid, userdetails.active_rolename_web);// userdetails.Userroles.Where(x => x.role_id.ToString() == userdetails.active_rolename_web && x.status == (int)Status.Active).FirstOrDefault();
                                                        if (activeRoleCheck == null && isLoginAction == false)
                                                        {
                                                            // return unauthorized
                                                            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                                            response.success = (int)ResponseStatusNumber.AccessRevoke;
                                                            response.message = ResponseMessages.RoleAccessRevoke;
                                                            var responseModel = JsonConvert.SerializeObject(result);
                                                            _logger.LogError("Unauthorize Access Revoked ", "Response Model: " + responseModel.ToString());
                                                            context.Result = new UnauthorizedObjectResult(response);
                                                        }
                                                        else
                                                        {
                                                            if (activeRoleCheck?.role_id == Guid.Parse("ff52a40a-b130-4388-bb1c-4237f8dae72e"))
                                                            {
                                                                isSuperAdmin = true;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var defaultRoleCheck = userService.activeRoleCheck(userdetails.uuid, userdetails.default_rolename_web); //userdetails.Userroles.Where(x => x.role_id.ToString() == userdetails.default_rolename_web && x.status == (int)Status.Active).FirstOrDefault();
                                                        if (defaultRoleCheck == null)
                                                        {
                                                            // return unauthorized
                                                            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                                            response.success = (int)ResponseStatusNumber.AccessRevoke;
                                                            response.message = ResponseMessages.RoleAccessRevoke;
                                                            var responseModel = JsonConvert.SerializeObject(result);
                                                            _logger.LogError("Unauthorize Access Revoked ", "Response Model: " + responseModel.ToString());
                                                            context.Result = new UnauthorizedObjectResult(response);
                                                        }
                                                        else
                                                        {
                                                            if (defaultRoleCheck?.role_id == Guid.Parse("ff52a40a-b130-4388-bb1c-4237f8dae72e"))
                                                            {
                                                                isSuperAdmin = true;
                                                            }
                                                        }
                                                    }
                                                    if (!String.IsNullOrEmpty(userdetails.active_site_id))
                                                    {
                                                        var activeSiteCheck = userService.activeSiteCheck(userdetails.uuid, userdetails.active_site_id);// userdetails.Usersites.Where(x => x.site_id.ToString() == userdetails.active_site_id && x.status == (int)Status.Active).FirstOrDefault();
                                                        if (activeSiteCheck == null && userdetails.active_site_status != (int)Status.AllSiteType && isSuperAdmin != true && isLoginAction == false)
                                                        {
                                                            // return unauthorized
                                                            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                                            response.success = (int)ResponseStatusNumber.AccessRevoke;
                                                            response.message = ResponseMessages.SiteAccessRevoked;
                                                            var responseModel = JsonConvert.SerializeObject(result);
                                                            _logger.LogError("Unauthorize Access Revoked ", "Response Model: " + responseModel.ToString());
                                                            context.Result = new UnauthorizedObjectResult(response);
                                                        }
                                                        else
                                                        {
                                                            GenericRequestModel.site_status = userdetails.active_site_status;
                                                            GenericRequestModel.company_status = userdetails.active_company_status;
                                                            RequestInfo.site_status = userdetails.active_site_status;
                                                            RequestInfo.company_status = userdetails.active_company_status;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var defaultSiteCheck = userService.activeSiteCheck(userdetails.uuid, userdetails.default_site_id); //userdetails.Usersites.Where(x => x.site_id.ToString() == userdetails.default_site_id && x.status == (int)Status.Active).FirstOrDefault();
                                                        if (defaultSiteCheck == null && userdetails.default_site_status != (int)Status.AllSiteType && isSuperAdmin != true)
                                                        {
                                                            // return unauthorized
                                                            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                                            response.success = (int)ResponseStatusNumber.AccessRevoke;
                                                            response.message = ResponseMessages.SiteAccessRevoked;
                                                            var responseModel = JsonConvert.SerializeObject(result);
                                                            _logger.LogError("Unauthorize Access Revoked ", "Response Model: " + responseModel.ToString());
                                                            context.Result = new UnauthorizedObjectResult(response);
                                                        }
                                                        else
                                                        {
                                                            GenericRequestModel.site_status = userdetails.default_site_status;
                                                            GenericRequestModel.company_status = userdetails.default_company_status;
                                                            RequestInfo.site_status = userdetails.default_site_status;
                                                            RequestInfo.company_status = userdetails.default_company_status;
                                                        }
                                                    }
                                                    var activeRole = !String.IsNullOrEmpty(role_name_) ? role_name_ : userdetails.active_rolename_app_name;
                                                    if (activeRole == null)
                                                    {
                                                        activeRole = userdetails.default_rolename_app_name;
                                                    }
                                                    if (AccesibleRoles.Contains(activeRole) || isLoginAction == true)
                                                    {

                                                        if (!String.IsNullOrEmpty(SITE_ID) && SITE_ID != Guid.Empty.ToString())
                                                        {
                                                            GenericRequestModel.site_id = SITE_ID;
                                                            RequestInfo.site_id = SITE_ID;
                                                        }
                                                        else
                                                        {
                                                            GenericRequestModel.site_id = userdetails.active_site_id;
                                                            RequestInfo.site_id = userdetails.active_site_id;
                                                        }
                                                        if (!String.IsNullOrEmpty(ROLE_ID) && ROLE_ID != Guid.Empty.ToString())
                                                        {
                                                            GenericRequestModel.role_id = ROLE_ID;//!string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                                                            GenericRequestModel.role_name = role_name_;
                                                            RequestInfo.role_id = ROLE_ID;
                                                            RequestInfo.role_name = role_name_;
                                                        }
                                                        else
                                                        {
                                                            GenericRequestModel.role_id = !string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                                                            GenericRequestModel.role_name = !string.IsNullOrEmpty(userdetails.active_rolename_app_name) ? userdetails.active_rolename_app_name : userdetails.default_rolename_app_name;
                                                            RequestInfo.role_id = !string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                                                            RequestInfo.role_name = !string.IsNullOrEmpty(userdetails.active_rolename_app_name) ? userdetails.active_rolename_app_name : userdetails.default_rolename_app_name;
                                                        }


                                                        //GenericRequestModel.site_id = SITE_ID;//userdetails.active_site_id;
                                                        GenericRequestModel.company_id = userdetails.active_company_id;
                                                        //GenericRequestModel.role_id = ROLE_ID;//!string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                                                        //GenericRequestModel.role_name = role_name_;// !string.IsNullOrEmpty(userdetails.active_rolename_app_name) ? userdetails.active_rolename_app_name : userdetails.default_rolename_app_name;
                                                                                                   //RequestInfo.site_id = userdetails.active_site_id;
                                                        RequestInfo.company_id = userdetails.active_company_id;
                                                        
                                                    }
                                                    else
                                                    {
                                                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                                        result.message = "Access Forbidden";
                                                        result.success = (int)ResponseStatusNumber.Forbidden;
                                                        var responseModel = JsonConvert.SerializeObject(result);
                                                        _logger.LogError("Access Forbidden ", "Response Model: " + responseModel.ToString());
                                                        context.Result = new ObjectResult(responseModel) { StatusCode = (int)HttpStatusCode.Forbidden };
                                                    }
                                                }
                                                else
                                                {
                                                    response.success = (int)ResponseStatusNumber.NotFound;
                                                    response.message = ResponseMessages.UserNotFound;
                                                }
                                            }
                                            else
                                            {
                                                if (DeviceValid == (int)ResponseStatusNumber.NotValidUser)
                                                {
                                                    result.message = "user is not valid";
                                                }
                                                else if (DeviceValid == (int)ResponseStatusNumber.DeviceNotAssignToUserCompany)
                                                {
                                                    result.message = ResponseMessages.DeviceNotRegisterToUserCompany;
                                                }
                                                else if (DeviceValid == (int)ResponseStatusNumber.UnauthorizedDevice)
                                                {
                                                    result.message = ResponseMessages.DeviceUnaAuthorized;
                                                }
                                                result.success = DeviceValid;
                                                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                                var responseModel = JsonConvert.SerializeObject(result);
                                                _logger.LogError("Unauthorize Device or User ", "Response Model: " + responseModel.ToString());
                                                context.Result = new UnauthorizedObjectResult(result);
                                            }
                                        }
                                        else
                                        {
                                            result.message = "DeviceID is required";
                                            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                            var responseModel = JsonConvert.SerializeObject(result);
                                            _logger.LogError("Unauthorize Device or User ", "Response Model: " + responseModel.ToString());
                                            context.Result = new BadRequestObjectResult(result);
                                        }
                                    }
                                    else
                                    {
                                        var userdetails = userService.GetUserSessionByUserIDAsyncOptimized(Requested_By.ToString());
                                        if (userdetails != null)
                                        {
                                            GenericRequestModel.site_status = userdetails.active_site_status;
                                            GenericRequestModel.company_status = userdetails.active_company_status;
                                            RequestInfo.site_status = userdetails.active_site_status;
                                            RequestInfo.company_status = userdetails.active_company_status;
                                            bool isSuperAdmin = false;
                                            if (!String.IsNullOrEmpty(userdetails.active_rolename_web))
                                            {
                                                // get active_role by user id and active_rolename_web

                                                var activeRoleCheck = userService.activeRoleCheck(userdetails.uuid, userdetails.active_rolename_web);// userdetails.Userroles.Where(x => x.role_id.ToString() == userdetails.active_rolename_web && x.status == (int)Status.Active).FirstOrDefault();
                                                if (activeRoleCheck == null && isLoginAction == false)
                                                {
                                                    // return unauthorized
                                                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                                    response.success = (int)ResponseStatusNumber.AccessRevoke;
                                                    response.message = ResponseMessages.RoleAccessRevoke;
                                                    var responseModel = JsonConvert.SerializeObject(result);
                                                    _logger.LogError("Unauthorize Access Revoked ", "Response Model: " + responseModel.ToString());
                                                    context.Result = new UnauthorizedObjectResult(response);
                                                }
                                                else
                                                {
                                                    if (activeRoleCheck?.role_id == Guid.Parse("ff52a40a-b130-4388-bb1c-4237f8dae72e"))
                                                    {
                                                        isSuperAdmin = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var defaultRoleCheck = userService.activeRoleCheck(userdetails.uuid, userdetails.default_rolename_web); //userdetails.Userroles.Where(x => x.role_id.ToString() == userdetails.default_rolename_web && x.status == (int)Status.Active).FirstOrDefault();
                                                if (defaultRoleCheck == null)
                                                {
                                                    // return unauthorized
                                                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                                    response.success = (int)ResponseStatusNumber.AccessRevoke;
                                                    response.message = ResponseMessages.RoleAccessRevoke;
                                                    var responseModel = JsonConvert.SerializeObject(result);
                                                    _logger.LogError("Unauthorize Access Revoked ", "Response Model: " + responseModel.ToString());
                                                    context.Result = new UnauthorizedObjectResult(response);
                                                }
                                                else
                                                {
                                                    if (defaultRoleCheck?.role_id == Guid.Parse("ff52a40a-b130-4388-bb1c-4237f8dae72e"))
                                                    {
                                                        isSuperAdmin = true;
                                                    }
                                                }
                                            }
                                            if (!String.IsNullOrEmpty(userdetails.active_site_id))
                                            {
                                                var activeSiteCheck = userService.activeSiteCheck(userdetails.uuid, userdetails.active_site_id);// userdetails.Usersites.Where(x => x.site_id.ToString() == userdetails.active_site_id && x.status == (int)Status.Active).FirstOrDefault();
                                                if (activeSiteCheck == null && userdetails.active_site_status != (int)Status.AllSiteType && isSuperAdmin != true && isLoginAction == false)
                                                {
                                                    // return unauthorized
                                                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                                    response.success = (int)ResponseStatusNumber.AccessRevoke;
                                                    response.message = ResponseMessages.SiteAccessRevoked;
                                                    var responseModel = JsonConvert.SerializeObject(result);
                                                    _logger.LogError("Unauthorize Access Revoked ", "Response Model: " + responseModel.ToString());
                                                    context.Result = new UnauthorizedObjectResult(response);
                                                }
                                                else
                                                {
                                                    GenericRequestModel.site_status = userdetails.active_site_status;
                                                    GenericRequestModel.company_status = userdetails.active_company_status;
                                                    RequestInfo.site_status = userdetails.active_site_status;
                                                    RequestInfo.company_status = userdetails.active_company_status;
                                                }
                                            }
                                            else
                                            {
                                                var defaultSiteCheck = userService.activeSiteCheck(userdetails.uuid, userdetails.default_site_id); //userdetails.Usersites.Where(x => x.site_id.ToString() == userdetails.default_site_id && x.status == (int)Status.Active).FirstOrDefault();
                                                if (defaultSiteCheck == null && userdetails.default_site_status != (int)Status.AllSiteType && isSuperAdmin != true)
                                                {
                                                    // return unauthorized
                                                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                                    response.success = (int)ResponseStatusNumber.AccessRevoke;
                                                    response.message = ResponseMessages.SiteAccessRevoked;
                                                    var responseModel = JsonConvert.SerializeObject(result);
                                                    _logger.LogError("Unauthorize Access Revoked ", "Response Model: " + responseModel.ToString());
                                                    context.Result = new UnauthorizedObjectResult(response);
                                                }
                                                else
                                                {
                                                    GenericRequestModel.site_status = userdetails.default_site_status;
                                                    GenericRequestModel.company_status = userdetails.default_company_status;
                                                    RequestInfo.site_status = userdetails.default_site_status;
                                                    RequestInfo.company_status = userdetails.default_company_status;
                                                }
                                            }
                                            var activeRole = !String.IsNullOrEmpty(role_name_) ? role_name_ : userdetails.active_rolename_web_name;
                                            if (activeRole == null)
                                            {
                                                activeRole = userdetails.default_rolename_web_name;
                                            }
                                            if (DEVICE_UUID != null && DEVICE_UUID != Guid.Empty.ToString() && DEVICE_UUID != "")// if request header have device uuid then consider it as tech by default
                                            {
                                                activeRole = "Technician";
                                            }
                                            if (AccesibleRoles.Contains(activeRole) || isLoginAction == true)
                                            {

                                                if (!String.IsNullOrEmpty(SITE_ID) && SITE_ID != Guid.Empty.ToString())
                                                {
                                                    GenericRequestModel.site_id = SITE_ID;
                                                    RequestInfo.site_id = SITE_ID;
                                                }
                                                else
                                                {
                                                    GenericRequestModel.site_id = userdetails.active_site_id;
                                                    RequestInfo.site_id = userdetails.active_site_id;
                                                }
                                                if (!String.IsNullOrEmpty(ROLE_ID) && ROLE_ID != Guid.Empty.ToString())
                                                {
                                                    GenericRequestModel.role_id = ROLE_ID;//!string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                                                    GenericRequestModel.role_name = role_name_;
                                                    RequestInfo.role_id = ROLE_ID;
                                                    RequestInfo.role_name = role_name_;
                                                }
                                                else
                                                {
                                                    GenericRequestModel.role_id = !string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                                                    GenericRequestModel.role_name = !string.IsNullOrEmpty(userdetails.active_rolename_app_name) ? userdetails.active_rolename_app_name : userdetails.default_rolename_app_name;
                                                    RequestInfo.role_id = !string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                                                    RequestInfo.role_name = !string.IsNullOrEmpty(userdetails.active_rolename_app_name) ? userdetails.active_rolename_app_name : userdetails.default_rolename_app_name;
                                                }


                                                //GenericRequestModel.site_id = SITE_ID;//userdetails.active_site_id;
                                                GenericRequestModel.company_id = userdetails.active_company_id;
                                                //GenericRequestModel.role_id = ROLE_ID;//!string.IsNullOrEmpty(userdetails.active_rolename_web) ? userdetails.active_rolename_web : userdetails.default_rolename_web;
                                                //GenericRequestModel.role_name = role_name_;// !string.IsNullOrEmpty(userdetails.active_rolename_web_name) ? userdetails.active_rolename_web_name : userdetails.default_rolename_web_name;
                                                                                           //RequestInfo.site_id = userdetails.active_site_id;
                                                RequestInfo.company_id = userdetails.active_company_id;
                                                //RequestInfo.role_id = !string.IsNullOrEmpty(userdetails.active_rolename_web) ? userdetails.active_rolename_web : userdetails.default_rolename_web;
                                                //RequestInfo.role_name = !string.IsNullOrEmpty(userdetails.active_rolename_web_name) ? userdetails.active_rolename_web_name : userdetails.default_rolename_web_name;
                                            }
                                            else
                                            {
                                                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                                                result.message = "Access Forbidden";
                                                result.success = (int)ResponseStatusNumber.Forbidden;
                                                var responseModel = JsonConvert.SerializeObject(result);
                                                _logger.LogError("Access Forbidden ", "Response Model: " + responseModel.ToString());
                                                context.Result = new ObjectResult(responseModel) { StatusCode = (int)HttpStatusCode.Forbidden };
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                var responseModel = JsonConvert.SerializeObject(response);
                                _logger.LogError("Unauthorize Cognito Token ", "Response Model: " + responseModel.ToString());
                                context.Result = new UnauthorizedObjectResult(response);
                            }
                        }
                        else
                        {
                            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            result.message = "Token not provided";
                            result.success = (int)ResponseStatusNumber.InvalidToken;
                            var responseModel = JsonConvert.SerializeObject(result);
                            _logger.LogError("Unauthorize Invalid JWT Token ", "Response Model: " + responseModel.ToString());
                            context.Result = new UnauthorizedObjectResult(result);
                        }
                    }
                }
            }
            else
            {
                // if auth type or platform id is not there then what to do here?

                //if request is from OP or MS then don't check JWT token
                if (!isLoginAction && (Device_UUID != null && Device_UUID != Guid.Empty) && (Requested_By != null && Requested_By != Guid.Empty))
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
                    int DeviceValid = validateuser.User(Requested_By.ToString(), Device_UUID);
                    if (DeviceValid > 0)
                    {
                        // Noting to Do
                        var userdetails = userService.GetUserSessionByUserIDAsync(Requested_By.ToString());
                        if (userdetails != null)
                        {
                            if (!String.IsNullOrEmpty(SITE_ID) && SITE_ID != Guid.Empty.ToString())
                            {
                                GenericRequestModel.site_id = SITE_ID;
                                RequestInfo.site_id = SITE_ID;
                            }
                            else
                            {
                                GenericRequestModel.site_id = userdetails.active_site_id;
                                RequestInfo.site_id = userdetails.active_site_id;
                            }
                            if (!String.IsNullOrEmpty(ROLE_ID) && ROLE_ID != Guid.Empty.ToString())
                            {
                                GenericRequestModel.role_id = ROLE_ID;//!string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                                GenericRequestModel.role_name = role_name_;
                                RequestInfo.role_id = ROLE_ID;
                                RequestInfo.role_name = role_name_;
                            }
                            else
                            {
                                GenericRequestModel.role_id = !string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                                GenericRequestModel.role_name = !string.IsNullOrEmpty(userdetails.active_rolename_app_name) ? userdetails.active_rolename_app_name : userdetails.default_rolename_app_name;
                                RequestInfo.role_id = !string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                                RequestInfo.role_name = !string.IsNullOrEmpty(userdetails.active_rolename_app_name) ? userdetails.active_rolename_app_name : userdetails.default_rolename_app_name;
                            }

                            //GenericRequestModel.site_id = SITE_ID;//userdetails.active_site_id;
                            GenericRequestModel.company_id = userdetails.active_company_id;
                            //GenericRequestModel.role_id = ROLE_ID;//!string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                            //GenericRequestModel.role_name = role_name_;//!string.IsNullOrEmpty(userdetails.active_rolename_app_name) ? userdetails.active_rolename_app_name : userdetails.default_rolename_app_name;
                        }
                    }
                    else
                    {
                        if (DeviceValid == (int)ResponseStatusNumber.NotValidUser)
                        {
                            result.message = "user is not valid";
                        }
                        else if (DeviceValid == (int)ResponseStatusNumber.DeviceNotAssignToUserCompany)
                        {
                            result.message = ResponseMessages.DeviceNotRegisterToUserCompany;
                        }
                        else if (DeviceValid == (int)ResponseStatusNumber.UnauthorizedDevice)
                        {
                            result.message = ResponseMessages.DeviceUnaAuthorized;
                        }
                        result.success = DeviceValid;
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
                else
                {
                    if (!isLoginAction)
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        var responseModel = JsonConvert.SerializeObject(result);
                        _logger.LogError("User has not provided required header information.");
                        context.Result = new BadRequestObjectResult(result);
                    }
                }
            }


            var userdetails_4 = userService.GetUserSessionByUserIDAsyncOptimized(Requested_By.ToString());

            if (!String.IsNullOrEmpty(SITE_ID) && SITE_ID != Guid.Empty.ToString())
            {
                GenericRequestModel.site_id = SITE_ID;
                RequestInfo.site_id = SITE_ID;
            }
            else
            {
                GenericRequestModel.site_id = userdetails_4.active_site_id;
                RequestInfo.site_id = userdetails_4.active_site_id;
            }
            // change company_id 
            //if (!String.IsNullOrEmpty(RequestInfo.site_id))
            //{
                var get_comapny_id = userService.GetSiteCompanyId(RequestInfo.site_id);
                GenericRequestModel.company_id = get_comapny_id.ToString();
                RequestInfo.company_id = get_comapny_id.ToString();
            //}
            
            if (!String.IsNullOrEmpty(ROLE_ID) && ROLE_ID != Guid.Empty.ToString())
            {
                GenericRequestModel.role_id = ROLE_ID;//!string.IsNullOrEmpty(userdetails.active_rolename_app) ? userdetails.active_rolename_app : userdetails.default_rolename_app;
                GenericRequestModel.role_name = role_name_;
                RequestInfo.role_id = ROLE_ID;
                RequestInfo.role_name = role_name_;
            }
            else
            {
                if(DEVICE_UUID == null || DEVICE_UUID == Guid.Empty.ToString() || DEVICE_UUID == "")
                {
                    GenericRequestModel.role_id = !string.IsNullOrEmpty(userdetails_4.active_rolename_app) ? userdetails_4.active_rolename_app : userdetails_4.default_rolename_app;
                    GenericRequestModel.role_name = !string.IsNullOrEmpty(userdetails_4.active_rolename_app_name) ? userdetails_4.active_rolename_app_name : userdetails_4.default_rolename_app_name;
                    RequestInfo.role_id = !string.IsNullOrEmpty(userdetails_4.active_rolename_app) ? userdetails_4.active_rolename_app : userdetails_4.default_rolename_app;
                    RequestInfo.role_name = !string.IsNullOrEmpty(userdetails_4.active_rolename_app_name) ? userdetails_4.active_rolename_app_name : userdetails_4.default_rolename_app_name;
                }
                else // if requst is from mobile then set active role as tech
                {
                    GenericRequestModel.role_id = "b22217c2-a932-498c-8ab2-11fc2628104b";
                    GenericRequestModel.role_name = "Technician";
                    RequestInfo.role_id = "b22217c2-a932-498c-8ab2-11fc2628104b";
                    RequestInfo.role_name = "Technician";
                }
            }
            //RequestInfo.role_name = role_name_;
            //RequestInfo.role_id = ROLE_ID;
            //RequestInfo.site_id = SITE_ID;
            UpdatedGenericRequestmodel.CurrentUser = RequestInfo;

            var _IsUserSessionIsRequiredOrNot = userService.IsUserSessionIsRequiredOrNot(get_comapny_id);

            if (!isLoginAction && _IsUserSessionIsRequiredOrNot)// && PLATFORM_TYPE.ToLower() != AuthenticationConstants.MobilePlatform)
            {
                if (!string.IsNullOrEmpty(USER_SESSION_ID) && USER_SESSION_ID != "[object Object]" && USER_SESSION_ID != Guid.Empty.ToString())
                {
                    var isValid_session_id = userService.CheckUserSessionIsValidOrNot(Requested_By, Guid.Parse(USER_SESSION_ID), Guid.Parse(ROLE_ID));
                    if (!isValid_session_id) // if user_session_id is not valid then return error
                    {
                        context.HttpContext.Response.StatusCode = (int)ResponseStatusNumber.InValidSessionId;
                        result.success = (int)ResponseStatusNumber.InValidSessionId;
                        result.message = "Invalid UserSessionId!";
                        var responseModel = JsonConvert.SerializeObject(result);
                        _logger.LogError("Invalid UserSessionId!", "Response Model: " + responseModel.ToString());
                        context.Result = new UnauthorizedObjectResult(result);
                    }
                }
                /*else
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    var responseModel = JsonConvert.SerializeObject(result);
                    _logger.LogError("Invalid UserSessionId!", "Response Model: " + responseModel.ToString());
                    context.Result = new UnauthorizedObjectResult(result);
                }*/
            }
        }
    }
}
