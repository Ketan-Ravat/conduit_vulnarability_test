using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using Jarvis.Filter;
using Jarvis.Resource;
using Jarvis.Service.Abstract;
using Jarvis.Service.Concrete;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jarvis.Controllers {
    [TypeFilter(typeof(requestvalidationFilter))]
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiController]
    public class AssetPMController : ControllerBase {
        public AWSConfigurations _options { get; set; }
        private readonly IAssetPMService assetPMService;
        private readonly IS3BucketService s3BucketService;
        public AssetPMController(IAssetPMService _assetPMService)
        {
            this.assetPMService = _assetPMService;
        }

        /// <summary>
        /// Assign PM to Asset
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> AddAssetPM([FromBody] AssignPMToAsset request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await assetPMService.AddAssetPM(request);
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
        /// Delete PM By ID
        /// </summary>
        [HttpGet()]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> assignpmtoassetcecco()
        {
            Response_Message responcemodel = new Response_Message();
            var result = await assetPMService.assignpmtoassetcecco();
            if (result > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.message = ResponseMessages.PMCategoryDeleted;
            }
            else if (result == (int)ResponseStatusNumber.NotFound)
            {
                responcemodel.success = (int)ResponseStatusNumber.NotFound;
                responcemodel.message = ResponseMessages.nodatafound;
            }
            else
            {
                responcemodel.success = (int)ResponseStatusNumber.Error;
                responcemodel.message = ResponseMessages.Error;
            }

            return Ok(responcemodel);
        }
        /// <summary>
        /// Assign PM to Asset
        /// </summary>
        [HttpGet("{asset_pm_plan_id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> RemoveAssetPM(Guid asset_pm_plan_id)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await assetPMService.RemoveAssetPMPlan(asset_pm_plan_id);
                if (result > 0)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordUpdated;
                }
                else if (result == (int)ResponseStatusNumber.NotFound)
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
        /// Get Asset PM by ID
        /// </summary>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> GetById(Guid id)
        {
            Response_Message responcemodel = new Response_Message();
            AssetPMResponseModel response = new AssetPMResponseModel();
            response = await assetPMService.GetAssetPMByID(id);
            if (response != null)
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
        /// Get PMs by Asset ID
        /// </summary>
        [HttpGet("{asset_id}/{filter_type?}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> GetPMByAssetId(Guid asset_id, int filter_type = 0)
        {
            Response_Message responcemodel = new Response_Message();
            var response = await assetPMService.GetAssetPMByAssetID(asset_id, filter_type);
            if (response?.list.Count > 0)
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
        /// Update Asset PM
        /// </summary>
        [HttpPut]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin, Manager,Executive" })]
        public async Task<IActionResult> Update([FromBody] UpdateAssetPMRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await assetPMService.UpdateAssetPM(request);
                if (result.response_status > 0)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordUpdated;
                    responcemodel.data = null;
                }
                else if (result.response_status == (int)ResponseStatusNumber.can_not_update_future_assetpm)
                {
                    responcemodel.success = (int)ResponseStatusNumber.can_not_update_future_assetpm;
                    responcemodel.message = "Sorry! , You can't Update overdue or future AssetPM";
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
        /// Delete Asset PM By ID
        /// </summary>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> Delete(Guid id)
        {
            Response_Message responcemodel = new Response_Message();
            var result = await assetPMService.DeleteAssetPM(id);
            if (result > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.message = ResponseMessages.PMCategoryDeleted;
            }
            else if (result == (int)ResponseStatusNumber.NotFound)
            {
                responcemodel.success = (int)ResponseStatusNumber.NotFound;
                responcemodel.message = ResponseMessages.nodatafound;
            }
            else
            {
                responcemodel.success = (int)ResponseStatusNumber.Error;
                responcemodel.message = ResponseMessages.Error;
            }

            return Ok(responcemodel);
        }

        /// <summary>
        /// Duplicate Asset PM
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> Duplicate([FromBody] UpdateAssetPMRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await assetPMService.DuplicateAssetPM(request);
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
        /// Mark Completed Triggers
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> MarkComplete([FromBody] PMMarkCompletedRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            var result = await assetPMService.MarkCompletedPM(request);
            if (result > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.message = ResponseMessages.MarkedCompleted;
            }
            else
            {
                responcemodel.success = (int)ResponseStatusNumber.Error;
                responcemodel.message = ResponseMessages.Error;
            }
            return Ok(responcemodel);
        }

        /// <summary>
        /// Update the status of Trigger
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> UpdateTriggerStatus()
        {
            Response_Message responcemodel = new Response_Message();
            var result = await assetPMService.UpdateTriggerStatus();
            if (result > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.message = ResponseMessages.MarkedCompleted;
            }
            else
            {
                responcemodel.success = (int)ResponseStatusNumber.Error;
                responcemodel.message = ResponseMessages.Error;
            }
            return Ok(responcemodel);
        }

        /// <summary>
        /// Update the task Status
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> UpdateTaskStatus(PMTriggerTaskRequestModel requestModel)
        {
            Response_Message responcemodel = new Response_Message();
            var result = await assetPMService.UpdateTaskStatus(requestModel);
            if (result > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.message = ResponseMessages.RecordUpdated;
            }
            else
            {
                responcemodel.success = (int)ResponseStatusNumber.Error;
                responcemodel.message = ResponseMessages.Error;
            }
            return Ok(responcemodel);
        }

        /// <summary>
        /// PM List Section >> Dashboard
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> DashboardPendingPMItems(DashboardPendingPMItemsRequestModel requestModel)
        {
            Response_Message responcemodel = new Response_Message();
            var result = await assetPMService.DashboardPendingPMItems(requestModel);
            if (result?.list?.Count > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.data = result;
            }
            else if (result.result == (int)ResponseStatusNumber.NotFound)
            {
                responcemodel.success = (int)ResponseStatusNumber.NotFound;
                responcemodel.message = ResponseMessages.nodatafound;
            }
            else
            {
                responcemodel.success = (int)ResponseStatusNumber.Error;
                responcemodel.message = ResponseMessages.Error;
            }
            return Ok(responcemodel);
        }

        /// <summary>
        /// Get Upcoming PMs
        /// </summary>
        /// <param name="filter_type"></param>
        [HttpGet("{filter_type}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> UpComingPMs(int filter_type)
        {
            Response_Message responcemodel = new Response_Message();
            var result = await assetPMService.UpComingPMItems(filter_type);
            if (result?.Count > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.data = result;
            }
            else
            {
                responcemodel.success = (int)ResponseStatusNumber.NotFound;
                responcemodel.message = ResponseMessages.nodatafound;
            }
            return Ok(responcemodel);
        }

        /// <summary>
        /// Get Upcoming PMs Weekly 
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> UpComingPMsWeekly()
        {
            Response_Message responcemodel = new Response_Message();
            var result = await assetPMService.UpComingPMItemsWeekly();
            if (result?.upcomingPMs?.Count > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.data = result;
            }
            else
            {
                responcemodel.success = (int)ResponseStatusNumber.NotFound;
                responcemodel.message = ResponseMessages.nodatafound;
            }
            return Ok(responcemodel);
        }

        /// <summary>
        /// Get Filtered Asset Name Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        [HttpPost]
        public async Task<IActionResult> FilterPendingPMItemsAssetIds([FromBody] FilterPendingPMItemsOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<AssetListResponseModel> response = new ListViewModel<AssetListResponseModel>();
            response = await assetPMService.FilterPendingPMItemsAssetIds(requestModel);
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
        /// Get Filtered Asset PM PLANs Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive" })]
        [HttpPost]
        public async Task<IActionResult> FilterPendingPMItemsPMPlans([FromBody] FilterPendingPMItemsOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<PMPlansResponseModel> response = new ListViewModel<PMPlansResponseModel>();
            response = await assetPMService.FilterPendingPMItemsPMPlans(requestModel);
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
        /// Get Filtered Asset PM Items Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive" })]
        [HttpPost]
        public async Task<IActionResult> FilterPendingPMItemsPMItems([FromBody] FilterPendingPMItemsOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<AssetPMResponseModel> response = new ListViewModel<AssetPMResponseModel>();
            response = await assetPMService.FilterPendingPMItemsPMItems(requestModel);
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
        /// Get Filtered Asset PM By Sites Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive" })]
        [HttpPost]
        public async Task<IActionResult> FilterPendingPMItemsSites([FromBody] FilterPendingPMItemsOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<SitesViewModel> response = new ListViewModel<SitesViewModel>();
            response = await assetPMService.FilterPendingPMItemsSites(requestModel);
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
        /// If PM over due, and still not resolved. Send email PM report to Executive everyday till PM marked done
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SendDuePMReportToExecutive()
        {
            Response_Message responcemodel = new Response_Message();
            assetPMService.SendOverDuePMReportToExecutive();
            return Ok(responcemodel);
        }

        /// <summary>
        /// PM List Section >> Dashboard >> PM Metrics
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> PMDashboardMetrics()
        {
            Response_Message responcemodel = new Response_Message();
            var result = await assetPMService.DashboardPMMetrics();
            if (result != null)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.data = result;
            }
            else
            {
                responcemodel.success = (int)ResponseStatusNumber.Error;
                responcemodel.message = ResponseMessages.Error;
            }
            return Ok(responcemodel);
        }

        /// <summary>
        /// Add/Edit PM >> Get All Service Dealers
        /// </summary>
        [HttpGet("{pageindex?}/{pagesize?}/{searchstring?}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> GetAllServiceDealers(int pageindex = 0, int pagesize = 0, string searchstring = "")
        {
            Response_Message responcemodel = new Response_Message();
            var result = await assetPMService.GetAllServiceDealers(searchstring, pageindex, pagesize);
            if (result != null)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.data = result;
            }
            else
            {
                responcemodel.success = (int)ResponseStatusNumber.Error;
                responcemodel.message = ResponseMessages.Error;
            }
            return Ok(responcemodel);
        }

        /// <summary>
        /// Get Asset Meter Hour History by Asset ID
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> GetAssetMeterHourHistory([FromBody] AssetMeterHourHistoryRequestModel requestModel)
        {
            Response_Message responcemodel = new Response_Message();
            var response = await assetPMService.GetAssetMeterHourHistory(requestModel);
            if (response?.list.Count > 0)
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
        /// Get PM Plans by Class Id to add in asset
        /// </summary>
        [HttpGet("{inspectiontemplate_asset_class_id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        public IActionResult GetPMPlansByClassId(Guid inspectiontemplate_asset_class_id)
        {
            Response_Message responcemodel = new Response_Message();
            List<GetPMPlansByClassIdResponsemodel> response = new List<GetPMPlansByClassIdResponsemodel>();
            response =  assetPMService.GetPMPlansByClassId(inspectiontemplate_asset_class_id);
            if (response != null && response.Count>0)
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
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult LinkAssetPMToWOLine(LinkPMToWOLineRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetPMService.LinkPMToWOLine(requestmodel);
            if (response > 0)
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
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public IActionResult GetAssetPMList(GetAssetPMListRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetPMService.GetAssetPMList(requestmodel);
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

        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public IActionResult GetAssetPMListOptimized(GetAssetPMListRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetPMService.GetAssetPMListOptimized(requestmodel);
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
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> MarkPMcompletedNewflow(MarkPMcompletedNewflowRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await assetPMService.MarkPMcompletedNewflow(requestmodel);
            if (response > 0)
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
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpGet]
        public IActionResult AssetPMCount()
        {
            Response_Message responsemodel = new Response_Message();
            var response =  assetPMService.AssetPMCount();
            if (response !=null)
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


        [HttpPost]
        public async Task<IActionResult> AddUpdatePMItemsMasterData(AddUpdatePMItemsMasterDataRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();

            int response = await assetPMService.AddUpdatePMItemsMasterData(requestModel);

            if (response == (int)(ResponseStatusNumber.Success))
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message= ResponseMessages.RecordUpdated;
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
        public IActionResult GetAssetPMListAssetWise(GetAssetPMListRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetPMService.GetAssetPMListAssetWise(requestmodel);
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public IActionResult GetFilterDropdownAssetPMList(GetAssetPMListRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetPMService.GetFilterDropdownAssetPMList(requestmodel);
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
        public IActionResult PMLastCompletedDateReport(PMLastCompletedDateReportRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetPMService.PMLastCompletedDateReport(requestmodel);
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
        public async Task<IActionResult> BulkCreatePMWOline(BulkCreatePMWOlineRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await assetPMService.BulkCreatePMWOline(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordAdded;
                responsemodel.data = response;
            }
            else if (response == (int)ResponseStatusNumber.AlreadyExists)
            {
                responsemodel.success = (int)ResponseStatusNumber.AlreadyExists;
                responsemodel.message = "Some PMs Are already added to Workorder, Please Download latest Report and Try Again!";
                responsemodel.data = response;
            }
            else if (response == (int)ResponseStatusNumber.NotExists)
            {
                responsemodel.success = (int)ResponseStatusNumber.NotExists;
                responsemodel.message = "AssetPM does not exist!";
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
        public async Task<IActionResult> BulkUpdatePMLastcompleted(BulkupdatePMLastcompletedRequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            int response = await assetPMService.BulkUpdatePMLastcompleted(requestmodel);
            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = "Bulk update PMs are currently in progress we will notify on email once it is successfully completed if you dont receive an email in short time please contact support";
            }
            else if (response == (int)ResponseStatusNumber.NotExists)
            {
                responsemodel.message = "AssetPM does not exist!";
                responsemodel.success = (int)ResponseStatusNumber.NotExists;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        //[TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpGet]
        public async Task<IActionResult> AssignPMtoCeccoSiteScript()
        {
            Response_Message responsemodel = new Response_Message();
            int response = await assetPMService.AssignPMtoCeccoSiteScript();
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
        public async Task<IActionResult> AddPMbySteps(AddPMbyStepsRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await assetPMService.AddPMbySteps(requestmodel);
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
        public IActionResult GetPMWOlineByIDSteps(GetPMWOlineByIDStepsRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetPMService.GetPMWOlineByIDSteps(requestmodel);
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
        public async Task<IActionResult> ScriptForToUpdateAssetPMsDueDateDueInDueFlag()
        {
            Response_Message responcemodel = new Response_Message();
            var result = await assetPMService.ScriptForToUpdateAssetPMsDueDateDueInDueFlag();
            if (result > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.message = ResponseMessages.MarkedCompleted;
            }
            else
            {
                responcemodel.success = (int)ResponseStatusNumber.Error;
                responcemodel.message = ResponseMessages.Error;
            }
            return Ok(responcemodel);
        }

        [HttpGet]
        public async Task<IActionResult> AutomateScriptForToUpdateAssetPMsDueDateDueInDueFlag()
        {
            Response_Message responcemodel = new Response_Message();
            var result = await assetPMService.AutomateScriptForToUpdateAssetPMsDueDateDueInDueFlag();
            if (result > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.message = ResponseMessages.MarkedCompleted;
            }
            else
            {
                responcemodel.success = (int)ResponseStatusNumber.Error;
                responcemodel.message = ResponseMessages.Error;
            }
            return Ok(responcemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> BulkCreateIRAssetPMsAssetInIRWO(BulkCreatePMWOlineRequestmodel requestmodel)
        { 
            Response_Message responsemodel = new Response_Message();
            var response = await assetPMService.BulkCreateIRAssetPMsAssetInIRWO(requestmodel);

            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordAdded;
                responsemodel.data = response;
            }
            else if (response == (int)ResponseStatusNumber.NotExists)
            {
                responsemodel.success = (int)ResponseStatusNumber.NotExists;
                responsemodel.message = "AssetPM does not Exist!!";
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
        public async Task<IActionResult> EnableDisableAssetPMsStatus(EnableDisableAssetPMsStatusRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await assetPMService.EnableDisableAssetPMsStatus(requestModel);
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpGet("{woonboardingassets_id}")]
        public async Task<IActionResult> UpdateWorkStartDateOnEditOpenWOLine(Guid woonboardingassets_id)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await assetPMService.UpdateWorkStartDateOnEditOpenWOLine(woonboardingassets_id);
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive, Company Admin, SuperAdmin" })]
        [HttpGet]
        public async Task<IActionResult> ScriptToAdd1YearAssetPMs()
        {
            Response_Message responsemodel = new Response_Message();
            var response = await assetPMService.ScriptToAdd1YearAssetPMs();
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

    }
}