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

namespace Jarvis.Repo.Concrete {
    public class SiteRepository : BaseGenericRepository<Sites>, ISiteRepository {
        //private IBaseGenericRepository<User> userRepository;
        //private readonly DBContextFactory Context;
        //private DbSet<T> dbSet;

        public SiteRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }

        public Sites FindSiteBySiteLocation(string location, Guid company_id)
        {
            return context.Sites.Where(x => x.location == location && x.company_id == company_id).FirstOrDefault();
        }

        public Sites GetSiteById(string site_id)
        {
            return context.Sites.Where(x => x.site_id.ToString() == site_id).Include(x => x.Company).FirstOrDefault();
        }

        public async Task<List<Sites>> GetAllSitesForFilter()
        {
            List<Sites> response = new List<Sites>();
            string rolename = string.Empty;
            //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
            if (!string.IsNullOrEmpty(GenericRequestModel.role_id))
            {
                rolename = context.UserRoles.Where(x => x.role_id.ToString() == GenericRequestModel.role_id && x.status == (int)Status.Active && x.user_id == GenericRequestModel.requested_by).Select(x => x.Role.name).FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(rolename))
            {
                if (rolename == GlobalConstants.Admin)
                {
                    if (GenericRequestModel.site_status == (int)Status.Active)
                    {
                        response = context.Sites.Where(x => x.site_id.ToString() == GenericRequestModel.site_id && x.status == (int)Status.Active).ToList();
                    }
                    else
                    {
                        if (GenericRequestModel.company_status == (int)Status.Active)
                        {
                            var companies = context.Company.Where(x => x.company_id.ToString() == GenericRequestModel.company_id && x.status == (int)Status.Active).Select(x => x.company_id).ToList();
                            response = context.Sites.Where(x => companies.Contains(x.company_id) && x.status == (int)Status.Active).ToList();
                        }
                        else if (GenericRequestModel.company_status == (int)Status.AllCompanyType)
                        {
                            if (GenericRequestModel.site_status != (int)Status.AllSiteType)
                            {
                                response = context.Sites.Where(x => x.status == (int)Status.Active && x.site_id.ToString() == GenericRequestModel.site_id).ToList();
                            }
                            else
                            {
                                response = context.Sites.Where(x => x.status == (int)Status.Active).ToList();
                            }
                        }
                    }
                }
                else
                {
                    var usersites = context.UserSites.Where(x => x.user_id == GenericRequestModel.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

                    if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                    {
                        response = context.Sites.Where(x => usersites.Contains(x.site_id)).ToList();
                    }
                    else
                    {
                        response = context.Sites.Where(x => usersites.Contains(x.site_id) && x.site_id.ToString() == GenericRequestModel.site_id).ToList();
                    }
                }
            }
            return response;
        }

        public async Task<Sites> GetAllTypeSite()
        {
            return context.Sites.Where(x => x.status == (int)Status.AllSiteType).FirstOrDefault();
        }

        public async Task<List<User>> GetSitesByCompanyID(Guid company_id)
        {
            List<User> user = new List<User>();
            var sites = await context.Sites.Where(x => x.company_id == company_id).Include(x => x.UserSites).ThenInclude(x => x.User).ToListAsync();
            if (sites?.Count > 0)
            {
                foreach (var site in sites)
                {
                    user.AddRange(site.UserSites.Select(x => x.User).Distinct().ToList());
                }
            }
            return user;
        }

        public async Task<List<Sites>> GetActiveSitesByCompanyID(Guid company_id)
        {
            return await context.Sites.Where(x => x.company_id == company_id && x.status == (int)Status.Active)
                .Include(x => x.StatusMaster).Include(x => x.Company).ToListAsync();
        }
        public List<SiteDocuments> GetAllSiteDocumentsByIds(List<Guid> sitedocument_id)
        {
            return context.SiteDocuments.Where(x => sitedocument_id.Contains(x.sitedocument_id)).ToList();
        }
    }
}
