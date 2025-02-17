using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete
{
    public class MobileWorkorderRepository : BaseGenericRepository<WorkOrders>, IMobileWorkorderRepository
    {
        public MobileWorkorderRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
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

                query = query.Where(x => usersites.Contains(x.site_id));
                query = query.Where(x => x.wo_type == (int)Status.Acceptance_Test_WO || x.wo_type == (int)Status.Maintenance_WO);
                /////// if request is for technician then give based on technician user
                ///
                if (requestModel.technician_user_id != null && requestModel.technician_user_id.Value != Guid.Empty)
                {
                    //   var wos = context.WOcategorytoTaskMapping.Where(x => x.technician_user_id == requestModel.technician_user_id && !x.is_archived).Select(x=>x.wo_id).Distinct().ToList();
                    query = query.Where(x => x.status != (int)Status.PlannedWO);
                }
                if (requestModel.from_date != null && requestModel.to_date != null)
                {
                    query = query.Where(s => s.start_date.Date >= requestModel.from_date.Value.Date && s.start_date.Date <= requestModel.to_date.Value.Date);
                }
                if (requestModel.wo_status != null && requestModel.wo_status.Count > 0)
                {
                    query = query.Where(x => requestModel.wo_status.Contains(x.status));
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
                    x.manual_wo_number.Contains(search)
                    );
                }
                if (requestModel.wo_type != null && requestModel.wo_type.Count > 0)
                {
                    query = query.Where(x => requestModel.wo_type.Contains(x.wo_type));
                }
                query = query.OrderByDescending(x => x.created_at);
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
                    .Include(x => x.TechnicianUser)
                    .ToList();
            }
            return (response, total_list_count);
        }

        public WorkOrders ViewWorkOrderDetailsById(Guid wo_id)
        {
            return context.WorkOrders.Where(x => x.wo_id == wo_id && !x.is_archive)
                .Include(x => x.WOInspectionsTemplateFormIOAssignment)
                 .ThenInclude(x => x.InspectionsTemplateFormIO)
                  .ThenInclude(x => x.FormIOType)
                .Include(x => x.StatusMaster)
                .Include(x => x.TechnicianUser)
                .Include(x => x.WOTypeStatusMaster)
                .Include(x => x.Sites)
                .Include(x => x.ClientCompany)
                .Include(x => x.WorkOrderAttachments)

                .FirstOrDefault();
        }
        public List<Tasks> GetAllTasks(string searchstring)
        {
            IQueryable<Tasks> query = context.Tasks;
            query = query.Where(x => (x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id || x.FormIO.is_master_form) && x.FormIO.status == (int)Status.Active).Include(x => x.Asset);
            if (!String.IsNullOrEmpty(searchstring))
            {
                searchstring = searchstring.ToLower();
                query = query.Where(x => !x.isArchive);

                query = query.Where(x =>
                   x.task_title.ToLower().Contains(searchstring)
                || x.task_code.ToString().Contains(searchstring)
                || x.description.ToLower().Contains(searchstring)
                || x.Asset.name.ToLower().Contains(searchstring)
                || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                || x.FormIO.form_name.ToLower().Contains(searchstring)
                );

            }
            else
            {
                query = query.Where(x => (!x.isArchive));
            }
            return query
                .Include(x => x.Asset)
                .Include(x => x.AssetTasks)
                    .ThenInclude(x => x.Asset)
                .Include(x => x.FormIO)
                  .ThenInclude(x => x.FormIOType)
                .OrderByDescending(x => x.created_at).ToList();
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
        public (List<AssetFormIO>, int) GetAllAssetTemplateList(string assetid, int pagesize, int pageindex)
        {
            int total_size = 0;
            List<AssetFormIO> tempList = new List<AssetFormIO>();
            IQueryable<AssetFormIO> query = context.AssetFormIO;

            List<Guid> usersites = new List<Guid>();
            string userid = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
            if (String.IsNullOrEmpty(ViewModels.RequestResponseViewModel.UpdatedGenericRequestmodel.CurrentUser.site_id))
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

            query = query.Where(x => usersites.Contains(x.site_id.Value));
            if (assetid != null && !String.IsNullOrEmpty(assetid))
            {
                query = query.Include(x => x.Asset).ThenInclude(x => x.Sites).Where(x => x.asset_id.ToString() == assetid && (x.status == (int)Status.Ready_for_review || x.status == (int)Status.Completed || x.status == (int)Status.Submitted))
                                                .OrderByDescending(x => x.created_at);
            }
            else
            {
                query = query.Include(x => x.Asset).ThenInclude(x => x.Sites).Where(x => (x.status == (int)Status.Ready_for_review || x.status == (int)Status.Completed)
                                                               && x.requested_by != null)
                                                .OrderByDescending(x => x.created_at);
            }

            total_size = query.Count();

            if (pagesize > 0 && pageindex > 0)
            {
                query = query.Skip((pageindex - 1) * pagesize).Take(pagesize);
            }
            var response = query
                .Include(x => x.Sites)
                             //  .Include(x => x.WorkOrders)
                             //  .ThenInclude(x => x.StatusMaster)
                             // .Include(x => x.WorkOrders).ThenInclude(x => x.Sites)
                             //  .Include(x => x.Asset)
                             //  .ThenInclude(x => x.Sites)
                             //  .Include(x => x.PDFReportStatusMaster)
                             //   .Include(x => x.StatusMaster)
                             //    .Include(x => x.WOcategorytoTaskMapping)
                             .ToList();

          //  var asset_ids = response.Select(q => q.asset_id).ToList();
          //  var asset_details = context.Assets.Where(q => asset_ids.Contains(q.asset_id))
          //      .Include(q => q.Sites)
          //      .ToList();
         //   response.ForEach(q =>
          //  {
          //      q.Asset = asset_details.Where(a => a.asset_id == q.asset_id).FirstOrDefault();
          //  });


            return (response, total_size);
        }
        public ListViewModel<Asset> GetSubAssetsByAssetID(string asset_id, int pagesize, int pageindex)
        {
            ListViewModel<Asset> assets = new ListViewModel<Asset>();

            var children = context.Assets.Where(x => x.asset_id == Guid.Parse(asset_id)).Select(x => x.children).FirstOrDefault();
            if (children != null)
            {
                var childlist = children.Split(',').ToList().Where(x => !String.IsNullOrEmpty(x)).ToList();

                IQueryable<Asset> query = context.Assets.Where(x => childlist.Contains(x.internal_asset_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));

                assets.listsize = query.Count();
                if (pageindex > 0 && pagesize > 0)
                {
                    query = query.Skip((pageindex - 1) * pagesize).Take(pagesize).OrderBy(x => x.name);
                }
                else
                {
                    query = query.OrderBy(x => x.name);
                }
                query = query.Include(x => x.StatusMaster)
                    .Include(x=>x.AssetProfileImages)
                    .Include(x=>x.InspectionTemplateAssetClass)
                    .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOBuildings)
                    .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOFloors)
                    .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIORooms)
                    .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOSections);
                var asste_list = query.ToList();
                assets.list = asste_list;
            }
            assets.pageIndex = pageindex;
            assets.pageSize = pagesize;


            return assets;
        }
        public List<Issue> FilterIssues(FilterIssueRequestModel requestModel)
        {
            List<Issue> issues = new List<Issue>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
                {
                    rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
                }

                List<Guid> assetlist = new List<Guid>();

                IQueryable<Asset> query = context.Assets;

                if (!string.IsNullOrEmpty(rolename))
                {

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

                    if (requestModel.site_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.site_id.Contains(x.site_id.ToString()));
                    }

                    assetlist = query.Select(x => x.asset_id).ToList().Distinct().ToList();

                    issues = context.Issue
                      .Include(x => x.StatusMaster)
                      //Include(x => x.Inspection)
                      //.ThenInclude(x => x.Asset)
                      //.Include(x => x.Asset)
                      .Include(x => x.Sites)
                      .Where(x => assetlist.Contains(x.asset_id)).ToList();

                    // status check
                    if (requestModel.status?.Count > 0)
                    {
                        issues = issues.Where(x => assetlist.Contains(x.asset_id) && requestModel.status.Contains(x.status)).ToList();
                    }

                    // Asset ids check
                    if (requestModel.asset_id?.Count > 0)
                    {
                        issues = issues.Where(x => requestModel.asset_id.Contains(x.asset_id.ToString())).ToList();
                    }

                    // Work Order priority
                    if (requestModel.priority?.Count > 0)
                    {
                        issues = issues.Where(x => requestModel.priority.Contains(x.priority)).ToList();
                    }

                    // Work Order Title
                    if (requestModel.issue_title?.Count > 0)
                    {
                        issues = issues.Where(x => requestModel.issue_title.Contains(x.name)).ToList();
                    }


                    // Search Work Orders
                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        var searchPriority = requestModel.search_string.ToLower();
                        var searchStringLower = new List<int>();


                        DateTime dt = DateTime.ParseExact("2000-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        try
                        {
                            dt = Convert.ToDateTime(requestModel.search_string);
                        }
                        catch
                        {
                            // do nothing;
                        }

                        if (searchPriority.Contains("high"))
                        {
                            searchStringLower.Add((int)WorkOrderPriority.High);
                            if (searchPriority.Contains("very"))
                            {
                                searchStringLower.Add((int)WorkOrderPriority.Very_High);
                            }
                        }
                        else if (searchPriority.Contains("very"))
                        {
                            searchStringLower.Add((int)WorkOrderPriority.Very_High);
                        }
                        else if (searchPriority.Contains("low"))
                        {
                            searchStringLower.Add((int)WorkOrderPriority.Low);
                        }
                        else if (searchPriority.Contains("medium"))
                        {
                            searchStringLower.Add((int)WorkOrderPriority.Medium);
                        }

                        var searchstring = requestModel.search_string.ToLower();
                        DateTime fromdate = new DateTime();
                        DateTime todate = new DateTime();

                        if (!String.IsNullOrEmpty(requestModel.timezone))
                        {
                            requestModel.timezone = requestModel.timezone.Replace('-', '/');
                            //DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(requestModel.timezone));
                            DateTime currentTime = DateTime.Now;
                            var diff = currentTime - DateTime.UtcNow;
                            fromdate = dt.AddHours(diff.Hours).AddMinutes(diff.Minutes);
                            todate = fromdate.AddHours(24);
                            Console.WriteLine("currentdate time " + currentTime);
                        }

                        Console.WriteLine("timezone " + requestModel.timezone);
                        Console.WriteLine("fromdate " + fromdate);
                        Console.WriteLine("todate " + todate);


                        if (dt.Date.ToString("MM/dd/yyyy") == "01/01/2000")
                        {
                            issues = issues.Where(x => (x.name.ToLower().Contains(searchstring.ToLower()) || x.StatusMaster.status_name.ToLower().Contains(searchstring.ToLower()) || x.status.ToString().Contains(searchstring.ToLower())
                                          || x.Inspection.Asset.internal_asset_id.Contains(searchstring.ToLower()) || searchStringLower.Contains(x.priority) || x.Sites.location.Contains(searchstring.ToLower()) || x.Sites.site_name.Contains(searchstring.ToLower())
                                          || x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower()))).ToList();
                        }
                        else
                        {
                            issues = issues.Where(x => (x.name.ToLower().Contains(searchstring.ToLower()) || x.StatusMaster.status_name.ToLower().Contains(searchstring.ToLower()) || x.status.ToString().Contains(searchstring.ToLower())
                                          || x.Inspection.Asset.internal_asset_id.Contains(searchstring.ToLower()) || searchStringLower.Contains(x.priority) || x.Sites.location.Contains(searchstring.ToLower()) || x.Sites.site_name.Contains(searchstring.ToLower())
                                          || x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower()) || (x.created_at.Value >= fromdate && x.created_at.Value <= todate))).ToList();
                        }
                    }
                }
            }
            return issues;
        }

        public ListViewModel<Asset> FilterAssets(FilterAssetsRequestModel requestModel)
        {
            ListViewModel<Asset> assets = new ListViewModel<Asset>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_name))
                {
                    //rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
                    rolename = UpdatedGenericRequestmodel.CurrentUser.role_name;
                }

                if (!string.IsNullOrEmpty(rolename))
                {
                    IQueryable<Asset> query = context.Assets;

                    if (rolename != GlobalConstants.Admin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

                        if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Where(x => usersites.Contains(x.site_id));
                        }
                        else
                        {
                            if (rolename == GlobalConstants.Executive && requestModel.site_id?.Count > 0)
                            {
                                query = query.Where(x => usersites.Contains(x.site_id) && requestModel.site_id.Contains(x.site_id.ToString()));
                            }
                            else
                            {
                                query = query.Where(x => usersites.Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                            }
                        }
                    }
                    else if (rolename == GlobalConstants.Admin)
                    {
                        if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id));
                            }
                            else
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                            }
                        }
                        else if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.AllCompanyType)
                        {
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status != (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                            }
                        }
                    }

                    if (requestModel.status > 0)
                    {
                        if (requestModel.status == (int)Status.Active || requestModel.status == (int)Status.AssetActive)
                        {
                            query = query.Where(x => x.status == (int)Status.AssetActive);
                        }
                        else if (requestModel.status == (int)Status.Deactive || requestModel.status == (int)Status.AssetDeactive)
                        {
                            query = query.Where(x => x.status == (int)Status.AssetDeactive);
                        }
                    }

                    if (requestModel.site_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.site_id.Contains(x.site_id.ToString()));
                    }

                    if (requestModel.company_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.company_id.Contains(x.Sites.company_id.ToString()));
                    }

                    // add asset_id Filter
                    if (requestModel.asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.asset_id.Contains(x.asset_id.ToString()));
                    }

                    // add internal_asset_id Filter
                    if (requestModel.internal_asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.internal_asset_id.Contains(x.internal_asset_id));
                    }
                    // add levels Filter
                    if (requestModel.Levels != null && requestModel.Levels.Count > 0)
                    {
                        query = query.Where(x => requestModel.Levels.Contains(x.levels));
                    }
                    // add model name Filter
                    if (requestModel.model_name?.Count > 0)
                    {
                        query = query.Where(x => requestModel.model_name.Contains(x.model_name));
                    }

                    // add model year Filter
                    if (requestModel.model_year?.Count > 0)
                    {
                        query = query.Where(x => requestModel.model_year.Contains(x.model_year));
                    }

                    // search string
                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        //string search = "search string";
                        //var param = Expression.Parameter(typeof(Asset), "c");
                        //var property = Expression.Property(param, "name");
                        //var expr = Expression.Call(
                        //               typeof(NpgsqlDbFunctionsExtensions),
                        //               nameof(NpgsqlDbFunctionsExtensions.ILike),
                        //               Type.EmptyTypes,
                        //               Expression.Property(null, typeof(EF), nameof(EF.Functions)),
                        //               property,
                        //               Expression.Constant($"%{search}%"));

                        //var list = context.Assets.Where(Expression.Lambda<Func<Asset, Boolean>>(expr, param))
                        //    .ToListAsync();
                        var searchstring = requestModel.search_string.ToLower().ToString();
                        query = query.Where(x => (x.name.ToLower().Contains(searchstring) || x.asset_type.ToLower().Contains(searchstring) || x.Sites.Company.company_name.ToLower().Contains(searchstring) ||
                        x.Sites.site_name.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring)
                        || x.internal_asset_id.ToLower().Contains(searchstring) || x.model_year.Contains(searchstring)));
                    }

                    // only show if issues open
                    if (requestModel.show_open_issues == 1)
                    {
                        query = query.Where(x => x.Issues.Where(x => x.status == (int)Status.New || x.status == (int)Status.InProgress || x.status == (int)Status.Waiting).Count() > 0);
                    }

                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }

                    assets.listsize = query.Count();
                    query = query.OrderBy(x => x.name).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).OrderBy(x => x.name);
                    //assets.list = query.Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Issues).Include(x => x.Inspection).ThenInclude(x => x.StatusMaster).ToList();

                    assets.list = query.Include(x => x.Sites)
                                      // .Include(y => y.Sites.Company)
                                       .Include(x => x.StatusMaster)
                                       //.Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                                       //.Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                                      // .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                                      // .Include(x => x.AssetFormIOBuildingMappings.FormIOSections)
                                     //  .Include(x => x.AssetFormIOBuildingMappings.FormIOSections.FormIOLocationNotes)
                                       //.Include(x => x.Issues)
                                       ///.Include(x => x.Inspection)
                                       //.ThenInclude(x => x.StatusMaster)
                                       .ToList();

                    /*if (assets.list != null && assets.list.Count > 0)
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
                    assets.pageIndex = requestModel.pageindex;
                    assets.pageSize = requestModel.pagesize;
                }
            }
            return assets;
        }

        public Issue GetIssueById(string issueId, string userid)
        {
            var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

            //var assetsitelist = context.Assets.Where(x => usersites.Contains(x.site_id)).Select(x => x.asset_id).Distinct().ToList();

            //return context.WorkOrder.Include(x => x.StatusMaster).Include(x => x.Inspection).Include(x => x.Inspection.Asset).Where(x => assetsitelist.Contains(x.site_id.Value) && x.work_order_uuid.ToString() == workOrderId).FirstOrDefaultAsync().Result;

            return context.Issue
                .Where(x => x.issue_uuid.ToString() == issueId)
                .Include(x => x.Asset).ThenInclude(x => x.Sites)
                .Where(x => usersites.Contains(x.Asset.site_id))
                .Include(x => x.StatusMaster)
                .Include(x => x.Inspection)
                .Include(x => x.Attributes)
                .FirstOrDefault();
        }

        public async Task<AssetFormIO> GetAssetFormIOByAssetFormId(Guid asset_form_id)
        {
            return await context.AssetFormIO.Where(u => u.asset_form_id == asset_form_id).Include(x=>x.WorkOrders).Include(x=>x.WOcategorytoTaskMapping
            ).FirstOrDefaultAsync();
        }
        public (List<AssetPMs>, int) GetAssetPMList(GetAssetPMListMobileRequestmodel requestModel)
        {
            int total_list_count = 0;
            IQueryable<AssetPMs> query = context.AssetPMs.Where(x => !x.is_archive);

            if (requestModel.asset_form_id != null)
            {
                query = query.Where(x => x.asset_form_id == requestModel.asset_form_id);
            }
          
            query = query.OrderByDescending(x => x.created_at);
            total_list_count = query.Count();
            
            if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
            {
                query = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);//.OrderBy(x => x.created_at);
            }
            return (query.ToList(), total_list_count);
        }
        public AssetPMs GetAssetpmById(Guid asset_pm_id)
        {
            return context.AssetPMs.Where(x => x.asset_pm_id == asset_pm_id).FirstOrDefault();
        }

        public DeviceInfo  GetDeviceInfoByuuid(Guid UUID)
        {
            return context.DeviceInfo.Where(x => x.device_uuid == UUID).FirstOrDefault();
        }
    }
}
