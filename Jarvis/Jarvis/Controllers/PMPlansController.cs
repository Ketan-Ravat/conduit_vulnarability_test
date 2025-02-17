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

namespace Jarvis.Controllers
{
    [TypeFilter(typeof(requestvalidationFilter))]
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiController]
    public class PMPlansController : ControllerBase
    {
        public AWSConfigurations _options { get; set; }
        private readonly IPMPlansService plansService;
        public PMPlansController(IPMPlansService _plansService)
        {
            this.plansService = _plansService;
        }
        /// <summary>
        /// Add PM Plan
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive,SuperAdmin" })]
        public async Task<IActionResult> AddUpdatePMPlan([FromBody] PMPlansRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await plansService.AddUpdatePMPlan(request);
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
        /// Get All PM Plans by Category ID
        /// </summary>
        [HttpGet("{pm_category_id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive,SuperAdmin" })]
        public async Task<IActionResult> Get(Guid pm_category_id)
        {
            Response_Message responcemodel = new Response_Message();
            ListViewModel<PMPlansResponseModel> response = new ListViewModel<PMPlansResponseModel>();
            response = await plansService.GetAllPMPlans(pm_category_id);
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
        /// Get PM Plan by Plan ID
        /// </summary>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive,SuperAdmin" })]
        public async Task<IActionResult> GetById(Guid id)
        {
            Response_Message responcemodel = new Response_Message();
            PMPlansResponseModel response = new PMPlansResponseModel();
            response = await plansService.GetPMPlanByID(id);
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
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        public async Task<IActionResult> Delete(Guid id)
        {
            Response_Message responcemodel = new Response_Message();
            var result = await plansService.DeletePMPlanByID(id);
            if (result > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.message = ResponseMessages.PMCategoryDeleted;
            }
            else if (result == (int)ResponseStatusNumber.PMPlanIsLinked)
            {
                responcemodel.success = (int)ResponseStatusNumber.PMPlanIsLinked;
                responcemodel.message = ResponseMessages.PMPlanIsLinked;
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
        /// Duplicate PM
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive,SuperAdmin" })]
        public async Task<IActionResult> Duplicate(DuplicatePMRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            var result = await plansService.DuplicatePMPlan(request);
            if (result > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.message = ResponseMessages.RecordAdded;
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
        /// Duplicate PM
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive,SuperAdmin" })]
        public async Task<IActionResult> MarkDefaultPMPlan(MarkDefaultPMPlanRequestmodel request)
        {
            Response_Message responcemodel = new Response_Message();
            var result = await plansService.MarkDefaultPMPlan(request);
            if (result > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.message = ResponseMessages.RecordAdded;
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
    }
}