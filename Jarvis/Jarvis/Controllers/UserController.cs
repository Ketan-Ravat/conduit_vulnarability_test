using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Jarvis.db.Models;
using Jarvis.Service.Abstract;
using Jarvis.Service.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security;
using Jarvis.ViewModels;
using Jarvis.Resource;
using Jarvis.Shared;
using Jarvis.Shared.StatusEnums;
using Microsoft.Extensions.Options;
using Jarvis.Shared.Helper;
using Jarvis.Shared.Utility;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Jarvis.ViewModels.ViewModels;
using Newtonsoft.Json;
using Jarvis.ViewModels.Filter;
using Jarvis.Filter;
using System.ComponentModel.DataAnnotations;
using Jarvis.ViewModels.RequestResponseViewModel;
using static Jarvis.Service.Concrete.UserService;
using DocumentFormat.OpenXml.Spreadsheet;
using ZXing;

namespace Jarvis.Controllers {
    [TypeFilter(typeof(requestvalidationFilter))]
    //[TypeFilter(typeof(ValidationFilterAttribute))]
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiController]
    public class UserController : Controller {
        private readonly IUserService userService;

        private readonly IBaseService baseService;
        private readonly IS3BucketService s3BucketService;
        public AWSConfigurations _options { get; set; }

        public UserController(IUserService userService, IBaseService baseService, IOptions<AWSConfigurations> options)
        {
            this.userService = userService;
            this.baseService = baseService;
            this._options = options.Value;
            this.s3BucketService = new S3BucketService();
        }

        //// GET: api/student
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/student/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        /// <summary>
        /// Add Or Update User
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        public async Task<IActionResult> AddUpdateUser([FromBody] UserRequestModel user)
        {
            Response_Message response_Message = new Response_Message();
            //try
            //{
            if (ModelState.IsValid)
            {
                var response = await userService.InsertUpdateUserDetails(user, _options.cognito_resend_aws_access_key, _options.cognito_resend_aws_secret_key);
                if (response.response_status > 0)
                {
                    response_Message.success = (int)ResponseStatusNumber.Success;
                    response_Message.data = response;
                }
                else if (response.response_status == (int)ResponseStatusNumber.NotFound)
                {
                    response_Message.message = ResponseMessages.UserNotFound;
                    response_Message.success = (int)ResponseStatusNumber.NotFound;
                }
                else if (response.response_status == (int)ResponseStatusNumber.AlreadyExists)
                {
                    response_Message.message = ResponseMessages.UserAlreadyExists;
                    response_Message.success = (int)ResponseStatusNumber.AlreadyExists;
                }
                else if (!string.IsNullOrEmpty(response.response_message))
                {
                    response_Message.message = response.response_message;
                    response_Message.success = (int)ResponseStatusNumber.Error;
                }
                else
                {
                    //response_Message.data = response;
                    response_Message.message = ResponseMessages.Error;
                    response_Message.success = (int)ResponseStatusNumber.Error;
                }

                //response.success = true;
                //byte[] bytes = Encoding.Unicode.GetBytes(user.password);
                //byte[] inArray = HashAlgorithm.Create("SHA1").ComputeHash(bytes);
                //string hashpassword = Convert.ToBase64String(inArray);

                //if (context.User.Any(x => x.email == user.email))
                //{
                //    User users = context.User.First(x => x.email == user.email);
                //    users.password = hashpassword;
                //    users.modifiedAt = DateTime.UtcNow;
                //    users.modified_by = user.modified_by;
                //    //users.Mobile = user.Mobile;
                //    //context.User.Attach(user);
                //}
                //else
                //{
                //    context.User.Add(new User()
                //    {
                //        email = user.email,
                //        username = user.username,
                //        password = hashpassword,
                //        status = user.status,
                //        roleid = context.Role.Where(x => x.name == user.Role.name).SingleOrDefault().roleid,
                //        created_by = user.created_by,
                //        createdAt = DateTime.UtcNow
                //        //Mobile = user.Mobile
                //    }) ;
                //}
                //context.SaveChanges();
                //}
                //else
                //{ 
                //    response.success = false; 
                //}
            }
            else
            {
                BadRequest(ModelState);
                //response.success = false;
                response_Message.message = ResponseMessages.NotValidModel;
                response_Message.success = (int)ResponseStatusNumber.Error;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in AddUpdateUser : " , e.ToString());
            //    response_Message.message = ResponseMessages.Error;
            //    response_Message.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(response_Message);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        public async Task<IActionResult> UserSites([FromBody] UsersitesRequestModel requestModel)
        {
            Response_Message response_Message = new Response_Message();

            //try
            //{
            if (ModelState.IsValid)
            {
                UsersitesResponseModel responseModel = new UsersitesResponseModel();
                responseModel = await baseService.UserSites(requestModel);

                if (responseModel.result > 0)
                {
                    response_Message.data = responseModel;
                    response_Message.success = (int)ResponseStatusNumber.Success;
                }
                else
                {
                    response_Message.data = responseModel;
                    response_Message.message = ResponseMessages.Error;
                    response_Message.success = (int)ResponseStatusNumber.False;
                }
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in UserSites ", e.ToString());
            //    response_Message.message = ResponseMessages.Error;
            //    response_Message.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(response_Message);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] RoleRequestModel role)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            if (ModelState.IsValid)
            {
                bool result = await userService.AddRole(role);
                if (result)
                {
                    response.success = (int)ResponseStatusNumber.Success;
                }
                else
                {
                    response.message = ResponseMessages.Error;
                    response.success = (int)ResponseStatusNumber.Error;
                }
            }
            else
            {
                response.success = (int)ResponseStatusNumber.Error;
                BadRequest(ModelState);
            }

            //}catch(Exception e)
            //{
            //    Logger.Log("Error in AddRole ", e.ToString());
            //    response.message = e.Message.ToString();
            //    response.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Login 
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Operator,Maintenance staff,Executive,Technician" })]
        public async Task<Response_Message> Login([FromBody] LoginRequestModel login)
        {
            Response_Message response_Message = new Response_Message();
            if (ModelState.IsValid)
            {
                Guid guidOutput;
                bool isValid = true;
                if (login.barcodeId != null && login.barcodeId != String.Empty)
                {
                    isValid = Guid.TryParse(login.barcodeId, out guidOutput);
                }
                if (isValid)
                {
                    LoginResponceModel response = new LoginResponceModel();
                    response = await userService.UserLogin(login);
                    if (response.uuid != Guid.Empty && response.status == (int)Status.Active)
                    {
                        response_Message.data = response;
                        response_Message.success = (int)ResponseStatusNumber.Success;
                    }
                    else if (response.status == (int)Status.Deactive)
                    {
                        response_Message.message = ResponseMessages.DeActiveUser;
                        response_Message.success = (int)ResponseStatusNumber.False;
                    }
                    else if (response.response_status == (int)ResponseStatusNumber.DeviceNotAssignToUserCompany)
                    {
                        response_Message.message = ResponseMessages.DeviceNotRegisterToUserCompany;
                        response_Message.success = (int)ResponseStatusNumber.DeviceNotAssignToUserCompany;
                    }
                    else if (response.response_status == (int)ResponseStatusNumber.UnauthorizedDevice)
                    {
                        response_Message.message = ResponseMessages.DeviceUnaAuthorized;
                        response_Message.success = (int)ResponseStatusNumber.UnauthorizedDevice;
                    }
                    else
                    {
                        if (login.barcodeId != null)
                        {
                            response_Message.message = ResponseMessages.UserNotFound;
                            response_Message.success = (int)ResponseStatusNumber.NotExists;
                        }
                        else
                        {
                            response_Message.message = ResponseMessages.UsernamePasswordincorrect;
                            response_Message.success = (int)ResponseStatusNumber.NotFound;
                        }
                    }
                }
                else
                {
                    response_Message.message = ResponseMessages.NotValidGuid;
                    response_Message.success = (int)ResponseStatusNumber.Error;
                }
            }
            else
            {
                response_Message.success = (int)ResponseStatusNumber.Error;
                response_Message.message = ResponseMessages.NotValidModel;
            }
            return response_Message;
        }


        /// <summary>
        /// Get Users
        /// </summary>
        /// <param name="userid" example=""></param>
        /// <param name="status" example=""></param>
        /// <param name="pageindex" example="10"></param>
        /// <param name="pagesize" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpGet("{status?}/{pagesize?}/{pageindex?}")]
        public IActionResult GetUsers(int status, int pageindex, int pagesize)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            var users = userService.GetUsers(status, pageindex, pagesize);
            users.pageIndex = pageindex;
            users.pageSize = pagesize;
            if (users.list.Count > 0)
            {
                response.data = users;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else if (users.list.Count == 0 && pageindex > 1)
            {
                response.data = users;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            //}catch(Exception e)
            //{
            //    Logger.Log("Error in GetUsers ", e.ToString());
            //    response.message = ResponseMessages.Error;
            //    response.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Get Filtered Users
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPost]
        public IActionResult FilterUsers([FromBody] FilterUsersRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            var users = userService.FilterUsers(requestModel);
            if (users?.list.Count > 0)
            {
                response.data = users;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(response);
        }

        /// <summary>
        /// Get Filtered Users
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPost]
        public IActionResult FilterUsersOptimized([FromBody] FilterUsersRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            var users = userService.FilterUsersOptimized(requestModel);
            if (users?.list.Count > 0)
            {
                response.data = users;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpGet("{site_id?}")]
        public IActionResult GetAllTechniciansList(string site_id)
        {
            Response_Message response = new Response_Message();
            var users = userService.GetAllTechniciansList(site_id);
            if (users != null)
            {
                response.data = users;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpGet("{site_id?}")]
        public IActionResult GetAllBackOfficeUsersList(string site_id)
        {
            Response_Message response = new Response_Message();
            var users = userService.GetAllBackOfficeUsersList(site_id);
            if (users != null)
            {
                response.data = users;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(response);
        }


        /// <summary>
        /// Get Filtered Users Role Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPost]
        public IActionResult FilterUsersRoleOptions([FromBody] FilterUsersRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            var users = userService.FilterUsersRoleOptions(requestModel);
            if (users?.list?.Count > 0)
            {
                response.data = users;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(response);
        }

        /// <summary>
        /// Get Filtered Users Sites Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPost]
        public IActionResult FilterUsersSitesOptions([FromBody] FilterUsersRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            var users = userService.FilterUsersSitesOptions(requestModel);
            if (users?.list?.Count > 0)
            {
                response.data = users;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(response);
        }

        /// <summary>
        /// Get Filtered Users Company Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPost]
        public IActionResult FilterUsersCompanyOptions([FromBody] FilterUsersRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            var users = userService.FilterUsersCompanyOptions(requestModel);
            if (users?.list?.Count > 0)
            {
                response.data = users;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(response);
        }

        /// <summary>
        /// Generate User Barcode
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager" })]
        [HttpPost]
        public async Task<IActionResult> GenerateUserBarcode(GenerateUserBarcodeRequestModel requestdata)
        {
            //try
            //{
            // get the IDs of users and get the details of user by id.
            // create pdf for that
            AssetBarcodeGenerator userBarcodelists = new AssetBarcodeGenerator();
            List<AssetBarcodeGeneratorList> userBarcodelist = new List<AssetBarcodeGeneratorList>();
            var count = 0;
            foreach (var userid in requestdata.userid)
            {
                var barcodeid = userService.GetBarcodeIdById(userid);
                AssetBarcodeGeneratorList user = new AssetBarcodeGeneratorList();
                var isExist = await s3BucketService.Exists(barcodeid + ".png", _options.barcodes_bucket_name, _options.aws_access_key, _options.aws_secret_key);

                if (isExist)
                {
                    user.asset_barcode_image = UrlGenerator.GetAssetBarcodeImagesURL(barcodeid + ".png");
                }
                else
                {
                    var imagename = await CreateBarcode.GetQRCode(barcodeid.ToString(), _options.aws_access_key, _options.aws_secret_key, _options.barcodes_bucket_name);
                    user.asset_barcode_image = UrlGenerator.GetAssetBarcodeImagesURL(imagename);
                }

                var users = await userService.GetUserByID(userid);
                if (users != null && users.uuid != null && users.uuid != Guid.Empty)
                {
                    user.asset_id = users.barcode_id.ToString();
                    user.asset_name = users.firstname + " " + users.lastname;
                    List<string> roles = new List<string>();
                    users.Userroles.Where(x => x.status == (int)Status.Active).ToList().ForEach(x => roles.Add(x.Role.name));
                    user.internal_asset_id = String.Join(",", roles);
                    //     users.username;
                    List<string> listOfCompany = new List<string>();
                    List<string> listOfSites = new List<string>();
                    users.Usersites.Where(x => x.status == (int)Status.Active).ToList().ForEach(
                        x =>
                        listOfSites.Add(x.Sites.site_name)
                        );
                    users.Usersites.Where(x => x.status == (int)Status.Active).ToList().ForEach(
                        x =>
                        listOfCompany.Add(x.Sites.Company.company_name)
                        );
                    var listStrings = listOfCompany.Select(x => x).Distinct().ToList();
                    var listSites = listOfSites.Select(x => x).Distinct().ToList();
                    string company = String.Join(",", listStrings);
                    string sites = string.Join(",", listOfSites);
                    user.site_name = sites + "\n" + company;
                    //user.site_name = String.Join("," , listStrings);
                }
                userBarcodelist.Add(user);
                count++;
            }

            userBarcodelists.asset = userBarcodelist;


            var pdfBytes = CreateBarcode.CreatePDF(userBarcodelists, CreatePDFTypes.User);
            var fileName = DateTime.Now.Ticks.ToString() + ".pdf";
            var filePathLocal = fileName;
            var done = await CreateBarcode.ByteArrayToFile(filePathLocal, pdfBytes, _options.aws_access_key, _options.aws_secret_key, _options.barcodes_bucket_name);

            if (done)
            {
                //response.data = "https://s3-us-west-2.amazonaws.com/jarvis.barcodesimages/637116799754807863.pdf";
                //response.success = (int)ResponseStatusNumber.Success;
                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(filePathLocal));
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(stream.ToArray())
                };
                result.Content.Headers.ContentDisposition =
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = filePathLocal
                    };
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/pdf");
                var FileStream = new FileStream(filePathLocal, FileMode.Open);
                return File(FileStream, "application/pdf", "Test.pdf");
                //return result;
            }
            else
            {
                return NotFound();
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GenerateUserBarcode ", e.ToString());
            //    return NotFound();
            //}
        }

        //[HttpPost]
        ////SendNotificationRequest request
        //public async Task<int> SendNotification(SendNotificationRequest request)
        //{
        //    //return await userService.SendNotification(request);

        //    List<string> token = new List<string>();
        //    //token.Add("c7kBhQ_MVGo:APA91bExjZsES5kzoGMJAg8FVdbPMyaPUjrtsAlrY5A5aIKe0AiA0UnAnzPQLfhT44GpuYNzo_-xyv8b41yVY3wEOg091CfdoV-NV2g-ATXAYCaJuilpU-qN24R7gE4o4044wBgqshJb");
        //    token.Add(request.token);

        //    RootRequestObj operatorReq = new RootRequestObj()
        //    {
        //        registration_ids = token,
        //        priority = "high",
        //        data = new RequestData()
        //        {
        //            title = "ABC",
        //            body = "ABC",
        //            type = 1,
        //            ref_id = "1",
        //            custom = 1,
        //        },
        //        notification = new Notifications()
        //        {
        //            title = "ABC",
        //            body = "ABC",
        //        }
        //    };
        //    HttpClient client = new HttpClient();
        //    string reqJson = JsonConvert.SerializeObject(operatorReq);
        //    var buffer = System.Text.Encoding.UTF8.GetBytes(reqJson);
        //    var byteContent = new ByteArrayContent(buffer);
        //    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //    string key = string.Empty;
        //    //key = "BEtBN8OeVS7Wk7aU2aO2nAmUn3KZ3WjjTd1u2KpvJvVqmZHTzonY3ZSNXK2mOPM9bconZp78COsX3-R56ByuYsU";

        //    //key = @"AAAAGHa-uJo:APA91bGGtZFqkBNdc45y2IC-F2G2M5AHQjdmV8WCzL9tCDEKINnsPaXhDWb6OfHm8S6DvnSNn_0FFToNd2A0KX_J8YbgYPo34ZO-yU4ZsYH1O777QxxS-oDU84sp7kDfpx0-VszzpkMT";
        //    key = @"AAAA8y37LzE:APA91bEmJZZRxnJNaM5BwiiSpmYC7kYhWgksBbDb0DfEjC5d_zNDUP5hekg3fOleiJ9kn8-addR4bfshXFisJXNaqF_hdoypaFupMznMFjyiczD3B86YFYDCG8Q19vPMZIQhbid5PZ5n";

        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + key);
        //    //byteContent.Headers.Add("Authorization", customerAppKey);
        //    Logger.Log("send noti res " + key + " key " + reqJson);
        //    string url = "https://fcm.googleapis.com/fcm/send";
        //    var res = await client.PostAsync(url, byteContent);
        //    if (res.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        var restring = await res.Content.ReadAsStringAsync();
        //        Logger.Log("Noti res " + restring);
        //        return (int)ResponseStatusNumber.Success;
        //    }
        //    else
        //    {
        //        Logger.Log("Noti res error");
        //        return (int)ResponseStatusNumber.Error;
        //    }

        //    //public async Task<int> sendAndroid(RootRequestObj request, string platform = PlatformTypes.Android)
        //    //{

        //    //}

        //}

        /// <summary>
        /// Get User Notification
        /// </summary>
        /// <param name="pageindex" example="10"></param>
        /// <param name="pagesize" example="1"></param>
        [HttpGet("{pagesize?}/{pageindex?}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Operator,Maintenance staff,Executive,Technician" })]
        public IActionResult GetNotifications(int pageindex, int pagesize)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{ 
            NotificationListViewModel<NotificationViewModel> response = new NotificationListViewModel<NotificationViewModel>();
            response = userService.GetNotification(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), UpdatedGenericRequestmodel.CurrentUser.role_id, pagesize, pageindex);
            response.pageIndex = pageindex;
            response.pageSize = pagesize;
            if (response.list != null && response.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else if (pageindex > 0 && response.list.Count == 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetNotifications ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get User Notification Count By status
        /// 1 = New/Unread
        /// 2= Read
        /// </summary>
        [HttpGet("{status}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Operator,Maintenance staff,Executive,Technician" })]
        public IActionResult GetNotificationsCount(int? status)
        {
            Response_Message responsemodel = new Response_Message();
            int response = 0;
            response = userService.GetNotificationsCount(status.Value);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = "Success";
                responsemodel.data = response;
            }
            else if (response == 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Update Notification Status 
        /// 1 = New
        /// 2 = Read
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Operator,Maintenance staff,Executive,Technician" })]
        public async Task<IActionResult> UpdateNotificationsStatus(UpdateNotificationStatusRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            int response = (int)ResponseStatusNumber.Error;
            response = await userService.UpdateNotificationStatus(requestModel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordUpdated;
                responsemodel.data = response;
            }
            else if (response == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Mark as UNRead status = 1
        /// Mark as Read status = 2
        /// </summary>
        [HttpGet("{status}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Operator,Maintenance staff,Executive,Technician" })]
        public async Task<IActionResult> MarkAllNotificationStatus(int status)
        {
            Response_Message responsemodel = new Response_Message();
            int response = (int)ResponseStatusNumber.Error;
            response = await userService.MarkAllNotificationStatus(status, UpdatedGenericRequestmodel.CurrentUser.requested_by);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordUpdated;
                responsemodel.data = response;
            }
            else if (response == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Logout
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Operator,Maintenance staff,Executive,Technician" })]
        public IActionResult Logout()
        {
            Response_Message response = new Response_Message();
            //try
            //{
            var responseModel = userService.Logout();
            if (responseModel)
            {
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.UserNotFound;
            }
            //}
            //catch(Exception e)
            //{
            //    Logger.Log("Error in Logout ", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Logout Old APP
        /// </summary>
        [HttpGet("{userid}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Operator,Maintenance staff,Executive,Technician" })]
        public IActionResult Logout(string userid)
        {
            Response_Message response = new Response_Message();
            var responseModel = userService.Logout();
            if (responseModel)
            {
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.UserNotFound;
            }
            return Ok(response);
        }

        /// <summary>
        /// Get User By Id
        /// </summary>
        /// <param name="uuid" example=""></param>
        [HttpGet("{uuid}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        public IActionResult GetUserById(string uuid)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            var responseModel = userService.GetUserByIDFromParent(uuid);

            if (responseModel != null && responseModel.uuid != null && responseModel.uuid != Guid.Empty)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }

            //}catch(Exception e)
            //{
            //    Logger.Log("Error in GetUserById ", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("{userid}/{uuid}")]
        public async Task<IActionResult> ResetPassword(string userid, string uuid)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            var responseModel = await userService.ResetPassword(userid, uuid);

            if (responseModel.result > 0)
            {
                response.success = responseModel.result;
            }
            else if (responseModel.result == (int)ResponseStatusNumber.NotFound)
            {
                response.success = responseModel.result;
                response.message = ResponseMessages.nodatafound;
            }
            else
            {
                response.success = responseModel.result;
                response.message = ResponseMessages.Error;
            }
            //}
            //catch(Exception e)
            //{
            //    Logger.Log("Error in ResetPassword ", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("{userid}/{uuid}")]
        public async Task<IActionResult> ResetUserBarcode(string userid, string uuid)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            var responseModel = await userService.ResetUserBarcode(userid, uuid);

            if (responseModel > 0)
            {
                response.success = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.success = responseModel;
                response.message = ResponseMessages.nodatafound;
            }
            else
            {
                response.success = responseModel;
                response.message = ResponseMessages.Error;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in ResetUserBarcode", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Get User Details Offline sync
        /// </summary>
        /// <param name="timestamp" example=""></param>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Operator,Company Admin" })]
        [HttpGet("{timestamp?}/{pagesize?}/{pageindex?}")]
        public IActionResult GetUserDetails(string timestamp, int pagesize, int pageindex)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            var users = userService.GetUserDetails(timestamp, pageindex, pagesize);
            if (users.list.Count > 0)
            {
                response.data = users;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else if (users.list.Count == 0 && pageindex > 1)
            {
                response.data = users;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            //}
            //catch(Exception e)
            //{
            //    Logger.Log("Error in GetUserDetails ", e.ToString());
            //    response.message = ResponseMessages.Error;
            //    response.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Update Notification Token
        /// </summary>
        [TypeFilter(typeof(ValidationFilterAttribute))]
        [HttpPost]
        public async Task<IActionResult> UpdateNotificationToken([FromBody] UpdateUserTokenRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            var result = await userService.UpdateUserToken(requestModel);
            if (result > 0)
            {
                response.success = result;
            }
            else if (result == (int)ResponseStatusNumber.NotFound)
            {
                response.success = result;
                response.message = ResponseMessages.nodatafound;
            }
            else if (result == (int)ResponseStatusNumber.Error)
            {
                response.success = result;
                response.message = ResponseMessages.Error;
            }
            //}catch(Exception e)
            //{
            //    Logger.Log("Error in UpdateNotificationToken ", e.ToString());
            //    response.message = ResponseMessages.Error;
            //    response.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Get Roles
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> GetRoles()
        {
            Response_Message response = new Response_Message();
            //try
            //{
            var responseModel = await userService.GetRoles();
            if (responseModel.list.Count > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            //}
            //catch(Exception e)
            //{
            //    Logger.Log("Error in GetRoles ", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Update User Status
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager" })]
        [HttpPost]
        public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            var responseModel = await userService.UpdateUserStatus(requestModel);
            if (responseModel > 0)
            {
                response.success = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.success = responseModel;
                response.message = ResponseMessages.nodatafound;
            }
            else
            {
                response.success = responseModel;
                response.message = ResponseMessages.Error;
            }
            //}catch(Exception e)
            //{
            //    Logger.Log("Error in UpdateUserStatus ", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Search User 
        /// </summary>
        /// <param name="userid" example=""></param>
        /// <param name="searchstring" example=""></param>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager" })]
        [HttpGet("{searchstring}/{pagesize?}/{pageindex?}")]
        public async Task<IActionResult> SearchUser(string searchstring, int pagesize, int pageindex)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            var responseModel = await userService.SearchUser(searchstring, pageindex, pagesize);
            responseModel.pageIndex = pageindex;
            responseModel.pageSize = pagesize;
            if (responseModel.list.Count > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else if (responseModel.list.Count == 0 && pageindex > 1)
            {
                response.data = responseModel;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            //}
            //catch(Exception e)
            //{
            //    Logger.Log("Error in SearchUser ", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Change Email Notification Status
        /// </summary>
        /// <param name="userid" example=""></param>
        /// <param name="status" example=""></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Operator,Maintenance staff,Executive" })]
        [HttpGet("{status}")]
        public async Task<IActionResult> TriggerEmailNotificationStatus(bool status)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.TriggerEmailNotificationStatus(status);
            if (responseModel > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// Change PM Notification status
        /// </summary>
        /// <param name="status" example=""></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin" })]
        [HttpGet("{status}")]
        public async Task<IActionResult> TriggerPMNotificationStatus(bool status)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.TriggerPMNotificationStatus(status);
            if (responseModel > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// Trigger Operator Usage Report Email Notification Status
        /// </summary>
        [HttpGet("{userid}/{status}")]
        public async Task<IActionResult> TriggerOperatorUsageEmailNotificationStatus(string userid, bool status)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.TriggerOperatorUsageEmailNotificationStatus(userid, status);
            if (responseModel > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(response);
        }
        /// <summary>
        /// Reset User Password
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> ResetUserPassword(UpdateUserPasswordRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            int result = await userService.ResetUserPassword(requestModel);
            response.success = result;
            if (result > 0)
            {
                response.message = ResponseMessages.PasswordChanged;
            }
            else if (result == (int)ResponseStatusNumber.NotFound)
            {
                response.success = result;
                response.message = ResponseMessages.UserNotFound;
            }
            else if (result == (int)ResponseStatusNumber.AlreadyUsedToken)
            {
                response.message = ResponseMessages.AlreadyUsedToken;
            }
            else if (result == (int)ResponseStatusNumber.TokenExpired)
            {
                response.message = ResponseMessages.TokenExpired;
            }
            else if (result == (int)ResponseStatusNumber.InvalidToken)
            {
                response.message = ResponseMessages.InvalidToken;
            }
            else if (result == (int)ResponseStatusNumber.NewPasswordMustBeDifferent)
            {
                response.message = ResponseMessages.NewPasswordMustBeDifferent;
            }
            else if (result == (int)ResponseStatusNumber.Error)
            {
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// Send Reset Password Link  
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SendResetPasswordLink([FromQuery][Required] string email)
        {
            Response_Message response = new Response_Message();

            int result = await userService.ResetPasswordEmail(email);
            if (result > 0)
            {
                response.success = result;
                response.message = ResponseMessages.ResetPasswordLinkSent;
            }
            else
            {
                if (result == (int)ResponseStatusNumber.DeActiveRecord)
                {
                    response.message = ResponseMessages.DeActiveUser;
                }
                else if (result == (int)ResponseStatusNumber.NotFound)
                {
                    response.message = ResponseMessages.EmailNotRegister;
                }
                else
                {
                    response.message = ResponseMessages.Error;
                }
                response.success = result;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPut]
        public async Task<IActionResult> DefaultRole(UpdateDefaultRoleRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.UpdateDefaultRole(requestModel);
            if (responseModel > 0)
            {
                response.success = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.success = responseModel;
                response.message = ResponseMessages.nodatafound;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotExists)
            {
                response.success = responseModel;
                response.message = ResponseMessages.NotAssignRole;
            }
            else
            {
                response.success = responseModel;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpPut]
        public async Task<IActionResult> DefaultSite(UpdateDefaultSiteRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.UpdateDefaultSite(requestModel);
            if (responseModel > 0)
            {
                response.success = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.success = responseModel;
                response.message = ResponseMessages.nodatafound;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotExists)
            {
                response.success = responseModel;
                response.message = ResponseMessages.NotAssignSite;
            }
            else
            {
                response.success = responseModel;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// Get the Quicksight embeding Url
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Company Admin" })]
        [HttpGet()]
        public async Task<IActionResult> GetQuickSightUrl([FromQuery] string dashboardtype = "1")
        {
            Response_Message response = new Response_Message();
            var rolename = await userService.GetQuickSightEmbedUrlAsync();
            var embedingUrl = "";
            if (rolename == GlobalConstants.Manager || rolename == GlobalConstants.CompanyAdmin || rolename == GlobalConstants.Executive)
            {
                if (dashboardtype == "1")
                {
                    embedingUrl = await s3BucketService.GetQuickSightEmbedUrl(_options.quicksight_aws_access_key, _options.quicksight_aws_secret_key, _options.account_id, _options.manager_dashboard_id, _options.role_arn);
                }
                else if (dashboardtype == "2")
                {
                    embedingUrl = await s3BucketService.GetQuickSightEmbedUrl(_options.quicksight_aws_access_key, _options.quicksight_aws_secret_key, _options.account_id, _options.pm_overview_dashboard_id, _options.role_arn);
                }
                else if (dashboardtype == "3")
                {
                    embedingUrl = await s3BucketService.GetQuickSightEmbedUrl(_options.quicksight_aws_access_key, _options.quicksight_aws_secret_key, _options.account_id, _options.business_overview_dashboard_id, _options.role_arn);
                }
                else if (dashboardtype == "4")
                {
                    embedingUrl = await s3BucketService.GetQuickSightEmbedUrl(_options.quicksight_aws_access_key, _options.quicksight_aws_secret_key, _options.account_id, _options.asset_overview_dashboard_id, _options.role_arn);
                }
                else if (dashboardtype == "5")
                {
                    embedingUrl = await s3BucketService.GetQuickSightEmbedUrl(_options.quicksight_aws_access_key, _options.quicksight_aws_secret_key, _options.account_id, _options.nfpa_70b_compliance_dashboard_id, _options.role_arn);
                }
                else if (dashboardtype == "6")
                {
                    embedingUrl = await s3BucketService.GetQuickSightEmbedUrl(_options.quicksight_aws_access_key, _options.quicksight_aws_secret_key, _options.account_id, _options.nfpa_70e_compliance_dashboard_id, _options.role_arn);
                }
                if (!String.IsNullOrEmpty(embedingUrl))
                {
                    response.data = embedingUrl;
                }
                else
                {
                    response.message = ResponseMessages.Error;
                }
            }
            else if (rolename == GlobalConstants.Executive) // deprectaed 
            {
                embedingUrl = await s3BucketService.GetQuickSightEmbedUrl(_options.aws_access_key, _options.aws_secret_key, _options.account_id, _options.executive_dashboard_id, _options.role_arn);
                if (!String.IsNullOrEmpty(embedingUrl))
                {
                    response.data = embedingUrl;
                }
                else
                {
                    response.message = ResponseMessages.Error;
                }
            }
            else
            {
                response.message = ResponseMessages.QuickSightUnauthorizedUser;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpPut]
        public async Task<IActionResult> ActiveSite(UpdateActiveSiteRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.UpdateActiveSite(requestModel);
            if (responseModel > 0)
            {
                response.success = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.success = responseModel;
                response.message = ResponseMessages.nodatafound;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotExists)
            {
                response.success = responseModel;
                response.message = ResponseMessages.NotAssignSite;
            }
            else
            {
                response.success = responseModel;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPut]
        public async Task<IActionResult> ActiveRole(UpdateActiveRoleRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.UpdateActiveRole(requestModel);
            if (responseModel > 0)
            {
                response.success = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.success = responseModel;
                response.message = ResponseMessages.nodatafound;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotExists)
            {
                response.success = responseModel;
                response.message = ResponseMessages.NotAssignRole;
            }
            else
            {
                response.success = responseModel;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// Validate Reset Password Token
        /// </summary>
        //[HttpGet]
        //public async Task<IActionResult> ValidateResetPasswordToken([FromQuery][Required] string token)
        //{
        //    Response_Message response = new Response_Message();
        //    int result = await userService.ValidateResetPasswordToken(token);
        //    response.success = result;
        //    if (result > 0)
        //    {
        //        response.message = ResponseMessages.ValidToken;
        //    }
        //    else if (result == (int)ResponseStatusNumber.AlreadyUsedToken)
        //    {
        //        response.message = ResponseMessages.AlreadyUsedToken;
        //    }
        //    else if (result == (int)ResponseStatusNumber.TokenExpired)
        //    {
        //        response.message = ResponseMessages.TokenExpired;
        //    }
        //    else if (result == (int)ResponseStatusNumber.InvalidToken)
        //    {
        //        response.message = ResponseMessages.InvalidToken;
        //    }
        //    else if (result == (int)ResponseStatusNumber.Error)
        //    {
        //        response.message = ResponseMessages.Error;
        //    }
        //    return Ok(response);
        //}

        /// <summary>
        /// Get All Operators list for filter
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpGet]
        public async Task<IActionResult> GetAllOperatorsList()
        {
            Response_Message response = new Response_Message();
            var result = await userService.GetAllOperatorsList();
            response.success = result.result;
            if (result.result > 0)
            {
                response.data = result;
            }
            else if (result.result == (int)ResponseStatusNumber.NotFound)
            {
                response.message = ResponseMessages.nodatafound;
            }
            else if (result.result == (int)ResponseStatusNumber.Error)
            {
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// Resend Temporary Password
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,SuperAdmin,Executive" })]
        [HttpGet]
        public async Task<IActionResult> ResendTemporaryPassword([FromQuery][Required] string user_id)
        {
            Response_Message response = new Response_Message();
            var result = await userService.ResendCognitoUser(user_id, _options.cognito_resend_aws_access_key, _options.cognito_resend_aws_secret_key);
            response.success = result;
            if (result > 0)
            {
                response.data = result;
            }
            else if (result == (int)ResponseStatusNumber.NotFound)
            {
                response.message = ResponseMessages.nodatafound;
            }
            else if (result == (int)ResponseStatusNumber.Error)
            {
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// Email Verified on Cognito New Password Creation Success
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> VerifyEmail()
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.UpdateEmailVerified(true);
            if (responseModel > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// Email Verified on Cognito New Password Creation Success
        /// </summary>
        /// <returns></returns>
        [HttpPost]
       // [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> VerifyEmailV2(VerifyEmailV2Requestmodel requestmodel)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.UpdateEmailVerifiedV2(requestmodel.email, requestmodel.company_id,requestmodel.domain_name);
            if (responseModel > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpPut]
        public async Task<IActionResult> DefaultCompany(UpdateDefaultCompanyRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.UpdateDefaultCompany(requestModel);
            if (responseModel > 0)
            {
                response.success = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.success = responseModel;
                response.message = ResponseMessages.nodatafound;
            }
            else
            {
                response.success = responseModel;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPut]
        public async Task<IActionResult> DefaultClientCompany(UpdateDefaultCompanyRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.UpdateDefaultClientCompany(requestModel);
            if (responseModel > 0)
            {
                response.success = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.success = responseModel;
                response.message = ResponseMessages.nodatafound;
            }
            else
            {
                response.success = responseModel;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpPut]
        public async Task<IActionResult> ActiveCompany(UpdateDefaultCompanyRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.UpdateActiveCompany(requestModel);
            if (responseModel > 0)
            {
                response.success = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.success = responseModel;
                response.message = ResponseMessages.nodatafound;
            }
            else
            {
                response.success = responseModel;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPut]
        public async Task<IActionResult> ActiveClientCompany(UpdateDefaultCompanyRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.UpdateActiveClientCompany(requestModel);
            if (responseModel > 0)
            {
                response.success = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.success = responseModel;
                response.message = ResponseMessages.nodatafound;
            }
            else
            {
                response.success = responseModel;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// DailyReport = 22
        /// WeeklyReport = 23
        /// None = 0
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Executive,Company Admin" })]
        [HttpGet("{status}")]
        public async Task<IActionResult> UpdateExecutiveEmailNotificationStatus(int status)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.UpdateExecutiveEmailNotificationStatus(status);
            if (responseModel > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(response);
        }
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Executive,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UpdateExecutivePMDueReportEmailStatus(ExecutivePMDueEmailConfigRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.UpdateExecutivePMDueReportEmailStatus(requestModel);
            if (responseModel > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// DailyReport = 22
        /// WeeklyReport = 23
        /// None = 0
        /// </summary>
        /// <returns></returns>
        // [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Executive,Company Admin" })]
        [HttpPost()]
        public IActionResult GetUserRolesByEmail(GetUserRolesByEmailRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            var responseModel = userService.GetUserRolesByEmail(requestModel.email_id.ToLower().Trim());
            if (responseModel != null && responseModel.Count>0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else if (responseModel != null && responseModel.Count == 0)
            {
                response.success = (int)ResponseStatusNumber.NotExists;
                response.data = responseModel;
                response.message = ResponseMessages.UserNotFound;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpPost]
        public async Task<IActionResult> CreateClientCompany(CreateClientCompanyRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            CreateUpdateSiteResponsemodel responseModel = await userService.CreateClientCompany(requestModel);
            if (responseModel.success == (int)ResponseStatusNumber.Success)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
                response.message = (string)ResponseMessages.RecordUpdated;
            }
            else if (responseModel.success == (int)ResponseStatusNumber.user_exist_with_single_site)
            {
                response.success = (int)ResponseStatusNumber.user_exist_with_single_site;
                response.message = "You can not inactive this clientcompany as Some users have only this clientcompany access please reassign to another clientcompany first to these users " + responseModel.user_emails;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpPost]
        public async Task<IActionResult> CreateUpdateSite(CreateUpdateSiteRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            var responseModel = await userService.CreateUpdateSite(requestModel);
            if (responseModel != null && responseModel.success == (int)ResponseStatusNumber.Success)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
                response.message = (string)ResponseMessages.RecordUpdated;
            }
            else if (responseModel.success == (int)ResponseStatusNumber.user_exist_with_single_site)
            {
                response.success = (int)ResponseStatusNumber.user_exist_with_single_site;
                response.message = "You can not inactive this site as Some users have only this site access please reassign to another site first to these users "+ responseModel.user_emails; 
            }
            else 
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpPost]
        public IActionResult GetAllClientCompanyWithSites(GetAllClientCompanyWithSitesRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            var responseModel =  userService.GetAllClientCompanyWithSites(requestModel);
            if (responseModel != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }



        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadUserProfilePicture()
        {
            Response_Message responsemodel = new Response_Message();

            List<IFormFile> filesToUpload = new List<IFormFile>();
            var user_file_name = String.Empty;

            if (Request.Form.Files.Count > 0)
            {
                foreach (var file in Request.Form.Files)
                {
                    user_file_name = file.FileName;
                    filesToUpload.Add(file);
                }

                var list = await s3BucketService.UploadAssetImage(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.conduit_dev_userprofile_bucket);

                if (list != null && list.original_imege_file != null)
                {
                    var items = new WorkOrderAttachmentsResponseModel
                    {
                        filename = list.original_imege_file,
                        file_url = UrlGenerator.GetProfilePictureURL(list.original_imege_file),
                        thumbnail_filename = list.thumbnail_image_file,
                        thumbnail_file_url = UrlGenerator.GetProfilePictureURL(list.thumbnail_image_file),
                        user_uploaded_name = user_file_name
                    };
                    responsemodel.success = 1;
                    responsemodel.data = items;
                    return Ok(responsemodel);
                }
                else
                {
                    responsemodel.success = 0;
                    Res_ErrorMessage mess = new Res_ErrorMessage(Resource.ResponseMessages.UPLOADFILE_INVALIDFORMAT, Resource.ResponseMessages.UPLOADFILE_TITLE);
                    responsemodel.data = mess;
                    return Ok(responsemodel);
                }
            }
            else
            {
                responsemodel.success = 0;
                responsemodel.data = "No file selected!!!";
                return Ok(responsemodel);
            }
        }

        //[TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Executive,Manager" })]
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadSiteProfileImage()
        {
            Response_Message responsemodel = new Response_Message();

            List<IFormFile> filesToUpload = new List<IFormFile>();
            var user_file_name = String.Empty;

            if (Request.Form.Files.Count > 0)
            {
                foreach (var file in Request.Form.Files)
                {
                    user_file_name = file.FileName;
                    filesToUpload.Add(file);
                }
                string site_id = Request.Form["site_id"];
                string company_id = Request.Form["company_id"];

                //var list = await s3BucketService.UploadAssetImage(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.conduit_dev_siteprofile_bucket);
                var list = await s3BucketService.UploadSiteProfileImage(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.conduit_siteprofile_bucket, company_id, site_id);

                if (list != null && list.original_imege_file != null)
                {
                    var items = new WorkOrderAttachmentsResponseModel
                    {
                        filename = list.user_uploaded_filename,
                        file_url = list.original_imege_file,//UrlGenerator.GetProfilePictureURL(list.original_imege_file),
                        //thumbnail_filename = list.thumbnail_image_file,
                        //thumbnail_file_url = UrlGenerator.GetProfilePictureURL(list.thumbnail_image_file),
                        user_uploaded_name = user_file_name
                    };
                    responsemodel.success = 1;
                    responsemodel.data = items;
                    return Ok(responsemodel);
                }
                else
                {
                    responsemodel.success = 0;
                    Res_ErrorMessage mess = new Res_ErrorMessage(Resource.ResponseMessages.UPLOADFILE_INVALIDFORMAT, Resource.ResponseMessages.UPLOADFILE_TITLE);
                    responsemodel.data = mess;
                    return Ok(responsemodel);
                }
            }
            else
            {
                responsemodel.success = 0;
                responsemodel.data = "No file selected!!!";
                return Ok(responsemodel);
            }
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive" })]
        [HttpPost]
        public async  Task<IActionResult> AddUpdateWorkOrderWatcher(AddUpdateWorkOrderWatcherRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.AddUpdateWorkOrderWatcher(requestModel);
            if (responseModel == (int)ResponseStatusNumber.Success)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
                response.message = (string)ResponseMessages.RecordUpdated;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpGet("{user_id}")]
        public IActionResult GetActiveUserSitesAndRoles(Guid user_id)
        {
            Response_Message response = new Response_Message();
            var responseModel = userService.GetActiveUserSitesAndRoles(user_id);
            if (responseModel != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
                response.message = (string)ResponseMessages.SuccessMessage;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpGet]
        public IActionResult GetAllProjectManagersList()
        {
            Response_Message response = new Response_Message();
            var users = userService.GetAllProjectManagersList();
            if (users != null)
            {
                response.data = users;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpGet("{wo_id}")]
        public IActionResult GetAllTechniciansLocationByWOId(Guid wo_id)
        {
            Response_Message response = new Response_Message();
            var responseModel = userService.GetAllTechniciansLocationByWOId(wo_id);
            if (responseModel != null&& responseModel.list != null && responseModel.list.Count > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
                response.message = (string)ResponseMessages.SuccessMessage;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpPost]
        public async Task<IActionResult> AddUserGeoLocationData(AddUserGeoLocationDataRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            int responseModel = await userService.AddUserGeoLocationData(requestModel);
            if (responseModel == (int)ResponseStatusNumber.Success)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
                response.message = (string)ResponseMessages.RecordAdded;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpPost]
        public IActionResult GetAllTechniciansForCalendar(GetAllCalanderWorkordersRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            var users = userService.GetAllTechniciansForCalendar(requestModel);
            if (users != null && users.list!=null && users.list.Count>0)
            {
                response.data = users;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpPost]
        public IActionResult GetAllLeadsForCalendar(GetAllCalanderWorkordersRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            var users = userService.GetAllLeadsForCalendar(requestModel);
            if (users != null && users.list != null && users.list.Count > 0)
            {
                response.data = users;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(response);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpGet("{vendor_id}")]
        public async Task<IActionResult> ViewVendorDetailsById(Guid vendor_id)
        {
            Response_Message response = new Response_Message();
            var res = await userService.ViewVendorDetailsById(vendor_id);
            if (res != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = res;
                response.message = (string)ResponseMessages.SuccessMessage;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> CreateUpdateVendor(CreateUpdateVendorRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            int response = await userService.CreateUpdateVendor(requestModel);

            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> CreateUpdateContact(CreateUpdateContactRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            int response = await userService.CreateUpdateContact(requestModel);

            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public IActionResult GetAllVendorList(GetAllVendorListRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            GetAllVendorListResponseModel response = new GetAllVendorListResponseModel();

            response = userService.GetAllVendorList(requestModel);

            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.SuccessMessage;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpGet]
        public IActionResult GetAllVendorsContactsForDropdown()
        {
            Response_Message response = new Response_Message();
            var res = userService.GetAllVendorsContactsForDropdown();
            if (res != null&& res.workorder_vendor_contacts_list!=null && res.workorder_vendor_contacts_list.Count>0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = res;
                response.message = (string)ResponseMessages.SuccessMessage;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpPost]
        public async Task<IActionResult> GetRefreshedContactsByWOId(GetRefreshedContactsByWOIdRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            var res = await userService.GetRefreshedContactsByWOId(requestModel);
            if (res != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = res;
                response.message = (string)ResponseMessages.SuccessMessage;
            }
            else
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpGet]
        public IActionResult GetSiteUsersDetailsById([FromQuery][Required] Guid site_id)
        {
            Response_Message response = new Response_Message();
            var responseModel = userService.GetSiteUsersDetailsById(site_id);
            if (responseModel != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

    }
}
