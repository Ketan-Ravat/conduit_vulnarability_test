using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete
{
    public class IssueRepository : BaseGenericRepository<Issue>, IIssueRepository
    {
        public IssueRepository(DBContextFactory dataContext) : base(dataContext)
        {
            //this.context = dataContext;
        }


        public virtual async Task<bool> Update(Issue entity)
        {
            bool IsSuccess = false;
            try
            {
                dbSet.Update(entity);
                var response = await context.SaveChangesAsync();
                if (response > 0)
                {
                    IsSuccess = true;
                }
                else
                {
                    IsSuccess = false;
                }
            }
            catch (Exception e)
            {
                IsSuccess = false;
                throw e;
            }
            return IsSuccess;
        }


        public List<Issue> GetIssueByAssetId(string asset_id, int pagesize, int pageindex)
        {
            if (pageindex != 0 && pagesize != 0)
            {
                return context.Issue.Where(x => x.asset_id.ToString() == asset_id).Include(x => x.StatusMaster).ToList().Take(pagesize).Skip((pageindex - 1) * pagesize).ToList();
            }
            else
            {
                return context.Issue.Where(x => x.asset_id.ToString() == asset_id).Include(x => x.StatusMaster).ToList();
            }
        }

        public List<Issue> GetIssueByInternalAssetId(string internal_asset_id, int pagesize = 0, int pageindex = 0)
        {
            if (pageindex != 0 && pagesize != 0)
            {
                return context.Issue.Where(x => x.internal_asset_id == internal_asset_id).Include(x => x.StatusMaster).ToList().Take(pagesize).Skip((pageindex - 1) * pagesize).ToList();
            }
            else
            {
                return context.Issue.Where(x => x.internal_asset_id == internal_asset_id).Include(x => x.StatusMaster).ToList();
            }
        }

        public int CreateIssue(Issue entity)
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

        public List<Issue> GetIssues(string userid, int status, int pagesize, int pageindex)
        {
            List<Issue> issues = new List<Issue>();

            if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
            {
                string role = context.UserRoles.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();

                List<Guid> assetlist = null;
                var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    assetlist = context.Assets.Where(x => usersites.Contains(x.site_id)).Select(x => x.asset_id).ToList().Distinct().ToList();
                }
                else
                {
                    assetlist = context.Assets.Where(x => usersites.Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.asset_id).ToList().Distinct().ToList();
                }

                if (role == GlobalConstants.MS)
                {
                    if (status > 0)
                    {
                        if (status == (int)Status.New)
                        {
                            //workOrders = context.WorkOrder.Include(x => x.Inspection).Include(x => x.Inspection.Asset).Include(x => x.StatusMaster).Where(x => assetlist.Contains(x.site_id.Value) && x.status == status).ToList();
                            issues = context.Issue
                                .Include(x => x.StatusMaster)
                                .Where(x => assetlist.Contains(x.asset_id) && x.status == status).ToList();
                        }
                        else
                        {
                            //workOrders = context.WorkOrder.Include(x => x.StatusMaster).Where(x => usersites.Contains(x.site_id.Value) && x.modified_by == userid && x.status == status).ToList();
                            issues = context.Issue.Include(x => x.StatusMaster).Where(x => assetlist.Contains(x.asset_id) && x.status == status).ToList();
                        }
                    }
                    else
                    {
                        //workOrders = context.WorkOrder.Include(x => x.StatusMaster).Where(x => usersites.Contains(x.site_id.Value) && (x.modified_by == userid || x.status == (int)Status.New)).ToList();
                        issues = context.Issue.Include(x => x.StatusMaster).Where(x => assetlist.Contains(x.asset_id)).ToList();
                    }
                }
                else if (role == GlobalConstants.Manager)
                {
                    if (status > 0)
                    {
                        //workOrders = context.WorkOrder.Include(x => x.StatusMaster).Include(x => x.Inspection).Where(x => usersites.Contains(x.site_id.Value) && x.created_by == userid && x.status == status).ToList();
                        //workOrders = context.WorkOrder.Include(x => x.StatusMaster).Include(x => x.Inspection).Where(x => assetlist.Contains(x.site_id.Value) && x.status == status).ToList();
                        issues = context.Issue.Include(x => x.StatusMaster).Where(x => assetlist.Contains(x.asset_id) && x.status == status).ToList();
                    }
                    else
                    {
                        //workOrders = context.WorkOrder.Include(x => x.StatusMaster).Include(x => x.Inspection).Where(x => usersites.Contains(x.site_id.Value) && x.created_by == userid).ToList();
                        //workOrders = context.WorkOrder.Include(x => x.StatusMaster).Include(x => x.Inspection).Where(x => assetlist.Contains(x.site_id.Value)).ToList();
                        issues = context.Issue.Include(x => x.StatusMaster).Where(x => assetlist.Contains(x.asset_id)).ToList();
                    }
                }
                else if (!string.IsNullOrEmpty(role))
                {
                    if (status > 0)
                    {
                        issues = context.Issue.Include(x => x.StatusMaster).Where(x => assetlist.Contains(x.asset_id) && x.status == status).ToList();
                    }
                    else
                    {
                        issues = context.Issue.Include(x => x.StatusMaster).Where(x => assetlist.Contains(x.asset_id)).ToList();
                    }
                }
            }
            return issues;
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

                    issues = context.Issue.Include(x => x.StatusMaster).Include(x => x.Inspection).ThenInclude(x => x.Asset).Include(x => x.Asset).Include(x => x.Sites).Where(x => assetlist.Contains(x.asset_id)).ToList();

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

        public List<Issue> FilterIssuesTitleOption(FilterIssueRequestModel requestModel)
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

                    issues = context.Issue.Include(x => x.StatusMaster).Include(x => x.Inspection).ThenInclude(x => x.Asset).Include(x => x.Asset).Include(x => x.Sites).Where(x => assetlist.Contains(x.asset_id)).ToList();

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
                        var searchStringLower = requestModel.search_string.ToLower();


                        DateTime dt = DateTime.ParseExact("2000-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        try
                        {
                            dt = Convert.ToDateTime(requestModel.search_string);
                        }
                        catch
                        {
                            // do nothing;
                        }
                        if ("high".Contains(searchStringLower))
                        {
                            searchStringLower = "2";
                        }
                        else if ("veryhigh".Contains(searchStringLower) || "very high".Contains(searchStringLower))
                        {
                            searchStringLower = "1";
                        }
                        else if ("low".Contains(searchStringLower))
                        {
                            searchStringLower = "4";
                        }
                        else if ("medium".Contains(searchStringLower))
                        {
                            searchStringLower = "3";
                        }

                        var searchstring = requestModel.search_string.ToLower();
                        DateTime fromdate = new DateTime();
                        DateTime todate = new DateTime();

                        if (!String.IsNullOrEmpty(requestModel.timezone))
                        {
                            requestModel.timezone = requestModel.timezone.Replace('-', '/');
                            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(requestModel.timezone));
                            //DateTime currentTime = DateTime.Now;
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
                                          || x.internal_asset_id.Contains(searchstring.ToLower()) || x.priority.ToString() == searchStringLower || x.Sites.location.Contains(searchstring.ToLower()) || x.Sites.site_name.Contains(searchstring.ToLower())
                                          || x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower()))).ToList();
                        }
                        else
                        {
                            issues = issues.Where(x => (x.name.ToLower().Contains(searchstring.ToLower()) || x.StatusMaster.status_name.ToLower().Contains(searchstring.ToLower()) || x.status.ToString().Contains(searchstring.ToLower())
                                          || x.internal_asset_id.Contains(searchstring.ToLower()) || x.priority.ToString() == searchStringLower || x.Sites.location.Contains(searchstring.ToLower()) || x.Sites.site_name.Contains(searchstring.ToLower())
                                          || x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower()) || (x.created_at.Value >= fromdate && x.created_at.Value <= todate))).ToList();
                        }
                    }

                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        var searchstring = requestModel.option_search_string.ToLower();
                        issues = issues.Where(x => x.name.ToLower().Contains(searchstring.ToLower())).ToList();
                    }
                }
            }
            return issues;
        }

        public List<Issue> FilterIssuesAssetOption(FilterIssueRequestModel requestModel)
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

                    issues = context.Issue.Include(x => x.StatusMaster).Include(x => x.Inspection).ThenInclude(x => x.Asset).Include(x => x.Asset).Include(x => x.Sites).Where(x => assetlist.Contains(x.asset_id)).ToList();

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
                        var searchStringLower = requestModel.search_string.ToLower();


                        DateTime dt = DateTime.ParseExact("2000-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        try
                        {
                            dt = Convert.ToDateTime(requestModel.search_string);
                        }
                        catch
                        {
                            // do nothing;
                        }
                        if ("high".Contains(searchStringLower))
                        {
                            searchStringLower = "2";
                        }
                        else if ("veryhigh".Contains(searchStringLower) || "very high".Contains(searchStringLower))
                        {
                            searchStringLower = "1";
                        }
                        else if ("low".Contains(searchStringLower))
                        {
                            searchStringLower = "4";
                        }
                        else if ("medium".Contains(searchStringLower))
                        {
                            searchStringLower = "3";
                        }

                        var searchstring = requestModel.search_string.ToLower();
                        DateTime fromdate = new DateTime();
                        DateTime todate = new DateTime();

                        if (!String.IsNullOrEmpty(requestModel.timezone))
                        {
                            requestModel.timezone = requestModel.timezone.Replace('-', '/');
                            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(requestModel.timezone));
                            //DateTime currentTime = DateTime.Now;
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
                                          || x.internal_asset_id.Contains(searchstring.ToLower()) || x.priority.ToString() == searchStringLower || x.Sites.location.Contains(searchstring.ToLower()) || x.Sites.site_name.Contains(searchstring.ToLower())
                                          || x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower()))).ToList();
                        }
                        else
                        {
                            issues = issues.Where(x => (x.name.ToLower().Contains(searchstring.ToLower()) || x.StatusMaster.status_name.ToLower().Contains(searchstring.ToLower()) || x.status.ToString().Contains(searchstring.ToLower())
                                          || x.internal_asset_id.Contains(searchstring.ToLower()) || x.priority.ToString() == searchStringLower || x.Sites.location.Contains(searchstring.ToLower()) || x.Sites.site_name.Contains(searchstring.ToLower())
                                          || x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower()) || (x.created_at.Value >= fromdate && x.created_at.Value <= todate))).ToList();
                        }
                    }

                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        var searchstring = requestModel.option_search_string.ToLower();
                        issues = issues.Where(x => x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower())
                        || x.Inspection.Asset.internal_asset_id.ToLower().Contains(searchstring.ToLower())).ToList();
                    }
                }
            }
            return issues;
        }

        public List<Issue> FilterIssuesSitesOption(FilterIssueRequestModel requestModel)
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

                    issues = context.Issue.Include(x => x.StatusMaster).Include(x => x.Inspection).ThenInclude(x => x.Asset).Include(x => x.Asset).Include(x => x.Sites).Where(x => assetlist.Contains(x.asset_id)).ToList();

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
                        var searchStringLower = requestModel.search_string.ToLower();


                        DateTime dt = DateTime.ParseExact("2000-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        try
                        {
                            dt = Convert.ToDateTime(requestModel.search_string);
                        }
                        catch
                        {
                            // do nothing;
                        }
                        if ("high".Contains(searchStringLower))
                        {
                            searchStringLower = "2";
                        }
                        else if ("veryhigh".Contains(searchStringLower) || "very high".Contains(searchStringLower))
                        {
                            searchStringLower = "1";
                        }
                        else if ("low".Contains(searchStringLower))
                        {
                            searchStringLower = "4";
                        }
                        else if ("medium".Contains(searchStringLower))
                        {
                            searchStringLower = "3";
                        }

                        var searchstring = requestModel.search_string.ToLower();
                        DateTime fromdate = new DateTime();
                        DateTime todate = new DateTime();

                        if (!String.IsNullOrEmpty(requestModel.timezone))
                        {
                            requestModel.timezone = requestModel.timezone.Replace('-', '/');
                            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(requestModel.timezone));
                            //DateTime currentTime = DateTime.Now;
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
                                          || x.internal_asset_id.Contains(searchstring.ToLower()) || x.priority.ToString() == searchStringLower || x.Sites.location.Contains(searchstring.ToLower()) || x.Sites.site_name.Contains(searchstring.ToLower())
                                          || x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower()))).ToList();
                        }
                        else
                        {
                            issues = issues.Where(x => (x.name.ToLower().Contains(searchstring.ToLower()) || x.StatusMaster.status_name.ToLower().Contains(searchstring.ToLower()) || x.status.ToString().Contains(searchstring.ToLower())
                                          || x.internal_asset_id.Contains(searchstring.ToLower()) || x.priority.ToString() == searchStringLower || x.Sites.location.Contains(searchstring.ToLower()) || x.Sites.site_name.Contains(searchstring.ToLower())
                                          || x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower()) || (x.created_at.Value >= fromdate && x.created_at.Value <= todate))).ToList();
                        }
                    }

                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        var searchstring = requestModel.option_search_string.ToLower();
                        issues = issues.Where(x => x.Sites.site_name.ToLower().Contains(searchstring.ToLower())
                        || x.Sites.location.ToLower().Contains(searchstring.ToLower())).ToList();
                    }
                }
            }
            return issues;
        }

        public List<Issue> GetIssuesTitle(string siteid)
        {
            List<Issue> issues = new List<Issue>();
            string rolename = string.Empty;
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
                {
                    rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
                }
                IQueryable<Asset> query = context.Assets;

                List<Guid> assetlist = new List<Guid>();

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

                    if (!string.IsNullOrEmpty(siteid) && siteid != "0")
                    {
                        query = query.Where(x => x.site_id.ToString() == siteid);
                    }

                    assetlist = query.Select(x => x.asset_id).ToList().Distinct().ToList();

                    issues = context.Issue.Where(x => assetlist.Contains(x.asset_id)).ToList();
                }
            }
            return issues;
        }

        public int CreateIssueStatus(IssueStatus entity)
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
                    context.IssueStatus.Add(entity);
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

        public Issue GetIssueByIssueId(Guid work_order_uuid)
        {
            return context.Issue.Where(x => x.issue_uuid == work_order_uuid)
                .Include(x => x.Asset).Include(x => x.Inspection)
                .Include(x => x.IssueRecord).FirstOrDefault();
        }

        //public int UpdateWorkOrder(WorkOrder workorder)
        //{
        //    int result = (int)ResponseStatusNumber.Error;
        //    try
        //    {
        //        context.Update(workorder);
        //        if (workorder.inspection_id != null && workorder.inspection_id != Guid.Empty)
        //        {
        //            var inspection = context.Inspection.Include(x => x.Asset).Where(x => x.inspection_id == workorder.inspection_id).FirstOrDefault();

        //            if (inspection != null && inspection.inspection_id != null && workorder.inspection_id != Guid.Empty)
        //            {
        //                if (workorder.status == (int)Status.Completed || inspection.status == (int)Status.Approved)
        //                {
        //                    //inspection.status = (int)Status.Approved;
        //                    inspection.Asset.status = (int)Status.AssetActive;
        //                }
        //                else
        //                {
        //                    //inspection.status = (int)Status.InspectionMaintenance;
        //                    inspection.Asset.status = (int)Status.InMaintenanace;
        //                }
        //                context.Update(inspection);
        //            }
        //        }
        //        else
        //        {
        //            var asset = context.Assets.Where(x => x.asset_id == workorder.asset_id).FirstOrDefault();
        //            if (asset != null && asset.asset_id != null && workorder.asset_id != Guid.Empty)
        //            {
        //                if (workorder.status == (int)Status.Completed || asset.status == (int)Status.AssetActive)
        //                {
        //                    //inspection.status = (int)Status.Approved;
        //                    asset.status = (int)Status.AssetActive;
        //                }
        //                else
        //                {
        //                    //inspection.status = (int)Status.InspectionMaintenance;
        //                    asset.status = (int)Status.InMaintenanace;
        //                }
        //                context.Update(asset);
        //            }
        //        }
        //        context.SaveChanges();
        //        result = (int)ResponseStatusNumber.Success;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    return result;
        //}


        public int UpdateIssue(Issue issue)
        {
            int result = (int)ResponseStatusNumber.Error;
            try
            {
                context.Update(issue);
                //if (workorder.inspection_id != null && workorder.inspection_id != Guid.Empty)
                //{
                //    var inspection = context.Inspection.Include(x => x.Asset).Where(x => x.inspection_id == workorder.inspection_id).FirstOrDefault();

                //    if (inspection != null && inspection.inspection_id != null && workorder.inspection_id != Guid.Empty)
                //    {
                //        if (workorder.status == (int)Status.Completed && inspection.status == (int)Status.Rejected)
                //        {
                //            //inspection.status = (int)Status.Approved;
                //            inspection.Asset.status = (int)Status.AssetActive;
                //        }
                //        else if (workorder.status != (int)Status.Completed && inspection.status == (int)Status.Rejected)
                //        {
                //            //inspection.status = (int)Status.InspectionMaintenance;
                //            inspection.Asset.status = (int)Status.InMaintenanace;
                //        }
                //        context.Update(inspection);
                //    }
                //}
                //else
                //{
                //    var asset = context.Assets.Where(x => x.asset_id == workorder.asset_id).FirstOrDefault();
                //    if (asset != null && asset.asset_id != null && workorder.asset_id != Guid.Empty)
                //    {
                //        if (workorder.status == (int)Status.Completed || asset.status == (int)Status.AssetActive)
                //        {
                //            //inspection.status = (int)Status.Approved;
                //            asset.status = (int)Status.AssetActive;
                //        }
                //        else
                //        {
                //            //inspection.status = (int)Status.InspectionMaintenance;
                //            asset.status = (int)Status.InMaintenanace;
                //        }
                //        context.Update(asset);
                //    }
                //}
                context.SaveChanges();
                result = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        public int ValidateInternalAssetID(string userid, string internal_asset_id)
        {
            int isvalid = (int)ResponseStatusNumber.Error;
            var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
            if (usersites.Count > 0)
            {
                var response = context.Assets.Where(x => x.internal_asset_id == internal_asset_id && usersites.Contains(x.site_id)).FirstOrDefault();
                if (response != null && response.asset_id != null && response.asset_id != Guid.Empty)
                {
                    response = context.Assets.Where(x => x.internal_asset_id == internal_asset_id && x.status == (int)Status.AssetActive).FirstOrDefault();
                    if (response != null && response.asset_id != null && response.asset_id != Guid.Empty)
                    {
                        isvalid = (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        isvalid = (int)ResponseStatusNumber.False;
                    }
                }
                else
                {
                    isvalid = (int)ResponseStatusNumber.NotFound;
                }
            }
            else
            {
                isvalid = (int)ResponseStatusNumber.NotFound;
            }

            return isvalid;
        }

        //public int AddWorkOrderStatus(WorkOrderStatus entity)
        //{
        //    int IsSuccess;
        //    try
        //    {
        //        if (entity == null)
        //        {
        //            throw new ArgumentNullException("entity");
        //        }
        //        else
        //        {
        //            context.WorkOrderStatus.Add(entity);
        //            IsSuccess = (int)ResponseStatusNumber.Success;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        IsSuccess = (int)ResponseStatusNumber.Error;
        //        throw e;
        //    }
        //    return IsSuccess;
        //}

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

        public List<Issue> GetTodayNewIssues(string userid, int pagesize, int pageindex)
        {
            var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

            //var time = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Pacific Standard Time").Date;

            var assetlist = context.Assets.Where(x => usersites.Contains(x.site_id)).Select(x => x.asset_id).Distinct().ToList();

            var time = DateTime.UtcNow.AddHours(-8).Date;

            List<Issue> issues = new List<Issue>();

            //workOrders = context.WorkOrder.Include(x => x.StatusMaster).Where(x => usersites.Contains(x.site_id.Value) && x.status == (int)Status.New && x.created_at.Value.Date == DateTime.UtcNow.Date).ToList();
            issues = context.Issue.Include(x => x.StatusMaster).Where(x => assetlist.Contains(x.asset_id) && x.status == (int)Status.New && x.created_at.Value.AddHours(-8).Date == time.Date).ToList();

            if (pageindex > 0)
            {
                return issues.OrderByDescending(g => g.created_at.Value).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
            }
            else
            {
                return issues;
            }
        }

        public List<Issue> GetIssueByAssetId(string userid, string assetid, int pagesize, int pageindex)
        {
            //var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

            //var assetlist = context.Assets.Where(x => usersites.Contains(x.site_id)).Distinct().ToList();

            List<Issue> issues = new List<Issue>();

            issues = context.Issue.Include(x => x.Inspection).Include(x => x.StatusMaster).Where(x => x.asset_id.ToString() == assetid).ToList();

            return issues;
        }

        public List<Issue> SearchIssueByAssetId(string userid, string assetid, string searchstring, int pagesize, int pageindex)
        {
            var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

            var searchStringLower = searchstring.ToLower().ToString();

            List<Issue> issues = new List<Issue>();

            if (searchStringLower == String.Empty)
            {
                issues = context.Issue.Include(x => x.StatusMaster).Include(x => x.Sites).Where(x => x.asset_id.ToString() == assetid).ToList();
            }
            else
            {
                if ("high".Contains(searchStringLower))
                {
                    searchStringLower = "2";
                }
                else if ("veryhigh".Contains(searchStringLower) || "very high".Contains(searchStringLower))
                {
                    searchStringLower = "1";
                }
                else if ("low".Contains(searchStringLower))
                {
                    searchStringLower = "4";
                }
                else if ("medium".Contains(searchStringLower))
                {
                    searchStringLower = "3";
                }
                //workOrders = context.WorkOrder.Include(x => x.StatusMaster).Include(x => x.Sites).Where(x => x.asset_id.ToString() == assetid && usersites.Contains(x.site_id.Value)
                //               && (x.name.ToLower().Contains(searchstring.ToLower().ToString()) || x.StatusMaster.status_name.ToLower().Contains(searchstring.ToLower().ToString())
                //                  || x.internal_asset_id.Contains(searchstring.ToLower().ToString()) || x.priority.ToString() == searchStringLower || x.Sites.location.Contains(searchstring.ToLower().ToString()) || x.Sites.site_name.Contains(searchstring.ToLower().ToString()))
                //                ).ToList();
                issues = context.Issue.Include(x => x.StatusMaster).Include(x => x.Inspection).Include(x => x.Inspection.Asset).Where(x => (x.asset_id.ToString() == assetid)
                                && (x.name.ToLower().Contains(searchstring.ToLower()) || x.StatusMaster.status_name.ToLower().Contains(searchstring.ToLower()) || x.status.ToString().Contains(searchstring.ToLower())
                                  || x.internal_asset_id.Contains(searchstring.ToLower()) || x.priority.ToString() == searchStringLower || x.Sites.location.Contains(searchstring.ToLower()) || x.Sites.site_name.Contains(searchstring.ToLower())
                                  || x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower()))).ToList();
            }

            return issues.OrderByDescending(x => x.created_at).ToList();
        }

        public List<Issue> SearchIssues(string userid, string searchstring, string timezone, int pagesize, int pageindex)
        {

            List<Issue> issues = new List<Issue>();

            var role = context.UserRoles.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();

            List<Guid> assetlist = null;
            if (role == GlobalConstants.Executive)
            {
                var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                assetlist = context.Assets.Where(x => usersites.Contains(x.site_id)).Select(x => x.asset_id).Distinct().ToList();
            }
            else
            {
                assetlist = context.Assets.Where(x => x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.asset_id).ToList().Distinct().ToList();
            }

            //workOrders = context.WorkOrder.Include(x => x.StatusMaster).Where(x => assetlist.Contains(x.asset_id.Value)).ToList();
            var searchStringLower = searchstring.ToLower();

            DateTime dt = DateTime.ParseExact("2000-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
            try
            {
                dt = Convert.ToDateTime(searchstring);
            }
            catch
            {
                // do nothing;
            }
            if ("high".Contains(searchStringLower))
            {
                searchStringLower = "2";
            }
            else if ("veryhigh".Contains(searchStringLower) || "very high".Contains(searchStringLower))
            {
                searchStringLower = "1";
            }
            else if ("low".Contains(searchStringLower))
            {
                searchStringLower = "4";
            }
            else if ("medium".Contains(searchStringLower))
            {
                searchStringLower = "3";
            }

            if (searchstring != string.Empty)
            {

                DateTime fromdate = new DateTime();
                DateTime todate = new DateTime();

                timezone = timezone.Replace('-', '/');
                if (timezone != null)
                {
                    DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timezone));
                    //DateTime currentTime = DateTime.Now;
                    var diff = currentTime - DateTime.UtcNow;
                    fromdate = dt.AddHours(diff.Hours).AddMinutes(diff.Minutes);
                    todate = fromdate.AddHours(24);
                    Console.WriteLine("currentdate time " + currentTime);
                }

                Console.WriteLine("timezone " + timezone);
                Console.WriteLine("fromdate " + fromdate);
                Console.WriteLine("todate " + todate);

                if (role == GlobalConstants.MS)
                {
                    if (dt.Date.ToString("MM/dd/yyyy") == "01/01/2000")
                    {
                        issues = context.Issue.Include(x => x.StatusMaster).Include(x => x.Inspection).Include(x => x.Inspection.Asset).Where(x => assetlist.Contains(x.asset_id) && x.modified_by == userid
                                    && (x.name.ToLower().Contains(searchstring.ToLower()) || x.StatusMaster.status_name.ToLower().Contains(searchstring.ToLower()) || x.status.ToString().Contains(searchstring.ToLower())
                                      || x.internal_asset_id.Contains(searchstring.ToLower()) || x.priority.ToString() == searchStringLower || x.Sites.location.Contains(searchstring.ToLower()) || x.Sites.site_name.Contains(searchstring.ToLower())
                                      || x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower()))).ToList();
                    }
                    else
                    {
                        issues = context.Issue.Include(x => x.StatusMaster).Include(x => x.Inspection).Include(x => x.Inspection.Asset).Where(x => assetlist.Contains(x.asset_id) && x.modified_by == userid
                                    && (x.name.ToLower().Contains(searchstring.ToLower()) || x.StatusMaster.status_name.ToLower().Contains(searchstring.ToLower()) || x.status.ToString().Contains(searchstring.ToLower())
                                      || x.internal_asset_id.Contains(searchstring.ToLower()) || x.priority.ToString() == searchStringLower || x.Sites.location.Contains(searchstring.ToLower()) || x.Sites.site_name.Contains(searchstring.ToLower())
                                      || x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower()) || (x.created_at.Value >= fromdate && x.created_at.Value <= todate))).ToList();
                    }
                }
                else if (role == GlobalConstants.Manager)
                {
                    if (dt.Date.ToString("MM/dd/yyyy") == "01/01/2000")
                    {
                        issues = context.Issue.Include(x => x.StatusMaster).Include(x => x.Inspection).Include(x => x.Inspection.Asset).Where(x => assetlist.Contains(x.asset_id)
                                    && (x.name.ToLower().Contains(searchstring.ToLower()) || x.StatusMaster.status_name.ToLower().Contains(searchstring.ToLower()) || x.status.ToString().Contains(searchstring.ToLower())
                                      || x.internal_asset_id.Contains(searchstring.ToLower()) || x.priority.ToString() == searchStringLower || x.Sites.location.Contains(searchstring.ToLower()) || x.Sites.site_name.Contains(searchstring.ToLower())
                                      || x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower()))).ToList();
                    }
                    else
                    {
                        issues = context.Issue.Include(x => x.StatusMaster).Include(x => x.Inspection).Include(x => x.Inspection.Asset).Where(x => assetlist.Contains(x.asset_id)
                                    && (x.name.ToLower().Contains(searchstring.ToLower()) || x.StatusMaster.status_name.ToLower().Contains(searchstring.ToLower()) || x.status.ToString().Contains(searchstring.ToLower())
                                      || x.internal_asset_id.Contains(searchstring.ToLower()) || x.priority.ToString() == searchStringLower || x.Sites.location.Contains(searchstring.ToLower()) || x.Sites.site_name.Contains(searchstring.ToLower())
                                      || x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower()) || (x.created_at.Value >= fromdate && x.created_at.Value <= todate))).ToList();
                    }
                }
                else if (!string.IsNullOrEmpty(role))
                {
                    if (dt.Date.ToString("MM/dd/yyyy") == "01/01/2000")
                    {
                        issues = context.Issue.Include(x => x.StatusMaster).Include(x => x.Inspection).Include(x => x.Inspection.Asset).Where(x => assetlist.Contains(x.asset_id)
                                    && (x.name.ToLower().Contains(searchstring.ToLower()) || x.StatusMaster.status_name.ToLower().Contains(searchstring.ToLower()) || x.status.ToString().Contains(searchstring.ToLower())
                                      || x.internal_asset_id.Contains(searchstring.ToLower()) || x.priority.ToString() == searchStringLower || x.Sites.location.Contains(searchstring.ToLower()) || x.Sites.site_name.Contains(searchstring.ToLower())
                                      || x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower()))).ToList();
                    }
                    else
                    {
                        issues = context.Issue.Include(x => x.StatusMaster).Include(x => x.Inspection).Include(x => x.Inspection.Asset).Where(x => assetlist.Contains(x.asset_id)
                                    && (x.name.ToLower().Contains(searchstring.ToLower()) || x.StatusMaster.status_name.ToLower().Contains(searchstring.ToLower()) || x.status.ToString().Contains(searchstring.ToLower())
                                      || x.internal_asset_id.Contains(searchstring.ToLower()) || x.priority.ToString() == searchStringLower || x.Sites.location.Contains(searchstring.ToLower()) || x.Sites.site_name.Contains(searchstring.ToLower())
                                      || x.Inspection.Asset.name.ToLower().Contains(searchstring.ToLower()) || (x.created_at.Value >= fromdate && x.created_at.Value <= todate))).ToList();
                    }
                }
            }
            return issues.OrderByDescending(x => x.created_at).ToList();
        }

        public Issue GetIssueByInspectionId(string inspectionid)
        {
            return context.Issue.Where(x => x.inspection_id.ToString() == inspectionid).FirstOrDefault();
        }

        public List<Issue> GetIssuesByInspectionId(string inspectionid)
        {
            return context.Issue.Where(x => x.inspection_id.ToString() == inspectionid)
                .Include(x => x.Asset)
                .Include(x => x.Attributes).ToList();
        }

        public Issue GetAttributeIssueByBarcodeId(string attributes_id, string barcode_id)
        {
            return context.Issue.Where(x => x.asset_id.ToString() == barcode_id &&
            x.attribute_id.ToString() == attributes_id && x.status != (int)Status.Completed)
                .Include(x => x.StatusMaster)
                .FirstOrDefault();
        }

        public Issue GetAttributeIssueByInspectionId(string attributes_id, string inspection_id)
        {
            return context.Issue.Where(x => x.inspection_id.ToString() == inspection_id &&
            x.attribute_id.ToString() == attributes_id)
                .Include(x => x.StatusMaster)
                .FirstOrDefault();
        }

        public Issue GetAttributeIssueByInternalAssetId(string attributes_id, string internal_asset_id)
        {
            Guid asset_id = context.Assets.Where(x => x.internal_asset_id == internal_asset_id).Select(x => x.asset_id).FirstOrDefault();
            return context.Issue.Where(x => x.asset_id == asset_id &&
            x.attribute_id.ToString() == attributes_id && x.status != (int)Status.Completed)
                .Include(x => x.StatusMaster)
                .FirstOrDefault();
        }

        public bool HaveAlreadyIssue(Guid attribute_id, Guid asset_id, DateTime date_time, DateTime inspection_created_at)
        {
            //var workorder = context.WorkOrder.Where(x => x.attribute_id == attribute_id && x.asset_id == asset_id && (x.checkout_datetime >= date_time && x.status != (int)Status.Completed || x.checkout_datetime == date_time && x.status == (int)Status.Completed)).FirstOrDefault();
            var issue = context.Issue.Where(x => x.attribute_id == attribute_id && x.asset_id == asset_id && ((x.checkout_datetime <= date_time && x.status != (int)Status.Completed)
            || ((x.modified_at.Value > inspection_created_at || x.checkout_datetime == date_time) && x.status == (int)Status.Completed))).FirstOrDefault();
            if (issue != null && issue.issue_uuid != Guid.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public long GetMaxIssueNumber(Guid site_id)
        {
            long number = 0;
            var workordernumber = context.Issue.Where(x => x.site_id == site_id).Select(x => x.issue_number == null ? 0 : x.issue_number.Value).ToList();
            if (workordernumber.Count > 0)
            {
                number = workordernumber.Max();
            }
            return number;
        }

        public List<Asset> GetAssetsIssue(string userid, int pagesize, int pageindex, int status)
        {
            //List<Asset> asset = new List<Asset>();
            //if (status == (int)Status.Completed)
            //{
            var role = context.UserRoles.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();
            var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

            if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
            {
                return context.Assets
                    .Include(x => x.Issues)
                        .ThenInclude(x => x.Inspection)
                            .ThenInclude(x => x.StatusMaster)
                    .Include(x => x.Issues)
                        .ThenInclude(x => x.Inspection)
                            .ThenInclude(x => x.User)
                    .Include(x => x.Issues)
                        .ThenInclude(x => x.StatusMaster)
                        .Where(x => usersites.Contains(x.site_id) && x.Issues.Count > 0)
                        .Include(x => x.StatusMaster).ToList();
            }
            else
            {
                return context.Assets
                    .Include(x => x.Issues)
                        .ThenInclude(x => x.Inspection)
                            .ThenInclude(x => x.StatusMaster)
                    .Include(x => x.Issues)
                        .ThenInclude(x => x.Inspection)
                            .ThenInclude(x => x.User)
                    .Include(x => x.Issues)
                        .ThenInclude(x => x.StatusMaster)
                        .Where(x => usersites.Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id && x.Issues.Count > 0)
                        .Include(x => x.StatusMaster).ToList();
            }



            //    asset.ForEach(x =>
            //    {
            //        List<WorkOrder> workorders = new List<WorkOrder>();
            //        x.WorkOrders.ToList().ForEach(y =>
            //        {
            //            if (y.status == (int)Status.Completed)
            //            {
            //                workorders.Add(y);
            //            }
            //        });
            //        x.WorkOrders = workorders;
            //    });
            //}
            //else
            //{
            //    asset = context.Assets
            //            .Include(x => x.WorkOrders)
            //                .ThenInclude(x => x.Inspection)
            //                    .ThenInclude(x => x.StatusMaster)
            //            .Include(x => x.WorkOrders)
            //                .ThenInclude(x => x.StatusMaster)
            //                .Where(x => usersites.Contains(x.site_id) && x.WorkOrders.Count > 0)
            //                .Include(x => x.StatusMaster).ToList();

            //    asset.ForEach(x =>
            //    {
            //        List<WorkOrder> workorders = new List<WorkOrder>();
            //        x.WorkOrders.ToList().ForEach(y =>
            //            {
            //                if (y.status != (int)Status.Completed)
            //                {
            //                    workorders.Add(y);
            //                }
            //            });
            //        x.WorkOrders = workorders;
            //    });
            //}
            //return asset;
        }

        public List<Issue> GetAllIssues(string userid, string timestamp)
        {
            List<Issue> issues = new List<Issue>();
            //var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

            List<Guid> usersites = new List<Guid>();
            if (String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
            {
                usersites = context.User.Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).Select(x => x.ac_default_site.Value).ToList();
            }
            else
            {
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
            }

            bool date = false;
            string dateString = timestamp;
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime dateTime = new DateTime();
            // It throws Argument null exception  
            try
            {
                dateTime = DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm:ss", provider);
                date = true;
            }
            catch
            {
                date = false;
            }

            if (usersites.Count > 0)
            {
                if (date)
                {
                    issues = context.Issue.Where(x => usersites.Contains(x.Asset.site_id) && x.modified_at >= dateTime).ToList();
                }
                else
                {
                    issues = context.Issue.Where(x => usersites.Contains(x.Asset.site_id)).ToList();
                }
            }
            return issues;
        }

        public async Task<List<Issue>> GetPendingIssues(string userid)
        {
            List<Issue> DashboardOutstandingIssues = new List<Issue>();
            //var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
            if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
            {
                List<Guid> usersites = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }

                DashboardOutstandingIssues = await context.Issue.Where(x => x.status != (int)Status.Completed && usersites.Contains(x.Asset.site_id)).Include(x => x.Asset).Include(x => x.Sites).Include(x => x.Attributes).ToListAsync();

            }
            return DashboardOutstandingIssues;
        }

        public async Task<int> CreateIssueFromIssue()
        {
            int IsSuccess = (int)ResponseStatusNumber.Error;
            try
            {
                //List<Issue> issues = new List<Issue>();
                //var allWorkOrders = context.WorkOrder.ToList();
                //if (allWorkOrders?.Count > 0)
                //{
                //    allWorkOrders.ForEach(x =>
                //    {
                //        Issue issue = new Issue();
                //        issue.issue_uuid = x.work_order_uuid;
                //        issue.asset_id = x.asset_id;
                //        issue.attribute_id = x.attribute_id;
                //        issue.inspection_id = x.inspection_id;
                //        issue.internal_asset_id = x.internal_asset_id;
                //        issue.name = x.name;
                //        issue.issue_number = x.work_order_number;
                //        issue.description = x.description;
                //        issue.notes = x.notes;
                //        issue.attributes_value = x.attributes_value;
                //        issue.status = x.status;
                //        issue.comments = x.comments;
                //        issue.priority = x.priority;
                //        issue.checkout_datetime = x.checkout_datetime;
                //        issue.requested_datetime = x.requested_datetime;
                //        issue.created_by = x.created_by;
                //        issue.created_at = x.created_at;
                //        issue.modified_by = x.modified_by;
                //        issue.modified_at = x.modified_at;
                //        issue.updated_at = x.updated_at;
                //        issue.site_id = x.site_id;
                //        issue.company_id = x.company_id;
                //        issue.maintainence_staff_id = x.maintainence_staff_id;
                //        issues.Add(issue);
                //    });

                //    context.Issue.AddRange(issues);
                //    IsSuccess = (int)ResponseStatusNumber.Success;
                //}
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public async Task<int> CreateIssueStatus()
        {
            int IsSuccess = (int)ResponseStatusNumber.Error;
            try
            {
                //var allWorkOrders = context.WorkOrderStatus.ToList();
                //if (allWorkOrders?.Count > 0)
                //{
                //    List<IssueStatus> IssueStatus = new List<IssueStatus>();
                //    allWorkOrders.ForEach(x =>
                //    {

                //        IssueStatus issueStatus = new IssueStatus();
                //        issueStatus.issue_status_id = x.work_order_status_id;
                //        issueStatus.issue_id = x.work_order_id;
                //        issueStatus.status = x.status;
                //        issueStatus.modified_at = x.modified_at;
                //        issueStatus.modified_by = x.modified_by;
                //        IssueStatus.Add(issueStatus);
                //    });

                //    context.IssueStatus.AddRange(IssueStatus);
                //    IsSuccess = (int)ResponseStatusNumber.Success;
                //}
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public async Task<int> CreateIssueRecords()
        {
            int IsSuccess = (int)ResponseStatusNumber.Error;
            try
            {
                //var allWorkOrders = context.WorkOrderRecord.ToList();
                //if (allWorkOrders?.Count > 0)
                //{
                //    List<IssueRecord> IssueRecord = new List<IssueRecord>();
                //    allWorkOrders.ForEach(x =>
                //    {
                //        IssueRecord issueRecord = new IssueRecord();
                //        issueRecord.issue_record_uuid = x.work_order_record_uuid;
                //        issueRecord.issue_uuid = x.work_order_uuid;
                //        issueRecord.attrubute_id = x.attrubute_id;
                //        issueRecord.asset_id = x.asset_id;
                //        issueRecord.inspection_id = x.inspection_id;
                //        issueRecord.status = x.status;
                //        issueRecord.requested_datetime = x.requested_datetime;
                //        issueRecord.created_by = x.created_by;
                //        issueRecord.fixed_datetime = x.fixed_datetime;
                //        issueRecord.fixed_by = x.fixed_by;
                //        issueRecord.created_at = x.created_at;
                //        issueRecord.checkout_datetime = x.checkout_datetime;
                //        IssueRecord.Add(issueRecord);
                //    });

                //    context.IssueRecord.AddRange(IssueRecord);
                //    IsSuccess = (int)ResponseStatusNumber.Success;
                //}
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }
        public List<Issue> GetIssuesForDailyReport(List<Guid> siteid, DateTime startdate, DateTime enddate)
        {
            List<Issue> issues = new List<Issue>();
            issues = context.Issue.Where(x => siteid.Contains(x.Asset.site_id) && x.created_at >= startdate && x.created_at < enddate).Include(x => x.Asset).Include(x => x.Sites).Include(x => x.StatusMaster).ToList();

            return issues;
        }

        public Issue GetIssueByMrId(Guid mr_uuid)
        {
            return context.Issue.Where(x => x.mr_id == mr_uuid).FirstOrDefault();
        }

        public TempAsset GetTempIssueByAssetId(Guid asset_id , Guid wo_id)
        {
            return context.TempAsset.Where(x => x.asset_id == asset_id && x.wo_id == wo_id && !x.is_deleted)
                .Include(x=>x.InspectionTemplateAssetClass)
                .Include(x=>x.TempFormIOBuildings)
                .Include(x=>x.TempFormIOFloors)
                .Include(x=>x.TempFormIORooms)
                .Include(x=>x.TempFormIOSections)
                .FirstOrDefault();
        }
        public TempAsset GetTempIssueByAssetIdAsnotracking(Guid asset_id , Guid wo_id)
        {
            return context.TempAsset.Where(x => x.asset_id == asset_id && x.wo_id == wo_id && !x.is_deleted)
                .Include(x=>x.InspectionTemplateAssetClass)
                .Include(x=>x.TempFormIOBuildings)
                .Include(x=>x.TempFormIOFloors)
                .Include(x=>x.TempFormIORooms)
                .Include(x=>x.TempFormIOSections).AsNoTracking()
                .FirstOrDefault();
        }

        public WOOnboardingAssets GetWolneById(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Include(x=>x.TempAsset)
                .Include(x=>x.WOOnboardingAssetsImagesMapping).AsNoTracking()
                .FirstOrDefault();
        }
        public TempAsset GetWolneTempAssetById(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
               .Select(x => x.TempAsset).AsNoTracking()
               .FirstOrDefault();
        }
        public WOOnboardingAssets GetWolneByIdAsnotracking(Guid woonboardingassets_id)
        {
            // detach existing entity from local
            var existingEntity = context.WOOnboardingAssets.Local
                    .FirstOrDefault(e => e.woonboardingassets_id == woonboardingassets_id);

            if (existingEntity != null)
            {
                context.Entry(existingEntity).State = EntityState.Detached;
                // Alternatively, you can use context.Entry(existingEntity).Reload() to refresh from the database.
            }

            return context.WOOnboardingAssets.Where(x => x.woonboardingassets_id == woonboardingassets_id)
                .Include(x => x.TempAsset)
                .Include(x => x.WOOnboardingAssetsImagesMapping).AsNoTracking()
                .FirstOrDefault();
        }
        public TempAsset GetTempAssetbyId(Guid tempasset_id)
        {
            return context.TempAsset.Where(x => x.tempasset_id == tempasset_id)
                .Include(x=>x.InspectionTemplateAssetClass)
                .Include(x=>x.TempFormIOBuildings)
                .Include(x=>x.TempFormIOFloors)
                .Include(x=>x.TempFormIORooms)
                .Include(x=>x.TempFormIOSections)
                .Include(x=>x.WOOnboardingAssets)
                .FirstOrDefault();
        }
        public AssetIssue GetMainIssue(Guid asset_issue_id)
        {
            return context.AssetIssue.Where(x => x.asset_issue_id == asset_issue_id)
                .Include(x=>x.Asset)
                .Include(x=>x.AssetIssueImagesMapping)
                .FirstOrDefault();
        }

        public WOLineIssue GetwolineissueById(Guid wo_line_issue_id)
        {
            return context.WOLineIssue.Where(x => x.wo_line_issue_id == wo_line_issue_id).FirstOrDefault();
        }

        public (List<GetAllAssetIssuesOptimizedResponsemodel>, int) GetAllAssetIssuesOptimized(GetAllAssetIssuesRequestmodel requestmodel)
        {
            IQueryable<AssetIssue> query = context.AssetIssue.Where(x => !x.is_deleted
            && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
            )
            .Include(x => x.Asset).ThenInclude(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.FormIOType)
            .Include(x => x.WorkOrders).Include(x => x.StatusMaster);

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
                query = query.Where(x => x.issue_title.ToLower().Trim().Contains(search)
                || x.issue_description.ToLower().Trim().Contains(search) || x.Asset.name.ToLower().Trim().Contains(search)
                || x.WorkOrders.manual_wo_number.ToLower().Trim().Contains(search) || x.issue_number.ToLower().Trim().Contains(search) );
            }

            query = query.OrderByDescending(x => x.created_at);
            int total_list_count = query.Count();
            if (requestmodel.page_size != 0 && requestmodel.page_index != 0)
            {
                query = query.Skip((requestmodel.page_index - 1) * requestmodel.page_size).Take(requestmodel.page_size);
            }

            var response = query.Select(x => new GetAllAssetIssuesOptimizedResponsemodel
            {
                asset_issue_id = x.asset_issue_id,
                issue_number = x.issue_number,
                asset_name = x.Asset!=null ? x.Asset.name:null,
                asset_id = x.asset_id,
                issue_status = x.issue_status,
                issue_status_name = x.StatusMaster!=null?x.StatusMaster.status_name:null,
                issue_caused_id = x.issue_caused_id,
                manual_wo_number = x.WorkOrders!=null ? x.WorkOrders.manual_wo_number:null,
                issue_type = x.issue_type,
                asset_class_type = x.Asset.InspectionTemplateAssetClass!=null && x.Asset.InspectionTemplateAssetClass.FormIOType!=null ? x.Asset.InspectionTemplateAssetClass.FormIOType.form_type_name:null,
                priority = x.priority,
                issue_title = x.issue_title,
                wo_id = x.wo_id,
                issue_time_elapsed = (x.created_at == null || x.issue_status == (int)Status.Completed) ? "" : DateTimeUtil.GetBeforetimeText(x.created_at.Value),

                asset_class_name = x.Asset!=null && x.Asset.InspectionTemplateAssetClass!=null ? x.Asset.InspectionTemplateAssetClass.asset_class_name : null,
                asset_condition_index_type = x.Asset != null ? x.Asset.condition_index_type : null,
                asset_criticality_index_type = x.Asset != null ? x.Asset.criticality_index_type : null,
                asset_operating_condition_state = x.Asset != null ? x.Asset.asset_operating_condition_state : null,
                issue_description = x.issue_description,
                thermal_anomaly_corrective_action = x.WOLineIssue != null ? x.WOLineIssue.thermal_anomaly_corrective_action : null,
                site_name = x.Sites != null ? x.Sites.site_name : null,
                client_company_name = x.Sites != null && x.Sites.ClientCompany != null ? x.Sites.ClientCompany.client_company_name : null,
                company_name = x.Sites != null && x.Sites.Company != null ? x.Sites.Company.company_name : null,
                comments = String.Join(", ", context.AssetIssueComments.Where(y => y.asset_issue_id == x.asset_issue_id && !y.is_deleted).Select(y => y.comment).ToList())

            }).ToList();

            return (response, total_list_count);
        }

        public List<GetAllWOLineTempIssuesResponsemodel> GetAlltempIssuebyWOidOptimized(GetAllIssueByWOidRequestmodel requestmodel)
        {
            IQueryable<WOLineIssue> query = context.WOLineIssue.Where(x => !x.is_deleted && x.wo_id == requestmodel.wo_id)
                .Include(x => x.Asset)
                .Include(x => x.StatusMaster)
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

            var response = query.Select(x => new GetAllWOLineTempIssuesResponsemodel
            {
                issue_status = x.issue_status, issue_status_name = x.StatusMaster.status_name,issue_title = x.issue_title,
                asset_name = x.WOOnboardingAssets != null && x.WOOnboardingAssets.TempAsset != null ? x.WOOnboardingAssets.TempAsset.asset_name : x.form_retrived_asset_name,
                issue_description = x.issue_description, wo_line_issue_id = x.wo_line_issue_id,
                issue_type = x.issue_type, issue_caused_id = x.issue_caused_id,
                field_note = x.field_note, back_office_note = x.back_office_note,
                woonboardingassets_id = x.woonboardingassets_id, asset_form_id = x.asset_form_id,
                wo_id = x.wo_id, asset_id = x.asset_id, origin_wo_id = x.original_wo_id,
                origin_manual_wo_number = context.WorkOrders.Where(w => w.wo_id == x.original_wo_id).FirstOrDefault().manual_wo_number,
                origin_wo_type = context.WorkOrders.Where(w => w.wo_id == x.original_wo_id).FirstOrDefault().wo_type,
                origin_wo_line_id = (x.original_asset_form_id != null ? x.original_asset_form_id : x.original_woonboardingassets_id),

            }).ToList();

            return response;
        }

        public List<link_main_issue_list> GetAllmainIssuebyWOidOptimized(GetAllIssueByWOidRequestmodel requestmodel)
        {
            IQueryable<AssetIssue> query = context.AssetIssue.Where(x => x.wo_id == requestmodel.wo_id && !x.is_deleted)
                                         .Include(x => x.WorkOrders).Include(x => x.WOLineIssue)
                                         .Include(x => x.WOOnboardingAssets.TempAsset).Include(x => x.Asset);
            if (!String.IsNullOrEmpty(requestmodel.search_string))
            {
                string search = requestmodel.search_string.ToLower().Trim();
                query = query.Where(x => x.issue_title.ToLower().Trim().Contains(search) || x.Asset.name.ToLower().Trim().Contains(search) || x.WorkOrders.manual_wo_number.ToLower().Trim().Contains(search));
            }
           
            var response = query.Select(x => new link_main_issue_list
            {
                issue_status = x.issue_status, priority = x.priority,
                asset_name = x.WOOnboardingAssets != null && x.WOOnboardingAssets.TempAsset != null ? x.WOOnboardingAssets.TempAsset.asset_name : x.WOOnboardingAssets.asset_name,
                issue_title = x.issue_title, issue_description = x.issue_description,
                issue_number = x.issue_number, issue_type = x.issue_type,
                asset_issue_id = x.asset_issue_id, created_at = x.created_at,
                origin_wo_id = x.WOLineIssue != null ? x.WOLineIssue.original_wo_id : null,
                origin_wo_line_id = (x.WOLineIssue.original_asset_form_id != null ? x.WOLineIssue.original_asset_form_id : x.WOLineIssue.original_woonboardingassets_id),
                origin_manual_wo_number = x.WOLineIssue != null ? context.WorkOrders.Where(w => w.wo_id == x.WOLineIssue.original_wo_id).FirstOrDefault().manual_wo_number : null,
                origin_wo_type = x.WOLineIssue != null ? context.WorkOrders.Where(w => w.wo_id == x.WOLineIssue.original_wo_id).FirstOrDefault().wo_type : 0,

            }).ToList();

            return response;
        }
        public IRWOImagesLabelMapping GetIRWOImageLabelMappingById(Guid irwoimagelabelmapping_id)
        {
            return context.IRWOImagesLabelMapping.Where(x => x.irwoimagelabelmapping_id == irwoimagelabelmapping_id && !x.is_deleted).FirstOrDefault();
        }
    }
}
