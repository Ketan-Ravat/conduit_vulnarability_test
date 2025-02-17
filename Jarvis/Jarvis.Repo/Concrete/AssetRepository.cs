using Jarvis.db.DBResponseModel;
using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.Helper;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete
{
    public class AssetRepository : BaseGenericRepository<Asset>, IAssetRepository
    {
        //private IBaseGenericRepository<User> userRepository;
        //private readonly DBContextFactory context;
        //private DbSet<T> dbSet;

        public AssetRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }

        public void AddAssets(List<Asset> assets)
        {
            AddRange(assets);
        }

        public void AddAsset(Asset asset)
        {
            Add(asset);
        }

        public virtual async Task<int> Insert(Asset entity)
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
                    //var alreadyregister = context.User.Where(x => x.email == entity.email).FirstOrDefault();
                    //if (alreadyregister != null)
                    //{
                    //    if (alreadyregister.uuid == null || alreadyregister.uuid == Guid.Empty)
                    //    {
                    Add(entity);
                    IsSuccess = (int)ResponseStatusNumber.Success;
                    //}
                    //else
                    //{
                    //    IsSuccess = (int)ResponseStatusNumber.AlreadyExists;
                    //}
                }
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
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
        }

        public async Task<int> UploadAssetPhoto(Asset entity)
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

        public virtual async Task<int> Update(Asset entity)
        {
            int IsSuccess = 0;
            try
            {
                Asset isregister = new Asset();
                if (entity.internal_asset_id != null)
                {
                    isregister = context.Assets.Where(x => x.internal_asset_id == entity.internal_asset_id).FirstOrDefault();
                }
                else
                {
                    isregister = context.Assets.Where(x => x.asset_id == entity.asset_id).FirstOrDefault();
                }

                if (isregister.asset_id != Guid.Empty || isregister.asset_id != null)
                {
                    dbSet.Update(entity);
                    IsSuccess = await context.SaveChangesAsync();
                }
                else
                {
                    IsSuccess = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public Asset GetAssetsByInternalAssetID(string internal_asset_id)
        {
            return context.Assets.Where(x => x.internal_asset_id == internal_asset_id).FirstOrDefault();
        }

        public Asset GetCompanyAssetsByInternalAssetID(string internal_asset_id, string company_id)
        {
            return context.Assets.Where(x => x.internal_asset_id == internal_asset_id && x.company_id == company_id).FirstOrDefault();
        }

        public List<Asset> GetAllAssets(string userid, int status, int pagesize, int pageindex)
        {
            List<Asset> assets = new List<Asset>();
            if (userid != null)
            {
                string rolename = string.Empty;
                //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
                {
                    rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(rolename))
                {
                    var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

                    IQueryable<Asset> query = context.Assets;

                    if (rolename != GlobalConstants.Admin)
                    {
                        if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Where(x => usersites.Contains(x.site_id));
                        }
                        else
                        {
                            query = query.Where(x => usersites.Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
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

                    if (status > 0)
                    {
                        if (status == (int)Status.Active || status == (int)Status.AssetActive)
                        {
                            query = query.Where(x => x.status == (int)Status.AssetActive);
                        }
                        else if (status == (int)Status.Deactive || status == (int)Status.AssetDeactive)
                        {
                            query = query.Where(x => x.status == (int)Status.AssetDeactive);
                        }
                    }

                    assets = query.Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).ToListAsync().Result;
                }
            }
            return assets.OrderByDescending(x => x.created_at).ToList();
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
                // query = query.Skip((pageindex - 1) * pagesize).Take(pagesize).OrderBy(x => x.name);
                assets.list = query.Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Issues).Include(x => x.Inspection).ThenInclude(x => x.StatusMaster).ToList();

            }
            assets.pageIndex = pageindex;
            assets.pageSize = pagesize;


            return assets;
        }
        public ListViewModel<Asset> GetChildrenByAssetID(string asset_id, int pagesize, int pageindex)
        {
            ListViewModel<Asset> assets = new ListViewModel<Asset>();

            var children = context.AssetChildrenHierarchyMapping.Where(x => x.asset_id == Guid.Parse(asset_id) && !x.is_deleted).Select(x => x.children_asset_id).ToList();
            if (children != null)
            {
                IQueryable<Asset> query = context.Assets.Where(x => children.Contains(x.asset_id) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));

                assets.listsize = query.Count();
                if (pageindex > 0 && pagesize > 0)
                {
                    query = query.Skip((pageindex - 1) * pagesize).Take(pagesize).OrderBy(x => x.name);
                }
                else
                {
                    query = query.OrderBy(x => x.name);
                }
                // query = query.Skip((pageindex - 1) * pagesize).Take(pagesize).OrderBy(x => x.name);
                assets.list = query.Include(x => x.Sites).ToList();

            }
            assets.pageIndex = pageindex;
            assets.pageSize = pagesize;


            return assets;
        }
        public (ListViewModel<Asset>, List<AssetFormIOBuildingMappings>) FilterAssets(FilterAssetsRequestModel requestModel)
        {
            ListViewModel<Asset> assets = new ListViewModel<Asset>();
            List<AssetFormIOBuildingMappings> building_hierarchy_list = new List<AssetFormIOBuildingMappings>();
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
                    // add criticality_index_type Filter
                    if (requestModel.criticality_index_type != null && requestModel.criticality_index_type.Count > 0)
                    {
                        query = query.Where(x => requestModel.criticality_index_type.Contains(x.criticality_index_type.Value));
                    }
                    // add condition_index_type Filter
                    if (requestModel.condition_index_type != null && requestModel.condition_index_type.Count > 0)
                    {
                        query = query.Where(x => requestModel.condition_index_type.Contains(x.condition_index_type.Value));
                    }
                    // add inspectiontemplate_asset_class_id Filter
                    if (requestModel.inspectiontemplate_asset_class_id != null && requestModel.inspectiontemplate_asset_class_id.Count > 0)
                    {
                        query = query.Where(x => requestModel.inspectiontemplate_asset_class_id.Contains(x.inspectiontemplate_asset_class_id.Value));
                    }
                    // add formiobuilding_id Filter
                    if (requestModel.formiobuilding_id != null && requestModel.formiobuilding_id.Count > 0)
                    {
                        query = query.Where(x => requestModel.formiobuilding_id.Contains(x.AssetFormIOBuildingMappings.formiobuilding_id.Value));
                    }
                    // add formiofloor_id Filter
                    if (requestModel.formiofloor_id != null && requestModel.formiofloor_id.Count > 0)
                    {
                        query = query.Where(x => requestModel.formiofloor_id.Contains(x.AssetFormIOBuildingMappings.formiofloor_id.Value));
                    }
                    // add formioroom_id Filter
                    if (requestModel.formioroom_id != null && requestModel.formioroom_id.Count > 0)
                    {
                        query = query.Where(x => requestModel.formioroom_id.Contains(x.AssetFormIOBuildingMappings.formioroom_id.Value));
                    }
                    // add formiosection_id Filter
                    if (requestModel.formiosection_id != null && requestModel.formiosection_id.Count > 0)
                    {
                        query = query.Where(x => requestModel.formiosection_id.Contains(x.AssetFormIOBuildingMappings.formiosection_id.Value));
                    }

                    // add formiosection_id Filter
                    if (requestModel.thermal_classification_id != null && requestModel.thermal_classification_id.Count > 0)
                    {
                        query = query.Where(x => requestModel.thermal_classification_id.Contains(x.thermal_classification_id.Value));
                    }
                    // add formiosection_id Filter
                    if (requestModel.asset_operating_condition_state != null && requestModel.asset_operating_condition_state.Count > 0)
                    {
                        query = query.Where(x => requestModel.asset_operating_condition_state.Contains(x.asset_operating_condition_state.Value));
                    }
                    // add formiosection_id Filter
                    if (requestModel.code_compliance != null && requestModel.code_compliance.Count > 0)
                    {
                        query = query.Where(x => requestModel.code_compliance.Contains(x.code_compliance.Value));
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
                    building_hierarchy_list = query
                        .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOBuildings)
                        .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOFloors)
                        .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIORooms)
                        .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOSections)
                        .Select(x => x.AssetFormIOBuildingMappings).ToList();

                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }

                    assets.listsize = query.Count();
                    query = query.OrderBy(x => x.name).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).OrderBy(x => x.name);
                    //assets.list = query.Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Issues).Include(x => x.Inspection).ThenInclude(x => x.StatusMaster).ToList();


                    assets.list = query.Include(x => x.Sites)
                                       .Include(y => y.Sites.Company)
                                       .Include(x => x.AssetIssue)
                                       .Include(x => x.StatusMaster)
                                       .Include(x => x.AssetIssue)
                                       .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                                       .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                                       .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                                       .Include(x => x.AssetFormIOBuildingMappings.FormIOSections)
                                       .Include(x => x.AssetFormIOBuildingMappings.FormIOSections.FormIOLocationNotes)
                                       .Include(x => x.AssetProfileImages)
                                       .Include(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.FormIOType)
                                       //.Include(x => x.Issues)
                                       ///.Include(x => x.Inspection)
                                       //.ThenInclude(x => x.StatusMaster)
                                       .ToList();

                    if (assets.list != null && assets.list.Count > 0)
                    {
                        var asset_ids = assets.list.Select(x => x.asset_id);
                        //var inspections = context.Inspection.Where(x => asset_ids.Contains(x.asset_id)).Include(x => x.StatusMaster).ToList();
                        var issues = context.Issue.Where(x => asset_ids.Contains(x.asset_id)).ToList();
                        assets.list.ForEach(x =>
                        {
                            //  x.Inspection = inspections.Where(q => q.asset_id == x.asset_id).ToList();
                            x.Issues = issues.Where(q => q.asset_id == x.asset_id).ToList();
                        });
                    }
                    assets.pageIndex = requestModel.pageindex;
                    assets.pageSize = requestModel.pagesize;
                }
            }
            return (assets, building_hierarchy_list);
        }

        public List<Asset> FilterAssetNameOptions(FilterAssetsOptionsRequestModel requestModel)
        {
            List<Asset> assets = new List<Asset>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
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
                        var searchstring = requestModel.search_string.ToLower().ToString();
                        query = query.Where(x => (x.name.ToLower().Contains(searchstring) || x.asset_type.ToLower().Contains(searchstring) || x.Sites.Company.company_name.ToLower().Contains(searchstring) ||
                        x.Sites.site_name.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring)
                        || x.internal_asset_id.ToLower().Contains(searchstring) || x.model_year.Contains(searchstring)));
                    }
                    // only show if issues open
                    if (requestModel.show_open_issues == 1)
                    {
                        query = query.Include(x => x.Issues).Where(x => x.Issues.Where(x => x.status == (int)Status.New || x.status == (int)Status.InProgress || x.status == (int)Status.Waiting).Count() > 0);
                    }
                    // search option string
                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        var searchstring = requestModel.option_search_string.ToLower().ToString();
                        query = query.Where(x => (x.name.ToLower().Contains(searchstring) ||
                        x.internal_asset_id.ToLower().Contains(searchstring)));
                    }

                    //if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    //{
                    //    requestModel.pagesize = 20;
                    //    requestModel.pageindex = 1;
                    //}
                    //assets = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).OrderBy(x => x.name).ToList();
                    assets = query.OrderBy(x => x.name).ToList();
                }
            }
            return assets;
        }

        public List<string> FilterAssetModelOptions(FilterAssetsOptionsRequestModel requestModel)
        {
            List<string> assets = new List<string>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
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
                        var searchstring = requestModel.search_string.ToLower().ToString();
                        query = query.Where(x => (x.name.ToLower().Contains(searchstring) || x.asset_type.ToLower().Contains(searchstring) || x.Sites.Company.company_name.ToLower().Contains(searchstring) ||
                        x.Sites.site_name.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring)
                        || x.internal_asset_id.ToLower().Contains(searchstring) || x.model_year.Contains(searchstring)));
                    }
                    // only show if issues open
                    if (requestModel.show_open_issues == 1)
                    {
                        query = query.Include(x => x.Issues).Where(x => x.Issues.Where(x => x.status == (int)Status.New || x.status == (int)Status.InProgress || x.status == (int)Status.Waiting).Count() > 0);
                    }
                    // search option string
                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        var searchstring = requestModel.option_search_string.ToLower().ToString();
                        query = query.Where(x => x.model_name.ToLower().Contains(searchstring));
                    }
                    assets = query.Where(x => !string.IsNullOrEmpty(x.model_name)).Select(x => x.model_name).Distinct().OrderBy(x => x).ToList();
                }
            }
            return assets;
        }

        public List<string> FilterAssetModelYearOptions(FilterAssetsOptionsRequestModel requestModel)
        {
            List<string> assets = new List<string>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
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
                        var searchstring = requestModel.search_string.ToLower().ToString();
                        query = query.Where(x => (x.name.ToLower().Contains(searchstring) || x.asset_type.ToLower().Contains(searchstring) || x.Sites.Company.company_name.ToLower().Contains(searchstring) ||
                        x.Sites.site_name.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring)
                        || x.internal_asset_id.ToLower().Contains(searchstring) || x.model_year.Contains(searchstring)));
                    }
                    // only show if issues open
                    if (requestModel.show_open_issues == 1)
                    {
                        query = query.Include(x => x.Issues).Where(x => x.Issues.Where(x => x.status == (int)Status.New || x.status == (int)Status.InProgress || x.status == (int)Status.Waiting).Count() > 0);
                    }
                    // search option string
                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        var searchstring = requestModel.option_search_string.ToLower().ToString();
                        query = query.Where(x => x.model_year.ToLower().Contains(searchstring));
                    }
                    assets = query.Where(x => !string.IsNullOrEmpty(x.model_year)).Select(x => x.model_year).Distinct().OrderBy(x => x).ToList();
                }
            }
            return assets;
        }

        public List<int> FilterAssetStatusOptions(FilterAssetsOptionsRequestModel requestModel)
        {
            List<int> assets = new List<int>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
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
                        var searchstring = requestModel.search_string.ToLower().ToString();
                        query = query.Where(x => (x.name.ToLower().Contains(searchstring) || x.asset_type.ToLower().Contains(searchstring) || x.Sites.Company.company_name.ToLower().Contains(searchstring) ||
                        x.Sites.site_name.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring)
                        || x.internal_asset_id.ToLower().Contains(searchstring) || x.model_year.Contains(searchstring)));
                    }
                    // only show if issues open
                    if (requestModel.show_open_issues == 1)
                    {
                        query = query.Include(x => x.Issues).Where(x => x.Issues.Where(x => x.status == (int)Status.New || x.status == (int)Status.InProgress || x.status == (int)Status.Waiting).Count() > 0);
                    }
                    // search option string
                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        var searchstring = requestModel.option_search_string.ToLower().ToString();
                        query = query.Where(x => (x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring)));
                    }
                    assets = query.Where(x => x.status != null).Select(x => x.status.Value).Distinct().ToList();
                }
            }
            return assets;
        }

        public List<Sites> FilterAssetSitesOptions(FilterAssetsOptionsRequestModel requestModel)
        {
            List<Sites> assets = new List<Sites>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
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
                        var searchstring = requestModel.search_string.ToLower().ToString();
                        query = query.Where(x => (x.name.ToLower().Contains(searchstring) || x.asset_type.ToLower().Contains(searchstring) || x.Sites.Company.company_name.ToLower().Contains(searchstring) ||
                        x.Sites.site_name.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring)
                        || x.internal_asset_id.ToLower().Contains(searchstring) || x.model_year.Contains(searchstring)));
                    }

                    // only show if issues open
                    if (requestModel.show_open_issues == 1)
                    {
                        query = query.Include(x => x.Issues).Where(x => x.Issues.Where(x => x.status == (int)Status.New || x.status == (int)Status.InProgress || x.status == (int)Status.Waiting).Count() > 0);
                    }
                    // search option string
                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        var searchstring = requestModel.option_search_string.ToLower().ToString();
                        query = query.Include(x => x.Sites).Where(x => x.Sites.site_name.ToLower().Contains(searchstring));
                    }
                    assets = query.Where(x => x.site_id != null).Select(x => x.Sites).Distinct().ToList();
                }
            }
            return assets;
        }

        public List<Company> FilterAssetCompanyOptions(FilterAssetsOptionsRequestModel requestModel)
        {
            List<Company> assets = new List<Company>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
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
                        var searchstring = requestModel.search_string.ToLower().ToString();
                        query = query.Where(x => (x.name.ToLower().Contains(searchstring) || x.asset_type.ToLower().Contains(searchstring) || x.Sites.Company.company_name.ToLower().Contains(searchstring) ||
                        x.Sites.site_name.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring)
                        || x.internal_asset_id.ToLower().Contains(searchstring) || x.model_year.Contains(searchstring)));
                    }


                    // only show if issues open
                    if (requestModel.show_open_issues == 1)
                    {
                        query = query.Include(x => x.Issues).Where(x => x.Issues.Where(x => x.status == (int)Status.New || x.status == (int)Status.InProgress || x.status == (int)Status.Waiting).Count() > 0);
                    }

                    // search option string
                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        var searchstring = requestModel.option_search_string.ToLower().ToString();
                        query = query.Where(x => (x.Sites.Company.company_name.ToLower().Contains(searchstring)));
                    }

                    assets = query.Where(x => x.site_id != null).Select(x => x.Sites.Company).Distinct().ToList();
                }
            }
            return assets;
        }

        public Asset GetAsset(int pagesize, int pageindex, GetAssetsByIdRequestModel requestModel)
        {
            Asset asset = new Asset();
            if (requestModel.barcode_id != null && requestModel.barcode_id != string.Empty)
            {
                asset = context.Assets.Include(x => x.InspectionTemplateAssetClass).Include(x => x.InspectionForms).Include(x => x.Sites).Include(y => y.Sites.Company).Include(z => z.Inspection).Include(x => x.Issues).Include(x => x.StatusMaster).Where(x => x.asset_barcode_id == requestModel.barcode_id).FirstOrDefaultAsync().Result;
                //if (pageindex != 0 && pagesize != 0)
                //{
                //    asset.Inspection = asset.Inspection.OrderByDescending(x => x.created_at).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                //}
            }
            else if (requestModel.internal_asset_id != null && requestModel.internal_asset_id != string.Empty)
            {
                asset = context.Assets.Include(x => x.StatusMaster).Include(x => x.InspectionForms).Include(x => x.Sites).Include(y => y.Sites.Company).Include(z => z.Inspection).Include(x => x.Issues).Include(x => x.InspectionTemplateAssetClass)
                    .Where(x => x.internal_asset_id == requestModel.internal_asset_id).FirstOrDefaultAsync().Result;
                //if (pageindex != 0 && pagesize != 0)
                //{

                //    asset.Inspection = asset.Inspection.OrderByDescending(x => x.created_at).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                //}
            }
            else if (requestModel.asset_id != null && requestModel.asset_id != string.Empty)
            {
                asset = context.Assets.Include(x => x.StatusMaster).Include(x => x.InspectionForms).Include(x => x.Sites).ThenInclude(x => x.ClientCompany).Include(y => y.Sites.Company).Include(z => z.Inspection).Include(x => x.Issues)
                    .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                    .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                    .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                    .Include(x => x.AssetFormIOBuildingMappings.FormIOSections.FormIOLocationNotes)
                    .Include(x => x.AssetProfileImages)
                    .Include(x => x.AssetIRWOImagesLabelMapping)
                    .Include(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.FormIOType)
                    .Include(x => x.AssetParentHierarchyMapping)
                    .Include(x => x.AssetReplacementMapping)
                    .Include(x => x.AssetIssue)
                    .Include(x => x.AssetPMs)
                    .Include(x => x.AssetTopLevelcomponentMapping)
                    .Include(x => x.AssetSubLevelcomponentMapping)
                    .Where(x => x.asset_id.ToString() == requestModel.asset_id).FirstOrDefaultAsync().Result;
            }
            else if (requestModel.qr_code != null && requestModel.qr_code != string.Empty)
            {
                asset = context.Assets.Include(x => x.StatusMaster).Include(x => x.InspectionForms).Include(x => x.Sites).ThenInclude(x => x.ClientCompany).Include(y => y.Sites.Company).Include(z => z.Inspection).Include(x => x.Issues)
                    .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                    .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                    .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                    .Include(x => x.AssetFormIOBuildingMappings.FormIOSections.FormIOLocationNotes)
                    .Include(x => x.AssetProfileImages)
                    .Include(x => x.AssetIRWOImagesLabelMapping)
                    .Include(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.FormIOType)
                    .Include(x => x.AssetParentHierarchyMapping)
                    .Include(x => x.AssetIssue)
                    .Include(x => x.AssetPMs)
                    .Include(x => x.AssetTopLevelcomponentMapping)
                    .Include(x => x.AssetSubLevelcomponentMapping)
                    .Where(x => x.QR_code == requestModel.qr_code && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefaultAsync().Result;
            }
            return asset;
        }

        public List<Inspection> GetAllInspections(string userid, int pagesize = 0, int pageindex = 0)
        {
            List<Inspection> inspections = new List<Inspection>();
            if (userid != null)
            {
                string rolename = string.Empty;
                //var role = context.User.Include(x => x.Role).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
                {
                    rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == x.user_id).Select(x => x.Role.name).FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(rolename))
                {
                    if (rolename == GlobalConstants.Admin)
                    {
                        inspections = context.Inspection.Include(z => z.Asset).Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Issues).ToListAsync().Result;
                    }
                    else
                    {
                        var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                        {
                            inspections = context.Inspection.Include(z => z.Asset).Where(x => usersites.Contains(x.Asset.site_id)).Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Issues).ToListAsync().Result;
                        }
                        else
                        {
                            inspections = context.Inspection.Include(z => z.Asset).Where(x => usersites.Contains(x.Asset.site_id) && x.Asset.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Issues).ToListAsync().Result;
                        }

                    }
                }


            }

            return inspections.OrderByDescending(x => x.datetime_requested).ToList();
        }

        public async Task<List<Inspection>> GetAllCheckedOutAssets(string userid, int pagesize, int pageindex)
        {
            //if(userid != null)
            //{
            //    var userrole = context.User.Include(x => x.Role).Where(y => y.uuid.ToString() == userid).FirstOrDefault().Role.name;
            //    if(userrole == "Manager")
            //    {
            //        var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid).Select(x => x.site_id).ToList();
            //        return context.Assets.Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.Inspection).Where(x => usersites.Contains(x.site_id) && ).ToListAsync().Result; 
            //    }
            //}


            List<Inspection> responseassets = new List<Inspection>();
            if (userid != null)
            {
                List<Guid> usersites = new List<Guid>();
                if (String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
                {
                    usersites = await context.User.Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).Select(x => x.ac_active_site.Value).ToListAsync();
                }
                else
                {
                    if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                    {
                        usersites = await context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToListAsync();
                    }
                    else
                    {
                        usersites = await context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToListAsync();
                    }
                }
                //if (pageindex > 0 && pagesize > 0)
                //{
                //    responseassets = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset)
                //        .Where(x => usersites.Contains(x.site_id) && (x.status == (int)Status.Approved)
                //        && x.modified_at.Date == DateTime.UtcNow.Date).OrderByDescending(x => x.modified_at).Skip((pageindex - 1) * pagesize).Take(pagesize).ToListAsync();
                //}
                //else
                //{


                //responseassets = await context.Inspection.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.Asset)
                //    .Where(x => usersites.Contains(x.site_id) && (x.status == (int)Status.Approved)
                //    && x.modified_at.Date == DateTime.UtcNow.Date).OrderByDescending(x => x.modified_at).ToListAsync();

                responseassets = await context.Inspection
                                    .Where(x => usersites.Contains(x.Asset.site_id) && (x.status == (int)Status.Approved))
                                    .Include(x => x.Sites).Include(x => x.Sites.Company)
                                    .Include(x => x.Asset).Include(x => x.User)
                                    .OrderByDescending(x => x.modified_at).ToListAsync();
                //}


                //var inspection = context.Inspection.Where(x => usersites.Contains(x.Asset.site_id)).Select(x => x.asset_id).FirstOrDefault();
                //if (inspection != null && inspection != Guid.Empty)
                //{
                //    var assets = await context.Assets.Include(x => x.Sites).Include(x => x.Sites.Company)
                //        .Include(x => x.Inspection)
                //        .Where(x => usersites.Contains(x.site_id)).ToListAsync();
                //    var assetList = assets.ToList();

                //    var sortedList = assets.OrderByDescending(x =>
                //                            {
                //                                x.Inspection = x.Inspection.OrderByDescending(y => y.created_at).ToList();
                //                                return x.created_at;
                //                            }).ToList();

                //    var list = (from asset in sortedList
                //                from inspec in asset.Inspection
                //                where inspec.status == (int)Status.Approved
                //                orderby inspec.created_at descending
                //                select asset).Skip((pageindex - 1) * pagesize).Take(pagesize);

                //    responseassets = list.ToList();
                //}
            }
            return responseassets;
        }

        public List<Inspection> GetPendingIspectionByManager(string userid)
        {
            List<Inspection> inspections = new List<Inspection>();
            if (userid != null && !string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
            {
                string userrole = context.UserRoles.Where(y => y.user_id.ToString() == userid && y.status == (int)Status.Active && y.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();

                if (userrole == GlobalConstants.Manager)
                {
                    var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid).Select(x => x.site_id).ToList();
                    inspections = context.Inspection.Include(x => x.Sites).Include(y => y.Sites.Company).Include(z => z.Asset).Where(x => usersites.Contains(x.site_id) && x.status == 8).ToListAsync().Result;
                }
            }
            return inspections;
        }

        public bool AddAssetTransactionHistory(AssetTransactionHistory assetTxnHistory)
        {
            bool isSuccess = false;
            var result = context.AssetTransactionHistory.Add(assetTxnHistory);
            if (result != null)
            {
                isSuccess = true;
            }
            return isSuccess;
        }


        public Asset GetAssetByAssetID(string asset_id)
        {
            return context.Assets.Where(x => x.asset_id.ToString() == asset_id)
                .Include(x => x.Sites).ThenInclude(x => x.Company)
                .Include(x => x.StatusMaster)
                .Include(x => x.AssetPMs).ThenInclude(x => x.AssetPMsTriggerConditionMapping)
                .Include(x => x.AssetIssue)
                .Include(x => x.AssetFormIO).ThenInclude(x => x.WOcategorytoTaskMapping.WOInspectionsTemplateFormIOAssignment)
                .Include(x => x.AssetFormIOBuildingMappings)
                .Include(x => x.AssetParentHierarchyMapping)
                .Include(x => x.AssetTopLevelcomponentMapping)
                .Include(x => x.AssetSubLevelcomponentMapping)
                .Include(x => x.InspectionTemplateAssetClass)
                .FirstOrDefault();
        }
        public Asset GetduplicateAssetbyQR(string qr_code, Guid asset_id)
        {

            return context.Assets.Where(x => x.asset_id != asset_id && qr_code == x.QR_code && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.status == (int)Status.AssetActive)
                .FirstOrDefault();
        }
        public Asset GetduplicateAssetbyQR_CreateAsset(string qr_code)
        {

            return context.Assets.Where(x => qr_code == x.QR_code && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.status == (int)Status.AssetActive)
                .FirstOrDefault();
        }

        public Asset GetAssetByID(string userid, string asset_id)
        {
            //List<Asset> assets = new List<Asset>();
            Asset asset = new Asset();
            if (userid != null && !string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
            {
                string role = context.UserRoles.Where(x => x.user_id.ToString() == userid && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active).Select(x => x.Role.name).FirstOrDefault();
                if (role == GlobalConstants.Admin)
                {
                    asset = context.Assets
                        .Where(x => x.asset_id.ToString() == asset_id)
                        .Include(x => x.Sites).Include(y => y.Sites.Company)
                        .Include(x => x.Inspection).FirstOrDefault();
                }
                else if (!string.IsNullOrEmpty(role))
                {
                    var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                    {
                        asset = context.Assets
                        .Where(x => usersites.Contains(x.site_id) && x.asset_id.ToString() == asset_id && x.status == (int)Status.AssetActive)
                        .Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster)
                        .Include(x => x.Inspection).FirstOrDefault();
                    }
                    else
                    {
                        asset = context.Assets
                        .Where(x => usersites.Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id && x.asset_id.ToString() == asset_id && x.status == (int)Status.AssetActive)
                        .Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster)
                        .Include(x => x.Inspection).FirstOrDefault();
                    }
                }
            }
            return asset;
        }
        public Asset GetAssetByIDWithoutStatusCheck(string userid, string asset_id)
        {
            //List<Asset> assets = new List<Asset>();
            Asset asset = new Asset();
            if (userid != null)
            {
                string role = context.UserRoles.Where(x => x.user_id.ToString() == userid && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active).Select(x => x.Role.name).FirstOrDefault();
                //var role = context.User.Include(x => x.Role).Where(x => x.uuid.ToString() == userid).FirstOrDefault().Role.name;
                if (role == GlobalConstants.Admin)
                {
                    asset = context.Assets
                        .Where(x => x.asset_id.ToString() == asset_id)
                        .Include(x => x.Sites).Include(y => y.Sites.Company)
                        .Include(x => x.Inspection).FirstOrDefault();
                }
                else
                {
                    var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                    {
                        asset = context.Assets
                        .Where(x => usersites.Contains(x.site_id) && x.asset_id.ToString() == asset_id)
                        .Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster)
                        .Include(x => x.Inspection).FirstOrDefault();
                    }
                    else
                    {
                        asset = context.Assets
                       .Where(x => usersites.Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id && x.asset_id.ToString() == asset_id)
                       .Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster)
                       .Include(x => x.Inspection).FirstOrDefault();
                    }
                }
            }
            return asset;
        }

        public Asset GetAssetByIDFromInternalCall(string asset_id)
        {
            Asset asset = new Asset();

            asset = context.Assets
                .Where(x => x.asset_id.ToString() == asset_id)
                .Include(x => x.Sites).Include(y => y.Sites.Company)
                .Include(x => x.Inspection).FirstOrDefault();
            return asset;
        }
        public Asset GetAssetByQRcode(string QR_code)
        {
            return context.Assets.Where(x => x.QR_code == QR_code && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id))
                 .Include(x => x.Sites)
                 .FirstOrDefault();
        }
        public Asset GetAssetByInternalID(string userid, string internal_asset_id)
        {
            //List<Asset> assets = new List<Asset>();
            Asset asset = new Asset();

            if (userid != null && !string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
            {
                string role = context.UserRoles.Where(x => x.user_id.ToString() == userid && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active).Select(x => x.Role.name).FirstOrDefault();
                if (role == GlobalConstants.Executive)
                {
                    var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (usersites.Count() > 0)
                    {
                        if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                        {
                            asset = context.Assets
                            .Where(x => usersites.Contains(x.site_id) && x.internal_asset_id.ToLower() == internal_asset_id.ToLower() && x.status == (int)Status.AssetActive)
                            .Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster)
                            .Include(x => x.Inspection).ThenInclude(x => x.Issues).FirstOrDefault();
                        }
                        else
                        {
                            asset = context.Assets
                            .Where(x => usersites.Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id && x.internal_asset_id.ToLower() == internal_asset_id.ToLower() && x.status == (int)Status.AssetActive)
                            .Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster)
                            .Include(x => x.Inspection).ThenInclude(x => x.Issues).FirstOrDefault();
                        }
                    }
                }
                else if (role == GlobalConstants.Admin)
                {
                    asset = context.Assets
                        .Where(x => x.internal_asset_id.ToLower() == internal_asset_id.ToLower() && x.status == (int)Status.AssetActive)
                        .Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster)
                        .Include(x => x.Inspection).ThenInclude(x => x.Issues).FirstOrDefault();
                }
                else if (!string.IsNullOrEmpty(role))
                {
                    var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                    {
                        asset = context.Assets
                            .Where(x => usersites.Contains(x.site_id) && x.internal_asset_id.ToLower() == internal_asset_id.ToLower() && x.status == (int)Status.AssetActive)
                            .Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster)
                            .Include(x => x.Inspection).ThenInclude(x => x.Issues).FirstOrDefault();
                    }
                    else
                    {
                        asset = context.Assets
                            .Where(x => usersites.Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id && x.internal_asset_id.ToLower() == internal_asset_id.ToLower() && x.status == (int)Status.AssetActive)
                            .Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster)
                            .Include(x => x.Inspection).ThenInclude(x => x.Issues).FirstOrDefault();
                    }

                }
            }

            return asset;
        }

        //public BaseViewModel CheckAssetIntoInspectionByAssetID(string barcode_id, string userid)
        //{
        //    BaseViewModel response = new BaseViewModel();
        //    var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid).Select(x => x.site_id).ToList();

        //    var asset = context.Assets.Where(x => usersites.Contains(x.site_id) && x.asset_id.ToString() == barcode_id).FirstOrDefault();

        //    if (asset == null || asset.asset_id == null || asset.asset_id == Guid.Empty)
        //    {
        //        response.success = false;
        //        response.message = "Invalid barcode";
        //    }
        //    else
        //    {
        //        //var assetdetails = context.Assets.Where(x => x.asset_id.ToString() == barcode_id).FirstOrDefault();
        //        if(asset == null || asset.inspectionform_id == null)
        //        {
        //            if(asset == null)
        //            {
        //                response.success = false;
        //                response.message = "Invalid barcode";
        //            }
        //            else
        //            {
        //                response.success = false;
        //                response.message = "This asset have no inspectionform";
        //            }
        //        }
        //        else
        //        {
        //            var inspectionstatus = context.Inspection.Include(x => x.Asset).Where(x => x.asset_id.ToString() == barcode_id).OrderByDescending(x => x.created_at).FirstOrDefault();

        //            if (inspectionstatus != null && inspectionstatus.status == 0 || inspectionstatus.status == (int)Status.Approved || inspectionstatus.status == (int)Status.Rejected && inspectionstatus.status == (int)Status.AssetActive)
        //            {
        //                response.success = true;
        //                var assetstatus = context.Assets.Where(x => x.asset_id.ToString() == barcode_id && usersites.Contains(x.site_id)).OrderByDescending(x => x.created_at).Select(x => x.status).FirstOrDefault();

        //                if (assetstatus == (int)Status.AssetActive)
        //                {
        //                    response.success = true;
        //                }
        //                else
        //                {
        //                    response.success = false;
        //                    response.message = "Asset is not active";
        //                }
        //            }
        //            else if (inspectionstatus != null && inspectionstatus.Asset.status == (int)Status.InMaintenanace)
        //            {
        //                response.success = false;
        //                response.message = "Asset is still in maintenance";
        //            }
        //            else
        //            {
        //                response.success = false;
        //                response.message = "Inspection pending";
        //            }
        //        }

        //    }
        //    return response;
        //}


        public BaseViewModel CheckAssetIntoInspectionByAssetID(string barcode_id, string userid)
        {
            BaseViewModel response = new BaseViewModel();
            Asset asset = new Asset();
            string rolename = string.Empty;
            //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
            if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
            {
                rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
            }
            var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
            if (!string.IsNullOrEmpty(rolename))
            {
                if (rolename != GlobalConstants.Admin)
                {
                    asset = context.Assets.Where(x => usersites.Contains(x.site_id) && x.asset_barcode_id == barcode_id).FirstOrDefault();
                }
                else
                {
                    asset = context.Assets.Where(x => x.asset_barcode_id == barcode_id).FirstOrDefault();
                }
            }


            if (asset == null || asset.asset_barcode_id == null || asset.asset_barcode_id == "")
            {
                response.success = false;
                response.message = "Invalid barcode";
            }
            else
            {
                //var assetdetails = context.Assets.Where(x => x.asset_id.ToString() == barcode_id).FirstOrDefault();
                if (asset == null || asset.inspectionform_id == null)
                {
                    if (asset == null)
                    {
                        response.success = false;
                        response.message = "Invalid barcode";
                    }
                    else
                    {
                        response.success = false;
                        response.message = "This asset have no inspectionform";
                    }
                }
                else
                {
                    var inspectionstatus = context.Inspection
                        .Where(x => x.asset_id.ToString() == asset.asset_id.ToString()).Include(x => x.Asset).OrderByDescending(x => x.created_at).FirstOrDefault();

                    if (inspectionstatus != null && (inspectionstatus.status == 0 || inspectionstatus.status == (int)Status.Approved || inspectionstatus.status == (int)Status.Rejected && inspectionstatus.Asset.status == (int)Status.AssetActive))
                    {
                        response.success = true;
                        var assetstatus = context.Assets.Where(x => x.asset_barcode_id == barcode_id).OrderByDescending(x => x.created_at).Select(x => x.status).FirstOrDefault();

                        if (assetstatus == (int)Status.AssetActive)
                        {
                            response.success = true;
                        }
                        else if (inspectionstatus != null && inspectionstatus.Asset.status == (int)Status.InMaintenanace)
                        {
                            response.success = false;
                            response.message = "Asset is still in maintenance";
                        }
                        else
                        {
                            response.success = false;
                            response.message = "Asset is not active";
                        }
                    }
                    else if (inspectionstatus != null && inspectionstatus.Asset.status == (int)Status.InMaintenanace)
                    {
                        response.success = false;
                        response.message = "Asset is still in maintenance";
                    }
                    else if (inspectionstatus != null && inspectionstatus.status == (int)Status.Pending)
                    {
                        response.success = true;
                    }
                    else
                    {
                        response.success = true;
                        var assetstatus = context.Assets.Where(x => x.asset_barcode_id == barcode_id).OrderByDescending(x => x.created_at).Select(x => x.status).FirstOrDefault();

                        if (assetstatus == (int)Status.AssetActive)
                        {
                            response.success = true;
                        }
                        else
                        {
                            response.success = false;
                            response.message = "Asset is not active";
                        }
                    }
                }

            }
            return response;
        }

        //public BaseViewModel CheckAssetIntoInspectionByInternalAssetId(string internal_asset_id,string userid)
        //{
        //    BaseViewModel response = new BaseViewModel();

        //    var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid).Select(x => x.site_id).ToList();

        //    var asset = context.Assets.Where(x => usersites.Contains(x.site_id) && x.internal_asset_id == internal_asset_id).FirstOrDefault();
        //    if (asset == null || asset.asset_id == null || asset.asset_id == Guid.Empty)
        //    {
        //        response.success = false;
        //        response.message = "Please enter correct Asset ID";
        //    }
        //    else
        //    {
        //        //var assetdetails = context.Assets.Where(x => x.internal_asset_id == internal_asset_id).FirstOrDefault();
        //        if (asset == null || asset.inspectionform_id == null)
        //        {
        //            if (asset == null)
        //            {
        //                response.success = false;
        //                response.message = "Please enter correct Asset ID";
        //            }
        //            else
        //            {
        //                response.success = false;
        //                response.message = "This asset have no inspectionform";
        //            }
        //        }
        //        else
        //        {
        //            var inspectionstatus = context.Inspection.Include(x => x.Asset).Where(x => x.Asset.internal_asset_id == internal_asset_id).OrderByDescending(x => x.created_at).FirstOrDefault();

        //            if (inspectionstatus != null && (inspectionstatus.status == 0 || inspectionstatus.status == (int)Status.Approved || inspectionstatus.status == (int)Status.Rejected && inspectionstatus.Asset.status == (int)Status.AssetActive))
        //            {
        //                response.success = true;

        //                var assetstatus = context.Assets.Where(x => x.internal_asset_id == internal_asset_id && usersites.Contains(x.site_id)).Select(x => x.status).FirstOrDefault();
        //                if (assetstatus != null && assetstatus == (int)Status.AssetActive)
        //                {
        //                    response.success = true;
        //                }
        //                else
        //                {
        //                    response.success = false;
        //                    response.message = "Asset is not active";
        //                }
        //            }
        //            else if (inspectionstatus != null && inspectionstatus.Asset.status == (int)Status.InMaintenanace)
        //            {
        //                response.success = false;
        //                response.message = "Asset is still in maintenance";
        //            }
        //            else
        //            {
        //                response.success = false;
        //                response.message = "Inspection pending";
        //            }
        //        }
        //    }
        //    return response;
        //}


        public BaseViewModel CheckAssetIntoInspectionByInternalAssetId(string internal_asset_id, string userid)
        {
            BaseViewModel response = new BaseViewModel();

            Asset asset = new Asset();
            string rolename = string.Empty;
            //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
            if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
            {
                rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
            }
            var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
            if (!string.IsNullOrEmpty(rolename))
            {
                if (rolename != GlobalConstants.Admin)
                {
                    asset = context.Assets.Where(x => usersites.Contains(x.site_id) && x.internal_asset_id == internal_asset_id).FirstOrDefault();
                }
                else
                {
                    asset = context.Assets.Where(x => x.internal_asset_id == internal_asset_id).FirstOrDefault();
                }
            }
            if (asset == null || asset.asset_id == null || asset.asset_id == Guid.Empty)
            {
                response.success = false;
                response.message = "Please enter correct Asset ID";
            }
            else
            {
                //var assetdetails = context.Assets.Where(x => x.internal_asset_id == internal_asset_id).FirstOrDefault();
                if (asset == null || asset.inspectionform_id == null)
                {
                    if (asset == null)
                    {
                        response.success = false;
                        response.message = "Please enter correct Asset ID";
                    }
                    else
                    {
                        response.success = false;
                        response.message = "This asset have no inspectionform";
                    }
                }
                else
                {
                    var inspectionstatus = context.Inspection.Include(x => x.Asset).Where(x => x.Asset.internal_asset_id == internal_asset_id).OrderByDescending(x => x.created_at).FirstOrDefault();

                    if (inspectionstatus != null && (inspectionstatus.status == 0 || inspectionstatus.status == (int)Status.Approved || inspectionstatus.status == (int)Status.Rejected && inspectionstatus.Asset.status == (int)Status.AssetActive))
                    {
                        response.success = true;

                        var assetstatus = context.Assets.Where(x => x.internal_asset_id == internal_asset_id).Select(x => x.status).FirstOrDefault();
                        if (assetstatus != null && assetstatus == (int)Status.AssetActive)
                        {
                            response.success = true;
                        }
                        else if (inspectionstatus != null && inspectionstatus.Asset.status == (int)Status.InMaintenanace)
                        {
                            response.success = false;
                            response.message = "Asset is still in maintenance";
                        }
                        else
                        {
                            response.success = false;
                            response.message = "Asset is not active";
                        }
                    }
                    else if (inspectionstatus != null && inspectionstatus.Asset.status == (int)Status.InMaintenanace)
                    {
                        response.success = false;
                        response.message = "Asset is still in maintenance";
                    }
                    else if (inspectionstatus != null && inspectionstatus.status == (int)Status.Pending)
                    {
                        response.success = true;
                        //response.success = false;
                        //response.message = "Inspection pending";
                    }
                    else
                    {
                        response.success = true;
                        var assetstatus = context.Assets.Where(x => x.internal_asset_id == internal_asset_id).OrderByDescending(x => x.created_at).Select(x => x.status).FirstOrDefault();

                        if (assetstatus == (int)Status.AssetActive)
                        {
                            response.success = true;
                        }
                        else
                        {
                            response.success = false;
                            response.message = "Asset is not active";
                        }
                    }
                }
            }
            return response;
        }



        //public BaseViewModel CheckAssetIntoInspectionByAssetID(string barcode_id, string userid)
        //{
        //    BaseViewModel response = new BaseViewModel();
        //    var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid).Select(x => x.site_id).ToList();

        //    var asset = context.Assets.Where(x => usersites.Contains(x.site_id) && x.asset_id.ToString() == barcode_id).FirstOrDefault();

        //    if (asset == null || asset.asset_id == null || asset.asset_id == Guid.Empty)
        //    {
        //        response.success = false;
        //        response.message = "Invalid barcode";
        //    }
        //    else
        //    {
        //        //var assetdetails = context.Assets.Where(x => x.asset_id.ToString() == barcode_id).FirstOrDefault();
        //        if(asset == null || asset.inspectionform_id == null)
        //        {
        //            if(asset == null)
        //            {
        //                response.success = false;
        //                response.message = "Invalid barcode";
        //            }
        //            else
        //            {
        //                response.success = false;
        //                response.message = "This asset have no inspectionform";
        //            }
        //        }
        //        else
        //        {
        //            var inspectionstatus = context.Inspection.Include(x => x.Asset).Where(x => x.asset_id.ToString() == barcode_id).OrderByDescending(x => x.created_at).FirstOrDefault();

        //            if (inspectionstatus != null && inspectionstatus.status == 0 || inspectionstatus.status == (int)Status.Approved || inspectionstatus.status == (int)Status.Rejected && inspectionstatus.status == (int)Status.AssetActive)
        //            {
        //                response.success = true;
        //                var assetstatus = context.Assets.Where(x => x.asset_id.ToString() == barcode_id && usersites.Contains(x.site_id)).OrderByDescending(x => x.created_at).Select(x => x.status).FirstOrDefault();

        //                if (assetstatus == (int)Status.AssetActive)
        //                {
        //                    response.success = true;
        //                }
        //                else
        //                {
        //                    response.success = false;
        //                    response.message = "Asset is not active";
        //                }
        //            }
        //            else if (inspectionstatus != null && inspectionstatus.Asset.status == (int)Status.InMaintenanace)
        //            {
        //                response.success = false;
        //                response.message = "Asset is still in maintenance";
        //            }
        //            else
        //            {
        //                response.success = false;
        //                response.message = "Inspection pending";
        //            }
        //        }

        //    }
        //    return response;
        //}

        //public BaseViewModel CheckAssetIntoInspectionByInternalAssetId(string internal_asset_id,string userid)
        //{
        //    BaseViewModel response = new BaseViewModel();

        //    var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid).Select(x => x.site_id).ToList();

        //    var asset = context.Assets.Where(x => usersites.Contains(x.site_id) && x.internal_asset_id == internal_asset_id).FirstOrDefault();
        //    if (asset == null || asset.asset_id == null || asset.asset_id == Guid.Empty)
        //    {
        //        response.success = false;
        //        response.message = "Please enter correct Asset ID";
        //    }
        //    else
        //    {
        //        //var assetdetails = context.Assets.Where(x => x.internal_asset_id == internal_asset_id).FirstOrDefault();
        //        if (asset == null || asset.inspectionform_id == null)
        //        {
        //            if (asset == null)
        //            {
        //                response.success = false;
        //                response.message = "Please enter correct Asset ID";
        //            }
        //            else
        //            {
        //                response.success = false;
        //                response.message = "This asset have no inspectionform";
        //            }
        //        }
        //        else
        //        {
        //            var inspectionstatus = context.Inspection.Include(x => x.Asset).Where(x => x.Asset.internal_asset_id == internal_asset_id).OrderByDescending(x => x.created_at).FirstOrDefault();

        //            if (inspectionstatus != null && (inspectionstatus.status == 0 || inspectionstatus.status == (int)Status.Approved || inspectionstatus.status == (int)Status.Rejected && inspectionstatus.Asset.status == (int)Status.AssetActive))
        //            {
        //                response.success = true;

        //                var assetstatus = context.Assets.Where(x => x.internal_asset_id == internal_asset_id && usersites.Contains(x.site_id)).Select(x => x.status).FirstOrDefault();
        //                if (assetstatus != null && assetstatus == (int)Status.AssetActive)
        //                {
        //                    response.success = true;
        //                }
        //                else
        //                {
        //                    response.success = false;
        //                    response.message = "Asset is not active";
        //                }
        //            }
        //            else if (inspectionstatus != null && inspectionstatus.Asset.status == (int)Status.InMaintenanace)
        //            {
        //                response.success = false;
        //                response.message = "Asset is still in maintenance";
        //            }
        //            else
        //            {
        //                response.success = false;
        //                response.message = "Inspection pending";
        //            }
        //        }
        //    }
        //    return response;
        //}

        public async Task<List<Asset>> SearchAssets(string userid, string searchstrings, int status, int pagesize, int pageindex)
        {
            var searchstring = searchstrings.ToLower().ToString();

            List<Asset> assets = new List<Asset>();

            if (searchstring != null && userid != null && !string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
            {
                string rolename = context.UserRoles.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();

                IQueryable<Asset> query = context.Assets;

                if (status > 0)
                {
                    if (status == (int)Status.Active || status == (int)Status.AssetActive)
                    {
                        query = query.Where(x => x.status == (int)Status.AssetActive);
                    }
                    else if (status == (int)Status.Deactive || status == (int)Status.AssetDeactive)
                    {
                        query = query.Where(x => x.status == (int)Status.AssetDeactive);
                    }
                }

                if (rolename == GlobalConstants.Admin)
                {
                    query = query.Where(x => (x.name.ToLower().Contains(searchstring) ||
                            x.asset_type.ToLower().Contains(searchstring) || x.Sites.Company.company_name.ToLower().Contains(searchstring) ||
                            x.Sites.site_name.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring)
                            || x.internal_asset_id.ToLower().Contains(searchstring) || x.model_year.Contains(searchstring)));
                }
                else if (!string.IsNullOrEmpty(rolename))
                {
                    var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (searchstring != String.Empty)
                    {
                        query = query.Where(x => usersites.Contains(x.site_id) && UpdatedGenericRequestmodel.CurrentUser.site_id == x.site_id.ToString() &&
                        (x.name.ToLower().Contains(searchstring) || x.asset_type.ToLower().Contains(searchstring) || x.Sites.Company.company_name.ToLower().Contains(searchstring) ||
                        x.Sites.site_name.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring)
                        || x.internal_asset_id.ToLower().Contains(searchstring) || x.model_year.Contains(searchstring)));
                    }
                    else
                    {
                        query = query.Where(x => usersites.Contains(x.site_id) && UpdatedGenericRequestmodel.CurrentUser.site_id == x.site_id.ToString());
                    }
                }
                assets = query.Include(x => x.Sites).Include(x => x.Sites.Company).Include(x => x.StatusMaster).OrderByDescending(x => x.modified_at).ToList();
            }
            return assets;
        }

        public async Task<List<Asset>> GetAllAssetsWithInspectionForm(string userid, string timestamp)
        {
            List<Asset> assets = new List<Asset>();
            var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
            bool date = false;
            string dateString = timestamp;
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime dateTime = new DateTime();
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
                    assets = await context.Assets
                        .Include(x => x.Inspection)
                            .ThenInclude(x => x.StatusMaster)
                        .Where(x => usersites.Contains(x.site_id) && (x.modified_at >= dateTime || x.Inspection.Any(y => y.modified_at >= dateTime)))
                        .Include(x => x.InspectionForms)
                        .Include(x => x.StatusMaster)
                        .Include(x => x.Sites)
                            .ThenInclude(x => x.Company)
                        .ToListAsync();
                }
                else
                {
                    assets = await context.Assets
                        .Where(x => usersites.Contains(x.site_id))
                        .Include(x => x.Inspection)
                            .ThenInclude(x => x.StatusMaster)
                        .Include(x => x.InspectionForms)
                        .Include(x => x.StatusMaster)
                        .Include(x => x.Sites)
                            .ThenInclude(x => x.Company)
                        .ToListAsync();
                }
                assets.ForEach(x => x.Inspection = x.Inspection.ToList().OrderByDescending(x => x.modified_at).Take(1).ToList());
            }
            return assets;
        }

        public async Task<List<Asset>> GetAssetsWithInspection(string userid)
        {
            List<Asset> assets = new List<Asset>();
            if (userid != null && !string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
            {
                string rolename = context.UserRoles.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();
                if (!string.IsNullOrEmpty(rolename))
                {
                    if (rolename == GlobalConstants.Admin)
                    {
                        assets = await context.Assets.Include(x => x.Inspection).Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.InspectionForms).Where(x => x.inspectionform_id != null && x.inspectionform_id != Guid.Empty).ToListAsync();
                    }
                    else
                    {
                        var usersites = new List<Guid>();
                        if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                        {
                            usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        }
                        else
                        {
                            usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        }
                        assets = await context.Assets
                            .Where(x => usersites.Contains(x.site_id) && (x.inspectionform_id != null && x.inspectionform_id != Guid.Empty))
                            .Include(x => x.Inspection)
                            .Include(x => x.Sites)
                                .ThenInclude(y => y.Company)
                            .Include(x => x.StatusMaster)
                            .Include(x => x.InspectionForms)
                            .ToListAsync();
                    }
                }
            }
            return assets;
        }


        public async Task<List<DashboardOutstandingIssues>> DashboardOutstandingIssues(string userid)
        {
            List<DashboardOutstandingIssues> DashboardOutstandingIssues = new List<DashboardOutstandingIssues>();
            var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

            if (usersites.Count > 0)
            {
                DashboardOutstandingIssues = context.DashboardOutstandingIssues.Where(x => usersites.Contains(x.site_id)).ToList();
            }
            return DashboardOutstandingIssues;
        }

        public async Task<List<Inspection>> GetInspections(string userid)
        {
            List<Inspection> inspections = new List<Inspection>();
            var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

            if (usersites.Count > 0)
            {
                var asset = context.Assets.Where(x => usersites.Contains(x.site_id)).Select(x => x.asset_id).ToList();

                if (asset.Count > 0)
                {
                    inspections = await context.Inspection.Where(x => asset.Contains(x.asset_id)).Include(x => x.Asset).ToListAsync();
                }
            }
            return inspections;
        }

        public GetSyncDataResponseModel GetSyncData(string userid, DateTime? timestamp)
        {
            GetSyncDataResponseModel responseModel = new GetSyncDataResponseModel();

            var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
            //bool date = false;
            //string dateString = timestamp;
            //CultureInfo provider = CultureInfo.InvariantCulture;
            List<User> users = new List<User>();
            //DateTime dateTime = new DateTime();
            //try
            //{
            //    dateTime = DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm:ss", provider);
            //    date = true;
            //}
            //catch
            //{
            //    // do nothing
            //}

            List<Guid> asset_ids = context.Assets.Where(x => usersites.Contains(x.site_id) && x.status == (int)Status.AssetActive).Select(x => x.asset_id).ToList();

            //Get Assets
            IQueryable<Asset> assets_query = context.Assets.Where(x => x.status == (int)Status.AssetActive && usersites.Contains(x.site_id));

            if (timestamp != null)
            {
                assets_query = assets_query.Where(x => x.modified_at >= timestamp);
            }
            //else
            //{
            //    assets_query = assets_query.Where(x => x.status == (int)Status.AssetActive);
            //}

            // Get Inspection Forms
            responseModel.inspectionforms = assets_query.Include(x => x.InspectionForms).ThenInclude(x => x.StatusMaster).Where(x => x.InspectionForms != null).Select(x => x.InspectionForms).Distinct().ToList();

            responseModel.asset_list = assets_query.Count();

            if (timestamp != null)
            {
                responseModel.inspectionforms = responseModel.inspectionforms.Where(x => x != null && x.modified_at >= timestamp).ToList();
            }

            responseModel.inspection_form_list = responseModel.inspectionforms.Count();

            // Get Assets
            responseModel.assets = assets_query.Include(x => x.Sites)
                                .ThenInclude(y => y.Company)
                            .Include(x => x.StatusMaster).ToList();

            // Find Asset Site wise work order
            IQueryable<Issue> workorder_query = context.Issue.Where(x => asset_ids.Contains(x.asset_id));

            // Find All Pending Work orders
            responseModel.issues = workorder_query.Where(x => x.status != (int)Status.Completed).Include(x => x.StatusMaster).ToList();

            // Find n(20) number of completed work orders
            var nnumberofissue = workorder_query.Where(x => x.status == (int)Status.Completed).OrderByDescending(x => x.created_at).Take(20).Include(x => x.StatusMaster).ToList();

            responseModel.issues.AddRange(nnumberofissue);

            // Find Count of work orders
            responseModel.issue_list = responseModel.issues.Count();

            //Find PST UTC Time Diffrence
            //DateTime currentTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Pacific Standard Time");
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles"));
            var diff = currentTime - DateTime.UtcNow;
            var time = DateTime.UtcNow.AddHours(diff.Hours).AddMinutes(diff.Minutes).Date;

            //Find Inspection By Asset Sites
            IQueryable<Inspection> inspections_query = context.Inspection.Where(x => asset_ids.Contains(x.asset_id));

            List<Guid> inspectionids = nnumberofissue.Select(x => x.inspection_id).Distinct().ToList();

            //Find Today's New Inspections
            responseModel.inspections = inspections_query.Where(x => x.created_at.Date >= time.Date
            || (x.Issues != null && x.Issues.Any(x => x.status != (int)Status.Completed))
            || inspectionids.Contains(x.inspection_id)).Include(x => x.StatusMaster).Include(x => x.User).ToList();

            //Find n(20) number of approved Inspections
            responseModel.inspections.AddRange(inspections_query.Where(x => x.status == (int)Status.Approved).Take(20).Include(x => x.StatusMaster).Include(x => x.User).ToList());

            responseModel.inspections = responseModel.inspections.Distinct().ToList();

            // Find Count of Inspections
            responseModel.inspections_list = responseModel.inspections.Count();

            //Find Operator And MS users
            List<Guid> userlist = context.UserSites.Include(x => x.User).Include(x => x.StatusMaster)
                                    .Where(x => x.status == (int)Status.Active && usersites.Contains(x.site_id)).Select(x => x.user_id).Distinct().ToList();

            List<Guid> userRoleList = context.UserRoles.Include(x => x.User).Include(x => x.StatusMaster).Include(x => x.Role)
                                    .Where(x => x.status == (int)Status.Active && (x.Role.name == GlobalConstants.MS || x.Role.name == GlobalConstants.Operator)).Select(x => x.user_id).Distinct().ToList();


            if (userlist.Count > 0 && timestamp != null)
            {
                users = context.User.Include(x => x.Userroles).Include(x => x.Usersites).ThenInclude(x => x.Sites).ThenInclude(x => x.Company).Include(x => x.StatusMaster).Include(x => x.LanguageMaster).Include(x => x.Role_App)
                        .Where(x => (x.modified_at >= timestamp || x.Usersites.Any(y => y.modified_at >= timestamp || y.modified_at.ToString() == null))
                        && userlist.Contains(x.uuid) && userRoleList.Contains(x.uuid)).ToList();
            }
            else if (userlist.Count > 0)
            {
                users = context.User.Include(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites).ThenInclude(x => x.Company).Include(x => x.StatusMaster).Include(x => x.LanguageMaster).Include(x => x.Role_App)
                        .Where(x => userlist.Contains(x.uuid) && userRoleList.Contains(x.uuid)).ToList();
            }

            users.ForEach(x => x.Usersites = x.Usersites.Where(y => y.status == (int)Status.Active).ToList());

            responseModel.user_list = users.Count();

            responseModel.users = users;

            responseModel.active_assets = asset_ids;

            // Get Master Data 
            responseModel.masterdata = context.MasterData.FirstOrDefault();

            return responseModel;
        }

        public AssetInspectionListResponseModel GetAssetInspectionReport(string internal_asset_id, int pagesize, int pageindex)
        {
            AssetInspectionListResponseModel responseModel = new AssetInspectionListResponseModel();

            var usersites = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

            if (usersites.Count > 0)
            {
                IQueryable<AssetInspectionReport> query = null;
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    query = context.AssetInspectionReport.Where(x => usersites.Contains(x.Asset.site_id)).Include(x => x.Asset).OrderByDescending(x => x.created_at);
                }
                else
                {
                    query = context.AssetInspectionReport.Where(x => x.Asset.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Include(x => x.Asset).OrderByDescending(x => x.created_at);
                }

                if (!string.IsNullOrEmpty(internal_asset_id))
                {
                    query = query.Where(x => x.Asset.internal_asset_id == internal_asset_id);
                }

                responseModel.list_size = query.Count();

                if (pagesize > 0 && pageindex > 0)
                {
                    query = query.Skip((pageindex - 1) * pagesize).Take(pagesize);
                }
                responseModel.AssetInspectionReport = query.Include(x => x.Asset).Include(x => x.StatusMaster).ToList();
            }
            return responseModel;
        }

        public List<AssetInspectionReport> GetAssetInspectionReportByID(List<Guid> reports_id)
        {
            return context.AssetInspectionReport.Where(x => reports_id.Contains(x.report_id)).Include(x => x.Asset).Include(x => x.StatusMaster).ToList();
        }

        public AssetInspectionReport GetAssetInspectionReport(Guid asset_id, DateTime from_date, DateTime to_date)
        {
            return context.AssetInspectionReport.Where(x => x.asset_id == asset_id && x.from_date == from_date && x.to_date == to_date && x.status != (int)Status.ReportFailed).FirstOrDefault();
        }


        public List<Inspection> GetAllOperatorInspectionReport(string userid)
        {
            List<Inspection> inspections = new List<Inspection>();
            if (userid != null)
            {
                var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

                inspections = context.Inspection.Include(z => z.Asset).Where(x => usersites.Contains(x.Asset.site_id)).Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.User).ToListAsync().Result;
            }
            return inspections.OrderByDescending(x => x.datetime_requested).ToList();
        }

        public List<User> GetAllOperatorBySiteList(string siteid)
        {
            return context.UserSites.Include(x => x.User).Include(x => x.User.Role).Where(x => x.site_id.ToString() == siteid && x.status == (int)Status.Active && x.User.Role.name == "Operator").Select(x => x.User).ToList();
        }

        public List<Inspection> GetAllOperatorInspectionReportByOperator(string userid)
        {
            List<Inspection> inspections = new List<Inspection>();
            if (userid != null)
            {
                DayOfWeek weekStart = DayOfWeek.Sunday; // or Sunday, or whenever
                DateTime startingDate = DateTime.Today;
                while (startingDate.DayOfWeek != weekStart)
                    startingDate = startingDate.AddDays(-1);

                DateTime start = startingDate.AddDays(-7);
                DateTime end = startingDate.AddDays(-1);
                DateTime startPSTTime = start.ToUniversalTime();
                DateTime endPSTTime = end.ToUniversalTime();
                inspections = context.Inspection.Include(z => z.Asset).Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.User).Where(x => x.operator_id.ToString() == userid && x.created_at >= startPSTTime && x.created_at <= endPSTTime).ToListAsync().Result;
            }
            return inspections.OrderByDescending(x => x.datetime_requested).ToList();
        }

        public Inspection GetInspectionsByINSId(string inspection_id)
        {
            return context.Inspection.Include(z => z.Asset).Where(x => x.inspection_id.ToString() == inspection_id).Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.User).FirstOrDefault();
        }

        public async Task<List<Asset>> GetAssetsWithInspectionForWeeklyEmail(string userid)
        {
            List<Asset> assets = new List<Asset>();
            if (userid != null)
            {
                var usersites = new List<Guid>();
                usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                assets = await context.Assets
                    .Where(x => usersites.Contains(x.site_id) && (x.inspectionform_id != null && x.inspectionform_id != Guid.Empty))
                    .Include(x => x.Inspection)
                    .Include(x => x.Sites)
                        .ThenInclude(y => y.Company)
                    .Include(x => x.StatusMaster)
                    .Include(x => x.InspectionForms)
                    .ToListAsync();
            }
            return assets;
        }

        public List<Asset> GetAssetsBySiteID(Guid siteid)
        {
            List<Asset> asset = new List<Asset>();
            asset = context.Assets.Where(x => x.site_id == siteid).Include(x => x.StatusMaster).Include(x => x.AssetPMs)
                .ThenInclude(x => x.PMTriggers).Include(x => x.AssetPMs).ThenInclude(x => x.ServiceDealers).Include(x => x.AssetPMNotificationConfigurations).ToListAsync().Result;
            return asset;
        }

        public async Task<List<AssetType>> GetAllAssetTypes(string searchstring)
        {
            string rolename = string.Empty;
            if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
            {
                rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
            }
            IQueryable<AssetType> query = context.AssetTypes;
            if (!string.IsNullOrEmpty(rolename))
            {
                if (rolename != GlobalConstants.Admin)
                {
                    query = query.Where(x => (!x.isArchive) && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id));
                }
                else
                {
                    query = query.Where(x => (!x.isArchive));
                }
            }

            if (!String.IsNullOrEmpty(searchstring))
            {
                searchstring = searchstring.ToLower();
                query = query.Where(x => (x.name.ToLower().Contains(searchstring)));
            }
            return query.OrderByDescending(x => x.created_at).ToList();
        }
        public async Task<AssetType> GetAssetTypeById(int id)
        {
            return await context.AssetTypes.Where(u => u.asset_type_id == id).FirstOrDefaultAsync();
        }

        public async Task<List<AssetActivityLogs>> GetActivityLogs(string asset_id, string userid, int filter_type)
        {
            List<AssetActivityLogs> inspections = new List<AssetActivityLogs>();
            var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

            if (usersites.Count > 0)
            {
                // give only active sites records
                //var asset = context.Assets.Where(x => usersites.Contains(x.site_id)).Select(x => x.asset_id).ToList();
                var asset = context.Assets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.status == (int)Status.AssetActive).Select(x => x.asset_id).ToList();
                if (!String.IsNullOrEmpty(asset_id))
                {
                    asset = asset.Where(x => x.ToString() == asset_id).ToList();
                }
                if (asset.Count > 0)
                {
                    if (filter_type == (int)FilterActivityTypes.Inspection)
                    {
                        inspections = await context.AssetActivityLogs.Where(x => asset.Contains(x.asset_id)
                        && (x.activity_type == (int)ActivityTypes.AssetAutoApprove || x.activity_type == (int)ActivityTypes.ManagerAcceptInspection
                        || x.activity_type == (int)ActivityTypes.OperatorRequestForAsset)).Include(x => x.Asset).ThenInclude(x => x.Sites).ToListAsync();
                    }
                    else if (filter_type == (int)FilterActivityTypes.Issue)
                    {
                        inspections = await context.AssetActivityLogs.Where(x => asset.Contains(x.asset_id)
                        && (x.activity_type == (int)ActivityTypes.IssueResolved || x.activity_type == (int)ActivityTypes.NewIssueCreated))
                            .Include(x => x.Asset).ThenInclude(x => x.Sites).ToListAsync();
                    }
                    else if (filter_type == (int)FilterActivityTypes.PM)
                    {
                        inspections = await context.AssetActivityLogs.Where(x => asset.Contains(x.asset_id)
                        && (x.activity_type == (int)ActivityTypes.PMDue || x.activity_type == (int)ActivityTypes.PMOverDue
                        || x.activity_type == (int)ActivityTypes.PMNotificationLog)).Include(x => x.Asset).ThenInclude(x => x.Sites).ToListAsync();
                    }
                    else
                    {
                        inspections = await context.AssetActivityLogs.Where(x => asset.Contains(x.asset_id)).Include(x => x.Asset).ThenInclude(x => x.Sites).ToListAsync();
                    }
                }
            }
            return inspections;
        }

        public bool AddAssetMeterHoursHistory(AssetMeterHourHistory assetMeterHourHistory)
        {
            bool isSuccess = false;
            var result = context.AssetMeterHourHistory.Add(assetMeterHourHistory);
            if (result != null)
            {
                isSuccess = true;
            }
            return isSuccess;
        }

        public ListViewModel<Asset> GetParentAssets(FilterAssetsRequestModel requestModel)
        {
            ListViewModel<Asset> assets = new ListViewModel<Asset>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_name))
                {
                    rolename = UpdatedGenericRequestmodel.CurrentUser.role_name;
                }

                if (!string.IsNullOrEmpty(rolename))
                {
                    IQueryable<Asset> query = context.Assets.Where(x => x.levels == "1" && x.status == (int)Status.AssetActive);

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
                        var searchstring = requestModel.search_string.ToLower().ToString();
                        query = query.Where(x => (x.name.ToLower().Contains(searchstring) || x.asset_type.ToLower().Contains(searchstring) || x.Sites.Company.company_name.ToLower().Contains(searchstring) ||
                        x.Sites.site_name.ToLower().Contains(searchstring) || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring)
                        || x.internal_asset_id.ToLower().Contains(searchstring) || x.model_year.Contains(searchstring)));
                    }

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
                    query = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).OrderBy(x => x.name);
                    assets.list = query.Include(x => x.Sites).Include(y => y.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Issues)
                        .Include(x => x.Inspection).ThenInclude(x => x.StatusMaster).ToList();
                    assets.pageIndex = requestModel.pageindex;
                    assets.pageSize = requestModel.pagesize;
                }
            }
            return assets;
        }

        public ListViewModel<Asset> GetChildAssets(string parent)
        {
            ListViewModel<Asset> assets = new ListViewModel<Asset>();
            IQueryable<Asset> query = context.Assets.Where(x => x.parent == parent);
            assets.list = query.ToList();
            return assets;
        }

        public List<Asset> GetAllRawHierarchyAssets()
        {
            List<Asset> response = new List<Asset>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null)
            {
                IQueryable<Asset> query = context.Assets.Where(x => x.status == (int)Status.AssetActive);
                List<Guid> usersites = new List<Guid>();
                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_name))
                {
                    //rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
                    rolename = UpdatedGenericRequestmodel.CurrentUser.role_name;
                }

                if (rolename != GlobalConstants.Admin)
                {
                    // var usersites = context.UserSites.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

                    if (String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
                    {
                        usersites = context.User.Where(x => x.uuid == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.ac_default_site.Value).ToList();
                    }
                    else if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                    {
                        usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    }
                    else
                    {
                        usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                    }

                    query = query.Where(x => usersites.Contains(x.site_id));
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
                /*if (String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
                {
                    usersites = context.User.Where(x => x.uuid == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.ac_default_site.Value).ToList();
                }
                else if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }*/
                response = query.Where(x => x.company_id == UpdatedGenericRequestmodel.CurrentUser.company_id).OrderBy(x => x.name)
                    .Include(x => x.Sites).ToList();
            }
            return response;
        }
        public List<Asset> GetAllAssetsForCluster(String wo_id)
        {
            // if request is from WO and if wo is completed and if main Asset is created from any temp asset then do not return that main asset
            if (!String.IsNullOrEmpty(wo_id))
            {
                var get_wo_status = context.WorkOrders.Where(x => x.wo_id == Guid.Parse(wo_id)).FirstOrDefault();

                if (get_wo_status.status == (int)Status.Completed)
                {
                    var get_new_created_asset_ids = context.TempAsset.Where(x => x.wo_id == Guid.Parse(wo_id) && x.new_created_asset_id != null).Select(x => x.new_created_asset_id.Value).ToList();
                    return context.Assets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.status == (int)Status.AssetActive
                                  && !get_new_created_asset_ids.Contains(x.asset_id)
                                  //&& x.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent
                                  )
                                 .Include(x => x.AssetParentHierarchyMapping)
                                 .Include(x => x.AssetChildrenHierarchyMapping)
                                 .Include(x => x.AssetSubLevelcomponentMapping).ThenInclude(x => x.Asset)
                                 .Include(x => x.InspectionTemplateAssetClass.FormIOType)
                                 .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                                 .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                                 .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                                 .Include(x => x.AssetFormIOBuildingMappings.FormIOSections)
                                 .ToList();
                }
            }
            return context.Assets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.status == (int)Status.AssetActive
                                  //&& x.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent
                                  )
                                 .Include(x => x.AssetParentHierarchyMapping)
                                 .Include(x => x.AssetChildrenHierarchyMapping)
                                 .Include(x => x.AssetSubLevelcomponentMapping).ThenInclude(x => x.Asset)
                                 .Include(x => x.InspectionTemplateAssetClass.FormIOType)
                                 .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                                 .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                                 .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                                 .Include(x => x.AssetFormIOBuildingMappings.FormIOSections)
                                 .ToList();
        }
        public List<Asset> GetAllRawHierarchyAssetsForOffline(DateTime? sync_time)
        {
            List<Asset> response = new List<Asset>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null)
            {
                IQueryable<Asset> query = context.Assets;
                List<Guid> usersites = new List<Guid>();
                string rolename = string.Empty;
                if (sync_time != null)
                {
                    query = query.Where(x => x.created_at.Value.Date >= sync_time.Value.Date && x.modified_at.Value.Date >= sync_time.Value.Date);

                }
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_name))
                {
                    //rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
                    rolename = UpdatedGenericRequestmodel.CurrentUser.role_name;
                }

                if (rolename != GlobalConstants.Admin)
                {
                    // var usersites = context.UserSites.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

                    if (String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
                    {
                        usersites = context.User.Where(x => x.uuid == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.ac_default_site.Value).ToList();
                    }
                    else if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                    {
                        usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    }
                    else
                    {
                        usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                    }

                    query = query.Where(x => usersites.Contains(x.site_id));
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
                /*if (String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
                {
                    usersites = context.User.Where(x => x.uuid == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.ac_default_site.Value).ToList();
                }
                else if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }*/

                response = query.Where(x => x.company_id == UpdatedGenericRequestmodel.CurrentUser.company_id).Include(x => x.Sites).ToList();
            }
            return response;
        }
        public List<Asset> GetAssetsByInternalIds(List<string> internal_asset_ids)
        {
            return context.Assets.Where(x => internal_asset_ids.Contains(x.internal_asset_id)).ToList();
        }

        public Asset GetAssetByIDForhierarchychange(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id)
                .FirstOrDefault();
        }
        public Asset GetAssetByInternalIDForhierarchychange(string asset_internal_id)
        {
            return context.Assets.Where(x => x.internal_asset_id.ToLower().Trim() == asset_internal_id.ToLower().Trim() && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id))
                .Include(x => x.Sites).FirstOrDefault();
        }

        public List<Asset> GetAssetByIDs(List<Guid> asset_ids)
        {
            return context.Assets.Where(x => asset_ids.Contains(x.asset_id)).ToList();
        }
        public List<Asset> GetAssetByAssetNames(List<string> asset_names)
        {
            return context.Assets.Where(x => asset_names.Contains(x.name.ToLower().Trim()) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
             && x.status == (int)Status.AssetActive).ToList();
        }
        public List<WOOnboardingAssets> GetOBWOAssetByAssetNames(List<string> asset_names, Guid? woonboardingassets_id)
        {
            return context.WOOnboardingAssets.Where(x => asset_names.Contains(x.asset_name.ToLower().Trim()) && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
            && !x.is_deleted && x.woonboardingassets_id != woonboardingassets_id
            ).ToList();
        }
        public List<WOOnboardingAssetsImagesMapping> GetOBAssetImages(List<Guid> asset_OB_images_id)
        {
            return context.WOOnboardingAssetsImagesMapping.Where(x => asset_OB_images_id.Contains(x.woonboardingassetsimagesmapping_id)).ToList();
        }
        public WOOnboardingAssetsImagesMapping GetOBAssetImagebyID(Guid asset_OB_images_id)
        {
            return context.WOOnboardingAssetsImagesMapping.Where(x => x.woonboardingassetsimagesmapping_id == asset_OB_images_id).FirstOrDefault();
        }
        public AssetIssueImagesMapping GetOBAssetIssueImagebyID(Guid asset_issue_image_mapping_id)
        {
            return context.AssetIssueImagesMapping.Where(x => x.asset_issue_image_mapping_id == asset_issue_image_mapping_id).FirstOrDefault();
        }
        public WOOBAssetFedByMapping GetOBAssetFedByID(Guid? wo_ob_asset_fed_by_id)
        {
            return context.WOOBAssetFedByMapping.Where(x => x.wo_ob_asset_fed_by_id == wo_ob_asset_fed_by_id).FirstOrDefault();
        }
        public IRWOImagesLabelMapping GetIRImageLabelMappingByID(Guid? irwoimagelabelmapping_id)
        {
            return context.IRWOImagesLabelMapping.Where(x => x.irwoimagelabelmapping_id == irwoimagelabelmapping_id).FirstOrDefault();
        }
        public List<AssetProfileImages> GetAssetImages(List<Guid> asset_profile_images_id)
        {
            return context.AssetProfileImages.Where(x => asset_profile_images_id.Contains(x.asset_profile_images_id)).ToList();
        }
        public AssetProfileImages GetAssetImagebyID(Guid asset_profile_images_id)
        {
            return context.AssetProfileImages.Where(x => x.asset_profile_images_id == asset_profile_images_id).FirstOrDefault();
        }

        public User Getuserbyid(Guid user_id)
        {
            return context.User.Where(x => x.uuid == user_id).FirstOrDefault();
        }
        public (List<AssetNotes>, int) GetAssetNotes(GetAssetNotesRequestmodel requestModel)
        {
            int total_count = 0;
            IQueryable<AssetNotes> query = context.AssetNotes.Where(x => !x.is_deleted)
                .OrderByDescending(x => x.created_at)
                ;
            if (requestModel.asset_id != null)
            {
                query = query.Where(x => x.asset_id == requestModel.asset_id);
            }
            total_count = query.Count();
            if (requestModel.page_index > 0 && requestModel.page_size > 0)
            {
                query = query.Skip((requestModel.page_index - 1) * requestModel.page_size).Take(requestModel.page_size);
            }

            return (query.ToList(), total_count);
        }

        public (List<AssetAttachmentMapping>, int) GetAssetAttachments(GetAssetAttachmentsRequestmodel requestModel)
        {
            int total_count = 0;
            IQueryable<AssetAttachmentMapping> query = context.AssetAttachmentMapping.Where(x => !x.is_deleted)
                .OrderByDescending(x => x.created_at)
                ;
            if (requestModel.asset_id != null)
            {
                query = query.Where(x => x.asset_id == requestModel.asset_id);
            }
            total_count = query.Count();
            if (requestModel.pageindex > 0 && requestModel.pagesize > 0)
            {
                query = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
            }

            return (query.ToList(), total_count);
        }

        public AssetAttachmentMapping GetAssetAttachmentById(Guid assetatachmentmapping_id)
        {
            return context.AssetAttachmentMapping.Where(x => x.assetatachmentmapping_id == assetatachmentmapping_id).FirstOrDefault();
        }

        public AssetNotes GetAssetnoteByID(Guid asset_notes_id)
        {
            return context.AssetNotes.Where(x => x.asset_notes_id == asset_notes_id).FirstOrDefault();
        }
        public List<FormIOBuildings> GetBuildingsBySite()
        {
            return context.FormIOBuildings.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).ToList();
        }
        public List<FormIOFloors> GetFloorsBySite()
        {

            return context.FormIOFloors.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).ToList();
        }
        public List<FormIORooms> GetRoomsBySite()
        {
            return context.FormIORooms.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).ToList();
        }
        public List<FormIOSections> GetSectionBySite()
        {
            return context.FormIOSections.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).ToList();
        }
        public int FilterAssetHierarchyLevelOptions()
        {
            var level = context.Assets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.status == (int)Status.AssetActive && x.levels != null).OrderByDescending(x => x.levels).Select(x => x.levels).FirstOrDefault();
            if (!String.IsNullOrEmpty(level))
            {
                return int.Parse(level);
            }
            return 0;
        }
        public List<AssetChildrenHierarchyMapping> GetChildByAssetId(Guid asset_id)
        {
            return context.AssetChildrenHierarchyMapping.Where(x => x.asset_id == asset_id && !x.is_deleted).ToList();
        }
        public List<Asset> GetAssetforcecco()
        {
            return context.Assets.Where(x => x.site_id.ToString() == "01c64534-ae74-43c8-8902-ff8386cd48f4")
                                        .Include(x => x.Issues)
                                         .ToList();

        }

        public List<Guid> GetAssetParentsByIDs(List<Guid> asset_parent_ids)
        {
            return context.Assets.Where(x => asset_parent_ids.Contains(x.asset_id) && x.status == (int)Status.AssetActive).Select(x => x.asset_id).ToList();
        }

        public (List<AssetSubLevelcomponentMapping>, int) GetSubcomponentsByAssetId(GetSubcomponentsByAssetIdRequestmodel requestModel)
        {
            int total_count = 0;
            IQueryable<AssetSubLevelcomponentMapping> query = context.AssetSubLevelcomponentMapping.Where(x => !x.is_deleted)
                .OrderByDescending(x => x.created_at)
                ;
            if (requestModel.asset_id != null)
            {
                query = query.Where(x => x.asset_id == requestModel.asset_id);
            }
            total_count = query.Count();
            if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
            {
                query = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
            }

            return (query.ToList(), total_count);
        }

        public Asset GetsubcomonentAssetDetail(Guid sublevelcomponent_asset_id)
        {
            return context.Assets.Where(x => x.asset_id == sublevelcomponent_asset_id)
                 .Include(x => x.InspectionTemplateAssetClass)
                 .Include(x => x.AssetProfileImages)
                 .FirstOrDefault();
        }
        public AssetSubLevelcomponentMapping UpdateCircuitForAssetSubcomponent(Guid asset_sublevelcomponent_mapping_id)
        {
            return context.AssetSubLevelcomponentMapping.Where(x => x.asset_sublevelcomponent_mapping_id == asset_sublevelcomponent_mapping_id).FirstOrDefault();
        }
        public AssetSubLevelcomponentMapping DeleteAssetSubcomponent(Guid asset_sublevelcomponent_mapping_id)
        {
            return context.AssetSubLevelcomponentMapping.Where(x => x.asset_sublevelcomponent_mapping_id == asset_sublevelcomponent_mapping_id).FirstOrDefault();
        }
        public AssetTopLevelcomponentMapping GetToplevelmappingofSubcomponent(Guid asset_id)
        {
            return context.AssetTopLevelcomponentMapping.Where(x => x.asset_id == asset_id && !x.is_deleted).FirstOrDefault();
        }
        public List<Asset> GetSubcomponentAssetsToAddinAsset()
        {
            return context.Assets
                .Include(x => x.AssetTopLevelcomponentMapping)
                .Where(x => x.status == (int)Status.AssetActive
            && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
            && x.component_level_type_id == (int)ComponentLevelTypes.SublevelComponent
            && (x.AssetTopLevelcomponentMapping.Count() == 0 || x.AssetTopLevelcomponentMapping.All(x => x.is_deleted))
            ).ToList();

        }


        public Asset GetAssetByIdForNewSubcomponent(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id)
                .Include(x => x.AssetFormIOBuildingMappings)
                .Include(x => x.AssetTopLevelcomponentMapping)
                .Include(x => x.AssetSubLevelcomponentMapping)
                .FirstOrDefault();
        }

        public List<AssetParentHierarchyMapping> GetParentAssetByAssetId(Guid asset_id)
        {
            return context.AssetParentHierarchyMapping.Where(x => x.asset_id == asset_id && !x.is_deleted)
                .ToList();
        }

        public List<AssetChildrenHierarchyMapping> GetChildrenAssetByAssetId(Guid asset_id)
        {
            return context.AssetChildrenHierarchyMapping.Where(x => x.asset_id == asset_id && !x.is_deleted)
                .ToList();
        }

        public AssetParentHierarchyMapping UpdateAssetFedByCircuit(Guid asset_parent_hierrachy_id)
        {
            return context.AssetParentHierarchyMapping.Where(x => x.asset_parent_hierrachy_id == asset_parent_hierrachy_id).FirstOrDefault();
        }
        public AssetChildrenHierarchyMapping UpdateAssetFeedingCircuit(Guid asset_children_hierrachy_id)
        {
            return context.AssetChildrenHierarchyMapping.Where(x => x.asset_children_hierrachy_id == asset_children_hierrachy_id).FirstOrDefault();
        }
        public AssetSubLevelcomponentMapping GetAssetsSubcomponentbyid(Guid asset_id, Guid subcomponent_asset_id)
        {
            return context.AssetSubLevelcomponentMapping.Where(x => x.asset_id == asset_id && x.sublevelcomponent_asset_id == subcomponent_asset_id && !x.is_deleted).FirstOrDefault();
        }

        public ClusterDiagramPDFSiteMapping GetClusterDiagramPDFSiteMappingBySiteId(Guid siteId)
        {
            return context.ClusterDiagramPDFSiteMapping.Where(x => x.site_id == siteId).FirstOrDefault();
        }

        public (List<Asset>, int) GetAssetsLocationDetails(GetAssetsLocationDetailsRequestModel request)
        {
            IQueryable<Asset> query = context.Assets.Where(x => x.status == (int)Status.AssetActive);

            if (request.site_id != null && request.site_id != Guid.Empty)
            {
                query = query.Where(x => x.site_id == request.site_id);
            }


            int total_count = query.Count();

            if (request.pageindex > 0 && request.pagesize > 0)
            {
                query = query.OrderByDescending(x => x.created_at).Skip((request.pageindex - 1) * request.pagesize).Take(request.pagesize);
            }
            else
            {
                query = query.OrderByDescending(x => x.created_at);
            }

            return (query.Include(x => x.Sites)
                .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOBuildings)
                        .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOFloors)
                        .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIORooms)
                        .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOSections)
                        .Include(x => x.InspectionTemplateAssetClass)
                        .Include(x => x.AssetPMs).ThenInclude(x => x.AssetPMPlans)
                .ToList(), total_count);
        }


        public (List<Asset>, int) GetTopLevelAssetsData(GetTopLevelAssetsRequestModel request)
        {
            IQueryable<Asset> query = context.Assets.Where(x => x.component_level_type_id == 1);

            if (request.site_id != null && request.site_id != Guid.Empty)
            {
                query = query.Where(x => x.site_id == request.site_id);
            }
            // search string
            if (!string.IsNullOrEmpty(request.search_string))
            {

                var searchstring = request.search_string.ToLower().ToString();
                query = query.Where(x => (x.name.ToLower().Contains(searchstring) ||
                x.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name.ToLower().Contains(searchstring) ||
                x.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name.ToLower().Contains(searchstring) ||
                x.AssetFormIOBuildingMappings.FormIORooms.formio_room_name.ToLower().Contains(searchstring) ||
                x.AssetFormIOBuildingMappings.FormIOSections.formio_section_name.ToString().ToLower() == searchstring));
            }

            int total_count = query.Count();

            if (request.pageindex > 0 && request.pagesize > 0)
            {
                query = query.OrderByDescending(x => x.created_at).Skip((request.pageindex - 1) * request.pagesize).Take(request.pagesize);
            }
            else
            {
                query = query.OrderByDescending(x => x.created_at);
            }

            return (query.Include(x => x.Sites)
                .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOBuildings)
                        .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOFloors)
                        .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIORooms)
                        .Include(x => x.AssetFormIOBuildingMappings).ThenInclude(x => x.FormIOSections)
                        .Include(x => x.AssetSubLevelcomponentMapping)
                .ToList(), total_count);
        }

        public string GetAssetNameByAssetId(Guid assetId)
        {
            return context.Assets.Where(x => x.asset_id == assetId).Select(x => x.name).FirstOrDefault();
        }
        public List<AssetParentHierarchyMapping> GetfedbyviaSubcomponent(Guid sublevelcomponent_asset_id)
        {
            return context.AssetParentHierarchyMapping.Where(x => x.via_subcomponent_asset_id == sublevelcomponent_asset_id && !x.is_deleted).ToList();
        }
        public List<AssetChildrenHierarchyMapping> GetfeedingviaSubcomponent(Guid sublevelcomponent_asset_id)
        {
            return context.AssetChildrenHierarchyMapping.Where(x => x.via_subcomponent_asset_id == sublevelcomponent_asset_id && !x.is_deleted).ToList();
        }

        public InspectionsTemplateFormIO InsertFormIOTemplate(Guid form_id)
        {
            return context.InspectionsTemplateFormIO.Where(x => x.form_id == form_id).FirstOrDefault();

        }

        public List<Asset> GetAssetsByListOfAssetIds(List<Guid> asset_id)
        {
            return context.Assets.Where(x => asset_id.Contains(x.asset_id)).Include(x => x.AssetFormIOBuildingMappings).Include(x => x.AssetTopLevelcomponentMapping).Include(x => x.AssetSubLevelcomponentMapping).ToList();
        }


        public FormIOBuildings GetBuildingForDelete(int formiobuilding_id)
        {
            FormIOBuildings building = null;
            bool isAny = context.AssetFormIOBuildingMappings.Where(x => x.formiobuilding_id == formiobuilding_id && x.Asset.status == (int)Status.AssetActive).Any();
            if (!isAny)
            {
                var res = context.FormIOBuildings.Where(x => x.formiobuilding_id == formiobuilding_id).FirstOrDefault();
                building = res;
            }
            return building;
        }

        public FormIOFloors GetFloorForDelete(int formiofloor_id)
        {
            FormIOFloors floor = null;
            bool isAny = context.AssetFormIOBuildingMappings.Where(x => x.formiofloor_id == formiofloor_id && x.Asset.status == (int)Status.AssetActive).Any();
            if (!isAny)
            {
                var res = context.FormIOFloors.Where(x => x.formiofloor_id == formiofloor_id).FirstOrDefault();
                floor = res;
            }
            return floor;
        }

        public FormIORooms GetRoomForDelete(int formioroom_id)
        {
            FormIORooms room = null;
            bool isAny = context.AssetFormIOBuildingMappings.Where(x => x.formioroom_id == formioroom_id && x.Asset.status == (int)Status.AssetActive).Any();
            if (!isAny)
            {
                var res = context.FormIORooms.Where(x => x.formioroom_id == formioroom_id).FirstOrDefault();
                room = res;
            }
            return room;
        }

        public FormIOBuildings GetBuildingById(int formiobuilding_id)
        {
            return context.FormIOBuildings.Where(x => x.formiobuilding_id == formiobuilding_id).FirstOrDefault();
        }

        public FormIOFloors GetFloorById(int formiofloor_id)
        {
            return context.FormIOFloors.Where(x => x.formiofloor_id == formiofloor_id).FirstOrDefault();
        }
        public FormIORooms GetRoomById(int formioroom_id)
        {
            return context.FormIORooms.Where(x => x.formioroom_id == formioroom_id).FirstOrDefault();
        }

        public FormIOSections GetSectionById(int formiosection_id)
        {
            return context.FormIOSections.Where(x => x.formiosection_id == formiosection_id).FirstOrDefault();
        }

        public List<AssetFormIOBuildingMappings> GetAssetsFromLocations(UpdateLocationDetailsRequestModel request)
        {
            IQueryable<AssetFormIOBuildingMappings> query = context.AssetFormIOBuildingMappings;

            if (request.editing_location_flag == (int)AddLocationType.Floor)
            {
                query = query.Where(x => x.formiofloor_id == request.formiofloor_id);
            }

            if (request.editing_location_flag == (int)AddLocationType.Room)
            {
                query = query.Where(x => x.formioroom_id == request.formioroom_id);
            }

            return query.ToList();
        }

        public bool CheckForLocationNameIsExist(UpdateLocationDetailsRequestModel request)
        {
            bool isAny = false;
            if (request.editing_location_flag == (int)AddLocationType.Building)
            {
                isAny = context.FormIOBuildings.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                    && x.formio_building_name.ToLower().Trim() == request.location_name.ToLower().Trim()).Any();
            }

            if (request.editing_location_flag == (int)AddLocationType.Floor)
            {
                isAny = context.FormIOFloors.Where(x => x.formiobuilding_id == request.formiobuilding_id
                    && x.formio_floor_name.ToLower().Trim() == request.location_name.ToLower().Trim()).Any();
            }

            if (request.editing_location_flag == (int)AddLocationType.Room)
            {
                isAny = context.FormIORooms.Where(x => x.formiofloor_id == request.formiofloor_id
                    && x.formio_room_name.ToLower().Trim() == request.location_name.ToLower().Trim()).Any();
            }

            if (request.editing_location_flag == (int)AddLocationType.Section)
            {
                isAny = context.FormIOSections.Where(x => x.formioroom_id == request.formioroom_id
                    && x.formio_section_name.ToLower().Trim() == request.location_name.ToLower().Trim()).Any();
            }

            return isAny;
        }

        public Asset GetSubLevelAssetById(Guid sublevelcomponent_asset_id)
        {
            return context.Assets.Where(x => x.asset_id == sublevelcomponent_asset_id)
                .Include(x => x.AssetFormIOBuildingMappings)
                .Include(x => x.InspectionTemplateAssetClass)
                .FirstOrDefault();
        }

        public (List<WOOnboardingAssets>, int) GetOBWOAssetsOfRequestedAsset(GetOBWOAssetsOfRequestedAssetRequestModel request)
        {
            IQueryable<WOOnboardingAssets> query = context.WOOnboardingAssets.Where(x => !x.is_deleted && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                 && (x.status == (int)Status.Ready_for_review || x.status == (int)Status.Completed || x.status == (int)Status.Submitted)
                 && !x.is_woline_from_other_inspection).Include(x => x.WorkOrders);

            if (request.asset_id != null)
            {
                query = query.Where(x => x.asset_id == request.asset_id && x.status == (int)Status.Completed
                && x.WorkOrders.status == (int)Status.Completed);
            }
            if (request.is_requested_for_not_completed_wos_woline)
            {
                query = query.Where(x => x.WorkOrders.status != (int)Status.Completed);
            }
            if (request.wo_type != null && request.wo_type.Count > 0)
            {
                query = query.Include(x => x.WorkOrders).Where(x => request.wo_type.Contains(x.WorkOrders.wo_type));
            }

            if (request.status != null && request.status.Count > 0)
            {
                query = query.Include(x => x.WorkOrders).Where(x => request.status.Contains(x.status));
            }

            // search string
            if (!string.IsNullOrEmpty(request.search_string))
            {
                var searchstring = request.search_string.ToLower().ToString();
                query = query.Include(x => x.Asset).Where(x => x.Asset != null ? (x.Asset.name.ToLower().Contains(searchstring)) : (x.asset_name.ToLower().Contains(searchstring)));
            }

            // search with qr_code 
            if (!string.IsNullOrEmpty(request.qr_code))
            {
                var searchstring = request.qr_code.ToLower().ToString();
                query = query.Include(x => x.TempAsset).Where(x => x.TempAsset != null ? (x.TempAsset.QR_code.ToLower().Contains(searchstring)) : (x.QR_code.ToLower().Contains(searchstring)));
            }



            int total_count = query.Count();

            if (request.page_index > 0 && request.page_size > 0)
            {
                query = query.OrderByDescending(x => x.created_at).Skip((request.page_index - 1) * request.page_size).Take(request.page_size);
            }
            else
            {
                query = query.OrderByDescending(x => x.created_at);
            }

            return (query
                .Include(x => x.WorkOrders)
                .Include(x => x.Asset)
                .Include(x => x.ActiveAssetPMWOlineMapping)
                .Include(x => x.ActiveAssetPMWOlineMapping).ThenInclude(x => x.AssetPMs)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping).ThenInclude(x => x.TempFormIOBuildings)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping).ThenInclude(x => x.TempFormIOFloors)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping).ThenInclude(x => x.TempFormIORooms)
                .Include(x => x.WOOBAssetTempFormIOBuildingMapping).ThenInclude(x => x.TempFormIOSections)
                .Include(x => x.TempAsset).ThenInclude(x => x.TempFormIOBuildings)
                .Include(x => x.TempAsset).ThenInclude(x => x.TempFormIOFloors)
                .Include(x => x.TempAsset).ThenInclude(x => x.TempFormIORooms)
                .Include(x => x.TempAsset).ThenInclude(x => x.TempFormIOSections)
                .Include(x => x.TempAsset.TempMasterBuilding)
                .Include(x => x.TempAsset.TempMasterFloor)
                .Include(x => x.TempAsset.TempMasterRoom)
                .ToList(), total_count);
        }


        public (List<GetAllWOOBAssetsByAssetIdResponseModel>, int) GetAllWOOBAssetsByAssetId(GetOBWOAssetsOfRequestedAssetRequestModel request)
        {
            IQueryable<WOOnboardingAssets> query = context.WOOnboardingAssets.Include(x => x.WorkOrders)
                .Where(x => x.asset_id == request.asset_id && !x.is_deleted && x.status != (int)Status.Completed
                && !x.is_woline_from_other_inspection && x.WorkOrders.status != (int)Status.Completed);

            // search string
            if (!string.IsNullOrEmpty(request.search_string))
            {
                var searchstring = request.search_string.ToLower().ToString();
                query = query.Where(x => (x.asset_name.ToLower().Contains(searchstring))
                || (x.WorkOrders.manual_wo_number.ToLower().Contains(searchstring)));
            }

            int total_count = query.Count();

            if (request.page_index > 0 && request.page_size > 0)
            {
                query = query.OrderByDescending(x => x.created_at).Skip((request.page_index - 1) * request.page_size).Take(request.page_size);
            }
            else
            {
                query = query.OrderByDescending(x => x.created_at);
            }

            if (request.wo_type != null && request.wo_type.Count > 0)
            {
                query = query.Where(x => request.wo_type.Contains(x.WorkOrders.wo_type));
            }
            if (request.status != null && request.status.Count > 0)
            {
                query = query.Where(x => request.status.Contains(x.status));
            }

            query = query.Include(x => x.ActiveAssetPMWOlineMapping.AssetPMs);

            var response = query
              .Select(x => new GetAllWOOBAssetsByAssetIdResponseModel
              {
                  woonboardingassets_id = x.woonboardingassets_id,
                  asset_name = x.asset_name,
                  component_level_type_id = x.component_level_type_id,
                  condition_index_type = x.condition_index_type,
                  criticality_index_type = x.criticality_index_type,
                  asset_operating_condition_state = x.asset_operating_condition_state,
                  asset_class_code = x.asset_class_code,
                  asset_class_name = x.asset_class_name,
                  inspection_type = x.inspection_type.Value,
                  status = x.status,
                  wo_id = x.wo_id,
                  wo_type = x.WorkOrders.wo_type,
                  manual_wo_number = x.WorkOrders.manual_wo_number,
                  asset_pm_id = x.ActiveAssetPMWOlineMapping != null ? x.ActiveAssetPMWOlineMapping.AssetPMs.asset_pm_id : Guid.Empty,
                  pm_id = x.ActiveAssetPMWOlineMapping != null ? x.ActiveAssetPMWOlineMapping.AssetPMs.pm_id : null,
                  asset_pm_title = x.ActiveAssetPMWOlineMapping != null ? x.ActiveAssetPMWOlineMapping.AssetPMs.title : null,
                  QR_code = x.inspection_type == (int)MWO_inspection_wo_type.PM ? x.QR_code : x.TempAsset.QR_code,
                  pm_inspection_type_id = x.ActiveAssetPMWOlineMapping != null && x.ActiveAssetPMWOlineMapping.AssetPMs.pm_id != null ? context.PMs.Where(y => y.pm_id == x.ActiveAssetPMWOlineMapping.AssetPMs.pm_id).Select(y => y.pm_inspection_type_id).FirstOrDefault() : null,
              })
              .ToList();
            return (response, total_count);
        }

        public List<WorkOrders> GetAllWOBySiteId(Guid site_id)
        {
            return context.WorkOrders.Where(x => x.site_id == site_id && (x.wo_type == (int)Status.Onboarding_WO || x.wo_type == (int)Status.IR_Scan_WO)
                                                && !x.is_archive)

                        .Include(x => x.WOOnboardingAssets)
                        .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOLineBuildingMapping.FormIOBuildings)
                        .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOLineBuildingMapping.FormIOFloors)
                        .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOLineBuildingMapping.FormIORooms)
                        .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOLineBuildingMapping.FormIOSections)
                        .Include(x => x.WOOnboardingAssets).ThenInclude(x => x.WOOBAssetTempFormIOBuildingMapping)
                        .ToList();
        }

        public TempFormIOBuildings GetTempFormIOBuildingByName(string building_name, Guid site_id, Guid wo_id)
        {
            return context.TempFormIOBuildings.Where(x => x.temp_formio_building_name.Trim().ToLower() == building_name.Trim().ToLower()
            && x.wo_id == wo_id && x.site_id == site_id).FirstOrDefault();
        }
        public TempFormIOFloors GetTempFormIFloorByName(string floor_name, Guid site_id, Guid wo_id, Guid temp_buildingId)
        {
            return context.TempFormIOFloors.Where(x => x.temp_formio_floor_name.Trim().ToLower() == floor_name.Trim().ToLower()
            && x.wo_id == wo_id && x.temp_formiobuilding_id == temp_buildingId && x.site_id == site_id).FirstOrDefault();
        }
        public TempFormIORooms GetTempFormIORoomByName(string room_name, Guid site_id, Guid wo_id, Guid temp_floorId)
        {
            return context.TempFormIORooms.Where(x => x.temp_formio_room_name.Trim().ToLower() == room_name.Trim().ToLower()
            && x.wo_id == wo_id && x.temp_formiofloor_id == temp_floorId && x.site_id == site_id).FirstOrDefault();
        }
        public TempFormIOSections GetTempFormIOSectionByName(string section_name, Guid site_id, Guid wo_id, Guid temp_roomId)
        {
            return context.TempFormIOSections.Where(x => x.temp_formio_section_name.Trim().ToLower() == section_name.Trim().ToLower()
            && x.wo_id == wo_id && x.temp_formioroom_id == temp_roomId && x.site_id == site_id).FirstOrDefault();
        }

        public WOOBAssetTempFormIOBuildingMapping GetWOOBAssetTempLocationMapping(Guid woonboardingassets_id)
        {
            return context.WOOBAssetTempFormIOBuildingMappings.Where(x => x.woonboardingassets_id == woonboardingassets_id).FirstOrDefault();
        }

        public List<AssetChildrenHierarchyMapping> GetAssetChildren(Guid asset_id)
        {
            return context.AssetChildrenHierarchyMapping.Where(x => x.asset_id == asset_id && !x.is_deleted).ToList();
        }
        public List<WOOBAssetFedByMapping> GetTempAssetChildren(Guid woonboardingassets_id)
        {
            return context.WOOBAssetFedByMapping.Where(x => x.parent_asset_id == woonboardingassets_id).ToList();
        }
        public Asset GetAssetbyIdforFeedingscircuit(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id).FirstOrDefault();
        }
        public Asset GetAssetDatabyId(string asset_id)
        {
            return context.Assets.Where(x => x.asset_id.ToString() == asset_id).FirstOrDefault();
        }

        public List<Sites> GetAllSitesForScript()
        {
            List<Guid> list = new List<Guid>();
            list.Add(Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d"));  //cecco
            list.Add(Guid.Parse("22a8170a-f97c-432b-976a-28c4d7abf3ca")); //EEE
            //list.Add(Guid.Parse("f1c579ce-1571-47fd-8d4b-6e3e35df3eff")); //Demo Company
            list.Add(Guid.Parse("0b2b3b98-f141-40f1-88b9-fa8de7224c0f")); //stansell

            return context.Sites.Where(x => list.Contains(x.company_id) && x.status == (int)Status.Active).ToList();
        }

        public AssetParentHierarchyMapping GetAssetparentMapping(Guid asset_id, Guid parent_asset_id)
        {
            return context.AssetParentHierarchyMapping.Where(x => x.asset_id == asset_id && x.parent_asset_id == parent_asset_id && !x.is_deleted).FirstOrDefault();
        }

        public List<AssetProfileImages> GetAssetProfileImagesByAssetId(Guid asset_id)
        {
            return context.AssetProfileImages.Where(x => x.asset_id == asset_id && !x.is_deleted).ToList();
        }

        public List<IRWOImagesLabelMapping> GetIRWOImagesByAssetId(Guid asset_id)
        {
            var all_woonboardingassets_ids = context.WOOnboardingAssets.Where(x => x.asset_id == asset_id && !x.is_deleted).Select(x => x.woonboardingassets_id).ToList();

            return context.IRWOImagesLabelMapping.Where(x => all_woonboardingassets_ids.Contains(x.woonboardingassets_id.Value) && !x.is_deleted).ToList();
        }
        public List<AssetIssueImagesMapping> GetAssetIssueImages(Guid asset_id)
        {
            return context.AssetIssueImagesMapping.Where(x => !x.is_deleted && !x.AssetIssue.is_deleted && x.AssetIssue.asset_id == asset_id).ToList();
        }


        public WOOnboardingAssets GetOBWOAssetsByAssetId(Guid asset_id, Guid wo_id)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && x.asset_id == asset_id && !x.is_deleted && x.inspection_type == (int)MWO_inspection_wo_type.OnBoarding).FirstOrDefault();
        }
        public WOOnboardingAssets GetinstallWOlineFromTempAsset(Guid asset_id, Guid wo_id)
        {
            WOOnboardingAssets install_woline = null;
            var temp_asset = context.TempAsset.Where(x => x.asset_id == asset_id && x.wo_id == wo_id && !x.is_deleted)
                .Include(x => x.WOOnboardingAssets)
                .FirstOrDefault();
            if (temp_asset != null)
            {
                install_woline = temp_asset.WOOnboardingAssets.Where(x => x.inspection_type == (int)MWO_inspection_wo_type.OnBoarding).FirstOrDefault();
            }

            return install_woline;

        }

        public List<Asset> GetToplevelFebbyAssetlist(List<Guid> asset_id)
        {
            return context.Assets.Where(x => x.status == (int)Status.AssetActive
            && x.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent
            && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
            && x.company_id == UpdatedGenericRequestmodel.CurrentUser.company_id
            && !asset_id.Contains(x.asset_id)
            )
                .Include(x => x.Sites).OrderBy(x => x.name).ToList();
        }

        public List<Asset> GetTopLevelFedByAssetList()
        {
            List<Asset> response = new List<Asset>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null)
            {
                IQueryable<Asset> query = context.Assets.Where(x => x.status == (int)Status.AssetActive && x.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent);
                List<Guid> usersites = new List<Guid>();
                string rolename = string.Empty;
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_name))
                {
                    //rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
                    rolename = UpdatedGenericRequestmodel.CurrentUser.role_name;
                }

                if (rolename != GlobalConstants.Admin)
                {
                    // var usersites = context.UserSites.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

                    if (String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
                    {
                        usersites = context.User.Where(x => x.uuid == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.ac_default_site.Value).ToList();
                    }
                    else if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                    {
                        usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    }
                    else
                    {
                        usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                    }

                    query = query.Where(x => usersites.Contains(x.site_id));
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

                response = query.Where(x => x.company_id == UpdatedGenericRequestmodel.CurrentUser.company_id).OrderBy(x => x.name)
                    .Include(x => x.Sites).ToList();
            }
            return response;
        }

        public List<WOOnboardingAssets> GetTopLevelFedByWOOBAssetList(GetOBTopLevelFedByAssetListRequestModel requestmodel, List<Guid> child_asset)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == requestmodel.wo_id && x.woonboardingassets_id != requestmodel.woonboardingassets_id
            && !x.is_deleted && x.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent
            && !child_asset.Contains(x.woonboardingassets_id)
            )
                .OrderBy(x => x.asset_name)
                .Include(x => x.StatusMaster)
                .ToList();
        }

        public List<Asset> GetAllTopLevelAssetsList(Guid? wo_id)
        {
            IQueryable<Asset> query = context.Assets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                 && x.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent && x.status == (int)Status.AssetActive);

            if (wo_id == null || wo_id == Guid.Empty)
            {
                return query.ToList();
            }
            else
            {
                var asset_ids = query.Select(x => x.asset_id).ToList();

                var asset_ids_fromWO = context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && x.asset_id != null
                && asset_ids.Contains(x.asset_id.Value) && !x.is_deleted).Select(x => x.asset_id).ToList();

                query = query.Where(x => !asset_ids_fromWO.Contains(x.asset_id));

                return query.ToList();
            }
            /*//return context.Assets.Where(x=> x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) 
            //     && x.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent && x.status == (int)Status.AssetActive).ToList();*/
        }

        public List<Asset> GetAllSubLevelAssetsMappingList(Guid? wo_id)
        {
            IQueryable<Asset> query = context.Assets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                 && x.component_level_type_id == (int)ComponentLevelTypes.SublevelComponent && x.status == (int)Status.AssetActive)
                .Include(x => x.AssetTopLevelcomponentMapping);

            if (wo_id == null || wo_id == Guid.Empty)
            {
                return query.ToList();
            }
            else
            {
                var asset_ids = query.Select(x => x.asset_id).ToList();

                var asset_ids_fromWO = context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && x.asset_id != null
                && asset_ids.Contains(x.asset_id.Value) && !x.is_deleted).Select(x => x.asset_id).ToList();

                query = query.Where(x => !asset_ids_fromWO.Contains(x.asset_id));

                return query.ToList();
            }

            /*return context.Assets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                 && x.component_level_type_id == (int)ComponentLevelTypes.SublevelComponent && x.status == (int)Status.AssetActive)
                .Include(x=>x.AssetTopLevelcomponentMapping).ToList();*/
        }

        public List<WOOnboardingAssets> GetAllTopLevelOBWOAssetsList(Guid wo_id)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == wo_id
                 && x.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent
                 //&& x.asset_id == null
                 && !x.is_deleted).ToList();
        }

        public List<WOOnboardingAssets> GetAllSubLevelOBWOAssetsMappingList(Guid wo_id)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == wo_id
                 && x.component_level_type_id == (int)ComponentLevelTypes.SublevelComponent
                 //&& x.asset_id == null
                 && !x.is_deleted)
                .Include(x => x.WOlineTopLevelcomponentMapping).ToList();
        }
        public List<WOOBAssetFedByMapping> GetWOlinefedbbyMapping(Guid wo_id)
        {
            return context.WOOBAssetFedByMapping.Where(x => x.WOOnboardingAssets.wo_id == wo_id && !x.is_deleted)
                .Include(x => x.WOOnboardingAssets)
                .ThenInclude(x => x.TempAsset)
                .ToList();
        }

        public string GetAssetImageByTypeAndId(Guid asset_id, int type)
        {
            string url = null;
            string img_name = context.AssetProfileImages.Where(x => x.asset_id == asset_id && x.asset_photo_type == type && !x.is_deleted)
                .Select(x => x.asset_photo).FirstOrDefault();

            if (!String.IsNullOrEmpty(img_name))
                url = UrlGenerator.GetAssetImagesURL(img_name);

            return url;
        }

        public List<WOOnboardingAssets> GetallToplevelWolines(Guid wo_id)
        {
            return context.WOOnboardingAssets.Where(x => x.wo_id == wo_id && x.TempAsset.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent
            && x.TempAsset.asset_id == null
            && !x.is_deleted &&
            (x.inspection_type == null || x.inspection_type == (int)MWO_inspection_wo_type.OnBoarding || x.inspection_type == 0)
            ).Include(x => x.TempAsset)
            .Include(x => x.WOlineSubLevelcomponentMapping)
            .Include(x => x.TempAsset).ThenInclude(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.FormIOType)
            .Include(x => x.TempAsset).ThenInclude(x => x.TempFormIOBuildings)
            .Include(x => x.TempAsset).ThenInclude(x => x.TempFormIOFloors)
            .Include(x => x.TempAsset).ThenInclude(x => x.TempFormIORooms)
            .Include(x => x.TempAsset).ThenInclude(x => x.TempFormIOSections)
                .ToList();
        }
        public List<WOOnboardingAssets> GetSublevelWOlines(List<Guid> sublevel_list)
        {
            return context.WOOnboardingAssets.Where(x => sublevel_list.Contains(x.woonboardingassets_id))
                .Include(x => x.TempAsset)
                .ToList();
        }

        public Asset GetAssetByNameClassCode(string asset_name, string class_code)
        {
            return context.Assets.Include(x => x.InspectionTemplateAssetClass).Where(x => x.name == asset_name
                && x.InspectionTemplateAssetClass.asset_class_code == class_code && x.status == (int)Status.AssetActive).FirstOrDefault();
        }

        public List<WOOnboardingAssets> GetExistingAssetWOline(Guid wo_id)
        {
            return context.WOOnboardingAssets.Where(x => (x.TempAsset.asset_id != null || x.TempAsset.new_created_asset_id != null) && x.wo_id == wo_id
            && !x.is_deleted && !x.is_woline_from_other_inspection
            ).Include(x => x.WOOBAssetFedByMapping).ToList();
        }
        public Asset GetAssetById(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id && x.status == (int)Status.AssetActive).FirstOrDefault();
        }
        public AssetProfileImages GetAssetProfileImageById(Guid asset_profile_images_id)
        {
            return context.AssetProfileImages.Where(x => x.asset_profile_images_id == asset_profile_images_id && !x.is_deleted).FirstOrDefault();
        }

        public AssetIssueImagesMapping GetAssetIssueImageById(Guid asset_issue_image_mapping_id)
        {
            return context.AssetIssueImagesMapping.Where(x => x.asset_issue_image_mapping_id == asset_issue_image_mapping_id && !x.is_deleted).FirstOrDefault();
        }
        public AssetIRWOImagesLabelMapping GetIRWOImagesLabelById(Guid assetirwoimageslabelmapping_id)
        {
            return context.AssetIRWOImagesLabelMapping.Where(x => x.assetirwoimageslabelmapping_id == assetirwoimageslabelmapping_id && !x.is_deleted).FirstOrDefault();
        }
        public List<AssetIRWOImagesLabelMapping> GetAssetIRWOImagesByAssetId(Guid asset_id)
        {
            return context.AssetIRWOImagesLabelMapping.Where(x => x.asset_id == asset_id && !x.is_deleted).ToList();
        }

        public AssetParentHierarchyMapping GetParentMapping(Guid child_asset_id, Guid parent_asset_id)
        {
            return context.AssetParentHierarchyMapping.Where(x => x.asset_id == child_asset_id && x.parent_asset_id == parent_asset_id && !x.is_deleted).FirstOrDefault();

        }
        public AssetChildrenHierarchyMapping GetChildMapping(Guid parent_asset_id, Guid child_asset_id)
        {
            return context.AssetChildrenHierarchyMapping.Where(x => x.asset_id == parent_asset_id && x.children_asset_id == child_asset_id && !x.is_deleted).FirstOrDefault();
        }

        public List<AssetChildrenHierarchyMapping> GetAssetChildrenByOCP(Guid sublevelcomponent_asset_id)
        {
            return context.AssetChildrenHierarchyMapping.Where(x => x.via_subcomponent_asset_id == sublevelcomponent_asset_id && !x.is_deleted).ToList();
        }
        public AssetSubLevelcomponentMapping GetSubcomponentMapping(Guid subcomponent_asset_id)
        {
            return context.AssetSubLevelcomponentMapping.Where(x => x.sublevelcomponent_asset_id == subcomponent_asset_id && !x.is_deleted).FirstOrDefault();
        }

        public List<Guid> GetAssetsLinkedSubcomponents(Guid assset_id, Guid asset_children_hierrachy_id)
        {
            return context.AssetChildrenHierarchyMapping.Where(x => x.asset_id == assset_id && !x.is_deleted
            && x.asset_children_hierrachy_id != asset_children_hierrachy_id
            && x.via_subcomponent_asset_id != null && x.via_subcomponent_asset_id != Guid.Empty
            ).Select(x => x.via_subcomponent_asset_id.Value).ToList();
        }

        public List<WOOnboardingAssets> GetNewlyCreatedAssetsbyWO(Guid wo_id)
        {
            return context.WOOnboardingAssets.Where(x => x.TempAsset.new_created_asset_id != null)
                .Include(x => x.TempAsset)
                .ToList();
        }

        public (List<AssetListExclude>, int, List<AssetlocationHierarchyExclude>) FilterAssetOptimized(FilterAssetsRequestModel requestModel)
        {
            List<AssetListExclude> assets = new List<AssetListExclude>();
            IQueryable<Asset> query = context.Assets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.status != (int)Status.Disposed);

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
            // add criticality_index_type Filter
            if (requestModel.criticality_index_type != null && requestModel.criticality_index_type.Count > 0)
            {
                query = query.Where(x => requestModel.criticality_index_type.Contains(x.criticality_index_type.Value));
            }
            // add condition_index_type Filter
            if (requestModel.condition_index_type != null && requestModel.condition_index_type.Count > 0)
            {
                query = query.Where(x => requestModel.condition_index_type.Contains(x.condition_index_type.Value));
            }
            // add inspectiontemplate_asset_class_id Filter
            if (requestModel.inspectiontemplate_asset_class_id != null && requestModel.inspectiontemplate_asset_class_id.Count > 0)
            {
                query = query.Where(x => requestModel.inspectiontemplate_asset_class_id.Contains(x.inspectiontemplate_asset_class_id.Value));
            }
            // add formiobuilding_id Filter
            if (requestModel.formiobuilding_id != null && requestModel.formiobuilding_id.Count > 0)
            {
                query = query.Where(x => requestModel.formiobuilding_id.Contains(x.AssetFormIOBuildingMappings.formiobuilding_id.Value));
            }
            // add formiofloor_id Filter
            if (requestModel.formiofloor_id != null && requestModel.formiofloor_id.Count > 0)
            {
                query = query.Where(x => requestModel.formiofloor_id.Contains(x.AssetFormIOBuildingMappings.formiofloor_id.Value));
            }
            // add formioroom_id Filter
            if (requestModel.formioroom_id != null && requestModel.formioroom_id.Count > 0)
            {
                query = query.Where(x => requestModel.formioroom_id.Contains(x.AssetFormIOBuildingMappings.formioroom_id.Value));
            }
            // add formiosection_id Filter
            if (requestModel.formiosection_id != null && requestModel.formiosection_id.Count > 0)
            {
                query = query.Where(x => requestModel.formiosection_id.Contains(x.AssetFormIOBuildingMappings.formiosection_id.Value));
            }

            // add formiosection_id Filter
            if (requestModel.thermal_classification_id != null && requestModel.thermal_classification_id.Count > 0)
            {
                query = query.Where(x => requestModel.thermal_classification_id.Contains(x.thermal_classification_id.Value));
            }
            // add formiosection_id Filter
            if (requestModel.asset_operating_condition_state != null && requestModel.asset_operating_condition_state.Count > 0)
            {
                query = query.Where(x => requestModel.asset_operating_condition_state.Contains(x.asset_operating_condition_state.Value));
            }
            // add formiosection_id Filter
            if (requestModel.code_compliance != null && requestModel.code_compliance.Count > 0)
            {
                query = query.Where(x => requestModel.code_compliance.Contains(x.code_compliance.Value));
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
                var searchstring = requestModel.search_string.ToLower().ToString();
                query = query.Where(x => (x.name.ToLower().Contains(searchstring)
                || x.asset_type.ToLower().Contains(searchstring)
                //|| x.Sites.Company.company_name.ToLower().Contains(searchstring) || x.Sites.site_name.ToLower().Contains(searchstring) 
                || x.status.ToString().ToLower() == searchstring || x.StatusMaster.status_name.ToLower().Contains(searchstring)
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

            int total_count = query.Count();

            var building_data = query.Where(x => x.AssetFormIOBuildingMappings.formiobuilding_id != null && x.AssetFormIOBuildingMappings.formiofloor_id != null && x.AssetFormIOBuildingMappings.formioroom_id != null)
                 .Select(x => new AssetlocationHierarchyExclude
                 {
                     building_id = x.AssetFormIOBuildingMappings.formiobuilding_id.Value,
                     building_name = x.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name,
                     floor_id = x.AssetFormIOBuildingMappings.formiofloor_id.Value,
                     floor_name = x.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name,
                     room_id = x.AssetFormIOBuildingMappings.formioroom_id.Value,
                     room_name = x.AssetFormIOBuildingMappings.FormIORooms.formio_room_name,
                 })
               .ToList();
            query = query.OrderBy(x => x.name).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).OrderBy(x => x.name);
            var response = query
               .Select(x => new AssetListExclude
               {
                   asset_id = x.asset_id,
                   asset_name = x.name,
                   condition_index_type = x.condition_index_type,
                   condition_index_type_name = ((condition_index_type)x.condition_index_type.Value).ToString(),
                   criticality_index_type = x.criticality_index_type,
                   criticality_index_type_name = ((criticality_index_type)x.criticality_index_type.Value).ToString(),
                   code_compliance = x.code_compliance,
                   asset_operating_condition_state = x.asset_operating_condition_state,
                   inspectiontemplate_asset_class_id = x.inspectiontemplate_asset_class_id,
                   asset_class_code = x.InspectionTemplateAssetClass.asset_class_code,
                   asset_class_name = x.InspectionTemplateAssetClass.asset_class_name,
                   asset_class_type = x.InspectionTemplateAssetClass.FormIOType.form_type_name,
                   room = x.AssetFormIOBuildingMappings.FormIORooms.formio_room_name,
                   status = x.status,
                   status_name = x.StatusMaster.status_name,
                   issue_count = x.AssetIssue.Where(x=>!x.is_deleted).Count(),
                   asset_profile_image = x.asset_profile_image != null ? x.asset_profile_image 
                   : x.AssetProfileImages.Where(y => y.asset_photo_type == 1 && !y.is_deleted).FirstOrDefault() != null
                    ? UrlGenerator.GetAssetImagesURL(x.AssetProfileImages.Where(y => y.asset_photo_type == 1 && !y.is_deleted).Select(x => x.asset_photo).FirstOrDefault()) : null                
               })
               .ToList();


            return (response, total_count, building_data);
        }

        public (List<GetAllBuildingLocationsData>, int) GetAllBuildingLocations(int pagesize, int pageindex)
        {
            IQueryable<FormIOBuildings> query = context.FormIOBuildings.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).OrderBy(x => x.formio_building_name);

            int count = query.Count();

            if (pagesize > 0 && pageindex > 0)
            {
                query = query.OrderBy(x => x.formio_building_name).Skip((pageindex - 1) * pagesize).Take(pagesize);
            }

            var response = query.Select(x => new GetAllBuildingLocationsData
            {
                formio_building_name = x.formio_building_name,
                formiobuilding_id = x.formiobuilding_id,
                floor_count = context.FormIOFloors.Where(y => y.formiobuilding_id == x.formiobuilding_id && y.site_id != Guid.Empty && y.site_id != null).Count()

            }).ToList();

            return (response, count);
        }

        public (List<GetAllFloorsByBuildingData>, int) GetAllFloorsByBuilding(int formiobuilding_id, int pagesize, int pageindex)
        {
            IQueryable<FormIOFloors> query = context.FormIOFloors
                .Where(x => x.formiobuilding_id.Value == formiobuilding_id && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));

            int count = query.Count();

            if (pagesize > 0 && pageindex > 0)
            {
                query = query.OrderByDescending(x => x.created_at).Skip((pageindex - 1) * pagesize).Take(pagesize);
            }

            var response = query.Select(x => new GetAllFloorsByBuildingData
            {
                formio_floor_name = x.formio_floor_name,
                formiofloor_id = x.formiofloor_id,
                formiobuilding_id = x.formiobuilding_id,
                room_count = context.FormIORooms.Where(y => y.formiofloor_id == x.formiofloor_id && y.site_id != Guid.Empty && y.site_id != null).Count()

            }).ToList();

            return (response, count);
        }
        public (List<GetAllRoomsByFloorData>, int) GetAllRoomsByFloor(int formiofloor_id, int pagesize, int pageindex)
        {
            IQueryable<FormIORooms> query = context.FormIORooms
                .Where(x => x.formiofloor_id.Value == formiofloor_id && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));

            int count = query.Count();

            if (pagesize > 0 && pageindex > 0)
            {
                query = query.OrderByDescending(x => x.created_at).Skip((pageindex - 1) * pagesize).Take(pagesize);
            }

            var response = query.Select(x => new GetAllRoomsByFloorData
            {
                formio_room_name = x.formio_room_name,
                formiofloor_id = x.formiofloor_id,
                formioroom_id = x.formioroom_id,
                asset_count = context.AssetFormIOBuildingMappings.Include(x => x.Asset).Where(y => y.formioroom_id == x.formioroom_id
                && y.formiofloor_id == x.formiofloor_id && y.asset_id != null && y.Asset.status == (int)Status.AssetActive).Count()

            }).ToList();

            return (response, count);
        }
        public string GetViasubcomponenentCircuit(Guid via_subcomponent_asset_id)
        {
            return context.AssetSubLevelcomponentMapping.Where(x => x.sublevelcomponent_asset_id == via_subcomponent_asset_id && !x.is_deleted).Select(x => x.circuit).FirstOrDefault();
        }

        public List<AssetParentHierarchyMapping> GetParentMappingByParentAssetId(Guid parent_asset_id)
        {
            return context.AssetParentHierarchyMapping.Where(x => x.parent_asset_id == parent_asset_id && !x.is_deleted)
                .ToList();
        }
        public List<AssetTopLevelcomponentMapping> GetToplevelMappingsListByTopLevelAssetId(Guid asset_id)
        {
            return context.AssetTopLevelcomponentMapping.Where(x => x.toplevelcomponent_asset_id == asset_id && !x.is_deleted).ToList();
        }
        public List<AssetChildrenHierarchyMapping> GetChildrenMappingByChildAssetId(Guid asset_id)
        {
            return context.AssetChildrenHierarchyMapping.Where(x => x.children_asset_id == asset_id && !x.is_deleted)
                .ToList();
        }
        public List<AssetParentHierarchyMapping> GetParentMappingsByOcpOcpMainId(Guid asset_id)
        {
            return context.AssetParentHierarchyMapping.Where(x => (x.fed_by_via_subcomponant_asset_id == asset_id
            || x.via_subcomponent_asset_id == asset_id) && !x.is_deleted)
                .ToList();
        }

        public WOOnboardingAssetsImagesMapping GetLatestNameplateImageById(Guid woonboardingassets_id)
        {
            return context.WOOnboardingAssetsImagesMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id
            && x.asset_photo_type == (int)AssetPhotoType.Nameplate_Photo
            && !x.is_deleted).OrderByDescending(x => x.created_at).FirstOrDefault();
        }
        public AssetProfileImages GetLatestNameplateImageByAssetId(Guid asset_id)
        {
            return context.AssetProfileImages.Where(x => x.asset_id == asset_id
            && x.asset_photo_type == (int)AssetPhotoType.Nameplate_Photo
            && !x.is_deleted).OrderByDescending(x => x.created_at).FirstOrDefault();
        }
        public AssetGroup GetAssetGroupById(Guid asset_group_id)
        {
            return context.AssetGroup.Where(x => x.asset_group_id == asset_group_id).FirstOrDefault();
        }
        public List<AssetGroup> AssetGroupsDropdownList()
        {
            return context.AssetGroup.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_deleted).ToList();
        }

        public List<Asset> AssetListDropdownForAssetGroup(Guid asset_group_id)
        {
            return context.Assets.Where(x => x.asset_group_id != asset_group_id && x.status == (int)Status.AssetActive).ToList();
        }
        public (List<GetAllAssetGroupsList_Class>, int) GetAllAssetGroupsList(GetAllAssetGroupsListRequestModel requestModel)
        {
            IQueryable<AssetGroup> query = context.AssetGroup.
                Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_deleted)
                .Include(x => x.Assets).ThenInclude(x => x.InspectionTemplateAssetClass);

            // search string
            if (!string.IsNullOrEmpty(requestModel.search_string))
            {
                var searchstring = requestModel.search_string.ToLower().ToString();
                query = query.Where(x => (x.asset_group_name.ToLower().Contains(searchstring)
                || x.Assets.Any(a => a.InspectionTemplateAssetClass.asset_class_code.ToLower().Contains(searchstring)
                || x.Assets.Any(a => a.InspectionTemplateAssetClass.asset_class_name.ToLower().Contains(searchstring)
                || x.Assets.Any(a => a.name.ToLower().Contains(searchstring)
                )))));
            }

            int count = query.Count();

            if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
            {
                query = query.OrderByDescending(x => x.created_at).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
            }

            var response = query.Select(x => new GetAllAssetGroupsList_Class
            {
                asset_group_id = x.asset_group_id,
                asset_group_description = x.asset_group_description,
                asset_group_name = x.asset_group_name,
                criticality_index_type = x.criticality_index_type,
                asset_list = x.Assets.Where(x => x.status == (int)Status.AssetActive).Select(y => new AssetGroup_AssetData_Class
                {
                    asset_id = y.asset_id,
                    criticality_index_type = y.criticality_index_type,
                    name = y.name,
                    inspectiontemplate_asset_class_id = y.inspectiontemplate_asset_class_id.Value,
                    asset_class_code = y.InspectionTemplateAssetClass.asset_class_code,
                    asset_health_json = "{\"electrical\":\"alert\",\"visualMechanical\":\"na\",\"ir\":\"danger\",\"serviceability\":\"acceptable\"}",
                    asset_health_index = 0.9

                }).ToList()
            }).ToList();

            return (response, count);
        }
        public (List<GetAllOnboardingAssetsByWOIdResponseModel>, int) GetAllOnboardingAssetsByWOId(GetAllOnboardingAssetsByWOIdRequestModel requestModel)
        {
            IQueryable<WOOnboardingAssets> query = context.WOOnboardingAssets.Where(x => x.wo_id == requestModel.wo_id && !x.is_deleted);

            if (requestModel.asset_class_code != null && requestModel.asset_class_code.Count > 0)
            {
                query = query.Where(x => requestModel.asset_class_code.Contains(x.asset_class_code));
            }
            if (requestModel.asset_class_name != null && requestModel.asset_class_name.Count > 0)
            {
                query = query.Where(x => requestModel.asset_class_name.Contains(x.asset_class_name));
            }
            if (requestModel.buildings != null && requestModel.buildings.Count > 0)
            {
                query = query.Where(x => requestModel.buildings.Contains(x.building));
            }
            if (requestModel.floors != null && requestModel.floors.Count > 0)
            {
                query = query.Where(x => requestModel.floors.Contains(x.floor));
            }
            if (requestModel.rooms != null && requestModel.rooms.Count > 0)
            {
                query = query.Where(x => requestModel.rooms.Contains(x.room));
            }
            if (requestModel.sections != null && requestModel.sections.Count > 0)
            {
                query = query.Where(x => requestModel.sections.Contains(x.section));
            }
            // search string
            if (!string.IsNullOrEmpty(requestModel.search_string))
            {
                var searchstring = requestModel.search_string.ToLower().ToString();
                query = query.Where(x => (x.asset_name.ToLower().Contains(searchstring))
                    || (x.asset_class_name.ToLower().Contains(searchstring))
                    || (x.asset_class_code.ToLower().Contains(searchstring))
                    || (x.building.ToLower().Contains(searchstring))
                    || (x.floor.ToLower().Contains(searchstring))
                    || (x.room.ToLower().Contains(searchstring))
                    || (x.section.ToLower().Contains(searchstring))
                    );
            }
            int count = query.Count();

            if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
            {
                query = query.OrderByDescending(x => x.created_at).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
            }

            var response = query.Select(x => new GetAllOnboardingAssetsByWOIdResponseModel
            {
                asset_name = x.asset_name,
                asset_class_code = x.asset_class_code,
                asset_class_name = x.asset_class_name,
                building = x.building,
                floor = x.floor,
                room = x.room,
                section = x.section,
                status = x.status,
                component_level_type_id = x.TempAsset.component_level_type_id,
                site_id = x.site_id,
                site_name = x.Sites.site_name,
                status_name = x.StatusMaster.status_name,
                inspectiontemplate_asset_class_id = x.TempAsset.inspectiontemplate_asset_class_id,
                QR_code = x.TempAsset.QR_code,
                form_nameplate_info = x.TempAsset.form_nameplate_info,
                arc_flash_label_valid = x.TempAsset.arc_flash_label_valid,
                maintenance_index_type = x.TempAsset.maintenance_index_type,
                asset_class_type = x.TempAsset.InspectionTemplateAssetClass != null && x.TempAsset.InspectionTemplateAssetClass.FormIOType != null ? x.TempAsset.InspectionTemplateAssetClass.FormIOType.form_type_name : null,
                temp_issues_count = context.WOLineIssue.Where(y => y.woonboardingassets_id == x.woonboardingassets_id && !y.is_deleted).Count(),
                issues_title_list = x.WOLineIssue.Where(y => !y.is_deleted) != null ? x.WOLineIssue.Where(y => !y.is_deleted).Select(y => y.issue_title).ToList() : new List<string>()
                //toplevelcomponent_asset_id = x.component_level_type_id==2 ? x.woonboardingassets_id:
            }).ToList();

            return (response, count);
        }

        public List<asset_node_main_class> GetAllAssetsListForReactFlow()
        {
            IQueryable<Asset> query = context.Assets.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && x.status == (int)Status.AssetActive)
                .Include(x => x.InspectionTemplateAssetClass).ThenInclude(x => x.FormIOType)
                .Include(x => x.AssetTopLevelcomponentMapping);

            var res = query.Select(x => new asset_node_main_class
            {
                asset_id = x.asset_id.ToString(),
                asset_node_data_json = x.asset_node_data_json,
                type = "",//x.InspectionTemplateAssetClass.FormIOType.form_type_name,
                position = new asset_node_position
                {
                    x = x.x_axis,
                    y = x.y_axis
                },
                data = new asset_node_data_class
                {
                    code = x.InspectionTemplateAssetClass.asset_class_code,
                    label = x.name,
                    background = "none",
                    border = "#ededed"
                },
                parent_id = x.component_level_type_id==2 && x.AssetTopLevelcomponentMapping.Where(x=>!x.is_deleted).FirstOrDefault() != null ? x.AssetTopLevelcomponentMapping.Where(x => !x.is_deleted).FirstOrDefault().toplevelcomponent_asset_id.ToString() : null,
                extent = x.component_level_type_id == 2 ? "parent" : null

            }).ToList();

            return res;
        }

        public List<initialEdges_class> GetAllAssetsConnectionsForReactFlow()
        {
            IQueryable<AssetParentHierarchyMapping> query = context.AssetParentHierarchyMapping
                .Where(x => !x.is_deleted && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));

            var res = query.Select(x => new initialEdges_class
            {
                id = x.asset_parent_hierrachy_id.ToString(),
                source = x.parent_asset_id.ToString(),
                target = x.asset_id.ToString(),
                via_subcomponent_asset_id = x.via_subcomponent_asset_id,
                fed_by_via_subcomponant_asset_id= x.fed_by_via_subcomponant_asset_id,
                label=x.label,
                style = new style_class { stroke = x.style }

            }).ToList();

            return res;
        }
        public List<Asset> GetAllAssetsListByIds(List<Guid> asset_ids)
        {
            return context.Assets.Where(x => asset_ids.Contains(x.asset_id)).ToList();
        }

        public List<Asset> GetAllAssetsByAssetGroupId(Guid assetGroupId)
        {
            return context.Assets.Where(x => x.asset_group_id == assetGroupId && x.status == (int)Status.AssetActive).ToList();
        }


        public List<TempAsset> GetAllTempAssetsByAssetGroupId(Guid assetGroupId)
        {
            return context.TempAsset.Where(x => x.asset_group_id == assetGroupId && !x.is_deleted).ToList();
        }

        public List<Asset> GetAssetsByAssetId(List<Guid> assetIds)
        {
            return context.Assets
                  .Where(x => assetIds.Contains(x.asset_id) && x.status == (int)Status.AssetActive)
                  .ToList();
        }

        public List<AssetParentHierarchyMapping> GetAssetParentHirerachyList(List<string> parentHirerachyIds)
        {
            return context.AssetParentHierarchyMapping.Where(x => parentHirerachyIds.Contains(x.asset_parent_hierrachy_id.ToString())).ToList();
        }
        public AssetChildrenHierarchyMapping GetAssetChildrenMapping(Guid parent_id,Guid child_id)
        {
            return context.AssetChildrenHierarchyMapping.Where(x => x.asset_id==parent_id && x.children_asset_id == child_id && !x.is_deleted).FirstOrDefault();
        }
    }
}
