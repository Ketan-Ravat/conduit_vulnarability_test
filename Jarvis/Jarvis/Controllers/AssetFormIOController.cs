using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jarvis.Filter;
using Jarvis.Resource;
using Jarvis.Service.Abstract;
using Jarvis.Service.Concrete;
using Jarvis.Shared.Helper;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Status = Jarvis.Shared.StatusEnums.Status;

namespace Jarvis.Controllers
{
    [TypeFilter(typeof(requestvalidationFilter))]
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class AssetFormIOController : ControllerBase
    {
        public AWSConfigurations _options { get; set; }
        private readonly IS3BucketService s3BucketService;
        private readonly IAssetFormIOService AssetFormIOService;
        private readonly IAssetService assetService;
        public AssetFormIOController(IAssetFormIOService IAssetFormIOService, IOptions<AWSConfigurations> options, IAssetService assetService)
        {
            this.AssetFormIOService = IAssetFormIOService;
            this._options = options.Value;
            this.s3BucketService = new S3BucketService();
            this.assetService = assetService;
        }

        /// <summary>
        /// Get Asset By ID
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpGet("{asset_id}")]
        public async Task<IActionResult> GetAssetFormIOByAssetId(string asset_id)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await AssetFormIOService.GetAssetFormIOByAssetId(Guid.Parse(asset_id));
            if (response.asset_id != null && response.asset_id != Guid.Empty)
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
        /// Add Update Asset Form IO
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> AddUpdateAssetFormIO([FromBody] AssetFormIORequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await AssetFormIOService.AddUpdateAssetFormIO(request,_options.LVCB_Form_id);
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
        /// Get Inspection form list by asset id
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpPost()]
        public  IActionResult GetAllAssetInspectionListByAssetId([FromBody] GetAllAssetInspectionListByAssetIdRequestModel request)
        {
            Response_Message responsemodel = new Response_Message();
            var result =  AssetFormIOService.GetAllAssetTemplateList(request);
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
        /// Check Asset formio Inspection Report Status
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpGet]
        public async Task<IActionResult> ReportStatus([FromQuery] string asset_form_id)
        {
            Response_Message response = new Response_Message();
            AssetFormIOReportStatusResponsemodel responseModels = await AssetFormIOService.GetReportStatus(asset_form_id);
            if (responseModels != null && responseModels.pdf_report_status == (int)Status.ReportCompleted)
            {
                response.data = responseModels;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else if (responseModels != null && responseModels.pdf_report_status == (int)Status.ReportInProgress)
            {
                response.data = responseModels;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else if (responseModels != null && responseModels.pdf_report_status == (int)Status.ReportFailed)
            {
                response.data = responseModels;
                response.success = (int)ResponseStatusNumber.Success;
                //response.message = "The previous report completion attempt failed. Please try again later. If the issue persists, please contact support!";
                response.message = "The previous report completion attempt failed. Logs : " + responseModels.report_lambda_logs;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }
        /// <summary>
        /// Add Update Asset Form IO
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> UpdateOnlyAssetFormIO([FromBody] UpdateOnlyAssetFormIORequestmodel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await AssetFormIOService.UpdateOnlyAssetFormIO(request);
                if (result > 0)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordUpdated;
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
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> AddUpdateAssetFormIOOffline([FromBody] AddUpdateAssetFormIOOfflineRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await AssetFormIOService.AddUpdateAssetFormIOOffline(request, _options.offline_sync_bucket, _options.S3_aws_access_key, _options.S3_aws_secret_key);
                if (result !=null && result.success == (int)ResponseStatusNumber.Success)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordAdded;
                    responcemodel.data = result;
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.Error;
                    responcemodel.message = ResponseMessages.Error;
                    responcemodel.data = result;
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

        [HttpGet]
        public async Task<IActionResult> ExractandStoreOnlydatafromOldForm()
        {
            Response_Message response = new Response_Message();
            var responseModels =await  AssetFormIOService.ExractandStoreOnlydatafromOldForm();
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
        /// Add Update Asset Form IO
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> UpdateAssetInfo([FromBody] UpdateAssetInfoRequestmodel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await AssetFormIOService.UpdateAssetInfo(request);
                if (result == (int)ResponseStatusNumber.Success)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordAdded;
                    responcemodel.data = result;
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.Error;
                    responcemodel.message = ResponseMessages.Error;
                    responcemodel.data = result;
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
        public async Task<IActionResult> GetFormIOInsulationResistanceTest([FromBody] FormIOInsulationResistanceTestRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result =  AssetFormIOService.GetFormIOInsulationResistanceTest(request);
                if (result !=null)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                   // responcemodel.message = ResponseMessages.RecordAdded;
                    responcemodel.data = result;
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.Error;
                    responcemodel.message = ResponseMessages.Error;
                    responcemodel.data = result;
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
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> changeassetformiostatus([FromBody] changeassetformiostatusRequestmodel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await AssetFormIOService.changeassetformiostatus(request);
                if (result>0)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordUpdated;
                    responcemodel.data = result;
                }
                else if(result == (int)ResponseStatusNumber.WO_completed)
                {
                    responcemodel.success = (int)ResponseStatusNumber.WO_completed;
                    responcemodel.message = "Workorder Is Completed";
                }
                else if (result == (int)ResponseStatusNumber.form_is_not_completed)
                {
                    responcemodel.success = (int)ResponseStatusNumber.form_is_not_completed;
                    responcemodel.message = "Form needs to be Completed";
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.Error;
                    responcemodel.message = ResponseMessages.Error;
                    responcemodel.data = result;
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
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> ChangeAssetFormIOStatusFormultiple([FromBody] ChangeAssetFormIOStatusFormultipleRequestmodel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await AssetFormIOService.ChangeAssetFormIOStatusFormultiple(request);
                if (result > 0)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordUpdated;
                    responcemodel.data = result;
                }
                else if (result == (int)ResponseStatusNumber.WO_completed)
                {
                    responcemodel.success = (int)ResponseStatusNumber.WO_completed;
                    responcemodel.message = "Workorder Is Completed";
                }
                else if (result == (int)ResponseStatusNumber.form_is_not_completed)
                {
                    responcemodel.success = (int)ResponseStatusNumber.form_is_not_completed;
                    responcemodel.message = "Form needs to be Completed";
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.Error;
                    responcemodel.message = ResponseMessages.Error;
                    responcemodel.data = result;
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

        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> GetAssetsForSubmittedFilterOptions()
        {
            Response_Message response = new Response_Message();
            var responseModels =  AssetFormIOService.GetAssetsForSubmittedFilterOptions();
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

        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> GetWorkOrdersForSubmittedFilterOptions()
        {
            Response_Message response = new Response_Message();
            var responseModels = AssetFormIOService.GetWorkOrdersForSubmittedFilterOptions();
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

        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> GetInspectedForSubmittedFilterOptions()
        {
            Response_Message response = new Response_Message();
            var responseModels = AssetFormIOService.GetInspectedForSubmittedFilterOptions();
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
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> GetApprovedForSubmittedFilterOptions()
        {
            Response_Message response = new Response_Message();
            var responseModels = AssetFormIOService.GetApprovedForSubmittedFilterOptions();
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
        /// Add Update Asset Form IO
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Technician,Executive" })]
        public IActionResult GetAssetFormJsonbyId([FromBody] GetAssetFormJsonbyIdRequestmodel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result =  AssetFormIOService.GetAssetFormJsonbyId(request);
                if (result !=null)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordUpdated;
                    responcemodel.data = result;
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.Error;
                    responcemodel.message = ResponseMessages.Error;
                    responcemodel.data = result;
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
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Technician,Executive" })]
        public async Task<IActionResult> updateformjson()
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await AssetFormIOService.updateformjson();
                if (result != null)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordUpdated;
                    responcemodel.data = result;
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.Error;
                    responcemodel.message = ResponseMessages.Error;
                    responcemodel.data = result;
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
        /// Get Completed Assets from WO for export in excel
        /// </summary>
        /// <param name="requestmodel" example=""></param>
       // [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> ReplaceAssetformIOJson(Guid siteId,Guid companyId)
        {
            Response_Message responsemodel = new Response_Message();
            var response = AssetFormIOService.ReplaceAssetformIOJson(siteId,companyId);
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
        /// View Work Order By ID 
        /// </summary>
        /// <param name="wo_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{asset_form_id}")]
        public IActionResult GetAssetformByID(string asset_form_id)
        {
            Response_Message responsemodel = new Response_Message();
            GetAssetformByIDResponsemodel response = new GetAssetformByIDResponsemodel();
            response = AssetFormIOService.GetAssetformByID(asset_form_id);
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
        [HttpPost]
        public IActionResult GetAssetformByIDForBulkReport(GetAssetformByIDForBulkReportRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            GetAssetformByIDForBulkReportResponsemodel response = new GetAssetformByIDForBulkReportResponsemodel();
            response = AssetFormIOService.GetAssetformByIDForBulkReport(requestmodel);
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
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public IActionResult GetAssetformEquipmentList(GetAssetformEquipmentListRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            
            var response = AssetFormIOService.GetAssetformEquipmentList(requestmodel);
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


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin",  })]
        [HttpPost]
        public IActionResult GetAllEquipmentList(GetAllEquipmentListRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = AssetFormIOService.GetAllEquipmentListService(requestmodel);
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


        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician,SuperAdmin" })]
        public async Task<IActionResult> AddUpdateEquipment(AddUpdateEquipmentRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await AssetFormIOService.AddUpdateEquipmentService(request);
                if (result != null && result.equipment_id != null && result.equipment_id != Guid.Empty && result.success == (int)ResponseStatusNumber.Success)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordAdded;
                    responcemodel.data = result;
                }
                else if (result != null && result.success == (int)ResponseStatusNumber.equipment_number_must_be_unique)
                {
                    responcemodel.success = (int)ResponseStatusNumber.equipment_number_must_be_unique;
                    responcemodel.message = "Equipment number must be unique";
                    responcemodel.data = result;
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.Error;
                    responcemodel.message = ResponseMessages.Error;
                    responcemodel.data = result;
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

        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician,SuperAdmin" })]
        public async Task<IActionResult> DeleteEquipment(Guid equipmentId)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await AssetFormIOService.DeleteEquipmentService(equipmentId);
                if (result == (int)ResponseStatusNumber.Success)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordDeleted;
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

        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        public async Task<IActionResult> FilterAttributesEquipment()
        {
            Response_Message responsemodel = new Response_Message();

            FilterAttributesEquipmentResponseModel response = new FilterAttributesEquipmentResponseModel();
            response = await AssetFormIOService.FilterAttributesEquipmentService();

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
        [HttpGet]
        public IActionResult GetAssetFormIOByAssetID(Guid asset_id)
        {
            Response_Message response = new Response_Message();

            GetAssetFormIOByAssetIdResponseModel responseModel = new GetAssetFormIOByAssetIdResponseModel();

            responseModel = AssetFormIOService.GetAssetFormIOByAssetID(asset_id);

            if (responseModel != null )
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }

            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpGet]
        public async Task<IActionResult> ScriptForAddNFPA70BToAssets()
        {
            Response_Message response = new Response_Message();

            int res = await AssetFormIOService.scriptforgehealtcare();
            if (res > 0)
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

        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> GetAssetsForSubmittedFilterOptionsByStatus(GetAssetsForSubmittedFilterOptionsByStatusRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            var responseModels = AssetFormIOService.GetAssetsForSubmittedFilterOptionsByStatus(requestModel);
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

        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> GetWorkOrdersForSubmittedFilterOptionsByStatus(GetAssetsForSubmittedFilterOptionsByStatusRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            var responseModels = AssetFormIOService.GetWorkOrdersForSubmittedFilterOptionsByStatus(requestModel);
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

        [HttpGet]
        //[TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        public async Task<IActionResult> UpdateEquipmentCalibrationStatusByDateAndInterval()
        {
            Response_Message responsemodel = new Response_Message();

            int response = await AssetFormIOService.UpdateEquipmentCalibrationStatusByDateAndInterval();
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


        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> GenerateBulkNetaReport(GenerateBulkNetaReportRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            string aws_access_key = _options.sqs_aws_access_key;
            string aws_secret_key = _options.sqs_aws_secret_key;
            var res = await AssetFormIOService.GenerateBulkNetaReport(requestModel,aws_access_key,aws_secret_key);
            if (res == (int)ResponseStatusNumber.Success)
            {
                response.data = res;
                response.success = (int)ResponseStatusNumber.Success;
                response.message = "Report request generated successfully! You can find the status in the Reviews -> Reports section. Once ready, download it from the Reports section.";
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }

        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public async Task<IActionResult> GetAllNetaInspectionBulkReportTrackingList(GetAllNetaInspectionBulkReportTrackingListRequestModel requestModel)
        {
            Response_Message responseModel = new Response_Message();
            var response = await AssetFormIOService.GetAllNetaInspectionBulkReportTrackingList(requestModel);
            if (response.list != null && response.list.Count > 0)
            {
                responseModel.data = response;
                responseModel.success = (int)ResponseStatusNumber.Success;
                responseModel.message = ResponseMessages.SuccessMessage;
            }
            else
            {
                responseModel.success = (int)ResponseStatusNumber.NotFound;
                responseModel.message = ResponseMessages.nodatafound;
            }
            return Ok(responseModel);
        }

        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Executive,Manager,Technician,Company Admin" })]
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadDocumentForAssetClassForm()
        {
            //AssetService assetService = new AssetService(mapper);
            Response_Message response = new Response_Message();
            //try
            //{
            List<IFormFile> filesToUpload = new List<IFormFile>();
            foreach (var file1 in Request.Form.Files)
            {
                //Logger.Log("Images ", file1.FileName.ToString());
                filesToUpload.Add(file1);
            }

            var list = await s3BucketService.UploadDocumentByFilename(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.jinja_report_template_bucket);
            if (list.Count > 0)
            {
                string asset_class_id = Request.Form["inspectiontemplate_asset_class_id"];
                //string stringsite = Request.Form["site_id"];
                ImagesListObjectViewModel images = new ImagesListObjectViewModel();
                List<s3uploaded_files> images_names = new List<s3uploaded_files>();
                foreach (var item in list)
                {
                    s3uploaded_files s3uploaded_files = new s3uploaded_files();
                    s3uploaded_files.user_uploaded_file_name = item.user_uploaded_file_name;
                    s3uploaded_files.s3_file_name = item.s3_file_name;
                    images_names.Add(s3uploaded_files);
                }
                string filename = list.FirstOrDefault().s3_file_name;
                var pdf_url = UrlGenerator.GetAssetClassJinjaTemplateURL(filename , _options.jinja_report_template_bucket);
                var result = await assetService.UpdateAssetClassPDFUrl(Guid.Parse(asset_class_id), pdf_url);
                if (result > 0)
                {
                    response.success = (int)ResponseStatusNumber.Success;
                    response.message = ResponseMessages.SuccessMessage;
                    response.data = pdf_url;
                }
                else if (result == (int)ResponseStatusNumber.NotFound)
                {
                    response.success = (int)ResponseStatusNumber.NotFound;
                    response.message = ResponseMessages.nodatafound;
                }
                else
                {
                    response.success = (int)ResponseStatusNumber.Error;
                    response.message = ResponseMessages.Error;
                }
            }
            
            return Ok(response);
        }

    }
}
