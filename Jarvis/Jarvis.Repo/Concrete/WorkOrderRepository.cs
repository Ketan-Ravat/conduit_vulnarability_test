using Amazon.Runtime.Internal.Util;
using DocumentFormat.OpenXml.Wordprocessing;
using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.Helper;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels.EmailRequestViewModel;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Core.Operations;
using Org.BouncyCastle.Asn1.Ocsp;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete
{
    public class WorkOrderRepository : BaseGenericRepository<WorkOrders>, IWorkOrderRepository
    {
        public WorkOrderRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }

        public virtual async Task<int> Insert(WorkOrders entity)
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

        public virtual async Task<int> Update(WorkOrders entity)
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

        public WorkOrders GetWorkOrderById(Guid wo_id)
        {
            return context.WorkOrders.Where(u => u.wo_id == wo_id && !u.is_archive)
                .Include(x => x.Sites)
                 .ThenInclude(x => x.ClientCompany)
                .Include(x => x.Asset).Include(x => x.Sites)
                .Include(x => x.WorkOrderTasks).ThenInclude(x => x.Tasks)
                .Include(x => x.WorkOrderAttachments)
                .Include(x => x.StatusMaster)
                .Include(x => x.PriorityStatusMaster)
                .Include(x => x.WOTypeStatusMaster)
                .Include(x => x.MaintenanceRequests).ThenInclude(x => x.Issue)
                .Include(x => x.ServiceDealers).ThenInclude(x => x.StatusMaster)
                .Include(x => x.WOcategorytoTaskMapping).ThenInclude(x => x._assigned_asset.AssetFormIOBuildingMappings)
                .Include(x => x.WOcategorytoTaskMapping).ThenInclude(x => x._assigned_asset.AssetFormIOBuildingMappings)
                .Include(x => x.WOcategorytoTaskMapping).ThenInclude(x => x._assigned_asset.AssetFormIOBuildingMappings)
                .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                 .ThenInclude(x => x.WOcategorytoTaskMapping)
                  .ThenInclude(x => x.AssetFormIO)
                  .Include(x => x.WOInspectionsTemplateFormIOAssignment).ThenInclude(x => x.Tasks)
                  .Include(x => x.WOInspectionsTemplateFormIOAssignment).ThenInclude(x => x.Parent_Asset).ThenInclude(x => x.Sites).ThenInclude(x => x.ClientCompany)
                  .Include(x => x.WOInspectionsTemplateFormIOAssignment).ThenInclude(x => x.WOcategorytoTaskMapping).ThenInclude(x => x.Asset)
                  .Include(x => x.WOInspectionsTemplateFormIOAssignment).ThenInclude(x => x.InspectionsTemplateFormIO)

                //.Include(x=>x.AssetFormIO)
                //.Include(x => x.WOInspectionsTemplateFormIOAssignment).ThenInclude(x => x.WOcategorytoTaskMapping).ThenInclude(x => x.AssetFormIO)
                .FirstOrDefault();
        }
        public WorkOrders GetWorkOrderByIdForComplete(Guid wo_id)
        {
            return context.WorkOrders.Where(u => u.wo_id == wo_id && !u.is_archive)
                .Include(x => x.Sites)
                //    .ThenInclude(x => x.ClientCompany)
                //     .Include(x => x.Asset).Include(x => x.Sites)
                //    .Include(x => x.WorkOrderTasks).ThenInclude(x => x.Tasks)
                //    .Include(x => x.WorkOrderAttachments)
                .Include(x => x.StatusMaster)
            //    .Include(x => x.PriorityStatusMaster)
                .Include(x => x.WOTypeStatusMaster)
                //   .Include(x => x.MaintenanceRequests).ThenInclude(x => x.Issue)
                //   .Include(x => x.ServiceDealers).ThenInclude(x => x.StatusMaster)
                //   .Include(x => x.WOcategorytoTaskMapping).ThenInclude(x => x._assigned_asset.AssetFormIOBuildingMappings)
                //    .Include(x => x.WOcategorytoTaskMapping).ThenInclude(x => x._assigned_asset.AssetFormIOBuildingMappings)
                //    .Include(x => x.WOcategorytoTaskMapping).ThenInclude(x => x._assigned_asset.AssetFormIOBuildingMappings)
                //    .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                //     .ThenInclude(x => x.WOcategorytoTaskMapping)
                //       .ThenInclude(x => x.AssetFormIO)
                //       .Include(x => x.WOInspectionsTemplateFormIOAssignment).ThenInclude(x => x.Tasks)
                //       .Include(x => x.WOInspectionsTemplateFormIOAssignment).ThenInclude(x => x.Parent_Asset).ThenInclude(x => x.Sites).ThenInclude(x => x.ClientCompany)
                //       .Include(x => x.WOInspectionsTemplateFormIOAssignment).ThenInclude(x => x.WOcategorytoTaskMapping).ThenInclude(x => x.Asset)
                //        .Include(x => x.WOInspectionsTemplateFormIOAssignment).ThenInclude(x => x.InspectionsTemplateFormIO)
                .FirstOrDefault();
        }

        public WorkOrders GetWorkOrderByIdForMoveIRImages(Guid wo_id)
        {
            return context.WorkOrders.Where(u => u.wo_id == wo_id && !u.is_archive)
               .Include(x => x.IRScanWOImageFileMapping)
               .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.IRWOImagesLabelMapping)
                .FirstOrDefault();
        }

        public WorkOrders GetWOByidforUpdate(Guid wo_id)
        {
            return context.WorkOrders.Where(u => u.wo_id == wo_id && !u.is_archive)
                .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                .Include(x => x.WOLineIssue)
                .Include(x => x.WorkOrderTechnicianMapping)
                .Include(x => x.Sites)
                 .ThenInclude(x => x.ClientCompany)
                //.Include(x=>x.WOcategorytoTaskMapping).ThenInclude(x=>x.AssetFormIO)
                .FirstOrDefault();
        }
        public WorkOrders GetWOByidforUpdateOffline(Guid wo_id)
        {
            return context.WorkOrders.Where(u => u.wo_id == wo_id && !u.is_archive)
                .FirstOrDefault();
        }
        public (int, int) GetAssetformioThreadcount(Guid wo_id)
        {
            return (context.AssetFormIO.Where(u => u.wo_id == wo_id && (u.status == (int)Status.Completed || u.status == (int)Status.Submitted)) // total_woline
                          .Count(),
                    context.AssetFormIO.Where(u => u.wo_id == wo_id && (u.status == (int)Status.Completed || u.status == (int)Status.Submitted) && u.is_main_asset_created) // total_main_asset_created
                          .Count());
        }
        public (int, int) GetOBWOLineThreadcount(Guid wo_id)
        {
            return (context.WOOnboardingAssets.Where(u => u.wo_id == wo_id && (u.status == (int)Status.Completed || u.status == (int)Status.Submitted) && !u.is_deleted) // total_woline
                          .Count(),
                    context.WOOnboardingAssets.Where(u => u.wo_id == wo_id && (u.status == (int)Status.Completed || u.status == (int)Status.Submitted) && !u.is_deleted && u.is_main_asset_created) // total_main_asset_created
                          .Count());
        }
        public WorkOrderTasks GetWOTaskById(Guid wo_task_id)
        {
            return context.WorkOrderTasks.Where(x => x.wo_task_id == wo_task_id).FirstOrDefault();
        }
        public async Task<ListViewModel<WorkOrders>> GetAllWorkOrders(GetAllWorkOrderRequestModel requestModel)
        {
            ListViewModel<WorkOrders> WOItems = new ListViewModel<WorkOrders>();
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

                IQueryable<WorkOrders> query = context.WorkOrders.Where(x => usersites.Contains(x.site_id) && !x.is_archive);

                if (requestModel.status > 0)
                {
                    query = query.Where(x => x.status == requestModel.status);
                }


                // Work Order priority
                if (requestModel.priority > 0)
                {
                    query = query.Where(x => x.priority == requestModel.priority);
                }

                // Work Order Title
                if (requestModel.wo_id != null && requestModel.wo_id != "")
                {
                    query = query.Where(x => x.wo_id.ToString() == requestModel.wo_id);
                }

                // Work Order Number
                if (requestModel.wo_number > 0)
                {
                    query = query.Where(x => x.wo_number == requestModel.wo_number);
                }

                // work order type -- k
                if (requestModel.wo_type != null && requestModel.wo_type > 0)
                {
                    query = query.Where(x => x.wo_type == requestModel.wo_type.Value);
                }

                if (!string.IsNullOrEmpty(requestModel.search_string))
                {
                    var searchstring = requestModel.search_string.ToLower().ToString();
                    query = query.Where(x => (x.title.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring)
                             || x.priority.ToString().ToLower() == searchstring || x.PriorityStatusMaster.status_name.ToLower().Contains(searchstring)
                             || x.wo_number.ToString().Contains(searchstring.ToLower())
                             || x.wo_type.ToString().ToLower() == searchstring || x.WOTypeStatusMaster.status_name.ToLower().Contains(searchstring)));
                }

                WOItems.listsize = query.Count();

                WOItems.list = query.Skip((requestModel.pageIndex - 1) * requestModel.pageSize).Take(requestModel.pageSize).ToList();

                //var QueryToList = query.Include(x => x.Sites).Include(x => x.Asset).Include(x => x.StatusMaster).Include(x => x.PriorityStatusMaster).Include(x => x.WOTypeStatusMaster)
                //  .Include(x => x.WorkOrderTasks).ThenInclude(x => x.Tasks)
                //  .Include(x => x.WorkOrderAttachments)
                //  .Include(x => x.MaintenanceRequests).ThenInclude(x => x.Issue).ToList();
                ////.OrderByDescending(x => x.created_at).ToListAsync().Result;
                ///

                WOItems.list = query.Include(x => x.Sites).Include(x => x.Asset).Include(x => x.StatusMaster).Include(x => x.PriorityStatusMaster).Include(x => x.WOTypeStatusMaster)
                  .Include(x => x.WorkOrderTasks).ThenInclude(x => x.Tasks)
                  .Include(x => x.WorkOrderAttachments)
                  .Include(x => x.MaintenanceRequests).ThenInclude(x => x.Issue)
                  .Include(x => x.ServiceDealers).ThenInclude(x => x.StatusMaster)
                  .OrderByDescending(x => x.created_at).ToListAsync().Result;

                //var groupByQuery = QueryToList.GroupBy(x => x.status)
                //    .Select(group =>
                //    (group.Key == (int)Status.WOOpen || group.Key == (int)Status.WOInProgress || group.Key == (int)Status.WOReOpen
                //     || group.Key == (int)Status.WOCompleted || group.Key == (int)Status.WOCancelled) ? group.OrderByDescending(x => x.created_at) : group.OrderBy(x => x.created_at)
                //    ).OrderBy(group => group.First().status);

                //var groupByQuery = QueryToList.GroupBy(x => x.status)
                //    .Select(group =>
                //    (group.Key == (int)Status.WOOpen ||
                //        group.Key == (int)Status.WOInProgress || group.Key == (int)Status.WOReOpen
                //        || group.Key == (int)Status.WOCompleted || group.Key == (int)Status.WOCancelled ? group.OrderByDescending(x => x.created_at) : group.OrderBy(x => x.created_at))
                //    ).OrderBy(group => group.First().status);

                //var groupByQuery = QueryToList.GroupBy(x => x.status).Select(group => group.Key == (int)Status.WOOpen ? group.OrderByDescending(x => x.created_at) : group.OrderBy(x => x.created_at))
                //    .OrderByDescending(group => group.First().status);

                //foreach (var item in groupByQuery)
                //{
                //    WOItems.list.AddRange(item);
                //}

            }

            return WOItems;
        }

        public List<Issue> GetNewIssuesListByAssetId(string asset_id, string mr_id, string searchstring)
        {
            IQueryable<Issue> query = context.Issue;

            if (mr_id != null && !String.IsNullOrEmpty(mr_id))
            {
                query = query.Where(x => x.asset_id.ToString() == asset_id && x.mr_id != null && x.mr_id.ToString() != mr_id && x.status == (int)Status.New).Include(x => x.Inspection);
            }
            else
            {
                query = query.Where(x => x.asset_id.ToString() == asset_id && x.mr_id != null && x.status == (int)Status.New).Include(x => x.Inspection);
            }

            if (!String.IsNullOrEmpty(searchstring))
            {
                searchstring = searchstring.ToLower();
                query = query.Where(x => (x.name.ToLower().Contains(searchstring)
                || x.issue_number.ToString().Contains(searchstring)));
            }

            return query.OrderByDescending(x => x.created_at).ToList();
        }

        public List<AssetActivityLogs> WorkOrderStatusHistory(Guid wo_id)
        {
            var workOrderLogList = context.AssetActivityLogs.Include(x => x.Asset).ThenInclude(x => x.Sites)
                .Where(x => x.ref_id == wo_id.ToString() &&
                ((x.activity_type == (int)ActivityTypes.WorkOrderCreated || x.activity_type == (int)ActivityTypes.WorkOrderUpdated
                || x.activity_type == (int)ActivityTypes.WorkOrderDeleted || x.activity_type == (int)ActivityTypes.WorkOrderIssueLink
                || x.activity_type == (int)ActivityTypes.WorkOrderIssueUnlink))).ToList();

            foreach (var data in workOrderLogList)
            {
                if (data.activity_type == (int)ActivityTypes.WorkOrderIssueLink || data.activity_type == (int)ActivityTypes.WorkOrderIssueUnlink)
                {
                    data.activity_header = data.activity_message;
                }
            }

            var mrIdList = context.MaintenanceRequests.Where(x => x.wo_id.ToString() == wo_id.ToString()).Select(x => x.mr_id.ToString()).ToList();
            if (mrIdList != null && mrIdList.Count > 0)
            {
                var issueIdList = context.Issue.Where(x => mrIdList.Contains(x.mr_id.ToString())).Select(x => x.issue_uuid.ToString()).ToList();
                var issueList = context.Issue.Where(x => mrIdList.Contains(x.mr_id.ToString())).ToList();
                var issueLogList = context.AssetActivityLogs.Include(x => x.Asset).ThenInclude(x => x.Sites)
                    .Where(x => issueIdList.Contains(x.ref_id) &&
                    ((x.activity_type == (int)ActivityTypes.IssueResolved || x.activity_type == (int)ActivityTypes.NewIssueCreated))).ToList();

                foreach (var item in issueLogList)
                {
                    foreach (var data in issueList)
                    {
                        if (item.ref_id == data.issue_uuid.ToString())
                        {
                            item.activity_header = "#" + data.issue_number + " " + data.name;
                            if (item.activity_type == (int)ActivityTypes.NewIssueCreated)
                            {
                                item.activity_header = item.activity_header;
                                item.updated_by = data.created_by;
                            }
                        }
                    }
                }

                workOrderLogList = workOrderLogList.Concat(issueLogList).ToList();
            }
            return workOrderLogList.OrderByDescending(x => x.created_at).ToList();
        }

        public (List<WorkOrders>, int) FilterWorkOrderTitleOption(FilterWorkOrderOptionsRequestModel requestModel)
        {
            List<WorkOrders> workorders = new List<WorkOrders>();
            int total_count = 0;
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
                {
                    rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(rolename))
                {
                    IQueryable<WorkOrders> query = context.WorkOrders;

                    if (rolename != GlobalConstants.Admin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Where(x => usersites.Contains(x.site_id));
                        }
                        else
                        {
                            query = query.Where(x => usersites.Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                        }
                    }

                    //   workorders = context.WorkOrders.Include(x => x.StatusMaster).ToList();

                    // status check
                    if (requestModel.status > 0)
                    {
                        query = query.Where(x => x.status == requestModel.status);
                    }

                    // Work Order priority
                    if (requestModel.priority > 0)
                    {
                        query = query.Where(x => x.priority == requestModel.priority);
                    }

                    // Work Order Title
                    if (requestModel.wo_id != null && requestModel.wo_id != "")
                    {
                        query = query.Where(x => x.wo_id.ToString() == requestModel.wo_id);
                    }

                    // Work Order Number
                    if (requestModel.wo_number > 0)
                    {
                        query = query.Where(x => x.wo_number == requestModel.wo_number);
                    }

                    // Work Order type
                    if (requestModel.wo_type != null && requestModel.wo_type.Value > 0)
                    {
                        query = query.Where(x => x.wo_type == requestModel.wo_type.Value);
                    }

                    // Search Work Orders
                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        var searchstring = requestModel.search_string.ToLower();
                        query = query.Where(x => (x.title.ToLower().Contains(searchstring.ToLower()) || x.status.ToString().Contains(searchstring.ToLower()) || x.StatusMaster.status_name.ToLower().Contains(searchstring.ToLower())
                                          || x.wo_number.ToString().Contains(searchstring.ToLower()) || x.wo_type.ToString().Contains(searchstring.ToLower()) || x.WOTypeStatusMaster.status_name.ToString() == searchstring
                                          || x.priority.ToString() == searchstring || x.PriorityStatusMaster.status_name.ToLower().Contains(searchstring.ToLower())));

                    }

                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        var searchstring = requestModel.option_search_string.ToLower();
                        query = query.Where(x => x.title.ToLower().Contains(searchstring.ToLower()));
                    }
                    total_count = query.Count();
                    if (requestModel.pageSize > 0 && requestModel.pageIndex > 0)
                    {
                        query = query.Skip((requestModel.pageIndex - 1) * requestModel.pageSize).Take(requestModel.pageSize);
                    }
                    workorders = query.ToList();
                }
            }
            return (workorders, total_count);
        }

        public (List<WorkOrders>, int) FilterWorkOrderNumberOption(FilterWorkOrderOptionsRequestModel requestModel)
        {
            List<WorkOrders> workorders = new List<WorkOrders>();
            int total_count = 0;
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
                {
                    rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
                }

                //List<Guid> assetlist = new List<Guid>();

                if (!string.IsNullOrEmpty(rolename))
                {
                    IQueryable<WorkOrders> query = context.WorkOrders;
                    if (rolename != GlobalConstants.Admin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Where(x => usersites.Contains(x.site_id));
                        }
                        else
                        {
                            query = query.Where(x => usersites.Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                        }
                    }

                    //workorders = context.WorkOrders.Include(x => x.StatusMaster).ToList();

                    // status check
                    if (requestModel.status > 0)
                    {
                        query = query.Where(x => x.status == requestModel.status);
                    }

                    // Work Order priority
                    if (requestModel.priority > 0)
                    {
                        query = query.Where(x => x.priority == requestModel.priority);
                    }

                    // Work Order Title
                    if (requestModel.wo_id != null && requestModel.wo_id != "")
                    {
                        query = query.Where(x => x.wo_id.ToString() == requestModel.wo_id);
                    }

                    // Work Order Number
                    if (requestModel.wo_number > 0)
                    {
                        query = query.Where(x => x.wo_number == requestModel.wo_number);
                    }

                    // Work Order type
                    if (requestModel.wo_type != null && requestModel.wo_type.Value > 0)
                    {
                        query = query.Where(x => x.wo_type == requestModel.wo_type.Value);
                    }

                    // Search Work Orders
                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        var searchstring = requestModel.search_string.ToLower();
                        query = query.Where(x => (x.title.ToLower().Contains(searchstring.ToLower()) || x.status.ToString().Contains(searchstring.ToLower()) || x.StatusMaster.status_name.ToLower().Contains(searchstring.ToLower())
                                          || x.wo_number.ToString().Contains(searchstring.ToLower()) || x.wo_type.ToString().Contains(searchstring.ToLower()) || x.WOTypeStatusMaster.status_name.ToString() == searchstring
                                          || x.priority.ToString() == searchstring || x.PriorityStatusMaster.status_name.ToLower().Contains(searchstring.ToLower())));
                    }

                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        var searchstring = requestModel.option_search_string.ToLower();
                        query = query.Where(x => x.wo_number.ToString().Contains(searchstring.ToLower()));
                    }
                    total_count = query.Count();
                    if (requestModel.pageSize > 0 && requestModel.pageIndex > 0)
                    {
                        query = query.Skip((requestModel.pageIndex - 1) * requestModel.pageSize).Take(requestModel.pageSize);
                    }
                    workorders = query.ToList();
                }
            }
            return (workorders, total_count);
        }

        public (List<WorkOrders>, int total_list_count) GetAllWorkOrdersNewflow(string userid, NewFlowWorkorderListRequestModel requestModel)
        {
            List<WorkOrders> response = new List<WorkOrders>();
            int total_list_count = 0;
            if (userid != null)
            {
                IQueryable<WorkOrders> query = context.WorkOrders.Where(x => !x.is_archive);
                List<Guid> usersites = new List<Guid>();
                if (String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
                {
                    usersites = context.User.Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).Select(x => x.ac_default_site.Value).ToList();
                }
                else if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }

                if (requestModel.site_id != null && requestModel.site_id.Count > 0)
                    query = query.Where(x => requestModel.site_id.Contains(x.site_id.ToString()));
                else// as per new requirement return no data if site_id filter is null
                    query = query.Where(x => usersites.Contains(x.site_id));

                //query = query.Where(x => usersites.Contains(x.site_id));

                query = query.Where(x => x.wo_type == (int)Status.Acceptance_Test_WO || x.wo_type == (int)Status.Maintenance_WO || x.wo_type == (int)Status.Onboarding_WO || x.wo_type == (int)Status.Maintenance_WO || x.wo_type == (int)Status.IR_Scan_WO);
                /////// if request is for technician then give based on technician user
                ///
                if (requestModel.technician_user_id != null && requestModel.technician_user_id.Value != Guid.Empty)
                {
                    //   var wos = context.WOcategorytoTaskMapping.Where(x => x.technician_user_id == requestModel.technician_user_id && !x.is_archived).Select(x=>x.wo_id).Distinct().ToList();
                    //query = query.Where(x => x.status != (int)Status.PlannedWO);

                    var all_wo = context.WorkOrderTechnicianMapping.Where(x => x.user_id == requestModel.technician_user_id && !x.is_deleted && x.WorkOrders.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).Select(x => x.wo_id).Distinct().ToList();

                    if (all_wo.Count > 0)
                    {
                        query = query.Include(x => x.WorkOrderTechnicianMapping).Where(x => (all_wo.Contains(x.wo_id) || x.WorkOrderTechnicianMapping.Where(x => !x.is_deleted).Count() == 0) && x.status != (int)Status.PlannedWO);
                    }
                    else
                    {
                        query = query.Include(x => x.WorkOrderTechnicianMapping).Where(x => x.WorkOrderTechnicianMapping.Where(x => !x.is_deleted).Count() == 0);
                    }
                }
                if (requestModel.from_date != null && requestModel.to_date != null)
                {
                    query = query.Where(s => s.start_date.Date >= requestModel.from_date.Value.Date && s.start_date.Date <= requestModel.to_date.Value.Date);
                }
                if (requestModel.wo_status != null && requestModel.wo_status.Count > 0)
                {
                    query = query.Where(x => requestModel.wo_status.Contains(x.status));
                }
                if (requestModel.quote_status != null && requestModel.quote_status.Count > 0)
                {
                    query = query.Where(x => requestModel.quote_status.Contains(x.quote_status.Value));
                }
                if (requestModel.site_id != null && requestModel.site_id.Count > 0)
                {
                    query = query.Where(x => requestModel.site_id.Contains(x.site_id.ToString()));
                }



                if (!String.IsNullOrEmpty(requestModel.search_string))
                {
                    string search = requestModel.search_string.Trim().ToLower();
                    query = query.Where(x =>
                    x.description.Contains(search) ||
                    x.wo_number.ToString().Contains(search) ||
                    x.manual_wo_number.ToLower().Trim().Contains(search)
                    );
                }
                if (requestModel.wo_type != null && requestModel.wo_type.Count > 0)
                {
                    query = query.Where(x => requestModel.wo_type.Contains(x.wo_type));
                }
                query = query.OrderByDescending(x => x.start_date).ThenByDescending(x => x.created_at);
                total_list_count = query.Count();
                // query = query.Skip((1 - 1) * 20).Take(20);//.OrderBy(x => x.created_at);
                if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
                {
                    query = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);//.OrderBy(x => x.created_at);
                }

                response = query
                    .Include(x => x.StatusMaster)
                    .Include(x => x.WOTypeStatusMaster)
                    .Include(x => x.Sites)
                    .ThenInclude(x => x.ClientCompany)
                    .Include(x => x.TechnicianUser)
                    .Include(x => x.WorkOrderAttachments)
                    .ToList();
            }
            return (response, total_list_count);
        }

        public (List<WOlistExcludeProperties>, int total_list_count) GetAllWorkOrdersNewflowOptimized(string userid, NewFlowWorkorderListRequestModel requestModel)
        {
            List<WOlistExcludeProperties> response = new List<WOlistExcludeProperties>();
            int total_list_count = 0;

            IQueryable<WorkOrders> query = context.WorkOrders.Where(x => !x.is_archive);

            if (requestModel.site_id != null && requestModel.site_id.Count > 0)
                query = query.Where(x => requestModel.site_id.Contains(x.site_id.ToString()));
            else// as per new requirement return no data if site_id filter is null
                query = query.Where(x => x.site_id == Guid.Empty);//Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));

            query = query.Where(x => x.wo_type == (int)Status.Acceptance_Test_WO || x.wo_type == (int)Status.Maintenance_WO || x.wo_type == (int)Status.Onboarding_WO || x.wo_type == (int)Status.Maintenance_WO || x.wo_type == (int)Status.IR_Scan_WO);
            /////// if request is for technician then give based on technician user
            ///
            if (requestModel.technician_user_id != null && requestModel.technician_user_id.Value != Guid.Empty)
            {
                //   var wos = context.WOcategorytoTaskMapping.Where(x => x.technician_user_id == requestModel.technician_user_id && !x.is_archived).Select(x=>x.wo_id).Distinct().ToList();
                //query = query.Where(x => x.status != (int)Status.PlannedWO);

                var all_wo = context.WorkOrderTechnicianMapping.Where(x => x.user_id == requestModel.technician_user_id && !x.is_deleted && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).Select(x => x.wo_id).Distinct().ToList();

                if (all_wo.Count > 0)
                {
                    query = query.Include(x => x.WorkOrderTechnicianMapping).Where(x => (all_wo.Contains(x.wo_id) || x.WorkOrderTechnicianMapping.Where(x => !x.is_deleted).Count() == 0) && x.status != (int)Status.PlannedWO);
                }
                else
                {
                    query = query.Include(x => x.WorkOrderTechnicianMapping).Where(x => x.WorkOrderTechnicianMapping.Where(x => !x.is_deleted).Count() == 0);
                }
            }
            if (requestModel.from_date != null && requestModel.to_date != null)
            {
                query = query.Where(s => s.start_date.Date >= requestModel.from_date.Value.Date && s.start_date.Date <= requestModel.to_date.Value.Date);
            }
            if (requestModel.wo_status != null && requestModel.wo_status.Count > 0)
            {
                query = query.Where(x => requestModel.wo_status.Contains(x.status));
            }
            if (requestModel.quote_status != null && requestModel.quote_status.Count > 0)
            {
                query = query.Where(x => requestModel.quote_status.Contains(x.quote_status.Value));
            }
            if (requestModel.site_id != null && requestModel.site_id.Count > 0)
            {
                query = query.Where(x => requestModel.site_id.Contains(x.site_id.ToString()));
            }

            if (requestModel.is_requested_from_workorders_tab)
            { // if wo tab then don't show QuoteWO status data 
                query = query.Where(x => x.status != (int)Status.QuoteWO);
            }
            else // if quotes tab then don't show null quote_status data
            {
                query = query.Where(x => x.quote_status != null);
            }


            if (!String.IsNullOrEmpty(requestModel.search_string))
            {
                string search = requestModel.search_string.Trim().ToLower();
                query = query.Where(x =>
                x.description.Contains(search) ||
                x.wo_number.ToString().Contains(search) ||
                x.manual_wo_number.ToLower().Trim().Contains(search)
                );
            }
            if (requestModel.wo_type != null && requestModel.wo_type.Count > 0)
            {
                query = query.Where(x => requestModel.wo_type.Contains(x.wo_type));
            }
            query = query.OrderByDescending(x => x.start_date).ThenByDescending(x => x.created_at);
            total_list_count = query.Count();
            // query = query.Skip((1 - 1) * 20).Take(20);//.OrderBy(x => x.created_at);
            if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
            {
                query = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);//.OrderBy(x => x.created_at);
            }
            response = query
              .Select(x => new WOlistExcludeProperties
              {
                  wo_type = x.wo_type,
                  manual_wo_number = x.manual_wo_number,
                  wo_type_name = x.WOTypeStatusMaster.status_name,
                  site_name = x.Sites.site_name,
                  client_company_name = x.Sites.ClientCompany.client_company_name,
                  start_date = x.start_date,
                  wo_status_id = x.status,
                  due_date = x.due_at,
                  wo_id = x.wo_id,
                  due_in = x.wo_due_time_duration,
                  wo_due_overdue_flag = x.wo_due_overdue_flag,
                  quote_status_id = x.quote_status,
                  quote_status_name = x.QuoteStatusMaster.status_name,
                  site_id = x.site_id
              })
              .ToList();

            return (response, total_list_count);
        }

        public (List<InspectionsTemplateFormIO>, int total_list_count) GetAllCatagoryForWO(string userid, GetAllCatagoryForWORequestModel requestModel)
        {
            List<InspectionsTemplateFormIO> response = new List<InspectionsTemplateFormIO>();
            int total_list_count = 0;
            if (userid != null)
            {
                IQueryable<InspectionsTemplateFormIO> query = context.InspectionsTemplateFormIO;
                /* List<Guid> usersites = new List<Guid>();
                 if (String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
                 {
                     usersites = context.User.Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).Select(x => x.ac_default_site.Value).ToList();
                 }
                 else if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                 {
                     usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                 }
                 else
                 {
                     usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                 }
                */
                //query = query.Where(x => x.Tasks != null);
                //query = query.Where(x => !x.WOInspectionsTemplateFormIOAssignment.Any(q => q.wo_id == requestModel.wo_id && !q.is_archived));
                query = query.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id);
                total_list_count = query.Count();
                if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
                {
                    query = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
                }

                response = query
                                .Include(x => x.FormIOType)
                                .ToList();
            }
            return (response, total_list_count);
        }
        public (List<Asset>, int total_list_count) GetAssetsToAssignOBWO(GetAssetsToAssignOBWORequestmodel requestmodel)
        {
            List<Asset> response = new List<Asset>();
            int total_list_count = 0;


            IQueryable<Asset> query = context.Assets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.status == (int)Status.AssetActive);

            if (requestmodel.wo_id != null)
            {
                var assigned_assets = context.WOOnboardingAssets.Where(x => x.wo_id == requestmodel.wo_id && !x.is_deleted && x.asset_id != null).Select(x => x.asset_id.Value).ToList();
                query = query.Where(x => !assigned_assets.Contains(x.asset_id));
            }

            query = query.Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                            .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                            .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                            .Include(x => x.AssetFormIOBuildingMappings.FormIOSections)
                            .Include(x => x.InspectionTemplateAssetClass);

            if (requestmodel.formiobuilding_id != null && requestmodel.formiobuilding_id > 0) // deprecated
            {
                query = query.Where(x => x.AssetFormIOBuildingMappings.formiobuilding_id == requestmodel.formiobuilding_id);
            }
            if (requestmodel.formiofloor_id != null && requestmodel.formiofloor_id > 0) // deprecated
            {
                query = query.Where(x => x.AssetFormIOBuildingMappings.formiofloor_id == requestmodel.formiofloor_id);
            }
            if (requestmodel.formioroom_id != null && requestmodel.formioroom_id > 0) // deprecated
            {
                query = query.Where(x => x.AssetFormIOBuildingMappings.formioroom_id == requestmodel.formioroom_id);
            }
            FormIOBuildings get_main_building = null;
            if (!String.IsNullOrEmpty(requestmodel.temp_formio_building_name))
            {
                get_main_building = context.FormIOBuildings.Where(x => x.formio_building_name.ToLower().Trim() == requestmodel.temp_formio_building_name.ToLower().Trim()
                                                                        && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
                if (get_main_building != null)
                {
                    query = query.Where(x => x.AssetFormIOBuildingMappings.formiobuilding_id == get_main_building.formiobuilding_id);
                }
                else
                {
                    query = query.Where(x => x.asset_id == null);
                }
            }
            FormIOFloors get_main_floor = null;
            if (!String.IsNullOrEmpty(requestmodel.temp_formio_floor_name)) // deprecated
            {
                if (get_main_building != null)
                {
                    get_main_floor = context.FormIOFloors.Where(x => x.formio_floor_name.ToLower().Trim() == requestmodel.temp_formio_floor_name.ToLower().Trim()
                                                                       && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                                                                       && x.formiobuilding_id == get_main_building.formiobuilding_id
                                                                       ).FirstOrDefault();
                    if (get_main_floor != null)
                    {
                        query = query.Where(x => x.AssetFormIOBuildingMappings.formiofloor_id == get_main_floor.formiofloor_id);
                    }
                    else
                    {
                        query = query.Where(x => x.asset_id == null);
                    }
                }
            }
            FormIORooms get_main_room = null;
            if (!String.IsNullOrEmpty(requestmodel.temp_formio_room_name))
            {
                if (get_main_floor != null)
                {
                    get_main_room = context.FormIORooms.Where(x => x.formio_room_name.ToLower().Trim() == requestmodel.temp_formio_room_name.ToLower().Trim()
                                                                       && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                                                                       && x.formiofloor_id == get_main_floor.formiofloor_id
                                                                       ).FirstOrDefault();
                    if (get_main_room != null)
                    {
                        query = query.Where(x => x.AssetFormIOBuildingMappings.formioroom_id == get_main_room.formioroom_id);
                    }
                    else
                    {
                        query = query.Where(x => x.asset_id == null);
                    }
                }

            }
            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.Trim().ToLower();
                query = query.Where(x => x.name.Trim().ToLower().Contains(search)
                || x.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name.Trim().ToLower().Contains(search)
                || x.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name.Trim().ToLower().Contains(search)
                || x.AssetFormIOBuildingMappings.FormIORooms.formio_room_name.Trim().ToLower().Contains(search)
                || x.InspectionTemplateAssetClass.asset_class_code.Trim().ToLower().Contains(search)
                || x.InspectionTemplateAssetClass.asset_class_name.Trim().ToLower().Contains(search)
                );
            }
            query = query.OrderBy(x => x.name);
            total_list_count = query.Count();
            if (requestmodel.pagesize > 0 && requestmodel.pageindex > 0)
            {
                query = query.Skip((requestmodel.pageindex - 1) * requestmodel.pagesize).Take(requestmodel.pagesize);
            }

            response = query
                            .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                            .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                            .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                            .Include(x => x.AssetFormIOBuildingMappings.FormIOSections)
                            .Include(x => x.InspectionTemplateAssetClass)
                            .ToList();

            return (response, total_list_count);
        }
        public List<Asset> GetComponantLevelMainAssets(GetComponantLevelAssetsRequestmodel requestmodel)
        {
            List<Asset> response = new List<Asset>();
            IQueryable<Asset> query = context.Assets;

            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.status == (int)Status.AssetActive
            );


            if (requestmodel.component_level_type_id != null && requestmodel.component_level_type_id > 0)
            {
                query = query.Where(x => x.component_level_type_id == requestmodel.component_level_type_id);
            }

            query = query.OrderBy(x => x.name);

            response = query.Include(x => x.InspectionTemplateAssetClass)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                            .ToList();

            return (response);
        }
        public List<WOOnboardingAssets> GetComponantLevelTempAssets(GetComponantLevelAssetsRequestmodel requestmodel)
        {
            List<WOOnboardingAssets> response = new List<WOOnboardingAssets>();
            IQueryable<WOOnboardingAssets> query = context.WOOnboardingAssets;

            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_deleted
            );


            if (requestmodel.component_level_type_id != null && requestmodel.component_level_type_id > 0)
            {
                query = query.Where(x => x.component_level_type_id == requestmodel.component_level_type_id);
            }
            if (requestmodel.wo_id != null)
            {
                query = query.Where(x => x.wo_id == requestmodel.wo_id);
            }

            query = query.Include(x => x.Asset)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms)
                .Include(x => x.TempAsset.TempMasterBuilding)
                .Include(x => x.TempAsset.TempMasterFloor)
                .Include(x => x.TempAsset.TempMasterRoom)
                ;

            query = query.OrderBy(x => x.asset_name);


            response = query.ToList();

            return (response);
        }

        public List<WOInspectionsTemplateFormIOAssignment> GetAllCatagoryOFWO(Guid wo_id)
        {
            return context.WOInspectionsTemplateFormIOAssignment.Where(x => x.wo_id == wo_id && !x.is_archived).ToList();
        }
        public WorkOrders ViewWorkOrderDetailsById(Guid wo_id)
        {
            return context.WorkOrders.Where(x => x.wo_id == wo_id && !x.is_archive)
                // .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                //   .ThenInclude(x => x.InspectionsTemplateFormIO)
                //   .ThenInclude(x => x.FormIOType)
                .Include(x => x.StatusMaster)
                .Include(x => x.TechnicianUser)
                .Include(x => x.WOTypeStatusMaster)
                .Include(x => x.Sites).ThenInclude(x => x.Company)
                .Include(x => x.ClientCompany)
                .Include(x => x.WorkOrderAttachments)
                .Include(x => x.ResponsibleParty)
                .Include(x => x.WorkOrderTechnicianMapping).ThenInclude(x => x.TechnicianUser)
                .Include(x => x.WorkOrderBackOfficeUserMapping).ThenInclude(x => x.BackOfficeUser)
                .FirstOrDefault();
        }
        public WorkOrders WorkOrderDetailsByIdForExportPDF(Guid wo_id)
        {
            return context.WorkOrders.Where(x => x.wo_id == wo_id && !x.is_archive)
                // .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                //   .ThenInclude(x => x.InspectionsTemplateFormIO)
                //   .ThenInclude(x => x.FormIOType)
                .Include(x => x.StatusMaster)
                .Include(x => x.TechnicianUser)
                .Include(x => x.WOTypeStatusMaster)
                .Include(x => x.Sites)
                .Include(x => x.ClientCompany)
                .Include(x => x.WorkOrderAttachments)
                .FirstOrDefault();
        }
        public WorkOrders ViewOBWODetailsById(Guid wo_id)
        {
            return context.WorkOrders.Where(x => x.wo_id == wo_id && !x.is_archive)
                .Include(x => x.StatusMaster)
                .Include(x => x.WOTypeStatusMaster)
                .Include(x => x.ClientCompany)
                .Include(x => x.Sites)
                .Include(x => x.Asset)
                .Include(x => x.ResponsibleParty)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.Asset)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.TempAsset).ThenInclude(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.FormIOType)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.Sites)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.StatusMaster)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOLineIssue)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.TempAsset).ThenInclude(x => x.TempFormIOBuildings)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.TempAsset).ThenInclude(x => x.TempFormIOFloors)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.TempAsset).ThenInclude(x => x.TempFormIORooms)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.TempAsset).ThenInclude(x => x.TempFormIOSections)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.TempAsset.TempMasterBuilding)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.TempAsset.TempMasterFloor)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.TempAsset.TempMasterRoom)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.StatusMaster).Include(x => x.Asset.AssetTopLevelcomponentMapping)
                .Include(x => x.WorkOrderTechnicianMapping).ThenInclude(x => x.TechnicianUser)
                .Include(x => x.WorkOrderBackOfficeUserMapping).ThenInclude(x => x.BackOfficeUser)
                .Include(x => x.WorkOrderAttachments)
                .FirstOrDefault();
        }

        public List<WOInspectionsTemplateFormIOAssignment> GetWOFormiomapping(Guid wo_id)
        {
            return context.WOInspectionsTemplateFormIOAssignment.Where(x => x.wo_id == wo_id && !x.is_archived)
                .Include(x => x.Tasks)
                .Include(x => x.StatusMaster)
                .Include(x => x.Parent_Asset)
                .Include(x => x.User)
                .Include(x => x.InspectionsTemplateFormIO)
                 .ThenInclude(x => x.FormIOType)
                .Include(x => x.WOcategorytoTaskMapping)
                   .ThenInclude(x => x.Tasks)
                     .ThenInclude(x => x.AssetTasks)
                     .Include(x => x.WOcategorytoTaskMapping)
                     .ThenInclude(x => x.AssetFormIO)
                // .ThenInclude(x => x.Asset)
                /* .Include(x=>x.InspectionsTemplateFormIO)
                     .ThenInclude(x=>x.Tasks)
                       .ThenInclude(x=>x.AssetTasks)
                         .ThenInclude(x=>x.Asset)*/
                .ToList();
        }
        public List<WOInspectionsTemplateFormIOAssignment> GetWOFormiomappingForBulkImport(Guid wo_id)
        {
            return context.WOInspectionsTemplateFormIOAssignment.Where(x => x.wo_id == wo_id && !x.is_archived)
                .ToList();
        }
        public List<WOInspectionsTemplateFormIOAssignment> GetWOcategorymapping(Guid wo_id) //ALERT!!!! do not include AssetFormIO table in this method
        {
            var wo_category = context.WOInspectionsTemplateFormIOAssignment.Where(x => x.wo_id == wo_id && !x.is_archived)
                //   .Include(x => x.Tasks)
                .Include(x => x.StatusMaster)
                .Include(x => x.Parent_Asset)
                .Include(x => x.User)
                     // .Include(x => x.InspectionsTemplateFormIO)
                     //  .ThenInclude(x => x.FormIOType)
                     //  .Include(x => x.WOcategorytoTaskMapping)
                     //  .ThenInclude(x => x.Tasks)
                     // .ThenInclude(x => x.AssetTasks)
                     .Include(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.FormIOType)
                .ToList();
            /* var form_ids = wo_category.Select(x => x.form_id).ToList();
             var formio_master = context.InspectionsTemplateFormIO.Where(x => form_ids.Contains(x.form_id)).Include(x => x.FormIOType).ToList();
             wo_category.ForEach(x =>
             {
                 x.InspectionsTemplateFormIO = formio_master.Where(q => q.form_id == x.form_id).FirstOrDefault();
             });*/
            return wo_category;

        }
        public List<WOcategorytoTaskMapping> GetWoCategoryToTaskToviewWO(Guid wo_inspectionsTemplateFormIOAssignment_id)
        {
            return context.WOcategorytoTaskMapping.Where(x => x.wo_inspectionsTemplateFormIOAssignment_id == wo_inspectionsTemplateFormIOAssignment_id
            && !x.is_archived).ToList();
        }
        public List<FormIOFormsExcludedProprties> GetExcludedFormIOFormsByIds(List<Guid> form_ids)
        {
            var query = context.InspectionsTemplateFormIO.Where(x => form_ids.Contains(x.form_id));
            var response = query
                           .Select(x => new FormIOFormsExcludedProprties
                           {
                               form_id = x.form_id,
                               form_name = x.form_name,
                               form_type = x.FormIOType.form_type_name,
                               form_description = x.form_description,
                               status = x.status,
                               status_name = x.StatusMaster.status_name,
                               form_type_id = x.form_type_id,
                               work_procedure = x.work_procedure
                           }).ToList();
            return response;
        }
        public List<Tasks> GetTaskByFormID(Guid form_id) // return only task which are not mapped with any WOs
        {
            //return context.Tasks.Where(x => x.form_id == form_id && (x.WOcategorytoTaskMapping == null || (x.WOcategorytoTaskMapping!=null && x.WOcategorytoTaskMapping.All(w=>w.is_archived)))).ToList();
            return context.Tasks.Where(x => x.form_id == form_id)
                .Include(x => x.FormIO)
                .ToList();
        }
        public Tasks GetTaskByID(Guid task_id) // return only task which are not mapped with any WOs
        {
            //return context.Tasks.Where(x => x.form_id == form_id && (x.WOcategorytoTaskMapping == null || (x.WOcategorytoTaskMapping!=null && x.WOcategorytoTaskMapping.All(w=>w.is_archived)))).ToList();
            return context.Tasks.Where(x => x.task_id == task_id)
                .Include(x => x.FormIO).FirstOrDefault();
        }

        public (List<User>, int total_list_count) GetAllTechnician(GetAllTechnicianRequestModel requestModel)
        {
            List<User> response = new List<User>();
            int total_list_count = 0;
            string userid = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
            IQueryable<User> query = context.User;
            var users_from_userroles = context.UserRoles.Where(x => x.Role.name == GlobalConstants.Technician && x.status == (int)Status.Active).Select(x => x.user_id).ToList();
            query = query.Where(x => users_from_userroles.Contains(x.uuid));


            List<Guid> usersites = new List<Guid>();
            if (String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
            {
                usersites = context.User.Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).Select(x => x.ac_default_site.Value).ToList();
            }
            else if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
            {
                usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
            }
            else
            {
                usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
            }

            var users_by_sites = context.UserSites.Where(x => usersites.Contains(x.site_id) && x.status == (int)Status.Active).Select(x => x.user_id).Distinct().ToList();
            query = query.Where(x => users_by_sites.Contains(x.uuid));

            total_list_count = query.Count();
            if (requestModel.page_size > 0 && requestModel.page_index > 0)
            {
                query = query.Skip((requestModel.page_index - 1) * requestModel.page_size).Take(requestModel.page_size);
            }

            response = query
                            .ToList();
            return (response, total_list_count);
        }

        public List<WOcategorytoTaskMapping> GetWOcategoryTaskByCategoryID(string wo_inspectionsTemplateFormIOAssignment_id)
        {
            return context.WOcategorytoTaskMapping.Where(x => x.wo_inspectionsTemplateFormIOAssignment_id.ToString() == wo_inspectionsTemplateFormIOAssignment_id && !x.is_archived)
                                                          //.Include(x=>x.Tasks)
                                                          // .ThenInclude(x=>x.Asset)
                                                          .Include(x => x.AssetFormIO)
                                                            .ThenInclude(x => x.StatusMaster)
                                                            .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                                                             .ThenInclude(x => x.InspectionsTemplateFormIO)
                                                            //  .Include(x=>x.assigned_asset)
                                                            // .Include(x=>x.User)
                                                            .Include(x => x._assigned_asset)
                                                            .OrderByDescending(x => x.is_parent_task)
                                                          .ToList();
        }
        /* public List<WOcategorytoTaskMapping> GetWOcategoryTaskByWoid(Guid wo_id)
         {
             return context.WOcategorytoTaskMapping.Where(x => x.wo_id == wo_id && !x.is_archived)
                                                           .Include(x => x.Tasks)
                                                            .ThenInclude(x => x.Asset)
                                                           .Include(x => x.AssetFormIO)
                                                             .ThenInclude(x => x.StatusMaster)
                                                           .ToList();
         }*/
        public List<WOcategorytoTaskMapping> GetAllWOCategoryTaskByWOid(Guid wo_id)
        {
            return context.WOcategorytoTaskMapping.Where(x => x.wo_id == wo_id && !x.is_archived)
                                                          .Include(x => x.AssetFormIO)
                                                            .ThenInclude(x => x.StatusMaster)
                                                            .Include(x => x._assigned_asset)
                                                          //.Include(x=>x.assigned_asset)
                                                          .ToList();
        }
        public List<WOcategorytoTaskMapping> GetAllWOCategoryTaskByWOidForTask(Guid wo_id, int status)
        {
            if (status == 0)
            {
                return context.WOcategorytoTaskMapping.Where(x => x.wo_id == wo_id && !x.is_archived)
                                                            .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                                                              .ThenInclude(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.FormIOType)
                                                         .Include(x => x.AssetFormIO)
                                                            .ThenInclude(x => x.StatusMaster)
                                                            .Include(x => x.AssetFormIO)
                                                            .ThenInclude(x => x.Sites)
                                                            .Include(x => x._assigned_asset)
                                                          //.Include(x=>x.assigned_asset)
                                                          .ToList();
            }
            else
            {
                return context.WOcategorytoTaskMapping.Where(x => x.wo_id == wo_id && !x.is_archived && x.AssetFormIO.status == status)
                                                            .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                                                              .ThenInclude(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.FormIOType)
                                                         .Include(x => x.AssetFormIO)
                                                            .ThenInclude(x => x.StatusMaster)
                                                            .Include(x => x._assigned_asset)
                                                          //.Include(x=>x.assigned_asset)
                                                          .ToList();
            }
        }
        public List<WOcategorytoTaskMapping> GetAllWOCategoryTaskByWOidOffline(DateTime? sync_time)
        {
            IQueryable<WOcategorytoTaskMapping> query = context.WOcategorytoTaskMapping;
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.updated_at.Value >= sync_time);
            }
            else
            {
                query = query.Where(x => !x.is_archived && !x.WorkOrders.is_archive);
            }
            query = query.Where(x => x.WorkOrders.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            var repsonse = query
                                .Include(x => x.Tasks)
                                //.Include(x => x.AssetFormIO)
                                //  .ThenInclude(x => x.StatusMaster)
                                .Include(x => x._assigned_asset)
                                // .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                                //   .ThenInclude(x => x.InspectionsTemplateFormIO)
                                ///.Include(x=>x.assigned_asset)
                                .ToList();
            return repsonse;
        }
        public List<WOcategorytoTaskMapping> Getcategorytaskbycategoryids(List<Guid> WOcategory_id, DateTime? sync_time)
        {
            IQueryable<WOcategorytoTaskMapping> query = context.WOcategorytoTaskMapping;
            // if (sync_time != null)
            //  {
            //    query = query.Where(x => x.created_at.Date >= sync_time.Value.Date || x.updated_at.Value.Date >= sync_time);
            // }
            query = query.Where(x => WOcategory_id.Contains(x.wo_inspectionsTemplateFormIOAssignment_id));
            query = query.Where(x => x.WorkOrders.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            var repsonse = query
                                .Include(x => x.Tasks)
                                .Include(x => x.WorkOrders)
                                // .Include(x => x.AssetFormIO)
                                //   .ThenInclude(x => x.StatusMaster)
                                .Include(x => x._assigned_asset)
                                // .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                                //   .ThenInclude(x => x.InspectionsTemplateFormIO)
                                ///.Include(x=>x.assigned_asset)
                                .ToList();
            return repsonse;
        }
        public AssetFormIO GetFormByTaskID(Guid WOcategorytoTaskMapping_id)
        {
            return context.AssetFormIO.Where(x => x.WOcategorytoTaskMapping_id == WOcategorytoTaskMapping_id)
                .Include(x => x.Asset)
                .ThenInclude(x => x.Sites)
                .Include(x => x.WOcategorytoTaskMapping)
                .FirstOrDefault();
        }
        public AssetFormIO GetFormByTaskIDForBulkImport(Guid WOcategorytoTaskMapping_id)
        {
            return context.AssetFormIO.Where(x => x.WOcategorytoTaskMapping_id == WOcategorytoTaskMapping_id)
                .FirstOrDefault();
        }
        public WOcategorytoTaskMapping AssignTechniciantoWOcategoryTask(Guid WOcategorytoTaskMapping_id)
        {
            return context.WOcategorytoTaskMapping.Where(x => x.WOcategorytoTaskMapping_id == WOcategorytoTaskMapping_id).FirstOrDefault();
        }
        public WOcategorytoTaskMapping AssignAssettoWOcategoryTask(Guid WOcategorytoTaskMapping_id)
        {
            return context.WOcategorytoTaskMapping.Where(x => x.WOcategorytoTaskMapping_id == WOcategorytoTaskMapping_id)
                .Include(x => x.AssetFormIO)
                .Include(x => x.WorkOrders)
                .FirstOrDefault();
        }
        public List<WOcategorytoTaskMapping> UpdateMultiWOCategoryTaskStatus(List<Guid> WOcategorytoTaskMapping_id)
        {
            return context.WOcategorytoTaskMapping.Where(x => WOcategorytoTaskMapping_id.Contains(x.WOcategorytoTaskMapping_id))
                .Include(x => x.AssetFormIO)
                .Include(x => x._assigned_asset)
                /* .Include(x => x.Tasks)
                 .ThenInclude(x => x.FormIO)
                 .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                 .ThenInclude(x => x.WOcategorytoTaskMapping)
                 .ThenInclude(x => x.AssetFormIO)
                 .Include(x => x._assigned_asset)
                 .Include(x => x.WorkOrders)
                  .ThenInclude(x => x.WOInspectionsTemplateFormIOAssignment)*/
                .ToList();
        }
        public WOcategorytoTaskMapping GetWOcategoryTaskByTaskID(Guid WOcategorytoTaskMapping_id)
        {
            return context.WOcategorytoTaskMapping.Where(x => x.WOcategorytoTaskMapping_id == WOcategorytoTaskMapping_id)
                .Include(x => x.AssetFormIO)
                .Include(x => x.Tasks)
                .ThenInclude(x => x.FormIO)
                .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                .ThenInclude(x => x.WOcategorytoTaskMapping)
                .ThenInclude(x => x.AssetFormIO)
                .Include(x => x._assigned_asset)
                .Include(x => x.WorkOrders)
                 .ThenInclude(x => x.WOInspectionsTemplateFormIOAssignment)
                .FirstOrDefault();
        }
        public WOcategorytoTaskMapping GetWOcategoryTaskByTaskIDForUpdateStatus(Guid WOcategorytoTaskMapping_id)
        {
            return context.WOcategorytoTaskMapping.Where(x => x.WOcategorytoTaskMapping_id == WOcategorytoTaskMapping_id)
                .Include(x => x.AssetFormIO)
                .Include(x => x._assigned_asset)
                /* .Include(x => x.Tasks)
                 .ThenInclude(x => x.FormIO)
                 .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                 .ThenInclude(x => x.WOcategorytoTaskMapping)
                 .ThenInclude(x => x.AssetFormIO)
                 .Include(x => x._assigned_asset)
                 .Include(x => x.WorkOrders)
                  .ThenInclude(x => x.WOInspectionsTemplateFormIOAssignment)*/
                .FirstOrDefault();
        }
        public WOcategorytoTaskMapping GetWOcategoryTaskByTaskIDMobile(Guid WOcategorytoTaskMapping_id)
        {
            return context.WOcategorytoTaskMapping.Where(x => x.WOcategorytoTaskMapping_id == WOcategorytoTaskMapping_id)
                .FirstOrDefault();
        }
        public List<WOcategorytoTaskMapping> GetWOcategoryTaskByTaskIDforCopyfields(List<Guid> WOcategorytoTaskMapping_id)
        {
            return context.WOcategorytoTaskMapping.Where(x => WOcategorytoTaskMapping_id.Contains(x.WOcategorytoTaskMapping_id))
                .Include(x => x.AssetFormIO)
                .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                 .ThenInclude(x => x.InspectionsTemplateFormIO)
                .ToList();
        }
        public WOInspectionsTemplateFormIOAssignment GetWOcategoryID(Guid wo_inspectionsTemplateFormIOAssignment_id)
        {
            return context.WOInspectionsTemplateFormIOAssignment.Where(x => x.wo_inspectionsTemplateFormIOAssignment_id == wo_inspectionsTemplateFormIOAssignment_id)
                          .Include(x => x.WOcategorytoTaskMapping)
                          .ThenInclude(x => x.AssetFormIO)
                           .ThenInclude(x => x.AssetIssue)
                           .Include(x => x.WOcategorytoTaskMapping)
                          .ThenInclude(x => x.AssetFormIO)
                           .ThenInclude(x => x.WOLineIssue)
                          .Include(x => x.WorkOrders)
                           .Include(x => x.WOcategorytoTaskMapping)
                          .ThenInclude(x => x.AssetFormIO)
                          .ThenInclude(x => x.AssetPMs)
                          .FirstOrDefault();
        }
        public WOInspectionsTemplateFormIOAssignment GetWOcategoryIDFormobile(Guid wo_inspectionsTemplateFormIOAssignment_id)
        {
            return context.WOInspectionsTemplateFormIOAssignment.Where(x => x.wo_inspectionsTemplateFormIOAssignment_id == wo_inspectionsTemplateFormIOAssignment_id)
                          .FirstOrDefault();
        }
        public WOInspectionsTemplateFormIOAssignment GetWOcategoryWOID(Guid wo_id)
        {
            return context.WOInspectionsTemplateFormIOAssignment.Where(x => x.wo_id == wo_id)
                          .FirstOrDefault();
        }

        public WorkOrderAttachments GetWOAttachmentById(Guid wo_attachment_id)
        {
            return context.WorkOrderAttachments.Where(x => x.wo_attachment_id == wo_attachment_id).FirstOrDefault();
        }
        public List<string> GetWOAttachmentsById(Guid wo_attachment_id)
        {
            return context.WorkOrderAttachments.Where(x => x.wo_attachment_id == wo_attachment_id && !x.is_archive).Select(x=>x.filename).ToList();
        }

        public int GetAssetscountBySite(string site_id)
        {
            return context.Assets.Where(x => x.site_id.ToString() == site_id).Count();
        }
        public User GetUserByID(Guid user_id)
        {
            return context.User.Where(x => x.uuid == user_id).FirstOrDefault();
        }
        public List<User> GetUsersByIDs(List<Guid> user_id)
        {
            return context.User.Where(x => user_id.Contains(x.uuid)).ToList();
        }

        public Asset GetAssetByLocation(string location)
        {
            return context.Assets.Where(x => x.name.ToLower().Trim() == location && x.status == (int)Status.AssetActive && x.company_id == UpdatedGenericRequestmodel.CurrentUser.company_id && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id)
                .Include(x => x.AssetTopLevelcomponentMapping)
                .Include(x => x.AssetSubLevelcomponentMapping)
                .Include(x => x.AssetProfileImages)
                .FirstOrDefault();
        }
        public Asset GetAssetByAssetnames(List<string> location)
        {
            return context.Assets.Where(x => location.Contains(x.name) && x.status == (int)Status.AssetActive && x.company_id == UpdatedGenericRequestmodel.CurrentUser.company_id && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).FirstOrDefault();
        }
        public Asset GetAssetByInternalID(string internal_id)
        {
            return context.Assets.Where(x => x.internal_asset_id == internal_id && x.status == (int)Status.AssetActive && x.company_id == UpdatedGenericRequestmodel.CurrentUser.company_id && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).FirstOrDefault();
        }
        public Asset GetAssetByInternalIDCount(string internal_id, Guid requested_asset_id)
        {
            return context.Assets.Where(x => x.internal_asset_id == internal_id && x.status == (int)Status.AssetActive && x.company_id == UpdatedGenericRequestmodel.CurrentUser.company_id && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id
                                            && x.asset_id != requested_asset_id).FirstOrDefault();
        }
        public Asset GetAssetByID(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id && x.status == (int)Status.AssetActive && x.company_id == UpdatedGenericRequestmodel.CurrentUser.company_id)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOSections.FormIOLocationNotes)
                .Include(x => x.InspectionTemplateAssetClass)
                .Include(x => x.AssetProfileImages)
                .Include(x => x.AssetIRWOImagesLabelMapping)
                .Include(x => x.AssetTopLevelcomponentMapping)
                .FirstOrDefault();
        }

        public InspectionsTemplateFormIO GetFormIOByName(Guid form_id)
        {
            return context.InspectionsTemplateFormIO.Where(x => x.form_id == form_id && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id))
                .Include(x => x.Tasks)
                .Include(x => x.FormIOType)
                .FirstOrDefault();
        }
        public InspectionForms GetinspectionformbyFormType(int formiotype, Guid company_id)
        {
            return context.InspectionForms.Where(x => x.form_type_id == formiotype && x.company_id == company_id).FirstOrDefault();
        }

        public (List<WorkOrders>, int total_list_count) GetWOsForOffline(string userid, DateTime? sync_date)
        {
            List<WorkOrders> response = new List<WorkOrders>();
            int total_list_count = 0;
            if (userid != null)
            {
                IQueryable<WorkOrders> query = context.WorkOrders;
                List<Guid> usersites = new List<Guid>();
                if (String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
                {
                    usersites = context.User.Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).Select(x => x.ac_default_site.Value).ToList();
                }
                else if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
                query = query.Where(x => x.wo_type == (int)Status.Acceptance_Test_WO || x.wo_type == (int)Status.Maintenance_WO || x.wo_type == (int)Status.Onboarding_WO || x.wo_type == (int)Status.Maintenance_WO || x.wo_type == (int)Status.IR_Scan_WO);
                //if(sync_date == null)
                //{
                query = query.Where(x => x.status != (int)Status.PlannedWO);
                //}
                if (sync_date != null)
                {
                    query = query.Where(x => x.created_at.Value >= sync_date.Value || x.modified_at.Value >= sync_date.Value);
                }
                else
                {
                    query = query.Where(x => !x.is_archive);
                }
                query = query.OrderByDescending(x => x.created_at);
                total_list_count = query.Count();
                response = query
                    .Include(x => x.StatusMaster)
                    .Include(x => x.WOTypeStatusMaster)
                    .Include(x => x.WorkOrderAttachments)
                    .Include(x => x.Sites)
                     .ThenInclude(x => x.ClientCompany)
                    .ToList();
            }
            return (response, total_list_count);
        }
        public List<WOcategorytoTaskMapping> GetWOcompletedTask(Guid wo_inspectionsTemplateFormIOAssignment_id)
        {
            return context.WOcategorytoTaskMapping.Where(x => x.wo_inspectionsTemplateFormIOAssignment_id == wo_inspectionsTemplateFormIOAssignment_id
                                                            && (x.AssetFormIO.status == (int)Status.Completed || x.AssetFormIO.status == (int)Status.Submitted))
                                                          .Include(x => x.AssetFormIO)
                                                            .ThenInclude(x => x.StatusMaster)
                                                            .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                                                             .ThenInclude(x => x.InspectionsTemplateFormIO)
                                                            .Include(x => x._assigned_asset)
                                                            .OrderByDescending(x => x.is_parent_task)
                                                            .ToList();
        }
        public List<WOcategorytoTaskMapping> GetWOcompletedTaskForBulkImport(Guid wo_inspectionsTemplateFormIOAssignment_id)
        {
            return context.WOcategorytoTaskMapping.Where(x => x.wo_inspectionsTemplateFormIOAssignment_id == wo_inspectionsTemplateFormIOAssignment_id
                                                            && (x.AssetFormIO.status != (int)Status.Completed && x.AssetFormIO.status != (int)Status.Submitted && x.AssetFormIO.status != (int)Status.Deactive))
                                                            .ToList();
        }
        public ListViewModel<Asset> OfflineAssetData(DateTime? sync_time)
        {
            ListViewModel<Asset> assets = new ListViewModel<Asset>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {

                IQueryable<Asset> query = context.Assets;
                // var usersites = context.UserSites.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

                // var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                if (sync_time != null)
                {
                    query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
                }
                query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
                query = query.Where(x => x.status == (int)Status.AssetActive);
                assets.listsize = query.Count();
                query = query.OrderBy(x => x.name);
                assets.list = query.Include(x => x.Sites)
                                   .Include(y => y.Sites.Company)
                                   .Include(x => x.StatusMaster)
                                   .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                                   .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                                   .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                                   .Include(x => x.AssetFormIOBuildingMappings.FormIOSections.FormIOLocationNotes)
                                   .Include(x => x.InspectionTemplateAssetClass).ThenInclude(x=>x.FormIOType)
                                   //.Include(x => x.Issues)
                                   ///.Include(x => x.Inspection)
                                   //.ThenInclude(x => x.StatusMaster)
                                   .ToList();

                /*  if (assets.list != null && assets.list.Count > 0)
                  {
                      var asset_ids = assets.list.Select(x => x.asset_id);
                      //var inspections = context.Inspection.Where(x => asset_ids.Contains(x.asset_id)).Include(x => x.StatusMaster).ToList();
                      var issues = context.Issue.Where(x => asset_ids.Contains(x.asset_id)).ToList();
                      assets.list.ForEach(x =>
                      {
                          //  x.Inspection = inspections.Where(q => q.asset_id == x.asset_id).ToList();
                          x.Issues = issues.Where(q => q.asset_id == x.asset_id).ToList();
                      });
                  }*/
                // assets.pageIndex = requestModel.pageindex;
                // assets.pageSize = requestModel.pagesize;

            }
            return assets;
        }
        public List<AssetClassFormIOMapping> GetAssetClassFormIOMappingOffline(DateTime? sync_time)
        {
            IQueryable<AssetClassFormIOMapping> query = context.AssetClassFormIOMapping;
            // var usersites = context.UserSites.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

            // var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();


            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.isarchive);
            }
            return query.ToList();

        }
        public List<AssetProfileImages> GetAssetProfileImagesForoffline(DateTime? sync_time)
        {
            IQueryable<AssetProfileImages> query = context.AssetProfileImages;
            query = query.Where(x => x.Asset.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }
        public List<InspectionTemplateAssetClass> GetMasterClassForOffline(DateTime? sync_time)
        {
            IQueryable<InspectionTemplateAssetClass> query = context.InspectionTemplateAssetClass;
            query = query.Where(x => x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.isarchive);
            }
            return query.Include(x => x.FormIOType).ToList();
        }
        public List<AssetClassFormIOMapping> GetMasterClassFormMappingForOffline(DateTime? sync_time)
        {
            IQueryable<AssetClassFormIOMapping> query = context.AssetClassFormIOMapping;
            query = query.Where(x => x.InspectionTemplateAssetClass.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.isarchive);
            }
            return query.ToList();
        }
        public List<WOOnboardingAssets> GetOBAssetdetailsOffline(DateTime? sync_time)
        {
            IQueryable<WOOnboardingAssets> query = context.WOOnboardingAssets;
            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            query = query
                        .Include(x => x.StatusMaster)
                        ;
            return query.ToList();
        }
        public List<WOLineBuildingMapping> GetWOLineBuildingMappingOffline(DateTime? sync_time)
        {
            IQueryable<WOLineBuildingMapping> query = context.WOLineBuildingMapping;
            query = query
                .Include(x => x.WOOnboardingAssets)
                .Where(x => x.WOOnboardingAssets.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            return query.ToList();
        }
        public List<FormIOBuildings> GetFormIOBuildingsOffline(DateTime? sync_time)
        {
            IQueryable<FormIOBuildings> query = context.FormIOBuildings;
            query = query
                .Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            return query.ToList();
        }
        public List<FormIOFloors> GetFormIOFloorsOffline(DateTime? sync_time)
        {
            IQueryable<FormIOFloors> query = context.FormIOFloors;
            query = query
                .Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            return query.ToList();
        }
        public List<FormIORooms> GetFormIORoomsOffline(DateTime? sync_time)
        {
            IQueryable<FormIORooms> query = context.FormIORooms;
            query = query
                .Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            return query.ToList();
        }
        public List<FormIOSections> GetFormIOSectionOffline(DateTime? sync_time)
        {
            IQueryable<FormIOSections> query = context.FormIOSections;
            query = query
                .Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            return query.ToList();
        }
        public List<WOOBAssetFedByMapping> GetOBFedByAssetMappingOffline(DateTime? sync_time)
        {
            IQueryable<WOOBAssetFedByMapping> query = context.WOOBAssetFedByMapping;
            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.updated_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }

            return query.ToList();
        }
        public List<WOOnboardingAssetsImagesMapping> GetOBAssetdetailsImagesOffline(DateTime? sync_time)
        {
            IQueryable<WOOnboardingAssetsImagesMapping> query = context.WOOnboardingAssetsImagesMapping;
            query = query.Where(x => x.WOOnboardingAssets.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            //query = query;
            return query.ToList();
        }
        public List<IRWOImagesLabelMapping> GetOBAssetdetailsImagesLabelsOffline(DateTime? sync_time)
        {
            IQueryable<IRWOImagesLabelMapping> query = context.IRWOImagesLabelMapping;
            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.updated_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            //query = query;
            return query.ToList();
        }
        public List<AssetParentHierarchyMapping> GetAssetParentMappingOffline(DateTime? sync_time)
        {
            IQueryable<AssetParentHierarchyMapping> query = context.AssetParentHierarchyMapping;
            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.updated_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            //query = query;
            return query.ToList();
        }
        public List<Equipment> GetFormIOEquipmentOffline(DateTime? sync_time)
        {
            IQueryable<Equipment> query = context.Equipment;
            query = query
                .Where(x => x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)
                );

            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.isarchive);
            }
            return query.ToList();

        }
        public List<WOInspectionsTemplateFormIOAssignment> GetAllCatagoryForWOOffline(DateTime? sync_time)
        {
            List<WOInspectionsTemplateFormIOAssignment> response = new List<WOInspectionsTemplateFormIOAssignment>();
            int total_list_count = 0;
            IQueryable<WOInspectionsTemplateFormIOAssignment> query = context.WOInspectionsTemplateFormIOAssignment;
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.updated_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_archived && !x.WorkOrders.is_archive);
            }
            query = query.Where(x => x.WorkOrders.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            total_list_count = query.Count();
            response = query
                 .Include(x => x.Parent_Asset)
                 .Include(x => x.StatusMaster)
                 .Include(x => x.InspectionTemplateAssetClass)

                 .ToList();

            var wo_form_io_master_ids = response.Select(x => x.form_id).Distinct().ToList();
            var form_io_master = context.InspectionsTemplateFormIO.Where(x => wo_form_io_master_ids.Contains(x.form_id))
                .Include(x => x.FormIOType).ToList();
            response.ForEach(x =>
            {
                x.InspectionsTemplateFormIO = form_io_master.Where(q => q.form_id == x.form_id).FirstOrDefault();
            });
            return response;
        }
        public List<WOInspectionsTemplateFormIOAssignment> GetCatagoryForWOOfflineByCategoryids(List<Guid> wo_inspection_template_formio_ids)
        {
            List<WOInspectionsTemplateFormIOAssignment> response = new List<WOInspectionsTemplateFormIOAssignment>();
            int total_list_count = 0;
            IQueryable<WOInspectionsTemplateFormIOAssignment> query = context.WOInspectionsTemplateFormIOAssignment;
            query = query.Where(x => wo_inspection_template_formio_ids.Contains(x.wo_inspectionsTemplateFormIOAssignment_id));
            query = query.Where(x => x.WorkOrders.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            total_list_count = query.Count();
            response = query
                 .Include(x => x.Parent_Asset)
                 .Include(x => x.StatusMaster)
                 .Include(x => x.InspectionTemplateAssetClass)
                 .ToList();

            var wo_form_io_master_ids = response.Select(x => x.form_id).Distinct().ToList();
            var form_io_master = context.InspectionsTemplateFormIO.Where(x => wo_form_io_master_ids.Contains(x.form_id))
                .Include(x => x.FormIOType).ToList();
            response.ForEach(x =>
            {
                x.InspectionsTemplateFormIO = form_io_master.Where(q => q.form_id == x.form_id).FirstOrDefault();
            });
            return response;
        }
        public List<AssetFormIOExclude> GetAllAssetFormByWOIDOffline(List<Guid> wo_id, DateTime? sync_time)
        {
            IQueryable<AssetFormIO> query = context.AssetFormIO;
            if (wo_id.Count > 0)
            {
                query = query.Where(x => !x.WOcategorytoTaskMapping.is_archived);
                query = query.Where(x => wo_id.Contains(x.wo_id.Value));
                query = query.Where(x => x.WorkOrders.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            }
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
                query = query.Where(x => x.WorkOrders.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            }
            else
            {
                query = query.Where(x => x.status != (int)Status.Deactive && !x.WorkOrders.is_archive);
                query = query.Where(x => x.WorkOrders.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            }

            var response = query
                .Select(x => new AssetFormIOExclude
                {
                    asset_form_id = x.asset_form_id,
                    asset_id = x.asset_id,
                    site_id = x.site_id,
                    form_id = x.form_id,
                    asset_form_name = x.asset_form_name,
                    asset_form_type = x.asset_form_type,
                    asset_form_description = x.asset_form_description,
                    // asset_form_data = x.asset_form_data,
                    requested_by = x.requested_by,
                    created_at = x.created_at,
                    created_by = x.created_by,
                    modified_at = x.modified_at,
                    modified_by = x.modified_by,
                    accepted_by = x.accepted_by,
                    status = x.status,
                    wo_id = x.wo_id,
                    WOcategorytoTaskMapping_id = x.WOcategorytoTaskMapping_id,
                    pdf_report_status = x.pdf_report_status,
                    pdf_report_url = x.pdf_report_url,
                    form_retrived_asset_name = x.form_retrived_asset_name,
                    form_retrived_asset_id = x.form_retrived_asset_id,
                    form_retrived_location = x.form_retrived_location,
                    // form_retrived_data = x.form_retrived_data,
                    intial_form_filled_date = x.intial_form_filled_date,
                    form_retrived_nameplate_info = x.form_retrived_nameplate_info,
                    inspected_at = x.inspected_at,
                    accepted_at = x.accepted_at,
                    export_pdf_at = x.export_pdf_at,
                    asset_name = x.Asset.name,
                    status_name = x.StatusMaster.status_name,
                    timezone = x.Asset.Sites.timezone,
                    wo_inspectionsTemplateFormIOAssignment_id = x.WOcategorytoTaskMapping.wo_inspectionsTemplateFormIOAssignment_id,
                    building = x.building,
                    floor = x.floor,
                    room = x.room,
                    section = x.section,
                    form_retrived_workOrderType = x.form_retrived_workOrderType
                })
                .ToList();
            var form_ids = response.Select(x => x.asset_form_id).ToList();
            var asset_form_data = context.AssetFormIO
                .Select(x => new AssetFormIOExclude
                {
                    asset_form_id = x.asset_form_id,
                    asset_form_data = x.asset_form_data,
                    building = x.building,
                    floor = x.floor,
                    room = x.room,
                    section = x.section,
                }).Where(x => form_ids.Contains(x.asset_form_id)).ToList();
            response.ForEach(x =>
            {
                x.asset_form_data = asset_form_data.Where(q => q.asset_form_id == x.asset_form_id).Select(q => q.asset_form_data).FirstOrDefault();
            });
            return response;
        }

        public List<AssetFormIOExclude> GetAssetFormBycategorytaskid(List<Guid> WOcategorytoTaskMapping_id, DateTime? sync_time)
        {
            IQueryable<AssetFormIO> query = context.AssetFormIO;

            query = query.Where(x => WOcategorytoTaskMapping_id.Contains(x.WOcategorytoTaskMapping_id.Value));

            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            var response = query
                .Select(x => new AssetFormIOExclude
                {
                    asset_form_id = x.asset_form_id,
                    asset_id = x.asset_id,
                    site_id = x.site_id,
                    form_id = x.form_id,
                    asset_form_name = x.asset_form_name,
                    asset_form_type = x.asset_form_type,
                    asset_form_description = x.asset_form_description,
                    //asset_form_data = x.asset_form_data,
                    requested_by = x.requested_by,
                    created_at = x.created_at,
                    created_by = x.created_by,
                    modified_at = x.modified_at,
                    modified_by = x.modified_by,
                    accepted_by = x.accepted_by,
                    status = x.status,
                    wo_id = x.wo_id,
                    WOcategorytoTaskMapping_id = x.WOcategorytoTaskMapping_id,
                    pdf_report_status = x.pdf_report_status,
                    pdf_report_url = x.pdf_report_url,
                    form_retrived_asset_name = x.form_retrived_asset_name,
                    form_retrived_asset_id = x.form_retrived_asset_id,
                    form_retrived_location = x.form_retrived_location,
                    // form_retrived_data = x.form_retrived_data,
                    intial_form_filled_date = x.intial_form_filled_date,
                    form_retrived_nameplate_info = x.form_retrived_nameplate_info,
                    inspected_at = x.inspected_at,
                    accepted_at = x.accepted_at,
                    export_pdf_at = x.export_pdf_at,
                    asset_name = x.Asset.name,
                    status_name = x.StatusMaster.status_name,
                    timezone = x.Asset.Sites.timezone
                })
                .ToList();
            return response;
        }

        List<User> IWorkOrderRepository.GetUserByIDs(List<Guid> user_ids)
        {
            return context.User.Where(x => user_ids.Contains(x.uuid)).ToList();
        }

        public FormIOBuildings GetFormIOBuildingByName(string building_name)
        {
            return context.FormIOBuildings.Where(x => x.formio_building_name.Trim().ToLower() == building_name.Trim().ToLower() && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public FormIOBuildings GetFormIOBuildingByNameTemp(string building_name, Guid siteId)
        {
            return context.FormIOBuildings.Where(x => x.formio_building_name.Trim().ToLower() == building_name.Trim().ToLower() && x.site_id == siteId).FirstOrDefault();
        }
        public FormIOBuildings ScriptGetFormIOBuildingByName(string building_name, Guid site_id, Guid company_id)
        {
            return context.FormIOBuildings.Where(x => x.formio_building_name.Trim().ToLower() == building_name.Trim().ToLower() && x.company_id == company_id && x.site_id == site_id).FirstOrDefault();
        }
        public FormIOFloors GetFormIOFloorByName(string floor_name, int building_id)
        {
            return context.FormIOFloors.Where(x => x.formio_floor_name.Trim().ToLower() == floor_name.Trim().ToLower() && x.formiobuilding_id == building_id && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public FormIOFloors GetFormIOFloorByNameTemp(string floor_name, int building_id, Guid siteId)
        {
            return context.FormIOFloors.Where(x => x.formio_floor_name.Trim().ToLower() == floor_name.Trim().ToLower() && x.formiobuilding_id == building_id && x.site_id == siteId).FirstOrDefault();
        }
        public FormIOFloors ScriptGetFormIOFloorByName(string floor_name, int building_id, Guid site_id, Guid company_id)
        {
            return context.FormIOFloors.Where(x => x.formio_floor_name.Trim().ToLower() == floor_name.Trim().ToLower() && x.formiobuilding_id == building_id && x.company_id == company_id && x.site_id == site_id).FirstOrDefault();
        }
        public FormIORooms GetFormIORoomByName(string room_name, int floor_id)
        {
            return context.FormIORooms.Where(x => x.formio_room_name.Trim().ToLower() == room_name.Trim().ToLower() && x.formiofloor_id == floor_id && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public FormIORooms GetFormIORoomByNameTemp(string room_name, int floor_id, Guid siteId)
        {
            return context.FormIORooms.Where(x => x.formio_room_name.Trim().ToLower() == room_name.Trim().ToLower() && x.formiofloor_id == floor_id && x.site_id == siteId).FirstOrDefault();
        }
        public FormIORooms ScriptGetFormIORoomByName(string room_name, int floor_id, Guid site_id, Guid company_id)
        {
            return context.FormIORooms.Where(x => x.formio_room_name.Trim().ToLower() == room_name.Trim().ToLower() && x.formiofloor_id == floor_id && x.company_id == company_id && x.site_id == site_id).FirstOrDefault();
        }
        public FormIOSections GetFormIOSectionByName(string section_name, int room_id)
        {
            return context.FormIOSections.Where(x => x.formio_section_name.Trim().ToLower() == section_name.Trim().ToLower() && x.formioroom_id == room_id && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public FormIOSections GetFormIOSectionByNameTemp(string section_name, int room_id, Guid siteId)
        {
            return context.FormIOSections.Where(x => x.formio_section_name.Trim().ToLower() == section_name.Trim().ToLower() && x.formioroom_id == room_id && x.site_id == siteId).FirstOrDefault();
        }
        public FormIOSections ScriptGetFormIOSectionByName(string section_name, int room_id, Guid site_id, Guid company_id)
        {
            return context.FormIOSections.Where(x => x.formio_section_name.Trim().ToLower() == section_name.Trim().ToLower() && x.formioroom_id == room_id && x.company_id == company_id && x.site_id == site_id).FirstOrDefault();
        }
        public FormIOLocationNotes GetFormIONotesBySection(int section_id)
        {
            return context.FormIOLocationNotes.Where(x => x.formiosection_id == section_id).FirstOrDefault();
        }
        public List<FormIOBuildings> GetAssetBuildingHierarchy()
        {
            return context.FormIOBuildings.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id))
                .Include(x => x.FormIOFloors)
                 .ThenInclude(x => x.FormIORooms)
                 .ThenInclude(x => x.FormIOSections)
                  .ThenInclude(x => x.AssetFormIOBuildingMappings)
                   .ThenInclude(x => x.Asset)
                .ToList();
        }
        public List<AssetFormIO> getallformsbywos(Guid wo_id)
        {
            return context.AssetFormIO.Where(x => x.wo_id == wo_id && (x.status == (int)Status.InProgress || x.status == (int)Status.Ready_for_review)).ToList();
        }
        public WorkOrders GetWOByidforDelete(Guid wo_id)
        {
            return context.WorkOrders.Where(x => x.wo_id == wo_id && x.status != (int)Status.Completed)
                .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                .Include(x => x.WOcategorytoTaskMapping)
                .Include(x => x.AssetFormIO)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOOnboardingAssetsDateTimeTracking)
                .Include(x => x.AssetPMs).ThenInclude(x => x.ActiveAssetPMWOlineMapping)
                .Include(x => x.AssetIssue)
                .FirstOrDefault();
        }
        public WOInspectionsTemplateFormIOAssignment GetWOCategoryStatusforStatusmanage(Guid wo_inspectionsTemplateFormIOAssignment_id)
        {
            return context.WOInspectionsTemplateFormIOAssignment.Where(x => x.wo_inspectionsTemplateFormIOAssignment_id == wo_inspectionsTemplateFormIOAssignment_id)
                                                                .Include(x => x.WOcategorytoTaskMapping)
                                                                .FirstOrDefault();
        }
        public List<AssetFormIOStatusExcluded> GetWOAssetFormIOStatusforStatusmanage(List<Guid> WOcategorytoTaskMapping_id)
        {
            var response = context.AssetFormIO.Where(x => WOcategorytoTaskMapping_id.Contains(x.WOcategorytoTaskMapping_id.Value) && x.status != (int)Status.Deactive)
              .Select(x => new AssetFormIOStatusExcluded
              {
                  asset_form_id = x.asset_form_id,
                  status = x.status
              })
              .ToList();
            return response;
        }
        public WorkOrders GetWOByidforUpdateStatus(Guid wo_id)
        {
            return context.WorkOrders.Where(x => x.wo_id == wo_id && x.status != (int)Status.Completed)
                .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                // .Include(x => x.WOcategorytoTaskMapping)
                // .Include(x => x.AssetFormIO)
                .Include(x => x.WOOnboardingAssets)
                .FirstOrDefault();
        }
        public List<WorkOrderAttachments> GetWOsAttachmentsForOffline(DateTime? sync_time)
        {
            IQueryable<WorkOrderAttachments> query = context.WorkOrderAttachments.Where(x => x.WorkOrders.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time == null)
            {
                query = query.Where(x => !x.is_archive);
            }
            else
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            return query.ToList();
        }

        public List<Guid> GetAssetClassByForm(Guid Form_id)
        {
            return context.AssetClassFormIOMapping.Where(x => x.form_id == Form_id && !x.isarchive).Select(x => x.InspectionTemplateAssetClass.inspectiontemplate_asset_class_id).ToList();
        }
        public List<Guid> GetAssetClassBywotype(int wo_type_id)
        {
            return context.AssetClassFormIOMapping.Where(x => x.wo_type == wo_type_id && !x.isarchive && x.InspectionsTemplateFormIO != null).Select(x => x.InspectionTemplateAssetClass.inspectiontemplate_asset_class_id).ToList();
        }

        public InspectionTemplateAssetClass GetInspectionTemplateAssetClass(Guid inspectiontemplate_asset_class_id)
        {
            return context.InspectionTemplateAssetClass.Where(x => x.inspectiontemplate_asset_class_id == inspectiontemplate_asset_class_id)
                .Include(x => x.PMCategory)
                    .ThenInclude(x => x.PMPlans)
                .FirstOrDefault();
        }

        public List<Asset> GetAssetsByAssetClass(List<Guid> asset_class_id)
        {
            return context.Assets.Where(x => asset_class_id.Contains(x.inspectiontemplate_asset_class_id.Value) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
            && x.status == (int)Status.AssetActive
            )
                .Include(x => x.InspectionTemplateAssetClass)
                 .ThenInclude(x => x.AssetClassFormIOMapping)
                 .OrderBy(x => x.name)
                //.Include(x=>x.InspectionsTemplateFormIO)
                .ToList();
        }
        public List<WorkOrders> GetWOs()
        {
            return context.WorkOrders.ToList();
        }
        public bool IsWONumberValid(Guid? wo_id, string wo_number)
        {
            if (wo_id != null)
            {
                var get_wo = context.WorkOrders.Where(x => x.wo_id != wo_id && x.manual_wo_number.Trim().ToLower() == wo_number.Trim().ToLower()
                && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_archive).FirstOrDefault();
                if (get_wo != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                var get_wo = context.WorkOrders.Where(x => x.manual_wo_number.Trim().ToLower() == wo_number.Trim().ToLower() && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_archive).FirstOrDefault();
                if (get_wo != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public bool IsWONumberValidFromEstimator(Guid? wo_id, string wo_number, Guid site_id)
        {
            if (wo_id != null)
            {
                var get_wo = context.WorkOrders.Where(x => x.wo_id != wo_id && x.manual_wo_number.Trim().ToLower() == wo_number.Trim().ToLower()
                && x.site_id == site_id && !x.is_archive).FirstOrDefault();
                if (get_wo != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                var get_wo = context.WorkOrders.Where(x => x.manual_wo_number.Trim().ToLower() == wo_number.Trim().ToLower() && x.site_id == site_id && !x.is_archive).FirstOrDefault();
                if (get_wo != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public List<AssetClassFormIOMappingExcludeProperty> GetAssetclassFormToAddcategory(GetAssetclassFormToAddcategoryRequestmodel requestmodel)
        {
            IQueryable<AssetClassFormIOMapping> query = context.AssetClassFormIOMapping.Where(x => x.InspectionTemplateAssetClass.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)
                                                        && !x.isarchive && x.wo_type == requestmodel.wo_type && x.InspectionsTemplateFormIO != null);
            var response = query
               .Select(x => new AssetClassFormIOMappingExcludeProperty
               {
                   inspectiontemplate_asset_class_id = x.inspectiontemplate_asset_class_id,
                   asset_class_name = x.InspectionTemplateAssetClass.asset_class_name,
                   asset_class_code = x.InspectionTemplateAssetClass.asset_class_code,
                   form_id = x.form_id,
                   form_name = x.InspectionsTemplateFormIO.form_name
               })
               .ToList();

            return response;
        }

        public InspectionTemplateAssetClass GetAssetclassByCode(string asset_class_code)
        {
            return context.InspectionTemplateAssetClass.Where(x => x.asset_class_code.ToLower().Trim() == asset_class_code
                                                                 && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)
                                                                 && !x.isarchive)
                .Include(x => x.AssetClassFormIOMapping)
                .FirstOrDefault();
        }
        public InspectionTemplateAssetClass GetAssetclassByCodeandCompanyId(string asset_class_code, Guid company_id)
        {
            return context.InspectionTemplateAssetClass.Where(x => x.asset_class_code.ToLower().Trim() == asset_class_code
                                                                 && x.company_id == company_id
                                                                 && !x.isarchive)
                .Include(x => x.AssetClassFormIOMapping)
                .FirstOrDefault();
        }
        public InspectionTemplateAssetClass GetAssetclassByCodeForDuplicate(string asset_class_code, Guid? inspectiontemplate_asset_class_id)
        {
            if (inspectiontemplate_asset_class_id == null)
            {
                return context.InspectionTemplateAssetClass.Where(x => x.asset_class_code.ToLower().Trim() == asset_class_code
                                                                 && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)
                                                                 && !x.isarchive)
                .Include(x => x.AssetClassFormIOMapping)
                .FirstOrDefault();
            }
            else
            {
                return context.InspectionTemplateAssetClass.Where(x => x.asset_class_code.ToLower().Trim() == asset_class_code
                                                                 && x.inspectiontemplate_asset_class_id != inspectiontemplate_asset_class_id
                                                                 && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)
                                                                 && !x.isarchive)
                .Include(x => x.AssetClassFormIOMapping)
                .FirstOrDefault();
            }
        }
        public InspectionTemplateAssetClass GetAssetclassByID(Guid inspectiontemplate_asset_class_id)
        {
            return context.InspectionTemplateAssetClass.Where(x => x.inspectiontemplate_asset_class_id == inspectiontemplate_asset_class_id
                                                                 && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)
                                                                 && !x.isarchive)
                .Include(x => x.AssetClassFormIOMapping)
                .Include(x => x.PMCategory)
                .FirstOrDefault();
        }
        public WOOnboardingAssets GetOBWOAssetDetailsById(Guid woonboardingassets_id)
        {
            var ob_asset = context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                           .Include(x => x.Sites)
                           .Include(x => x.SitewalkthroughTempPmEstimation).ThenInclude(x => x.PMPlans).ThenInclude(x => x.PMs)
                          .Include(x => x.WOOnboardingAssetsImagesMapping)
                          .Include(x => x.IRWOImagesLabelMapping)
                          .Include(x => x.StatusMaster)
                          .Include(x => x.Asset)
                           .ThenInclude(x => x.InspectionTemplateAssetClass)
                          .Include(x => x.WOOBAssetFedByMapping)
                          .Include(x => x.AssetIssue)
                          //.Include(x => x.WOLineIssue).ThenInclude(x => x.WOlineIssueImagesMapping)
                          .Include(x => x.WorkOrders)
                          .Include(x => x.WOLineBuildingMapping)
                          .Include(x => x.WOlineTopLevelcomponentMapping)
                          //.Include(x => x.WOlineSubLevelcomponentMapping).ThenInclude(x => x.WOOnboardingAssets.WOOnboardingAssetsImagesMapping)
                          .Include(x => x.AssetPMs.ActiveAssetPMWOlineMapping)
                          .Include(x => x.WOLineBuildingMapping.FormIOBuildings)
                          .Include(x => x.WOLineBuildingMapping.FormIOFloors)
                          .Include(x => x.WOLineBuildingMapping.FormIORooms)
                          .Include(x => x.WOLineBuildingMapping.FormIOSections)
                          .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings)
                          .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors)
                          .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms)
                          .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections)
                          .Include(x => x.TempAssetPMs).ThenInclude(x => x.TempActiveAssetPMWOlineMapping).ThenInclude(x => x.WOOnboardingAssets)
                          .Include(x => x.TempActiveAssetPMWOlineMapping.TempAssetPMs)
                          .Include(x => x.TempAsset.TempFormIOBuildings)
                          .Include(x => x.TempAsset.TempFormIOFloors)
                          .Include(x => x.TempAsset.TempFormIORooms)
                          .Include(x => x.TempAsset.TempFormIOSections)
                          .Include(x => x.TempAsset.TempMasterBuilding)
                          .Include(x => x.TempAsset.TempMasterFloor)
                          .Include(x => x.TempAsset.TempMasterRoom)
                          .Include(x => x.TempAsset.InspectionTemplateAssetClass.FormIOType)
                          .Include(x => x.TempAsset.AssetGroup)
                          .Include(x => x.WOOnboardingAssetsDateTimeTracking)
                          .FirstOrDefault();
            if (ob_asset != null)
            {
                ob_asset.WOLineIssue = context.WOLineIssue.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Include(x => x.WOlineIssueImagesMapping).ToList();

                ob_asset.WOlineSubLevelcomponentMapping = context.WOlineSubLevelcomponentMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                    .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOOnboardingAssetsImagesMapping).ToList();
            }
            return ob_asset;
        }

        public WOOnboardingAssets GetOBWOAssetDetailsByIdForMWO(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                          .FirstOrDefault();
        }
        public WOOnboardingAssets GetWOLineByQRcode(string qr_code, Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id != woonboardingassets_id
                                                      && x.QR_code.ToLower().Trim() == qr_code && !x.is_deleted
                                                      && (x.inspection_type == (int)MWO_inspection_wo_type.OnBoarding || x.inspection_type == (int)MWO_inspection_wo_type.Inspection)
                                                      && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_deleted)
                                                     .FirstOrDefault();
        }
        public WorkOrders GetOBWOByID(Guid wo_id)
        {
            return context.WorkOrders.Where(x => x.wo_id == wo_id)
                          .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOOnboardingAssetsImagesMapping)
                          .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.IRWOImagesLabelMapping)
                          .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.Sites)
                          .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOOBAssetFedByMapping)
                          .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOLineIssue)
                          .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOlineTopLevelcomponentMapping)
                          .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOlineSubLevelcomponentMapping)
                          .FirstOrDefault();
        }
        public WOOnboardingAssets GetOBWOAssetByID(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                          .Include(x => x.WOOnboardingAssetsImagesMapping)
                          .Include(x => x.IRWOImagesLabelMapping)
                          .Include(x => x.Sites)
                          .Include(x => x.WOOBAssetFedByMapping)
                          .Include(x => x.WOLineIssue).ThenInclude(x => x.WOlineIssueImagesMapping)
                          .Include(x => x.AssetPMs)
                          .Include(x => x.ActiveAssetPMWOlineMapping).ThenInclude(x => x.WOlineAssetPMImagesMapping)
                          .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings)
                          .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors)
                          .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms)
                          .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections)
                          .Include(x => x.TempAsset)
                          .FirstOrDefault();
        }
        public WOOnboardingAssets GetCompletedIssueWOlinebyTempAsset(Guid tempasset_id)
        {
            return context.WOOnboardingAssets.Where(x => (x.inspection_type == (int)MWO_inspection_wo_type.Repair
                                                    || x.inspection_type == (int)MWO_inspection_wo_type.Replace
                                                    || x.inspection_type == (int)MWO_inspection_wo_type.Trouble_Call_Check
                                                    || x.inspection_type == (int)MWO_inspection_wo_type.Others)
                                                    && x.status == (int)Status.Completed
                                                    && !x.is_deleted

            ).FirstOrDefault();
        }
        public WorkOrders GetOBWOByIDForComplete(Guid wo_id)
        {
            return context.WorkOrders.Where(x => x.wo_id == wo_id)
                           .Include(x => x.Sites)
                          .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOOBAssetFedByMapping)
                          .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOlineTopLevelcomponentMapping)
                          .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOlineSubLevelcomponentMapping)
                         .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.TempActiveAssetPMWOlineMapping.TempAssetPMs.WOOnboardingAssets)
                         .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.TempActiveAssetPMWOlineMapping.WOlineAssetPMImagesMapping)
                         .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.TempActiveAssetPMWOlineMapping.TempAssetPMs.PMs.PMPlans)
                         .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.TempActiveAssetPMWOlineMapping.TempAssetPMs.PMs.PMAttachments)
                         .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.TempActiveAssetPMWOlineMapping.TempAssetPMs.PMs.PMsTriggerConditionMapping)
                          .FirstOrDefault();
        }

        public List<WOOnboardingAssets> GetIssueInstallWoline(Guid wo_id)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && x.is_woline_from_other_inspection && !x.is_deleted && !x.is_main_asset_created)
                          .ToList();
        }

        public List<ExportCompletedAssetsByWOExcludedProperty> ExportCompletedAssetsByWO(ExportCompletedAssetsByWORequestmodel requestmodel)
        {
            IQueryable<AssetFormIO> query = context.AssetFormIO.Where(x => (x.status == (int)Status.Completed || x.status == (int)Status.Submitted));
            if (requestmodel.wo_id != null)
            {
                query = query.Where(x => x.wo_id == requestmodel.wo_id);
            }
            else
            {
                query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            }

            var response = query
               .Select(x => new ExportCompletedAssetsByWOExcludedProperty
               {
                   form_retrived_asset_name = x.form_retrived_asset_name,
                   intial_form_filled_date = x.intial_form_filled_date
               })
               .ToList();
            return response;
        }

        public List<WOOnboardingAssets> GetOBAssetsListByWOId(ExportCompletedAssetsByWORequestmodel requestmodel)
        {
            IQueryable<WOOnboardingAssets> query = context.WOOnboardingAssets.Where(x => !x.is_deleted
            && !x.is_woline_from_other_inspection
            && (x.status == (int)Status.Completed || x.status == (int)Status.Submitted));

            if (requestmodel.wo_id != null)
            {
                query = query.Where(x => x.wo_id == requestmodel.wo_id);
            }
            else
            {
                query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            }

            return query.Include(x => x.TempAsset).Include(x => x.Asset).ToList();
        }

        public List<InspectionsTemplateFormIO> GetMasterFormsByFormIDs(List<Guid> form_ids)
        {
            return context.InspectionsTemplateFormIO.Where(x => form_ids.Contains(x.form_id)).ToList();
        }
        public List<AssetFormIO> GetAssetFormIOByWOid(Guid wo_id)
        {
            return context.AssetFormIO.Where(x => x.wo_id == wo_id && (x.status == (int)Status.Completed || x.status == (int)Status.Submitted) && !x.is_main_asset_created)
                .Include(x => x.WOLineIssue)
                .Include(x => x.AssetPMs)
                .ToList();

        }
        public List<WOInspectionsTemplateFormIOAssignment> GetWOCategoriesByWOid(Guid wo_id)
        {
            return context.WOInspectionsTemplateFormIOAssignment.Where(x => x.wo_id == wo_id && !x.is_archived).ToList();
        }
        public List<InspectionTemplateFormIoExclude> GetMasterFormExcluded(List<Guid> form_ids)
        {
            IQueryable<InspectionsTemplateFormIO> query = context.InspectionsTemplateFormIO.Where(x => form_ids.Contains(x.form_id));
            var response = query
                .Select(x => new InspectionTemplateFormIoExclude
                {
                    form_id = x.form_id,
                    form_type_id = x.form_type_id.Value,
                    form_type_name = x.FormIOType.form_type_name
                })
                .ToList();
            return response;
        }
        public List<WOcategorytoTaskMapping> GetWOCategorytaskByWOid(Guid wo_id)
        {
            return context.WOcategorytoTaskMapping.Where(x => x.wo_id == wo_id && !x.is_archived)
                .Include(x => x._assigned_asset)
                 .ThenInclude(x => x.AssetFormIOBuildingMappings)
                .ToList();
        }

        public List<WOOnboardingAssets> GetOBAssetForMWO(Guid wo_id)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && !x.is_deleted && !x.is_woline_from_other_inspection)
                                             .Include(x => x.Asset).ThenInclude(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.FormIOType)
                                             .Include(x => x.Asset).ThenInclude(x => x.AssetTopLevelcomponentMapping)
                                             .Include(x => x.StatusMaster)
                                             .Include(x => x.TempAsset).ThenInclude(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.FormIOType)
                                             .Include(x => x.TempAsset).ThenInclude(x => x.TempFormIOBuildings)
                                             .Include(x => x.TempAsset).ThenInclude(x => x.TempFormIOFloors)
                                             .Include(x => x.TempAsset).ThenInclude(x => x.TempFormIORooms)
                                             .Include(x => x.TempAsset).ThenInclude(x => x.TempFormIOSections)
                                             .Include(x => x.TempAsset.TempMasterBuilding)
                                             .Include(x => x.TempAsset.TempMasterFloor)
                                             .Include(x => x.TempAsset.TempMasterRoom)
                                             .Include(x => x.ActiveAssetPMWOlineMapping.AssetPMs)
                                             .Include(x => x.TempActiveAssetPMWOlineMapping.TempAssetPMs.PMs)
                                             .Include(x => x.WOlineTopLevelcomponentMapping)
                                             .Include(x => x.Sites)
                                             .Include(x => x.WOLineIssue)
                                             //.Include(x=>x.WOOBAssetTempFormIOBuildingMapping).ThenInclude(x=>x.TempFormIOBuildings)
                                             //.Include(x=>x.WOOBAssetTempFormIOBuildingMapping).ThenInclude(x=>x.TempFormIOFloors)
                                             //.Include(x=>x.WOOBAssetTempFormIOBuildingMapping).ThenInclude(x=>x.TempFormIORooms)
                                             //.Include(x=>x.WOOBAssetTempFormIOBuildingMapping).ThenInclude(x=>x.TempFormIOSections)
                                             .ToList();
        }

        public Asset GetAssetByQRCode(List<string> qr_code)
        {
            qr_code = qr_code.ConvertAll(x => x.ToLower().Trim());
            return context.Assets.Where(x => qr_code.Contains(x.QR_code.ToLower().Trim()) && x.status == (int)Status.AssetActive && x.company_id == UpdatedGenericRequestmodel.CurrentUser.company_id && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).FirstOrDefault();
        }
        public Asset GetAssetByQRCodeForOBWO(string qr_code,Guid? asset_id)
        {
            return context.Assets.Where(x => x.QR_code.ToLower().Trim() == qr_code && x.status == (int)Status.AssetActive&& x.asset_id!= asset_id && x.company_id == UpdatedGenericRequestmodel.CurrentUser.company_id && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                .FirstOrDefault();
        }
        public Asset GetAssetByQRCodeExist(List<string> qr_code, List<Guid> existing_asset_id)
        {
            qr_code = qr_code.ConvertAll(x => x.ToLower().Trim());
            return context.Assets.Where(x => qr_code.Contains(x.QR_code.ToLower().Trim()) && !existing_asset_id.Contains(x.asset_id) && x.status == (int)Status.AssetActive && x.company_id == UpdatedGenericRequestmodel.CurrentUser.company_id && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).FirstOrDefault();
        }
        public WOOnboardingAssets GetFedByOBAssetByID(Guid asset_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == asset_id)

                .FirstOrDefault();
        }


        public InspectionTemplateAssetClass GetAssetClassById(Guid inspectiontemplate_asset_class_id)
        {
            return context.InspectionTemplateAssetClass.Where(x => x.inspectiontemplate_asset_class_id == inspectiontemplate_asset_class_id).FirstOrDefault();
        }
        public List<WOOnboardingAssets> GetOBWOAssetsByWOid(GetOBFedByAssetListRequestmodel requestmodel)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == requestmodel.wo_id && x.woonboardingassets_id != requestmodel.woonboardingassets_id && !x.is_deleted)
                .OrderBy(x => x.asset_name)
                .Include(x => x.StatusMaster)
                .Include(x => x.TempAsset.InspectionTemplateAssetClass)
                .ToList();
        }
        public List<WOOnboardingAssets> GetOBWOAssetsByWOidForMWO(GetAssetsToAssigninMWOInspectionRequestmodel requestmodel)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == requestmodel.wo_id && !x.is_deleted && x.inspection_type == (int)MWO_inspection_wo_type.OnBoarding)
                .OrderBy(x => x.asset_name)
                .Include(x => x.StatusMaster)
                .ToList();
        }
        public Asset GetAsssetByInternalID(string internal_id, Guid site_id)
        {
            return context.Assets.Where(x => x.internal_asset_id.ToLower().Trim() == internal_id && x.status == (int)Status.AssetActive
                                          && x.site_id == site_id)
                .Include(x => x.AssetTopLevelcomponentMapping)
                .Include(x => x.AssetSubLevelcomponentMapping)
                .Include(x => x.AssetProfileImages)
                .FirstOrDefault();
        }
        public Asset GetAssetByFedBy(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id && x.status == (int)Status.AssetActive && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public Asset GetAssetByAssetIdActive(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id && x.status == (int)Status.AssetActive).FirstOrDefault();
        }
        public List<Asset> GetAssetsForHierarchy()
        {
            List<string> company_ids = new List<string>
            {
                "915eda95-162f-40b7-94df-623961b3f1dc",
"4122f9d4-5d95-4616-9b2b-1f4d72116cc6",
"22a8170a-f97c-432b-976a-28c4d7abf3ca",
"47aa8c4d-684a-4da2-9aad-cfbb851d3f6d",
"2ec5c208-dfcd-4bfa-b6a9-babbd4a0e19f",
"375afa96-cf69-4649-84f1-a7df353102fe",
"f8f8ccf7-44d8-4a24-84cc-3de435638795",
"34aa5c4d-684a-4da2-9aad-cfbb851d3f6d"
            };
            return context.Assets.Where(x => x.status == (int)Status.AssetActive && company_ids.Contains(x.company_id))
                .Include(x => x.AssetParentHierarchyMapping)
                .Include(x => x.AssetChildrenHierarchyMapping)
                .ToList();
        }
        public List<Asset> GetAssetsByInternalid(List<string> internal_id, Guid site_id)
        {
            var internal_id1 = internal_id.ConvertAll(x => x.ToLower());
            return context.Assets.Where(x => x.site_id == site_id && internal_id1.Contains(x.internal_asset_id.ToLower().Trim()))
                .Include(x => x.AssetParentHierarchyMapping)
                .Include(x => x.AssetChildrenHierarchyMapping)
                .ToList();
        }
        public List<Asset> GetAssetsByFedByIDs(List<Guid> asset_ids)
        {
            return context.Assets.Where(x => asset_ids.Contains(x.asset_id) && x.status == (int)Status.AssetActive && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).ToList();
        }
        public WOOBAssetFedByMapping GetWOLineByFedby(Guid woonboardingassets_id)
        {
            return context.WOOBAssetFedByMapping.Where(x => x.parent_asset_id == woonboardingassets_id && !x.is_deleted).FirstOrDefault();
        }
        public WOlineSubLevelcomponentMapping Getsublevelcomponenttocheck(Guid woonboardingassets_id)
        {
            return context.WOlineSubLevelcomponentMapping.Where(x => x.sublevelcomponent_asset_id == woonboardingassets_id && !x.is_deleted).FirstOrDefault();
        }
        public WOlineTopLevelcomponentMapping Gettoplevelcomponenttocheck(Guid woonboardingassets_id)
        {
            return context.WOlineTopLevelcomponentMapping.Where(x => x.toplevelcomponent_asset_id == woonboardingassets_id && !x.is_deleted).FirstOrDefault();
        }
        public IRScanWOImageFileMapping GetIRScanImageFileByNAme(string img_file_name, string wo_id)
        {
            return context.IRScanWOImageFileMapping.Where(x => x.img_file_name == img_file_name && x.wo_id == Guid.Parse(wo_id) && !x.is_deleted).FirstOrDefault();
        }
        public List<IRWOImagesLabelMapping> GetIRImageLabelsByWOID(string wo_id)
        {
            return context.IRWOImagesLabelMapping.Where(x => x.WOOnboardingAssets.wo_id == Guid.Parse(wo_id) && !x.is_deleted)
                                                 .Include(x => x.WOOnboardingAssets)
                                                 .ToList();
        }

        public List<IRScanWOImageFileMapping> GetImageNameMappingByID(Guid wo_id)
        {
            return context.IRScanWOImageFileMapping.Where(x => x.wo_id == wo_id && !x.is_deleted).ToList();
        }
        public bool IsOtherInspectionTypeINWO(Guid wo_id)
        {
            var ob_asset = context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && !x.is_deleted && x.status == (int)Status.Completed).ToList();
            if (ob_asset != null && ob_asset.Count > 0)
            {
                return true;
            }
            return false;
        }
        public FormIOAuthToken GetFormIOToken()
        {
            return context.FormIOAuthToken.FirstOrDefault();
        }
        public WOOnboardingAssets GetAssetFromWOline(string woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == Guid.Parse(woonboardingassets_id)).FirstOrDefault();
        }
        public WOOnboardingAssets GetwooblineforMWObyid(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id).FirstOrDefault();

        }
        public AssetFormIO GetLastInspectionByAssetID(Guid asset_id)
        {
            return context.AssetFormIO.Where(x => x.asset_id == asset_id && x.status == (int)Status.Completed).OrderByDescending(x => x.modified_at).FirstOrDefault();
        }
        public AssetFormIO GetAssetformdataByID(Guid asset_form_id)
        {
            return context.AssetFormIO.Where(x => x.asset_form_id == asset_form_id).FirstOrDefault();
        }
        public List<WOInspectionsTemplateFormIOAssignment> GetWOCategoryByIds(List<Guid> wo_inspectionsTemplateFormIOAssignment_id)
        {
            return context.WOInspectionsTemplateFormIOAssignment.Where(x => wo_inspectionsTemplateFormIOAssignment_id.Contains(x.wo_inspectionsTemplateFormIOAssignment_id)).ToList();
        }
        public WorkOrders GetWObyIdforIRlambdareport(Guid wo_id)
        {
            return context.WorkOrders.Where(x => x.wo_id == wo_id).FirstOrDefault();
        }
        public WOLineIssue GetWOLineIssueById(Guid wo_line_issue_id)
        {
            return context.WOLineIssue.Where(x => x.wo_line_issue_id == wo_line_issue_id)
                .Include(x => x.WOlineIssueImagesMapping)
                .FirstOrDefault();
        }
        public WOOnboardingAssets GetWOlineByOBAssetId(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Include(x => x.WorkOrders)
                .Include(x => x.WOLineIssue)
                .Include(x => x.AssetPMs)
                .Include(x => x.ActiveAssetPMWOlineMapping)
                .Include(x => x.TempAsset)
                .Include(x => x.Asset)
                .FirstOrDefault();
        }
        public WOOnboardingAssets GetWOlineByIdForLinkIssue(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Include(x => x.WOOnboardingAssetsImagesMapping)
                .FirstOrDefault();
        }
        public (List<WOLineIssue>, int) GetAllWOLineTempIssues(GetAllWOLineTempIssuesRequestmodel requestmodel)
        {
            IQueryable<WOLineIssue> query = context.WOLineIssue.Where(x => !x.is_deleted)
                .Include(x => x.Asset)
                ;

            if (requestmodel.wo_id != null && requestmodel.wo_id != Guid.Empty)
            {
                query = query.Where(x => x.wo_id == requestmodel.wo_id);
            }
            else if (requestmodel.asset_form_id != null && requestmodel.asset_form_id != Guid.Empty)
            {
                query = query.Where(x => x.asset_form_id == requestmodel.asset_form_id);
            }
            else if (requestmodel.woonboardingassets_id != null && requestmodel.woonboardingassets_id != Guid.Empty)
            {
                query = query.Where(x => x.woonboardingassets_id == requestmodel.woonboardingassets_id);
            }

            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.ToLower().Trim();
                query = query.Where(x =>
                x.issue_title.ToLower().Trim().Contains(search)
              || x.issue_description.ToLower().Trim().Contains(search)
              );
            }
            int total_list_count = query.Count();
            if (requestmodel.page_size != 0 && requestmodel.page_index != 0)
            {
                query = query.Skip((requestmodel.page_index - 1) * requestmodel.page_size).Take(requestmodel.page_size);
            }
            query = query
                        .Include(x => x.StatusMaster);

            return (query.ToList(), total_list_count);
        }
        public List<WOLineIssue> GetAlltempIssuebyWOid(GetAllIssueByWOidRequestmodel requestmodel)
        {
            IQueryable<WOLineIssue> query = context.WOLineIssue.Where(x => !x.is_deleted && x.wo_id == requestmodel.wo_id)
                .Include(x => x.Asset)
                .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.TempAsset)
                ;
            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.ToLower().Trim();
                query = query.Where(x =>
                x.issue_title.ToLower().Trim().Contains(search)
              || x.issue_description.ToLower().Trim().Contains(search)
              || (x.Asset != null ? x.Asset.name.ToLower().Trim().Contains(search) : x.form_retrived_asset_name.ToLower().Trim().Contains(search))
              || (x.WorkOrders != null ? x.WorkOrders.manual_wo_number.ToLower().Trim().Contains(search) : x.issue_title.ToLower().Trim().Contains(search))
              );
            }

            return query.ToList();
        }
        public List<AssetIssue> GetAllmainIssuebyWOid(GetAllIssueByWOidRequestmodel requestmodel)
        {
            IQueryable<AssetIssue> query = context.AssetIssue.Where(x => x.wo_id == requestmodel.wo_id && !x.is_deleted)
                                         .Include(x => x.WorkOrders)
                                         .Include(x => x.WOLineIssue)
                                         .Include(x => x.WOOnboardingAssets.TempAsset)
                                         .Include(x => x.Asset);
            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.ToLower().Trim();
                query = query.Where(x => x.issue_title.ToLower().Trim().Contains(search)
                         || x.Asset.name.ToLower().Trim().Contains(search)
                         || x.WorkOrders.manual_wo_number.ToLower().Trim().Contains(search)
                );
            }
            if (requestmodel.wo_id != null)
            {
                query = query.Where(x => x.WOLineIssue.original_wo_id != requestmodel.wo_id);
            }
            return query.ToList();
        }
        public (List<AssetIssue>, int) GetAllAssetIssues(GetAllAssetIssuesRequestmodel requestmodel)
        {
            IQueryable<AssetIssue> query = context.AssetIssue.Where(x => !x.is_deleted
            && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
            )
            .Include(x => x.Asset).ThenInclude(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.FormIOType)
            .Include(x => x.WorkOrders);

            if (requestmodel.status != null && requestmodel.status.Count > 0)
            {
                query = query.Where(x => requestmodel.status.Contains(x.issue_status.Value));
            }
            if (requestmodel.asset_id != null && requestmodel.asset_id != Guid.Empty)
            {
                query = query.Where(x => x.asset_id == requestmodel.asset_id);
            }
            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.ToLower().Trim();
                query = query.Where(x =>
                x.issue_title.ToLower().Trim().Contains(search)
              || x.issue_description.ToLower().Trim().Contains(search)
              || x.Asset.name.ToLower().Trim().Contains(search)
              || x.WorkOrders.manual_wo_number.ToLower().Trim().Contains(search)
              || x.issue_number.ToLower().Trim().Contains(search)
              );
            }
            query = query.OrderByDescending(x => x.created_at);
            int total_list_count = query.Count();
            if (requestmodel.page_size != 0 && requestmodel.page_index != 0)
            {
                query = query.Skip((requestmodel.page_index - 1) * requestmodel.page_size).Take(requestmodel.page_size);
            }
            query = query

                         .Include(x => x.StatusMaster)
                         .Include(x => x.WOLineIssue)
                         .Include(x => x.AssetIssueImagesMapping)
                         ;
            return (query.ToList(), total_list_count);
        }
        public (List<AssetIssueComments>, int) GetAllAssetIssueComments(GetAllAssetIssueCommentsRequestmodel requestmodel)
        {
            IQueryable<AssetIssueComments> query = context.AssetIssueComments.Where(x => !x.is_deleted);

            if (requestmodel.asset_issue_id != null && requestmodel.asset_issue_id != Guid.Empty)
            {
                query = query.Where(x => x.asset_issue_id == requestmodel.asset_issue_id);
            }

            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.ToLower().Trim();
                query = query.Where(x =>
                x.comment.ToLower().Trim().Contains(search)
              );
            }
            int total_list_count = query.Count();
            if (requestmodel.page_size != 0 && requestmodel.page_index != 0)
            {
                query = query.Skip((requestmodel.page_index - 1) * requestmodel.page_size).Take(requestmodel.page_size);
            }
            query = query
                         .Include(x => x.User)
                         .Include(x => x.Roles)
                         ;
            return (query.ToList(), total_list_count);
        }
        public AssetIssueComments GetIssueCommentById(Guid asset_issue_comments_id)
        {
            return context.AssetIssueComments.Where(x => x.asset_issue_comments_id == asset_issue_comments_id).FirstOrDefault();
        }

        public AssetIssue GetAssetIssueById(Guid asset_issue_id)
        {
            return context.AssetIssue.Where(x => x.asset_issue_id == asset_issue_id)
                .Include(x => x.AssetIssueImagesMapping)
                .FirstOrDefault();
        }
        public List<AssetIssue> GetAssetIssueByMultiId(List<Guid> asset_issue_id)
        {
            return context.AssetIssue.Where(x => asset_issue_id.Contains(x.asset_issue_id))
                .Include(x => x.AssetIssueImagesMapping)
                .ToList();
        }
        public AssetIssue ViewAssetIssueDetailsById(Guid asset_issue_id)
        {
            return context.AssetIssue.Where(x => x.asset_issue_id == asset_issue_id)
                .Include(x => x.WorkOrders)
                .Include(x => x.AssetIssueImagesMapping)
                .Include(x => x.Asset)
                .Include(x => x.WOLineIssue)
                .Include(x => x.Sites).ThenInclude(x => x.ClientCompany)
                .FirstOrDefault();
        }
        public List<AssetIssue> GetMainIssueByAssetId(Guid asset_id)
        {
            return context.AssetIssue.Where(x => x.asset_id == asset_id && x.issue_status != (int)Status.Completed && !x.is_issue_linked && !x.is_deleted)
                .Include(x => x.WorkOrders)
                .Include(x => x.Asset)
                .ToList();
        }
        public List<WOLineIssue> GetIssuesIssueByAssetId(IssueListtoLinkWOlineRequestmodel requestmodel)
        {

            IQueryable<WOLineIssue> query = context.WOLineIssue.Where(x => !x.is_main_issue_created && !x.is_deleted && !x.is_issue_linked_for_fix);
            if (requestmodel.asset_id != null)
            {
                query = query.Where(x => x.original_asset_id == requestmodel.asset_id || (x.asset_id == null && x.woonboardingassets_id == null));
            }
            if (requestmodel.issues_temp_asset_id != null)
            {
                query = query.Where(x => x.original_woonboardingassets_id == requestmodel.issues_temp_asset_id || (x.original_woonboardingassets_id == null && x.woonboardingassets_id == null));
            }
            query = query.Where(x => x.wo_id == requestmodel.wo_id);

            return query
                .Include(x => x.WorkOrders)
                .ToList();
        }
        public List<WOLineIssue> GetFloatingTempIssueByAssetId(IssueListtoLinkWOlineRequestmodel requestmodel, List<Guid> already_got_temp_issues)
        {

            IQueryable<WOLineIssue> query = context.WOLineIssue.Where(x => !x.is_main_issue_created && !x.is_deleted && !x.is_issue_linked_for_fix
            && !already_got_temp_issues.Contains(x.wo_line_issue_id)
            );

            query = query.Where(x => x.wo_id == requestmodel.wo_id);

            return query
                .Include(x => x.WorkOrders)
                .ToList();
        }

        public List<AssetIssue> GetMainIssueByAssetformid(GetWOLinkedIssueRequestmodel requestmodel)
        {
            IQueryable<AssetIssue> query = context.AssetIssue.Where(x => x.asset_form_id == requestmodel.asset_form_id && !x.is_deleted)
                                            .Include(x => x.WorkOrders)
                                            .Include(x => x.Asset);
            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.ToLower().Trim();
                query = query.Where(x => x.issue_title.ToLower().Trim().Contains(search)
                         || x.Asset.name.ToLower().Trim().Contains(search)
                         || x.WorkOrders.manual_wo_number.ToLower().Trim().Contains(search)
                );
            }
            return query.ToList();
        }
        public List<WOLineIssue> GetTempIssueByAssetformid(GetWOLinkedIssueRequestmodel requestmodel)
        {
            IQueryable<WOLineIssue> query = context.WOLineIssue.Where(x => x.asset_form_id == requestmodel.asset_form_id && !x.is_main_issue_created && x.is_issue_linked_for_fix && !x.is_deleted)
                                                .Include(x => x.WorkOrders)
                                                .Include(x => x.Asset);
            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.ToLower().Trim();
                query = query.Where(x => x.issue_title.ToLower().Trim().Contains(search)
                         || x.Asset.name.ToLower().Trim().Contains(search)
                         || x.WorkOrders.manual_wo_number.ToLower().Trim().Contains(search)
                );
            }
            return query.ToList();
            /*return context.WOLineIssue.Where(x => x.asset_form_id
            == asset_form_id && !x.is_main_issue_created && x.is_issue_linked_for_fix && !x.is_deleted)
                .Include(x => x.WorkOrders)
                .Include(x => x.Asset)
                .ToList();*/
        }


        public List<AssetIssue> GetMainIssueBywoobassetid(GetWOLinkedIssueRequestmodel requestmodel)
        {
            IQueryable<AssetIssue> query = context.AssetIssue.Where(x => x.woonboardingassets_id == requestmodel.woonboardingassets_id && !x.is_deleted)
                                           .Include(x => x.WorkOrders)
                                           .Include(x => x.WOOnboardingAssets.TempAsset)
                                           .Include(x => x.Asset);
            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.ToLower().Trim();
                query = query.Where(x => x.issue_title.ToLower().Trim().Contains(search)
                         || x.Asset.name.ToLower().Trim().Contains(search)
                         || x.WorkOrders.manual_wo_number.ToLower().Trim().Contains(search)
                );
            }
            return query.ToList();

            /*return context.AssetIssue.Where(x => x.woonboardingassets_id == woonboardingassets_id && !x.is_deleted)
                .Include(x => x.WorkOrders)
                .Include(x => x.Asset)
                .ToList();*/
        }
        public List<WOLineIssue> GetTempIssueBywoobassetid(GetWOLinkedIssueRequestmodel requestmodel)
        {
            IQueryable<WOLineIssue> query = context.WOLineIssue.Where(x => x.woonboardingassets_id == requestmodel.woonboardingassets_id && !x.is_main_issue_created && !x.is_deleted)
                                               .Include(x => x.WorkOrders)
                                               .Include(x => x.Asset);

            /*if (!requestmodel.is_request_from_issue_service)
            {
                query = query.Where(x => x.is_issue_linked_for_fix);
            }*/
            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.ToLower().Trim();
                query = query.Where(x => x.issue_title.ToLower().Trim().Contains(search)
                         || x.Asset.name.ToLower().Trim().Contains(search)
                         || x.WorkOrders.manual_wo_number.ToLower().Trim().Contains(search)
                );
            }
            return query.ToList();


            /*return context.WOLineIssue.Where(x => x.woonboardingassets_id == woonboardingassets_id && !x.is_main_issue_created && x.is_issue_linked_for_fix && !x.is_deleted)
                .Include(x => x.WorkOrders)
                .Include(x => x.Asset)
                .ToList();*/
        }


        public List<WOLineIssue> GetTempIssueByMultiId(List<Guid> wo_line_issue_id)
        {
            return context.WOLineIssue.Where(x => wo_line_issue_id.Contains(x.wo_line_issue_id))
                .Include(x => x.WorkOrders)
                .Include(x => x.WOlineIssueImagesMapping)
                .ToList();
        }
        public AssetIssue GetAssetIssueByIdForAssetupdate(Guid asset_issue_id)
        {
            return context.AssetIssue.Where(x => x.asset_issue_id == asset_issue_id)
                .Include(x => x.WorkOrders)
                .Include(x => x.AssetFormIO)
                .Include(x => x.WOOnboardingAssets)
                .Include(x => x.Asset)
                  .ThenInclude(x => x.AssetIssue)
                .FirstOrDefault();
        }
        public List<Guid> GetAssetformIdsbyWO(Guid wo_id)
        {
            return context.AssetFormIO.Where(x => x.wo_id == wo_id && (x.status == (int)Status.Completed || x.status == (int)Status.Submitted))
                .Select(x => x.asset_form_id)
                .ToList();
        }
        public List<Asset> GetAssetwhichhavenoIssue(Guid wo_id)
        {
            return context.AssetFormIO
                .Include(x => x.AssetIssue)
                .Include(x => x.Asset).ThenInclude(x => x.AssetIssue)
                .Where(x => x.wo_id == wo_id && (x.status == (int)Status.Completed || x.status == (int)Status.Submitted) && (x.AssetIssue.Where(q => !q.is_deleted).Count() == 0))
                .Select(x => x.Asset)
                .ToList();
        }
        public List<Asset> GetWOlineAssetwhichhavenoIssue(Guid wo_id)
        {
            return context.WOOnboardingAssets
                .Include(x => x.AssetIssue)
                .Include(x => x.Asset).ThenInclude(x => x.AssetIssue)
                .Where(x => x.wo_id == wo_id && (x.status == (int)Status.Completed || x.status == (int)Status.Submitted) && (x.AssetIssue.Where(q => !q.is_deleted).Count() == 0))
                .Select(x => x.Asset)
                .ToList();
        }
        public List<Guid> GetWOObWOlineIds(Guid wo_id)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && (x.status == (int)Status.Completed || x.status == (int)Status.Submitted))
                .Select(x => x.woonboardingassets_id)
                .ToList();
        }
        public List<AssetIssue> GetAssetIssuebyAssetformId(List<Guid> asset_form_ids)
        {
            return context.AssetIssue.Where(x => asset_form_ids.Contains(x.asset_form_id.Value) && x.issue_status != (int)Status.Completed && !x.is_deleted).ToList();
        }
        public List<AssetIssue> GetAssetIssuebyOBWOline(List<Guid> woonboardingassets_id)
        {
            return context.AssetIssue.Where(x => woonboardingassets_id.Contains(x.woonboardingassets_id.Value)
            //&& x.issue_status != (int)Status.Completed 
            && !x.is_deleted)
                .Include(x => x.AssetIssueImagesMapping)
                .Include(x => x.WOOnboardingAssets.WOOnboardingAssetsImagesMapping)
                .ToList();
        }
        public List<AssetIssue> GetAssetIssuebyIds(List<Guid> asset_issue_id)
        {
            return context.AssetIssue.Where(x => asset_issue_id.Contains(x.asset_issue_id) && x.issue_status != (int)Status.Completed)
                .Include(x => x.AssetIssueImagesMapping)
                .ToList();
        }
        public List<WOLineIssue> GetWOLineIssuebyIds(List<Guid> wo_line_issue_id)
        {
            return context.WOLineIssue.Where(x => wo_line_issue_id.Contains(x.wo_line_issue_id)).ToList();
        }
        // return those issue which wo line is not completed 
        public List<AssetIssue> GetIssueByWOidtoUnlink(Guid wo_id)
        {
            return context.AssetIssue.Where(x => x.wo_id == wo_id
                                              && ((x.AssetFormIO != null && x.AssetFormIO.status != (int)Status.Completed && x.AssetFormIO.status != (int)Status.Submitted) || (x.WOOnboardingAssets != null && x.WOOnboardingAssets.status != (int)Status.Completed))
                                              && x.is_issue_linked).ToList();
        }
        public List<AssetIssue> GetnotCompletedIssueByWOidtoUnlink(Guid wo_id)
        {
            return context.AssetIssue.Where(x => x.wo_id == wo_id
                                              && x.issue_status != (int)Status.Completed
                                              && x.is_issue_linked).ToList();
        }
        public List<AssetPMs> GetAssetpmByWOidtoUnlink(Guid wo_id)
        {
            return context.AssetPMs.Where(x => x.wo_id == wo_id
                                              && ((x.AssetFormIO != null && x.AssetFormIO.status != (int)Status.Completed && x.AssetFormIO.status != (int)Status.Submitted)
                                              || ((x.WOOnboardingAssets != null && x.WOOnboardingAssets.status != (int)Status.Completed))
                                              )
                                              ).ToList();
        }
        public List<AssetPMs> GetAssetpmByWOidtoUnlinkUpdated(Guid wo_id)
        {
            return context.ActiveAssetPMWOlineMapping.Include(x => x.AssetPMs.ActiveAssetPMWOlineMapping).Where(x => x.WOOnboardingAssets.wo_id == wo_id
                        && x.is_active && !x.is_deleted && x.WOOnboardingAssets.status != (int)Status.Completed
                        ).Select(x => x.AssetPMs).ToList();


            // return context.AssetPMs.Where(x => x.ActiveAssetPMWOlineMapping == wo_id
            //                                   && ((x.AssetFormIO != null && x.AssetFormIO.status != (int)Status.Completed && x.AssetFormIO.status != (int)Status.Submitted)
            //                                   || ((x.WOOnboardingAssets != null && x.WOOnboardingAssets.status != (int)Status.Completed))
            //                                   )
            //                                   ).ToList();
        }
        public List<AssetPMs> GetAssetPMsbyIds(List<Guid> asset_pm_id)
        {
            return context.AssetPMs.Where(x => asset_pm_id.Contains(x.asset_pm_id))
                .Include(x => x.Asset).ThenInclude(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.AssetClassFormIOMapping)
                .ToList();
        }
        public AssetPMs GetAssetPMsbyId(Guid asset_pm_id)
        {
            return context.AssetPMs.Where(x => x.asset_pm_id == asset_pm_id)
                .Include(x => x.Asset)
                   .ThenInclude(x => x.InspectionTemplateAssetClass)
                .Include(x => x.WOOnboardingAssets)
                .ThenInclude(x => x.IRWOImagesLabelMapping)
                .Include(x => x.ActiveAssetPMWOlineMapping)
                 .ThenInclude(x => x.WOlineAssetPMImagesMapping)
                 .Include(x => x.WorkOrders)
                .FirstOrDefault();
        }
        public TempAssetPMs GetTempAssetPMsbyIdForsubmit(Guid temp_asset_pm_id)
        {
            return context.TempAssetPMs.Where(x => x.temp_asset_pm_id == temp_asset_pm_id)
                .Include(x => x.TempActiveAssetPMWOlineMapping)
                 .ThenInclude(x => x.WOOnboardingAssets)
                 .ThenInclude(x => x.IRWOImagesLabelMapping)
                 .Include(x => x.TempActiveAssetPMWOlineMapping)
                  .ThenInclude(x => x.WOlineAssetPMImagesMapping)
                .FirstOrDefault();
        }
        public TempAssetPMs GetTempAssetPMsbyId(Guid temp_asset_pm_id)
        {
            return context.TempAssetPMs.Where(x => x.temp_asset_pm_id == temp_asset_pm_id)
                .Include(x => x.TempActiveAssetPMWOlineMapping)
                .Include(x => x.WOOnboardingAssets)
                .FirstOrDefault();
        }
        public TempAssetPMs GetTempAssetPMsbyIdOfflineUpdate(Guid temp_asset_pm_id)
        {
            return context.TempAssetPMs.Where(x => x.temp_asset_pm_id == temp_asset_pm_id)
                .FirstOrDefault();
        }
        public TempActiveAssetPMWOlineMapping GetTempActiveAssetPMwolinemappingbyIdOfflineUpdate(Guid temp_active_asset_pm_woline_mapping_id)
        {
            return context.TempActiveAssetPMWOlineMapping.Where(x => x.temp_active_asset_pm_woline_mapping_id == temp_active_asset_pm_woline_mapping_id).FirstOrDefault();
        }

        public PMs GetPMById(Guid pm_id)
        {
            return context.PMs.Where(x => x.pm_id == pm_id)
                .Include(x => x.PMAttachments)
                .Include(x => x.PMsTriggerConditionMapping)
                .FirstOrDefault();
        }
        public List<FormIOBuildings> GetLocationHierarchyForWO(GetLocationHierarchyForWORequestmodel request)
        {
            IQueryable<FormIOBuildings> query = context.FormIOBuildings.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id))
                                           .Include(x => x.FormIOFloors).ThenInclude(x => x.FormIORooms).OrderBy(x => x.formio_building_name);
            if (!String.IsNullOrEmpty(request.search_string))
            {
                string search = request.search_string.ToLower().Trim();
                query = query.Where(x =>
                  x.formio_building_name.ToLower().Trim().Contains(search) ||
                  x.FormIOFloors.Select(x => x.formio_floor_name.ToLower().Trim()).Contains(search) ||
                  x.FormIOFloors.SelectMany(x => x.FormIORooms).Select(x => x.formio_room_name.ToLower().Trim()).Contains(search)
                );
            }
            return query.ToList();
        }


        public List<WOOnboardingAssets> GetWOlinesByLocation(GetWOlinesByLocationRequestmodel requestmodel)
        {
            IQueryable<WOLineBuildingMapping> query = context.WOLineBuildingMapping
                                                             .Include(x => x.WOOnboardingAssets)
                                                             .Include(x => x.WOOnboardingAssets.Asset)
                                                             .Include(x => x.WOOnboardingAssets.StatusMaster)
                                                             .Where(x => !x.WOOnboardingAssets.is_deleted && x.WOOnboardingAssets.wo_id == requestmodel.wo_id);

            query = query.Where(x => x.formiobuilding_id == requestmodel.formiobuilding_id &&
                                    x.formiofloor_id == requestmodel.formiofloor_id &&
                                    x.formioroom_id == requestmodel.formioroom_id);

            return query.Select(x => x.WOOnboardingAssets).ToList();
        }
        public List<AssetFormIO> GetOpenAssetformio(Guid wo_d)
        {
            return context.AssetFormIO.Where(x => x.wo_id == wo_d && x.status == (int)Status.open).Include(x => x.WOcategorytoTaskMapping).ToList();
        }
        public List<WOOnboardingAssets> GetOpenwoobline(Guid wo_d)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == wo_d && x.status == (int)Status.open
            && !x.is_woline_from_other_inspection
            )
                .Include(x => x.WOLineIssue)
                .Include(x => x.AssetIssue)
                .Include(x => x.AssetPMs.ActiveAssetPMWOlineMapping)
                .Include(x => x.TempActiveAssetPMWOlineMapping)
                .ToList();
        }
        public List<WOOnboardingAssets> ScriptforWOlinelocation()
        {
            return context
                .WOOnboardingAssets
                .Include(x => x.WOLineBuildingMapping)
                .Include(x => x.WorkOrders)
                .Include(x => x.Sites)
                .Where(x => x.WOLineBuildingMapping == null && !x.is_deleted &&
                (x.WorkOrders.wo_type == (int)Status.Onboarding_WO || x.WorkOrders.wo_type == (int)Status.IR_Scan_WO)
                )
                .ToList();
        }
        public List<InspectionTemplateAssetClass> Scriptforformandclass()
        {
            return context.InspectionTemplateAssetClass.Where(x => x.company_id == Guid.Parse("0b2b3b98-f141-40f1-88b9-fa8de7224c0f") && !x.isarchive
            // && x.inspectiontemplate_asset_class_id== Guid.Parse("6c58b3e3-d7ad-47c7-a2fc-dfafef1f1f51")
            )
                .Include(x => x.AssetClassFormIOMapping)
                //.ThenInclude(x=>x.InspectionsTemplateFormIO)
                .ToList();
        }
        public List<InspectionsTemplateFormIO> Scriptforformioform()
        {
            return context.InspectionsTemplateFormIO.Where(x => x.company_id == Guid.Parse("4122f9d4-5d95-4616-9b2b-1f4d72116cc6") && x.status == 1
            // && x.inspectiontemplate_asset_class_id== Guid.Parse("6c58b3e3-d7ad-47c7-a2fc-dfafef1f1f51")
            )
                //.ThenInclude(x=>x.InspectionsTemplateFormIO)
                .ToList();
        }
        public string Scriptforformandclass_get_WPnyid(Guid form_id)
        {
            return context.InspectionsTemplateFormIO.Where(x => x.form_id == form_id).Select(x => x.work_procedure).FirstOrDefault();
        }
        public Guid Scriptforformandclass_get_idbyWP(string WP)
        {
            return context.InspectionsTemplateFormIO.Where(x => x.work_procedure == WP && x.company_id == Guid.Parse("0b2b3b98-f141-40f1-88b9-fa8de7224c0f") && x.status != 2).Select(x => x.form_id).FirstOrDefault();
        }
        public List<Guid> ExistingWolineForLinkPM(List<Guid> asset_id, Guid wo_id)
        {
            return context.AssetFormIO.Where(x =>
                                                x.asset_id != null
                                            && x.wo_id == wo_id
                                            && asset_id.Contains(x.asset_id.Value)
                                            && x.status != (int)Status.Deactive).Select(x => x.asset_id.Value).ToList();
        }
        public Guid GetAssetformIOtoLinkPM(Guid asset_id, Guid wo_id)
        {
            return context.AssetFormIO.Where(x =>
                                                x.asset_id != null
                                            && x.wo_id == wo_id
                                            && x.asset_id.Value == asset_id
                                            && x.status != (int)Status.Deactive).Select(x => x.asset_form_id).FirstOrDefault();
        }
        public WOOnboardingAssets GetOBWOlineByQRCode(GetOBWOlineByQRCodeRequestmodel requestmodel)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == requestmodel.wo_id && !x.is_deleted
            && x.QR_code.Contains(requestmodel.search_string)
            )
             .Include(x => x.Sites)
                          .Include(x => x.WOOnboardingAssetsImagesMapping)
                          .Include(x => x.IRWOImagesLabelMapping)
                          .Include(x => x.StatusMaster)
                          .Include(x => x.Asset)
                           .ThenInclude(x => x.InspectionTemplateAssetClass)
                          .Include(x => x.WOOBAssetFedByMapping)
                          .Include(x => x.AssetIssue)
                          .Include(x => x.WOLineIssue).ThenInclude(x => x.WOlineIssueImagesMapping)
                          .Include(x => x.WorkOrders)
                          .Include(x => x.WOLineBuildingMapping)
                          .Include(x => x.WOlineTopLevelcomponentMapping)
                          .Include(x => x.WOlineSubLevelcomponentMapping).ThenInclude(x => x.WOOnboardingAssets.WOOnboardingAssetsImagesMapping)
                          .Include(x => x.AssetPMs.ActiveAssetPMWOlineMapping)
                          .Include(x => x.WOLineBuildingMapping.FormIOBuildings)
                          .Include(x => x.WOLineBuildingMapping.FormIOFloors)
                          .Include(x => x.WOLineBuildingMapping.FormIORooms)
                          .Include(x => x.WOLineBuildingMapping.FormIOSections)
                          .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings)
                          .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors)
                          .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms)
                          .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections)
                          .Include(x => x.TempAssetPMs).ThenInclude(x => x.TempActiveAssetPMWOlineMapping).ThenInclude(x => x.WOOnboardingAssets)
                          .Include(x => x.TempActiveAssetPMWOlineMapping.TempAssetPMs)
                          .Include(x => x.TempAsset.TempFormIOBuildings)
                          .Include(x => x.TempAsset.TempFormIOFloors)
                          .Include(x => x.TempAsset.TempFormIORooms)
                          .Include(x => x.TempAsset.TempFormIOSections)
                          .Include(x => x.TempAsset.InspectionTemplateAssetClass.FormIOType)
            .FirstOrDefault();
        }
        public Asset GetAssetidForPMattch(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id).Include(x => x.InspectionTemplateAssetClass.AssetClassFormIOMapping).FirstOrDefault();
        }

        public List<AssetPMs> GetAssetPmsForOffline(DateTime? sync_time)
        {
            List<AssetPMs> response = new List<AssetPMs>();

            IQueryable<AssetPMs> query = context.AssetPMs;
            query = query
                .Where(x => x.Asset.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                );

            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_archive);
            }
            return query.ToList();

        }
        public List<AssetIssue> GetAssetMainIssueForOffline(DateTime? sync_time)
        {
            List<AssetIssue> response = new List<AssetIssue>();

            IQueryable<AssetIssue> query = context.AssetIssue
                .Include(x => x.Asset)
                .Include(x => x.StatusMaster)
                ;
            query = query
                .Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                );

            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();

        }
        public List<WOLineIssue> GetAssetWoloineIssueForOffline(DateTime? sync_time)
        {
            List<WOLineIssue> response = new List<WOLineIssue>();

            IQueryable<WOLineIssue> query = context.WOLineIssue
                 .Include(x => x.Asset)
                .Include(x => x.StatusMaster)
                ;
            query = query
                .Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                );

            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted && x.is_issue_linked_for_fix);
            }
            return query.ToList();

        }
        public List<AssetIssueImagesMapping> GetAssetIssueImagesForOffline(DateTime? sync_time)
        {
            List<AssetIssueImagesMapping> response = new List<AssetIssueImagesMapping>();

            IQueryable<AssetIssueImagesMapping> query = context.AssetIssueImagesMapping;
            query = query
                .Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                );

            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();

        }
        public List<AssetIRWOImagesLabelMapping> GetAssetIRVisualImageMappingOffline(DateTime? sync_time)
        {
            IQueryable<AssetIRWOImagesLabelMapping> query = context.AssetIRWOImagesLabelMapping;
            query = query
                .Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                );

            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.updated_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();

        }
        public List<AssetAttachmentMapping> GetAssetAttachmentsMappingOffline(DateTime? sync_time)
        {
            IQueryable<AssetAttachmentMapping> query = context.AssetAttachmentMapping;
            query = query
                .Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                );

            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();

        }
        public List<WOlineTopLevelcomponentMapping> GetWolineToplevelAssetMappingOffline(DateTime? sync_time)
        {
            IQueryable<WOlineTopLevelcomponentMapping> query = context.WOlineTopLevelcomponentMapping;
            query = query
                .Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                );

            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.updated_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();

        }
        public List<WOlineSubLevelcomponentMapping> GetWolineSublevelAssetMappingOffline(DateTime? sync_time)
        {
            IQueryable<WOlineSubLevelcomponentMapping> query = context.WOlineSubLevelcomponentMapping;
            query = query
                .Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                );

            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.updated_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();

        }

        public List<AssetTopLevelcomponentMapping> GetAssetToplevelAssetMappingOffline(DateTime? sync_time)
        {
            IQueryable<AssetTopLevelcomponentMapping> query = context.AssetTopLevelcomponentMapping;
            query = query
                .Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                );

            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.updated_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();

        }
        public List<AssetSubLevelcomponentMapping> GetAssetSublevelAssetMappingOffline(DateTime? sync_time)
        {
            IQueryable<AssetSubLevelcomponentMapping> query = context.AssetSubLevelcomponentMapping;
            query = query
                .Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                );

            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.updated_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();

        }

        public AssetPMs GetAssetPMtoUpdateOffline(Guid asset_pm_id)
        {
            return context.AssetPMs.Where(x => x.asset_pm_id == asset_pm_id)
                .Include(x => x.WOOnboardingAssets)
                .Include(x => x.WorkOrders)
                .Include(x => x.WOOnboardingAssets.IRWOImagesLabelMapping)
                .FirstOrDefault();
        }
        public AssetIssue GetAssetIssuebyIdforOffline(Guid asset_issue_id)
        {
            return context.AssetIssue.Where(x => x.asset_issue_id == asset_issue_id).FirstOrDefault();
        }

        public WOOnboardingAssets GetWOlineforUnlinkissue(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Include(x => x.AssetIssue)
                .Include(x => x.WOOnboardingAssetsImagesMapping)
                .FirstOrDefault();
        }
        public Asset GetAssetforFedby(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id)
                .Include(x => x.AssetParentHierarchyMapping)
                .Include(x => x.AssetTopLevelcomponentMapping)
                .Include(x => x.AssetSubLevelcomponentMapping)
                .Include(x => x.AssetProfileImages)
                .FirstOrDefault();
        }
        public List<WorkOrders> GetwobyIdsforIssueList(List<Guid> wo_ids)
        {
            return context.WorkOrders.Where(x => wo_ids.Contains(x.wo_id)).ToList();
        }
        public Asset GetAssetByIdforExisting(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id)
                .Include(x => x.InspectionTemplateAssetClass)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOSections)
                .Include(x => x.AssetParentHierarchyMapping)
                .Include(x => x.AssetProfileImages)
                //.Include(x => x.AssetIRWOImagesLabelMapping) // removed as per EFM-1857
                .Include(x => x.AssetTopLevelcomponentMapping)
                .Include(x => x.AssetSubLevelcomponentMapping)
                .Include(x => x.TempAsset)
                .FirstOrDefault();
        }
        public Asset GetAssetByIdforExistingScript(Guid asset_id)
        {



            return context.Assets.Where(x => x.asset_id == asset_id)
                .Include(x => x.InspectionTemplateAssetClass)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOSections)
                .Include(x => x.AssetParentHierarchyMapping)
                .Include(x => x.AssetProfileImages)
                .Include(x => x.AssetIRWOImagesLabelMapping)
                .Include(x => x.AssetTopLevelcomponentMapping)
                .Include(x => x.AssetSubLevelcomponentMapping).AsNoTracking()
                .FirstOrDefault();
        }

        public WOOnboardingAssets GetWolineforUpdateComponant(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Include(x => x.WOlineTopLevelcomponentMapping)
                .FirstOrDefault();
        }
        public Asset Getsublevelcomponent(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id)
                .Include(x => x.AssetTopLevelcomponentMapping)
                .Include(x => x.AssetProfileImages)
                .FirstOrDefault();
        }
        public List<InspectionTemplateAssetClass> GetAllAssetClassForExport()
        {
            return context.InspectionTemplateAssetClass.Where(x => x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && !x.isarchive).ToList();
        }
        public List<Asset> GetAssetsByAssetClassId(Guid inspectiontemplate_asset_class_id)
        {
            return context.Assets.Where(x => x.inspectiontemplate_asset_class_id == inspectiontemplate_asset_class_id
            && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
            && x.status == (int)Status.AssetActive
            ).ToList();
        }

        public AssetIssue GetAssetIssueByAssetIssueIdRepo(Guid asset_issue_id)
        {
            return context.AssetIssue.Where(x => x.asset_issue_id == asset_issue_id)
                          .Include(x => x.Asset)
                          .Include(x => x.Asset.InspectionTemplateAssetClass.AssetClassFormIOMapping)
                          .Include(x => x.WorkOrders).FirstOrDefault();
        }

        public string AddIssuesDirectlyToMaintenanceWORepo(AddIssuesDirectlyToMaintenanceWORequestModel request)
        {
            return "Success";
        }

        public AssetSubLevelcomponentMapping CheckSubcomponent(Guid requested_assset_id, Guid subcomponent_asset_id)
        {
            return context.AssetSubLevelcomponentMapping.Where(x => x.asset_id == requested_assset_id && x.sublevelcomponent_asset_id == subcomponent_asset_id && !x.is_deleted).FirstOrDefault();
        }
        public AssetTopLevelcomponentMapping CheckToplevelAssetofSubcomponent(Guid requested_asset_id, Guid toplevel_asset_id)
        {
            return context.AssetTopLevelcomponentMapping.Where(x => x.asset_id == requested_asset_id && x.toplevelcomponent_asset_id == toplevel_asset_id && !x.is_deleted).FirstOrDefault();
        }

        public WOlineTopLevelcomponentMapping GetWolinetoplevelforOfflineupdate(Guid woline_toplevelcomponent_mapping_id)
        {
            return context.WOlineTopLevelcomponentMapping.Where(x => x.woline_toplevelcomponent_mapping_id == woline_toplevelcomponent_mapping_id).FirstOrDefault();
        }
        public WOlineSubLevelcomponentMapping GetWolinesublevelforOfflineupdate(Guid woline_sublevelcomponent_mapping_id)
        {
            return context.WOlineSubLevelcomponentMapping.Where(x => x.woline_sublevelcomponent_mapping_id == woline_sublevelcomponent_mapping_id).FirstOrDefault();
        }

        public AssetTopLevelcomponentMapping GetAssettoplevelforOfflineupdate(Guid asset_toplevelcomponent_mapping_id)
        {
            return context.AssetTopLevelcomponentMapping.Where(x => x.asset_toplevelcomponent_mapping_id == asset_toplevelcomponent_mapping_id).FirstOrDefault();
        }
        public AssetSubLevelcomponentMapping GetAssetsublevelforOfflineupdate(Guid asset_sublevelcomponent_mapping_id)
        {
            return context.AssetSubLevelcomponentMapping.Where(x => x.asset_sublevelcomponent_mapping_id == asset_sublevelcomponent_mapping_id).FirstOrDefault();
        }

        public WOlineTopLevelcomponentMapping GettoplevelmappingforDelete(Guid sublevelcomponent_asset_id)
        {
            return context.WOlineTopLevelcomponentMapping.Where(x => x.woonboardingassets_id == sublevelcomponent_asset_id && !x.is_deleted).FirstOrDefault();
        }
        public WOlineSubLevelcomponentMapping GetsublevelmappingforDelete(Guid woline_id, Guid sublevel_component_asset_id)
        {
            return context.WOlineSubLevelcomponentMapping.Where(x => x.woonboardingassets_id == woline_id && x.sublevelcomponent_asset_id == sublevel_component_asset_id && !x.is_deleted).FirstOrDefault();
        }

        public int GetTotalNumberOfIssues(Guid siteId)
        {
            return context.AssetIssue.Where(x => x.site_id == siteId && !x.is_deleted).Count();
        }

        public string GetSiteCodeById(Guid siteId)
        {
            return context.Sites.Where(x => x.site_id == siteId).Select(x => x.site_code).FirstOrDefault();
        }

        public (List<AssetIssue>, int) GetIssuesForIssueNumber(Guid siteId)
        {
            IQueryable<AssetIssue> query = context.AssetIssue.Where(x => !x.is_deleted && x.site_id == siteId).OrderBy(x => x.created_at);

            var totalIssue = query.Count();

            return (query.ToList(), totalIssue);
        }

        public List<Sites> GetAllSitesIds()
        {
            return context.Sites.Where(x => x.status == 1).ToList();
        }
        public Asset GetTobeReplaceAsset(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id).FirstOrDefault();
        }
        public List<AssetPMs> GetAssetpmToaddWOline(List<Guid> asset_pm_ids)
        {
            return context.AssetPMs.Where(x => asset_pm_ids.Contains(x.asset_pm_id))
                 .Include(x => x.Asset).ThenInclude(x => x.InspectionTemplateAssetClass)
                 .Include(x => x.Asset).ThenInclude(x => x.AssetFormIOBuildingMappings)
                 .Include(x => x.ActiveAssetPMWOlineMapping)
                .ToList();
        }

        public PMs GetpmPmid(Guid pm_id)
        {
            return context.PMs.Where(x => x.pm_id == pm_id)
                .Include(x => x.PMPlans)
                 .ThenInclude(x => x.PMCategory)
                  .ThenInclude(x => x.InspectionTemplateAssetClass)
                .FirstOrDefault();
        }

        public PMItemMasterForms GetPMMasterFormByAssetpm(string asset_class_code, string plan_name, string title)
        {
            return context.PMItemMasterForms.Where(x => x.asset_class_code.ToLower().Trim() == asset_class_code.ToLower().Trim()
            && x.plan_name.ToLower().Trim() == plan_name.ToLower().Trim()
            && x.pm_title.ToLower().Trim() == title.ToLower().Trim()
            && !x.is_deleted
            && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)
            ).FirstOrDefault();
        }


        public (List<Asset>, int total_list_count) GetAssetsbyLocationHierarchy(GetAssetsbyLocationHierarchyRequestmodel requestmodel)
        {
            List<Asset> response = new List<Asset>();
            int total_list_count = 0;


            IQueryable<Asset> query = context.Assets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.status == (int)Status.AssetActive
                                                                    //&& x.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent
                                                                    );


            query = query.Include(x => x.AssetTopLevelcomponentMapping)
                            .Include(x => x.AssetSubLevelcomponentMapping)
                            .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                            .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                            .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                            .Include(x => x.AssetFormIOBuildingMappings.FormIOSections)
                            .Include(x => x.InspectionTemplateAssetClass);

            if (requestmodel.formiobuilding_id != null && requestmodel.formiobuilding_id > 0)
            {
                query = query.Where(x => x.AssetFormIOBuildingMappings.formiobuilding_id == requestmodel.formiobuilding_id);
            }
            if (requestmodel.formiofloor_id != null && requestmodel.formiofloor_id > 0)
            {
                query = query.Where(x => x.AssetFormIOBuildingMappings.formiofloor_id == requestmodel.formiofloor_id);
            }
            if (requestmodel.formioroom_id != null && requestmodel.formioroom_id > 0)
            {
                query = query.Where(x => x.AssetFormIOBuildingMappings.formioroom_id == requestmodel.formioroom_id);
            }
            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.Trim().ToLower();
                query = query.Where(x => x.name.Trim().ToLower().Contains(search)
                || x.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name.Trim().ToLower().Contains(search)
                || x.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name.Trim().ToLower().Contains(search)
                || x.AssetFormIOBuildingMappings.FormIORooms.formio_room_name.Trim().ToLower().Contains(search)
                || x.InspectionTemplateAssetClass.asset_class_code.Trim().ToLower().Contains(search)
                || x.InspectionTemplateAssetClass.asset_class_name.Trim().ToLower().Contains(search)
                );
            }
            query = query.OrderBy(x => x.name);
            total_list_count = query.Count();
            if (requestmodel.pagesize > 0 && requestmodel.pageindex > 0)
            {
                query = query.Skip((requestmodel.pageindex - 1) * requestmodel.pagesize).Take(requestmodel.pagesize);
            }

            response = query
                            .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                            .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                            .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                            .Include(x => x.AssetFormIOBuildingMappings.FormIOSections)
                            .Include(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.FormIOType)
                            .ToList();

            return (response, total_list_count);
        }

        public Asset GetSubLevelAssetById(Guid sublevelcomponent_asset_id)
        {
            return context.Assets.Where(x => x.asset_id == sublevelcomponent_asset_id)
                .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOSections)
                .Include(x => x.InspectionTemplateAssetClass.FormIOType).FirstOrDefault();
        }

        public TempFormIOBuildings GetTempFormIOBuildingByName(string building_name, Guid wo_id)
        {
            return context.TempFormIOBuildings.Where(x => x.temp_formio_building_name.Trim().ToLower() == building_name.Trim().ToLower() && x.wo_id == wo_id && !x.is_deleted && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public TempFormIOBuildings GetTempFormIOBuildingByNameV2(string building_name)
        {
            return context.TempFormIOBuildings.Where(x => x.temp_formio_building_name.Trim().ToLower() == building_name.Trim().ToLower() && !x.is_deleted && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public FormIOBuildings GetMainFormIOBuildingByName(string building_name)
        {
            return context.FormIOBuildings.Where(x => x.formio_building_name.Trim().ToLower() == building_name.Trim().ToLower() && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }

        public TempFormIOFloors GetTempFormIOFloorByName(string floor_name, Guid temp_building_id, Guid wo_id)
        {
            return context.TempFormIOFloors.Where(x => x.temp_formio_floor_name.Trim().ToLower() == floor_name.Trim().ToLower() && x.wo_id == wo_id && !x.is_deleted && x.temp_formiobuilding_id == temp_building_id && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public TempFormIOFloors GetTempFormIOFloorByNameV2(string floor_name, string building_name)
        {
            return context.TempFormIOFloors.Where(x => x.temp_formio_floor_name.Trim().ToLower() == floor_name.Trim().ToLower() && !x.is_deleted && x.TempFormIOBuildings.temp_formio_building_name.ToLower().Trim() == building_name.ToLower().Trim() && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public TempFormIORooms GetTempFormIORoomByNameV2(string room_name, string temp_floor_name, string temp_building_name)
        {
            return context.TempFormIORooms.Where(x => x.temp_formio_room_name.Trim().ToLower() == room_name.Trim().ToLower() && !x.is_deleted && x.TempFormIOFloors.temp_formio_floor_name.Trim().ToLower() == temp_floor_name.ToLower().Trim() && x.TempFormIOFloors.TempFormIOBuildings.temp_formio_building_name.ToLower().Trim() == temp_building_name.ToLower().Trim() && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public FormIORooms GetMainFormIORoomByName(string room_name, string temp_floor_name, string temp_building_name)
        {
            return context.FormIORooms.Where(x => x.formio_room_name.Trim().ToLower() == room_name.Trim().ToLower() && x.FormIOFloors.formio_floor_name.Trim().ToLower() == temp_floor_name.ToLower().Trim() && x.FormIOFloors.FormIOBuildings.formio_building_name.ToLower().Trim() == temp_building_name.ToLower().Trim() && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public FormIOFloors GetMainFormIOFloorByName(string floor_name, string building_name)
        {
            return context.FormIOFloors.Where(x => x.formio_floor_name.Trim().ToLower() == floor_name.Trim().ToLower() && x.FormIOBuildings.formio_building_name.ToLower().Trim() == building_name.ToLower().Trim() && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public TempFormIORooms GetTempFormIORoomByName(string room_name, Guid temp_floor_id, Guid wo_id)
        {
            return context.TempFormIORooms.Where(x => x.temp_formio_room_name.Trim().ToLower() == room_name.Trim().ToLower() && x.wo_id == wo_id && !x.is_deleted && x.temp_formiofloor_id == temp_floor_id && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public TempFormIOSections GetTempFormIOSectionByName(string section_name, Guid temp_room_id, Guid wo_id)
        {
            return context.TempFormIOSections.Where(x => x.temp_formio_section_name.Trim().ToLower() == section_name.Trim().ToLower() && x.wo_id == wo_id && !x.is_deleted && x.temp_formioroom_id == temp_room_id && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }

        public List<TempFormIOBuildings> GetTempLocationHierarchyForWO(Guid wo_id, string search_string)
        {
            IQueryable<TempFormIOBuildings> query = context.TempFormIOBuildings
                .Where(x => x.wo_id == wo_id && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_deleted)
               .Include(x => x.TempFormIOFloors).ThenInclude(x => x.TempFormIORooms).OrderBy(x => x.temp_formio_building_name);

            if (!String.IsNullOrEmpty(search_string))
            {
                string search = search_string.ToLower().Trim();
                query = query.Where(x =>
                  x.temp_formio_building_name.ToLower().Trim().Contains(search) ||
                  x.TempFormIOFloors.Select(x => x.temp_formio_floor_name.ToLower().Trim()).Contains(search) ||
                  x.TempFormIOFloors.SelectMany(x => x.TempFormIORooms).Select(x => x.temp_formio_room_name.ToLower().Trim()).Contains(search)
                );
            }

            return query.ToList();
        }

        public (List<WOOnboardingAssets>, int total_list_count) GetWOOBAssetsbyLocationHierarchy(GetWOOBAssetsbyLocationHierarchyRequestModel requestmodel)
        {
            List<WOOnboardingAssets> response = new List<WOOnboardingAssets>();
            int total_list_count = 0;

            IQueryable<WOOnboardingAssets> query = context.WOOnboardingAssets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                                                         && x.wo_id == requestmodel.wo_id && !x.is_deleted);

            query = query.Include(x => x.WOOBAssetTempFormIOBuildingMapping)
                            .Include(x => x.Asset)
                            .Include(x => x.StatusMaster)
                            .Include(x => x.WOlineTopLevelcomponentMapping)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections);

            if (requestmodel.temp_formiobuilding_id != null && requestmodel.temp_formiobuilding_id != Guid.Empty)
            {
                query = query.Where(x => x.WOOBAssetTempFormIOBuildingMapping.temp_formiobuilding_id == requestmodel.temp_formiobuilding_id);
            }
            if (requestmodel.temp_formiofloor_id != null && requestmodel.temp_formiofloor_id != Guid.Empty)
            {
                query = query.Where(x => x.WOOBAssetTempFormIOBuildingMapping.temp_formiofloor_id == requestmodel.temp_formiofloor_id);
            }
            if (requestmodel.temp_formioroom_id != null && requestmodel.temp_formioroom_id != Guid.Empty)
            {
                query = query.Where(x => x.WOOBAssetTempFormIOBuildingMapping.temp_formioroom_id == requestmodel.temp_formioroom_id);
            }
            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.Trim().ToLower();
                query = query.Where(x => x.asset_name.Trim().ToLower().Contains(search)
                || x.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings.temp_formio_building_name.Trim().ToLower().Contains(search)
                || x.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors.temp_formio_floor_name.Trim().ToLower().Contains(search)
                || x.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms.temp_formio_room_name.Trim().ToLower().Contains(search)
                || x.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections.temp_formio_section_name.Trim().ToLower().Contains(search));
            }

            query = query.OrderByDescending(x => x.created_at);
            total_list_count = query.Count();

            if (requestmodel.pagesize > 0 && requestmodel.pageindex > 0)
            {
                query = query.Skip((requestmodel.pageindex - 1) * requestmodel.pagesize).Take(requestmodel.pagesize);
            }

            response = query.ToList();

            return (response, total_list_count);
        }
        public (List<WOOnboardingAssets>, int total_list_count) GetWOOBAssetsbyLocationHierarchyV2(GetWOOBAssetsbyLocationHierarchyRequestModel requestmodel)
        {
            List<WOOnboardingAssets> response = new List<WOOnboardingAssets>();
            int total_list_count = 0;

            IQueryable<WOOnboardingAssets> query = context.WOOnboardingAssets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                                                         && x.wo_id == requestmodel.wo_id && !x.is_deleted);

            query = query.Include(x => x.WOOBAssetTempFormIOBuildingMapping)
                            .Include(x => x.Asset)
                            .Include(x => x.TempAsset)
                            .Include(x => x.StatusMaster)
                            .Include(x => x.WOlineTopLevelcomponentMapping)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections)
                            .Include(x => x.TempAsset.TempMasterBuilding)
                            .Include(x => x.TempAsset.TempMasterFloor)
                            .Include(x => x.TempAsset.TempMasterRoom);

            if (!String.IsNullOrEmpty(requestmodel.buildig_name))
            {
                query = query.Where(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings.temp_formio_building_name.ToLower().Trim() == requestmodel.buildig_name.ToLower().Trim());
                //query = query.Where(x => x.WOOBAssetTempFormIOBuildingMapping.buil == requestmodel.temp_formiobuilding_id);
            }
            if (!String.IsNullOrEmpty(requestmodel.floor_name))
            {
                //query = query.Where(x => x.WOOBAssetTempFormIOBuildingMapping.temp_formiofloor_id == requestmodel.temp_formiofloor_id);
                query = query.Where(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors.temp_formio_floor_name.ToLower().Trim() == requestmodel.floor_name.ToLower().Trim());
            }
            if (!String.IsNullOrEmpty(requestmodel.room_name))
            {
                query = query.Where(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms.temp_formio_room_name.ToLower().Trim() == requestmodel.room_name.ToLower().Trim());
            }
            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.Trim().ToLower();
                query = query.Where(x => x.asset_name.Trim().ToLower().Contains(search)
                || x.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings.temp_formio_building_name.Trim().ToLower().Contains(search)
                || x.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors.temp_formio_floor_name.Trim().ToLower().Contains(search)
                || x.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms.temp_formio_room_name.Trim().ToLower().Contains(search)
                || x.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections.temp_formio_section_name.Trim().ToLower().Contains(search));
            }

            query = query.OrderByDescending(x => x.created_at);
            total_list_count = query.Count();

            if (requestmodel.pagesize > 0 && requestmodel.pageindex > 0)
            {
                query = query.Skip((requestmodel.pageindex - 1) * requestmodel.pagesize).Take(requestmodel.pagesize);
            }

            response = query.ToList();

            return (response, total_list_count);
        }

        public (List<WOOnboardingAssets>, int total_list_count) GetWOOBAssetsbyLocationHierarchyV3(GetWOOBAssetsbyLocationHierarchyRequestModel requestmodel)
        {
            List<WOOnboardingAssets> response = new List<WOOnboardingAssets>();
            int total_list_count = 0;

            IQueryable<WOOnboardingAssets> query = context.WOOnboardingAssets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                                                         && x.wo_id == requestmodel.wo_id && !x.is_deleted);

            query = query.Include(x => x.WOOBAssetTempFormIOBuildingMapping)
                            .Include(x => x.Asset)
                            .Include(x => x.TempAsset)
                            .Include(x => x.StatusMaster)
                            .Include(x => x.WOlineTopLevelcomponentMapping)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections)
                            .Include(x => x.TempAsset.TempMasterBuilding)
                            .Include(x => x.TempAsset.TempMasterFloor)
                            .Include(x => x.WOOnboardingAssetsImagesMapping)
                            .Include(x => x.TempAsset.TempMasterRoom);

            if (!String.IsNullOrEmpty(requestmodel.room_name))
            {
                query = query.Where(x => x.TempAsset.TempMasterRoom != null ? x.TempAsset.TempMasterRoom.temp_master_room_name.ToLower().Trim() == requestmodel.room_name.ToLower().Trim() : x.room.ToLower().Trim() == requestmodel.room_name.ToLower().Trim());
            }
            if (requestmodel.temp_master_room_id != null&&requestmodel.temp_master_room_id != Guid.Empty)
            {
                query = query.Where(x=>x.TempAsset.temp_master_room_id==requestmodel.temp_master_room_id);
            }

            //if (!String.IsNullOrEmpty(requestmodel.buildig_name))
            //{
            //    query = query.Where(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings.temp_formio_building_name.ToLower().Trim() == requestmodel.buildig_name.ToLower().Trim());
            //    //query = query.Where(x => x.WOOBAssetTempFormIOBuildingMapping.buil == requestmodel.temp_formiobuilding_id);
            //}
            //if (!String.IsNullOrEmpty(requestmodel.floor_name))
            //{
            //    //query = query.Where(x => x.WOOBAssetTempFormIOBuildingMapping.temp_formiofloor_id == requestmodel.temp_formiofloor_id);
            //    query = query.Where(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors.temp_formio_floor_name.ToLower().Trim() == requestmodel.floor_name.ToLower().Trim());
            //}

            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.Trim().ToLower();
                query = query.Where(x => x.asset_name.Trim().ToLower().Contains(search)
                || x.TempAsset.TempMasterBuilding != null ? x.TempAsset.TempMasterBuilding.temp_master_building_name.Trim().ToLower().Contains(search) : x.asset_name == search
                || x.TempAsset.TempMasterFloor != null ? x.TempAsset.TempMasterFloor.temp_master_floor_name.Trim().ToLower().Contains(search) : x.asset_name == search
                || x.TempAsset.TempMasterRoom != null ? x.TempAsset.TempMasterRoom.temp_master_room_name.Trim().ToLower().Contains(search) : x.asset_name == search
                || x.TempAsset.temp_master_section.Trim().ToLower().Contains(search));
            }

            query = query.OrderByDescending(x => x.created_at);
            total_list_count = query.Count();

            if (requestmodel.pagesize > 0 && requestmodel.pageindex > 0)
            {
                query = query.Skip((requestmodel.pageindex - 1) * requestmodel.pagesize).Take(requestmodel.pagesize);
            }

            response = query.ToList();

            return (response, total_list_count);
        }

        public List<Guid> GetAllSites()
        {
            return context.Sites.Where(x => x.company_id == Guid.Parse("22a8170a-f97c-432b-976a-28c4d7abf3ca")).Select(x => x.site_id).ToList();
        }

        public List<Equipment> GetAllEquipment(Guid site_id)
        {
            return context.Equipment.Where(x => x.site_id == site_id && !x.isarchive).ToList();
        }

        public int GetAssetCount()
        {
            return context.Assets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
            && x.status == (int)Status.AssetActive
            ).Count();
        }

        public List<PMItemMasterForms> GetPMMasterFormForOffline(DateTime? sync_time)
        {
            IQueryable<PMItemMasterForms> query = context.PMItemMasterForms.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id
            );
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.updated_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }
        public List<PMs> GetPMMasterForOffline(DateTime? sync_time)
        {
            IQueryable<PMs> query = context.PMs.Where(x => x.PMPlans.PMCategory.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id
            );
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_archive);
            }

            return query.ToList();
        }
        public List<PMPlans> GetPMPlanMasterForOffline(DateTime? sync_time)
        {
            IQueryable<PMPlans> query = context.PMPlans.Where(x => x.PMCategory.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id
           );
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => x.status == (int)Status.Active);
            }
            return query.ToList();
        }
        public List<PMCategory> GetPMCategoryMasterForOffline(DateTime? sync_time)
        {
            IQueryable<PMCategory> query = context.PMCategory.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id
           );
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => x.status == (int)Status.Active);
            }
            return query.ToList();
        }
        public List<ActiveAssetPMWOlineMapping> GetPMSubmittedData(DateTime? sync_time)
        {
            IQueryable<ActiveAssetPMWOlineMapping> query = context.ActiveAssetPMWOlineMapping.Where(x => x.AssetPMs.Asset.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
           );
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            return query.ToList();
        }

        public AssetFormIOBuildingMappings GetAssetLocationDataById(Guid asset_id)
        {
            return context.AssetFormIOBuildingMappings.Where(x => x.asset_id == asset_id)
                .Include(x => x.FormIOBuildings)
                .Include(x => x.FormIOFloors)
                .Include(x => x.FormIORooms)
                .Include(x => x.FormIOSections)
                .FirstOrDefault();
        }

        public Guid GetAssetPMsbyWOLineId(Guid woonboardingassets_id)
        {
            return context.ActiveAssetPMWOlineMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Select(x => x.asset_pm_id).FirstOrDefault();
        }
        public Guid GetTempAssetPMsbyWOLineId(Guid woonboardingassets_id)
        {
            return context.TempActiveAssetPMWOlineMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Select(x => x.temp_asset_pm_id).FirstOrDefault();
        }
        public Asset GetAssetbyIDForCondition(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id)
                .Include(x => x.AssetIssue).FirstOrDefault();
        }
        public TempFormIOBuildings IsExistTempBuildingToAdd(Guid wo_id, int formiobuilding_id)
        {
            return context.TempFormIOBuildings.Where(x => x.wo_id == wo_id && x.formiobuilding_id == formiobuilding_id && !x.is_deleted).FirstOrDefault();
        }
        public TempFormIOFloors IsExistTempFloorToAdd(Guid wo_id, int formiofloor_id)
        {
            return context.TempFormIOFloors.Where(x => x.wo_id == wo_id && x.formiofloor_id == formiofloor_id && !x.is_deleted).FirstOrDefault();
        }
        public TempFormIORooms IsExistTempRoomsToAdd(Guid wo_id, int formioroom_id)
        {
            return context.TempFormIORooms.Where(x => x.wo_id == wo_id && x.formioroom_id == formioroom_id && !x.is_deleted).FirstOrDefault();
        }

        public TempFormIOBuildings GetTempBuildingForDelete(Guid temp_formiobuilding_id)
        {
            TempFormIOBuildings temp_building = null;
            bool isAny = context.WOOBAssetTempFormIOBuildingMappings.Where(x => x.temp_formiobuilding_id == temp_formiobuilding_id && !x.WOOnboardingAssets.is_deleted).Any();
            if (!isAny)
            {
                var res = context.TempFormIOBuildings.Where(x => x.temp_formiobuilding_id == temp_formiobuilding_id)
                    .Include(x => x.TempFormIOFloors).ThenInclude(x => x.TempFormIORooms).ThenInclude(x => x.TempFormIOSections)
                    .FirstOrDefault();
                temp_building = res;
            }
            return temp_building;
        }
        public TempFormIOFloors GetTempFloorForDelete(Guid temp_formiofloor_id)
        {
            TempFormIOFloors temp_floor = null;
            bool isAny = context.WOOBAssetTempFormIOBuildingMappings.Where(x => x.temp_formiofloor_id == temp_formiofloor_id && !x.WOOnboardingAssets.is_deleted).Any();
            if (!isAny)
            {
                var res = context.TempFormIOFloors.Where(x => x.temp_formiofloor_id == temp_formiofloor_id)
                    .Include(x => x.TempFormIORooms).ThenInclude(x => x.TempFormIOSections)
                    .FirstOrDefault();
                temp_floor = res;
            }
            return temp_floor;
        }
        public TempFormIORooms GetTempRoomForDelete(Guid temp_formioroom_id)
        {
            TempFormIORooms temp_room = null;
            bool isAny = context.WOOBAssetTempFormIOBuildingMappings.Where(x => x.temp_formioroom_id == temp_formioroom_id && !x.WOOnboardingAssets.is_deleted).Any();
            if (!isAny)
            {
                var res = context.TempFormIORooms.Where(x => x.temp_formioroom_id == temp_formioroom_id)
                    .Include(x => x.TempFormIOSections)
                    .FirstOrDefault();
                temp_room = res;
            }
            return temp_room;
        }
        public int GetAssetFormIOCountByWOType(int wo_type)
        {
            return context.AssetFormIO.Include(x => x.WorkOrders).Where(x => x.WorkOrders.wo_type == wo_type
            && x.WorkOrders.status != (int)Status.Completed && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
               //&& (x.status == (int)Status.Ready_for_review || x.status == (int)Status.Completed || x.status == (int)Status.Submitted) // old flow 
               && (x.status == (int)Status.Ready_for_review) // new flow: now only RFR count 
               ).Count();
        }
        public TempFormIOSections GetTempSectionByName(string section, Guid temp_formioroom_id)
        {
            return context.TempFormIOSections.Where(x => x.temp_formio_section_name.ToLower().Trim() == section.ToLower().Trim() && !x.is_deleted && x.temp_formioroom_id == temp_formioroom_id).AsNoTracking().FirstOrDefault();
        }

        public ActiveAssetPMWOlineMapping GetPMSubmittedDataForOffline(Guid active_asset_pm_woline_mapping_id)
        {
            return context.ActiveAssetPMWOlineMapping.Where(x => x.active_asset_pm_woline_mapping_id == active_asset_pm_woline_mapping_id).FirstOrDefault();
        }
        public List<Asset> CountForBuildingAssets(List<int> formiobuilding_id)
        {
            return context.Assets.Where(x => formiobuilding_id.Contains(x.AssetFormIOBuildingMappings.formiobuilding_id.Value)
                    && x.status == (int)Status.AssetActive && x.component_level_type_id > 0)
                        .Include(x => x.AssetFormIOBuildingMappings)
                        .Include(x => x.AssetTopLevelcomponentMapping)
                        .ToList();
        }

        public Guid GetTopLevelWOOBAsset(Guid woonboardingassets_id)
        {
            return context.WOlineTopLevelcomponentMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id && !x.is_deleted).Select(x => x.toplevelcomponent_asset_id).FirstOrDefault();
        }

        public List<AssetPMsTriggerConditionMapping> GetAssetPMTriggerConditionMappingOffline(DateTime? sync_time)
        {
            IQueryable<AssetPMsTriggerConditionMapping> query = context.AssetPMsTriggerConditionMapping;
            query = query.Where(x => x.AssetPMs.Asset.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_archive);
            }
            return query.ToList();
        }

        public List<WOOnboardingAssets> GetWOOBAssetsByWOId(Guid wo_id)
        {
            return context.WOOnboardingAssets.Include(x => x.IRWOImagesLabelMapping).Where(x => x.wo_id == wo_id && !x.is_deleted && !x.WorkOrders.is_archive && x.IRWOImagesLabelMapping.Count > 0)
                .Include(x => x.TempAsset.InspectionTemplateAssetClass)
                .ToList();
        }
        public (List<GetOBIRImagesByWOIdResponseModel>,int) GetWOOBAssetsByWOId_v2(GetOBIRImagesByWOId_V2RequestModel requestModel)
        {
            List<GetOBIRImagesByWOIdResponseModel> res = new List<GetOBIRImagesByWOIdResponseModel>();

            IQueryable<WOOnboardingAssets> query = context.WOOnboardingAssets.Include(x => x.IRWOImagesLabelMapping)
                .Where(x => x.wo_id == requestModel.wo_id && !x.is_deleted && !x.WorkOrders.is_archive && x.IRWOImagesLabelMapping.Count > 0)
                .Include(x => x.TempAsset.InspectionTemplateAssetClass);

            int count = query.Count();

            if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
            {
                query = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);//.OrderBy(x => x.created_at);
            }

            res = query.Select(x => new GetOBIRImagesByWOIdResponseModel
            {
                asset_name = x.TempAsset.asset_name,
                asset_class_name = x.TempAsset.InspectionTemplateAssetClass.asset_class_name,
                asset_class_code = x.TempAsset.InspectionTemplateAssetClass.asset_class_name,

                ob_ir_Image_label_list = x.IRWOImagesLabelMapping.Where(u => !u.is_deleted).Select(y => new View_OBIRImage_label
                {
                    irwoimagelabelmapping_id = y.irwoimagelabelmapping_id,
                    ir_image_label = y.ir_image_label,
                    ir_image_label_url = UrlGenerator.GetIRImagesURL(y.ir_image_label, y.s3_image_folder_name),
                    visual_image_label = y.visual_image_label,
                    visual_image_label_url = UrlGenerator.GetIRImagesURL(y.visual_image_label, y.s3_image_folder_name),
                    s3_image_folder_name = y.s3_image_folder_name,
                    woonboardingassets_id = y.woonboardingassets_id,
                    is_deleted = y.is_deleted
                }).ToList()

            }).ToList();

            res = res.OrderBy(x => x.ob_ir_Image_label_list.FirstOrDefault()?.ir_image_label).ToList();

            return (res, count);
        }

        public List<AssetPMs> GetOpenAssetpms(Guid asset_id)
        {
            return context.AssetPMs.Where(x => x.asset_id == asset_id && x.status != (int)Status.Completed && !x.is_archive).ToList();
        }

        public List<AssetPMs> GetCurrentAssetPMsByAssetId(Guid asset_id)
        {
            var list = context.AssetPMs.Where(x => x.asset_id == asset_id && x.datetime_starting_at != null && x.status != (int)Status.Completed && x.due_date > DateTime.UtcNow.AddDays(-1).Date && !x.is_archive).ToList()
                        .GroupBy(x => new { x.asset_id, x.pm_id })
                        .Select(g => g.OrderBy(x => x.datetime_starting_at).FirstOrDefault());
            var assetpms = context.AssetPMs.Where(x => list.Select(y => y.asset_pm_id).Contains(x.asset_pm_id)).ToList();

            return assetpms;
        }

        public bool CheckForAssetPMIsActiveOrNot(Guid asset_pm_id)
        {
            return context.ActiveAssetPMWOlineMapping.Where(x => x.asset_pm_id == asset_pm_id && x.is_active && !x.is_deleted).Any();
        }
        public List<TempFormIOBuildings> GetTempBuildingforOffline(DateTime? sync_time)
        {
            IQueryable<TempFormIOBuildings> query = context.TempFormIOBuildings;
            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }
        public List<TempFormIOFloors> GetTempFloorforOffline(DateTime? sync_time)
        {
            IQueryable<TempFormIOFloors> query = context.TempFormIOFloors;
            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }
        public List<TempFormIORooms> GetTempRoomforOffline(DateTime? sync_time)
        {
            IQueryable<TempFormIORooms> query = context.TempFormIORooms;
            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }
        public List<TempFormIOSections> GetTempSectionforOffline(DateTime? sync_time)
        {
            IQueryable<TempFormIOSections> query = context.TempFormIOSections;
            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }
        public List<WOOBAssetTempFormIOBuildingMapping> GetTemplocationwolineMappingoffline(DateTime? sync_time)
        {
            IQueryable<WOOBAssetTempFormIOBuildingMapping> query = context.WOOBAssetTempFormIOBuildingMappings;
            query = query.Where(x => x.WOOnboardingAssets.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);

            }
            return query.ToList();
        }
        public List<TempAssetPMs> GetTempassetpmsoffline(DateTime? sync_time)
        {
            IQueryable<TempAssetPMs> query = context.TempAssetPMs;
            query = query.Where(x => x.WOOnboardingAssets.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);

            }
            return query.ToList();
        }
        public List<TempActiveAssetPMWOlineMapping> GetTempActiveassetpmsoffline(DateTime? sync_time)
        {
            IQueryable<TempActiveAssetPMWOlineMapping> query = context.TempActiveAssetPMWOlineMapping;
            query = query.Where(x => x.TempAssetPMs.WOOnboardingAssets.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);

            }
            return query.ToList();
        }
        public List<TempAsset> GetTempassetoffline(DateTime? sync_time)
        {
            IQueryable<TempAsset> query = context.TempAsset;
            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }
        public List<WOlineIssueImagesMapping> TempIssueImagesMappingOffline(DateTime? sync_time)
        {
            IQueryable<WOlineIssueImagesMapping> query = context.WOlineIssueImagesMapping;
            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }

        public List<SitewalkthroughTempPmEstimation> GetSiteWalkthroughTempPMEstimationOffline(DateTime? sync_time)
        {
            IQueryable<SitewalkthroughTempPmEstimation> query = context.SitewalkthroughTempPmEstimation;
            query = query.Where(x => x.WOOnboardingAssets.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).Include(x => x.PMPlans).Include(x => x.PMs);
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }

        public TempFormIOBuildings GetTempBuildingforOfflineUpdate(Guid temp_formiobuilding_id)
        {
            return context.TempFormIOBuildings.Where(x => x.temp_formiobuilding_id == temp_formiobuilding_id).FirstOrDefault();
        }
        public TempFormIOFloors GetTempFloorforOfflineUpdate(Guid temp_formiofloor_id)
        {
            return context.TempFormIOFloors.Where(x => x.temp_formiofloor_id == temp_formiofloor_id).FirstOrDefault();
        }
        public TempFormIORooms GetTempRoomforOfflineUpdate(Guid temp_formioroom_id)
        {
            return context.TempFormIORooms.Where(x => x.temp_formioroom_id == temp_formioroom_id).FirstOrDefault();
        }
        public WOOBAssetTempFormIOBuildingMapping GetTemplocationwolinemappingforOfflineUpdate(Guid wo_ob_asset_temp_formiobuilding_id)
        {
            return context.WOOBAssetTempFormIOBuildingMappings.Where(x => x.wo_ob_asset_temp_formiobuilding_id == wo_ob_asset_temp_formiobuilding_id)
                .Include(x => x.WOOnboardingAssets)
                .FirstOrDefault();
        }
        public TempFormIOSections GetTempSectionformOfflineUpdate(string section_name, Guid? temp_formioroom_id)
        {
            return context.TempFormIOSections.Where(x => x.temp_formio_section_name.ToLower().Trim() == section_name.ToLower().Trim()
            && x.temp_formioroom_id == temp_formioroom_id
            && !x.is_deleted
            ).FirstOrDefault();
        }
        public WOOnboardingAssets GetMainAssetWOlineforPM(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Include(x => x.TempAsset.TempFormIOBuildings)
                .Include(x => x.TempAsset.TempFormIOFloors)
                .Include(x => x.TempAsset.TempFormIORooms)
                .Include(x => x.TempAsset.TempFormIOSections)
                .FirstOrDefault();
        }
        public AssetPMs GetAssetpmByidForInspectionlist(Guid asset_pm_id)
        {
            return context.AssetPMs.Where(x => x.asset_pm_id == asset_pm_id).FirstOrDefault();
        }

        public List<Asset> Getalltoplevelforscript()
        {
            return context.Assets.Where(x => x.component_level_type_id == 1 && x.company_id == "22a8170a-f97c-432b-976a-28c4d7abf3ca"
            //&& x.asset_id == Guid.Parse("a6d6ece3-aa20-40b4-8ccb-c44c6341a45b")
            )
                .Include(x => x.AssetFormIOBuildingMappings)
                .Include(x => x.AssetSubLevelcomponentMapping)
                .ToList();
        }

        public WOOnboardingAssets GetSublevelWolinefornameplateImage(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Include(x => x.WOOnboardingAssetsImagesMapping)
                .FirstOrDefault();
        }

        public Asset AssetGettoplevelmainAsset(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                .FirstOrDefault();
        }

        public WOOnboardingAssets GetOBWOAssetforLocation(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping)
                .Include(x => x.WOlineSubLevelcomponentMapping)
                .Include(x => x.TempAsset)
                .Include(x => x.WOLineBuildingMapping)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections)
                .FirstOrDefault();
        }

        public WorkOrderTechnicianMapping GetWorkOrderTechnicianMappingById(Guid wo_technician_mapping_id)
        {
            return context.WorkOrderTechnicianMapping.Where(x => x.wo_technician_mapping_id == wo_technician_mapping_id && x.is_deleted == false).FirstOrDefault();
        }

        public AssetPMPlans GetAssetPMPlanforMapping(Guid pm_plan_id)
        {
            return context.AssetPMPlans.Where(x => x.pm_plan_id == pm_plan_id && x.status == (int)Status.Active).FirstOrDefault();
        }

        public WOOBAssetTempFormIOBuildingMapping GetTempLocationOfOBWOAssetByOBAssetID(Guid woonboardingassets_id)
        {
            return context.WOOBAssetTempFormIOBuildingMappings.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                //.Include(x => x.WOOnboardingAssets)
                .FirstOrDefault();
        }
        public int GetOBWOAssetCountByStatus(Guid wo_id, int status_code)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && x.status == status_code
                && !x.is_woline_from_other_inspection && !x.is_deleted).Count();
        }

        public int GetAssetFormIOCountByStatus(Guid wo_id, int status_code)
        {
            return context.AssetFormIO.Where(x => x.wo_id == wo_id && x.status == status_code).Count();
        }

        // public AssetPMs GetAssetpmByidForInspectionlist(Guid asset_pm_id)
        // {
        //   return context.AssetPMs.Where(x => x.asset_pm_id == asset_pm_id).FirstOrDefault();
        // public AssetPMs GetAssetpmByidForInspectionlist(Guid asset_pm_id)
        // {
        //   return context.AssetPMs.Where(x => x.asset_pm_id == asset_pm_id).FirstOrDefault();
        //}

        public List<WorkOrders> GetWODetilsForReport()
        {
            List<Guid> company_ids = new List<Guid> { Guid.Parse("eb5b139e-6f74-4ace-920b-9e4fa5c808d5"), Guid.Parse("7a4513ef-a190-4f45-84db-632ee59e8753"), Guid.Parse("f1c579ce-1571-47fd-8d4b-6e3e35df3eff") };

            var sites = context.Sites.Where(x => !company_ids.Contains(x.company_id)).Select(x => x.site_id).ToList();

            return context.WorkOrders.Where(x => sites.Contains(x.site_id)
            && x.status != 15
            && !x.is_archive
            )
                .Include(x => x.Sites.ClientCompany)
                .Include(x => x.Sites.Company)
                .Include(x => x.WorkOrderTechnicianMapping).ThenInclude(x => x.TechnicianUser)
                .Include(x => x.StatusMaster)
                .Include(x => x.WOLineIssue)
                //.Take(5)
                .ToList();
        }

        public List<WorkOrders> GetCompletedWODetilsForReport()
        {
            List<Guid> company_ids = new List<Guid> { Guid.Parse("eb5b139e-6f74-4ace-920b-9e4fa5c808d5"), Guid.Parse("7a4513ef-a190-4f45-84db-632ee59e8753"), Guid.Parse("f1c579ce-1571-47fd-8d4b-6e3e35df3eff") };

            var sites = context.Sites.Where(x => !company_ids.Contains(x.company_id) && x.status == 1).Select(x => x.site_id).ToList();

            return context.WorkOrders.Where(x => sites.Contains(x.site_id)
            && x.status == 15
            && !x.is_archive
            && (x.modified_at >= DateTime.Parse("2023-12-16 06:30:00.000") || x.modified_at <= DateTime.Parse("2024-01-16 06:30:00.000"))
            )
             .Include(x => x.Sites.ClientCompany)
            .Include(x => x.Sites.Company)
            .Include(x => x.WorkOrderTechnicianMapping).ThenInclude(x => x.TechnicianUser)
            .Include(x => x.StatusMaster)
            .Include(x => x.WOLineIssue)
            //.Take(5)
            .ToList();
        }
        public List<Sites> GetsitesForReport()
        {
            List<Guid> company_ids = new List<Guid> { Guid.Parse("eb5b139e-6f74-4ace-920b-9e4fa5c808d5"), Guid.Parse("7a4513ef-a190-4f45-84db-632ee59e8753"), Guid.Parse("f1c579ce-1571-47fd-8d4b-6e3e35df3eff") };

            return context.Sites.Where(x => !company_ids.Contains(x.company_id) && x.status == 1)
                .Include(x => x.Company)
                .Include(x => x.ClientCompany)
                .ToList();
        }
        public List<AssetIssue> GetIssueDetailsForReport(Guid site_id)
        {

            return context.AssetIssue.Where(x => x.site_id == site_id
            && !x.is_deleted
            )
             .Include(x => x.Sites.ClientCompany)
            .Include(x => x.Sites.Company)
            .Include(x => x.StatusMaster)
            //.Take(5)
            .ToList();
        }

        public Asset GetMainAssetforTempIssue(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                .Include(x => x.AssetFormIOBuildingMappings.FormIOSections)
                .Include(x => x.InspectionTemplateAssetClass)
                .FirstOrDefault();
        }
        public WOOnboardingAssets GetwolineForTempIssue(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .FirstOrDefault();

        }

        public List<Asset> GetMainAssetstoAddIssue(GetAssetListForIssueRequestmodel request)
        {
            return context.Assets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.status == (int)Status.AssetActive).ToList();
        }
        public List<WOOnboardingAssets> GetTempAssetstoAddissue(GetAssetListForIssueRequestmodel request)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == request.wo_id && !x.is_deleted && x.inspection_type == (int)MWO_inspection_wo_type.OnBoarding
            && x.asset_id == null
            ).ToList();
        }
        public WOOnboardingAssets GetWOlineByIdForissueupdate(Guid woline_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woline_id)

                .FirstOrDefault();
        }
        public Asset GetMainAssetByIdForissueupdate(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id)
                .Include(x => x.InspectionTemplateAssetClass)
                .FirstOrDefault();
        }
        public List<WorkOrders> GetAllCalendarWorkorders(GetAllCalanderWorkordersRequestModel requestModel)
        {
            IQueryable<WorkOrders> query = context.WorkOrders.Where(x =>
            ((x.start_date.Date >= requestModel.start_date.Date && x.start_date.Date <= requestModel.end_date.Date)
            || (x.due_at.Date >= requestModel.start_date.Date && x.due_at.Date <= requestModel.end_date.Date)
            || (requestModel.start_date.Date >= x.start_date.Date && requestModel.end_date.Date <= x.due_at.Date))
            && !x.is_archive && x.status != (int)Status.QuoteWO);// && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)

            if (requestModel.site_id != null && requestModel.site_id.Count > 0)
                query = query.Where(x => requestModel.site_id.Contains(x.site_id));
            else// as per new requirement return no data if site_id filter is null
                query = query.Where(x => x.site_id == Guid.Empty);//Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));

            if (requestModel.status != null && requestModel.status.Count > 0)
            {
                query = query.Where(x => requestModel.status.Contains(x.status));
            }

            if (requestModel.technician_ids != null && requestModel.technician_ids.Count > 0)
            {
                var wo_ids = context.WorkOrderTechnicianMapping.Where(x => requestModel.technician_ids.Contains(x.user_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_deleted).Select(x => x.wo_id).ToList();
                query = query.Where(x => wo_ids.Contains(x.wo_id));
            }

            if (requestModel.lead_ids != null && requestModel.lead_ids.Count > 0)
            {
                var wo_ids = context.WorkOrderBackOfficeUserMapping.Where(x => requestModel.lead_ids.Contains(x.user_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_deleted).Select(x => x.wo_id).ToList();
                query = query.Where(x => wo_ids.Contains(x.wo_id));
            }
            if (requestModel.vendor_ids != null && requestModel.vendor_ids.Count > 0)
            {
                var wo_ids = context.WorkordersVendorContactsMapping.Where(x => requestModel.vendor_ids.Contains(x.vendor_id.Value) && !x.is_deleted).Select(x => x.wo_id).ToList();
                query = query.Where(x => wo_ids.Contains(x.wo_id));
            }

            if (requestModel.vendor_ids != null && requestModel.vendor_ids.Count > 0)
            {
                var wo_ids = context.WorkordersVendorContactsMapping.Where(x => requestModel.vendor_ids.Contains(x.vendor_id.Value) && !x.is_deleted).Select(x => x.wo_id).ToList();
                query = query.Where(x => wo_ids.Contains(x.wo_id));
            }

            query = query.Include(x => x.WorkOrderTechnicianMapping).ThenInclude(x => x.TechnicianUser)
                .Include(x => x.WorkOrderBackOfficeUserMapping).ThenInclude(x => x.BackOfficeUser)
                .Include(x => x.WorkordersVendorContactsMapping).ThenInclude(x => x.Vendors)
                .Include(x => x.ResponsibleParty);

            /*//return context.WorkOrders.Where(x => ((x.start_date.Date >= requestModel.start_date.Date && x.start_date.Date <= requestModel.end_date.Date)
            //|| (x.due_at.Date >= requestModel.start_date.Date && x.due_at.Date <= requestModel.end_date.Date))
            //&& !x.is_archive && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id))
            //    .Include(x => x.WorkOrderTechnicianMapping).ThenInclude(x=>x.TechnicianUser)
            //    .Include(x => x.WorkOrderBackOfficeUserMapping).ThenInclude(x => x.BackOfficeUser)
            //    .Include(x => x.ResponsibleParty)
            //    .ToList();*/

            return query.ToList();
        }

        public List<WOlineIssueImagesMapping> GetWOlineIssueImages(List<Guid> requested_deleted_image_mapping_ids)
        {
            return context.WOlineIssueImagesMapping.Where(x => requested_deleted_image_mapping_ids.Contains(x.woline_issue_image_mapping_id)).ToList();
        }

        public TempFormIOBuildings GetTempBuilding(string building, Guid wo_id)
        {
            return context.TempFormIOBuildings.Where(x => x.temp_formio_building_name.ToLower().Trim() == building.ToLower().Trim()
                                                        && x.wo_id == wo_id
                                                        && !x.is_deleted
                                                        ).FirstOrDefault();
        }
        public TempFormIOBuildings GetTempBuildingAsnotracking(string building, Guid wo_id)
        {
            return context.TempFormIOBuildings.Where(x => x.temp_formio_building_name.ToLower().Trim() == building.ToLower().Trim()
                                                        && x.wo_id == wo_id
                                                        && !x.is_deleted
                                                        ).AsNoTracking().FirstOrDefault();
        }
        public TempFormIOFloors GetTempFloor(string floor, Guid temp_formiobuilding_id, Guid wo_id)
        {
            return context.TempFormIOFloors.Where(x => x.temp_formio_floor_name.ToLower().Trim() == floor.ToLower().Trim()
                                                        && x.wo_id == wo_id
                                                        && x.temp_formiobuilding_id == temp_formiobuilding_id
                                                        && !x.is_deleted
                                                        ).FirstOrDefault();
        }
        public TempFormIOFloors GetTempFloorAsnotracking(string floor, Guid temp_formiobuilding_id, Guid wo_id)
        {
            return context.TempFormIOFloors.Where(x => x.temp_formio_floor_name.ToLower().Trim() == floor.ToLower().Trim()
                                                        && x.wo_id == wo_id
                                                        && x.temp_formiobuilding_id == temp_formiobuilding_id
                                                        && !x.is_deleted
                                                        ).AsNoTracking().FirstOrDefault();
        }
        public TempFormIORooms GetTempRoom(string room, Guid temp_formiofloor_id, Guid wo_id)
        {
            return context.TempFormIORooms.Where(x => x.temp_formio_room_name.ToLower().Trim() == room.ToLower().Trim()
                                                        && x.wo_id == wo_id
                                                        && x.temp_formiofloor_id == temp_formiofloor_id
                                                        && !x.is_deleted
                                                        ).FirstOrDefault();
        }
        public TempFormIORooms GetTempRoomAsnotracking(string room, Guid temp_formiofloor_id, Guid wo_id)
        {
            return context.TempFormIORooms.Where(x => x.temp_formio_room_name.ToLower().Trim() == room.ToLower().Trim()
                                                        && x.wo_id == wo_id
                                                        && x.temp_formiofloor_id == temp_formiofloor_id
                                                        && !x.is_deleted
                                                        ).AsNoTracking().FirstOrDefault();
        }
        public TempFormIOSections GetTempSection(string section, Guid temp_formioroom_id, Guid wo_id)
        {
            return context.TempFormIOSections.Where(x => x.temp_formio_section_name.ToLower().Trim() == section.ToLower().Trim()
                                                        && x.wo_id == wo_id
                                                        && x.temp_formioroom_id == temp_formioroom_id
                                                        && !x.is_deleted
                                                        ).FirstOrDefault();
        }
        public TempFormIOSections GetTempSectionAsnotracking(string section, Guid temp_formioroom_id, Guid wo_id)
        {
            return context.TempFormIOSections.Where(x => x.temp_formio_section_name.ToLower().Trim() == section.ToLower().Trim()
                                                        && x.wo_id == wo_id
                                                        && x.temp_formioroom_id == temp_formioroom_id
                                                        && !x.is_deleted
                                                        ).AsNoTracking().FirstOrDefault();
        }
        public int GetTempRoomCountForWorkOrder(Guid wo_id)
        {
            return context.TempAsset.Where(x => !x.is_deleted && x.wo_id == wo_id).Select(x => x.temp_formioroom_id).Distinct().Count();
            //return context.TempFormIORooms.Where(x => x.wo_id == wo_id && !x.is_deleted).Count();
        }

        public int GetOBWOAssetCountByWOType(int wo_type)
        {
            return context.WOOnboardingAssets.Include(x => x.WorkOrders).Where(x => !x.is_woline_from_other_inspection && x.WorkOrders.wo_type == wo_type && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                && !x.is_deleted
                //&& (x.status == (int)Status.Ready_for_review || x.status == (int)Status.Completed || x.status == (int)Status.Submitted)) // old flow
                && (x.status == (int)Status.Ready_for_review)) // new flow : now only RFR count will return
                .Count();
        }

        public WOOnboardingAssets GetInstallWOlinefromTempAssetId(Guid tempasset_id)
        {
            var temp_asset = context.TempAsset.Where(x => x.tempasset_id == tempasset_id)
                                                .Include(x => x.WOOnboardingAssets)
                                                .FirstOrDefault();

            var install_woline = temp_asset.WOOnboardingAssets.Where(x => x.inspection_type == (int)MWO_inspection_wo_type.OnBoarding && !x.is_deleted).FirstOrDefault();

            return install_woline;
        }

        public TempAsset GetTempAssetbyId(Guid tempasset_id)
        {
            return context.TempAsset.Where(x => x.tempasset_id == tempasset_id).FirstOrDefault();
        }

        public TempAsset GetTempAssetByMainAssetId(Guid asset_id, Guid wo_id)
        {
            return context.TempAsset.Where(x => x.asset_id == asset_id && x.wo_id == wo_id).FirstOrDefault();
        }
        public TempAsset GetTempAssetBywoline(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Include(x => x.TempAsset)
                .Select(x => x.TempAsset)
                .FirstOrDefault();
        }

        public TempAsset GetTempAssetForDelete(Guid tempasset_id)
        {
            return context.TempAsset.Where(x => x.tempasset_id == tempasset_id)
                .Include(x => x.WOOnboardingAssets).FirstOrDefault();
        }
        public WOOnboardingAssets GetWolineForTempAssetdelete(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Include(x => x.WOlineTopLevelcomponentMapping)
                .Include(x => x.WOlineSubLevelcomponentMapping)
                .Include(x => x.WOOBAssetFedByMapping)
                .FirstOrDefault();
        }

        public List<WOOnboardingAssets> GetWOSublevelByIds(List<Guid> woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => woonboardingassets_id.Contains(x.woonboardingassets_id)).ToList();
        }

        public WOOnboardingAssets IsInstallWOlineExist(Guid wo_id, Guid asset_id)
        {
            return context.WOOnboardingAssets.Where(x => x.asset_id == asset_id && x.wo_id == wo_id && x.inspection_type == (int)MWO_inspection_wo_type.OnBoarding && !x.is_deleted).FirstOrDefault();
        }

        public List<string> GetWorkorderWatcherByWOId(Guid wo_id)
        {
            return context.WorkOrderWatcherUserMapping.Where(x => x.ref_id == wo_id && x.ref_type == (int)WatcherRefType.Workorder && !x.is_deleted).Select(x => x.user_id.ToString()).ToList();
        }

        public bool CheckUserIsWatcherOrNot(Guid wo_id, Guid user_id)
        {
            return context.WorkOrderWatcherUserMapping.Where(x => x.ref_id == wo_id && x.user_id == user_id
            && x.ref_type == (int)WatcherRefType.Workorder && !x.is_deleted).Any();
        }
        public List<WorkOrders> GetAllWorkordersForScheduler()
        {
            return context.WorkOrders.Where(x => x.due_at != null && x.due_at != DateTime.MinValue
            && !x.is_archive && x.status != (int)Status.Completed)
                .ToList();
        }


        public int GetAssetIssueCountBySite()
        {
            return context.AssetIssue.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_deleted && x.issue_status != (int)Status.Completed).Count();
        }
        public int GetWorkorderCountBySite()
        {
            return context.WorkOrders.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_archive && x.status != (int)Status.Completed).Count();
        }
        public int GetEquipmentCount()
        {
            return context.Equipment.Where(x => x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && !x.isarchive).Count();
        }

        public List<TempAsset> GetAllTempAssetsByWOId(Guid wo_id)
        {
            return context.TempAsset.Where(x => x.wo_id == wo_id && !x.is_deleted).Include(x=>x.SitewalkthroughTempPmEstimation).ToList();
        }
        public WOLineIssue GetTempIssueforOfflineUpdate(Guid wo_line_issue_id)
        {
            return context.WOLineIssue.Where(x => x.wo_line_issue_id == wo_line_issue_id).FirstOrDefault();
        }
        public WOlineIssueImagesMapping GetTempIssueImageforOfflineUpdate(Guid woline_issue_image_mapping_id)
        {
            return context.WOlineIssueImagesMapping.Where(x => x.woline_issue_image_mapping_id == woline_issue_image_mapping_id).FirstOrDefault();
        }
        public List<WorkOrders> GetAllWOforTempassetscript()
        {
            return context.WorkOrders.Where(x => !x.is_archive && x.wo_type == (int)Status.Maintenance_WO
            //&& x.wo_id == Guid.Parse("f39d640f-807c-446d-87ac-dd53b5c2306e")
            && x.Sites.company_id == Guid.Parse("f1c579ce-1571-47fd-8d4b-6e3e35df3eff")
            ).ToList();
        }
        public List<WOOnboardingAssets> GetAllwolinesforScript(Guid wo_id)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && !x.is_deleted && x.tempasset_id == null && x.inspection_type == (int)MWO_inspection_wo_type.OnBoarding)
                .Include(x => x.Sites)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping)
                .ToList();
        }
        public List<WOOnboardingAssets> GetAllissuewolinesforScript(Guid wo_id)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && !x.is_deleted
            // && x.woonboardingassets_id == Guid.Parse("50ce0d5e-4ae7-4418-970f-fb9fe62e4006")
            //&& x.Sites.company_id != Guid.Parse("f1c579ce-1571-47fd-8d4b-6e3e35df3eff")
            && x.tempasset_id == null && (
                                        x.inspection_type == (int)MWO_inspection_wo_type.Repair
                                    || x.inspection_type == (int)MWO_inspection_wo_type.Replace
                                    || x.inspection_type == (int)MWO_inspection_wo_type.Trouble_Call_Check
                                    ))
                .Include(x => x.Sites)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping)
                .Include(x => x.AssetIssue)
                .ToList();
        }
        public List<WOOnboardingAssets> GetAllwolinesforIssueScript(Guid wo_id)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && !x.is_deleted && x.tempasset_id == null && x.inspection_type != (int)MWO_inspection_wo_type.OnBoarding)
                .Include(x => x.Sites)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping)
                .Include(x => x.AssetIssue)
                .ToList();
        }
        public WOOnboardingAssets GetInstallwOlinebyassetid(Guid asset_id, Guid wo_id)
        {
            return context.WOOnboardingAssets.Where(x => x.asset_id == asset_id && x.wo_id == wo_id && x.inspection_type == (int)MWO_inspection_wo_type.OnBoarding && !x.is_deleted).FirstOrDefault();
        }
        public InspectionTemplateAssetClass GetAssetclassbycodefroscript(string class_code, Guid company_id)
        {
            return context.InspectionTemplateAssetClass.Where(x => x.asset_class_code.ToLower() == class_code.ToLower() && x.company_id == company_id && !x.isarchive).FirstOrDefault();
        }
        public TempAsset GetTempAssetbyAssetid(Guid asset_id, Guid wo_id)
        {
            return context.TempAsset.Where(x => x.asset_id == asset_id && x.wo_id == wo_id && !x.is_deleted).FirstOrDefault();
        }

        public WOOnboardingAssets GetMainIssueWOlineForScript(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id).AsNoTracking().FirstOrDefault();
        }
        public TempFormIOBuildings GetTempFormIOBuildingByNameForScript(string building_name, Guid wo_id)
        {
            return context.TempFormIOBuildings.Where(x => x.temp_formio_building_name.Trim().ToLower() == building_name.Trim().ToLower() && x.wo_id == wo_id && !x.is_deleted).FirstOrDefault();
        }

        public TempFormIOFloors GetTempFormIOFloorByNameForScript(string floor_name, Guid temp_building_id, Guid wo_id)
        {
            return context.TempFormIOFloors.Where(x => x.temp_formio_floor_name.Trim().ToLower() == floor_name.Trim().ToLower() && x.wo_id == wo_id && !x.is_deleted && x.temp_formiobuilding_id == temp_building_id).FirstOrDefault();
        }
        public TempFormIORooms GetTempFormIORoomByNameForScript(string room_name, Guid temp_floor_id, Guid wo_id)
        {
            return context.TempFormIORooms.Where(x => x.temp_formio_room_name.Trim().ToLower() == room_name.Trim().ToLower() && x.wo_id == wo_id && !x.is_deleted && x.temp_formiofloor_id == temp_floor_id).FirstOrDefault();
        }
        public TempFormIOSections GetTempFormIOSectionByNameForScript(string section_name, Guid temp_room_id, Guid wo_id)
        {
            return context.TempFormIOSections.Where(x => x.temp_formio_section_name.Trim().ToLower() == section_name.Trim().ToLower() && x.wo_id == wo_id && !x.is_deleted).FirstOrDefault();
        }
        public WOlineSubLevelcomponentMapping GetSublevelMappping(Guid woonboardingassets_id, Guid sublevel_woonboardingassets_id)
        {
            return context.WOlineSubLevelcomponentMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id && x.sublevelcomponent_asset_id == sublevel_woonboardingassets_id
            && !x.is_deleted
            ).FirstOrDefault();
        }

        public WOLineIssue GetTempIssueByWOline(Guid woonboardingassets_id)
        {
            return context.WOLineIssue.Where(x => x.woonboardingassets_id == woonboardingassets_id && !x.is_deleted).FirstOrDefault();
        }

        public AssetPMs GetIRAssetPMsByAssetId(Guid asset_id)
        {
            return context.AssetPMs.Where(x => x.asset_id == asset_id && !x.is_archive 
            && x.title.Replace(" ", "").ToLower().Contains("infraredthermography") && x.status != (int)Status.Completed
            && x.pm_due_overdue_flag != (int)pm_due_overdue_flag.PM_Overdue).OrderBy(c => c.datetime_starting_at)
                .FirstOrDefault();
        }
        public AssetPMs GetVisualAssetPMsByAssetId(Guid asset_id)
        {
            return context.AssetPMs.Where(x => x.asset_id == asset_id && !x.is_archive 
            && x.title.Replace(" ", "").ToLower().Contains("visualinspection") && x.status != (int)Status.Completed
            && x.pm_due_overdue_flag != (int)pm_due_overdue_flag.PM_Overdue).OrderBy(c => c.datetime_starting_at)
                .FirstOrDefault();
        }

        public TimeMaterials GetTimeMaterialById(Guid time_material_id)
        {
            return context.TimeMaterials.Where(x => x.time_material_id == time_material_id).FirstOrDefault();
        }

        public (List<TimeMaterials>, int, double) GetAllTimeMaterialsForWO(GetAllTimeMaterialsForWORequestModel requestModel)
        {
            IQueryable<TimeMaterials> query = context.TimeMaterials.Where(x => x.wo_id == requestModel.wo_id && !x.is_deleted);

            if (requestModel.time_material_category_type != null)
            {
                query = query.Where(x => x.time_material_category_type == requestModel.time_material_category_type);
            }

            if (!String.IsNullOrEmpty(requestModel.search_string))
            {
                string search_str = requestModel.search_string.ToLower().Trim();
                query = query.Where(x => x.description.ToLower().Trim().Contains(search_str));
            }

            int listsize = query.Count();
            var sum_of_all_markup_amount = query.Sum(x => x.total_of_markup);

            query = query.OrderByDescending(x => x.created_at);

            if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
            {
                query = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
            }

            return (query.ToList(), listsize, sum_of_all_markup_amount);
        }

        public int GetTimeMaterialCountByType(Guid wo_id, int time_material_category_type)
        {
            return context.TimeMaterials.Where(x => x.wo_id == wo_id && x.time_material_category_type == time_material_category_type
                    && !x.is_deleted).Count();
        }

        public int GetAllTimeMaterialCountByWOId(Guid wo_id)
        {
            return context.TimeMaterials.Where(x => x.wo_id == wo_id && !x.is_deleted).Count();
        }

        public List<User> GetAllWatcherOfWorkorder(Guid wo_id)
        {
            var watchers = context.WorkOrderWatcherUserMapping.Where(x => x.ref_type == (int)WatcherRefType.Workorder
                && x.ref_id == wo_id && !x.is_deleted).Select(x => x.user_id).ToList();

            return context.User.Where(x => watchers.Contains(x.uuid) && x.status == (int)Status.Active).ToList();
        }

        public WOOnboardingAssetsImagesMapping GetWOOBImageById(Guid woonboardingassetsimagesmapping_id)
        {
            return context.WOOnboardingAssetsImagesMapping.Where(x => x.woonboardingassetsimagesmapping_id == woonboardingassetsimagesmapping_id).FirstOrDefault();
        }

        public WorkOrders GetWObyIDForOfflineIssue(Guid wo_id)
        {
            return context.WorkOrders.Where(x => x.wo_id == wo_id).FirstOrDefault();
        }

        public WOOnboardingAssets GetOBWOAssetByName(string wo_id, string name)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == Guid.Parse(wo_id) && x.asset_name == name && !x.is_deleted).FirstOrDefault();
        }
        public WOlineSubLevelcomponentMapping GetWolineSublevelAssetMappingById(Guid sublevelcomponent_asset_id, Guid woonboardingassets_id)
        {
            return context.WOlineSubLevelcomponentMapping.Where(x => x.sublevelcomponent_asset_id == sublevelcomponent_asset_id
                    && x.woonboardingassets_id == woonboardingassets_id && !x.is_deleted)
                .Include(x => x.WOOnboardingAssetsImagesMapping).FirstOrDefault();
        }

        public List<WOOnboardingAssetsImagesMapping> GetWOOBAssetImagesById(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssetsImagesMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id && !x.is_deleted).ToList();
        }

        public List<AssetProfileImages> GetMainAssetImagesById(Guid asset_id)
        {
            return context.AssetProfileImages.Where(x => x.asset_id == asset_id
            && (x.asset_photo_type == (int)AssetPhotoType.Nameplate_Photo ||
            x.asset_photo_type == (int)AssetPhotoType.Exterior_Photo || x.asset_photo_type == (int)AssetPhotoType.Additional_Photos)
            && !x.is_deleted).ToList();
        }
        public List<AssetProfileImages> GetMainAssetImagesByWOLineId(Guid woonboardingassets_id)
        {
            var ob_asset = context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id && !x.is_deleted).FirstOrDefault();
            var asset_id = ob_asset != null && ob_asset.asset_id != null ? ob_asset.asset_id : Guid.Empty;

            return context.AssetProfileImages.Where(x => x.asset_id == asset_id
            && (x.asset_photo_type == (int)AssetPhotoType.Nameplate_Photo || x.asset_photo_type == (int)AssetPhotoType.Asset_Profile
            || x.asset_photo_type == (int)AssetPhotoType.Exterior_Photo || x.asset_photo_type == (int)AssetPhotoType.Additional_Photos)
            && !x.is_deleted).ToList();
        }

        public InspectionTemplateAssetClass GetAssetClassByClasscode(string class_code)
        {
            return context.InspectionTemplateAssetClass.Where(x => x.asset_class_code.ToLower().Trim() == class_code.ToLower().Trim()
                                                                && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)
                                                                && !x.isarchive
                                                                ).Include(x => x.FormIOType)
                                                                .FirstOrDefault();
        }

        public string GetOBWOAssetNameByID(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Select(x => x.asset_name).FirstOrDefault();
        }

        public Guid? GetOBWOAssetByNameClass(string wo_id, string name, string class_code)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == Guid.Parse(wo_id) && x.asset_name == name
            && x.asset_class_code == class_code && !x.is_deleted)
                .Select(x => x.woonboardingassets_id).FirstOrDefault();
        }
        public WOOnboardingAssets GetWOlinebyIdforOperatingcondition(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Include(x => x.WOLineIssue)
                .Include(x => x.TempAsset)
                .FirstOrDefault();
        }
        public DeviceInfo GetdeviceInfoById(Guid device_uuid)
        {
            return context.DeviceInfo.Where(x => x.device_uuid == device_uuid).FirstOrDefault();
        }

        public string GetAssetClassTypeById(Guid inspectiontemplate_asset_class_id)
        {
            return context.InspectionTemplateAssetClass.Where(x => x.inspectiontemplate_asset_class_id == inspectiontemplate_asset_class_id).Select(x => x.FormIOType.form_type_name).FirstOrDefault();
        }

        public List<IRWOImagesLabelMapping> GetIRImageByname(string image_name, Guid wo_id)
        {
            return context.IRWOImagesLabelMapping.Where(x => (x.ir_image_label.ToLower().Trim() == image_name || x.visual_image_label.ToLower().Trim() == image_name) && x.WOOnboardingAssets.wo_id == wo_id).ToList();
        }
        public List<WOOnboardingAssets> GetAllIRPMWOline(Guid wo_id)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && x.inspection_type == (int)MWO_inspection_wo_type.PM && !x.is_deleted)
                .Include(x => x.ActiveAssetPMWOlineMapping).ThenInclude(x => x.AssetPMs)
                .ToList();
        }


        public WOOnboardingAssets GetOBSublevelAssetById(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Include(x => x.TempAsset)
                .Include(x => x.WOLineBuildingMapping)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections).FirstOrDefault();
        }
        public bool CheckIsClassAvailableOrNot(List<string> class_code_list)
        {
            var query = context.InspectionTemplateAssetClass
                .Where(x => x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)
                && !x.isarchive).Select(x => x.asset_class_code.ToLower().Trim());

            return class_code_list.Any(y => !query.Contains(y.ToLower().Trim()));
        }
        public AssetSubLevelcomponentMapping GetAssetSublevelMappingBySubId(Guid sublevelcomponent_asset_id)
        {
            return context.AssetSubLevelcomponentMapping.Where(x => x.sublevelcomponent_asset_id == sublevelcomponent_asset_id && !x.is_deleted).FirstOrDefault();
        }

        public TrackMobileSyncOffline GetOfflineRequestTrackData(Guid trackmobilesyncoffline_id)
        {
            return context.TrackMobileSyncOffline.Where(x => x.trackmobilesyncoffline_id == trackmobilesyncoffline_id).FirstOrDefault();
        }

        public WOOnboardingAssets GetWOlineForcompletestatus(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.WorkOrders.wo_type == (int)Status.IR_Scan_WO && x.status == (int)Status.Ready_for_review
            && x.woonboardingassets_id == woonboardingassets_id
            )
                .Include(x => x.WOLineIssue)
                .FirstOrDefault();
        }

        public List<WOOnboardingAssets> GetOBWOAssetsByIDs(List<Guid> woonboardingassets_id_list)
        {
            return context.WOOnboardingAssets.Where(x => woonboardingassets_id_list.Contains(x.woonboardingassets_id) && !x.is_deleted).ToList();
        }

        public List<ResponsibleParty> GetAllResponsiblePartyList()
        {
            return context.ResponsibleParty.Where(x => !x.is_deleted).ToList();
        }

        public List<WOOnboardingAssets> GetAllOBAssetsWithQRCodeByWOId(string wo_id)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == Guid.Parse(wo_id) && !x.is_woline_from_other_inspection && !x.is_deleted)
                .Include(x => x.TempAsset).Include(x => x.Asset).ToList();
        }

        public int GetAssetsCountByLocation(int building_id, int floor_id, int room_id)
        {
            int count = 0;

            if (building_id > 0 && floor_id > 0 && room_id > 0)
            {
                count = context.Assets.Include(x => x.AssetTopLevelcomponentMapping).Include(x => x.AssetFormIOBuildingMappings)
                    .Where(x => x.status == (int)Status.AssetActive && x.component_level_type_id > 0
                        && (x.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent
                        || (x.component_level_type_id == (int)ComponentLevelTypes.SublevelComponent && x.AssetTopLevelcomponentMapping.Where(x => !x.is_deleted).Count() > 0))
                        && x.AssetFormIOBuildingMappings.formiobuilding_id == building_id
                        && x.AssetFormIOBuildingMappings.formiofloor_id == floor_id
                        && x.AssetFormIOBuildingMappings.formioroom_id == room_id)
                        .Count();
            }
            else if (building_id > 0 && floor_id > 0 && room_id == -1)
            {
                count = context.Assets.Include(x => x.AssetTopLevelcomponentMapping).Include(x => x.AssetFormIOBuildingMappings)
                    .Where(x => x.status == (int)Status.AssetActive && x.component_level_type_id > 0
                        && (x.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent
                        || (x.component_level_type_id == (int)ComponentLevelTypes.SublevelComponent && x.AssetTopLevelcomponentMapping.Where(x => !x.is_deleted).Count() > 0))
                        && x.AssetFormIOBuildingMappings.formiobuilding_id == building_id
                        && x.AssetFormIOBuildingMappings.formiofloor_id == floor_id)
                        .Count();
            }
            else if (building_id > 0 && floor_id == -1 && room_id == -1)
            {
                count = context.Assets.Include(x => x.AssetTopLevelcomponentMapping).Include(x => x.AssetFormIOBuildingMappings)
                    .Where(x => x.status == (int)Status.AssetActive && x.component_level_type_id > 0
                        && (x.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent
                        || (x.component_level_type_id == (int)ComponentLevelTypes.SublevelComponent && x.AssetTopLevelcomponentMapping.Where(x => !x.is_deleted).Count() > 0))
                        && x.AssetFormIOBuildingMappings.formiobuilding_id == building_id)
                        .Count();
            }

            return count;
        }
        public WorkOrderBackOfficeUserMapping GetWorkOrderBackOfficeMappingById(Guid wo_backoffice_user_mapping_id)
        {
            return context.WorkOrderBackOfficeUserMapping.Where(x => x.wo_backoffice_user_mapping_id == wo_backoffice_user_mapping_id && !x.is_deleted).FirstOrDefault();
        }

        public Sites GetcompanyURLbySiteId(Guid site_id)
        {
            return context.Sites.Where(x => x.site_id == site_id).Include(x => x.Company).Include(x => x.ClientCompany).FirstOrDefault();
        }

        public User GetUserFirstnameById(Guid user_id)
        {
            return context.User.Where(x => x.uuid == user_id).FirstOrDefault();
        }

        public List<string> GetWOAssignedUserNamesbyId(List<Guid> user_ids)
        {
            return context.User.Where(x => user_ids.Contains(x.uuid)).Select(x => x.firstname).ToList();
        }

        public Guid GetcompantBySiteId(Guid site_id)
        {
            return context.Sites.Where(x => x.site_id == site_id).Select(x => x.company_id).FirstOrDefault();
        }
        public WorkOrders GetWODetailsForUserAssignmentEmail(Guid wo_id)
        {
            return context.WorkOrders.Where(x => x.wo_id == wo_id)
                .Include(x => x.WorkOrderTechnicianMapping).ThenInclude(x => x.TechnicianUser)
                .Include(x => x.WorkOrderBackOfficeUserMapping).ThenInclude(x => x.BackOfficeUser)
                .FirstOrDefault();
        }

        public List<GetIRImageFilePathExclude> GetImagesFilePaths(List<string> file_names, string wo_id)
        {
            return context.IRWOImagesLabelMapping
                .Where(x => (file_names.Contains(x.ir_image_label) || file_names.Contains(x.visual_image_label)) && x.WOOnboardingAssets.wo_id == Guid.Parse(wo_id))
                .Select(x => new GetIRImageFilePathExclude
                {
                    s3_image_folder_name = x.s3_image_folder_name,
                    ir_image_label = x.ir_image_label,
                    visual_image_label = x.visual_image_label
                }).ToList();
        }

        public List<TempFormIOBuildings> GetTempBuildingv2()
        {
            return context.TempFormIOBuildings.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_deleted).Select(x => new TempFormIOBuildings
            {
                temp_formio_building_name = x.temp_formio_building_name,
                temp_formiobuilding_id = x.temp_formiobuilding_id
            }).ToList().GroupBy(b => b.temp_formio_building_name)
                                   .Select(g => g.FirstOrDefault()).ToList();
        }

        public List<TempMasterBuilding> GetTempMasterBuilding(GetAllTempMasterLocationForWORequestModel requestModel)
        {
            var ids = context.TempMasterBuildingWOMapping.Where(x => x.wo_id == requestModel.wo_id.Value && !x.is_deleted).Select(x => x.temp_master_building_id).ToList();

            return context.TempMasterBuilding.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_deleted)//!ids.Contains(x.temp_master_building_id) &&
                .Select(x => new TempMasterBuilding
                {
                    temp_master_building_name = x.temp_master_building_name,
                    temp_master_building_id = x.temp_master_building_id
                }).ToList().GroupBy(b => b.temp_master_building_name)
                                   .Select(g => g.FirstOrDefault()).ToList();
        }

        public List<TempFormIOFloors> GetTempFloorsv2(string building_name)
        {
            return context.TempFormIOFloors.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_deleted && x.TempFormIOBuildings.temp_formio_building_name == building_name).Select(x => new TempFormIOFloors
            {
                temp_formio_floor_name = x.temp_formio_floor_name,
                temp_formiofloor_id = x.temp_formiofloor_id
            }).ToList().GroupBy(b => b.temp_formio_floor_name)
                                   .Select(g => g.FirstOrDefault()).ToList();
        }
        public List<TempMasterFloor> GetTempMasterFloor(GetAllTempMasterLocationForWORequestModel requestModel, string building_name)
        {
            //var ids = context.TempMasterFloorWOMapping.Where(x => x.wo_id == requestModel.wo_id.Value && !x.is_deleted).Select(x => x.temp_master_floor_id).ToList();

            return context.TempMasterFloor.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.TempMasterBuilding.temp_master_building_name == building_name && !x.is_deleted)//!ids.Contains(x.temp_master_floor_id) &&
                .Select(x => new TempMasterFloor
                {
                    temp_master_floor_name = x.temp_master_floor_name,
                    temp_master_floor_id = x.temp_master_floor_id
                }).ToList().GroupBy(b => b.temp_master_floor_name)
                                   .Select(g => g.FirstOrDefault()).ToList();
        }
        public List<TempFormIORooms> GetTempRoomsv2(string floor_name, string building_name)
        {
            return context.TempFormIORooms.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_deleted
            && x.TempFormIOFloors.temp_formio_floor_name == floor_name
            && x.TempFormIOFloors.TempFormIOBuildings.temp_formio_building_name == building_name
            )
            .Select(x => new TempFormIORooms
            {
                temp_formio_room_name = x.temp_formio_room_name,
                temp_formioroom_id = x.temp_formioroom_id
            }).ToList().GroupBy(b => b.temp_formio_room_name)
                                   .Select(g => g.FirstOrDefault()).ToList();
        }
        public List<TempMasterRoom> GetTempMasterRoom(GetAllTempMasterLocationForWORequestModel requestModel, string floor_name, string building_name)
        {
            return context.TempMasterRoom.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_deleted
                          && x.TempMasterFloor.temp_master_floor_name == floor_name
                          && x.TempMasterFloor.TempMasterBuilding.temp_master_building_name == building_name
                          )
                         .Select(x => new TempMasterRoom
                         {
                            temp_master_room_name = x.temp_master_room_name,
                            temp_master_room_id = x.temp_master_room_id
                         }).ToList().GroupBy(b => b.temp_master_room_name)
                           .Select(g => g.FirstOrDefault()).ToList();
        }
        public List<FormIOBuildings> GetMainBuildingv2(List<string> temp_building_names)
        {
            return context.FormIOBuildings.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !temp_building_names.Contains(x.formio_building_name))
            .Select(x => new FormIOBuildings
            {
                formio_building_name = x.formio_building_name,
                formiobuilding_id = x.formiobuilding_id
            }).ToList().GroupBy(b => b.formio_building_name)
                                   .Select(g => g.FirstOrDefault())
                                   .ToList();
        }
        public List<FormIOFloors> GetMainFloorsv2(string building_name, List<string> temp_floor_names)
        {
            return context.FormIOFloors.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !temp_floor_names.Contains(x.formio_floor_name)
            && x.FormIOBuildings.formio_building_name == building_name
            )
            .Select(x => new FormIOFloors
            {
                formio_floor_name = x.formio_floor_name,
                formiofloor_id = x.formiofloor_id
            }).ToList().GroupBy(b => b.formio_floor_name)
                                   .Select(g => g.FirstOrDefault())
                                   .ToList();
        }
        public List<FormIORooms> GetMainRoomsv2(string floor_name, string building_name, List<string> temp_room_names)
        {
            return context.FormIORooms.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !temp_room_names.Contains(x.formio_room_name)
            && x.FormIOFloors.formio_floor_name == floor_name
            && x.FormIOFloors.FormIOBuildings.formio_building_name == building_name
            )
            .Select(x => new FormIORooms
            {
                formio_room_name = x.formio_room_name,
                formioroom_id = x.formioroom_id
            }).ToList().GroupBy(b => b.formio_room_name)
                                   .Select(g => g.FirstOrDefault())
                                   .ToList();
        }

        public List<TempAsset> GetActiveWOlineLocations(Guid wo_id , string search_string)
        {
            IQueryable<TempAsset> query = context.TempAsset.Where(x => x.wo_id == wo_id && !x.is_deleted)
                                                .Include(x => x.TempFormIOBuildings)
                                                .Include(x => x.TempFormIOFloors)
                                                .Include(x => x.TempFormIORooms);

            if (!String.IsNullOrEmpty(search_string))
            {
                search_string = search_string.ToLower().Trim();
                query = query.Where(x => 
                   x.TempFormIOBuildings.temp_formio_building_name.ToLower().Contains(search_string)
                || x.TempFormIOFloors.temp_formio_floor_name.ToLower().Contains(search_string)
                || x.TempFormIORooms.temp_formio_room_name.ToLower().Contains(search_string)
                );
            }

            return query.ToList();
        }
        public List<TempAsset> GetTempAssetsWithTempMasterLocations(GetAllTempMasterLocationForWORequestModel requestModel)
        {
            IQueryable<TempAsset> query = context.TempAsset.Where(x => x.wo_id == requestModel.wo_id && !x.is_deleted);

            query = query.Include(x => x.TempMasterBuilding).Include(x => x.TempMasterFloor).Include(x => x.TempMasterRoom);

            if (!String.IsNullOrEmpty(requestModel.search_string))
            {
                query = query.Include(x => x.TempMasterBuilding).Include(x => x.TempMasterFloor).Include(x => x.TempMasterRoom)
                    .Where(x => (x.TempMasterBuilding.temp_master_building_name.ToLower() == requestModel.search_string.ToLower()
                    || x.TempFormIOFloors.temp_formio_floor_name.ToLower() == requestModel.search_string.ToLower()
                    || x.TempMasterRoom.temp_master_room_name.ToLower() == requestModel.search_string.ToLower()
                    || x.temp_master_section.ToLower() == requestModel.search_string.ToLower()
                    ));
            }

            return query.ToList();
        }
        public bool CheckWOLineExistOrNot(Guid wo_id, Guid asset_id)
        {
            return context.TempAsset.Where(x => x.asset_id == asset_id && x.wo_id == wo_id && !x.is_deleted).Any();
        }
        public string GetWOLineByAssetIdQRCodeWOId(Guid wo_id, Guid asset_id, string qr_code)
        {
            var tempasset = context.TempAsset.Where(x => x.wo_id == wo_id && x.asset_id == asset_id && x.QR_code.ToLower().Trim() == qr_code && !x.is_deleted)
                .Include(x => x.WOOnboardingAssets).FirstOrDefault();
            if (tempasset != null)
            {
                return context.WOOnboardingAssets.Where(x => x.tempasset_id.Value == tempasset.tempasset_id && !x.is_deleted).Select(x => x.woonboardingassets_id.ToString()).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
        public string GettempBuildingNameById(Guid temp_formiobuilding_id)
        {
            return context.TempFormIOBuildings.Where(x => x.temp_formiobuilding_id == temp_formiobuilding_id).Select(x => x.temp_formio_building_name).FirstOrDefault();
        }
        public string GettempFloorNameById(Guid temp_formiofloor_id)
        {
            return context.TempFormIOFloors.Where(x => x.temp_formiofloor_id == temp_formiofloor_id).Select(x => x.temp_formio_floor_name).FirstOrDefault();
        }
        public string GettempRoomNameById(Guid temp_formioroom_id)
        {
            return context.TempFormIORooms.Where(x => x.temp_formioroom_id == temp_formioroom_id).Select(x => x.temp_formio_room_name).FirstOrDefault();
        }
        public string GetTempMasterRoomNameById(Guid temp_master_room_id)
        {
            return context.TempMasterRoom.Where(x => x.temp_master_room_id == temp_master_room_id).Select(x => x.temp_master_room_name).FirstOrDefault();
        }
        public Guid GetNewCreatedAssetID(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id).Select(x => x.TempAsset.new_created_asset_id.Value).FirstOrDefault();
        }
        public List<WorkOrderTechnicianMapping> GetWOAlreadyAssignedTech(List<Guid> already_sent_mail_user, Guid wo_id)
        {
            return context.WorkOrderTechnicianMapping.Where(x => !already_sent_mail_user.Contains(x.user_id) && !x.is_deleted && x.wo_id == wo_id).ToList();
        }
        public List<WorkOrderBackOfficeUserMapping> GetWOAlreadyAssignedBackoffice(List<Guid> already_sent_mail_user, Guid wo_id)
        {
            return context.WorkOrderBackOfficeUserMapping.Where(x => !already_sent_mail_user.Contains(x.user_id) && !x.is_deleted && x.wo_id == wo_id).ToList();
        }
        public TempFormIORooms GetDefaultTempLocation()
        {
            return context.TempFormIORooms.Include(x => x.TempFormIOFloors).ThenInclude(x => x.TempFormIOBuildings)
                .Where(x => x.temp_formio_room_name == "Default" && !x.is_deleted
                && x.TempFormIOFloors.temp_formio_floor_name == "Default" && !x.TempFormIOFloors.is_deleted
                && x.TempFormIOFloors.TempFormIOBuildings.temp_formio_building_name == "Default" && !x.TempFormIOFloors.TempFormIOBuildings.is_deleted
                && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id))
                .FirstOrDefault();
        }

        public WOOnboardingAssetsDateTimeTracking GetWOOnboardingAssetsDateTimeTrackingById(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssetsDateTimeTracking.Where(x => x.woonboardingassets_id == woonboardingassets_id).FirstOrDefault();
        }
        public List<feeding_circuit_list_class> GetOBAssetFeedingCircuitList(Guid woonboardingassets_id)
        {
            List<feeding_circuit_list_class> response = new List<feeding_circuit_list_class>();

            var get_childs = context.WOOBAssetFedByMapping.Where(x => x.parent_asset_id == woonboardingassets_id && !x.is_deleted).Include(x => x.WOOnboardingAssets).ToList();
            //var dynamicform1 = "";
            response = get_childs
               .Select(x => new feeding_circuit_list_class
               {
                   children_asset_id = x.woonboardingassets_id,
                   children_asset_name = x.WOOnboardingAssets.asset_name,
                   via_subcomponent_asset_id = x.via_subcomponant_asset_id,
                   fed_by_via_subcomponant_asset_id = x.fed_by_via_subcomponant_asset_id,

                   fed_by_via_subcomponant_asset_name = x.fed_by_via_subcomponant_asset_id != null ?
                     x.is_fed_by_via_subcomponant_asset_from_ob_wo ? context.WOOnboardingAssets.Where(y => y.woonboardingassets_id == x.fed_by_via_subcomponant_asset_id).Select(y => y.asset_name).FirstOrDefault()
                         : context.Assets.Where(y => y.asset_id == x.fed_by_via_subcomponant_asset_id).Select(y => y.name).FirstOrDefault()
                             : null,

                   via_subcomponent_asset_name = x.via_subcomponant_asset_id != null ?
                     x.is_via_subcomponant_asset_from_ob_wo ? context.WOOnboardingAssets.Where(y => y.woonboardingassets_id == x.via_subcomponant_asset_id).Select(y => y.asset_name).FirstOrDefault()
                         : context.Assets.Where(y => y.asset_id == x.via_subcomponant_asset_id).Select(y => y.name).FirstOrDefault()
                             : null,

                   circuit = x.fed_by_via_subcomponant_asset_id != null ? context.WOlineSubLevelcomponentMapping.Where(z => z.sublevelcomponent_asset_id == x.fed_by_via_subcomponant_asset_id).Select(x => x.circuit).FirstOrDefault() : null,
                   
                   nameplate_json = x.WOOnboardingAssets.form_nameplate_info
               })
               .ToList();
            return response;
        }

        public List<WOlineTopLevelcomponentMapping> GetWOLineToplevelMappingsById(Guid woonboardingassets_id)
        {
            return context.WOlineTopLevelcomponentMapping
                .Where(x => (x.toplevelcomponent_asset_id == woonboardingassets_id || x.woonboardingassets_id == woonboardingassets_id)
                && !x.is_deleted).ToList();
        }
        public List<WOlineSubLevelcomponentMapping> GetWOLineSublevelMappingsById(Guid woonboardingassets_id)
        {
            return context.WOlineSubLevelcomponentMapping.Where(x =>
            (x.sublevelcomponent_asset_id == woonboardingassets_id || x.woonboardingassets_id == woonboardingassets_id) && !x.is_deleted).ToList();
        }
        public List<WOOBAssetFedByMapping> GetWOLineFedByMappingsById(Guid woonboardingassets_id)
        {
            return context.WOOBAssetFedByMapping
                .Where(x => (x.parent_asset_id == woonboardingassets_id || x.woonboardingassets_id == woonboardingassets_id)
            && !x.is_deleted).ToList();
        }

        public int GetIssuesCountByOBWOAssetId(Guid woonboardingassets_id)
        {
            return context.WOLineIssue.Where(x => x.woonboardingassets_id == woonboardingassets_id && !x.is_deleted).Count();
        }
        public TempMasterBuilding GetTempMasterBuildingByName(string building_name)
        {
            return context.TempMasterBuilding.Where(x => x.temp_master_building_name.Trim().ToLower() == building_name.Trim().ToLower() && !x.is_deleted && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public TempMasterBuilding GetTempMasterBuildingByName_V2Script(string building_name,Guid site_id)
        {
            return context.TempMasterBuilding.Where(x => x.temp_master_building_name.Trim().ToLower() == building_name.Trim().ToLower() && !x.is_deleted && x.site_id == site_id).FirstOrDefault();
        }
        public TempMasterFloor GetTempMasterFloorByName(string floor_name, string building_name)
        {
            return context.TempMasterFloor.Include(x => x.TempMasterBuilding)
                .Where(x => x.temp_master_floor_name.Trim().ToLower() == floor_name.Trim().ToLower() && !x.is_deleted && x.TempMasterBuilding.temp_master_building_name.ToLower().Trim() == building_name.ToLower().Trim() && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public TempMasterFloor GetTempMasterFloorByName_V2Script(string floor_name, string building_name,Guid site_id)
        {
            return context.TempMasterFloor.Include(x => x.TempMasterBuilding)
                .Where(x => x.temp_master_floor_name.Trim().ToLower() == floor_name.Trim().ToLower() && !x.is_deleted && x.TempMasterBuilding.temp_master_building_name.ToLower().Trim() == building_name.ToLower().Trim() && x.site_id == site_id).FirstOrDefault();
        }
        public TempMasterRoom GetTempMasterRoomByName(string room_name, string temp_floor_name, string temp_building_name)
        {
            return context.TempMasterRoom.Where(x => x.temp_master_room_name.Trim().ToLower() == room_name.Trim().ToLower() && !x.is_deleted && x.TempMasterFloor.temp_master_floor_name.Trim().ToLower() == temp_floor_name.ToLower().Trim() && x.TempMasterFloor.TempMasterBuilding.temp_master_building_name.ToLower().Trim() == temp_building_name.ToLower().Trim() && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
        public TempMasterRoom GetTempMasterRoomByName_V2Script(string room_name, string temp_floor_name, string temp_building_name,Guid site_id)
        {
            return context.TempMasterRoom.Where(x => x.temp_master_room_name.Trim().ToLower() == room_name.Trim().ToLower() && !x.is_deleted && x.TempMasterFloor.temp_master_floor_name.Trim().ToLower() == temp_floor_name.ToLower().Trim() && x.TempMasterFloor.TempMasterBuilding.temp_master_building_name.ToLower().Trim() == temp_building_name.ToLower().Trim() && x.site_id ==site_id).FirstOrDefault();
        }

        public TempMasterBuildingWOMapping GetTempMasterBuildingWOMappingById(Guid temp_master_building_id, Guid wo_id)
        {
            return context.TempMasterBuildingWOMapping.Where(x => x.temp_master_building_id == temp_master_building_id && x.wo_id == wo_id && !x.is_deleted).FirstOrDefault();
        }
        public TempMasterFloorWOMapping GetTempMasterFloorWOMappingById(Guid temp_master_floor_id, Guid wo_id)
        {
            return context.TempMasterFloorWOMapping.Where(x => x.temp_master_floor_id == temp_master_floor_id && x.wo_id == wo_id && !x.is_deleted).FirstOrDefault();
        }
        public TempMasterRoomWOMapping GetTempMasterRoomWOMappingById(Guid temp_master_room_id, Guid wo_id)
        {
            return context.TempMasterRoomWOMapping.Where(x => x.temp_master_room_id == temp_master_room_id && x.wo_id == wo_id && !x.is_deleted).FirstOrDefault();
        }
        public IRWOImagesLabelMapping GetIRWOImageLabelMappingByName(Guid woonboardingassets_id, string ir_image, string visual_image)
        {
            IQueryable<IRWOImagesLabelMapping> query = context.IRWOImagesLabelMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id && !x.is_deleted);

            if (!String.IsNullOrEmpty(ir_image))
            {
                query = query.Where(x => x.ir_image_label.Contains(ir_image));
            }
            if (!String.IsNullOrEmpty(visual_image))
            {
                query = query.Where(x => x.visual_image_label.Contains(visual_image));
            }

            return query.FirstOrDefault();
        }
            
        public List<AssetPMs> GetSchedulePMsByAssetIds(List<Guid> asset_id)
        {
            return context.AssetPMs.Where(x => asset_id.Contains(x.asset_id) && x.status == (int)Status.Schedule && !x.is_archive &&
            (x.title.Replace(" ", "").ToLower().Contains("visualinspection")
            || x.title.Replace(" ", "").ToLower().Contains("infraredthermography"))).ToList();
        }
        public List<AssetPMs> GetSchedulePMsByWOIdAssetIds(List<Guid> asset_id, Guid wo_id)
        {
            return context.AssetPMs.Where(x => asset_id.Contains(x.asset_id) && x.status == (int)Status.Schedule && x.wo_id == wo_id && !x.is_archive &&
            (x.title.Replace(" ", "").ToLower().Contains("visualinspection")
            || x.title.Replace(" ", "").ToLower().Contains("infraredthermography"))).ToList();
        }
        public List<WOlineIssueImagesMapping> GetIssueImagesByIRVisualId(Guid irwoimagelabelmapping_id)
        {
            return context.WOlineIssueImagesMapping.Where(x => x.irwoimagelabelmapping_id == irwoimagelabelmapping_id && !x.is_deleted).ToList();
        }
        public List<temp_asset_data> GetAllWOOnboardingAssetsByWOId(Guid wo_id)
        {
            List<temp_asset_data> response = new List<temp_asset_data>();

            var query = context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && !x.is_woline_from_other_inspection && !x.is_deleted)
                .Include(x => x.TempAsset)
                .Include(x=>x.TempAsset.TempFormIOBuildings)
                .Include(x=>x.TempAsset.TempFormIOFloors)
                .Include(x=>x.TempAsset.TempFormIORooms)
                .ToList();

            response = query.Select(x => new temp_asset_data
            {
                asset_id = x.asset_id,
                woonboardingassets_id = x.woonboardingassets_id,
                asset_name = x.asset_name,
                asset_class_code = x.asset_class_code,
                asset_class_name = x.asset_class_name,
                building = x.TempAsset.TempFormIOBuildings.temp_formio_building_name,
                floor = x.TempAsset.TempFormIOFloors.temp_formio_floor_name,
                room = x.TempAsset.TempFormIORooms.temp_formio_room_name,
                section = x.TempAsset.TempFormIOSections!=null?x.TempAsset.TempFormIOSections.temp_formio_section_name:"Default",
                location = x.location,
                QR_code = x.QR_code,
                condition_index_type = x.condition_index_type,
                criticality_index_type = x.criticality_index_type,
                asset_operating_condition_state = x.asset_operating_condition_state

            }).ToList();

            return response;
        }

        public List<assets_fedby_mappings_class> GetWOOBFedByMappingsByWOId(Guid wo_id)
        {
            List<assets_fedby_mappings_class> response = new List<assets_fedby_mappings_class>();

            var ids = context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && !x.is_woline_from_other_inspection && !x.is_deleted).Select(x=>x.woonboardingassets_id).ToList();

            var query = context.WOOBAssetFedByMapping.Where(x => ids.Contains(x.woonboardingassets_id.Value) && !x.is_deleted).ToList();
            
            response = query.Select(x => new assets_fedby_mappings_class
            {
                asset_name = context.WOOnboardingAssets.Where(y=>y.woonboardingassets_id==x.woonboardingassets_id.Value).Select(x=>x.asset_name).FirstOrDefault(),
                
                fedby_asset_name = x.is_parent_from_ob_wo ? context.WOOnboardingAssets.Where(y => y.woonboardingassets_id == x.parent_asset_id).Select(x => x.asset_name).FirstOrDefault()
                    : context.Assets.Where(y => y.asset_id == x.parent_asset_id).Select(x => x.name).FirstOrDefault(),
                
                fedby_ocp_asset_name = x.fed_by_via_subcomponant_asset_id!=null ? x.is_fed_by_via_subcomponant_asset_from_ob_wo ? context.WOOnboardingAssets.Where(y => y.woonboardingassets_id == x.fed_by_via_subcomponant_asset_id.Value).Select(x => x.asset_name).FirstOrDefault()
                    : context.Assets.Where(y => y.asset_id == x.fed_by_via_subcomponant_asset_id.Value).Select(x => x.name).FirstOrDefault():null,

                ocp_asset_name = x.via_subcomponant_asset_id!=null ? x.is_via_subcomponant_asset_from_ob_wo ? context.WOOnboardingAssets.Where(y => y.woonboardingassets_id == x.via_subcomponant_asset_id.Value).Select(x => x.asset_name).FirstOrDefault()
                    : context.Assets.Where(y => y.asset_id == x.via_subcomponant_asset_id.Value).Select(x => x.name).FirstOrDefault():null,

                length = x.length,
                style = x.style,
                number_of_conductor = x.number_of_conductor,
                conductor_type_id = x.conductor_type_id,
                raceway_type_id = x.raceway_type_id,
                fed_by_usage_type_id = x.fed_by_usage_type_id
            }).ToList();

            return response;
        }

        public List<asset_subcomponents_mappings_class> GetTopSubComponentMappingsByWOId(Guid wo_id)
        {
            List<asset_subcomponents_mappings_class> response = new List<asset_subcomponents_mappings_class>();

            var ids = context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && x.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent 
                && !x.is_woline_from_other_inspection && !x.is_deleted).Select(x => x.woonboardingassets_id).ToList();

            var query = context.WOlineSubLevelcomponentMapping.Where(x => ids.Contains(x.woonboardingassets_id) && !x.is_deleted).ToList();

            response = query.Select(x => new asset_subcomponents_mappings_class
            {
                toplevel_asset_name = context.WOOnboardingAssets.Where(y => y.woonboardingassets_id == x.woonboardingassets_id).Select(x => x.asset_name).FirstOrDefault(),
                
                subcomponent_asset_name = x.is_sublevelcomponent_from_ob_wo ? context.WOOnboardingAssets.Where(y => y.woonboardingassets_id == x.sublevelcomponent_asset_id).Select(x => x.asset_name).FirstOrDefault()
                    : context.Assets.Where(y => y.asset_id == x.sublevelcomponent_asset_id).Select(x => x.name).FirstOrDefault(),

                subcomponent_asset_class_code = x.is_sublevelcomponent_from_ob_wo ? context.WOOnboardingAssets.Where(y => y.woonboardingassets_id == x.sublevelcomponent_asset_id).Select(x => x.asset_class_code).FirstOrDefault()
                    : context.Assets.Where(y => y.asset_id == x.sublevelcomponent_asset_id).Select(x => x.InspectionTemplateAssetClass.asset_class_code).FirstOrDefault(),

            }).ToList();

            return response;
        }
        public Guid GetCCFromSiteId(Guid site_id)
        {
            return context.Sites.Where(x => x.site_id == site_id).FirstOrDefault().client_company_id.Value;
        }
        public Guid GetCompanyIdFromSiteId(Guid site_id)
        {
            return context.Sites.Where(x => x.site_id == site_id).FirstOrDefault().company_id;
        }
        public List<TempMasterBuilding> GetTempMasterBuildingforOffline(DateTime? sync_time)
        {
            IQueryable<TempMasterBuilding> query = context.TempMasterBuilding;
            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }
        public List<TempMasterFloor> GetTempMasterFloorforOffline(DateTime? sync_time)
        {
            IQueryable<TempMasterFloor> query = context.TempMasterFloor;
            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }
        public List<TempMasterRoom> GetTempMasterRoomforOffline(DateTime? sync_time)
        {
            IQueryable<TempMasterRoom> query = context.TempMasterRoom;
            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }
        public List<TempMasterBuildingWOMapping> GetTempMasterBuildingWOMappingforOffline(DateTime? sync_time)
        {
            IQueryable<TempMasterBuildingWOMapping> query = context.TempMasterBuildingWOMapping;
            query = query.Include(x => x.TempMasterBuilding).Where(x => x.TempMasterBuilding.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }
        public List<TempMasterFloorWOMapping> GetTempMasterFloorWOMappingforOffline(DateTime? sync_time)
        {
            IQueryable<TempMasterFloorWOMapping> query = context.TempMasterFloorWOMapping;
            query = query.Include(x => x.TempMasterFloor).Where(x => x.TempMasterFloor.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }
        public List<TempMasterRoomWOMapping> GetTempMasterRoomWOMappingforOffline(DateTime? sync_time)
        {
            IQueryable<TempMasterRoomWOMapping> query = context.TempMasterRoomWOMapping;
            query = query.Include(x => x.TempMasterRoom).Where(x => x.TempMasterRoom.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }
        public List<TempMasterBuilding> GetAllTempMasterBuildingsListForWO(GetAllTempMasterLocationForWORequestModel requestModel)
        {
            var building_ids = context.TempMasterBuildingWOMapping
                .Where(x => x.wo_id == requestModel.wo_id && !x.is_deleted).Select(x => x.temp_master_building_id).Distinct().ToList();

            IQueryable<TempMasterBuilding> query = context.TempMasterBuilding.Where(x => building_ids.Contains(x.temp_master_building_id) && !x.is_deleted);

            //query = query.Include(x => x.TempMasterFloor).ThenInclude(x => x.TempMasterFloorWOMapping);

        
            if (!String.IsNullOrEmpty(requestModel.search_string))
            {
                string search = requestModel.search_string.ToLower().Trim();
                query = query.Where(x =>
                  x.temp_master_building_name.ToLower().Trim().Contains(search));
            }

            return query.ToList();
        }
        public List<TempMasterFloor> GetAllTempMasterFloorsListForWO(GetAllTempMasterLocationForWORequestModel requestModel)
        {
            var floor_ids = context.TempMasterFloorWOMapping
                .Where(x => x.wo_id == requestModel.wo_id && !x.is_deleted).Select(x => x.temp_master_floor_id).Distinct().ToList();

            IQueryable<TempMasterFloor> query = context.TempMasterFloor.Where(x => floor_ids.Contains(x.temp_master_floor_id) && !x.is_deleted);

            if (!String.IsNullOrEmpty(requestModel.search_string))
            {
                string search = requestModel.search_string.ToLower().Trim();
                query = query.Where(x =>
                  x.temp_master_floor_name.ToLower().Trim().Contains(search));
            }

            return query.ToList();
        }
        public List<TempMasterRoom> GetAllTempMasterRoomsListForWO(GetAllTempMasterLocationForWORequestModel requestModel)
        {
            var room_ids = context.TempMasterRoomWOMapping
                .Where(x => x.wo_id == requestModel.wo_id && !x.is_deleted).Select(x => x.temp_master_room_id).Distinct().ToList();

            IQueryable<TempMasterRoom> query = context.TempMasterRoom.Where(x => room_ids.Contains(x.temp_master_room_id) && !x.is_deleted);

            if (!String.IsNullOrEmpty(requestModel.search_string))
            {
                string search = requestModel.search_string.ToLower().Trim();
                query = query.Where(x =>
                  x.temp_master_room_name.ToLower().Trim().Contains(search));
            }

            return query.ToList();
        }
        public int GetTempMasterLocationCount(Guid wo_id)
        {
            return context.TempMasterBuildingWOMapping.Where(x => x.wo_id == wo_id && !x.is_deleted).Count();
        }
        public List<WorkOrderTechnicianMapping> GetWOTechnicanMappingForOffline(DateTime? sync_time)
        {
            IQueryable<WorkOrderTechnicianMapping> query = context.WorkOrderTechnicianMapping;
            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
            //&& x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by 
            && !x.is_deleted);
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value);
            }
            else
            {
                query = query.Where(x => !x.is_deleted);
            }
            return query.ToList();
        }

        public WorkordersVendorContactsMapping GetWOContactMappingById(Guid workorders_vendor_contacts_mapping_id)
        {
            return context.WorkordersVendorContactsMapping
                .Where(x => x.workorders_vendor_contacts_mapping_id == workorders_vendor_contacts_mapping_id && !x.is_deleted).FirstOrDefault();
        }
        public List<WorkordersVendorContactsMapping> GetWOContactMappingsByVendorId(Guid vendor_id, Guid wo_id)
        {
            return context.WorkordersVendorContactsMapping
                .Where(x => x.vendor_id.Value == vendor_id && x.wo_id == wo_id && !x.is_deleted).ToList();
        }
        public string GetAssetProfileImage(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssetsImagesMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id && x.asset_photo_type == (int)AssetPhotoType.Asset_Profile && !x.is_deleted).Select(x => x.asset_photo).FirstOrDefault();
        }
        public List<WO_Vendor_Contacts_Mapping_View_Class> GetWOContactMappingsByWOId(Guid wo_id)
        {
            var data = context.WorkordersVendorContactsMapping
                .Where(x => x.wo_id == wo_id && !x.is_deleted)
                .Include(x=>x.Vendors).ThenInclude(x=>x.Contacts) // Filter by workorder_id and not deleted
                .ToList().GroupBy(x => new
                {
                    x.vendor_id,
                    x.Vendors.vendor_name,
                    x.Vendors.vendor_email
                }) 
                .Select(group => new WO_Vendor_Contacts_Mapping_View_Class
                {
                    vendor_id = group.Key.vendor_id,
                    vendor_name = group.Key.vendor_name,
                    vendor_email = group.Key.vendor_email,
                    contacts_list = group.Select(x => new Contacts_Data_View_Obj_Class
                    {
                        vendor_id = x.vendor_id.Value,
                        contact_id = x.contact_id,
                        name = x.Contacts.name,
                        email = x.Contacts.email,
                        contact_invite_status = x.contact_invite_status,
                        workorders_vendor_contacts_mapping_id = x.workorders_vendor_contacts_mapping_id
                    }).ToList()
                })
                .ToList();

            return data;
        }

        public List<WorkordersVendorContactsMapping> GetWOVendorMappingByVendorId(Guid vendor_id)
        {
            return context.WorkordersVendorContactsMapping.Where(x=>x.vendor_id==vendor_id && !x.is_deleted).ToList();
        }
        public List<WorkordersVendorContactsMapping> GetWOContactMappingByContactId(Guid contact_id)
        {
            return context.WorkordersVendorContactsMapping.Where(x => x.contact_id == contact_id && !x.is_deleted).ToList();
        }

        public List<WorkordersVendorContactsMapping> GetVendorContactMappings(List<Guid> contacts_id, Guid vendor_id , Guid wo_id)
        {
            return context.WorkordersVendorContactsMapping.Where(x => x.vendor_id == vendor_id && x.wo_id == wo_id && !x.is_deleted && !contacts_id.Contains(x.contact_id)).ToList();
        }
        public List<AssetPMs> GetVisualSchedulePMsByAssetIds(List<Guid> asset_id)
        {
            return context.AssetPMs.Where(x => asset_id.Contains(x.asset_id) && x.status == (int)Status.Schedule && !x.is_archive &&
            (x.title.Replace(" ", "").ToLower().Contains("visualinspection"))).ToList();
        }
        public int GetWorkOrderTypeById(Guid wo_id)
        {
            return context.WorkOrders.Where(x => x.wo_id == wo_id && !x.is_archive).Select(x => x.wo_type).FirstOrDefault();
        }
        public bool IsAssetisAssigned(Guid asset_id)
        {
            var woline = context.WOOnboardingAssets.Where(x => x.asset_id == asset_id && x.status != (int)Status.Completed && !x.is_deleted).FirstOrDefault();
            if (woline != null)
            {
                return true;
            }
            return false;
        }
        public bool IsAssetisAssignedToOtherWOs(Guid asset_id, Guid wo_id)
        {
            var woline = context.WOOnboardingAssets.Include(x => x.WorkOrders)
                .Where(x => x.asset_id == asset_id && x.status != (int)Status.Completed && !x.is_deleted
                && x.wo_id != wo_id && x.WorkOrders.status != (int)Status.Completed && !x.WorkOrders.is_archive).FirstOrDefault();
            if (woline != null)
            {
                return true;
            }
            return false;
        }
        public List<Guid> CheckIfContactsAreAlreadyAdded(Guid wo_id, List<Guid> req_contact_ids)
        {
            var db_contact_ids = context.WorkordersVendorContactsMapping.Where(x=>x.wo_id==wo_id&&!x.is_deleted).Select(x=>x.contact_id).Distinct().ToList();
            var ids = req_contact_ids.Where(x=>!db_contact_ids.Contains(x)).ToList();

            return ids;
        }
        public List<string> GetContactsEmailsByWOId(Guid wo_id)
        {
            var ids = context.WorkordersVendorContactsMapping.Where(x => x.wo_id == wo_id && !x.is_deleted)
                .Select(x => x.contact_id).ToList();

            var lead_ids = context.WorkOrderBackOfficeUserMapping.Where(x => x.wo_id == wo_id && !x.is_deleted)
                .Select(x => x.user_id).ToList();
            var tech_ids = context.WorkOrderTechnicianMapping.Where(x => x.wo_id == wo_id && !x.is_deleted)
                .Select(x => x.user_id).ToList();

            List<Guid> mergedList = lead_ids.Concat(tech_ids).ToList();

            var user_emails = context.User.Where(x=> mergedList.Contains(x.uuid) && x.status==1).Select(x => x.email).Distinct().ToList();
            var con_emails = context.Contacts.Where(x => ids.Contains(x.contact_id)).Select(x => x.email).Distinct().ToList();

            List<string> response = user_emails.Concat(con_emails).ToList();

            return response;
            //return context.Contacts.Where(x => ids.Contains(x.contact_id)).Select(x => x.email).Distinct().ToList();
        }

        public (int,int) GetAcceptRejectCountForWO(Guid wo_id)
        {
            var total_count = context.WorkordersVendorContactsMapping.Where(y =>y.wo_id==wo_id && !y.is_deleted).Count();
            var accept_count = context.WorkordersVendorContactsMapping.Where(y =>y.wo_id==wo_id && !y.is_deleted && y.contact_invite_status == (int)Contact_Invite_Status.Accepted && !y.is_deleted).Count();
            return (total_count, accept_count);
        }

        public List<vendor_details_class> GetVedorContactsDetailsForWO(Guid wo_id)
        {
            var data = context.WorkordersVendorContactsMapping
                .Where(x => x.wo_id == wo_id && !x.is_deleted)
                .Include(x => x.Vendors).ThenInclude(x => x.Contacts) // Filter by workorder_id and not deleted
                .ToList().GroupBy(x => new
                {
                    x.vendor_id,
                    x.Vendors.vendor_name,
                    x.Vendors.vendor_email
                })
                .Select(group => new vendor_details_class
                {
                    vendor_name = group.Key.vendor_name,
                    vendor_contacts = group.Select(x => new vendor_contact_details_class
                    {
                        vendor_contact_name = x.Contacts.name,
                        vendor_email = x.Contacts.email,
                        vendor_phone = x.Contacts.phone_number
                    }).ToList()
                })
                .ToList();

            return data;
        }

        public List<Guid> CheckForUserEmailFlag(List<Guid> list)
        {
            return context.CompanyFeatureMappings.Where(x => list.Contains(x.user_id.Value) &&
                              x.feature_id.ToString() == GlobalConstants.hide_email_for_user_feature_id && !x.is_required).
                             Select(x => x.user_id.Value).ToList();
        }

        public GetPMEstimationResponseModel GetPMEstimation(GetPMEstimationRequestModel requestModel)
        {
            GetPMEstimationResponseModel responseModel = new GetPMEstimationResponseModel();
            IQueryable<PMEstimation> pmsListQuery = null;

            var defaultPmPlanInfo = context.InspectionTemplateAssetClass
                                       .Where(x => x.inspectiontemplate_asset_class_id == requestModel.class_id)
                                       .SelectMany(x => x.PMCategory.PMPlans)
                                       .Where(plan => plan.is_default_pm_plan)
                                       .Select(plan => new { plan.pm_plan_id, plan.plan_name })
                                       .FirstOrDefault();

            if (defaultPmPlanInfo == null)
            {
                throw new InvalidOperationException("No default PM plan found.");
            }

            if (requestModel.woonboardingassets_id == null)
            {

                 pmsListQuery = context.PMPlans
                                     .Where(plan => plan.pm_plan_id == defaultPmPlanInfo.pm_plan_id)
                                     .SelectMany(plan => plan.PMs)
                                     .Select(pm => new PMEstimation
                                     {
                                         pm_id = pm.pm_id,
                                         title = pm.title,
                                         estimation_time = pm.estimation_time
                                     });

               
                    responseModel.pm_plan_id = defaultPmPlanInfo.pm_plan_id;
                    responseModel.plan_name = defaultPmPlanInfo.plan_name;
                
               
            }
            else
            {
                var Is_existsIn_SiteWalkThroughTempPmEstimation = context.SitewalkthroughTempPmEstimation.Any(x => x.inspectiontemplate_asset_class_id == requestModel.class_id && x.woonboardingassets_id == requestModel.woonboardingassets_id);

                if(Is_existsIn_SiteWalkThroughTempPmEstimation)
                {
                    pmsListQuery = context.SitewalkthroughTempPmEstimation
                                 .Where(x => x.inspectiontemplate_asset_class_id == requestModel.class_id && x.woonboardingassets_id == requestModel.woonboardingassets_id)
                                 .Select(x => new PMEstimation
                                 {
                                     pm_id = x.pm_id,                // Directly from the table
                                     title = x.PMs.title,         // Retrieve title from the related PMs table
                                     estimation_time = x.estimation_time,
                                     sitewalkthrough_temp_pm_estimation_id = x.sitewalkthrough_temp_pm_estimation_id
                                 });
                           
                    var PmPlanInfo = context.SitewalkthroughTempPmEstimation
                                    .Where(x => x.inspectiontemplate_asset_class_id == requestModel.class_id && x.woonboardingassets_id == requestModel.woonboardingassets_id)
                                    .Select(x => new { x.pm_plan_id, x.PMPlans.plan_name })
                                    .FirstOrDefault();

                    if (PmPlanInfo != null)
                    {
                        responseModel.pm_plan_id = PmPlanInfo.pm_plan_id;
                        responseModel.plan_name = PmPlanInfo.plan_name;
                    }
                    
                }
                else
                {
                    pmsListQuery = context.PMPlans
                                         .Where(plan => plan.pm_plan_id == defaultPmPlanInfo.pm_plan_id)
                                         .SelectMany(plan => plan.PMs)
                                         .Select(pm => new PMEstimation
                                         {
                                             pm_id = pm.pm_id,
                                             title = pm.title,
                                             estimation_time = pm.estimation_time
                                         });

                    
                        responseModel.pm_plan_id = defaultPmPlanInfo.pm_plan_id;
                        responseModel.plan_name = defaultPmPlanInfo.plan_name;
                    

                }

            }
            responseModel.pm_estimation_list = pmsListQuery.ToList();

            return responseModel;

        }

        public int GetIRWOImagesCount(Guid wo_id)
        {
            return context.IRWOImagesLabelMapping.Where(x => x.WOOnboardingAssets.wo_id == wo_id && !x.is_deleted).Count();
        }

        public SitewalkthroughTempPmEstimation GetSiteWalkThroughPMEstimationbyID(Guid sitewalkthrough_temp_pm_estimation_id)
        {
            return context.SitewalkthroughTempPmEstimation.Where(x => x.sitewalkthrough_temp_pm_estimation_id == sitewalkthrough_temp_pm_estimation_id).FirstOrDefault();
        }

        public List<SitewalkthroughTempPmEstimation> GetPMEstimationbyWOlineid(Guid woonbowoonboardingassets_id)
        {
            return context.SitewalkthroughTempPmEstimation.Where(x => x.woonboardingassets_id == woonbowoonboardingassets_id && !x.is_deleted).ToList();
        }

       
        public AssetPMs GetCurrentAssetPM(Guid asset_id,Guid pm_id)
        {

           
               return context.AssetPMs.Where(x => x.asset_id == asset_id && !x.is_archive
                    && x.pm_id != null && x.pm_id == pm_id
                    && x.status != (int)Status.Completed
                    && x.pm_due_overdue_flag != (int)pm_due_overdue_flag.PM_Overdue)
                   .OrderBy(x => x.datetime_starting_at).FirstOrDefault();

                
            
        }

       public List<Guid> GetPMIdList(Guid asset_id)
        {
            return context.AssetPMs.Where(x => x.asset_id == asset_id).Select(x => x.pm_id.Value).Distinct().ToList();
        }

        public List<AssetPMs> GetAssetpmsbyAssetId(Guid asset_id)
        {
            return context.AssetPMs.Where(x => x.asset_id == asset_id && !x.is_archive).ToList();
        }

        public (List<WOOnboardingAssets>, int total_list_count) GetTotalOBWOAssetRoomWise(Guid wo_id, string room_name)
        {
            List<WOOnboardingAssets> response = new List<WOOnboardingAssets>();
            int total_list_count = 0;

            IQueryable<WOOnboardingAssets> query = context.WOOnboardingAssets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                                                         && x.wo_id == wo_id && !x.is_deleted);

            query = query.Include(x => x.WOOBAssetTempFormIOBuildingMapping)
                            .Include(x => x.Asset)
                            .Include(x => x.TempAsset)
                            .Include(x => x.StatusMaster)
                            .Include(x => x.WOlineTopLevelcomponentMapping)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms)
                            .Include(x => x.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections)
                            .Include(x => x.TempAsset.TempMasterBuilding)
                            .Include(x => x.TempAsset.TempMasterFloor)
                            .Include(x => x.WOOnboardingAssetsImagesMapping)
                            .Include(x => x.TempAsset.TempMasterRoom);

            if (!String.IsNullOrEmpty(room_name))
            {
                query = query.Where(x => x.TempAsset.TempMasterRoom != null ? x.TempAsset.TempMasterRoom.temp_master_room_name.ToLower().Trim() == room_name.ToLower().Trim() : x.room.ToLower().Trim() == room_name.ToLower().Trim());
            }

          
            query = query.OrderByDescending(x => x.created_at);
            total_list_count = query.Count();


            response = query.ToList();

            return (response, total_list_count);
        }
 
        public SitewalkthroughTempPmEstimation GetSiteWalkThroughTempPMEstimationByID(Guid sitewalkthrough_temp_pm_estimation_id)
        {
            return context.SitewalkthroughTempPmEstimation.Where(x => x.sitewalkthrough_temp_pm_estimation_id == sitewalkthrough_temp_pm_estimation_id && !x.is_deleted).FirstOrDefault();
        }


        public GetPMEstimationResponseModel GetPMEstimationByClassId(Guid class_id)
        {
            GetPMEstimationResponseModel responseModel = new GetPMEstimationResponseModel();
            IQueryable<PMEstimation> pmsListQuery = null;

            var defaultPmPlanInfo = context.InspectionTemplateAssetClass
                                       .Where(x => x.inspectiontemplate_asset_class_id == class_id)
                                       .SelectMany(x => x.PMCategory.PMPlans)
                                       .Where(plan => plan.is_default_pm_plan)
                                       .Select(plan => new { plan.pm_plan_id, plan.plan_name })
                                       .FirstOrDefault();

            if (defaultPmPlanInfo != null)
            { 
                pmsListQuery = context.PMPlans
                                       .Where(plan => plan.pm_plan_id == defaultPmPlanInfo.pm_plan_id)
                                       .SelectMany(plan => plan.PMs)
                                       .Select(pm => new PMEstimation
                                       {
                                           pm_id = pm.pm_id,
                                           title = pm.title,
                                           estimation_time = pm.estimation_time

                                       });


                responseModel.pm_plan_id = defaultPmPlanInfo.pm_plan_id;
                responseModel.plan_name = defaultPmPlanInfo.plan_name;
                responseModel.pm_estimation_list = pmsListQuery.ToList();
            }
            
            return responseModel;
        }

        public Guid? GetTempAssetIdByWoOnboardingAssetId(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id && !x.is_deleted).Select(x => x.tempasset_id).FirstOrDefault();
        }

        public Guid GetPMPlanIdByPMId(Guid pm_id)
        {
            return context.PMs.Where(x => x.pm_id == pm_id && !x.is_archive).Select(x => x.pm_plan_id).FirstOrDefault();
        }
    }
}
