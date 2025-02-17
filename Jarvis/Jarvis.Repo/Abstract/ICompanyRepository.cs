using DocumentFormat.OpenXml.Spreadsheet;
using Jarvis.db.Models;
using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface ICompanyRepository<T> where T : class
    {
        /// <summary>
        /// Insert entity to db
        /// </summary>
        /// <param name="entity"></param>
        bool Insert(T entity);

        bool Update(T entity);

        Task<List<Company>> GetAllCompany();

        Task<List<Company>> GetAllCompanyForFilter();

        Task<Company> GetCompanyByCode(string company_code);

        Company GetUserPoolDetailsByCompanyCode(string company_code);
        Task<Sites> GetSiteDetails(string siteid);

        Task<List<Company>> GetAllCompaniesWithSites();

        Task<List<Sites>> GetAllSites();

        Company GetCompanyByID(string company_id);

        ClientCompany GetClientCompanyById(Guid clientCompanyId);

        public (List<ClientCompany>, int total_list_count) GetAllClientCompany(ClientCompanyListRequestModel requestModel);

        public (List<Sites>, int total_list_count) GetAllSite(SiteListRequestModel requestModel);

        Sites GetSiteById(Guid siteid);

        Company GetDomainDetailsByUserpool(string user_pool_id);
        (List<SiteDocuments>, int total_list_count) GetAllSiteDocument(GetAllSiteDocumentRequestmodel requestModel);
        List<SiteDocuments> GetAllSiteDocumentsByIds(List<Guid> sitedocument_id);
        SiteContact GetSiteContactById(Guid sitecontact_id);
        (List<GetAllSiteContactsResponseModel>, int total_list_count) GetAllSiteContacts(GetAllSiteContactsRequestModel requestModel);
    }
}
