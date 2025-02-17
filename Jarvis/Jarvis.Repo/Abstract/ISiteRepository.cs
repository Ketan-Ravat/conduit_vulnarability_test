using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface ISiteRepository
    {
        Sites FindSiteBySiteLocation(string location, Guid company_id);

        Sites GetSiteById(string site_id);

        Task<List<Sites>> GetAllSitesForFilter();

        Task<Sites> GetAllTypeSite();

        Task<List<User>> GetSitesByCompanyID(Guid company_id);
        Task<List<Sites>> GetActiveSitesByCompanyID(Guid company_id);

        List<SiteDocuments> GetAllSiteDocumentsByIds(List<Guid> sitedocument_id);
    }
}
