using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jarvis.Filter;
using Jarvis.Service.Abstract;
using Jarvis.Shared;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using Jarvis.ViewModels.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Jarvis.Resource;

namespace Jarvis.Controllers
{
    [TypeFilter(typeof(requestvalidationFilter))]
    [TypeFilter(typeof(ValidationFilterAttribute))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [TypeFilter(typeof(ExceptionFilter))]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class InspectionFormController : ControllerBase
    {
        private readonly IInspectionFormService inspectionFormService;
        public InspectionFormController(IInspectionFormService _inspectionFormService)
        {
            this.inspectionFormService = _inspectionFormService;
        }

        /// <summary>
        /// Add Inspection Form
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddInspectionForm([FromBody]InspectionFormRequestViewModel request)
        {
            bool result = false;
            Response_Message responcemodel = new Response_Message();
            //try
            //{
                result = await inspectionFormService.AddInspectionForm(request);
                responcemodel.success = (int)ResponseStatusNumber.Success;
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in AddInspectionForm ", e.ToString());
            //    responcemodel.success = (int)ResponseStatusNumber.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responcemodel);
        }

        /// <summary>
        /// Add Inspection Form Attributes
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddInspectionFormAttributes([FromBody]InspectionFormAttributesRequestModel request)
        {
            bool result = false;
            Response_Message responcemodel = new Response_Message();
            //try
            //{
                result = await inspectionFormService.AddInspectionFormAttributes(request);
                responcemodel.success = (int)ResponseStatusNumber.Success;
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in AddInspectionFormAttributes ", e.ToString());
            //    responcemodel.success = (int)ResponseStatusNumber.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responcemodel);
        }

        /// <summary>
        /// Get All Inspection
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = false)]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpGet("{companyid}")]
        public async Task<IActionResult> GetAllInspectionFormByCompanyId(Guid companyid)
        {
            Response_Message response = new Response_Message();

            var responseModel = await inspectionFormService.GetAllInspectionFormByCompanyId(companyid);
            if (responseModel.list.Count > 0)
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