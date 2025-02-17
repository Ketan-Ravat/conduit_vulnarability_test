using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jarvis.db.Models;
using Jarvis.Filter;
using Jarvis.Resource;
using Jarvis.Service.Abstract;
using Jarvis.Service.Concrete;
using Jarvis.Shared;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using Jarvis.ViewModels.Filter;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.Extensions.Options;
using Jarvis.Shared.Helper;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Jarvis.Controllers
{
    //[TypeFilter(typeof(ValidationFilterAttribute))]
    [TypeFilter(typeof(requestvalidationFilter))]
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService companyService;
        private readonly IS3BucketService s3BucketService;
        private readonly IUserService userService;
        public AWSConfigurations _options { get; set; }
        public CompanyController(ICompanyService _companyService, IOptions<AWSConfigurations> options,IUserService userService)
        {
            this.companyService = _companyService;
            this.s3BucketService = new S3BucketService();
            this._options = options.Value;
            this.userService = userService;
        }

        /// <summary>
        /// Get All Company
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpGet]
        public async Task<IActionResult> GetAllCompany()
        {
            Response_Message responseModel = new Response_Message();
            //try
            //{
                List<GetAllCompanyResponseModel> response = new List<GetAllCompanyResponseModel>();
                response = await companyService.GetAllCompany();
                if(response.Count > 0)
                {
                    responseModel.data = response;
                    responseModel.success = (int)ResponseStatusNumber.Success;
                }
                else
                {
                    responseModel.message = ResponseMessages.nodatafound;
                    responseModel.success = (int)ResponseStatusNumber.NotFound;
                }
            //}
            //catch(Exception e)
            //{
            //    Logger.Log("Error in GetAllCompany ", e.ToString());
            //    responseModel.message = ResponseMessages.Error;
            //    responseModel.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(responseModel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpGet]
        public async Task<IActionResult> GetAllCompanyForFilter()
        {
            Response_Message responseModel = new Response_Message();
            List<GetAllCompanyResponseModel> response = new List<GetAllCompanyResponseModel>();
            response = await companyService.GetAllCompanyForFilter();
            if (response.Count > 0)
            {
                responseModel.data = response;
                responseModel.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                responseModel.message = ResponseMessages.nodatafound;
                responseModel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responseModel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpGet]
        public async Task<IActionResult> GetAllSitesForFilter()
        {
            Response_Message responseModel = new Response_Message();
            List<GetCompanySitesViewModel> response = new List<GetCompanySitesViewModel>();
            response = await companyService.GetAllSitesForFilter();
            if (response.Count > 0)
            {
                responseModel.data = response;
                responseModel.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                responseModel.message = ResponseMessages.nodatafound;
                responseModel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responseModel);
        }

        /// <summary>
        /// Add Company
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddCompany([FromBody]CompanyRequestModel request)
        {
            bool result = false;
            //try
            //{
                 result = await companyService.AddCompany(request);
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in AddCompany : ", e.ToString());
            //    throw e;
            //}
            return Ok(result);
        }

        /// <summary>
        /// 
        /// <param name="companyId"></param>
        /// <returns></returns>
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> DeleteCompany(string companyId)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await companyService.DeleteCompany(companyId);
            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordDeleted;
                responsemodel.data = response;
            }
            else if (response == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.success = (int)ResponseStatusNumber.Error;
                responsemodel.message = ResponseMessages.Error;
                responsemodel.data = response;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Add Sites
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddSite([FromBody]SiteRequestModel request)
        {
            bool result = false;
            //try
            //{
                result = await companyService.AddSites(request);
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in AddSite : ", e.ToString());
            //    throw e;
            //}
            return Ok(result);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin" })]
        [HttpPost]
        public IActionResult GetAllClientCompany(ClientCompanyListRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<ClientCompanyListResponseModel> response = new ListViewModel<ClientCompanyListResponseModel>();
            response = companyService.GetAllClientCompany(requestModel);

            response.pageIndex = requestModel.pageindex;
            response.pageSize = requestModel.pagesize;
            if (response.list != null && response.list.Count > 0)
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
        /// Add and Update Client Company
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AddUpdateClientCompany([FromBody] ClientCompanyRequestModel request)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await companyService.AddUpdateClientCompany(request);
            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordAdded;
                responsemodel.data = response;
            }
            else if (response == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.success = (int)ResponseStatusNumber.Error;
                responsemodel.message = ResponseMessages.Error;
                responsemodel.data = response;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Delete Client Company
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> DeleteClientCompany([FromBody] ClientCompanyIdRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();

            if (!string.IsNullOrEmpty(requestModel.clientCompanyId))
            {
                var response = await companyService.DeleteClientCompany(requestModel.clientCompanyId);
                if (response == (int)ResponseStatusNumber.Success)
                {
                    responsemodel.success = (int)ResponseStatusNumber.Success;
                    responsemodel.message = ResponseMessages.RecordDeleted;
                    responsemodel.data = response;
                }
                else if (response == (int)ResponseStatusNumber.NotFound)
                {
                    responsemodel.success = (int)ResponseStatusNumber.NotFound;
                    responsemodel.message = ResponseMessages.nodatafound;
                    responsemodel.data = response;
                }
                else
                {
                    responsemodel.success = (int)ResponseStatusNumber.Error;
                    responsemodel.message = ResponseMessages.Error;
                    responsemodel.data = response;
                }
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Get All Site 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin" })]
        [HttpPost]
        public IActionResult GetAllSites(SiteListRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<SiteListResponseModel> response = new ListViewModel<SiteListResponseModel>();
            response = companyService.GetAllSite(requestModel);

            response.pageIndex = requestModel.pageindex;
            response.pageSize = requestModel.pagesize;
            if (response.list != null && response.list.Count > 0)
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
        /// Add and Update Site
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AddUpdateSite([FromBody] AddUpdateSiteRequestModel request)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await companyService.AddUpdateSite(request);
            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordAdded;
                responsemodel.data = response;
            }
            else if (response == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.success = (int)ResponseStatusNumber.Error;
                responsemodel.message = ResponseMessages.Error;
                responsemodel.data = response;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Delete site
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> DeleteSite([FromBody] DeleteSiteRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();

            if (!string.IsNullOrEmpty(requestModel.siteId))
            {
                var response = await companyService.DeleteSite(requestModel.siteId);
                if (response == (int)ResponseStatusNumber.Success)
                {
                    responsemodel.success = (int)ResponseStatusNumber.Success;
                    responsemodel.message = ResponseMessages.RecordDeleted;
                    responsemodel.data = response;
                }
                else if (response == (int)ResponseStatusNumber.NotFound)
                {
                    responsemodel.success = (int)ResponseStatusNumber.NotFound;
                    responsemodel.message = ResponseMessages.nodatafound;
                    responsemodel.data = response;
                }
                else
                {
                    responsemodel.success = (int)ResponseStatusNumber.Error;
                    responsemodel.message = ResponseMessages.Error;
                    responsemodel.data = response;
                }
            }

            return Ok(responsemodel);
        }

        
        [HttpGet]
        public IActionResult GetUserPoolDetails(string company_code)
        {
            Response_Message responseModel = new Response_Message();
            UserPoolResponseModel userpool = companyService.GetUserPoolDetails(company_code);
            if(userpool != null)
            {
                responseModel.data = userpool;
                responseModel.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                responseModel.success = (int)ResponseStatusNumber.NotFound;
                responseModel.message = ResponseMessages.nodatafound;
            }
            return Ok(responseModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyLogos(string company_code)
        {
            Response_Message responseModel = new Response_Message();
            GetCompanyLogosResponsemodel userpool = await companyService.GetCompanyLogos(company_code);
            if (userpool != null)
            {
                responseModel.data = userpool;
                responseModel.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                responseModel.success = (int)ResponseStatusNumber.NotFound;
                responseModel.message = ResponseMessages.nodatafound;
            }
            return Ok(responseModel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "SuperAdmin" })]
        [HttpGet]
        public async Task<IActionResult> GetAllCompaniesWithSites()
        {
            Response_Message responseModel = new Response_Message();
            List<GetAllCompanyResponseModel> response = new List<GetAllCompanyResponseModel>();
            response = await companyService.GetAllCompaniesWithSites();
            if (response.Count > 0)
            {
                responseModel.data = response;
                responseModel.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                responseModel.message = ResponseMessages.nodatafound;
                responseModel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responseModel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager" })]
        [HttpPut]
        public async Task<IActionResult> UpdateSiteData(SiteRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            int responseModel = await companyService.UpdateSiteData(requestModel);
            if (responseModel > 0)
            {
                response.success = responseModel;
                response.message = ResponseMessages.RecordUpdated;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.success = responseModel;
                response.message = ResponseMessages.nodatafound;
            }
            else
            {
                response.success = responseModel;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpGet]
        public IActionResult GetAWSParameters()
        {
            Response_Message responseModel = new Response_Message();
           
            var response =  companyService.TestAPI();
            if (response!=  null)
            {
                responseModel.data = response;
                responseModel.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                responseModel.message = ResponseMessages.nodatafound;
                responseModel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responseModel);
        }


        [HttpGet]
        public IActionResult GetDomainDetailsByUserpool(string user_pool_id)
        {
            Response_Message responseModel = new Response_Message();
            var userpool = companyService.GetDomainDetailsByUserpool(user_pool_id);
            if (userpool != null)
            {
                responseModel.data = userpool;
                responseModel.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                responseModel.success = (int)ResponseStatusNumber.NotFound;
                responseModel.message = ResponseMessages.nodatafound;
            }
            return Ok(responseModel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpGet]
        public IActionResult GetAllFeaturesFlagsByCompany()
        {
            Response_Message responseModel = new Response_Message();
            var response = companyService.GetAllFeaturesFlagsByCompany();
            if (response != null && response.list != null && response.list.Count > 0)
            {
                responseModel.data = response;
                responseModel.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                responseModel.success = (int)ResponseStatusNumber.NotFound;
                responseModel.message = ResponseMessages.nodatafound;
            }
            return Ok(responseModel);
        }
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpPost]
        public async Task<IActionResult> UpdateFeatureFlagForCompany(UpdateFeatureFlagForCompanyRequestModel requestModel)
        {
            Response_Message responseModel = new Response_Message();
            var response = await companyService.UpdateFeatureFlagForCompany(requestModel);
            if (response == (int)ResponseStatusNumber.Success)
            {
                responseModel.data = response;
                responseModel.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                responseModel.success = (int)ResponseStatusNumber.NotFound;
                responseModel.message = ResponseMessages.nodatafound;
            }
            return Ok(responseModel);
        }

        /// <summary>
        /// Upload Attachments for Work Order
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadSiteDocument()
        {
            Response_Message responsemodel = new Response_Message();

            List<IFormFile> filesToUpload = new List<IFormFile>();
            List<string> user_file_name = new List<string>();

            if (Request.Form.Files.Count > 0)
            {
                foreach (var file in Request.Form.Files)
                {
                    user_file_name.Add(file.FileName);
                    filesToUpload.Add(file);
                }

                var list = await s3BucketService.UploadSiteDocument(filesToUpload, _options.S3_aws_access_key, _options.S3_aws_secret_key, _options.sitedocument_bucket);

                if (list.Count > 0)
                {
                    //update in table
                    UploadSiteDocumentRequestmodel UploadSiteDocumentRequestmodel = new UploadSiteDocumentRequestmodel();
                    UploadSiteDocumentRequestmodel.file_name = list;
                    UploadSiteDocumentRequestmodel.folder_path = UpdatedGenericRequestmodel.CurrentUser.site_id;
                    await companyService.UploadSiteDocument(UploadSiteDocumentRequestmodel);


                    /*ar items = new WorkOrderAttachmentsResponseModel
                    {
                        filename = list[0],
                        file_url = UrlGenerator.GetWorkOrderAttachmentURL(list[0]),
                        user_uploaded_name = user_file_name
                    };*/
                    responsemodel.success = 1;
                    responsemodel.data = null;
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

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public IActionResult GetAllSiteDocument(GetAllSiteDocumentRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = companyService.GetAllSiteDocument(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.SuccessMessage;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> DeleteSiteDocument(DeleteSiteDocumentRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await companyService.DeleteSiteDocument(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordDeleted;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Technician" })]
        [HttpPost]
        public async Task<IActionResult> AddUpdateSiteContact(AddUpdateSiteContactRequestModel request)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await userService.AddUpdateSiteContact(request);
            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordAdded;
                responsemodel.data = response;
            }
            else if (response == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.success = (int)ResponseStatusNumber.Error;
                responsemodel.message = ResponseMessages.Error;
                responsemodel.data = response;
            }

            return Ok(responsemodel);
        }

        [HttpGet("{sitecontact_id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin,Technician" })]
        public async Task<IActionResult> DeleteSiteContact(Guid sitecontact_id)
        {
            Response_Message responsemodel = new Response_Message();
            var result = await userService.DeleteSiteContactById(sitecontact_id);
            if (result > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordDeleted;
            }
            else if (result == (int)ResponseStatusNumber.NotFound)
            {
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
                responsemodel.message = ResponseMessages.nodatafound;
            }
            else
            {
                responsemodel.success = (int)ResponseStatusNumber.Error;
                responsemodel.message = ResponseMessages.Error;
            }

            return Ok(responsemodel);
        }

        [HttpGet("{sitecontact_id}")]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin,Technician" })]
        public IActionResult GetSiteContactById(Guid sitecontact_id)
        {
            Response_Message responsemodel = new Response_Message();
           
            var result = companyService.GetSiteContactById(sitecontact_id);
            if (result != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = result;
            }
            else
            {
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
                responsemodel.message = ResponseMessages.nodatafound;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Maintenance staff,Technician" })]
        [HttpPost]
        public IActionResult GetAllSiteContacts(GetAllSiteContactsRequestModel requestModel)
        {
            Response_Message responseModel = new Response_Message();
            var response = companyService.GetAllSiteContacts(requestModel);
            if (response != null && response.list != null && response.list.Count > 0)
            {
                responseModel.data = response;
                responseModel.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                responseModel.success = (int)ResponseStatusNumber.NotFound;
                responseModel.message = ResponseMessages.nodatafound;
            }
            return Ok(responseModel);
        }

    }
}