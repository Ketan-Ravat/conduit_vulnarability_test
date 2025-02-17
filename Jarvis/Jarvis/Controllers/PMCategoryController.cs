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
    public class PMCategoryController : ControllerBase
    {
        public AWSConfigurations _options { get; set; }
        private readonly IPMCategoryService categoryService;
        public PMCategoryController(IPMCategoryService _categoryService)
        {
            this.categoryService = _categoryService;
        }
        /// <summary>
        /// Add PM Category
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        public async Task<IActionResult> AddUpdatePMCategory([FromBody] PMCategoryRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await categoryService.AddUpdatePMCategory(request);
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
        /// Get All PM Categories
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        public async Task<IActionResult> Get()
        {
            Response_Message responcemodel = new Response_Message();
            ListViewModel<PMCategoryResponseModel> response = new ListViewModel<PMCategoryResponseModel>();
            response = await categoryService.GetAllPMCategory();
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
        /// Get PM Category By ID
        /// </summary>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        public async Task<IActionResult> Get(Guid id)
        {
            Response_Message responcemodel = new Response_Message();
            PMCategoryResponseModel response = new PMCategoryResponseModel();
            response = await categoryService.GetPMCategoryByID(id);
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
        /// Delete PM Category By ID
        /// </summary>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        public async Task<IActionResult> Delete(Guid id)
        {
            Response_Message responcemodel = new Response_Message();
            PMCategoryResponseModel response = new PMCategoryResponseModel();
            var result = await categoryService.DeletePMCategoryByID(id);
            if (result > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.message = ResponseMessages.PMCategoryDeleted;
            }
            else if (result == (int)ResponseStatusNumber.PMPlansExist)
            {
                responcemodel.success = (int)ResponseStatusNumber.PMPlansExist;
                responcemodel.message = ResponseMessages.PMPlansExisting;
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