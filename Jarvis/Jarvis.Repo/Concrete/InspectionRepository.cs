using Jarvis.db.DBResponseModel;
using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResponseStatusNumber = Jarvis.Shared.StatusEnums.ResponseStatusNumber;

namespace Jarvis.Repo.Concrete
{
    public class InspectionRepository : BaseGenericRepository<Inspection>, IInspectionRepository
    {
        //private IBaseGenericRepository<User> userRepository;
        //private readonly DBContextFactory context;
        //private DbSet<T> dbSet;

        public InspectionRepository(DBContextFactory dataContext) : base(dataContext)
        {
            //this.context = dataContext;
            //this.userRepository = userRepository;
            //dbSet = context.Set<User>();
        }

        public virtual async Task<int> Insert(Inspection entity)
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
                    IsSuccess = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                }
            }
            catch (Exception e)
            {
                IsSuccess = (int)Shared.StatusEnums.ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public List<Inspection> GetAllInspections()
        {
            return context.Inspection.Include(x => x.Sites).Include(y => y.Sites.Company).Include(z => z.Asset).ToListAsync().Result;
        }

        public Inspection GetInspectionById(string inspectionid, string userid)
        {
            Inspection inspection = new Inspection();
            if (userid != null)
            {

                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(GenericRequestModel.role_id))
                {
                    rolename = context.UserRoles.Where(x => x.role_id.ToString() == GenericRequestModel.role_id && x.status == (int)Status.Active && x.user_id == GenericRequestModel.requested_by).Select(x => x.Role.name).FirstOrDefault();
                }

                IQueryable<Inspection> query = context.Inspection;
                if (!string.IsNullOrEmpty(rolename))
                {
                    if (rolename != GlobalConstants.Admin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid).Select(y => y.site_id).ToList();

                        inspection = context.Inspection.Include(x => x.Asset)
                            .Where(z => usersites.Contains(z.Asset.site_id) && z.inspection_id.ToString() == inspectionid)
                            .Include(x => x.User)
                            .Include(y => y.Sites)
                            .Include(z => z.Sites.Company)
                            .FirstOrDefault();
                    }
                    else
                    {
                        inspection = context.Inspection.Include(x => x.Asset)
                                   .Where(z => z.inspection_id.ToString() == inspectionid)
                                   .Include(x => x.User)
                                   .Include(y => y.Sites)
                                   .Include(z => z.Sites.Company)
                                   .FirstOrDefault();
                    }
                }
            }
            return inspection;
        }
        public Inspection GetInspectionByIdForOperator(string inspectionid, string userid)
        {
            Inspection inspection = new Inspection();
            if (userid != null)
            {
                var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid).Select(y => y.site_id).ToList();

                inspection = context.Inspection.Include(x => x.Asset)
                    .Where(z => usersites.Contains(z.Asset.site_id) && z.inspection_id.ToString() == inspectionid)
                    .Include(x => x.User)
                    .Include(y => y.Sites)
                    .Include(z => z.Sites.Company)
                    .FirstOrDefault();

            }
            return inspection;
        }

    public string FindUserNameById(Guid operator_id)
        {
            return context.User.Where(x => x.uuid == operator_id).FirstOrDefault().username;
        }

        public InspectionFormAttributes GetAttributesCategoryFromId(Guid attributes_id)
        {
            var attributes = context.InspectionFormAttributes.Where(x => x.attributes_id == attributes_id).FirstOrDefault();
            return attributes;
        }

        public InspectionFormAttributes GetAttributesFromName(string attributes_name)
        {
            //int attributescategory = 0;
            return context.InspectionFormAttributes.Where(x => x.name == attributes_name).FirstOrDefault();
            //if (attributes != null)
            //{
            //    attributescategory = attributes.category_id;
            //}
            //return attributescategory;
        }

        public async Task<List<Inspection>> PendingInspection(string userid)
        {
            List<Inspection> inspections = new List<Inspection>();
            if (userid != null)
            {
                //var time = DateTime.UtcNow.AddHours(-8).Date;

                //var usersites = await context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToListAsync();
                List<Guid> usersites = new List<Guid>();
                if (String.IsNullOrEmpty(GenericRequestModel.site_id))
                {
                    usersites = await context.User.Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).Select(x => x.ac_default_site.Value).ToListAsync();
                }
                else if(GenericRequestModel.site_status == (int)Status.AllSiteType)
                {
                    usersites = await context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToListAsync();
                }
                else
                {
                    usersites = await context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active && x.site_id.ToString() == GenericRequestModel.site_id).Select(x => x.site_id).ToListAsync();
                }
                // For Todays Pending Request as per UTC Time date
                //inspections = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset)
                //    .Where(x => usersites.Contains(x.site_id) && (x.status == (int)Status.Pending)
                //    && x.created_at.Date == DateTime.UtcNow.Date).OrderByDescending(x => x.created_at).ToListAsync();


                // For Todays Pending Request as per PST Time date
                //inspections = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset)
                //    .Where(x => usersites.Contains(x.Asset.site_id) && (x.status == (int)Status.Pending)
                //    && x.created_at.AddHours(-8).Date == time.Date).OrderByDescending(x => x.created_at).ToListAsync();

                // For All Pending Request
                inspections = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset).ThenInclude(x => x.Issues)
                    .Where(x => usersites.Contains(x.site_id) && x.status == (int)Status.Pending).OrderBy(x => x.created_at).ToListAsync();
            }
            return inspections;
        }


        public async Task<List<Inspection>> PendingInspectionByOperator(string userid)
        {
            List<Inspection> inspections = new List<Inspection>();
            if (userid != null)
            {
                var usersites = await context.UserSites.Where(x => x.user_id.ToString() == userid).Select(x => x.site_id).ToListAsync();
                context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset);
                inspections = await context.Inspection.Where(x => x.operator_id.ToString() == userid && x.status == (int)Status.Pending).ToListAsync();
            }
            return inspections;
        }

        public async Task<List<Inspection>> CheckOutAssets(string userid)
        {
            List<Inspection> responseassets = new List<Inspection>();
            if (userid != null)
            {
                //var usersites = await context.UserSites.Where(x => x.user_id.ToString() == userid).Select(x => x.site_id).ToListAsync();
                //var inspection = context.Inspection.Where(x => usersites.Contains(x.Asset.site_id) && x.modified_at.Date == DateTime.UtcNow.Date).Select(x => x.asset_id).FirstOrDefault();
                //if (inspection != null && inspection != Guid.Empty)
                //{
                //    var assets = await context.Assets.Include(x => x.Sites).Include(x => x.Sites.Company)
                //        .Include(x => x.Inspection)
                //        .Where(x => usersites.Contains(x.site_id)).ToListAsync();
                //    var assetList = assets.ToList();

                //    var list = from asset in assetList
                //               from inspec in asset.Inspection
                //               where inspec.status == (int)Status.Approved
                //               select asset;
                //    responseassets = list.ToList();
                //}

                //var currentTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Pacific Standard Time");
                DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles"));
                //DateTime currentTime = DateTime.Now;
                var diff = currentTime - DateTime.UtcNow;
                var time = DateTime.UtcNow.AddHours(diff.Hours).AddMinutes(diff.Minutes).Date;

                if (userid != null)
                {
                    //var usersites = await context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToListAsync();\

                    //responseassets = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset)
                    //    .Where(x => usersites.Contains(x.site_id) && (x.status == (int)Status.Approved)
                    //    && x.modified_at.Date == DateTime.UtcNow.Date).OrderByDescending(x => x.modified_at).ToListAsync();
                    List<Guid> usersites = new List<Guid>();
                    if (String.IsNullOrEmpty(GenericRequestModel.site_id))
                    {
                        usersites = await context.User.Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).Select(x => x.ac_active_site.Value).ToListAsync();
                    }
                    else if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                    {
                        usersites = await context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToListAsync();
                    }
                    else
                    {
                        usersites = await context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active && x.site_id.ToString() == GenericRequestModel.site_id).Select(x => x.site_id).ToListAsync();
                    }
                    responseassets = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset)
                        .Where(x => usersites.Contains(x.Asset.site_id) && (x.status == (int)Status.Approved)
                        && x.created_at.AddHours(diff.Hours).AddMinutes(diff.Minutes).Date == time.Date).OrderByDescending(x => x.created_at).ToListAsync();
                }
            }
            return responseassets;
        }

        public async Task<List<Inspection>> CheckOutAssetsByOperator(string userid)
        {
            List<Inspection> responseassets = new List<Inspection>();
            if (userid != null)
            {
                if (userid != null)
                {
                    DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles"));
                    var diff = currentTime - DateTime.UtcNow;

                    //var time = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Pacific Standard Time").Date;
                    var time = DateTime.UtcNow.AddHours(diff.Hours).AddMinutes(diff.Minutes).Date;
                    //var time = DateTime.ParseExact(DateTime.UtcNow.AddHours(-8).ToString(), "yyyy-MM-dd", null).Date;

                    var usersites = await context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToListAsync();

                    responseassets = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset).Include(x => x.StatusMaster)
                        .Where(x => usersites.Contains(x.Asset.site_id) && ((x.operator_id.ToString() == userid) && (
                            (x.status == (int)Status.Approved && x.modified_at.AddHours(diff.Hours).AddMinutes(diff.Minutes).Date == time.Date)
                        || (x.status == (int)Status.Rejected && x.created_at.AddHours(diff.Hours).AddMinutes(diff.Minutes).Date == time.Date)
                        || (x.status == (int)Status.Pending && x.created_at.AddHours(diff.Hours).AddMinutes(diff.Minutes).Date == time.Date)))).OrderByDescending(x => x.created_at).ToListAsync();

                    //responseassets = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset).Include(x => x.StatusMaster)
                    //    .Where(x => usersites.Contains(x.site_id) && ((x.operator_id.ToString() == userid) && ((x.status == (int)Status.Approved && x.modified_at.Date == DateTime.UtcNow.Date)
                    //    || (x.status == (int)Status.Rejected && x.created_at.Date == DateTime.UtcNow.Date)
                    //    || (x.status == (int)Status.Pending && x.created_at.Date == DateTime.UtcNow.Date)))).OrderByDescending(x => x.created_at).ToListAsync();

                    //responseassets = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset)
                    //        .Where(x => usersites.Contains(x.site_id) && ((x.status == (int)Status.Pending && x.created_at.Date == DateTime.UtcNow.Date))).OrderByDescending(x => x.created_at).ToListAsync();
                }
            }
            return responseassets;
        }

        public async Task<int> UpdateInspectionStatus(Guid inspection_id, int status, bool isapproved, string userid)
        {
            int response = (int)ResponseStatusNumber.NotFound;
            if (inspection_id != null && inspection_id != Guid.Empty)
            {
                var inspectionstatus = context.Inspection.Where(x => x.inspection_id == inspection_id).FirstOrDefault();
                if (status == (int)Status.New && inspectionstatus.status > 0)
                {
                    inspectionstatus.manager_id = userid;
                    inspectionstatus.modified_at = DateTime.UtcNow;
                    if (isapproved)
                    {
                        inspectionstatus.status = (int)Status.Approved;
                    }
                    else
                    {
                        inspectionstatus.status = (int)Status.Rejected;
                    }
                }
                try
                {
                    if (inspectionstatus.status > 0)
                    {
                        //context.Inspection.Attach(inspectionstatus)
                        await context.SaveChangesAsync();
                        response = (int)ResponseStatusNumber.Success;
                    }
                }
                catch (Exception e)
                {
                    response = (int)ResponseStatusNumber.Error;
                    throw e;
                }
            }
            return response;
        }

        public async Task<Inspection> ApproveInspectionStatus(ApproveInspectionRequestModel requestModel)
        {
            int response = (int)ResponseStatusNumber.NotFound;
            Inspection inspection = new Inspection();
            if (requestModel.inspection_id != null && requestModel.inspection_id != Guid.Empty)
            {
                var usersites = context.UserSites.Where(x => x.user_id.ToString() == requestModel.manager_id).Select(y => y.site_id).ToList();

                inspection = context.Inspection.Include(x => x.Asset).Include(y => y.Sites).Include(z => z.Sites.Company).Where(z => usersites.Contains(z.site_id) && z.inspection_id == requestModel.inspection_id).FirstOrDefault();

                //inspection = context.Inspection.Include(x=>x.Asset).Where(x => x.inspection_id == requestModel.inspection_id).FirstOrDefault();
                if (inspection != null)
                {
                    inspection.status = requestModel.status;
                    inspection.modified_at = DateTime.UtcNow;
                    inspection.manager_id = requestModel.manager_id;
                    inspection.manager_notes = requestModel.manager_notes;
                    inspection.meter_hours = requestModel.meter_hours;
                    inspection.modified_by = requestModel.manager_id;
                }
                try
                {
                    if (inspection.status > 0)
                    {
                        //context.Inspection.Attach(inspectionstatus)
                        await context.SaveChangesAsync();
                        response = (int)ResponseStatusNumber.Success;
                    }
                }
                catch (Exception e)
                {
                    //response = (int)ResponseStatusNumber.Error;
                    throw e;
                }
            }
            return inspection;
        }


        public async Task<Inspection> GetLastInspectionByInternalAssetId(string internal_asset_id)
        {
            Inspection inspection = new Inspection();

            if (internal_asset_id != null && internal_asset_id != string.Empty)
            {
                inspection = await context.Inspection.Include(x => x.Asset).Where(x => x.Asset.internal_asset_id == internal_asset_id).OrderByDescending(x => x.created_at).FirstOrDefaultAsync();
            }
            return inspection;
        }

        public string GetUserNameById(Guid user_id)
        {
            return context.User.Where(x => x.uuid == user_id)?.Select(x => x.username).FirstOrDefault();
        }

        public async Task<List<Inspection>> GetInspectionByAssetId(string userid, string assetid, int pagesize, int pageindex)
        {
            List<Inspection> inspection = new List<Inspection>();
            string rolename = string.Empty;
            if (!string.IsNullOrEmpty(GenericRequestModel.role_id))
            {
                rolename = context.UserRoles.Where(x => x.role_id.ToString() == GenericRequestModel.role_id && x.status == (int)Status.Active && x.user_id.ToString() == userid).Select(x => x.Role.name).FirstOrDefault();
            }

            IQueryable<Inspection> query = context.Inspection;
            if (!string.IsNullOrEmpty(rolename))
            {
                if (rolename != GlobalConstants.Admin)
                {
                    var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(y => y.site_id).ToList();
                    inspection = await context.Inspection.Include(x => x.StatusMaster).Where(z => usersites.Contains(z.Asset.site_id) && z.asset_id.ToString() == assetid).OrderByDescending(x => x.created_at).ToListAsync();
                }
                else
                {
                    inspection = await context.Inspection.Include(x => x.StatusMaster).Where(z => z.asset_id.ToString() == assetid).OrderByDescending(x => x.created_at).ToListAsync();
                }
            }
            return inspection;
        }

        public async Task<List<Inspection>> SearchInspections(string userid, string searchstrings, string timezone, int pagesize, int pageindex)
        {
            List<Inspection> Inspectionlist = new List<Inspection>();
            string searchstring = searchstrings.ToLower().ToString();

            if (userid != null)
            {
                List<Guid> usersites = new List<Guid>();
                string rolename = context.User.Include(x => x.Role).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).Select(x => x.Role.name).FirstOrDefault();

                if (rolename == GlobalConstants.Admin)
                {
                    usersites = context.Sites.Select(y => y.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active && x.site_id.ToString() == GenericRequestModel.site_id).Select(y => y.site_id).ToList();
                }
                //DateTime dt = Convert.ToDateTime("01/01/2019");
                DateTime dt = DateTime.ParseExact("2019-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                try
                {
                    dt = Convert.ToDateTime(searchstring);
                }
                catch
                {
                    // do nothing;
                }

                DateTime fromdate = new DateTime();
                DateTime todate = new DateTime();

                timezone = timezone.Replace('-', '/');
                if (timezone != null)
                {
                    DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timezone));
                    var diff = currentTime - DateTime.UtcNow;
                    fromdate = dt.AddHours(diff.Hours).AddMinutes(diff.Minutes);
                    todate = fromdate.AddHours(24);
                    Console.WriteLine("Current Time : " + currentTime);
                }
                Console.WriteLine("fromdate : " + fromdate);
                Console.WriteLine("todate : " + todate);
                Console.WriteLine("timezone : " + timezone);

                if (searchstring == String.Empty)
                {
                    Inspectionlist = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset).Include(x => x.User).Include(x => x.StatusMaster).Where(x => usersites.Contains(x.site_id)).ToListAsync();
                }
                else
                {
                    if (dt.Date.ToString("MM/dd/yyyy") != "01/01/2019")
                    {
                        Inspectionlist = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset).Include(x => x.User).Include(x => x.StatusMaster).Where(x => usersites.Contains(x.Asset.site_id)
                    && (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                    || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring) ||
                    (x.datetime_requested.Value >= fromdate && x.datetime_requested.Value <= todate))).ToListAsync();
                    }
                    else
                    {
                        Inspectionlist = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset).Include(x => x.User).Include(x => x.StatusMaster).Where(x => usersites.Contains(x.Asset.site_id)
                      && (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                      || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring))).ToListAsync();
                    }
                }
            }
            return Inspectionlist.OrderByDescending(x => x.datetime_requested).ToList();
        }

        public async Task<List<Inspection>> SearchInspectionsByAsset(string userid, string assetId, string searchstrings, string timezone, int pagesize, int pageindex)
        {
            List<Inspection> Inspectionlist = new List<Inspection>();
            string searchstring = searchstrings.ToLower().ToString();

            if (userid != null)
            {
                List<Guid> usersites = new List<Guid>();
                string rolename = context.User.Include(x => x.Role).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).Select(x => x.Role.name).FirstOrDefault();
                if (rolename == GlobalConstants.Admin)
                {
                    usersites = context.Sites.Select(y => y.site_id).ToList();
                }
                else
                {
                    if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                    {
                        usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(y => y.site_id).ToList();
                    }
                    else
                    {
                        usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.site_id.ToString() == GenericRequestModel.site_id && x.status == (int)Status.Active).Select(y => y.site_id).ToList();
                    }
                }
                DateTime dt = DateTime.ParseExact("2019-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Convert.ToDateTime("01/01/2019",);
                try
                {
                    dt = Convert.ToDateTime(searchstring);
                }
                catch
                {
                    // do nothing;
                }

                if (searchstring == String.Empty)
                {
                    Inspectionlist = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset).Include(x => x.User).Include(x => x.StatusMaster).Where(x => usersites.Contains(x.Asset.site_id) && x.asset_id.ToString() == assetId).ToListAsync();
                }
                else
                {
                    DateTime fromdate = new DateTime();
                    DateTime todate = new DateTime();

                    timezone = timezone.Replace('-', '/');
                    if (timezone != null)
                    {
                        DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timezone));
                        var diff = currentTime - DateTime.UtcNow;
                        fromdate = dt.AddHours(diff.Hours).AddMinutes(diff.Minutes);
                        todate = fromdate.AddHours(24);

                    }

                    if (dt.Date.ToString("MM/dd/yyyy") != "01/01/2019")
                    {
                        Inspectionlist = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset).Include(x => x.User).Where(x => usersites.Contains(x.Asset.site_id)
                        && x.asset_id.ToString().Contains(assetId) && (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                        || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring)
                        || x.meter_hours.ToString().Contains(searchstring) || (x.datetime_requested.Value >= fromdate && x.datetime_requested.Value <= todate))).ToListAsync();
                    }
                    else
                    {
                        Inspectionlist = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset).Include(x => x.User).Include(x => x.StatusMaster).Where(x => usersites.Contains(x.Asset.site_id)
                        && x.asset_id.ToString().Contains(assetId) && (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                        || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring)
                        || x.meter_hours.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring))).ToListAsync();
                    }
                }
                //if (pageindex > 0)
                //{
                //    return Inspectionlist.Skip((pageindex - 1) * pagesize).Take(pagesize).OrderByDescending(x => x.modified_at).ToList();
                //}
                //else
                //{
                return Inspectionlist.OrderByDescending(x => x.created_at).ToList();

                //}
            }
            return Inspectionlist;
        }

        public int CheckPendingInspection(string assetGuId, string operatorId)
        {
            int response = (int)ResponseStatusNumber.Success;
            var usersites = context.UserSites.Where(x => x.user_id.ToString() == operatorId).Select(y => y.site_id).ToList();

            if (usersites.Count > 0)
            {
                var inspection = context.Inspection.Include(x => x.Asset).Where(x => usersites.Contains(x.Asset.site_id) && x.asset_id.ToString() == assetGuId && x.status == (int)Status.Pending).ToList();
                if (inspection.Count > 0)
                {
                    response = (int)AssetStatus.PendingInspection;
                }
                else
                {
                    var asset = context.Assets.Where(x => usersites.Contains(x.site_id) && x.asset_id.ToString() == assetGuId && x.status == (int)Status.InMaintenanace).ToList();
                    if (asset.Count > 0)
                    {
                        response = (int)AssetStatus.AssetInMaintenance;
                    }
                }
            }
            return response;
        }

        public async Task<List<Inspection>> GetAllInspections(string userid, DateTime start, DateTime end)
        {
            List<Inspection> Inspectionlist = new List<Inspection>();

            if (userid != null)
            {
                string role = context.UserRoles.Where(x => x.user_id.ToString() == userid && x.role_id.ToString() == GenericRequestModel.role_id && x.status == (int)Status.Active).Select(x => x.Role.name).FirstOrDefault();
                if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                {
                    var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(y => y.site_id).ToList();
                    Inspectionlist = await context.Inspection.Include(x => x.Asset).Include(x => x.StatusMaster).Where(z => usersites.Contains(z.Asset.site_id) && (z.created_at.Date >= start.Date && z.created_at.Date <= end.Date)).ToListAsync();
                }
                else if (!string.IsNullOrEmpty(role))
                {
                    Inspectionlist = await context.Inspection.Include(x => x.Asset).Include(x => x.StatusMaster).Where(z => GenericRequestModel.site_id == z.Asset.site_id.ToString() && (z.created_at.Date >= start.Date && z.created_at.Date <= end.Date)).ToListAsync();
                }

            }
            return Inspectionlist;
        }

        public int CheckAssetInspectionByTime(string assetGuid, string operatorId, string requested_datetime)
        {
            int response = (int)ResponseStatusNumber.Success;
            var usersites = context.UserSites.Where(x => x.user_id.ToString() == operatorId).Select(y => y.site_id).ToList();

            if (usersites.Count > 0)
            {
                var inspection = context.Inspection.Include(x => x.Asset).Where(x => usersites.Contains(x.Asset.site_id) && x.asset_id.ToString() == assetGuid
                                  && DateTime.Compare(x.datetime_requested.Value, DateTime.Parse(requested_datetime)) == 0 ? true : false).ToList();
                if (inspection.Count > 0)
                {
                    response = (int)AssetStatus.AlreadyHaveInspection;
                }
                //else
                //{
                //    var asset = context.Assets.Where(x => usersites.Contains(x.site_id) && x.asset_id.ToString() == assetGuid && x.status == (int)Status.InMaintenanace).ToList();
                //    if (asset.Count > 0)
                //    {
                //        response = (int)AssetStatus.AssetInMaintenance;
                //    }
                //}
            }
            return response;
        }

        public int GetPendingInspection(string inspection_id)
        {
            int havependinginspection = (int)ResponseStatusNumber.Error;

            var inspection = context.Inspection.Where(x => x.inspection_id.ToString() == inspection_id).FirstOrDefault();

            if (inspection != null)
            {
                var pending = context.Inspection.Where(x => x.created_at <= inspection.created_at && x.asset_id == inspection.asset_id && x.status == (int)Status.Pending && x.inspection_id != inspection.inspection_id).ToList();
                if (pending.Count > 0)
                {
                    havependinginspection = (int)ResponseStatusNumber.AlreadyExists;
                }
                else
                {
                    havependinginspection = (int)ResponseStatusNumber.Success;
                }
            }
            else
            {
                havependinginspection = (int)ResponseStatusNumber.NotFound;
            }
            return havependinginspection;
        }

        public MasterData GetMasterData()
        {
            return context.MasterData.FirstOrDefault();
        }

        public async Task<List<Inspection>> FindAllPeningInspection()
        {
            return await context.Inspection.Where(x => x.status == (int)Status.Pending)
                .Include(x => x.Sites)
                .Include(x => x.Asset)
                .Include(x => x.User).ToListAsync();
        }

        public InspectionListResponseModel GetInspections(string internal_asset_id, Nullable<DateTime> from_date, Nullable<DateTime> to_date)
        {
            InspectionListResponseModel responseModel = new InspectionListResponseModel();

            IQueryable<Inspection> query = context.Inspection.Where(x => x.Asset.internal_asset_id == internal_asset_id);

            if (from_date != null && to_date != null)
            {
                query = query.Where(x => x.datetime_requested.Value.Date >= from_date.Value.Date && x.datetime_requested.Value.Date <= to_date.Value.Date);
            }

            responseModel.list_size = query.Count();

            responseModel.Inspection = query.Include(x => x.User).Include(x => x.StatusMaster).Include(x => x.Sites).Include(x => x.Asset).ToList();

            responseModel.Inspection = responseModel.Inspection.OrderByDescending(x => x.datetime_requested).ToList();
            return responseModel;
        }

        public ListViewModel<Inspection> FilterInspections(FilterInspectionsRequestModel requestModel)
        {
            ListViewModel<Inspection> inspections = new ListViewModel<Inspection>();
            if (GenericRequestModel.requested_by != null && GenericRequestModel.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(GenericRequestModel.role_id))
                {
                    rolename = context.UserRoles.Where(x => x.role_id.ToString() == GenericRequestModel.role_id && x.status == (int)Status.Active && x.user_id == GenericRequestModel.requested_by).Select(x => x.Role.name).FirstOrDefault();
                }

                IQueryable<Inspection> query = context.Inspection;
                if (!string.IsNullOrEmpty(rolename))
                {
                    if (rolename != GlobalConstants.Admin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == GenericRequestModel.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if(GenericRequestModel.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Include(x => x.Asset).Where(x => usersites.Contains(x.Asset.site_id));
                        }
                        else
                        {
                            if (rolename == GlobalConstants.Executive && requestModel.site_id?.Count > 0)
                            {
                                query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()) && usersites.Contains(x.Asset.site_id));
                            }
                            else
                            {
                                query = query.Include(x => x.Asset).Where(x => x.Asset.site_id.ToString() == GenericRequestModel.site_id && usersites.Contains(x.Asset.site_id));
                            }
                        }
                    }
                    else if (rolename == GlobalConstants.Admin)
                    {
                        if (GenericRequestModel.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == GenericRequestModel.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id));
                            }
                            else
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                        else if (GenericRequestModel.company_status == (int)Status.AllCompanyType)
                        {
                            if (GenericRequestModel.site_status != (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                    }

                    if (requestModel.site_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()));
                    }

                    if (requestModel.company_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).ThenInclude(x=>x.Sites.Company).Where(x => requestModel.company_id.Contains(x.Asset.Sites.company_id.ToString()));
                    }

                    //inspections = query.Include(x => x.Asset).Include(x => x.User).Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Issues).ToListAsync().Result;


                    // add status filter
                    if (requestModel.status > 0)
                    {
                        query = query.Where(x => x.status == requestModel.status);
                    }

                    // add asset_id Filter
                    if (requestModel.asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.asset_id.Contains(x.Asset.asset_id.ToString()));
                    }

                    // add internal_asset_id Filter
                    if (requestModel.internal_asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.internal_asset_id.Contains(x.Asset.internal_asset_id));
                    }

                    // add shiftNumber Filter
                    if (requestModel.shift_number?.Count > 0)
                    {
                        query = query.Where(x => requestModel.shift_number.Contains(x.shift));
                    }

                    // add operator ID Filter
                    if (requestModel.requestor_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.requestor_id.Contains(x.operator_id.ToString()));
                    }

                    // add supervisor ID Filter
                    if (requestModel.manager_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.manager_id.Contains(x.manager_id));
                    }

                    // onlt new not ok or all inspections
                    if (requestModel.new_not_ok_attribute == 1)
                    {
                        query = query.Where(x => x.Issues != null && x.Issues.Count > 0);
                    }

                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        // add search string filter
                        string searchstring = requestModel.search_string.ToLower().ToString();
                        DateTime dt = DateTime.ParseExact("2019-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        try
                        {
                            dt = Convert.ToDateTime(searchstring);
                        }
                        catch
                        {
                            // do nothing;
                        }

                        DateTime fromdate = new DateTime();
                        DateTime todate = new DateTime();

                        requestModel.timezone = requestModel.timezone.Replace('-', '/');
                        if (requestModel.timezone != null)
                        {
                            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(requestModel.timezone));
                            var diff = currentTime - DateTime.UtcNow;
                            fromdate = dt.AddHours(diff.Hours).AddMinutes(diff.Minutes);
                            todate = fromdate.AddHours(24);
                            Console.WriteLine("Current Time : " + currentTime);
                        }
                        Console.WriteLine("fromdate : " + fromdate);
                        Console.WriteLine("todate : " + todate);
                        Console.WriteLine("timezone : " + requestModel.timezone);

                        if (dt.Date.ToString("MM/dd/yyyy") != "01/01/2019")
                        {
                            query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring) ||
                            (x.datetime_requested.Value >= fromdate && x.datetime_requested.Value <= todate)));
                        }
                        else
                        {
                            query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring)));
                        }
                    }

                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    inspections.listsize = query.Count();
                    query = query.OrderByDescending(x => x.datetime_requested).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
                    inspections.list = query.Include(x => x.Asset).Include(x => x.User).Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Issues).ToListAsync().Result;
                    inspections.pageIndex = requestModel.pageindex;
                    inspections.pageSize = requestModel.pagesize;
                }
            }

            return inspections;
        }

        public ListViewModel<Inspection> FilterInspectionAssetNameOptions(FilterInspectionOptionsRequestModel requestModel)
        {
            ListViewModel<Inspection> inspections = new ListViewModel<Inspection>();
            if (GenericRequestModel.requested_by != null && GenericRequestModel.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(GenericRequestModel.role_name))
                {
                    //rolename = context.UserRoles.Where(x => x.role_id.ToString() == GenericRequestModel.role_id && x.status == (int)Status.Active && x.user_id == GenericRequestModel.requested_by).Select(x => x.Role.name).FirstOrDefault();
                    rolename = GenericRequestModel.role_name;
                }

                IQueryable<Inspection> query = context.Inspection;
                if (!string.IsNullOrEmpty(rolename))
                {
                    if (rolename != GlobalConstants.Admin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == GenericRequestModel.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Include(x => x.Asset).Where(x => usersites.Contains(x.Asset.site_id));
                        }
                        else
                        {
                            if (rolename == GlobalConstants.Executive && requestModel.site_id?.Count > 0)
                            {
                                query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()) && usersites.Contains(x.Asset.site_id));
                            }
                            else
                            {
                                query = query.Include(x => x.Asset).Where(x => x.Asset.site_id.ToString() == GenericRequestModel.site_id && usersites.Contains(x.Asset.site_id));
                            }
                        }
                    }
                    else if (rolename == GlobalConstants.Admin)
                    {
                        if (GenericRequestModel.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == GenericRequestModel.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id));
                            }
                            else
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                        else if (GenericRequestModel.company_status == (int)Status.AllCompanyType)
                        {
                            if (GenericRequestModel.site_status != (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                    }

                    if (requestModel.site_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()));
                    }

                    if (requestModel.company_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).ThenInclude(x => x.Sites.Company).Where(x => requestModel.company_id.Contains(x.Asset.Sites.company_id.ToString()));
                    }

                    //inspections = query.Include(x => x.Asset).Include(x => x.User).Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Issues).ToListAsync().Result;

                    // add status filter
                    if (requestModel.status > 0)
                    {
                        query = query.Where(x => x.status == requestModel.status);
                    }

                    // add asset_id Filter
                    if (requestModel.asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.asset_id.Contains(x.Asset.asset_id.ToString()));
                    }

                    // add internal_asset_id Filter
                    if (requestModel.internal_asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.internal_asset_id.Contains(x.Asset.internal_asset_id));
                    }

                    // add shiftNumber Filter
                    if (requestModel.shift_number?.Count > 0)
                    {
                        query = query.Where(x => requestModel.shift_number.Contains(x.shift));
                    }

                    // add operator ID Filter
                    if (requestModel.requestor_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.requestor_id.Contains(x.operator_id.ToString()));
                    }

                    // add supervisor ID Filter
                    if (requestModel.manager_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.manager_id.Contains(x.manager_id));
                    }

                    // onlt new not ok or all inspections
                    if (requestModel.new_not_ok_attribute == 1)
                    {
                        query = query.Where(x => x.Issues != null && x.Issues.Count > 0);
                    }

                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        // add search string filter
                        string searchstring = requestModel.search_string.ToLower().ToString();
                        DateTime dt = DateTime.ParseExact("2019-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        try
                        {
                            dt = Convert.ToDateTime(searchstring);
                        }
                        catch
                        {
                            // do nothing;
                        }

                        DateTime fromdate = new DateTime();
                        DateTime todate = new DateTime();

                        requestModel.timezone = requestModel.timezone.Replace('-', '/');
                        if (requestModel.timezone != null)
                        {
                            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(requestModel.timezone));
                            var diff = currentTime - DateTime.UtcNow;
                            fromdate = dt.AddHours(diff.Hours).AddMinutes(diff.Minutes);
                            todate = fromdate.AddHours(24);
                            Console.WriteLine("Current Time : " + currentTime);
                        }
                        Console.WriteLine("fromdate : " + fromdate);
                        Console.WriteLine("todate : " + todate);
                        Console.WriteLine("timezone : " + requestModel.timezone);

                        if (dt.Date.ToString("MM/dd/yyyy") != "01/01/2019")
                        {
                            query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring) ||
                            (x.datetime_requested.Value >= fromdate && x.datetime_requested.Value <= todate)));
                        }
                        else
                        {
                            query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring)));
                        }
                    }

                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        // add option search string filter
                        string searchstring = requestModel.option_search_string.ToLower().ToString();
                        query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)));
                    }

                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    inspections.listsize = query.Count();
                    query = query.OrderByDescending(x => x.datetime_requested).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
                    inspections.list = query.Include(x => x.Asset).ToListAsync().Result;
                    inspections.pageIndex = requestModel.pageindex;
                    inspections.pageSize = requestModel.pagesize;
                }
            }

            return inspections;
        }

        public List<Inspection> FilterInspectionStatusOptions(FilterInspectionOptionsRequestModel requestModel)
        {
            List<Inspection> inspections = new List<Inspection>();
            if (GenericRequestModel.requested_by != null && GenericRequestModel.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(GenericRequestModel.role_name))
                {
                    //rolename = context.UserRoles.Where(x => x.role_id.ToString() == GenericRequestModel.role_id && x.status == (int)Status.Active && x.user_id == GenericRequestModel.requested_by).Select(x => x.Role.name).FirstOrDefault();
                    rolename = GenericRequestModel.role_name;
                }

                IQueryable<Inspection> query = context.Inspection;
                if (!string.IsNullOrEmpty(rolename))
                {
                    if (rolename != GlobalConstants.Admin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == GenericRequestModel.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Include(x => x.Asset).Where(x => usersites.Contains(x.Asset.site_id));
                        }
                        else
                        {
                            if (rolename == GlobalConstants.Executive && requestModel.site_id?.Count > 0)
                            {
                                query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()) && usersites.Contains(x.Asset.site_id));
                            }
                            else
                            {
                                query = query.Include(x => x.Asset).Where(x => x.Asset.site_id.ToString() == GenericRequestModel.site_id && usersites.Contains(x.Asset.site_id));
                            }
                        }
                    }
                    else if (rolename == GlobalConstants.Admin)
                    {
                        if (GenericRequestModel.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == GenericRequestModel.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id));
                            }
                            else
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                        else if (GenericRequestModel.company_status == (int)Status.AllCompanyType)
                        {
                            if (GenericRequestModel.site_status != (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                    }

                    if (requestModel.site_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()));
                    }

                    if (requestModel.company_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).ThenInclude(x => x.Sites.Company).Where(x => requestModel.company_id.Contains(x.Asset.Sites.company_id.ToString()));
                    }

                    //inspections = query.Include(x => x.Asset).Include(x => x.User).Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Issues).ToListAsync().Result;
                    inspections = query.ToList();

                    // add status filter
                    if (requestModel.status > 0)
                    {
                        inspections = inspections.Where(x => x.status == requestModel.status).ToList();
                    }

                    // add asset_id Filter
                    if (requestModel.asset_id?.Count > 0)
                    {
                        inspections = inspections.Where(x => requestModel.asset_id.Contains(x.Asset.asset_id.ToString())).ToList();
                    }

                    // add internal_asset_id Filter
                    if (requestModel.internal_asset_id?.Count > 0)
                    {
                        inspections = inspections.Where(x => requestModel.internal_asset_id.Contains(x.Asset.internal_asset_id)).ToList();
                    }

                    // add shiftNumber Filter
                    if (requestModel.shift_number?.Count > 0)
                    {
                        inspections = inspections.Where(x => requestModel.shift_number.Contains(x.shift)).ToList();
                    }

                    // add operator ID Filter
                    if (requestModel.requestor_id?.Count > 0)
                    {
                        inspections = inspections.Where(x => requestModel.requestor_id.Contains(x.operator_id.ToString())).ToList();
                    }

                    // add supervisor ID Filter
                    if (requestModel.manager_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.manager_id.Contains(x.manager_id));
                    }

                    // onlt new not ok or all inspections
                    if (requestModel.new_not_ok_attribute == 1)
                    {
                        inspections = inspections.Where(x => x.Issues?.Count > 0).ToList();
                    }

                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        // add search string filter
                        string searchstring = requestModel.search_string.ToLower().ToString();
                        DateTime dt = DateTime.ParseExact("2019-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        try
                        {
                            dt = Convert.ToDateTime(searchstring);
                        }
                        catch
                        {
                            // do nothing;
                        }

                        DateTime fromdate = new DateTime();
                        DateTime todate = new DateTime();

                        requestModel.timezone = requestModel.timezone.Replace('-', '/');
                        if (requestModel.timezone != null)
                        {
                            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(requestModel.timezone));
                            var diff = currentTime - DateTime.UtcNow;
                            fromdate = dt.AddHours(diff.Hours).AddMinutes(diff.Minutes);
                            todate = fromdate.AddHours(24);
                            Console.WriteLine("Current Time : " + currentTime);
                        }
                        Console.WriteLine("fromdate : " + fromdate);
                        Console.WriteLine("todate : " + todate);
                        Console.WriteLine("timezone : " + requestModel.timezone);

                        if (dt.Date.ToString("MM/dd/yyyy") != "01/01/2019")
                        {
                            inspections = inspections.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring) ||
                            (x.datetime_requested.Value >= fromdate && x.datetime_requested.Value <= todate))).ToList();
                        }
                        else
                        {
                            inspections = inspections.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring))).ToList();
                        }
                    }

                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        // add option search string filter
                        string searchstring = requestModel.option_search_string.ToLower().ToString();
                        inspections = inspections.Where(x => (x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring))).ToList();
                    }
                }
            }

            return inspections.OrderByDescending(x => x.datetime_requested).ToList();
        }

        public ListViewModel<Inspection> FilterInspectionShiftNumberOptions(FilterInspectionOptionsRequestModel requestModel)
        {
            ListViewModel<Inspection> inspections = new ListViewModel<Inspection>();
            if (GenericRequestModel.requested_by != null && GenericRequestModel.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(GenericRequestModel.role_name))
                {
                    //rolename = context.UserRoles.Where(x => x.role_id.ToString() == GenericRequestModel.role_id && x.status == (int)Status.Active && x.user_id == GenericRequestModel.requested_by).Select(x => x.Role.name).FirstOrDefault();
                    rolename = GenericRequestModel.role_name;
                }

                IQueryable<Inspection> query = context.Inspection;
                if (!string.IsNullOrEmpty(rolename))
                {
                    if (rolename != GlobalConstants.Admin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == GenericRequestModel.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Include(x => x.Asset).Where(x => usersites.Contains(x.Asset.site_id));
                        }
                        else
                        {
                            if (rolename == GlobalConstants.Executive && requestModel.site_id?.Count > 0)
                            {
                                query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()) && usersites.Contains(x.Asset.site_id));
                            }
                            else
                            {
                                query = query.Include(x => x.Asset).Where(x => x.Asset.site_id.ToString() == GenericRequestModel.site_id && usersites.Contains(x.Asset.site_id));
                            }
                        }
                    }
                    else if (rolename == GlobalConstants.Admin)
                    {
                        if (GenericRequestModel.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == GenericRequestModel.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id));
                            }
                            else
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                        else if (GenericRequestModel.company_status == (int)Status.AllCompanyType)
                        {
                            if (GenericRequestModel.site_status != (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                    }

                    if (requestModel.site_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()));
                    }

                    if (requestModel.company_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).ThenInclude(x => x.Sites.Company).Where(x => requestModel.company_id.Contains(x.Asset.Sites.company_id.ToString()));
                    }

                    //inspections = query.Include(x => x.Asset).Include(x => x.User).Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Issues).ToListAsync().Result;
                    // add status filter
                    if (requestModel.status > 0)
                    {
                        query = query.Where(x => x.status == requestModel.status);
                    }

                    // add asset_id Filter
                    if (requestModel.asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.asset_id.Contains(x.Asset.asset_id.ToString()));
                    }

                    // add internal_asset_id Filter
                    if (requestModel.internal_asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.internal_asset_id.Contains(x.Asset.internal_asset_id));
                    }

                    // add shiftNumber Filter
                    if (requestModel.shift_number?.Count > 0)
                    {
                        query = query.Where(x => requestModel.shift_number.Contains(x.shift));
                    }

                    // add operator ID Filter
                    if (requestModel.requestor_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.requestor_id.Contains(x.operator_id.ToString()));
                    }

                    // add supervisor ID Filter
                    if (requestModel.manager_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.manager_id.Contains(x.manager_id));
                    }

                    // onlt new not ok or all inspections
                    if (requestModel.new_not_ok_attribute == 1)
                    {
                        query = query.Where(x => x.Issues != null && x.Issues.Count > 0);
                    }

                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        // add search string filter
                        string searchstring = requestModel.search_string.ToLower().ToString();
                        DateTime dt = DateTime.ParseExact("2019-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        try
                        {
                            dt = Convert.ToDateTime(searchstring);
                        }
                        catch
                        {
                            // do nothing;
                        }

                        DateTime fromdate = new DateTime();
                        DateTime todate = new DateTime();

                        requestModel.timezone = requestModel.timezone.Replace('-', '/');
                        if (requestModel.timezone != null)
                        {
                            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(requestModel.timezone));
                            var diff = currentTime - DateTime.UtcNow;
                            fromdate = dt.AddHours(diff.Hours).AddMinutes(diff.Minutes);
                            todate = fromdate.AddHours(24);
                            Console.WriteLine("Current Time : " + currentTime);
                        }
                        Console.WriteLine("fromdate : " + fromdate);
                        Console.WriteLine("todate : " + todate);
                        Console.WriteLine("timezone : " + requestModel.timezone);

                        if (dt.Date.ToString("MM/dd/yyyy") != "01/01/2019")
                        {
                            query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring) ||
                            (x.datetime_requested.Value >= fromdate && x.datetime_requested.Value <= todate)));
                        }
                        else
                        {
                            query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring)));
                        }
                    }

                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        // add option search string filter
                        string searchstring = requestModel.option_search_string.ToLower().ToString();
                        query = query.Where(x => (x.shift.ToString() == searchstring));
                    }

                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }

                    inspections.listsize = query.Count();
                    query = query.OrderByDescending(x => x.datetime_requested).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
                    inspections.list = query.ToListAsync().Result;
                    inspections.pageIndex = requestModel.pageindex;
                    inspections.pageSize = requestModel.pagesize;
                }
            }

            return inspections;
        }

        public ListViewModel<Inspection> FilterInspectionOperatorsOptions(FilterInspectionOptionsRequestModel requestModel)
        {
            ListViewModel<Inspection> inspections = new ListViewModel<Inspection>();
            if (GenericRequestModel.requested_by != null && GenericRequestModel.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(GenericRequestModel.role_name))
                {
                    //rolename = context.UserRoles.Where(x => x.role_id.ToString() == GenericRequestModel.role_id && x.status == (int)Status.Active && x.user_id == GenericRequestModel.requested_by).Select(x => x.Role.name).FirstOrDefault();
                    rolename = GenericRequestModel.role_name;
                }

                IQueryable<Inspection> query = context.Inspection;
                if (!string.IsNullOrEmpty(rolename))
                {
                    if (rolename != GlobalConstants.Admin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == GenericRequestModel.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Include(x => x.Asset).Where(x => usersites.Contains(x.Asset.site_id));
                        }
                        else
                        {
                            if (rolename == GlobalConstants.Executive && requestModel.site_id?.Count > 0)
                            {
                                query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()) && usersites.Contains(x.Asset.site_id));
                            }
                            else
                            {
                                query = query.Include(x => x.Asset).Where(x => x.Asset.site_id.ToString() == GenericRequestModel.site_id && usersites.Contains(x.Asset.site_id));
                            }
                        }
                    }
                    else if (rolename == GlobalConstants.Admin)
                    {
                        if (GenericRequestModel.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == GenericRequestModel.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id));
                            }
                            else
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                        else if (GenericRequestModel.company_status == (int)Status.AllCompanyType)
                        {
                            if (GenericRequestModel.site_status != (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                    }

                    if (requestModel.site_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()));
                    }

                    if (requestModel.company_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).ThenInclude(x => x.Sites.Company).Where(x => requestModel.company_id.Contains(x.Asset.Sites.company_id.ToString()));
                    }

                    //inspections = query.Include(x => x.Asset).Include(x => x.User).Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Issues).ToListAsync().Result;
                    //inspections = query.ToList();

                    // add status filter
                    if (requestModel.status > 0)
                    {
                        query = query.Where(x => x.status == requestModel.status);
                    }

                    // add asset_id Filter
                    if (requestModel.asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.asset_id.Contains(x.Asset.asset_id.ToString()));
                    }

                    // add internal_asset_id Filter
                    if (requestModel.internal_asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.internal_asset_id.Contains(x.Asset.internal_asset_id));
                    }

                    // add shiftNumber Filter
                    if (requestModel.shift_number?.Count > 0)
                    {
                        query = query.Where(x => requestModel.shift_number.Contains(x.shift));
                    }

                    // add operator ID Filter
                    if (requestModel.requestor_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.requestor_id.Contains(x.operator_id.ToString()));
                    }

                    // add supervisor ID Filter
                    if (requestModel.manager_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.manager_id.Contains(x.manager_id));
                    }

                    // onlt new not ok or all inspections
                    if (requestModel.new_not_ok_attribute == 1)
                    {
                        query = query.Where(x => x.Issues != null && x.Issues.Count > 0);
                    }

                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        // add search string filter
                        string searchstring = requestModel.search_string.ToLower().ToString();
                        DateTime dt = DateTime.ParseExact("2019-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        try
                        {
                            dt = Convert.ToDateTime(searchstring);
                        }
                        catch
                        {
                            // do nothing;
                        }

                        DateTime fromdate = new DateTime();
                        DateTime todate = new DateTime();

                        requestModel.timezone = requestModel.timezone.Replace('-', '/');
                        if (requestModel.timezone != null)
                        {
                            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(requestModel.timezone));
                            var diff = currentTime - DateTime.UtcNow;
                            fromdate = dt.AddHours(diff.Hours).AddMinutes(diff.Minutes);
                            todate = fromdate.AddHours(24);
                            Console.WriteLine("Current Time : " + currentTime);
                        }
                        Console.WriteLine("fromdate : " + fromdate);
                        Console.WriteLine("todate : " + todate);
                        Console.WriteLine("timezone : " + requestModel.timezone);

                        if (dt.Date.ToString("MM/dd/yyyy") != "01/01/2019")
                        {
                            query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring) ||
                            (x.datetime_requested.Value >= fromdate && x.datetime_requested.Value <= todate)));
                        }
                        else
                        {
                            query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring)));
                        }
                    }

                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        // add option search string filter
                        string searchstring = requestModel.option_search_string.ToLower().ToString();
                        query = query.Where(x => (x.User.username.ToLower().Contains(searchstring) || x.User.firstname.ToLower().Contains(searchstring) || x.User.lastname.ToLower().Contains(searchstring)));
                    }

                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    inspections.listsize = query.Count();
                    query = query.OrderByDescending(x => x.datetime_requested).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
                    inspections.list = query.Include(x=>x.User).ToListAsync().Result;
                    inspections.pageIndex = requestModel.pageindex;
                    inspections.pageSize = requestModel.pagesize;
                }
            }

            return inspections;
        }

        public ListViewModel<Inspection> FilterInspectionSitesOptions(FilterInspectionOptionsRequestModel requestModel)
        {
            ListViewModel<Inspection> inspections = new ListViewModel<Inspection>();
            if (GenericRequestModel.requested_by != null && GenericRequestModel.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(GenericRequestModel.role_name))
                {
                    //rolename = context.UserRoles.Where(x => x.role_id.ToString() == GenericRequestModel.role_id && x.status == (int)Status.Active && x.user_id == GenericRequestModel.requested_by).Select(x => x.Role.name).FirstOrDefault();
                    rolename = GenericRequestModel.role_name;
                }

                IQueryable<Inspection> query = context.Inspection;
                if (!string.IsNullOrEmpty(rolename))
                {
                    if (rolename != GlobalConstants.Admin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == GenericRequestModel.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Include(x => x.Asset).Where(x => usersites.Contains(x.Asset.site_id));
                        }
                        else
                        {
                            if (rolename == GlobalConstants.Executive && requestModel.site_id?.Count > 0)
                            {
                                query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()) && usersites.Contains(x.Asset.site_id));
                            }
                            else
                            {
                                query = query.Include(x => x.Asset).Where(x => x.Asset.site_id.ToString() == GenericRequestModel.site_id && usersites.Contains(x.Asset.site_id));
                            }
                        }
                    }
                    else if (rolename == GlobalConstants.Admin)
                    {
                        if (GenericRequestModel.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == GenericRequestModel.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id));
                            }
                            else
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                        else if (GenericRequestModel.company_status == (int)Status.AllCompanyType)
                        {
                            if (GenericRequestModel.site_status != (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                    }

                    if (requestModel.site_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()));
                    }

                    if (requestModel.company_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).ThenInclude(x => x.Sites.Company).Where(x => requestModel.company_id.Contains(x.Asset.Sites.company_id.ToString()));
                    }

                    //inspections = query.Include(x => x.Asset).Include(x => x.User).Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Issues).ToListAsync().Result;
                    // add status filter
                    if (requestModel.status > 0)
                    {
                        query = query.Where(x => x.status == requestModel.status);
                    }

                    // add asset_id Filter
                    if (requestModel.asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.asset_id.Contains(x.Asset.asset_id.ToString()));
                    }

                    // add internal_asset_id Filter
                    if (requestModel.internal_asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.internal_asset_id.Contains(x.Asset.internal_asset_id));
                    }

                    // add shiftNumber Filter
                    if (requestModel.shift_number?.Count > 0)
                    {
                        query = query.Where(x => requestModel.shift_number.Contains(x.shift));
                    }

                    // add operator ID Filter
                    if (requestModel.requestor_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.requestor_id.Contains(x.operator_id.ToString()));
                    }

                    // add supervisor ID Filter
                    if (requestModel.manager_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.manager_id.Contains(x.manager_id));
                    }

                    // onlt new not ok or all inspections
                    if (requestModel.new_not_ok_attribute == 1)
                    {
                        query = query.Where(x => x.Issues != null && x.Issues.Count > 0);
                    }

                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        // add search string filter
                        string searchstring = requestModel.search_string.ToLower().ToString();
                        DateTime dt = DateTime.ParseExact("2019-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        try
                        {
                            dt = Convert.ToDateTime(searchstring);
                        }
                        catch
                        {
                            // do nothing;
                        }

                        DateTime fromdate = new DateTime();
                        DateTime todate = new DateTime();

                        requestModel.timezone = requestModel.timezone.Replace('-', '/');
                        if (requestModel.timezone != null)
                        {
                            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(requestModel.timezone));
                            var diff = currentTime - DateTime.UtcNow;
                            fromdate = dt.AddHours(diff.Hours).AddMinutes(diff.Minutes);
                            todate = fromdate.AddHours(24);
                            Console.WriteLine("Current Time : " + currentTime);
                        }
                        Console.WriteLine("fromdate : " + fromdate);
                        Console.WriteLine("todate : " + todate);
                        Console.WriteLine("timezone : " + requestModel.timezone);

                        if (dt.Date.ToString("MM/dd/yyyy") != "01/01/2019")
                        {
                            query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring) ||
                            (x.datetime_requested.Value >= fromdate && x.datetime_requested.Value <= todate)));
                        }
                        else
                        {
                            query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring)));
                        }
                    }

                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        // add option search string filter
                        string searchstring = requestModel.option_search_string.ToLower().ToString();
                        query = query.Where(x => (x.Sites.site_name.ToLower().Contains(searchstring)));
                    }

                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    inspections.listsize = query.Count();
                    query = query.OrderByDescending(x => x.datetime_requested).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
                    inspections.list = query.Include(x => x.Sites).ToListAsync().Result;
                    inspections.pageIndex = requestModel.pageindex;
                    inspections.pageSize = requestModel.pagesize;
                }
            }

            return inspections;
        }

        public ListViewModel<Inspection> FilterInspectionCompanyOptions(FilterInspectionOptionsRequestModel requestModel)
        {
            ListViewModel<Inspection> inspections = new ListViewModel<Inspection>();
            if (GenericRequestModel.requested_by != null && GenericRequestModel.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(GenericRequestModel.role_name))
                {
                    //rolename = context.UserRoles.Where(x => x.role_id.ToString() == GenericRequestModel.role_id && x.status == (int)Status.Active && x.user_id == GenericRequestModel.requested_by).Select(x => x.Role.name).FirstOrDefault();
                    rolename = GenericRequestModel.role_name;
                }

                IQueryable<Inspection> query = context.Inspection;
                if (!string.IsNullOrEmpty(rolename))
                {
                    if (rolename != GlobalConstants.Admin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == GenericRequestModel.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Include(x => x.Asset).Where(x => usersites.Contains(x.Asset.site_id));
                        }
                        else
                        {
                            if (rolename == GlobalConstants.Executive && requestModel.site_id?.Count > 0)
                            {
                                query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()) && usersites.Contains(x.Asset.site_id));
                            }
                            else
                            {
                                query = query.Include(x => x.Asset).Where(x => x.Asset.site_id.ToString() == GenericRequestModel.site_id && usersites.Contains(x.Asset.site_id));
                            }
                        }
                    }
                    else if (rolename == GlobalConstants.Admin)
                    {
                        if (GenericRequestModel.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == GenericRequestModel.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id));
                            }
                            else
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                        else if (GenericRequestModel.company_status == (int)Status.AllCompanyType)
                        {
                            if (GenericRequestModel.site_status != (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                    }

                    if (requestModel.site_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()));
                    }

                    if (requestModel.company_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).ThenInclude(x => x.Sites.Company).Where(x => requestModel.company_id.Contains(x.Asset.Sites.company_id.ToString()));
                    }

                    //inspections = query.Include(x => x.Asset).Include(x => x.User).Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Issues).ToListAsync().Result;
                    
                    // add status filter
                    if (requestModel.status > 0)
                    {
                        query = query.Where(x => x.status == requestModel.status);
                    }

                    // add asset_id Filter
                    if (requestModel.asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.asset_id.Contains(x.Asset.asset_id.ToString()));
                    }

                    // add internal_asset_id Filter
                    if (requestModel.internal_asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.internal_asset_id.Contains(x.Asset.internal_asset_id));
                    }

                    // add shiftNumber Filter
                    if (requestModel.shift_number?.Count > 0)
                    {
                        query = query.Where(x => requestModel.shift_number.Contains(x.shift));
                    }

                    // add operator ID Filter
                    if (requestModel.requestor_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.requestor_id.Contains(x.operator_id.ToString()));
                    }

                    // add supervisor ID Filter
                    if (requestModel.manager_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.manager_id.Contains(x.manager_id));
                    }

                    // onlt new not ok or all inspections
                    if (requestModel.new_not_ok_attribute == 1)
                    {
                        query = query.Where(x => x.Issues != null && x.Issues.Count > 0);
                    }

                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        // add search string filter
                        string searchstring = requestModel.search_string.ToLower().ToString();
                        DateTime dt = DateTime.ParseExact("2019-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        try
                        {
                            dt = Convert.ToDateTime(searchstring);
                        }
                        catch
                        {
                            // do nothing;
                        }

                        DateTime fromdate = new DateTime();
                        DateTime todate = new DateTime();

                        requestModel.timezone = requestModel.timezone.Replace('-', '/');
                        if (requestModel.timezone != null)
                        {
                            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(requestModel.timezone));
                            var diff = currentTime - DateTime.UtcNow;
                            fromdate = dt.AddHours(diff.Hours).AddMinutes(diff.Minutes);
                            todate = fromdate.AddHours(24);
                            Console.WriteLine("Current Time : " + currentTime);
                        }
                        Console.WriteLine("fromdate : " + fromdate);
                        Console.WriteLine("todate : " + todate);
                        Console.WriteLine("timezone : " + requestModel.timezone);

                        if (dt.Date.ToString("MM/dd/yyyy") != "01/01/2019")
                        {
                            query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring) ||
                            (x.datetime_requested.Value >= fromdate && x.datetime_requested.Value <= todate)));
                        }
                        else
                        {
                            query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring)));
                        }
                    }

                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        // add option search string filter
                        string searchstring = requestModel.option_search_string.ToLower().ToString();
                        query = query.Where(x => (x.Sites.Company.company_name.ToLower().Contains(searchstring) || x.Sites.Company.company_code.ToLower().Contains(searchstring)));
                    }

                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    inspections.listsize = query.Count();
                    query = query.OrderByDescending(x => x.datetime_requested).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
                    inspections.list = query.Include(x => x.Sites).Include(y => y.Sites.Company).ToListAsync().Result;
                    inspections.pageIndex = requestModel.pageindex;
                    inspections.pageSize = requestModel.pagesize;
                }
            }

            return inspections;
        }

        public List<Inspection> GetInspectionsForWeeklyReport(List<Guid> siteid, DateTime startdate, DateTime enddate)
        {
            List<Inspection> inspections = new List<Inspection>();
            inspections = context.Inspection.Where(x => siteid.Contains(x.Asset.site_id) && x.created_at >= startdate && x.created_at < enddate).Include(x => x.Asset).Include(x => x.Sites).Include(x => x.StatusMaster).ToList();

            return inspections;
        }

        public List<ImagesListObject> GetAllInspectionsImages()
        {
            ListViewModel<Inspection> inspections = new ListViewModel<Inspection>();
            IQueryable<Inspection> query = context.Inspection.OrderByDescending(x=>x.created_by);
            return query.Where(x => x.image_list != null).Select(x => x.image_list).ToList();
        }

        public ListViewModel<Inspection> FilterInspectionSupervisorOptions(FilterInspectionOptionsRequestModel requestModel)
        {
            ListViewModel<Inspection> inspections = new ListViewModel<Inspection>();
            if (GenericRequestModel.requested_by != null && GenericRequestModel.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(GenericRequestModel.role_name))
                {
                    rolename = GenericRequestModel.role_name;
                }

                IQueryable<Inspection> query = context.Inspection;
                if (!string.IsNullOrEmpty(rolename))
                {
                    if (rolename != GlobalConstants.Admin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == GenericRequestModel.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Include(x => x.Asset).Where(x => usersites.Contains(x.Asset.site_id));
                        }
                        else
                        {
                            if (rolename == GlobalConstants.Executive && requestModel.site_id?.Count > 0)
                            {
                                query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()) && usersites.Contains(x.Asset.site_id));
                            }
                            else
                            {
                                query = query.Include(x => x.Asset).Where(x => x.Asset.site_id.ToString() == GenericRequestModel.site_id && usersites.Contains(x.Asset.site_id));
                            }
                        }
                    }
                    else if (rolename == GlobalConstants.Admin)
                    {
                        if (GenericRequestModel.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == GenericRequestModel.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (GenericRequestModel.site_status == (int)Status.AllSiteType)
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id));
                            }
                            else
                            {
                                query = query.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                        else if (GenericRequestModel.company_status == (int)Status.AllCompanyType)
                        {
                            if (GenericRequestModel.site_status != (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.site_id.ToString() == GenericRequestModel.site_id);
                            }
                        }
                    }

                    if (requestModel.site_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).Where(x => requestModel.site_id.Contains(x.Asset.site_id.ToString()));
                    }

                    if (requestModel.company_id?.Count > 0)
                    {
                        query = query.Include(x => x.Asset).ThenInclude(x => x.Sites.Company).Where(x => requestModel.company_id.Contains(x.Asset.Sites.company_id.ToString()));
                    }

                    // add status filter
                    if (requestModel.status > 0)
                    {
                        query = query.Where(x => x.status == requestModel.status);
                    }

                    // add asset_id Filter
                    if (requestModel.asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.asset_id.Contains(x.Asset.asset_id.ToString()));
                    }

                    // add internal_asset_id Filter
                    if (requestModel.internal_asset_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.internal_asset_id.Contains(x.Asset.internal_asset_id));
                    }

                    // add shiftNumber Filter
                    if (requestModel.shift_number?.Count > 0)
                    {
                        query = query.Where(x => requestModel.shift_number.Contains(x.shift));
                    }

                    // add operator ID Filter
                    if (requestModel.requestor_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.requestor_id.Contains(x.operator_id.ToString()));
                    }

                    // onlt new not ok or all inspections
                    if (requestModel.new_not_ok_attribute == 1)
                    {
                        query = query.Where(x => x.Issues != null && x.Issues.Count > 0);
                    }

                    // add supervisor ID Filter
                    if (requestModel.manager_id?.Count > 0)
                    {
                        query = query.Where(x => requestModel.manager_id.Contains(x.manager_id));
                    }

                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        // add search string filter
                        string searchstring = requestModel.search_string.ToLower().ToString();
                        DateTime dt = DateTime.ParseExact("2019-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        try
                        {
                            dt = Convert.ToDateTime(searchstring);
                        }
                        catch
                        {
                            // do nothing;
                        }

                        DateTime fromdate = new DateTime();
                        DateTime todate = new DateTime();

                        requestModel.timezone = requestModel.timezone.Replace('-', '/');
                        if (requestModel.timezone != null)
                        {
                            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(requestModel.timezone));
                            var diff = currentTime - DateTime.UtcNow;
                            fromdate = dt.AddHours(diff.Hours).AddMinutes(diff.Minutes);
                            todate = fromdate.AddHours(24);
                            Console.WriteLine("Current Time : " + currentTime);
                        }
                        Console.WriteLine("fromdate : " + fromdate);
                        Console.WriteLine("todate : " + todate);
                        Console.WriteLine("timezone : " + requestModel.timezone);

                        if (dt.Date.ToString("MM/dd/yyyy") != "01/01/2019")
                        {
                            query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring) ||
                            (x.datetime_requested.Value >= fromdate && x.datetime_requested.Value <= todate)));
                        }
                        else
                        {
                            query = query.Where(x => (x.Asset.name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                            || x.status.ToString() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.shift.ToString().Contains(searchstring) || x.User.username.ToLower().Contains(searchstring)));
                        }
                    }

                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        // add option search string filter
                        string searchstring = requestModel.option_search_string.ToLower().ToString();
                        var supervisorId = query.Where(x => x.manager_id != null && x.manager_id != "").Select(x => x.manager_id.ToString()).ToList();
                        var users = context.User.Where(x => supervisorId.Contains(x.uuid.ToString()) && (x.firstname.ToLower().Contains(searchstring) || x.lastname.ToLower().Contains(searchstring))).Select(x => x.uuid.ToString()).ToList();
                        query = query.Where(x => users.Contains(x.manager_id));
                        //query = query.Where(x => (x.User.username.ToLower().Contains(searchstring) || x.User.firstname.ToLower().Contains(searchstring) || x.User.lastname.ToLower().Contains(searchstring)));
                    }

                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    inspections.listsize = query.Count();
                    //query = query.OrderByDescending(x => x.datetime_requested).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
                    //inspections.list = query.Where(x => x.manager_id != null && x.manager_id != "").Include(x => x.User).ToListAsync().Result;
                    inspections.list = query.Where(x => x.manager_id != null && x.manager_id != "").ToListAsync().Result;
                    //inspections.pageIndex = requestModel.pageindex;
                    //inspections.pageSize = requestModel.pagesize;
                }
            }

            return inspections;
        }
    }
}