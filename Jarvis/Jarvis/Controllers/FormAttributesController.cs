using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Jarvis.Filter;
using Jarvis.Service.Abstract;
using Jarvis.Shared;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using Jarvis.ViewModels.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jarvis.Controllers
{
    [TypeFilter(typeof(requestvalidationFilter))]
    [TypeFilter(typeof(ValidationFilterAttribute))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [TypeFilter(typeof(ExceptionFilter))]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class FormAttributesController : ControllerBase
    {
        private readonly IFormAttributesService attributesService;
        public FormAttributesController(IFormAttributesService _attributeService)
        {
            this.attributesService = _attributeService;
        }

        [HttpPost]
        public async Task<IActionResult> AddAttributes([FromBody]FormAttributesViewModel request)
        {
            bool result = false;
            Response_Message responcemodel = new Response_Message();
            //try
            //{
                result = await attributesService.AddFormAttributes(request);
                responcemodel.success = (int)ResponseStatusNumber.Success;
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in AddAttributes ", e.ToString());
            //    responcemodel.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(responcemodel);
        }

        [HttpGet]
        public IActionResult GetAllAttributes()
        {
            Response_Message responcemodel = new Response_Message();
            //try
            //{
                var result = attributesService.GetAllInspectionAttributes();
                if(result.Count > 0)
                {
                    responcemodel.data = result;
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.NotFound;
                }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetAllAttributes ", e.ToString());
            //    responcemodel.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(responcemodel);
        }

        [HttpGet]
        public IActionResult GetAllInspectionAttributsCategory()
        {
            Response_Message responcemodel = new Response_Message();
            //try
            //{
                var result = attributesService.GetAllInspectionAttributsCategory();
                if (result.Count > 0)
                {
                    responcemodel.data = result;
                    responcemodel.success = (int)ResponseStatusNumber.Success;
                }
                else
                {
                    responcemodel.success = (int)ResponseStatusNumber.NotFound;
                }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetAllInspectionAttributsCategory ", e.ToString());
            //    responcemodel.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(responcemodel);
        }
    }
}