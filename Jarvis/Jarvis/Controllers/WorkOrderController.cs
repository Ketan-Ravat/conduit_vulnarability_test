using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Jarvis.Filter;
using Jarvis.Resource;
using Jarvis.Service.Abstract;
using Jarvis.Shared;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using Jarvis.ViewModels.Filter;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.Extensions.Options;
using Jarvis.Service.Concrete;
using Jarvis.Shared.Helper;
using System.Net;
using Jarvis.db.Models;
using MongoDB.Driver.Core.WireProtocol.Messages;
using Carbon.Json;
using Jarvis.Shared.Utility;
using Logger = Jarvis.Shared.Utility.Logger;

namespace Jarvis.Controllers
{
    [TypeFilter(typeof(requestvalidationFilter))]
    //[TypeFilter(typeof(ValidationFilterAttribute))]
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiController]
    public class WorkOrderController : ControllerBase
    {
        private readonly IIssueService issueService;
        private readonly IS3BucketService s3BucketService;
        private readonly IWorkOrderService workorderService;
        private Logger _logger;

        public AWSConfigurations _options { get; set; }
        public WorkOrderController(IIssueService _issueService, IWorkOrderService _workorderService, IOptions<AWSConfigurations> options)
        {
            this.issueService = _issueService;
            this.workorderService = _workorderService;
            this._options = options.Value;
            _logger = Logger.GetInstance<WorkOrderController>();

            this.s3BucketService = new S3BucketService();
        }

        /// <summary>
        /// Get All Work Order
        /// </summary>
        /// <param name="userid" example=""></param>
        /// <param name="status" example=""></param>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Company Admin" })]
        [HttpGet("{userid}/{status?}/{pagesize?}/{pageindex?}/{siteid?}")]
        public IActionResult GetAllWorkOrders(string userid,int status, int pagesize, int pageindex, string siteid) 
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            ListViewModel<IssueResponseModel> response = new ListViewModel<IssueResponseModel>();
            response = issueService.GetAllIssues(status, pagesize, pageindex);
            response.pageIndex = pageindex;
            response.pageSize = pagesize;
            if (response.list != null && response.list.Count > 0)
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
            //    Logger.Log("Error in GetAllAssets ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }
        /// <summary>
        /// Get Work Order By ID 
        /// </summary>
        /// <param name="issue_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Company Admin" })]
        [HttpGet("{userid}/{issue_id}")]
        public async Task<IActionResult> GetWorkOrderDetailsById(string userid,string issue_id)
        {
            Response_Message responsemodel = new Response_Message();
            IssueResponseModel response = new IssueResponseModel();
            response = await issueService.GetIssueByID(issue_id);
            if (response != null && response.issue_uuid != null && response.issue_uuid != Guid.Empty)
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
        /// Get Today Work Orders
        /// </summary>
        /// <param name="userid" example=""></param>
        /// <param name="pagesize" example=""></param>
        /// <param name="pageindex" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Maintenance staff,Company Admin" })]
        [HttpGet("{userid}/{pagesize?}/{pageindex?}")]
        public IActionResult GetTodayNewWorkOrders(string userid,int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<IssueResponseModel> response = new ListViewModel<IssueResponseModel>();
            response = issueService.GetTodayNewIssues(pagesize, pageindex);
            response.pageSize = pagesize;
            response.pageIndex = pageindex;
            if (response.list != null && response.list.Count > 0)
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
            return Ok(responsemodel);
        }

        /// <summary>
        /// Update Work Order Maintenance Staff for old apis
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Maintenance staff,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UpdateWorkOrderByMaintenance([FromBody] UpdateIssueByMaintenanceRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            requestModel.issue_uuid = requestModel.work_order_uuid;
            int result = await issueService.UpdateIssueByMaintenance(requestModel);
            if (result > 0)
            {
                response.success = result;
            }
            else if (result == (int)ResponseStatusNumber.DateRequired)
            {
                response.message = ResponseMessages.ValidDatetime;
                response.success = result;
            }
            else if (result == (int)ResponseStatusNumber.NotFound)
            {
                response.success = result;
                response.message = ResponseMessages.nodatafound;
            }
            else if (result == (int)ResponseStatusNumber.Exceeded)
            {
                response.success = (int)ResponseStatusNumber.False;
                response.message = ResponseMessages.AlreadyApprovedIssue;
            }
            else if (result == (int)ResponseStatusNumber.DeviceRecordNotFound)
            {
                response.success = result;
                response.message = ResponseMessages.DeviceNotFound;
            }
            else
            {
                response.success = result;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// Add Update Work Order
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager" })]
        public async Task<IActionResult> AddUpdateWorkOrder([FromBody] AddWorkOrderRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await workorderService.AddUpdateWorkOrder(request);
                if (result.response_status > 0)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordAdded;
                    responcemodel.data = result;
                }
                else if (result.response_status == (int)ResponseStatusNumber.AlreadyExists)
                {
                    responcemodel.success = (int)ResponseStatusNumber.AlreadyExists;
                    responcemodel.message = ResponseMessages.UserAlreadyExists;
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.Error;
                    responcemodel.message = ResponseMessages.Error;
                }
            }
            else
            {
                BadRequest(ModelState);
                responcemodel.message = ResponseMessages.NotValidModel;
                responcemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responcemodel);
        }

        /// <summary>
        /// Get All Work Orders
        /// </summary>
        /// <param name="filter_type"></param>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager" })]
        public async Task<IActionResult> GetAllWorkOrders([FromBody] GetAllWorkOrderRequestModel filter_type)
        {
            Response_Message responsemodel = new Response_Message();
            var result = await workorderService.GetAllWorkOrder(filter_type);
            if (result?.list?.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = result;
            }
            else if (result.result == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
                responsemodel.message = ResponseMessages.nodatafound;
            }
            else
            {
                responsemodel.success = (int)ResponseStatusNumber.Error;
                responsemodel.message = ResponseMessages.Error;
            }
            return Ok(responsemodel);
        }


        /// <summary>
        /// Get Issues with New status and of same asset
        /// </summary>
        /// <param name="asset_id"></param>
        /// <param name="mr_id"></param>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager" })]
        public async Task<IActionResult> GetNewIssuesListByAssetId(FilterWorkOrderIssueRequestModel filter_type)
        {
            Response_Message responsemodel = new Response_Message();
            var result = await workorderService.GetNewIssuesListByAssetId(filter_type);
            if (result?.list?.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = result;
            }
            else if (result.result == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
                responsemodel.message = ResponseMessages.nodatafound;
            }
            else
            {
                responsemodel.success = (int)ResponseStatusNumber.Error;
                responsemodel.message = ResponseMessages.Error;
            }
            return Ok(responsemodel);
        }


        /// <summary>
        /// Upload Attachments for Work Order
        /// </summary>
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadWorkOrderAttachment()
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

                var list = await s3BucketService.UploadAttachment(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.workorderattachment_bucket_name);

                if (list.Count > 0)
                {
                    var items = new WorkOrderAttachmentsResponseModel
                    {
                        filename = list[0],
                        file_url = UrlGenerator.GetWorkOrderAttachmentURL(list[0]),
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

        /// <summary>
        /// Upload Attachments for Work Order
        /// </summary>
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadOBWOAssetImage ()
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

                //var list = await s3BucketService.UploadAssetImage(filesToUpload, _options.S3_aws_access_key, _options.S3_aws_secret_key, _options.asset_bucket_name);
                var images_list = await s3BucketService.UploadAssetMultipleImage(filesToUpload, _options.S3_aws_access_key, _options.S3_aws_secret_key, _options.asset_bucket_name);
                List<MultipleImageUpload_ResponseModel_class> res = new List<MultipleImageUpload_ResponseModel_class>();
                WorkOrderAttachmentsResponseModel WorkOrderAttachmentsResponseModel = new WorkOrderAttachmentsResponseModel();
                foreach (var img in images_list)
                {
                    if (img != null && img.original_imege_file != null)
                    {
                        var items = new WorkOrderAttachmentsResponseModel
                        {
                            filename = img.original_imege_file,
                            file_url = UrlGenerator.GetAssetImagesURL(img.original_imege_file),
                            thumbnail_filename = img.thumbnail_image_file,
                            thumbnail_file_url = UrlGenerator.GetAssetImagesURL(img.thumbnail_image_file),
                            user_uploaded_name = img.user_uploaded_filename
                        };
                        var listitem = new MultipleImageUpload_ResponseModel_class
                        {
                            filename = img.original_imege_file,
                            file_url = UrlGenerator.GetAssetImagesURL(img.original_imege_file),
                            thumbnail_filename = img.thumbnail_image_file,
                            thumbnail_file_url = UrlGenerator.GetAssetImagesURL(img.thumbnail_image_file),
                            user_uploaded_name = img.user_uploaded_filename
                        };

                        responsemodel.success = 1;
                        res.Add(listitem);
                        WorkOrderAttachmentsResponseModel = items;
                        //return Ok(responsemodel);
                    }
                    /*else
                    {
                        responsemodel.success = 0;
                        Res_ErrorMessage mess = new Res_ErrorMessage(Resource.ResponseMessages.UPLOADFILE_INVALIDFORMAT, Resource.ResponseMessages.UPLOADFILE_TITLE);
                        responsemodel.data = mess;
                        return Ok(responsemodel);
                    }*/
                }
                WorkOrderAttachmentsResponseModel.image_list = res;
                responsemodel.data = WorkOrderAttachmentsResponseModel;
                return Ok(responsemodel);
                /*if (list != null && list.original_imege_file != null)
                {
                    var items = new WorkOrderAttachmentsResponseModel
                    {
                        filename = list.original_imege_file,
                        file_url = UrlGenerator.GetAssetImagesURL(list.original_imege_file),
                        thumbnail_filename = list.thumbnail_image_file,
                        thumbnail_file_url = UrlGenerator.GetAssetImagesURL(list.thumbnail_image_file),
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
                }*/
            }
            else
            {
                responsemodel.success = 0;
                responsemodel.data = "No file selected!!!";
                return Ok(responsemodel);
            }
        }


        /// <summary>
        /// Work Order Status History
        /// </summary>
        /// <param name="wo_id"></param>
        [HttpGet("{wo_id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager" })]
        public async Task<IActionResult> WorkOrderStatusHistory(Guid wo_id)
        {
            Response_Message responsemodel = new Response_Message();
            var result = await workorderService.WorkOrderStatusHistory(wo_id);
            if (result?.list?.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = result;
            }
            else if (result.result == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
                responsemodel.message = ResponseMessages.nodatafound;
            }
            else
            {
                responsemodel.success = (int)ResponseStatusNumber.Error;
                responsemodel.message = ResponseMessages.Error;
            }
            return Ok(responsemodel);
        }


        /// <summary>
        /// Resolve maintenance Request
        /// </summary>
        /// <param name="wo_id"></param>
        [HttpGet("{wo_id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager" })]
        public async Task<IActionResult> DeleteWorkOrder(Guid wo_id)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await workorderService.DeleteWorkOrder(wo_id);
                if (result > 0)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordUpdated;
                    responcemodel.data = result;
                }
                else if (result == (int)ResponseStatusNumber.InvalidData)
                {
                    responcemodel.success = (int)ResponseStatusNumber.InvalidData;
                    responcemodel.message = ResponseMessages.Error;
                    responcemodel.data = result;
                }
                else if (result == (int)ResponseStatusNumber.NotFound)
                {
                    responcemodel.success = (int)ResponseStatusNumber.NotFound;
                    responcemodel.message = ResponseMessages.nodatafound;
                    responcemodel.data = result;
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.Error;
                    responcemodel.message = ResponseMessages.Error;
                }
            }
            else
            {
                BadRequest(ModelState);
                responcemodel.message = ResponseMessages.NotValidModel;
                responcemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responcemodel);
        }

        /// <summary>
        /// Get All Work Order With Filter Title Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> FilterWorkOrderTitleOptions([FromBody] FilterWorkOrderOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<WorkOrderResponseModel> response = new ListViewModel<WorkOrderResponseModel>();
            response = workorderService.FilterWorkOrderTitleOption(requestModel);
            if (response?.list?.Count > 0)
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
        /// Get All Work Order With Filter Number Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> FilterWorkOrderNumberOptions([FromBody] FilterWorkOrderOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<WorkOrderResponseModel> response = new ListViewModel<WorkOrderResponseModel>();
            response = workorderService.FilterWorkOrderNumberOption(requestModel);
            if (response?.list?.Count > 0)
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
        /// Get Work Order By Id
        /// </summary>
        /// <param name="wo_id"></param>
        [HttpGet("{wo_id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager" })]
        public async Task<IActionResult> GetWorkOrderById(Guid wo_id)
        {
            Response_Message responsemodel = new Response_Message();
            var result = await workorderService.GetWorkOrderById(wo_id);
            if (result.response_status > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = result;
            }
            else
            {
                responsemodel.success = (int)ResponseStatusNumber.Error;
                responsemodel.message = ResponseMessages.Error;
            }
            return Ok(responsemodel);
        }



        //-----------------------------------  new requirement workorder implementation ------------------------------------//
        /// <summary>
        /// Get All Work Order for new requirement
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public IActionResult GetAllWorkOrdersNewflow(NewFlowWorkorderListRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            ListViewModel<NewFlowWorkorderListResponseModel> response = new ListViewModel<NewFlowWorkorderListResponseModel>();
            response = workorderService.GetAllWorkOrdersNewflow(requestModel);
            response.pageIndex = requestModel.pageindex;
            response.pageSize = requestModel.pagesize;
            if (response.list != null && response.list.Count > 0)
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
            //    Logger.Log("Error in GetAllAssets ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get All Work Order for new requirement
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public IActionResult GetAllWorkOrdersNewflowOptimized(NewFlowWorkorderListRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            ListViewModel<GetAllWorkOrdersNewflowOptimizedResponsemodel> response = new ListViewModel<GetAllWorkOrdersNewflowOptimizedResponsemodel>();
            response = workorderService.GetAllWorkOrdersNewflowOptimized(requestModel);
            response.pageIndex = requestModel.pageindex;
            response.pageSize = requestModel.pagesize;
            if (response.list != null && response.list.Count > 0)
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
            //    Logger.Log("Error in GetAllAssets ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> CreateWorkorderNewflow(NewFlowCreateWORequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            var response  =  await workorderService.CreateWorkorderNewflow(requestModel , _options.S3_aws_access_key, _options.S3_aws_secret_key);
            if (response !=null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
                if (response.response_status == (int)ResponseStatusNumber.duplicate_wo_number)
                {
                    responsemodel.success = (int)ResponseStatusNumber.duplicate_wo_number;
                    responsemodel.data = null;
                    responsemodel.message = "Duplicate WO number";
                }
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetAllAssets ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> GetAllCatagoryForWO(GetAllCatagoryForWORequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            var response =  workorderService.GetAllCatagoryForWO(requestModel);
            if (response != null && response.list != null && response.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetAllAssets ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AssignCategorytoWO(AssignCategorytoWORequestmodel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            var response = await workorderService.AssignCategorytoWO(requestModel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = null;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetAllAssets ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AssignMultipleAssetClasstoWO(AssignMultipleAssetClasstoWORequestmodel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            var response = await workorderService.AssignMultipleAssetClasstoWO(requestModel);
            if (response!=null && response.success == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = null;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else if (response != null && response.success == (int)ResponseStatusNumber.class_not_having_form)
            {
                responsemodel.success = (int)ResponseStatusNumber.class_not_having_form;
                responsemodel.data = null;
                responsemodel.message = response.assset_class +  " " + "Does not have Maintenance Form Assigned.";
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetAllAssets ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AssignAssetClasstoWO(AssignAssetClasstoWORequestmodel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            var response = await workorderService.AssignAssetClasstoWO(requestModel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetAllAssets ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }
        /// <summary>
        /// View Work Order By ID 
        /// </summary>
        /// <param name="wo_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{wo_id}")]
        public IActionResult ViewWorkOrderDetailsById(string wo_id)
        {
            Response_Message responsemodel = new Response_Message();
            ViewWorkOrderDetailsByIdResponsemodel response = new ViewWorkOrderDetailsByIdResponsemodel();
            response =  workorderService.ViewWorkOrderDetailsById(wo_id);
            if (response != null && response.wo_id != null && response.wo_id != Guid.Empty)
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
        /// View Work Order By ID 
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetAllTechnician(GetAllTechnicianRequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<GetAllTechnicianResponsemodel> response = new ListViewModel<GetAllTechnicianResponsemodel>();
            response = workorderService.GetAllTechnician(requestmodel);
            if (response != null && response.list != null && response.list.Count>0)
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
        /// View Work Order By ID 
        /// </summary>
        /// <param name="wo_inspectionsTemplateFormIOAssignment_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{wo_inspectionsTemplateFormIOAssignment_id}")]
        public IActionResult GetWOcategoryTaskByCategoryID(string wo_inspectionsTemplateFormIOAssignment_id)
        {
            Response_Message responsemodel = new Response_Message();
            //GetWOcategoryTaskByCategoryIDResponsemodel response = new GetWOcategoryTaskByCategoryIDResponsemodel();
            var response = workorderService.GetWOcategoryTaskByCategoryID(wo_inspectionsTemplateFormIOAssignment_id);
            if (response != null && response.Count> 0)
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
        /// View Work Order By ID 
        /// </summary>
        /// <param name="wo_inspectionsTemplateFormIOAssignment_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{wo_inspectionsTemplateFormIOAssignment_id}")]
        public IActionResult GetWOGridView(string wo_inspectionsTemplateFormIOAssignment_id)
        {
            Response_Message responsemodel = new Response_Message();
            //GetWOcategoryTaskByCategoryIDResponsemodel response = new GetWOcategoryTaskByCategoryIDResponsemodel();
            var response = workorderService.GetWOGridView(wo_inspectionsTemplateFormIOAssignment_id);
            if (response != null && response.task_list!=null && response.task_list.Count> 0)
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
        /// View Work Order By ID 
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AssignTechniciantoWOcategory(AssignTechniciantoWOcategoryRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.AssignTechniciantoWOcategory(requestmodel);
            if (response >0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
        
            return Ok(responsemodel);
        }

        /// <summary>
        /// View Work Order By ID 
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AssignAssettoWOcategoryTask(AssignAssettoWOcategoryRequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.AssignAssettoWOcategory(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// View Work Order By ID 
        /// </summary>
        /// <param name="issue_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{WOcategorytoTaskMapping_id}")]
        public IActionResult GetFormByWOTaskID(string WOcategorytoTaskMapping_id)
        {
            Response_Message responsemodel = new Response_Message();
            //GetWOcategoryTaskByCategoryIDResponsemodel response = new GetWOcategoryTaskByCategoryIDResponsemodel();
            var response = workorderService.GetFormByWOTaskID(WOcategorytoTaskMapping_id);
            if (response != null)
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
        /// View Work Order By ID 
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UpdateWOCategoryTaskStatus(UpdateWOCategoryTaskRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.UpdateWOCategoryTask(requestmodel , _options.aws_access_key , _options.aws_secret_key,_options.formio_pdf_bucket);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// View Work Order By ID 
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UpdateMultiWOCategoryTaskStatus(UpdateMultiWOCategoryTaskStatusRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.UpdateMultiWOCategoryTaskStatus(requestmodel, _options.aws_access_key, _options.aws_secret_key);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// update status of WOCategory
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UpdateWOCategoryStatus(UpdateWOCategoryRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.UpdateWOCategoryStatus(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// update WO status
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UpdateWOStatus(UpdateWOStatusRequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.UpdateWOStatus(requestmodel);
            if (response.success == 1)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else if (response.success > 1)
            {
                responsemodel.success = (int)ResponseStatusNumber.Error;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.IssueWithTask + response.ToString();
            }
            else if (response.success == (int)ResponseStatusNumber.internal_asset_id_must_be_unique)
            {
                responsemodel.success = (int)ResponseStatusNumber.internal_asset_id_must_be_unique;
                responsemodel.data = response;
               /// responsemodel.message =  ResponseMessages.InternalAssetIDmustbeUnique.Replace("{taskid}" , response.task_id.ToString());
                responsemodel.message = response.asset_id + "Asset ID must be unique";;
            }
            else if (response.success == (int)ResponseStatusNumber.identification_must_be_unique)
            {
                responsemodel.success = (int)ResponseStatusNumber.identification_must_be_unique;
                responsemodel.data = response;
             //   responsemodel.message = ResponseMessages.IdentificationMustbeunique.Replace("{taskid}", response.task_id.ToString());
                responsemodel.message = response.asset_id + "Identification Name must be unique";
            }
            else if (response.success == (int)ResponseStatusNumber.identification_must_be_unique)
            {
                responsemodel.success = (int)ResponseStatusNumber.identification_must_be_unique;
                responsemodel.data = response;
                responsemodel.message = response.asset_id + "must be  unique";
            }
            else if (response.success == (int)ResponseStatusNumber.qr_code_must_be_unique)
            {
                responsemodel.success = (int)ResponseStatusNumber.qr_code_must_be_unique;
                responsemodel.data = response;
                responsemodel.message = response.asset_id + "QRcode must be unique";
            }
            else if (response.success == (int)ResponseStatusNumber.fed_by_wo_ines_must_be_completes)
            {
                responsemodel.success = (int)ResponseStatusNumber.fed_by_wo_ines_must_be_completes;
                responsemodel.data = response;
                responsemodel.message = response.asset_id + " WO line must be complete to complete this Work order";
            }
            else if (response.success == (int)ResponseStatusNumber.compoentlevel_wo_ines_must_be_completes)
            {
                responsemodel.success = (int)ResponseStatusNumber.compoentlevel_wo_ines_must_be_completes;
                responsemodel.data = response;
                responsemodel.message = response.asset_id + " WO line must be complete to complete this Work order";
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// update status of WOCategory
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> MapWOAttachmenttoWO(MapWOAttachmenttoWORequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.MapWOAttachmenttoWO(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// update status of WOCategory
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> DeleteWOAttachment(DeleteWOAttachmentRequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.DeleteWOAttachment(requestmodel);
            if (response > 0)
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

        /// <summary>
        /// Multi Copy Lines
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> MultiCopyWOTask(MultiCopyWOTaskRequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.MultiCopyWOTask(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Delete WO category
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> DeleteWOCategory(DeleteWOCategoryRequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.DeleteWOCategory(requestmodel);
            if (response > 0)
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

        /// <summary>
        /// Delete WO category
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> DeleteWOCategoryTask(DeleteWOCategoryTaskRequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.DeleteWOCategoryTask(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else if (response == (int)ResponseStatusNumber.can_not_delete_parent_task)
            {
                responsemodel.message = ResponseMessages.can_not_delete_parent_task;
                responsemodel.success = (int)ResponseStatusNumber.can_not_delete_parent_task;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Delete WO category
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetWOCalenderEvents(GetWOCalenderEventsRequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response =  workorderService.GetWOCalenderEvents(requestmodel);
            if (response != null && response.Count>0)
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

        /// <summary>
        /// View Work Order By ID 
        /// </summary>
        /// <param name="wo_inspectionsTemplateFormIOAssignment_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{wo_id}/{status?}")]
        public IActionResult GetAllWOCategoryTaskByWOid(string wo_id,int status = 0)
        {
            Response_Message responsemodel = new Response_Message();
            //GetWOcategoryTaskByCategoryIDResponsemodel response = new GetWOcategoryTaskByCategoryIDResponsemodel();
            var response = workorderService.GetAllWOCategoryTaskByWOid(wo_id,status);
            if (response != null &&  response.Count > 0)
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
        /// View Work Order By ID 
        /// </summary>
        /// <param name="wo_inspectionsTemplateFormIOAssignment_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet()]
        public IActionResult GetWOBacklogCardList([FromQuery]string search_string)
        {
            Response_Message responsemodel = new Response_Message();
            //GetWOcategoryTaskByCategoryIDResponsemodel response = new GetWOcategoryTaskByCategoryIDResponsemodel();
            var response = workorderService.GetWOBacklogCardList(search_string,null);
            if (response != null)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetWOBacklogCardList_V2(GetWOBacklogCardList_V2RequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            //GetWOcategoryTaskByCategoryIDResponsemodel response = new GetWOcategoryTaskByCategoryIDResponsemodel();
            var response = workorderService.GetWOBacklogCardList(requestModel.search_string, requestModel.site_id);
            if (response != null)
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
        /// Delete WO category
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UploadQuote(UploadQuoteRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.UploadQuote(requestmodel);
            if (response != null && response.success > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else if (response != null && response.success == (int)ResponseStatusNumber.no_form_available)
            {
                responsemodel.message = ResponseMessages.FormnotExist.Replace("{form_name}", response.form_name);
                responsemodel.success = response.success;
            }
            else if (response != null && response.success ==  (int)ResponseStatusNumber.form_have_no_task)
            {
                responsemodel.message = ResponseMessages.FormDontHaveTask.Replace("{form_name}", response.form_name);
                responsemodel.success = response.success;
            }
            else if (response != null && response.success == (int)ResponseStatusNumber.internal_asset_id_must_be_unique)
            {
                responsemodel.message = ResponseMessages.AssetMustbeUnique.Replace("{Identification}", response.form_name);
                responsemodel.success = response.success;
            }
            else if (response != null && response.success == (int)ResponseStatusNumber.draft_form)
            {
                responsemodel.message = response.form_name + " Is Draft Form";
                responsemodel.success = response.success;
            }
            else if (response != null && response.success == (int)ResponseStatusNumber.asset_class_not_found)
            {
                responsemodel.message = response.asset_class_code + " Asset Class not found";
                responsemodel.success = response.success;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }
        /// <summary>
        /// Get Sync Data
        /// </summary>
        /// <param name="userid" example="a343180f-6474-42fd-a486-9f9491d91a43"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{userid}")]
        public async Task<IActionResult> GetWOsForOffline(string userid)
        {
            Response_Message response = new Response_Message();  // true for remove NFPA issue for backward compatible
            GetWOsForOfflineResponsemodel responseModels = await  workorderService.GetWOsForOffline(userid,true);
            if (responseModels != null)
            {
                response.data = responseModels;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }
        /// </summary>
        /// <param name="userid" example="a343180f-6474-42fd-a486-9f9491d91a43"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{userid}")]
        public async Task<IActionResult> GetWOsForOffline_V2(string userid)
        {
            Response_Message response = new Response_Message();    // false new flow to show NFPA issue
            GetWOsForOfflineResponsemodel responseModels = await workorderService.GetWOsForOffline(userid,false);
            if (responseModels != null)
            {
                response.data = responseModels;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }
        /// <summary>
        /// View Work Order By ID 
        /// </summary>
        /// <param name="wo_id" example=""></param>
       // [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{wo_id}")]
        public IActionResult WorkOrderDetailsByIdForExportPDF(string wo_id)
        {
            Response_Message responsemodel = new Response_Message();
            WorkOrderDetailsByIdForExportPDFResponsemodel response = new WorkOrderDetailsByIdForExportPDFResponsemodel();
            response = workorderService.WorkOrderDetailsByIdForExportPDF(wo_id);
            if (response != null && response.wo_id != null && response.wo_id != Guid.Empty)
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
        /// copy fields from form
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> CopyFieldsFromForm(CopyFieldsFromFormRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.CopyFieldsFromForm(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else if (response == (int)ResponseStatusNumber.nameplate_info_not_found)
            {
                responsemodel.message = "No NameplateInfo fields are set for Form";
                responsemodel.success = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Add Update Asset Form IO
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> UpdateWOOffline([FromBody] UpdateWOOfflineRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await workorderService.UpdateWOOffline(request , _options.sqs_aws_access_key, _options.sqs_aws_secret_key , _options.offline_sync_bucket , _options.S3_aws_access_key, _options.S3_aws_secret_key);
                if (result!=null &&  result.success > 0)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordAdded;
                    if(request.is_lambda_enable)
                        responcemodel.data = result; // if lambda is enable then send object
                    else
                        responcemodel.data = result.success;// if lambda is desable then send int.
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.Error;
                    responcemodel.message = ResponseMessages.Error;
                }
            }
            else
            {
                BadRequest(ModelState);
                responcemodel.message = ResponseMessages.NotValidModel;
                responcemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responcemodel);
        }
        /// <summary>
        /// Add Update Asset Form IO
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public  IActionResult GetOfflineSyncLambdaStatus([FromBody] GetOfflineSyncLambdaStatusRequestmodel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result =  workorderService.GetOfflineSyncLambdaStatus(request);
                if (result != null )
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordAdded;
                    responcemodel.data = result;
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.Error;
                    responcemodel.message = ResponseMessages.Error;
                }
            }
            else
            {
                BadRequest(ModelState);
                responcemodel.message = ResponseMessages.NotValidModel;
                responcemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responcemodel);
        }

        /// <summary>
        /// Add Update Asset Form IO
        /// </summary>
        [HttpPost]
        //[TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> UpdateWOOfflineAfterLambdaExecution([FromBody] UpdateWOOfflineRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await workorderService.UpdateWOOfflineAfterLambdaExecution(request, _options.sqs_aws_access_key, _options.sqs_aws_secret_key);
                if (result > 0)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordAdded;
                    responcemodel.data = result;
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.Error;
                    responcemodel.message = ResponseMessages.Error;
                }
            }
            else
            {
                BadRequest(ModelState);
                responcemodel.message = ResponseMessages.NotValidModel;
                responcemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responcemodel);
        }

        /// <summary>
        /// View Work Order By ID 
        /// </summary>
        /// <param name="wo_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet()]
        public IActionResult GetAssetBuildingHierarchy()
        {
            Response_Message responsemodel = new Response_Message();
            GetAssetBuildingHierarchyResponsemodel response = new GetAssetBuildingHierarchyResponsemodel();
            response = workorderService.GetAssetBuildingHierarchy();
            if (response != null)
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
        /// View Work Order By ID 
        /// </summary>
        /// <param name="wo_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{wo_id}")]
        public IActionResult GetAssetBuildingHierarchyByWorkorder(string wo_id)
        {
            Response_Message responsemodel = new Response_Message();
            GetAssetBuildingHierarchyResponsemodel response = new GetAssetBuildingHierarchyResponsemodel();
            response = workorderService.GetAssetBuildingHierarchyByWorkorder(wo_id);
            if (response != null)
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
        /// copy fields from form
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> DeleteWO(DeleteWORequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.DeleteWO(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// View Work Order By ID 
        /// </summary>
        /// <param name="wo_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{form_id}")]
        public IActionResult GetAssetsToAssign(string form_id)
        {
            Response_Message responsemodel = new Response_Message();
            //GetAssetBuildingHierarchyResponsemodel response = new GetAssetBuildingHierarchyResponsemodel();
            var response = workorderService.GetAssetsToAssign(Guid.Parse(form_id));
            if (response != null)
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
        /// copy fields from form
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> GetAssetclassFormToAddcategory(GetAssetclassFormToAddcategoryRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response =  workorderService.GetAssetclassFormToAddcategory(requestmodel);
            if (response != null)
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

        /// <summary>
        /// Asset List to add in Maintennace WO
        /// </summary>
        /// <param name="wo_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{wo_type}")]
        public IActionResult GetAssetsToAssigninWO(string wo_type)
        {
            Response_Message responsemodel = new Response_Message();
            //GetAssetBuildingHierarchyResponsemodel response = new GetAssetBuildingHierarchyResponsemodel();
            var response = workorderService.GetAssetsToAssigninWO(int.Parse(wo_type));
            if (response != null)
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
        /// Asset List to add in Maintennace WO
        /// </summary>
        /// <param name="wo_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost()]
        public IActionResult GetAssetsToAssigninMWOInspection(GetAssetsToAssigninMWOInspectionRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            //GetAssetBuildingHierarchyResponsemodel response = new GetAssetBuildingHierarchyResponsemodel();
            var response = workorderService.GetAssetsToAssigninMWOInspection(requestmodel);
            if (response != null)
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
        /// copy fields from form
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive" })]
        [HttpPost]
        public async Task<IActionResult> UploadAssettoOBWO(UploadAssettoOBWORequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.UploadAssettoOBWO(requestmodel);
            if (response.respose == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else if(response.respose == (int)ResponseStatusNumber.AlreadyExists)
            {
                responsemodel.message = " Duplicate asset found " +  response.asset_name;
                responsemodel.success = (int)ResponseStatusNumber.AlreadyExists;
            }
            else if (response.respose == (int)ResponseStatusNumber.InvalidData)
            {
                responsemodel.message = "No data found!";
                responsemodel.success = (int)ResponseStatusNumber.InvalidData;
            }
            else if (response.respose == (int)ResponseStatusNumber.asset_class_not_found)
            {
                responsemodel.message = "Asset-Class not found!";
                responsemodel.success = (int)ResponseStatusNumber.asset_class_not_found;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// View Work Order By ID 
        /// </summary>
        /// <param name="wo_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{wo_id}")]
        public IActionResult ViewOBWODetailsById(string wo_id)
        {
            Response_Message responsemodel = new Response_Message();
            ViewOBWODetailsByIdResponsemodel response = new ViewOBWODetailsByIdResponsemodel();
            response = workorderService.ViewOBWODetailsById(wo_id);
            if (response != null && response.wo_id != null && response.wo_id != Guid.Empty)
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
        /// View Work Order By ID 
        /// </summary>
        /// <param name="wo_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{woonboardingassets_id}")]
        public IActionResult GetOBWOAssetDetailsById(string woonboardingassets_id)
        {
            Response_Message responsemodel = new Response_Message();
            GetOBWOAssetDetailsByIdResponsemodel response = new GetOBWOAssetDetailsByIdResponsemodel();
            response = workorderService.GetOBWOAssetDetailsById(woonboardingassets_id);

            if (response != null && response.wo_id != null && response.wo_id != Guid.Empty)
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

        /// </summary>
        /// <param name="wo_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{woonboardingassets_id}")]
        public IActionResult GetOBWOAssetDetailsById_V2(string woonboardingassets_id)
        {
            Response_Message responsemodel = new Response_Message();
            GetOBWOAssetDetailsByIdResponsemodel response = new GetOBWOAssetDetailsByIdResponsemodel();
            response = workorderService.GetOBWOAssetDetailsById_V2(woonboardingassets_id);

            if (response != null && response.wo_id != null && response.wo_id != Guid.Empty)
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
        /// update Asset in OBWO
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Technician,Company Admin,Executive" })]
        [HttpPost]
        public async Task<IActionResult> UpdateOBWOAssetDetails(UpdateOBWOAssetDetailsRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.UpdateOBWOAssetDetails(requestmodel);
            if (response.respose == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else if (response.respose == (int)ResponseStatusNumber.AlreadyExists)
            {
                responsemodel.message = response.asset_name + " Asset Already Exist In system";
                responsemodel.success = (int)ResponseStatusNumber.AlreadyExists;
            }
            else if (response.respose == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            else if (response.respose == (int)ResponseStatusNumber.SubLevel_Fedby_is_NotAllowed)
            {
                responsemodel.message = "SubLevel Asset is Not Allowed for FedBy";
                responsemodel.success = (int)ResponseStatusNumber.SubLevel_Fedby_is_NotAllowed;
            }
            else if (response.respose == (int)ResponseStatusNumber.can_not_change_top_to_sub)
            {
                responsemodel.message = "Please delete Sub-level mapping to change asset's component level from Top-level to Sub-level";
                responsemodel.success = (int)ResponseStatusNumber.SubLevel_Fedby_is_NotAllowed;
            }
            else if (response != null && response.respose == (int)ResponseStatusNumber.qr_code_must_be_unique)
            {
                responsemodel.success = (int)ResponseStatusNumber.qr_code_must_be_unique;
                responsemodel.message = response.asset_name;
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
        /// update Asset in OBWO
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Technician,Company Admin,Executive" })]
        [HttpPost]
        public async Task<IActionResult> DeleteOBWOAsset(DeleteOBWOAssetRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.DeleteOBWOAsset(requestmodel);
            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else if (response == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            else if (response == (int)ResponseStatusNumber.can_not_delete_fed_by_wo_line)
            {
                responsemodel.message = ResponseMessages.CannotDeleteFedByWOLine;
                responsemodel.success = (int)ResponseStatusNumber.can_not_delete_fed_by_wo_line;
            }
            else if (response == (int)ResponseStatusNumber.can_not_delete_sublevel_woline)
            {
                responsemodel.message = "Can not Delete Sublevel WOLine";
                responsemodel.success = (int)ResponseStatusNumber.can_not_delete_sublevel_woline;
            }
            else if (response == (int)ResponseStatusNumber.can_not_delete_toplevel_woline)
            {
                responsemodel.message = "Can not Delete Toplevel WOLine";
                responsemodel.success = (int)ResponseStatusNumber.can_not_delete_toplevel_woline;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// update WO status
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UpdateOBWOStatus(UpdateOBWOStatusRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.UpdateOBWOStatus(requestmodel);
            if (response.success == 1)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else if (response.success > 1)
            {
                responsemodel.success = (int)ResponseStatusNumber.Error;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.IssueWithTask + response.ToString();
            }
            else if (response.success == (int)ResponseStatusNumber.identification_must_be_unique)
            {
                responsemodel.success = (int)ResponseStatusNumber.identification_must_be_unique;
                responsemodel.data = response;
                responsemodel.message = response.asset_name + " must be  unique";
            }
            else if (response.success == (int)ResponseStatusNumber.qr_code_must_be_unique)
            {
                responsemodel.success = (int)ResponseStatusNumber.qr_code_must_be_unique;
                responsemodel.data = response;
                responsemodel.message = response.asset_name + " must be unique";
            }
            else if (response.success == (int)ResponseStatusNumber.fed_by_wo_ines_must_be_completes)
            {
                responsemodel.success = (int)ResponseStatusNumber.fed_by_wo_ines_must_be_completes;
                responsemodel.data = response;
                responsemodel.message = response.asset_name + " WO line must be complete to complete this Work order";
            }
            else if (response.success == (int)ResponseStatusNumber.compoentlevel_wo_ines_must_be_completes)
            {
                responsemodel.success = (int)ResponseStatusNumber.compoentlevel_wo_ines_must_be_completes;
                responsemodel.data = response;
                responsemodel.message = response.asset_name + " WO line must be complete to complete this Work order";
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }


        /// <summary>
        /// delete S3 objects
        /// </summary>
        /// <param name="requestmodel"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Technician,Company Admin,Executive" })]
        [HttpPost]
        public async Task<IActionResult> DeleteAWSS3Object(DeleteAWSS3ObjectRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            string bucket_name = null;
            if (requestmodel.bucket_type == (int)AWSBucketsEnums.IR_Visual_Images)
            {
                bucket_name = _options.IR_photos_bucket_name;
            }
            else if (requestmodel.bucket_type == (int)AWSBucketsEnums.Asset_images)
            {
                bucket_name = _options.asset_bucket_name;
            }

            List<string> file_path_list = new List<string>();
            if (requestmodel.bucket_type == (int)AWSBucketsEnums.IR_Visual_Images)
            {
                requestmodel.file_name.ForEach(obj =>
                {
                    // string baseUrl = "https://s3-us-east-2.amazonaws.com//" + bucket_name + "/";
                    file_path_list.Add(obj);// obj.Substring(baseUrl.Length));
                });
            }
            else if (requestmodel.bucket_type == (int)AWSBucketsEnums.Asset_images)
            {
                file_path_list = requestmodel.file_name;
            }

            var list = await s3BucketService.deleteObjects(file_path_list, _options.S3_aws_access_key, _options.S3_aws_secret_key, bucket_name);

            if (list != null && list.HttpStatusCode == HttpStatusCode.OK)
            {
                responsemodel.success = 1;
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


        /// <summary>
        /// update WO status
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UpdateOBWOAssetStatus(UpdateOBWOAssetStatusRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.UpdateOBWOAssetStatus(requestmodel);
            if (response!=null && response.success == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else if (response!=null && response.success == (int)ResponseStatusNumber.qr_code_must_be_unique)
            {
                responsemodel.success = (int)ResponseStatusNumber.qr_code_must_be_unique;
                responsemodel.data = response;
                responsemodel.message = response.qr_code +  " QRcode must be unique";
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> ExportCompletedAssetsByWO(ExportCompletedAssetsByWORequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response =  workorderService.ExportCompletedAssetsByWO(requestmodel);
            if (response !=null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message =null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UpdateHierarchyandLevelForPythonscript(UpdateHierarchyandLevelForPythonscriptRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.UpdateHierarchyandLevelForPythonscript(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Upload Asset IR Photots
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Executive,Manager,Technician" })]
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadIRPhotos()
        {
            Response_Message response = new Response_Message();
            //try
            //{
            List<IFormFile> filesToUpload = new List<IFormFile>();
            List<string> file_names = new List<string>();
            foreach (var file1 in Request.Form.Files)
            {
                //Logger.Log("Images ", file1.FileName.ToString());
                filesToUpload.Add(file1);
                file_names.Add(file1.FileName);
            }
            string stringwonumber = Request.Form["manual_wo_number"];
            string stringwoid = Request.Form["wo_id"];
            string img_type = Request.Form["ir_visual_image_type"];
            string string_site_id = UpdatedGenericRequestmodel.CurrentUser.site_id;

            //check camera type and ir_image_type 
            var flag = workorderService.GetIRWOCameraTypeFlags(Guid.Parse(stringwoid));
            var camera_type = flag.Item1; var ir_image_type = 0;
            
            if (flag.Item2 != null)
                ir_image_type = flag.Item2.Value;
            else if (img_type == "1")
                ir_image_type = (int)ir_visual_image_type.IR_Image_Only;
            else if (img_type == "2")
                ir_image_type = (int)ir_visual_image_type.Visual_Image_Only;

            // get file path of file from db
            var get_file_path = workorderService.GetImagesFilePaths(file_names, stringwoid);

            var list = await s3BucketService.UploadIRPhotos(filesToUpload, _options.S3_aws_access_key, _options.S3_aws_secret_key, _options.IR_photos_bucket_name, stringwoid, string_site_id, get_file_path, camera_type,ir_image_type);

            var add_images_to_table = await workorderService.AddIRImage(filesToUpload , stringwonumber, stringwoid , string_site_id);


            // if cam is FLIR & IR only img then call lamda function
            if (camera_type == (int)ir_visual_camera_type.FLIR && ir_image_type == (int)ir_visual_image_type.IR_Image_Only)
            {
                GenerateVisualImageFromFLIRCameraRequestmodel GenerateVisualImageFromFLIRCameraRequestmodel = new GenerateVisualImageFromFLIRCameraRequestmodel();
                GenerateVisualImageFromFLIRCameraRequestmodel.file_key_lst = list;

                string jsonString = System.Text.Json.JsonSerializer.Serialize(GenerateVisualImageFromFLIRCameraRequestmodel);
                await AssetFornioInspectionReport.GenerateVisualImageFromFLIRCamera(_options.sqs_aws_access_key, _options.sqs_aws_secret_key, jsonString, _logger);
            }

            // change file extention if its differnet
            var change_file_extention = await workorderService.ChangeIRPhotosExtention(list, stringwoid);

            // change file extension in IR PM if its differnet
            var change_IR_PM_file_extention = await workorderService.ChangeIRPMPhotosExtention(list, stringwoid);

            if (list.Count() > 0)
            {
                response.data = list;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in UploadAssetPhoto ", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AddOBFedByAsset(AddOBFedByAssetRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.AddOBFedByAsset(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> GetOBFedByAssetList(GetOBFedByAssetListRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.GetOBFedByAssetList(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetIRScanImagesFiles(GetIRScanImagesFilesRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response =  workorderService.GetIRScanImagesFiles(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }


        /// <summary>
        /// Generate Asset form io Inspection Report
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpPost]
        public async Task<IActionResult> GenerateAssetInspectionFormioReport([FromBody] GenerateAssetInspectionFormioReportRequestmodel requestModel)
        {
            Response_Message response = new Response_Message();

            string aws_access_key = _options.sqs_aws_access_key;
            string aws_secret_key = _options.sqs_aws_secret_key;
            var responseModels = await workorderService.GenerateAssetInspectionFormioReport(requestModel, aws_access_key, aws_secret_key, _options.formio_pdf_bucket);
            if (responseModels !=null && responseModels.success == (int)ResponseStatusNumber.Success)
            {
                response.data = responseModels;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else if (responseModels != null && responseModels.success == (int)ResponseStatusNumber.NotFound)
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = "Report Template Could not be found. Please contact Support!";
            }
            else 
            {
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// View Work Order By ID 
        /// </summary>
        /// <param name="issue_id" example=""></param>
        [HttpGet("{asset_form_id}")]
        public IActionResult GetFormJsonForLambda(string asset_form_id)
        {
            Response_Message responsemodel = new Response_Message();
            //GetWOcategoryTaskByCategoryIDResponsemodel response = new GetWOcategoryTaskByCategoryIDResponsemodel();
            var response = workorderService.GetFormJsonForLambda(asset_form_id);
            if (response != null)
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
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [HttpPost]
        public async Task<IActionResult> UpdateWOlinePDFurlfromLambda(UpdateWOlinePDFurlfromLambdaRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.UpdateWOlinePDFurlfromLambda(requestmodel);
            if ( response>0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }
        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Technician,Company Admin,Executive" })]
        [HttpPost]
        public IActionResult UpdateWOCategoryGroupString(UpdateWOCategoryGroupStringRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response =  workorderService.UpdateWOCategoryGroupString(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else if(response == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.message = "Not Found";
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
        /// Generate Asset form io Inspection Report
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpPost]
        public async Task<IActionResult> GenerateIRWOAssetReport([FromBody] GenerateIRWOAssetReportRequestmodel requestModel)
        {
            Response_Message response = new Response_Message();

            string aws_access_key = _options.sqs_aws_access_key;
            string aws_secret_key = _options.sqs_aws_secret_key;
            var responseModels = await workorderService.GenerateIRWOAssetReport(requestModel, aws_access_key, aws_secret_key, _options.formio_pdf_bucket);
            if (responseModels != null)
            {
                response.data = responseModels;
                response.message = ResponseMessages.RecordUpdated;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// Check Asset formio Inspection Report Status
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpGet]
        public async Task<IActionResult> IRWOAssetReportStatus([FromQuery] string wo_id)
        {
            Response_Message response = new Response_Message();
            IRWOAssetReportStatusResponsemodel responseModels = await workorderService.IRWOAssetReportStatus(wo_id);
            if (responseModels != null)
            {
                response.data = responseModels;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }

        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetAllWOLineTempIssues(GetAllWOLineTempIssuesRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.GetAllWOLineTempIssues(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public IActionResult GetAllAssetIssues(GetAllAssetIssuesRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.GetAllAssetIssues(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AddUpdateIssueComment(AddUpdateIssueCommentRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.AddUpdateIssueComment(requestmodel);
            if (response != null && response>0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else if (response == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.nodatafound;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }
        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public IActionResult GetAllAssetIssueComments(GetAllAssetIssueCommentsRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response =  workorderService.GetAllAssetIssueComments(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else if (response == null)
            {
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.nodatafound;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AddUpdateAssetIssue(AddUpdateAssetIssueRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.AddUpdateAssetIssue(requestmodel);
            if (response != null && response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else if (response == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.nodatafound;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// View Asset Issue By ID 
        /// </summary>
        /// <param name="wo_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpGet("{asset_issue_id}")]
        public IActionResult ViewAssetIssueDetailsById(string asset_issue_id)
        {
            Response_Message responsemodel = new Response_Message();
            ViewAssetIssueDetailsByIdResponsemodel response = new ViewAssetIssueDetailsByIdResponsemodel();
            response = workorderService.ViewAssetIssueDetailsById(asset_issue_id);
            if (response != null && response.asset_issue_id != null && response.asset_issue_id != Guid.Empty)
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
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult IssueListtoLinkWOline(IssueListtoLinkWOlineRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.IssueListtoLinkWOline(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> LinkIssueToWOLine(LinkIssueToWOLineRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.LinkIssueToWOLine(requestmodel);
            if ( response >0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Upload Attachments for Work Order
        /// </summary>
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadIssueImage()
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

                //var list = await s3BucketService.UploadAssetImage(filesToUpload, _options.S3_aws_access_key, _options.S3_aws_secret_key, _options.issue_photos_bucket);
                var images_list = await s3BucketService.UploadAssetMultipleImage(filesToUpload, _options.S3_aws_access_key, _options.S3_aws_secret_key, _options.issue_photos_bucket);
                List<MultipleImageUpload_ResponseModel_class> res = new List<MultipleImageUpload_ResponseModel_class>();
                WorkOrderAttachmentsResponseModel WorkOrderAttachmentsResponseModel = new WorkOrderAttachmentsResponseModel();
                foreach (var img in images_list)
                {
                    if (img != null && img.original_imege_file != null)
                    {
                        var items = new WorkOrderAttachmentsResponseModel
                        {
                            filename = img.original_imege_file,
                            file_url = UrlGenerator.GetIssueImagesURL(img.original_imege_file),
                            thumbnail_filename = img.thumbnail_image_file,
                            thumbnail_file_url = UrlGenerator.GetIssueImagesURL(img.thumbnail_image_file),
                            user_uploaded_name = img.user_uploaded_filename
                        };
                        var listitem = new MultipleImageUpload_ResponseModel_class
                        {
                            filename = img.original_imege_file,
                            file_url = UrlGenerator.GetIssueImagesURL(img.original_imege_file),
                            thumbnail_filename = img.thumbnail_image_file,
                            thumbnail_file_url = UrlGenerator.GetIssueImagesURL(img.thumbnail_image_file),
                            user_uploaded_name = img.user_uploaded_filename
                        };

                        responsemodel.success = 1;
                        res.Add(listitem);
                        WorkOrderAttachmentsResponseModel = items;
                        //return Ok(responsemodel);
                    }
                    /*else
                    {
                        responsemodel.success = 0;
                        Res_ErrorMessage mess = new Res_ErrorMessage(Resource.ResponseMessages.UPLOADFILE_INVALIDFORMAT, Resource.ResponseMessages.UPLOADFILE_TITLE);
                        responsemodel.data = mess;
                        return Ok(responsemodel);
                    }*/
                }
                WorkOrderAttachmentsResponseModel.image_list = res;
                responsemodel.data = WorkOrderAttachmentsResponseModel;
                return Ok(responsemodel);
                /*if (list != null && list.original_imege_file != null)
                {
                    var items = new WorkOrderAttachmentsResponseModel
                    {
                        filename = list.original_imege_file,
                        file_url = UrlGenerator.GetIssueImagesURL(list.original_imege_file),
                        thumbnail_filename = list.thumbnail_image_file,
                        thumbnail_file_url = UrlGenerator.GetIssueImagesURL(list.thumbnail_image_file),
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
                }*/
            }
            else
            {
                responsemodel.success = 0;
                responsemodel.data = "No file selected!!!";
                return Ok(responsemodel);
            }
        }

        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> DeleteAssetIssue(DeleteAssetIssueRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.DeleteAssetIssue(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else if (response == (int)ResponseStatusNumber.issue_is_not_open)
            {
                responsemodel.success = (int)ResponseStatusNumber.issue_is_not_open;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.DeleteOpenAssetIssue;
            }
            else if (response == (int)ResponseStatusNumber.issue_is_assigned)
            {
                responsemodel.success = (int)ResponseStatusNumber.issue_is_assigned;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.DeleteAssignedIssue;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetWOLinkedIssue(GetWOLinkedIssueRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.GetWOLinkedIssue(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }
        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UnlinkIssueFromWO(UnlinkIssueFromWORequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.UnlinkIssueFromWO(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated; 
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }
        /// <summary>
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> LinkAssetPMToWO(LinkAssetPMToWORequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.LinkAssetPMToWO(requestmodel);
            if (response != null && response.response_status==(int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else if (response != null && response.response_status == (int)ResponseStatusNumber.Error)
            {
                responsemodel.success = (int)ResponseStatusNumber.Error;
                responsemodel.data = response;
                responsemodel.message = "Below Assets do not have forms assign to its class " + response.asset_name;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// View Asset Issue By ID 
        /// </summary>
        /// <param name="wo_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{wo_id}")]
        public IActionResult GetWOCompletedThreadStatus(string wo_id)
        {
            Response_Message responsemodel = new Response_Message();
            GetWOCompletedThreadStatusResponsemodel response = new GetWOCompletedThreadStatusResponsemodel();
            response = workorderService.GetWOCompletedThreadStatus(Guid.Parse(wo_id));
            if (response != null)
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
        /// View Work Order By ID 
        /// </summary>
        /// <param name="wo_id" example=""></param>
       // [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{wo_id}")]
        public IActionResult GetAssetFormDataForBulkImport(string wo_id)
        {
            Response_Message responsemodel = new Response_Message();
            GetAssetFormDataForBulkImportResponsemodel response = new GetAssetFormDataForBulkImportResponsemodel();
            response = workorderService.GetAssetFormDataForBulkImport(wo_id);
            if (response != null )
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
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AddAssetLocationData(AddAssetLocationDataRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.AddAssetLocationData(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else if (response == (int)ResponseStatusNumber.AlreadyExists)
            {
                responsemodel.success = (int)ResponseStatusNumber.AlreadyExists;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordExist;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetLocationHierarchyForWO(GetLocationHierarchyForWORequestmodel request)
        {
            Response_Message responsemodel = new Response_Message();
            
            var response = workorderService.GetLocationHierarchyForWO(request);
            if (response != null)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetLocationHierarchyForWO_Version2(GetLocationHierarchyForWORequestmodel request)
        {
            Response_Message responsemodel = new Response_Message();

            var response = workorderService.GetLocationHierarchyForWO_Version2(request);
            if (response != null)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetWOlinesByLocation(GetWOlinesByLocationRequestmodel request)
        {
            Response_Message responsemodel = new Response_Message();

            var response = workorderService.GetWOlinesByLocation(request);
            if (response != null)
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
        /// Generate Asset form io Inspection Report
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpPost]
        public async Task<IActionResult> BulkImportAssetFormIO([FromBody] BulkImportAssetFormIORequestmodel requestModel)
        {
            Response_Message response = new Response_Message();

            
            string aws_access_key = _options.sqs_aws_access_key;
            string aws_secret_key = _options.sqs_aws_secret_key;
            var responseModels = await workorderService.BulkImportAssetFormIO(requestModel, aws_access_key, aws_secret_key, _options.formio_pdf_bucket);
            if (responseModels >0)
            {
                response.data = responseModels;
                response.message = ResponseMessages.RecordUpdated;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// Check Asset formio Inspection Report Status
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpGet]
        public IActionResult BulkImportAssetFormIOStatus([FromQuery] string wo_id)
        {
            Response_Message response = new Response_Message();
        
            var responseModels =  workorderService.BulkImportAssetFormIOStatus(wo_id);
            if (responseModels != null)
            {
                response.data = responseModels;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }

        /// <summary>
        /// Check Asset formio Inspection Report Status
        /// </summary>
        [HttpGet]
        public IActionResult ScriptforWOlinelocation()
        {
            Response_Message response = new Response_Message();
            var responseModels = workorderService.ScriptforWOlinelocation();
            if (responseModels != null)
            {
                response.data = responseModels;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }


        /// <summary>
        /// Check Asset formio Inspection Report Status
        /// </summary>
        [HttpGet]
        public IActionResult Scriptforformandclass()
        {
            Response_Message response = new Response_Message();
            var responseModels = workorderService.Scriptforformandclass();
            if (responseModels != null)
            {
                response.data = responseModels;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }

        /// <summary>
        /// Generate Asset form io Inspection Report
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpPost]
        public async Task<IActionResult> GetOBWOlineByQRCode([FromBody] GetOBWOlineByQRCodeRequestmodel requestModel)
        {
            Response_Message response = new Response_Message();
            var responseModels =  workorderService.GetOBWOlineByQRCode(requestModel);
            if (responseModels.Item1 != null && responseModels.Item2== (int)ResponseStatusNumber.Success)
            {
                response.data = responseModels.Item1;
                response.message = ResponseMessages.SuccessMessage;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else if (responseModels.Item2 == (int)ResponseStatusNumber.asset_in_different_location)
            {
                response.message = "QR code not linked to an Asset at " + UpdatedGenericRequestmodel.CurrentUser.site_name;
                response.success = (int)ResponseStatusNumber.asset_in_different_location;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }


        /// <summary>
        /// Get temp and main issue by wo id which is linked 
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetAllIssueByWOid(GetAllIssueByWOidRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.GetAllIssueByWOid(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Add WOline for existing Asset in OB/IR WOs
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AssignExistingAssettoOBWO(AssignExistingAssettoOBWORequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.AssignExistingAssettoOBWO(requestmodel);
           
            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Add WOline for existing Asset in OB/IR WOs
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetAssetsToAssignOBWO(GetAssetsToAssignOBWORequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response =  workorderService.GetAssetsToAssignOBWO(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Get componant level assets top/subcompoannat
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetAssetPMConditionDataForExport(GetAssetPMConditionDataForExportRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.GetAssetPMConditionDataForExport(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }


        /// <summary>
        /// Get componant level assets top/subcompoannat
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetComponentLevelAssets(GetComponantLevelAssetsRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.GetComponantLevelAssets(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Upload Attachments for Work Order
        /// </summary>
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadOBComponentImages()
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

                var list = await s3BucketService.UploadAssetImage(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.asset_bucket_name);

                if (list != null && list.original_imege_file != null)
                {
                    var items = new WorkOrderAttachmentsResponseModel
                    {
                        filename = list.original_imege_file,
                        file_url = UrlGenerator.GetAssetImagesURL(list.original_imege_file),
                        thumbnail_filename = list.thumbnail_image_file,
                        thumbnail_file_url = UrlGenerator.GetAssetImagesURL(list.thumbnail_image_file),
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

        //Add Issues Directly to Maintenance WorkOrder
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AddIssuesDirectlyToMaintenanceWO(AddIssuesDirectlyToMaintenanceWORequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.AddIssuesDirectlyToMaintenanceWOService(requestmodel);
            if (response != null && response.success == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else if (response != null&& response.success == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.nodatafound;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }


        //[TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician" })]
        [HttpGet]
        public async Task<IActionResult> SetIssueNumberInAssetIssues()
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.SetIssueNumberInAssetIssues();
            if (response >0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = "Suucessful";
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }


        [HttpPost]
        public async Task<IActionResult> AddDataToEquipment()
        {
            Response_Message responsemodel = new Response_Message();
            int response = await workorderService.AddDataToEquipment();
            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = "Successful!";
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }


       // [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> ChangeCalibrationStatusScript()
        {
            Response_Message responsemodel = new Response_Message();
            int response = await workorderService.ChangeCalibrationStatusScript();
            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = "Successful!";
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult DeleteLinkOfAssetPMWithWOLine(DeleteLinkOfAssetPMWithWOLineRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.DeleteLinkOfAssetPMWithWOLine(requestmodel);
            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AddAssetPMWoline(AddAssetPMWolineRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.AddAssetPMWoline(requestmodel);
            if (response != null && response.Count>0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetAssetPMFormById(GetAssetPMFormByIdRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response =  workorderService.GetAssetPMFormById(requestmodel);
            if (response != null && response.success == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else if (response != null && response.success == (int)ResponseStatusNumber.PM_Master_Form_not_available)
            {
                responsemodel.success = (int)ResponseStatusNumber.PM_Master_Form_not_available;
                responsemodel.message = "There is no form linked with this PM Item, please contact admin";
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet]
        public IActionResult GetPMMasterFormByPMid(Guid pm_id)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.GetPMMasterFormByPMid(pm_id);
            if (response != null && response.success == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else if (response != null && response.success == (int)ResponseStatusNumber.PM_Master_Form_not_available)
            {
                responsemodel.success = (int)ResponseStatusNumber.PM_Master_Form_not_available;
                responsemodel.message = "There is no form linked with this PM Item, please contact admin";
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetAssetsbyLocationHierarchy(GetAssetsbyLocationHierarchyRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.GetAssetsbyLocationHierarchy(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> SubmitPMFormJson(SubmitPMFormJsonRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.SubmitPMFormJson(requestmodel);
            if (response > 0 )
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AddTempLocationData(AddTempLocationDataRequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.AddTempLocationDataV2(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else if (response == (int)ResponseStatusNumber.AlreadyExists)
            {
                responsemodel.success = (int)ResponseStatusNumber.AlreadyExists;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordExist;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AddExistingtoTempLocation(AddExistingtoTempLocationRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.AddExistingtoTempLocation(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else if (response == (int)ResponseStatusNumber.AlreadyExists)
            {
                responsemodel.success = (int)ResponseStatusNumber.AlreadyExists;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordExist;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet]
        public IActionResult GetActiveLocationByWO(Guid wo_id, string search_string)
        {
            Response_Message responsemodel = new Response_Message();

            var response = workorderService.GetActiveLocationByWO(wo_id,search_string);
            if (response != null)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet]
        public IActionResult GetTempLocationHierarchyForWO(Guid wo_id, string search_string)
        {
            Response_Message responsemodel = new Response_Message();

            var response = workorderService.GetTempLocationHierarchyForWOV2(wo_id, search_string);
            if (response != null)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet]
        public IActionResult GetTempLocationHierarchyForWO_V3(Guid wo_id, string search_string)
        {
            Response_Message responsemodel = new Response_Message();

            var response = workorderService.GetTempLocationHierarchyForWO_V3(wo_id, search_string);
            if (response != null)
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


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetWOOBAssetsbyLocationHierarchy(GetWOOBAssetsbyLocationHierarchyRequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.GetWOOBAssetsbyLocationHierarchy(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.SuccessMessage;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetWOOBAssetsbyTempMasterLocationHierarchy(GetWOOBAssetsbyLocationHierarchyRequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.GetWOOBAssetsbyTempMasterLocationHierarchy(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.SuccessMessage;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Upload Attachments for Work Order
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadWOPMFormImages()
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

                var list = await s3BucketService.UploadWOPMFormImages(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.issue_photos_bucket);

                if (list != null && list.original_imege_file != null)
                {
                    var items = new WorkOrderAttachmentsResponseModel
                    {
                        filename = list.original_imege_file,
                        file_url = UrlGenerator.GetIssueImagesURL(list.original_imege_file),
                        thumbnail_filename = list.thumbnail_image_file,
                        thumbnail_file_url = UrlGenerator.GetIssueImagesURL(list.thumbnail_image_file),
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


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> DeleteTempLocationDetails(DeleteTempLocationDetailsRequestModel request)
        {
            Response_Message response = new Response_Message();

            int res = await workorderService.DeleteTempLocationDetails(request);

            if (res == (int)ResponseStatusNumber.Success)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = res;
            }
            else if (res == (int)ResponseStatusNumber.AlreadyExists)
            {
                response.success = (int)ResponseStatusNumber.AlreadyExists;
                response.message = "Location can't be deleted!!";
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> updatesubassetlocationscript()
        {
            Response_Message response = new Response_Message();

            int res = await workorderService.updatesubassetlocationscript();

            if (res == (int)ResponseStatusNumber.Success)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = res;
            }
            else if (res == (int)ResponseStatusNumber.AlreadyExists)
            {
                response.success = (int)ResponseStatusNumber.AlreadyExists;
                response.message = "Location can't be deleted!!";
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet]
        public IActionResult GetOBIRImagesByWOId(Guid wo_id)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.GetIRImagesByWOId(wo_id);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetOBIRImagesByWOId_V2(GetOBIRImagesByWOId_V2RequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.GetIRImagesByWOId_V2(requestModel);
            if (response != null&&response.list.Count>0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> AddPMtoNewWoline(AddPMtoNewWolineRequestmodel request)
        {
            Response_Message response = new Response_Message();

            AddPMtoNewWolineResponsemodel res = await workorderService.AddPMtoNewWoline(request);

            if (res != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = res;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> CreateTempIssue(CreateTempIssueRequestmodel request)
        {
            Response_Message response = new Response_Message();

            var res = await workorderService.CreateTempIssue(request);

            if (res != null && res.success == (int)ResponseStatusNumber.Success)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = res;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public IActionResult GetAssetListForIssue(GetAssetListForIssueRequestmodel request)
        {
            Response_Message response = new Response_Message();

            var res =  workorderService.GetAssetListForIssue(request);

            if (res != null )
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = res;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetWODetilsForReport()
        {
            Response_Message response = new Response_Message();

            List<GetWODetilsForReportResponsemodel> res = await workorderService.GetWODetilsForReport();

            if (res != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = res;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetIssueDetailsForReport()
        {
            Response_Message response = new Response_Message();

            List<GetIssueDetailsForReportResponsemodel> res = await workorderService.GetIssueDetailsForReport();

            if (res != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = res;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public IActionResult GetAllCalendarWorkorders(GetAllCalanderWorkordersRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            ListViewModel<GetAllCalanderWorkordersResponseModel> response = new ListViewModel<GetAllCalanderWorkordersResponseModel>();
            response = workorderService.GetAllCalendarWorkorders(requestModel);
            //response.pageIndex = requestModel.pageindex;
            //response.pageSize = requestModel.pagesize;
            if (response.list != null && response.list.Count > 0)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpGet]
        public IActionResult GetWOTypeWiseSubmittedAssetsCount()
        {
            Response_Message responsemodel = new Response_Message();

            var response = workorderService.GetWOTypeWiseSubmittedAssetsCount();
            if (response != null)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{woonboardingassets_id}")]
        public IActionResult GetIssueWOlineDetailsById(string woonboardingassets_id)
        {
            Response_Message responsemodel = new Response_Message();

            var response = workorderService.GetIssueWOlineDetailsById(woonboardingassets_id);
            if (response != null)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AddUpdateTimeMaterial(AddUpdateTimeMaterialRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            int response = await workorderService.AddUpdateTimeMaterial(requestModel);

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
        public IActionResult GetAllTimeMaterialsForWO(GetAllTimeMaterialsForWORequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            GetAllTimeMaterialsForWOResponseModel response = new GetAllTimeMaterialsForWOResponseModel();

            response = workorderService.GetAllTimeMaterialsForWO(requestModel);
             
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> BulkCreateTimeMaterialsForWO(BulkCreateTimeMaterialsForWORequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            int response = await workorderService.BulkCreateTimeMaterialsForWO(requestmodel); 

            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordAdded;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }


        [HttpGet]
        public async Task<IActionResult> SendNotificationsForDueOverdueWorkorders()
        {  
            Response_Message responsemodel = new Response_Message();

            int response = await workorderService.SendNotificationsForDueOverdueWorkorders();
            if (response == (int)ResponseStatusNumber.Success)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> GetImageInfoByTextRact(GetImageInfoByTextRactRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();

            var question_list = workorderService.GetImageInfoByTextRact(requestmodel);

            var response =  await s3BucketService.DetectSampleAsync(_options.textract_aws_access_key, _options.textract_aws_secret_key,requestmodel.bucket_name,requestmodel.image_name, question_list.question_list);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
               // responsemodel.message = ResponseMessages.RecordAdded;
                responsemodel.data = response;
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
        public async Task<IActionResult> UpdateMultiOBWOAssetsStatus(UpdateMultiOBWOAssetsStatusRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            
            var response = await workorderService.UpdateMultiOBWOAssetsStatus(requestModel);
            if (response != null && response.status == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordUpdated;
                responsemodel.data = response;
            }
            else if (response != null && response.status == (int)ResponseStatusNumber.qr_code_must_be_unique)
            {
                responsemodel.success = (int)ResponseStatusNumber.qr_code_must_be_unique;
                responsemodel.message = response.assets_with_error;
                responsemodel.data = response;
            }
            else if (response != null && response.status == (int)ResponseStatusNumber.can_not_delete_toplevel_woline)
            {
                responsemodel.success = (int)ResponseStatusNumber.can_not_delete_toplevel_woline;
                responsemodel.message = response.assets_with_error;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        [HttpGet]
        public async Task<IActionResult> WorkorderTempAssetScriptForInstallWOline()
        {
            Response_Message responsemodel = new Response_Message();

            var response = await workorderService.WorkorderTempAssetScriptForInstallWOline();
            if (response != null)
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

        [HttpGet]
        public async Task<IActionResult> WorkorderTempAssetScriptForIssueWOline()
        {
            Response_Message responsemodel = new Response_Message();

            var response = await workorderService.WorkorderTempAssetScriptForIssueWOline();
            if (response != null)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpGet]
        public IActionResult GetAllResponsiblePartyList()
        {
            Response_Message responsemodel = new Response_Message();

            var response = workorderService.GetAllResponsiblePartyList();
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.SuccessMessage;
                responsemodel.data = response;
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
        public async Task<IActionResult> ChangeQuoteStatus(ChangeQuoteStatusRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await workorderService.ChangeQuoteStatus(requestModel);
            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordUpdated;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{wo_id}")]
        public IActionResult GetAllOBAssetsWithQRCodeByWOId(string wo_id)
        {
            Response_Message responsemodel = new Response_Message();

            var response = workorderService.GetAllOBAssetsWithQRCodeByWOId(wo_id);
            if (response != null)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AddExistingAssetToWorkorderByQRCode(AddExistingAssetToWorkorderByQRCodeRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await workorderService.AddExistingAssetToWorkorderByQRCode(requestModel);
            if (response.Item2 == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordAdded;
                responsemodel.data = response.Item1;
            }
            else if(response.Item2 == (int)ResponseStatusNumber.AlreadyExists)
            {
                responsemodel.message = "Asset already exist in this workorder.";
                responsemodel.success = (int)ResponseStatusNumber.AlreadyExists;
            }
            else if (response.Item2 == (int)ResponseStatusNumber.asset_in_different_location)
            {
                responsemodel.message = "QR code not linked to an Asset at " + UpdatedGenericRequestmodel.CurrentUser.site_name;
                responsemodel.success = (int)ResponseStatusNumber.asset_in_different_location;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet()]
        public IActionResult GetQuoteListStatusWise([FromQuery] string search_string)
        {
            Response_Message responsemodel = new Response_Message();
            //GetWOcategoryTaskByCategoryIDResponsemodel response = new GetWOcategoryTaskByCategoryIDResponsemodel();
            var response = workorderService.GetQuoteListStatusWise(search_string,null);
            if (response != null)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetQuoteListStatusWise_V2(GetWOBacklogCardList_V2RequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            //GetWOcategoryTaskByCategoryIDResponsemodel response = new GetWOcategoryTaskByCategoryIDResponsemodel();
            var response = workorderService.GetQuoteListStatusWise(requestModel.search_string, requestModel.site_id);
            if (response != null)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{wo_id}")]
        public IActionResult GetAllTempAssetDataForWO(Guid wo_id)
        {
            Response_Message responsemodel = new Response_Message();

            var response = workorderService.GetAllTempAssetDataForWO(wo_id);
            if (response != null)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> GenerateOnboardingWOReport(GenerateOnboardingWOReportRequestModel_2 requestModel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await workorderService.GenerateOnboardingWOReport(requestModel);
            if (response.status == (int)Status.Completed)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else if (response.status == (int)ResponseStatusNumber.TimedOut)
            {
                responsemodel.success = (int)ResponseStatusNumber.TimedOut;
                responsemodel.data = response;
                responsemodel.message = "Your request has been timed out, please try again!";
            }
            else if (response.status == (int)Status.ReportFailed)
            {
                responsemodel.success = (int)ResponseStatusNumber.Error;
                responsemodel.data = response;
                responsemodel.message = "Your request has been failed, please try again!";
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> GenerateMaintenanceWOReport(GenerateOnboardingWOReportRequestModel_2 requestModel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await workorderService.GenerateMaintenanceWOReport(requestModel);
            if (response.status == (int)Status.Completed)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else if (response.status == (int)ResponseStatusNumber.TimedOut)
            {
                responsemodel.success = (int)ResponseStatusNumber.TimedOut;
                responsemodel.data = response;
                responsemodel.message = "Your request has been timed out, please try again!";
            }
            else if (response.status == (int)Status.ReportFailed)
            {
                responsemodel.success = (int)ResponseStatusNumber.Error;
                responsemodel.data = response;
                responsemodel.message = "Your request has been failed, please try again!";
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }


        //[TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> CreateQuoteFromEstimator(CreateQuoteFromEstimatorRequestmodel requestModel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await workorderService.CreateQuoteFromEstimator(requestModel);
            if (response.quote_id != Guid.Empty)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordAdded;
                responsemodel.data = response.quote_id;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> AddNewTempMasterLocationData(AddTempMasterLocationDataRequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.AddNewTempMasterLocationData(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else if (response == (int)ResponseStatusNumber.AlreadyExists)
            {
                responsemodel.success = (int)ResponseStatusNumber.AlreadyExists;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordExist;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> AddExistingTempMasterLocation(AddExistingtoTempLocationRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await workorderService.AddExistingTempMasterLocation(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordAdded;
            }
            else if (response == (int)ResponseStatusNumber.AlreadyExists)
            {
                responsemodel.success = (int)ResponseStatusNumber.AlreadyExists;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordExist;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public IActionResult GetAllTempMasterLocationDropdownList(GetAllTempMasterLocationForWORequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = workorderService.GetAllTempMasterLocationDropdownList(requestModel);
            if (response != null)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public IActionResult GetAllTempMasterLocationsListForWO(GetAllTempMasterLocationForWORequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = workorderService.GetAllTempMasterLocationsListForWO(requestModel);
            if (response != null)
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


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public IActionResult GetPMEstimation(GetPMEstimationRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = workorderService.GetPMEstimation(requestModel);


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

    }
}