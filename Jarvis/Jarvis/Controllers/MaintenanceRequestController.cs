using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jarvis.Filter;
using Jarvis.Resource;
using Jarvis.Service.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using Jarvis.ViewModels.Filter;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Jarvis.Shared;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Jarvis.Service.Concrete;
using Jarvis.Shared.Helper;

namespace Jarvis.Controllers
{
    [TypeFilter(typeof(requestvalidationFilter))]
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiController]
    public class MaintenanceRequestController : ControllerBase
    {
        public AWSConfigurations _options { get; set; }
        private readonly IMaintenanceRequestService mrService;
        private readonly IS3BucketService s3BucketService;

        public MaintenanceRequestController(IMaintenanceRequestService _mrService, IOptions<AWSConfigurations> options)
        {
            this.mrService = _mrService;
            this._options = options.Value;
            this.s3BucketService = new S3BucketService();
        }

        /// <summary>
        /// Add Update Maintenance Request
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { GlobalConstants.Manager, GlobalConstants.CompanyAdmin })]
        public async Task<IActionResult> AddUpdateMaintenanceRequest([FromBody] AddMRRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await mrService.AddUpdateMaintenanceReq(request);
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
        /// Get All Maintenance Request
        /// </summary>
        /// <param name="filter_type"></param>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { GlobalConstants.Manager, GlobalConstants.CompanyAdmin })]
        public async Task<IActionResult> GetAllMaintenanceRequest([FromBody] GetAllMRRequestModel filter_type)
        {
            Response_Message responsemodel = new Response_Message();
            var result = await mrService.GetAllMaintenanceRequest(filter_type);
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
        /// Get Maintenance Request of Open status
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { GlobalConstants.Manager, GlobalConstants.CompanyAdmin })]
        [HttpGet]
        public async Task<IActionResult> GetMaintenanceRequestOpenStatusCount()
        {
            Response_Message response = new Response_Message();

            GetOpenMRCount responseModel = new GetOpenMRCount();
            responseModel = await mrService.MaintenanceRequestOpenStatusCount();
            if (responseModel.openMRCount > 0)
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
        /// Get Work Order with No Maintenance Request attached and of same Asset with Search String
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin" })]
        [HttpGet("{assetid}")]
        public async Task<IActionResult> GetAllWorkOrderWithNoMR(string assetid, [FromQuery] string searchstring, [FromQuery] int pageindex, [FromQuery] int pagesize)
        {
            Response_Message responsemodel = new Response_Message();
            var result = await mrService.GetAllWorkOrderWithNoMR(assetid, searchstring, pageindex, pagesize);
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
        /// Get Maintenace Request By Id
        /// </summary>
        /// <param name="mr_id"></param>
        [HttpGet("{mr_id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { GlobalConstants.Manager, GlobalConstants.CompanyAdmin })]
        public async Task<IActionResult> GetMaintenanceRequestById(Guid mr_id)
        {
            Response_Message responsemodel = new Response_Message();
            var result = await mrService.GetMaintenanceRequestById(mr_id);
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

        /// <summary>
        /// Resolve maintenance Request
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { GlobalConstants.Manager, GlobalConstants.CompanyAdmin })]
        public async Task<IActionResult> ResolveMaintenanceRequest([FromBody] ResolveMaintenanceRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await mrService.ResolveMaintenanceRequest(request);
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

        ///// <summary>
        ///// Get Filtered Type
        ///// </summary>
        //[TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager" })]
        //[HttpPost]
        //public async Task<IActionResult> FilterTypeOptions([FromBody] FilterMaintenanceRequestModel requestModel)
        //{
        //    Response_Message responsemodel = new Response_Message();
        //    ListViewModel<MRResponseModel> response = new ListViewModel<MRResponseModel>();
        //    response = await mrService.FilterTypeOptions(requestModel);
        //    if (response?.list?.Count > 0)
        //    {
        //        responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
        //        responsemodel.data = response;
        //    }
        //    else
        //    {
        //        responsemodel.message = ResponseMessages.nodatafound;
        //        responsemodel.success = (int)ResponseStatusNumber.NotFound;
        //    }
        //    return Ok(responsemodel);
        //}

        /// <summary>
        /// Get Filtered Type
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> FilterRequestedByOptions([FromBody] FilterMaintenanceRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<MRResponseModel> response = new ListViewModel<MRResponseModel>();
            response = await mrService.FilterRequestedByOptions(requestModel);
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
    }
}
