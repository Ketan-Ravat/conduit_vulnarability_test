using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels.RequestResponseViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete
{
    public class CompanyRepository<T> : IDisposable, ICompanyRepository<T> where T : class
    {
        private readonly DBContextFactory context;
        private DbSet<T> dbSet;

        //public UserRepository(DBContextFactory context , IBaseGenericRepository<User> userRepository)
        //{
        //    this.context = context;
        //    this.userRepository = userRepository;
        //}
        public CompanyRepository(DBContextFactory context)
        {
            this.context = context;
            //this.userRepository = userRepository;
            dbSet = context.Set<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public virtual bool Insert(T entity)
        {
            bool IsSuccess = false;
            try
            {

                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                else
                {
                    var response = dbSet.Add(entity);
                    IsSuccess = true;
                }
            }
            catch (Exception)
            {

            }
            //catch (DbEntityValidationException dbEx)
            //{
            //    var msg = string.Empty;

            //    foreach (var validationErrors in dbEx.EntityValidationErrors)
            //    {
            //        foreach (var validationError in validationErrors.ValidationErrors)
            //        {
            //            msg += string.Format("Property: {0} Error: {1}",
            //            validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
            //        }
            //    }
            //    var fail = new Exception(msg, dbEx);
            //    throw fail;
            //}
            return IsSuccess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public virtual bool Update(T entity)
        {
            bool IsSuccess = false;
            try
            {

                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                else
                {
                    var response = dbSet.Update(entity);
                    IsSuccess = true;
                }
            }
            catch (Exception)
            {

            }

            return IsSuccess;
        }

        public async Task<List<Company>> GetAllCompany()
        {
            return await context.Company.Where(x=>x.status == (int)Status.Active).Include(x => x.Sites).ToListAsync();
        }

        public async Task<List<Company>> GetAllCompanyForFilter()
        {
            if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.Active)
            {
                return await context.Company.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).ToListAsync();
            }
            else
            {
                return await context.Company.Where(x => x.status == (int)Status.Active).ToListAsync();
            }
        }

        public async Task<Company> GetCompanyByCode(string company_code)
        {
            return await context.Company.Where(x=>x.company_code == company_code).FirstOrDefaultAsync();
        }

        public Company GetUserPoolDetailsByCompanyCode(string company_code)
        {
            //return context.Company.Where(x => x.company_code.ToLower() == company_code.ToLower()).FirstOrDefault();
            return context.Company.Where(x => x.domain_name.ToLower() == company_code.ToLower()).FirstOrDefault();
        }

        public async Task<Sites> GetSiteDetails(string siteid)
        {
            return await context.Sites.Where(x => x.site_id.ToString() == siteid).Include(x=>x.Company).FirstOrDefaultAsync();
        }
        
        public async Task<List<Company>> GetAllCompaniesWithSites()
        {
            return await context.Company.Where(x =>( x.status == (int)Status.Active )//|| x.status == (int)Status.AllCompanyType)
                                                ).Include(x => x.Sites).OrderBy(x => x.status).ToListAsync();
        }

        public async Task<List<Sites>> GetAllSites()
        {
            return await context.Sites.Where(x => x.status == (int)Status.Active || x.status == (int)Status.AllSiteType).OrderBy(x=>x.status).ToListAsync();
        }

        public Company GetCompanyByID(string company_id)
        {
            return context.Company.Where(x => x.company_id.ToString() == company_id && x.status == (int)Status.Active).Include(x => x.Sites).FirstOrDefault();
        }
        public Company GetDomainDetailsByUserpool(string user_pool_id)
        {
            return context.Company.Where(x => x.user_pool_id == user_pool_id && x.status == (int)Status.Active).FirstOrDefault();
        }
        public ClientCompany GetClientCompanyById(Guid clientCompanyId)
        {
            return context.ClientCompany.Where(x => x.client_company_id == clientCompanyId && x.status == (int)Status.Active)
                .Include(x => x.Sites)
                .FirstOrDefault();
        }


        public (List<ClientCompany>, int total_list_count) GetAllClientCompany(ClientCompanyListRequestModel requestModel)
        {
            List<ClientCompany> response = new List<ClientCompany>();
            int total_list_count = 0;
            IQueryable<ClientCompany> query = context.ClientCompany.Where(x => x.status == (int)Status.Active && x.parent_company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id));

            //Filter Purpose
            if (requestModel.from_date != null && requestModel.to_date != null)
            {
                
            }

            //Filter Purpose Search string
            if (!string.IsNullOrEmpty(requestModel.search_string))
            {
                
            }

            query = query.OrderByDescending(x => x.created_at);
            total_list_count = query.Count();

            if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
            {
                query = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
            }

            response = query
                   .ToList();

            return (response, total_list_count);
        }

        public (List<Sites>, int total_list_count) GetAllSite(SiteListRequestModel requestModel)
        {
            List<Sites> response = new List<Sites>();
            int total_list_count = 0;
            IQueryable<Sites> query = context.Sites.Where(x => x.status == (int)Status.Active && x.client_company_id == requestModel.client_company_id);

            //Filter Purpose
            if (requestModel.from_date != null && requestModel.to_date != null)
            {

            }

            //Filter Purpose Search string
            if (!string.IsNullOrEmpty(requestModel.search_string))
            {

            }

            query = query.OrderByDescending(x => x.created_at);
            total_list_count = query.Count();

            if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
            {
                query = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
            }

            response = query
                   .ToList();

            return (response, total_list_count);
        }

        public Sites GetSiteById(Guid siteid)
        {
            return context.Sites.Where(x => x.site_id == siteid && x.status == (int)Status.Active)
                .FirstOrDefault();
        }

        /// <summary>
        /// IDisposable implementation
        /// </summary>
        private bool disposed = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public (List<SiteDocuments>, int total_list_count) GetAllSiteDocument(GetAllSiteDocumentRequestmodel requestModel)
        {
            List<SiteDocuments> response = new List<SiteDocuments>();
            int total_list_count = 0;
            IQueryable<SiteDocuments> query = context.SiteDocuments.Where(x => !x.is_archive && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id));

            query = query.OrderByDescending(x => x.created_at);
            total_list_count = query.Count();

            if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
            {
                query = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
            }

            response = query
                   .ToList();

            return (response, total_list_count);
        }

        public List<SiteDocuments> GetAllSiteDocumentsByIds(List<Guid> sitedocument_id)
        {
            return context.SiteDocuments.Where(x => sitedocument_id.Contains(x.sitedocument_id)).ToList();
        }

        public SiteContact GetSiteContactById(Guid sitecontact_id)
        {
            return context.SiteContact.Where(x => x.sitecontact_id == sitecontact_id && !x.is_deleted).FirstOrDefault();
        }

        public (List<GetAllSiteContactsResponseModel>, int total_list_count) GetAllSiteContacts(GetAllSiteContactsRequestModel requestModel)
        {
            List<GetAllSiteContactsResponseModel> response = new List<GetAllSiteContactsResponseModel>();
            int total_list_count = 0;
            IQueryable<SiteContact> query = context.SiteContact.Where(x => x.client_company_id == requestModel.client_company_id && !x.is_deleted);

            //Filter Purpose Search string
            if (!string.IsNullOrEmpty(requestModel.search_string))
            {
                var searchstring = requestModel.search_string.ToLower().ToString();
                query = query.Where(x => x.sitecontact_title.ToLower().Contains(searchstring)
                || x.sitecontact_email.ToLower().Contains(searchstring)
                || x.sitecontact_phone.ToLower().Contains(searchstring)
                || x.sitecontact_name.ToLower().Contains(searchstring));
            }

            query = query.OrderByDescending(x => x.created_at);
            total_list_count = query.Count();

            if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
            {
                query = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
            }



            response = query
                 .Select(x => new GetAllSiteContactsResponseModel
                 {
                     sitecontact_id = x.sitecontact_id,
                     sitecontact_title = x.sitecontact_title,
                     sitecontact_name = x.sitecontact_name,
                     sitecontact_email = x.sitecontact_email,
                     sitecontact_phone = x.sitecontact_phone
                     
                 })
                 .ToList();

            return (response, total_list_count);
        }




    }
}
