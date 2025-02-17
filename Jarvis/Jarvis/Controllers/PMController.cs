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
    public class PMController : ControllerBase
    {
        public AWSConfigurations _options { get; set; }
        private readonly IPMService pmService;
        private readonly IS3BucketService s3BucketService;
        public PMController(IPMService _pmService, IOptions<AWSConfigurations> options)
        {
            this.pmService = _pmService;
            this._options = options.Value;
            this.s3BucketService = new S3BucketService();
        }

        /// <summary>
        /// Upload Attachments for PM
        /// </summary>
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadAttachment()
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

                var list = await s3BucketService.UploadAttachment(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.pmattachment_bucket_name);

                if (list.Count > 0)
                {
                    var items = new PMAttachmentsResponseModel
                    {
                        filename = list[0],
                        file_url = UrlGenerator.GetPMAttachmentURL(list[0]),
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
        /// Add PM
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        public async Task<IActionResult> AddUpdatePM([FromBody] AddPMRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await pmService.AddUpdatePM(request);
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
        /// Get All PM by Plan ID
        /// </summary>
        [HttpGet("{pm_plan_id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        public async Task<IActionResult> Get(Guid pm_plan_id)
        {
            Response_Message responcemodel = new Response_Message();
            ListViewModel<GetAllPMsByPlanIdResponsemodel> response = new ListViewModel<GetAllPMsByPlanIdResponsemodel>();
            response = await pmService.GetAllPMsByPlanId(pm_plan_id);
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
        /// Get PM by ID
        /// </summary>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        public async Task<IActionResult> GetById(Guid id)
        {
            Response_Message responcemodel = new Response_Message();
            PMResponseModel response = new PMResponseModel();
            response = await pmService.GetPMByID(id);
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
        /// Delete PM By ID
        /// </summary>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        public async Task<IActionResult> Delete(Guid id)
        {
            Response_Message responcemodel = new Response_Message();
            var result = await pmService.DeletePMByID(id);
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
        /// Move PM From One Plan to Another Plan
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        public async Task<IActionResult> MovePM([FromBody] MovePMRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            var result = await pmService.MovePM(request);
            if (result.response_status > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.message = ResponseMessages.RecordUpdated;
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
            return Ok(responcemodel);
        }

        /// <summary>
        /// Delete PM By ID
        /// </summary>
        [HttpGet()]
        public async Task<IActionResult> createPMforOldAssetClass()
        {
            Response_Message responcemodel = new Response_Message();
            var result = await pmService.createPMforOldAssetClass();
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

        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        public IActionResult GetPMsListByAssetId(GetPMsListByAssetClassIdRequestModel requestModel)
        {
            Response_Message responcemodel = new Response_Message();
            var response = pmService.GetPMsListByAssetClassId(requestModel);
            if (response.Count > 0)
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

        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        public IActionResult GetPMsListByAssetClassId(GetPMsListByAssetClassIdRequestModel requestModel)
        {
            Response_Message responcemodel = new Response_Message();
            var response = pmService.GetPMsListByAssetClassId(requestModel);
            if (response.Count > 0)
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


        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        public async Task<IActionResult> ManuallyAssignAnyPMtoWO(AssignPMsToAssetRequestModel requestModel)
        {
            Response_Message responcemodel = new Response_Message();
            var response = await pmService.ManuallyAssignAnyPMtoWO(requestModel);

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
    }
}