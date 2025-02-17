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
using Org.BouncyCastle.Ocsp;

namespace Jarvis.Controllers
{
    [Route("api/[controller]/[action]")]
    [TypeFilter(typeof(requestvalidationFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService deviceService;
        private readonly IBaseService baseService;
        private readonly IS3BucketService s3BucketService;
        public AWSConfigurations _options { get; set; }

        public DeviceController(IDeviceService deviceService, IBaseService baseService, IOptions<AWSConfigurations> options)
        {
            this.deviceService = deviceService;
            this.baseService = baseService;
            this._options = options.Value;
            this.s3BucketService = new S3BucketService();
        }

        /// <summary>
        /// Add Device Register
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]DeviceInfoRequestModel requestModel)
        {
            Response_Message response_Message = new Response_Message();
            var response = await deviceService.Register(requestModel);
            if (response != null && response.status == (int)ResponseStatusNumber.Success)
            {
                response_Message.success = response.status;
                response_Message.data = response;
                response_Message.message = ResponseMessages.RecordAdded;
            }
            else if (response != null && response.status == (int)ResponseStatusNumber.AlreadyExists)
            {
                response_Message.success = response.status;
                response_Message.message = ResponseMessages.RecordExist;
            }
            else if (response != null && response.status == (int)ResponseStatusNumber.NotFoundCompanyCode)
            {
                response_Message.success = response.status;
                response_Message.message = ResponseMessages.CompanyNotFound;
            }
            else if (response != null && response.status == (int)ResponseStatusNumber.DeviceUnAuthorized)
            {
                response_Message.success = response.status;
                response_Message.message = ResponseMessages.DeviceUnAuthorizedSupport;
            }
            else
            {
                response_Message.success = response.status;
                response_Message.message = ResponseMessages.Error;
            }
            return Ok(response_Message);
        }

        /// <summary>
        /// Check Device Authentication Status
        /// </summary>
        /// <param name="device_uuid" example="a343180f-6474-42fd-a486-9f9491d91a43"></param>
        [HttpGet("{device_uuid}")]
        public IActionResult GetAuthStatus(Guid device_uuid)
        {
            Response_Message response_Message = new Response_Message();
            int response = deviceService.GetDeviceAuthinticationStatus(device_uuid);
            if (response > 0)
            {
                response_Message.success = response;
                response_Message.message = ResponseMessages.DeviceAuthorized;
            }
            else if (response == (int)ResponseStatusNumber.False)
            {
                response_Message.success = response;
                response_Message.message = ResponseMessages.DeviceUnaAuthorized;
            }
            else
            {
                response_Message.success = response;
                response_Message.message = ResponseMessages.DeviceNotFound;
            }
            return Ok(response_Message);
        }

        /// <summary>
        /// Update Device Info By ID
        /// </summary>
        /// <param name="device_info_id" example="a343180f-6474-42fd-a486-9f9491d91a43"></param>
        [HttpPut("{device_info_id}")]
        public async Task<IActionResult> Update(int device_info_id, [FromBody]DeviceInfoRequestModel requestModel)
        {
            Response_Message response_Message = new Response_Message();
            var response = await deviceService.Update(device_info_id, requestModel);
            if (response > 0)
            {
                response_Message.success = response;
                response_Message.message = ResponseMessages.RecordUpdated;
            }
            else if (response == (int)ResponseStatusNumber.NotFound)
            {
                response_Message.success = response;
                response_Message.message = ResponseMessages.nodatafound;
            }
            else
            {
                response_Message.success = response;
                response_Message.message = ResponseMessages.Error;
            }
            return Ok(response_Message);
        }

        /// <summary>
        /// Update Device Status
        /// </summary>
        /// <param name="device_info_id" example="a343180f-6474-42fd-a486-9f9491d91a43"></param>
        [HttpPut("{device_info_id}")]
        public async Task<IActionResult> UpdateStatus(int device_info_id, UpdateDeviceStatusRequestModel requestModel)
        {
            Response_Message response_Message = new Response_Message();
            int response = await deviceService.UpdateDeviceStatus(device_info_id, requestModel);
            if (response > 0)
            {
                response_Message.success = response;
                response_Message.message = ResponseMessages.RecordUpdated;
            }
            else if (response == (int)ResponseStatusNumber.NotFound)
            {
                response_Message.success = response;
                response_Message.message = ResponseMessages.nodatafound;
            }
            else
            {
                response_Message.success = response;
                response_Message.message = ResponseMessages.Error;
            }
            return Ok(response_Message);
        }

        /// <summary>
        /// Remove Sync Record By Weekly (Seduler Task)
        /// </summary>
        [HttpGet]
        public IActionResult RemoveSyncRecord()
        {
            Response_Message responsemodel = new Response_Message();
            deviceService.RemoveSyncRecord();
            responsemodel.success = (int)ResponseStatusNumber.Success;
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Filtered Device 
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> FilterDevice([FromBody] FilterDeviceRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<DeviceInfoViewModel> response = new ListViewModel<DeviceInfoViewModel>();
            response = await deviceService.FilterDevice(requestModel);
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
        /// Get Filtered Device Type Option Search
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> FilterDeviceTypeOptions([FromBody] FilterDeviceRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<string> response = new ListViewModel<string>();
            response = await deviceService.FilterDeviceTypeOptions(requestModel);
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
        /// Get Filtered Device Brand Option Search
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> FilterDeviceBrandOptions([FromBody] FilterDeviceRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<string> response = new ListViewModel<string>();
            response = await deviceService.FilterDeviceBrandOptions(requestModel);
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
        /// Get Filtered Device Model Option Search
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> FilterDeviceModelOptions([FromBody] FilterDeviceRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<string> response = new ListViewModel<string>();
            response = await deviceService.FilterDeviceModelOptions(requestModel);
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
        /// Get Filtered Device OS Option Search
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> FilterDeviceOSOptions([FromBody] FilterDeviceRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<string> response = new ListViewModel<string>();
            response = await deviceService.FilterDeviceOSOptions(requestModel);
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
        /// Get Filtered Device Sites Option Search
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> FilterDeviceSitesOptions([FromBody] FilterDeviceRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<SitesViewModel> response = new ListViewModel<SitesViewModel>();
            response = await deviceService.FilterDeviceSitesOptions(requestModel);
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
        /// Get Filtered Device Company Option Search
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> FilterDeviceCompanyOptions([FromBody] FilterDeviceRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<CompanyViewModel> response = new ListViewModel<CompanyViewModel>();
            response = await deviceService.FilterDeviceCompanyOptions(requestModel);
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
        /// Get All Device Model For Filter COlumn
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpGet]
        public IActionResult AllDevicesModel()
        {
            Response_Message response = new Response_Message();
            List<string> responseModels = deviceService.GetAllDeviceModelList();
            response.data = responseModels;
            response.success = (int)ResponseStatusNumber.Success;
            return Ok(response);
        }

        /// <summary>
        /// Get All Device OS For Filter COlumn
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpGet]
        public IActionResult AllDevicesOS()
        {
            Response_Message response = new Response_Message();
            List<string> responseModels = deviceService.GetAllDeviceOSList();
            response.data = responseModels;
            response.success = (int)ResponseStatusNumber.Success;
            return Ok(response);
        }

        /// <summary>
        /// Get All Device Brands For Filter COlumn
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpGet]
        public IActionResult AllDevicesBrand()
        {
            Response_Message response = new Response_Message();
            List<string> responseModels = deviceService.GetAllDeviceBrandList();
            response.data = responseModels;
            response.success = (int)ResponseStatusNumber.Success;
            return Ok(response);
        }


        /// <summary>
        /// Get All Device Types For Filter COlumn
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpGet]
        public IActionResult AllDevicesTypes()
        {
            Response_Message response = new Response_Message();
            List<string> responseModels = deviceService.GetAllDeviceTypesList();
            response.data = responseModels;
            response.success = (int)ResponseStatusNumber.Success;
            return Ok(response);
        }

        /// <summary>
        /// Update App Version By Device Info ID
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Operator,Maintenance staff,Company Admin" })]
        public async Task<IActionResult> UpdateAppVersion()
        {
            Response_Message response_Message = new Response_Message();
            var response = await deviceService.UpdateDeviceAppVersion(UpdatedGenericRequestmodel.CurrentUser.device_uuid, UpdatedGenericRequestmodel.CurrentUser.app_version);
            if (response > 0)
            {
                response_Message.success = response;
                response_Message.message = ResponseMessages.RecordUpdated;
            }
            else if (response == (int)ResponseStatusNumber.NotFound)
            {
                response_Message.success = response;
                response_Message.message = ResponseMessages.nodatafound;
            }
            else if (response == (int)ResponseStatusNumber.InvalidData)
            {
                response_Message.success = response;
                response_Message.message = ResponseMessages.Error;
            }
            else
            {
                response_Message.success = response;
                response_Message.message = ResponseMessages.Error;
            }
            return Ok(response_Message);
        }

        /// <summary>
        /// Upload Attachments for Work Order
        /// </summary>
        /// 
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadDeviceLogstoS3()
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
                var get_device_code = deviceService.GetdevicecodeByDeviceId(UpdatedGenericRequestmodel.CurrentUser.device_uuid);
                string folder_name = "";
                if (get_device_code != null)
                {
                    folder_name = get_device_code.device_code;
                }
                var list = await s3BucketService.UploadmobilelogsToS3(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.mobile_logs_bucket, folder_name);

                if (list != null && list.Count() >0)
                {
                    responsemodel.success = 1;
                    responsemodel.data = list;
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


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UpdateDeviceGeoLocation(UpdateDeviceGeoLocationRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await deviceService.UpdateDeviceGeoLocation(requestModel);
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

    }
}