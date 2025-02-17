using Jarvis.db.Models;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Jarvis.ViewModels.ViewModels;

namespace Jarvis.Service.Abstract
{
    public interface ICompanyService
    {
        Task<bool> AddCompany(CompanyRequestModel request);
        Task<bool> AddSites(SiteRequestModel request);

        SitesViewModel GetSiteByLocation(string location, Guid companyid);

        Task<List<GetAllCompanyResponseModel>> GetAllCompany();
        Task<List<GetAllCompanyResponseModel>> GetAllCompanyForFilter();
        Task<List<GetCompanySitesViewModel>> GetAllSitesForFilter();
        UserPoolResponseModel GetUserPoolDetails(string company_code);
        GetDomainDetailsByUserpoolResponsemodel GetDomainDetailsByUserpool(string user_pool_id);
        Task<GetCompanyLogosResponsemodel> GetCompanyLogos(string company_code);
        Task<List<GetAllCompanyResponseModel>> GetAllCompaniesWithSites();
        Task<int> UpdateSiteData(SiteRequestModel requestModel);
        Task<int> AddUpdateClientCompany(ClientCompanyRequestModel requestModel);
        ListViewModel<ClientCompanyListResponseModel> GetAllClientCompany(ClientCompanyListRequestModel request);
        Task<int> DeleteClientCompany(string clientCompanyId);

        Task<int> DeleteCompany(string companyId);

        ListViewModel<SiteListResponseModel> GetAllSite(SiteListRequestModel request);
        Task<int> AddUpdateSite(AddUpdateSiteRequestModel requestModel);
        Task<int> DeleteSite(string siteId);
        GetAllFeaturesFlagsByCompanyResponseModel GetAllFeaturesFlagsByCompany();
        Task<int> UpdateFeatureFlagForCompany(UpdateFeatureFlagForCompanyRequestModel requestModel);
        TestAPIResponsemodel TestAPI();

        ListViewModel<GetAllSiteDocumentResponsemodel> GetAllSiteDocument(GetAllSiteDocumentRequestmodel requestmodel);

        Task<int> UploadSiteDocument(UploadSiteDocumentRequestmodel requestmodel);
        Task<int> DeleteSiteDocument(DeleteSiteDocumentRequestmodel requestmodel);
        GetSiteContactByIdResponseModel GetSiteContactById(Guid sitecontact_id);
        ListViewModel<GetAllSiteContactsResponseModel> GetAllSiteContacts(GetAllSiteContactsRequestModel requestModel);
    }
}
