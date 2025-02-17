using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jarvis.Filter;
using Jarvis.Resource;
using Jarvis.Service.Abstract;
using Jarvis.Service.Concrete;
using Jarvis.Shared;
using Jarvis.Shared.Helper;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using Jarvis.ViewModels.Filter;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Jarvis.Controllers
{
    [TypeFilter(typeof(requestvalidationFilter))]
    //[TypeFilter(typeof(ValidationFilterAttribute))]
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiController]
    public class InspectionController : ControllerBase
    {
        public AWSConfigurations _options { get; set; }
        private readonly IInspectionService inspectionService;
        private readonly IS3BucketService s3BucketService;
        public InspectionController(IInspectionService _inspectionService, IOptions<AWSConfigurations> options)
        {
            this.inspectionService = _inspectionService;
            this.s3BucketService = new S3BucketService();
            this._options = options.Value;
        }

        /// <summary>
        /// Get All Inspection
        /// </summary>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpGet("{pagesize?}/{pageindex?}")]
        public IActionResult GetAllInspections(int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            ListViewModel<InspectionResponseModel> response = new ListViewModel<InspectionResponseModel>();
            response = inspectionService.GetAllInspections(pagesize, pageindex);
            response.pageSize = pagesize;
            response.pageIndex = pageindex;
            if (response.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else if (pageindex > 1 && response.list.Count == 0)
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
            //    Logger.Log("Error in GetAllInspections : ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }


        /// <summary>
        /// Get All Inspection with Filter
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpPost]
        public IActionResult FilterInspections([FromBody] FilterInspectionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<InspectionResponseModel> response = new ListViewModel<InspectionResponseModel>();
            response = inspectionService.FilterInspections(requestModel);
            if (response?.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Filter Inspection Asset Name Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpPost]
        public IActionResult FilterInspectionAssetNameOptions([FromBody] FilterInspectionOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<AssetListResponseModel> response = new ListViewModel<AssetListResponseModel>();
            response = inspectionService.FilterInspectionAssetNameOption(requestModel);
            if (response?.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Filter Inspection Asset Status Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpPost]
        public IActionResult FilterInspectionStatusOptions([FromBody] FilterInspectionOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<int> response = new ListViewModel<int>();
            response = inspectionService.FilterInspectionStatusOption(requestModel);
            if (response?.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Filter Inspection Asset Shift Number Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpPost]
        public IActionResult FilterInspectionShiftNumberOptions([FromBody] FilterInspectionOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<int> response = new ListViewModel<int>();
            response = inspectionService.FilterInspectionShiftNumberOption(requestModel);
            if (response?.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Filter Inspection Asset Operator Name Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpPost]
        public IActionResult FilterInspectionOperatorsOptions([FromBody] FilterInspectionOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<OperatorsListResponseModel> response = new ListViewModel<OperatorsListResponseModel>();
            response = inspectionService.FilterInspectionOperatorsOption(requestModel);
            if (response?.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Filter Inspection Supervisor Name Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpPost]
        public IActionResult FilterInspectionSupervisorOptions([FromBody] FilterInspectionOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<OperatorsListResponseModel> response = new ListViewModel<OperatorsListResponseModel>();
            response = inspectionService.FilterInspectionSupervisorOption(requestModel);
            if (response?.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Filter Inspection Asset Sites Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpPost]
        public IActionResult FilterInspectionSitesOptions([FromBody] FilterInspectionOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<SitesViewModel> response = new ListViewModel<SitesViewModel>();
            response = inspectionService.FilterInspectionSitesOption(requestModel);
            if (response?.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Filter Inspection Asset Company Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpPost]
        public IActionResult FilterInspectionCompanyOptions([FromBody] FilterInspectionOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<CompanyViewModel> response = new ListViewModel<CompanyViewModel>();
            response = inspectionService.FilterInspectionCompanyOption(requestModel);
            if (response?.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Add Asset Inspection 
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Operator,Company Admin,Executive" })]
        [HttpPost]
        public async Task<IActionResult> AddInspection([FromBody] InspectionRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            bool result = false;
            result = await inspectionService.AddInspection(requestModel);
            if (result)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in AddInspection ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Inspection By ID
        /// </summary>
        /// <param name="inspectionid" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,Operator,Maintenance staff,SuperAdmin,Technician" })]
        [HttpGet("{inspectionid}")]
        public async Task<IActionResult> GetInspectionById(string inspectionid)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            InspectionResponseModel response = new InspectionResponseModel();
            response = await inspectionService.GetInspectionById(inspectionid);
            if (response.inspection_id != null && response.inspection_id != Guid.Empty)
            {
                responsemodel.data = response;
                responsemodel.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetInspectionById ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Inspection By ID
        /// </summary>
        /// <param name="inspectionid" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,Operator,Maintenance staff,SuperAdmin" })]
        [HttpGet("{inspectionid}/{userid}")]
        public async Task<IActionResult> GetInspectionById(string inspectionid, string userid)
        {
            Response_Message responsemodel = new Response_Message();
            InspectionResponseModel response = new InspectionResponseModel();
            response = await inspectionService.GetInspectionByIdForOperator(inspectionid, userid);
            if (response.inspection_id != null && response.inspection_id != Guid.Empty)
            {
                responsemodel.data = response;
                responsemodel.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get All Pending Inspection By Manager
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive" })]
        [HttpGet]
        public IActionResult PendingInspectionByManager()
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            List<InspectionResponseModel> response = new List<InspectionResponseModel>();
            response = inspectionService.GetPendingInspectionByManager();
            if (response.Count > 0)
            {
                responsemodel.data = response;
                responsemodel.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in PendingInspectionByManager ", e.ToString());
            //    responsemodel.message = ResponseMessages.Error;
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get All Pending And Today CheckOut Asset By Manager
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive" })]
        [HttpGet]
        public async Task<IActionResult> PendingInspectionCheckoutAssetsByManager()
        {
            Response_Message response = new Response_Message();
            //try
            //{
            PendingInspectionCheckoutAssetsManagerResponseModel responseModel = new PendingInspectionCheckoutAssetsManagerResponseModel();
            responseModel = await inspectionService.PendingInspectionCheckoutAssetsManager();
            if (responseModel != null)
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
            //catch (Exception e)
            //{
            //    Logger.Log("Error in PendingInspectionCheckoutAssetsByManager ", e.ToString());
            //    response.message = ResponseMessages.Error;
            //    response.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Get Pending Reviews, Outstanding Issues and CheckOut Assets Count By Manager
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Technician,Company Admin,Executive" })]
        [HttpGet]
        public async Task<IActionResult> ManagerMobileDashboardDataCount()
        {
            Response_Message response = new Response_Message();

            ManagerMobileDashboardDataCountResponseModel responseModel = new ManagerMobileDashboardDataCountResponseModel();
            responseModel = await inspectionService.ManagerMobileDashboardDataCount();
            if (responseModel != null)
            {
                response.data = responseModel;
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
        /// Get Pending And CheckOut Asset By Operator 
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Operator,Company Admin,Executive" })]
        [HttpGet]
        public async Task<IActionResult> PendingInspectionCheckoutAssetsByOperator()
        {
            Response_Message response = new Response_Message();
            //try
            //{
            List<PendingAndCheckoutInspViewModel> responseModel = new List<PendingAndCheckoutInspViewModel>();
            responseModel = await inspectionService.PendingInspectionCheckoutAssetsByOperator();
            if (responseModel != null && responseModel.Count > 0)
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
            //catch (Exception e)
            //{
            //    Logger.Log("Error in PendingInspectionCheckoutAssetsByOperator ", e.ToString());
            //    response.message = ResponseMessages.Error;
            //    response.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Get Pending And CheckOut Asset By Operator User ID
        /// </summary>
        /// <param name="userid" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Operator,Company Admin,Executive" })]
        [HttpGet("{userid}")]
        public async Task<IActionResult> PendingInspectionCheckoutAssetsByOperator(string userid)
        {
            Response_Message response = new Response_Message();
            List<PendingAndCheckoutInspViewModel> responseModel = new List<PendingAndCheckoutInspViewModel>();
            responseModel = await inspectionService.PendingInspectionCheckoutAssetsByOperator();
            if (responseModel != null && responseModel.Count > 0)
            {
                response.data = responseModel;
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
        /// Create Inspection
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Operator,Manager,Company Admin,Executive" })]
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> CreateInspection()
        {
            Response_Message responcemodel = new Response_Message();
            //try
            //{
            //DateTime requested_datetime = DateTime.ParseExact("2019-01-01 00:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            string assetGuid = Request.Form["asset_id"];
            string operatorId = Request.Form["operator_id"];
            DateTime requested_datetime = new DateTime();
            DateTime created_datetime = new DateTime();
            try
            {
                requested_datetime = DateTime.ParseExact(Request.Form["requested_datetime"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                created_datetime = DateTime.ParseExact(Request.Form["created_datetime"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch
            {
                // do nothing;
                responcemodel.success = (int)ResponseStatusNumber.Error;
                responcemodel.message = ResponseMessages.ValidDatetime;
                return Ok(responcemodel);
            }

            //int inspectionstatus = (int)ResponseStatusNumber.False;
            int assetinspectionatsametime = (int)ResponseStatusNumber.False;
            if (assetGuid != null && operatorId != null)
            {
                assetinspectionatsametime = inspectionService.CheckAssetInspectionByTime(assetGuid, operatorId, Request.Form["requested_datetime"]);
                //inspectionstatus = inspectionService.CheckPendingInspection(assetGuid, operatorId);
            }

            if (assetinspectionatsametime > 0)
            {
                List<IFormFile> filesToUpload = new List<IFormFile>();

                foreach (var file1 in Request.Form.Files)
                {
                    Logger.Log("Images ", file1.FileName.ToString());
                    filesToUpload.Add(file1);
                }

                var list = await s3BucketService.UploadImage(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.inspection_bucket_name);
                if (list?.Count > 0)
                {
                    var fullPathUrls = UrlGenerator.GetInspectionImagesURL(list);
                    var uploadedlist = await s3BucketService.UploadThumbNailImagesForExisting(fullPathUrls, _options.aws_access_key, _options.aws_secret_key, _options.inspection_bucket_name, _options.thumbnail_folder_name, _options.thumbnail_height, _options.thumbnail_width);
                    Logger.Log("Thumbnail Uploaded Images "+ uploadedlist.ToString());
                }
                //if (list.Count > 0 || Convert.ToInt32(Request.Form["status"]) == (int)Status.Approved)
                //{
                Guid siteGuid = Guid.Empty;
                string stringsiteGuid = Request.Form["site_id"];

                InspectionRequestModel inspectionRequestModel = new InspectionRequestModel();
                inspectionRequestModel.asset_id = Guid.Parse(assetGuid);
                inspectionRequestModel.operator_id = Request.Form["operator_id"];
                inspectionRequestModel.manager_id = Request.Form["manager_id"];
                inspectionRequestModel.status = Convert.ToInt32(Request.Form["status"]);
                inspectionRequestModel.operator_notes = Request.Form["operator_notes"];
                var attribute_json = Request.Form["attribute_values"];
                //inspectionRequestModel.attribute_values = Request.Form["attribute_values"];
                inspectionRequestModel.company_id = Request.Form["company_id"];
                inspectionRequestModel.site_id = Guid.Parse(stringsiteGuid); ;
                inspectionRequestModel.created_at = created_datetime;
                inspectionRequestModel.modified_at = created_datetime;
                inspectionRequestModel.created_by = Request.Form["operator_id"];
                inspectionRequestModel.meter_hours = Convert.ToInt64(Request.Form["meter_hours"]);
                inspectionRequestModel.shift = Convert.ToInt32(Request.Form["shift"]);
                inspectionRequestModel.is_comment_important = Convert.ToBoolean(Request.Form["is_comment_important"]);
                ImagesListObjectViewModel images = new ImagesListObjectViewModel();
                List<string> images_names = new List<string>();
                foreach (var item in list)
                {
                    images_names.Add(item);
                }
                images.image_names = images_names;

                inspectionRequestModel.image_list = images;
                inspectionRequestModel.datetime_requested = requested_datetime;
                List<AssetsValueJsonObjectViewModel> attributesJsonObjectViewModel = new List<AssetsValueJsonObjectViewModel>();
                
                attributesJsonObjectViewModel = JsonConvert.DeserializeObject<List<AssetsValueJsonObjectViewModel>>(Request.Form["attribute_values"]);
                
                inspectionRequestModel.attribute_values = attributesJsonObjectViewModel.ToArray();
                var response = await AddInspection(inspectionRequestModel);
                return response;
                //}
                //else
                //{
                //    responcemodel.success = 0;
                //    responcemodel.message = ResponseMessages.NotValidModel;

                //}
                }
            //else if(inspectionstatus < 0 && assetinspectionatsametime > 0)
            //{
            //    if (inspectionstatus == (int)AssetStatus.AssetInMaintenance)
            //    {
            //        Logger.Log("Aseet in Maintenance");
            //        responcemodel.success = inspectionstatus;
            //        responcemodel.message = ResponseMessages.AssetInMaintenanace;
            //    }
            //    else if (inspectionstatus == (int)AssetStatus.PendingInspection)
            //    {
            //        Logger.Log("Aseet in Pending Inspection");
            //        responcemodel.success = inspectionstatus;
            //        responcemodel.message = ResponseMessages.PendingInspection;
            //    }
            //    else
            //    {
            //        responcemodel.success = (int)ResponseStatusNumber.Error;
            //        responcemodel.message = ResponseMessages.Error;
            //    }
            //}
            else
            {
                Logger.Log("Already Inspection Create at : ", requested_datetime.ToString());
                responcemodel.success = assetinspectionatsametime;
                responcemodel.message = ResponseMessages.AlreadyCheckOut;
            }

            return Ok(responcemodel);
            //}
            //catch (Exception ex)
            //{
            //    Logger.Log("Error in CreateInspection ", ex.ToString());
            //    return StatusCode(500, "Internal server error");
            //}
        }

        /// <summary>
        /// Approve Inspection
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive" })]
        [HttpPost]
        public async Task<ActionResult> ApproveInspection([FromBody] ApproveInspectionRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            var result = await inspectionService.ApproveInspection(requestModel);
            if (result > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
            }
            else if (result == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = result;
            }
            else if (result == (int)ResponseStatusNumber.AlreadyExists)
            {
                responsemodel.message = ResponseMessages.PendingInspectionAlreadyExists;
                responsemodel.success = result;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = result;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in ApproveInspection ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Inspection By Asset ID
        /// </summary>
        /// <param name="assetid" example=""></param>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpGet("{assetid}/{pagesize?}/{pageindex?}")]
        public async Task<IActionResult> GetInspectionByAssetId(string assetid, int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            var result = await inspectionService.GetInspectionByAssetId(assetid, pagesize, pageindex);
            result.pageIndex = pageindex;
            result.pageSize = pagesize;
            if (result.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = result;
            }
            else if (pageindex > 1 && result.list.Count == 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = result;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetInspectionByAssetId ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Search Inspection 
        /// </summary>
        /// <param name="searchstring" example=""></param>
        /// <param name="timezone" example=""></param>
        /// <param name="pagesize" example=""></param>
        /// <param name="pageindex" example=""></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpGet("{searchstring?}/{timezone?}/{pagesize?}/{pageindex?}")]
        public async Task<IActionResult> SearchInspection(string searchstring, string timezone, int pagesize, int pageindex)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            ListViewModel<InspectionResponseModel> responseModel = new ListViewModel<InspectionResponseModel>();
            responseModel = await inspectionService.SearchInspections(searchstring, timezone, pagesize, pageindex);
            responseModel.pageIndex = pageindex;
            responseModel.pageSize = pagesize;
            if (responseModel.list.Count > 0)
            {
                response.data = responseModel;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else if (pageindex > 1 && responseModel.list.Count == 0)
            {
                response.data = responseModel;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in SearchInspection ", e.ToString());
            //    response.message = ResponseMessages.Error;
            //    response.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Search Inspection By Asset
        /// </summary>
        /// <param name="assetid" example=""></param>
        /// <param name="searchstring" example=""></param>
        /// <param name="timezone" example=""></param>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpGet("{assetid}/{searchstring}/{timezone}/{pagesize?}/{pageindex?}")]
        public async Task<IActionResult> SearchInspectionByAsset(string assetid, string searchstring, string timezone, int pagesize, int pageindex)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            ListViewModel<InspectionResponseModel> responseModel = new ListViewModel<InspectionResponseModel>();
            responseModel = await inspectionService.SearchInspectionsByAsset(assetid, searchstring, timezone, pagesize, pageindex);
            responseModel.pageSize = pagesize;
            responseModel.pageIndex = pageindex;
            if (responseModel.list.Count > 0)
            {
                response.data = responseModel;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else if (pageindex > 1 && responseModel.list.Count == 0)
            {
                response.data = responseModel;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in SearchInspectionByAsset ", e.ToString());
            //    response.message = ResponseMessages.Error;
            //    response.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Create Inspection Offline
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(ValidationFilterAttribute))]
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> CreateInspectionOffline()
        {
            Response_Message responcemodel = new Response_Message();
            //try
            //{
            DateTime requested_datetime = new DateTime();
            try
            {
                requested_datetime = DateTime.ParseExact(Request.Form["requested_datetime"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch
            {
                // do nothing;
                responcemodel.success = (int)ResponseStatusNumber.Error;
                responcemodel.message = ResponseMessages.ValidDatetime;
                return Ok(responcemodel);
            }
            //Guid assetGuid = Guid.Empty;
            string assetGuid = Request.Form["asset_id"];
            string operatorId = Request.Form["user_id"];
            //int inspectionstatus = (int)ResponseStatusNumber.False;
            int assetinspectionatsametime = (int)ResponseStatusNumber.False;
            if (assetGuid != null && operatorId != null)
            {
                assetinspectionatsametime = inspectionService.CheckAssetInspectionByTime(assetGuid, operatorId, Request.Form["requested_datetime"]);
                //inspectionstatus = inspectionService.CheckPendingInspection(assetGuid, operatorId);
            }
            if (assetinspectionatsametime > 0)
            {

                List<IFormFile> filesToUpload = new List<IFormFile>();

                foreach (var file1 in Request.Form.Files)
                {
                    Logger.Log("Images ", file1.FileName.ToString());
                    filesToUpload.Add(file1);
                }

                var list = await s3BucketService.UploadImage(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.inspection_bucket_name);

                Guid siteGuid = Guid.Empty;
                string stringsiteGuid = Request.Form["site_id"];
                InspectionRequestModel inspectionRequestModel = new InspectionRequestModel();
                inspectionRequestModel.asset_id = Guid.Parse(assetGuid);
                inspectionRequestModel.operator_id = Request.Form["user_id"];
                inspectionRequestModel.operator_notes = Request.Form["operator_notes"];
                inspectionRequestModel.company_id = Request.Form["company_id"];
                inspectionRequestModel.site_id = Guid.Parse(stringsiteGuid);
                DateTime createddt = DateTime.ParseExact(Request.Form["created_at"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                inspectionRequestModel.created_at = createddt;
                inspectionRequestModel.modified_at = createddt;
                DateTime requesteddt = DateTime.ParseExact(Request.Form["requested_datetime"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                inspectionRequestModel.datetime_requested = requesteddt;
                inspectionRequestModel.created_by = Request.Form["user_id"];
                inspectionRequestModel.meter_hours = Convert.ToInt64(Request.Form["meter_hours"]);
                inspectionRequestModel.shift = Convert.ToInt32(Request.Form["shift"]);
                inspectionRequestModel.status = Convert.ToInt32(Request.Form["status"]);
                //inspectionRequestModel.status = (int)Status.Approved;
                ImagesListObjectViewModel images = new ImagesListObjectViewModel();
                List<string> images_names = new List<string>();
                foreach (var item in list)
                {
                    images_names.Add(item);
                }
                images.image_names = images_names;

                inspectionRequestModel.image_list = images;

                List<AssetsValueJsonObjectViewModel> attributesJsonObjectViewModel = new List<AssetsValueJsonObjectViewModel>();
                attributesJsonObjectViewModel = JsonConvert.DeserializeObject<List<AssetsValueJsonObjectViewModel>>(Request.Form["attribute_values"]);
                inspectionRequestModel.attribute_values = attributesJsonObjectViewModel.ToArray();
                var responce = await inspectionService.CreateInpsectionOffline(inspectionRequestModel);
                if (responce > 0)
                {
                    responcemodel.success = responce;
                }
                else if (responce == (int)ResponseStatusNumber.NotFound)
                {
                    responcemodel.success = responce;
                    responcemodel.message = ResponseMessages.nodatafound;
                }
                else if (responce == (int)ResponseStatusNumber.DeviceRecordNotFound)
                {
                    responcemodel.success = responce;
                    responcemodel.message = ResponseMessages.DeviceNotFound;
                }
                else
                {
                    responcemodel.success = responce;
                    responcemodel.message = ResponseMessages.Error;
                }
            }
            else
            {
                Logger.Log("Alreadt Offline Inspection Create at : ", requested_datetime.ToString());
                responcemodel.success = assetinspectionatsametime;
                responcemodel.message = ResponseMessages.AlreadyCheckOut;
            }
            //}
            //catch (Exception ex)
            //{
            //    Logger.Log("Error in CreateInspectionOffline ", ex.ToString());
            //    responcemodel.success = (int)ResponseStatusNumber.Error;
            //    responcemodel.message = ResponseMessages.Error;
            //}
            return Ok(responcemodel);
        }

        /// <summary>
        /// Upload Bulk Inspection
        /// </summary>
        [TypeFilter(typeof(ValidationFilterAttribute))]
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadbulkInspection(UploadInspectionRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            var result = await inspectionService.UploadbulkInspection(requestModel);
            responsemodel.data = result;
            responsemodel.success = (int)ResponseStatusNumber.Success;
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in UploadbulkInspection ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Master Data
        /// </summary>
        [HttpGet]
        public IActionResult GetMasterData()
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            var result = inspectionService.GetMasterData();
            responsemodel.data = result;
            responsemodel.success = (int)ResponseStatusNumber.Success;
            //}
            //catch(Exception e)
            //{
            //    Logger.Log("Error in UploadbulkInspection ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //}
            return Ok(responsemodel);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllPendingInspectionForEmail(CancellationToken cancellationToken)
        //{
        //    //inspectionService.SendEmailNotificationForPendingInspection(cancellationToken);
        //    return Ok();
        //}

        /// <summary>
        /// Create Offline Inspection / Update Work Order (Sync)  
        /// </summary>
        [TypeFilter(typeof(ValidationFilterAttribute))]
        [HttpPost]
        public async Task<IActionResult> InspectionIssueOffline([FromBody] OfflineSyncDataRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            InspectionIssueOfflineResponseModel result = await inspectionService.InspectionIssueOffline(requestModel);
            responsemodel.data = result;
            responsemodel.success = (int)ResponseStatusNumber.Success;
            return Ok(responsemodel);
        }

        /// <summary>
        /// Create Offline Inspection / Update Work Order (Sync)  for API
        /// </summary>
        [TypeFilter(typeof(ValidationFilterAttribute))]
        [HttpPost]
        public async Task<IActionResult> InspectionWorkOrderOffline([FromBody] OfflineSyncDataRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            if (requestModel.work_orders?.Count > 0)
            {
                foreach (var item in requestModel.work_orders)
                {
                    item.issue_uuid = item.work_order_uuid;
                }
                requestModel.issues = requestModel.work_orders;
            }
            InspectionIssueOfflineResponseModel result = await inspectionService.InspectionIssueOffline(requestModel);
            responsemodel.data = result;
            responsemodel.success = (int)ResponseStatusNumber.Success;
            return Ok(responsemodel);
        }

        /// <summary>
        /// Upload Images (Offline Image)
        /// </summary>
        [TypeFilter(typeof(ValidationFilterAttribute))]
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadImages()
        {
            Response_Message responsemodel = new Response_Message();
            List<IFormFile> filesToUpload = new List<IFormFile>();
            foreach (var file1 in Request.Form.Files)
            {
                Logger.Log("Images ", file1.FileName.ToString());
                filesToUpload.Add(file1);
            }

            List<string> list = await s3BucketService.UploadOfflineImage(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.inspection_bucket_name, _options.thumbnail_height, _options.thumbnail_width, _options.thumbnail_folder_name);
            responsemodel.data = list;
            responsemodel.success = (int)ResponseStatusNumber.Success;
            return Ok(responsemodel);
        }

        /// <summary>
        /// Send Pending Inspection Email To Manager (Seduler Service)
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(ValidationFilterAttribute))]
        [HttpGet]
        public IActionResult SendPendingInspectionEmail()
        {
            Response_Message responsemodel = new Response_Message();
            inspectionService.SendPendingInspectionEmail();
            responsemodel.success = (int)ResponseStatusNumber.Success;
            return Ok(responsemodel);
        }

        /// <summary>
        /// Send Operator Asset Usage Report Email To Manager (Seduler Service)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SendOperatorAssetUsageReportEmail()
        {
            Response_Message responsemodel = new Response_Message();
            inspectionService.GetOperatorUsageReport();
            responsemodel.success = (int)ResponseStatusNumber.Success;
            return Ok(responsemodel);
        }

        /// <summary>
        /// Send Executive user daily reports
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SendExecutiveDailyReportEmail()
        {
            Response_Message responsemodel = new Response_Message();
            inspectionService.GetExecutiveDailyReport();
            responsemodel.success = (int)ResponseStatusNumber.Success;
            return Ok(responsemodel);
        }

        /// <summary>
        /// Send Executive user Weekly reports
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SendExecutiveWeeklyReportEmail()
        {
            Response_Message responsemodel = new Response_Message();
            inspectionService.GetExecutiveWeeklyReport();
            responsemodel.success = (int)ResponseStatusNumber.Success;
            return Ok(responsemodel);
        }

        /// <summary>
        /// Create Thumbnail Image for Existing Images
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CreateThumbNailImagesFromExisting()
        {
            Response_Message responcemodel = new Response_Message();
            // in case of IFormFile
            //await filesToUpload.CopyToAsync(memoryStream);
            //using (var img = Image.FromStream(memoryStream))
            //{
            //    var thumb = img.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
            //    return thumb;
            //}
            var response = await inspectionService.GetAllInspectionImages(_options.aws_access_key, _options.aws_secret_key, _options.inspection_bucket_name, _options.thumbnail_folder_name,_options.thumbnail_height, _options.thumbnail_width);
            responcemodel.data = response;

            return Ok(responcemodel);
        }
    }
}