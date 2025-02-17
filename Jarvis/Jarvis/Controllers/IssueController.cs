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

namespace Jarvis.Controllers
{
    [TypeFilter(typeof(requestvalidationFilter))]
    //[TypeFilter(typeof(ValidationFilterAttribute))]
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiController]
    public class IssueController : ControllerBase
    {
        private readonly IIssueService issueService;
        public IssueController(IIssueService _issueService)
        {
            this.issueService = _issueService;
        }

        /// <summary>
        /// Create Work Order
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive" })]
        [HttpPost]
        public async Task<IActionResult> CreateIssue([FromBody]IssueRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            Guid guidOutput;
            var responseModel = await issueService.CreateIssue(requestModel);
            bool isValid = Guid.TryParse(responseModel.issueguid, out guidOutput);
            if (isValid)
            {
                response.data = guidOutput;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else if (responseModel.status == (int)ResponseStatusNumber.NotFound)
            {
                response.message = ResponseMessages.nodatafound;
                response.success = (int)ResponseStatusNumber.NotFound;
            }
            else if (responseModel.status == (int)ResponseStatusNumber.Error)
            {
                //response.data = ResponseMessages.Error;
                response.message = responseModel.message;
                response.success = (int)ResponseStatusNumber.Error;
            }
            //}
            //catch(Exception e)
            //{
            //    Logger.Log("Error in Create WorkOrder : " + e.ToString());
            //    response.message = ResponseMessages.Error;
            //    response.success = (int)ResponseStatusNumber.Error;
            return Ok(response);
        }

        /// <summary>
        /// Get All Work Order
        /// </summary>
        /// <param name="status" example=""></param>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Company Admin" })]
        [HttpGet("{status?}/{pagesize?}/{pageindex?}")]
        public IActionResult GetAllIssues(int status, int pagesize, int pageindex)
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
        /// Get All Work Order With Filter
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Company Admin" })]
        [HttpPost]
        public IActionResult FilterIssues([FromBody]FilterIssueRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<IssueResponseModel> response = new ListViewModel<IssueResponseModel>();
            response = issueService.FilterIssues(requestModel);
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
        /// Get All Work Order With Filter Title Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Company Admin" })]
        [HttpPost]
        public IActionResult FilterIssuesTitleOptions([FromBody] FilterIssueRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<IssuesNameResponseModel> response = new ListViewModel<IssuesNameResponseModel>();
            response = issueService.FilterIssuesTitleOption(requestModel);
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
        /// Get All Work Order With Filter Asset name Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Company Admin" })]
        [HttpPost]
        public IActionResult FilterIssuesAssetOptions([FromBody] FilterIssueRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<AssetListResponseModel> response = new ListViewModel<AssetListResponseModel>();
            response = issueService.FilterIssuesAssetOption(requestModel);
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
        /// Get All Work Order With Filter Asset Sites Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Company Admin" })]
        [HttpPost]
        public IActionResult FilterIssuesSitesOptions([FromBody] FilterIssueRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<SitesViewModel> response = new ListViewModel<SitesViewModel>();
            response = issueService.FilterIssuesSitesOption(requestModel);
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
        /// Get All Work Orders Titles For Filter
        /// </summary>
        /// <param name="siteid"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Company Admin" })]
        [HttpGet]
        public IActionResult GetAllIssueTitle([FromQuery]string siteid)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<IssuesNameResponseModel> response = new ListViewModel<IssuesNameResponseModel>();
            response = issueService.GetAllIssuesTitle(siteid);
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

        /// <summary>
        /// Get Work Order By ID 
        /// </summary>
        /// <param name="issue_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Company Admin" })]
        [HttpGet("{issue_id}")]
        public async Task<IActionResult> GetIssueDetailsById(string issue_id)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
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
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetWorkOrderDetailsByIdAsync ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Update Work Order By Manager
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Technician,Company Admin,Executive" })]
        [HttpPost]
        public async Task<IActionResult> UpdateIssueByManager([FromBody]UpdateIssueRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            int result = await issueService.UpdateIssueByManager(requestModel);
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
                response.message = ResponseMessages.nodatafound;
                response.success = result;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = result;
            }
            //}catch(Exception e)
            //{
            //    Logger.Log("Error in UpdateWorkOrderByManager ", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Get Today Work Orders
        /// </summary>
        /// <param name="pagesize" example=""></param>
        /// <param name="pageindex" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Maintenance staff,Company Admin" })]
        [HttpGet("{pagesize?}/{pageindex?}")]
        public IActionResult GetTodayNewIssues(int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
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
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetTodayNewWorkOrders ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Update Work Order Maintenance Staff
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Maintenance staff,Company Admin,Executive" })]
        [HttpPost]
        public async Task<IActionResult> UpdateIssueByMaintenance([FromBody]UpdateIssueByMaintenanceRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            //try
            //{
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
            //}
            //catch(Exception e)
            //{
            //    response.message = ResponseMessages.Error;
            //    response.success = (int)ResponseStatusNumber.Error;
            //    Logger.Log("Error in UpdateWorkOrderByMaintenance ", e.ToString());
            //}
            return Ok(response);
        }

        /// <summary>
        /// Get Work Order By Asset ID
        /// </summary>
        /// <param name="assetid" example=""></param>
        /// <param name="pagesize" example=""></param>
        /// <param name="pageindex" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpGet("{assetid}/{pagesize?}/{pageindex?}")]
        public IActionResult GetIssueByAssetId(string assetid, int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            ListViewModel<IssueResponseModel> response = new ListViewModel<IssueResponseModel>();
            response = issueService.GetIssueByAssetId(assetid, pagesize, pageindex);
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
            //    Logger.Log("Error in GetWorkOrderByAssetId ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Search Work Order By Asset ID
        /// </summary>
        /// <param name="assetid" example=""></param>
        /// <param name="searchstring" example=""></param>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Company Admin" })]
        [HttpGet("{assetid}/{searchstring}/{pagesize?}/{pageindex?}")]
        public IActionResult SearchIssueByAssetId(string assetid, string searchstring, int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            ListViewModel<IssueResponseModel> response = new ListViewModel<IssueResponseModel>();
            response = issueService.SearchIssueByAssetId(assetid, searchstring, pagesize, pageindex);
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
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in SearchWorkOrderByAssetId ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Search Work Order 
        /// </summary>
        /// <param name="searchstring" example=""></param>
        /// <param name="timezone" example="Asia-Calcutta"></param>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Company Admin" })]
        [HttpGet("{searchstring}/{timezone}/{pagesize?}/{pageindex?}")]
        public IActionResult SearchIssues(string searchstring, string timezone, int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            ListViewModel<IssueResponseModel> response = new ListViewModel<IssueResponseModel>();
            response = issueService.SearchIssues(searchstring, timezone, pagesize, pageindex);
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
            //    Console.WriteLine("Error in SearchWorkOrders " + e.ToString());
            //    Logger.Log("Error in SearchWorkOrders ", e.ToString());
            //    responsemodel.success = (int)ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Asset Issue
        /// </summary>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Maintenance staff,Company Admin,Executive" })]
        [HttpGet("{pagesize?}/{pageindex?}")]
        public IActionResult GetAssetsIssue(int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<AssetIssueResponseModel> response = new ListViewModel<AssetIssueResponseModel>();
            response = issueService.GetAssetsIssue(pagesize, pageindex);
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
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get All Work Order (Sync)
        /// </summary>
        /// <param name="timestamp" example=""></param>
        /// <param name="pagesize" example=""></param>
        /// <param name="pageindex" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Company Admin" })]
        [HttpGet("{timestamp}/{pagesize?}/{pageindex?}")]
        public IActionResult GetAllIssue(string timestamp, int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<IssueResponseModel> response = new ListViewModel<IssueResponseModel>();
            response = issueService.GetAllIssue(timestamp, pagesize, pageindex);
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
        
        [HttpGet]
        public async Task<IActionResult> CreateIssuesFromWorkOrder()
        {
            Response_Message response = new Response_Message();
            var responseModel = await issueService.CreateIssuesFromWorkOrder();
            if (responseModel > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> CreateIssueStatus()
        {
            Response_Message response = new Response_Message();
            var responseModel = await issueService.CreateIssueStatus();
            if (responseModel > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> CreateIssueRecords()
        {
            Response_Message response = new Response_Message();
            var responseModel = await issueService.CreateIssueRecords();
            if (responseModel > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(response);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> LinkIssueToWOFromIssueListTab(LinkIssueToWOFromIssueListTabRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            
            var response = await issueService.LinkIssueToWOFromIssueListTab(requestModel);
            if (response !=null && String.IsNullOrEmpty(response.empty_asset_class_list))
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = "Issue Linked Successfully";
                responsemodel.data = response;
            }
            else if (response != null && !String.IsNullOrEmpty(response.empty_asset_class_list))
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = response.empty_asset_class_list + "Asset class does not have form linked";
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> LinkIssueToWOFromIssueListTabForSteps(LinkIssueToWOFromIssueListTabRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await issueService.LinkIssueToWOFromIssueListTabForSteps(requestModel);
            if (response != null && String.IsNullOrEmpty(response.empty_asset_class_list))
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = "Issue Linked Successfully";
                responsemodel.data = response;
            }
            else if (response != null && !String.IsNullOrEmpty(response.empty_asset_class_list))
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = response.empty_asset_class_list + "Asset class does not have form linked";
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AddIssueBySteps(AddIssueByStepsRequestmodel requestModel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await issueService.AddIssueBySteps(requestModel);
            if (response > 0)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> UpdateIssueBySteps(UpdateIssueByStepsRequestmodel requestModel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await issueService.UpdateIssueBySteps(requestModel);
            if (response > 0)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin,SuperAdmin" })]
        [HttpPost]
        public IActionResult GetAllAssetIssuesOptimized(GetAllAssetIssuesRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = issueService.GetAllAssetIssuesOptimized(requestmodel);
            if (response.list != null && response.list.Count > 0)
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

        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public IActionResult GetAllIssueByWOidOptimized(GetAllIssueByWOidRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = issueService.GetAllIssueByWOidOptimized(requestmodel);
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> UpdateIRVisualImageLabel(UpdateIRVisualImageLabelRequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await issueService.UpdateIRVisualImageLabel(requestmodel);
            if (response == (int)ResponseStatusNumber.Success)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Company Admin" })]
        [HttpGet("{report_type}")]
        public async Task<IActionResult> GenerateSiteIssuesReport(string report_type)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await issueService.GenerateSiteIssuesReport(report_type);
            if (response.status == (int)Status.Completed)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.SuccessMessage;
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
    }
}