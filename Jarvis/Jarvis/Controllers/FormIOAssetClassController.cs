using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Microsoft.Extensions.Options;

namespace Jarvis.Controllers
{
    [TypeFilter(typeof(requestvalidationFilter))]
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class FormIOAssetClassController : ControllerBase
    {
        public AWSConfigurations _options { get; set; }
        private readonly IS3BucketService s3BucketService;
        private readonly IFormIOAssetClassService FormIOAssetClassService;
        public FormIOAssetClassController(IFormIOAssetClassService IFormIOAssetClassService, IOptions<AWSConfigurations> options)
        {
            this.FormIOAssetClassService = IFormIOAssetClassService;
            this._options = options.Value;
            this.s3BucketService = new S3BucketService();
        }
        /// <summary>
        /// Add Update Asset Form IO
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        public IActionResult GetAllAssetClass([FromBody] GetAllAssetClassRequestmodel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result =  FormIOAssetClassService.GetAllAssetClass(request);
                if (result !=null && result.list!=null && result.list.Count>0)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordAdded;
                    responcemodel.data = result;
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.NotFound;
                    responcemodel.message = ResponseMessages.nodatafound;
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
        public async Task<IActionResult> AddAssetClass([FromBody] AddAssetClassRequestmodel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await FormIOAssetClassService.AddAssetClass(request);
                if (result !=null && result.inspectiontemplate_asset_class_id!=null && result.inspectiontemplate_asset_class_id!= Guid.Empty)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordAdded;
                    responcemodel.data = result;
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.AlreadyExists;
                    responcemodel.message = "Asset Class Already Exist";
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
        /// Get Asset By ID
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpGet("{inspectiontemplate_asset_class_id}")]
        public IActionResult GetFormsByAssetclassID(string inspectiontemplate_asset_class_id)
        {
            Response_Message responsemodel = new Response_Message();
            var response =  FormIOAssetClassService.GetFormsByAssetclassID(inspectiontemplate_asset_class_id);
            if (response != null && response.Count > 0)
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
        /// Get Asset By ID
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpGet("{form_id}")]
        public IActionResult GetFormIOFormById(string form_id)
        {
            Response_Message responsemodel = new Response_Message();
            var response = FormIOAssetClassService.GetFormIOFormById(Guid.Parse(form_id));
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
        /// Get Asset By ID
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpGet("{inspectiontemplate_asset_class_id}")]
        public IActionResult GetFormPropertiesByAssetclassID(string inspectiontemplate_asset_class_id)
        {
            Response_Message responsemodel = new Response_Message();
            var response = FormIOAssetClassService.GetFormPropertiesByAssetclassID(Guid.Parse(inspectiontemplate_asset_class_id));
            if (response != null && response.Count>0)
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
        public async Task<IActionResult> AddFormInAssetClass([FromBody] AddFormInAssetClassRequestmodel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await FormIOAssetClassService.AddFormInAssetClass(request);
                if (result== (int)ResponseStatusNumber.Success)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordAdded;
                    responcemodel.data = result;
                }
                else if (result == (int)ResponseStatusNumber.AlreadyExists)
                {
                    responcemodel.success = (int)ResponseStatusNumber.AlreadyExists;
                    responcemodel.message = "Form Already Exist";
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.NotFound;
                    responcemodel.message = ResponseMessages.nodatafound;
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
        /// Get Asset By ID
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpGet("{inspectiontemplate_asset_class_id}")]
        public IActionResult GetFormListtoAddByAssetclassID(string inspectiontemplate_asset_class_id)
        {
            Response_Message responsemodel = new Response_Message();
            var response = FormIOAssetClassService.GetFormListtoAddByAssetclassID(Guid.Parse(inspectiontemplate_asset_class_id));
            if (response != null && response.Count > 0)
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPost]
        public IActionResult GetFormListtoAddByAssetclassID_V2(GetFormListtoAddByAssetclassIDRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = FormIOAssetClassService.GetFormListtoAddByAssetclassID_V2(requestModel);
            if (response != null && response.Count > 0)
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
        public async Task<IActionResult> DeleteFormFromAssetClass([FromBody] DeleteFormFromAssetClassRequestmodel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await FormIOAssetClassService.DeleteFormFromAssetClass(request);
                if (result == (int)ResponseStatusNumber.Success)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordUpdated;
                    responcemodel.data = result;
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.NotFound;
                    responcemodel.message = ResponseMessages.nodatafound;
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
        /// Get Asset By ID
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpGet()]
        public IActionResult GetAllAssetClassCodes()
        {
            Response_Message responsemodel = new Response_Message();
            var response = FormIOAssetClassService.GetAllAssetClassCodes();
            if (response != null && response.Count > 0)
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
        public async Task<IActionResult> DeleteAssetClass([FromBody] DeleteAssetClassRequestmodel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await FormIOAssetClassService.DeleteAssetClass(request);
                if (result == (int)ResponseStatusNumber.Success)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordUpdated;
                    responcemodel.data = result;
                }
                else if (result == (int)ResponseStatusNumber.asset_class_already_used)
                {
                    responcemodel.success = (int)ResponseStatusNumber.asset_class_already_used;
                    responcemodel.message = "Asset class is used in WO";
                }
                else
                {
                    responcemodel.message = ResponseMessages.Error;
                    responcemodel.success = (int)ResponseStatusNumber.Error;
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
        public async Task<IActionResult> UpdateNamePlateinfo([FromBody] UpdateNamePlateinfoRequestmodel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await FormIOAssetClassService.UpdateNamePlateinfo(request);
                if (result == (int)ResponseStatusNumber.Success)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordUpdated;
                    responcemodel.data = result;
                }
                else
                {
                    responcemodel.message = ResponseMessages.Error;
                    responcemodel.success = (int)ResponseStatusNumber.Error;
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
        /// Get Asset By ID
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpGet("{inspectiontemplate_asset_class_id}")]
        public IActionResult GetFormNameplateInfobyClassId(string inspectiontemplate_asset_class_id)
        {
            Response_Message responsemodel = new Response_Message();
            var response = FormIOAssetClassService.GetFormNameplateInfobyClassId(Guid.Parse(inspectiontemplate_asset_class_id));
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

    }
}
