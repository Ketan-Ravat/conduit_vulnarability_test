using Jarvis.Filter;
using Jarvis.Resource;
using Jarvis.Service.Abstract;
using Jarvis.Service.Concrete;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Jarvis.Controllers
{
    [TypeFilter(typeof(requestvalidationFilter))]
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService DashboardService;
        private readonly IS3BucketService s3BucketService;
        private readonly IWorkOrderService workorderService;
        public AWSConfigurations _options { get; set; }
        public DashboardController(IDashboardService _dashboardService,IWorkOrderService _workorderService, IOptions<AWSConfigurations> options)
        {
            this.DashboardService = _dashboardService;
            this.workorderService = _workorderService;
            this._options = options.Value;
            this.s3BucketService = new S3BucketService();
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin" })]
        [HttpGet]
        public IActionResult GetAdminDashboardCount()
        {
            Response_Message response = new Response_Message();
            GetAdminDashboardCountResponseModel responseModel = new GetAdminDashboardCountResponseModel();
           
            responseModel = DashboardService.GetAdminDashboardCount();
            
            if (responseModel != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
                response.message = (string)ResponseMessages.SuccessMessage;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin" })]
        [HttpPost]
        public IActionResult GetAllWorkordersListByStatus(GetTopWorkordersListByStatusRequestModel request)
        { 
            Response_Message response = new Response_Message();

            var responseModel = DashboardService.GetAllWorkordersListByStatus(request);

            if (responseModel != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
                response.message = (string)ResponseMessages.SuccessMessage;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin" })]
        [HttpPost]
        public IActionResult GetAllLineItemsByStatus(GetTopWorkordersListByStatusRequestModel request)
        {
            Response_Message response = new Response_Message();

            var responseModel = DashboardService.GetAllLineItemsByStatus(request);

            if (responseModel != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
                response.message = (string)ResponseMessages.SuccessMessage;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin" })]
        [HttpGet("{pagesize?}/{pageindex?}")]
        public IActionResult GetTechniciansListWithSubmittedLinesCount(int pagesize, int pageindex)
        {
            Response_Message response = new Response_Message();

            var responseModel = DashboardService.GetTechniciansListWithSubmittedLinesCount(pagesize,pageindex);

            if (responseModel != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
                response.message = (string)ResponseMessages.SuccessMessage;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin" })]
        [HttpPost]
        public IActionResult UpcomingSiteOpportunitiesDashboard(UpcomingSiteOpportunitiesDashboardRequestModel requestmodel)
        {
            Response_Message response = new Response_Message();

            var responseModel = DashboardService.UpcomingSiteOpportunitiesDashboard(requestmodel);

            if (responseModel != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
                response.message = (string)ResponseMessages.SuccessMessage;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Company Admin,SuperAdmin" })]
        [HttpGet("{feature_type}")]
        public IActionResult GetFeatureWiseURLs(int feature_type)
        {
            Response_Message response = new Response_Message();

            var responseModel = DashboardService.GetFeatureWiseURLs(feature_type);
            
            if (responseModel != null && responseModel.status == (int)ResponseStatusNumber.Success)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
                response.message = (string)ResponseMessages.SuccessMessage;
            }
            else
            {
                response.message = ResponseMessages.Error;
                response.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(response);
        }
    }
}
