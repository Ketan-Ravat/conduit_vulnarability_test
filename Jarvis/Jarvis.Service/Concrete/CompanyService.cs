using AutoMapper;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Vml;
using Jarvis.db.Models;
using Jarvis.Repo;
using Jarvis.Service.Abstract;
using Jarvis.Shared;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Jarvis.ViewModels.ViewModels;
using System.Configuration;
using SendGrid;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Jarvis.Service.Concrete
{
    public class CompanyService : BaseService, ICompanyService
    {
        public readonly IMapper _mapper;

        public CompanyService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
        }

        public async Task<bool> AddCompany(CompanyRequestModel company)
        {
            bool result = false;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    company.created_at = DateTime.UtcNow;
                    var company_code = GenerateRandomString.RandomString(6);
                    var companyExist = await _UoW.CompanyRepository<Company>().GetCompanyByCode(company_code);
                    if(companyExist == null)
                    {
                        company.company_code = company_code;
                        var companyrequest = _mapper.Map<Company>(company);
                        result = _UoW.CompanyRepository<Company>().Insert(companyrequest);
                        if (result)
                        {
                            _UoW.SaveChanges();
                            _dbtransaction.Commit();
                        }
                    }
                }
                catch (Exception)
                {
                    _dbtransaction.Rollback();
                }
            }

            return result;
        }


        public async Task<bool> AddSites(SiteRequestModel requestModel)
        {
            bool result = false;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    requestModel.created_at = DateTime.UtcNow;
                    var sitesrequest = _mapper.Map<Sites>(requestModel);
                    result = _UoW.CompanyRepository<Sites>().Insert(sitesrequest);
                    if (result)
                    {
                        _UoW.SaveChanges();
                        _dbtransaction.Commit();
                    }
                }
                catch (Exception)
                {
                    _dbtransaction.Rollback();
                }
            }

            return result;
        }

        public SitesViewModel GetSiteByLocation(string location, Guid companyid)
        {
            SitesViewModel sites = new SitesViewModel();
            try
            {
                var result = _UoW.SiteRepository.FindSiteBySiteLocation(location, companyid);
                sites = _mapper.Map<SitesViewModel>(result);
            }
            catch (Exception e)
            {
                Shared.Logger.Log("Exception to find site by location " + e.Message);
                throw e;
            }
            return sites;
        }

        public async Task<List<GetAllCompanyResponseModel>> GetAllCompany()
        {
            List<GetAllCompanyResponseModel> responseModels = new List<GetAllCompanyResponseModel>();
            try
            {
                var response = await _UoW.CompanyRepository<Company>().GetAllCompany();
                if (response.Count > 0)
                {
                    responseModels = _mapper.Map<List<GetAllCompanyResponseModel>>(response);
                    responseModels.ForEach(x =>
                        {
                            if (x.status == (int)Status.Active) { x.status_name = "Active"; }
                            else { x.status_name = "Deactive"; }
                            x.Sites.ToList().ForEach(y =>
                        {
                            if (y.status == (int)Status.Active) { y.status_name = "Active"; }
                            else { y.status_name = "Deactive"; }
                        });
                        });
                }
                else
                {
                    //do nothing;
                }
            }
            catch (Exception e)
            {
                Shared.Logger.Log("Error in GetAllCompany", e.Message);
                throw e;
            }
            return responseModels;
        }

        public async Task<List<GetAllCompanyResponseModel>> GetAllCompanyForFilter()
        {
            List<GetAllCompanyResponseModel> responseModels = new List<GetAllCompanyResponseModel>();
            try
            {
                var response = await _UoW.CompanyRepository<Company>().GetAllCompanyForFilter();
                if (response.Count > 0)
                {
                    responseModels = _mapper.Map<List<GetAllCompanyResponseModel>>(response);
                }
                else
                {
                    //do nothing;
                }
            }
            catch (Exception e)
            {
                Shared.Logger.Log("Error in GetAllCompany", e.Message);
                throw e;
            }
            return responseModels;
        }

        public async Task<List<GetCompanySitesViewModel>> GetAllSitesForFilter()
        {
            List<GetCompanySitesViewModel> responseModels = new List<GetCompanySitesViewModel>();
            try
            {
                var response = await _UoW.SiteRepository.GetAllSitesForFilter();
                if (response.Count > 0)
                {
                    responseModels = _mapper.Map<List<GetCompanySitesViewModel>>(response);
                }
                else
                {
                    //do nothing;
                }
            }
            catch (Exception e)
            {
                Shared.Logger.Log("Error in GetAllCompany", e.Message);
                throw e;
            }
            return responseModels;
        }

        public UserPoolResponseModel GetUserPoolDetails(string company_code)
        {
            try
            {
                UserPoolResponseModel responseModel = null;
                Company company = _UoW.CompanyRepository<Company>().GetUserPoolDetailsByCompanyCode(company_code);
                if (company != null && company.company_id != Guid.Empty)
                {
                    responseModel = _mapper.Map<UserPoolResponseModel>(company);
                }
                return responseModel;
            }
            catch
            {
                throw;
            }
        }

        public GetDomainDetailsByUserpoolResponsemodel GetDomainDetailsByUserpool(string user_pool_id)
        {
            GetDomainDetailsByUserpoolResponsemodel response = new GetDomainDetailsByUserpoolResponsemodel();

            var get_pool_details = _UoW.CompanyRepository<Company>().GetDomainDetailsByUserpool(user_pool_id);
            response.domain_name = get_pool_details.domain_name;
            response.domain_url = get_pool_details.domain_name  + ConfigurationManager.AppSettings["WebAppDomainForMail"];

            return response;
        }
        public async Task<GetCompanyLogosResponsemodel> GetCompanyLogos(string company_code)
        {
            GetCompanyLogosResponsemodel responseModel = null;
            try
            {
               
                Company company = _UoW.CompanyRepository<Company>().GetUserPoolDetailsByCompanyCode(company_code);
                if (company != null && company.company_id != Guid.Empty)
                {
                    responseModel = _mapper.Map<GetCompanyLogosResponsemodel>(company);
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            using (Stream stream = await client.GetStreamAsync(responseModel.company_logo))
                            {
                               // if (stream == null)
                                //    return (Picture)null;
                                byte[] buffer = new byte[16384];
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    while (true)
                                    {
                                        int num = await stream.ReadAsync(buffer, 0, buffer.Length);
                                        int read;
                                        if ((read = num) > 0)
                                            ms.Write(buffer, 0, read);
                                        else
                                            break;
                                    }
                                    responseModel.company_logo_base64 = Convert.ToBase64String(ms.ToArray());
                                }
                                buffer = (byte[])null;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                return responseModel;
            }
            catch
            {
                return responseModel;
            }
        }

        public async Task<List<GetAllCompanyResponseModel>> GetAllCompaniesWithSites()
        {
            List<GetAllCompanyResponseModel> responseModels = new List<GetAllCompanyResponseModel>();
            try
            {
                var response = await _UoW.CompanyRepository<Company>().GetAllCompaniesWithSites();
                if (response.Count > 0)
                {
                    foreach (var company in response)
                    {
                        company.Sites = company.Sites.Where(x => x.status == (int)Status.Active).ToList();
                      /*  if (company.status == (int)Status.AllCompanyType)
                        {
                            var allsites = await _UoW.CompanyRepository<Company>().GetAllSites();
                            if(allsites?.Count > 0)
                            {
                                company.Sites = allsites;
                            }
                        }
                        else
                        {
                            if(company.Sites?.Count > 1)
                            {
                                var alltypesites = await _UoW.SiteRepository.GetAllTypeSite(); // remove all site types
                                if (alltypesites != null)
                                {
                                    company.Sites.Add(alltypesites);
                                }
                            }
                        }*/
                    }
                    responseModels = _mapper.Map<List<GetAllCompanyResponseModel>>(response);
                }
                else
                {
                    //do nothing;
                }
            }
            catch (Exception e)
            {
                Shared.Logger.Log("Error in GetAllCompany", e.Message);
                throw e;
            }
            return responseModels;
        }

        public async Task<int> UpdateSiteData(SiteRequestModel requestModel)
        {
            try
            {
                var Sitedetails = _UoW.SiteRepository.GetSiteById(requestModel.site_id.ToString());
                if (Sitedetails != null)
                {
                    Sitedetails.isAutoApprove = requestModel.isAutoApprove;
                    Sitedetails.showHideApprove = requestModel.showHideApprove;
                    Sitedetails.isManagerNotes = requestModel.isManagerNotes;
                    Sitedetails.modified_at = DateTime.UtcNow;

                    //Note - modified_by cannot be updated as it will affect mobile functionality, as only null modified_by handled from MObile app
                    //Sitedetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    bool update = await _UoW.BaseGenericRepository<Sites>().Update(Sitedetails);

                    if (update)
                    {
                        return (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        return (int)ResponseStatusNumber.Error;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public ListViewModel<ClientCompanyListResponseModel> GetAllClientCompany(ClientCompanyListRequestModel request)
        {
            ListViewModel<ClientCompanyListResponseModel> response = new ListViewModel<ClientCompanyListResponseModel>();

            var clientCompanyListDetails =  _UoW.CompanyRepository<ClientCompany>().GetAllClientCompany(request);
            var mappedlist = _mapper.Map<List<ClientCompanyListResponseModel>>(clientCompanyListDetails.Item1);

            if (mappedlist != null && mappedlist.Count > 0)
            {
                response.list = mappedlist;
                response.listsize = clientCompanyListDetails.Item2;
            }

            return response;
        }

        public async Task<int> AddUpdateClientCompany(ClientCompanyRequestModel requestModel)
        {
            int responseStatusNumber = (int)ResponseStatusNumber.Error; 

            try
            {
                if (requestModel.client_company_id != null && requestModel.client_company_id != Guid.Empty) 
                {
                    var getClientCompany = _UoW.CompanyRepository<ClientCompany>().GetClientCompanyById(requestModel.client_company_id.Value);

                    if (getClientCompany != null) 
                    { 
                        getClientCompany.client_company_name = requestModel.client_company_name;
                        getClientCompany.owner_address = requestModel.owner_address; 
                        getClientCompany.owner = requestModel.owner;
                        getClientCompany.modified_at = DateTime.UtcNow;
                        getClientCompany.clientcompany_code = requestModel.client_company_name;


                        var siteIdInDatabase = getClientCompany.Sites.Select(s => s.site_id).ToList();
                        var siteIdInRequest = requestModel.site_ids;


                        var sitesNotInDB = siteIdInRequest.Except(siteIdInDatabase).ToList();                        
                        var valuesInDBNotInrequest = siteIdInDatabase.Except(siteIdInRequest).ToList();

                        //Add Company ClientId from Site which is request
                        foreach (var siteId in sitesNotInDB)
                        {
                            var getSiteDetails = _UoW.CompanyRepository<Sites>().GetSiteById(siteId);
                            if (getSiteDetails != null)
                            {
                                getSiteDetails.client_company_id = requestModel.client_company_id;
                                getSiteDetails.modified_at = DateTime.UtcNow;
                                getSiteDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                _UoW.CompanyRepository<Sites>().Update(getSiteDetails);
                            }
                        }


                        //Remove Company ClientId from Site which is not request
                        foreach (var siteId in valuesInDBNotInrequest)
                        {
                            var getSiteDetails = _UoW.CompanyRepository<Sites>().GetSiteById(siteId);
                            if (getSiteDetails != null)
                            {
                                getSiteDetails.client_company_id = null;
                                getSiteDetails.modified_at = DateTime.UtcNow;
                                getSiteDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                _UoW.CompanyRepository<Sites>().Update(getSiteDetails);
                            }
                        }



                        var update = _UoW.CompanyRepository<ClientCompany>().Update(getClientCompany);
                        if (update)
                        {
                            _UoW.SaveChanges();
                            responseStatusNumber = (int)ResponseStatusNumber.Success;
                        }
                    }
                    else
                    {
                        responseStatusNumber = (int)ResponseStatusNumber.NotFound;
                    }
                  
                }
                else 
                {

                    ClientCompany clientCompany = new ClientCompany();
                    clientCompany.owner_address = requestModel.owner_address;
                    clientCompany.owner = requestModel.owner;
                    clientCompany.created_at = DateTime.UtcNow;
                    clientCompany.modified_at = DateTime.UtcNow;
                    clientCompany.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();                    
                    clientCompany.status = requestModel.status;
                    clientCompany.parent_company_id = requestModel.parent_company_id;
                    clientCompany.clientcompany_code = requestModel.client_company_name;

                    var clientCompanyExit = await _UoW.CompanyRepository<ClientCompany>().GetCompanyByCode(requestModel.client_company_name.ToLower().Trim().ToString());
                    if (clientCompanyExit == null)
                    {
                        var result = _UoW.CompanyRepository<ClientCompany>().Insert(clientCompany);

                        if (result)
                        {
                            _UoW.SaveChanges();                            
                            responseStatusNumber = (int)ResponseStatusNumber.Success;
                        }
                    }
                }

                return responseStatusNumber;
            }
            catch (Exception)
            {
                return responseStatusNumber;
            }
        }

        public async Task<int> AddUpdateSite(AddUpdateSiteRequestModel requestModel)
        {
            int responseStatusNumber = (int)ResponseStatusNumber.Error;

            try
            {
                if (requestModel.site_id != null && requestModel.site_id != Guid.Empty)
                {
                    var getsiteDetails = await _UoW.CompanyRepository<Sites>().GetSiteDetails(requestModel.site_id.ToString());

                    if (getsiteDetails != null)
                    {
                        getsiteDetails.site_name = requestModel.site_name;
                        getsiteDetails.site_code = requestModel.site_code;
                        getsiteDetails.location = requestModel.location;
                        getsiteDetails.isAutoApprove = requestModel.isAutoApprove;
                        getsiteDetails.showHideApprove = requestModel.showHideApprove;
                        getsiteDetails.isManagerNotes = requestModel.isManagerNotes;
                        getsiteDetails.modified_at = DateTime.UtcNow;
                        getsiteDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        var update = _UoW.CompanyRepository<Sites>().Update(getsiteDetails);
                        if (update)
                        {
                            _UoW.SaveChanges();
                            responseStatusNumber = (int)ResponseStatusNumber.Success;
                        }
                    }
                    else
                    {
                        responseStatusNumber = (int)ResponseStatusNumber.NotFound;
                    }

                }
                else
                {

                    Sites sites = new Sites();
                    sites.site_name = requestModel.site_name;
                    sites.site_code = requestModel.site_code;
                    sites.location = requestModel.location;
                    sites.company_id = requestModel.company_id;
                    sites.isAutoApprove = requestModel.isAutoApprove;
                    sites.showHideApprove = requestModel.showHideApprove;
                    sites.isManagerNotes = requestModel.isManagerNotes;
                    sites.created_at = DateTime.UtcNow;
                    sites.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    
                    var result = _UoW.CompanyRepository<Sites>().Insert(sites);
                    
                    if (result)
                    {
                        _UoW.SaveChanges();
                        responseStatusNumber = (int)ResponseStatusNumber.Success;
                    }
                }

                return responseStatusNumber;
            }
            catch (Exception)
            {
                return responseStatusNumber;
            }
        }



        public async Task<int> DeleteClientCompany(string clientCompanyId)
        {
            int responseStatusNumber = (int)ResponseStatusNumber.Error;
            try
            {
                if (!string.IsNullOrEmpty(clientCompanyId))
                {
                    var getClientCompany = _UoW.CompanyRepository<ClientCompany>().GetClientCompanyById(Guid.Parse(clientCompanyId));
                    if (getClientCompany != null)
                    {
                        getClientCompany.status = (int)Status.Deactive;
                        getClientCompany.modified_at = DateTime.UtcNow;
                        getClientCompany.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        var update = _UoW.CompanyRepository<ClientCompany>().Update(getClientCompany);
                        if (update)
                        {
                            _UoW.SaveChanges();
                            responseStatusNumber = (int)ResponseStatusNumber.Success;
                        }
                    }
                    else
                    {
                        responseStatusNumber = (int)ResponseStatusNumber.NotExists;
                    }
                }
                else
                {
                    responseStatusNumber = (int)ResponseStatusNumber.Error; 
                }

                return responseStatusNumber;
            }
            catch (Exception)
            {
                return responseStatusNumber;
            }
        }

        public async Task<int> DeleteCompany(string companyId)
        {
            int responseStatusNumber = (int)ResponseStatusNumber.Error;
            try
            {
                if (!string.IsNullOrEmpty(companyId))
                {
                    var getCompany = _UoW.CompanyRepository<Company>().GetCompanyByID(companyId);
                    if (getCompany != null)
                    {
                        getCompany.company_id = Guid.Parse(companyId);
                        getCompany.status = (int)Status.Deactive;

                        var update = await _UoW.BaseGenericRepository<Company>().Update(getCompany);
                        if (update)
                        {
                            _UoW.SaveChanges();
                            responseStatusNumber = (int)ResponseStatusNumber.Success;
                        }
                    }
                    else
                    {
                        responseStatusNumber = (int)ResponseStatusNumber.NotExists;
                    }
                }
                else
                {
                    responseStatusNumber = (int)ResponseStatusNumber.Error;
                }

                return responseStatusNumber;
            }
            catch (Exception)
            {
                return responseStatusNumber;
            }
        }

        public ListViewModel<SiteListResponseModel> GetAllSite(SiteListRequestModel request)
        {
            ListViewModel<SiteListResponseModel> response = new ListViewModel<SiteListResponseModel>();

            var siteListDetails = _UoW.CompanyRepository<Sites>().GetAllSite(request);
            var mappedlist = _mapper.Map<List<SiteListResponseModel>>(siteListDetails.Item1);

            if (mappedlist != null && mappedlist.Count > 0)
            {
                response.list = mappedlist;
                response.listsize = siteListDetails.Item2;
            }

            return response;
        }

        public async Task<int> DeleteSite(string siteId)
        {
            int responseStatusNumber = (int)ResponseStatusNumber.Error;
            try
            {
                if (!string.IsNullOrEmpty(siteId))
                {
                    var getSiteDetails = await _UoW.CompanyRepository<Sites>().GetSiteDetails(siteId);
                    if (getSiteDetails != null)
                    {
                        getSiteDetails.status = (int)Status.Deactive;

                        var update =  _UoW.CompanyRepository<Sites>().Update(getSiteDetails);
                        if (update)
                        {
                            _UoW.SaveChanges();
                            responseStatusNumber = (int)ResponseStatusNumber.Success;
                        }
                    }
                    else
                    {
                        responseStatusNumber = (int)ResponseStatusNumber.NotExists;
                    }
                }
                else
                {
                    responseStatusNumber = (int)ResponseStatusNumber.Error;
                }

                return responseStatusNumber;
            }
            catch (Exception)
            {
                return responseStatusNumber;
            }
        }

        public GetAllFeaturesFlagsByCompanyResponseModel GetAllFeaturesFlagsByCompany()
        {
            GetAllFeaturesFlagsByCompanyResponseModel responseModel = new GetAllFeaturesFlagsByCompanyResponseModel();
            try
            {
                var get_features = _UoW.UserRepository.GetAllFeaturesByCompanyId(Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id));
                if(get_features!=null && get_features.list!=null && get_features.list.Count>0)
                {
                    responseModel = get_features;
                }
            }
            catch(Exception e)
            {
            }
            return responseModel;
        }
        public async Task<int> UpdateFeatureFlagForCompany(UpdateFeatureFlagForCompanyRequestModel requestModel)
        {
            int res = (int)ResponseStatusNumber.Error;
            try
            {
                var get_companyfeature_mapping = _UoW.UserRepository.GetCompanyFeatureMappingById(requestModel.company_feature_id);
                if (get_companyfeature_mapping != null)
                {
                    get_companyfeature_mapping.is_required = requestModel.is_required;
                    var update = await _UoW.BaseGenericRepository<CompanyFeatureMapping>().Update(get_companyfeature_mapping);
                    if (update)
                    {
                        res = (int)ResponseStatusNumber.Success;
                    }
                }
            }
            catch (Exception e)
            {
            }
            return res;
        }

        public TestAPIResponsemodel TestAPI()
        {
            TestAPIResponsemodel response = new TestAPIResponsemodel();
            response.access_key = ConfigurationManager.AppSettings["S3_aws_access_key"];
            response.secret_key = ConfigurationManager.AppSettings["S3_aws_secret_key"];
            response.cognito_mfa_access_key = ConfigurationManager.AppSettings["cognito_mfa_access_key"];
            response.cognito_mfa_secret_key = ConfigurationManager.AppSettings["cognito_mfa_secret_key"];

            return response;
        }

        public ListViewModel<GetAllSiteDocumentResponsemodel>  GetAllSiteDocument(GetAllSiteDocumentRequestmodel requestmodel)
        {
            ListViewModel<GetAllSiteDocumentResponsemodel> responsemodel = new ListViewModel<GetAllSiteDocumentResponsemodel>();

            var get_site_documents = _UoW.CompanyRepository<SiteDocuments>().GetAllSiteDocument(requestmodel);
            var mappedlist = _mapper.Map<List<GetAllSiteDocumentResponsemodel>>(get_site_documents.Item1);
            foreach(var item in mappedlist)
            {
                if (!String.IsNullOrEmpty(item.created_by))
                {
                    var get_created_user = _UoW.WorkOrderRepository.GetUserFirstnameById(Guid.Parse(item.created_by));
                    item.created_by = get_created_user.firstname + " " + get_created_user.lastname;
                }
                if (!String.IsNullOrEmpty(item.modified_by))
                {
                    var get_created_user = _UoW.WorkOrderRepository.GetUserFirstnameById(Guid.Parse(item.modified_by));
                    item.modified_by = get_created_user.firstname + " " + get_created_user.lastname;
                }
            }
            if (mappedlist != null && mappedlist.Count > 0)
            {
                responsemodel.list = mappedlist;
                responsemodel.listsize = get_site_documents.Item2;
            }


            return responsemodel;
        }

        public async Task<int> UploadSiteDocument(UploadSiteDocumentRequestmodel requestmodel)
        {

            foreach(var item in requestmodel.file_name)
            {
                SiteDocuments SiteDocuments = new SiteDocuments();
                SiteDocuments.file_name = item;
                SiteDocuments.s3_folder_name = requestmodel.folder_path;
                SiteDocuments.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                SiteDocuments.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                SiteDocuments.created_at =DateTime.UtcNow;
                SiteDocuments.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                var insert = await _UoW.BaseGenericRepository<SiteDocuments>().Insert(SiteDocuments);
                _UoW.SaveChanges();
            }

            return 1;
        }

        public async Task<int> DeleteSiteDocument(DeleteSiteDocumentRequestmodel requestmodel)
        {
            int success = (int)ResponseStatusNumber.Error;

            var get_site_documents = _UoW.SiteRepository.GetAllSiteDocumentsByIds(requestmodel.sitedocument_id);

            foreach (var item in get_site_documents)
            {
                item.is_archive = true;
                item.modified_at = DateTime.UtcNow;
                item.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                await _UoW.BaseGenericRepository<SiteDocuments>().Update(item);
                _UoW.SaveChanges();

                success = (int)ResponseStatusNumber.Success;
            }

            return success;
        }

       
       

        public  GetSiteContactByIdResponseModel GetSiteContactById(Guid sitecontact_id)
        {
            GetSiteContactByIdResponseModel responseModel = new GetSiteContactByIdResponseModel();
            try
            {
                var siteContactDetails =  _UoW.CompanyRepository<SiteContact>().GetSiteContactById(sitecontact_id);
                if (siteContactDetails!= null)
                {

                    responseModel.sitecontact_title = siteContactDetails.sitecontact_title;
                    responseModel.sitecontact_name = siteContactDetails.sitecontact_name;
                    responseModel.sitecontact_email = siteContactDetails.sitecontact_email;
                    responseModel.sitecontact_phone = siteContactDetails.sitecontact_phone;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;

        }

        public ListViewModel<GetAllSiteContactsResponseModel> GetAllSiteContacts(GetAllSiteContactsRequestModel requestModel)
        {
            ListViewModel<GetAllSiteContactsResponseModel> responsemodel = new ListViewModel<GetAllSiteContactsResponseModel>();

            var get_all_site_contacts = _UoW.CompanyRepository<SiteContact>().GetAllSiteContacts(requestModel);
           
           
            if (get_all_site_contacts.Item1 != null && get_all_site_contacts.Item1.Count > 0)
            {
                responsemodel.list = get_all_site_contacts.Item1;
                responsemodel.listsize = get_all_site_contacts.Item2;
                responsemodel.pageIndex = requestModel.pageindex;
                responsemodel.pageSize = requestModel.pagesize;
            }


            return responsemodel;
        }


    }
}
