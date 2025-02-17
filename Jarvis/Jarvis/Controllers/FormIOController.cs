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

namespace Jarvis.Controllers
{
    [TypeFilter(typeof(requestvalidationFilter))]
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiController]
    public class FormIOController : ControllerBase
    {
        private readonly IFormIOService FormIOService;
        public FormIOController(IFormIOService IFormIOService)
        {
            this.FormIOService = IFormIOService;
            
        }


        /// <summary>
        /// Get All Form IO
        /// </summary>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpGet()]
        public IActionResult GetAllForm([FromQuery]int pageindex, [FromQuery]int pagesize , [FromQuery]string search_string)
        {
            Response_Message responsemodel = new Response_Message();
           
            ListViewModel<GetAllFormIOFormResponsemodel> response = new ListViewModel<GetAllFormIOFormResponsemodel>();
            response = FormIOService.GetAllForms(pagesize, pageindex , search_string);
            response.pageSize = pagesize;
            response.pageIndex = pageindex;
            if (response.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else if (pageindex > 1 && response.list.Count == 0)
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
        /// Add Update Form IO
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        public async Task<IActionResult> AddUpdateFormIO([FromBody] AddFormIORequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await FormIOService.AddUpdateFormIO(request);
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
                else if (result.response_status == (int)ResponseStatusNumber.invalidworkprocedure)
                {
                    responcemodel.success = (int)ResponseStatusNumber.invalidworkprocedure;
                    responcemodel.message = ResponseMessages.InvalidWorkProcedure;
                }
                else if (result.response_status == (int)ResponseStatusNumber.invalidformname)
                {
                    responcemodel.success = (int)ResponseStatusNumber.invalidformname;
                    responcemodel.message = "Form Name Must be unique";
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
        /// Get All Inspection
        /// </summary>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpGet()]
        public IActionResult DashboardPIchartcount()
        {
            Response_Message responsemodel = new Response_Message();

            FormIOPIChartCountResponseModel response = new FormIOPIChartCountResponseModel();
            response = FormIOService.DashboardPIchartcount();
            if (response!=null)
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
        /// Get All Inspection
        /// </summary>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpGet()]
        public IActionResult DashboardPropertiescounts()
        {
            Response_Message responsemodel = new Response_Message();

            DashboardPropertiescountsResponseModel response = new DashboardPropertiescountsResponseModel();
            response = FormIOService.DashboardPropertiescounts();
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
        /// Get All Form IO
        /// </summary>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpGet("{pagesize?}/{pageindex?}")]
        public IActionResult GetAllFormNames(int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();

            ListViewModel<FormIOResponseModel> response = new ListViewModel<FormIOResponseModel>();
            response = FormIOService.GetAllFormNames(pagesize, pageindex);
            response.pageSize = pagesize;
            response.pageIndex = pageindex;
            if (response.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else if (pageindex > 1 && response.list.Count == 0)
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
        /// Get All Form Types
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpGet("{pageindex?}/{pagesize?}/{searchstring?}")]
        public async Task<IActionResult> GetAllFormTypes(int pageindex = 0, int pagesize = 0, string searchstring = "")
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<FormTypeResponseModel> response = new ListViewModel<FormTypeResponseModel>();
            response = await FormIOService.GetAllFormTypes(pageindex, pagesize, searchstring);
            if (response?.list?.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
                responsemodel.message = ResponseMessages.nodatafound;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Delete WO category
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,SuperAdmin,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> DeleteForm(DeleteFormRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await FormIOService.DeleteForm(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else if(response == (int)ResponseStatusNumber.form_is_used)
            {
                responsemodel.message = ResponseMessages.FormisUsed;
                responsemodel.success = (int)ResponseStatusNumber.form_is_used;
            }
            else if (response == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get All Inspection
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = false)]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive" })]
        [HttpGet("{form_id}")]
        public IActionResult GetFormDataTemplateByFormId(Guid form_id)
        {
            Response_Message response = new Response_Message();

            var responseModel = FormIOService.GetFormDataTemplateByFormId(form_id);
            if (responseModel != null)
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

    }
}
