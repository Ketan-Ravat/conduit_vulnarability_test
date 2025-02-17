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
    public class TaskController : ControllerBase
    {
        public AWSConfigurations _options { get; set; }
        private readonly ITaskService taskService;
        public TaskController(ITaskService _taskService)
        {
            this.taskService = _taskService;
        }
        /// <summary>
        /// Add or Update PM Task
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Technician" })]
        public async Task<IActionResult> AddUpdateTask([FromBody] TaskRequestModel request)
        {
            Response_Message responcemodel = new Response_Message();
            if (ModelState.IsValid)
            {
                var result = await taskService.AddUpdateTask(request);
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
        /// Get All PM Tasks
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Technician" })]
        public IActionResult Get([FromQuery(Name = "pageindex")] int pageindex, [FromQuery(Name = "pagesize")] int pagesize, [FromQuery(Name = "searchstring")] string searchstring)
        {
            Response_Message responcemodel = new Response_Message();
            ListViewModel<TaskResponseModel> response = new ListViewModel<TaskResponseModel>();
            response =  taskService.GetAllTasks(pageindex, pagesize, searchstring);
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
        /// Get PM Task by Task ID
        /// </summary>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Technician" })]
        public async Task<IActionResult> Get(Guid id)
        {
            Response_Message responcemodel = new Response_Message();
            TaskResponseModel response = new TaskResponseModel();
            response = await taskService.GetTaskByID(id);
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
        /// Delete PM Plan By ID
        /// </summary>
        [HttpGet("{id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Technician" })]
        public async Task<IActionResult> Delete(Guid id)
        {
            Response_Message responcemodel = new Response_Message();
            var result = await taskService.DeleteTaskByID(id);
            if (result > 0)
            {
                responcemodel.success = (int)ResponseStatusNumber.Success;
                responcemodel.message = ResponseMessages.PMCategoryDeleted;
            }
            else if (result == (int)ResponseStatusNumber.TaskInUse)
            {
                responcemodel.success = (int)ResponseStatusNumber.TaskInUse;
                responcemodel.message = ResponseMessages.TaskAlreadyLinked;
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