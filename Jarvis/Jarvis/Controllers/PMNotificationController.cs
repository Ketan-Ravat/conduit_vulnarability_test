using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jarvis.Filter;
using Jarvis.Resource;
using Jarvis.Service.Abstract;
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
    public class PMNotificationController : ControllerBase {
        public AWSConfigurations _options { get; set; }
        private readonly IPMNotificationService pmNotificationService;
        public PMNotificationController(IPMNotificationService _pmNotificationService)
        {
            this.pmNotificationService = _pmNotificationService;
        }
        /// <summary>
        /// Add/Update PM Notification Configuration from Company Admin Portal
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin" })]
        public async Task<IActionResult> AddUpdatePMNotification([FromBody] CompanyPMNotificationRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await pmNotificationService.AddUpdatePMNotification(request);
                if (result.response_status > 0)
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
        /// Add/Update PM Notification Configuration from Manager Portal for Asset
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> AddUpdateAssetPMNotification([FromBody] AssetPMNotificationRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await pmNotificationService.AddUpdateAssetPMNotification(request);
                if (result.response_status > 0)
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
        /// Get PM Notification Configuration from Company Admin Portal
        /// </summary>
        [HttpGet("{company_id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin" })]
        public async Task<IActionResult> GetPMNotification(Guid company_id)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await pmNotificationService.GetPMNotification(company_id);
                if (result.response_status > 0)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordAdded;
                    responcemodel.data = result;
                }
                else if (result.response_status == (int)ResponseStatusNumber.NotFound)
                {
                    responcemodel.success = (int)ResponseStatusNumber.NotFound;
                    responcemodel.message = ResponseMessages.nodatafound;
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
        /// Get PM Notification Configuration from Company Admin Portal
        /// </summary>
        [HttpGet("{asset_id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> GetAssetPMNotification(Guid asset_id)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await pmNotificationService.GetAssetPMNotification(asset_id);
                if (result.response_status > 0)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.data = result;
                }
                else if (result.response_status == (int)ResponseStatusNumber.NotFound)
                {
                    responcemodel.success = (int)ResponseStatusNumber.NotFound;
                    responcemodel.message = ResponseMessages.nodatafound;
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
        /// Execute PM Notification
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExecutePMNotification()
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await pmNotificationService.ExecutePMNotification();
                if (result > 0)
                {
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                    responcemodel.message = ResponseMessages.RecordAdded;
                    responcemodel.data = result;
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
        /// Update the PM Item notification status for Manager
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive" })]
        public async Task<IActionResult> TriggerPMItemNotification([FromQuery] Guid trigger_id, [FromQuery] bool is_disabled)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await pmNotificationService.TriggerPMItemNotification(trigger_id, is_disabled);
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
        /// Execute PM Email Notification for vendors
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExecutePMNotificationForVendors()
        {
            Response_Message responcemodel = new Response_Message();
            var result = await pmNotificationService.ExecutePMNotificationForVendors();
            responcemodel.success = (int)ResponseStatusNumber.Success;
            responcemodel.message = ResponseMessages.RecordAdded;
            responcemodel.data = result;
            return Ok(responcemodel);
        }
    }
}