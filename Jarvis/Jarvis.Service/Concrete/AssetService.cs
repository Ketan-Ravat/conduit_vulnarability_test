using AutoMapper;
using Jarvis.db.DBResponseModel;
using Jarvis.db.Models;
using Jarvis.Service.Abstract;
using Jarvis.Shared.Helper;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.Utility;
using Jarvis.ViewModels.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
//using OfficeOpenXml;
//using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TimeZoneConverter;
using Logger = Jarvis.Shared.Utility.Logger;

namespace Jarvis.Service.Concrete
{
    public class AssetService : BaseService, IAssetService
    {
        public readonly IMapper _mapper;
        private Logger _logger;

        public AssetService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            _logger = Logger.GetInstance<AssetService>();
        }

        public async Task<List<string>> AddAssetsFromExcelAsync(AddAssetRequestModel request)
        {
            bool isSuccess = false;
            List<string> notUpdated = new List<string>();
            //using (var _dbtransaction = _UoW.BeginTransaction())
            //{
            try
            {
                var assets = _mapper.Map<List<Asset>>(request.AssetRequestModel);

                foreach (var asset in assets)
                {
                    //var addedAsset = _UoW.AssetRepository.GetAssetsByInternalAssetID(asset.internal_asset_id);
                    var addedAsset = _UoW.AssetRepository.GetCompanyAssetsByInternalAssetID(asset.internal_asset_id, request.company_id.ToString());
                    _UoW.BeginTransaction();
                    if (addedAsset != null && addedAsset.asset_id != null)
                    {
                        addedAsset.internal_asset_id = asset.internal_asset_id;
                        addedAsset.name = asset.name;
                        addedAsset.children = asset.children;
                        addedAsset.parent = asset.parent;
                        addedAsset.asset_type = asset.asset_type;
                        addedAsset.product_name = asset.product_name;
                        addedAsset.model_name = asset.model_name;
                        addedAsset.asset_serial_number = asset.asset_serial_number;
                        addedAsset.modified_at = DateTime.UtcNow;
                        addedAsset.model_year = asset.model_year;
                        addedAsset.current_stage = asset.current_stage;
                        addedAsset.levels = asset.levels;
                        addedAsset.condition_index = asset.condition_index;
                        addedAsset.criticality_index = asset.criticality_index;
                        addedAsset.condition_state = asset.condition_state;

                        var site = _UoW.SiteRepository.FindSiteBySiteLocation(asset.site_location, request.company_id);

                        if (site != null)
                        {
                            addedAsset.site_id = site.site_id;
                            addedAsset.company_id = site.company_id.ToString();
                            addedAsset.site_location = site.location;
                            addedAsset.status = (int)Shared.StatusEnums.Status.AssetActive;
                            var updateResult = await _UoW.AssetRepository.Update(addedAsset);
                            if (updateResult > 0)
                            {
                                _UoW.CommitTransaction();
                            }
                            else
                            {
                                notUpdated.Add(asset.internal_asset_id);
                                _UoW.RollbackTransaction();
                            }
                        }
                        else
                        {
                            notUpdated.Add(asset.internal_asset_id);
                            _UoW.RollbackTransaction();
                        }
                    }
                    else
                    {
                        asset.created_at = DateTime.UtcNow;
                        asset.modified_at = DateTime.UtcNow;
                        asset.usage = 0;
                        var site = _UoW.SiteRepository.FindSiteBySiteLocation(asset.site_location, request.company_id);

                        if (site != null)
                        {
                            asset.site_id = site.site_id;
                            asset.company_id = site.company_id.ToString();
                            asset.site_location = site.location;
                            asset.status = (int)Shared.StatusEnums.Status.AssetActive;
                            var insertResult = await _UoW.AssetRepository.Insert(asset);
                            if (insertResult > 0)
                            {
                                _UoW.SaveChanges();
                                if (asset.asset_barcode_id == null || asset.asset_barcode_id == "")
                                {
                                    asset.asset_barcode_id = asset.asset_id.ToString();
                                }
                                else
                                {
                                    asset.asset_barcode_id = asset.asset_barcode_id;
                                }
                                var updateResult = await _UoW.AssetRepository.Update(asset);
                                if (updateResult > 0)
                                {
                                    _UoW.SaveChanges();
                                    _UoW.CommitTransaction();
                                }
                            }
                            else
                            {
                                notUpdated.Add(asset.internal_asset_id);
                                _UoW.RollbackTransaction();
                            }
                        }
                        else
                        {
                            notUpdated.Add(asset.internal_asset_id);
                            _UoW.RollbackTransaction();
                        }
                    }
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                _UoW.RollbackTransaction();
                //Logger.Log("Error in add assets " + e.Message);
                throw e;
            }
            return notUpdated;
        }

        public ListViewModel<AssetsResponseModel> GetAllAssets(int status, int pagesize, int pageindex)
        {
            ListViewModel<AssetsResponseModel> responseModel = new ListViewModel<AssetsResponseModel>();
            try
            {
                var response = _UoW.AssetRepository.GetAllAssets(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), status, pagesize, pageindex);
                if (response.Count > 0)
                {
                    int totalassets = response.Count;
                    if (pageindex != 0 && pagesize != 0)
                    {
                        response = response.Skip((pageindex - 1) * pagesize).Take(pagesize).OrderByDescending(x => x.created_at).ToList();
                    }
                    responseModel.list = _mapper.Map<List<AssetsResponseModel>>(response);
                    responseModel.listsize = totalassets;
                    //List<AssetInspectionFormResponseModel> assetInspectionFormResponseModels = new List<AssetInspectionFormResponseModel>();
                    //foreach (var asset in responseModel)
                    //{
                    //    AssetInspectionFormResponseModel assetInspectionFormResponseModel = new AssetInspectionFormResponseModel();
                    //    if (asset.InspectionForms != null && asset.InspectionForms.form_attributes != null)
                    //    {
                    //        var formAttrbutes = asset.InspectionForms.form_attributes;
                    //        List<CategoryWiseAttributes> categoryWiseAttributes = new List<CategoryWiseAttributes>();

                    //        var results = formAttrbutes.GroupBy(
                    //                p => p.category_id,
                    //                (key, g) => new { category_id = key, Attributes = g.ToList() });
                    //        var list1 = results.ToList();

                    //        foreach (var item in list1)
                    //        {
                    //            CategoryWiseAttributes categoryWiseAttribute = new CategoryWiseAttributes();
                    //            categoryWiseAttribute.category_id = item.category_id;
                    //            var categoryObject = _UoW.InspectionFormRepository.GetCategoryByID(item.category_id);
                    //            if (categoryObject != null)
                    //            {
                    //                categoryWiseAttribute.name = categoryObject.name;
                    //            }
                    //            categoryWiseAttribute.form_attributes = item.Attributes;
                    //            categoryWiseAttributes.Add(categoryWiseAttribute);
                    //        }
                    //        //assetInspectionFormResponseModel.categoryAttributeList = categoryWiseAttributes;
                    //        //assetInspectionFormResponseModels.Add(asset.InspectionForms);
                    //        //assetInspectionFormResponseModels.Add(assetInspectionFormResponseModel);

                    //        asset.InspectionForms.categoryAttributeList = categoryWiseAttributes;
                    //    }
                    //    var workorderList = _UoW.WorkOrderRepository.GetWorkOrderByAssetId(asset.asset_id.ToString(),pagesize,pageindex);
                    //    if (workorderList.Count > 0)
                    //    {
                    //        var workOrderResponse = _mapper.Map<List<WorkOrderResponseModel>>(workorderList);
                    //        asset.WorkOrders = workOrderResponse;
                    //    }
                    //    else
                    //    {
                    //        // do nothing;
                    //    }
                    //}
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public ListViewModel<AssetsResponseModel> GetSubAssetsByAssetID(string asset_id, int pagesize, int pageindex)
        {
            ListViewModel<AssetsResponseModel> responseModel = new ListViewModel<AssetsResponseModel>();
            try
            {
                var response = _UoW.AssetRepository.GetSubAssetsByAssetID(asset_id, pagesize, pageindex);
                if (response?.list?.Count > 0)
                {
                    responseModel.list = _mapper.Map<List<AssetsResponseModel>>(response.list);
                    if (responseModel.list?.Count > 0)
                    {
                        responseModel.list.ForEach(x =>
                        {
                            x.Issues = x.Issues.Where(y => y.status == (int)Status.New || y.status == (int)Status.InProgress || y.status == (int)Status.Waiting).ToList();
                            x.openIssuesCount = x.Issues.Count();
                            if (x.Inspections?.Count > 0)
                            {
                                x.inspectionlistsize = x.Inspections.Count;
                            }
                            if (x.Issues?.Count > 0)
                            {
                                x.issuelistsize = x.Issues.Count;
                            }
                            var totalPM = _UoW.AssetPMsRepository.GetAssetPMCountByAssetId(x.asset_id);
                            x.assetPMCount = totalPM;
                            var is_child = response.list.Where(q => q.asset_id == x.asset_id).Select(w => w.children).FirstOrDefault();
                            x.is_child_available = false;
                            if (!String.IsNullOrEmpty(is_child))
                            {
                                x.is_child_available = true;
                            }
                        });

                    }
                    responseModel.listsize = response.listsize;
                    responseModel.pageIndex = pageindex;
                    responseModel.pageSize = pagesize;
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public ListViewModel<GetChildrenByAssetIDResponsemodel> GetChildrenByAssetID(string asset_id, int pagesize, int pageindex)
        {
            ListViewModel<GetChildrenByAssetIDResponsemodel> responseModel = new ListViewModel<GetChildrenByAssetIDResponsemodel>();
            try
            {
                var response = _UoW.AssetRepository.GetChildrenByAssetID(asset_id, pagesize, pageindex);
                if (response?.list?.Count > 0)
                {
                    responseModel.list = _mapper.Map<List<GetChildrenByAssetIDResponsemodel>>(response.list);
                    responseModel.listsize = response.listsize;
                    responseModel.pageIndex = pageindex;
                    responseModel.pageSize = pagesize;
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public FilterAssetsResponseview<AssetsResponseModel> FilterAssets(FilterAssetsRequestModel requestModel)
        {
            FilterAssetsResponseview<AssetsResponseModel> responseModel = new FilterAssetsResponseview<AssetsResponseModel>();
            try
            {
                var response = _UoW.AssetRepository.FilterAssets(requestModel);
                if (response.Item1?.list?.Count > 0)
                {
                    responseModel.list = _mapper.Map<List<AssetsResponseModel>>(response.Item1.list);
                    if (responseModel.list?.Count > 0)
                    {
                        responseModel.list.ForEach(x =>
                        {
                            x.Issues = x.Issues.Where(y => y.status == (int)Status.New || y.status == (int)Status.InProgress || y.status == (int)Status.Waiting).ToList();
                            x.openIssuesCount = x.Issues.Count();
                            if (x.Inspections?.Count > 0)
                            {
                                x.inspectionlistsize = x.Inspections.Count;
                            }
                            if (x.Issues?.Count > 0)
                            {
                                x.issuelistsize = x.Issues.Count;
                            }
                            var totalPM = _UoW.AssetPMsRepository.GetAssetPMCountByAssetId(x.asset_id);
                            x.assetPMCount = totalPM;
                            x.is_LVCB_Asset = false;
                            var form_list = requestModel.form_io_form_id.Split(',').ToList().Where(x => !String.IsNullOrEmpty(x)).ToList();
                            if (x.form_id != null && form_list.Contains(x.form_id.ToString()))
                            {
                                x.is_LVCB_Asset = true;
                            }
                        });

                        /// assign building mapping to response 
                        /// 

                        var building_hierarchy_mapping = response.Item2.Where(x => x != null);

                        var building_date = building_hierarchy_mapping.Where(x => x.formiobuilding_id != null && x.FormIOBuildings != null).Select(x => x.FormIOBuildings).ToList();
                        var building = building_date.GroupBy(p => p.formiobuilding_id)
                                   .Select(g => g.First())
                                   .ToList();
                        var floor = building_hierarchy_mapping.Where(x => x.FormIOFloors != null).Select(x => x.FormIOFloors).GroupBy(p => p.formiofloor_id)
                                   .Select(g => g.First())
                                   .ToList();
                        var room = building_hierarchy_mapping.Where(x => x.FormIORooms != null).Select(x => x.FormIORooms).GroupBy(p => p.formioroom_id)
                                   .Select(g => g.First())
                                   .ToList();
                        var section = building_hierarchy_mapping.Where(x => x.FormIOSections != null).Select(x => x.FormIOSections).GroupBy(p => p.formiosection_id)
                                   .Select(g => g.First())
                                   .ToList();
                        responseModel.filterassetbuildingbocationoptions = new FilterAssetBuildingLocationOptions();
                        responseModel.filterassetbuildingbocationoptions.buildings = _mapper.Map<List<FilterAssetBuildingLocationOptionsmapping>>(building);
                        responseModel.filterassetbuildingbocationoptions.floors = _mapper.Map<List<FilterAssetBuildingLocationOptionsmapping>>(floor);
                        responseModel.filterassetbuildingbocationoptions.rooms = _mapper.Map<List<FilterAssetBuildingLocationOptionsmapping>>(room);
                        responseModel.filterassetbuildingbocationoptions.sections = _mapper.Map<List<FilterAssetBuildingLocationOptionsmapping>>(section);

                        building_hierarchy_mapping = building_hierarchy_mapping.GroupBy(x => x.formioroom_id).Select(g => g.First()).ToList();

                        responseModel.filterassetbuildingbocationoptions.rooms_with_floor_building = _mapper.Map<List<FilterAssetRoomFloorBuildingLocationOptionsmapping>>(building_hierarchy_mapping);

                    }
                    responseModel.listsize = response.Item1.listsize;
                    responseModel.pageIndex = requestModel.pageindex;
                    responseModel.pageSize = requestModel.pagesize;
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public ListViewModel<AssetListResponseModel> FilterAssetsNameOptions(FilterAssetsOptionsRequestModel requestModel)
        {
            ListViewModel<AssetListResponseModel> filterResponse = new ListViewModel<AssetListResponseModel>();
            try
            {
                var response = _UoW.AssetRepository.FilterAssetNameOptions(requestModel);
                if (response.Count > 0)
                {
                    int totalassets = response.Count;
                    /*if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }*/
                    if (requestModel.pageindex != 0 || requestModel.pagesize != 0)
                    {
                        response = response.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).OrderBy(x => x.name).ToList();
                    }
                    filterResponse.list = _mapper.Map<List<AssetListResponseModel>>(response);
                    filterResponse.listsize = totalassets;
                    filterResponse.pageIndex = requestModel.pageindex;
                    filterResponse.pageSize = requestModel.pagesize;
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return filterResponse;
        }

        public ListViewModel<string> FilterAssetsModelOptions(FilterAssetsOptionsRequestModel requestModel)
        {
            ListViewModel<string> filterResponse = new ListViewModel<string>();
            try
            {
                var response = _UoW.AssetRepository.FilterAssetModelOptions(requestModel);
                if (response.Count > 0)
                {
                    int totalassets = response.Count;
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }

                    filterResponse.list = response.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                    filterResponse.listsize = totalassets;
                    filterResponse.pageIndex = requestModel.pageindex;
                    filterResponse.pageSize = requestModel.pagesize;
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return filterResponse;
        }

        public ListViewModel<string> FilterAssetsModelYearOptions(FilterAssetsOptionsRequestModel requestModel)
        {
            ListViewModel<string> filterResponse = new ListViewModel<string>();
            try
            {
                var response = _UoW.AssetRepository.FilterAssetModelYearOptions(requestModel);
                if (response.Count > 0)
                {
                    int totalassets = response.Count;
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    filterResponse.list = response.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                    filterResponse.listsize = totalassets;
                    filterResponse.pageIndex = requestModel.pageindex;
                    filterResponse.pageSize = requestModel.pagesize;
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return filterResponse;
        }

        public ListViewModel<int> FilterAssetsStatusOptions(FilterAssetsOptionsRequestModel requestModel)
        {
            ListViewModel<int> filterResponse = new ListViewModel<int>();
            try
            {
                var response = _UoW.AssetRepository.FilterAssetStatusOptions(requestModel);
                if (response.Count > 0)
                {
                    int totalassets = response.Count;
                    filterResponse.list = response;
                    filterResponse.listsize = totalassets;
                    filterResponse.pageIndex = requestModel.pageindex;
                    filterResponse.pageSize = requestModel.pagesize;
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return filterResponse;
        }

        public ListViewModel<SitesViewModel> FilterAssetsSitesOptions(FilterAssetsOptionsRequestModel requestModel)
        {
            ListViewModel<SitesViewModel> filterResponse = new ListViewModel<SitesViewModel>();
            try
            {
                var response = _UoW.AssetRepository.FilterAssetSitesOptions(requestModel);
                if (response.Count > 0)
                {
                    var sites = response;
                    filterResponse.list = _mapper.Map<List<SitesViewModel>>(sites);
                    filterResponse.list.ForEach(x =>
                    {
                        var sitedetails = sites.Where(y => y.site_id == x.site_id).FirstOrDefault();
                        if (sitedetails != null)
                        {
                            x.comapny_name = sitedetails.Company?.company_name;
                        }
                    });
                    int totalassets = filterResponse.list.Count;
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    filterResponse.list = filterResponse.list.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                    filterResponse.listsize = totalassets;
                    filterResponse.pageIndex = requestModel.pageindex;
                    filterResponse.pageSize = requestModel.pagesize;
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return filterResponse;
        }

        public ListViewModel<CompanyViewModel> FilterAssetsCompanyOptions(FilterAssetsOptionsRequestModel requestModel)
        {
            ListViewModel<CompanyViewModel> filterResponse = new ListViewModel<CompanyViewModel>();
            try
            {
                var response = _UoW.AssetRepository.FilterAssetCompanyOptions(requestModel);
                if (response.Count > 0)
                {
                    var company = response;
                    filterResponse.list = _mapper.Map<List<CompanyViewModel>>(company);
                    int total = filterResponse.list.Count;
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    filterResponse.list = filterResponse.list.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                    filterResponse.listsize = total;
                    filterResponse.pageIndex = requestModel.pageindex;
                    filterResponse.pageSize = requestModel.pagesize;
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return filterResponse;
        }

        public async Task<AssetsResponseModel> GetAsset(int pagesize, int pageindex, GetAssetsByIdRequestModel requestModel)
        {



            AssetsResponseModel responseModel = new AssetsResponseModel();
            try
            {

                BaseViewModel ispendingstage = new BaseViewModel();

                //var userRole = await _UoW.UserRepository.GetRoleFromRole(requestModel.userid);
                if (requestModel.barcode_id != null && requestModel.barcode_id != string.Empty)
                {
                    ispendingstage = _UoW.AssetRepository.CheckAssetIntoInspectionByAssetID(requestModel.barcode_id, UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                }
                else
                {
                    ispendingstage = _UoW.AssetRepository.CheckAssetIntoInspectionByInternalAssetId(requestModel.internal_asset_id, UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                }
                var userrole = string.Empty;
                var roles = await _UoW.UserRepository.GetAllRoles();
                if (roles?.Count > 0)
                {
                    userrole = roles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.name).FirstOrDefault();
                }
                if (ispendingstage.success || userrole == GlobalConstants.Manager || userrole == GlobalConstants.Executive || userrole == GlobalConstants.Technician || userrole == GlobalConstants.CompanyAdmin)
                {
                    var response = _UoW.AssetRepository.GetAsset(pagesize, pageindex, requestModel);

                    if (response != null)
                    {
                        if (response.AssetProfileImages != null && response.AssetProfileImages.Count > 0)
                        {
                            response.AssetProfileImages = response.AssetProfileImages.OrderByDescending(x => x.created_at).ToList();
                        }
                        if (response.AssetSubLevelcomponentMapping != null && response.AssetSubLevelcomponentMapping.Count > 0)
                        {
                            response.AssetSubLevelcomponentMapping = response.AssetSubLevelcomponentMapping.Where(x => !x.is_deleted).OrderByDescending(x => x.created_at).ToList();
                        }
                        if (response.AssetParentHierarchyMapping != null && response.AssetParentHierarchyMapping.Count > 0)
                        {
                            response.AssetParentHierarchyMapping = response.AssetParentHierarchyMapping.Where(x => !x.is_deleted).ToList();
                            // get asset's parent 
                            var asset_parents_ids = response.AssetParentHierarchyMapping.Select(x => x.parent_asset_id.Value).ToList();
                            var db_asset_parents = _UoW.AssetRepository.GetAssetParentsByIDs(asset_parents_ids);

                            response.AssetParentHierarchyMapping = response.AssetParentHierarchyMapping.Where(x => db_asset_parents.Contains(x.parent_asset_id.Value)).ToList();
                        }
                        int inspectionlistsize = 0;
                        int openIssuesCount = 0;
                        int pmCount = 0;

                        if (response.AssetIssue?.Count > 0)
                        {
                            openIssuesCount = response.AssetIssue.Where(x => !x.is_deleted).Count();
                        }
                        if (response.AssetPMs?.Count > 0)
                        {
                            pmCount = response.AssetPMs.Count();
                        }


                        //string userrole = await _UoW.UserRepository.GetUserRoleFromId(requestModel.userid);
                        if ((Roles.Operator == UpdatedGenericRequestmodel.CurrentUser.role_id && response.InspectionForms != null) || (Roles.Operator != UpdatedGenericRequestmodel.CurrentUser.role_id))
                        {

                            responseModel = _mapper.Map<AssetsResponseModel>(response);

                            //get np template from class
                            if (!String.IsNullOrEmpty(response.InspectionTemplateAssetClass.form_nameplate_info) && !String.IsNullOrEmpty(response.form_retrived_nameplate_info))
                            {
                                responseModel.form_retrived_nameplate_info = CompareNPDatajsonAndTemplateJson(response.InspectionTemplateAssetClass.form_nameplate_info, response.form_retrived_nameplate_info);
                            }

                            if (responseModel.asset_profile_images != null && responseModel.asset_profile_images.Count > 0)
                            {
                                responseModel.asset_profile_images = responseModel.asset_profile_images.Where(x => !x.is_deleted).ToList();
                            }
                            if (responseModel.ob_ir_Image_label_list != null && responseModel.ob_ir_Image_label_list.Count > 0)
                            {
                                responseModel.ob_ir_Image_label_list = responseModel.ob_ir_Image_label_list.Where(x => !x.is_deleted).ToList();
                            }
                            if (responseModel.asset_parent_mapping_list != null && responseModel.asset_parent_mapping_list.Count > 0)
                            {
                                responseModel.asset_parent_mapping_list.ForEach(parent =>
                                {

                                    parent.parent_asset_name = _UoW.WorkOrderRepository.GetAssetByAssetIdActive(parent.parent_asset_id.Value).name;

                                    if (parent.via_subcomponent_asset_id != null && parent.via_subcomponent_asset_id != Guid.Empty)
                                    {
                                        var via_sub = _UoW.WorkOrderRepository.GetAssetByAssetIdActive(parent.via_subcomponent_asset_id.Value);
                                        parent.via_subcomponent_asset_name = via_sub != null ? via_sub.name : null;
                                    }
                                    if (parent.fed_by_via_subcomponant_asset_id != null && parent.fed_by_via_subcomponant_asset_id != Guid.Empty)
                                    {
                                        var via_fedbysub = _UoW.WorkOrderRepository.GetAssetByAssetIdActive(parent.fed_by_via_subcomponant_asset_id.Value);
                                        parent.fed_by_via_subcomponant_asset_name = via_fedbysub != null ? via_fedbysub.name : null;
                                    }

                                });
                            }
                            if (response.AssetReplacementMapping != null && response.AssetReplacementMapping.Count > 0)
                            {
                                var lattest_replaced_asset = response.AssetReplacementMapping.Where(x => !x.is_deleted).OrderByDescending(x => x.created_at).FirstOrDefault();
                                if (lattest_replaced_asset != null && lattest_replaced_asset.replaced_asset_id != Guid.Empty)
                                {
                                    responseModel.replaced_asset_id = lattest_replaced_asset.replaced_asset_id;
                                    var get_asset = _UoW.AssetRepository.GetAssetByIDs(new List<Guid> { lattest_replaced_asset.replaced_asset_id });
                                    responseModel.replaced_asset_name = get_asset.FirstOrDefault().name;
                                }
                            }

                            if (String.IsNullOrEmpty(responseModel.asset_profile_image))
                            {
                                // New Flow for Profile Image--09April2024
                                string profile_url = null;
                                Guid asset_id = response.asset_id;

                                if (String.IsNullOrEmpty(profile_url))//profile
                                    profile_url = _UoW.AssetRepository.GetAssetImageByTypeAndId(asset_id, (int)AssetPhotoType.Asset_Profile);
                                if (String.IsNullOrEmpty(profile_url))//now additional will be same treated as profile
                                    profile_url = _UoW.AssetRepository.GetAssetImageByTypeAndId(asset_id, (int)AssetPhotoType.Additional_Photos);
                                if (String.IsNullOrEmpty(profile_url))//nameplate
                                    profile_url = _UoW.AssetRepository.GetAssetImageByTypeAndId(asset_id, (int)AssetPhotoType.Nameplate_Photo);
                                if (String.IsNullOrEmpty(profile_url) && responseModel.ob_ir_Image_label_list != null && responseModel.ob_ir_Image_label_list.Count > 0)
                                    //ir image
                                    profile_url = responseModel.ob_ir_Image_label_list.Select(x => x.ir_image_label_url).FirstOrDefault();
                                if (String.IsNullOrEmpty(profile_url) && responseModel.ob_ir_Image_label_list != null && responseModel.ob_ir_Image_label_list.Count > 0)
                                    //visual image
                                    profile_url = responseModel.ob_ir_Image_label_list.Select(x => x.visual_image_label_url).FirstOrDefault();
                                if (String.IsNullOrEmpty(profile_url))//Thermal Anomaly image
                                    profile_url = _UoW.AssetRepository.GetAssetImageByTypeAndId(asset_id, (int)AssetPhotoType.Thermal_Anomly_Photo);
                                if (String.IsNullOrEmpty(profile_url))//NEC Violation image
                                    profile_url = _UoW.AssetRepository.GetAssetImageByTypeAndId(asset_id, (int)AssetPhotoType.NEC_Violation_Photo);
                                if (String.IsNullOrEmpty(profile_url))//Osha Violation image
                                    profile_url = _UoW.AssetRepository.GetAssetImageByTypeAndId(asset_id, (int)AssetPhotoType.OSHA_Violation_Photo);
                                if (String.IsNullOrEmpty(profile_url))//PM_Additional_General_photo  
                                    profile_url = _UoW.AssetRepository.GetAssetImageByTypeAndId(asset_id, (int)AssetPhotoType.PM_Additional_General_photo);
                                if (String.IsNullOrEmpty(profile_url))//PM_Additional_Environment_photo  
                                    profile_url = _UoW.AssetRepository.GetAssetImageByTypeAndId(asset_id, (int)AssetPhotoType.PM_Additional_Environment_photo);
                                if (String.IsNullOrEmpty(profile_url))//Exterior  image
                                    profile_url = _UoW.AssetRepository.GetAssetImageByTypeAndId(asset_id, (int)AssetPhotoType.Exterior_Photo);
                                if (String.IsNullOrEmpty(profile_url))//Schedule  
                                    profile_url = _UoW.AssetRepository.GetAssetImageByTypeAndId(asset_id, (int)AssetPhotoType.Schedule_Photos);
                                if (String.IsNullOrEmpty(profile_url))//Repair  
                                    profile_url = _UoW.AssetRepository.GetAssetImageByTypeAndId(asset_id, (int)AssetPhotoType.Repair_Woline_Issue_Photo);
                                if (String.IsNullOrEmpty(profile_url))//Replace  image
                                    profile_url = _UoW.AssetRepository.GetAssetImageByTypeAndId(asset_id, (int)AssetPhotoType.Replace_Woline_Issue_photo);
                                if (String.IsNullOrEmpty(profile_url))//other  
                                    profile_url = _UoW.AssetRepository.GetAssetImageByTypeAndId(asset_id, (int)AssetPhotoType.Other_Woline_Issue_photo);

                                responseModel.asset_profile_image = profile_url;
                            }

                            if (responseModel.component_level_type_id == (int)ComponentLevelTypes.SublevelComponent)
                            {
                                if (response.AssetTopLevelcomponentMapping != null && response.AssetTopLevelcomponentMapping.Count() > 0)
                                {
                                    var db_toplevel = response.AssetTopLevelcomponentMapping.Where(x => !x.is_deleted).FirstOrDefault();
                                    if (db_toplevel != null)
                                    {
                                        responseModel.asset_toplevel_componenent = new AssetToplevelComponenent();
                                        responseModel.asset_toplevel_componenent.asset_toplevelcomponent_mapping_id = db_toplevel.asset_toplevelcomponent_mapping_id;
                                        responseModel.asset_toplevel_componenent.toplevelcomponent_asset_id = db_toplevel.toplevelcomponent_asset_id;
                                        if (responseModel.asset_toplevel_componenent.toplevelcomponent_asset_id != Guid.Empty)
                                        {
                                            var get_toplevel_asset = _UoW.AssetRepository.GetAssetByIdForNewSubcomponent(db_toplevel.toplevelcomponent_asset_id);
                                            if (get_toplevel_asset != null)
                                            {
                                                responseModel.asset_toplevel_componenent.toplevelcomponent_asset_name = get_toplevel_asset.name;
                                            }
                                        }
                                    }
                                }

                            }

                            // view TopLevel's Subcomponents name
                            if (responseModel.asset_subcomponents_mapping_list != null && responseModel.asset_subcomponents_mapping_list.Count > 0)
                            {
                                foreach (var item in responseModel.asset_subcomponents_mapping_list)
                                {
                                    var sub_asset = _UoW.AssetRepository.GetsubcomonentAssetDetail(item.sublevelcomponent_asset_id.Value);
                                    if (sub_asset != null)
                                    {
                                        item.sublevelcomponent_asset_name = sub_asset.name;
                                        item.sublevelcomponent_asset_class_id = sub_asset.inspectiontemplate_asset_class_id;
                                        item.asset_class_name = sub_asset.InspectionTemplateAssetClass.asset_class_code;
                                        item.asset_class_code = sub_asset.InspectionTemplateAssetClass.asset_class_name;

                                        // Nameplate,Exterior and Multiple AdditionalImages of SubComponents New Flow
                                        if (sub_asset.AssetProfileImages != null && sub_asset.AssetProfileImages.Count > 0)
                                        {
                                            var db_img_list = sub_asset.AssetProfileImages.Where(x => !x.is_deleted).ToList();
                                            var mapped_img_list = _mapper.Map<List<subcomponentasset_image_list_class>>(db_img_list);

                                            item.subcomponentasset_image_list = mapped_img_list;
                                        }
                                    }

                                }
                            }



                            responseModel.inspectionlistsize = inspectionlistsize;
                            responseModel.openIssuesCount = openIssuesCount;
                            responseModel.assetPMCount = pmCount;
                            responseModel.is_LVCB_Asset = false;
                            if (!String.IsNullOrEmpty(responseModel.parent))
                            {
                                var get_asset = _UoW.AssetRepository.GetAssetByInternalIDForhierarchychange(responseModel.parent);
                                if (get_asset != null)
                                {
                                    responseModel.parent = get_asset.name;
                                    responseModel.parent_asset_id = get_asset.asset_id;
                                }
                            }

                            var form_list = requestModel.form_io_form_id.Split(',').ToList().Where(x => !String.IsNullOrEmpty(x)).ToList();
                            if (responseModel.form_id != null && form_list.Contains(responseModel.form_id.ToString()))
                            {
                                responseModel.is_LVCB_Asset = true;
                            }
                            if (responseModel.asset_id == Guid.Parse("45339e2e-41f6-47ee-9770-4af7f96248ef"))
                            {
                                responseModel.visual_insepction_due_in = "OVERDUE";
                                responseModel.mechanical_insepction_due_in = "OVERDUE";
                                responseModel.electrical_insepction_due_in = "750 Days";
                                responseModel.infrared_insepction_due_in = "OVERDUE";
                            }
                            if (responseModel.asset_id == Guid.Parse("2fb51f4f-b47b-4c82-a4cb-1c2f4f9413a6"))
                            {
                                responseModel.visual_insepction_due_in = "180 Days";
                                responseModel.mechanical_insepction_due_in = "180 Days";
                                responseModel.electrical_insepction_due_in = "738 Days";
                                responseModel.infrared_insepction_due_in = "OVERDUE";
                            }
                            if (UpdatedGenericRequestmodel.CurrentUser.site_id == "01c64534-ae74-43c8-8902-ff8386cd48f4")
                            {
                                responseModel.visual_insepction_due_in = responseModel.visual_insepction_last_performed != null ? String.Format("{0:s}", responseModel.visual_insepction_last_performed.Value.AddYears(1)) : null;
                                responseModel.infrared_insepction_due_in = responseModel.infrared_insepction_last_performed != null ? String.Format("{0:s}", responseModel.infrared_insepction_last_performed.Value) : null;
                            }
                            if (responseModel.InspectionForms != null && responseModel.InspectionForms.form_attributes != null)
                            {
                                var formAttrbutes = responseModel.InspectionForms.form_attributes;
                                List<CategoryWiseAttributes> categoryWiseAttributes = new List<CategoryWiseAttributes>();

                                var results = formAttrbutes.GroupBy(
                                        p => p.category_id,
                                        (key, g) => new { category_id = key, Attributes = g.ToList() });

                                var list1 = results.ToList();

                                foreach (var item in list1)
                                {

                                    item.Attributes.ToList().ForEach(x => responseModel.lastinspection_attribute_values.ForEach(z =>
                                    {
                                        if (x.category_id == z.category_id && x.attributes_id == z.id.ToString())
                                        {
                                            Issue workorder = null;
                                            x.value_parameters.ToList().ForEach(y =>
                                            {
                                                if (y.name == "Not Ok")
                                                {
                                                    if (response != null && response.asset_id != Guid.Empty)
                                                    {
                                                        if (workorder == null)
                                                        {
                                                            workorder = new Issue();
                                                        }
                                                        workorder = _UoW.IssueRepository.GetAttributeIssueByBarcodeId(x.attributes_id, response.asset_id.ToString());
                                                    }
                                                    //else
                                                    //{
                                                    //    if (workorder == null)
                                                    //    {
                                                    //        workorder = new WorkOrder();
                                                    //    }
                                                    //    workorder = _UoW.WorkOrderRepository.GetAttributeWorkOrderByInternalAssetId(x.attributes_id, response.asset_id.ToString());
                                                    //}
                                                }
                                                if (y.name == z.value)
                                                {
                                                    y.is_default = true;
                                                }
                                                else
                                                {
                                                    y.is_default = false;
                                                }
                                            });

                                            if (workorder != null)
                                            {
                                                x.issue = _mapper.Map<IssueViewModel>(workorder);
                                                x.workorder = _mapper.Map<IssueViewModel>(workorder);
                                            }
                                        }
                                    }));

                                    CategoryWiseAttributes categoryWiseAttribute = new CategoryWiseAttributes();
                                    categoryWiseAttribute.category_id = item.category_id;
                                    var categoryObject = _UoW.InspectionFormRepository.GetCategoryByID(item.category_id);
                                    if (categoryObject != null)
                                    {
                                        categoryWiseAttribute.name = categoryObject.name;
                                        categoryWiseAttribute.spanish_name = PreferLanguageSingleton.Instance.GetLanguageKeyByName(categoryObject.name, (int)Language.spanish).Result;
                                    }
                                    categoryWiseAttribute.form_attributes = item.Attributes;
                                    categoryWiseAttributes.Add(categoryWiseAttribute);
                                }
                                responseModel.InspectionForms.categoryAttributeList = categoryWiseAttributes;
                            }
                            /*
                            //List<WorkOrder> workorderList = new List<WorkOrder>();
                            //if (requestModel.barcode_id != null && requestModel.barcode_id != string.Empty)
                            //{
                            //    workorderList = _UoW.WorkOrderRepository.GetWorkOrderByAssetId(requestModel.barcode_id, 0, 0);
                            //}
                            //else
                            //{
                            //    workorderList = _UoW.WorkOrderRepository.GetWorkOrderByInternalAssetId(requestModel.internal_asset_id, 0, 0);
                            //}
                            //int workorderlistsize = 0;
                            //if (workorderList.Count > 0)
                            //{
                            //    workorderlistsize = workorderList.Count;

                            //    if (pageindex > 0 && pagesize > 0)
                            //    {
                            //        workorderList = workorderList.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                            //    }
                            //}

                            //var workorderLists = _mapper.Map<List<WorkOrderResponseModel>>(workorderList);
                            */
                            responseModel.Inspections.ForEach(x => x.operator_name = _UoW.UserRepository.GetUserFromUserId(x.operator_id.ToString()));

                            /*
                             * //if (workorderList != null)
                            //{
                            //    var workOrderResponse = _mapper.Map<List<WorkOrderResponseModel>>(workorderList);
                            //    responseModel.WorkOrders = workOrderResponse;
                            //    responseModel.workorderlistsize = workorderlistsize;
                            //}
                            //else
                            //{
                            //    // do nothing;
                            //}*/
                        }
                        else
                        {
                            responseModel.role = 1;
                        }
                        responseModel.response_status = (int)ResponseStatusNumber.Success;
                    }
                    else if (!String.IsNullOrEmpty(requestModel.qr_code))
                    {
                        responseModel.response_status = (int)ResponseStatusNumber.asset_in_different_location;
                    }
                }
                else
                {
                    responseModel.message = ispendingstage.message;
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw;
            }
            return responseModel;
        }

        public ListViewModel<InspectionResponseModel> GetAllInspections(string userid, int pagesize, int pageindex)
        {
            ListViewModel<InspectionResponseModel> responseModel = new ListViewModel<InspectionResponseModel>();
            try
            {
                var response = _UoW.AssetRepository.GetAllInspections(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), pagesize, pagesize);
                if (response.Count > 0)
                {
                    responseModel.list = _mapper.Map<List<InspectionResponseModel>>(response);
                }
            }
            catch (Exception e)
            {
                // Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public async Task<ListViewModel<PendingAndCheckoutInspViewModel>> GetAllCheckedOutAssets(int pagesize, int pageindex)
        {
            ListViewModel<PendingAndCheckoutInspViewModel> responseModels = new ListViewModel<PendingAndCheckoutInspViewModel>();
            try
            {
                var response = await _UoW.AssetRepository.GetAllCheckedOutAssets(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), pagesize, pageindex);
                if (response.Count > 0)
                {
                    int totalcheckedoutasset = response.Count;
                    if (pagesize > 0 && pageindex > 0)
                    {
                        response = response.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    }
                    responseModels.list = _mapper.Map<List<PendingAndCheckoutInspViewModel>>(response);
                    responseModels.listsize = totalcheckedoutasset;
                    //responseModels.list.ForEach(x =>
                    //{
                    //    x.timeelapsed = DateTimeUtil.GetBeforetimeText(x.created_at.Value).FirstOrDefault();
                    //});
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModels;
        }

        public int ValidateInternalAssetID(string internal_asset_id)
        {
            int response = (int)ResponseStatusNumber.Error;
            try
            {
                response = _UoW.IssueRepository.ValidateInternalAssetID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), internal_asset_id);
            }
            catch (Exception e)
            {
                throw e;
            }
            return response;
        }

        public Asset GetAssetDetailsByID(string asset_id)
        {
            return _UoW.AssetRepository.GetAssetByAssetID(asset_id);
        }
        public Asset GetAssetDetailsByQRcode(string QR_code)
        {
            return _UoW.AssetRepository.GetAssetByQRcode(QR_code);
        }

        public AssetViewModel GetAssetByAssetID(string asset_id)
        {
            AssetViewModel asset = new AssetViewModel();
            var response = _UoW.AssetRepository.GetAssetByAssetID(asset_id);
            if (response != null && response.asset_id != null && response.asset_id != Guid.Empty)
            {
                asset = _mapper.Map<AssetViewModel>(response);
            }
            return asset;
        }
        public AssetViewModel GetAssetByIneternalID(string asset_internal_id)
        {
            AssetViewModel asset = new AssetViewModel();
            var response = _UoW.AssetRepository.GetAssetByInternalIDForhierarchychange(asset_internal_id);
            if (response != null && response.asset_id != null && response.asset_id != Guid.Empty)
            {
                asset.asset_id = response.asset_id.ToString();
                asset.name = response.name;
                asset.internal_asset_id = response.internal_asset_id;
                //asset.site_code = response.Sites.site_code;
                asset.site_code = "cvsh  ";
                //  asset.site_logo_img = response.Sites.location;
                // asset = _mapper.Map<AssetViewModel>(response);
            }
            return asset;
        }
        public async Task<ListViewModel<AssetsResponseModel>> SearchAssets(string searchstring, int status, int pagesize, int pageindex)
        {
            try
            {
                ListViewModel<AssetsResponseModel> assets = new ListViewModel<AssetsResponseModel>();
                var assetlist = await _UoW.AssetRepository.SearchAssets(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), searchstring, status, pagesize, pageindex);

                if (assetlist.Count > 0)
                {
                    int totalasset = assetlist.Count;
                    if (pageindex > 0 && pagesize > 0)
                    {
                        assetlist = assetlist.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    }
                    assets.list = _mapper.Map<List<AssetsResponseModel>>(assetlist);
                    assets.listsize = totalasset;
                }
                assets.pageSize = pagesize;
                assets.pageIndex = pageindex;
                return assets;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int> UploadAssetPhoto(UploadAssetPhotoRequestModel requestModel)
        {
            int result = (int)ResponseStatusNumber.Error;
            try
            {
                var asset = _UoW.AssetRepository.GetAssetByAssetID(requestModel.asset_id);
                if (asset != null && asset.asset_id != null && asset.asset_id != Guid.Empty)
                {
                    asset.asset_photo = requestModel.asset_photo;
                    asset.modified_at = DateTime.UtcNow;
                    result = await _UoW.AssetRepository.UploadAssetPhoto(asset);
                }
                else
                {
                    result = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }
        public async Task<int> UploadAssetAttachments(UploadAssetAttachmentsRequestmodel requestModel)
        {
            int result = (int)ResponseStatusNumber.Error;
            try
            {
                foreach (var file in requestModel.S3_files)
                {
                    AssetAttachmentMapping AssetAttachmentMapping = new AssetAttachmentMapping();
                    AssetAttachmentMapping.asset_id = Guid.Parse(requestModel.asset_id);
                    AssetAttachmentMapping.site_id = Guid.Parse(requestModel.site_id);
                    AssetAttachmentMapping.file_name = file.s3_file_name;
                    AssetAttachmentMapping.user_uploaded_file_name = file.user_uploaded_file_name;
                    AssetAttachmentMapping.created_at = DateTime.UtcNow;
                    AssetAttachmentMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    AssetAttachmentMapping.is_deleted = false;

                    var insert = await _UoW.BaseGenericRepository<AssetAttachmentMapping>().Insert(AssetAttachmentMapping);
                    _UoW.SaveChanges();
                }
                result = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }


        public async Task<int> UploadAssetimage(UploadAssetPhotoRequestModel requestModel)
        {
            int result = (int)ResponseStatusNumber.Error;
            try
            {
                var asset = _UoW.AssetRepository.GetAssetByAssetID(requestModel.asset_id);
                if (asset != null && asset.asset_id != null && asset.asset_id != Guid.Empty)
                {
                    asset.modified_at = DateTime.UtcNow;
                    AssetProfileImages AssetProfileImages = new AssetProfileImages();
                    AssetProfileImages.asset_id = asset.asset_id;
                    AssetProfileImages.asset_photo = requestModel.asset_photo;
                    AssetProfileImages.asset_thumbnail_photo = requestModel.thumbnail_photo;
                    AssetProfileImages.created_at = DateTime.UtcNow;
                    AssetProfileImages.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    AssetProfileImages.is_deleted = false;
                    AssetProfileImages.asset_photo_type = requestModel.asset_photo_type;
                    var _insert_image = await _UoW.BaseGenericRepository<AssetProfileImages>().Insert(AssetProfileImages);
                    _UoW.SaveChanges();
                    result = await _UoW.AssetRepository.UploadAssetPhoto(asset);
                }
                else
                {
                    result = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        public async Task<List<AssetsResponseModel>> GetAllAssetsWithInspectionForm(string timestamp)
        {
            try
            {
                List<AssetsResponseModel> assetlist = new List<AssetsResponseModel>();
                var asset = await _UoW.AssetRepository.GetAllAssetsWithInspectionForm(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), timestamp);
                if (asset.Count > 0)
                {
                    assetlist = _mapper.Map<List<AssetsResponseModel>>(asset);

                    List<AssetInspectionFormResponseModel> assetInspectionFormResponseModels = new List<AssetInspectionFormResponseModel>();

                    foreach (var assets in assetlist)
                    {
                        AssetInspectionFormResponseModel assetInspectionFormResponseModel = new AssetInspectionFormResponseModel();
                        if (assets.InspectionForms != null && assets.InspectionForms.form_attributes != null)
                        {
                            var formAttrbutes = assets.InspectionForms.form_attributes;
                            List<CategoryWiseAttributes> categoryWiseAttributes = new List<CategoryWiseAttributes>();

                            var results = formAttrbutes.GroupBy(
                                    p => p.category_id,
                                    (key, g) => new { category_id = key, Attributes = g.ToList() });
                            var list1 = results.ToList();

                            foreach (var item in list1)
                            {
                                CategoryWiseAttributes categoryWiseAttribute = new CategoryWiseAttributes();
                                categoryWiseAttribute.category_id = item.category_id;
                                var categoryObject = _UoW.InspectionFormRepository.GetCategoryByID(item.category_id);
                                if (categoryObject != null)
                                {
                                    categoryWiseAttribute.name = categoryObject.name;
                                    categoryWiseAttribute.spanish_name = PreferLanguageSingleton.Instance.GetLanguageKeyByName(categoryObject.name, (int)Language.spanish).Result;
                                }

                                foreach (var attribute in item.Attributes)
                                {
                                    Issue workorder = _UoW.IssueRepository.GetAttributeIssueByBarcodeId(attribute.attributes_id, assets.asset_id.ToString());
                                    if (workorder != null)
                                    {
                                        attribute.issue = _mapper.Map<IssueViewModel>(workorder);
                                        attribute.workorder = _mapper.Map<IssueViewModel>(workorder);
                                    }
                                }
                                categoryWiseAttribute.form_attributes = item.Attributes;

                                categoryWiseAttributes.Add(categoryWiseAttribute);
                            }
                            assets.InspectionForms.categoryAttributeList = categoryWiseAttributes;
                        }
                        else
                        {
                            // do nothing;
                        }
                    }
                }
                return assetlist;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ListViewModel<MonthlyInspection>> GetAssetInspectionReportMonthly(DateTime start, DateTime end)
        {
            ListViewModel<MonthlyInspection> responseModel = new ListViewModel<MonthlyInspection>();
            try
            {

                var totalasset = await _UoW.InspectionRepository.GetAllInspections(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), start, end);
                if (totalasset.Count > 0)
                {


                    //responseModel.list = (from a in totalasset
                    //              group a by new {
                    //                  a.site_id,
                    //                  a.created_at.Month
                    //              } into b
                    //              select new MonthlyInspection
                    //              {
                    //                  month = b.Key.Month,
                    //                  yard = b.Key.site_id.ToString(),
                    //                  totalinspection = b.Count(),
                    //                  approvedinspection =  totalasset.Count(x => x.status == (int)Status.Approved && x.created_at.Month == b.Key.Month && x.site_id == b.Key.site_id)
                    //              }).ToList();


                    responseModel.list = (from a in totalasset
                                          group a by new
                                          {
                                              a.site_id
                                          } into b
                                          select new MonthlyInspection
                                          {
                                              //month = b.Key.Month,
                                              yard = b.Key.site_id.ToString(),
                                              totalinspection = b.Count(),
                                              approvedinspection = totalasset.Count(x => x.status == (int)Status.Approved && x.site_id == b.Key.site_id)
                                          }).ToList();


                    responseModel.list.ForEach(x =>
                    {
                        var site = _UoW.SiteRepository.GetSiteById(x.yard);
                        if (site != null)
                        {
                            x.yard = site.site_name;
                        }
                    });
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }

        public async Task<ListViewModel<WeeklyInspection>> GetAssetInspectionReportWeekly(DateTime start, DateTime end)
        {
            ListViewModel<WeeklyInspection> responseModel = new ListViewModel<WeeklyInspection>();
            try
            {
                var totalasset = await _UoW.AssetRepository.GetAssetsWithInspection(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (totalasset.Count > 0)
                {
                    responseModel.list = (from a in totalasset
                                          group a by new
                                          {
                                              a.inspectionform_id,
                                              a.site_id
                                          } into b
                                          select new WeeklyInspection
                                          {
                                              //week = start.ToString("dd-MM-yyyy") + " To " + end.ToString("dd-MM-yyyy"),
                                              yard = b.Key.site_id.ToString(),
                                              equipment_type = b.ToList().Select(x => x.InspectionForms.name).FirstOrDefault(),
                                              totalinspection = b.ToList().Select(x => x.Inspection.Where(y => y.created_at >= start && y.created_at < end).Count()).Sum(),
                                              approvedinspection = (b.ToList().Select(x => x.Inspection.Where(y => y.created_at >= start && y.created_at < end && y.status == (int)Status.Approved).Count()).Sum()) == 0 ? 0 :
                                                                   ((double)b.ToList().Select(x => x.Inspection.Where(y => y.created_at >= start && y.created_at < end && y.status == (int)Status.Approved).Count()).Sum() /
                                                                   (double)b.ToList().Select(x => x.Inspection.Where(y => y.created_at >= start && y.created_at < end).Count()).Sum() * 100)
                                          }).ToList();

                    responseModel.list.ForEach(x =>
                    {
                        var site = _UoW.SiteRepository.GetSiteById(x.yard);
                        if (site != null)
                        {
                            x.yard = site.site_name;
                        }
                    });
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }

        public async Task<ListViewModel<AssetWeeklyReport>> GetAssetReportWeekly(DateTime start, DateTime end)
        {
            ListViewModel<AssetWeeklyReport> responseModel = new ListViewModel<AssetWeeklyReport>();
            try
            {
                var totalasset = await _UoW.AssetRepository.GetAssetsWithInspection(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (totalasset.Count > 0)
                {

                    responseModel.list = (from a in totalasset
                                          group a by new
                                          {
                                              a.asset_id
                                          } into b
                                          select new AssetWeeklyReport
                                          {
                                              //week = start.ToString("dd-MM-yyyy") + " To " + end.ToString("dd-MM-yyyy"),
                                              yard = b.Key.asset_id.ToString(),
                                              asset_name = b.Where(x => x.asset_id == b.Key.asset_id).FirstOrDefault().name,
                                              internal_asset_id = b.Where(x => x.asset_id == b.Key.asset_id).FirstOrDefault().internal_asset_id,
                                              begin_hours = b.Select(x => x.Inspection?.Where(y => y.asset_id == b.Key.asset_id && y.created_at >= start && y.created_at < end).OrderByDescending(x => x.created_at)?.LastOrDefault()?.meter_hours.ToString())?.LastOrDefault(),
                                              end_hours = b.Select(x => x.Inspection?.Where(y => y.asset_id == b.Key.asset_id && y.created_at >= start && y.created_at < end).OrderByDescending(x => x.created_at)?.FirstOrDefault()?.meter_hours.ToString())?.FirstOrDefault(),
                                              totalinspection = b.ToList().Select(x => x.Inspection.Where(y => y.created_at >= start && y.created_at < end).Count()).Sum(),
                                              approvedinspection = (b.ToList().Select(x => x.Inspection.Where(y => y.created_at >= start && y.created_at < end && y.status == (int)Status.Approved).Count()).Sum()) == 0 ? 0 :
                                                                   ((double)b.ToList().Select(x => x.Inspection.Where(y => y.created_at >= start && y.created_at < end && y.status == (int)Status.Approved).Count()).Sum() /
                                                                   (double)b.ToList().Select(x => x.Inspection.Where(y => y.created_at >= start && y.created_at < end).Count()).Sum() * 100)
                                          }).ToList();

                    responseModel.list.ForEach(x =>
                    {
                        var asset = _UoW.AssetRepository.GetAssetByIDWithoutStatusCheck(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), x.yard);
                        if (asset != null)
                        {
                            x.yard = asset.Sites.site_name;
                        }
                    });
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }

        public async Task<DashboardOutstandingIssuesResponseModel> DashboardOutstandingIssues()
        {

            try
            {
                //DashboardOutstandingIssuesResponseModel responseModel = new DashboardOutstandingIssuesResponseModel();
                //var dashboardOutstandingIssues = await _UoW.AssetRepository.DashboardOutstandingIssues(userid);
                //if(dashboardOutstandingIssues.Count > 0)
                //{
                //    List<Report> Jsondata = new List<Report>();
                //    dashboardOutstandingIssues.ForEach(x =>
                //    {
                //        Report data = new Report();
                //        data.site_id = x.site_id;
                //        data.created_at = x.created_at;
                //        data.modified_at = x.modified_at;
                //        var assetwise = x.data.GroupBy(y => y.asset_id).ToList();
                //        if(assetwise.Count > 0)
                //        {
                //            List<Asset_Details> datass = new List<Asset_Details>();
                //            //data.asset_id = assetwise.Select(x => x.Key);
                //            //data.data = assetwise.Select(x => x.ToList());
                //            assetwise.ForEach(z => {
                //                Asset_Details datas = new Asset_Details();
                //                var assetlist = z.OrderByDescending(x => x.not_ok_since).ToList();
                //                datas.asset = _mapper.Map<List<ReportJsonDatas>>(assetlist);
                //                z.ToList().ForEach(y =>
                //                {
                //                    datas.created_at = x.created_at;
                //                    datas.modified_at = x.modified_at;
                //                    datas.site_id = y.site_id;
                //                    datas.asset_id = y.asset_id;
                //                    datas.asset_name = y.asset_name;
                //                    datas.site_name = y.site_name;
                //                    datas.internal_asset_id = y.internal_asset_id;
                //                });
                //                datass.Add(datas);
                //            });
                //            data.asset_details = datass.OrderByDescending(x => x.asset.Count()).ToList(); ;
                //            Jsondata.Add(data);
                //        }
                //    });
                //    responseModel.reports = Jsondata;
                //}

                DashboardOutstandingIssuesResponseModel responseModel = new DashboardOutstandingIssuesResponseModel();
                responseModel.reports = new List<Report>();
                List<Issue> workorders = await _UoW.IssueRepository.GetPendingIssues(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());

                var group_workorder = workorders.GroupBy(x => x.Asset.site_id).ToList();

                if (group_workorder.Count > 0)
                {
                    List<Report> Jsondata = new List<Report>();
                    group_workorder.ForEach(x =>
                    {
                        Report data = new Report();
                        data.created_at = DateTime.UtcNow;
                        data.modified_at = DateTime.UtcNow;
                        data.site_id = x.Key;
                        var workorder_groupby_asset = x.ToList().GroupBy(y => y.asset_id).ToList();
                        if (workorder_groupby_asset.Count > 0)
                        {
                            List<Asset_Details> datass = new List<Asset_Details>();
                            workorder_groupby_asset.ForEach(z =>
                            {
                                var workorder_groupby_attributes = z.GroupBy(s => s.attribute_id).ToList();
                                Asset_Details datas = new Asset_Details();
                                var orderby_workorder = z.ToList().OrderByDescending(x => x.created_at).ToList();
                                workorder_groupby_attributes.ForEach(s =>
                                {
                                    var assets = _mapper.Map<List<ReportJsonDatas>>(s.Take(1).ToList());
                                    datas.asset.AddRange(assets);
                                });
                                orderby_workorder.ForEach(x =>
                                {
                                    datas.created_at = DateTime.UtcNow;
                                    datas.modified_at = DateTime.UtcNow;
                                    datas.site_id = x.site_id;
                                    datas.asset_id = x.asset_id;
                                    datas.asset_name = x.Asset.name;
                                    datas.site_name = x.Sites.site_name;
                                    datas.internal_asset_id = x.Asset.internal_asset_id;
                                });
                                datass.Add(datas);
                            });
                            data.asset_details = datass.OrderByDescending(x => x.asset.Count()).ToList();
                        }
                        Jsondata.Add(data);
                    });
                    responseModel.reports = Jsondata;
                }

                //DashboardOutstandingIssuesResponseModel responseModel = new DashboardOutstandingIssuesResponseModel();
                //responseModel.reports = new List<Report>();
                //var dashboardOutstandingIssues = await _UoW.AssetRepository.DashboardOutstandingIssues(userid);
                //if (dashboardOutstandingIssues.Count > 0)
                //{
                //    List<Report> Jsondata = new List<Report>();
                //    dashboardOutstandingIssues.ForEach(x =>
                //    {
                //        Report data = new Report();
                //        data.site_id = x.site_id;
                //        data.created_at = x.created_at;
                //        data.modified_at = x.modified_at;
                //        var assetwise = x.data.GroupBy(y => y.asset_id).ToList();
                //        data.asset_details = new List<Asset_Details>();
                //        if (assetwise.Count > 0)
                //        {
                //            List<Asset_Details> datass = new List<Asset_Details>();
                //            assetwise.ForEach(z => {
                //                Asset_Details datas = new Asset_Details();
                //                var assetlist = z.OrderByDescending(x => x.not_ok_since).ToList();
                //                datas.asset = _mapper.Map<List<ReportJsonDatas>>(assetlist);
                //                z.ToList().ForEach(y =>
                //                {
                //                    datas.created_at = x.created_at;
                //                    datas.modified_at = x.modified_at;
                //                    datas.site_id = y.site_id;
                //                    datas.asset_id = y.asset_id;
                //                    datas.asset_name = y.asset_name;
                //                    datas.site_name = y.site_name;
                //                    datas.internal_asset_id = y.internal_asset_id;
                //                });
                //                datass.Add(datas);
                //            });
                //            data.asset_details = datass.OrderByDescending(x => x.asset.Count()).ToList();
                //        }
                //        Jsondata.Add(data);
                //    });
                //    responseModel.reports = Jsondata;
                //}
                return responseModel;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<ListViewModel<GetLatestMeterHoursReportResponseModel>> GetLatestMeterHoursReport()
        {
            try
            {
                ListViewModel<GetLatestMeterHoursReportResponseModel> reports = new ListViewModel<GetLatestMeterHoursReportResponseModel>();
                var assetwithinspection = _UoW.AssetRepository.GetAllInspections(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), 0, 0);
                if (assetwithinspection.Count > 0)
                {
                    var allasset = assetwithinspection.GroupBy(x => x.asset_id);

                    var inspection = allasset.Select(x => x.OrderByDescending(y => y.created_at).Take(1).ToList()).ToList();

                    inspection.ForEach(x => x.ForEach(y =>
                    {
                        GetLatestMeterHoursReportResponseModel report = new GetLatestMeterHoursReportResponseModel();
                        report.asset_id = y.asset_id;
                        report.latest_inspection_date = y.created_at;
                        report.internal_asset_id = y.Asset.internal_asset_id;
                        report.asset_name = y.Asset.name;
                        report.current_meter_hours = y.Asset.meter_hours.ToString();
                        report.timezone = y.Sites.timezone;
                        reports.list.Add(report);
                    }));
                }
                return reports;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int> UpdateMeterHours(UpdateMeterHoursRequestModel request)
        {
            try
            {
                Asset asset = _UoW.AssetRepository.GetAssetByID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), request.asset_id);
                if (asset != null && asset.asset_id != Guid.Empty)
                {
                    AssetTransactionHistory assetTransactionHistory = new AssetTransactionHistory();
                    assetTransactionHistory.asset_id = asset.asset_id;
                    assetTransactionHistory.created_at = DateTime.UtcNow;
                    assetTransactionHistory.comapny_id = asset.company_id;
                    assetTransactionHistory.site_id = asset.site_id.ToString();
                    assetTransactionHistory.attributeValues = asset.lastinspection_attribute_values;
                    assetTransactionHistory.inspection_form_id = asset.inspectionform_id.ToString();
                    assetTransactionHistory.meter_hours = asset.meter_hours;
                    _UoW.BeginTransaction();
                    bool txnHistoryResult = _UoW.AssetRepository.AddAssetTransactionHistory(assetTransactionHistory);
                    if (txnHistoryResult)
                    {
                        _UoW.SaveChanges();
                        AssetMeterHourHistory assetMeterHourHistory = new AssetMeterHourHistory();
                        assetMeterHourHistory.meter_hours = request.meter_hours;
                        assetMeterHourHistory.updated_at = DateTime.UtcNow;
                        assetMeterHourHistory.site_id = asset.site_id.ToString();
                        assetMeterHourHistory.company_id = asset.company_id;
                        assetMeterHourHistory.asset_id = asset.asset_id;
                        assetMeterHourHistory.status = (int)Status.Active;
                        assetMeterHourHistory.requested_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        bool updateassetmeterhours = _UoW.AssetRepository.AddAssetMeterHoursHistory(assetMeterHourHistory);
                        if (updateassetmeterhours)
                        {
                            _UoW.SaveChanges();
                            asset.meter_hours = request.meter_hours;
                            asset.modified_at = DateTime.UtcNow;
                            asset.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            int updateasset = await _UoW.AssetRepository.Update(asset);
                            if (updateasset > 0)
                            {
                                _UoW.CommitTransaction();
                                return (int)ResponseStatusNumber.Success;
                            }
                            else
                            {
                                _UoW.RollbackTransaction();
                                return (int)ResponseStatusNumber.Error;
                            }
                        }
                        else
                        {
                            _UoW.RollbackTransaction();
                            return (int)ResponseStatusNumber.Error;
                        }
                    }
                    else
                    {
                        _UoW.RollbackTransaction();
                        return (int)ResponseStatusNumber.Error;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _UoW.RollbackTransaction();
                throw e;
            }
        }

        public async Task<SyncDataResponseModel> GetSyncData(string userid)
        {
            try
            {
                SyncDataResponseModel responseModel = new SyncDataResponseModel();
                responseModel.assets = new ListViewModel<AssetDetailsViewModel>();
                responseModel.inspection_forms = new ListViewModel<InspectionFormDataViewModel>();
                responseModel.users = new ListViewModel<GetUserDetailsResponseModel>();
                responseModel.inspections = new ListViewModel<AssetInspectionViewModel>();
                responseModel.issues = new ListViewModel<IssueViewModel>();
                responseModel.workorders = new ListViewModel<IssueViewModel>();
                responseModel.active_assets = new ListViewModel<Guid>();

                if (UpdatedGenericRequestmodel.CurrentUser.device_uuid != null && UpdatedGenericRequestmodel.CurrentUser.device_uuid != Guid.Empty)
                {
                    bool force_to_reset = true;
                    DateTime? sync_time = null;
                    var device_info = _UoW.DeviceRepository.GetDeviceInfoByUUId(UpdatedGenericRequestmodel.CurrentUser.device_uuid);

                    Guid? user_site_id = _UoW.UserRepository.GetUserSite(userid);

                    if (user_site_id == device_info.last_sync_site_id)
                    {
                        force_to_reset = false;
                        sync_time = device_info.last_sync_time;
                    }
                    device_info.last_sync_site_id = user_site_id;
                    device_info.last_sync_time = DateTime.UtcNow;
                    device_info.modified_at = DateTime.UtcNow;
                    device_info.modified_by = userid;
                    if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.app_version))
                    {
                        device_info.app_version = UpdatedGenericRequestmodel.CurrentUser.app_version;
                    }

                    bool update_device_info = await _UoW.BaseGenericRepository<DeviceInfo>().Update(device_info);
                    if (update_device_info)
                    {
                        GetSyncDataResponseModel response = _UoW.AssetRepository.GetSyncData(userid, sync_time);

                        if (response.asset_list > 0)
                        {
                            responseModel.assets.list = _mapper.Map<List<AssetDetailsViewModel>>(response.assets);
                        }

                        if (response.inspection_form_list > 0)
                        {
                            responseModel.inspection_forms.list = _mapper.Map<List<InspectionFormDataViewModel>>(response.inspectionforms);
                        }

                        if (response.user_list > 0)
                        {
                            responseModel.users.list = _mapper.Map<List<GetUserDetailsResponseModel>>(response.users);
                        }

                        if (response.inspections_list > 0)
                        {
                            responseModel.inspections.list = _mapper.Map<List<AssetInspectionViewModel>>(response.inspections);
                        }

                        if (response.issue_list > 0)
                        {
                            responseModel.issues.list = _mapper.Map<List<IssueViewModel>>(response.issues);
                            responseModel.workorders.list = _mapper.Map<List<IssueViewModel>>(response.issues);
                        }

                        if (response.masterdata != null)
                        {
                            responseModel.master_data = _mapper.Map<MasterDataResponseModel>(response.masterdata);
                        }

                        if (response.active_assets != null)
                        {
                            responseModel.active_assets.list = response.active_assets;
                        }

                        foreach (var form in responseModel.inspection_forms.list)
                        {
                            var categoryattributes = form.form_attributes.GroupBy(
                                            p => p.category_id,
                                            (key, g) => new { category_id = key, Attributes = g.ToList() });
                            var list1 = categoryattributes.ToList();
                            List<CategoryWiseAttributes> categoryWiseAttributes = new List<CategoryWiseAttributes>();
                            foreach (var item in list1)
                            {
                                CategoryWiseAttributes categoryWiseAttribute = new CategoryWiseAttributes();
                                categoryWiseAttribute.category_id = item.category_id;
                                string category_name = InspectionFormCategorySingleton.Instance.GetCategoryName(item.category_id);
                                if (category_name != null)
                                {
                                    categoryWiseAttribute.name = category_name;
                                    categoryWiseAttribute.spanish_name = PreferLanguageSingleton.Instance.GetLanguageKeyByName(category_name, (int)Language.spanish).Result;
                                }
                                categoryWiseAttribute.form_attributes = item.Attributes;
                                categoryWiseAttributes.Add(categoryWiseAttribute);
                            }
                            form.categoryAttributeList = categoryWiseAttributes;
                            form.form_attributes = null;
                        }

                        foreach (var form in responseModel.inspections.list)
                        {
                            var categoryattributes = form.attribute_values.GroupBy(
                                            p => p.category_id,
                                            (key, g) => new { category_id = key, Attributes = g.ToList() });
                            var list1 = categoryattributes.ToList();
                            List<CategoryWiseAttributesInsepction> categoryWiseAttributes = new List<CategoryWiseAttributesInsepction>();
                            foreach (var item in list1)
                            {
                                CategoryWiseAttributesInsepction categoryWiseAttribute = new CategoryWiseAttributesInsepction();
                                categoryWiseAttribute.category_id = item.category_id;
                                string category_name = InspectionFormCategorySingleton.Instance.GetCategoryName(item.category_id);
                                if (category_name != null)
                                {
                                    categoryWiseAttribute.name = category_name;
                                    categoryWiseAttribute.category_spanish_name = PreferLanguageSingleton.Instance.GetLanguageKeyByName(category_name, (int)Language.spanish).Result;
                                }
                                categoryWiseAttribute.attribute_values = item.Attributes;
                                categoryWiseAttributes.Add(categoryWiseAttribute);
                            }
                            form.attributes = categoryWiseAttributes;
                            form.attribute_values = null;
                        }

                        responseModel.assets.listsize = response.asset_list;
                        responseModel.inspection_forms.listsize = response.inspection_form_list;
                        responseModel.users.listsize = response.user_list;
                        responseModel.inspections.listsize = response.inspections_list;
                        responseModel.issues.listsize = response.issue_list;
                        responseModel.workorders.listsize = response.issue_list;
                        responseModel.active_assets.listsize = response.active_assets.Count();
                        responseModel.assets.success = true;
                        responseModel.inspection_forms.success = true;
                        responseModel.users.success = true;
                        responseModel.inspections.success = true;
                        responseModel.issues.success = true;
                        responseModel.workorders.success = true;
                        responseModel.master_data.force_to_reset = force_to_reset;
                    }
                    else
                    {
                        _logger.LogError("Erro In Update Device Info");
                    }
                }
                else
                {
                    _logger.LogError("Not Found Device UUID");
                }
                return responseModel;
            }
            catch
            {
                throw;
            }
        }

        public ListViewModel<AssetInspectionReportResponseModel> GetAssetInspectionReport(string internal_asset_id, int pagesize, int pageindex)
        {
            try
            {
                ListViewModel<AssetInspectionReportResponseModel> response = new ListViewModel<AssetInspectionReportResponseModel>();

                AssetInspectionListResponseModel reports = _UoW.AssetRepository.GetAssetInspectionReport(internal_asset_id, pagesize, pageindex);

                if (reports.AssetInspectionReport.Count() > 0)
                {
                    response.list = _mapper.Map<List<AssetInspectionReportResponseModel>>(reports.AssetInspectionReport);
                }
                response.listsize = reports.list_size;
                response.pageSize = pagesize;
                response.pageIndex = pageindex;
                return response;
            }
            catch { throw; }
        }

        public async Task<AssetInspectionReportResponseModel> GenerateAssetInspectionReport(AssetInspectionReportRequestModel requestModel, DateTime from_date, DateTime to_date, string aws_access_key, string aws_secret_key)
        {
            try
            {
                AssetInspectionReportResponseModel response = new AssetInspectionReportResponseModel();

                Asset asset = _UoW.AssetRepository.GetAssetByInternalID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), requestModel.internal_asset_id);

                if (asset != null && asset.asset_id != Guid.Empty)
                {
                    AssetInspectionReport asset_report = _UoW.AssetRepository.GetAssetInspectionReport(asset.asset_id, from_date, to_date);
                    if (asset_report != null && asset_report.report_id != Guid.Empty)
                    {
                        response.success = (int)ResponseStatusNumber.AlreadyExists;
                    }
                    //response.asset = _mapper.Map<AssetViewModel>(asset);
                    //if (inspections.Inspection.Count() > 0)
                    //{
                    //    response.inspections.list = _mapper.Map<List<AssetInspectionViewModel>>(inspections.Inspection);

                    //    response.inspections.list.ForEach(x =>
                    //    {
                    //        x.manager_name = _UoW.UserRepository.GetUserNameByID(x.manager_id);

                    //        var categoryattributes = x.attribute_values.GroupBy(
                    //                    p => p.category_id,
                    //                    (key, g) => new { category_id = key, Attributes = g.ToList() });
                    //        var list1 = categoryattributes.ToList();
                    //        List<CategoryWiseAttributesInsepction> categoryWiseAttributes = new List<CategoryWiseAttributesInsepction>();
                    //        foreach (var item in list1)
                    //        {
                    //            CategoryWiseAttributesInsepction categoryWiseAttribute = new CategoryWiseAttributesInsepction();
                    //            categoryWiseAttribute.category_id = item.category_id;
                    //            string category_name = InspectionFormCategorySingleton.Instance.GetCategoryName(item.category_id);
                    //            if (category_name != null)
                    //            {
                    //                categoryWiseAttribute.name = category_name;
                    //                categoryWiseAttribute.category_spanish_name = PreferLanguageSingleton.Instance.GetLanguageKeyByName(category_name, (int)Language.spanish).Result;
                    //            }
                    //            categoryWiseAttribute.attribute_values = item.Attributes;
                    //            categoryWiseAttributes.Add(categoryWiseAttribute);
                    //        }
                    //        x.attributes = categoryWiseAttributes;
                    //        x.attribute_values = null;
                    //    });
                    //}
                    else
                    {
                        InspectionListResponseModel inspections = _UoW.InspectionRepository.GetInspections(requestModel.internal_asset_id, from_date, to_date);

                        if (inspections.Inspection.Count() > 0)
                        {
                            AssetInspectionReport report = new AssetInspectionReport();
                            report.asset_id = asset.asset_id;
                            report.from_date = from_date;
                            report.to_date = to_date;
                            report.status = (int)Status.ReportInProgress;
                            report.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            report.created_at = DateTime.UtcNow;
                            report.modified_at = DateTime.UtcNow;

                            bool report_added = await _UoW.BaseGenericRepository<AssetInspectionReport>().Update(report);
                            if (report_added)
                            {
                                response = _mapper.Map<AssetInspectionReportResponseModel>(report);
                                if (response.status == (int)Status.ReportInProgress)
                                {
                                    response.status_name = Status.ReportInProgress.ToString();
                                }
                                else if (response.status == (int)Status.ReportCompleted)
                                {
                                    response.status_name = Status.ReportCompleted.ToString();
                                }
                                else if (response.status == (int)Status.ReportFailed)
                                {
                                    response.status_name = Status.ReportFailed.ToString();
                                }
                                response.success = (int)ResponseStatusNumber.Success;
                                // Call Lamda

                                AssetInspectionReportLamda.GenerateReport(aws_access_key, aws_secret_key, report.report_id, _logger);

                            }
                            else
                            {
                                response.success = (int)ResponseStatusNumber.Error;
                            }
                        }
                        else
                        {
                            response.success = (int)ResponseStatusNumber.NotFoundInspectionForReport;
                            response.message = "No inspections performed for " + asset.name + " in between " + requestModel.from_date + " and " + requestModel.to_date;
                        }
                    }
                }
                else
                {
                    response.success = (int)ResponseStatusNumber.NotFound;
                }
                return response;
            }
            catch { throw; }
        }

        public async Task<ListViewModel<InspectionResponseModel>> GetAssetInspectionForReportView(AssetInspectionReportRequestModel requestModel)
        {
            try
            {
                ListViewModel<InspectionResponseModel> response = new ListViewModel<InspectionResponseModel>();

                Asset asset = _UoW.AssetRepository.GetAssetByInternalID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), requestModel.internal_asset_id);

                if (asset != null && asset.asset_id != Guid.Empty)
                {
                    var Inspections = asset.Inspection.Where(x => x.Issues?.Count > 0 || x.is_comment_important == true).ToList();
                    if (Inspections?.Count > 0)
                    {
                        foreach (var item in Inspections)
                        {
                            var inspection = _UoW.AssetRepository.GetInspectionsByINSId(item.inspection_id.ToString());
                            if (inspection != null)
                            {
                                InspectionResponseModel inspectionResponseModel = _mapper.Map<InspectionResponseModel>(inspection);
                                if (!string.IsNullOrEmpty(inspection.manager_id))
                                {
                                    inspectionResponseModel.manager_name = _UoW.UserRepository.GetUserNameByID(inspection.manager_id);
                                }
                                var categoryattributes = inspectionResponseModel.attribute_values.GroupBy(
                                                p => p.category_id,
                                                (key, g) => new { category_id = key, Attributes = g.ToList() });
                                var list1 = categoryattributes.ToList();
                                List<CategoryWiseAttributesInsepction> categoryWiseAttributes = new List<CategoryWiseAttributesInsepction>();
                                foreach (var listitem in list1)
                                {
                                    CategoryWiseAttributesInsepction categoryWiseAttribute = new CategoryWiseAttributesInsepction();
                                    categoryWiseAttribute.category_id = listitem.category_id;
                                    string category_name = InspectionFormCategorySingleton.Instance.GetCategoryName(listitem.category_id);
                                    if (category_name != null)
                                    {
                                        categoryWiseAttribute.name = category_name;
                                        categoryWiseAttribute.category_spanish_name = PreferLanguageSingleton.Instance.GetLanguageKeyByName(category_name, (int)Language.spanish).Result;
                                    }
                                    var newnotokAttr = listitem.Attributes.Where(x => item.Issues.Select(y => y.attribute_id).ToList().Contains(x.id)).ToList();
                                    if (newnotokAttr != null)
                                    {
                                        categoryWiseAttribute.attribute_values = newnotokAttr.Where(x => x.value?.ToLower() == "not ok").ToList();
                                        categoryWiseAttributes.Add(categoryWiseAttribute);
                                    }
                                    else
                                    {
                                        categoryWiseAttribute.attribute_values = listitem.Attributes.Where(x => x.value?.ToLower() == "not ok").ToList();
                                        categoryWiseAttributes.Add(categoryWiseAttribute);
                                    }
                                }
                                inspectionResponseModel.attributes = categoryWiseAttributes;
                                inspectionResponseModel.attribute_values = null;
                                response.list.Add(inspectionResponseModel);
                            }
                            else
                            {
                                response.result = (int)ResponseStatusNumber.NotFoundInspectionForReport;
                            }
                        }
                        response.listsize = response.list?.Count > 0 ? response.list.Count : 0;
                        if (requestModel.pageindex > 0 && requestModel.pagesize > 0)
                        {
                            response.list = response.list.OrderByDescending(x => x.datetime_requested).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                        }
                        response.result = (int)ResponseStatusNumber.Success;
                        response.success = true;
                    }
                    else
                    {
                        response.result = (int)ResponseStatusNumber.NotFoundInspectionForReport;
                    }
                }
                else
                {
                    response.result = (int)ResponseStatusNumber.NotFound;
                }
                //var workorders = asset.WorkOrders;
                //if (workorders?.Count > 0)
                //{
                //    var WorkOrdersByInspections = workorders.GroupBy(x => x.inspection_id).ToList();
                //    if (WorkOrdersByInspections?.Count > 0)
                //    {
                //        foreach(var item in WorkOrdersByInspections)
                //        {
                //            InspectionResponseModel inspectionResponseModel = new InspectionResponseModel();
                //            var inspection = _UoW.AssetRepository.GetInspectionsByINSId(item.Key.ToString());
                //            if (inspection != null)
                //            {
                //                inspectionResponseModel = _mapper.Map<InspectionResponseModel>(inspection);
                //                if (!string.IsNullOrEmpty(inspection.manager_id))
                //                {
                //                    inspectionResponseModel.manager_name = _UoW.UserRepository.GetUserNameByID(inspection.manager_id);
                //                }
                //                List<InspectionAttributesJsonObjectViewModel> assetsValueJsonObjects = new List<InspectionAttributesJsonObjectViewModel>();
                //                foreach(var workorder in item)
                //                {
                //                    // get the attribute name by attribute id
                //                    var attrName = workorder.Attributes.name;
                //                    InspectionAttributesJsonObjectViewModel attribute = new InspectionAttributesJsonObjectViewModel();
                //                    attribute.category_id = workorder.Attributes.category_id;
                //                    attribute.id = workorder.Attributes.attributes_id;
                //                    attribute.name = workorder.Attributes.name;
                //                    attribute.value = "Not Ok";
                //                    assetsValueJsonObjects.Add(attribute);
                //                }
                //                inspectionResponseModel.attribute_values = assetsValueJsonObjects;
                //                var categoryattributes = inspectionResponseModel.attribute_values.GroupBy(
                //                        p => p.category_id,
                //                        (key, g) => new { category_id = key, Attributes = g.ToList() });
                //                var list1 = categoryattributes.ToList();
                //                List<CategoryWiseAttributesInsepction> categoryWiseAttributes = new List<CategoryWiseAttributesInsepction>();
                //                foreach (var listitem in list1)
                //                {
                //                    CategoryWiseAttributesInsepction categoryWiseAttribute = new CategoryWiseAttributesInsepction();
                //                    categoryWiseAttribute.category_id = listitem.category_id;
                //                    string category_name = InspectionFormCategorySingleton.Instance.GetCategoryName(listitem.category_id);
                //                    if (category_name != null)
                //                    {
                //                        categoryWiseAttribute.name = category_name;
                //                        categoryWiseAttribute.category_spanish_name = PreferLanguageSingleton.Instance.GetLanguageKeyByName(category_name, (int)Language.spanish).Result;
                //                    }
                //                    categoryWiseAttribute.attribute_values = listitem.Attributes.Where(x => x.value?.ToLower() == "not ok").ToList();
                //                    categoryWiseAttributes.Add(categoryWiseAttribute);
                //                }
                //                inspectionResponseModel.attributes = categoryWiseAttributes;
                //                inspectionResponseModel.attribute_values = null;
                //                response.list.Add(inspectionResponseModel);
                //            }
                //        }
                //        response.listsize = response.list?.Count > 0 ? response.list.Count : 0;
                //        if (requestModel.pageindex > 0 && requestModel.pagesize > 0)
                //        {
                //            response.list = response.list.OrderByDescending(x => x.datetime_requested).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                //        }
                //        response.result = (int)ResponseStatusNumber.Success;
                //        response.success = true;
                //    }
                //    else
                //    {
                //        response.result = (int)ResponseStatusNumber.NotFoundInspectionForReport;
                //    }
                //}
                //else
                //{
                //    response.result = (int)ResponseStatusNumber.NotFoundInspectionForReport;
                //}
                response.pageSize = requestModel.pagesize;
                response.pageIndex = requestModel.pageindex;
                return response;
            }
            catch (Exception e) { throw e; }
        }

        public async Task<List<AssetInspectionReportResponseModel>> GetReportStatus(ReportStatusRequestModel requestModel)
        {
            try
            {
                List<AssetInspectionReportResponseModel> responseModel = new List<AssetInspectionReportResponseModel>();
                List<AssetInspectionReport> reports = _UoW.AssetRepository.GetAssetInspectionReportByID(requestModel.reports_id);
                if (reports != null && reports.Count() > 0)
                {
                    reports.ForEach(x =>
                    {
                        if (x.status == (int)Status.ReportInProgress && x.created_at.Value.AddMinutes(2) < DateTime.UtcNow)
                        {
                            x.status = (int)Status.ReportFailed;
                            x.modified_at = DateTime.UtcNow;
                        }
                    });

                    bool report_added = _UoW.BaseGenericRepository<AssetInspectionReport>().UpdateList(reports);
                    if (report_added)
                    {
                        _logger.LogError("Generation Failed Report Timeout");
                    }
                    else
                    {
                        _logger.LogError("Error In Update Generation Failed Report Timeout");
                    }
                    reports = _UoW.AssetRepository.GetAssetInspectionReportByID(requestModel.reports_id);
                    responseModel = _mapper.Map<List<AssetInspectionReportResponseModel>>(reports);
                }
                return responseModel;
            }
            catch { throw; }
        }

        public List<AssetListResponseModel> GetAllAssetsList()
        {
            try
            {
                List<AssetListResponseModel> responseModels = new List<AssetListResponseModel>();

                List<Asset> assets = _UoW.AssetRepository.GetAllAssets(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), 0, 0, 0);

                if (assets.Count() > 0)
                {
                    responseModels = _mapper.Map<List<AssetListResponseModel>>(assets);
                }

                return responseModels;
            }
            catch { throw; }
        }

        public List<string> GetAllAssetsModelsList()
        {
            try
            {
                List<string> responseModels = new List<string>();

                List<Asset> assets = _UoW.AssetRepository.GetAllAssets(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), 0, 0, 0);

                if (assets.Count() > 0)
                {
                    responseModels = assets.Select(x => x.model_name).Distinct().ToList();
                }

                return responseModels;
            }
            catch { throw; }
        }

        public List<string> GetAllAssetsModelYearsList()
        {
            try
            {
                List<string> responseModels = new List<string>();

                List<Asset> assets = _UoW.AssetRepository.GetAllAssets(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), 0, 0, 0);

                if (assets.Count() > 0)
                {
                    responseModels = assets.Select(x => x.model_year).Distinct().ToList();
                }

                return responseModels;
            }
            catch { throw; }
        }

        public async Task<int> UpdateAssetStatus(UpdateAssetStatusRequestModel requestModel)
        {
            int response = (int)ResponseStatusNumber.Error;
            try
            {
                if (requestModel.status == (int)Status.AssetActive || requestModel.status == (int)Status.AssetDeactive || requestModel.status == (int)Status.Disposed)
                {
                    Asset asset = _UoW.AssetRepository.GetAssetByAssetID(requestModel.asset_id);
                    if (asset != null && asset.asset_id != null && asset.asset_id != Guid.Empty)
                    {
                        _UoW.BeginTransaction();
                        asset.status = requestModel.status;
                        if (asset.status == (int)Status.AssetDeactive || asset.status == (int)Status.Disposed)
                        {
                            await MarkAssetStatusAsInActiveDispose(Guid.Parse(requestModel.asset_id));
                        }
                        asset.modified_at = DateTime.UtcNow;
                        asset.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        bool result = await _UoW.BaseGenericRepository<Asset>().Update(asset);
                        if (result)
                        {
                            _UoW.CommitTransaction();
                            response = (int)ResponseStatusNumber.Success;
                        }
                        else
                        {
                            _UoW.RollbackTransaction();
                            response = (int)ResponseStatusNumber.Error;
                        }
                    }
                    else
                    {
                        response = (int)ResponseStatusNumber.NotFound;
                    }
                }
                else
                {
                    response = (int)ResponseStatusNumber.InvalidStatus;
                }
            }
            catch
            {
                _UoW.RollbackTransaction();
                throw;
            }
            return response;
        }

        public async Task<AssetsResponseModel> InsertUpdateAssetDetails(AssetRequestModel assetRequest)
        {
            AssetsResponseModel assetResponse = new AssetsResponseModel();
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    if (assetRequest.asset_id != null && assetRequest.asset_id != Guid.Empty)
                    {
                        //var assDetails = _UoW.AssetRepository.GetAssetsByInternalAssetID(assetRequest.internal_asset_id);
                        var assetDetails = _UoW.AssetRepository.GetAssetByAssetID(assetRequest.asset_id.ToString());
                        if (assetDetails != null)
                        {
                            assetDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            assetDetails.modified_at = DateTime.UtcNow;
                            assetDetails.site_id = new Guid(assetRequest.site_id);
                            assetDetails.status = Int32.Parse(assetRequest.status);
                            assetDetails.inspectionform_id = new Guid(assetRequest.inspectionform_id);
                            assetDetails.meter_hours = assetRequest.meter_hours;

                            result = _UoW.AssetRepository.Update(assetDetails).Result;

                            if (result > 0)
                            {
                                _dbtransaction.Commit();
                                assetResponse = _mapper.Map<AssetsResponseModel>(assetDetails);
                            }
                            else
                            {
                                _dbtransaction.Rollback();
                            }
                        }
                        assetResponse.response_status = result;
                    }
                    else
                    {
                        var assDetails = _UoW.AssetRepository.GetAssetsByInternalAssetID(assetRequest.internal_asset_id);
                        if (assDetails != null)
                        {
                            assetResponse.response_status = (int)ResponseStatusNumber.AlreadyExists;
                        }
                        else
                        {
                            assetRequest.created_at = DateTime.UtcNow;
                            //assetmodel.modified_at = DateTime.UtcNow;
                            //assetmodel.name = assetRequest.name;
                            //assetmodel.parent = assetRequest.parent;
                            //assetmodel.children = assetRequest.children;
                            //assetmodel.asset_type = assetRequest.asset_type;
                            //assetmodel.product_name = assetRequest.product_name;
                            //assetmodel.model_name = assetRequest.model_name;
                            //assetmodel.asset_serial_number = assetRequest.asset_serial_number;
                            //assetmodel.model_year = assetRequest.model_year;
                            //assetmodel.company_id = assetRequest.company_id;
                            //assetmodel.site_id = new Guid(assetRequest.site_id);
                            //assetmodel.status = Int32.Parse(assetRequest.status);
                            //assetmodel.current_stage = assetRequest.current_stage;
                            //assetmodel.inspectionform_id = new Guid(assetRequest.inspectionform_id);

                            var asset = _mapper.Map<Asset>(assetRequest);
                            result = await _UoW.AssetRepository.Insert(asset);

                            if (result > 0)
                            {
                                _UoW.SaveChanges();
                                _UoW.CommitTransaction();
                                assetResponse = _mapper.Map<AssetsResponseModel>(asset);
                                assetResponse.response_status = result;
                            }
                            else
                            {
                                assetResponse.response_status = result;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _UoW.RollbackTransaction();
                    assetResponse.response_status = (int)ResponseStatusNumber.Error;
                }
            }
            return assetResponse;
        }

        public async Task<AssetTypeResponseModel> AddUpdateAssetType(AssetTypeRequestModel requestModel)
        {
            AssetTypeResponseModel responseModel = new AssetTypeResponseModel();
            bool result = false;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    if (requestModel.asset_type_id != null && requestModel.asset_type_id != 0)
                    {
                        var assettypeDetails = await _UoW.AssetRepository.GetAssetTypeById(requestModel.asset_type_id);
                        {
                            assettypeDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            assettypeDetails.modified_at = DateTime.UtcNow;
                            assettypeDetails.name = requestModel.name;
                            assettypeDetails.isArchive = requestModel.isArchive;
                            result = _UoW.BaseGenericRepository<AssetType>().Update(assettypeDetails).Result;
                            if (result)
                            {
                                responseModel = _mapper.Map<AssetTypeResponseModel>(assettypeDetails);
                                _dbtransaction.Commit();
                            }
                            else
                            {
                                _dbtransaction.Rollback();
                            }
                        }
                        responseModel.response_status = Convert.ToInt32(result);
                    }
                    else
                    {
                        requestModel.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        requestModel.created_at = DateTime.UtcNow;
                        requestModel.isArchive = false;
                        var assetType = _mapper.Map<AssetType>(requestModel);
                        result = await _UoW.BaseGenericRepository<AssetType>().Insert(assetType);
                        if (result)
                        {
                            responseModel = _mapper.Map<AssetTypeResponseModel>(assetType);
                            responseModel.response_status = (int)ResponseStatusNumber.Success;
                            _UoW.SaveChanges();
                            _dbtransaction.Commit();
                        }
                        else
                        {
                            responseModel.response_status = (int)ResponseStatusNumber.Error;
                        }
                    }

                }
                catch (Exception ex)
                {
                    _dbtransaction.Rollback();
                    responseModel.response_status = (int)ResponseStatusNumber.Error;
                }

            }
            return responseModel;
        }

        public async Task<ListViewModel<AssetTypeResponseModel>> GetAllAssetTypes(int pageindex, int pagesize, string searchstring)
        {
            ListViewModel<AssetTypeResponseModel> typeResponse = new ListViewModel<AssetTypeResponseModel>();
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var typeDetails = await _UoW.AssetRepository.GetAllAssetTypes(searchstring);
                    if (typeDetails?.Count > 0)
                    {
                        if (pageindex > 0 && pagesize > 0)
                        {
                            typeResponse.listsize = typeDetails.Count;
                            typeDetails = typeDetails.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                        }
                        typeResponse.list = _mapper.Map<List<AssetTypeResponseModel>>(typeDetails);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return typeResponse;
        }

        public async Task<AssetTypeResponseModel> GetAssetTypeByID(int id)
        {
            AssetTypeResponseModel typeResponse = new AssetTypeResponseModel();
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var typeDetails = await _UoW.AssetRepository.GetAssetTypeById(id);
                    if (typeDetails?.asset_type_id != null)
                    {
                        typeResponse = _mapper.Map<AssetTypeResponseModel>(typeDetails);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return typeResponse;
        }

        public async Task<ListViewModel<AssetActivityLogsViewModel>> GetActivityLogs(string userid, int pagesize, int pageindex, string asset_id, int filter_type)
        {
            ListViewModel<AssetActivityLogsViewModel> responseModel = new ListViewModel<AssetActivityLogsViewModel>();
            try
            {
                var response = await _UoW.AssetRepository.GetActivityLogs(asset_id, userid, filter_type);
                if (response?.Count > 0)
                {
                    int totalActivity = response.Count();
                    if (pageindex > 0 && pagesize > 0)
                    {
                        response = response.OrderByDescending(x => x.created_at).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    }
                    else
                    {
                        response = response.OrderByDescending(g => g.created_at).ToList();
                    }
                    responseModel.list = _mapper.Map<List<AssetActivityLogsViewModel>>(response);
                    foreach (var activity in responseModel.list)
                    {
                        if (!String.IsNullOrEmpty(activity.updated_by))
                        {
                            var user = await _UoW.UserRepository.GetUserByID(activity.updated_by);
                            activity.updated_by_name = user?.firstname + " " + user?.lastname;
                        }
                    }
                    responseModel.listsize = totalActivity;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Exception to get Notifications: ", e.Message);
                throw;
            }

            return responseModel;
        }

        public ListViewModel<AssetsResponseModel> GetAllHierarchyAssets(FilterAssetsRequestModel requestModel)
        {
            ListViewModel<AssetsResponseModel> responseModel = new ListViewModel<AssetsResponseModel>();
            try
            {
                var response = _UoW.AssetRepository.GetParentAssets(requestModel);
                if (response?.list?.Count > 0)
                {
                    responseModel.list = _mapper.Map<List<AssetsResponseModel>>(response.list);
                    if (responseModel.list?.Count > 0)
                    {
                        responseModel.list.ForEach(x =>
                        {
                            //var children = _UoW.AssetRepository.GetChildAssets(x.internal_asset_id.ToString());
                            //x.nodes = _mapper.Map<List<AssetsResponseModel>>(children.list);
                            var is_children_availabel = response.list.Where(s => s.asset_id == x.asset_id).Select(q => q.children).FirstOrDefault();
                            if (!String.IsNullOrEmpty(is_children_availabel))
                            {
                                x.is_child_available = true;
                            }
                            else
                            {
                                x.is_child_available = false;
                            }
                        });

                    }
                    responseModel.listsize = response.listsize;
                    responseModel.pageIndex = requestModel.pageindex;
                    responseModel.pageSize = requestModel.pagesize;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }

        public List<GetAllHierarchyAssetsResponseModel> GetAllRawHierarchyAssets()
        {
            List<GetAllHierarchyAssetsResponseModel> responseModel = new List<GetAllHierarchyAssetsResponseModel>();
            try
            {
                var response = _UoW.AssetRepository.GetAllRawHierarchyAssets();
                if (response?.Count > 0)
                {
                    responseModel = _mapper.Map<List<GetAllHierarchyAssetsResponseModel>>(response);

                    responseModel.ForEach(x =>
                    {
                        if (!String.IsNullOrEmpty(x.children))
                        {
                            x.is_child_available = true;
                        }
                    });


                    //  responseModel.list = _mapper.Map<List<GetAllHierarchyAssetsResponseModel>>(response);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }
        public List<GetAllAssetsForClusterResponsemodel> GetAllAssetsForCluster(string wo_id)
        {
            List<GetAllAssetsForClusterResponsemodel> responseModel = new List<GetAllAssetsForClusterResponsemodel>();
            try
            {
                var asset_response = _UoW.AssetRepository.GetAllAssetsForCluster(wo_id);
                List<AssetParentHierarchyMapping> asset_parent_mapping = new List<AssetParentHierarchyMapping>();
                foreach (var item in asset_response)
                {
                    item.AssetParentHierarchyMapping = item.AssetParentHierarchyMapping.Where(x => !x.is_deleted).ToList();
                    asset_parent_mapping.AddRange(item.AssetParentHierarchyMapping);
                }
                var response = asset_response.Where(x => x.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent).ToList();
                if (response?.Count > 0)
                {
                    response.ForEach(x =>
                    {
                        x.AssetChildrenHierarchyMapping = x.AssetChildrenHierarchyMapping.Where(x => !x.is_deleted).ToList();
                    });
                    responseModel = _mapper.Map<List<GetAllAssetsForClusterResponsemodel>>(response);
                    responseModel.ForEach(x =>
                    {

                        /// add OCP and OCP main for fedby
                        /// 
                        x.children_list?.ForEach(y =>
                        {
                            var get_parent = asset_parent_mapping.Where(z => z.asset_id == y.children_asset_id && z.parent_asset_id == x.asset_id).FirstOrDefault();
                            if (get_parent != null)
                            {
                                y.ocp_main_subcomponent_asset = get_parent.fed_by_via_subcomponant_asset_id;
                                y.ocp_subcomponent_asset = get_parent.via_subcomponent_asset_id;
                            }
                        });
                        x.subcomponent_list?.ForEach(y =>
                        {
                            var sub_asset = asset_response.Where(z => z.asset_id == y.asset_id).FirstOrDefault();
                            if (sub_asset != null)
                            {
                                y.asset_name = sub_asset.name;
                                y.asset_operating_condition_state = sub_asset.asset_operating_condition_state;
                            }
                        });
                    });

                }
                // get asset hierarchy from wo 
                if (!String.IsNullOrEmpty(wo_id))
                {
                    // add all top level WOLINES in response 
                    var get_all_toplevel_wolines = _UoW.AssetRepository.GetallToplevelWolines(Guid.Parse(wo_id));
                    foreach (var item in get_all_toplevel_wolines)
                    {
                        GetAllAssetsForClusterResponsemodel GetAllAssetsForClusterResponsemodel = new GetAllAssetsForClusterResponsemodel();
                        GetAllAssetsForClusterResponsemodel.asset_id = item.woonboardingassets_id;
                        GetAllAssetsForClusterResponsemodel.is_asset_temp = true;
                        GetAllAssetsForClusterResponsemodel.asset_operating_condition_state = item.TempAsset.asset_operating_condition_state;
                        GetAllAssetsForClusterResponsemodel.name = item.TempAsset.asset_name;
                        GetAllAssetsForClusterResponsemodel.internal_asset_id = item.TempAsset.QR_code;
                        GetAllAssetsForClusterResponsemodel.criticality_index_type = item.TempAsset.criticality_index_type;
                        GetAllAssetsForClusterResponsemodel.asset_class_name = item.TempAsset.InspectionTemplateAssetClass != null ? item.TempAsset.InspectionTemplateAssetClass.asset_class_name : null;
                        GetAllAssetsForClusterResponsemodel.asset_class_code = item.TempAsset.InspectionTemplateAssetClass != null ? item.TempAsset.InspectionTemplateAssetClass.asset_class_code : null;
                        if (item.TempAsset.InspectionTemplateAssetClass != null && item.TempAsset.InspectionTemplateAssetClass.FormIOType != null)
                            GetAllAssetsForClusterResponsemodel.asset_class_type = item.TempAsset.InspectionTemplateAssetClass != null ? item.TempAsset.InspectionTemplateAssetClass.FormIOType.form_type_name : null;
                        //GetAllAssetsForClusterResponsemodel.asset_class_code = item.TempAsset.InspectionTemplateAssetClass != null ? item.TempAsset.InspectionTemplateAssetClass.asset_class_code : null;
                        GetAllAssetsForClusterResponsemodel.building = item.TempAsset.TempFormIOBuildings != null ? item.TempAsset.TempFormIOBuildings.temp_formio_building_name : null;
                        GetAllAssetsForClusterResponsemodel.floor = item.TempAsset.TempFormIOFloors != null ? item.TempAsset.TempFormIOFloors.temp_formio_floor_name : null;
                        GetAllAssetsForClusterResponsemodel.room = item.TempAsset.TempFormIORooms != null ? item.TempAsset.TempFormIORooms.temp_formio_room_name : null;
                        GetAllAssetsForClusterResponsemodel.section = item.TempAsset.TempFormIOSections != null ? item.TempAsset.TempFormIOSections.temp_formio_section_name : null;
                        GetAllAssetsForClusterResponsemodel.temp_asset_woline_status = item.status;

                        if (item.WOlineSubLevelcomponentMapping != null && item.WOlineSubLevelcomponentMapping.Count > 0)
                        {
                            GetAllAssetsForClusterResponsemodel.subcomponent_list = new List<ClusterSubcomponents>();
                            var sublevel_list = item.WOlineSubLevelcomponentMapping.Where(x => !x.is_deleted && x.is_sublevelcomponent_from_ob_wo).Select(x => x.sublevelcomponent_asset_id).ToList();
                            var get_sublevel_wolnes = _UoW.AssetRepository.GetSublevelWOlines(sublevel_list);

                            foreach (var sublevel in get_sublevel_wolnes)
                            {
                                ClusterSubcomponents ClusterSubcomponents = new ClusterSubcomponents();
                                ClusterSubcomponents.asset_name = sublevel.asset_name;
                                ClusterSubcomponents.asset_id = sublevel.woonboardingassets_id;
                                ClusterSubcomponents.asset_operating_condition_state = sublevel.asset_operating_condition_state;
                                ClusterSubcomponents.is_asset_temp = true;
                                GetAllAssetsForClusterResponsemodel.subcomponent_list.Add(ClusterSubcomponents);
                            }
                        }
                        responseModel.Add(GetAllAssetsForClusterResponsemodel);
                    }


                    // check if any existing asset is assigned to WO then change asset_id to woline_id
                    var get_existing_asset_woline1 = _UoW.AssetRepository.GetExistingAssetWOline(Guid.Parse(wo_id));
                    foreach (var item in get_existing_asset_woline1)
                    {
                        // change asset id of top level asset
                        if (responseModel.Where(x => x.asset_id == item.asset_id).FirstOrDefault() != null)
                        {
                            //update childern as per woline if any existing is removed or not
                            var get_inactive_fedby = item.WOOBAssetFedByMapping.Where(x => x.is_deleted && !x.is_parent_from_ob_wo).ToList();
                            foreach (var inactive_fedby in get_inactive_fedby)
                            {
                                var fedby_asset = responseModel.Where(x => x.asset_id == inactive_fedby.parent_asset_id).FirstOrDefault();
                                if (fedby_asset != null)
                                {
                                    if (responseModel.Where(x => x.asset_id == inactive_fedby.parent_asset_id).FirstOrDefault()
                                        .children_list.Where(x => x.children_asset_id == item.asset_id || x.children_asset_id == item.woonboardingassets_id).FirstOrDefault() != null)
                                    {
                                        responseModel.Where(x => x.asset_id == inactive_fedby.parent_asset_id).FirstOrDefault()
                                        .children_list.Where(x => x.children_asset_id == item.asset_id || x.children_asset_id == item.woonboardingassets_id).FirstOrDefault().is_deleted = true;
                                    }
                                }
                            }

                            //if (item.asset_name == "SATS1")
                            //{
                            // if existing in main and not exist in obwoline then remove from response also.
                            var woline_fedby = item.WOOBAssetFedByMapping.Where(x => !x.is_deleted).Select(x => x.parent_asset_id).ToList();

                            var extra_fed_by = responseModel.Where(x => ((x.temprory_asset_id != null && !woline_fedby.Contains(x.temprory_asset_id.Value)) && x.children_list != null &&
                                (x.children_list.Select(x => x.children_asset_id).Contains(item.asset_id) || x.children_list.Select(x => x.children_asset_id).Contains(item.woonboardingassets_id)))).ToList();

                            foreach (var fedby in extra_fed_by)
                            {
                                if (responseModel.Where(x => x.asset_id == fedby.asset_id).FirstOrDefault().children_list != null)
                                {
                                    responseModel.Where(x => x.asset_id == fedby.asset_id).FirstOrDefault().children_list.Where(x => x.children_asset_id == item.asset_id || x.children_asset_id == item.woonboardingassets_id).FirstOrDefault().is_deleted = true;
                                }
                            }
                            //}


                            responseModel.Where(x => x.asset_id == item.asset_id).FirstOrDefault().name = item.asset_name;
                            responseModel.Where(x => x.asset_id == item.asset_id).FirstOrDefault().is_main_asset_assigned = true;
                            responseModel.Where(x => x.asset_id == item.asset_id).FirstOrDefault().temp_asset_woline_status = item.status;
                            responseModel.Where(x => x.asset_id == item.asset_id).FirstOrDefault().temprory_asset_id = item.asset_id;
                            responseModel.Where(x => x.asset_id == item.asset_id).FirstOrDefault().asset_id = item.woonboardingassets_id;

                        }
                    }

                    foreach (var item in responseModel)
                    {
                        if (item.children_list != null && item.children_list.Count > 0)
                        {
                            item.children_list = item.children_list.Where(x => !x.is_deleted).ToList();
                        }
                    }
                    // add children mapping in to list 
                    var get_fedby_mapping = _UoW.AssetRepository.GetWOlinefedbbyMapping(Guid.Parse(wo_id));
                    if (get_fedby_mapping != null && get_fedby_mapping.Count > 0)
                    {
                        var parent_groupby = get_fedby_mapping.GroupBy(x => x.parent_asset_id);
                        foreach (var item in parent_groupby)
                        {
                            // is parent exist in main asset 
                            var is_main_exist = responseModel.Where(x => x.asset_id == item.Key || x.temprory_asset_id == item.Key).FirstOrDefault();
                            if (is_main_exist != null) // if asset is exist in main then insert woline in to its children
                            {
                                if (is_main_exist.children_list == null)
                                    is_main_exist.children_list = new List<AssetChildrenMappingForCluster>();

                                foreach (var child in item)
                                {
                                    // check if children is exist or not 
                                    var is_child_exist = is_main_exist.children_list.Where(x => x.children_asset_id == child.WOOnboardingAssets.asset_id).FirstOrDefault();
                                    if (is_child_exist == null) // if child is not exist then add
                                    {
                                        AssetChildrenMappingForCluster AssetChildrenMappingForCluster = new AssetChildrenMappingForCluster();
                                        if (child.WOOnboardingAssets.TempAsset != null && child.WOOnboardingAssets.TempAsset.asset_id != null)
                                        {
                                            AssetChildrenMappingForCluster.children_asset_id = child.WOOnboardingAssets.TempAsset.asset_id.Value;
                                        }
                                        else
                                        {
                                            AssetChildrenMappingForCluster.children_asset_id = child.woonboardingassets_id;
                                        }
                                        AssetChildrenMappingForCluster.asset_children_hierrachy_id = Guid.Empty;
                                        AssetChildrenMappingForCluster.is_asset_temp = true;
                                        AssetChildrenMappingForCluster.ocp_main_subcomponent_asset = child.fed_by_via_subcomponant_asset_id;
                                        AssetChildrenMappingForCluster.ocp_main_subcomponent_asset = child.via_subcomponant_asset_id;

                                        responseModel.Where(x => x.asset_id == item.Key || x.temprory_asset_id == item.Key).FirstOrDefault().children_list.Add(AssetChildrenMappingForCluster);
                                    }
                                }
                            }
                        }
                    }

                    // check if any existing asset is assigned to WO then change asset_id to woline_id
                    var get_existing_asset_woline = _UoW.AssetRepository.GetExistingAssetWOline(Guid.Parse(wo_id));
                    foreach (var item in get_existing_asset_woline)
                    {
                        // chnage in child list if existing asset added
                        responseModel.ForEach(toplevel =>
                        {
                            if (toplevel.children_list != null)
                            {
                                toplevel.children_list.ForEach(child_ =>
                                {
                                    if (child_.children_asset_id == item.asset_id)
                                    {
                                        child_.children_asset_id = item.woonboardingassets_id;
                                    }
                                });
                            }
                        });
                    }


                }
                responseModel.ForEach(x =>
                {
                    if (x.children_list != null && x.children_list.Count > 0)
                    {
                        x.is_child_available = true;
                    }
                });


            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }
        public ListViewModel<string> GetAssetLevelOptions()
        {
            ListViewModel<string> response = new ListViewModel<string>();
            var assets = _UoW.AssetRepository.GetAllRawHierarchyAssets();
            if (assets != null && assets.Count > 0)
            {
                var total_levels = assets.Select(x => x.levels).Distinct().ToList();
                response.list = total_levels;
            }
            return response;
        }

        public GetNameplateInfoByAssetidResponsemodel GetNameplateInfoByAssetid(string assetid)
        {
            GetNameplateInfoByAssetidResponsemodel respose = null;

            var asset = _UoW.AssetRepository.GetAssetByAssetID(assetid);
            if (asset != null)
            {
                respose = new GetNameplateInfoByAssetidResponsemodel();
                if (!String.IsNullOrEmpty(asset.InspectionTemplateAssetClass.form_nameplate_info) && !String.IsNullOrEmpty(asset.form_retrived_nameplate_info))
                {
                    respose.form_retrived_nameplate_info = CompareNPDatajsonAndTemplateJson(asset.InspectionTemplateAssetClass.form_nameplate_info, asset.form_retrived_nameplate_info);
                }
                respose.asset_id = asset.asset_id;
            }
            return respose;
        }

        public string CompareNPDatajsonAndTemplateJson(string template_json, string data_json)
        {
            string output_json = data_json;
            // Create a new output JSON object
            var outputJson = new Dictionary<string, string>();

            try
            {
                if (!String.IsNullOrEmpty(data_json))
                {
                    string firstJsonString = template_json;
                    string secondJsonString = data_json;
                    // Parse both JSON strings to dynamic objects
                    var templateJson = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(firstJsonString);
                    var userDataJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(secondJsonString);

                    // Iterate over the template keys and compare with user data
                    foreach (var key in templateJson.Keys)
                    {
                        if (userDataJson.ContainsKey(key))
                        {
                            // If the key exists in the user data, take the value from there
                            outputJson[key] = userDataJson[key];
                        }
                        else
                        {
                            // If the key is missing in user data, set the value as blank
                            outputJson[key] = "";
                        }
                    }
                    return JsonConvert.SerializeObject(outputJson, Formatting.Indented);
                }
                else // if result data is null then prepare result from template
                {
                    // Parse the JSON string into a JObject
                    JObject originalJson = JObject.Parse(template_json);

                    // Create a new dictionary to hold the flattened JSON structure
                    Dictionary<string, string> flatJson = new Dictionary<string, string>();

                    // Iterate through each property in the original JSON
                    foreach (var property in originalJson.Properties())
                    {
                        // Use the key as it is, and set the value as an empty string
                        flatJson[property.Name] = "";
                    }

                    // Convert the flat dictionary back to JSON
                    output_json = JsonConvert.SerializeObject(flatJson, Formatting.Indented);

                    return output_json;
                }
            }
            catch (Exception ex)
            {
                return output_json;
            }
            // Convert the output dictionary to a JSON string
            return output_json;
        }
        public async Task<int> ChangeAssetHierarchy(ChangeAssetHierarchyRequestmodel requestModel)
        {
            // update main asset only if both source and destination are main.
            if (!requestModel.is_changing_asset_id_temp && !requestModel.is_destination_asset_id_temp)
            {
                var get_moving_asset = _UoW.AssetRepository.GetAssetByIDForhierarchychange(requestModel.changing_asset_id);
                var get_destination_asset = _UoW.AssetRepository.GetAssetByIDForhierarchychange(requestModel.destination_asset_id);

                AssetParentHierarchyMapping AssetParentHierarchyMapping = new AssetParentHierarchyMapping();
                AssetParentHierarchyMapping.asset_id = get_moving_asset.asset_id;
                AssetParentHierarchyMapping.parent_asset_id = get_destination_asset.asset_id;
                AssetParentHierarchyMapping.fed_by_usage_type_id = 1; // set default as normal
                AssetParentHierarchyMapping.created_at = DateTime.UtcNow;
                AssetParentHierarchyMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                AssetParentHierarchyMapping.is_deleted = false;
                AssetParentHierarchyMapping.site_id = get_moving_asset.site_id;

                var insert_parent = await _UoW.BaseGenericRepository<AssetParentHierarchyMapping>().Insert(AssetParentHierarchyMapping);

                AssetChildrenHierarchyMapping AssetChildrenHierarchyMapping = new AssetChildrenHierarchyMapping();
                AssetChildrenHierarchyMapping.asset_id = get_destination_asset.asset_id;
                AssetChildrenHierarchyMapping.children_asset_id = get_moving_asset.asset_id;
                AssetChildrenHierarchyMapping.created_at = DateTime.UtcNow;
                AssetChildrenHierarchyMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                AssetChildrenHierarchyMapping.is_deleted = false;
                AssetChildrenHierarchyMapping.site_id = get_moving_asset.site_id;

                var insert_children = await _UoW.BaseGenericRepository<AssetChildrenHierarchyMapping>().Insert(AssetChildrenHierarchyMapping);

                _UoW.SaveChanges();

            }
            else // in temp asset section source will always temp but destination can be temp or main.
            {
                WOOBAssetFedByMapping WOOBAssetFedByMapping = new WOOBAssetFedByMapping();
                WOOBAssetFedByMapping.woonboardingassets_id = requestModel.changing_asset_id;
                WOOBAssetFedByMapping.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                WOOBAssetFedByMapping.parent_asset_id = requestModel.destination_asset_id;
                if (requestModel.is_destination_asset_id_temp)
                    WOOBAssetFedByMapping.is_parent_from_ob_wo = true;
                else
                    WOOBAssetFedByMapping.is_parent_from_ob_wo = false;
                WOOBAssetFedByMapping.created_at = DateTime.UtcNow;
                WOOBAssetFedByMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                WOOBAssetFedByMapping.is_deleted = false;


                var insert_fedby = await _UoW.BaseGenericRepository<WOOBAssetFedByMapping>().Insert(WOOBAssetFedByMapping);
                _UoW.SaveChanges();
            }


            #region old flow of asset hierarchy
            /*
            int level_difference = 1;
            int previous_moving_asset_level = 1;

            var get_moving_asset = _UoW.AssetRepository.GetAssetByIDForhierarchychange(requestModel.changing_asset_id);
            var get_destination_asset = _UoW.AssetRepository.GetAssetByIDForhierarchychange(requestModel.destination_asset_id);
            previous_moving_asset_level = int.Parse(get_moving_asset.levels);
           
            

            /// remove from previose asset's children data
            if (get_moving_asset.levels != "1")
            {
                var get_parent_of_moving_asset = _UoW.AssetRepository.GetAssetByInternalIDForhierarchychange(get_moving_asset.parent);
                var childlist = get_parent_of_moving_asset.children.Split(',').ToList().Where(x => !String.IsNullOrEmpty(x)).ToList();
                childlist.Remove(get_moving_asset.internal_asset_id);
                string combinedString = string.Join(",", childlist.ToArray());
                get_parent_of_moving_asset.children = combinedString;//JsonSerializer.Serialize(childlist);
                get_parent_of_moving_asset.modified_at = DateTime.UtcNow;
                var update_asset = await _UoW.BaseGenericRepository<Asset>().Update(get_parent_of_moving_asset);
            }
             

            // update parent 
            get_moving_asset.parent = get_destination_asset.internal_asset_id;

            // add in new parent children list
            if (!String.IsNullOrEmpty(get_destination_asset.children))
            {
                get_destination_asset.children = get_destination_asset.children + "," + get_moving_asset.internal_asset_id;
            }
            else
            {
                get_destination_asset.children =  get_moving_asset.internal_asset_id;
            }

            /// change levels for all children assets
            /// 
            var destined_assset_level = int.Parse(get_destination_asset.levels);
            get_moving_asset.levels = (destined_assset_level + 1).ToString();

            level_difference = int.Parse(get_moving_asset.levels) - previous_moving_asset_level;
            
            get_moving_asset.modified_at = DateTime.UtcNow;
            var update = await _UoW.BaseGenericRepository<Asset>().Update(get_moving_asset);

            get_destination_asset.modified_at = DateTime.UtcNow;
            update = await _UoW.BaseGenericRepository<Asset>().Update(get_destination_asset);

            List<executed_query> total_assets = new List<executed_query>();
            var get_moving_childrens_asset = _UoW.AssetRepository.GetSubAssetsByAssetID(get_moving_asset.asset_id.ToString(), 0, 0);
            get_moving_childrens_asset.list.ForEach(x =>
            {
                executed_query executed_query = new executed_query();
                executed_query.asset_id = x.asset_id;
                executed_query.is_query_executed =false;
                total_assets.Add(executed_query);
            });
            try
            {
                while (true)
                {
                    foreach (var item in total_assets.ToList())
                    {
                        if (!item.is_query_executed)
                        {
                            item.is_query_executed = true;
                            get_moving_childrens_asset = _UoW.AssetRepository.GetSubAssetsByAssetID(item.asset_id.ToString(), 0, 0);
                            get_moving_childrens_asset.list.ForEach(x =>
                            {
                                executed_query executed_query = new executed_query();
                                executed_query.asset_id = x.asset_id;
                                executed_query.is_query_executed = false;
                                total_assets.Add(executed_query);
                            });
                        }
                    }
                    if (total_assets.All(x => x.is_query_executed))
                    {
                        break;
                    }
                }
            }
            catch(Exception ex)
            {

            }

            var child_assets = _UoW.AssetRepository.GetAssetByIDs(total_assets.Select(x => x.asset_id).ToList());
            foreach(var asset in child_assets)
            {
                asset.levels = (int.Parse(asset.levels) + level_difference).ToString();
                asset.modified_at = DateTime.UtcNow;
                update = await _UoW.BaseGenericRepository<Asset>().Update(asset);
            }
            */
            #endregion old flow of asset hierarchy

            return 1;
        }


        public async Task<int> AddUpdateAssetNotes(AddUpdateAssetNotesRequestmodel requestmodel)
        {
            int response = (int)ResponseStatusNumber.Error;
            try
            {
                if (requestmodel.asset_notes_id != null)
                {
                    var asset_note = _UoW.AssetRepository.GetAssetnoteByID(requestmodel.asset_notes_id.Value);
                    asset_note.asset_note = requestmodel.asset_note;
                    asset_note.is_deleted = requestmodel.is_deleted;
                    asset_note.updated_at = DateTime.UtcNow;
                    asset_note.updated_by = UpdatedGenericRequestmodel.CurrentUser.requested_by;

                    var update = await _UoW.BaseGenericRepository<AssetNotes>().Update(asset_note);
                    if (update)
                    {
                        _UoW.SaveChanges();
                        response = (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        _UoW.RollbackTransaction();

                    }
                }
                else
                {
                    AssetNotes AssetNotes = new AssetNotes();
                    AssetNotes.asset_id = requestmodel.asset_id;
                    AssetNotes.asset_note = requestmodel.asset_note;
                    AssetNotes.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                    AssetNotes.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by;
                    AssetNotes.created_at = DateTime.UtcNow;
                    AssetNotes.asset_note_added_by_userid = UpdatedGenericRequestmodel.CurrentUser.requested_by;

                    var get_user = _UoW.AssetRepository.Getuserbyid(UpdatedGenericRequestmodel.CurrentUser.requested_by);
                    AssetNotes.asset_note_added_by_user = get_user.firstname + get_user.lastname;

                    var insert = await _UoW.BaseGenericRepository<AssetNotes>().Insert(AssetNotes);

                    if (insert)
                    {
                        _UoW.SaveChanges();
                        response = (int)ResponseStatusNumber.Success;

                    }
                    else
                    {
                        _UoW.RollbackTransaction();
                        response = (int)ResponseStatusNumber.Success;
                    }
                }

            }
            catch (Exception ex)
            {
                _UoW.RollbackTransaction();
                response = (int)ResponseStatusNumber.Success;
            }

            return response;
        }

        public ListViewModel<GetAssetNotesResponsemodel> GetAssetNotes(GetAssetNotesRequestmodel requestModel)
        {
            ListViewModel<GetAssetNotesResponsemodel> response = new ListViewModel<GetAssetNotesResponsemodel>();

            var site = _UoW.SiteRepository.GetSiteById(UpdatedGenericRequestmodel.CurrentUser.site_id);
            var get_asset_notes = _UoW.AssetRepository.GetAssetNotes(requestModel);

            response.list = _mapper.Map<List<GetAssetNotesResponsemodel>>(get_asset_notes.Item1);

            response.list.ForEach(x =>
            {
                try
                {
                    TimeZoneInfo estTimeZoneInfo = TZConvert.GetTimeZoneInfo(site.timezone);
                    x.created_at = TimeZoneInfo.ConvertTimeFromUtc(x.created_at, estTimeZoneInfo);

                }
                catch (Exception ex)
                {
                    _logger.LogInformation("date time convert issue" + ex.Message);
                }

            });

            response.listsize = get_asset_notes.Item2;
            response.pageSize = requestModel.page_size;
            response.pageIndex = requestModel.page_index;

            return response;

        }


        public FilterAssetBuildingLocationOptions FilterAssetBuildingLocationOptions()
        {
            FilterAssetBuildingLocationOptions response = new FilterAssetBuildingLocationOptions();
            var get_buildings = _UoW.AssetRepository.GetBuildingsBySite();
            var get_floors = _UoW.AssetRepository.GetFloorsBySite();
            var get_rooms = _UoW.AssetRepository.GetRoomsBySite();
            var get_sections = _UoW.AssetRepository.GetSectionBySite();

            response.buildings = _mapper.Map<List<FilterAssetBuildingLocationOptionsmapping>>(get_buildings);
            response.floors = _mapper.Map<List<FilterAssetBuildingLocationOptionsmapping>>(get_floors);
            response.rooms = _mapper.Map<List<FilterAssetBuildingLocationOptionsmapping>>(get_rooms);
            response.sections = _mapper.Map<List<FilterAssetBuildingLocationOptionsmapping>>(get_sections);

            return response;
        }
        public FilterAssetHierarchyLevelOptionsResponsemodel FilterAssetHierarchyLevelOptions()
        {
            FilterAssetHierarchyLevelOptionsResponsemodel response = new FilterAssetHierarchyLevelOptionsResponsemodel();
            response.hierarchy_max_level = _UoW.AssetRepository.FilterAssetHierarchyLevelOptions();
            return response;
        }
        public class executed_query
        {
            public Guid asset_id { get; set; }
            public bool is_query_executed { get; set; }
        }

        public async Task<int> DeleteAssetImage(DeleteAssetImageRequestmodel requestModel)
        {
            var reponse = (int)ResponseStatusNumber.Error;
            var get_asset_images = _UoW.AssetRepository.GetAssetImages(requestModel.asset_profile_images_id);

            foreach (var image in get_asset_images)
            {
                image.is_deleted = true;
                image.modified_at = DateTime.UtcNow;
                image.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                var update = await _UoW.BaseGenericRepository<AssetProfileImages>().Update(image);
                if (update)
                {
                    reponse = (int)ResponseStatusNumber.Success;
                }
            }
            return reponse;
        }



        public ExcelWorksheet _activteworksheet { get; set; }
        List<user_details> users = new List<user_details>();
        public async Task<int> UploadAssetExcelTest()
        {

            //string folderName = DocumensUploadPath.UploadPath((int)UploadDocTypes.JobOrder);
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var fullPath = Path.Combine(pathToSave, "ClientAsset.xlsx");

            using (var ms = new MemoryStream())
            {
                FileStream file1 = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                file1.CopyTo(ms);
                // file.CopyTo(ms);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage package = new ExcelPackage(ms);
                int count = package.Workbook.Worksheets.Count;
                if (count > 0)
                {
                    _activteworksheet = package.Workbook.Worksheets.FirstOrDefault();
                }
                ValidateRecords();
                if (users.Count > 0)
                {
                    /// star inserting data from here 
                    /// 

                    try
                    {

                        /*  foreach (var x in users)
                          {
                            _UoW.BeginTransaction();
                              var get_parent = _UoW.WorkOrderRepository.GetAssetByLocation(x.location.Trim().ToLower());
                              if(get_parent== null)
                              {
                                  get_parent = new Asset();
                                  get_parent.name = x.location;
                                  get_parent.internal_asset_id = x.location;
                                  get_parent.created_at = DateTime.UtcNow;
                                  get_parent.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                                  get_parent.company_id = UpdatedGenericRequestmodel.CurrentUser.company_id;
                                  get_parent.status = 3;
                                  get_parent.levels = "1";
                                  var insert_parent = _UoW.BaseGenericRepository<Asset>().Insert(get_parent);
                                  _UoW.SaveChanges();

                              }

                              var building = _UoW.WorkOrderRepository.GetFormIOBuildingByName(x.Building.ToLower().Trim());
                              if(building == null)
                              {
                                  building = new FormIOBuildings();
                                  building.formio_building_name = x.Building;
                                  building.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                                  building.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                                  var insert = _UoW.BaseGenericRepository<FormIOBuildings>().Insert(building);
                                  _UoW.SaveChanges();
                              }

                              var floor = _UoW.WorkOrderRepository.GetFormIOFloorByName(x.Floor.Trim().ToLower(), building.formiobuilding_id);
                              if (floor == null)
                              {
                                  floor = new FormIOFloors();
                                  floor.formio_floor_name  = x.Floor;
                                  floor.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                                  floor.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                                  floor.formiobuilding_id = building.formiobuilding_id;
                                  var insert = _UoW.BaseGenericRepository<FormIOFloors>().Insert(floor);
                                  _UoW.SaveChanges();
                              }    

                              var room = _UoW.WorkOrderRepository.GetFormIORoomByName(x.Room.Trim().ToLower(), floor.formiofloor_id);
                              if(room == null)
                              {
                                  room = new FormIORooms();
                                  room.formio_room_name = x.Room;
                                  room.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                                  room.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                                  room.formiofloor_id = floor.formiofloor_id;
                                  var insert = _UoW.BaseGenericRepository<FormIORooms>().Insert(room);
                                  _UoW.SaveChanges();
                              }
                              if (!String.IsNullOrEmpty(x.Section))
                              {
                                  var section = _UoW.WorkOrderRepository.GetFormIOSectionByName(x.Section.Trim().ToLower(), room.formioroom_id);
                                  if (section == null)
                                  {
                                      section = new FormIOSections();
                                      section.formio_section_name = x.Section;
                                      section.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                                      section.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                                      section.formioroom_id = room.formioroom_id;
                                      var insert = _UoW.BaseGenericRepository<FormIOSections>().Insert(section);
                                      _UoW.SaveChanges();
                                  }
                              }
                            _UoW.CommitTransaction();
                          }*/


                        /*    foreach (var x in users)
                            {
                                var get_asset = _UoW.WorkOrderRepository.GetAssetByInternalID(x.location.Trim().ToLower());
                                if(get_asset == null)
                                {
                                    _UoW.BeginTransaction();
                                    var get_parent = _UoW.WorkOrderRepository.GetAssetByLocation(x.location.Trim().ToLower());
                                    Asset asset = new Asset();
                                    asset.name = x.identification;
                                    asset.internal_asset_id = x.AssetID;
                                    asset.parent = get_parent.internal_asset_id;
                                    asset.company_id = "22a8170a-f97c-432b-976a-28c4d7abf3ca";
                                    asset.site_id = Guid.Parse("62307cf5-a54c-4a8b-ad83-cb16f6302cf0");
                                    asset.form_id = Guid.Parse("e2afc3b1-47ea-4f93-8b56-052706667f32");
                                    asset.created_at = DateTime.UtcNow;
                                    asset.status = 3;

                                    asset.levels = "2";

                                    nameplate_info nameplate_info = new nameplate_info();
                                    nameplate_info.manufacturer = x.Manufacturer;
                                    nameplate_info.catalogNumber = x.Catalog_Number;
                                    nameplate_info.model = x.Model;
                                    nameplate_info.serialNumber = x.Serial_Number;
                                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(nameplate_info);
                                    asset.form_retrived_nameplate_info = json;

                                    var insert = await _UoW.BaseGenericRepository<Asset>().Insert(asset);

                                    AssetFormIOBuildingMappings AssetFormIOBuildingMappings = new AssetFormIOBuildingMappings();

                                    if (!String.IsNullOrEmpty(x.Building))
                                    {
                                        var building = _UoW.WorkOrderRepository.GetFormIOBuildingByName(x.Building.Trim().ToLower());
                                        AssetFormIOBuildingMappings.formiobuilding_id = building.formiobuilding_id;
                                    }
                                    if (!String.IsNullOrEmpty(x.Floor))
                                    {
                                        var floor = _UoW.WorkOrderRepository.GetFormIOFloorByName(x.Floor.Trim().ToLower(), AssetFormIOBuildingMappings.formiobuilding_id.Value);
                                        AssetFormIOBuildingMappings.formiofloor_id = floor.formiofloor_id;
                                    }
                                    if (!String.IsNullOrEmpty(x.Room))
                                    {
                                        var floor = _UoW.WorkOrderRepository.GetFormIOFloorByName(x.Floor.Trim().ToLower(), AssetFormIOBuildingMappings.formiobuilding_id.Value);
                                        var room = _UoW.WorkOrderRepository.GetFormIORoomByName(x.Room.Trim().ToLower(), floor.formiofloor_id);
                                        AssetFormIOBuildingMappings.formioroom_id = room.formioroom_id;
                                    }
                                    if (!String.IsNullOrEmpty(x.Section))
                                    {
                                        var room = _UoW.WorkOrderRepository.GetFormIORoomByName(x.Room.Trim().ToLower(), AssetFormIOBuildingMappings.formiofloor_id.Value);
                                        var section = _UoW.WorkOrderRepository.GetFormIOSectionByName(x.Section.Trim().ToLower(), room.formioroom_id);
                                        AssetFormIOBuildingMappings.formiosection_id = section.formiosection_id;
                                    }
                                    AssetFormIOBuildingMappings.asset_id = asset.asset_id;

                                    var insert1 = await _UoW.BaseGenericRepository<AssetFormIOBuildingMappings>().Insert(AssetFormIOBuildingMappings);
                                    _UoW.SaveChanges();
                                    _UoW.CommitTransaction();
                                }

                            }*/

                        foreach (var x in users)
                        {
                            var get_parent = _UoW.WorkOrderRepository.GetAssetByLocation(x.location.Trim().ToLower());
                            if (!String.IsNullOrEmpty(get_parent.children))
                            {
                                get_parent.children = get_parent.children + "," + x.AssetID;
                            }
                            else
                            {
                                get_parent.children = x.AssetID;
                            }
                            var updae = await _UoW.BaseGenericRepository<Asset>().Update(get_parent);
                            _UoW.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        _UoW.RollbackTransaction();
                    }
                }

            }
            return 1;
        }


        public async Task<EditAssetDetailsResponseModel> EditAssetDetails(EditAssetDetailsRequestmodel requestModel)
        {
            AssetPMService assetPMService = new AssetPMService(_mapper);
            EditAssetDetailsResponseModel response_model = new EditAssetDetailsResponseModel();
            //int response = (int)ResponseStatusNumber.Error;

            /// check qr code duplication 
            /// 
            if (!String.IsNullOrEmpty(requestModel.QR_code))
            {
                if (requestModel.asset_id != null)
                {
                    var db_qr = _UoW.AssetRepository.GetduplicateAssetbyQR(requestModel.QR_code.ToLower().Trim(), requestModel.asset_id.Value);
                    if (db_qr != null)
                    {
                        response_model.status = (int)ResponseStatusNumber.qr_code_must_be_unique;
                        return response_model;
                    }
                }
                else
                {
                    var db_qr = _UoW.AssetRepository.GetduplicateAssetbyQR_CreateAsset(requestModel.QR_code.ToLower().Trim());
                    if (db_qr != null)
                    {
                        response_model.status = (int)ResponseStatusNumber.qr_code_must_be_unique;
                        return response_model;
                    }
                }
            }

            Asset get_asset = null;
            if (requestModel.asset_id != null)
            {
                get_asset = _UoW.AssetRepository.GetAssetByAssetID(requestModel.asset_id.ToString());
            }
            else
            {
                get_asset = new Asset();
            }

            if (get_asset != null)
            {
                /// update parent 
                /// 
                if (!String.IsNullOrEmpty(requestModel.parent_asset_internal_id))
                {
                    if (get_asset.parent != requestModel.parent_asset_internal_id)
                    {
                        var get_new_asset = _UoW.AssetRepository.GetAssetsByInternalAssetID(requestModel.parent_asset_internal_id);
                        ChangeAssetHierarchyRequestmodel ChangeAssetHierarchyRequestmodel = new ChangeAssetHierarchyRequestmodel();
                        ChangeAssetHierarchyRequestmodel.changing_asset_id = get_asset.asset_id;
                        ChangeAssetHierarchyRequestmodel.destination_asset_id = get_new_asset.asset_id;

                        await ChangeAssetHierarchy(ChangeAssetHierarchyRequestmodel);
                    }
                }

                get_asset.name = requestModel.asset_name;
                if (requestModel.status != null)
                {
                    get_asset.status = requestModel.status;
                    if (requestModel.status == (int)Status.AssetDeactive && requestModel.asset_id != null && requestModel.asset_id != Guid.Empty)
                    {
                        await MarkAssetStatusAsInActiveDispose(requestModel.asset_id.Value);
                    }
                }
                if (requestModel.component_level_type_id != null)
                {
                    get_asset.component_level_type_id = requestModel.component_level_type_id.Value;
                }

                bool is_asset_condition_changed = false;

                if (get_asset.condition_index_type != requestModel.condition_index_type || get_asset.criticality_index_type != requestModel.criticality_index_type)
                    is_asset_condition_changed = true;

                get_asset.criticality_index = requestModel.criticality_index;
                get_asset.condition_index = requestModel.condition_index;
                get_asset.criticality_index_type = requestModel.criticality_index_type;
                get_asset.condition_index_type = requestModel.condition_index_type;
                get_asset.commisiion_date = requestModel.commisiion_date;
                get_asset.visual_insepction_last_performed = requestModel.visual_insepction_last_performed;
                get_asset.mechanical_insepction_last_performed = requestModel.mechanical_insepction_last_performed;
                get_asset.electrical_insepction_last_performed = requestModel.electrical_insepction_last_performed;
                get_asset.infrared_insepction_last_performed = requestModel.infrared_insepction_last_performed;
                get_asset.arc_flash_study_last_performed = requestModel.arc_flash_study_last_performed;
                get_asset.asset_operating_condition_state = requestModel.asset_operating_condition_state;
                get_asset.asset_placement = requestModel.asset_placement;
                get_asset.thermal_classification_id = requestModel.thermal_classification_id;
                get_asset.commisiion_date = requestModel.commisiion_date;
                get_asset.form_retrived_nameplate_info = requestModel.form_retrived_nameplate_info;
                get_asset.panel_schedule = requestModel.panel_schedule;
                get_asset.arc_flash_label_valid = requestModel.arc_flash_label_valid;
                get_asset.asset_group_id = requestModel.asset_group_id;
                get_asset.x_axis = requestModel.x_axis > 0 ? requestModel.x_axis : get_asset.x_axis;
                get_asset.y_axis = requestModel.y_axis > 0 ? requestModel.y_axis : get_asset.y_axis;
                get_asset.asset_node_data_json = !String.IsNullOrEmpty(requestModel.asset_node_data_json) ? requestModel.asset_node_data_json : get_asset.asset_node_data_json;
                get_asset.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                get_asset.company_id = UpdatedGenericRequestmodel.CurrentUser.company_id;
                if (get_asset.asset_operating_condition_state == null)
                {
                    get_asset.asset_operating_condition_state = (int)AssetOperatingConduitionState.Operating_Normally;
                }
                get_asset.maintenance_index_type = requestModel.maintenance_index_type;
                get_asset.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                //get_asset.QR_code = requestModel.QR_code;
                //if (!String.IsNullOrEmpty(requestModel.QR_code))
                //{
                //    get_asset.internal_asset_id = requestModel.QR_code;
                //}

                get_asset.Location = requestModel.Location;
                get_asset.code_compliance = requestModel.code_compliance;
                get_asset.asset_expected_usefull_life = requestModel.asset_expected_usefull_life;
                if (requestModel.inspectiontemplate_asset_class_id != null)  // adding if condition as FE needs to deploy first 
                {
                    get_asset.inspectiontemplate_asset_class_id = requestModel.inspectiontemplate_asset_class_id;
                }

                get_asset.QR_code = requestModel.QR_code;
                /// if QR code is not null then QR code will be asset's internal id
                if (!String.IsNullOrEmpty(get_asset.QR_code))
                {
                    get_asset.internal_asset_id = requestModel.QR_code;
                }
                /*else // if QR code is empty then asset's unique internal id will be its QR code // now we are allowing null QR code for assets
                {
                    int asset_count = _UoW.WorkOrderRepository.GetAssetscountBySite(get_asset.site_id.ToString());
                    var get_sitecode = _UoW.WorkOrderRepository.GetSiteCodeById(get_asset.site_id);
                    get_asset.internal_asset_id = get_sitecode + (asset_count + 1).ToString();
                    get_asset.QR_code = get_sitecode + (asset_count + 1).ToString();
                }*/

                /*
                /// if QR code is not null then QR code will be asset's internal id
                if (String.IsNullOrEmpty(get_asset.internal_asset_id) &&!String.IsNullOrEmpty(requestModel.QR_code)) 
                {

                    get_asset.internal_asset_id = requestModel.QR_code;
                    get_asset.QR_code = requestModel.QR_code;
                }
                else // if QR code is empty then asset's unique internal id will be its QR code
                {
                    int asset_count = _UoW.WorkOrderRepository.GetAssetscountBySite(get_asset.site_id.ToString());
                    var get_sitecode = _UoW.WorkOrderRepository.GetSiteCodeById(get_asset.site_id);
                    get_asset.internal_asset_id = get_sitecode + (asset_count + 1).ToString();
                    get_asset.QR_code = get_sitecode + (asset_count + 1).ToString();
                }
                */

                /// insert building data in db if not there

                FormIOBuildings FormIOBuildings = null;
                FormIOFloors FormIOFloors = null;
                FormIORooms FormIORooms = null;
                FormIOSections FormIOSections = null;


                if (String.IsNullOrEmpty(requestModel.building))
                {
                    requestModel.building = "Default";
                }
                if (String.IsNullOrEmpty(requestModel.floor))
                {
                    requestModel.floor = "Default";
                }
                if (String.IsNullOrEmpty(requestModel.room))
                {
                    requestModel.room = "Default";
                }
                if (String.IsNullOrEmpty(requestModel.section))
                {
                    requestModel.section = "Default";
                }


                //if (String.IsNullOrEmpty(requestModel.section) || requestModel.section =="")
                //{
                //    requestModel.section = "Default";
                //}

                if (!String.IsNullOrEmpty(requestModel.building))
                {
                    FormIOBuildings = _UoW.WorkOrderRepository.GetFormIOBuildingByName(requestModel.building);
                    if (FormIOBuildings == null)
                    {
                        FormIOBuildings = new FormIOBuildings();
                        FormIOBuildings.formio_building_name = requestModel.building;
                        FormIOBuildings.created_at = DateTime.UtcNow;
                        FormIOBuildings.site_id = get_asset.site_id;
                        FormIOBuildings.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);

                        var insertbuilding = await _UoW.BaseGenericRepository<FormIOBuildings>().Insert(FormIOBuildings);
                        _UoW.SaveChanges();
                    }
                }
                if (!String.IsNullOrEmpty(requestModel.floor))
                {
                    FormIOFloors = _UoW.WorkOrderRepository.GetFormIOFloorByName(requestModel.floor, FormIOBuildings.formiobuilding_id);
                    if (FormIOFloors == null)
                    {
                        FormIOFloors = new FormIOFloors();
                        FormIOFloors.formio_floor_name = requestModel.floor;
                        FormIOFloors.formiobuilding_id = FormIOBuildings.formiobuilding_id;
                        FormIOFloors.created_at = DateTime.UtcNow;
                        FormIOFloors.site_id = get_asset.site_id;
                        FormIOFloors.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);

                        var insertfloor = await _UoW.BaseGenericRepository<FormIOFloors>().Insert(FormIOFloors);
                        _UoW.SaveChanges();
                    }
                }
                if (!String.IsNullOrEmpty(requestModel.room))
                {
                    FormIORooms = _UoW.WorkOrderRepository.GetFormIORoomByName(requestModel.room, FormIOFloors.formiofloor_id);
                    if (FormIORooms == null)
                    {
                        FormIORooms = new FormIORooms();
                        FormIORooms.formio_room_name = requestModel.room;
                        FormIORooms.formiofloor_id = FormIOFloors.formiofloor_id;
                        FormIORooms.created_at = DateTime.UtcNow;
                        FormIORooms.site_id = get_asset.site_id;
                        FormIORooms.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);

                        var insertroom = await _UoW.BaseGenericRepository<FormIORooms>().Insert(FormIORooms);
                        _UoW.SaveChanges();
                    }
                }
                if (!String.IsNullOrEmpty(requestModel.section))
                {
                    FormIOSections = _UoW.WorkOrderRepository.GetFormIOSectionByName(requestModel.section, FormIORooms.formioroom_id);
                    if (FormIOSections == null)
                    {
                        FormIOSections = new FormIOSections();
                        FormIOSections.formio_section_name = requestModel.section;
                        FormIOSections.formioroom_id = FormIORooms.formioroom_id;
                        FormIOSections.created_at = DateTime.UtcNow;
                        FormIOSections.site_id = get_asset.site_id;
                        FormIOSections.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);

                        var insertroom = await _UoW.BaseGenericRepository<FormIOSections>().Insert(FormIOSections);
                        _UoW.SaveChanges();
                    }
                }


                if (get_asset.AssetFormIOBuildingMappings == null)
                {
                    get_asset.AssetFormIOBuildingMappings = new AssetFormIOBuildingMappings();
                }
                if (FormIOBuildings != null)
                    get_asset.AssetFormIOBuildingMappings.formiobuilding_id = FormIOBuildings.formiobuilding_id;
                if (FormIOFloors != null)
                    get_asset.AssetFormIOBuildingMappings.formiofloor_id = FormIOFloors.formiofloor_id;
                if (FormIORooms != null)
                    get_asset.AssetFormIOBuildingMappings.formioroom_id = FormIORooms.formioroom_id;
                if (FormIOSections != null)
                    get_asset.AssetFormIOBuildingMappings.formiosection_id = FormIOSections.formiosection_id;

                if (requestModel.formiobuilding_id > 0 && requestModel.formiofloor_id > 0 && requestModel.formioroom_id > 0)
                {
                    get_asset.AssetFormIOBuildingMappings.formiobuilding_id = requestModel.formiobuilding_id;
                    get_asset.AssetFormIOBuildingMappings.formiofloor_id = requestModel.formiofloor_id;
                    get_asset.AssetFormIOBuildingMappings.formioroom_id = requestModel.formioroom_id;
                }

                if (!String.IsNullOrEmpty(requestModel.section) && requestModel.formioroom_id > 0)
                {
                    FormIOSections = _UoW.WorkOrderRepository.GetFormIOSectionByName(requestModel.section, requestModel.formioroom_id);
                    if (FormIOSections == null)
                    {
                        FormIOSections = new FormIOSections();
                        FormIOSections.formio_section_name = requestModel.section;
                        FormIOSections.formioroom_id = requestModel.formioroom_id;
                        FormIOSections.created_at = DateTime.UtcNow;
                        FormIOSections.site_id = get_asset.site_id;
                        FormIOSections.company_id = get_asset.Sites.company_id;

                        var insertroom = await _UoW.BaseGenericRepository<FormIOSections>().Insert(FormIOSections);
                        _UoW.SaveChanges();
                    }
                    if (FormIOSections != null)
                        get_asset.AssetFormIOBuildingMappings.formiosection_id = FormIOSections.formiosection_id;
                }


                //-- Upate Asset Details
                if (requestModel.asset_id != null && requestModel.asset_id != Guid.Empty)
                {
                    get_asset.modified_at = DateTime.UtcNow;

                    var update = await _UoW.BaseGenericRepository<Asset>().Update(get_asset);
                    _UoW.SaveChanges();

                    if (is_asset_condition_changed)
                    {
                        await assetPMService.AddRemoveAssetPMsOnAssetCondition(requestModel.asset_id.Value);
                    }
                }
                //-- Create New Asset
                else
                {
                    get_asset.created_at = DateTime.UtcNow;

                    var insert = await _UoW.BaseGenericRepository<Asset>().Insert(get_asset);
                    _UoW.SaveChanges();

                    if (get_asset.inspectiontemplate_asset_class_id != null)
                    {
                        var inspectionTemplateAssetClass = _UoW.WorkOrderRepository.GetInspectionTemplateAssetClass(get_asset.inspectiontemplate_asset_class_id.Value);
                        // insert default asset pm
                        if (inspectionTemplateAssetClass != null && inspectionTemplateAssetClass.PMCategory != null
                       && inspectionTemplateAssetClass.PMCategory.PMPlans != null && inspectionTemplateAssetClass.PMCategory.PMPlans.Count > 0)
                        {
                            var get_default_pm_plan = inspectionTemplateAssetClass.PMCategory.PMPlans.Where(x => x.is_default_pm_plan).FirstOrDefault();
                            if (get_default_pm_plan != null)
                            {
                                AssignPMToAsset AssignPMToAsset = new AssignPMToAsset();
                                AssignPMToAsset.asset_id = get_asset.asset_id;
                                AssignPMToAsset.pm_plan_id = get_default_pm_plan.pm_plan_id;


                                var assignpm = await assetPMService.AddAssetPM(AssignPMToAsset);
                            }
                        }
                    }
                }

                response_model.asset_id = get_asset.asset_id;


                // For Updating AssetPMs Due_Date , Due_Flag and Due_In based on Asset cricality condition
                // AssetPMService assetPMService = new AssetPMService(_mapper);
                await assetPMService.UpdateDueDateDueInDueFlagForAssetPMsByAssetId(get_asset.asset_id);

                if (requestModel.asset_profile_images != null && requestModel.asset_profile_images.Count > 0)
                {
                    var new_image_list = requestModel.asset_profile_images.Where(x => x.asset_profile_images_id == null).ToList();

                    var requested_deleting_images = requestModel.asset_profile_images.Where(x => x.asset_profile_images_id != null && x.is_deleted == true).ToList();
                    if (new_image_list.Count > 0 && new_image_list != null)
                    {
                        foreach (var item in new_image_list)
                        {
                            AssetProfileImages assetProfileImages = new AssetProfileImages();
                            assetProfileImages.asset_photo_type = item.asset_photo_type;
                            assetProfileImages.asset_id = get_asset.asset_id;
                            assetProfileImages.asset_photo = item.asset_photo;
                            assetProfileImages.asset_thumbnail_photo = item.asset_thumbnail_photo;
                            assetProfileImages.image_extracted_json = item.image_extracted_json;
                            assetProfileImages.created_at = DateTime.UtcNow;
                            assetProfileImages.created_by = UpdatedGenericRequestmodel.CurrentUser.request_id;
                            assetProfileImages.is_deleted = false;

                            var insertimage = await _UoW.BaseGenericRepository<AssetProfileImages>().Insert(assetProfileImages);
                            _UoW.SaveChanges();
                        }
                    }
                    if (requested_deleting_images.Count > 0 && requested_deleting_images != null)
                    {
                        var requested_deleting_images_ids = requested_deleting_images.Select(x => x.asset_profile_images_id).ToList();
                        foreach (var id in requested_deleting_images_ids)
                        {
                            var get_asset_image = _UoW.AssetRepository.GetAssetImagebyID(id.Value);

                            get_asset_image.is_deleted = true;
                            get_asset_image.modified_by = UpdatedGenericRequestmodel.CurrentUser.request_id;
                            get_asset_image.modified_at = DateTime.UtcNow;

                            var deleteimage = await _UoW.BaseGenericRepository<AssetProfileImages>().Update(get_asset_image);
                            _UoW.SaveChanges();
                        }
                        var deleted_url_list = requested_deleting_images.Select(x => x.asset_photo).ToList();
                        if (!String.IsNullOrEmpty(get_asset.asset_profile_image) && deleted_url_list.Contains(get_asset.asset_profile_image))
                        {
                            get_asset.asset_profile_image = null;
                            var update_asset = await _UoW.BaseGenericRepository<Asset>().Update(get_asset);
                            _UoW.SaveChanges();
                        }
                    }
                }


                //if this asset is subcomponent then edit toplevel
                if (requestModel.asset_toplevel_componenent != null)
                {
                    if (requestModel.asset_toplevel_componenent.asset_toplevelcomponent_mapping_id != null)
                    {
                        // check if top level is changed or not 
                        var current_toplevel = get_asset.AssetTopLevelcomponentMapping.Where(x => !x.is_deleted).FirstOrDefault();
                        if (current_toplevel != null)
                        {
                            if (requestModel.asset_toplevel_componenent.toplevelcomponent_asset_id != current_toplevel.toplevelcomponent_asset_id)
                            {
                                // delete current top level of this asset 
                                current_toplevel.is_deleted = true;
                                current_toplevel.updated_at = DateTime.UtcNow;
                                var update_top = await _UoW.BaseGenericRepository<AssetTopLevelcomponentMapping>().Update(current_toplevel);
                                // delete subcompoennt mapping of toplevel 
                                var current_subcomponent = _UoW.WorkOrderRepository.CheckSubcomponent(current_toplevel.toplevelcomponent_asset_id, get_asset.asset_id);
                                if (current_subcomponent != null)
                                {
                                    current_subcomponent.is_deleted = true;
                                    current_subcomponent.updated_at = DateTime.UtcNow;
                                    var update_sublevel = await _UoW.BaseGenericRepository<AssetSubLevelcomponentMapping>().Update(current_subcomponent);
                                }
                                ////now insert new top level in db
                                AssetTopLevelcomponentMapping AssetTopLevelcomponentMapping = new AssetTopLevelcomponentMapping();
                                AssetTopLevelcomponentMapping.asset_id = get_asset.asset_id;
                                AssetTopLevelcomponentMapping.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                                AssetTopLevelcomponentMapping.toplevelcomponent_asset_id = requestModel.asset_toplevel_componenent.toplevelcomponent_asset_id;
                                AssetTopLevelcomponentMapping.created_at = DateTime.UtcNow;
                                AssetTopLevelcomponentMapping.is_deleted = false;

                                var insert_toplevel = await _UoW.BaseGenericRepository<AssetTopLevelcomponentMapping>().Insert(AssetTopLevelcomponentMapping);
                                _UoW.SaveChanges();

                                AssetSubLevelcomponentMapping AssetSubLevelcomponentMapping = new AssetSubLevelcomponentMapping();
                                AssetSubLevelcomponentMapping.asset_id = requestModel.asset_toplevel_componenent.toplevelcomponent_asset_id;
                                AssetSubLevelcomponentMapping.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                                AssetSubLevelcomponentMapping.sublevelcomponent_asset_id = get_asset.asset_id;
                                AssetSubLevelcomponentMapping.created_at = DateTime.UtcNow;
                                AssetSubLevelcomponentMapping.is_deleted = false;

                                var insert_subcomponant = await _UoW.BaseGenericRepository<AssetSubLevelcomponentMapping>().Insert(AssetSubLevelcomponentMapping);
                                _UoW.SaveChanges();

                                var get_topLevel_asset_byId = _UoW.AssetRepository.GetAssetByAssetID(AssetTopLevelcomponentMapping.toplevelcomponent_asset_id.ToString());
                                get_asset.internal_asset_id = get_topLevel_asset_byId.internal_asset_id + (get_topLevel_asset_byId.AssetSubLevelcomponentMapping.Count + 1).ToString();


                            }
                        }
                    }
                    else // insert top level
                    {

                        AssetTopLevelcomponentMapping AssetTopLevelcomponentMapping = new AssetTopLevelcomponentMapping();
                        AssetTopLevelcomponentMapping.asset_id = get_asset.asset_id;
                        AssetTopLevelcomponentMapping.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                        AssetTopLevelcomponentMapping.toplevelcomponent_asset_id = requestModel.asset_toplevel_componenent.toplevelcomponent_asset_id;
                        AssetTopLevelcomponentMapping.created_at = DateTime.UtcNow;
                        AssetTopLevelcomponentMapping.is_deleted = false;

                        var insert_toplevel = await _UoW.BaseGenericRepository<AssetTopLevelcomponentMapping>().Insert(AssetTopLevelcomponentMapping);
                        _UoW.SaveChanges();

                        AssetSubLevelcomponentMapping AssetSubLevelcomponentMapping = new AssetSubLevelcomponentMapping();
                        AssetSubLevelcomponentMapping.asset_id = requestModel.asset_toplevel_componenent.toplevelcomponent_asset_id;
                        AssetSubLevelcomponentMapping.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                        AssetSubLevelcomponentMapping.sublevelcomponent_asset_id = get_asset.asset_id;
                        AssetSubLevelcomponentMapping.created_at = DateTime.UtcNow;
                        AssetSubLevelcomponentMapping.is_deleted = false;

                        var insert_subcomponant = await _UoW.BaseGenericRepository<AssetSubLevelcomponentMapping>().Insert(AssetSubLevelcomponentMapping);
                        _UoW.SaveChanges();
                    }
                }
                else // remove any top component if exist 
                {
                    /*if(get_asset.AssetTopLevelcomponentMapping!=null && get_asset.AssetTopLevelcomponentMapping.Count > 0)
                    {
                        var current_toplevel = get_asset.AssetTopLevelcomponentMapping.Where(x => !x.is_deleted).FirstOrDefault();

                        current_toplevel.is_deleted = true;
                        current_toplevel.updated_at = DateTime.UtcNow;
                        var update_top = await _UoW.BaseGenericRepository<AssetTopLevelcomponentMapping>().Update(current_toplevel);
                        // delete subcompoennt mapping of toplevel 
                        var current_subcomponent = _UoW.WorkOrderRepository.CheckSubcomponent(current_toplevel.toplevelcomponent_asset_id, get_asset.asset_id);
                        if (current_subcomponent != null)
                        {
                            current_subcomponent.is_deleted = true;
                            current_subcomponent.updated_at = DateTime.UtcNow;
                            var update_sublevel = await _UoW.BaseGenericRepository<AssetSubLevelcomponentMapping>().Update(current_subcomponent);
                        }
                    }*/

                }


                //if asset is TopLevel then Add/Update/Delete its Subcomponents and images
                if (requestModel.asset_subcomponents_mapping_list != null && requestModel.asset_subcomponents_mapping_list.Count > 0)
                {
                    // Add new Subcomponents
                    var added_ob_asset_sublevel = requestModel.asset_subcomponents_mapping_list.Where(x => (x.asset_sublevelcomponent_mapping_id == null || x.asset_sublevelcomponent_mapping_id == Guid.Empty) && !x.is_deleted).ToList();  // insert sublevel
                    if (added_ob_asset_sublevel != null && added_ob_asset_sublevel.Count > 0)
                    {
                        foreach (var item in added_ob_asset_sublevel)
                        {
                            //- Add Existing Subcomponent Asset 
                            if (item.sublevelcomponent_asset_id != null && item.sublevelcomponent_asset_id != Guid.Empty)
                            {
                                AddNewSubComponentRequestmodel addNewSubComponentRequestmodel = new AddNewSubComponentRequestmodel();
                                addNewSubComponentRequestmodel.is_subcomponent_for_existing = true;
                                addNewSubComponentRequestmodel.asset_id = get_asset.asset_id;
                                addNewSubComponentRequestmodel.sublevelcomponent_asset_id = item.sublevelcomponent_asset_id;
                                addNewSubComponentRequestmodel.subcomponentasset_image_list = item.subcomponentasset_image_list;
                                addNewSubComponentRequestmodel.circuit = item.circuit;

                                await AddNewSubComponent(addNewSubComponentRequestmodel);
                            }
                            else //- Create New Subcomponent Asset
                            {
                                AddNewSubComponentRequestmodel addNewSubComponentRequestmodel = new AddNewSubComponentRequestmodel();
                                addNewSubComponentRequestmodel.is_subcomponent_for_existing = false;
                                addNewSubComponentRequestmodel.asset_id = get_asset.asset_id;
                                addNewSubComponentRequestmodel.sublevelcomponent_asset_name = item.sublevelcomponent_asset_name;
                                addNewSubComponentRequestmodel.inspectiontemplate_asset_class_id = item.inspectiontemplate_asset_class_id;
                                addNewSubComponentRequestmodel.subcomponentasset_image_list = item.subcomponentasset_image_list;
                                addNewSubComponentRequestmodel.circuit = item.circuit;

                                await AddNewSubComponent(addNewSubComponentRequestmodel);
                            }
                        }
                    }

                    // Update/Delete Existing Subcomponents
                    var update_asset_sublevel = requestModel.asset_subcomponents_mapping_list.Where(x => (x.asset_sublevelcomponent_mapping_id != null && x.asset_sublevelcomponent_mapping_id != Guid.Empty)).ToList();  // update sublevel
                    if (update_asset_sublevel != null && update_asset_sublevel.Count > 0)
                    {
                        foreach (var update_sub in update_asset_sublevel)
                        {
                            // if requested sublevel is for delete 
                            if (update_sub.is_deleted)
                            {
                                DeleteAssetSubcomponentRequestmodel deleteAssetSubcomponentRequestmodel = new DeleteAssetSubcomponentRequestmodel();
                                deleteAssetSubcomponentRequestmodel.asset_sublevelcomponent_mapping_id = update_sub.asset_sublevelcomponent_mapping_id.Value;
                                await DeleteAssetSubcomponent(deleteAssetSubcomponentRequestmodel);
                            }
                            else // update circuit 
                            {
                                var sublevelcomponent = _UoW.AssetRepository.UpdateCircuitForAssetSubcomponent(update_sub.asset_sublevelcomponent_mapping_id.Value);
                                sublevelcomponent.circuit = update_sub.circuit;
                                sublevelcomponent.updated_at = DateTime.UtcNow;
                                sublevelcomponent.updated_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                var update = await _UoW.BaseGenericRepository<AssetSubLevelcomponentMapping>().Update(sublevelcomponent);
                            }

                            // Add/Delete Subcomponent Asset Images NewFlow
                            if (update_sub.subcomponentasset_image_list != null && update_sub.subcomponentasset_image_list.Count > 0)
                            {
                                var new_added_images_list = update_sub.subcomponentasset_image_list.Where(x => x.asset_profile_images_id == null && !x.is_deleted).ToList();

                                foreach (var image in new_added_images_list)
                                {
                                    AssetProfileImages AssetProfileImages = new AssetProfileImages();
                                    AssetProfileImages.asset_photo = image.asset_photo;
                                    AssetProfileImages.asset_thumbnail_photo = image.asset_thumbnail_photo;

                                    AssetProfileImages.asset_id = update_sub.sublevelcomponent_asset_id.Value;
                                    AssetProfileImages.asset_photo_type = image.asset_photo_type;
                                    AssetProfileImages.created_at = DateTime.UtcNow;
                                    AssetProfileImages.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                                    await _UoW.BaseGenericRepository<AssetProfileImages>().Insert(AssetProfileImages);
                                    _UoW.SaveChanges();
                                }

                                var deleted_images_list = update_sub.subcomponentasset_image_list.Where(x => x.asset_profile_images_id != null && x.is_deleted == true).ToList();

                                foreach (var image in deleted_images_list)
                                {
                                    var get_img = _UoW.AssetRepository.GetAssetImagebyID(image.asset_profile_images_id.Value);

                                    if (get_img != null)
                                    {
                                        get_img.is_deleted = true;
                                        get_img.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                        get_img.modified_at = DateTime.UtcNow;

                                        await _UoW.BaseGenericRepository<AssetProfileImages>().Update(get_img);
                                        _UoW.SaveChanges();
                                    }
                                }
                            }


                        }
                    }

                }


                ////  Add/Update parent mappings and children (FedBy Mappings)
                ///
                if (requestModel.asset_parent_mapping_list != null && requestModel.asset_parent_mapping_list.Count > 0)
                {

                    var new_added_parent = requestModel.asset_parent_mapping_list.Where(x => x.asset_parent_hierrachy_id == null).ToList();
                    foreach (var new_parent in new_added_parent)
                    {
                        Guid? ocp_asset_id = null; // if subcomponent assets are just added then find id by name,classcode
                        if (new_parent.via_subcomponent_asset_id == null || new_parent.via_subcomponent_asset_id == Guid.Empty)
                        {
                            var this_asset2 = _UoW.AssetRepository.GetAssetByNameClassCode(new_parent.via_subcomponent_asset_name, new_parent.via_subcomponant_asset_class_code);
                            if (this_asset2 != null) { ocp_asset_id = this_asset2.asset_id; }
                        }
                        AssetParentHierarchyMapping AssetParentHierarchyMapping = new AssetParentHierarchyMapping();
                        AssetParentHierarchyMapping.parent_asset_id = new_parent.parent_asset_id;
                        AssetParentHierarchyMapping.asset_id = get_asset.asset_id;
                        AssetParentHierarchyMapping.fed_by_usage_type_id = new_parent.fed_by_usage_type_id; // set default as normal
                        AssetParentHierarchyMapping.via_subcomponent_asset_id = new_parent.via_subcomponent_asset_id != null ? new_parent.via_subcomponent_asset_id : ocp_asset_id;
                        AssetParentHierarchyMapping.fed_by_via_subcomponant_asset_id = new_parent.fed_by_via_subcomponant_asset_id;
                        AssetParentHierarchyMapping.length = new_parent.length;
                        AssetParentHierarchyMapping.style = new_parent.style;
                        AssetParentHierarchyMapping.conductor_type_id = new_parent.conductor_type_id;
                        AssetParentHierarchyMapping.raceway_type_id = new_parent.raceway_type_id;
                        AssetParentHierarchyMapping.number_of_conductor = new_parent.number_of_conductor;

                        AssetParentHierarchyMapping.created_at = DateTime.UtcNow;
                        AssetParentHierarchyMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        AssetParentHierarchyMapping.is_deleted = false;
                        AssetParentHierarchyMapping.site_id = get_asset.site_id;
                        //AssetParentHierarchyMapping.fed_by_usage_type_id = 1;

                        var insert_parent = await _UoW.BaseGenericRepository<AssetParentHierarchyMapping>().Insert(AssetParentHierarchyMapping);

                        AssetChildrenHierarchyMapping AssetChildrenHierarchyMapping = new AssetChildrenHierarchyMapping();
                        AssetChildrenHierarchyMapping.children_asset_id = get_asset.asset_id;
                        AssetChildrenHierarchyMapping.asset_id = new_parent.parent_asset_id;
                        AssetChildrenHierarchyMapping.via_subcomponent_asset_id = new_parent.fed_by_via_subcomponant_asset_id;// new_parent.via_subcomponent_asset_id != null ? new_parent.via_subcomponent_asset_id : ocp_asset_id;
                        // assign circuit of fed by subcopnat
                        if (new_parent.fed_by_via_subcomponant_asset_id != null && new_parent.fed_by_via_subcomponant_asset_id.Value != Guid.Empty)
                        {
                            var get_subcomponent_mapping = _UoW.AssetRepository.GetSubcomponentMapping(new_parent.fed_by_via_subcomponant_asset_id.Value);
                            if (get_subcomponent_mapping != null)
                            {
                                AssetChildrenHierarchyMapping.circuit = get_subcomponent_mapping.circuit;
                            }

                        }

                        AssetChildrenHierarchyMapping.created_at = DateTime.UtcNow;
                        AssetChildrenHierarchyMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        AssetChildrenHierarchyMapping.is_deleted = false;
                        AssetChildrenHierarchyMapping.site_id = get_asset.site_id;

                        var insert_child = await _UoW.BaseGenericRepository<AssetChildrenHierarchyMapping>().Insert(AssetChildrenHierarchyMapping);

                    }

                    //update parent
                    var updated_parents = requestModel.asset_parent_mapping_list.Where(x => x.asset_parent_hierrachy_id != null && !x.is_deleted).ToList();
                    foreach (var updated_parent in updated_parents)
                    {
                        Guid? ocp_asset_id = null; // if subcomponent assets are just added then find id by name,classcode
                        if (updated_parent.via_subcomponent_asset_id == null || updated_parent.via_subcomponent_asset_id == Guid.Empty)
                        {
                            var this_asset2 = _UoW.AssetRepository.GetAssetByNameClassCode(updated_parent.via_subcomponent_asset_name, updated_parent.via_subcomponant_asset_class_code);
                            if (this_asset2 != null) { ocp_asset_id = this_asset2.asset_id; }
                        }
                        var parent_maping = get_asset.AssetParentHierarchyMapping.Where(x => x.parent_asset_id == updated_parent.parent_asset_id && !x.is_deleted).FirstOrDefault();
                        if (parent_maping != null)
                        {
                            parent_maping.is_deleted = updated_parent.is_deleted;
                            parent_maping.fed_by_usage_type_id = updated_parent.fed_by_usage_type_id;
                            parent_maping.via_subcomponent_asset_id = updated_parent.via_subcomponent_asset_id != null ? updated_parent.via_subcomponent_asset_id : ocp_asset_id;
                            parent_maping.fed_by_via_subcomponant_asset_id = updated_parent.fed_by_via_subcomponant_asset_id;
                            parent_maping.length = updated_parent.length;
                            parent_maping.style = updated_parent.style;
                            parent_maping.conductor_type_id = updated_parent.conductor_type_id;
                            parent_maping.raceway_type_id = updated_parent.raceway_type_id;
                            parent_maping.number_of_conductor = updated_parent.number_of_conductor;
                            parent_maping.updated_at = DateTime.UtcNow;
                            parent_maping.updated_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                            var update_parent = await _UoW.BaseGenericRepository<AssetParentHierarchyMapping>().Update(parent_maping);
                        }

                        var childs_of_this_parent = _UoW.AssetRepository.GetChildByAssetId(updated_parent.parent_asset_id.Value);
                        var child = childs_of_this_parent.Where(x => x.children_asset_id == get_asset.asset_id).FirstOrDefault();
                        if (child != null)
                        {
                            child.is_deleted = updated_parent.is_deleted;
                            child.via_subcomponent_asset_id = parent_maping.fed_by_via_subcomponant_asset_id; // updated_parent.via_subcomponent_asset_id != null ? updated_parent.via_subcomponent_asset_id : ocp_asset_id;
                            // assign circuit of fed by subcopnat
                            if (parent_maping.fed_by_via_subcomponant_asset_id != null && parent_maping.fed_by_via_subcomponant_asset_id.Value != Guid.Empty)
                            {
                                var get_subcomponent_mapping = _UoW.AssetRepository.GetSubcomponentMapping(parent_maping.fed_by_via_subcomponant_asset_id.Value);
                                if (get_subcomponent_mapping != null)
                                {
                                    child.circuit = get_subcomponent_mapping.circuit;
                                }

                            }
                            child.updated_at = DateTime.UtcNow;
                            child.updated_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                            var update_child = await _UoW.BaseGenericRepository<AssetChildrenHierarchyMapping>().Update(child);
                        }

                    }

                    // delete parent
                    var deleted_parents = requestModel.asset_parent_mapping_list.Where(x => x.asset_parent_hierrachy_id != null && x.is_deleted).ToList();
                    foreach (var deleted_parent in deleted_parents)
                    {
                        var parent_maping = get_asset.AssetParentHierarchyMapping.Where(x => x.parent_asset_id == deleted_parent.parent_asset_id && !x.is_deleted).FirstOrDefault();
                        if (parent_maping != null)
                        {
                            parent_maping.is_deleted = true;

                            var update = await _UoW.BaseGenericRepository<AssetParentHierarchyMapping>().Update(parent_maping);
                        }

                        // delete from child
                        var childs_of_this_parent = _UoW.AssetRepository.GetChildByAssetId(deleted_parent.parent_asset_id.Value);
                        var child = childs_of_this_parent.Where(x => x.children_asset_id == get_asset.asset_id).FirstOrDefault();
                        if (child != null)
                        {
                            child.is_deleted = true;
                            var update = await _UoW.BaseGenericRepository<AssetChildrenHierarchyMapping>().Update(child);
                        }
                    }

                    _UoW.SaveChanges();
                }



                if (get_asset.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent && get_asset.AssetSubLevelcomponentMapping != null)
                {
                    foreach (var subLevelAsset in get_asset.AssetSubLevelcomponentMapping)
                    {
                        if (!subLevelAsset.is_deleted)
                        {
                            var get_subLevel_asset = _UoW.AssetRepository.GetSubLevelAssetById(subLevelAsset.sublevelcomponent_asset_id);

                            if (requestModel.formiobuilding_id > 0 && requestModel.formiofloor_id > 0 && requestModel.formioroom_id > 0)
                            {
                                get_subLevel_asset.AssetFormIOBuildingMappings.formiobuilding_id = requestModel.formiobuilding_id;
                                get_subLevel_asset.AssetFormIOBuildingMappings.formiofloor_id = requestModel.formiofloor_id;
                                get_subLevel_asset.AssetFormIOBuildingMappings.formioroom_id = requestModel.formioroom_id;
                            }
                            else
                            {
                                if (FormIOBuildings != null)
                                    get_subLevel_asset.AssetFormIOBuildingMappings.formiobuilding_id = FormIOBuildings.formiobuilding_id;
                                if (FormIOFloors != null)
                                    get_subLevel_asset.AssetFormIOBuildingMappings.formiofloor_id = FormIOFloors.formiofloor_id;
                                if (FormIORooms != null)
                                    get_subLevel_asset.AssetFormIOBuildingMappings.formioroom_id = FormIORooms.formioroom_id;
                            }
                            if (FormIOSections != null)
                                get_subLevel_asset.AssetFormIOBuildingMappings.formiosection_id = FormIOSections.formiosection_id;


                            var update2 = await _UoW.BaseGenericRepository<Asset>().Update(get_subLevel_asset);
                            _UoW.SaveChanges();
                        }
                    }
                }

                response_model.status = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response_model.status = (int)ResponseStatusNumber.NotFound;
                //response = (int)ResponseStatusNumber.NotFound;
            }

            return (response_model);
        }

        public async Task<int> MarkAssetStatusAsInActiveDispose(Guid asset_id)
        {
            try
            {
                var get_asset = _UoW.AssetRepository.GetAssetByAssetID(asset_id.ToString());

                if (get_asset != null)
                {
                    if (get_asset.component_level_type_id == (int)ComponentLevelTypes.ToplevelComponent)
                    {
                        AssetParentHierarchyMapping fedby = null;
                        if (get_asset.AssetParentHierarchyMapping != null && get_asset.AssetParentHierarchyMapping.Count > 0)
                        {
                            fedby = get_asset.AssetParentHierarchyMapping.Where(x => !x.is_deleted).FirstOrDefault();
                        }

                        var parentmapping_byChildAssetId = _UoW.AssetRepository.GetParentAssetByAssetId(asset_id);
                        var childmapping_byChildAssetId = _UoW.AssetRepository.GetChildrenMappingByChildAssetId(asset_id);

                        var parentmapping_byParentAssetId = _UoW.AssetRepository.GetParentMappingByParentAssetId(asset_id);
                        var childmapping_byParentAssetId = _UoW.AssetRepository.GetChildByAssetId(asset_id);

                        if (fedby != null)
                        {
                            get_asset.AssetParentHierarchyMapping.FirstOrDefault().is_deleted = true;

                            var update2 = await _UoW.BaseGenericRepository<Asset>().Update(get_asset);
                            _UoW.SaveChanges();

                            foreach (var child in parentmapping_byParentAssetId)
                            {
                                AssetParentHierarchyMapping AssetParentHierarchyMapping = new AssetParentHierarchyMapping();
                                AssetParentHierarchyMapping.parent_asset_id = fedby.parent_asset_id;
                                AssetParentHierarchyMapping.asset_id = child.asset_id;
                                AssetParentHierarchyMapping.fed_by_usage_type_id = 1; // set default as normal
                                AssetParentHierarchyMapping.created_at = DateTime.UtcNow;
                                AssetParentHierarchyMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                AssetParentHierarchyMapping.is_deleted = false;
                                AssetParentHierarchyMapping.site_id = get_asset.site_id;

                                var insert_parent = await _UoW.BaseGenericRepository<AssetParentHierarchyMapping>().Insert(AssetParentHierarchyMapping);
                                _UoW.SaveChanges();
                            }

                            foreach (var child in childmapping_byParentAssetId)
                            {
                                AssetChildrenHierarchyMapping AssetChildrenHierarchyMapping = new AssetChildrenHierarchyMapping();
                                AssetChildrenHierarchyMapping.children_asset_id = child.children_asset_id;
                                AssetChildrenHierarchyMapping.asset_id = fedby.parent_asset_id;
                                AssetChildrenHierarchyMapping.created_at = DateTime.UtcNow;
                                AssetChildrenHierarchyMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                AssetChildrenHierarchyMapping.is_deleted = false;
                                AssetChildrenHierarchyMapping.site_id = get_asset.site_id;

                                var insert_child = await _UoW.BaseGenericRepository<AssetChildrenHierarchyMapping>().Insert(AssetChildrenHierarchyMapping);
                                _UoW.SaveChanges();
                            }

                            var asset_name = _UoW.AssetRepository.GetAssetNameByAssetId(fedby.parent_asset_id.Value);
                            AddUpdateAssetNotesRequestmodel addUpdateAssetNotesRequestmodel = new AddUpdateAssetNotesRequestmodel();
                            addUpdateAssetNotesRequestmodel.asset_id = asset_id;
                            addUpdateAssetNotesRequestmodel.asset_note = "previously fed by " + asset_name + " feeder length 25";
                            await AddUpdateAssetNotes(addUpdateAssetNotesRequestmodel);
                        }

                        //if asset has FedBy Asset
                        foreach (var item in parentmapping_byChildAssetId)
                        {
                            item.is_deleted = true;

                            var update2 = await _UoW.BaseGenericRepository<AssetParentHierarchyMapping>().Update(item);
                            _UoW.SaveChanges();
                        }
                        //if asset has FedBy Asset
                        foreach (var item in childmapping_byChildAssetId)
                        {
                            item.is_deleted = true;

                            var update2 = await _UoW.BaseGenericRepository<AssetChildrenHierarchyMapping>().Update(item);
                            _UoW.SaveChanges();
                        }

                        // if Asset has Child Assets
                        foreach (var item in parentmapping_byParentAssetId)
                        {
                            item.is_deleted = true;

                            var update2 = await _UoW.BaseGenericRepository<AssetParentHierarchyMapping>().Update(item);
                            _UoW.SaveChanges();
                        }
                        // if Asset has Child Assets
                        foreach (var item in childmapping_byParentAssetId)
                        {
                            item.is_deleted = true;

                            var update2 = await _UoW.BaseGenericRepository<AssetChildrenHierarchyMapping>().Update(item);
                            _UoW.SaveChanges();
                        }

                        var subcomponents_mappings = get_asset.AssetSubLevelcomponentMapping.Where(x => !x.is_deleted).ToList();
                        var toplevel_mappings = _UoW.AssetRepository.GetToplevelMappingsListByTopLevelAssetId(asset_id);

                        foreach (var item in subcomponents_mappings)
                        {
                            item.is_deleted = true;

                            var update2 = await _UoW.BaseGenericRepository<AssetSubLevelcomponentMapping>().Update(item);
                            _UoW.SaveChanges();
                        }

                        foreach (var item in toplevel_mappings)
                        {
                            item.is_deleted = true;

                            var update2 = await _UoW.BaseGenericRepository<AssetTopLevelcomponentMapping>().Update(item);
                            _UoW.SaveChanges();
                        }
                    }
                    else if (get_asset.component_level_type_id == (int)ComponentLevelTypes.SublevelComponent)
                    {
                        if (get_asset.AssetTopLevelcomponentMapping.Where(x => !x.is_deleted) != null && get_asset.AssetTopLevelcomponentMapping.Where(x => !x.is_deleted).Count() > 0)
                        {
                            var asset_name = _UoW.AssetRepository.GetAssetNameByAssetId(get_asset.AssetTopLevelcomponentMapping.Where(x => !x.is_deleted).FirstOrDefault().toplevelcomponent_asset_id);
                            AddUpdateAssetNotesRequestmodel addUpdateAssetNotesRequestmodel = new AddUpdateAssetNotesRequestmodel();
                            addUpdateAssetNotesRequestmodel.asset_id = asset_id;
                            addUpdateAssetNotesRequestmodel.asset_note = "previously part of " + asset_name + " asset";
                            await AddUpdateAssetNotes(addUpdateAssetNotesRequestmodel);

                            var sublevel_map = _UoW.AssetRepository.GetAssetsSubcomponentbyid(get_asset.AssetTopLevelcomponentMapping.Where(x => !x.is_deleted).FirstOrDefault().toplevelcomponent_asset_id, asset_id);
                            if (sublevel_map != null)
                            {
                                sublevel_map.is_deleted = true;
                                var update3 = await _UoW.BaseGenericRepository<AssetSubLevelcomponentMapping>().Update(sublevel_map);
                                _UoW.SaveChanges();
                            }

                            get_asset.AssetTopLevelcomponentMapping.Where(x => !x.is_deleted).FirstOrDefault().is_deleted = true;
                            var update2 = await _UoW.BaseGenericRepository<Asset>().Update(get_asset);
                            _UoW.SaveChanges();

                            var get_parent_mappingsbyOCP = _UoW.AssetRepository.GetParentMappingsByOcpOcpMainId(asset_id);
                            foreach (var item in get_parent_mappingsbyOCP)
                            {
                                if (item.via_subcomponent_asset_id == asset_id)
                                    item.via_subcomponent_asset_id = null;
                                else if (item.fed_by_via_subcomponant_asset_id == asset_id)
                                    item.fed_by_via_subcomponant_asset_id = null;
                                var update3 = await _UoW.BaseGenericRepository<AssetParentHierarchyMapping>().Update(item);
                                _UoW.SaveChanges();
                            }
                        }
                    }

                    if (get_asset.AssetPMs != null && get_asset.AssetPMs.Count > 0)
                    {
                        foreach (var item in get_asset.AssetPMs.Where(x => !x.is_archive))
                        {
                            item.is_archive = true;
                            item.modified_at = DateTime.UtcNow;

                            var update2 = await _UoW.BaseGenericRepository<AssetPMs>().Update(item);
                            _UoW.SaveChanges();
                        }
                    }
                    if (get_asset.AssetIssue != null && get_asset.AssetIssue.Count > 0)
                    {
                        foreach (var item in get_asset.AssetIssue.Where(x => !x.is_deleted))
                        {
                            item.is_deleted = true;
                            item.modified_at = DateTime.UtcNow;

                            var update2 = await _UoW.BaseGenericRepository<AssetIssue>().Update(item);
                            _UoW.SaveChanges();
                        }
                    }

                    if (get_asset.AssetFormIO != null && get_asset.AssetFormIO.Count > 0)
                    {
                        foreach (var item in get_asset.AssetFormIO.Where(x => x.status != (int)Status.Deactive))
                        {
                            item.status = (int)Status.Deactive;
                            item.modified_at = DateTime.UtcNow;
                            if (item.WOcategorytoTaskMapping != null && item.WOcategorytoTaskMapping.WOInspectionsTemplateFormIOAssignment != null)
                            {
                                item.WOcategorytoTaskMapping.is_archived = true;
                                item.WOcategorytoTaskMapping.updated_at = DateTime.UtcNow;
                                item.WOcategorytoTaskMapping.WOInspectionsTemplateFormIOAssignment.is_archived = true;
                                item.WOcategorytoTaskMapping.WOInspectionsTemplateFormIOAssignment.updated_at = DateTime.UtcNow;
                            }
                            var update2 = await _UoW.BaseGenericRepository<AssetFormIO>().Update(item);
                            _UoW.SaveChanges();
                        }
                    }

                }

            }
            catch (Exception e)
            {
            }
            return 1;
        }

        public int GenertaeExcel(string assetlist)
        {
            ExcelPackage ExcelPkg = new ExcelPackage();
            ExcelWorksheet wsSheet1 = ExcelPkg.Workbook.Worksheets.Add("Sheet1");
            using (ExcelRange Rng = wsSheet1.Cells[2, 2, 2, 2])
            {
                Rng.Value = "Welcome to Everyday be coding - tutorials for beginners";
                //Rng.Merge = true;  
                Rng.Style.Font.Size = 16;
                Rng.Style.Font.Bold = true;
                Rng.Style.Font.Italic = true;
            }
            int rowIndex = 4;
            int colIndex = 2;
            int PixelTop = 88;
            int PixelLeft = 129;
            int Height = 320;
            int Width = 200;
            /*System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(assetlist.asset[0].asset_barcode_image);
            webRequest.AllowWriteStreamBuffering = true;
            webRequest.Timeout = 30000;

            System.Net.WebResponse webResponse = webRequest.GetResponse();

            System.IO.Stream stream = webResponse.GetResponseStream();
            Image img = Image.FromStream(stream);
            */
            ExcelPicture pic = wsSheet1.Drawings.AddPicture("imag", assetlist);
            pic.SetPosition(rowIndex, 0, colIndex, 0);
            //pic.SetPosition(PixelTop, PixelLeft);  
            pic.SetSize(Height, Width);
            //pic.SetSize(40);  
            wsSheet1.Protection.IsProtected = false;
            wsSheet1.Protection.AllowSelectLockedCells = false;
            ExcelPkg.SaveAs(new FileInfo(@"D:\New.xlsx"));

            return 1;
        }

        public ListViewModel<GetAssetAttachmentsResponsemodel> GetAssetAttachments(GetAssetAttachmentsRequestmodel requestModel)
        {
            ListViewModel<GetAssetAttachmentsResponsemodel> response = new ListViewModel<GetAssetAttachmentsResponsemodel>();

            var get_asset_attachments = _UoW.AssetRepository.GetAssetAttachments(requestModel);

            response.list = _mapper.Map<List<GetAssetAttachmentsResponsemodel>>(get_asset_attachments.Item1);

            response.listsize = get_asset_attachments.Item2;
            response.pageSize = requestModel.pagesize;
            response.pageIndex = requestModel.pageindex;

            return response;

        }
        public async Task<int> DeleteAssetAttachments(DeleteAssetAttachmentsRequestmodel requestmodel)
        {
            int result = (int)ResponseStatusNumber.Error;

            var attachmewnt = _UoW.AssetRepository.GetAssetAttachmentById(requestmodel.assetatachmentmapping_id);

            attachmewnt.is_deleted = true;
            attachmewnt.modified_at = DateTime.UtcNow;
            attachmewnt.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

            var update = await _UoW.BaseGenericRepository<AssetAttachmentMapping>().Update(attachmewnt);
            if (update)
            {
                _UoW.SaveChanges();
                result = (int)ResponseStatusNumber.Success;
            }

            return result;
        }
        public ListViewModel<GetSubcomponentsByAssetIdResponsemodel> GetSubcomponentsByAssetId(GetSubcomponentsByAssetIdRequestmodel requestmodel)
        {
            ListViewModel<GetSubcomponentsByAssetIdResponsemodel> response = new ListViewModel<GetSubcomponentsByAssetIdResponsemodel>();

            var get_asset_subcomponent = _UoW.AssetRepository.GetSubcomponentsByAssetId(requestmodel);

            response.list = _mapper.Map<List<GetSubcomponentsByAssetIdResponsemodel>>(get_asset_subcomponent.Item1);


            // if request is from circuit tab then remove subcomponents which are already set fro via subcomponent
            if (requestmodel.asset_children_hierrachy_id != null && requestmodel.asset_children_hierrachy_id != Guid.Empty)
            {
                var get_linked_subcomponents = _UoW.AssetRepository.GetAssetsLinkedSubcomponents(requestmodel.asset_id, requestmodel.asset_children_hierrachy_id.Value);


                response.list = response.list.Where(x => !get_linked_subcomponents.Contains(x.sublevelcomponent_asset_id)).ToList();
            }

            foreach (var list in response.list)
            {
                var get_subcomponent_details = _UoW.AssetRepository.GetsubcomonentAssetDetail(list.sublevelcomponent_asset_id);
                list.sublevelcomponent_asset_name = get_subcomponent_details.name;
                if (get_subcomponent_details.inspectiontemplate_asset_class_id != null)
                {
                    list.sublevelcomponent_asset_class_id = get_subcomponent_details.inspectiontemplate_asset_class_id.Value;
                    list.sublevelcomponent_asset_class_name = get_subcomponent_details.InspectionTemplateAssetClass.asset_class_name;
                    list.sublevelcomponent_asset_class_code = get_subcomponent_details.InspectionTemplateAssetClass.asset_class_code;
                }

                // get amp rating from nameplateinfo
                try
                {
                    dynamic dynamicform1 = Newtonsoft.Json.JsonConvert.DeserializeObject(get_subcomponent_details.form_retrived_nameplate_info);
                    list.rated_amps = dynamicform1.ampereRating;
                }
                catch (Exception ex)
                {

                }
            }
            response.listsize = get_asset_subcomponent.Item2;
            response.pageSize = requestmodel.pagesize;
            response.pageIndex = requestmodel.pageindex;
            return response;

        }
        public async Task<int> UpdateCircuitForAssetSubcomponent(UpdateCircuitForAssetSubcomponentRequestmodel requestmodel)
        {
            int result = (int)ResponseStatusNumber.Error;

            var sublevelcomponent = _UoW.AssetRepository.UpdateCircuitForAssetSubcomponent(requestmodel.asset_sublevelcomponent_mapping_id);

            sublevelcomponent.circuit = requestmodel.circuit;
            sublevelcomponent.updated_at = DateTime.UtcNow;
            sublevelcomponent.updated_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

            var update = await _UoW.BaseGenericRepository<AssetSubLevelcomponentMapping>().Update(sublevelcomponent);
            if (update)
            {
                _UoW.SaveChanges();

                // update in feeding table if this subcomponent is added as OCP 
                var get_child_by_ocp = _UoW.AssetRepository.GetAssetChildrenByOCP(sublevelcomponent.sublevelcomponent_asset_id);
                foreach (var child in get_child_by_ocp)
                {
                    child.circuit = requestmodel.circuit;
                    child.updated_at = DateTime.UtcNow;
                    update = await _UoW.BaseGenericRepository<AssetChildrenHierarchyMapping>().Update(child);
                }
                result = (int)ResponseStatusNumber.Success;
            }

            return result;
        }

        public async Task<int> DeleteAssetSubcomponent(DeleteAssetSubcomponentRequestmodel requestmodel)
        {
            int result = (int)ResponseStatusNumber.Error;

            var getsubcomponent = _UoW.AssetRepository.DeleteAssetSubcomponent(requestmodel.asset_sublevelcomponent_mapping_id);

            getsubcomponent.is_deleted = true;
            getsubcomponent.updated_at = DateTime.UtcNow;
            getsubcomponent.updated_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

            var update = await _UoW.BaseGenericRepository<AssetSubLevelcomponentMapping>().Update(getsubcomponent);

            // delete subcomponents top level also
            var get_subcomponentstoplevel = _UoW.AssetRepository.GetToplevelmappingofSubcomponent(getsubcomponent.sublevelcomponent_asset_id);
            if (get_subcomponentstoplevel != null)
            {
                get_subcomponentstoplevel.is_deleted = true;
                get_subcomponentstoplevel.updated_at = DateTime.UtcNow;
                get_subcomponentstoplevel.updated_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                update = await _UoW.BaseGenericRepository<AssetTopLevelcomponentMapping>().Update(get_subcomponentstoplevel);
            }

            // delete via subcomponent also from fed by or feeding 
            var get_fed_via_sub = _UoW.AssetRepository.GetfedbyviaSubcomponent(getsubcomponent.sublevelcomponent_asset_id);
            foreach (var item in get_fed_via_sub)
            {
                item.via_subcomponent_asset_id = null;
                item.updated_at = DateTime.UtcNow;
                var update_fed_by = await _UoW.BaseGenericRepository<AssetParentHierarchyMapping>().Update(item);
                _UoW.SaveChanges();
            }

            var get_feeding_via_sub = _UoW.AssetRepository.GetfeedingviaSubcomponent(getsubcomponent.sublevelcomponent_asset_id);
            foreach (var item in get_feeding_via_sub)
            {
                item.via_subcomponent_asset_id = null;
                item.updated_at = DateTime.UtcNow;
                var update_feeding = await _UoW.BaseGenericRepository<AssetChildrenHierarchyMapping>().Update(item);
                _UoW.SaveChanges();
            }
            if (update)
            {
                _UoW.SaveChanges();
                result = (int)ResponseStatusNumber.Success;
            }

            return result;
        }

        public ListViewModel<GetSubcomponentAssetsToAddinAssetResponsemodel> GetSubcomponentAssetsToAddinAsset()
        {
            ListViewModel<GetSubcomponentAssetsToAddinAssetResponsemodel> response = new ListViewModel<GetSubcomponentAssetsToAddinAssetResponsemodel>();

            var get_asset_subcomponent = _UoW.AssetRepository.GetSubcomponentAssetsToAddinAsset();

            response.list = _mapper.Map<List<GetSubcomponentAssetsToAddinAssetResponsemodel>>(get_asset_subcomponent);
            response.listsize = get_asset_subcomponent.Count();
            response.pageSize = 0;
            response.pageIndex = 0;
            return response;

        }

        public async Task<int> AddNewSubComponent(AddNewSubComponentRequestmodel requestmodel)
        {
            int response = (int)ResponseStatusNumber.Error;
            var get_main_asset = _UoW.AssetRepository.GetAssetByIdForNewSubcomponent(requestmodel.asset_id);
            Guid? sub_component_asset = requestmodel.sublevelcomponent_asset_id;
            try
            {
                _UoW.BeginTransaction();
                if (!requestmodel.is_subcomponent_for_existing) // create new asset and then assign subcomponent
                {
                    Asset asset = new Asset();
                    asset.name = requestmodel.sublevelcomponent_asset_name;
                    asset.inspectiontemplate_asset_class_id = requestmodel.inspectiontemplate_asset_class_id;
                    asset.component_level_type_id = (int)ComponentLevelTypes.SublevelComponent;
                    asset.status = (int)Status.AssetActive;
                    asset.asset_operating_condition_state = (int)AssetOperatingConduitionState.Operating_Normally;
                    asset.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                    asset.company_id = UpdatedGenericRequestmodel.CurrentUser.company_id;
                    asset.created_at = DateTime.UtcNow;

                    asset.AssetFormIOBuildingMappings = new AssetFormIOBuildingMappings();
                    asset.AssetFormIOBuildingMappings.formiobuilding_id = get_main_asset.AssetFormIOBuildingMappings.formiobuilding_id;
                    asset.AssetFormIOBuildingMappings.formiofloor_id = get_main_asset.AssetFormIOBuildingMappings.formiofloor_id;
                    asset.AssetFormIOBuildingMappings.formioroom_id = get_main_asset.AssetFormIOBuildingMappings.formioroom_id;
                    asset.AssetFormIOBuildingMappings.formiosection_id = get_main_asset.AssetFormIOBuildingMappings.formiosection_id;

                    var get_asset_class = _UoW.FormIOAssetClassRepository.GetAssetclassbyIDForNameplateinfo(requestmodel.inspectiontemplate_asset_class_id.Value);
                    WorkOrderService woservice = new WorkOrderService(_mapper);
                    asset.form_retrived_nameplate_info = woservice.AddNamePlateListData(get_asset_class.form_nameplate_info);

                    asset.internal_asset_id = get_main_asset.internal_asset_id + " - " + (get_main_asset.AssetSubLevelcomponentMapping.Count + 1).ToString();
                    asset.QR_code = get_main_asset.internal_asset_id + " - " + (get_main_asset.AssetSubLevelcomponentMapping.Count + 1).ToString();

                    var insert = await _UoW.BaseGenericRepository<Asset>().Insert(asset);
                    _UoW.SaveChanges();

                    sub_component_asset = asset.asset_id;
                }
                else
                {
                    //existing
                    var get_subLevel_asset = _UoW.AssetRepository.GetSubLevelAssetById(requestmodel.sublevelcomponent_asset_id.Value);
                    get_subLevel_asset.AssetFormIOBuildingMappings.formiobuilding_id = get_main_asset.AssetFormIOBuildingMappings.formiobuilding_id;
                    get_subLevel_asset.AssetFormIOBuildingMappings.formiofloor_id = get_main_asset.AssetFormIOBuildingMappings.formiofloor_id;
                    get_subLevel_asset.AssetFormIOBuildingMappings.formioroom_id = get_main_asset.AssetFormIOBuildingMappings.formioroom_id;
                    get_subLevel_asset.AssetFormIOBuildingMappings.formiosection_id = get_main_asset.AssetFormIOBuildingMappings.formiosection_id;

                    get_subLevel_asset.internal_asset_id = get_main_asset.internal_asset_id + " - " + (get_main_asset.AssetSubLevelcomponentMapping.Count + 1).ToString();

                    var update2 = await _UoW.BaseGenericRepository<Asset>().Update(get_subLevel_asset);
                    _UoW.SaveChanges();
                }
                // insert top level and subcomponent

                AssetSubLevelcomponentMapping AssetSubLevelcomponentMapping = new AssetSubLevelcomponentMapping();
                AssetSubLevelcomponentMapping.asset_id = get_main_asset.asset_id;
                AssetSubLevelcomponentMapping.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                AssetSubLevelcomponentMapping.sublevelcomponent_asset_id = sub_component_asset.Value;
                AssetSubLevelcomponentMapping.circuit = requestmodel.circuit;
                AssetSubLevelcomponentMapping.created_at = DateTime.UtcNow;
                AssetSubLevelcomponentMapping.is_deleted = false;

                var insert_subcomponant = await _UoW.BaseGenericRepository<AssetSubLevelcomponentMapping>().Insert(AssetSubLevelcomponentMapping);
                _UoW.SaveChanges();

                AssetTopLevelcomponentMapping AssetTopLevelcomponentMapping = new AssetTopLevelcomponentMapping();
                AssetTopLevelcomponentMapping.asset_id = sub_component_asset.Value;
                AssetTopLevelcomponentMapping.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                AssetTopLevelcomponentMapping.toplevelcomponent_asset_id = get_main_asset.asset_id;
                AssetTopLevelcomponentMapping.created_at = DateTime.UtcNow;
                AssetTopLevelcomponentMapping.is_deleted = false;

                var insert_toplevel = await _UoW.BaseGenericRepository<AssetTopLevelcomponentMapping>().Insert(AssetTopLevelcomponentMapping);
                _UoW.SaveChanges();


                //--- As of now this code is Added here (BE purpose only)
                //--- bcz this API is used in EditAssetDetails for Add Subcomponents with AssetImages
                // Add/Delete Subcomponent Asset Images NewFlow
                if (requestmodel.subcomponentasset_image_list != null && requestmodel.subcomponentasset_image_list.Count > 0)
                {
                    var new_added_images_list = requestmodel.subcomponentasset_image_list.Where(x => x.asset_profile_images_id == null && !x.is_deleted).ToList();

                    foreach (var image in new_added_images_list)
                    {
                        AssetProfileImages AssetProfileImages = new AssetProfileImages();
                        AssetProfileImages.asset_photo = image.asset_photo;
                        AssetProfileImages.asset_thumbnail_photo = image.asset_thumbnail_photo;

                        AssetProfileImages.asset_id = sub_component_asset.Value;
                        AssetProfileImages.asset_photo_type = image.asset_photo_type;
                        AssetProfileImages.created_at = DateTime.UtcNow;
                        AssetProfileImages.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        await _UoW.BaseGenericRepository<AssetProfileImages>().Insert(AssetProfileImages);
                        _UoW.SaveChanges();
                    }

                    var deleted_images_list = requestmodel.subcomponentasset_image_list.Where(x => x.asset_profile_images_id != null && x.is_deleted == true).ToList();

                    foreach (var image in deleted_images_list)
                    {
                        var get_img = _UoW.AssetRepository.GetAssetImagebyID(image.asset_profile_images_id.Value);

                        if (get_img != null)
                        {
                            get_img.is_deleted = true;
                            get_img.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            get_img.modified_at = DateTime.UtcNow;

                            await _UoW.BaseGenericRepository<AssetProfileImages>().Update(get_img);
                            _UoW.SaveChanges();
                        }
                    }
                }

                _UoW.CommitTransaction();

                if (!requestmodel.is_subcomponent_for_existing && requestmodel.inspectiontemplate_asset_class_id != null)
                {
                    var inspectionTemplateAssetClass = _UoW.WorkOrderRepository.GetInspectionTemplateAssetClass(requestmodel.inspectiontemplate_asset_class_id.Value);
                    // insert default asset pm
                    if (inspectionTemplateAssetClass != null && inspectionTemplateAssetClass.PMCategory != null
                     && inspectionTemplateAssetClass.PMCategory.PMPlans != null && inspectionTemplateAssetClass.PMCategory.PMPlans.Count > 0)
                    {
                        var get_default_pm_plan = inspectionTemplateAssetClass.PMCategory.PMPlans.Where(x => x.is_default_pm_plan).FirstOrDefault();
                        if (get_default_pm_plan != null)
                        {
                            AssignPMToAsset AssignPMToAsset = new AssignPMToAsset();
                            AssignPMToAsset.asset_id = sub_component_asset.Value;
                            AssignPMToAsset.pm_plan_id = get_default_pm_plan.pm_plan_id;

                            AssetPMService assetpm = new AssetPMService(_mapper);
                            var assignpm = await assetpm.AddAssetPM(AssignPMToAsset);
                        }
                    }
                }

                response = (int)ResponseStatusNumber.Success;
            }
            catch (Exception ex)
            {
                _UoW.RollbackTransaction();
            }

            return response;
        }

        public GetAssetCircuitDetailsResponsemodel GetAssetCircuitDetails(GetAssetCircuitDetailsRequestmodel requestmodel)
        {
            GetAssetCircuitDetailsResponsemodel response = new GetAssetCircuitDetailsResponsemodel();

            var get_parent_assets = _UoW.AssetRepository.GetParentAssetByAssetId(requestmodel.asset_id); // get fed by assets
            if (get_parent_assets.Count > 0)
            {
                response.fedby_asset_list = new List<FedByAssetsResponse>();
                foreach (var parent_asset in get_parent_assets)
                {
                    FedByAssetsResponse FedByAssetsResponse = new FedByAssetsResponse();
                    FedByAssetsResponse.asset_parent_hierrachy_id = parent_asset.asset_parent_hierrachy_id;
                    FedByAssetsResponse.parent_asset_id = parent_asset.parent_asset_id;
                    FedByAssetsResponse.via_subcomponent_asset_id = parent_asset.via_subcomponent_asset_id;
                    FedByAssetsResponse.fed_by_usage_type_id = parent_asset.fed_by_usage_type_id;
                    FedByAssetsResponse.style = parent_asset.style;
                    FedByAssetsResponse.length = parent_asset.length;
                    FedByAssetsResponse.conductor_type_id = parent_asset.conductor_type_id;
                    FedByAssetsResponse.raceway_type_id = parent_asset.raceway_type_id;
                    FedByAssetsResponse.number_of_conductor = parent_asset.number_of_conductor;
                    FedByAssetsResponse.fed_by_via_subcomponant_asset_id = parent_asset.fed_by_via_subcomponant_asset_id;

                    if (FedByAssetsResponse.parent_asset_id != null)
                    {
                        var get_via_subcomponent = _UoW.AssetRepository.GetsubcomonentAssetDetail(FedByAssetsResponse.parent_asset_id.Value);
                        FedByAssetsResponse.parent_asset_class_name = get_via_subcomponent.InspectionTemplateAssetClass.asset_class_name;
                        FedByAssetsResponse.parent_asset_name = get_via_subcomponent.name;
                        try
                        {
                            dynamic dynamicform1 = Newtonsoft.Json.JsonConvert.DeserializeObject(get_via_subcomponent.form_retrived_nameplate_info);
                            FedByAssetsResponse.amps = dynamicform1.ampereRating;
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    if (FedByAssetsResponse.via_subcomponent_asset_id != null)
                    {
                        var get_via_subcomponent = _UoW.AssetRepository.GetsubcomonentAssetDetail(FedByAssetsResponse.via_subcomponent_asset_id.Value);
                        if (get_via_subcomponent != null)
                        {
                            FedByAssetsResponse.via_subcomponent_asset_name = get_via_subcomponent.name;
                        }
                    }
                    if (FedByAssetsResponse.fed_by_via_subcomponant_asset_id != null)
                    {
                        var get_fedby_via_subcomponent = _UoW.AssetRepository.GetsubcomonentAssetDetail(FedByAssetsResponse.fed_by_via_subcomponant_asset_id.Value);
                        if (get_fedby_via_subcomponent != null)
                        {
                            FedByAssetsResponse.fed_by_via_subcomponent_asset_name = get_fedby_via_subcomponent.name;
                        }
                    }
                    response.fedby_asset_list.Add(FedByAssetsResponse);
                }
            }

            var get_children_assets = _UoW.AssetRepository.GetChildrenAssetByAssetId(requestmodel.asset_id); // get feeding  assets
            if (get_children_assets.Count > 0)
            {
                response.feeding_asset_list = new List<FeedingAssetsResponse>();
                foreach (var child_asset in get_children_assets)
                {
                    FeedingAssetsResponse FeedingAssetsResponse = new FeedingAssetsResponse();
                    FeedingAssetsResponse.asset_children_hierrachy_id = child_asset.asset_children_hierrachy_id;
                    FeedingAssetsResponse.children_asset_id = child_asset.children_asset_id;
                    FeedingAssetsResponse.via_subcomponent_asset_id = child_asset.via_subcomponent_asset_id;

                    // get circuit of via subcompoennt 
                    if (child_asset.via_subcomponent_asset_id != null)
                    {
                        var get_viasubcomepont_circuit = _UoW.AssetRepository.GetViasubcomponenentCircuit(child_asset.via_subcomponent_asset_id.Value);
                        FeedingAssetsResponse.circuit = get_viasubcomepont_circuit;
                    }


                    if (FeedingAssetsResponse.children_asset_id != null)
                    {
                        var get_children = _UoW.AssetRepository.GetsubcomonentAssetDetail(FeedingAssetsResponse.children_asset_id.Value);
                        FeedingAssetsResponse.children_asset_clss_name = get_children.InspectionTemplateAssetClass.asset_class_name;
                        FeedingAssetsResponse.children_asset_name = get_children.name;

                        try
                        {
                            dynamic dynamicform1 = Newtonsoft.Json.JsonConvert.DeserializeObject(get_children.form_retrived_nameplate_info);
                            FeedingAssetsResponse.amps = dynamicform1.ampereRating;
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    if (FeedingAssetsResponse.via_subcomponent_asset_id != null)
                    {
                        var get_via_subcomponent = _UoW.AssetRepository.GetsubcomonentAssetDetail(FeedingAssetsResponse.via_subcomponent_asset_id.Value);
                        FeedingAssetsResponse.via_subcomponent_asset_name = get_via_subcomponent.name;
                        // get via subcomponent mapping
                        var get_subcomponent_mapping = _UoW.AssetRepository.GetAssetsSubcomponentbyid(requestmodel.asset_id, FeedingAssetsResponse.via_subcomponent_asset_id.Value);
                        //if (get_subcomponent_mapping != null)
                        //{
                        //    FeedingAssetsResponse.circuit = get_subcomponent_mapping.circuit;
                        //}
                    }

                    response.feeding_asset_list.Add(FeedingAssetsResponse);
                }
            }
            return response;
        }

        public async Task<int> UpdateAssetFedByCircuit(UpdateAssetFedByCircuitRequestmodel requestmodel)
        {
            int response = (int)ResponseStatusNumber.Error;
            var get_parent_mapping = _UoW.AssetRepository.UpdateAssetFedByCircuit(requestmodel.asset_parent_hierrachy_id);
            get_parent_mapping.via_subcomponent_asset_id = requestmodel.via_subcomponent_asset_id;
            get_parent_mapping.fed_by_usage_type_id = requestmodel.fed_by_usage_type_id;
            if (requestmodel.fed_by_usage_type_id == null || requestmodel.fed_by_usage_type_id == 0)
            {
                get_parent_mapping.fed_by_usage_type_id = 1; // set default as 1- normal
            }
            get_parent_mapping.length = requestmodel.length;
            get_parent_mapping.style = requestmodel.style;
            get_parent_mapping.conductor_type_id = requestmodel.conductor_type_id;
            get_parent_mapping.raceway_type_id = requestmodel.raceway_type_id;
            get_parent_mapping.number_of_conductor = requestmodel.number_of_conductor;
            get_parent_mapping.fed_by_via_subcomponant_asset_id = requestmodel.fed_by_via_subcomponant_asset_id;

            var updtate = await _UoW.BaseGenericRepository<AssetParentHierarchyMapping>().Update(get_parent_mapping);
            if (updtate)
            {
                _UoW.SaveChanges();

                // update children mapping 
                var get_child = _UoW.AssetRepository.GetChildMapping(get_parent_mapping.parent_asset_id.Value, get_parent_mapping.asset_id.Value);
                get_child.via_subcomponent_asset_id = requestmodel.fed_by_via_subcomponant_asset_id;

                // update circuit from OCP
                if (requestmodel.fed_by_via_subcomponant_asset_id.Value != null)
                {
                    var get_ocp = _UoW.AssetRepository.GetSubcomponentMapping(requestmodel.fed_by_via_subcomponant_asset_id.Value);
                    if (get_ocp != null)
                    {
                        get_child.circuit = get_ocp.circuit;
                    }
                }
                updtate = await _UoW.BaseGenericRepository<AssetChildrenHierarchyMapping>().Update(get_child);

                response = (int)ResponseStatusNumber.Success;
            }
            return response;
        }
        public async Task<int> UpdateAssetFeedingCircuit(UpdateAssetFeedingCircuitRequestmodel requestmodel)
        {
            int response = (int)ResponseStatusNumber.Error;
            var get_children_mapping = _UoW.AssetRepository.UpdateAssetFeedingCircuit(requestmodel.asset_children_hierrachy_id);
            get_children_mapping.via_subcomponent_asset_id = requestmodel.via_subcomponent_asset_id;

            // update circuit from via subcomponent
            if (requestmodel.via_subcomponent_asset_id != null)
            {
                var get_subcompoent = _UoW.AssetRepository.GetSubcomponentMapping(requestmodel.via_subcomponent_asset_id.Value);
                get_children_mapping.circuit = get_subcompoent.circuit;
            }
            else
            {
                get_children_mapping.circuit = null;
            }
            //if (get_children_mapping.via_subcomponent_asset_id != null)
            //{
            //    var get_subcomponent = _UoW.AssetRepository.GetAssetsSubcomponentbyid(get_children_mapping.asset_id.Value, get_children_mapping.via_subcomponent_asset_id.Value);
            //    if (get_subcomponent != null)
            //    {
            //        get_subcomponent.circuit = requestmodel.circuit;
            //        var update_circuit = await _UoW.BaseGenericRepository<AssetSubLevelcomponentMapping>().Update(get_subcomponent);
            //    }
            //}
            var updtate = await _UoW.BaseGenericRepository<AssetChildrenHierarchyMapping>().Update(get_children_mapping);
            if (updtate)
            {
                _UoW.SaveChanges();

                // update subcomponent in fed by also
                var get_parent = _UoW.AssetRepository.GetParentMapping(get_children_mapping.children_asset_id.Value, get_children_mapping.asset_id.Value);
                get_parent.fed_by_via_subcomponant_asset_id = requestmodel.via_subcomponent_asset_id;
                updtate = await _UoW.BaseGenericRepository<AssetParentHierarchyMapping>().Update(get_parent);

                response = (int)ResponseStatusNumber.Success;
            }
            return response;
        }


        public async Task<int> UpdateDigitalOneLine(UpdateDigitalOneLineRequestModel requestmodel)
        {
            int response = (int)ResponseStatusNumber.Error;

            var get_pdf_data = _UoW.AssetRepository.GetClusterDiagramPDFSiteMappingBySiteId(requestmodel.site_id);

            if (get_pdf_data != null)
            {
                get_pdf_data.site_id = requestmodel.site_id;
                get_pdf_data.file_name = requestmodel.file_name;
                get_pdf_data.upload_status = requestmodel.upload_status;
                get_pdf_data.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                get_pdf_data.modified_at = DateTime.UtcNow;
                get_pdf_data.is_deleted = true;

                var update = await _UoW.BaseGenericRepository<ClusterDiagramPDFSiteMapping>().Update(get_pdf_data);

                response = (int)ResponseStatusNumber.Success;
            }
            else
            {
                ClusterDiagramPDFSiteMapping clusterDiagramPDFSiteMapping = new ClusterDiagramPDFSiteMapping();
                clusterDiagramPDFSiteMapping.site_id = requestmodel.site_id;
                clusterDiagramPDFSiteMapping.file_name = requestmodel.file_name;
                clusterDiagramPDFSiteMapping.upload_status = requestmodel.upload_status;
                clusterDiagramPDFSiteMapping.created_at = DateTime.UtcNow;
                clusterDiagramPDFSiteMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                clusterDiagramPDFSiteMapping.is_deleted = false;

                var insert = await _UoW.BaseGenericRepository<ClusterDiagramPDFSiteMapping>().Insert(clusterDiagramPDFSiteMapping);
                _UoW.SaveChanges();

                response = (int)ResponseStatusNumber.Success;
            }

            return response;
        }


        public GetUploadedOneLinePdfResponseModel GetUploadedOneLinePdfDataBySiteIdService(Guid siteId)
        {
            GetUploadedOneLinePdfResponseModel response = new GetUploadedOneLinePdfResponseModel();

            var get_pdf = _UoW.AssetRepository.GetClusterDiagramPDFSiteMappingBySiteId(siteId);
            if (get_pdf != null)
            {
                response.site_id = get_pdf.site_id;
                response.file_name = get_pdf.file_name;
                response.cluster_diagram_pdf_id = get_pdf.cluster_diagram_pdf_id;
                response.file_url = UrlGenerator.GetClusterOnelinePdfURL(get_pdf.file_name);
            }
            return response;
        }


        public ListViewModel<GetAssetsLocationDetailsResponseModel> GetAssetsLocationDetailsService(GetAssetsLocationDetailsRequestModel requestModel)
        {
            ListViewModel<GetAssetsLocationDetailsResponseModel> responseModel = new ListViewModel<GetAssetsLocationDetailsResponseModel>();
            try
            {
                var a = UpdatedGenericRequestmodel.CurrentUser.domain_name;
                var ab = UpdatedGenericRequestmodel.CurrentUser.site_id;

                var response = _UoW.AssetRepository.GetAssetsLocationDetails(requestModel);
                if (response.Item1.Count > 0)
                {
                    int totalassets = response.Item2;
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }

                    responseModel.list = _mapper.Map<List<GetAssetsLocationDetailsResponseModel>>(response.Item1);

                    responseModel.listsize = totalassets;
                    responseModel.pageIndex = requestModel.pageindex;
                    responseModel.pageSize = requestModel.pagesize;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }


        public ListViewModel<GetTopLevelAssetsResponseModel> GetTopLevelAssetsService(GetTopLevelAssetsRequestModel requestModel)
        {
            ListViewModel<GetTopLevelAssetsResponseModel> responseModel = new ListViewModel<GetTopLevelAssetsResponseModel>();
            try
            {
                var response = _UoW.AssetRepository.GetTopLevelAssetsData(requestModel);
                if (response.Item1.Count > 0)
                {
                    int totalassets = response.Item2;
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    responseModel.list = _mapper.Map<List<GetTopLevelAssetsResponseModel>>(response.Item1);

                    responseModel.list.ForEach(item =>
                    {
                        if (item.subLevel_components != null && item.subLevel_components.Count > 0)
                        {
                            item.subLevel_components.ForEach(i =>
                            {
                                i.sublevelcomponent_asset_name = _UoW.AssetRepository.GetAssetNameByAssetId(i.sublevelcomponent_asset_id);

                            });
                        }

                    });

                    responseModel.listsize = totalassets;
                    responseModel.pageIndex = requestModel.pageindex;
                    responseModel.pageSize = requestModel.pagesize;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }


        public async Task<int> ChangeSelectedAssetsLocation(ChangeSelectedAssetsLocationRequestModel requestmodel)
        {
            int response = (int)ResponseStatusNumber.Error;

            try
            {
                var get_all_assets = _UoW.AssetRepository.GetAssetsByListOfAssetIds(requestmodel.asset_id);

                if (get_all_assets != null)
                {
                    foreach (var asset in get_all_assets)
                    {
                        asset.AssetFormIOBuildingMappings.formiobuilding_id = requestmodel.formiobuilding_id;
                        asset.AssetFormIOBuildingMappings.formiofloor_id = requestmodel.formiofloor_id;
                        asset.AssetFormIOBuildingMappings.formioroom_id = requestmodel.formioroom_id;

                        foreach (var sub_asset in asset.AssetSubLevelcomponentMapping)
                        {
                            if (!sub_asset.is_deleted)
                            {
                                var get_subLevel_asset = _UoW.AssetRepository.GetSubLevelAssetById(sub_asset.sublevelcomponent_asset_id);
                                get_subLevel_asset.AssetFormIOBuildingMappings.formiobuilding_id = requestmodel.formiobuilding_id;
                                get_subLevel_asset.AssetFormIOBuildingMappings.formiofloor_id = requestmodel.formiofloor_id;
                                get_subLevel_asset.AssetFormIOBuildingMappings.formioroom_id = requestmodel.formioroom_id;
                                var update = await _UoW.BaseGenericRepository<Asset>().Update(get_subLevel_asset);

                                _UoW.SaveChanges();
                            }

                        }
                        var update2 = await _UoW.BaseGenericRepository<Asset>().Update(asset);

                        _UoW.SaveChanges();
                        if (asset.component_level_type_id == (int)ComponentLevelTypes.SublevelComponent && requestmodel.toplevelcomponent_asset_id != null)
                        {
                            var get_topLevel = _UoW.AssetRepository.GetToplevelmappingofSubcomponent(asset.asset_id);
                            if (get_topLevel.toplevelcomponent_asset_id != requestmodel.toplevelcomponent_asset_id) // if requested top level and db stored top level component are difernet then only do below
                            {
                                get_topLevel.is_deleted = true;
                                get_topLevel.updated_at = DateTime.UtcNow;
                                await _UoW.BaseGenericRepository<AssetTopLevelcomponentMapping>().Update(get_topLevel);

                                var getSubLevel = _UoW.AssetRepository.GetAssetsSubcomponentbyid(get_topLevel.toplevelcomponent_asset_id, asset.asset_id);
                                getSubLevel.is_deleted = true;
                                getSubLevel.updated_at = DateTime.UtcNow;
                                await _UoW.BaseGenericRepository<AssetSubLevelcomponentMapping>().Update(getSubLevel);

                                AssetTopLevelcomponentMapping assetTopLevelcomponentMapping = new AssetTopLevelcomponentMapping();
                                assetTopLevelcomponentMapping.toplevelcomponent_asset_id = requestmodel.toplevelcomponent_asset_id.Value;
                                assetTopLevelcomponentMapping.asset_id = asset.asset_id;
                                assetTopLevelcomponentMapping.created_at = DateTime.UtcNow;
                                assetTopLevelcomponentMapping.is_deleted = false;
                                assetTopLevelcomponentMapping.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                                assetTopLevelcomponentMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                await _UoW.BaseGenericRepository<AssetTopLevelcomponentMapping>().Insert(assetTopLevelcomponentMapping);


                                AssetSubLevelcomponentMapping assetSubLevelcomponentMapping = new AssetSubLevelcomponentMapping();
                                assetSubLevelcomponentMapping.asset_id = requestmodel.toplevelcomponent_asset_id.Value;
                                assetSubLevelcomponentMapping.sublevelcomponent_asset_id = asset.asset_id;
                                assetSubLevelcomponentMapping.is_deleted = false;
                                assetSubLevelcomponentMapping.created_at = DateTime.UtcNow;
                                assetSubLevelcomponentMapping.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                                assetSubLevelcomponentMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                assetSubLevelcomponentMapping.image_name = getSubLevel.image_name;
                                assetSubLevelcomponentMapping.circuit = getSubLevel.circuit;
                                await _UoW.BaseGenericRepository<AssetSubLevelcomponentMapping>().Insert(assetSubLevelcomponentMapping);
                                _UoW.SaveChanges();

                                // change via sub component

                            }

                        }
                    }
                    response = (int)ResponseStatusNumber.Success;
                }
            }
            catch (Exception e)
            {
            }

            return response;
        }


        public async Task<int> DeleteLocationDetails(DeleteLocationDetailsRequestModel requestmodel)
        {
            int response = (int)ResponseStatusNumber.Error;

            if (requestmodel.formiobuilding_id > 0)
            {
                var building = _UoW.AssetRepository.GetBuildingForDelete(requestmodel.formiobuilding_id);
                if (building != null)
                {
                    building.site_id = Guid.Empty;
                    var update = await _UoW.BaseGenericRepository<FormIOBuildings>().Update(building);
                    _UoW.SaveChanges();
                    response = (int)ResponseStatusNumber.Success;
                }
                else
                {
                    response = (int)ResponseStatusNumber.AlreadyExists;
                }
            }

            if (requestmodel.formiofloor_id > 0)
            {
                var floor = _UoW.AssetRepository.GetFloorForDelete(requestmodel.formiofloor_id);
                if (floor != null)
                {
                    floor.site_id = Guid.Empty;
                    var update = await _UoW.BaseGenericRepository<FormIOFloors>().Update(floor);
                    _UoW.SaveChanges();
                    response = (int)ResponseStatusNumber.Success;
                }
                else
                {
                    response = (int)ResponseStatusNumber.AlreadyExists;
                }

            }

            if (requestmodel.formioroom_id > 0)
            {
                var room = _UoW.AssetRepository.GetRoomForDelete(requestmodel.formioroom_id);
                if (room != null)
                {
                    room.site_id = Guid.Empty;
                    var update = await _UoW.BaseGenericRepository<FormIORooms>().Update(room);
                    _UoW.SaveChanges();
                    response = (int)ResponseStatusNumber.Success;
                }
                else
                {
                    response = (int)ResponseStatusNumber.AlreadyExists;
                }
            }

            return response;
        }


        public async Task<int> UpdateLocationDetails(UpdateLocationDetailsRequestModel requestmodel)
        {
            int response = (int)ResponseStatusNumber.Error;

            if (requestmodel.editing_location_flag == (int)AddLocationType.Building)
            {
                var building = _UoW.AssetRepository.GetBuildingById(requestmodel.formiobuilding_id.Value);
                if (building != null)
                {
                    var isAny = _UoW.AssetRepository.CheckForLocationNameIsExist(requestmodel);
                    if (!isAny)
                    {
                        building.formio_building_name = requestmodel.location_name;

                        var update = await _UoW.BaseGenericRepository<FormIOBuildings>().Update(building);
                        _UoW.SaveChanges();
                        response = (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        response = (int)ResponseStatusNumber.AlreadyExists;
                    }

                }
            }

            if (requestmodel.editing_location_flag == (int)AddLocationType.Floor)
            {
                var floor = _UoW.AssetRepository.GetFloorById(requestmodel.formiofloor_id.Value);
                if (floor != null)
                {
                    var isAny = _UoW.AssetRepository.CheckForLocationNameIsExist(requestmodel);
                    if (!isAny)
                    {
                        floor.formiobuilding_id = requestmodel.formiobuilding_id;
                        floor.formio_floor_name = requestmodel.location_name;

                        var get_assets = _UoW.AssetRepository.GetAssetsFromLocations(requestmodel);
                        foreach (var asset in get_assets)
                        {
                            asset.formiobuilding_id = requestmodel.formiobuilding_id;
                            await _UoW.BaseGenericRepository<AssetFormIOBuildingMappings>().Update(asset);
                            _UoW.SaveChanges();
                        }

                        var update = await _UoW.BaseGenericRepository<FormIOFloors>().Update(floor);
                        _UoW.SaveChanges();
                        response = (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        response = (int)ResponseStatusNumber.AlreadyExists;
                    }

                }
            }

            if (requestmodel.editing_location_flag == (int)AddLocationType.Room)
            {
                var room = _UoW.AssetRepository.GetRoomById(requestmodel.formioroom_id.Value);
                if (room != null)
                {
                    var isAny = _UoW.AssetRepository.CheckForLocationNameIsExist(requestmodel);
                    if (!isAny)
                    {
                        if (requestmodel.formiofloor_id == 0 || requestmodel.formiofloor_id == null)
                        {
                            FormIOFloors FormIOFloors = new FormIOFloors();
                            FormIOFloors.formio_floor_name = requestmodel.floor_name;
                            FormIOFloors.formiobuilding_id = requestmodel.formiobuilding_id.Value;
                            FormIOFloors.created_at = DateTime.UtcNow;
                            FormIOFloors.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                            FormIOFloors.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);

                            var insertfloor = await _UoW.BaseGenericRepository<FormIOFloors>().Insert(FormIOFloors);
                            _UoW.SaveChanges();
                            requestmodel.formiofloor_id = FormIOFloors.formiofloor_id;
                        }

                        room.formiofloor_id = requestmodel.formiofloor_id;
                        room.formio_room_name = requestmodel.location_name;

                        var get_assets = _UoW.AssetRepository.GetAssetsFromLocations(requestmodel);
                        foreach (var asset in get_assets)
                        {
                            asset.formiobuilding_id = requestmodel.formiobuilding_id;
                            asset.formiofloor_id = requestmodel.formiofloor_id;
                            await _UoW.BaseGenericRepository<AssetFormIOBuildingMappings>().Update(asset);
                        }

                        var update = await _UoW.BaseGenericRepository<FormIORooms>().Update(room);
                        _UoW.SaveChanges();
                        response = (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        response = (int)ResponseStatusNumber.AlreadyExists;
                    }
                }
            }

            if (requestmodel.editing_location_flag == (int)AddLocationType.Section)
            {
                var section = _UoW.AssetRepository.GetSectionById(requestmodel.formiosection_id.Value);
                if (section != null)
                {
                    var isAny = _UoW.AssetRepository.CheckForLocationNameIsExist(requestmodel);
                    if (!isAny)
                    {
                        section.formio_section_name = requestmodel.location_name;

                        var update = await _UoW.BaseGenericRepository<FormIOSections>().Update(section);
                        _UoW.SaveChanges();
                        response = (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        response = (int)ResponseStatusNumber.AlreadyExists;
                    }

                }
            }

            return response;
        }


        public ListViewModel<GetOBWOAssetsOfRequestedAssetResponseModel> GetOBWOAssetsOfRequestedAsset(GetOBWOAssetsOfRequestedAssetRequestModel requestModel)
        {
            ListViewModel<GetOBWOAssetsOfRequestedAssetResponseModel> responseModel = new ListViewModel<GetOBWOAssetsOfRequestedAssetResponseModel>();
            try
            {
                var response = _UoW.AssetRepository.GetOBWOAssetsOfRequestedAsset(requestModel);
                if (response.Item1.Count > 0)
                {
                    int totalassets = response.Item2;
                    if (requestModel.page_index == 0 || requestModel.page_size == 0)
                    {
                        requestModel.page_size = 20;
                        requestModel.page_index = 1;
                    }
                    responseModel.list = _mapper.Map<List<GetOBWOAssetsOfRequestedAssetResponseModel>>(response.Item1);

                    responseModel.list.ForEach(x =>
                    {
                        if (x.modified_by != null && x.modified_by != "")
                        {
                            var technician_user = _UoW.WorkOrderRepository.GetUserByID(Guid.Parse(x.modified_by));
                            if (technician_user != null)
                            {
                                x.modified_by_name = technician_user.firstname + " " + technician_user.lastname;
                            }
                        }
                        if (x.asset_pm_id != null)
                        {
                            var asset_pm = _UoW.WorkOrderRepository.GetAssetpmByidForInspectionlist(x.asset_pm_id.Value);
                            var get_pm = _UoW.WorkOrderRepository.GetPMById(asset_pm.pm_id.Value);
                            if (get_pm != null)
                            {
                                x.pm_inspection_type_id = get_pm.pm_inspection_type_id;
                            }

                        }
                    });

                    responseModel.listsize = totalassets;
                    responseModel.pageIndex = requestModel.page_index;
                    responseModel.pageSize = requestModel.page_size;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }



        public ListViewModel<GetAllWOOBAssetsByAssetIdResponseModel> GetAllWOOBAssetsByAssetId(GetOBWOAssetsOfRequestedAssetRequestModel requestModel)
        {
            ListViewModel<GetAllWOOBAssetsByAssetIdResponseModel> responseModel = new ListViewModel<GetAllWOOBAssetsByAssetIdResponseModel>();
            try
            {
                var response = _UoW.AssetRepository.GetAllWOOBAssetsByAssetId(requestModel);
                if (response.Item1.Count > 0)
                {
                    int totalassets = response.Item2;
                    //responseModel.list = _mapper.Map<List<GetAllWOOBAssetsByAssetIdResponseModel>>(response.Item1);

                    responseModel.list = response.Item1;
                    responseModel.listsize = totalassets;
                    responseModel.pageIndex = requestModel.page_index;
                    responseModel.pageSize = requestModel.page_size;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }

        public GetAssetFeedingCircuitForReportResponsemodel GetAssetFeedingCircuitForReport(GetAssetFeedingCircuitForReportRequestmdel request)
        {
            GetAssetFeedingCircuitForReportResponsemodel response = new GetAssetFeedingCircuitForReportResponsemodel();
            List<AssetFeedingCircuitList> asset_feeding_circuit_list = new List<AssetFeedingCircuitList>();
            AssetFeedingCircuitList initial = new AssetFeedingCircuitList();
            //initial.asset_id = request.asset_id;
            //initial.is_script_done = false;
            // asset_feeding_circuit_list.Add(initial);
            var initial_feeding_assets = AssetFeedinglogic(request.asset_id);
            asset_feeding_circuit_list.AddRange(initial_feeding_assets);
            bool continue_script = true;
            while (continue_script)
            {
                try
                {
                    foreach (var item in asset_feeding_circuit_list.ToList())
                    {

                        if (!item.is_script_done)
                        {
                            var feeding_assets = AssetFeedinglogic(item.feeding_asset_id);
                            asset_feeding_circuit_list.AddRange(feeding_assets);
                            // check close loop with same fedby and feeding is exist in list or not
                            var duplicates = asset_feeding_circuit_list
                                            .GroupBy(x => new { x.fed_by_asset_id, x.feeding_asset_id })
                                            .Where(g => g.Count() > 1)
                                            .SelectMany(g => g);
                            if (duplicates.Any())
                            {
                                response.success = (int)ResponseStatusNumber.Error;
                                response.message = "There is a close loop with fedby Asset" + duplicates.FirstOrDefault().fed_by_asset_name + " and feeding Asset " + duplicates.FirstOrDefault().feeding_asset_name + " Please correct proper circuit and try again";
                                throw new Exception();
                            }
                        }
                        item.is_script_done = true;
                    }
                    var is_all_item_done = asset_feeding_circuit_list.Where(x => !x.is_script_done).Count();
                    if (is_all_item_done == 0)
                    {
                        response.success = (int)ResponseStatusNumber.Success;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    break;
                }
            }

            response.asset_feeding_circuit_list = asset_feeding_circuit_list;

            return response;
        }
        public List<AssetFeedingCircuitList> AssetFeedinglogic(Guid asset_id)
        {
            List<AssetFeedingCircuitList> response = new List<AssetFeedingCircuitList>();
            var get_childrens = _UoW.AssetRepository.GetAssetChildren(asset_id);
            foreach (var item in get_childrens)
            {
                AssetFeedingCircuitList children = new AssetFeedingCircuitList();
                children.feeding_asset_id = item.children_asset_id.Value;

                var get_child_asset = _UoW.AssetRepository.GetAssetbyIdforFeedingscircuit(item.children_asset_id.Value);
                children.feeding_asset_name = get_child_asset.name;

                var get_parent_asset = _UoW.AssetRepository.GetAssetbyIdforFeedingscircuit(asset_id);
                children.fed_by_asset_name = get_parent_asset.name;

                // get fed by usage type id 
                var get_parent_mapping = _UoW.AssetRepository.GetAssetparentMapping(item.children_asset_id.Value, asset_id);
                if (get_parent_mapping != null)
                {
                    children.fed_by_usage_type_id = get_parent_mapping.fed_by_usage_type_id;
                    if (get_parent_mapping.fed_by_usage_type_id == null || get_parent_mapping.fed_by_usage_type_id == 0) // if usage type is null or 0 then set default 1 
                    {
                        children.fed_by_usage_type_id = 1;
                    }
                }

                if (children.fed_by_usage_type_id == null)
                {
                    children.fed_by_usage_type_id = 1;
                }
                children.is_script_done = false;
                children.fed_by_asset_id = asset_id;
                response.Add(children);
            }

            return response;
        }


        public GetAssetFeedingCircuitForReportResponsemodel GetTempAssetFeedingCircuitForReport(Guid woonboardingassets_id)
        {
            GetAssetFeedingCircuitForReportResponsemodel response = new GetAssetFeedingCircuitForReportResponsemodel();
            List<AssetFeedingCircuitList> asset_feeding_circuit_list = new List<AssetFeedingCircuitList>();
            AssetFeedingCircuitList initial = new AssetFeedingCircuitList();
            //initial.asset_id = request.asset_id;
            //initial.is_script_done = false;
            // asset_feeding_circuit_list.Add(initial);
            var initial_feeding_assets = TempAssetFeedinglogic(woonboardingassets_id);
            asset_feeding_circuit_list.AddRange(initial_feeding_assets);
            bool continue_script = true;
            while (continue_script)
            {
                try
                {
                    foreach (var item in asset_feeding_circuit_list.ToList())
                    {
                        //  var test = asset_feeding_circuit_list.Where(x => x.feeding_asset_id == item.fed_by_asset_id && x.is_script_done).FirstOrDefault();
                        //if (!asset_feeding_circuit_list.Where(x=>x.feeding_asset_id == item.fed_by_asset_id && x.is_script_done).Any())
                        //{
                        if (!item.is_script_done)
                        {
                            var feeding_assets = TempAssetFeedinglogic(item.feeding_asset_id);
                            asset_feeding_circuit_list.AddRange(feeding_assets);
                            // check close loop with same fedby and feeding is exist in list or not
                            var duplicates = asset_feeding_circuit_list
                                            .GroupBy(x => new { x.fed_by_asset_id, x.feeding_asset_id })
                                            .Where(g => g.Count() > 1)
                                            .SelectMany(g => g);
                            if (duplicates.Any())
                            {
                                response.success = (int)ResponseStatusNumber.Error;
                                response.message = "There is a close loop with fedby Asset" + duplicates.FirstOrDefault().fed_by_asset_name + " and feeding Asset " + duplicates.FirstOrDefault().feeding_asset_name + " Please correct proper circuit and try again";
                                throw new Exception();
                            }
                        }
                        //}

                        item.is_script_done = true;
                    }
                    var is_all_item_done = asset_feeding_circuit_list.Where(x => !x.is_script_done).Count();
                    if (is_all_item_done == 0)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    break;
                }
            }

            response.asset_feeding_circuit_list = asset_feeding_circuit_list;

            return response;
        }
        public List<AssetFeedingCircuitList> TempAssetFeedinglogic(Guid woonboardingassets_id)
        {
            List<AssetFeedingCircuitList> response = new List<AssetFeedingCircuitList>();
            var get_childrens = _UoW.AssetRepository.GetTempAssetChildren(woonboardingassets_id);
            foreach (var item in get_childrens)
            {
                AssetFeedingCircuitList children = new AssetFeedingCircuitList();
                children.feeding_asset_id = item.woonboardingassets_id.Value;
                children.is_script_done = false;
                children.fed_by_asset_id = woonboardingassets_id;
                response.Add(children);
            }

            return response;
        }

        public GetAllImagesForAssetResponseModel GetAllImagesForAsset(Guid asset_id)
        {
            GetAllImagesForAssetResponseModel response = new GetAllImagesForAssetResponseModel();
            try
            {
                var get_asset_profile_images = _UoW.AssetRepository.GetAssetProfileImagesByAssetId(asset_id);

                var get_asset_ir_images = _UoW.AssetRepository.GetAssetIRWOImagesByAssetId(asset_id);

                var map = _mapper.Map<List<AssetImageDetails>>(get_asset_profile_images);
                response.asset_image_list = map;

                foreach (var img in get_asset_ir_images)
                {
                    if (img.ir_image_label != null)
                    {
                        AssetImageDetails assetImageDetails = new AssetImageDetails();
                        assetImageDetails.asset_image_name = img.ir_image_label;
                        assetImageDetails.asset_photo_type = (int)AssetPhotoType.Asset_IR_Image;
                        assetImageDetails.asset_image_url = UrlGenerator.GetIRImagesURL(img.ir_image_label, img.s3_image_folder_name);
                        assetImageDetails.is_deleted = img.is_deleted;
                        assetImageDetails.created_at = img.created_at;
                        assetImageDetails.assetirwoimageslabelmapping_id = img.assetirwoimageslabelmapping_id;
                        bool is_image_exist = false;
                        try
                        {
                            using (var httpClient = new HttpClient())
                            {
                                var headRequest = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, assetImageDetails.asset_image_url)).Result;
                                if (headRequest.StatusCode == HttpStatusCode.OK)
                                {
                                    is_image_exist = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        if (is_image_exist)
                        {
                            response.asset_image_list.Add(assetImageDetails);
                        }
                    }

                    if (img.visual_image_label != null)
                    {
                        AssetImageDetails assetImageDetails2 = new AssetImageDetails();

                        assetImageDetails2.asset_image_name = img.visual_image_label;
                        assetImageDetails2.asset_photo_type = (int)AssetPhotoType.Asset_Visual_Image;
                        assetImageDetails2.asset_image_url = UrlGenerator.GetIRImagesURL(img.visual_image_label, img.s3_image_folder_name);
                        assetImageDetails2.is_deleted = img.is_deleted;
                        assetImageDetails2.created_at = img.created_at;
                        assetImageDetails2.assetirwoimageslabelmapping_id = img.assetirwoimageslabelmapping_id;
                        bool is_image_exist = false;
                        try
                        {
                            using (var httpClient = new HttpClient())
                            {
                                var headRequest = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, assetImageDetails2.asset_image_url)).Result;
                                if (headRequest.StatusCode == HttpStatusCode.OK)
                                {
                                    is_image_exist = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        if (is_image_exist)
                        {
                            response.asset_image_list.Add(assetImageDetails2);
                        }
                    }
                }

                // add issue images explicitly 
                var get_issue_images = _UoW.AssetRepository.GetAssetIssueImages(asset_id);
                if (get_issue_images.Count > 0)
                {
                    if (response.asset_image_list == null)
                    {
                        response.asset_image_list = new List<AssetImageDetails>();
                    }
                    foreach (var img in get_issue_images)
                    {
                        // check if image is exist or not in repponse 
                        var image_exist = response.asset_image_list.Where(x => x.asset_image_name == img.image_file_name).FirstOrDefault();
                        if (image_exist == null)
                        {
                            AssetImageDetails AssetImageDetails = new AssetImageDetails();
                            AssetImageDetails.asset_issue_image_mapping_id = img.asset_issue_image_mapping_id;
                            AssetImageDetails.asset_image_name = img.image_file_name;
                            AssetImageDetails.created_at = DateTime.UtcNow;
                            AssetImageDetails.asset_image_url = UrlGenerator.GetIssueImagesURL(img.image_file_name);
                            if (img.image_duration_type_id == (int)IssueImageDuration.before)
                            {
                                AssetImageDetails.asset_photo_type = (int)AssetPhotoType.Issue_before_photo;
                            }
                            if (img.image_duration_type_id == (int)IssueImageDuration.after)
                            {
                                AssetImageDetails.asset_photo_type = (int)AssetPhotoType.Issue_after_photo;
                            }
                            response.asset_image_list.Add(AssetImageDetails);
                        }
                    }
                }
                response.asset_image_list = response.asset_image_list.GroupBy(f => f.asset_image_name)
                                                                                .Select(g => g.First())
                                                                                .ToList();
            }
            catch (Exception e)
            {
            }
            return response;
        }

        public GetLocationDataByAssetIdResponseModel GetLocationDataByAssetId(Guid asset_id)
        {
            GetLocationDataByAssetIdResponseModel response = new GetLocationDataByAssetIdResponseModel();
            try
            {
                var get_location = _UoW.WorkOrderRepository.GetAssetLocationDataById(asset_id);
                if (get_location != null)
                {
                    var map = _mapper.Map<GetLocationDataByAssetIdResponseModel>(get_location);
                    response = map;
                }
            }
            catch (Exception e)
            {
            }

            return response;
        }
        public GetOBWOAssetDetailsByIdResponsemodel GetAssetDetailsByIdForTempAsset(GetAssetDetailsByIdForTempAsset requestModel)
        {
            GetOBWOAssetDetailsByIdResponsemodel response = new GetOBWOAssetDetailsByIdResponsemodel();
            WorkOrderService woservice = new WorkOrderService(_mapper);
            try
            {

                if (requestModel.woonboardingassets_id != null)
                {
                    var res = woservice.GetOBWOAssetDetailsById(requestModel.woonboardingassets_id.ToString());
                    if (res != null)
                    {
                        response = res;
                    }
                }
                else if (requestModel.asset_id != null)
                {
                    // check in temp asset 
                    var get_woline_from_tempasset = _UoW.AssetRepository.GetinstallWOlineFromTempAsset(requestModel.asset_id.Value, requestModel.wo_id.Value);
                    if (get_woline_from_tempasset != null)
                    {
                        var res2 = woservice.GetOBWOAssetDetailsById(get_woline_from_tempasset.woonboardingassets_id.ToString());
                        if (res2 != null)
                        {
                            response = res2;
                        }
                    }
                    else
                    {
                        // check in direct woline if exist
                        var get_obwo_asset = _UoW.AssetRepository.GetOBWOAssetsByAssetId(requestModel.asset_id.Value, requestModel.wo_id.Value);
                        if (get_obwo_asset != null)
                        {
                            var res2 = woservice.GetOBWOAssetDetailsById(get_obwo_asset.woonboardingassets_id.ToString());
                            if (res2 != null)
                            {
                                response = res2;
                            }
                        }
                        // create chunk data from Asset to woline and then return
                        else
                        {
                            var get_asset = _UoW.WorkOrderRepository.GetAssetByIdforExisting(requestModel.asset_id.Value);
                            get_asset.AssetProfileImages = get_asset.AssetProfileImages.Where(x => (x.asset_photo_type == (int)AssetPhotoType.Asset_Profile || x.asset_photo_type == (int)AssetPhotoType.Nameplate_Photo
                            || x.asset_photo_type == (int)AssetPhotoType.Exterior_Photo
                            || x.asset_photo_type == (int)AssetPhotoType.Schedule_Photos
                            || x.asset_photo_type == (int)AssetPhotoType.Additional_Photos
                            ) && !x.is_deleted).ToList();
                            //get_asset.AssetIRWOImagesLabelMapping = get_asset.AssetIRWOImagesLabelMapping.Where(x => !x.is_deleted).ToList();
                            get_asset.AssetParentHierarchyMapping = get_asset.AssetParentHierarchyMapping.Where(x => !x.is_deleted).ToList();
                            get_asset.AssetTopLevelcomponentMapping = get_asset.AssetTopLevelcomponentMapping.Where(x => !x.is_deleted).ToList();
                            get_asset.AssetSubLevelcomponentMapping = get_asset.AssetSubLevelcomponentMapping.Where(x => !x.is_deleted).ToList();

                            var map = _mapper.Map<GetOBWOAssetDetailsByIdResponsemodel>(get_asset);

                            if (map.wo_ob_asset_sublevelcomponent_mapping != null && map.wo_ob_asset_sublevelcomponent_mapping.Count > 0)
                            {
                                map.wo_ob_asset_sublevelcomponent_mapping.ForEach(x =>
                                {

                                    var get_subasset = _UoW.AssetRepository.GetSubLevelAssetById(x.sublevelcomponent_asset_id);
                                    x.sublevelcomponent_asset_name = get_subasset.name;
                                    if (get_subasset.inspectiontemplate_asset_class_id != null)
                                    {
                                        x.sublevelcomponent_asset_class_id = get_subasset.inspectiontemplate_asset_class_id.Value;
                                        x.sublevelcomponent_asset_class_name = get_subasset.InspectionTemplateAssetClass.asset_class_name;
                                        x.sublevelcomponent_asset_class_code = get_subasset.InspectionTemplateAssetClass.asset_class_code;
                                    }
                                });
                            }

                            if (map.wo_ob_asset_fed_by_mapping != null && map.wo_ob_asset_fed_by_mapping.Count > 0)
                            {
                                map.wo_ob_asset_fed_by_mapping.ForEach(x =>
                                {
                                    var get_fedasset = _UoW.AssetRepository.GetSubLevelAssetById(x.parent_asset_id);
                                    x.parent_asset_name = get_fedasset.name;
                                });
                            }
                            if (map.wo_ob_asset_toplevelcomponent_mapping != null && map.wo_ob_asset_toplevelcomponent_mapping.Count > 0)
                            {
                                map.wo_ob_asset_toplevelcomponent_mapping.ForEach(x =>
                                {

                                    var get_toplevelasset = _UoW.AssetRepository.GetSubLevelAssetById(x.toplevelcomponent_asset_id);
                                    x.toplevelcomponent_asset_name = get_toplevelasset.name;
                                });
                            }

                            response = map;

                        }
                    }

                }
            }
            catch (Exception e)
            {
            }
            return response;
        }

        public GetOBTopLevelFedByAssetListResponseModel GetOBTopLevelFedByAssetList(GetOBTopLevelFedByAssetListRequestModel requestModel)
        {
            GetOBTopLevelFedByAssetListResponseModel response = new GetOBTopLevelFedByAssetListResponseModel();
            try
            {
                List<Guid> child_asset = new List<Guid>();
                // if woline is for main asset then check in main assets
                if (requestModel.woonboardingassets_id != null)
                {
                    var get_woline = _UoW.WorkOrderRepository.GetWOlineByOBAssetId(requestModel.woonboardingassets_id.Value);
                    if (get_woline != null && get_woline.TempAsset != null && get_woline.TempAsset.asset_id != null) // woline is for main asset
                    {
                        // get all child of this woline
                        GetAssetFeedingCircuitForReportRequestmdel GetAssetFeedingCircuitForReportRequestmdel = new GetAssetFeedingCircuitForReportRequestmdel();
                        GetAssetFeedingCircuitForReportRequestmdel.asset_id = get_woline.TempAsset.asset_id.Value;
                        var feeding_assets = GetAssetFeedingCircuitForReport(GetAssetFeedingCircuitForReportRequestmdel);
                        child_asset = feeding_assets.asset_feeding_circuit_list.Select(x => x.feeding_asset_id).ToList();
                    }
                }

                var get_assets = _UoW.AssetRepository.GetToplevelFebbyAssetlist(child_asset);
                response.main_asset_list = new List<GetAllHierarchyAssetsResponseModel>();
                response.ob_wo_asset_list = new List<OBWOAssetDetails>();

                if (get_assets?.Count > 0)
                {
                    response.main_asset_list = _mapper.Map<List<GetAllHierarchyAssetsResponseModel>>(get_assets);

                    response.main_asset_list.ForEach(x =>
                    {
                        if (!String.IsNullOrEmpty(x.children))
                        {
                            x.is_child_available = true;
                        }
                    });
                }

                // for temp asset close loop
                if (requestModel.woonboardingassets_id != null)
                {
                    var get_woline = _UoW.WorkOrderRepository.GetWOlineByOBAssetId(requestModel.woonboardingassets_id.Value);
                    //if (get_woline != null && get_woline.TempAsset.asset_id == null) // woline is for main asset
                    //{
                    // get all child of this woline
                    var feeding_assets = GetTempAssetFeedingCircuitForReport(requestModel.woonboardingassets_id.Value);
                    child_asset = feeding_assets.asset_feeding_circuit_list.Select(x => x.feeding_asset_id).ToList();
                    //}
                }
                var db_ob_wo_assets = _UoW.AssetRepository.GetTopLevelFedByWOOBAssetList(requestModel, child_asset);
                if (db_ob_wo_assets.Count > 0)
                {
                    response.ob_wo_asset_list = _mapper.Map<List<OBWOAssetDetails>>(db_ob_wo_assets);
                }

            }
            catch (Exception e)
            {
            }
            return response;
        }


        public GetSubcomponentsForFedByMappingResponseModel GetTopLevelSubLevlComponentHiararchy(Guid? wo_id, string woonboaardingasset_id)
        {
            GetSubcomponentsForFedByMappingResponseModel response = new GetSubcomponentsForFedByMappingResponseModel();
            try
            {
                if (wo_id == null)
                    wo_id = Guid.Empty;

                var get_all_toplevel_main_asset = _UoW.AssetRepository.GetAllTopLevelAssetsList(wo_id.Value);
                var get_all_sublevel_main_assets = _UoW.AssetRepository.GetAllSubLevelAssetsMappingList(wo_id.Value);

                response.toplevel_main_assets = _mapper.Map<List<TopLevelAssetData>>(get_all_toplevel_main_asset);
                response.subcomponent_main_assets = _mapper.Map<List<SubLevelAssetData>>(get_all_sublevel_main_assets);


                if (wo_id != null && wo_id != Guid.Empty)
                {
                    var get_all_toplevel_obwo_asset = _UoW.AssetRepository.GetAllTopLevelOBWOAssetsList(wo_id.Value);
                    var get_all_sublevel_obwo_assets = _UoW.AssetRepository.GetAllSubLevelOBWOAssetsMappingList(wo_id.Value);

                    response.toplevel_obwo_assets = _mapper.Map<List<TopLevelAssetData>>(get_all_toplevel_obwo_asset);
                    response.subcomponent_obwo_assets = _mapper.Map<List<SubLevelAssetData>>(get_all_sublevel_obwo_assets);

                    response.subcomponent_obwo_assets = response.subcomponent_obwo_assets.Where(x => x.toplevelcomponent_asset_id != Guid.Empty && x.toplevelcomponent_asset_id.ToString() != woonboaardingasset_id).ToList();
                    response.toplevel_obwo_assets = response.toplevel_obwo_assets.Where(x => x.woonboardingassets_id != woonboaardingasset_id).ToList();
                }

                // Remove SubComponents which does not have TopLevel Linked
                response.subcomponent_main_assets = response.subcomponent_main_assets.Where(x => x.toplevelcomponent_asset_id != Guid.Empty && x.toplevelcomponent_asset_id.ToString() != woonboaardingasset_id).ToList();

                // Remove Current asset in FedByAsset(TopLevel) List and its subcomponents
                response.toplevel_main_assets = response.toplevel_main_assets.Where(x => x.asset_id != woonboaardingasset_id).ToList();

            }
            catch (Exception e)
            {
            }
            return response;
        }


        public async Task<int> UploadBulkMainAssets(UploadBulkMainAssetsRequestModel requestmodel)
        {
            AssetPMService assetPMService = new AssetPMService(_mapper);
            int res = (int)ResponseStatusNumber.Error;
            try
            {
                var classlist1 = requestmodel.asset_data.Where(x => x.asset_id == null).Select(x => x.asset_class_code).Distinct().ToList();
                var classlist2 = requestmodel.asset_subcomponents_mappings.Select(x => x.subcomponent_asset_class_code).Distinct().ToList();
                var class_code_list = classlist1.Concat(classlist2).Distinct().ToList();
                var isAny = _UoW.WorkOrderRepository.CheckIsClassAvailableOrNot(class_code_list);

                if (requestmodel.asset_data == null || requestmodel.asset_data.Count == 0)
                {
                    res = (int)ResponseStatusNumber.InvalidData;
                    return res;
                }
                else if (isAny)
                {
                    res = (int)ResponseStatusNumber.asset_class_not_found;
                    return res;
                }
                // Create/Edit New Assets
                if (requestmodel.asset_data != null && requestmodel.asset_data.Count > 0)
                {
                    //Bulk Edit Assets
                    foreach (var item in requestmodel.asset_data.Where(x => x.asset_id != null && x.asset_id != Guid.Empty))
                    {
                        var get_assets = _UoW.AssetRepository.GetAssetbyIdforFeedingscircuit(item.asset_id.Value);
                        if (get_assets != null)
                        {
                            get_assets.name = item.asset_name;
                            if (item.status != null && item.status > 0)
                                get_assets.status = item.status;

                            bool is_asset_condition_changed = false;

                            if (get_assets.condition_index_type != item.condition_index_type || get_assets.criticality_index_type != item.criticality_index_type)
                                is_asset_condition_changed = true;

                            get_assets.asset_placement = item.asset_placement;
                            get_assets.condition_index_type = item.condition_index_type;
                            get_assets.QR_code = item.QR_code;
                            get_assets.criticality_index_type = item.criticality_index_type;
                            get_assets.asset_operating_condition_state = item.asset_operating_condition_state;
                            //get_assets.commisiion_date = item.commisiion_date;
                            get_assets.maintenance_index_type = item.maintenance_index_type;
                            get_assets.arc_flash_label_valid = item.arc_flash_label_valid;
                            get_assets.modified_at = DateTime.UtcNow;
                            get_assets.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                            var update = await _UoW.BaseGenericRepository<Asset>().Update(get_assets);
                            if (update)
                            {
                                _UoW.SaveChanges();
                                if (is_asset_condition_changed)
                                {
                                    await assetPMService.AddRemoveAssetPMsOnAssetCondition(item.asset_id.Value);
                                }
                                res = (int)ResponseStatusNumber.Success;
                            }
                        }
                    }

                    foreach (var get_asset in requestmodel.asset_data.Where(x => x.asset_id == null))
                    {
                        var asset_class = _UoW.WorkOrderRepository.GetAssetClassByClasscode(get_asset.asset_class_code);
                        if (asset_class != null)
                        {
                            EditAssetDetailsRequestmodel editAssetDetailsRequestmodel = new EditAssetDetailsRequestmodel();
                            editAssetDetailsRequestmodel.asset_name = get_asset.asset_name;
                            editAssetDetailsRequestmodel.inspectiontemplate_asset_class_id = asset_class.inspectiontemplate_asset_class_id;
                            editAssetDetailsRequestmodel.component_level_type_id = get_asset.component_level_type_id;
                            editAssetDetailsRequestmodel.status = (int)Status.AssetActive;
                            editAssetDetailsRequestmodel.QR_code = get_asset.QR_code;
                            editAssetDetailsRequestmodel.building = get_asset.building;
                            editAssetDetailsRequestmodel.floor = get_asset.floor;
                            editAssetDetailsRequestmodel.room = get_asset.room;
                            editAssetDetailsRequestmodel.section = get_asset.section;
                            editAssetDetailsRequestmodel.criticality_index_type = get_asset.criticality_index_type;
                            editAssetDetailsRequestmodel.condition_index_type = get_asset.condition_index_type;
                            editAssetDetailsRequestmodel.thermal_classification_id = get_asset.thermal_classification_id;
                            editAssetDetailsRequestmodel.asset_placement = get_asset.asset_placement;
                            editAssetDetailsRequestmodel.asset_operating_condition_state = get_asset.asset_operating_condition_state;
                            editAssetDetailsRequestmodel.maintenance_index_type = get_asset.maintenance_index_type;
                            editAssetDetailsRequestmodel.arc_flash_label_valid = get_asset.arc_flash_label_valid;

                            var respone = await EditAssetDetails(editAssetDetailsRequestmodel);
                            get_asset.asset_id = respone.asset_id;
                        }
                    }
                }

                //Add Toplevel-Sublevel Mappings 
                if (requestmodel.asset_subcomponents_mappings != null && requestmodel.asset_subcomponents_mappings.Count > 0)
                {
                    foreach (var item in requestmodel.asset_subcomponents_mappings)
                    {
                        var toplevel_id = requestmodel.asset_data.Where(x => x.asset_name == item.toplevel_asset_name).Select(x => x.asset_id).FirstOrDefault();

                        /*// If Subcomponents are Added in asset_data list then use this
                        //var sublevel_id = requestmodel.asset_data.Where(x => x.asset_name == item.subcomponent_asset_name 
                        //    && x.asset_class_code==item.subcomponent_asset_class_code).Select(x => x.asset_id).FirstOrDefault();

                        //AddNewSubComponentRequestmodel addNewSubComponentRequestmodel = new AddNewSubComponentRequestmodel();
                        //addNewSubComponentRequestmodel.asset_id = toplevel_id.Value;
                        //addNewSubComponentRequestmodel.sublevelcomponent_asset_id = sublevel_id.Value;
                        //addNewSubComponentRequestmodel.is_subcomponent_for_existing = true;
                        //await AddNewSubComponent(addNewSubComponentRequestmodel);
                        */


                        //if Subcomponent Assets are not in asset_data list then use this to Create New Subcomponent Assets

                        var asset_class = _UoW.WorkOrderRepository.GetAssetClassByClasscode(item.subcomponent_asset_class_code);
                        if (toplevel_id != null && asset_class != null)
                        {
                            AddNewSubComponentRequestmodel addNewSubComponentRequestmodel2 = new AddNewSubComponentRequestmodel();
                            addNewSubComponentRequestmodel2.asset_id = toplevel_id.Value;
                            addNewSubComponentRequestmodel2.sublevelcomponent_asset_name = item.subcomponent_asset_name;
                            addNewSubComponentRequestmodel2.inspectiontemplate_asset_class_id = asset_class.inspectiontemplate_asset_class_id;
                            addNewSubComponentRequestmodel2.is_subcomponent_for_existing = false;

                            await AddNewSubComponent(addNewSubComponentRequestmodel2);
                        }
                    }
                }

                // Add FedBy (Parent) Mappings 
                if (requestmodel.assets_fedby_mappings != null && requestmodel.assets_fedby_mappings.Count > 0)
                {

                    //var new_added_parent = requestmodel.assets_fedby_mappings.Where(x => x.asset_parent_hierrachy_id == null).ToList();
                    foreach (var new_parent in requestmodel.assets_fedby_mappings)
                    {
                        var get_fedby_asset = requestmodel.asset_data.Where(x => x.asset_name == new_parent.fedby_asset_name).FirstOrDefault();//_UoW.WorkOrderRepository.GetOBWOAssetByName(wo_id,this_asset.fedby_asset_name);
                        var get_asset = requestmodel.asset_data.Where(x => x.asset_name == new_parent.asset_name).FirstOrDefault();// _UoW.WorkOrderRepository.GetOBWOAssetByName(wo_id,this_asset.asset_name);

                        var sublevel_with_class = requestmodel.asset_subcomponents_mappings.Where(x => x.subcomponent_asset_name == new_parent.ocp_asset_name).FirstOrDefault();
                        var get_sublevel_asset = sublevel_with_class != null ? _UoW.AssetRepository.GetAssetByNameClassCode(new_parent.ocp_asset_name, sublevel_with_class.subcomponent_asset_class_code) : null;

                        var fedby_subcomp_and_class = requestmodel.asset_subcomponents_mappings.Where(x => x.subcomponent_asset_name == new_parent.fedby_ocp_asset_name).FirstOrDefault();
                        var get_fedby_sublevel_asset = fedby_subcomp_and_class != null ? _UoW.AssetRepository.GetAssetByNameClassCode(new_parent.fedby_ocp_asset_name, fedby_subcomp_and_class.subcomponent_asset_class_code) : null;//requestmodel.asset_subcomponents_mappings.Where(x => x.subcomponent_asset_name == new_parent.fedby_ocp_asset_name).FirstOrDefault();

                        if (get_asset != null && get_fedby_asset != null)
                        {
                            AssetParentHierarchyMapping AssetParentHierarchyMapping = new AssetParentHierarchyMapping();
                            AssetParentHierarchyMapping.parent_asset_id = get_fedby_asset.asset_id;
                            AssetParentHierarchyMapping.asset_id = get_asset.asset_id;
                            AssetParentHierarchyMapping.fed_by_usage_type_id = 1; // set default as normal

                            AssetParentHierarchyMapping.conductor_type_id = new_parent.conductor_type_id;
                            AssetParentHierarchyMapping.raceway_type_id = new_parent.raceway_type_id;
                            AssetParentHierarchyMapping.style = new_parent.style;
                            AssetParentHierarchyMapping.length = new_parent.length;
                            AssetParentHierarchyMapping.number_of_conductor = new_parent.number_of_conductor;
                            if (get_fedby_sublevel_asset != null)
                            {
                                AssetParentHierarchyMapping.fed_by_via_subcomponant_asset_id = get_fedby_sublevel_asset.asset_id;
                            }
                            if (get_sublevel_asset != null)
                            {
                                AssetParentHierarchyMapping.via_subcomponent_asset_id = get_sublevel_asset.asset_id;
                            }

                            AssetParentHierarchyMapping.created_at = DateTime.UtcNow;
                            AssetParentHierarchyMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            AssetParentHierarchyMapping.is_deleted = false;
                            AssetParentHierarchyMapping.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);

                            var insert_parent = await _UoW.BaseGenericRepository<AssetParentHierarchyMapping>().Insert(AssetParentHierarchyMapping);
                            _UoW.SaveChanges();

                            AssetChildrenHierarchyMapping AssetChildrenHierarchyMapping = new AssetChildrenHierarchyMapping();
                            AssetChildrenHierarchyMapping.children_asset_id = get_asset.asset_id;
                            AssetChildrenHierarchyMapping.asset_id = get_fedby_asset.asset_id;
                            if (get_sublevel_asset != null)
                            {
                                AssetChildrenHierarchyMapping.via_subcomponent_asset_id = get_sublevel_asset.asset_id;
                            }
                            AssetChildrenHierarchyMapping.created_at = DateTime.UtcNow;
                            AssetChildrenHierarchyMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            AssetChildrenHierarchyMapping.is_deleted = false;
                            AssetParentHierarchyMapping.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);

                            var insert_child = await _UoW.BaseGenericRepository<AssetChildrenHierarchyMapping>().Insert(AssetChildrenHierarchyMapping);
                            _UoW.SaveChanges();
                        }
                    }
                }
                res = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
            }
            return res;
        }

        public async Task<int> EditBulkMainAssets(EditBulkMainAssetsRequestModel requestModel)
        {
            AssetPMService assetPMService = new AssetPMService(_mapper);
            int res = (int)ResponseStatusNumber.Error;
            try
            {
                foreach (var item in requestModel.assets_list)
                {
                    var get_assets = _UoW.AssetRepository.GetAssetbyIdforFeedingscircuit(item.asset_id);
                    if (get_assets != null)
                    {
                        bool is_asset_condition_changed = false;

                        if (get_assets.condition_index_type != item.condition_index_type || get_assets.criticality_index_type != item.criticality_index_type)
                            is_asset_condition_changed = true;

                        get_assets.name = item.name;
                        get_assets.status = item.status;
                        get_assets.asset_placement = item.asset_placement;
                        get_assets.condition_index_type = item.condition_index_type;
                        get_assets.QR_code = item.QR_code;
                        get_assets.criticality_index_type = item.criticality_index_type;
                        get_assets.asset_operating_condition_state = item.asset_operating_condition_state;
                        get_assets.commisiion_date = item.commisiion_date;
                        get_assets.maintenance_index_type = item.maintenance_index_type;
                        get_assets.arc_flash_label_valid = item.arc_flash_label_valid;
                        get_assets.modified_at = DateTime.UtcNow;
                        get_assets.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        var update = await _UoW.BaseGenericRepository<Asset>().Update(get_assets);
                        if (update)
                        {
                            _UoW.SaveChanges();
                            res = (int)ResponseStatusNumber.Success;

                            if (is_asset_condition_changed)
                                await assetPMService.AddRemoveAssetPMsOnAssetCondition(item.asset_id);
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            return res;
        }


        public async Task<int> AssetImageDeleteOrSetAsProfile(AssetImageDeleteOrSetAsProfileRequestModel requestModel)
        {
            int res = (int)ResponseStatusNumber.Error;
            try
            {
                var get_asset = _UoW.AssetRepository.GetAssetById(requestModel.asset_id);

                if (!requestModel.is_deleted) // if is_deleted = False then set imgage as profile for asset 
                {
                    get_asset.asset_profile_image = requestModel.asset_image_url;
                    get_asset.modified_at = DateTime.UtcNow;
                    get_asset.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    var update_asset = await _UoW.BaseGenericRepository<Asset>().Update(get_asset);

                    if (update_asset) { res = (int)ResponseStatusNumber.Success; }

                }
                else // if is_deleted = True then delete image from different table according to received id
                {

                    if (requestModel.asset_profile_images_id != null && requestModel.asset_profile_images_id != Guid.Empty)
                    {
                        var get_image = _UoW.AssetRepository.GetAssetProfileImageById(requestModel.asset_profile_images_id.Value);

                        get_image.is_deleted = true;
                        get_image.modified_at = DateTime.UtcNow;
                        get_image.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        var update_img = await _UoW.BaseGenericRepository<AssetProfileImages>().Update(get_image);
                        if (update_img) { res = (int)ResponseStatusNumber.Success; }
                    }
                    else if (requestModel.assetirwoimageslabelmapping_id != null && requestModel.assetirwoimageslabelmapping_id != Guid.Empty)
                    {
                        var get_image = _UoW.AssetRepository.GetIRWOImagesLabelById(requestModel.assetirwoimageslabelmapping_id.Value);

                        get_image.is_deleted = true;
                        get_image.updated_at = DateTime.UtcNow;
                        get_image.updated_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        var update_img = await _UoW.BaseGenericRepository<AssetIRWOImagesLabelMapping>().Update(get_image);
                        if (update_img) { res = (int)ResponseStatusNumber.Success; }
                    }
                    else if (requestModel.asset_issue_image_mapping_id != null && requestModel.asset_issue_image_mapping_id != Guid.Empty)
                    {
                        var get_image = _UoW.AssetRepository.GetAssetIssueImageById(requestModel.asset_issue_image_mapping_id.Value);

                        get_image.is_deleted = true;
                        get_image.modified_at = DateTime.UtcNow;
                        get_image.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        var update_img = await _UoW.BaseGenericRepository<AssetIssueImagesMapping>().Update(get_image);
                        if (update_img) { res = (int)ResponseStatusNumber.Success; }
                    }

                    // if requested image URL is Same as asset profile URL then remove that profile from asset also
                    if (requestModel.asset_image_url == get_asset.asset_profile_image)
                    {
                        get_asset.asset_profile_image = null;
                        get_asset.modified_at = DateTime.UtcNow;
                        get_asset.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        var update_asset = await _UoW.BaseGenericRepository<Asset>().Update(get_asset);
                    }
                }

                if (res == (int)ResponseStatusNumber.Success) { _UoW.SaveChanges(); }

            }
            catch (Exception e)
            {
            }
            return res;
        }


        public GetAllBuildingLocationsResponseModel GetAllBuildingLocations(int pagesize, int pageindex)
        {
            GetAllBuildingLocationsResponseModel responseModel = new GetAllBuildingLocationsResponseModel();
            try
            {
                var get_all_buildings = _UoW.AssetRepository.GetAllBuildingLocations(pagesize, pageindex);

                if (get_all_buildings.Item1 != null && get_all_buildings.Item1.Count > 0)
                {
                    responseModel.building_list = get_all_buildings.Item1;
                    responseModel.listsize = get_all_buildings.Item2;
                }
            }
            catch (Exception e)
            {
            }
            return responseModel;
        }
        public GetAllFloorsByBuildingResponseModel GetAllFloorsByBuilding(int formiobuilding_id, int pagesize, int pageindex)
        {
            GetAllFloorsByBuildingResponseModel responseModel = new GetAllFloorsByBuildingResponseModel();
            try
            {
                var get_all_floors = _UoW.AssetRepository.GetAllFloorsByBuilding(formiobuilding_id, pagesize, pageindex);

                if (get_all_floors.Item1 != null && get_all_floors.Item1.Count > 0)
                {
                    responseModel.floor_list = get_all_floors.Item1;
                    responseModel.listsize = get_all_floors.Item2;
                }
            }
            catch (Exception e)
            {
            }
            return responseModel;
        }

        public GetAllFloorsByBuildingResponseModel GetAllFloorsByBuilding_Dropdown(int formiobuilding_id)
        {
            GetAllFloorsByBuildingResponseModel responseModel = new GetAllFloorsByBuildingResponseModel();
            try
            {
                var get_all_floors = _UoW.AssetRepository.GetAllFloorsByBuilding(formiobuilding_id, 0, 0);

                // add 1-100 floors explicitly 
                List<string> pre_floor_list = new List<string> { "Basement", "Ground" };
                for (int i = 1; i <= 100; i++)
                {
                    pre_floor_list.Add("Floor - " + i.ToString());
                }
                var existing_floor = get_all_floors.Item1.Select(x => x.formio_floor_name.ToLower()).ToList();
                var missing_floors = pre_floor_list.Where(x => !String.IsNullOrEmpty(x) && !existing_floor.Contains(x.ToLower())).ToList();
                foreach (var missing_floor in missing_floors)
                {
                    GetAllFloorsByBuildingData GetAllFloorsByBuildingData = new GetAllFloorsByBuildingData();
                    GetAllFloorsByBuildingData.formio_floor_name = missing_floor;
                    GetAllFloorsByBuildingData.formiobuilding_id = formiobuilding_id;
                    get_all_floors.Item1.Add(GetAllFloorsByBuildingData);
                }

                if (get_all_floors.Item1 != null && get_all_floors.Item1.Count > 0)
                {
                    responseModel.floor_list = get_all_floors.Item1;
                    responseModel.listsize = get_all_floors.Item2;
                }
            }
            catch (Exception e)
            {
            }
            return responseModel;
        }

        public GetAllRoomsByFloorResponseModel GetAllRoomsByFloor(int formiofloor_id, int pagesize, int pageindex)
        {
            GetAllRoomsByFloorResponseModel responseModel = new GetAllRoomsByFloorResponseModel();
            try
            {
                var get_all_rooms = _UoW.AssetRepository.GetAllRoomsByFloor(formiofloor_id, pagesize, pageindex);

                if (get_all_rooms.Item1 != null && get_all_rooms.Item1.Count > 0)
                {
                    responseModel.room_list = get_all_rooms.Item1;
                    responseModel.listsize = get_all_rooms.Item2;
                }
            }
            catch (Exception e)
            {
            }
            return responseModel;
        }

        //Script for TempLocations
        public async Task<int> ScriptForAddTempLocations()
        {
            try
            {

                var get_all_sites = _UoW.AssetRepository.GetAllSitesForScript();
                foreach (var site in get_all_sites)
                {
                    var get_all_wo = _UoW.AssetRepository.GetAllWOBySiteId(site.site_id);
                    foreach (var wo in get_all_wo)
                    {
                        foreach (var wo_asset in wo.WOOnboardingAssets)
                        {
                            try
                            {


                                if (wo_asset.WOOBAssetTempFormIOBuildingMapping == null)
                                {
                                    string building = "";
                                    if (wo_asset.WOLineBuildingMapping != null && wo_asset.WOLineBuildingMapping.FormIOBuildings != null)
                                    {
                                        building = wo_asset.WOLineBuildingMapping.FormIOBuildings.formio_building_name;
                                    }
                                    else
                                    {
                                        building = wo_asset.building;
                                    }

                                    string floor = "";
                                    if (wo_asset.WOLineBuildingMapping != null && wo_asset.WOLineBuildingMapping.FormIOFloors != null)
                                    {
                                        floor = wo_asset.WOLineBuildingMapping.FormIOFloors.formio_floor_name;
                                    }
                                    else
                                    {
                                        floor = wo_asset.floor;
                                    }

                                    string room = "";
                                    if (wo_asset.WOLineBuildingMapping != null && wo_asset.WOLineBuildingMapping.FormIORooms != null)
                                    {
                                        room = wo_asset.WOLineBuildingMapping.FormIORooms.formio_room_name;
                                    }
                                    else
                                    {
                                        room = wo_asset.room;
                                    }

                                    string section = "";
                                    if (wo_asset.WOLineBuildingMapping != null && wo_asset.WOLineBuildingMapping.FormIOSections != null)
                                    {
                                        section = wo_asset.WOLineBuildingMapping.FormIOSections.formio_section_name;
                                    }
                                    else
                                    {
                                        section = wo_asset.section;
                                    }

                                    if (String.IsNullOrEmpty(building))
                                    {
                                        building = "Default";
                                    }
                                    if (String.IsNullOrEmpty(floor))
                                    {
                                        floor = "Default";
                                    }
                                    if (String.IsNullOrEmpty(room))
                                    {
                                        room = "Default";
                                    }
                                    if (String.IsNullOrEmpty(section))
                                    {
                                        section = "Default";
                                    }

                                    TempFormIOBuildings tempFormIOBuildings = null;
                                    TempFormIOFloors tempFormIOFloors = null;
                                    TempFormIORooms tempFormIORooms = null;
                                    TempFormIOSections tempFormIOSections = null;

                                    if (!String.IsNullOrEmpty(building))
                                    {
                                        tempFormIOBuildings = _UoW.AssetRepository.GetTempFormIOBuildingByName(building, site.site_id, wo.wo_id);
                                        if (tempFormIOBuildings == null)
                                        {
                                            tempFormIOBuildings = new TempFormIOBuildings();
                                            tempFormIOBuildings.temp_formio_building_name = building;
                                            tempFormIOBuildings.wo_id = wo_asset.wo_id;
                                            tempFormIOBuildings.created_at = DateTime.UtcNow;
                                            tempFormIOBuildings.site_id = site.site_id;
                                            tempFormIOBuildings.company_id = site.company_id;

                                            var insertbuilding = await _UoW.BaseGenericRepository<TempFormIOBuildings>().Insert(tempFormIOBuildings);
                                            _UoW.SaveChanges();
                                        }
                                    }
                                    if (!String.IsNullOrEmpty(floor))
                                    {
                                        tempFormIOFloors = _UoW.AssetRepository.GetTempFormIFloorByName(floor, site.site_id, wo.wo_id, tempFormIOBuildings.temp_formiobuilding_id);
                                        if (tempFormIOFloors == null)
                                        {
                                            tempFormIOFloors = new TempFormIOFloors();
                                            tempFormIOFloors.temp_formio_floor_name = floor;
                                            tempFormIOFloors.temp_formiobuilding_id = tempFormIOBuildings.temp_formiobuilding_id;
                                            tempFormIOFloors.wo_id = wo_asset.wo_id;
                                            tempFormIOFloors.created_at = DateTime.UtcNow;
                                            tempFormIOFloors.site_id = site.site_id;
                                            tempFormIOFloors.company_id = site.company_id;

                                            var insertfloor = await _UoW.BaseGenericRepository<TempFormIOFloors>().Insert(tempFormIOFloors);
                                            _UoW.SaveChanges();
                                        }
                                    }
                                    if (!String.IsNullOrEmpty(room))
                                    {
                                        tempFormIORooms = _UoW.AssetRepository.GetTempFormIORoomByName(room, site.site_id, wo.wo_id, tempFormIOFloors.temp_formiofloor_id);
                                        if (tempFormIORooms == null)
                                        {
                                            tempFormIORooms = new TempFormIORooms();
                                            tempFormIORooms.temp_formio_room_name = room;
                                            tempFormIORooms.temp_formiofloor_id = tempFormIOFloors.temp_formiofloor_id;
                                            tempFormIORooms.wo_id = wo_asset.wo_id;
                                            tempFormIORooms.created_at = DateTime.UtcNow;
                                            tempFormIORooms.site_id = site.site_id;
                                            tempFormIORooms.company_id = site.company_id;

                                            var insertroom = await _UoW.BaseGenericRepository<TempFormIORooms>().Insert(tempFormIORooms);
                                            _UoW.SaveChanges();
                                        }
                                    }
                                    if (!String.IsNullOrEmpty(section))
                                    {
                                        tempFormIOSections = _UoW.AssetRepository.GetTempFormIOSectionByName(section, site.site_id, wo.wo_id, tempFormIORooms.temp_formioroom_id);
                                        if (tempFormIOSections == null)
                                        {
                                            tempFormIOSections = new TempFormIOSections();
                                            tempFormIOSections.temp_formio_section_name = section;
                                            tempFormIOSections.temp_formioroom_id = tempFormIORooms.temp_formioroom_id;
                                            tempFormIOSections.wo_id = wo_asset.wo_id;
                                            tempFormIOSections.created_at = DateTime.UtcNow;
                                            tempFormIOSections.site_id = site.site_id;
                                            tempFormIOSections.company_id = site.company_id;

                                            var insertsection = await _UoW.BaseGenericRepository<TempFormIOSections>().Insert(tempFormIOSections);
                                            _UoW.SaveChanges();
                                        }
                                    }

                                    var woLine_temp_mapping = _UoW.AssetRepository.GetWOOBAssetTempLocationMapping(wo_asset.woonboardingassets_id);
                                    if (woLine_temp_mapping == null)
                                    {
                                        WOOBAssetTempFormIOBuildingMapping wOOBAssetTempFormIOBuildingMapping = new WOOBAssetTempFormIOBuildingMapping();
                                        wOOBAssetTempFormIOBuildingMapping.woonboardingassets_id = wo_asset.woonboardingassets_id;
                                        wOOBAssetTempFormIOBuildingMapping.temp_formiobuilding_id = tempFormIOBuildings.temp_formiobuilding_id;
                                        wOOBAssetTempFormIOBuildingMapping.temp_formiofloor_id = tempFormIOFloors.temp_formiofloor_id;
                                        wOOBAssetTempFormIOBuildingMapping.temp_formioroom_id = tempFormIORooms.temp_formioroom_id;
                                        wOOBAssetTempFormIOBuildingMapping.temp_formiosection_id = tempFormIOSections.temp_formiosection_id;
                                        wOOBAssetTempFormIOBuildingMapping.created_at = DateTime.UtcNow;

                                        var insertmapping = await _UoW.BaseGenericRepository<WOOBAssetTempFormIOBuildingMapping>().Insert(wOOBAssetTempFormIOBuildingMapping);
                                        _UoW.SaveChanges();
                                    }
                                    else
                                    {
                                        woLine_temp_mapping.woonboardingassets_id = wo_asset.woonboardingassets_id;
                                        woLine_temp_mapping.temp_formiobuilding_id = tempFormIOBuildings.temp_formiobuilding_id;
                                        woLine_temp_mapping.temp_formiofloor_id = tempFormIOFloors.temp_formiofloor_id;
                                        woLine_temp_mapping.temp_formioroom_id = tempFormIORooms.temp_formioroom_id;
                                        woLine_temp_mapping.temp_formiosection_id = tempFormIOSections.temp_formiosection_id;
                                        woLine_temp_mapping.created_at = DateTime.UtcNow;

                                        var updatemapping = await _UoW.BaseGenericRepository<WOOBAssetTempFormIOBuildingMapping>().Update(woLine_temp_mapping);
                                        _UoW.SaveChanges();
                                    }

                                }
                            }
                            catch (Exception e)
                            {
                            }

                        }


                    }


                }
            }
            catch (Exception e)
            {
            }
            return 1;
        }

        public FilterAssetsOptimizedResponseview<FilterAssetOptimizedResponsemodel> FilterAssetOptimized(FilterAssetsRequestModel requestModel)
        {
            FilterAssetsOptimizedResponseview<FilterAssetOptimizedResponsemodel> responseModel = new FilterAssetsOptimizedResponseview<FilterAssetOptimizedResponsemodel>();
            try
            {
                var asset_list = _UoW.AssetRepository.FilterAssetOptimized(requestModel);
                var mapper = _mapper.Map<List<FilterAssetOptimizedResponsemodel>>(asset_list.Item1);
                var building_mapper = _mapper.Map<List<FilterAssetRoomFloorBuildingLocationOptionsmapping>>(asset_list.Item3);

                building_mapper = building_mapper.GroupBy(x => x.room_id).Select(g => g.First()).ToList();

                responseModel.list = mapper;
                responseModel.rooms_with_floor_building = building_mapper;
                responseModel.pageSize = requestModel.pagesize;
                responseModel.pageIndex = requestModel.pageindex;
                responseModel.listsize = asset_list.Item2;

            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public async Task<int> UpdateAssetClassPDFUrl(Guid inspectiontemplate_asset_class_id, string pdf_report_template_url)
        {
            int res = (int)ResponseStatusNumber.Error;
            try
            {
                var get_asset_class = _UoW.WorkOrderRepository.GetAssetClassById(inspectiontemplate_asset_class_id);

                get_asset_class.pdf_report_template_url = pdf_report_template_url;
                get_asset_class.modified_at = DateTime.UtcNow;
                get_asset_class.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                var update = await _UoW.BaseGenericRepository<InspectionTemplateAssetClass>().Update(get_asset_class);
                if (update)
                {
                    _UoW.SaveChanges();
                    res = (int)ResponseStatusNumber.Success;
                }
            }
            catch (Exception e)
            {
            }
            return res;
        }

        public async Task<int> InsertFormIOTemplate()
        {
            int result = (int)ResponseStatusNumber.Error;
            try
            {


                string listitems = @"[
                     {
                      ""FORM ID"": ""2bcd61f4-4190-4494-bae2-33282d23b04f"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-02T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""1511\"",\""humidity\"":\""156\"",\""identification\"":\""DFGHJK\"",\""parent\"":\""\"",\""assetId\"":\""\"",\""building\"":\""1\"",\""floor\"":\""2\"",\""room\"":\""3\"",\""section\"":\""4\""},\""nameplateInformation\"":{\""manufacturer\"":\""5\"",\""modelNumber\"":\""6\"",\""serialNumber\"":\""7\"",\""powerFactorRating\"":\""8\"",\""phaseWire\"":\""3PH\/4W\"",\""kWRating\"":\""1\"",\""kVaRating\"":\""01\"",\""horsepower\"":\""1\"",\""voltageRating\"":\""1\"",\""ampereRating\"":\""1\""},\""pleaseSelectTests\"":{\""visualInspection\"":true,\""testConnections\"":true,\""dcInsulationResistanceTest\"":true,\""windingResistanceTests\"":true,\""powerFactorTipUpTest\"":true},\""visualInspection\"":{\""exciter\"":\""Normal\"",\""diodes\"":\""Normal\"",\""ctMounting\"":\""Normal\"",\""meteringCtsAndWiring\"":\""Normal\"",\""shortingTerminalBlocks\"":\""Normal\"",\""fuseBlocks\"":\""Normal\"",\""controlPanelLamps\"":\""Normal\"",\""tLeads\"":\""Normal\"",\""terminalBlocks\"":\""Normal\"",\""greaseBearing\"":\""Added\"",\""generatorCompartment\"":\""Dirty\"",\""highVoltageCables\"":\""Normal\"",\""lowVoltageCables\"":\""Normal\"",\""grounding\"":\""Normal\""},\""testConnections\"":{\""energized\"":\""1\"",\""guarded\"":\""Yes\"",\""grounded\"":\""Frame\"",\""testVoltageKVDc\"":\""5\""},\""dcInsulationResistanceTest\"":{\""dataGrid\"":[{\""timeInMinutes\"":0.25,\""insulationResistance1\"":\""1\"",\""insulationResistance2\"":\""\"",\""insulationResistance3\"":\""\""}],\""primaryToGroundSecondaryGuarded3\"":\""\"",\""polarization10over1_1\"":\""\""},\""windingResistanceTests\"":{\""resistance1\"":4,\""resistance2\"":5,\""resistance3\"":6},\""powerFactorTipUpTest\"":{\""ma1\"":1,\""w1\"":2,\""powerFactor1\"":3,\""ma2\"":3,\""w2\"":5,\""powerFactor2\"":5,\""ma3\"":5,\""w3\"":5,\""powerFactor3\"":5,\""ma4\"":5,\""w4\"":5,\""powerFactor4\"":5,\""ma5\"":5,\""w5\"":5,\""powerFactor5\"":5,\""ma6\"":5,\""w6\"":5,\""powerFactor6\"":5,\""ma7\"":5,\""w7\"":5,\""powerFactor7\"":5,\""ma8\"":5,\""w8\"":5,\""powerFactor8\"":5,\""ma9\"":5,\""w9\"":5,\""powerFactor9\"":5,\""ma10\"":5,\""w10\"":5,\""powerFactor10\"":5,\""ma11\"":5,\""w11\"":5,\""powerFactor11\"":5,\""ma12\"":5,\""w12\"":5,\""powerFactor12\"":5,\""ma13\"":5,\""w13\"":5,\""powerFactor13\"":5,\""ma14\"":5,\""w14\"":5,\""powerFactor14\"":5},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The generator PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""12\"",\""testedBy\"":\""12\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""12\"",\""name\"":\""13\"",\""serialNumber\"":\""14\"",\""calibrationDate\"":\""2023-10-02T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""e1f1d6b5-c222-4b83-8731-b08f8c2f1ae8"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-02T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""131\"",\""humidity\"":\""121\"",\""identification\"":\""12\"",\""parent\"":\""212\"",\""assetId\"":\""11\"",\""building\"":\""12\"",\""floor\"":\""1\"",\""room\"":\""1\"",\""section\"":\""1\""},\""nameplateInformation\"":{\""type\"":\""5\"",\""depthOfElectrodes\"":\""3\"",\""sizeOfGroundingElectrodes\"":\""4\"",\""sizeGroundingConductor\"":\""4\""},\""tests\"":{\""dataGrid\"":[{\""number\"":\""Row Index is Zero\"",\""tests\"":\""\"",\""ohms\"":\""\"",\""test\"":\""1\"",\""from\"":\""3\"",\""to\"":\""2\""}]},\""footer\"":{\""testEquipmentNumber\"":\""212\"",\""inspectionVerdict\"":\""acceptable\"",\""comments\"":\""Acceptable.\"",\""testedBy\"":\""12\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""1\"",\""name\"":\""1\"",\""serialNumber\"":\""1\"",\""calibrationDate\"":\""2023-10-02T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""d508a406-fa42-4c04-a25f-f196304115f2"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-02T22:53:28.9103341\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""1\"",\""humidity\"":\""21\"",\""identification\"":\""121\"",\""parent\"":\""12\"",\""assetId\"":\""11\"",\""building\"":\""1\"",\""floor\"":\""4\"",\""room\"":\""1\"",\""section\"":\""2\""},\""nameplateInformation\"":{\""manufacturer\"":\""1\"",\""serialNumber\"":\""2\"",\""powerFactorRating\"":\""6\"",\""phaseWire\"":\""6\"",\""kwRating\"":\""7\"",\""kvaRating\"":\""89\"",\""voltageRating\"":\""54\"",\""ampereRating\"":\""5\""},\""visualMechanicalInspection\"":{\""exciter\"":\""normal\"",\""diodes\"":\""normal\"",\""ctMounting\"":\""normal\"",\""meteringCtsAndWiring\"":\""normal\"",\""shortingTerminalBlocks\"":\""seeComments\"",\""fuseBlocks\"":\""seeComments\"",\""controlPanelLamps\"":\""normal\"",\""tLeads\"":\""seeComments\"",\""terminalBlocks\"":\""normal\"",\""greaseBearing\"":\""seeComments\"",\""generatorCompartment\"":\""seeComments\"",\""highVoltageCables\"":\""normal\"",\""lowVoltageCables\"":\""seeComments\"",\""grounding\"":\""nA\""},\""footer\"":{\""testEquipmentNumber\"":\""212\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""The Generator PASSED and is acceptable for operation.\"",\""testedBy\"":\""21211\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""3\"",\""name\"":\""31\"",\""serialNumber\"":\""9\"",\""calibrationDate\"":\""2023-10-03T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""e0d6aa3e-0ff9-4fc8-bd6b-b8df69b57536"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""212\"",\""humidity\"":\""11\"",\""identification\"":\""23\"",\""parent\"":\""12\"",\""assetId\"":\""11\"",\""building\"":\""21\"",\""floor\"":\""1\"",\""room\"":\""1\"",\""section\"":\""13\"",\""workOrderType\"":\""Acceptance Test\""},\""nameplateInformation\"":{\""manufacturer\"":\""2\"",\""serialNumber\"":\""5\"",\""catalogNumber\"":\""25\"",\""type\"":\""2\"",\""contactType\"":\""55\"",\""shuntTripVoltage\"":\""5\"",\""ampereRating\"":\""2\"",\""interruptingRating\"":\""5\"",\""voltageRating\"":\""1\"",\""mechanismType\"":\""1\"",\""controlVoltage\"":\""2\""},\""pleaseSelectTests\"":{\""visualInspection\"":true,\""operationalTests\"":true,\""insulationResistanceContactsOpen\"":true,\""insulationResistanceContactsClosed\"":true,\""leakageInMilliamps\"":true,\""contactResistanceTest\"":true,\""topFuseClipResistance\"":true,\""bottomFuseClipResistance\"":true,\""fuseResistanceInMicroOhms\"":true},\""visualInspection\"":{\""contactor\"":\""New\"",\""contactWearGap\"":\""N\/A\"",\""contactSequence\"":\""N\/A\"",\""mechanism\"":\""Lubricated\"",\""shutter\"":\""N\/A\"",\""fuseType\"":\""21\"",\""spareFusesPresent\"":\""12\"",\""auxiliaryContacts\"":\""N\/A\"",\""cubicle\"":\""121\"",\""rackingMechanism\"":\""See Comments\"",\""switchGrounded\"":\""Yes\"",\""connections\"":\""See Comments\"",\""panelLights\"":\""See Comments\"",\""heaters\"":\""Normal\"",\""fuseAmperage\"":\""121\""},\""operationalTests\"":{\""electricalOperation\"":\""N\/A\"",\""manualOperation\"":\""See Comments\""},\""insulationResistanceContactsOpen\"":{\""p1AsFound\"":\""1\"",\""p1AsLeft\"":\""21\"",\""p2AsFound\"":\""1\"",\""p2AsLeft\"":\""2\"",\""p3AsFound\"":\""12\"",\""p3AsLeft\"":\""1\"",\""testVoltage\"":12},\""insulationResistanceContactsClosed\"":{\""p1AsFound\"":\""2\"",\""p1AsLeft\"":\""45\"",\""p2AsFound\"":\""1\"",\""p2AsLeft\"":\""1\"",\""p3AsFound\"":\""2\"",\""p3AsLeft\"":\""6\"",\""testVoltage\"":212},\""leakageInMilliamps\"":{\""pole1\"":\""112\"",\""pole2\"":\""1\"",\""pole3\"":\""45\"",\""leakageInMilliampsAt\"":222},\""contactResistanceTest\"":{\""pole1AsFound\"":43,\""pole1AsLeft\"":43,\""pole2AsFound\"":12,\""pole2AsLeft\"":12,\""pole3AsFound\"":124,\""pole3AsLeft\"":124},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The contactor PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""54\"",\""testedBy\"":\""433\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""12\"",\""name\"":\""1\"",\""serialNumber\"":\""2\"",\""calibrationDate\"":\""2023-10-10T00:00:00+05:30\""}]},\""topFuseClipResistance\"":{\""pole1AsFound\"":445,\""pole1AsLeft\"":445,\""pole2AsFound\"":4,\""pole2AsLeft\"":4,\""pole3AsFound\"":441,\""pole3AsLeft\"":441},\""bottomFuseClipResistance\"":{\""pole1AsFound\"":2,\""pole1AsLeft\"":2,\""pole2AsFound\"":5,\""pole2AsLeft\"":5,\""pole3AsFound\"":6,\""pole3AsLeft\"":6},\""fuseResistanceInMicroOhms\"":{\""pole1AsFound\"":7,\""pole1AsLeft\"":7,\""pole3AsFound\"":4,\""pole3AsLeft\"":4,\""pole2AsFound\"":3,\""pole2AsLeft\"":3}}}""
                     },
                     {
                      ""FORM ID"": ""ab7cfb9d-4a2a-421e-84dc-16039d988d44"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-04T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""131\"",\""humidity\"":\""44\"",\""identification\"":\""21\"",\""parent\"":\""21\"",\""assetId\"":\""11\"",\""building\"":\""32\"",\""floor\"":\""12\"",\""room\"":\""2\"",\""section\"":\""2\"",\""workOrderType\"":\""Acceptance Test\""},\""nameplateInformation\"":{\""manufacturer\"":\""562\"",\""modelNumber\"":\""1\"",\""serialNumber\"":\""2\"",\""powerFactorRating\"":\""5\"",\""phaseWire\"":\""3PH\/3W\"",\""kWRating\"":\""5\"",\""kVaRating\"":\""6\"",\""horsepower\"":\""32\"",\""voltageRating\"":\""14\"",\""ampereRating\"":\""54\""},\""pleaseSelectTests\"":{\""testConnections\"":true,\""dcInsulationResistance\"":true,\""windingResistanceTests\"":true,\""powerFactorTipUpTest\"":true},\""testConnections\"":{\""energized\"":\""21\"",\""guarded\"":\""Yes\"",\""grounded\"":\""SeeComments\"",\""testVoltageKVDc\"":\""121\""},\""dcInsulationResistance\"":{\""dataGrid\"":[{\""timeInMinutes\"":0.25,\""insulationResistance1\"":\""112\"",\""insulationResistance2\"":\""\"",\""insulationResistance3\"":\""\""}],\""primaryToGroundSecondaryGuarded3\"":\""\"",\""polarization10over1_1\"":\""\""},\""windingResistanceTests\"":{\""resistance1\"":21,\""resistance2\"":21,\""resistance3\"":2},\""powerFactorTipUpTest\"":{\""ma1\"":9,\""w1\"":5,\""powerFactor1\"":2,\""ma2\"":2,\""w2\"":4,\""powerFactor2\"":9,\""ma3\"":4,\""w3\"":5,\""powerFactor3\"":6,\""ma4\"":2,\""w4\"":4,\""powerFactor4\"":5,\""ma5\"":7,\""w5\"":9,\""powerFactor5\"":6,\""ma6\"":2,\""w6\"":5,\""powerFactor6\"":96,\""ma7\"":6,\""w7\"":4,\""powerFactor7\"":56,\""ma8\"":6,\""powerFactor8\"":6,\""w8\"":5,\""ma9\"":7,\""w9\"":5,\""powerFactor9\"":6,\""ma10\"":3,\""w10\"":2,\""powerFactor10\"":1,\""ma11\"":4,\""w11\"":5,\""powerFactor11\"":6,\""ma12\"":87,\""w12\"":8,\""powerFactor12\"":5,\""w13\"":4,\""powerFactor13\"":5,\""ma14\"":6,\""w14\"":1,\""powerFactor14\"":3,\""ma13\"":2},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The asset PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""43\"",\""testedBy\"":\""221\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""2\"",\""name\"":\""1\"",\""serialNumber\"":\""2\"",\""calibrationDate\"":\""2023-10-04T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""6e1c183d-b515-422d-b8a3-3b55e6de057c"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-02T23:07:37.2938771\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""21\"",\""humidity\"":\""1212\"",\""identification\"":\""54\"",\""parent\"":\""2\"",\""assetId\"":\""5\"",\""building\"":\""12\"",\""floor\"":\""1\"",\""room\"":\""1\"",\""section\"":\""1\""},\""nameplateInformation\"":{\""manufacturer\"":\""2\"",\""style\"":\""21\"",\""pTRatio\"":\""12\"",\""voltageRating\"":\""12\"",\""type\"":\""21\"",\""catalogNumber\"":\""21\"",\""scale\"":\""12\""},\""pleaseSelectTests\"":{\""testData\"":true},\""testData\"":{\""dataGrid\"":[{\""injectedVoltageInput\"":4,\""calculatedScaleReading\"":5,\""asLeft\"":2,\""asFound\"":8},{\""injectedVoltageInput\"":1,\""calculatedScaleReading\"":4,\""asFound\"":54,\""asLeft\"":54},{\""injectedVoltageInput\"":1,\""calculatedScaleReading\"":2,\""asFound\"":32,\""asLeft\"":8},{\""injectedVoltageInput\"":56,\""calculatedScaleReading\"":5,\""asFound\"":5,\""asLeft\"":5},{\""injectedVoltageInput\"":5,\""calculatedScaleReading\"":5,\""asFound\"":5,\""asLeft\"":5}]},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The voltmeter PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""54\"",\""testedBy\"":\""43\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""7\"",\""name\"":\""8\"",\""serialNumber\"":\""8\"",\""calibrationDate\"":\""2023-10-03T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""27ce4851-fad4-43cd-8271-005ce7fc2017"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""1233\"",\""humidity\"":\""33\"",\""identification\"":\""54\"",\""parent\"":\""4\"",\""assetId\"":\""4\"",\""building\"":\""23\"",\""floor\"":\""2\"",\""room\"":\""2\"",\""section\"":\""1\""},\""footer\"":{\""testEquipmentNumber\"":\""43\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""The VFD PASSED and is acceptable for operation.\"",\""testedBy\"":\""21\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""32\"",\""name\"":\""2\"",\""serialNumber\"":\""2\"",\""calibrationDate\"":\""2023-10-04T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""d5a67876-dd00-4e60-9a54-12ef3c30d27d"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""65\"",\""humidity\"":\""55\"",\""identification\"":\""32\"",\""parent\"":\""32\"",\""assetId\"":\""32\"",\""building\"":\""32\"",\""floor\"":\""4\"",\""room\"":\""2\"",\""section\"":\""3\""},\""footer\"":{\""testEquipmentNumber\"":\""3\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""The Reactor PASSED and is acceptable for operation.\"",\""testedBy\"":\""12\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""1\"",\""name\"":\""1\"",\""serialNumber\"":\""1\"",\""calibrationDate\"":\""2023-10-24T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""e641f5aa-0f4c-4ce4-a16e-19bd9c27d697"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""2121\"",\""humidity\"":\""212\"",\""identification\"":\""12\"",\""parent\"":\""21\"",\""assetId\"":\""33\"",\""building\"":\""3\"",\""floor\"":\""2\"",\""room\"":\""2\"",\""section\"":\""2\""},\""nameplateInformation\"":{\""manufacturer\"":\""3\"",\""ratedVoltage\"":\""3\"",\""maintenanceFree\"":\""3\"",\""catalogNumber\"":\""2\"",\""ratedAmpHour\"":\""1\"",\""batteryType\"":\""2\""},\""pleaseSelectTests\"":{\""tests\"":true},\""tests\"":{\""testData\"":[{\""cellNumber\"":1,\""specificGravity\"":\""3\"",\""voltageVdc\"":\""2\"",\""waterAdded\"":\""2\"",\""batteryConductance\"":\""1\""}]},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The battery PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""33\"",\""testedBy\"":\""223\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""2\"",\""name\"":\""1\"",\""serialNumber\"":\""3\"",\""calibrationDate\"":\""2023-10-03T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""e0471d9d-3bd7-467a-b3f6-4c4fb48b9f52"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""44\"",\""humidity\"":\""34\"",\""identification\"":\""12\"",\""parent\"":\""1\"",\""assetId\"":\""44\"",\""building\"":\""4\"",\""floor\"":\""3\"",\""room\"":\""3\"",\""section\"":\""3\""},\""nameplateInformation\"":{\""manufacturer\"":\""244\"",\""breakerType\"":\""4\"",\""model\"":\""4\"",\""catalogNumber\"":\""122\"",\""mechanismType\"":\""21\"",\""trippingVoltage\"":\""44\"",\""ampereRating\"":\""4\"",\""interruptingRating\"":\""4\"",\""kvRating\"":\""4\"",\""serialNumber\"":\""42\"",\""chargingVoltage\"":\""33\"",\""closingVoltage\"":\""31\""},\""pleaseSelectTests\"":{\""visualInspection\"":true,\""operationalTests\"":true,\""resistanceInOhmsAt5kvDcContactsClosed\"":true,\""resistanceInOhmsAt5kvDcContactsOpen\"":true,\""leakageInMilliamps\"":true,\""contactResistanceTest\"":true},\""visualInspection\"":{\""circuitBreaker\"":\""normal\"",\""insulatingMembers\"":\""Normal\"",\""operatingMechanism\"":\""Damaged\"",\""electricalConnections\"":\""Normal\"",\""contactSequence\"":\""Out of Sequence\"",\""vacuumBottles\"":\""Normal\"",\""auxiliaryContacts\"":\""Dirty\"",\""cubicle\"":\""Damaged\"",\""grounding\"":\""Dirty\"",\""auxiliaryDevices\"":\""N\/A\"",\""panelLights\"":\""Operational\"",\""rackingMechanism\"":\""Not Working\"",\""cubicleHeaters\"":\""Not Working\"",\""eGap\"":\""23\"",\""vacuumBottleContactWearIndicators\"":\""121\"",\""circuitBreakerOperationCounterAsFound\"":\""55\"",\""circuitBreakerOperationCounterAsLeft\"":\""3\""},\""operationalTests\"":{\""manualOpen\"":\""Normal\"",\""electricOpen\"":\""Normal\"",\""manualClose\"":\""Slow\"",\""electricClose\"":\""Normal\"",\""manualCharge\"":\""Slow\"",\""electricCharge\"":\""Normal\"",\""openElectricallyWithCircuitProtectiveRelay\"":\""Not Working\""},\""resistanceInOhmsAt5kvDcContactsClosed\"":{\""pole1\"":\""21\"",\""pole2\"":\""21\"",\""pole3\"":\""45\""},\""resistanceInOhmsAt5kvDcContactsOpen\"":{\""pole1\"":\""54\"",\""pole2\"":\""12\"",\""pole3\"":\""444\""},\""leakageInMilliamps\"":{\""pole1\"":\""4\"",\""pole2\"":\""6\"",\""pole3\"":\""7\"",\""leakageInMilliampsAt\"":12},\""contactResistanceTest\"":{\""pole1AsFound\"":5,\""pole1AsLeft\"":5,\""pole2AsFound\"":3,\""pole2AsLeft\"":3,\""pole3AsFound\"":2,\""pole3AsLeft\"":2},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The vacuum circuit breaker PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""54\"",\""testedBy\"":\""54\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""1\"",\""name\"":\""5\"",\""serialNumber\"":\""6\"",\""calibrationDate\"":\""2023-10-03T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""cb4437f7-2595-4a8c-bfee-fa7ad55025e7"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""65\"",\""humidity\"":\""77\"",\""identification\"":\""12\"",\""parent\"":\""43\"",\""assetId\"":\""44\"",\""building\"":\""3\"",\""floor\"":\""354\"",\""room\"":\""32\"",\""section\"":\""5\""},\""nameplateInformation\"":{\""manufacturer\"":\""3\"",\""model\"":\""5\"",\""serialNumber\"":\""5\"",\""configuration\"":\""2\"",\""type\"":\""5\"",\""catalogNumber\"":\""8\"",\""voltageRating\"":\""5\"",\""mcov\"":\""54\""},\""visualInspection\"":{\""mechanicalDamage\"":\""OK\"",\""paintCondition\"":\""OK\"",\""cleanliness\"":\""See Comments\"",\""properlyGrounded\"":\""See Comments\"",\""electricalDamage\"":\""See Comments\"",\""coversAndDoors\"":\""OK\"",\""electricalConnections\"":\""OK\""},\""resistanceTest\"":{\""busToDisconnectA\"":43,\""busToDisconnectB\"":3,\""busToDisconnectC\"":2,\""disconnectToContactsB\"":4,\""disconnectToContactsA\"":8,\""disconnectToContactsC\"":9,\""disconnectToSpdC\"":7,\""disconnectToSpdB\"":5,\""disconnectToSpdA\"":4,\""spdToGroundA\"":2,\""spdToGroundB\"":4,\""spdToGroundC\"":35},\""footer\"":{\""testEquipmentNumber\"":\""223\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""The surge protector PASSED and is acceptable for operation.\"",\""testedBy\"":\""12\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""5\"",\""name\"":\""4\"",\""serialNumber\"":\""3\"",\""calibrationDate\"":\""2023-10-11T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""343ba160-e292-4e45-a74a-b3f1cd63488f"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""4\"",\""humidity\"":\""34\"",\""identification\"":\""121\"",\""parent\"":\""21\"",\""assetId\"":\""2\"",\""building\"":\""2\"",\""floor\"":\""5\"",\""room\"":\""2\"",\""section\"":\""3\""},\""nameplateInformation\"":{\""manufacturer\"":\""54\"",\""breakerType\"":\""57\"",\""model\"":\""2\"",\""catalogNumber\"":\""4\"",\""mechanismType\"":\""4\"",\""trippingVoltage\"":\""8\"",\""oilCapacity\"":\""87\"",\""ampereRating\"":\""3\"",\""interruptingRating\"":\""4\"",\""kVRating\"":\""1\"",\""serialNumber\"":\""6\"",\""changingVoltage\"":\""7\"",\""closingVoltage\"":\""8\"",\""bil\"":\""66\""},\""operationalTests\"":{\""manualOpen\"":\""ok\"",\""electricOpen\"":\""ok\"",\""manualClose\"":\""nA\"",\""electricClose\"":\""ok\"",\""manualCharge\"":\""seeComments\"",\""electricCharge\"":\""ok\"",\""openElectricallyWithCircuitProtectiveRelay\"":\""nA\""},\""visualInspection\"":{\""circuitBreaker\"":\""seeComments\"",\""damagedBushings\"":\""none\"",\""oilLevel\"":\""n\/a\"",\""oilLeaks\"":\""none\"",\""electricalConnections\"":\""normal\"",\""panelLights\"":\""nA\"",\""bushingCtRatio\"":\""11\"",\""properlyGrounded\"":\""seeComments\"",\""auxiliaryContacts\"":\""n\/a\"",\""auxiliaryDevices\"":\""operational\"",\""isThePneumaticClosingMechanismSealed\"":\""yes\"",\""isTheAirCompressorOperational\"":\""yes\"",\""pneumaticClosingMechanismPressure\"":\""121\"",\""airCompressorOilLevel\"":\""223\"",\""circuitBreakerOperationCounterAsFound\"":454,\""circuitBreakerOperationCounterAsLeft\"":555},\""pleaseSelectTests\"":{\""resistance1\"":true,\""resistance2\"":true,\""contactResistance\"":true,\""testData1\"":true,\""testData2\"":true},\""resistance1\"":{\""pole1AsFound\"":\""12\"",\""pole2AsFound\"":\""1\"",\""pole3AsFound\"":\""2\"",\""pole1AsLeft\"":\""12\"",\""pole2AsLeft\"":\""13\"",\""pole3AsLeft\"":\""2\""},\""resistance2\"":{\""pole1AsFound\"":\""12\"",\""pole2AsFound\"":\""13\"",\""pole3AsFound\"":\""12\"",\""pole1AsLeft1\"":\""12\"",\""pole2AsLeft1\"":\""121\"",\""pole3AsLeft1\"":\""121\""},\""contactResistanceTest\"":{\""pole1AsFound\"":3,\""pole1AsLeft\"":3,\""pole2AsFound\"":12,\""pole2AsLeft\"":12,\""pole3AsFound\"":32,\""pole3AsLeft\"":32},\""tankLossIndexTest\"":{\""testKV1\"":4,\""milliAmps1\"":5,\""watts1\"":6,\""powerFactor1\"":2,\""powerFactorAtTwenty1\"":1,\""milliAmps2\"":4,\""watts2\"":5,\""powerFactor2\"":25,\""powerFactorAtTwenty2\"":9,\""testKV3\"":8,\""milliAmps3\"":5,\""watts3\"":6,\""powerFactor3\"":3,\""powerFactorAtTwenty3\"":2,\""testKV4\"":1,\""milliAmps4\"":54,\""watts4\"":65,\""powerFactor4\"":6,\""powerFactorAtTwenty4\"":3,\""testKV5\"":1,\""milliAmps5\"":5,\""watts5\"":6,\""powerFactor5\"":3,\""powerFactorAtTwenty5\"":1,\""testKV6\"":4,\""milliAmps6\"":5,\""watts6\"":6,\""powerFactor6\"":3,\""testKV2\"":1,\""milliAmps7\"":7,\""watts7\"":8,\""powerFactor7\"":9,\""powerFactorAtTwenty7\"":5,\""testKV8\"":4,\""milliAmps8\"":5,\""watts8\"":56,\""powerFactor8\"":2,\""testKV9\"":4,\""milliAmps9\"":4,\""watts9\"":5,\""powerFactor9\"":5,\""powerFactorAtTwenty9\"":5,\""testKV7\"":2,\""powerFactorAtTwenty8\"":12},\""bushingTests\"":{\""manufacturer\"":\""212\"",\""style\"":\""12\"",\""model\"":\""3\"",\""catalogNumber\"":\""2\"",\""type\"":\""3\"",\""ampereRating\"":\""2\"",\""testMethod\"":\""32\"",\""kvRating\"":\""2\"",\""serial1\"":\""1\"",\""faradRating1\"":\""32\"",\""serial2\"":\""2\"",\""faradRating2\"":\""5\"",\""serial3\"":\""3\"",\""faradRating3\"":\""4\"",\""serial4\"":\""34\"",\""faradRating4\"":\""23\"",\""serial5\"":\""12\"",\""faradRating5\"":\""12\"",\""serial6\"":\""4\"",\""faradRating6\"":\""3\"",\""percentPowerFactor1\"":4,\""testKV1\"":5,\""milliAmps1\"":6,\""watts1\"":2,\""powerFactorAtTwenty1\"":1,\""percentPowerFactor2\"":12,\""testKV2\"":1,\""milliAmps2\"":21,\""watts2\"":2,\""powerFactorAtTwenty2\"":1,\""percentPowerFactor3\"":2,\""milliAmps3\"":3,\""testKV3\"":2,\""powerFactorAtTwenty3\"":1,\""watts3\"":3,\""percentPowerFactor4\"":1,\""testKV4\"":2,\""milliAmps4\"":3,\""powerFactorAtTwenty4\"":1,\""watts4\"":34,\""percentPowerFactor5\"":1,\""testKV5\"":7,\""milliAmps5\"":7,\""watts5\"":8,\""powerFactorAtTwenty5\"":6,\""percentPowerFactor6\"":3,\""testKV6\"":32,\""watts6\"":23,\""milliAmps6\"":1,\""powerFactorAtTwenty6\"":1},\""footer\"":{\""testEquipmentNumber\"":\""212\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""The Circuit Breaker PASSED and is acceptable for operation.\"",\""testedBy\"":\""98\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""7\"",\""name\"":\""6\"",\""serialNumber\"":\""0\"",\""calibrationDate\"":\""2023-10-03T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""935ad55e-0190-43c5-9d24-ecac04ad99a6"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""1233\"",\""humidity\"":\""33\"",\""identification\"":\""76\"",\""parent\"":\""54\"",\""assetId\"":\""3\"",\""building\"":\""1\"",\""floor\"":\""2\"",\""room\"":\""23\"",\""section\"":\""32\""},\""nameplateInformation\"":{\""manufacturer\"":\""2\"",\""breakerType\"":\""3\"",\""model\"":\""2\"",\""catalogNumber\"":\""2\"",\""mechanismType\"":\""2\"",\""trippingVoltage\"":\""3\"",\""ampereRating\"":\""3\"",\""interruptingRating\"":\""1\"",\""kvRating\"":\""3\"",\""serialNumber\"":\""1\"",\""chargingVoltage\"":\""1\"",\""closingVoltage\"":\""33\""},\""visualInspection\"":{\""circuitBreaker\"":\""alert\"",\""insulatingMembers\"":\""alert\"",\""operatingMechanism\"":\""acceptable\"",\""electricalConnections\"":\""acceptable\"",\""mainContacts\"":\""danger\"",\""arcingContacts\"":\""nA\"",\""contactSequence\"":\""acceptable\"",\""arcChutes\"":\""acceptable\"",\""auxiliaryContacts\"":\""acceptable\"",\""blowOutCoil\"":\""acceptable\"",\""pufferOperation\"":\""acceptable\"",\""cubicle\"":\""danger\"",\""grounding\"":\""Yes\"",\""auxiliaryDevices\"":\""acceptable\"",\""panelLights\"":\""alert\"",\""rackingMechanism\"":\""acceptable\"",\""cubicleHeaters\"":\""danger\"",\""circuitBreakerOperationCounterAsFound\"":\""87\"",\""circuitBreakerOperationCounterAsLeft\"":\""55\""},\""operationalTests\"":{\""manualOpen\"":\""acceptable\"",\""electricOpen\"":\""danger\"",\""manualClose\"":\""danger\"",\""electricClose\"":\""nA\"",\""manualCharge\"":\""acceptable\"",\""electricCharge\"":\""danger\"",\""openElectricallyWithCircuitProtectiveRelay\"":\""acceptable\""},\""pleaseSelectTests\"":{\""resistance1\"":true,\""resistance2\"":true,\""contactResistance\"":true,\""testData\"":true},\""resistance1\"":{\""pole1AsFound\"":\""1\"",\""pole2AsFound\"":\""3\"",\""pole3AsFound\"":\""5\"",\""pole1AsLeft\"":\""1\"",\""pole2AsLeft\"":\""3\"",\""pole3AsLeft\"":\""5\""},\""resistance2\"":{\""pole1AsFound\"":\""4\"",\""pole2AsFound\"":\""5\"",\""pole3AsFound\"":\""6\"",\""pole1AsLeft1\"":\""4\"",\""pole2AsLeft1\"":\""5\"",\""pole3AsLeft1\"":\""6\""},\""contactResistanceTest\"":{\""pole1AsFound\"":4,\""pole1AsLeft\"":4,\""pole2AsFound\"":5,\""pole2AsLeft\"":5,\""pole3AsFound\"":67,\""pole3AsLeft\"":67},\""testData\"":{\""testKvMovableA\"":7,\""milliampsMovableA\"":85153,\""wattsMovableA\"":5,\""testKvFixedA\"":4,\""milliampsFixedA\"":25,\""wattsFixedA\"":5,\""testKvAcrossA\"":6,\""milliampsAcrossA\"":7,\""wattsAcrossA\"":9,\""testKvMovableB\"":36,\""milliampsMovableB\"":4,\""wattsMovableB\"":5,\""testKvFixedB\"":6,\""milliampsFixedB\"":2,\""wattsFixedB\"":7,\""testKvAcrossB\"":8,\""milliampsAcrossB\"":54,\""wattsAcrossB\"":5,\""testKvMovableC\"":2,\""milliampsMovableC\"":4,\""wattsMovableC\"":58,\""testKvFixedC\"":4,\""milliampsFixedC\"":7,\""wattsFixedC\"":8,\""testKvAcrossC\"":5,\""milliampsAcrossC\"":7,\""wattsAcrossC\"":5},\""footer\"":{\""testEquipmentNumber\"":\""89\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""The Circuit Breaker PASSED and is acceptable for operation.\"",\""testedBy\"":\""99\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""45\"",\""name\"":\""2\"",\""serialNumber\"":\""12\"",\""calibrationDate\"":\""2023-10-03T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""dfaf1461-58cd-4138-871b-17a011053b03"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""87\"",\""humidity\"":\""65\"",\""identification\"":\""12\"",\""parent\"":\""12\"",\""assetId\"":\""44\"",\""building\"":\""1\"",\""floor\"":\""6\"",\""room\"":\""3\"",\""section\"":\""5\""},\""nameplateInformation\"":{\""manufacturer\"":\""66\"",\""model\"":\""4\"",\""serialNumber\"":\""6\"",\""phaseAmpereRating\"":\""5\"",\""configuration\"":\""3PH\/3W\"",\""type\"":\""32\"",\""catalogNumber\"":\""6\"",\""voltageRating\"":\""8\"",\""neutralAmpereRating\"":\""1\"",\""faultWithstandRating\"":\""33\""},\""pleaseSelectTests\"":{\""visualInspection\"":true,\""insulationResistanceTest\"":true,\""contactResistance\"":true},\""visualInspection\"":{\""mechanicalDamage\"":\""None\"",\""paintCondition\"":\""OK\"",\""cleanliness\"":\""Dirty\/Cleaned\"",\""properlyGrounded\"":\""Yes\"",\""electricalConnections\"":\""OK\"",\""electricalDamage\"":\""See Comments\"",\""neutralBonding\"":\""Correct\"",\""coversAndDoors\"":\""OK\"",\""systemVoltage\"":\""767\""},\""insulationResistanceTest\"":{\""aPhaseAsFound\"":\""2\"",\""bPhaseAsFound\"":\""5\"",\""cPhaseAsFound\"":\""7\"",\""nPhaseAsFound\"":\""8\"",\""aPhaseAsLeft\"":\""2\"",\""bPhaseAsLeft\"":\""5\"",\""cPhaseAsLeft\"":\""7\"",\""nPhaseAsLeft\"":\""8\"",\""aTestVoltage\"":21,\""bTestVoltage\"":3,\""cTestVoltage\"":6,\""cTestVoltage1\"":8},\""contactResistance\"":{\""p1AsFound\"":87,\""p1AsLeft\"":87,\""p2AsFound\"":6,\""p2AsLeft\"":6,\""p3AsFound\"":86,\""p3AsLeft\"":86},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The switchboard PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""909\"",\""testedBy\"":\""55\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""3\"",\""name\"":\""56\"",\""serialNumber\"":\""8\"",\""calibrationDate\"":\""2023-10-03T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""2101b685-f9cd-42eb-971b-c6b5d0436e1c"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""131\"",\""humidity\"":\""6556\"",\""identification\"":\""12\"",\""parent\"":\""3\"",\""assetId\"":\""3\"",\""building\"":\""12\"",\""floor\"":\""4\"",\""room\"":\""33\"",\""section\"":\""43\""},\""nameplateInformation\"":{\""manufacturer\"":\""S & C\"",\""ampereRating\"":\""1\"",\""catalogNumber\"":\""3\"",\""faultWithstandRating\"":\""5\"",\""serialNumber\"":\""6\"",\""voltageRating\"":\""21\"",\""switchType\"":\""Vista\"",\""modelNumber\"":\""4\"",\""interruptingRating\"":\""54\"",\""contactType\"":\""SF6\""},\""visualInspection\"":{\""contactWearIndicators\"":\""N\/A\"",\""contactSequence\"":\""Acceptable\"",\""cubicleAndInsulatingMembers\"":\""Alert\"",\""damagedInsulators\"":\""Alert\"",\""electricalConnections\"":\""Acceptable\"",\""fuseType\"":\""N\/A\"",\""spareFusesPresent\"":\""N\/A\"",\""auxiliaryDevices\"":\""N\/A\"",\""operatingMechanism\"":\""Alert\"",\""switchGrounded\"":\""Acceptable\"",\""systemVoltage\"":\""24900\"",\""panelLights\"":\""N\/A\"",\""heaters\"":\""N\/A\"",\""shuntTripVoltage\"":\""N\/A\"",\""fuseSizes\"":\""N\/A\"",\""fuseRefillNumber\"":\""545\""},\""operationalTests\"":{\""manualOpen\"":\""Acceptable\"",\""manualClose\"":\""Acceptable\"",\""electricallyOpen\"":\""Acceptable\"",\""electricallyClose\"":\""Acceptable\"",\""snufferOperation\"":\""Acceptable\"",\""arcingContactOperation\"":\""Acceptable\"",\""fuseCutoutOperation\"":\""Alert\"",\""keyInterlockOperation\"":\""Danger\"",\""doorInterlockOperation\"":\""Acceptable\"",\""lockingMechanism\"":\""Acceptable\"",\""auxiliaryDevices\"":\""Acceptable\"",\""shuntTrip\"":\""Acceptable\""},\""testData1\"":{\""resistanceContactsClosedAsFound1\"":\""1\"",\""resistanceContactsClosedAsLeft1\"":\""15\"",\""resistanceContactsClosedAsFound2\"":\""6\"",\""resistanceContactsClosedAsLeft2\"":\""632\"",\""resistanceContactsClosedAsFound3\"":\""5\"",\""resistanceContactsClosedAsLeft3\"":\""55\"",\""resistanceContactsOpenAsFound1\"":\""7\"",\""resistanceContactsOpenAsLeft1\"":\""78\"",\""resistanceContactsOpenAsFound2\"":\""9\"",\""resistanceContactsOpenAsLeft2\"":\""98\"",\""resistanceContactsOpenAsFound3\"":\""58\"",\""resistanceContactsOpenAsLeft3\"":\""586\"",\""leakageCurrent\"":32,\""leakageResult1\"":1,\""leakageResult2\"":3,\""leakageResult3\"":4,\""contactResistanceAsFound1\"":4,\""contactResistanceAsLeft1\"":84,\""contactResistanceAsFound2\"":6,\""contactResistanceAsLeft2\"":86,\""contactResistanceAsFound3\"":6,\""contactResistanceAsLeft3\"":9},\""testData2\"":{\""resistanceContactsClosedAsFound1\"":\""4\"",\""resistanceContactsClosedAsLeft1\"":\""425\"",\""resistanceContactsClosedAsFound2\"":\""8\"",\""resistanceContactsClosedAsLeft2\"":\""88\"",\""resistanceContactsClosedAsFound3\"":\""5\"",\""resistanceContactsClosedAsLeft3\"":\""58\"",\""resistanceContactsOpenAsFound1\"":\""7\"",\""resistanceContactsOpenAsLeft1\"":\""78\"",\""resistanceContactsOpenAsFound2\"":\""8\"",\""resistanceContactsOpenAsLeft2\"":\""88\"",\""resistanceContactsOpenAsFound3\"":\""478\"",\""resistanceContactsOpenAsLeft3\"":\""4785\"",\""leakageCurrent\"":7,\""leakageResult1\"":8,\""leakageResult2\"":98,\""leakageResult3\"":78,\""contactResistanceAsFound1\"":25,\""contactResistanceAsLeft1\"":74,\""contactResistanceAsFound2\"":8,\""contactResistanceAsLeft2\"":58,\""contactResistanceAsFound3\"":7,\""contactResistanceAsLeft3\"":87},\""testData3\"":{\""resistanceContactsClosedAsFound1\"":\""4\"",\""resistanceContactsClosedAsLeft1\"":\""25\"",\""resistanceContactsClosedAsFound2\"":\""3\"",\""resistanceContactsClosedAsLeft2\"":\""356\"",\""resistanceContactsClosedAsFound3\"":\""9\"",\""resistanceContactsClosedAsLeft3\"":\""6\"",\""resistanceContactsOpenAsFound1\"":\""7\"",\""resistanceContactsOpenAsLeft1\"":\""78\"",\""resistanceContactsOpenAsFound2\"":\""94\"",\""resistanceContactsOpenAsLeft2\"":\""945\"",\""resistanceContactsOpenAsFound3\"":\""6\"",\""resistanceContactsOpenAsLeft3\"":\""61\"",\""leakageCurrent\"":7,\""leakageResult1\"":8,\""leakageResult2\"":9,\""leakageResult3\"":56,\""contactResistanceAsFound1\"":2,\""contactResistanceAsLeft1\"":72,\""contactResistanceAsFound2\"":9,\""contactResistanceAsLeft2\"":69,\""contactResistanceAsFound3\"":3,\""contactResistanceAsLeft3\"":5},\""testData4\"":{\""resistanceContactsClosedAsFound1\"":\""2\"",\""resistanceContactsClosedAsLeft1\"":\""24\"",\""resistanceContactsClosedAsFound2\"":\""7\"",\""resistanceContactsClosedAsLeft2\"":\""78\"",\""resistanceContactsClosedAsFound3\"":\""9\"",\""resistanceContactsClosedAsLeft3\"":\""96\"",\""resistanceContactsOpenAsFound1\"":\""5\"",\""resistanceContactsOpenAsLeft1\"":\""532\"",\""resistanceContactsOpenAsFound2\"":\""7\"",\""resistanceContactsOpenAsLeft2\"":\""79\"",\""resistanceContactsOpenAsFound3\"":\""6\"",\""resistanceContactsOpenAsLeft3\"":\""63\"",\""leakageCurrent\"":7,\""leakageResult1\"":5,\""leakageResult2\"":3,\""leakageResult3\"":1,\""contactResistanceAsFound1\"":7,\""contactResistanceAsLeft1\"":97,\""contactResistanceAsFound2\"":6,\""contactResistanceAsLeft2\"":36,\""contactResistanceAsFound3\"":18,\""contactResistanceAsLeft3\"":198},\""testData5\"":{\""overcurrent900AsFound1\"":5,\""overcurrent900AsLeft1\"":8,\""overcurrent900AsFound2\"":6,\""overcurrent900AsLeft2\"":3,\""overcurrent900AsFound3\"":4,\""overcurrent900AsLeft3\"":84,\""overcurrent600AsLeft1\"":7,\""overcurrent600AsFound2\"":9,\""overcurrent600AsLeft2\"":639,\""overcurrent600AsFound3\"":7,\""overcurrent600AsLeft3\"":67,\""overcurrent600AsFound1\"":2,\""overcurrent900AsFound4\"":4,\""overcurrent900AsLeft4\"":4,\""overcurrent900AsFound5\"":3,\""overcurrent900AsLeft5\"":3,\""overcurrent900AsFound6\"":4,\""overcurrent900AsLeft6\"":4,\""overcurrent600AsFound6\"":5,\""overcurrent600AsLeft6\"":5,\""overcurrent600AsFound5\"":4,\""overcurrent600AsLeft5\"":4,\""overcurrent600AsFound4\"":5,\""overcurrent600AsLeft4\"":5},\""footer\"":{\""testEquipmentNumber\"":\""2\"",\""inspectionVerdict\"":\""acceptable\"",\""comments\"":\""The Switch is acceptable for operation.\"",\""testedBy\"":\""23\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""1\"",\""name\"":\""23\"",\""serialNumber\"":\""4\"",\""calibrationDate\"":\""2023-10-04T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""717fc2bf-0e4a-43dc-95c0-11ddbf93270a"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""54\"",\""humidity\"":\""44\"",\""identification\"":\""1\"",\""parent\"":\""3\"",\""assetId\"":\""4\"",\""building\"":\""2\"",\""floor\"":\""2\"",\""room\"":\""1\"",\""section\"":\""3\""},\""nameplateInformation\"":{\""manufacturer\"":\""4\"",\""catalogNumber\"":\""4\"",\""styleNumber\"":\""2\"",\""voltageRating\"":\""7\"",\""timeAtRatedAmperage\"":\""56\"",\""type\"":\""2\"",\""modelNumber\"":\""4\"",\""serialNumber\"":\""7\"",\""ampereRating\"":\""9\"",\""wattageRating\"":\""3\""},\""pleaseSelectTests\"":{\""visualInspection\"":true,\""insulationResistanceTests\"":true},\""visualInspection\"":{\""primaryConnections\"":\""OK\"",\""properlyGrounded\"":\""Yes\"",\""bushing\"":\""N\/A\"",\""resistorCondition\"":\""OK\""},\""insulationResistanceTests\"":{\""insulationResistanceTest\"":\""65\"",\""measuredResistorValueInOhms\"":\""6\""},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The ground resistor PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""86\"",\""testedBy\"":\""6\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""4\"",\""name\"":\""2\"",\""serialNumber\"":\""4\"",\""calibrationDate\"":\""2023-10-11T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""b03505c8-9a05-49b0-a172-8aaf0801e0d5"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""54\"",\""humidity\"":\""3\"",\""identification\"":\""6\"",\""parent\"":\""8\"",\""assetId\"":\""1\"",\""building\"":\""8\"",\""floor\"":\""3\"",\""room\"":\""3\"",\""section\"":\""4\""},\""nameplateInformation\"":{\""manufacturer\"":\""75\"",\""insulationLevel\"":\""4\"",\""catalogNumber\"":\""5\"",\""primaryVoltage\"":\""5\"",\""turnsRatio\"":\""6\"",\""class\"":\""2\"",\""polarity\"":\""5\"",\""va\"":\""6\"",\""modelNumber\"":\""4\"",\""styleNumber\"":\""6\"",\""secondaryVoltage\"":\""4\"",\""bil\"":\""4\"",\""frequency\"":\""3\""},\""pleaseSelectTests\"":{\""visualInspection\"":true,\""raitoPolarityTests\"":true,\""insulationResistance\"":true},\""visualInspection\"":{\""primaryConnections\"":\""OK\"",\""secondaryConnections\"":\""See Comments\"",\""properlyGrounded\"":\""OK\"",\""bushingsInsulation\"":\""OK\""},\""raitoPolarityTests\"":{\""aNABSerialNumber\"":\""6\"",\""bNBCSerialNumber\"":\""4\"",\""cNACSerialNumber\"":\""3\"",\""aNABCalculatedRatio\"":\""45\"",\""bNBCCalculatedRatio\"":\""45\"",\""cNACCalculatedRatio\"":\""5\"",\""aNABMeasuredRatio\"":\""4\"",\""bNBCMeasuredRatio\"":\""2\"",\""cNACMeasuredRatio\"":\""6\"",\""polarity1\"":\""4\"",\""polarity2\"":\""2\"",\""polarity3\"":\""4\""},\""insulationResistance\"":{\""primaryToGround\"":\""13\"",\""aNABPrimaryToGround\"":\""2\"",\""bNBCPrimaryToGround\"":\""3\"",\""cNACPrimaryToGround\"":\""5\"",\""primaryToSecondary\"":\""4\"",\""aNABPrimaryToSecondary\"":\""3\"",\""bNBCPrimaryToSecondary\"":\""2\"",\""cNACPrimaryToSecondary\"":\""1\"",\""secondaryToGround\"":\""2\"",\""aNABSecondaryToGround\"":\""3\"",\""bNBCSecondaryToGround\"":\""34\"",\""cNACSecondaryToGround\"":\""34\""},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The potential transformer PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""54\"",\""testedBy\"":\""5\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""4\"",\""name\"":\""32\"",\""serialNumber\"":\""4\"",\""calibrationDate\"":\""2023-10-03T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""b4084a37-e065-4a4f-96d6-da40783dc267"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""4\"",\""humidity\"":\""66\"",\""identification\"":\""23\"",\""parent\"":\""5\"",\""assetId\"":\""6\"",\""building\"":\""12\"",\""floor\"":\""3\"",\""room\"":\""32\"",\""section\"":\""3\""},\""nameplateInformation\"":{\""manufacturer\"":\""12\"",\""style\"":\""5\"",\""modelNumber\"":\""4\"",\""currentRating\"":\""4\"",\""cTRatio\"":\""6\"",\""controlVoltage\"":\""44\"",\""type\"":\""3\"",\""catalogNumber\"":\""4\"",\""serialNumber\"":\""68\"",\""voltageRatio\"":\""8\"",\""pTRatio\"":\""6\""},\""pleaseSelectTests\"":{\""powerTestData\"":true,\""ampereTestData\"":true,\""voltageTestData\"":true},\""powerTestData\"":{\""calculatedWatts\"":\""2\"",\""injectedWatts\"":21,\""injectedWattsAsFound\"":4,\""injectedWattsAsLeft\"":54,\""injectedPowerFactorAsFound\"":4,\""injectedPowerFactorAsLeft\"":34},\""ampereTestData\"":{\""injectedCurrent\"":2,\""APhase2\"":4,\""CPhase2\"":24,\""calculatedScale\"":4,\""BPhase2\"":41},\""voltageTestData\"":{\""calculatedScale\"":\""3\"",\""APhase2\"":\""5\"",\""BPhase2\"":\""76\"",\""CPhase2\"":\""2\"",\""injectedVoltage\"":12},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The multifunction meter PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""12\"",\""testedBy\"":\""43\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""4\"",\""name\"":\""55\"",\""serialNumber\"":\""6\"",\""calibrationDate\"":\""2023-10-25T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""883ba335-3970-48b7-af22-080621013964"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""65\"",\""humidity\"":\""4\"",\""identification\"":\""2\"",\""parent\"":\""322\"",\""assetId\"":\""1\"",\""building\"":\""32\"",\""floor\"":\""3\"",\""room\"":\""4\"",\""section\"":\""5\""},\""nameplateInformation\"":{\""manufacturer\"":\""12\"",\""ratio\"":\""6\"",\""frequency\"":\""54\"",\""accuracyClass\"":\""7\"",\""relayingClass\"":\""1\"",\""ratingFactor\"":\""5\"",\""type\"":\""4\"",\""catalogNumber\"":\""2\"",\""bil\"":\""3\"",\""pole1SerialNumber\"":\""2\"",\""pole2SerialNumber\"":\""2\"",\""pole3SerialNumber\"":\""4\""},\""visualInspection\"":{\""primaryConnection\"":\""loose\"",\""properlyGrounded\"":\""yes\"",\""shortingDevices\"":\""554\"",\""secondaryConnection\"":\""tight\"",\""bushingInsulation\"":\""ok\""},\""ratioAndPolarityTests\"":{\""ratio_1\"":\""7\"",\""pole1_1\"":\""8\"",\""pole2_1\"":\""7\"",\""pole3_1\"":\""8\"",\""ratio_2\"":\""9\"",\""pole1_2\"":\""4\"",\""pole2_2\"":\""5\"",\""pole3_2\"":\""6\"",\""ratio_3\"":\""3\"",\""pole1_3\"":\""54\"",\""pole2_3\"":\""8\"",\""pole3_3\"":\""1\"",\""ratio_4\"":\""5\"",\""pole1_4\"":\""9\"",\""pole2_4\"":\""2\"",\""pole3_4\"":\""7\"",\""ratio_5\"":\""4\"",\""pole1_5\"":\""5\"",\""pole2_5\"":\""6\"",\""pole3_5\"":\""3\"",\""ratio_6\"":\""1\"",\""pole1_6\"":\""5\"",\""pole2_6\"":\""3\"",\""pole3_6\"":\""4\"",\""ratio_7\"":\""12\"",\""pole1_7\"":\""7\"",\""pole2_7\"":\""8\"",\""pole3_7\"":\""32\"",\""ratio_8\"":\""12\"",\""pole1_8\"":\""8\"",\""pole2_8\"":\""9\"",\""pole3_8\"":\""3\"",\""ratio_9\"":\""1\"",\""pole1_9\"":\""2\"",\""pole2_9\"":\""4\"",\""pole3_9\"":\""2\"",\""ratio_10\"":\""4\"",\""pole1_10\"":\""5\"",\""pole2_10\"":\""7\"",\""pole3_10\"":\""8\""},\""saturationTests\"":{\""currentFlow\"":\""11\"",\""currentFlow1Pole1\"":\""2\"",\""currentFlow1Pole2\"":\""7\"",\""currentFlow1Pole3\"":\""8\"",\""currentFlow1\"":\""1\"",\""currentFlow2Pole1\"":\""2\"",\""currentFlow2Pole2\"":\""4\"",\""currentFlow2Pole3\"":\""8\"",\""currentFlow2\"":\""1\"",\""currentFlow3Pole1\"":\""5\"",\""currentFlow3Pole2\"":\""7\"",\""currentFlow3Pole3\"":\""8\"",\""currentFlow3\"":\""1\"",\""currentFlow4Pole1\"":\""2\"",\""currentFlow4Pole2\"":\""4\"",\""currentFlow4Pole3\"":\""8\"",\""currentFlow4\"":\""2\"",\""currentFlow5Pole1\"":\""4\"",\""currentFlow5Pole2\"":\""8\"",\""currentFlow5Pole3\"":\""5\"",\""currentFlow5\"":\""7\"",\""currentFlow6Pole1\"":\""98\"",\""currentFlow6Pole2\"":\""6\"",\""currentFlow6Pole3\"":\""1\"",\""currentFlow6\"":\""8\"",\""currentFlow7Pole1\"":\""96\"",\""currentFlow7Pole2\"":\""23\"",\""currentFlow7Pole3\"":\""1\""},\""insulationResistance\"":{\""primaryToGround\"":\""8\"",\""primaryToGroundPole1\"":\""65\"",\""primaryToGroundPole2\"":\""4\"",\""primaryToGroundPole3\"":\""5\"",\""primaryToSecondary\"":\""41\"",\""primaryToSecondaryPole1\"":\""5\"",\""primaryToSecondaryPole2\"":\""3\"",\""primaryToSecondaryPole3\"":\""4\"",\""secondaryToGround\"":\""5\"",\""secondaryToGroundPole1\"":\""6\"",\""secondaryToGroundPole2\"":\""5\"",\""secondaryToGroundPole3\"":\""6\""},\""burdenTest\"":{\""injectedCurrent1\"":\""7\"",\""injectedCurrent2\"":\""98\"",\""injectedCurrent3\"":\""5\"",\""resistance1\"":\""4\"",\""resistance2\"":\""5\"",\""resistance3\"":\""5\""},\""footer\"":{\""testEquipmentNumber\"":\""444\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""The Transformer PASSED and is acceptable for operation.\"",\""testedBy\"":\""33\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""2\"",\""name\"":\""4\"",\""serialNumber\"":\""3\"",\""calibrationDate\"":\""2023-10-10T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""acd57276-5786-4369-96dc-7dad2ec2782c"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-04T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""212\"",\""humidity\"":\""33\"",\""identification\"":\""2\"",\""parent\"":\""4\"",\""assetId\"":\""5\"",\""building\"":\""2\"",\""floor\"":\""3\"",\""room\"":\""4\"",\""section\"":\""3\""},\""nameplateInformation\"":{\""manufacturer\"":\""3\"",\""modelCatalogNumber\"":\""2\"",\""serialNumber\"":\""2\"",\""frame\"":\""26\"",\""serviceFactor\"":\""6\"",\""temperatureRating\"":\""4\"",\""instructionBook\"":\""2\"",\""voltageRating\"":\""2\"",\""fullLoadAmps\"":\""3\"",\""horsepower\"":\""3\"",\""phaseWire\"":\""7\"",\""numberOfPoles\"":\""6\"",\""rpm\"":\""5\"",\""dutyCycle\"":\""4\""},\""testConnection\"":{\""energized\"":\""23\"",\""guarded\"":\""yes\"",\""grounded\"":\""frame\"",\""testVoltageKvDc\"":\""23\""},\""transformerDcInsulationResistance\"":{\""dataGrid\"":[{\""timeInMinutes\"":0.25,\""insulationResistance1\"":\""1\"",\""insulationResistance2\"":\""\"",\""insulationResistance3\"":\""\""}],\""polarizationRatio\"":\""\"",\""polarizationIndex\"":\""\""},\""footer\"":{\""testEquipmentNumber\"":\""45\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""The motor PASSED and is acceptable for operation.\"",\""testedBy\"":\""34\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""21\"",\""name\"":\""4\"",\""serialNumber\"":\""4\"",\""calibrationDate\"":\""2023-10-03T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""acde16cd-bdde-470b-a27a-8c189ee52d24"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""2\"",\""humidity\"":\""3\"",\""identification\"":\""2\"",\""parent\"":\""3\"",\""assetId\"":\""1\"",\""building\"":\""3\"",\""floor\"":\""3\"",\""room\"":\""3\"",\""section\"":\""2\""},\""nameplateInformation\"":{\""manufacturer\"":\""S&C\"",\""modelNumber\"":\""3\"",\""relayType\"":\""23\"",\""serialNumber\"":\""2\"",\""relaySettings\"":{\""pleaseSelectTests\"":{\""50p\"":true,\""50n\"":true,\""51p\"":true,\""51n\"":true},\""phaseRotation\"":\""2\"",\""phaseCtRatio\"":\""3\"",\""groundCtRatio\"":\""1\"",\""function50PSettings\"":{\""phasePickup\"":\""2\""},\""function50PSettings1\"":{\""phaseTimeDelay\"":\""3\""},\""function50NSettings\"":{\""neutralPickup\"":\""3\""},\""function50NSettings1\"":{\""neutralTimeDelay\"":\""1\""},\""function51PSettings\"":{\""pickup\"":\""4\"",\""curveType\"":\""u4\""},\""function51PSettings1\"":{\""timeDial\"":\""5\"",\""electroMechanicalReset\"":\""Yes\""},\""function51NSettings\"":{\""pickup\"":\""5\"",\""curveType\"":\""u1\""},\""function51NSettings1\"":{\""timeDial\"":\""3\"",\""electroMechanicalReset\"":\""No\""}}},\""electricalTests\"":{\""function50InstantaneousOvercurrentTests\"":{\""mfgCurvePickup\"":\""1.89 - 2.11\"",\""timeDelay\"":\""2.75 - 3.25\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""aPhaseDelay\"":\""\"",\""bPhaseDelay\"":\""\"",\""cPhaseDelay\"":\""\"",\""manualOverwrite\"":{\""iWantToOverwriteCalculatedValues\"":false,\""true\"":false}},\""function51PhaseOvercurrentTests\"":{\""mfgCurvePickup\"":\""3.83 - 4.17\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveDelay200\"":\""9.22 - 10.04\"",\""aPhaseDelay200\"":\""\"",\""bPhaseDelay200\"":\""\"",\""cPhaseDelay200\"":\""\"",\""mfgCurveDelay300\"":\""3.55 - 3.89\"",\""aPhaseDelay300\"":\""\"",\""bPhaseDelay300\"":\""\"",\""cPhaseDelay300\"":\""\"",\""mfgCurveDelay500\"":\""1.28 - 1.44\"",\""aPhaseDelay500\"":\""\"",\""bPhaseDelay500\"":\""\"",\""cPhaseDelay500\"":\""\"",\""manualOverwrite\"":{\""iWantToOverwriteCalculatedValues\"":false,\""true\"":false}},\""function50NInstantaneousNeutralOvercurrentTests\"":{\""mfgCurvePickup\"":\""2.86 - 3.14\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""timeDelay\"":\""0.92 - 1.08\"",\""aPhaseDelay\"":\""\"",\""bPhaseDelay\"":\""\"",\""cPhaseDelay\"":\""\"",\""manualOverwrite1\"":{\""iWantToOverwriteCalculatedValues\"":false,\""true\"":false}},\""function51NNeutralOvercurrentTests\"":{\""mfgCurvePickup\"":\""4.80 - 5.20\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveDelay200\"":\""2.19 - 2.42\"",\""aPhaseDelay200\"":\""\"",\""bPhaseDelay200\"":\""\"",\""cPhaseDelay200\"":\""\"",\""mfgCurveDelay300\"":\""1.97 - 2.19\"",\""aPhaseDelay300\"":\""\"",\""bPhaseDelay300\"":\""\"",\""cPhaseDelay300\"":\""\"",\""mfgCurveDelay500\"":\""1.54 - 1.72\"",\""aPhaseDelay500\"":\""\"",\""bPhaseDelay500\"":\""\"",\""cPhaseDelay500\"":\""\"",\""manualOverwrite2\"":{\""iWantToOverwriteCalculatedValues\"":false,\""true\"":false}}},\""footer\"":{\""testEquipmentNumber\"":\""4\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""passed\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""6\"",\""name\"":\""5\"",\""serialNumber\"":\""7\"",\""calibrationDate\"":\""2023-10-17T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""3b93cfa4-5cef-465d-af6e-6b6422923ec4"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""5\"",\""humidity\"":\""4\"",\""identification\"":\""12\"",\""parent\"":\""3\"",\""assetId\"":\""4\"",\""building\"":\""1\"",\""floor\"":\""3\"",\""room\"":\""3\"",\""section\"":\""4\""},\""nameplateInformation\"":{\""manufacturer\"":\""3\"",\""serialNumber\"":\""4\"",\""kVRating\"":\""4\"",\""ampereRating\"":\""3\""},\""test1\"":{\""m1Current1\"":\""12\"",\""m1GroundCurrent1\"":\""6\"",\""tieCurrent1\"":\""3\"",\""tieGroundCurrent1\"":\""6\"",\""m2Current1\"":\""4\"",\""m2GroundCurrent1\"":\""5\"",\""m1Current2\"":\""12\"",\""m1GroundCurrent2\"":\""6\"",\""tieCurrent2\"":\""3\"",\""tieGroundCurrent2\"":\""6\"",\""m2Current2\"":\""4\"",\""m2GroundCurrent2\"":\""5\"",\""m1Current3\"":\""12\"",\""m1GroundCurrent3\"":\""6\"",\""tieCurrent3\"":\""3\"",\""tieGroundCurrent3\"":\""6\"",\""m2Current3\"":\""4\"",\""m2GroundCurrent3\"":\""5\"",\""m1Current4\"":\""12\"",\""m1GroundCurrent4\"":\""6\"",\""tieCurrent4\"":\""3\"",\""tieGroundCurrent4\"":\""6\"",\""m2Current4\"":\""4\"",\""m2GroundCurrent4\"":\""5\""},\""test2\"":{\""m1Current1\"":\""7\"",\""m1GroundCurrent1\"":\""67\"",\""tieCurrent1\"":\""4\"",\""tieGroundCurrent1\"":\""2\"",\""m2Current1\"":\""1\"",\""m2GroundCurrent1\"":\""4\"",\""m1Current2\"":\""7\"",\""m1GroundCurrent2\"":\""67\"",\""tieCurrent2\"":\""4\"",\""tieGroundCurrent2\"":\""2\"",\""m2Current2\"":\""1\"",\""m2GroundCurrent2\"":\""4\"",\""m1Current3\"":\""7\"",\""m1GroundCurrent3\"":\""67\"",\""tieCurrent3\"":\""4\"",\""tieGroundCurrent3\"":\""2\"",\""m2Current3\"":\""1\"",\""m2GroundCurrent3\"":\""4\"",\""m1Current4\"":\""7\"",\""m1GroundCurrent4\"":\""67\"",\""tieCurrent4\"":\""4\"",\""tieGroundCurrent4\"":\""2\"",\""m2Current4\"":\""1\"",\""m2GroundCurrent4\"":\""4\""},\""footer\"":{\""testEquipmentNumber\"":\""99\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""The Insulation Resistance PASSED and is acceptable for operation.\"",\""testedBy\"":\""12\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""1\"",\""name\"":\""7\"",\""serialNumber\"":\""2\"",\""calibrationDate\"":\""2023-10-04T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""526b619c-f7e9-4459-b82d-7f5f3064fc48"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-05T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""76\"",\""humidity\"":\""7\"",\""identification\"":\""13\"",\""parent\"":\""34\"",\""assetId\"":\""2\"",\""building\"":\""1\"",\""floor\"":\""4\"",\""room\"":\""4\"",\""section\"":\""11\""},\""nameplateInformation1\"":{\""manufacturer\"":\""2\"",\""length\"":\""8\"",\""size\"":\""54\"",\""insulationType\"":\""4\"",\""typeOfTest\"":\""acceptance\"",\""ratedVoltage\"":\""4\"",\""operatingVoltage\"":\""6\"",\""factoryTestVoltage\"":\""7\"",\""maximumTestVoltage\"":\""6\"",\""externalEquipment\"":\""12\""},\""testData1\"":{\""dataGrid\"":[{\""timeMinutes\"":0.25,\""aPhaseOhms\"":\""2\"",\""aPhaseOhms1\"":\""3\"",\""aPhaseOhms2\"":\""4\"",\""aPhaseOhms3\"":\""6\""}]},\""footer\"":{\""testEquipmentNumber\"":\""44\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""The Cable Insulation Resistance PASSED and is acceptable for operation.\"",\""testedBy\"":\""34\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""4\"",\""name\"":\""3\"",\""serialNumber\"":\""2\"",\""calibrationDate\"":\""2023-10-12T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""0e4d58bf-d9de-4e47-bdde-bad11c5ff43a"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-04T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""54\"",\""humidity\"":\""3\"",\""identification\"":\""2\"",\""parent\"":\""4\"",\""assetId\"":\""5\"",\""building\"":\""3\"",\""floor\"":\""3\"",\""room\"":\""5\"",\""section\"":\""3\""},\""nameplateInformation\"":{\""manufacturer\"":\""2\"",\""kVa\"":\""5\"",\""modelNumber\"":\""7\"",\""primaryVoltage\"":\""4\"",\""amps\"":\""5\"",\""serialNumber\"":\""3\"",\""numberOfBranchCircuits\"":\""8\"",\""secondaryVoltage\"":\""6\""},\""visualInspection\"":{\""operatingCurrent\"":\""3\"",\""systemOperatingVoltage\"":\""5\"",\""noAlarmSet\"":\""6\"",\""warningArea\"":\""6\"",\""testPanel\"":\""seeComments\"",\""meterFullScale\"":\""5\"",\""alarm\"":\""3\"",\""operatingRange\"":\""4\""},\""pretestDataLeakageShouldBeLessThan5MAAllBranchBreakerClosed\"":{\""l1LeakageMA\"":\""4\"",\""l2LeakageMA\"":\""6\""},\""receptacleCheck\"":{\""dataGrid\"":[{\""circuits\"":1,\""ofReceptaclesPerCircuit\"":6,\""l1LeakageMA\"":6,\""l2LeakageMA\"":4,\""condition\"":\""6\""}]},\""limTripCurrent\"":{\""l1TripPoint\"":\""4\"",\""l2TripPoint\"":\""5\""},\""footer\"":{\""testEquipmentNumber\"":\""212\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""The Monitor PASSED and is acceptable for operation.\"",\""testedBy\"":\""5\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""6\"",\""name\"":\""6\"",\""serialNumber\"":\""8\"",\""calibrationDate\"":\""2023-10-04T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""9d8d4db1-f4e3-4dee-a33d-877eb4aecd4e"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-10T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""32\"",\""humidity\"":\""3\"",\""identification\"":\""1\"",\""parent\"":\""3\"",\""assetId\"":\""4\"",\""building\"":\""1\"",\""floor\"":\""2\"",\""room\"":\""2\"",\""section\"":\""4\"",\""workOrderType\"":\""Acceptance Test\""},\""nameplateInformation\"":{\""manufacturer\"":\""3\"",\""protectionClass\"":\""Distribution\"",\""model\"":\""32\"",\""catalogNumber\"":\""3\"",\""style\"":\""23\"",\""MCOV\"":\""4\"",\""dutyCycle\"":\""33\"",\""aPhaseSerialNumber\"":\""2\"",\""bPhaseSerialNumber\"":\""4\"",\""cPhaseSerialNumber\"":\""4\""},\""pleaseSelectTests\"":{\""visualInspection\"":true,\""doblePowerFactor\"":true,\""dcInsulationTest\"":true,\""acLeakageTest\"":true},\""visualInspection\"":{\""electricalAndGroundConnections\"":\""N\/A\"",\""properlyBoundedToTheGroundingElectrode\"":\""Yes\"",\""dirtOrSaltDeposits\"":\""Cleaned\"",\""sealsGasketsBlowOutPorts\"":\""N\/A\"",\""flashedOrChippedPorcelainOrInsulation\"":\""Yes\"",\""systemVoltage\"":\""23\""},\""doblePowerFactor\"":{\""choiceMaMva\"":\""\"",\""choiceMwWatts\"":\""\"",\""stepATestVoltage\"":\""2\"",\""phaseAAmps\"":\""4\"",\""phaseAWatts\"":\""5\"",\""stepBTestVoltage\"":\""2\"",\""phaseBAmps\"":\""3\"",\""phaseBWatts\"":\""4\"",\""stepCTestVoltage\"":\""4\"",\""phaseCAmps\"":\""5\"",\""phaseCWatts\"":\""6\""},\""dcInsulationTest\"":{\""phaseATestKv\"":\""3\"",\""phaseAResistance\"":\""5\"",\""phaseBTestKv\"":\""5\"",\""phaseBResistance\"":\""6\"",\""phaseCTestKv\"":\""3\"",\""phaseCResistance\"":\""6\""},\""acLeakageTest\"":{\""step1Kv\"":\""3\"",\""step2Kv\"":\""3\"",\""step3Kv\"":\""5\"",\""step4Kv\"":\""34\"",\""step5Kv\"":\""2\"",\""step1PhaseA\"":\""23\"",\""step1PhaseB\"":\""3\"",\""step1PhaseC\"":\""32\"",\""step2PhaseA\"":\""32\"",\""step2PhaseB\"":\""\"",\""step2PhaseC\"":\""23\"",\""step3PhaseA\"":\""6\"",\""step3PhaseB\"":\""8\"",\""step3PhaseC\"":\""9\"",\""step4PhaseA\"":\""3\"",\""step4PhaseB\"":\""4\"",\""step4PhaseC\"":\""2\"",\""step5PhaseA\"":\""6\"",\""step5PhaseB\"":\""5\"",\""step5PhaseC\"":\""5\""},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The lightning arresters PASSED and are acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""4\"",\""testedBy\"":\""34\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""6\"",\""name\"":\""8\"",\""serialNumber\"":\""4\"",\""calibrationDate\"":\""2023-10-05T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""67ef22e5-ecb9-45ec-9c1e-f08c3a73c216"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-04T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""32\"",\""humidity\"":\""3\"",\""identification\"":\""2\"",\""parent\"":\""3\"",\""assetId\"":\""4\"",\""building\"":\""1\"",\""floor\"":\""4\"",\""room\"":\""4\"",\""section\"":\""3\""},\""nameplateInformation\"":{\""manufacturer\"":\""5\"",\""serialNumber\"":\""3\"",\""meterMultiplier\"":\""13\"",\""voltageRating\"":\""4\"",\""phase\"":\""2\"",\""khPrimary\"":\""3\"",\""testAmpsTa\"":\""32\"",\""type\"":\""6\"",\""catalogNumber\"":\""4\"",\""class\"":\""4\"",\""amperes\"":\""5\"",\""wire\"":\""5\"",\""khSecondary\"":\""4\"",\""interval\"":\""3\"",\""multiplier\"":\""31\"",\""rsShaftReductionRatio\"":\""5\"",\""rrResisterRatio\"":\""3\"",\""cTRatio\"":\""3\"",\""pTRatio\"":\""2\""},\""pleaseSelectTests\"":{\""testCalibrationDataImport\"":true,\""testCalibrationDataExport\"":true,\""registerReadings\"":true},\""testCalibrationDataImport\"":{\""asFoundFullLoad\"":35,\""asLeftFullLoad\"":335,\""asLeftLightLoad\"":4,\""asFoundLightLoad\"":3,\""asFoundPowerFactor\"":3,\""asLeftPowerFactor\"":3},\""testCalibrationDataExport\"":{\""asFoundFullLoad\"":3,\""asLeftFullLoad\"":3,\""asFoundLightLoad\"":4,\""asLeftLightLoad\"":4,\""asFoundPowerFactor\"":34,\""asLeftPowerFactor\"":34},\""registerReadings\"":{\""asFoundKWHR\"":43,\""asLeftKWHR\"":43,\""asFoundDemand\"":41,\""asLeftDemand\"":41},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The watt-hour meter PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""7\"",\""testedBy\"":\""4\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""87\"",\""name\"":\""5\"",\""serialNumber\"":\""7\"",\""calibrationDate\"":\""2023-10-04T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""e00f9fe7-7b50-4cc1-94b5-b57aa42694e8"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""2\"",\""humidity\"":\""3\"",\""identification\"":\""21\"",\""parent\"":\""1\"",\""assetId\"":\""2\"",\""building\"":\""1\"",\""floor\"":\""3\"",\""room\"":\""3\"",\""section\"":\""2\""},\""pleaseSelectTests\"":{\""tripUnitInformation\"":true,\""tripUnitSettings\"":true,\""visualInspection\"":true,\""insulationResistancePoleToPole\"":true,\""insulationResistanceAcrossPole\"":true,\""contactResistanceTest\"":true,\""longtimeElements\"":true,\""instantaneousElements1\"":true},\""nameplateInformation\"":{\""manufacturer\"":\""3\"",\""model\"":\""3\"",\""catalogNumber\"":\""2\"",\""frameAmpereRating\"":\""2\"",\""trippingVoltage\"":\""2\"",\""closingVoltage\"":\""3\"",\""type\"":\""4\"",\""serialNumber\"":\""5\"",\""voltageRating\"":\""4\"",\""interruptingKaRating\"":\""3\"",\""chargingVoltage\"":\""4\"",\""shuntTripVoltageRating\"":\""4\""},\""tripUnitInformation\"":{\""tripManufacturer\"":\""2\"",\""tripCatalogNumber\"":\""3\"",\""tripSensorCtAmpereRating\"":\""3\"",\""tripModel\"":\""1\"",\""tripPlugAmpereRating\"":\""2\"",\""tripModuleAmpereRating\"":\""3\""},\""tripUnitSettings\"":{\""longtimePickUpRanges\"":\""3\"",\""instantaneousPickUpRanges\"":\""4\"",\""longtimePickUpAsFound\"":3,\""longtimePickUpAsTested\"":2,\""longtimePickUpAsLeft\"":3,\""instantaneousPickUpAsFound\"":6,\""instantaneousPickUpAsLeft\"":4,\""instantaneousPickUpAsTested\"":1},\""visualInspection\"":{\""circuitBreaker\"":\""Normal\"",\""mainContacts\"":\""Normal\"",\""contactSequence\"":\""Normal\"",\""arcingContacts\"":\""Normal\"",\""electricalConnections\"":\""N\/A\"",\""auxiliaryContacts\"":\""N\/A\"",\""auxiliaryDevices\"":\""See Comments\"",\""shuntTripOperation\"":\""N\/A\""},\""insulationResistancePoleToPole\"":{\""testCurrent\"":{\""ac\"":false,\""dc\"":true},\""p1AsFound\"":\""1\"",\""p1AsLeft\"":\""1\"",\""p2AsFound\"":\""12\"",\""p2AsLeft\"":\""12\"",\""p3AsFound\"":\""14\"",\""p3AsLeft\"":\""14\"",\""testVoltage\"":12},\""insulationResistanceAcrossPole\"":{\""testCurrent\"":{\""ac\"":false,\""dc\"":true},\""p1AsFound\"":\""2\"",\""p1AsLeft\"":\""2\"",\""p2AsFound\"":\""24\"",\""p2AsLeft\"":\""24\"",\""p3AsFound\"":\""2\"",\""p3AsLeft\"":\""2\"",\""testVoltage\"":12},\""contactResistanceTest\"":{\""p1AsFound\"":3,\""p1AsLeft\"":3,\""p2AsFound\"":5,\""p2AsLeft\"":5,\""p3AsFound\"":1,\""p3AsLeft\"":1},\""longtimeElements\"":{\""percentPickUp\"":\""300\"",\""equalToAmps\"":200,\""curveMin\"":1,\""curveMax\"":2,\""pole1\"":2,\""pole2\"":42,\""pole3\"":2,\""pole1Left\"":2,\""pole2Left\"":42,\""pole3Left\"":2},\""instantaneousElements1\"":{\""curveMinIe\"":1,\""curveMaxIe\"":3,\""pole1\"":1,\""pole2\"":1,\""pole3\"":1,\""pole1Left\"":1,\""pole2Left\"":1,\""pole3Left\"":1},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The circuit breaker PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""34\"",\""testedBy\"":\""3\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""1\"",\""name\"":\""3\"",\""serialNumber\"":\""3\"",\""calibrationDate\"":\""2023-10-04T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""d3a30667-d6f7-4053-b2a1-c1fba3af74ce"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-10T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""21\"",\""humidity\"":\""1\"",\""identification\"":\""12\"",\""parent\"":\""3\"",\""assetId\"":\""3\"",\""building\"":\""2\"",\""floor\"":\""3\"",\""room\"":\""3\"",\""section\"":\""1\"",\""workOrderType\"":\""Acceptance Test\""},\""nameplateInformation\"":{\""nameplateManufacturer\"":\""1\"",\""nameplateModel\"":\""1\"",\""nameplatePickUpRange\"":\""56\"",\""controlVoltageRating\"":\""7\"",\""nameplateType\"":\""3\"",\""nameplateCatalogNumber\"":\""1\"",\""nameplateTripTimeRange\"":\""5\"",\""nameplateCurve\"":\""7\"",\""overcurrentDevicemanufacturer\"":\""1\"",\""overcurrentDeviceModel\"":\""6\"",\""overcurrentDeviceCatalogNumber\"":\""7\"",\""overcurrentDeviceVoltageRating\"":\""8\"",\""overcurrentDevice\"":\""3\"",\""overcurrentDeviceType\"":\""5\"",\""overcurrentDeviceAmpereRating\"":\""5\"",\""overcurrentDeviceShuntTripVoltage\"":\""4\""},\""pleaseSelectTests\"":{\""visualInspection\"":true,\""groundFaultRelaySettings\"":true,\""electricalTests\"":true,\""pickupTests\"":true,\""timingTests\"":true},\""visualInspection\"":{\""groundFaultSensingType\"":\""Residual\"",\""neutralToGroundLocation\"":\""Correct\"",\""currentTransformerLocation\"":\""Correct\"",\""testPanel\"":\""Operational\""},\""groundFaultRelaySettings\"":{\""asFoundinAmperes\"":\""2\"",\""asLeftInAmperes\"":\""23\"",\""asFoundInSeconds\"":\""2\"",\""asLeftInSeconds\"":\""4\"",\""bElementB\"":5},\""electricalTests\"":{\""asFoundInOhms\"":\""2\"",\""asLeftInOhms\"":\""24\"",\""asFoundInAmps\"":\""2\"",\""asLeftInAmps\"":\""2\""},\""pickupTests\"":{\""percentPickUp\"":300,\""curveMin\"":12,\""curveMax\"":4,\""equalToAmps\"":2,\""asFound\"":1,\""asLeft\"":3},\""timingTests\"":{\""percentPickUp\"":150,\""curveMin\"":3,\""curveMax\"":2,\""equalToAmps\"":4,\""asFound\"":2,\""asLeft\"":4},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The asset PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""3\"",\""testedBy\"":\""21\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""2\"",\""name\"":\""4\"",\""serialNumber\"":\""2\"",\""calibrationDate\"":\""2023-10-04T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""b309299b-dee3-45b8-9084-dcde39542d0b"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-04T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""4\"",\""humidity\"":\""5\"",\""identification\"":\""2\"",\""parent\"":\""3\"",\""assetId\"":\""5\"",\""building\"":\""1\"",\""floor\"":\""3\"",\""room\"":\""3\"",\""section\"":\""21\""},\""nameplateInformation\"":{\""manufacturer\"":\""3\"",\""serialNumber\"":\""45\"",\""kVRating\"":\""4\"",\""ampereRating\"":\""5\""},\""pleaseSelectTests\"":{\""visualInspection\"":true,\""insulationResistance\"":true,\""leakageCurrent\"":true},\""visualInspection\"":{\""mechanicalDamage\"":\""Acceptable\"",\""paintCondition\"":\""Acceptable\"",\""cleanliness\"":\""Acceptable\"",\""properlyGrounded\"":\""Acceptable\"",\""systemConfiguration\"":\""3PH\/4W\"",\""electricalDamage\"":\""Danger\"",\""coversAndDoors\"":\""Acceptable\"",\""electricalConnections\"":\""Alert\"",\""systemVoltage\"":12},\""insulationResistance\"":{\""testVoltage\"":{\""ac\"":true,\""dc\"":false},\""phaseAResistance1\"":\""3\"",\""phaseBResistance1\"":\""2\"",\""phaseCResistance1\"":\""4\"",\""phaseAResistance2\"":\""5\"",\""phaseBResistance2\"":\""2\"",\""phaseCResistance2\"":\""4\"",\""phaseAResistance3\"":\""5\"",\""phaseBResistance3\"":\""4\"",\""phaseCResistance3\"":\""3\"",\""phaseAResistance4\"":\""3\"",\""phaseBResistance4\"":\""5\"",\""phaseCResistance4\"":\""5\"",\""phaseATestVoltage\"":3,\""phaseBTestVoltage\"":2,\""phaseCTestVoltage\"":1},\""leakageCurrent\"":{\""testVoltage\"":{\""ac\"":true,\""dc\"":false},\""phaseALeakage\"":\""3\"",\""phaseBLeakage\"":\""5\"",\""phaseCLeakage\"":\""6\"",\""phaseBTestVoltage\"":4,\""phaseATestVoltage\"":3,\""phaseCTestVoltage\"":5},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The medium-voltage bus PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""2\"",\""testedBy\"":\""54\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""4\"",\""name\"":\""23\"",\""serialNumber\"":\""4\"",\""calibrationDate\"":\""2023-10-04T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""3bd6a1d9-c8ca-4f5d-a1c9-dee7a1ef8a77"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""21\"",\""humidity\"":\""2\"",\""identification\"":\""4\"",\""parent\"":\""5\"",\""assetId\"":\""2\"",\""building\"":\""2\"",\""floor\"":\""4\"",\""room\"":\""4\"",\""section\"":\""21\""},\""nameplateInformation\"":{\""manufacturer\"":\""45\"",\""ampereRating\"":\""4\"",\""modelNumber\"":\""5\"",\""serialNumber\"":\""5\"",\""contactType\"":\""67\"",\""voltageRating\"":\""5\"",\""type\"":\""4\"",\""interruptingRating\"":\""44\"",\""numberOfPoles\"":\""5\""},\""visualInspection\"":{\""atsEncloserExterior\"":\""2\"",\""switch\"":\""New\"",\""Insulators\"":\""Dity\/Cleaned\"",\""cubicle\"":\""Dirty\"",\""connections\"":\""Tight\"",\""normalContacts\"":\""OK\"",\""arcingContacts\"":\""N\/A\"",\""mechanism\"":\""87\"",\""voltMeter\"":\""7\"",\""heaters\"":\""None\"",\""auxiliaryDevices\"":\""N\/A\"",\""panelLights\"":\""4\"",\""switchGrounded\"":\""No\"",\""contactSequence\"":\""Normal\"",\""shuntTrip\"":\""See Comments\"",\""shuntTripVoltage\"":\""N\/A\"",\""ampMeter\"":\""6\""},\""pleaseSelectTests\"":{\""timerSettings\"":true,\""infraredSurvey\"":true,\""voltageDrop\"":true,\""insulationContactResistanceTestData\"":true},\""timerSettings\"":{\""gsSettings\"":\""2\"",\""gsTd\"":\""3\"",\""n-eSettings\"":\""5\"",\""n-eTd\"":\""4\"",\""e-nSettings\"":\""4\"",\""e-nTd1\"":\""1\"",\""dttsN-eSettings\"":\""6\"",\""dttsN-eTd\"":\""7\"",\""dttsN-nSettings\"":\""6\"",\""dttsN-nTd\"":\""7\"",\""gcSettings\"":\""7\"",\""gcTd\"":\""5\""},\""voltageDrop\"":{\""normalAsFound1\"":\""2\"",\""normalAsLeft1\"":\""2\"",\""normalAsFound2\"":\""4\"",\""normalAsLeft2\"":\""4\"",\""normalAsFound3\"":\""6\"",\""normalAsLeft3\"":\""6\"",\""emergencyAsFound1\"":\""5\"",\""emergencyAsLeft1\"":\""5\"",\""emergencyAsFound2\"":\""6\"",\""emergencyAsLeft2\"":\""6\"",\""emergencyAsFound3\"":\""7\"",\""emergencyAsLeft3\"":\""7\"",\""bypassNormalAsFound1\"":\""66\"",\""bypassNormalAsLeft1\"":\""66\"",\""bypassNormalAsFound2\"":\""4\"",\""bypassNormalAsLeft2\"":\""4\"",\""bypassNormalAsFound3\"":\""6\"",\""bypassNormalAsLeft3\"":\""6\"",\""bypassEmergencyAsFound1\"":\""9\"",\""bypassEmergencyAsLeft1\"":\""9\"",\""bypassEmergencyAsFound2\"":\""1\"",\""bypassEmergencyAsLeft2\"":\""1\"",\""bypassEmergencyAsFound3\"":\""99\"",\""bypassEmergencyAsLeft3\"":\""99\""},\""testData\"":{\""normalAsFound5\"":\""1\"",\""normalAsLeft5\"":\""1\"",\""normalAsFound6\"":\""3\"",\""normalAsLeft6\"":\""3\"",\""normalAsFound7\"":\""4\"",\""normalAsLeft7\"":\""4\"",\""normalAsFound8\"":\""5\"",\""normalAsLeft8\"":\""5\"",\""emergencyAsFound5\"":\""5\"",\""emergencyAsLeft5\"":\""5\"",\""emergencyAsFound6\"":\""2\"",\""emergencyAsLeft6\"":\""2\"",\""emergencyAsFound7\"":\""3\"",\""emergencyAsLeft7\"":\""3\"",\""emergencyAsFound8\"":\""3\"",\""emergencyAsLeft8\"":\""3\"",\""bypassNormalAsFound5\"":\""2\"",\""bypassNormalAsLeft5\"":\""2\"",\""bypassNormalAsFound6\"":\""1\"",\""bypassNormalAsLeft6\"":\""1\"",\""bypassNormalAsFound7\"":\""6\"",\""bypassNormalAsLeft7\"":\""6\"",\""bypassNormalAsFound8\"":\""7\"",\""bypassNormalAsLeft8\"":\""7\"",\""bypassEmergencyAsFound5\"":\""5\"",\""bypassEmergencyAsLeft5\"":\""5\"",\""bypassEmergencyAsFound6\"":\""4\"",\""bypassEmergencyAsLeft6\"":\""4\"",\""bypassEmergencyAsFound7\"":\""3\"",\""bypassEmergencyAsLeft7\"":\""3\"",\""bypassEmergencyAsFound8\"":\""1\"",\""bypassEmergencyAsLeft8\"":\""1\"",\""normalAsFound9\"":\""3\"",\""normalAsLeft9\"":\""3\"",\""normalAsFound10\"":\""6\"",\""normalAsLeft10\"":\""6\"",\""normalAsFound11\"":\""8\"",\""normalAsLeft11\"":\""8\"",\""normalAsFound12\"":\""2\"",\""normalAsLeft12\"":\""2\"",\""emergencyAsFound9\"":\""2\"",\""emergencyAsLeft9\"":\""2\"",\""emergencyAsFound10\"":\""3\"",\""emergencyAsLeft10\"":\""3\"",\""emergencyAsFound11\"":\""3\"",\""emergencyAsLeft11\"":\""3\"",\""emergencyAsFound12\"":\""4\"",\""emergencyAsLeft12\"":\""4\"",\""bypassNormalAsFound9\"":\""2\"",\""bypassNormalAsLeft9\"":\""2\"",\""bypassNormalAsFound10\"":\""455\"",\""bypassNormalAsLeft10\"":\""455\"",\""bypassNormalAsFound11\"":\""3\"",\""bypassNormalAsLeft11\"":\""3\"",\""bypassNormalAsFound12\"":\""3\"",\""bypassNormalAsLeft12\"":\""3\"",\""bypassEmergencyAsFound9\"":\""7\"",\""bypassEmergencyAsLeft9\"":\""7\"",\""bypassEmergencyAsFound10\"":\""8\"",\""bypassEmergencyAsLeft10\"":\""8\"",\""bypassEmergencyAsFound11\"":\""7\"",\""bypassEmergencyAsLeft11\"":\""7\"",\""bypassEmergencyAsFound12\"":\""35\"",\""bypassEmergencyAsLeft12\"":\""35\"",\""testCondition1\"":12,\""testCondition2\"":23,\""normalAsFound13\"":12,\""normalAsLeft13\"":12,\""normalAsFound14\"":3,\""normalAsLeft14\"":3,\""normalAsFound15\"":4,\""normalAsLeft15\"":4,\""normalAsFound16\"":5,\""normalAsLeft16\"":5,\""emergencyAsFound16\"":8,\""emergencyAsLeft16\"":8,\""emergencyAsFound15\"":6,\""emergencyAsLeft15\"":6,\""emergencyAsFound14\"":8,\""emergencyAsLeft14\"":8,\""emergencyAsFound13\"":6,\""emergencyAsLeft13\"":6,\""bypassNormalAsFound13\"":1,\""bypassNormalAsLeft13\"":1,\""bypassNormalAsFound15\"":6,\""bypassNormalAsLeft15\"":6,\""bypassNormalAsFound16\"":6,\""bypassNormalAsLeft16\"":6,\""bypassNormalAsFound14\"":5,\""bypassNormalAsLeft14\"":5,\""bypassEmergencyAsFound14\"":34,\""bypassEmergencyAsLeft14\"":34,\""bypassEmergencyAsFound13\"":6,\""bypassEmergencyAsLeft13\"":6,\""bypassEmergencyAsFound15\"":8,\""bypassEmergencyAsLeft15\"":8,\""bypassEmergencyAsFound16\"":8,\""bypassEmergencyAsLeft16\"":8},\""footer\"":{\""testEquipmentNumber\"":\""12\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""The transfer switch PASSED and is acceptable for operation.\"",\""testedBy\"":\""12\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""1\"",\""name\"":\""5\"",\""serialNumber\"":\""7\"",\""calibrationDate\"":\""2023-10-11T00:00:00+05:30\""}],\""copyright\"":{}},\""infraredSurvey\"":{\""atsNormalSource\"":\""1\"",\""atsNormalSourceBypass\"":\""3\"",\""atsEmergencySource\"":\""5\"",\""atsEmergencySourceBypass\"":\""67\""}}}""
                     },
                     {
                      ""FORM ID"": ""8999443f-4b84-41a5-bcf9-4c111d52a23b"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-04T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""76\"",\""humidity\"":\""7\"",\""identification\"":\""5\"",\""parent\"":\""6\"",\""assetId\"":\""4\"",\""building\"":\""64\"",\""floor\"":\""4\"",\""room\"":\""4\"",\""section\"":\""5\"",\""workOrderType\"":\""Acceptance Test\""},\""nameplateInformation\"":{\""manufacturer\"":\""3\"",\""ampereRating\"":\""65\"",\""catalogNumber\"":\""4\"",\""faultWithstandRating\"":\""5\"",\""asymmetricalRating\"":\""7\"",\""voltageRating\"":\""6\"",\""serialNumber\"":\""4\"",\""model\"":\""5\"",\""interruptingRating\"":\""7\"",\""contactType\"":\""5\""},\""pleaseSelectTests\"":{\""visualInspection\"":true,\""operationalTests\"":true,\""insulationResistanceContactsOpen\"":true,\""insulationResistanceContactsClosed\"":true,\""leakageInMilliamps\"":true,\""contactResistanceTest\"":true,\""topFuseClipResistance\"":true,\""bottomFuseClipResistance\"":true,\""fuseResistanceInMicroOhms\"":true},\""visualInspection\"":{\""mainContacts\"":\""Normal\"",\""arcingContacts\"":\""Dirty \/ Cleaned\"",\""contactSequence\"":\""Normal\"",\""cubicleAndInsulatingMembers\"":\""Normal\"",\""damagedInsulators\"":\""None\"",\""electricalConnections\"":\""See Comments\"",\""fuseType\"":\""43\"",\""spareFusesPresent\"":\""Yes\"",\""groundingProvisions\"":\""No\"",\""operatingMechanism\"":\""See Comments\"",\""switchGrounded\"":\""Yes\"",\""systemVoltage\"":\""33\"",\""panelLights\"":\""Alert\"",\""heaters\"":\""See Comments\"",\""shuntTripVoltage\"":\""48VDc\"",\""fuseSizes\"":\""22\"",\""fuseRefillNumber\"":\""3\"",\""contactWearIndicators\"":\""4\""},\""operationalTests\"":{\""manualOpen\"":\""See Comments\"",\""manualClose\"":\""See Comments\"",\""electricallyOpen\"":\""See Comments\"",\""electricallyClose\"":\""OK\"",\""snufferOperation\"":\""OK\"",\""arcingContactOperation\"":\""See Comments\"",\""fuseCutoutOperation\"":\""OK\"",\""keyInterlockOperation\"":\""OK\"",\""doorInterlockOperation\"":\""N\/A\"",\""lockingMechanism\"":\""See Comments\"",\""auxiliaryDevices\"":\""N\/A\"",\""shuntTrip\"":\""See Comments\""},\""insulationResistanceContactsOpen\"":{\""p1AsFound\"":\""1\"",\""p1AsLeft\"":\""4\"",\""p2AsFound\"":\""3\"",\""p2AsLeft\"":\""4\"",\""p3AsFound\"":\""4\"",\""p3AsLeft\"":\""2\"",\""testVoltage\"":3},\""insulationResistanceContactsClosed\"":{\""p1AsFound\"":\""4\"",\""p1AsLeft\"":\""4\"",\""p2AsFound\"":\""2\"",\""p2AsLeft\"":\""2\"",\""p3AsFound\"":\""4\"",\""p3AsLeft\"":\""66\"",\""testVoltage\"":2},\""leakageInMilliamps\"":{\""pole1\"":\""3\"",\""pole2\"":\""5\"",\""pole3\"":\""6\"",\""leakageInMilliampsAt\"":12},\""contactResistanceTest\"":{\""pole1AsFound\"":3,\""pole1AsLeft\"":3,\""pole2AsFound\"":54,\""pole2AsLeft\"":54,\""pole3AsFound\"":6,\""pole3AsLeft\"":6},\""fuseTests\"":{},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The air switch PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""64\"",\""testedBy\"":\""6\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""34\"",\""name\"":\""5\"",\""serialNumber\"":\""677\"",\""calibrationDate\"":\""2023-10-11T00:00:00+05:30\""}]},\""bottomFuseClipResistance\"":{\""pole1AsFound\"":4,\""pole1AsLeft\"":4,\""pole2AsFound\"":65,\""pole2AsLeft\"":65,\""pole3AsFound\"":43,\""pole3AsLeft\"":43},\""topFuseClipResistance\"":{\""pole1AsFound\"":2,\""pole1AsLeft\"":2,\""pole2AsFound\"":3,\""pole2AsLeft\"":3,\""pole3AsFound\"":5,\""pole3AsLeft\"":5},\""fuseResistanceInMicroOhms\"":{\""pole1AsLeft\"":5,\""pole1AsFound\"":6,\""pole3AsFound\"":4,\""pole3AsLeft\"":4,\""pole2AsLeft\"":42,\""pole2AsFound\"":45}}}""
                     },
                     {
                      ""FORM ID"": ""173acff6-415b-4d7f-a830-66284f41bb2b"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""2\"",\""humidity\"":\""3\"",\""identification\"":\""12\"",\""parent\"":\""3\"",\""assetId\"":\""3\"",\""building\"":\""2\"",\""floor\"":\""4\"",\""room\"":\""4\"",\""section\"":\""3\""},\""nameplateInformation\"":{\""manufacturer\"":\""5\"",\""style\"":\""3\"",\""cTRatio\"":\""5\"",\""currentRating\"":\""35\"",\""type\"":\""4\"",\""catalogNumber\"":\""5\"",\""scale\"":\""3\""},\""pleaseSelectTests\"":{\""testData\"":true},\""testData\"":{\""calculatedScaleReading\"":3,\""asFound\"":3,\""asLeft\"":3,\""calculatedScaleReading1\"":4,\""asFound1\"":4,\""asLeft1\"":4,\""calculatedScaleReading2\"":2,\""asFound2\"":2,\""asLeft2\"":2,\""calculatedScaleReading3\"":5,\""asFound3\"":5,\""asLeft3\"":5,\""asFound4\"":5,\""asLeft4\"":5,\""asFound5\"":5,\""asLeft5\"":5,\""calculatedScaleReading4\"":5,\""calculatedScaleReading5\"":2},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The ammeter PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""3\"",\""testedBy\"":\""4\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""5\"",\""name\"":\""67\"",\""serialNumber\"":\""7\"",\""calibrationDate\"":\""2023-10-10T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""0c86c8f5-fdab-4854-ac9e-1dddcc83435b"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""4\"",\""humidity\"":\""2\"",\""identification\"":\""3\"",\""parent\"":\""5\"",\""assetId\"":\""2\"",\""building\"":\""4\"",\""floor\"":\""5\"",\""room\"":\""5\"",\""section\"":\""3\""},\""nameplateInformation\"":{\""manufacturer\"":\""4\"",\""voltageRating\"":\""6\"",\""kvarRating\"":\""4\"",\""ufRating\"":\""5\"",\""fluidType\"":\""2\"",\""serialNumber\"":\""6\"",\""type\"":\""6\"",\""modelNumber\"":\""4\"",\""styleNumber\"":\""4\"",\""catalogNumber\"":\""5\"",\""pcbLabeled\"":\""Non PCB\"",\""bil\"":\""4\""},\""pleaseSelectTests\"":{\""visualInspection\"":true,\""phaseToGroundInsulationAndCapcitanceTest\"":true},\""visualInspection\"":{\""primaryConnections\"":\""OK\"",\""properlyGrounded\"":\""No\"",\""fuseType\"":\""45\"",\""spareFusesPresent\"":\""Yes\"",\""bushing\"":\""OK\"",\""fluidLeakage\"":\""None\"",\""fuseStyle\"":\""35\""},\""phaseToGroundInsulationAndCapcitanceTest\"":{\""dischargeResistance1\"":\""4\"",\""capacitance1\"":\""4\"",\""dischargeResistance2\"":\""4\"",\""phaseB_capacitance\"":\""6\"",\""dischargeResistance3\"":\""6\"",\""capacitance3\"":\""1\""},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The surge capacitor PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""3\"",\""testedBy\"":\""5\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""6\"",\""name\"":\""3\"",\""serialNumber\"":\""2\"",\""calibrationDate\"":\""2023-11-09T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""96ce3165-1822-47f4-b7aa-214edbf2f0b5"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-06T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""2\"",\""humidity\"":\""3\"",\""identification\"":\""1\"",\""parent\"":\""3\"",\""assetId\"":\""2\"",\""building\"":\""2\"",\""floor\"":\""3\"",\""room\"":\""4\"",\""section\"":\""1\""},\""nameplateInformation\"":{\""manufacturer\"":\""4\"",\""styleNumber\"":\""4\"",\""kvArRating\"":\""1\"",\""serialNumber\"":\""2\"",\""pclLabeled\"":\""5\"",\""fluidCapacity\"":\""4\"",\""phase\"":\""1\"",\""numberOfBushings\"":\""5\"",\""modelNumber\"":\""3\"",\""catalogNumber\"":\""4\"",\""voltageRating\"":\""4\"",\""fluidType\"":\""4\"",\""pcbContent\"":\""pcbContaminated\"",\""fuseSize\"":\""4\""},\""visualInspection\"":{\""blownFuses\"":\""yes\"",\""bulgingOrRupturedTank\"":\""no\"",\""fuseSize\"":\""3\"",\""fluidLeaks\"":\""none\"",\""bushingCondition\"":\""ok\""},\""bPleaseSelectFormVersionB\"":{\""1\"":false,\""2\"":true},\""phaseToGroundInsulationTest\"":{\""testSetting\"":\""\"",\""insulationResistanceInOhmsAtKVDc\"":\""\""},\""phaseToPhaseInsulationAndCapacitanceTests1\"":{\""aToCDischargeResistorInsulationResistanceInOhmsAt05KVDc\"":\""\"",\""aToCCapacitanceInf\"":\""\"",\""bToADischargeResistorInsulationResistanceInOhmsAt05KVDc\"":\""\"",\""bToACapacitanceInf\"":\""\"",\""cToBDischargeResistorInsulationResistanceInOhmsAt05KVDc\"":\""\"",\""cToBCapacitanceInf\"":\""\""},\""testData\"":{\""dataGrid\"":[{\""identificationSerialNumber\"":{\""identification\"":\""1\"",\""serialNumber\"":\""3\"",\""kVar\"":\""45\""},\""insulationResistanceAt5KVDcInOhms\"":\""1\"",\""insulationResistanceAt5KVDcInOhms1\"":\""3\"",\""measuredCapacitanceInMicroFarads\"":{\""capacitance1\"":4,\""capacitance2\"":2,\""capacitance3\"":4,\""capacitance4\"":2}}]},\""footer\"":{\""testEquipmentNumber\"":\""2\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""The Capacitor PASSED and is acceptable for operation.\"",\""testedBy\"":\""123\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""1\"",\""name\"":\""3\"",\""serialNumber\"":\""1\"",\""calibrationDate\"":\""2023-10-12T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""e0d6aa3e-0ff9-4fc8-bd6b-b8df69b57536"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""1\"",\""humidity\"":\""3\"",\""identification\"":\""13\"",\""parent\"":\""3\"",\""assetId\"":\""4\"",\""building\"":\""1\"",\""floor\"":\""4\"",\""room\"":\""3\"",\""section\"":\""2\"",\""workOrderType\"":\""Acceptance Test\""},\""nameplateInformation\"":{\""manufacturer\"":\""1\"",\""serialNumber\"":\""4\"",\""catalogNumber\"":\""35\"",\""type\"":\""5\"",\""contactType\"":\""3\"",\""shuntTripVoltage\"":\""5\"",\""ampereRating\"":\""4\"",\""interruptingRating\"":\""5\"",\""voltageRating\"":\""6\"",\""mechanismType\"":\""4\"",\""controlVoltage\"":\""3\""},\""pleaseSelectTests\"":{\""visualInspection\"":true,\""operationalTests\"":true,\""insulationResistanceContactsOpen\"":true,\""insulationResistanceContactsClosed\"":true,\""leakageInMilliamps\"":true,\""contactResistanceTest\"":true,\""topFuseClipResistance\"":true,\""bottomFuseClipResistance\"":true,\""fuseResistanceInMicroOhms\"":true},\""visualInspection\"":{\""contactor\"":\""New\"",\""contactWearGap\"":\""See Comments\"",\""contactSequence\"":\""Normal\"",\""mechanism\"":\""Required Lubrication\"",\""shutter\"":\""N\/A\"",\""fuseType\"":\""3\"",\""spareFusesPresent\"":\""4\"",\""auxiliaryContacts\"":\""N\/A\"",\""cubicle\"":\""23\"",\""rackingMechanism\"":\""N\/A\"",\""switchGrounded\"":\""No\"",\""connections\"":\""See Comments\"",\""panelLights\"":\""OK\"",\""heaters\"":\""N\/A\"",\""fuseAmperage\"":\""23\""},\""operationalTests\"":{\""electricalOperation\"":\""N\/A\"",\""manualOperation\"":\""N\/A\""},\""insulationResistanceContactsOpen\"":{\""p1AsFound\"":\""3\"",\""p1AsLeft\"":\""4\"",\""p2AsFound\"":\""3\"",\""p2AsLeft\"":\""2\"",\""p3AsFound\"":\""4\"",\""p3AsLeft\"":\""3\"",\""testVoltage\"":23},\""insulationResistanceContactsClosed\"":{\""p1AsFound\"":\""4\"",\""p1AsLeft\"":\""4\"",\""p2AsFound\"":\""2\"",\""p2AsLeft\"":\""3\"",\""p3AsFound\"":\""1\"",\""p3AsLeft\"":\""3\"",\""testVoltage\"":2},\""leakageInMilliamps\"":{\""pole1\"":\""4\"",\""pole2\"":\""4\"",\""pole3\"":\""11\"",\""leakageInMilliampsAt\"":2},\""contactResistanceTest\"":{\""pole1AsFound\"":3,\""pole1AsLeft\"":3,\""pole2AsFound\"":1,\""pole2AsLeft\"":1,\""pole3AsFound\"":1,\""pole3AsLeft\"":13},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The contactor PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""3\"",\""testedBy\"":\""2\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""2\"",\""name\"":\""4\"",\""serialNumber\"":\""2\"",\""calibrationDate\"":\""2023-10-10T00:00:00+05:30\""}]},\""topFuseClipResistance\"":{\""pole1AsFound\"":3,\""pole1AsLeft\"":3,\""pole2AsFound\"":1,\""pole2AsLeft\"":1,\""pole3AsFound\"":1,\""pole3AsLeft\"":13},\""bottomFuseClipResistance\"":{\""pole1AsFound\"":3,\""pole1AsLeft\"":3,\""pole2AsFound\"":4,\""pole2AsLeft\"":4,\""pole3AsFound\"":2,\""pole3AsLeft\"":2},\""fuseResistanceInMicroOhms\"":{\""pole2AsFound\"":4,\""pole2AsLeft\"":4,\""pole3AsFound\"":2,\""pole3AsLeft\"":2,\""pole1AsFound\"":4,\""pole1AsLeft\"":4}}}""
                     },
                     {
                      ""FORM ID"": ""b136635a-794f-4210-97e9-6246ae6a4a96"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-05T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""23\"",\""humidity\"":\""4\"",\""identification\"":\""1\"",\""parent\"":\""3\"",\""assetId\"":\""4\"",\""building\"":\""2\"",\""floor\"":\""4\"",\""room\"":\""4\"",\""section\"":\""2\""},\""nameplateInformation\"":{\""manufacturer\"":\""3\"",\""ratio\"":\""2\"",\""bil\"":\""2\"",\""insulationLevel\"":\""3\"",\""acccuracyClass\"":\""4\"",\""type\"":\""4\"",\""catalogNumber\"":\""34\"",\""frequency\"":\""4\"",\""ratingFactor\"":\""2\"",\""relayingClass\"":\""1\""},\""pleaseSelectTests\"":{\""visualInspection\"":true,\""ratioAndPolarityTests\"":true,\""saturationTests\"":true,\""insulationResistance\"":true},\""visualInspection\"":{\""primaryConnection\"":\""Loose\"",\""properlyGrounded\"":\""Yes\"",\""shortingDevices\"":\""332\"",\""secondaryConnection\"":\""Tight\"",\""bushingInsulation\"":\""OK\""},\""ratioAndPolarityTests\"":{\""pole1SerialNumber\"":\""4\"",\""pole2SerialNumber\"":\""3\"",\""pole3SerialNumber\"":\""4\"",\""pole1CalculatedRatio\"":\""2\"",\""pole2CalculatedRatio\"":\""4\"",\""pole3CalculatedRatio\"":\""2\"",\""pole1MeasuredRatio\"":\""54\"",\""pole2MeasuredRatio\"":\""34\"",\""pole3MeasuredRatio\"":\""2\"",\""p1Polarity\"":\""3\"",\""p2Polarity\"":\""2\"",\""p3Polarity\"":\""1\""},\""saturationTests\"":{\""currentFlow\"":\""3\"",\""currentFlow1Pole1\"":\""3\"",\""currentFlow1Pole2\"":\""2\"",\""currentFlow1Pole3\"":\""2\"",\""currentFlow1\"":\""2\"",\""currentFlow2Pole1\"":\""2\"",\""currentFlow2Pole2\"":\""2\"",\""currentFlow2Pole3\"":\""4\"",\""currentFlow2\"":\""3\"",\""currentFlow3Pole1\"":\""2\"",\""currentFlow3Pole2\"":\""67\"",\""currentFlow3Pole3\"":\""7\"",\""currentFlow3\"":\""5\"",\""currentFlow4Pole1\"":\""3\"",\""currentFlow4Pole2\"":\""2\"",\""currentFlow4Pole3\"":\""5\"",\""currentFlow4\"":\""3\"",\""currentFlow5Pole1\"":\""6\"",\""currentFlow5Pole2\"":\""5\"",\""currentFlow5Pole3\"":\""6\"",\""currentFlow5\"":\""8\"",\""currentFlow6Pole1\"":\""6\"",\""currentFlow6Pole2\"":\""4\"",\""currentFlow6Pole3\"":\""2\"",\""currentFlow6\"":\""5\"",\""currentFlow7Pole1\"":\""3\"",\""currentFlow7Pole2\"":\""4\"",\""currentFlow7Pole3\"":\""5\""},\""insulationResistance\"":{\""primaryToGround\"":\""3\"",\""primaryToGroundPole1\"":\""6\"",\""primaryToGroundPole2\"":\""3\"",\""primaryToGroundPole3\"":\""2\"",\""primaryToSecondary\"":\""5\"",\""primaryToSecondaryPole1\"":\""3\"",\""primaryToSecondaryPole2\"":\""4\"",\""primaryToSecondaryPole3\"":\""2\"",\""secondaryToGround\"":\""7\"",\""secondaryToGroundPole1\"":\""0\"",\""secondaryToGroundPole2\"":\""87\"",\""secondaryToGroundPole3\"":\""1\""},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The transformer PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""43\"",\""testedBy\"":\""43\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""4\"",\""name\"":\""5\"",\""serialNumber\"":\""7\"",\""calibrationDate\"":\""2023-10-12T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""579e9fee-a9ff-4deb-8956-736eb674b730"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""3\"",\""humidity\"":\""2\"",\""identification\"":\""3\"",\""parent\"":\""2\"",\""assetId\"":\""4\"",\""building\"":\""1\"",\""floor\"":\""43\"",\""room\"":\""3\"",\""section\"":\""3\""},\""nameplateInformation\"":{\""manufacturer\"":\""4\"",\""serialNumber\"":\""1\"",\""bil\"":\""3\"",\""modelNumber\"":\""3\"",\""impedance\"":\""42\"",\""mainTankType\"":\""3\"",\""pcbLabeled\"":\""2\"",\""primaryVector\"":\""3\"",\""primaryVoltage\"":\""3\"",\""primaryAmperage\"":\""2\"",\""kVa\"":\""3\"",\""phase\"":\""3\"",\""class\"":\""1\"",\""temperatureRise\"":\""4\"",\""typeOfCoolant\"":\""MineralOil\"",\""capacityInGallons\"":\""2\"",\""pcbContentInPpm\"":\""3\"",\""secondaryVector\"":\""4\"",\""secondaryVoltage\"":\""2\"",\""secondaryAmperage\"":\""3\""},\""pleaseSelectTests\"":{\""tapChanger\"":true,\""gaugesReadingAndSettings\"":true,\""visualInspection\"":true,\""windingInsulationTests\"":true,\""excitingCurrentM4000\"":true,\""primaryBushingInformation\"":true,\""secondaryBushingInformation\"":true,\""bushingTests\"":true,\""primaryWindingTest\"":true,\""secondaryWindingTest\"":true,\""transformerTurnsRatioTests\"":true,\""transformerDcInsulationResistance\"":true},\""tapChanger\"":{\""sampleType\"":\""dy1\"",\""tapPosition1\"":\""A\"",\""tapPosition2\"":\""B\"",\""tapPosition3\"":\""C\"",\""tapPosition4\"":\""D\"",\""tapPosition5\"":\""E\"",\""tapPosition6\"":\""F\"",\""tapPosition7\"":\""G\"",\""tapPositionAsFound\"":\""4\"",\""tapPositionAsLeft\"":\""2\"",\""voltage1\"":2,\""voltage2\"":3,\""voltage3\"":4,\""voltage4\"":3,\""voltage5\"":34,\""voltage6\"":4,\""voltage7\"":4},\""gaugesReadingAndSettings\"":{\""pressureAsFound\"":\""4\"",\""pressureAsLeft\"":\""3\"",\""resetMaxTemperature\"":\""No\"",\""oilLevel\"":\""Yes\"",\""gaugeQuestion\"":\""Yes\"",\""oilTemperature\"":3,\""maximumOilTemp\"":4,\""alarmSetting\"":3,\""fanSetting\"":4},\""visualInspection\"":{\""damagedBushing\"":\""No\"",\""leakingTapChanger\"":\""Yes\"",\""leakingCoolingFins\"":\""No\"",\""fanOperation\"":\""Ok\"",\""electricalConnection\"":\""Tight\"",\""leakingBushings\"":\""N\/A\"",\""leakingGuages\"":\""Yes\"",\""damagedCoolingFins\"":\""N\/A\"",\""fanControlOperation\"":\""OK\"",\""livePartClearances\"":\""OK\"",\""bushingOilLevel\"":\""N\/A\"",\""damagedGuages\"":\""Yes\"",\""leakingCovers\"":\""No\"",\""properlyGrounded\"":\""No\"",\""syringeNumber\"":\""5\"",\""transformerSuddenPressureIndicator\"":\""Normal\""},\""windingInsulationTests\"":{\""powerFactorAtTwenty2\"":\""3\"",\""powerFactorAtTwenty3\"":\""89\"",\""powerFactorAtTwenty5\"":\""8\"",\""powerFactorAtTwenty6\"":\""8\"",\""oilTemperature\"":3,\""testKv1\"":2,\""ma1\"":3,\""watts1\"":3,\""powerFactor2\"":4,\""ma2\"":5,\""testKv2\"":1,\""watts2\"":3,\""testKv3\"":1,\""ma3\"":4,\""watts3\"":56,\""powerFactor3\"":7,\""watts4\"":8,\""ma4\"":67,\""testKv4\"":5,\""testKv5\"":7,\""ma5\"":8,\""watts5\"":9,\""powerFactor5\"":9,\""powerFactor6\"":9,\""ma6\"":7,\""watts6\"":8,\""testKv6\"":7},\""excitingCurrentM4000\"":{\""tapPositionEc\"":\""c\"",\""testKvEc\"":3,\""condition2resultsEc\"":54,\""condition1resultsEc\"":5,\""condition3resultsEc\"":6},\""primaryBushingInformation\"":{\""manufacturer\"":\""3\"",\""catalogNumber\"":\""3\"",\""powerFactor\"":\""3\"",\""style\"":\""4\"",\""type\"":\""34\"",\""faradRating\"":\""5\"",\""model\"":\""5\"",\""ampereRating\"":\""5\"",\""kvRating\"":\""5\""},\""secondaryBushingInformation\"":{\""manufacturer\"":\""5\"",\""catalogNumber\"":\""6\"",\""powerFactor\"":\""6\"",\""style\"":\""5\"",\""type\"":\""4\"",\""faradRating\"":\""6\"",\""model\"":\""6\"",\""ampereRating\"":\""4\"",\""kvRating\"":\""55\""},\""bushingTests\"":{\""testMethod1\"":\""7\"",\""testMethod2\"":\""2\"",\""testMethod3\"":\""7\"",\""testMethod4\"":\""25\"",\""testMethod5\"":\""5\"",\""testMethod6\"":\""1\"",\""testMethod7\"":\""8\"",\""testMethod8\"":\""2\"",\""testKv1\"":7,\""ma1\"":8,\""watts1\"":4,\""powerFactor1\"":5,\""powerFactorAtTwenty1\"":2,\""testKv2\"":8,\""ma2\"":4,\""watts2\"":5,\""powerFactor2\"":5,\""powerFactorAtTwenty2\"":58,\""testKv3\"":1,\""ma3\"":4,\""watts3\"":45,\""powerFactor3\"":25,\""powerFactorAtTwenty3\"":5,\""testKv4\"":4,\""ma4\"":5,\""watts4\"":25,\""powerFactor4\"":7,\""powerFactorAtTwenty4\"":8,\""ma5\"":7,\""watts5\"":4,\""powerFactor5\"":74,\""powerFactorAtTwenty5\"":5,\""testKv5\"":758,\""testKv6\"":7,\""ma6\"":25,\""watts6\"":5,\""powerFactor6\"":7,\""powerFactorAtTwenty6\"":5,\""ma7\"":12,\""watts7\"":5,\""powerFactor7\"":98,\""powerFactorAtTwenty7\"":7,\""testKv8\"":9,\""ma8\"":6,\""watts8\"":3,\""powerFactor8\"":2,\""powerFactorAtTwenty8\"":1,\""testKv7\"":2},\""primaryWindingTest\"":{\""windingResistanceReadings\"":\""MilliOhms\"",\""dataGrid\"":[{\""tap\"":\""3\"",\""h1ToH3\"":3,\""h2ToH1\"":4,\""h3ToH2\"":3}]},\""secondaryWindingTest\"":{\""windingResistanceReadings\"":\""Ohms\"",\""dataGrid\"":[{\""x1ToX0\"":4,\""x2ToX0\"":5,\""x3ToX0\"":7}],\""testEquipment\"":\""3\""},\""transformerTurnsRatioTests\"":{\""dataGrid\"":[{\""tap\"":\""3\"",\""calculated\"":4,\""h2H1ToX2X0\"":6,\""h3H2ToX3X0\"":7,\""h1H3ToX1X0\"":5}],\""testEquipment\"":\""23\""},\""transformerDcInsulationResistance\"":{\""dataGrid\"":[{\""insulationResistance1\"":\""1\"",\""insulationResistance2\"":\""7\"",\""insulationResistance3\"":\""7\"",\""timeInMinutes\"":0.25}],\""primaryToGroundSecondaryGuarded3\"":\""\"",\""primaryToGroundSecondaryGuarded4\"":\""\"",\""primaryToGroundSecondaryGuarded5\"":\""\"",\""polarization10over1_1\"":\""\"",\""polarization10over1_2\"":\""\"",\""polarization10over1_3\"":\""\"",\""testEquipmentNumber\"":\""1\"",\""primaryToGroundSecondaryGuarded\"":2,\""primaryToGroundSecondaryGuarded2\"":6,\""primaryToGroundSecondaryGuarded1\"":5},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The transformer PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""5\"",\""testedBy\"":\""3\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""6\"",\""name\"":\""8\"",\""serialNumber\"":\""8\"",\""calibrationDate\"":\""2023-10-11T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""e883523e-df75-40c0-858b-f1e3cdc4ad3f"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""2\"",\""humidity\"":\""3\"",\""identification\"":\""2\"",\""parent\"":\""3\"",\""assetId\"":\""2\"",\""building\"":\""1\"",\""floor\"":\""2\"",\""room\"":\""3\"",\""section\"":\""3\""},\""nameplateInformation\"":{\""manufacturer\"":\""\"",\""phase\"":\""\"",\""model\"":\""\"",\""primaryVector\"":\""\"",\""secondaryVector\"":\""\"",\""percentImpedance\"":\""\"",\""kva\"":\""\"",\""coolingType\"":\""\"",\""type\"":\""\"",\""primaryVoltage\"":\""\"",\""secondaryVoltage\"":\""\"",\""temperatureRise\"":\""\"",\""serialNumber\"":\""\"",\""class\"":\""\"",\""bil\"":\""\"",\""primaryAmperes\"":\""\"",\""secondaryAmperes\"":\""\""},\""pleaseSelectTests\"":{\""tapChanger\"":true,\""gaugeReadingsSettings\"":true,\""visualInspection\"":true,\""windingInsulationTestsM4000\"":true,\""excitingCurrentM4000\"":true,\""meu\"":true,\""primaryWindingTest\"":true,\""secondaryWindingTest\"":true,\""transformerTurnsRatioTests\"":true,\""transformerDcInsulationResistance\"":true},\""tapChanger\"":{\""sampleType\"":\""YY0\"",\""tapPosition1\"":\""A\"",\""tapPosition2\"":\""B\"",\""tapPosition3\"":\""C\"",\""tapPosition4\"":\""D\"",\""tapPosition5\"":\""E\"",\""tapPosition6\"":\""F\"",\""tapPosition7\"":\""G\"",\""tapPositionAsFound\"":\""5\"",\""tapPositionAsLeft\"":\""7\"",\""voltage1\"":8,\""voltage2\"":5,\""voltage3\"":4,\""voltage4\"":25,\""voltage5\"":5,\""voltage6\"":7,\""voltage7\"":8},\""gaugeReadingsSettings\"":{\""windingTemperature\"":\""24\"",\""maximumTemperature\"":\""2\"",\""fanTemperatureSetting\"":\""3\"",\""alarmTemperatureSetting\"":\""3\"",\""tripTemperatureSetting\"":\""\"",\""fanSwitchPosition\"":\""4\"",\""maxTemperatureIndicatorReset\"":\""No\""},\""visualInspection\"":{\""mechanicalDamage\"":\""See Comments\"",\""fanControlOperation\"":\""N\/A\"",\""fanOperation\"":\""OK\"",\""coronaDeposits\"":\""OK\"",\""damagedInsulators\"":\""N\/A\"",\""insulatingBarriers\"":\""OK\"",\""electricalConnections\"":\""OK\"",\""enclosurePaintConditions\"":\""OK\"",\""electricalDamage\"":\""See Comments\"",\""windingBlockingAndSupport\"":\""OK\"",\""resiliencyBoltsLooseOrRemoved\"":\""N\/A\"",\""coreProperlyGrounded\"":\""NO\"",\""enclosureProperlyGrounded\"":\""NO\"",\""coreAndOilCleanliness\"":\""See Comments\"",\""enclosureCleanliness\"":\""OK\""},\""windingInsulationTestsM4000\"":{\""testKv1\"":\""3\"",\""ma1\"":\""5\"",\""watts1\"":\""6\"",\""powerFactor1\"":\""\"",\""testKv2\"":\""5\"",\""ma2\"":\""3\"",\""watts2\"":\""5\"",\""powerFactor2\"":\""7\"",\""testKv3\"":\""6\"",\""ma3\"":\""5\"",\""watts3\"":\""67\"",\""powerFactor3\"":\""8\"",\""testKv4\"":\""4\"",\""ma4\"":\""8\"",\""watts4\"":\""8\"",\""powerFactor4\"":\""\"",\""testKv5\"":\""7\"",\""ma5\"":\""5\"",\""watts5\"":\""42\"",\""powerFactor5\"":\""1\"",\""testKv6\"":\""7\"",\""ma6\"":\""8\"",\""watts6\"":\""9\"",\""powerFactor6\"":\""9\"",\""testEquipmentNumber\"":\""44\""},\""excitingCurrentM4000\"":{\""tapPositionEc\"":\""2\"",\""testKvEc\"":\""6\"",\""condition1resultsEc\"":\""4\"",\""condition2resultsEc\"":\""7\"",\""condition3resultsEc\"":\""22\""},\""meu\"":{\""tapPositionMeu\"":\""2\"",\""condition1MvaMeu\"":\""3\"",\""condition1MultMeu\"":\""5\"",\""condition1ResultMeu\"":\""4\"",\""condition2MvaMeu\"":\""5\"",\""condition2MultMeu\"":\""6\"",\""condition2ResultMeu\"":\""6\"",\""condition3MvaMeu\"":\""3\"",\""condition3MultMeu\"":\""5\"",\""condition3ResultMeu\"":\""6\"",\""testKV1\"":2,\""meterReadingsMVa1\"":3,\""meterReadingsMW1\"":4,\""testKV2\"":5,\""meterReadingsMVa2\"":6,\""percentPowerFactor2\"":21,\""meterReadingsMW2\"":1,\""testKV4\"":1,\""meterReadingsMVa4\"":3,\""meterReadingsMW4\"":6,\""meterReadingsMVa5\"":3,\""meterReadingsMW5\"":6,\""percentPowerFactor5\"":6,\""testKV5\"":4,\""testKV3\"":4,\""meterReadingsMVa3\"":6,\""meterReadingsMW3\"":7,\""percentPowerFactor3\"":7,\""testKvMeu\"":4},\""primaryWindingTest\"":{\""windingResistanceReadings\"":\""\"",\""dataGrid\"":[{\""tap\"":\""4\"",\""h1ToH3\"":4,\""h2ToH1\"":5,\""h3ToH2\"":5,\""testEquipmentNumber\"":\""4\""}],\""testEquipmentNumber\"":\""4\""},\""secondaryWindingTest\"":{\""windingResistanceReadings\"":\""Ohms\"",\""dataGrid\"":[{\""x1ToX0\"":3,\""x2ToX0\"":5,\""x3ToX0\"":6}]},\""transformerTurnsRatioTests\"":{\""dataGrid\"":[{\""tap\"":\""5\"",\""calculated\"":6,\""h1H3ToX1X0\"":4,\""h3H2ToX3X0\"":3,\""h2H1ToX2X0\"":4}],\""testEquipment\"":\""45\""},\""transformerDcInsulationResistance\"":{\""primaryToGroundSecondaryGuarded3\"":\""\"",\""primaryToGroundSecondaryGuarded4\"":\""\"",\""primaryToGroundSecondaryGuarded5\"":\""\"",\""polarization10over1_1\"":\""\"",\""polarization10over1_2\"":\""\"",\""polarization10over1_3\"":\""\"",\""dataGrid\"":[{\""timeInMinutes\"":0.25,\""insulationResistance1\"":\""6\"",\""insulationResistance2\"":\""7\"",\""insulationResistance3\"":\""4\""}],\""testEquipmentNumber\"":\""4\"",\""primaryToGroundSecondaryGuarded1\"":4,\""primaryToGroundSecondaryGuarded\"":5,\""primaryToGroundSecondaryGuarded2\"":7},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The transformer PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""4\"",\""testedBy\"":\""6\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""8\"",\""name\"":\""4\"",\""serialNumber\"":\""3\"",\""calibrationDate\"":\""2023-10-18T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""bad9707c-039a-4ade-8df3-6e2eb8058fe6"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""1\"",\""humidity\"":\""2\"",\""identification\"":\""1\"",\""parent\"":\""2\"",\""assetId\"":\""3\"",\""building\"":\""1\"",\""floor\"":\""3\"",\""room\"":\""3\"",\""section\"":\""2\""},\""nameplateInformation\"":{\""manufacturer\"":\""3\"",\""model\"":\""3\"",\""voltageRating\"":\""4\"",\""configuration\"":\""3P\/4W\"",\""type\"":\""4\"",\""catalogNumber\"":\""5\"",\""ampereRating\"":\""34\""},\""pleaseSelectTests\"":{\""visualInspection\"":true,\""breakerCount\"":true,\""testConnection\"":true},\""visualInspection\"":{\""mechanicalDamage\"":\""N\/A\"",\""paintCondition\"":\""OK\"",\""cleanliness\"":\""New\"",\""properlyGrounded\"":\""Yes\"",\""electricalDamage\"":\""None\"",\""coversAndDoors\"":\""OK\"",\""electricalConnections\"":\""OK\"",\""systemVoltage\"":\""23\"",\""note\"":\""Note: The panel should be bonded if it is fed by a separately derived system (transformer). The panel should not be bounded if it is fed from another panel.\""},\""breakerCount\"":{\""dataGrid\"":[{\""numberOfPoles\"":1,\""ampRating\"":\""2\"",\""total\"":3}]},\""testConnection\"":{\""testVoltageN\"":\""2\"",\""aAsFound\"":\""2\"",\""bAsFound\"":\""3\"",\""cAsFound\"":\""3\"",\""nAsFound\"":\""3\"",\""aAsLeft\"":\""2\"",\""bAsLeft\"":\""3\"",\""cAsLeft\"":\""3\"",\""nAsLeft\"":\""3\"",\""testVoltageA\"":2,\""testVoltageB\"":4,\""testVoltageC\"":3},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The panelboard PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""2\"",\""testedBy\"":\""3\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""1\"",\""name\"":\""2\"",\""serialNumber\"":\""2\"",\""calibrationDate\"":\""2023-10-04T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""0f8f71d0-ea94-4e11-8f42-6b3ca422098c"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""12\"",\""humidity\"":\""1\"",\""identification\"":\""1\"",\""parent\"":\""22\"",\""assetId\"":\""1\"",\""building\"":\""2\"",\""floor\"":\""3\"",\""room\"":\""3\"",\""section\"":\""2\""},\""nameplateInformation\"":{\""manufacturer\"":\""1\"",\""length\"":\""4\"",\""size\"":\""3\"",\""insulationType\"":\""XLP\"",\""systemUsed\"":\""3\"",\""thickness\"":\""1\"",\""ratedKV\"":\""3\"",\""operatingKV\"":\""2\"",\""KV\"":\""2\"",\""maximumTestKV\"":\""3\"",\""conductorMaterial\"":\""AL\"",\""externalEquipment\"":\""2\""},\""pleaseSelectTests\"":{\""vlfA\"":true,\""vlfB\"":true,\""vlfC\"":true,\""vlfWithstand\"":true},\""vlfPhaseA\"":{\""durationMin\"":\""7\"",\""durationMin1\"":\""4\"",\""durationMin2\"":\""45\"",\""durationMin3\"":\""96\"",\""bVerdictB\"":\""Pass\"",\""rmsVoltage\"":7,\""tanDeltaMean1\"":1,\""tanDeltaStDev\"":2,\""rmsCurrentMA\"":6,\""capacitanceNF\"":9,\""rmsVoltage1\"":5,\""tanDeltaMean2\"":6,\""tanDeltaStDev1\"":3,\""rmsCurrentMA1\"":2,\""capacitanceNF1\"":1,\""rmsVoltage2\"":7,\""tanDeltaMean3\"":9,\""tanDeltaStDev2\"":6,\""rmsCurrentMA2\"":3,\""capacitanceNF2\"":1,\""rmsVoltage3\"":7,\""tanDeltaMean4\"":9,\""tanDeltaStDev3\"":6,\""rmsCurrentMA3\"":3,\""capacitanceNF3\"":5},\""vlfPhaseB\"":{\""durationMin\"":\""96\"",\""durationMin1\"":\""4\"",\""durationMin2\"":\""5\"",\""durationMin3\"":\""7\"",\""bVerdictB\"":\""Pass\"",\""rmsVoltage\"":7,\""tanDeltaMean1\"":8,\""tanDeltaStDev\"":1,\""rmsCurrentMA\"":2,\""capacitanceNF\"":3,\""rmsVoltage1\"":9,\""tanDeltaMean2\"":7,\""tanDeltaStDev1\"":4,\""rmsCurrentMA1\"":5,\""capacitanceNF1\"":54,\""rmsVoltage2\"":5,\""tanDeltaMean3\"":6,\""tanDeltaStDev2\"":8,\""rmsCurrentMA2\"":7,\""capacitanceNF2\"":9,\""rmsVoltage3\"":41,\""tanDeltaMean4\"":52,\""tanDeltaStDev3\"":58,\""rmsCurrentMA3\"":58,\""capacitanceNF3\"":52},\""vlfPhaseC\"":{\""durationMin\"":\""4\"",\""durationMin1\"":\""5\"",\""durationMin2\"":\""5\"",\""durationMin3\"":\""25\"",\""bVerdictB\"":\""Pass\"",\""rmsVoltage\"":7,\""tanDeltaMean1\"":8,\""tanDeltaStDev\"":5,\""rmsCurrentMA\"":41,\""capacitanceNF\"":5,\""rmsVoltage1\"":5,\""tanDeltaMean2\"":4,\""tanDeltaStDev1\"":58,\""rmsCurrentMA1\"":6,\""capacitanceNF1\"":41,\""rmsVoltage2\"":2,\""tanDeltaMean3\"":4,\""tanDeltaStDev2\"":8,\""rmsCurrentMA2\"":6,\""capacitanceNF2\"":14,\""rmsVoltage3\"":2,\""tanDeltaMean4\"":4,\""tanDeltaStDev3\"":5,\""rmsCurrentMA3\"":4,\""capacitanceNF3\"":52},\""vlfWithstand\"":{\""duration\"":\""6\"",\""duration1\"":\""41\"",\""duration2\"":\""52\"",\""duration3\"":\""5\"",\""bVerdictB\"":\""Pass\"",\""rmsVoltage\"":7,\""leakageCurrent\"":8,\""rmsVoltage1\"":41,\""leakageCurrent1\"":5,\""rmsVoltage2\"":7,\""leakageCurrent2\"":8,\""rmsVoltage3\"":4,\""leakageCurrent3\"":78},\""insulationResistance\"":{\""dataGrid\"":[{\""timeMinutes\"":1,\""voltageKV\"":1,\""aPhase\"":\""3\"",\""bPhase\"":\""4\"",\""cPhase\"":\""5\""}],\""aPhase\"":\""ERROR: missing unit(s)\"",\""aPhase1\"":\""\"",\""aPhase2\"":\""\""},\""shieldContinuity\"":{\""phaseA\"":23,\""phaseB\"":4,\""phaseC\"":5},\""dischargeTime\"":{\""dischargeTimeDownTo\"":5,\""phaseA\"":4,\""phaseB\"":5,\""phaseC\"":5},\""footer\"":{\""testEquipmentNumber\"":\""5\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""The cable PASSES and is acceptable for operation.\"",\""testedBy\"":\""45\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""3\"",\""name\"":\""5\"",\""serialNumber\"":\""5\"",\""calibrationDate\"":\""2023-10-04T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     },
                     {
                      ""FORM ID"": ""2bcd61f4-4190-4494-bae2-33282d23b04f"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-04T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""12\"",\""humidity\"":\""1\"",\""identification\"":\""2\"",\""parent\"":\""32\"",\""assetId\"":\""2\"",\""building\"":\""1\"",\""floor\"":\""3\"",\""room\"":\""3\"",\""section\"":\""1\""},\""nameplateInformation\"":{\""manufacturer\"":\""1\"",\""modelNumber\"":\""3\"",\""serialNumber\"":\""3\"",\""powerFactorRating\"":\""2\"",\""phaseWire\"":\""3PH\/3W\"",\""kWRating\"":\""3\"",\""kVaRating\"":\""1\"",\""horsepower\"":\""2\"",\""voltageRating\"":\""3\"",\""ampereRating\"":\""3\""},\""pleaseSelectTests\"":{\""visualInspection\"":true,\""testConnections\"":true,\""dcInsulationResistanceTest\"":true,\""windingResistanceTests\"":true,\""powerFactorTipUpTest\"":true},\""visualInspection\"":{\""exciter\"":\""Normal\"",\""diodes\"":\""See Comments\"",\""ctMounting\"":\""Normal\"",\""meteringCtsAndWiring\"":\""Normal\"",\""shortingTerminalBlocks\"":\""Normal\"",\""fuseBlocks\"":\""See Comments\"",\""controlPanelLamps\"":\""Normal\"",\""tLeads\"":\""Normal\"",\""terminalBlocks\"":\""Normal\"",\""greaseBearing\"":\""N\/A\"",\""generatorCompartment\"":\""Cleaned\"",\""highVoltageCables\"":\""See Comments\"",\""lowVoltageCables\"":\""Normal\"",\""grounding\"":\""See Comments\""},\""testConnections\"":{\""energized\"":\""23\"",\""guarded\"":\""No\"",\""grounded\"":\""N\/A\"",\""testVoltageKVDc\"":\""23\""},\""dcInsulationResistanceTest\"":{\""dataGrid\"":[{\""timeInMinutes\"":0.25,\""insulationResistance1\"":\""23\"",\""insulationResistance2\"":\""\"",\""insulationResistance3\"":\""\""}],\""primaryToGroundSecondaryGuarded3\"":\""\"",\""polarization10over1_1\"":\""\""},\""windingResistanceTests\"":{\""resistance1\"":2,\""resistance2\"":1,\""resistance3\"":21},\""powerFactorTipUpTest\"":{\""ma1\"":2,\""w1\"":3,\""powerFactor1\"":2,\""powerFactor2\"":3,\""ma2\"":3,\""w2\"":12,\""ma3\"":3,\""w3\"":4,\""powerFactor3\"":5,\""w4\"":4,\""powerFactor4\"":2,\""ma4\"":4,\""ma5\"":8,\""w5\"":6,\""powerFactor5\"":3,\""w6\"":3,\""powerFactor6\"":5,\""ma6\"":5,\""w7\"":6,\""powerFactor7\"":7,\""ma7\"":2,\""ma8\"":4,\""w8\"":56,\""powerFactor8\"":7,\""ma9\"":4,\""w9\"":5,\""powerFactor9\"":6,\""powerFactor10\"":7,\""w10\"":8,\""ma10\"":8,\""ma11\"":3,\""w11\"":5,\""powerFactor11\"":7,\""powerFactor12\"":9,\""ma12\"":3,\""w12\"":5,\""w13\"":5,\""ma13\"":6,\""powerFactor13\"":8,\""ma14\"":4,\""w14\"":6,\""powerFactor14\"":7},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The generator PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""23\"",\""testedBy\"":\""23\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""2\"",\""name\"":\""1\"",\""serialNumber\"":\""3\"",\""calibrationDate\"":\""2023-10-16T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""2cc61c02-7e66-4ba9-b0a5-a7169ff190e3"",
                      ""JSON"": ""{\""data\"":{\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-04T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""2\"",\""humidity\"":\""3\"",\""identification\"":\""13\"",\""parent\"":\""1\"",\""assetId\"":\""3\"",\""building\"":\""1\"",\""floor\"":\""3\"",\""room\"":\""3\"",\""section\"":\""54\""},\""pleaseSelectTests\"":{\""tripUnitInformation\"":true,\""tripUnitSettings\"":true,\""visualInspection\"":true,\""operationalTests\"":true,\""insulationResistancePoleToPole\"":true,\""insulationResistanceAcrossPole\"":true,\""contactResistanceTest\"":true,\""longTimeElements\"":true,\""shortTimeElements\"":true,\""groundFaultElements\"":true,\""instantaneousElements\"":true},\""nameplateInformation\"":{\""manufacturer\"":\""3\"",\""model\"":\""4\"",\""catalogNumber\"":\""36\"",\""frameAmpereRating\"":\""4\"",\""trippingVoltage\"":\""56\"",\""closingVoltage\"":\""44\"",\""type\"":\""2\"",\""serialNumber\"":\""4\"",\""voltageRating\"":\""2\"",\""interruptingKaRating\"":\""5\"",\""chargingVoltage\"":\""4\"",\""shuntTripVoltageRating\"":\""5\""},\""tripUnitInformation\"":{\""tripManufacturer\"":\""7\"",\""tripCatalogNumber\"":\""8\"",\""tripSensorCtAmpereRating\"":\""5\"",\""tripModuleAmpereRating\"":\""6\"",\""tripModel\"":\""31\"",\""tripPlugAmpereRating\"":\""4\""},\""tripUnitSettings\"":{\""longtimePickUpRanges\"":\""7\"",\""longtimeDelayRanges\"":\""5\"",\""longtimeDelayAsFound\"":\""14\"",\""longtimeDelayAsLeft\"":\""52\"",\""longtimeDelayAsTested\"":\""14\"",\""shorttimePickUpRanges\"":\""7\"",\""shorttimeDelayRanges\"":\""4\"",\""shorttimeDelayAsFound\"":\""5\"",\""shorttimeDelayAsLeft\"":\""4\"",\""shorttimeDelayAsTested\"":\""58\"",\""shorttimeI2tRanges\"":\""4\"",\""shorttimeI2tAsFound\"":\""5\"",\""shorttimeI2tAsLeft\"":\""5\"",\""shorttimeI2tAsTested\"":\""5\"",\""groundFaultPickUpRanges\"":\""7\"",\""groundFaultPickUpAsFound\"":\""8\"",\""groundFaultPickUpAsLeft\"":\""96\"",\""groundFaultPickUpAsTested\"":\""4\"",\""groundFaultDelayRanges\"":\""5\"",\""groundFaultDelayAsFound\"":\""4\"",\""groundFaultDelayAsLeft\"":\""56\"",\""groundFaultDelayAsTested\"":\""4\"",\""groundFaultI2tRanges\"":\""5\"",\""groundFaultI2tAsFound\"":\""5\"",\""groundFaultI2tAsLeft\"":\""6\"",\""groundFaultI2tAsTested\"":\""4\"",\""instantaneousPickUpRanges\"":\""6\"",\""instantaneousPickUpAsFound\"":\""4\"",\""instantaneousPickUpAsLeft\"":\""6\"",\""instantaneousPickUpAsTested\"":\""546\"",\""longtimePickUpAsFound\"":8,\""longtimePickUpAsLeft\"":5,\""longtimePickUpAsTested\"":4,\""shorttimePickUpAsFound\"":8,\""shorttimePickUpAsLeft\"":4,\""shorttimePickUpAsTested\"":5},\""visualInspection\"":{\""circuitBreaker\"":\""New\"",\""operatingMechanism\"":\""Normal\"",\""electricalConnections\"":\""Normal\"",\""mainContacts\"":\""New\"",\""arcingContacts\"":\""Dirty \/ Cleaned\"",\""contactSequence\"":\""Needs Repair\"",\""auxiliaryContacts\"":\""Normal\"",\""arcChutes\"":\""Normal\"",\""cubicle\"":\""New\"",\""grounding\"":\""Yes\"",\""auxiliaryDevices\"":\""Normal\"",\""panelLights\"":\""See Comments\"",\""rackingMechanism\"":\""Normal\"",\""shuntTripOperation\"":\""See Comments\""},\""operationalTests\"":{\""manualOpen\"":\""Slow\"",\""electricallyOpen\"":\""Slow\"",\""manualCharge\"":\""Not Working\"",\""tripWithProtectiveDevices\"":\""Normal\"",\""manualClose\"":\""Not Working\"",\""electricallyClose\"":\""Normal\"",\""electricallyCharge\"":\""Not Working\""},\""insulationResistancePoleToPole\"":{\""p1AsFound\"":\""2\"",\""p1AsLeft\"":\""2\"",\""p2AsFound\"":\""2\"",\""p2AsLeft\"":\""2\"",\""p3AsFound\"":\""2\"",\""p3AsLeft\"":\""2\"",\""testVoltage\"":1},\""insulationResistanceAcrossPole\"":{\""p1AsFound\"":\""2\"",\""p1AsLeft\"":\""23\"",\""p2AsLeft\"":\""23\"",\""p2AsFound\"":\""24\"",\""p3AsFound\"":\""21\"",\""p3AsLeft\"":\""22\"",\""testVoltage\"":1},\""contactResistanceTest\"":{\""p1AsFound\"":2,\""p1AsLeft\"":2,\""p2AsFound\"":3,\""p2AsLeft\"":3,\""p3AsLeft\"":1,\""p3AsFound\"":1},\""longtimeElements\"":{\""percentPickUp\"":\""300\"",\""pole1\"":\""3\"",\""pole2\"":\""2\"",\""pole3\"":\""3\"",\""pole1Left\"":\""3\"",\""pole2Left\"":\""2\"",\""pole3Left\"":\""3\"",\""curveMin\"":1,\""curveMax\"":2,\""equalToAmps\"":3},\""shorttimeElements\"":{\""percentPickUp\"":\""150\"",\""pole1\"":\""2\"",\""pole2\"":\""3\"",\""pole3\"":\""2\"",\""pole1Left\"":\""2\"",\""pole2Left\"":\""3\"",\""pole3Left\"":\""2\"",\""curveMin\"":2,\""equalToAmps\"":3,\""curveMax\"":3},\""groundFaultElements1\"":{\""percentPickUp\"":\""150\"",\""pole1\"":\""3\"",\""pole2\"":\""56\"",\""pole3\"":\""5\"",\""pole1Left\"":\""34\"",\""pole2Left\"":\""56\"",\""pole3Left\"":\""5\"",\""curveMax\"":3,\""equalToAmps\"":3,\""curveMin\"":3},\""instantaneousElements1\"":{\""pole1\"":\""4\"",\""pole2\"":\""7\"",\""pole3\"":\""5\"",\""pole1Left\"":\""4\"",\""pole2Left\"":\""7\"",\""pole3Left\"":\""5\"",\""curveMinIe\"":3,\""curveMaxIe\"":4},\""footer\"":{\""selection1\"":\""\"",\""inspectionVerdict\"":\""Pass\"",\""comments\"":\""The circuit breaker PASSED and is acceptable for operation.\"",\""defects\"":\""no\"",\""violations\"":\""no\"",\""testEquipmentNumber\"":\""34\"",\""testedBy\"":\""34\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""3\"",\""name\"":\""4\"",\""serialNumber\"":\""3\"",\""calibrationDate\"":\""2023-10-12T00:00:00+05:30\""}]}}}""
                     },
                     {
                      ""FORM ID"": ""3338b532-02c4-4822-bc81-92c2a4d78749"",
                      ""JSON"": ""{\""data\"":{\""21\"":{\""dataGrid\"":[{\""elementone\"":{\""elementIdentification\"":\""\"",\""pickUp\"":\""\"",\""timeDelay\"":\""\"",\""mfgCurvePickup\"":\""\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveTimeDelay\"":\""\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\""},\""elementNumber\"":{\""textField\"":\""Element #1\"",\""elementIdentification\"":\""23\"",\""pickUp\"":\""\"",\""timeDelay\"":\""\"",\""mfgCurvePickup\"":\""\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveTimeDelay\"":\""\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\"",\""reach\"":\""4\"",\""mta\"":\""64\"",\""delayCyc\"":\""5\"",\""mfgCurveReach\"":\""7\"",\""3phaseReach\"":\""8\"",\""aBPhaseReach\"":\""4\"",\""bCPhaseReach\"":\""5\"",\""aCPhaseReach\"":\""4\"",\""mfgCurveDelay\"":\""5\"",\""3phaseDelay\"":\""2\"",\""aBPhaseDelay\"":\""14\"",\""bCPhaseDelay\"":\""5\"",\""aCPhaseDelay\"":\""25\"",\""mfgCurveMTA\"":\""5\""}}]},\""24\"":{\""elementIdentification\"":\""44\"",\""pickUp\"":\""2\"",\""delayCyc\"":\""4\"",\""mfgCruveReach\"":\""5\"",\""resultReach\"":\""6\"",\""mfgCruveDelay\"":\""6\"",\""resultDelay\"":\""5\""},\""25\"":{\""elementIdentification\"":\""4\"",\""voltageDifference\"":\""4\"",\""underVoltageBlock\"":\""5\"",\""phaseAngle\"":\""6\"",\""overVoltageBlock\"":\""54\"",\""mfgCurveVoltageDifference\"":\""4\"",\""resultVoltageDifference\"":\""5\"",\""mfgCurvePhaseAngle\"":\""622\"",\""resultVoltagePhaseAngle\"":\""6\"",\""mfgCurveUnderVoltageBlock\"":\""4\"",\""resultVoltageUnderVoltageBlock\"":\""6\"",\""mfgCurveOverVoltageBlock\"":\""6\"",\""resultVoltageOverVoltageBlock\"":\""5\""},\""27\"":{\""elementIdentification\"":\""34\"",\""dataGrid\"":[{\""elementone\"":{\""elementIdentification\"":\""\"",\""pickUp\"":\""\"",\""timeDelay\"":\""\"",\""mfgCurvePickup\"":\""\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveTimeDelay\"":\""\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\""},\""elementNumber\"":{\""textField\"":\""Element #1\"",\""elementIdentification\"":\""\"",\""pickUp\"":\""5\"",\""timeDelay\"":\""6\"",\""mfgCurvePickup\"":\""7\"",\""aPhasePickup\"":\""8\"",\""bPhasePickup\"":\""4\"",\""cPhasePickup\"":\""5\"",\""mfgCurveTimeDelay\"":\""4\"",\""aPhaseTimeDelay\"":\""5\"",\""bPhasePickupTimeDelay\"":\""6\"",\""cPhaseTimeDelay\"":\""5\""}}]},\""32\"":{\""elementIdentification\"":\""23\"",\""dataGrid\"":[{\""elementone\"":{\""elementIdentification\"":\""\"",\""pickUp\"":\""\"",\""timeDelay\"":\""\"",\""mfgCurvePickup\"":\""\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveTimeDelay\"":\""\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\""},\""elementNumber\"":{\""textField\"":\""Element #1\"",\""elementIdentification\"":\""\"",\""pickUp\"":\""\"",\""timeDelay\"":\""\"",\""mfgCurvePickup\"":\""2\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveTimeDelay\"":\""\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\"",\""pickup\"":\""23\"",\""unit\"":\""w\"",\""delayCyc\"":\""23\"",\""resultPickup\"":\""4\"",\""mfgCurveDelay\"":\""3\"",\""resultDelay\"":\""2\""}}]},\""40\"":{\""elementIdentification\"":\""12\"",\""dataGrid\"":[{\""elementone\"":{\""elementIdentification\"":\""\"",\""pickUp\"":\""\"",\""timeDelay\"":\""\"",\""mfgCurvePickup\"":\""\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveTimeDelay\"":\""\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\""},\""elementNumber\"":{\""textField\"":\""Element #1\"",\""elementIdentification\"":\""\"",\""pickUp\"":\""2\"",\""timeDelay\"":\""3\"",\""mfgCurvePickup\"":\""2\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveTimeDelay\"":\""\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\"",\""Offset\"":\""2\"",\""resultPickup\"":\""3\"",\""mfgCurveDelay\"":\""33\"",\""resultDelay\"":\""1\""}}]},\""46\"":{\""elementIdentification\"":\""32\"",\""pickup\"":\""3\"",\""delay\"":\""4\"",\""mfgCurvePickup\"":\""2\"",\""resultPickup\"":\""3\"",\""mfgCurveTimeDelay200\"":\""3\"",\""resultTimeDelay200\"":\""2\"",\""mfgCurveTimeDelay300\"":\""2\"",\""resultTimeDelay300\"":\""3\"",\""mfgCurveTimeDelay500\"":\""312\"",\""resultTimeDelay500\"":\""2\""},\""50\"":{\""dataGrid\"":[{\""elementone\"":{\""elementIdentification\"":\""\"",\""pickUp\"":\""\"",\""timeDelay\"":\""\"",\""mfgCurvePickup\"":\""\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveTimeDelay\"":\""\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\""},\""elementNumber\"":{\""textField\"":\""Element #1\"",\""elementIdentification\"":\""21\"",\""pickUp\"":\""3\"",\""timeDelay\"":\""4\"",\""mfgCurvePickup\"":\""3\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveTimeDelay\"":\""\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\"",\""resultPickup\"":\""4\"",\""mfgCurveDelay\"":\""42\"",\""resultDelay\"":\""31\""}}]},\""51\"":{\""dataGrid\"":[{\""elementone\"":{\""elementIdentification\"":\""\"",\""pickUp\"":\""\"",\""timeDelay\"":\""\"",\""mfgCurvePickup\"":\""\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveTimeDelay\"":\""\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\""},\""elementNumber\"":{\""textField\"":\""Element #1\"",\""elementIdentification\"":\""23\"",\""pickUp\"":\""\"",\""timeDelay\"":\""\"",\""mfgCurvePickup\"":\""\"",\""aPhasePickup\"":\""98\"",\""bPhasePickup\"":\""4\"",\""cPhasePickup\"":\""56\"",\""mfgCurveTimeDelay\"":\""\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\"",\""pickup\"":\""4\"",\""curve\"":\""4\"",\""timeDial\"":\""4\"",\""emReset\"":\""2\"",\""mfgCurvePickUp\"":\""7\"",\""mfgCurveDelay200\"":\""4\"",\""aPhaseDelay1\"":\""5\"",\""bPhaseDelay200\"":\""9\"",\""cPhaseDelay200\"":\""7\"",\""mfgCurveDelay300\"":\""8\"",\""aPhasedelay300\"":\""3\"",\""bPhaseDelay300\"":\""1\"",\""cPhaseDelay300\"":\""5\"",\""mfgCurveDelay500\"":\""74\"",\""aPhasedelay500\"":\""5\"",\""bPhaseDelay500\"":\""2\"",\""cPhaseDelay500\"":\""58\""}}]},\""59\"":{\""dataGrid\"":[{\""elementone\"":{\""elementIdentification\"":\""\"",\""pickUp\"":\""\"",\""timeDelay\"":\""\"",\""mfgCurvePickup\"":\""\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveTimeDelay\"":\""\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\""},\""elementNumber\"":{\""textField\"":\""Element #1\"",\""elementIdentification\"":\""12\"",\""pickUp\"":\""123\"",\""timeDelay\"":\""3\"",\""mfgCurvePickup\"":\""\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""4\"",\""mfgCurveTimeDelay\"":\""5\"",\""aPhaseTimeDelay\"":\""25\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""8\"",\""mfgCurvePickUp\"":\""7\"",\""aPhasePickUp\"":\""8\"",\""bPhasePickUp\"":\""6\"",\""bPhaseTimeDelay\"":\""4\""}}]},\""67\"":{\""dataGrid\"":[{\""elementone\"":{\""elementIdentification\"":\""\"",\""pickUp\"":\""\"",\""timeDelay\"":\""\"",\""mfgCurvePickup\"":\""\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveTimeDelay\"":\""\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\""},\""elementNumber\"":{\""textField\"":\""Element #1\"",\""elementIdentification\"":\""23\"",\""pickUp\"":\""\"",\""timeDelay\"":\""44\"",\""mfgCurvePickup\"":\""\"",\""aPhasePickup\"":\""48\"",\""bPhasePickup\"":\""4\"",\""cPhasePickup\"":\""8\"",\""mfgCurveTimeDelay\"":\""\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\"",\""mfgCurvePickUp\"":\""3\"",\""aPhasePickUp\"":\""\"",\""bPhasePickUp\"":\""\"",\""bPhaseTimeDelay\"":\""\"",\""pickup\"":\""3\"",\""forwardAngle\"":\""3\"",\""reverseAngle\"":\""3\"",\""mfgCurveDelay\"":\""3\"",\""aPhaseDelay\"":\""1\"",\""bPhaseDelay\"":\""2\"",\""cPhaseDelay\"":\""5\"",\""mfgCurveForwardAngle\"":\""7\"",\""aPhaseForwardAngle\"":\""8\"",\""bPhaseForwardAngle\"":\""5\"",\""cPhaseForwardAngle\"":\""4\"",\""mfgCurveReverseAngle\"":\""5\"",\""aPhaseReverseAngle\"":\""2\"",\""bPhaseReverseAngle\"":\""4\"",\""cPhaseReverseAngle\"":\""58\""}}]},\""81\"":{\""dataGrid\"":[{\""elementone\"":{\""elementIdentification\"":\""\"",\""pickUp\"":\""\"",\""timeDelay\"":\""\"",\""mfgCurvePickup\"":\""\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveTimeDelay\"":\""\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\""},\""elementNumber\"":{\""textField\"":\""Element #1\"",\""elementIdentification\"":\""12\"",\""pickUp\"":\""3\"",\""timeDelay\"":\""4\"",\""mfgCurvePickup\"":\""12\"",\""aPhasePickup\"":\""\"",\""bPhasePickup\"":\""\"",\""cPhasePickup\"":\""\"",\""mfgCurveTimeDelay\"":\""23\"",\""aPhaseTimeDelay\"":\""\"",\""bPhasePickupTimeDelay\"":\""\"",\""cPhaseTimeDelay\"":\""\"",\""mfgCurvePickUp\"":\""\"",\""aPhasePickUp\"":\""\"",\""bPhasePickUp\"":\""\"",\""bPhaseTimeDelay\"":\""\"",\""resultPickup\"":\""3\"",\""resultTimeDelay\"":\""2\""}}]},\""87\"":{\""elementIdentification\"":\""23\"",\""operatingCurrentPickup\"":\""4\"",\""restraintSlope2\"":\""2\"",\""unrestrainedPickup\"":\""4\"",\""fourthHarmonicBlockingPercent\"":\""5\"",\""restraintSlope1\"":\""556\"",\""slope1Limit\"":\""7\"",\""secondHarmonicBlockingPercent\"":\""9\"",\""fifthHarmonicBlockingPercent\"":\""3\"",\""mfgCurveUnrestrained\"":\""7\"",\""aPhaseUnrestrained\"":\""9\"",\""bPhaseUnrestrained\"":\""3\"",\""cPhaseUnrestrained\"":\""1\"",\""mfgCurveWinding1\"":\""5\"",\""aPhaseWinding1\"":\""4\"",\""bPhaseWinding1\"":\""8\"",\""cPhaseWinding1\"":\""6\"",\""mfgCurveWinding2\"":\""14\"",\""aPhaseWinding2\"":\""52\"",\""bPhaseWinding2\"":\""32\"",\""cPhaseWinding2\"":\""2\"",\""mfgCurveSlope\"":\""21\"",\""pickupSlope\"":\""24\"",\""fundamentalApplied2ndHarmonic\"":\""7\"",\""mfgCurve2ndHarmonic\"":\""8\"",\""pickup2ndHarmonic\"":\""2\"",\""fundamentalApplied4thHarmonic\"":\""14\"",\""mfgCurve4thHarmonic\"":\""2\"",\""pickup4thHarmonic\"":\""4\""},\""header\"":{\""workOrderType\"":\""Acceptance Test\"",\""customer\"":\""Sandbox Facility\"",\""customerAddress\"":\""Egalvanic Way, Madison, WI\"",\""owner\"":\""Sandbox Company\"",\""ownerAddress\"":\""Egalvanic Way, Madison, WI\"",\""date\"":\""2023-10-03T00:00:00+05:30\"",\""workOrder\"":\""FORM-JSON-TEMP\"",\""temperature\"":\""3\"",\""humidity\"":\""4\"",\""identification\"":\""3\"",\""parent\"":\""2\"",\""assetId\"":\""4\"",\""building\"":\""2\"",\""floor\"":\""4\"",\""room\"":\""4\"",\""section\"":\""3\""},\""nameplateInformation\"":{\""manufacturer\"":\""4\"",\""modelNumber\"":\""5\"",\""relayType\"":\""5\"",\""serialNumber\"":\""4\""},\""relaySettings\"":{\""phaseRotation\"":\""4\"",\""nominalVoltage\"":\""5\"",\""phaseCtRatio\"":\""3\"",\""groundCtRatio\"":\""7\"",\""ptConfiguration\"":\""3\"",\""nominalCurrent\"":\""4\"",\""vtRatio\"":\""5\"",\""vsRatio\"":\""7\"",\""selectBoxes1\"":{\""21Element\"":true,\""24Element\"":true,\""25Element\"":true,\""27Element\"":true,\""32Element\"":true,\""40Element\"":true,\""46Element\"":true,\""50Element\"":true,\""51Element\"":true,\""59Element\"":true,\""67Element\"":true,\""81Element\"":true,\""87Element\"":true}},\""footer\"":{\""testEquipmentNumber\"":\""25\"",\""inspectionVerdict\"":\""Acceptable\"",\""comments\"":\""The transformer PASSED and is acceptable for operation.\"",\""testedBy\"":\""34\"",\""testEquipmentCalibrationTable\"":[{\""equipmentId\"":\""5\"",\""name\"":\""4\"",\""serialNumber\"":\""5\"",\""calibrationDate\"":\""2023-10-25T00:00:00+05:30\""}],\""copyright\"":{}}}}""
                     }
                    ]";


                List<Root> myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(listitems);

                foreach (var item in myDeserializedClass)
                {
                    var res = _UoW.AssetRepository.InsertFormIOTemplate(Guid.Parse(item.FORMID));
                    res.form_output_data_template = item.JSON;
                    await _UoW.BaseGenericRepository<InspectionsTemplateFormIO>().Update(res);
                    _UoW.SaveChanges();
                }
                result = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {

            }

            return result;
        }
        public async Task<GetNameplateJsonFromImagesResponseModel> GetNameplateJsonFromImages(GetNameplateJsonFromImagesRequestModel requestModel)
        {
            GetNameplateJsonFromImagesResponseModel responsemodel = new GetNameplateJsonFromImagesResponseModel();
            responsemodel.status = (int)ResponseStatusNumber.Error;
            try
            {
                if (requestModel.woonboardingassetsId != null && requestModel.woonboardingassetsId != Guid.Empty)
                {
                    var get_nameplate_image = _UoW.AssetRepository.GetLatestNameplateImageById(requestModel.woonboardingassetsId.Value);
                    if (get_nameplate_image != null && !String.IsNullOrEmpty(get_nameplate_image.image_extracted_json))
                    {
                        responsemodel.status = (int)Status.Completed;
                        responsemodel.nameplate_json = get_nameplate_image.image_extracted_json;
                        return responsemodel;
                    }
                }
                else if (requestModel.assetId != null && requestModel.assetId != Guid.Empty)
                {
                    var get_nameplate_image = _UoW.AssetRepository.GetLatestNameplateImageByAssetId(requestModel.assetId.Value);
                    if (get_nameplate_image != null && !String.IsNullOrEmpty(get_nameplate_image.image_extracted_json))
                    {
                        responsemodel.status = (int)Status.Completed;
                        responsemodel.nameplate_json = get_nameplate_image.image_extracted_json;
                        return responsemodel;
                    }
                }

                HttpClient client = new HttpClient();

                //{{baseUrl}}/nameplate/analyse
                string base_url = ConfigurationManager.AppSettings["eg_ai_nameplate_info_url"] + "nameplate/analyse";//ConfigurationManager.AppSettings["Cognito_Base_Url"];

                // Set a timeout for 1 minute
                var timeout = TimeSpan.FromSeconds(45);//.FromMinutes(1);
                var startTime = DateTime.UtcNow;
                var content = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(base_url, content).Result;

                while (DateTime.UtcNow - startTime < timeout)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = response.Content.ReadAsStringAsync().Result.ToString();
                        Root_Anayse_API json = Newtonsoft.Json.JsonConvert.DeserializeObject<Root_Anayse_API>(responseData);
                        var task_id = json.data.taskId;
                        var response_getapi = await CallSecondAPI(client, task_id);

                        if (response_getapi.status == (int)Status.Completed)
                            return response_getapi;
                        else if (response_getapi.status == (int)Status.running)
                            await Task.Delay(5000);
                        else
                            break;
                    }
                    else
                    {
                        break;
                    }
                    //await Task.Delay(5000);
                }
                // Check if the loop ended due to timeout
                if (DateTime.UtcNow - startTime >= timeout)
                {
                    responsemodel.status = (int)ResponseStatusNumber.TimedOut;
                }
            }
            catch (Exception e)
            {
            }
            return responsemodel;
        }

        public async Task<GetNameplateJsonFromImagesResponseModel> CallSecondAPI(HttpClient client, int task_id)
        {
            GetNameplateJsonFromImagesResponseModel responsemodel = new GetNameplateJsonFromImagesResponseModel();
            int res = (int)ResponseStatusNumber.Error;
            string nameplate_json = null;
            try
            {
                string base_url = ConfigurationManager.AppSettings["eg_ai_nameplate_info_url"] + "task/status/" + task_id;//ConfigurationManager.AppSettings["Cognito_Base_Url"];

                HttpResponseMessage response = await client.GetAsync(base_url);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    Root_Status_API json = Newtonsoft.Json.JsonConvert.DeserializeObject<Root_Status_API>(responseData);

                    if (json.data.status == "completed")
                    {
                        res = (int)Status.Completed;
                        nameplate_json = Newtonsoft.Json.JsonConvert.SerializeObject(json.data.result.nameplateJson);
                    }
                    else if (json.data.status == "running")
                    {
                        res = (int)Status.running;
                    }
                }
                else
                {
                }
            }
            catch (Exception e)
            {
            }
            responsemodel.status = res;
            responsemodel.nameplate_json = nameplate_json;
            return responsemodel;
        }

        public async Task<CreateUpdateAssetGroupResponseModel> CreateUpdateAssetGroup(CreateUpdateAssetGroupRequestModel requestModel)
        {
            int res = (int)ResponseStatusNumber.Error;
            CreateUpdateAssetGroupResponseModel responseModel = new CreateUpdateAssetGroupResponseModel();
            try
            {

                if (requestModel.asset_group_id == null || requestModel.asset_group_id == Guid.Empty)//-- Create AssetGroup
                {
                    AssetGroup assetGroup = new AssetGroup();

                    assetGroup.asset_group_name = requestModel.asset_group_name;
                    assetGroup.asset_group_description = requestModel.asset_group_description;
                    assetGroup.criticality_index_type = requestModel.criticality_index_type;
                    assetGroup.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                    assetGroup.is_deleted = false;
                    assetGroup.created_at = DateTime.UtcNow;
                    assetGroup.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    var insert = await _UoW.BaseGenericRepository<AssetGroup>().Insert(assetGroup);
                    if (insert)
                    {
                        _UoW.SaveChanges();
                        res = (int)ResponseStatusNumber.Success;
                    }
                    responseModel.asset_group_id = assetGroup.asset_group_id;
                }
                else
                {
                    var assetGroup = _UoW.AssetRepository.GetAssetGroupById(requestModel.asset_group_id.Value);

                    if (assetGroup != null)
                    {
                        if (requestModel.is_deleted) //-- Delete AssetGroup
                        {
                            assetGroup.is_deleted = true;
                            assetGroup.modified_at = DateTime.UtcNow;
                            assetGroup.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                            await DeleteAssetGroupIdFromAssets(assetGroup.asset_group_id);
                        }
                        else //-- Update AssetGroup
                        {
                            assetGroup.asset_group_name = requestModel.asset_group_name;
                            assetGroup.asset_group_description = requestModel.asset_group_description;
                            assetGroup.criticality_index_type = requestModel.criticality_index_type;
                            assetGroup.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                            assetGroup.modified_at = DateTime.UtcNow;
                            assetGroup.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        }
                        var update = await _UoW.BaseGenericRepository<AssetGroup>().Update(assetGroup);
                        if (update)
                        {
                            _UoW.SaveChanges();
                            res = (int)ResponseStatusNumber.Success;
                        }
                        responseModel.asset_group_id = assetGroup.asset_group_id;
                    }
                }

                var deletedAssets = requestModel.assets_list.Where(x => x.is_deleted).Select(a => a.asset_id).ToList();
                var deleted_asset = _UoW.AssetRepository.GetAssetsByAssetId(deletedAssets);

                foreach (var asset in deleted_asset)
                {
                    asset.asset_group_id = null;
                    asset.modified_at = DateTime.UtcNow;
                    asset.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    var update = await _UoW.BaseGenericRepository<Asset>().Update(asset);
                    if (update)
                    {
                        _UoW.SaveChanges();

                    }
                }

                var assets_list = requestModel.assets_list.Where(x => !x.is_deleted).Select(a => a.asset_id).ToList();
                var updated_asset = _UoW.AssetRepository.GetAssetsByAssetId(assets_list);

                foreach (var asset in updated_asset)
                {
                    asset.asset_group_id = responseModel.asset_group_id;
                    asset.modified_at = DateTime.UtcNow;
                    asset.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    var update = await _UoW.BaseGenericRepository<Asset>().Update(asset);
                    if (update)
                    {
                        _UoW.SaveChanges();

                    }

                }
            }
            catch (Exception ex)
            {
            }
            responseModel.status = res;
            return responseModel;

        }

        public async Task<bool> DeleteAssetGroupIdFromAssets(Guid assetGroupId)
        {
            try
            {
                var all_assets = _UoW.AssetRepository.GetAllAssetsByAssetGroupId(assetGroupId);
                foreach (var asset in all_assets)
                {
                    asset.asset_group_id = null;
                    asset.modified_at = DateTime.UtcNow;
                    asset.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    var update = await _UoW.BaseGenericRepository<Asset>().Update(asset);
                }

                var all_temp_assets = _UoW.AssetRepository.GetAllTempAssetsByAssetGroupId(assetGroupId);
                foreach (var temp_asset in all_temp_assets)
                {
                    temp_asset.asset_group_id = null;
                    temp_asset.modified_at = DateTime.UtcNow;
                    temp_asset.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    var update = await _UoW.BaseGenericRepository<TempAsset>().Update(temp_asset);
                }

                _UoW.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return true;
        }
        public AssetGroupsDropdownListResponseModel AssetGroupsDropdownList()
        {
            AssetGroupsDropdownListResponseModel response = new AssetGroupsDropdownListResponseModel();
            try
            {
                var get_asset_groups_list = _UoW.AssetRepository.AssetGroupsDropdownList();

                if (get_asset_groups_list != null && get_asset_groups_list.Count > 0)
                {
                    response.list = _mapper.Map<List<AssetGroupClass_Obj>>(get_asset_groups_list);
                }
            }
            catch (Exception ex)
            {
            }
            return response;
        }

        public AssetListDropdownForAssetGroupResponseModel AssetListDropdownForAssetGroup(Guid asset_group_id)
        {
            AssetListDropdownForAssetGroupResponseModel response = new AssetListDropdownForAssetGroupResponseModel();
            try
            {
                var get_asset_list = _UoW.AssetRepository.AssetListDropdownForAssetGroup(asset_group_id);

                if (get_asset_list != null && get_asset_list.Count > 0)
                {
                    response.list = _mapper.Map<List<Asset_Obj>>(get_asset_list);
                }
            }
            catch (Exception ex)
            {
            }
            return response;
        }

        public GetAllAssetGroupsListResponseModel GetAllAssetGroupsList(GetAllAssetGroupsListRequestModel requestModel)
        {
            GetAllAssetGroupsListResponseModel response = new GetAllAssetGroupsListResponseModel();
            try
            {
                var get_asset_groups_list = _UoW.AssetRepository.GetAllAssetGroupsList(requestModel);

                if (get_asset_groups_list.Item1 != null && get_asset_groups_list.Item1.Count > 0)
                {
                    response.list = get_asset_groups_list.Item1;
                }
                response.count = get_asset_groups_list.Item2;

            }
            catch (Exception ex)
            {
            }
            return response;
        }


        public ListViewModel<GetAllOnboardingAssetsByWOIdResponseModel> GetAllOnboardingAssetsByWOId(GetAllOnboardingAssetsByWOIdRequestModel requestModel)
        {
            ListViewModel<GetAllOnboardingAssetsByWOIdResponseModel> listViewModel = new ListViewModel<GetAllOnboardingAssetsByWOIdResponseModel>();
            try
            {
                var ob_assets = _UoW.AssetRepository.GetAllOnboardingAssetsByWOId(requestModel);

                if (ob_assets.Item1 != null && ob_assets.Item1.Count > 0)
                {
                    foreach (var item in ob_assets.Item1)
                    {
                        if (item.component_level_type_id == (int)ComponentLevelTypes.SublevelComponent)
                        {
                            item.toplevelcomponent_asset_id = _UoW.WorkOrderRepository.GetTopLevelWOOBAsset(item.woonboardingassets_id);
                        }
                        var img = _UoW.WorkOrderRepository.GetAssetProfileImage(item.woonboardingassets_id);
                        item.asset_profile = UrlGenerator.GetAssetImagesURL(img);
                    }

                    listViewModel.list = ob_assets.Item1;
                    listViewModel.listsize = ob_assets.Item2;
                }
                //listViewModel.list = _mapper.Map<List<GetAllOnboardingAssetsByWOIdResponseModel>>(ob_assets);

            }
            catch (Exception e)
            {
            }
            return listViewModel;
        }

        public GetAllAssetsListForReactFlowResponseModel GetAllAssetsListForReactFlow()
        {
            GetAllAssetsListForReactFlowResponseModel response = new GetAllAssetsListForReactFlowResponseModel();
            try
            {
                var get_assets = _UoW.AssetRepository.GetAllAssetsListForReactFlow();
                var inditialEdges = _UoW.AssetRepository.GetAllAssetsConnectionsForReactFlow();

                response.list = get_assets;
                response.initialEdges = inditialEdges;
            }
            catch (Exception e)
            {
            }
            return response;
        }

        public async Task<int> UpdateAssetsPositionForReactFlow(UpdateAssetsPositionForReactFlowRequestModel requestModel)
        {
            int res = (int)ResponseStatusNumber.Error;
            try
            {
                if (requestModel.assets.Count > 0)
                {
                    /*
                    var ids = requestModel.assets.Select(x => x.asset_id).ToList();
                    var get_assets = _UoW.AssetRepository.GetAllAssetsListByIds(ids);

                    foreach( var asset in get_assets)
                    {
                        var req_asset = requestModel.assets.Where(x=>x.asset_id==asset.asset_id).FirstOrDefault();
                        asset.x_axis = req_asset!=null ? req_asset.x_axis : 0;
                        asset.y_axis = req_asset!=null ? req_asset.y_axis : 0;
                        asset.modified_at = DateTime.UtcNow;
                        asset.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        var update = await _UoW.BaseGenericRepository<Asset>().Update(asset);
                    }
                    res = (int)ResponseStatusNumber.Success;
                    */

                    foreach (var item in requestModel.assets)
                    {
                        var get_asset = _UoW.AssetRepository.GetAssetDatabyId(item.asset_id);
                        //var assetclass = _UoW.WorkOrderRepository.GetAssetClassByClasscode(item.asset_class_code);

                        if (get_asset != null)
                        {
                            //get_asset.name = item.asset_name;
                            //if (assetclass != null)
                            //    get_asset.inspectiontemplate_asset_class_id = assetclass.inspectiontemplate_asset_class_id;

                            get_asset.x_axis = item.x_axis;
                            get_asset.y_axis = item.y_axis;
                            get_asset.asset_node_data_json = item.asset_node_data_json;
                            get_asset.modified_at = DateTime.UtcNow;
                            get_asset.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                            var update = await _UoW.BaseGenericRepository<Asset>().Update(get_asset);
                            _UoW.SaveChanges();
                        }
                        else
                        {
                            EditAssetDetailsRequestmodel req = new EditAssetDetailsRequestmodel();
                            req.asset_id = null;
                            //req.asset_name = item.asset_name;
                            //if (assetclass != null)
                            //    req.inspectiontemplate_asset_class_id = assetclass.inspectiontemplate_asset_class_id;

                            req.x_axis = item.x_axis;
                            req.y_axis = item.y_axis;
                            req.asset_node_data_json = item.asset_node_data_json;

                            await EditAssetDetails(req);
                        }
                    }
                    res = (int)ResponseStatusNumber.Success;
                }

                if (requestModel.edges.Count > 0)
                {
                    var deleted_parentHirerachyIdList = requestModel.edges.Where(x => x.is_deleted).Select(x => x.id).ToList();
                    if (deleted_parentHirerachyIdList.Count > 0)
                    {
                        var get_asset_parent_hirerachy_mapping = _UoW.AssetRepository.GetAssetParentHirerachyList(deleted_parentHirerachyIdList);

                        foreach (var item in get_asset_parent_hirerachy_mapping)
                        {
                            var get_child_map = _UoW.AssetRepository.GetAssetChildrenMapping(item.parent_asset_id.Value, item.asset_id.Value);
                            if (get_child_map != null)
                            {
                                get_child_map.is_deleted = true;
                                get_child_map.updated_at = DateTime.Now;
                                get_child_map.updated_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                await _UoW.BaseGenericRepository<AssetChildrenHierarchyMapping>().Update(get_child_map);
                            }
                            item.is_deleted = true;
                            item.updated_at = DateTime.Now;
                            item.updated_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            var update = await _UoW.BaseGenericRepository<AssetParentHierarchyMapping>().Update(item);
                        }
                    }

                    //update label 
                    foreach(var item in requestModel.edges.Where(x=> !x.is_deleted && !String.IsNullOrEmpty(x.id)))
                    {
                        var get_map = _UoW.AssetRepository.UpdateAssetFedByCircuit(Guid.Parse(item.id));
                        if (get_map != null)
                        {
                            get_map.label = item.label;
                            get_map.style = item.style;
                            get_map.updated_at = DateTime.Now;
                            get_map.updated_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            var update = await _UoW.BaseGenericRepository<AssetParentHierarchyMapping>().Update(get_map);
                        }
                    }

                    var newAddedMapping = requestModel.edges.Where(x => String.IsNullOrEmpty(x.id)).ToList();
                    foreach(var item in newAddedMapping)
                    {
                        AssetParentHierarchyMapping assetParentHierarchyMapping = new AssetParentHierarchyMapping();
                        assetParentHierarchyMapping.parent_asset_id = item.source;
                        assetParentHierarchyMapping.asset_id = item.target;
                        assetParentHierarchyMapping.fed_by_via_subcomponant_asset_id = item.fed_by_via_subcomponant_asset_id;
                        assetParentHierarchyMapping.via_subcomponent_asset_id = item.via_subcomponent_asset_id;
                        assetParentHierarchyMapping.label = item.label;
                        assetParentHierarchyMapping.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                        assetParentHierarchyMapping.is_deleted = false;
                        assetParentHierarchyMapping.created_at = DateTime.UtcNow;
                        assetParentHierarchyMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        ;

                        var insert = await _UoW.BaseGenericRepository<AssetParentHierarchyMapping>().Insert(assetParentHierarchyMapping);

                        AssetChildrenHierarchyMapping assetChildrenHierarchyMapping = new AssetChildrenHierarchyMapping();
                        assetChildrenHierarchyMapping.asset_id = item.source;
                        assetChildrenHierarchyMapping.children_asset_id = item.target;
                        assetChildrenHierarchyMapping.via_subcomponent_asset_id = item.via_subcomponent_asset_id;
                        assetChildrenHierarchyMapping.is_deleted = false;
                        assetChildrenHierarchyMapping.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                        assetChildrenHierarchyMapping.created_at = DateTime.UtcNow;
                        assetChildrenHierarchyMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        
                        var insert2 = await _UoW.BaseGenericRepository<AssetChildrenHierarchyMapping>().Insert(assetChildrenHierarchyMapping);
                    }
                    res = (int)ResponseStatusNumber.Success;
                    _UoW.SaveChanges();
                }
            }
            catch (Exception e)
            {
            }
            return res;
        }

        public class Root
        {
            [JsonProperty("FORM ID")]
            public string FORMID { get; set; }
            public string JSON { get; set; }
        }
        public class EditAssetDetailsResponseModel
        {
            public Guid? asset_id { get; set; }
            public int status { get; set; }
        }
        public class nameplate_info
        {
            public string manufacturer { get; set; }
            public string catalogNumber { get; set; }
            public string model { get; set; }
            public string serialNumber { get; set; }
        }
        public List<user_details> ValidateRecords()
        {


            for (int i = 2; i <= _activteworksheet.Dimension.Rows; i++)
            {
                user_details user_Details = new user_details();
                List<string> datalist = new List<string>();

                for (int j = 1; j <= _activteworksheet.Dimension.Columns; j++)
                {
                    var value = _activteworksheet.Cells[i, j].Value == null ? "" : _activteworksheet.Cells[i, j].Value.ToString();
                    if (value == "NULL" || String.IsNullOrEmpty(value)) // handle NULL string 
                    {
                        value = null;
                    }
                    datalist.Add(value);
                }
                users.Add(filldata(datalist));
            }
            return users;
        }

        public user_details filldata(List<string> datalist)
        {
            user_details user_Details = new user_details();
            user_Details.location = datalist[0];
            user_Details.identification = datalist[1];
            user_Details.AssetID = datalist[2];
            user_Details.Building = datalist[3];
            user_Details.Room = datalist[4];
            user_Details.Floor = datalist[5];
            user_Details.Section = datalist[6];
            user_Details.Notes = datalist[7];
            // user_Details.Issue_Comments = datalist[9];
            user_Details.Manufacturer = datalist[8];
            user_Details.Serial_Number = datalist[10];
            user_Details.Model = datalist[9];
            user_Details.Catalog_Number = datalist[11];
            return user_Details;
        }

        public class user_details
        {
            public string location { get; set; }
            public string identification { get; set; }
            public string AssetID { get; set; }
            public string Building { get; set; }
            public string Room { get; set; }
            public string Floor { get; set; }
            public string Section { get; set; }
            public string Notes { get; set; }
            public string Issue_Comments { get; set; }
            public string Manufacturer { get; set; }
            public string Serial_Number { get; set; }
            public string Model { get; set; }
            public string Catalog_Number { get; set; }
        }
    }
}
