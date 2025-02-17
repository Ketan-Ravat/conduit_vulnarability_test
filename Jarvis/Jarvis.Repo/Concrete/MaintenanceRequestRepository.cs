using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete {
    public class MaintenanceRequestRepository : BaseGenericRepository<MaintenanceRequests>, IMaintenanceRequestRepository {
        public MaintenanceRequestRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }

        public virtual async Task<int> Insert(MaintenanceRequests entity)
        {
            int IsSuccess;
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                else
                {
                    Add(entity);
                    IsSuccess = (int)ResponseStatusNumber.Success;
                }
            }
            catch (Exception ex)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw ex;
            }
            return IsSuccess;
        }

        public virtual async Task<int> Update(MaintenanceRequests entity)
        {
            int IsSuccess = 0;
            try
            {
                dbSet.Update(entity);
                IsSuccess = await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw ex;
            }
            return IsSuccess;
        }

        public async Task<MaintenanceRequests> GetMRsById(Guid mr_id)
        {
            return await context.MaintenanceRequests.Where(u => u.mr_id == mr_id)
                .Include(x => x.Asset).Include(x => x.Issue)
                .Include(x => x.StatusMaster)
                .FirstOrDefaultAsync();
        }

        public async Task<ListViewModel<MaintenanceRequests>> GetAllMaintenanceRequest(GetAllMRRequestModel requestModel)
        {
            ListViewModel<MaintenanceRequests> MRItems = new ListViewModel<MaintenanceRequests>();
            if (!String.IsNullOrEmpty(GenericRequestModel.site_id))
            {
                List<Guid> usersites = new List<Guid>();
                if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == GenericRequestModel.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == GenericRequestModel.requested_by && x.status == (int)Status.Active && x.site_id.ToString() == GenericRequestModel.site_id).Select(x => x.site_id).ToList();
                }

                if (requestModel.pageIndex == 0 || requestModel.pageSize == 0)
                {
                    requestModel.pageSize = 20;
                    requestModel.pageIndex = 1;
                }

                IQueryable<MaintenanceRequests> query = context.MaintenanceRequests.Where(x => usersites.Contains(x.site_id) && !x.is_archive);

                if (requestModel.mr_filter_type > 0)
                {
                    query = query.Where(x => x.status == requestModel.mr_filter_type);
                }
                
                if (requestModel.type > 0)
                {
                    query = query.Where(x => x.mr_type == requestModel.type);
                }

                if (requestModel.requested_by != null && requestModel.requested_by != "")
                {
                    query = query.Where(x => x.requested_by.ToString() == requestModel.requested_by);
                }

                if (!string.IsNullOrEmpty(requestModel.search_string))
                {
                    var searchstring = requestModel.search_string.ToLower().ToString();
                    query = query.Where(x => (x.title.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring)
                             || x.priority.ToString().ToLower() == searchstring || x.PriorityStatusMaster.status_name.ToLower().Contains(searchstring)
                             || x.mr_type.ToString().ToLower() == searchstring || x.MRTypeStatusMaster.status_name.ToLower().Contains(searchstring)));
                }

                MRItems.listsize = query.Count();

                MRItems.list = query.Skip((requestModel.pageIndex - 1) * requestModel.pageSize).Take(requestModel.pageSize).ToList();

                MRItems.list = query.Include(x => x.Sites)
                    .Include(x => x.Asset)
                    .Include(x => x.StatusMaster).Include(x => x.PriorityStatusMaster).Include(x => x.MRTypeStatusMaster).Include(x => x.WorkOrders)
                    .OrderByDescending(x => x.created_at).ToListAsync().Result;

            }

            return MRItems;
        }

        public async Task<List<MaintenanceRequests>> MaintenanceRequestOpenStatusCount(string userid)
        {
            List<MaintenanceRequests> openStatusList = new List<MaintenanceRequests>();
            if (userid != null)
            {
                List<Guid> usersites = new List<Guid>();
                if (String.IsNullOrEmpty(GenericRequestModel.site_id))
                {
                    usersites = await context.User.Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).Select(x => x.ac_default_site.Value).ToListAsync();
                }
                else if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                {
                    usersites = await context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToListAsync();
                }
                else
                {
                    usersites = await context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active && x.site_id.ToString() == GenericRequestModel.site_id).Select(x => x.site_id).ToListAsync();
                }

                openStatusList = await context.MaintenanceRequests.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset)
                    .Where(x => usersites.Contains(x.site_id) && x.status == (int)Status.MROpen).OrderBy(x => x.created_at).ToListAsync();
            }
            return openStatusList;
        }

        public Inspection GetInspectionIdByIssueId(Guid issueid)
        {
            return context.Issue.Include(x => x.Inspection).Where(x => x.issue_uuid == issueid).Select(x => x.Inspection).FirstOrDefault();
        }

        public async Task<List<WorkOrders>> GetAllWorkOrderWithNoMR(string assetid, string searchstring)
        {
            List<WorkOrders> woList = new List<WorkOrders>();
            IQueryable<WorkOrders> query = context.WorkOrders.Include(x => x.MaintenanceRequests).Where(x => x.MaintenanceRequests.Count == 0 && x.asset_id.ToString() == assetid
                        && (x.wo_type == (int)Status.WOManual || x.wo_type == (int)Status.WOInspection || x.wo_type == (int)Status.WOManualMaintenaceRequest) && x.status == (int)Status.WOOpen);

            if (!String.IsNullOrEmpty(searchstring))
            {
                searchstring = searchstring.Trim().ToLower();
                query = query.Where(x => x.wo_number.ToString().Contains(searchstring) || x.manual_wo_number.Contains(searchstring)  || x.title.ToLower().Contains(searchstring));
            }
            woList = query.ToList();
            return woList;
        }

        public async Task<List<MaintenanceRequests>> GetMRsByWorkOrderId(Guid wo_id)
        {
            List<MaintenanceRequests> mrList = new List<MaintenanceRequests>();
            IQueryable<MaintenanceRequests> query = context.MaintenanceRequests.Where(u => u.wo_id == wo_id).Include(x => x.Asset).Include(x => x.Issue);
            mrList = query.ToList();
            return mrList;
        }

        //public async Task<ListViewModel<MaintenanceRequests>> FilterTypeOptions(FilterMaintenanceRequestModel requestModel)
        //{
        //    ListViewModel<MaintenanceRequests> MRItems = new ListViewModel<MaintenanceRequests>();
        //    if (!String.IsNullOrEmpty(GenericRequestModel.site_id))
        //    {
        //        List<Guid> usersites = new List<Guid>();
        //        if (GenericRequestModel.site_status == (int)Status.AllSiteType)
        //        {
        //            usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == GenericRequestModel.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
        //        }
        //        else
        //        {
        //            usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == GenericRequestModel.requested_by && x.status == (int)Status.Active && x.site_id.ToString() == GenericRequestModel.site_id).Select(x => x.site_id).ToList();
        //        }
        //        if (requestModel.pageIndex == 0 || requestModel.pageSize == 0)
        //        {
        //            requestModel.pageSize = 20;
        //            requestModel.pageIndex = 1;
        //        }

        //        IQueryable<MaintenanceRequests> query = context.MaintenanceRequests.Where(x => usersites.Contains(x.site_id) && !x.is_archive);

        //        // MR Filter Type 
        //        if (requestModel.type == (int)Status.Manual)
        //        {
        //            query = query.Where(x => x.mr_type == (int)Status.Manual);
        //        }
        //        else if (requestModel.type == (int)Status.Inspection)
        //        {
        //            query = query.Where(x => x.mr_type == (int)Status.Inspection);
        //        }

        //        // MR Status
        //        if (requestModel.status == (int)Status.MROpen)
        //        {
        //            query = query.Where(x => x.status == (int)Status.MROpen);
        //        }
        //        else if (requestModel.status == (int)Status.MRCancelled)
        //        {
        //            query = query.Where(x => x.status == (int)Status.MRCancelled);
        //        }
        //        else if (requestModel.status == (int)Status.MRCompeleted)
        //        {
        //            query = query.Where(x => x.status == (int)Status.MRCompeleted);
        //        }
        //        else if (requestModel.status == (int)Status.MRWorkOrderCreated)
        //        {
        //            query = query.Where(x => x.status == (int)Status.MRWorkOrderCreated);
        //        }

        //        // add requested by Filter
        //        if (requestModel.requested_by?.Count > 0)
        //        {
        //            query = query.Where(x => requestModel.requested_by.Contains(x.requested_by.ToString()));
        //        }

        //        // search string
        //        if (!string.IsNullOrEmpty(requestModel.search_string))
        //        {
        //            var searchstring = requestModel.search_string.ToLower().ToString();
        //            query = query.Where(x => (x.title.ToLower().Contains(searchstring) ||
        //            (x.mr_type.ToString().ToLower().Contains(searchstring) ||
        //            x.requested_by.ToString().ToLower().Contains(searchstring) 
        //            //|| x.WorkOrders.wo_number.ToString().ToLower().Contains(searchstring)
        //            //|| x.WorkOrders.status.ToString().ToLower().Contains(searchstring) || x.WorkOrders.StatusMaster.status_name.ToString().ToLower() == searchstring
        //            || x.mr_type.ToString().ToLower() == searchstring || x.MRTypeStatusMaster.status_name.ToLower() == searchstring
        //            || x.priority.ToString().ToLower() == searchstring || x.PriorityStatusMaster.status_name.ToLower() == searchstring
        //            || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring))));
        //        }

        //        MRItems.listsize = query.Count();

        //        MRItems.list = MRItems.list.Skip((requestModel.pageIndex - 1) * requestModel.pageSize).Take(requestModel.pageSize).ToList();

        //        var MRList = query.Include(x => x.Sites)
        //            .Include(x => x.Asset)
        //            .Include(x => x.StatusMaster).Include(x => x.PriorityStatusMaster).Include(x => x.MRTypeStatusMaster).Include(x => x.WorkOrders)
        //            .OrderByDescending(x => x.created_at).ToListAsync().Result;

        //        //MRItems.list = MRList.GroupBy(x => x.mr_type).Select(x => x.First()).OrderByDescending(x => x.created_at).Distinct().ToList();
        //        MRItems.list = MRList.OrderByDescending(x => x.created_at).ToList();

        //    }
        //    return MRItems;
        //}

        public async Task<ListViewModel<MaintenanceRequests>> FilterRequestedByOptions(FilterMaintenanceRequestModel requestModel)
        {
            ListViewModel<MaintenanceRequests> maintenancerequest = new ListViewModel<MaintenanceRequests>();
            if (!String.IsNullOrEmpty(GenericRequestModel.site_id))
            {
                List<Guid> usersites = new List<Guid>();
                if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == GenericRequestModel.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == GenericRequestModel.requested_by && x.status == (int)Status.Active && x.site_id.ToString() == GenericRequestModel.site_id).Select(x => x.site_id).ToList();
                }
                if (requestModel.pageIndex == 0 || requestModel.pageSize == 0)
                {
                    requestModel.pageSize = 20;
                    requestModel.pageIndex = 1;
                }

                IQueryable<MaintenanceRequests> query = context.MaintenanceRequests.Where(x => usersites.Contains(x.site_id) && !x.is_archive);

                // MR Status
                if (requestModel.mr_filter_type > 0)
                {
                    query = query.Where(x => x.status == requestModel.mr_filter_type);
                }

                // MR Filter Type 
                if (requestModel.type == (int)Status.Manual)
                {
                    query = query.Where(x => x.mr_type == (int)Status.Manual);
                }
                else if (requestModel.type == (int)Status.Inspection)
                {
                    query = query.Where(x => x.mr_type == (int)Status.Inspection);
                }

                // add requested by Filter
                if (requestModel.requested_by != null && requestModel.requested_by != "")
                {
                    query = query.Where(x => x.requested_by.ToString() == requestModel.requested_by);
                }

                // search string
                if (!string.IsNullOrEmpty(requestModel.search_string))
                {
                    var searchstring = requestModel.search_string.ToLower().ToString();
                    query = query.Where(x => (x.title.ToLower().Contains(searchstring) ||
                    (x.mr_type.ToString().ToLower().Contains(searchstring) ||
                    x.requested_by.ToString().ToLower().Contains(searchstring)
                    || x.mr_type.ToString().ToLower() == searchstring || x.MRTypeStatusMaster.status_name.ToLower() == searchstring
                    || x.priority.ToString().ToLower() == searchstring || x.PriorityStatusMaster.status_name.ToLower() == searchstring
                    || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring))));
                }

                if (!string.IsNullOrEmpty(requestModel.option_search_string))
                {
                    // add option search string filter
                    string searchstring = requestModel.option_search_string.ToLower().ToString();
                    var requestBy = query.Select(x => x.requested_by.ToString()).ToList();
                    var users = context.User.Where(x => requestBy.Contains(x.uuid.ToString()) && (x.username.ToLower().Contains(searchstring) || x.firstname.ToLower().Contains(searchstring) || x.lastname.ToLower().Contains(searchstring))).Select(x => x.uuid.ToString()).ToList(); 
                    query = query.Where(x => users.Contains(x.requested_by.ToString()));
                }

                //if (requestModel.pageIndex == 0 || requestModel.pageSize == 0)
                //{
                //    requestModel.pageSize = 20;
                //    requestModel.pageIndex = 1;
                //}
                //maintenancerequest.listsize = query.Count();

                //query = query.OrderByDescending(x => x.created_at).Skip((requestModel.pageIndex - 1) * requestModel.pageSize).Take(requestModel.pageSize);

                maintenancerequest.list = query.ToListAsync().Result;
                //maintenancerequest.pageIndex = requestModel.pageIndex;
                //maintenancerequest.pageSize = requestModel.pageSize;
            }
            return maintenancerequest;
        }

    }
}
