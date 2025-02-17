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

namespace Jarvis.Controllers
{
    [TypeFilter(typeof(requestvalidationFilter))]
    //[TypeFilter(typeof(ValidationFilterAttribute))]
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiController]
    public class MobileWorkOrderController : ControllerBase
    {
        private readonly IS3BucketService s3BucketService;
        private readonly IMobileWorkOrderService mobileworkorderservice;
        public AWSConfigurations _options { get; set; }
        public MobileWorkOrderController(IMobileWorkOrderService _mobileworkorderService, IOptions<AWSConfigurations> options)
        {
            this.mobileworkorderservice = _mobileworkorderService;
            this._options = options.Value;
            this.s3BucketService = new S3BucketService();
        }

        /// <summary>
        /// Get All Work Order for new requirement Mobile
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetAllWorkOrdersNewflow(NewFlowWorkorderListRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            ListViewModel<MobileNewFlowWorkorderListResponseModel> response = new ListViewModel<MobileNewFlowWorkorderListResponseModel>();
            response = mobileworkorderservice.GetAllWorkOrdersNewflow(requestModel);
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
        /// View Work Order By ID 
        /// </summary>
        /// <param name="wo_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{wo_id}")]
        public IActionResult ViewWorkOrderDetailsById(string wo_id)
        {
            Response_Message responsemodel = new Response_Message();
            MobileViewWorkOrderDetailsByIdResponsemodel response = new MobileViewWorkOrderDetailsByIdResponsemodel();
            response = mobileworkorderservice.ViewWorkOrderDetailsById(wo_id);
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
        /// Get All PM Tasks
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Technician" })]
        public IActionResult GetAllTask([FromQuery(Name = "pageindex")] int pageindex, [FromQuery(Name = "pagesize")] int pagesize, [FromQuery(Name = "searchstring")] string searchstring)
        {
            Response_Message responcemodel = new Response_Message();
            ListViewModel<MobileTaskResponseModel> response = new ListViewModel<MobileTaskResponseModel>();
            response = mobileworkorderservice.GetAllTasks(pageindex, pagesize, searchstring);
            if (response?.list?.Count > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.data = response;
            }
            else
            {
                responcemodel.success = (int)ResponseStatusNumber.NotFound;
                responcemodel.message = ResponseMessages.nodatafound;
            }

            return Ok(responcemodel);
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
            var response = mobileworkorderservice.GetWOcategoryTaskByCategoryID(wo_inspectionsTemplateFormIOAssignment_id);
            if (response != null && response.Count > 0)
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
        [HttpGet("{wo_id}")]
        public IActionResult GetAllWOCategoryTaskByWOid(string wo_id)
        {
            Response_Message responsemodel = new Response_Message();
            //GetWOcategoryTaskByCategoryIDResponsemodel response = new GetWOcategoryTaskByCategoryIDResponsemodel();
            var response = mobileworkorderservice.GetAllWOCategoryTaskByWOid(wo_id);
            if (response != null && response.Count > 0)
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
        /// Get Sync Data
        /// </summary>
        /// <param name="userid" example="a343180f-6474-42fd-a486-9f9491d91a43"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{userid}")]
        public async Task<IActionResult> GetWOsForOffline(string userid)
        {
            Response_Message response = new Response_Message();
            MobileGetWOsForOfflineResponsemodel responseModels = await mobileworkorderservice.GetWOsForOffline(userid);
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
        /// Get Inspection form list by asset id
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpGet()]
        public IActionResult GetAllAssetInspectionListByAssetId(string assetid = null, int pagesize = 10, int pageindex = 1)
        {
            Response_Message responsemodel = new Response_Message();
            var result = mobileworkorderservice.GetAllAssetTemplateList(assetid, pagesize, pageindex);
            if (result?.list?.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = result;
            }
            else if (result?.list?.Count == 0)
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
        /// Get sub assets(child assets) by parent asset id
        /// </summary>
        /// <param name="status" example="0"></param>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpGet("{asset_id}/{pagesize?}/{pageindex?}")]
        public IActionResult GetSubAssetsByAssetID(string asset_id, int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            ListViewModel<MobileAssetsResponseModel> response = new ListViewModel<MobileAssetsResponseModel>();
            response = mobileworkorderservice.GetSubAssetsByAssetID(asset_id, pagesize, pageindex);
            response.pageIndex = pageindex;
            response.pageSize = pagesize;
            if (response.list.Count > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else if (pageindex > 1 && response.list.Count == 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
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
            //    responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get All Work Order With Filter
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult FilterIssues([FromBody] FilterIssueRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<MobileIssueResponseModel> response = new ListViewModel<MobileIssueResponseModel>();
            response = mobileworkorderservice.FilterIssues(requestModel);
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
        /// Get All Inspection with Filter
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin,Technician" })]
        [HttpPost]
        public IActionResult FilterInspections([FromBody] FilterInspectionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<MobileInspectionResponseModel> response = new ListViewModel<MobileInspectionResponseModel>();
            response = mobileworkorderservice.FilterInspections(requestModel);
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
        /// Get Filtered Asset 
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive , Technician" })]
        [HttpPost]
        public IActionResult FilterAsset([FromBody] FilterAssetsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<MobileAssetsResponseModel> response = new ListViewModel<MobileAssetsResponseModel>();
            response = mobileworkorderservice.FilterAssets(requestModel);
            if (response?.list?.Count > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
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
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff, Technician,Company Admin" })]
        [HttpGet("{issue_id}")]
        public async Task<IActionResult> GetIssueDetailsById(string issue_id)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            MobileIssueResponseModel response = new MobileIssueResponseModel();
            response = await mobileworkorderservice.GetIssueByID(issue_id);
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[]  { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpGet("{asset_form_id}")]
        public async Task<IActionResult> GetAssetFormIOByAssetformId(string asset_form_id)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await mobileworkorderservice.GetAssetFormIOByAssetFormId(Guid.Parse(asset_form_id));
            if (response != null)
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
        /// Get Filtered Asset 
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive , Technician" })]
        [HttpPost]
        public IActionResult GetAssetPMList([FromBody] GetAssetPMListMobileRequestmodel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<GetAssetPMListMobileResponsemodel> response = new ListViewModel<GetAssetPMListMobileResponsemodel>();
            response = mobileworkorderservice.GetAssetPMList(requestModel);
            if (response?.list?.Count > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
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
        /// Get Filtered Asset 
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive , Technician" })]
        [HttpPost]
        public async Task<IActionResult> UpdateAssetPMFixStatus([FromBody] UpdateAssetPMFixStatusRequestmodel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            
            var response = await mobileworkorderservice.UpdateAssetPMFixStatus(requestModel);
            if (response > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
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
