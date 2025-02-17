using AutoMapper;
using Jarvis.db.ExcludePropertiesfromDBHelper;
using DocumentFormat.OpenXml.Spreadsheet;
using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages;
using Microsoft.VisualBasic;

namespace Jarvis.Repo.Concrete
{
    public class AssetPMsRepository : BaseGenericRepository<AssetPMs>, IAssetPMsRepository
    {
        public AssetPMsRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }
        public virtual async Task<int> Insert(AssetPMs entity)
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
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public virtual async Task<int> InsertList(IEnumerable<AssetPMs> entity)
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
                    AddRange(entity);
                    IsSuccess = (int)ResponseStatusNumber.Success;
                }
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public virtual async Task<int> Update(AssetPMs entity)
        {
            int IsSuccess = 0;
            try
            {
                dbSet.Update(entity);
                IsSuccess = await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public async Task<AssetPMPlans> GetAssetPMPlanById(Guid asset_pm_plan_id)
        {
            return await context.AssetPMPlans.Where(u => u.asset_pm_plan_id == asset_pm_plan_id && u.status!=(int)Status.Completed)
                .Include(x => x.AssetPMs).ThenInclude(x => x.AssetPMTasks)
                .Include(x => x.AssetPMs).ThenInclude(x => x.AssetPMAttachments)
                .Include(x => x.AssetPMs).ThenInclude(x => x.ServiceDealers)
                .Include(x => x.StatusMaster).FirstOrDefaultAsync();
        }

        public async Task<AssetPMs> GetAssetPMById(Guid asset_pm_id)
        {
            return await context.AssetPMs.Where(u => u.asset_pm_id == asset_pm_id)
                .Include(x => x.PMTriggers).ThenInclude(x => x.PMTriggersTasks)
                .Include(x => x.Asset).Include(x => x.AssetPMTasks)
                .Include(x => x.AssetPMAttachments)
                .Include(x => x.ServiceDealers).ThenInclude(x => x.StatusMaster)
                .Include(x => x.StatusMaster).FirstOrDefaultAsync();
        }
        public async Task<AssetPMs> GetAssetPMByIdForView(Guid asset_pm_id)
        {
            return await context.AssetPMs.Where(u => u.asset_pm_id == asset_pm_id)
                .Include(x=>x.Asset).ThenInclude(x=>x.AssetIssue)
                .Include(x => x.AssetPMAttachments)
                .Include(x => x.PMTypeStatus)
                .Include(x => x.PMByStatus)
                .Include(x => x.PMDateTimeRepeatTypeStatus)
                 .Include(x => x.AssetPMsTriggerConditionMapping)
                .Include(x => x.StatusMaster).FirstOrDefaultAsync();
        }
        public async Task<AssetPMs> GetAssetPMByIdForUpdate(Guid asset_pm_id)
        {
            return await context.AssetPMs.Where(u => u.asset_pm_id == asset_pm_id)
                .Include(x=>x.AssetPMsTriggerConditionMapping)
                .Include(x => x.AssetPMAttachments)
                .Include(x=>x.Asset)
                .Include(x => x.StatusMaster).FirstOrDefaultAsync();
        }

        public async Task<List<AssetPMs>> GetAssetPMByAssetId(Guid asset_id, int filter_type)
        {
            IQueryable<AssetPMs> query = context.AssetPMs.Where(u => u.asset_id == asset_id && !u.is_archive);

            // Asset PM Filter Type Status
            if (filter_type == (int)DashboardPMType.CurrentUpcoming)
            {
                //var pmTrigger = context.PMTriggers.Where(x => x.asset_pm_status != (int)Status.PMCompleted).Select(y => y.asset_pm_id).ToList();

                //query = query.Where(z => pmTrigger.Contains(z.asset_pm_id));

                query = query.Where(x => x.status != (int)Status.PMCompleted);
            }
            else if (filter_type == (int)DashboardPMType.Completed)
            {
                //var pmTrigger = context.PMTriggers.Where(x => x.asset_pm_status == (int)Status.PMCompleted).Select(y => y.asset_pm_id).ToList();

                //query = query.Where(z => pmTrigger.Contains(z.asset_pm_id));

                query = query.Where(x => x.status == (int)Status.PMCompleted);
            }

            return await query
                .Include(x => x.PMTriggers).ThenInclude(x => x.PMTriggersTasks).Include(x => x.AssetPMTasks).ThenInclude(x => x.Tasks)
                .Include(x => x.AssetPMAttachments).Include(x => x.ServiceDealers).ThenInclude(x => x.StatusMaster)
                .Include(x => x.StatusMaster).Include(x => x.PMByStatus).Include(x => x.PMTypeStatus)
                .Include(x => x.PMDateTimeRepeatTypeStatus).Include(x => x.AssetPMPlans).ToListAsync();

            //return await context.AssetPMs.Where(u => u.asset_id == asset_id && !u.is_archive)
            //    .Include(x => x.PMTriggers).ThenInclude(x => x.PMTriggersTasks).Include(x => x.AssetPMTasks).ThenInclude(x => x.Tasks)
            //    .Include(x => x.AssetPMAttachments).Include(x => x.ServiceDealers).ThenInclude(x => x.StatusMaster)
            //    .Include(x => x.StatusMaster).Include(x => x.PMByStatus).Include(x => x.PMTypeStatus)
            //    .Include(x => x.PMDateTimeRepeatTypeStatus).Include(x => x.AssetPMPlans).ToListAsync();
        }

        public async Task<ListViewModel<PMTriggers>> GetPendingPMItems(DashboardPendingPMItemsRequestModel requestModel)
        {
            ListViewModel<PMTriggers> PMItems = new ListViewModel<PMTriggers>();
            if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
            {
                List<Guid> usersites = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (requestModel.pageIndex == 0 || requestModel.pageSize == 0)
                {
                    requestModel.pageSize = 20;
                    requestModel.pageIndex = 1;
                }

                IQueryable<PMTriggers> query = context.PMTriggers.Where(x => !x.is_archive && usersites.Contains(x.Asset.site_id)).Include(x => x.PMTriggersRemarks);
                // x.status != (int)Status.TriggerCompleted && x.due_datetime != null && x.due_datetime.Value > DateTime.UtcNow.Date
                //).OrderBy(x => x.status).ThenByDescending(x => x.due_datetime);

                // PM Filter Type Status
                if (requestModel.pm_filter_type == (int)DashboardPMType.CurrentUpcoming)
                {
                    query = query.Where(x => x.asset_pm_status != (int)Status.TriggerCompleted);
                }
                else if (requestModel.pm_filter_type == (int)DashboardPMType.Completed)
                {
                    query = query.Where(x => x.asset_pm_status == (int)Status.TriggerCompleted);
                }

                // PM Status
                if (requestModel.pm_status == (int)Status.OverDue)
                {
                    query = query.Where(x => x.status == (int)Status.OverDue);
                }
                else if (requestModel.pm_status == (int)Status.TriggerNew)
                {
                    query = query.Where(x => x.status == (int)Status.TriggerNew);
                }
                else if (requestModel.pm_status == (int)Status.PMWaiting)
                {
                    query = query.Where(x => x.status == (int)Status.PMWaiting);
                }
                else if (requestModel.pm_status == (int)Status.PMInProgress)
                {
                    query = query.Where(x => x.status == (int)Status.PMInProgress);
                }

                // add internal_asset_id Filter
                if (requestModel.internal_asset_id?.Count > 0)
                {
                    query = query.Where(x => requestModel.internal_asset_id.Contains(x.Asset.internal_asset_id));
                }

                // add pm Plan id Filter
                if (requestModel.pm_plan_id?.Count > 0)
                {
                    query = query.Where(x => requestModel.pm_plan_id.Contains(x.AssetPMs.AssetPMPlans.pm_plan_id.ToString()));
                }

                // add pm id Filter
                if (requestModel.pm_id?.Count > 0)
                {
                    query = query.Where(x => requestModel.pm_id.Contains(x.AssetPMs.pm_id.ToString()));
                }

                // add site_id Filter
                if (requestModel.site_id?.Count > 0)
                {
                    query = query.Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()));
                }

                // search string
                if (!string.IsNullOrEmpty(requestModel.search_string))
                {
                    var searchstring = requestModel.search_string.ToLower().ToString();
                    query = query.Where(x => (x.AssetPMs.title.ToLower().Contains(searchstring) || (x.AssetPMs.AssetPMPlans.plan_name.ToLower().Contains(searchstring) || x.Asset.name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring) ||
                    x.Asset.Sites.site_name.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring))));
                }

                PMItems.listsize = query.Count();

                var QueryToList = query.Include(x => x.Asset).ThenInclude(x => x.Sites).Include(x => x.AssetPMs).ThenInclude(x => x.AssetPMPlans).Include(x => x.PMTriggersTasks)
                    .ThenInclude(x => x.AssetPMTasks).ThenInclude(x => x.Tasks).Include(x => x.StatusMaster).Include(x => x.PMTriggersRemarks)
                    .Include(x => x.AssetPMs.ServiceDealers).Include(x => x.AssetPMs.ServiceDealers.StatusMaster)
                    .Include(x => x.AssetPMs.PMTriggers).ThenInclude(x => x.PMTriggersTasks).Include(x => x.AssetPMs.AssetPMTasks).ThenInclude(x => x.Tasks)
                    .Include(x => x.AssetPMs.AssetPMAttachments)
                    .Include(x => x.AssetPMs.StatusMaster).Include(x => x.AssetPMs.PMByStatus).Include(x => x.AssetPMs.PMTypeStatus)
                    .Include(x => x.AssetPMs.PMDateTimeRepeatTypeStatus).Include(x => x.AssetPMs.AssetPMPlans).ToList();


                var groupByQuery = QueryToList.GroupBy(trigger => trigger.status)
                    .Select(group =>
                    group.Key == (int)Status.TriggerCompleted ? group.OrderByDescending(x => x.PMTriggersRemarks.completed_on) :
                    group.Key == (int)Status.Due || group.Key == (int)Status.OverDue ? group.OrderByDescending(x => x.due_datetime) : group.OrderBy(x => x.due_datetime)
                    ).OrderBy(group => group.First().status);

                PMItems.list = new List<PMTriggers>();
                foreach(var item in groupByQuery)
                {
                    PMItems.list.AddRange(item);
                }
                PMItems.list = PMItems.list.Skip((requestModel.pageIndex - 1) * requestModel.pageSize).Take(requestModel.pageSize).ToList();

                //PMItems.list = query.Include(x => x.Asset).ThenInclude(x => x.Sites).Include(x => x.AssetPMs).ThenInclude(x => x.AssetPMPlans).Include(x => x.PMTriggersTasks)
                //    .ThenInclude(x => x.AssetPMTasks).ThenInclude(x => x.Tasks).Include(x => x.StatusMaster)
                //    .Include(x => x.AssetPMs.ServiceDealers).Include(x => x.AssetPMs.ServiceDealers.StatusMaster).Include(x => x.PMTriggersRemarks).ToList();

            }
            return PMItems;
        }

        public async Task<ListViewModel<Asset>> FilterPendingPMItemsAssetIds(FilterPendingPMItemsOptionsRequestModel requestModel)
        {
            ListViewModel<Asset> PMItems = new ListViewModel<Asset>();
            if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
            {
                List<Guid> usersites = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (requestModel.pageIndex == 0 || requestModel.pageSize == 0)
                {
                    requestModel.pageSize = 20;
                    requestModel.pageIndex = 1;
                }

                //IQueryable<PMTriggers> query = context.PMTriggers.Where(x => x.status != (int)Status.TriggerCompleted && !x.is_archive && usersites.Contains(x.Asset.site_id) && x.due_datetime != null && x.due_datetime.Value > DateTime.UtcNow.Date).OrderByDescending(x => x.due_datetime);
                IQueryable<PMTriggers> query = context.PMTriggers.Where(x => !x.is_archive && usersites.Contains(x.Asset.site_id)
                // x.status != (int)Status.TriggerCompleted && x.due_datetime != null && x.due_datetime.Value > DateTime.UtcNow.Date
                ).OrderBy(x => x.status).ThenByDescending(x => x.due_datetime);

                // PM Filter Type Status
                if (requestModel.pm_filter_type == (int)DashboardPMType.CurrentUpcoming)
                {
                    query = query.Where(x => x.status != (int)Status.TriggerCompleted);
                }
                else if (requestModel.pm_filter_type == (int)DashboardPMType.Completed)
                {
                    query = query.Where(x => x.status == (int)Status.TriggerCompleted);
                }

                // PM Status
                if (requestModel.pm_status == (int)Status.OverDue)
                {
                    query = query.Where(x => x.status == (int)Status.OverDue);
                }
                else if (requestModel.pm_status == (int)Status.TriggerNew)
                {
                    query = query.Where(x => x.status == (int)Status.TriggerNew);
                }
                else if (requestModel.pm_status == (int)Status.PMWaiting)
                {
                    query = query.Where(x => x.status == (int)Status.PMWaiting);
                }
                else if (requestModel.pm_status == (int)Status.PMInProgress)
                {
                    query = query.Where(x => x.status == (int)Status.PMInProgress);
                }

                // add internal_asset_id Filter
                if (requestModel.internal_asset_id?.Count > 0)
                {
                    query = query.Include(x => x.Asset).Where(x => requestModel.internal_asset_id.Contains(x.Asset.internal_asset_id));
                }

                // add pm Plan id Filter
                if (requestModel.pm_plan_id?.Count > 0)
                {
                    query = query.Include(x => x.AssetPMs.AssetPMPlans).Where(x => requestModel.pm_plan_id.Contains(x.AssetPMs.AssetPMPlans.pm_plan_id.ToString()));
                }

                // add pm id Filter
                if (requestModel.pm_id?.Count > 0)
                {
                    query = query.Where(x => requestModel.pm_id.Contains(x.AssetPMs.pm_id.ToString()));
                }

                // add site_id Filter
                if (requestModel.site_id?.Count > 0)
                {
                    query = query.Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()));
                }

                // search string
                if (!string.IsNullOrEmpty(requestModel.search_string))
                {
                    var searchstring = requestModel.search_string.ToLower().ToString();
                    query = query.Where(x => (x.AssetPMs.title.ToLower().Contains(searchstring) ||
                    (x.AssetPMs.AssetPMPlans.plan_name.ToLower().Contains(searchstring) ||
                    x.Asset.name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring) ||
                    x.Asset.Sites.site_name.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring
                    || x.StatusMaster.status_name.ToLower().Contains(searchstring))));
                }

                // search option string
                if (!string.IsNullOrEmpty(requestModel.option_search_string))
                {
                    var searchstring = requestModel.option_search_string.ToLower().ToString();
                    query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) ||
                    x.Asset.internal_asset_id.ToLower().Contains(searchstring)));
                }

                PMItems.listsize = query.Count();
                PMItems.list = query.Select(x => x.Asset).OrderBy(x => x.name).Distinct().ToList();

                PMItems.list = PMItems.list.Skip((requestModel.pageIndex - 1) * requestModel.pageSize).Take(requestModel.pageSize).ToList();

            }
            return PMItems;
        }

        public async Task<ListViewModel<AssetPMPlans>> FilterPendingPMItemsPMPlans(FilterPendingPMItemsOptionsRequestModel requestModel)
        {
            ListViewModel<AssetPMPlans> PMItems = new ListViewModel<AssetPMPlans>();
            if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
            {
                List<Guid> usersites = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (requestModel.pageIndex == 0 || requestModel.pageSize == 0)
                {
                    requestModel.pageSize = 20;
                    requestModel.pageIndex = 1;
                }

                //IQueryable<PMTriggers> query = context.PMTriggers.Where(x => x.status != (int)Status.TriggerCompleted && !x.is_archive && usersites.Contains(x.Asset.site_id) && x.due_datetime != null && x.due_datetime.Value > DateTime.UtcNow.Date).OrderByDescending(x => x.due_datetime);
                IQueryable<PMTriggers> query = context.PMTriggers.Where(x => !x.is_archive && usersites.Contains(x.Asset.site_id)
                // x.status != (int)Status.TriggerCompleted && x.due_datetime != null && x.due_datetime.Value > DateTime.UtcNow.Date
                ).OrderBy(x => x.status).ThenByDescending(x => x.due_datetime);

                // PM Filter Type Status
                if (requestModel.pm_filter_type == (int)DashboardPMType.CurrentUpcoming)
                {
                    query = query.Where(x => x.status != (int)Status.TriggerCompleted);
                }
                else if (requestModel.pm_filter_type == (int)DashboardPMType.Completed)
                {
                    query = query.Where(x => x.status == (int)Status.TriggerCompleted);
                }

                // PM Status
                if (requestModel.pm_status == (int)Status.OverDue)
                {
                    query = query.Where(x => x.status == (int)Status.OverDue);
                }
                else if (requestModel.pm_status == (int)Status.TriggerNew)
                {
                    query = query.Where(x => x.status == (int)Status.TriggerNew);
                }
                else if (requestModel.pm_status == (int)Status.PMWaiting)
                {
                    query = query.Where(x => x.status == (int)Status.PMWaiting);
                }
                else if (requestModel.pm_status == (int)Status.PMInProgress)
                {
                    query = query.Where(x => x.status == (int)Status.PMInProgress);
                }

                // add internal_asset_id Filter
                if (requestModel.internal_asset_id?.Count > 0)
                {
                    query = query.Include(x => x.Asset).Where(x => requestModel.internal_asset_id.Contains(x.Asset.internal_asset_id));
                }

                // add pm Plan id Filter
                if (requestModel.pm_plan_id?.Count > 0)
                {
                    query = query.Include(x => x.AssetPMs.AssetPMPlans).Where(x => requestModel.pm_plan_id.Contains(x.AssetPMs.AssetPMPlans.pm_plan_id.ToString()));
                }

                // add pm id Filter
                if (requestModel.pm_id?.Count > 0)
                {
                    query = query.Where(x => requestModel.pm_id.Contains(x.AssetPMs.pm_id.ToString()));
                }

                // add site_id Filter
                if (requestModel.site_id?.Count > 0)
                {
                    query = query.Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()));
                }

                // search string
                if (!string.IsNullOrEmpty(requestModel.search_string))
                {
                    var searchstring = requestModel.search_string.ToLower().ToString();
                    query = query.Where(x => (x.AssetPMs.title.ToLower().Contains(searchstring) ||
                    (x.AssetPMs.AssetPMPlans.plan_name.ToLower().Contains(searchstring) ||
                    x.Asset.name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring) ||
                    x.Asset.Sites.site_name.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring
                    || x.StatusMaster.status_name.ToLower().Contains(searchstring))));
                }

                // search option string
                if (!string.IsNullOrEmpty(requestModel.option_search_string))
                {
                    var searchstring = requestModel.option_search_string.ToLower().ToString();
                    query = query.Where(x => x.AssetPMs.AssetPMPlans.plan_name.ToLower().Contains(searchstring));
                }

                PMItems.listsize = query.Count();
                var AssetPMPlanList = query.Include(x => x.AssetPMs).ThenInclude(x => x.AssetPMPlans).Select(x => x.AssetPMs.AssetPMPlans).ToList();
                PMItems.list = AssetPMPlanList.GroupBy(x => x.pm_plan_id).Select(x => x.First()).OrderBy(x => x.plan_name).Distinct().ToList();

                PMItems.list = PMItems.list.Skip((requestModel.pageIndex - 1) * requestModel.pageSize).Take(requestModel.pageSize).ToList();

            }
            return PMItems;
        }

        public async Task<ListViewModel<AssetPMs>> FilterPendingPMItemsPMItems(FilterPendingPMItemsOptionsRequestModel requestModel)
        {
            ListViewModel<AssetPMs> PMItems = new ListViewModel<AssetPMs>();
            if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
            {
                List<Guid> usersites = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (requestModel.pageIndex == 0 || requestModel.pageSize == 0)
                {
                    requestModel.pageSize = 20;
                    requestModel.pageIndex = 1;
                }

                //IQueryable<PMTriggers> query = context.PMTriggers.Where(x => x.status != (int)Status.TriggerCompleted && !x.is_archive && usersites.Contains(x.Asset.site_id) && x.due_datetime != null && x.due_datetime.Value > DateTime.UtcNow.Date).OrderByDescending(x => x.due_datetime);
                IQueryable<PMTriggers> query = context.PMTriggers.Where(x => !x.is_archive && usersites.Contains(x.Asset.site_id)
                // x.status != (int)Status.TriggerCompleted && x.due_datetime != null && x.due_datetime.Value > DateTime.UtcNow.Date
                ).OrderBy(x => x.status).ThenByDescending(x => x.due_datetime);

                // PM Filter Type Status
                if (requestModel.pm_filter_type == (int)DashboardPMType.CurrentUpcoming)
                {
                    query = query.Where(x => x.status != (int)Status.TriggerCompleted);
                }
                else if (requestModel.pm_filter_type == (int)DashboardPMType.Completed)
                {
                    query = query.Where(x => x.status == (int)Status.TriggerCompleted);
                }

                // PM Status
                if (requestModel.pm_status == (int)Status.OverDue)
                {
                    query = query.Where(x => x.status == (int)Status.OverDue);
                }
                else if (requestModel.pm_status == (int)Status.TriggerNew)
                {
                    query = query.Where(x => x.status == (int)Status.TriggerNew);
                }
                else if (requestModel.pm_status == (int)Status.PMWaiting)
                {
                    query = query.Where(x => x.status == (int)Status.PMWaiting);
                }
                else if (requestModel.pm_status == (int)Status.PMInProgress)
                {
                    query = query.Where(x => x.status == (int)Status.PMInProgress);
                }

                // add internal_asset_id Filter
                if (requestModel.internal_asset_id?.Count > 0)
                {
                    query = query.Include(x => x.Asset).Where(x => requestModel.internal_asset_id.Contains(x.Asset.internal_asset_id));
                }

                // add pm Plan id Filter
                if (requestModel.pm_plan_id?.Count > 0)
                {
                    query = query.Include(x => x.AssetPMs.AssetPMPlans).Where(x => requestModel.pm_plan_id.Contains(x.AssetPMs.AssetPMPlans.pm_plan_id.ToString()));
                }

                // add pm id Filter
                if (requestModel.pm_id?.Count > 0)
                {
                    query = query.Where(x => requestModel.pm_id.Contains(x.AssetPMs.pm_id.ToString()));
                }

                // add site_id Filter
                if (requestModel.site_id?.Count > 0)
                {
                    query = query.Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()));
                }

                // search string
                if (!string.IsNullOrEmpty(requestModel.search_string))
                {
                    var searchstring = requestModel.search_string.ToLower().ToString();
                    query = query.Where(x => (x.AssetPMs.title.ToLower().Contains(searchstring) ||
                    (x.AssetPMs.AssetPMPlans.plan_name.ToLower().Contains(searchstring) ||
                    x.Asset.name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring) ||
                    x.Asset.Sites.site_name.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring
                    || x.StatusMaster.status_name.ToLower().Contains(searchstring))));
                }

                // search option string
                if (!string.IsNullOrEmpty(requestModel.option_search_string))
                {
                    var searchstring = requestModel.option_search_string.ToLower().ToString();
                    query = query.Where(x => x.AssetPMs.title.ToLower().Contains(searchstring));
                }

                PMItems.listsize = query.Count();
                PMItems.list = query.Select(x => x.AssetPMs).OrderBy(x => x.title).Distinct().ToList();

                PMItems.list = PMItems.list.Skip((requestModel.pageIndex - 1) * requestModel.pageSize).Take(requestModel.pageSize).ToList();

            }
            return PMItems;
        }

        public async Task<ListViewModel<Sites>> FilterPendingPMItemsSites(FilterPendingPMItemsOptionsRequestModel requestModel)
        {
            ListViewModel<Sites> Sites = new ListViewModel<Sites>();
            if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
            {
                List<Guid> usersites = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (requestModel.pageIndex == 0 || requestModel.pageSize == 0)
                {
                    requestModel.pageSize = 20;
                    requestModel.pageIndex = 1;
                }

                //IQueryable<PMTriggers> query = context.PMTriggers.Where(x => x.status != (int)Status.TriggerCompleted && !x.is_archive && usersites.Contains(x.Asset.site_id) && x.due_datetime != null && x.due_datetime.Value > DateTime.UtcNow.Date).OrderByDescending(x => x.due_datetime);
                IQueryable<PMTriggers> query = context.PMTriggers.Where(x => !x.is_archive && usersites.Contains(x.Asset.site_id)
                // x.status != (int)Status.TriggerCompleted && x.due_datetime != null && x.due_datetime.Value > DateTime.UtcNow.Date
                ).OrderBy(x => x.status).ThenByDescending(x => x.due_datetime);

                // PM Filter Type Status
                if (requestModel.pm_filter_type == (int)DashboardPMType.CurrentUpcoming)
                {
                    query = query.Where(x => x.status != (int)Status.TriggerCompleted);
                }
                else if (requestModel.pm_filter_type == (int)DashboardPMType.Completed)
                {
                    query = query.Where(x => x.status == (int)Status.TriggerCompleted);
                }

                // PM Status
                if (requestModel.pm_status == (int)Status.OverDue)
                {
                    query = query.Where(x => x.status == (int)Status.OverDue);
                }
                else if (requestModel.pm_status == (int)Status.TriggerNew)
                {
                    query = query.Where(x => x.status == (int)Status.TriggerNew);
                }
                else if (requestModel.pm_status == (int)Status.PMWaiting)
                {
                    query = query.Where(x => x.status == (int)Status.PMWaiting);
                }
                else if (requestModel.pm_status == (int)Status.PMInProgress)
                {
                    query = query.Where(x => x.status == (int)Status.PMInProgress);
                }

                // add internal_asset_id Filter
                if (requestModel.internal_asset_id?.Count > 0)
                {
                    query = query.Include(x => x.Asset).Where(x => requestModel.internal_asset_id.Contains(x.Asset.internal_asset_id));
                }

                // add pm Plan id Filter
                if (requestModel.pm_plan_id?.Count > 0)
                {
                    query = query.Include(x => x.AssetPMs.AssetPMPlans).Where(x => requestModel.pm_plan_id.Contains(x.AssetPMs.AssetPMPlans.pm_plan_id.ToString()));
                }

                // add pm id Filter
                if (requestModel.pm_id?.Count > 0)
                {
                    query = query.Where(x => requestModel.pm_id.Contains(x.AssetPMs.pm_id.ToString()));
                }

                // add site_id Filter
                if (requestModel.site_id?.Count > 0)
                {
                    query = query.Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()));
                }

                // search string
                if (!string.IsNullOrEmpty(requestModel.search_string))
                {
                    var searchstring = requestModel.search_string.ToLower().ToString();
                    query = query.Where(x => (x.AssetPMs.title.ToLower().Contains(searchstring) ||
                    (x.AssetPMs.AssetPMPlans.plan_name.ToLower().Contains(searchstring) ||
                    x.Asset.name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring) ||
                    x.Asset.Sites.site_name.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring
                    || x.StatusMaster.status_name.ToLower().Contains(searchstring))));
                }

                // search option string
                if (!string.IsNullOrEmpty(requestModel.option_search_string))
                {
                    var searchstring = requestModel.option_search_string.ToLower().ToString();
                    query = query.Where(x => x.Asset.site_location.ToLower().Contains(searchstring) || x.Asset.Sites.site_name.ToLower().Contains(searchstring));
                }

                Sites.listsize = query.Count();
                Sites.list = query.Select(x => x.Asset.Sites).Distinct().OrderBy(x => x.site_name).ToList();

                Sites.list = Sites.list.Skip((requestModel.pageIndex - 1) * requestModel.pageSize).Take(requestModel.pageSize).ToList();

            }
            return Sites;
        }

        public async Task<List<PMTriggers>> GetUpComingPMs()
        {
            List<PMTriggers> PMItems = new List<PMTriggers>();
            //var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
            if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
            {
                List<Guid> usersites = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }

                IQueryable<PMTriggers> query = context.PMTriggers.Where(x => x.status != (int)Status.TriggerCompleted && !x.is_archive && usersites.Contains(x.Asset.site_id) && x.due_datetime.Value != null && x.due_datetime.Value >= DateTime.UtcNow.Date).OrderByDescending(x => x.due_datetime);

                PMItems = query.ToList();

            }
            return PMItems;
        }

        public async Task<List<PMTriggers>> GetDueAssetPMs()
        {
            List<PMTriggers> PMItems = new List<PMTriggers>();

            IQueryable<PMTriggers> query = context.PMTriggers.Where(x => !x.is_archive && x.status == (int)Status.OverDue)
                .Include(x => x.StatusMaster)
                .Include(x => x.Asset)
                .ThenInclude(x => x.Sites)
                .Include(x => x.AssetPMs.ServiceDealers)
                .Include(x => x.AssetPMs.AssetPMPlans).OrderByDescending(x => x.due_datetime);

            PMItems = query.ToList();

            return PMItems;
        }

        public async Task<DashboardPMMetricsResponseModel> DashboardPMMetrics()
        {
            DashboardPMMetricsResponseModel responseMetrics = new DashboardPMMetricsResponseModel();
            if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
            {
                List<Guid> usersites = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }

                IQueryable<PMTriggers> query = context.PMTriggers.Where(x => !x.is_archive && usersites.Contains(x.Asset.site_id)).OrderByDescending(x => x.due_datetime);

                // PM Status
                responseMetrics.overdueCount = query.Where(x => x.status == (int)Status.OverDue).ToList().Count();

                responseMetrics.openCount = query.Where(x => x.status == (int)Status.TriggerNew).ToList().Count();

                responseMetrics.waitingCount = query.Where(x => x.status == (int)Status.PMWaiting).ToList().Count();

                responseMetrics.inProgressCount = query.Where(x => x.status == (int)Status.PMInProgress).ToList().Count();
            }
            return responseMetrics;
        }

        public async Task<List<ServiceDealers>> GetAllServiceDealers(string searchstring)
        {
            IQueryable<ServiceDealers> query = context.ServiceDealers;
            query = query.Where(x => (!x.is_archive));
            if (!String.IsNullOrEmpty(searchstring))
            {
                searchstring = searchstring.ToLower();
                query = query.Where(x => (x.name.ToLower().Contains(searchstring)) || (x.email.ToLower().Contains(searchstring)));
            }
            return query.OrderByDescending(x => x.created_at).ToList();
        }

        public int GetAssetPMCountByAssetId(Guid asset_id)
        {
            //var assetPMs = context.AssetPMs.Where(u => u.asset_id == asset_id && u.status == (int)Status.Active).ToListAsync();
            var assetPMs = context.AssetPMs.Where(u => u.asset_id == asset_id && !u.is_archive).ToListAsync();
            return assetPMs.Result.Count;
        }

        public async Task<List<AssetMeterHourHistory>> GetAssetMeterHourHistory(AssetMeterHourHistoryRequestModel requestModel)
        {
            IQueryable<AssetMeterHourHistory> query = context.AssetMeterHourHistory;
            if (!String.IsNullOrEmpty(requestModel.search_string))
            {
                requestModel.search_string = requestModel.search_string.ToLower();
                query = query.Where(u => u.asset_id == Guid.Parse(requestModel.asset_id)
                && (u.meter_hours.ToString().Contains(requestModel.search_string)));
            }
            else
            {
                query = query.Where(u => u.asset_id == Guid.Parse(requestModel.asset_id));
            }
            return await query.Include(x => x.Asset).ThenInclude(x => x.Sites).OrderByDescending(u => u.updated_at).ToListAsync();
        }
        public List<PMPlans> GetPMPlansByClassId(Guid inspectiontemplate_asset_class_id)
        {
            return context.PMPlans
                .Include(x=>x.PMs)
                .Include(x=>x.PMCategory)
                .Where(x => x.PMCategory.inspectiontemplate_asset_class_id == inspectiontemplate_asset_class_id && x.status == (int)Status.Active)
                .ToList();
        }
        public List<AssetPMs> GetAssetPMbyIDs(List<Guid> asset_pm_id)
        {
            return context.AssetPMs.Where(x => asset_pm_id.Contains(x.asset_pm_id)).ToList();
        }
        public (List<AssetPMs>, int) GetAssetPMList(GetAssetPMListRequestmodel requestmodel)
        {
            int total_list_count = 0;
            IQueryable<AssetPMs> query = context.AssetPMs
                .Include(x => x.Asset)
                .Include(x => x.AssetPMPlans)
                .Include(x => x.Asset).ThenInclude(x => x.InspectionTemplateAssetClass).ThenInclude(x=>x.FormIOType)
                .Where(x => !x.is_archive && x.Asset.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_pm_inspection_manual);

            if (requestmodel.asset_id != null && !requestmodel.is_requested_for_overdue_pm)
            {
                query = query.Where(x => x.asset_id == requestmodel.asset_id && x.pm_due_overdue_flag != (int)pm_due_overdue_flag.PM_Overdue);
            }
            else if (requestmodel.asset_id != null && requestmodel.is_requested_for_overdue_pm)
            {
                query = query.Where(x => x.asset_id == requestmodel.asset_id);
            }
            else
            {
                query = query.Where(x => x.is_assetpm_enabled);
            }

            if (requestmodel.asset_form_id != null)
            {
                query = query.Where(x => x.asset_form_id == requestmodel.asset_form_id);
            }

            // Add Asset Class Filter
            if (requestmodel.inspectiontemplate_asset_class_id != null && requestmodel.inspectiontemplate_asset_class_id.Count > 0)
            {
                query = query.Where(x => requestmodel.inspectiontemplate_asset_class_id.Contains(x.Asset.inspectiontemplate_asset_class_id.Value));
            }

            // Add AssetPM Title Filter
            if (requestmodel.title != null && requestmodel.title.Count > 0)
            {
                query = query.Where(x => requestmodel.title.Contains(x.title));
            }

            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.ToLower().Trim();
                query = query.Where(x =>
                x.title.ToLower().Trim().Contains(search)
              || x.AssetPMPlans.plan_name.ToLower().Trim().Contains(search)
              || x.Asset.name.ToLower().Trim().Contains(search)
              );
            }
            //if (requestmodel.is_requested_for_assign)
            //{
            //    query = query.Where(x => x.asset_form_id == null && x.wo_id == null && x.status == (int)Status.Active);
            //}
            //if (requestmodel.status != null && requestmodel.status.Count > 0)
            //{
            //    query = query.Where(x => requestmodel.status.Contains(x.status));
            //}
            
            if (requestmodel.is_requested_for_overdue_pm)
            {
                query = query.Where(x => x.due_date.Value.Date < DateTime.UtcNow.Date && x.status != (int)Status.Completed);
            }

            if (requestmodel.want_to_remove_overdue_pms)
            {
                query = query.Where(x => (x.due_date.Value.Date > DateTime.UtcNow.Date || x.due_date.Value.Date == DateTime.UtcNow.Date) 
                && x.status != (int)Status.Completed);
            }

            if (requestmodel.is_requested_for_current_assetpms)
            {
                //query = query.Where(x=>x.datetime_starting_at!=null && x.status != (int)Status.Completed && x.due_date > DateTime.UtcNow.Date);
                
                var list = query.Where(x => x.datetime_starting_at != null && x.status != (int)Status.Completed && x.due_date > DateTime.UtcNow.AddDays(-1).Date).ToList()
                    .GroupBy(x=>new {x.asset_id, x.pm_id })
                    .Select(g=>g.OrderBy(x=> x.datetime_starting_at).FirstOrDefault());

                query = query.Where(x => list.Select(y=>y.asset_pm_id).Contains(x.asset_pm_id));
            }
            if (requestmodel.is_requested_for_assign)
            {
                query = query.Where(x => x.asset_form_id == null && x.wo_id == null && x.status == (int)Status.Active);
            }
            if (requestmodel.status != null && requestmodel.status.Count > 0)
            {
                query = query.Where(x => requestmodel.status.Contains(x.status));
            }

            if (requestmodel.is_request_for_pm_report)
            {
                if (requestmodel.end_due_date != null)
                {
                    query = query.Where(x => x.due_date != null && (x.due_date < DateTime.UtcNow || x.status == 2 ||
                                                (x.due_date.Value.Date >= DateTime.UtcNow.Date && x.due_date.Value.Date <= requestmodel.end_due_date))
                                                && (x.status == (int)Status.Active || x.status == (int)Status.Schedule || x.status == (int)Status.InProgress));
                }
                else // if due date is not passed then return all open pms
                {
                    query = query.Where(x => x.due_date != null && (x.status == (int)Status.Active || x.status == (int)Status.Schedule || x.status == (int)Status.InProgress));
                }
            }

            query = query.OrderBy(x => x.status == 15).ThenBy(x => x.due_date);
            total_list_count = query.Count();
            if (requestmodel.pagesize > 0 && requestmodel.pageindex > 0)
            {
                query = query.Skip((requestmodel.pageindex - 1) * requestmodel.pagesize).Take(requestmodel.pagesize);
            }

            query = query
                        .Include(x => x.Asset).ThenInclude(x => x.AssetIssue)
                        .Include(x => x.Asset).ThenInclude(x => x.InspectionTemplateAssetClass)
                        .Include(x => x.Asset).ThenInclude(x => x.AssetTopLevelcomponentMapping)
                        .Include(x => x.Asset).ThenInclude(x=>x.AssetFormIOBuildingMappings).ThenInclude(x=>x.FormIOBuildings)
                        .Include(x => x.Asset).ThenInclude(x=>x.AssetFormIOBuildingMappings).ThenInclude(x=>x.FormIOFloors)
                        .Include(x => x.Asset).ThenInclude(x=>x.AssetFormIOBuildingMappings).ThenInclude(x=>x.FormIORooms)
                        .Include(x => x.Asset).ThenInclude(x=>x.AssetFormIOBuildingMappings).ThenInclude(x=>x.FormIOSections)
                        .Include(x => x.AssetPMPlans)
                        .Include(x => x.WorkOrders)
                        .Include(x => x.StatusMaster)
                        .Include(x => x.AssetPMsTriggerConditionMapping).ThenInclude(x => x.PMDateTimeRepeatTypeStatus);


            return (query.ToList(), total_list_count);
        }


        public (List<AssetPMListExcludeProperties>, int) GetAssetPMListOptimized(GetAssetPMListRequestmodel requestmodel)
        {
            List<AssetPMListExcludeProperties> response = new List<AssetPMListExcludeProperties>();

            int total_list_count = 0;
            IQueryable<AssetPMs> query = context.AssetPMs
                .Include(x => x.Asset)
                .Include(x => x.AssetPMPlans)
                .Include(x => x.Asset).ThenInclude(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.FormIOType)
                .Where(x => !x.is_archive && x.Asset.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_pm_inspection_manual);

            if (requestmodel.asset_id != null && !requestmodel.is_requested_for_overdue_pm)
                query = query.Where(x => x.asset_id == requestmodel.asset_id && x.pm_due_overdue_flag != (int)pm_due_overdue_flag.PM_Overdue);
            else if (requestmodel.asset_id != null && requestmodel.is_requested_for_overdue_pm)
                query = query.Where(x => x.asset_id == requestmodel.asset_id);
            else
                query = query.Where(x => x.is_assetpm_enabled);
            
            if (requestmodel.asset_form_id != null)
                query = query.Where(x => x.asset_form_id == requestmodel.asset_form_id);
            

            // Add Asset Class Filter
            if (requestmodel.inspectiontemplate_asset_class_id != null && requestmodel.inspectiontemplate_asset_class_id.Count > 0)
                query = query.Where(x => requestmodel.inspectiontemplate_asset_class_id.Contains(x.Asset.inspectiontemplate_asset_class_id.Value));
            
            // Add AssetPM Title Filter
            if (requestmodel.title != null && requestmodel.title.Count > 0)
                query = query.Where(x => requestmodel.title.Contains(x.title));
            

            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.ToLower().Trim();
                query = query.Where(x =>
                x.title.ToLower().Trim().Contains(search)
              || x.AssetPMPlans.plan_name.ToLower().Trim().Contains(search)
              || x.Asset.name.ToLower().Trim().Contains(search)
              );
            }

            //if (requestmodel.is_requested_for_assign)
            //    query = query.Where(x => x.asset_form_id == null && x.wo_id == null && x.status == (int)Status.Active);
            
            //if (requestmodel.status != null && requestmodel.status.Count > 0)
            //    query = query.Where(x => requestmodel.status.Contains(x.status));
            
            if (requestmodel.is_requested_for_overdue_pm)
                query = query.Where(x => x.due_date.Value.Date < DateTime.UtcNow.Date && x.status != (int)Status.Completed);
            

            if (requestmodel.want_to_remove_overdue_pms)
            {
                query = query.Where(x => (x.due_date.Value.Date > DateTime.UtcNow.Date || x.due_date.Value.Date == DateTime.UtcNow.Date)
                && x.status != (int)Status.Completed);
            }

            if (requestmodel.is_requested_for_current_assetpms)
            {
                //query = query.Where(x=>x.datetime_starting_at!=null && x.status != (int)Status.Completed && x.due_date > DateTime.UtcNow.Date);

                var list = query.Where(x => x.datetime_starting_at != null && x.status != (int)Status.Completed && x.due_date > DateTime.UtcNow.AddDays(-1).Date).ToList()
                    .GroupBy(x => new { x.asset_id, x.pm_id })
                    .Select(g => g.OrderBy(x => x.datetime_starting_at).FirstOrDefault());

                query = query.Where(x => list.Select(y => y.asset_pm_id).Contains(x.asset_pm_id));
            }

            if (requestmodel.is_requested_for_assign)
                query = query.Where(x => x.asset_form_id == null && x.wo_id == null && x.status == (int)Status.Active);

            if (requestmodel.status != null && requestmodel.status.Count > 0)
                query = query.Where(x => requestmodel.status.Contains(x.status));


            if (requestmodel.is_request_for_pm_report)
            {
                if (requestmodel.end_due_date != null)
                {
                    query = query.Where(x => x.due_date != null && (x.due_date < DateTime.UtcNow || x.status == 2 ||
                                                (x.due_date.Value.Date >= DateTime.UtcNow.Date && x.due_date.Value.Date <= requestmodel.end_due_date))
                                                && (x.status == (int)Status.Active || x.status == (int)Status.Schedule || x.status == (int)Status.InProgress));
                }
                else // if due date is not passed then return all open pms
                {
                    query = query.Where(x => x.due_date != null && (x.status == (int)Status.Active || x.status == (int)Status.Schedule || x.status == (int)Status.InProgress));
                }
            }

            /*
            if (requestmodel.is_request_for_pm_report)
            {
                if (requestmodel.end_due_date != null)
                {
                    query = query.Where(x => x.due_date != null && (x.due_date < DateTime.UtcNow || x.status == 2 ||
                                                (x.due_date.Value.Date >= DateTime.UtcNow.Date && x.due_date.Value.Date <= requestmodel.end_due_date))
                                                && (x.status == (int)Status.Active || x.status == (int)Status.Schedule || x.status == (int)Status.InProgress));
                }
                else // if due date is not passed then return all open pms
                {
                    query = query.Where(x => x.due_date != null && (x.status == (int)Status.Active || x.status == (int)Status.Schedule || x.status == (int)Status.InProgress));
                }
            }
            */

            query = query.OrderBy(x => x.status == 15).ThenBy(x => x.due_date);
            total_list_count = query.Count();

            if (requestmodel.pagesize > 0 && requestmodel.pageindex > 0)
            {
                query = query.Skip((requestmodel.pageindex - 1) * requestmodel.pagesize).Take(requestmodel.pagesize);
            }

            var site_name = context.Sites.Where(x=>x.site_id==Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).Select(x => x.site_name).FirstOrDefault();

            /*
            query = query
                        .Include(x => x.Asset).ThenInclude(x => x.AssetIssue)
                        .Include(x => x.Asset).ThenInclude(x => x.InspectionTemplateAssetClass)
                        .Include(x => x.Asset).ThenInclude(x => x.AssetTopLevelcomponentMapping)
                        .Include(x => x.Asset).ThenInclude(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOBuildings)
                        .Include(x => x.Asset).ThenInclude(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOFloors)
                        .Include(x => x.Asset).ThenInclude(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIORooms)
                        .Include(x => x.Asset).ThenInclude(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOSections)
                        .Include(x => x.AssetPMPlans)
                        .Include(x => x.WorkOrders)
                        .Include(x => x.StatusMaster)
                        .Include(x => x.AssetPMsTriggerConditionMapping).ThenInclude(x => x.PMDateTimeRepeatTypeStatus);
            */

            response = query.Select(x => new AssetPMListExcludeProperties
            {
                asset_pm_id = x.asset_pm_id,
                title = x.title,
                asset_id = x.asset_id,
                asset_name = x.Asset.name,
                facility_name = site_name,
                asset_pm_plan_id = x.asset_pm_plan_id,
                asset_plan_name = x.AssetPMPlans.plan_name,
                asset_class_name = x.Asset.InspectionTemplateAssetClass!=null ? x.Asset.InspectionTemplateAssetClass.asset_class_name:null,
                asset_class_code = x.Asset.InspectionTemplateAssetClass!=null ? x.Asset.InspectionTemplateAssetClass.asset_class_code:null,
                asset_class_type = x.Asset.InspectionTemplateAssetClass!=null && x.Asset.InspectionTemplateAssetClass.FormIOType!=null ? x.Asset.InspectionTemplateAssetClass.FormIOType.form_type_name:null,
                status = x.status,
                due_in = DateTimeUtil.GetDueOverdueTimingByDueDate(x.due_date.Value).Item1,
                due_date = x.due_date,
                estimation_time = x.estimation_time,
                //is_current_assetpm = IsThisCurrentAssetPM(x.asset_id,x.pm_id.Value,x.asset_pm_id),
                //pm_due_overdue_flag = x.due_date != null ? DateTime.UtcNow.Date == x.due_date.Value.Date ? (int)pm_due_overdue_flag.PM_Due : DateTime.UtcNow.Date <= x.due_date.Value.Date ? (int)pm_due_overdue_flag.PM_OnTrack : (int)pm_due_overdue_flag.PM_Overdue
                pm_due_overdue_flag = x.due_date != null
                    ? (DateTime.UtcNow.Date == x.due_date.Value.Date
                      ? (int)pm_due_overdue_flag.PM_Due
                         : (DateTime.UtcNow.Date > x.due_date.Value.Date
                            ? (int)pm_due_overdue_flag.PM_Overdue
                               : (int)pm_due_overdue_flag.PM_OnTrack))
                                 : x.pm_due_overdue_flag,

                is_current_assetpm = context.AssetPMs.Where(z => z.asset_id == x.asset_id && !z.is_archive
                && z.pm_id != null && z.pm_id == x.pm_id
                && z.status != (int)Status.Completed
                && z.pm_due_overdue_flag != (int)pm_due_overdue_flag.PM_Overdue)
               .OrderBy(c => c.datetime_starting_at).Select(c => c.asset_pm_id).FirstOrDefault() == x.asset_pm_id ? true : false

            }).ToList();

            return (response, total_list_count);
        }

        public int GetCompletedAssetPM()
        {
            return context.AssetPMs.Where(x => x.Asset.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
            && !x.is_archive
            && x.status == (int)Status.Completed
            && !x.is_pm_inspection_manual
            && x.is_assetpm_enabled
            ).Count();
        }
        public List<AssetPMs> GetActiveAssetPM()
        {
            return context.AssetPMs.Where(x => x.Asset.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
            && !x.is_archive
            && x.status != (int)Status.Completed
            && !x.is_pm_inspection_manual
            && x.is_assetpm_enabled
            ).Include(x=>x.Asset).ThenInclude(x=>x.AssetIssue)
            .Include(x=>x.AssetPMsTriggerConditionMapping)
            .Include(x => x.AssetPMPlans)
            .Include(x=>x.StatusMaster)
                .ToList();
        }
        public List<Asset> GetStaffAssers()
        {
            return context.Assets
                .Include(x => x.InspectionTemplateAssetClass)
                .Include(x => x.AssetPMs)
                .Where(x => x.site_id == Guid.Parse("e564c1f4-0fec-4927-998c-9cb816d6744a")
            && x.status ==3 
            )
                .ToList();
        }
        public PMPlans GetStaffPMplamns(Guid inspectiontemplate_asset_class_id)
        {
            return context.PMPlans
                .Include(x=>x.PMCategory)
                .Where(x => x.PMCategory.inspectiontemplate_asset_class_id == inspectiontemplate_asset_class_id).FirstOrDefault();
        }
        public InspectionTemplateAssetClass GetAssetclassbycode(string code)
        {
            return context.InspectionTemplateAssetClass.Where(x => x.asset_class_code == code && !x.isarchive && x.company_id == Guid.Parse("4122f9d4-5d95-4616-9b2b-1f4d72116cc6")).FirstOrDefault();
        }
        public int GetWOLineStatusByAssetFormId(Guid asset_form_id)
        {
            return context.AssetFormIO.Where(x => x.asset_form_id == asset_form_id).FirstOrDefault().status;
        }

        public (List<AssetPMs>, int) ReturnAllOverdueAssetPM(GetAssetPMListRequestmodel requestmodel)
        {
            int total_list_count = 0;
            IQueryable<AssetPMs> query = context.AssetPMs
                .Where(x => !x.is_archive && x.Asset.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.status != (int)Status.Completed)
                .Include(x => x.Asset)
                .Include(x => x.AssetPMPlans);

            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.ToLower().Trim();
                query = query.Where(x =>
                x.title.ToLower().Trim().Contains(search)
              || x.AssetPMPlans.plan_name.ToLower().Trim().Contains(search)
              || x.Asset.name.ToLower().Trim().Contains(search)
              );
            }
            
            total_list_count = query.Count();

            if (requestmodel.pagesize > 0 && requestmodel.pageindex > 0)
            {
                query = query.Skip((requestmodel.pageindex - 1) * requestmodel.pagesize).Take(requestmodel.pagesize);//.OrderBy(x => x.created_at);
            }

            query = query
                        .Include(x => x.Asset).ThenInclude(x => x.AssetIssue)
                        .Include(x => x.Asset).ThenInclude(x => x.InspectionTemplateAssetClass).ThenInclude(x=>x.FormIOType)
                        .Include(x => x.AssetPMPlans)
                        .Include(x => x.StatusMaster)
                        .Include(x => x.AssetPMsTriggerConditionMapping).ThenInclude(x => x.PMDateTimeRepeatTypeStatus);

            return (query.ToList(), total_list_count);
        }

        public string GetSiteNameById(Guid site_id)
        {
            return context.Sites.Where(x => x.site_id == site_id).Select(x => x.site_name).FirstOrDefault();
        }
        public List<Asset> GetAllAssetForPMReport(Guid site)
        {
            return context.Assets.Where(x => x.site_id == site && x.status == (int)Status.AssetActive)
                .Include(x => x.AssetPMs).ThenInclude(x => x.AssetPMPlans)
                .Include(x => x.InspectionTemplateAssetClass)
                .Include(x => x.AssetTopLevelcomponentMapping)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOSections)
                .Include(x => x.AssetPMs).ThenInclude(x => x.AssetPMsTriggerConditionMapping).ThenInclude(x => x.PMDateTimeRepeatTypeStatus)
                .ToList();
        }
        public List<AssetPMs> IsAnyPMAlreadyLinked(List<Guid> asset_pm_ids)
        {
            return context.AssetPMs.Where(x=>asset_pm_ids.Contains(x.asset_pm_id) && x.status != (int)Status.Active && !x.is_archive).ToList();
        }

        public List<Asset> GetceccoAssets()
        {
            //var date = DateTime.UtcNow.Date.AddDays(-1);

            var sites = context.Sites.Where(x => x.created_at > DateTime.Parse("03-04-2024 12:00:00 AM")
            && x.company_id == Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d")
            && x.site_id != Guid.Parse("1637f8c3-725c-4863-bda1-fc150628a026") //1000 Remington Blvd. site
            && x.site_id != Guid.Parse("1428ba5b-02e9-4fe6-8d13-6e9c0b02d6fb") //57 assets -- capital one (CC)
            && x.status == 1).Select(x => x.site_id).ToList();

            var count = context.Assets.Include(x=>x.AssetPMPlans).Where(x => sites.Contains(x.site_id)
            && x.AssetPMPlans.Count == 0 && x.status == (int)Status.AssetActive)
                .Count();

            return context.Assets.Include(x=>x.AssetPMPlans).Where(x => sites.Contains(x.site_id)
            && x.AssetPMPlans.Count == 0  && x.status == (int)Status.AssetActive)
                .ToList();
        }

        public PMCategory Gepmdetailforceccoscript(Guid class_id)
        {
            return context.PMCategory.Where(x => x.inspectiontemplate_asset_class_id == class_id && x.status == 1)
                .Include(x=>x.PMPlans)
                .FirstOrDefault();
        }
       
        public WOOnboardingAssets GetPMWOlineById(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                                             .Include(x=>x.TempAsset)
                                                .ThenInclude(x=>x.WOOnboardingAssets).ThenInclude(x=>x.TempActiveAssetPMWOlineMapping)
                                                .Include(x => x.TempAsset)
                                                .ThenInclude(x => x.WOOnboardingAssets).ThenInclude(x => x.ActiveAssetPMWOlineMapping).ThenInclude(x=>x.AssetPMs)
                                             .FirstOrDefault();
        }

        public List<Guid> ReturnOnlyOpenAssetPMIds(List<Guid> asset_pms_ids)
        {
            return context.AssetPMs.Where(x=> asset_pms_ids.Contains(x.asset_pm_id) && x.status == (int)Status.Active).Select(x=>x.asset_pm_id).ToList();
        }

        public bool CheckIfAssetPMIsFromOtherSite(List<Guid> asset_pms_ids)
        {
            return context.AssetPMs.Include(x=>x.Asset).Where(x=> asset_pms_ids.Contains(x.asset_pm_id) && x.Asset.site_id != Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).Any();
        }

        public int GetAssetPMCountBySite()
        {
            return context.AssetPMs.Include(x=>x.Asset)
                .Where(x=>x.Asset.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.status != (int)Status.Completed && !x.is_archive ).Count();
        }
        public WOOnboardingAssets GetPMWOlineForDetails(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x=>x.woonboardingassets_id == woonboardingassets_id)
                .Include(x => x.ActiveAssetPMWOlineMapping).ThenInclude(x=>x.AssetPMs).ThenInclude(x=>x.AssetPMPlans)
                .Include(x => x.TempActiveAssetPMWOlineMapping).ThenInclude(x=>x.TempAssetPMs).ThenInclude(x=>x.PMs).ThenInclude(x=>x.PMPlans)
                 .FirstOrDefault();
        }

        public List<Asset> GetAllAssetsForAssetPMsDueDateScript()
        {
            return context.Assets.Include(x => x.AssetPMs).Where(x => x.status == (int)Status.AssetActive
                   && x.company_id != "f1c579ce-1571-47fd-8d4b-6e3e35df3eff" // other then Demo Company
                   && x.AssetPMs.Count > 0  && x.AssetPMs.Any(y => !y.is_archive && y.status != (int)Status.Completed) )
                .Include(x=>x.AssetPMs).ThenInclude(x=>x.AssetPMsTriggerConditionMapping).Take(1)
                .ToList();
        }

        public List<AssetPMs> GetAllAssetPMsForDueDateScript()
        {
            var count = context.AssetPMs.Where(x => !x.is_archive && x.datetime_starting_at != null
            && x.Asset.Sites.company_id == Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d")
            && x.Asset.status == 3
            && x.status != 15
            && (x.modified_at == null || x.modified_at.Value.Date < DateTime.UtcNow.AddDays(-1).Date)
            )
                .Include(x => x.Asset)
                .Include(x => x.AssetPMsTriggerConditionMapping)
                .Count();

            return context.AssetPMs.Where(x => !x.is_archive && x.datetime_starting_at != null
            && x.Asset.Sites.company_id == Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d")
            && x.Asset.status == 3
            && x.status != 15
            && (x.modified_at == null || x.modified_at.Value.Date < DateTime.UtcNow.AddDays(-1).Date)
            )
                .Include(x => x.Asset)
                .Include(x => x.AssetPMsTriggerConditionMapping).Take(1000)
                .ToList();
        }
        
        public List<Guid> ReturnAllOpenAssetPMsAssetIds(List<Guid> asset_pms_ids)
        {
            return context.AssetPMs.Where(x => asset_pms_ids.Contains(x.asset_pm_id) && x.status == (int)Status.Active).Select(x => x.asset_id).ToList();
        }
        public List<AssetPMs> GetAssetPMsByAssetId(Guid asset_id)
        {
            return context.AssetPMs.Where(x => x.asset_id == asset_id && x.is_assetpm_enabled && !x.is_archive)
                .Include(x=>x.AssetPMsTriggerConditionMapping)
                .Include(x=>x.AssetPMAttachments)
                .ToList();
        }

        public int GetAssetPMsCountByAssetIdPMId(Guid asset_id,Guid pm_id)
        {   // add filter to remove overdue, completed
            return context.AssetPMs.Where(x=>x.asset_id == asset_id && x.pm_id == pm_id && x.is_assetpm_enabled
            && x.status!=(int)Status.Completed && x.due_date.Value.Date > DateTime.UtcNow.Date 
            && !x.is_archive).Count();
        }
        public List<Guid> GetAssetPMsIdsByAssetIdPMId(Guid asset_id, Guid pm_id)
        {
            return context.AssetPMs.Where(x => x.asset_id == asset_id  && x.pm_id == pm_id && x.is_assetpm_enabled && !x.is_archive)
                .OrderByDescending(x=>x.datetime_starting_at).Select(x=>x.asset_pm_id).ToList();
        }
        public DateTime? GetLastAssetPMStartDateByAssetIdPMId(Guid asset_id, Guid pm_id)
        {
            return context.AssetPMs.Where(x => x.asset_id == asset_id && x.pm_id == pm_id && x.is_assetpm_enabled && !x.is_archive)
                .OrderByDescending(x => x.datetime_starting_at).Select(x => x.datetime_starting_at).FirstOrDefault();
        }
        public List<AssetPMs> GetAssetPMsByAssetIdPMId(Guid asset_id, Guid pm_id, Guid asset_pm_id)
        {
            return context.AssetPMs.Where(x => x.asset_id == asset_id && x.pm_id == pm_id 
            && x.asset_pm_id != asset_pm_id  && !x.is_archive && x.is_assetpm_enabled)
                .OrderBy(x=>x.datetime_starting_at).Include(x => x.AssetPMsTriggerConditionMapping).ToList();
        }
        public List<AssetPMs> GetAllAssetPMsToAdd1YrPMs()
        {
            var list = context.AssetPMs.Where(x => !x.is_archive && x.pm_id!=null && x.datetime_starting_at != null &&x.asset_id.ToString()=="")
                .Include(x => x.Asset)
                .Include(x => x.AssetPMsTriggerConditionMapping)
                .Include(x => x.AssetPMAttachments)
                .ToList();

            var list3 = list.GroupBy(x => x.pm_id).Select(x => x.OrderByDescending(x => x.datetime_starting_at).First()).Distinct().ToList();

            return list3;
        }
        public bool IsThisCurrentAssetPM(Guid asset_id,Guid pm_id,Guid asset_pm_id)
        {
            var curr_asset_pm_id = context.AssetPMs.Where(x => x.asset_id == asset_id && !x.is_archive 
                && x.pm_id != null && x.pm_id == pm_id 
                && x.status != (int)Status.Completed
                && x.pm_due_overdue_flag != (int)pm_due_overdue_flag.PM_Overdue)
               .OrderBy(x=>x.datetime_starting_at).Select(x=>x.asset_pm_id).FirstOrDefault();

            if (curr_asset_pm_id == asset_pm_id)
                return true;
            else
                return false;
        }

        public string GetLastCompletedAssetPMFormJson(Guid pm_id,Guid asset_id)
        {
            var assetpm = context.AssetPMs.Where(x => x.pm_id == pm_id && x.asset_id == asset_id && x.is_assetpm_enabled && !x.is_archive 
            && x.asset_pm_completed_date != null && x.status == (int)Status.Completed)
                .OrderByDescending(x=>x.asset_pm_completed_date).Include(x=>x.ActiveAssetPMWOlineMapping).FirstOrDefault();

            if (assetpm != null && assetpm.ActiveAssetPMWOlineMapping!=null&& assetpm.ActiveAssetPMWOlineMapping.Count>0)
            {
                return assetpm.ActiveAssetPMWOlineMapping.Where(x => x.is_active && !x.is_deleted)
                    .Select(x => x.pm_form_output_data).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
        public int GetAssetPMCountByYear(DateTime datetime_starting_at, Guid asset_id,Guid pm_id)
        {
            var count = context.AssetPMs.Where(x=> x.asset_id==asset_id && x.pm_id==pm_id 
            && x.datetime_starting_at.Value.Year == datetime_starting_at.Year
            && x.status != (int)Status.Completed && !x.is_archive 
            && x.pm_due_overdue_flag != (int)pm_due_overdue_flag.PM_Overdue && 
            (x.datetime_starting_at.Value.Date >= new DateTime(datetime_starting_at.Year,1,1) 
            && (x.datetime_starting_at.Value.Date < datetime_starting_at.Date))).Count();

            return count + 1;
        }
        public List<AssetPMs> GetCurrentAssetPMsByAssetId_2(Guid asset_id)
        {
            var list = context.AssetPMs.Where(x => x.asset_id == asset_id && x.datetime_starting_at != null && x.status != (int)Status.Completed && x.due_date > DateTime.UtcNow.AddDays(-1).Date && !x.is_archive).ToList()
                        .GroupBy(x => new { x.asset_id, x.pm_id })
                        .Select(g => g.OrderBy(x => x.datetime_starting_at).FirstOrDefault());
            var assetpms = context.AssetPMs.Where(x => list.Select(y => y.asset_pm_id).Contains(x.asset_pm_id))
                .Include(x => x.AssetPMsTriggerConditionMapping)
                .Include(x => x.Asset)
                .Include(x => x.StatusMaster).ToList();

            return assetpms;
        }
        public string GetAssetPMCountByDueDate(DateTime due_date, Guid asset_id, Guid pm_id)
        {
            var count = context.AssetPMs.Where(x => x.asset_id == asset_id && x.pm_id == pm_id
            && x.due_date.Value.Year == due_date.Year
            && x.status != (int)Status.Completed && !x.is_archive
            && x.pm_due_overdue_flag != (int)pm_due_overdue_flag.PM_Overdue &&
            (x.due_date.Value.Date >= new DateTime(due_date.Year, 1, 1)
            && (x.due_date.Value < due_date))).Count();

            count = count + 1;

            var str = context.PMs.Where(x => x.pm_id == pm_id).Select(x => x.title).FirstOrDefault();

            var title = str != null ? str + " - " + due_date.Year + " - " + count : null;

            return title;
        }
        public List<Guid> GetAssetsListPMScript()
        {
            //var assetids = context.Assets.Where(x => x.site_id == Guid.Parse("ede5667c-0f8c-458f-bb99-0dbb6992310c") && x.status == 3).Select(x => x.asset_id).ToList();
            //var list = context.AssetPMs.Where(x=> assetids.Contains(x.asset_id) && !x.is_assetpm_enabled && !x.is_archive && x.status!=15).Select(x=>x.asset_id).Distinct().ToList();

            return context.Assets.Where(x => x.site_id == Guid.Parse("ede5667c-0f8c-458f-bb99-0dbb6992310c") && x.status == 3).Select(x => x.asset_id).ToList();
        }
        public User GetRequestedUser(Guid user_id)
        {
            return context.User.Where(x => x.uuid == user_id).Include(x => x.Active_Company).FirstOrDefault();
        }
        public string GetRequestedSite(Guid site_id)
        {
            return context.Sites.Where(x => x.site_id == site_id).Select(x => x.site_name).FirstOrDefault();
        }
    }
}
