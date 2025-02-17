using DocumentFormat.OpenXml.Spreadsheet;
using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels.RequestResponseViewModel;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jarvis.Repo.Concrete
{
    public class DashboardRepository : BaseGenericRepository<WorkOrders> , IDashboardRepository
    {
        public DashboardRepository(DBContextFactory context) : base(context)
        {
            this.context = context;
        }

        public int GetActiveSiteCount()
        {
            return context.Sites.Where(x=>x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)
                && x.status == (int)Status.Active ).Count();
        }

        public int GetActiveWorkordersCount()
        {
            return context.WorkOrders.Where(x=>x.status != (int)Status.Completed
            && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_archive ).Count();
        }

        public int GetActiveTechniciansCount()
        {
            return context.User.Include(x=>x.Userroles).Include(x=>x.Usersites)
                .Where(x=> x.status == (int)Status.Active
                && x.Usersites.Select(x=>x.site_id).Contains(Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)) 
                && x.Userroles.Select(x=>x.role_id).Contains(Guid.Parse(GlobalConstants.Technician_Role_id)) )
                .Count();
        }

        public int GetWOStatusWiseCount(int status)
        {
            return context.WorkOrders.Where(x=> x.status == status && !x.is_archive 
                   && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) )
                .Count();
        }
        public int GetWODueOverdueFlagWiseCount(int WO_due_overdue_flag)
        {
            return context.WorkOrders.Where(x => x.wo_due_overdue_flag == WO_due_overdue_flag && !x.is_archive
                   && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id))
                .Count();
        }

        public (List<WorkOrders>,int) GetAllWorkordersListByStatus(GetTopWorkordersListByStatusRequestModel request)
        {
            IQueryable<WorkOrders> query = context.WorkOrders.Where(x => !x.is_archive
                   && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).Include(x=>x.Asset).Include(x=>x.Sites);

            if(request.status != null )
            {
                query = query.Where(x => x.status == request.status).OrderBy(x => x.modified_at);
            }

            if (request.wo_due_overdue_flag != null)
            {
                query = query.Where(x => x.wo_due_overdue_flag == request.wo_due_overdue_flag).OrderBy(x => x.due_at);
            }

            int total_list_count = query.Count();

            if (request.pageindex > 0 && request.pagesize > 0)
            {
                query = query.Skip((request.pageindex - 1) * request.pagesize).Take(request.pagesize);
            }


            return (query.ToList(),total_list_count);
        }


        public (List<WOOnboardingAssets>,int) GetAllWOOBAssetsByStatus(GetTopWorkordersListByStatusRequestModel request)
        {
            IQueryable<WOOnboardingAssets> query = context.WOOnboardingAssets.Where(x => x.status == request.status && !x.is_deleted
                   && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));

            int total_list_count = query.Count();

            query = query.Include(x => x.Sites).Include(x => x.Asset).OrderBy(x => x.modified_at);

            if (request.pageindex > 0 && request.pagesize > 0)
            {
                query = query.Skip((request.pageindex - 1) * request.pagesize).Take(request.pagesize);
            }

            return (query.ToList(),total_list_count);
        }

        public (List<AssetFormIO>,int) GetAllAssetFormIOByStatus(GetTopWorkordersListByStatusRequestModel request)
        {
            IQueryable<AssetFormIO> query = context.AssetFormIO.Where(x => x.status == request.status 
                   && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));

            int total_list_count = query.Count();

            if (request.pageindex > 0 && request.pagesize > 0)
            {
                query = query.Skip((request.pageindex - 1) * request.pagesize).Take(request.pagesize);
            }

            query = query.Include(x => x.Sites).Include(x => x.Asset).OrderBy(x => x.modified_at);

            return (query.ToList(),total_list_count);
        }

        public List<string> GetTechniciansWhoSubmittedWOLinesOfLast30Days() // Last 30 days submited wolines by technician
        {
            return context.WOOnboardingAssets.Where(x=>x.status == (int)Status.Ready_for_review && !x.is_deleted
            &&  x.modified_at.Value.Date > DateTime.UtcNow.AddDays(-30).Date && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
            ).Select(x=>x.modified_by).ToList();
        }
        
        public List<string> GetTechniciansWhoSubmittedAssetFormIOOfLast30Days() // Last 30 days submited wolines by technician
        {
            return context.AssetFormIO.Where(x => x.status == (int)Status.Ready_for_review 
            && x.modified_at.Value.Date > DateTime.UtcNow.AddDays(-30).Date && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
            ).Select(x=>x.modified_by).ToList();
        }

        public int GetActiveAssetsCountBySites(UpcomingSiteOpportunitiesDashboardRequestModel request)
        {
            return context.Assets.Where(x=>x.status == (int)Status.AssetActive && request.sites.Contains(x.site_id) ).Count();
        }

        public int GetOpenAssetIssuesCountBySites(UpcomingSiteOpportunitiesDashboardRequestModel request)
        {
            return context.AssetIssue.Where(x => x.issue_status == (int)Status.open 
            && !x.is_deleted  && request.sites.Contains(x.site_id.Value)).Count();
        }

        public int GetOpenAssetPMsCountBySites(UpcomingSiteOpportunitiesDashboardRequestModel request)
        {
            return context.AssetPMs.Include(x=>x.Asset).Where(x => x.status == (int)Status.open
            && !x.is_archive && x.Asset.status == (int)Status.AssetActive && request.sites.Contains(x.Asset.site_id)).Count();
        }
        public int GetNetaInspectionFormLineItemsCount()
        {
            return context.AssetFormIO.Where(x => x.status == (int)Status.Ready_for_review
                   && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).Count();
        }

        public int GetRegularInspectionFormLineItemsCount()
        {
            return context.WOOnboardingAssets.Where(x => x.status == (int)Status.Ready_for_review && !x.is_deleted
                   && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).Count();
        }
    }
}
