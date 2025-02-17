using AutoMapper;
using Jarvis.db.Migrations;
using Jarvis.db.Models;
using Jarvis.Service.Abstract;
using Jarvis.Service.Notification;
using Jarvis.Service.Services;
using Jarvis.Shared;
using Jarvis.Shared.Helper;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jarvis.Service.Concrete
{
    public class InspectionService : BaseService, IInspectionService
    {
        public readonly IMapper _mapper;
        private Shared.Utility.Logger _logger;
        SyncRecord record = null;
        NotificationService notificationService = null;
        S3BucketService s3BucketService = null;
        IssueService issueService = null;
        public InspectionService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            _logger = Shared.Utility.Logger.GetInstance<InspectionService>();
        }

        public ListViewModel<InspectionResponseModel> GetAllInspections(int pagesize, int pageindex)
        {
            ListViewModel<InspectionResponseModel> responseModel = new ListViewModel<InspectionResponseModel>();
            try
            {
                var response = _UoW.AssetRepository.GetAllInspections(GenericRequestModel.requested_by.ToString(), pagesize, pageindex);
                if (response.Count > 0)
                {
                    int inspectionlist = response.Count;
                    if (pageindex > 0 && pagesize > 0)
                    {
                        response = response.OrderByDescending(x => x.datetime_requested).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    }

                    responseModel.list = _mapper.Map<List<InspectionResponseModel>>(response);
                    responseModel.listsize = inspectionlist;
                    //var responselist = responseModel;

                    //responseModel.list.ForEach(x =>
                    //{
                    //    if (x.status == (int)Status.Pending)
                    //    {
                    //        x.status_name = "Pending";
                    //    }
                    //    else if (x.status == (int)Status.Cencelled)
                    //    {
                    //        x.status_name = "Cencelled";
                    //    }
                    //    else if (x.status == (int)Status.Approved)
                    //    {
                    //        x.status_name = "Approved";
                    //    }
                    //    else if (x.status == (int)Status.InspectionMaintenance)
                    //    {
                    //        x.status_name = "InMaintenanace";
                    //    }
                    //});

                    //responseModel = responselist;
                    foreach (var inspection in responseModel.list)
                    {
                        List<CategoryWiseAttributesInsepction> categoryWiseAttributesInsepctions = new List<CategoryWiseAttributesInsepction>();
                        List<CategoryWiseAttributesInsepction> newcategoryWiseAttributesInsepctions = new List<CategoryWiseAttributesInsepction>();
                        //Guid operatorguid = Guid.Parse(response.operator_id.ToString());
                        var name = _UoW.InspectionRepository.FindUserNameById(inspection.operator_id);
                        inspection.operator_name = name;
                        //var resultlist = response.

                        var results = inspection.attribute_values.GroupBy(
                                    p => p.category_id,
                                    (key, g) => new { category_id = key, Attributes = g.ToList() });
                        var list1 = results.ToList();
                        foreach (var item in list1)
                        {
                            CategoryWiseAttributesInsepction categoryWiseAttribute = new CategoryWiseAttributesInsepction();
                            CategoryWiseAttributesInsepction newcategoryWiseAttribute = new CategoryWiseAttributesInsepction();
                            categoryWiseAttribute.category_id = item.category_id;
                            var categoryObject = _UoW.InspectionFormRepository.GetCategoryByID(item.category_id);
                            if (categoryObject != null)
                            {
                                categoryWiseAttribute.name = categoryObject.name;
                            }
                            categoryWiseAttribute.attribute_values = item.Attributes;
                            categoryWiseAttributesInsepctions.Add(categoryWiseAttribute);

                            var newnotokAttr = item.Attributes.Where(x => inspection.Issues.Select(y => y.attribute_id).ToList().Contains(x.id)).ToList();
                            if (newnotokAttr != null && newnotokAttr.Count > 0)
                            {
                                if (categoryObject != null)
                                {
                                    newcategoryWiseAttribute.name = categoryObject.name;
                                }
                                newcategoryWiseAttribute.attribute_values = newnotokAttr.Where(x => x.value?.ToLower() == "not ok").ToList();
                                newcategoryWiseAttributesInsepctions.Add(newcategoryWiseAttribute);
                            }
                        }
                        inspection.attributes = categoryWiseAttributesInsepctions;
                        inspection.new_notok_attributes_by_category = newcategoryWiseAttributesInsepctions;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }

        public ListViewModel<InspectionResponseModel> FilterInspections(FilterInspectionsRequestModel requestModel)
        {
            ListViewModel<InspectionResponseModel> responseModel = new ListViewModel<InspectionResponseModel>();
            try
            {
                var response = _UoW.InspectionRepository.FilterInspections(requestModel);
                if (response?.list?.Count > 0)
                {
                    //int inspectionlist = response.list.Count;
                    //if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    //{
                    //    requestModel.pagesize = 20;
                    //    requestModel.pageindex = 1;
                    //}
                    //response = response.OrderByDescending(x => x.datetime_requested).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                    responseModel.list = _mapper.Map<List<InspectionResponseModel>>(response.list);
                    foreach (var item in responseModel.list)
                    {
                        if(item.manager_id != null && item.manager_id != "")
                        {
                            var user = _UoW.UserRepository.GetUserByUserID(item.manager_id);
                            item.manager_name = user.firstname + " " + user.lastname;
                        }
                        else
                        {
                            item.manager_name = "";
                        }

                        if (item.status == (int)Status.Approved && (item.modified_at != null))
                        {
                            item.approval_date = item.modified_at;
                        }
                        
                    }
                    responseModel.listsize = response.listsize;
                    responseModel.pageIndex = response.pageIndex;
                    responseModel.pageSize = response.pageSize;
                    foreach (var inspection in responseModel.list)
                    {
                        List<CategoryWiseAttributesInsepction> categoryWiseAttributesInsepctions = new List<CategoryWiseAttributesInsepction>();
                        List<CategoryWiseAttributesInsepction> newcategoryWiseAttributesInsepctions = new List<CategoryWiseAttributesInsepction>();
                        List<InspectionAttributesJsonObjectViewModel> newNotOkAttributes = new List<InspectionAttributesJsonObjectViewModel>();
                        var name = _UoW.InspectionRepository.FindUserNameById(inspection.operator_id);
                        inspection.operator_name = name;
                        var results = inspection.attribute_values.GroupBy(
                                    p => p.category_id,
                                    (key, g) => new { category_id = key, Attributes = g.ToList() });
                        var list1 = results.ToList();
                        foreach (var item in list1)
                        {
                            CategoryWiseAttributesInsepction categoryWiseAttribute = new CategoryWiseAttributesInsepction();
                            CategoryWiseAttributesInsepction newcategoryWiseAttribute = new CategoryWiseAttributesInsepction();
                            categoryWiseAttribute.category_id = item.category_id;
                            var categoryObject = _UoW.InspectionFormRepository.GetCategoryByID(item.category_id);
                            if (categoryObject != null)
                            {
                                categoryWiseAttribute.name = categoryObject.name;
                            }
                            categoryWiseAttribute.attribute_values = item.Attributes;
                            categoryWiseAttributesInsepctions.Add(categoryWiseAttribute);

                            var newnotokAttr = item.Attributes.Where(x => inspection.Issues.Select(y => y.attribute_id).ToList().Contains(x.id)).ToList();
                            if (newnotokAttr != null && newnotokAttr.Count > 0)
                            {
                                if (categoryObject != null)
                                {
                                    newcategoryWiseAttribute.name = categoryObject.name;
                                }
                                newcategoryWiseAttribute.attribute_values = newnotokAttr.Where(x => x.value?.ToLower() == "not ok").ToList();
                                newcategoryWiseAttributesInsepctions.Add(newcategoryWiseAttribute);
                                newNotOkAttributes.AddRange(newcategoryWiseAttribute.attribute_values);
                            }
                        }
                        inspection.attributes = categoryWiseAttributesInsepctions;
                        inspection.new_notok_attributes_by_category = newcategoryWiseAttributesInsepctions;
                        inspection.new_notok_attributes = newNotOkAttributes;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }

        public ListViewModel<AssetListResponseModel> FilterInspectionAssetNameOption(FilterInspectionOptionsRequestModel requestModel)
        {
            ListViewModel<AssetListResponseModel> filterResponse = new ListViewModel<AssetListResponseModel>();
            try
            {
                var response = _UoW.InspectionRepository.FilterInspectionAssetNameOptions(requestModel);
                if (response?.list?.Count > 0)
                {
                    filterResponse.list = _mapper.Map<List<AssetListResponseModel>>(response.list.Select(x => x.Asset).Distinct().ToList());
                    //int inspectionlist = filterResponse.list.Count;
                    //if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    //{
                    //    requestModel.pagesize = 20;
                    //    requestModel.pageindex = 1;
                    //}
                    //filterResponse.list = filterResponse.list.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                    filterResponse.listsize = response.listsize;
                    filterResponse.pageIndex = response.pageIndex;
                    filterResponse.pageSize = response.pageSize;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return filterResponse;
        }

        public ListViewModel<int> FilterInspectionStatusOption(FilterInspectionOptionsRequestModel requestModel)
        {
            ListViewModel<int> filterResponse = new ListViewModel<int>();
            try
            {
                var response = _UoW.InspectionRepository.FilterInspectionStatusOptions(requestModel);
                if (response.Count > 0)
                {
                    filterResponse.list = response.Where(x => x.status > 0).Select(x => x.status).Distinct().ToList();
                    int inspectionlist = filterResponse.list.Count;
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    filterResponse.list = filterResponse.list.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                    filterResponse.listsize = inspectionlist;
                    filterResponse.pageIndex = requestModel.pageindex;
                    filterResponse.pageSize = requestModel.pagesize;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return filterResponse;
        }

        public ListViewModel<int> FilterInspectionShiftNumberOption(FilterInspectionOptionsRequestModel requestModel)
        {
            ListViewModel<int> filterResponse = new ListViewModel<int>();
            try
            {
                var response = _UoW.InspectionRepository.FilterInspectionShiftNumberOptions(requestModel);
                if (response?.list?.Count > 0)
                {
                    filterResponse.list = response.list.Select(x => x.shift).Distinct().ToList();
                    filterResponse.listsize = response.listsize;
                    filterResponse.pageIndex = response.pageIndex;
                    filterResponse.pageSize = response.pageSize;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return filterResponse;
        }

        public ListViewModel<OperatorsListResponseModel> FilterInspectionOperatorsOption(FilterInspectionOptionsRequestModel requestModel)
        {
            ListViewModel<OperatorsListResponseModel> filterResponse = new ListViewModel<OperatorsListResponseModel>();
            try
            {
                var response = _UoW.InspectionRepository.FilterInspectionOperatorsOptions(requestModel);
                if (response?.list?.Count > 0)
                {
                    filterResponse.list = _mapper.Map<List<OperatorsListResponseModel>>(response.list.Select(x => x.User).Distinct().ToList());
                    //int inspectionlist = filterResponse.list.Count;
                    //if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    //{
                    //    requestModel.pagesize = 20;
                    //    requestModel.pageindex = 1;
                    //}
                    //filterResponse.list = filterResponse.list.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                    filterResponse.listsize = response.listsize;
                    filterResponse.pageIndex = response.pageIndex;
                    filterResponse.pageSize = response.pageSize;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return filterResponse;
        }

        public ListViewModel<OperatorsListResponseModel> FilterInspectionSupervisorOption(FilterInspectionOptionsRequestModel requestModel)
        {
            ListViewModel<OperatorsListResponseModel> filterResponse = new ListViewModel<OperatorsListResponseModel>();
            try
            {
                var response = _UoW.InspectionRepository.FilterInspectionSupervisorOptions(requestModel);
                if (response?.list?.Count > 0)
                {
                    var distinctValue = response.list.Select(x => x.manager_id).ToList().Distinct().ToList();
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    //response.list = response.list.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).OrderBy(x => x.manager_id).ToList();
                    foreach (var item in distinctValue)
                    {
                        OperatorsListResponseModel model = new OperatorsListResponseModel();
                        var user = _UoW.UserRepository.GetUserByUserID(item.ToString());
                        if (user != null)
                        {
                            model.uuid = Guid.Parse(item);
                            model.username = user.firstname + " " + user.lastname;
                            filterResponse.list.Add(model);
                        }
                    }
                    filterResponse.list = filterResponse.list.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).OrderBy(x => x.username).ToList();
                    filterResponse.listsize = response.list.Count;
                    filterResponse.pageIndex = requestModel.pageindex;
                    filterResponse.pageSize = requestModel.pagesize;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return filterResponse;
        }

        public ListViewModel<SitesViewModel> FilterInspectionSitesOption(FilterInspectionOptionsRequestModel requestModel)
        {
            ListViewModel<SitesViewModel> filterResponse = new ListViewModel<SitesViewModel>();
            try
            {
                var response = _UoW.InspectionRepository.FilterInspectionSitesOptions(requestModel);
                if (response?.list?.Count > 0)
                {
                    filterResponse.list = _mapper.Map<List<SitesViewModel>>(response.list.Select(x => x.Sites).Distinct().ToList());
                    filterResponse.listsize = response.listsize;
                    filterResponse.pageIndex = response.pageIndex;
                    filterResponse.pageSize = response.pageSize;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return filterResponse;
        }

        public ListViewModel<CompanyViewModel> FilterInspectionCompanyOption(FilterInspectionOptionsRequestModel requestModel)
        {
            ListViewModel<CompanyViewModel> filterResponse = new ListViewModel<CompanyViewModel>();
            try
            {
                var response = _UoW.InspectionRepository.FilterInspectionCompanyOptions(requestModel);
                if (response?.list?.Count > 0)
                {
                    filterResponse.list = _mapper.Map<List<CompanyViewModel>>(response.list.Select(x => x.Sites.Company).Distinct().ToList());
                    filterResponse.listsize = response.listsize;
                    filterResponse.pageIndex = response.pageIndex;
                    filterResponse.pageSize = response.pageSize;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return filterResponse;
        }

        public async Task<bool> AddInspection(InspectionRequestModel requestModel)
        {
            bool result = false;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var inspectionmapper = _mapper.Map<Inspection>(requestModel);

                    var attributes = inspectionmapper.attribute_values.ToList();

                    List<AssetsValueJsonObject> assetsValueJsonObjects = new List<AssetsValueJsonObject>();

                    foreach (var insp in attributes)
                    {
                        AssetsValueJsonObject assetsValueJsonObject = new AssetsValueJsonObject();
                        var Attribute = _UoW.InspectionRepository.GetAttributesCategoryFromId(insp.id);
                        if (Attribute != null && Attribute.attributes_id != null)
                        {
                            assetsValueJsonObject.category_id = Attribute.category_id;
                            assetsValueJsonObject.id = insp.id;
                            assetsValueJsonObject.name = insp.name;
                            assetsValueJsonObject.value = insp.value;
                            assetsValueJsonObject.values_type = Attribute.values_type;
                            assetsValueJsonObject.date_time = insp.date_time;
                            assetsValueJsonObjects.Add(assetsValueJsonObject);
                        }
                        else
                        {
                            _UoW.RollbackTransaction();
                            result = false;
                            return result;
                        }
                    }

                    inspectionmapper.attribute_values = assetsValueJsonObjects;

                    //foreach(var attribute in inspectionmapper.attribute_values)
                    //{
                    //    if(attribute.value == "Not Ok")
                    //    {
                    //        WorkOrder workorder = new WorkOrder();
                    //        workorder.asset_id = requestModel.asset_id;
                    //        workorder.inspection_id = requestModel.inspection_id;
                    //        workorder.requested_datetime = requestModel.datetime_requested;
                    //        workorder.site_id = requestModel.site_id;
                    //        workorder.attribute_id = attribute.id;
                    //        workorder.checkout_datetime = requestModel.created_at.Value;
                    //        workorder.created_at = requestModel.created_at.Value;
                    //        workorder.created_by = requestModel.manager_id;
                    //        workorder.modified_at = requestModel.created_at.Value;
                    //        workorder.status = (int)Status.New;
                    //        int workorderresult = _UoW.WorkOrderRepository.CreateWorkOrder(workorder);
                    //        if (workorderresult > 0)
                    //        {
                    //            _UoW.SaveChanges();
                    //        }
                    //        else
                    //        {
                    //            _UoW.RollbackTransaction();
                    //            result = false;
                    //            return result;
                    //        }
                    //    }
                    //}
                    inspectionmapper.asset_id = requestModel.asset_id;
                    inspectionmapper.created_at = requestModel.created_at.Value;
                    inspectionmapper.modified_at = requestModel.created_at.Value;
                    var siteDetails = _UoW.SiteRepository.GetSiteById(requestModel.site_id.ToString());
                    if(requestModel.status == (int)Status.Approved && (siteDetails != null && !siteDetails.isAutoApprove))
                    {
                        inspectionmapper.status = (int)Status.Pending;
                        requestModel.status = (int)Status.Pending;
                    }
                    var response = await _UoW.InspectionRepository.Insert(inspectionmapper);
                    if (response > 0)
                    {
                        _UoW.SaveChanges();
                        result = true;
                    }
                    else
                    {
                        _UoW.RollbackTransaction();
                        result = false;
                        return result;
                    }

                    //if (result)
                    //{
                    //    responseModel = _mapper.Map<List<InspectionResponseModel>>(response);
                    //}
                    if (notificationService == null)
                    {
                        notificationService = new NotificationService(_mapper);
                    }

                    if (requestModel.status == (int)Status.Approved)
                    {
                        Asset asset = _UoW.AssetRepository.GetAssetByAssetID(requestModel.asset_id.ToString());
                        asset.asset_id = requestModel.asset_id;
                        asset.notes = requestModel.operator_notes;
                        asset.modified_at = requestModel.created_at.Value;
                        asset.asset_requested_on = requestModel.datetime_requested;
                        asset.asset_requested_by = requestModel.operator_id.ToString();
                        asset.asset_request_status = requestModel.status;
                        asset.asset_approved_by = string.Empty;
                        asset.asset_approved_on = requestModel.created_at.Value;
                        asset.lastinspection_attribute_values = inspectionmapper.attribute_values;
                        asset.meter_hours = requestModel.meter_hours;

                        var updateAsset = await _UoW.AssetRepository.Update(asset);


                        var inspectionformid = await _UoW.InspectionFormRepository.GetInspectionFromByAssetId(requestModel.asset_id);
                        AssetTransactionHistory assetTransactionHistory = new AssetTransactionHistory();
                        assetTransactionHistory.asset_id = requestModel.asset_id;
                        assetTransactionHistory.created_at = requestModel.created_at.Value;
                        assetTransactionHistory.inspection_id = inspectionmapper.inspection_id.ToString();
                        assetTransactionHistory.operator_id = requestModel.operator_id.ToString();
                        assetTransactionHistory.manager_id = requestModel.manager_id;
                        assetTransactionHistory.comapny_id = requestModel.company_id;
                        assetTransactionHistory.site_id = requestModel.site_id.ToString();
                        assetTransactionHistory.attributeValues = assetsValueJsonObjects;
                        assetTransactionHistory.inspection_form_id = inspectionformid.inspection_form_id.ToString();
                        assetTransactionHistory.meter_hours = requestModel.meter_hours;
                        assetTransactionHistory.shift = requestModel.shift;
                        var txnHistoryResult = _UoW.AssetRepository.AddAssetTransactionHistory(assetTransactionHistory);
                        if (txnHistoryResult)
                        {
                            _UoW.SaveChanges();
                            var activityLogs = NotificationGenerator.AssetAutoApprove(asset.name, asset.meter_hours.ToString(), Guid.Parse(GenericRequestModel.site_id));
                            activityLogs.asset_id = requestModel.asset_id;
                            activityLogs.ref_id = inspectionmapper.inspection_id.ToString();
                            activityLogs.created_at = DateTime.UtcNow;
                            activityLogs.activity_type = (int)ActivityTypes.AssetAutoApprove;
                            activityLogs.updated_by = GenericRequestModel.requested_by.ToString();
                            activityLogs.site_id = Guid.Parse(GenericRequestModel.site_id);
                            var res = await _UoW.BaseGenericRepository<AssetActivityLogs>().Update(activityLogs);
                            if (res)
                            {
                                _UoW.SaveChanges();
                            }
                            _dbtransaction.Commit();
                            await notificationService.SendNotification((int)NotificationStatus.AutoApproveInspection, inspectionmapper.inspection_id.ToString(), requestModel.operator_id);
                        }
                    }
                    else
                    {
                        Asset asset = _UoW.AssetRepository.GetAssetByAssetID(requestModel.asset_id.ToString());
                        asset.meter_hours = requestModel.meter_hours;
                        var updateAsset = await _UoW.AssetRepository.Update(asset);

                        if (updateAsset > 0)
                        {
                            _UoW.SaveChanges();
                            var Operator = await _UoW.UserRepository.GetUserByID(requestModel.operator_id);
                            var activityLogs = NotificationGenerator.AssetNewInspection(asset.name, asset.meter_hours.ToString(), Operator.firstname, asset.site_id);
                            activityLogs.asset_id = requestModel.asset_id;
                            activityLogs.created_at = DateTime.UtcNow;
                            activityLogs.updated_by = requestModel.operator_id;
                            activityLogs.ref_id = inspectionmapper.inspection_id.ToString();
                            activityLogs.site_id = Guid.Parse(GenericRequestModel.site_id);
                            var res = await _UoW.BaseGenericRepository<AssetActivityLogs>().Update(activityLogs);
                            if (res == true)
                            {
                                _UoW.SaveChanges();
                            }
                            _dbtransaction.Commit();
                            response = (int)ResponseStatusNumber.Success;
                            await notificationService.SendNotification((int)NotificationStatus.PendingNewInspection, inspectionmapper.inspection_id.ToString(), requestModel.operator_id);
                        }
                    }
                }
                catch (Exception e)
                {
                    _UoW.RollbackTransaction();
                    result = false;
                    throw e;
                }
            }
            return result;

        }

        public async Task<InspectionResponseModel> GetInspectionById(string inspectionid)
        {
            InspectionResponseModel response = new InspectionResponseModel();
            try
            {
                var inspection = _UoW.InspectionRepository.GetInspectionById(inspectionid, GenericRequestModel.requested_by.ToString());
                if (inspection != null && inspection.inspection_id != null && inspection.inspection_id != Guid.Empty)
                {
                    List<CategoryWiseAttributesInsepction> categoryWiseAttributesInsepctions = new List<CategoryWiseAttributesInsepction>();
                    response = _mapper.Map<InspectionResponseModel>(inspection);
                    //Guid operatorguid = Guid.Parse(response.operator_id.ToString());
                    //var name = _UoW.InspectionRepository.FindUserNameById(response.operator_id);
                    //response.operator_name = name;
                    //var resultlist = response.
                    if (response != null)
                    {
                        if (response.manager_id != null && response.manager_id != "")
                        {
                            var user = _UoW.UserRepository.GetUserByUserID(response.manager_id);
                            response.manager_name = user.firstname + " " + user.lastname;
                        }
                        else
                        {
                            response.manager_name = "";
                        }

                        if (response.status == (int)Status.Approved && (response.modified_at != null))
                        {
                            response.approval_date = response.modified_at;
                        }
                        
                    }
                    var results = response.attribute_values.GroupBy(
                                p => p.category_id,
                                (key, g) => new { category_id = key, Attributes = g.ToList() });
                    var list1 = results.ToList();
                    foreach (var item in list1)
                    {
                        CategoryWiseAttributesInsepction categoryWiseAttribute = new CategoryWiseAttributesInsepction();
                        categoryWiseAttribute.category_id = item.category_id;
                        var categoryObject = _UoW.InspectionFormRepository.GetCategoryByID(item.category_id);
                        if (categoryObject != null)
                        {
                            categoryWiseAttribute.name = categoryObject.name;
                            categoryWiseAttribute.category_spanish_name = Jarvis.ViewModels.Utility.PreferLanguageSingleton.Instance.GetLanguageKeyByName(categoryObject.name, (int)Language.spanish).Result;
                        }
                        categoryWiseAttribute.attribute_values = item.Attributes;
                        categoryWiseAttributesInsepctions.Add(categoryWiseAttribute);
                    }
                    response.attributes = categoryWiseAttributesInsepctions;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return response;
        }
        public async Task<InspectionResponseModel> GetInspectionByIdForOperator(string inspectionid, string userid)
        {
            InspectionResponseModel response = new InspectionResponseModel();
            try
            {
                var inspection = _UoW.InspectionRepository.GetInspectionByIdForOperator(inspectionid, userid);
                if (inspection != null && inspection.inspection_id != null && inspection.inspection_id != Guid.Empty)
                {
                    List<CategoryWiseAttributesInsepction> categoryWiseAttributesInsepctions = new List<CategoryWiseAttributesInsepction>();
                    response = _mapper.Map<InspectionResponseModel>(inspection);
                    //Guid operatorguid = Guid.Parse(response.operator_id.ToString());
                    //var name = _UoW.InspectionRepository.FindUserNameById(response.operator_id);
                    //response.operator_name = name;
                    //var resultlist = response.

                    var results = response.attribute_values.GroupBy(
                                p => p.category_id,
                                (key, g) => new { category_id = key, Attributes = g.ToList() });
                    var list1 = results.ToList();
                    foreach (var item in list1)
                    {
                        CategoryWiseAttributesInsepction categoryWiseAttribute = new CategoryWiseAttributesInsepction();
                        categoryWiseAttribute.category_id = item.category_id;
                        var categoryObject = _UoW.InspectionFormRepository.GetCategoryByID(item.category_id);
                        if (categoryObject != null)
                        {
                            categoryWiseAttribute.name = categoryObject.name;
                            categoryWiseAttribute.category_spanish_name = Jarvis.ViewModels.Utility.PreferLanguageSingleton.Instance.GetLanguageKeyByName(categoryObject.name, (int)Language.spanish).Result;
                        }
                        categoryWiseAttribute.attribute_values = item.Attributes;
                        categoryWiseAttributesInsepctions.Add(categoryWiseAttribute);
                    }
                    response.attributes = categoryWiseAttributesInsepctions;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return response;
        }

        public List<InspectionResponseModel> GetPendingInspectionByManager()
        {
            List<InspectionResponseModel> responseModel = new List<InspectionResponseModel>();
            try
            {
                var response = _UoW.AssetRepository.GetAllInspections(GenericRequestModel.requested_by.ToString());
                if (response.Count > 0)
                {
                    responseModel = _mapper.Map<List<InspectionResponseModel>>(response);

                    foreach (var inspection in responseModel)
                    {
                        List<CategoryWiseAttributesInsepction> categoryWiseAttributesInsepctions = new List<CategoryWiseAttributesInsepction>();
                        //Guid operatorguid = Guid.Parse(response.operator_id.ToString());
                        var name = _UoW.InspectionRepository.FindUserNameById(inspection.operator_id);
                        inspection.operator_name = name;
                        //var resultlist = response.

                        var results = inspection.attribute_values.GroupBy(
                                    p => p.category_id,
                                    (key, g) => new { category_id = key, Attributes = g.ToList() });
                        var list1 = results.ToList();
                        foreach (var item in list1)
                        {
                            CategoryWiseAttributesInsepction categoryWiseAttribute = new CategoryWiseAttributesInsepction();
                            categoryWiseAttribute.category_id = item.category_id;
                            var categoryObject = _UoW.InspectionFormRepository.GetCategoryByID(item.category_id);
                            if (categoryObject != null)
                            {
                                categoryWiseAttribute.name = categoryObject.name;
                            }
                            categoryWiseAttribute.attribute_values = item.Attributes;
                            categoryWiseAttributesInsepctions.Add(categoryWiseAttribute);
                        }
                        inspection.attributes = categoryWiseAttributesInsepctions;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }

        public async Task<PendingInspectionCheckoutAssetsManagerResponseModel> PendingInspectionCheckoutAssetsManager()
        {
            PendingInspectionCheckoutAssetsManagerResponseModel responseModel = new PendingInspectionCheckoutAssetsManagerResponseModel();
            try
            {
                var pendinginspection = await _UoW.InspectionRepository.PendingInspection(GenericRequestModel.requested_by.ToString());
                var checkoutassets = await _UoW.InspectionRepository.CheckOutAssets(GenericRequestModel.requested_by.ToString());
                //if (checkoutassets.Count > 0)
                //{
                //responseModel = _mapper.Map<List<PendingInspectionCheckoutAssetsManagerResponseModel>>(checkoutassets);
                responseModel.PendingInspection = _mapper.Map<List<PendingAndCheckoutInspViewModel>>(pendinginspection);
                responseModel.CheckOutAssets = _mapper.Map<List<PendingAndCheckoutInspViewModel>>(checkoutassets);

                var PendingInspectionlist = responseModel.PendingInspection.ToList();

                foreach (var inspection in PendingInspectionlist)
                {
                    if (inspection.attribute_values != null)
                    {
                        List<InspectionAttributesJsonObjectViewModel> newNotOkAttributes = new List<InspectionAttributesJsonObjectViewModel>();

                        var results = inspection.attribute_values.GroupBy(
                                        p => p.category_id,
                                        (key, g) => new { category_id = key, Attributes = g.ToList() });
                        var list1 = results.ToList();

                        foreach (var item in list1)
                        {
                            List<Guid> issues = new List<Guid>();

                            item.Attributes.ToList().ForEach(x => inspection.Asset.lastinspection_attribute_values.ForEach(z =>
                            {
                                if (x.category_id == z.category_id && x.id.ToString() == z.id.ToString())
                                {
                                    Issue workorder = null;

                                    if (workorder == null)
                                    {
                                        workorder = new Issue();
                                    }
                                    workorder = _UoW.IssueRepository.GetAttributeIssueByBarcodeId(x.id.ToString(), inspection.asset_id.ToString());

                                    if (workorder != null)
                                    {
                                        issues.Add(workorder.attribute_id);
                                    }
                                }
                            }));

                            var newnotokattrs = item.Attributes.Where(x => !issues.Contains(x.id)).ToList();
                            CategoryWiseAttributesInsepction categoryWiseAttribute = new CategoryWiseAttributesInsepction();
                            categoryWiseAttribute.category_id = item.category_id;
                            var categoryObject = _UoW.InspectionFormRepository.GetCategoryByID(item.category_id);
                            if (categoryObject != null)
                            {
                                categoryWiseAttribute.name = categoryObject.name;
                            }
                            categoryWiseAttribute.attribute_values = newnotokattrs.Where(x => x.value?.ToLower() == "not ok").ToList();
                            newNotOkAttributes.AddRange(categoryWiseAttribute.attribute_values);
                        }
                        inspection.new_notok_attributes = newNotOkAttributes;
                        //inspection.new_notok_attributes_by_category = categoryWiseAttributesInsepctions;
                    }
                }

                PendingInspectionlist.ToList().ForEach(inspection =>
                {
                    string name = _UoW.InspectionRepository.GetUserNameById(inspection.operator_id);
                    
                    if (name != null && name != string.Empty)
                    {
                        inspection.operator_name = name;
                    }

                    var user = _UoW.UserRepository.GetUserByUserID(inspection.operator_id.ToString());
                    if(user != null)
                    {
                        inspection.operator_firstname = user.firstname != null ? user.firstname : "";
                        inspection.operator_lastname = user.lastname != null ? user.lastname : "";
                    }

                    if (inspection.status == (int)Status.Pending)
                    {
                        inspection.status_name = "Pending";
                    }
                    else if (inspection.status == (int)Status.Approved)
                    {
                        inspection.status_name = "Approved";
                    }

                });

                PendingInspectionlist.ToList().ForEach(x =>
                {
                    x.Asset.Issues = x.Asset.Issues.Where(y => y.status == (int)Status.New || y.status == (int)Status.InProgress || y.status == (int)Status.Waiting).ToList();
                    x.openIssuesCount = x.Asset.Issues.Count();
                });

                responseModel.PendingInspection = PendingInspectionlist;

                var CheckOutAssetslist = responseModel.CheckOutAssets.ToList();

                foreach (var inspection in CheckOutAssetslist)
                {
                    if (inspection.attribute_values != null)
                    {
                        List<InspectionAttributesJsonObjectViewModel> newNotOkAttributes = new List<InspectionAttributesJsonObjectViewModel>();

                        var results = inspection.attribute_values.GroupBy(
                                    p => p.category_id,
                                    (key, g) => new { category_id = key, Attributes = g.ToList() });
                        var list1 = results.ToList();

                        foreach (var item in list1)
                        {
                            List<Guid> issues = new List<Guid>();

                            item.Attributes.ToList().ForEach(x =>
                            {
                                Issue workorder = null;

                                if (workorder == null)
                                {
                                    workorder = new Issue();
                                }
                                workorder = _UoW.IssueRepository.GetAttributeIssueByInspectionId(x.id.ToString(), inspection.inspection_id.ToString());

                                if (workorder != null)
                                {
                                    issues.Add(workorder.attribute_id);
                                }
                            });

                            var newnotokattrs = item.Attributes.Where(x => issues.Contains(x.id)).ToList();
                            CategoryWiseAttributesInsepction categoryWiseAttribute = new CategoryWiseAttributesInsepction();
                            categoryWiseAttribute.category_id = item.category_id;
                            categoryWiseAttribute.attribute_values = newnotokattrs.Where(x => x.value?.ToLower() == "not ok").ToList();
                            newNotOkAttributes.AddRange(categoryWiseAttribute.attribute_values);
                        }
                        inspection.new_notok_attributes = newNotOkAttributes;
                    }
                }

                CheckOutAssetslist.ForEach(x =>
                {
                    string name = _UoW.InspectionRepository.GetUserNameById(x.operator_id);
                    if (name != null && name != string.Empty)
                    {
                        x.operator_name = name;
                    }
                    if (x.status == (int)Status.Pending)
                    {
                        x.status_name = "Pending";
                    }
                    else if (x.status == (int)Status.Approved)
                    {
                        x.status_name = "Approved";
                    }
                });
                responseModel.CheckOutAssets = CheckOutAssetslist;
                responseModel.OutStandingIssuesCount = 0;
                List<Issue> workorders = await _UoW.IssueRepository.GetPendingIssues(GenericRequestModel.requested_by.ToString());
                var group_workorder = workorders.GroupBy(x => x.Asset.site_id).ToList();

                if (group_workorder.Count > 0)
                {
                    group_workorder.ForEach(x =>
                    {
                        Report data = new Report();
                        data.created_at = DateTime.UtcNow;
                        data.modified_at = DateTime.UtcNow;
                        data.site_id = x.Key;
                        var workorder_groupby_asset = x.ToList().GroupBy(y => y.asset_id).ToList();
                        if (workorder_groupby_asset.Count > 0)
                        {
                            responseModel.OutStandingIssuesCount = responseModel.OutStandingIssuesCount + workorder_groupby_asset.Count;
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

        public async Task<ManagerMobileDashboardDataCountResponseModel> ManagerMobileDashboardDataCount()                       //JAR-486
        {
            ManagerMobileDashboardDataCountResponseModel resModel = new ManagerMobileDashboardDataCountResponseModel();
            try
            {
                var pendinginspection = await _UoW.InspectionRepository.PendingInspection(GenericRequestModel.requested_by.ToString());
                var checkoutassets = 0;// await _UoW.InspectionRepository.CheckOutAssets(GenericRequestModel.requested_by.ToString());

                #region Pending Review
                resModel.PendingReview = 0;
                resModel.PendingReview = pendinginspection.Count;
                #endregion

                #region CheckOut Assets
                resModel.CheckOutAssets = 0;
                resModel.CheckOutAssets = 0;
                #endregion

                #region Outstanding Issues
                resModel.OutStandingIssues = 0;
                List<Issue> workorders = await _UoW.IssueRepository.GetPendingIssues(GenericRequestModel.requested_by.ToString());
                var group_workorder = workorders.GroupBy(x => x.Asset.site_id).ToList();

                if (group_workorder.Count > 0)
                {
                    group_workorder.ForEach(x =>
                    {
                        var workorder_groupby_asset = x.ToList().GroupBy(y => y.asset_id).ToList();
                        if (workorder_groupby_asset.Count > 0)
                        {
                            resModel.OutStandingIssues = resModel.OutStandingIssues + workorder_groupby_asset.Count;
                        }
                    });
                }

                resModel.OutStandingIssues = resModel.OutStandingIssues;
                #endregion

                #region Overdue PM Count
                resModel.OverDuePMCount = 0;
                var overduePM = await _UoW.AssetPMsRepository.DashboardPMMetrics();
                resModel.OverDuePMCount = overduePM.overdueCount;
                #endregion

                #region Upcoming PM Count
                resModel.UpcomingPMCount = 0;
                DashboardPendingPMItemsRequestModel data = new DashboardPendingPMItemsRequestModel();
                data.pm_status = 0;
                data.pm_filter_type = 1;
                var upcomingPM = await _UoW.AssetPMsRepository.GetPendingPMItems(data);
                resModel.UpcomingPMCount = upcomingPM.listsize;
                #endregion

                #region Assets Count
                var assets = _UoW.formIORepository.GetAssetsByCompanyID(GenericRequestModel.company_id, GenericRequestModel.requested_by.ToString());
                resModel.AssetsCount = assets.Count;

                #endregion Assets Count

                #region Workorders Count
                NewFlowWorkorderListRequestModel requestModel = new NewFlowWorkorderListRequestModel();
                requestModel.pageindex = 0;
                requestModel.pagesize= 0;
                //requestModel.technician_user_id= GenericRequestModel.requested_by;
                requestModel.wo_status = new List<int> { 73, 13, 69 };
               // if(UpdatedGenericRequestmodel.CurrentUser.role_id == "b22217c2-a932-498c-8ab2-11fc2628104b")
               // {
                    requestModel.technician_user_id = GenericRequestModel.requested_by;
               // }
                var wo = _UoW.WorkOrderRepository.GetAllWorkOrdersNewflow(GenericRequestModel.requested_by.ToString(), requestModel);
                int count = wo.Item1.Where(x => x.status != (int)Status.Completed).Count();
                resModel.WorkOrdersCount = count;
                #endregion Workorders Count

                #region Lines Count

                #endregion Lines Count 

                #region last_sync_date_time
                var get_device_info = _UoW.MobileWorkOrderRepository.GetDeviceInfoByuuid(GenericRequestModel.device_uuid);
                if (get_device_info != null)
                {
                    resModel.last_sync_date = get_device_info.last_sync_time;
                }
                #endregion last_sync_date_time
            }
            catch (Exception e)
            {
                throw e;
            }
            return resModel;
        }
        public async Task<List<PendingAndCheckoutInspViewModel>> PendingInspectionCheckoutAssetsByOperator()
        {
            List<PendingAndCheckoutInspViewModel> responseModel = new List<PendingAndCheckoutInspViewModel>();
            try
            {
                //var pendinginspection = await _UoW.InspectionRepository.PendingInspectionByOperator(userid);
                var checkoutassets = await _UoW.InspectionRepository.CheckOutAssetsByOperator(GenericRequestModel.requested_by.ToString());
                //if (checkoutassets.Count > 0)
                //{
                responseModel = _mapper.Map<List<PendingAndCheckoutInspViewModel>>(checkoutassets);
                //}
                //if (pendinginspection.Count > 0)
                //{
                //responseModel.PendingInspection = _mapper.Map<List<InspectionResponseModel>>(pendinginspection);
                //}

                var responsemodellist = responseModel.ToList();
                //responsemodellist.ToList().ForEach(x =>
                //{
                //    //x.inspection.ForEach(y =>
                //    //{
                //        if (x.status == (int)Status.Pending)
                //        {
                //            x.status_name = "Pending";
                //        }
                //        else if (x.status == (int)Status.Approved)
                //        {
                //            x.status_name = "Approved";
                //        }
                //    //});
                //});

                responseModel = responsemodellist;
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }

        public async Task<int> ApproveInspection(ApproveInspectionRequestModel requestModel)
        {
            int result = (int)ResponseStatusNumber.NotFound;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var alreadypendinginspection = _UoW.InspectionRepository.GetPendingInspection(requestModel.inspection_id.ToString());
                    if (alreadypendinginspection > 0)
                    {
                        DateTime workordercreatedatetime = DateTime.UtcNow;
                        var inspection = await _UoW.InspectionRepository.ApproveInspectionStatus(requestModel);
                        if (inspection != null && inspection.Asset != null)
                        {
                            long workordernumber = _UoW.IssueRepository.GetMaxIssueNumber(inspection.site_id);
                            foreach (var attribute in inspection.attribute_values)
                            {
                                if (attribute.value == "Not Ok")
                                {
                                    if (attribute.date_time != null)
                                    {
                                        bool haveworkorder = _UoW.IssueRepository.HaveAlreadyIssue(attribute.id, inspection.asset_id, attribute.date_time.Value, inspection.created_at);
                                        if (!haveworkorder)
                                        {
                                            Issue workorder = new Issue();
                                            workorder.name = "Issue - " + attribute.name;
                                            if (attribute.category_id == 1)
                                            {
                                                workorder.priority = (int)WorkOrderPriority.Medium;
                                            }
                                            else
                                            {
                                                workorder.priority = (int)WorkOrderPriority.Very_High;
                                            }
                                            workordernumber = workordernumber + 1;
                                            workorder.issue_number = workordernumber;
                                            workorder.asset_id = inspection.asset_id;
                                            workorder.company_id = inspection.company_id;
                                            workorder.inspection_id = requestModel.inspection_id;
                                            workorder.requested_datetime = inspection.datetime_requested.Value;
                                            workorder.site_id = inspection.site_id;
                                            workorder.attribute_id = attribute.id;
                                            workorder.internal_asset_id = inspection.Asset.internal_asset_id;
                                            workorder.checkout_datetime = attribute.date_time.Value;
                                            workorder.created_at = workordercreatedatetime;
                                            workorder.created_by = requestModel.manager_id;
                                            workorder.modified_at = DateTime.UtcNow;
                                            workorder.status = (int)Status.New;
                                            int workorderresult = _UoW.IssueRepository.CreateIssue(workorder);
                                            if (workorderresult > 0)
                                            {
                                                _UoW.SaveChanges();
                                                IssueRecord orderRecord = new IssueRecord();
                                                orderRecord.asset_id = workorder.asset_id;
                                                orderRecord.attrubute_id = workorder.attribute_id;
                                                orderRecord.inspection_id = workorder.inspection_id;
                                                orderRecord.requested_datetime = workorder.requested_datetime;
                                                orderRecord.issue_uuid = workorder.issue_uuid;
                                                orderRecord.created_at = workorder.created_at.Value;
                                                orderRecord.checkout_datetime = workorder.checkout_datetime;
                                                orderRecord.created_by = workorder.created_by;
                                                orderRecord.status = workorder.status;
                                                bool workorderrecordresult = await _UoW.BaseGenericRepository<IssueRecord>().Insert(orderRecord);
                                                if (workorderrecordresult)
                                                {
                                                    _UoW.SaveChanges();
                                                    var activityLogs = NotificationGenerator.NewIssueCreated(inspection.Asset.name, inspection.Asset.meter_hours.Value.ToString(), workorder.name);
                                                    activityLogs.asset_id = inspection.asset_id;
                                                    activityLogs.created_at = DateTime.UtcNow;
                                                    activityLogs.ref_id = workorder.issue_uuid.ToString();
                                                    activityLogs.site_id = inspection.site_id;
                                                    var response = await _UoW.BaseGenericRepository<AssetActivityLogs>().Update(activityLogs);
                                                    if (response == true)
                                                    {
                                                        _UoW.SaveChanges();
                                                    }
                                                }
                                                else
                                                {
                                                    _UoW.RollbackTransaction();
                                                    result = (int)ResponseStatusNumber.False;
                                                    return result;
                                                }
                                            }
                                            else
                                            {
                                                _UoW.RollbackTransaction();
                                                result = (int)ResponseStatusNumber.False;
                                                return result;
                                            }
                                            if (workorderresult > 0)
                                            {
                                                _UoW.SaveChanges();
                                                MaintenanceRequests mrIssueRecord = new MaintenanceRequests();
                                                mrIssueRecord.title = workorder.name;
                                                mrIssueRecord.asset_id = workorder.asset_id;
                                                mrIssueRecord.priority = (int)Status.Low;
                                                mrIssueRecord.mr_type = (int)Status.Inspection;
                                                mrIssueRecord.mr_type_id = workorder.issue_uuid;
                                                mrIssueRecord.requested_by = GenericRequestModel.requested_by;
                                                mrIssueRecord.status = (int)Status.MROpen;
                                                mrIssueRecord.site_id = inspection.site_id;
                                                mrIssueRecord.is_archive = false;
                                                mrIssueRecord.created_at = workorder.created_at.Value;
                                                mrIssueRecord.created_by = workorder.created_by;
                                                int mrresult = await _UoW.MRRepository.Insert(mrIssueRecord);
                                                if (mrresult > 0)
                                                {
                                                    if (workorder.issue_uuid != null)
                                                    {
                                                        var mrIssue = _UoW.IssueRepository.GetIssueByIssueId(workorder.issue_uuid);
                                                        mrIssue.mr_id = mrIssueRecord.mr_id;
                                                        bool updateIssue = await _UoW.IssueRepository.Update(mrIssue);
                                                    }
                                                    _UoW.SaveChanges();
                                                }
                                                else
                                                {
                                                    _UoW.RollbackTransaction();
                                                    result = (int)ResponseStatusNumber.False;
                                                    return result;
                                                }
                                            }
                                            else
                                            {
                                                _UoW.RollbackTransaction();
                                                result = (int)ResponseStatusNumber.False;
                                                return result;
                                            }
                                        }
                                    }
                                }
                            }
                            var asset = inspection.Asset;
                            asset.notes = requestModel.manager_notes;
                            asset.modified_at = DateTime.UtcNow;
                            asset.asset_requested_on = inspection.created_at;
                            asset.asset_requested_by = inspection.operator_id.ToString();
                            asset.asset_request_status = requestModel.status;
                            asset.asset_approved_by = requestModel.manager_id;
                            asset.asset_approved_on = DateTime.UtcNow;
                            asset.lastinspection_attribute_values = inspection.attribute_values;
                            //if (asset.meter_hours < inspection.meter_hours)
                            //{
                            //    asset.meter_hours = inspection.meter_hours;
                            //}
                            var updateAsset = await _UoW.AssetRepository.Update(asset);
                            if (updateAsset > 0)
                            {
                                AssetTransactionHistory assetTransactionHistory = new AssetTransactionHistory();
                                assetTransactionHistory.asset_id = asset.asset_id;
                                assetTransactionHistory.created_at = DateTime.UtcNow;
                                assetTransactionHistory.inspection_id = inspection.inspection_id.ToString();
                                assetTransactionHistory.operator_id = inspection.operator_id.ToString();
                                assetTransactionHistory.manager_id = inspection.manager_id;
                                assetTransactionHistory.comapny_id = asset.company_id;
                                assetTransactionHistory.site_id = asset.site_id.ToString();
                                assetTransactionHistory.attributeValues = inspection.attribute_values;
                                assetTransactionHistory.inspection_form_id = asset.inspectionform_id.ToString();
                                assetTransactionHistory.meter_hours = asset.meter_hours;
                                assetTransactionHistory.shift = inspection.shift;
                                var txnHistoryResult = _UoW.AssetRepository.AddAssetTransactionHistory(assetTransactionHistory);
                                if (txnHistoryResult == true)
                                {
                                    _UoW.SaveChanges();
                                    var activityLogs = NotificationGenerator.AssetAutoApprove(inspection.Asset.name, inspection.Asset.meter_hours.ToString(), asset.site_id);
                                    activityLogs.asset_id = requestModel.asset_id;
                                    activityLogs.ref_id = requestModel.inspection_id.ToString();
                                    activityLogs.created_at = DateTime.UtcNow;
                                    activityLogs.activity_type = (int)ActivityTypes.ManagerAcceptInspection;
                                    activityLogs.updated_by = GenericRequestModel.requested_by.ToString();
                                    activityLogs.site_id = Guid.Parse(GenericRequestModel.site_id);
                                    var res = await _UoW.BaseGenericRepository<AssetActivityLogs>().Update(activityLogs);
                                    if (res)
                                    {
                                        _UoW.SaveChanges();
                                    }
                                    _dbtransaction.Commit();
                                    result = (int)ResponseStatusNumber.Success;
                                    if (notificationService == null)
                                    {
                                        notificationService = new NotificationService(_mapper);
                                    }
                                    await notificationService.SendNotification((int)NotificationStatus.ManagerApproveInspection, requestModel.inspection_id.ToString(), requestModel.manager_id);
                                }
                                else
                                {
                                    _UoW.RollbackTransaction();
                                    result = (int)ResponseStatusNumber.False;
                                }
                            }
                            else
                            {
                                _UoW.RollbackTransaction();
                                result = (int)ResponseStatusNumber.False;
                            }
                            //}
                            //else
                            //{
                            //    result = (int)ResponseStatusNumber.NotFound;
                            //}
                        }
                    }
                    else
                    {
                        result = alreadypendinginspection;
                    }
                }
                catch (Exception e)
                {
                    _UoW.RollbackTransaction();
                    result = (int)ResponseStatusNumber.Error;
                    throw e;
                }
            }
            return result;

        }

        public async Task<ListViewModel<InspectionResponseModel>> GetInspectionByAssetId(string assetid, int pagesize, int pageindex)
        {
            ListViewModel<InspectionResponseModel> responseModel = new ListViewModel<InspectionResponseModel>();
            try
            {
                var response = await _UoW.InspectionRepository.GetInspectionByAssetId(GenericRequestModel.requested_by.ToString(), assetid, pagesize, pageindex);
                if (response.Count > 0)
                {
                    int totalcount = response.Count;
                    if (pageindex > 0 && pagesize > 0)
                    {
                        response.OrderByDescending(x => x.created_at).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    }
                    responseModel.list = _mapper.Map<List<InspectionResponseModel>>(response);
                    responseModel.listsize = totalcount;
                    //responseModel.list = _mapper.Map<List<InspectionResponseModel>>(response);

                    foreach (var inspection in responseModel.list)
                    {
                        List<CategoryWiseAttributesInsepction> categoryWiseAttributesInsepctions = new List<CategoryWiseAttributesInsepction>();
                        var name = _UoW.InspectionRepository.FindUserNameById(inspection.operator_id);
                        inspection.operator_name = name;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }

        public async Task<ListViewModel<InspectionResponseModel>> SearchInspections(string searchstring, string timezone, int pagesize, int pageindex)
        {
            try
            {
                ListViewModel<InspectionResponseModel> responseModel = new ListViewModel<InspectionResponseModel>();

                var inspectionlist = await _UoW.InspectionRepository.SearchInspections(GenericRequestModel.requested_by.ToString(), searchstring, timezone, pagesize, pageindex);

                if (inspectionlist.Count > 0)
                {
                    int totalinspection = inspectionlist.Count;
                    if (pagesize > 0 && pageindex > 0)
                    {
                        inspectionlist = inspectionlist.OrderByDescending(x => x.datetime_requested).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    }
                    responseModel.list = _mapper.Map<List<InspectionResponseModel>>(inspectionlist);
                    responseModel.listsize = totalinspection;
                }
                responseModel.pageIndex = pageindex;
                responseModel.pageSize = pagesize;
                return responseModel;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ListViewModel<InspectionResponseModel>> SearchInspectionsByAsset(string assetid, string searchstring, string timezone, int pagesize, int pageindex)
        {
            try
            {
                ListViewModel<InspectionResponseModel> responseModel = new ListViewModel<InspectionResponseModel>();

                var inspectionlist = await _UoW.InspectionRepository.SearchInspectionsByAsset(GenericRequestModel.requested_by.ToString(), assetid, searchstring, timezone, pagesize, pageindex);
                if (inspectionlist.Count > 0)
                {
                    int inspectionlistcount = inspectionlist.Count;
                    if (pageindex > 0)
                    {
                        inspectionlist.OrderByDescending(x => x.created_at).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    }
                    responseModel.list = _mapper.Map<List<InspectionResponseModel>>(inspectionlist);
                    responseModel.listsize = inspectionlistcount;
                }
                responseModel.pageIndex = pageindex;
                responseModel.pageSize = pagesize;
                return responseModel;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int> CreateInpsectionOffline(InspectionRequestModel inspectionRequestModel)
        {
            try
            {
                int response = (int)ResponseStatusNumber.Error;

                var inspectionForms = await _UoW.InspectionFormRepository.GetInspectionFromByAssetId(inspectionRequestModel.asset_id);

                if (inspectionForms != null && inspectionForms.inspection_form_id != null && inspectionForms.inspection_form_id != Guid.Empty)
                {
                    var inspectionmapper = _mapper.Map<Inspection>(inspectionRequestModel);

                    var attributes = inspectionmapper.attribute_values.ToList();

                    List<AssetsValueJsonObject> assetsValueJsonObjects = new List<AssetsValueJsonObject>();

                    foreach (var insp in attributes)
                    {
                        AssetsValueJsonObject assetsValueJsonObject = new AssetsValueJsonObject();
                        var Attribute = _UoW.InspectionRepository.GetAttributesCategoryFromId(insp.id);
                        if (Attribute != null && Attribute.attributes_id != null)
                        {
                            assetsValueJsonObject.category_id = Attribute.category_id;
                            assetsValueJsonObject.id = insp.id;
                            assetsValueJsonObject.name = insp.name;
                            assetsValueJsonObject.value = insp.value;
                            assetsValueJsonObject.values_type = Attribute.values_type;
                            assetsValueJsonObject.date_time = insp.date_time;
                            assetsValueJsonObjects.Add(assetsValueJsonObject);
                        }
                        else
                        {
                            //_UoW.RollbackTransaction();
                            response = (int)ResponseStatusNumber.Error;
                            return response;
                        }
                    }

                    inspectionmapper.attribute_values = assetsValueJsonObjects;
                    var siteDetails = _UoW.SiteRepository.GetSiteById(inspectionRequestModel.site_id.ToString());
                    if(inspectionRequestModel.status == (int)Status.Approved && (siteDetails != null && !siteDetails.isAutoApprove))
                    {
                        inspectionmapper.status = (int)Status.Pending;
                        inspectionRequestModel.status = (int)Status.Pending;
                    }

                    var request = _mapper.Map<Inspection>(inspectionmapper);
                    _UoW.BeginTransaction();
                    response = await _UoW.InspectionRepository.Insert(request);

                    if (response > 0 && inspectionRequestModel.status == (int)Status.Approved)
                    {
                        Asset asset = _UoW.AssetRepository.GetAssetByAssetID(inspectionRequestModel.asset_id.ToString());
                        asset.asset_id = inspectionRequestModel.asset_id;
                        asset.notes = inspectionRequestModel.operator_notes;
                        asset.modified_at = inspectionRequestModel.created_at;
                        asset.asset_requested_on = inspectionRequestModel.datetime_requested;
                        asset.asset_requested_by = inspectionRequestModel.operator_id.ToString();
                        asset.asset_request_status = inspectionRequestModel.status;
                        asset.asset_approved_by = string.Empty;
                        asset.asset_approved_on = inspectionRequestModel.created_at;
                        asset.modified_by = inspectionRequestModel.operator_id;
                        asset.lastinspection_attribute_values = inspectionmapper.attribute_values;
                        if (asset.meter_hours < inspectionRequestModel.meter_hours)
                        {
                            asset.meter_hours = inspectionRequestModel.meter_hours;
                        }
                        var updateAsset = await _UoW.AssetRepository.Update(asset);

                        if (updateAsset > 0)
                        {
                            _UoW.SaveChanges();
                            var inspectionformid = await _UoW.InspectionFormRepository.GetInspectionFromByAssetId(inspectionRequestModel.asset_id);
                            AssetTransactionHistory assetTransactionHistory = new AssetTransactionHistory();
                            assetTransactionHistory.asset_id = inspectionRequestModel.asset_id;
                            assetTransactionHistory.created_at = inspectionRequestModel.created_at;
                            assetTransactionHistory.inspection_id = inspectionRequestModel.inspection_id.ToString();
                            assetTransactionHistory.operator_id = inspectionRequestModel.operator_id.ToString();
                            assetTransactionHistory.comapny_id = inspectionRequestModel.company_id;
                            assetTransactionHistory.site_id = inspectionRequestModel.site_id.ToString();
                            assetTransactionHistory.attributeValues = asset.lastinspection_attribute_values;
                            assetTransactionHistory.inspection_form_id = inspectionformid.inspection_form_id.ToString();
                            if (asset.meter_hours < inspectionRequestModel.meter_hours)
                            {
                                assetTransactionHistory.meter_hours = inspectionRequestModel.meter_hours;
                            }
                            assetTransactionHistory.shift = inspectionRequestModel.shift;
                            var txnHistoryResult = _UoW.AssetRepository.AddAssetTransactionHistory(assetTransactionHistory);

                            if (txnHistoryResult)
                            {
                                _UoW.SaveChanges();
                                if (record == null)
                                {
                                    record = new SyncRecord(_mapper);
                                }
                                var device_info = _UoW.DeviceRepository.GetDeviceInfoByUUId(GenericRequestModel.device_uuid);
                                if (device_info != null)
                                {
                                    InsertSyncRecordRequestModel syncrecord = new InsertSyncRecordRequestModel();
                                    syncrecord.device_battery_percentage = GenericRequestModel.device_battery_percentage;
                                    syncrecord.device_latitude = GenericRequestModel.device_latitude;
                                    syncrecord.device_longitude = GenericRequestModel.device_longitude;
                                    syncrecord.device_info_id = device_info.device_info_id;
                                    syncrecord.device_uuid = GenericRequestModel.device_uuid;
                                    syncrecord.is_inspection = true;
                                    syncrecord.mac_address = GenericRequestModel.mac_address;
                                    syncrecord.requested_by = GenericRequestModel.requested_by;
                                    syncrecord.request_id = GenericRequestModel.request_id;
                                    string requeststring = Newtonsoft.Json.JsonConvert.SerializeObject(inspectionRequestModel);
                                    syncrecord.request_model = requeststring;
                                    bool recordresult = await record.Insert(syncrecord);
                                    if (!recordresult)
                                    {
                                        _UoW.RollbackTransaction();
                                        response = (int)ResponseStatusNumber.Error;
                                        return response;
                                    }
                                }
                                else
                                {
                                    _UoW.RollbackTransaction();
                                    response = (int)ResponseStatusNumber.DeviceRecordNotFound;
                                    return response;
                                }
                                var activityLogs = NotificationGenerator.AssetAutoApprove(asset.name, asset.meter_hours.ToString(), asset.site_id);
                                activityLogs.asset_id = inspectionRequestModel.asset_id;
                                activityLogs.created_at = DateTime.UtcNow;
                                activityLogs.site_id = asset.site_id;
                                activityLogs.updated_by = GenericRequestModel.requested_by.ToString();
                                activityLogs.activity_type = (int)ActivityTypes.AssetAutoApprove;
                                activityLogs.ref_id = request.inspection_id.ToString();
                                var res = await _UoW.BaseGenericRepository<AssetActivityLogs>().Update(activityLogs);
                                if (res == true)
                                {
                                    _UoW.SaveChanges();
                                }
                                _UoW.CommitTransaction();
                                if (notificationService == null)
                                {
                                    notificationService = new NotificationService(_mapper);
                                }
                                await notificationService.SendNotification((int)NotificationStatus.AutoApproveInspection, inspectionmapper.inspection_id.ToString(), request.operator_id.ToString());
                                response = (int)ResponseStatusNumber.Success;
                            }
                            else
                            {
                                response = (int)ResponseStatusNumber.Error;
                                _UoW.RollbackTransaction();
                            }
                        }
                        else
                        {
                            response = (int)ResponseStatusNumber.Error;
                            _UoW.RollbackTransaction();
                        }
                    }
                    else if (response > 0)
                    {
                        Asset asset = _UoW.AssetRepository.GetAssetByAssetID(inspectionRequestModel.asset_id.ToString());
                        if (asset.meter_hours < inspectionRequestModel.meter_hours)
                        {
                            asset.meter_hours = inspectionRequestModel.meter_hours;
                        }
                        var updateAsset = await _UoW.AssetRepository.Update(asset);
                        if (updateAsset > 0)
                        {
                            _UoW.SaveChanges();
                            if (record == null)
                            {
                                record = new SyncRecord(_mapper);
                            }
                            var device_info = _UoW.DeviceRepository.GetDeviceInfoByUUId(GenericRequestModel.device_uuid);
                            if (device_info != null)
                            {
                                InsertSyncRecordRequestModel syncrecord = new InsertSyncRecordRequestModel();
                                syncrecord.device_battery_percentage = GenericRequestModel.device_battery_percentage;
                                syncrecord.device_latitude = GenericRequestModel.device_latitude;
                                syncrecord.device_longitude = GenericRequestModel.device_longitude;
                                syncrecord.device_info_id = device_info.device_info_id;
                                syncrecord.device_uuid = GenericRequestModel.device_uuid;
                                syncrecord.is_inspection = true;
                                syncrecord.mac_address = GenericRequestModel.mac_address;
                                syncrecord.requested_by = GenericRequestModel.requested_by;
                                syncrecord.request_id = GenericRequestModel.request_id;
                                string requeststring = Newtonsoft.Json.JsonConvert.SerializeObject(inspectionRequestModel);
                                syncrecord.request_model = requeststring;
                                bool recordresult = await record.Insert(syncrecord);
                                if (!recordresult)
                                {
                                    _UoW.RollbackTransaction();
                                    response = (int)ResponseStatusNumber.Error;
                                    return response;
                                }
                                var Operator = await _UoW.UserRepository.GetUserByID(inspectionRequestModel.operator_id);
                                var activityLogs = NotificationGenerator.AssetNewInspection(asset.name, asset.meter_hours.ToString(), Operator.firstname, asset.site_id);
                                activityLogs.asset_id = inspectionRequestModel.asset_id;
                                activityLogs.created_at = DateTime.UtcNow;
                                activityLogs.updated_by = inspectionRequestModel.operator_id;
                                activityLogs.ref_id = inspectionmapper.inspection_id.ToString();
                                activityLogs.site_id = asset.site_id;
                                var res = await _UoW.BaseGenericRepository<AssetActivityLogs>().Update(activityLogs);
                                if (res == true)
                                {
                                    _UoW.SaveChanges();
                                    response = (int)ResponseStatusNumber.Success;
                                }
                            }
                            else
                            {
                                _UoW.RollbackTransaction();
                                response = (int)ResponseStatusNumber.DeviceRecordNotFound;
                                return response;
                            }
                            _UoW.CommitTransaction();
                            if (notificationService == null)
                            {
                                notificationService = new NotificationService(_mapper);
                            }
                            await notificationService.SendNotification((int)NotificationStatus.PendingNewInspection, request.inspection_id.ToString(), request.operator_id.ToString());
                            response = (int)ResponseStatusNumber.Success;
                        }
                        else
                        {
                            response = (int)ResponseStatusNumber.Error;
                            _UoW.RollbackTransaction();
                        }
                    }
                    else
                    {
                        response = (int)ResponseStatusNumber.Error;
                        _UoW.RollbackTransaction();
                    }
                }
                else
                {
                    response = (int)ResponseStatusNumber.NotFound;
                }
                return response;
            }
            catch (Exception e)
            {
                _UoW.RollbackTransaction();
                throw e;
            }
        }

        public async Task<List<string>> UploadbulkInspection(UploadInspectionRequestModel requestModel)
        {
            List<string> result = new List<string>();
            try
            {
                List<AssetsValueJsonObject> assetsValueJsonObjects = new List<AssetsValueJsonObject>();
                Asset asset = new Asset();
                foreach (var insp in requestModel.inspection)
                {
                    assetsValueJsonObjects.Clear();
                    if (asset != null && asset.asset_id != Guid.Empty && asset.asset_id != null)
                    {
                        if (asset.internal_asset_id != insp.internal_asset_id)
                        {
                            asset = _UoW.AssetRepository.GetCompanyAssetsByInternalAssetID(insp.internal_asset_id, requestModel.company_id);
                        }
                    }
                    else
                    {
                        asset = _UoW.AssetRepository.GetCompanyAssetsByInternalAssetID(insp.internal_asset_id, requestModel.company_id);
                    }
                    if (asset != null && asset.asset_id != null && asset.asset_id != Guid.Empty)
                    {
                        Inspection inspection = new Inspection();
                        foreach (var attribute in insp.inspection_form)
                        {
                            AssetsValueJsonObject assetsValueJsonObject = new AssetsValueJsonObject();
                            var attributes = _UoW.InspectionRepository.GetAttributesFromName(attribute.name);
                            if (attributes != null)
                            {
                                assetsValueJsonObject.category_id = attributes.category_id;
                                assetsValueJsonObject.id = attributes.attributes_id;
                                assetsValueJsonObject.name = attribute.name;
                                if (attribute.value == "Y")
                                {
                                    assetsValueJsonObject.value = "OK";
                                }
                                else
                                {
                                    assetsValueJsonObject.value = "Not Ok";
                                }

                                assetsValueJsonObjects.Add(assetsValueJsonObject);
                            }
                            else
                            {
                                //_UoW.RollbackTransaction();
                                //return result;
                            }
                        }

                        inspection.attribute_values = assetsValueJsonObjects;
                        inspection.operator_notes = insp.notes;
                        inspection.operator_id = Guid.Parse(insp.operator_name);
                        inspection.shift = insp.shift;
                        inspection.meter_hours = insp.meter_hours;
                        inspection.created_at = DateTime.ParseExact(insp.inspection_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        inspection.modified_at = DateTime.ParseExact(insp.inspection_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        inspection.status = (int)Status.Approved;
                        inspection.company_id = asset.company_id;
                        inspection.site_id = asset.site_id;
                        inspection.asset_id = asset.asset_id;
                        inspection.datetime_requested = DateTime.ParseExact(insp.inspection_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        _UoW.BeginTransaction();
                        var response = await _UoW.InspectionRepository.Insert(inspection);
                        if (response > 0)
                        {
                            _UoW.SaveChanges();
                            if (asset.meter_hours < insp.meter_hours)
                            {
                                asset.meter_hours = insp.meter_hours;
                                asset.modified_at = DateTime.UtcNow;
                                var updateasset = await _UoW.AssetRepository.Update(asset);
                                _UoW.SaveChanges();
                            }
                            _UoW.CommitTransaction();
                            result.Add("Added SuccessFully " + insp.internal_asset_id);
                        }
                        else
                        {
                            _UoW.RollbackTransaction();
                            result.Add("Not Added SuccessFully " + insp.internal_asset_id);
                        }
                    }
                    else
                    {
                        //_UoW.RollbackTransaction();
                        result.Add("Not Found internal_asset_id " + insp.internal_asset_id);
                    }
                }
            }
            catch (Exception e)
            {
                _UoW.RollbackTransaction();
                throw e;
            }
            return result;
        }

        public int CheckPendingInspection(string assetGuid, string operatorId)
        {
            try
            {
                return _UoW.InspectionRepository.CheckPendingInspection(assetGuid, operatorId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int CheckAssetInspectionByTime(string assetGuid, string operatorId, string requested_datetime)
        {
            try
            {
                return _UoW.InspectionRepository.CheckAssetInspectionByTime(assetGuid, operatorId, requested_datetime);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public MasterDataResponseModel GetMasterData()
        {
            try
            {
                MasterDataResponseModel responseModel = new MasterDataResponseModel();
                var MasterData = _UoW.InspectionRepository.GetMasterData();
                if (MasterData != null)
                {
                    responseModel = _mapper.Map<MasterDataResponseModel>(MasterData);
                }
                return responseModel;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //public async Task SendEmailNotificationForPendingInspection(CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        await ExecuteOnceAsync(cancellationToken);
        //        //while (!cancellationToken.IsCancellationRequested)
        //        //{
        //        //    //Time when method needs to be called
        //        //    var DailyTime = "06:00:00";
        //        //    var timeParts = DailyTime.Split(new char[1] { ':' });

        //        //    var dateNow = DateTime.Now;
        //        //    var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day,
        //        //               int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]));
        //        //    TimeSpan ts;
        //        //    if (date > dateNow)
        //        //        ts = date - dateNow;
        //        //    else
        //        //    {
        //        //        date = date.AddDays(1);
        //        //        ts = date - dateNow;
        //        //    }

        //        //    int d = (int)System.DateTime.Now.DayOfWeek;
        //        //    _logger.LogError("Send Pending Email DayOfWeek: ", d.ToString());
        //        //    if (d != 6 || d != 7)
        //        //    {
        //        //        _logger.LogError("Date of sending email: ", DateTime.UtcNow.ToString());

        //        //    }

        //        //    await Task.Delay(ts, cancellationToken);
        //        //}
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError("Exception Occured Send Email", e.ToString());
        //        //throw e;
        //    }
        //}

        public async Task SendPendingInspectionEmail()
        {
            try
            {
                List<Inspection> inspections = await _UoW.InspectionRepository.FindAllPeningInspection();

                List<UserSites> managers = _UoW.UserRepository.GetAllManager();

                var sitewiseinspection = inspections.GroupBy(x => x.Asset.site_id);

                var sitewisemanager = managers.GroupBy(x => x.user_id).ToList();

                List<ManagerPendingInspectionResponseModel> users = new List<ManagerPendingInspectionResponseModel>();
                sitewisemanager.ForEach(x =>
                {
                    ManagerPendingInspectionResponseModel user = new ManagerPendingInspectionResponseModel();
                    x.ToList().ForEach(z =>
                    {
                        user.user_id = z.User.uuid;
                        user.username = z.User.username;
                        user.email = z.User.email;
                        user.firstname = z.User.firstname;
                        user.lastname = z.User.lastname;
                        user.callbackUrl = z.Sites.Company.domain_name;
                        sitewiseinspection.ToList().ForEach(y =>
                        {
                            if (z.site_id == y.Key)
                            {
                                List<InspectionDetails> inspection = _mapper.Map<List<InspectionDetails>>(y.ToList());

                                if (user.inspections == null)
                                {
                                    user.inspections = inspection;
                                }
                                else
                                {
                                    user.inspections.AddRange(inspection);
                                }
                            }
                        });
                    });
                    users.Add(user);
                });
                users = users.Where(x => x.email != null && x.email != "" && x.inspections != null && x.inspections.Count > 0).ToList();
                if (users.Count > 0)
                {
                    //SendEmail.PendingInspection(users,_logger);
                    PendingInspection(users);
                }
            }
            catch (Exception e)
            {
                ApiException apiException = new ApiException(e);
                var jsonString = JsonConvert.SerializeObject(apiException);
                _logger.LogWarning("Exception in Execute Async: " + jsonString);
            }
        }

        public async Task<InspectionIssueOfflineResponseModel> InspectionIssueOffline(OfflineSyncDataRequestModel requestModel)
        {
            InspectionIssueOfflineResponseModel responses = new InspectionIssueOfflineResponseModel();
            responses.inspections = new List<Guid>();
            responses.issues = new List<Guid>();
            try
            {
                requestModel.inspections.ForEach(x =>
                {
                    try
                    {
                        int assetinspectionatsametime = (int)ResponseStatusNumber.Success;
                        int inspection_response = 0;
                        List<AssetsValueJsonObjectViewModel> attributesJsonObjectViewModel = new List<AssetsValueJsonObjectViewModel>();
                        attributesJsonObjectViewModel = JsonConvert.DeserializeObject<List<AssetsValueJsonObjectViewModel>>(x.attribute_value);
                        x.attribute_values = attributesJsonObjectViewModel.ToArray();
                        InspectionRequestModel inspection = new InspectionRequestModel();
                        inspection = _mapper.Map<InspectionRequestModel>(x);
                        if (inspection.asset_id != null && inspection.asset_id != Guid.Empty && inspection.operator_id != null)
                        {
                            assetinspectionatsametime = CheckAssetInspectionByTime(inspection.asset_id.ToString(), inspection.operator_id, inspection.datetime_requested.ToString("yyyy-MM-dd HH:mm:ss"));
                            _logger.LogInformation("inspection already created response: " + assetinspectionatsametime.ToString() + inspection.asset_id.ToString());
                        }

                        if (assetinspectionatsametime > 0)
                        {
                            Task<int> response = CreateInpsectionOffline(inspection);
                            inspection_response = response.Result;
                        }

                        if (inspection_response > 0 || assetinspectionatsametime == (int)AssetStatus.AlreadyHaveInspection)
                        {
                            responses.inspections.Add(x.uuid);
                        }
                        else
                        {
                            _logger.LogInformation("Error in create inspection: " + inspection_response);
                        }
                    }
                    catch (Exception e)
                    {
                        ApiException apiException = new ApiException(e);
                        var jsonString = JsonConvert.SerializeObject(apiException);
                        _logger.LogInformation("Exception in create inspection offline: " + jsonString);
                    }
                });

                if (issueService == null)
                {
                    issueService = new IssueService(_mapper);
                }
                requestModel.issues.ForEach(x =>
                {
                    try
                    {
                        UpdateIssueByMaintenanceRequestModel workorderrequestmodel = _mapper.Map<UpdateIssueByMaintenanceRequestModel>(x);
                        Task<int> response = issueService.UpdateIssueByMaintenance(workorderrequestmodel);
                        if (response.Result > 0)
                        {
                            responses.issues.Add(x.uuid);
                        }
                        else
                        {
                            _logger.LogInformation("Error in update workorder: " + response);
                        }
                    }
                    catch (Exception e)
                    {
                        ApiException apiException = new ApiException(e);
                        var jsonString = JsonConvert.SerializeObject(apiException);
                        _logger.LogInformation("Exception in create workOrder offline: " + jsonString);
                    }
                });
            }
            catch (Exception e)
            {
                ApiException apiException = new ApiException(e);
                var jsonString = JsonConvert.SerializeObject(apiException);
                _logger.LogInformation("Exception in create inspection workOrder offline: " + jsonString);
                // Do nothing
            }
            return responses;
        }

        public async Task GetOperatorUsageReport()
        {
            List<OperatorUsageWeeklyReportResponseModel> AllManagerOperatorUsage = new List<OperatorUsageWeeklyReportResponseModel>();
            try
            {
                List<UserSites> managers = _UoW.UserRepository.GetAllManagerForOperatorUsageReport();
                var sitewisemanager = managers.GroupBy(x => x.user_id).ToList();
                DayOfWeek weekStart = DayOfWeek.Sunday; // or Sunday, or whenever
                DateTime startingDate = DateTime.Today;
                while (startingDate.DayOfWeek != weekStart)
                    startingDate = startingDate.AddDays(-1);

                DateTime start = startingDate.AddDays(-7);
                DateTime end = startingDate.AddDays(-1);
                DateTime startPSTTime = start.ToUniversalTime();
                DateTime endPSTTime = end.ToUniversalTime();

                _logger.LogInformation("Start Time and End time is " + startPSTTime + " " + endPSTTime);
                sitewisemanager.ForEach(manager =>
                {
                    OperatorUsageWeeklyReportResponseModel operatorUsage = new OperatorUsageWeeklyReportResponseModel();
                    operatorUsage.manager_id = manager.Key.ToString();
                    operatorUsage.manager_name = manager.Where(x => x.user_id == manager.Key).Select(x => x.User.firstname).FirstOrDefault();
                    operatorUsage.fromdate = startPSTTime.ToString("MM/dd/yyyy hh:mm tt");
                    operatorUsage.todate = endPSTTime.ToString("MM/dd/yyyy hh:mm tt");
                    operatorUsage.email_id = manager.Where(x => x.user_id == manager.Key).FirstOrDefault()?.User.email;
                    ListViewModel<AssetOperatorUsageWeeklyReport> responseModel = new ListViewModel<AssetOperatorUsageWeeklyReport>();

                    operatorUsage.SiteWiseReports = new List<SiteWiseReport>();
                    foreach (var sites in manager)
                    {
                        SiteWiseReport siteDetails = new SiteWiseReport();
                        siteDetails.site_id = sites.site_id.ToString();
                        siteDetails.site_name = sites.Sites.site_name;
                        var Operators = _UoW.AssetRepository.GetAllOperatorBySiteList(sites.site_id.ToString());
                        if (Operators?.Count > 0)
                        {
                            siteDetails.operatorWiseReports = new List<OperatorWiseReport>();
                            siteDetails.OperatorWithoutInspection = new OperatorWithoutInspection();
                            siteDetails.OperatorWithoutInspectionList = new List<OperatorWithoutInspection>();
                            Operators.ForEach(x =>
                            {
                                OperatorWiseReport operatorWise = new OperatorWiseReport();
                                var inspections = _UoW.AssetRepository.GetAllOperatorInspectionReportByOperator(x.uuid.ToString());
                                if (inspections?.Count > 0)
                                {
                                    var InspectionByAsset = inspections.GroupBy(x => x.asset_id).ToList();
                                    operatorWise.OperatorUsage = new List<AssetOperatorUsageWeeklyReport>();
                                    InspectionByAsset.ForEach(ins =>
                                    {
                                        AssetOperatorUsageWeeklyReport assetOperatorUsageWeeklyReport = new AssetOperatorUsageWeeklyReport();
                                        assetOperatorUsageWeeklyReport.operator_id = x.uuid.ToString();
                                        assetOperatorUsageWeeklyReport.operator_name = x.firstname + " " + x.lastname;
                                        assetOperatorUsageWeeklyReport.yard = ins.Key.ToString();
                                        assetOperatorUsageWeeklyReport.totalinspection = ins.Count();
                                        assetOperatorUsageWeeklyReport.asset_name = ins.Where(x => x.Asset.asset_id == ins.Key).FirstOrDefault().Asset.name;
                                        assetOperatorUsageWeeklyReport.internal_asset_id = ins.Where(x => x.Asset.asset_id == ins.Key).FirstOrDefault().Asset.internal_asset_id;
                                        var createdAt = ins.OrderByDescending(x => x.created_at).LastOrDefault().created_at;
                                        DateTime createdAtPSTTime = TimeZoneInfo.ConvertTime(createdAt, TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles"));
                                        assetOperatorUsageWeeklyReport.lastinspectiondate = createdAtPSTTime.ToString("MM/dd/yyyy hh:mm tt");
                                        operatorWise.operator_fname = x.firstname;
                                        operatorWise.operator_lname = x.lastname;
                                        operatorWise.operator_id = x.uuid.ToString();
                                        operatorWise.OperatorUsage.Add(assetOperatorUsageWeeklyReport);
                                    });
                                    siteDetails.operatorWiseReports.Add(operatorWise);
                                }
                                else
                                {
                                    // this op has not did any inspection
                                    OperatorWithoutInspection operatorWithout = new OperatorWithoutInspection();
                                    operatorWithout.operator_id = x.uuid.ToString();
                                    operatorWithout.operator_fname = x.firstname;
                                    operatorWithout.operator_lname = x.lastname;
                                    siteDetails.OperatorWithoutInspectionList.Add(operatorWithout);
                                }
                                if (siteDetails.OperatorWithoutInspectionList?.Count > 0)
                                {
                                    siteDetails.OperatorWithoutInspection.operatorsNameList = String.Join(", ", siteDetails.OperatorWithoutInspectionList.Select(x => x.operator_fname + " " + x.operator_lname));
                                }
                            });
                        }
                        operatorUsage.SiteWiseReports.Add(siteDetails);
                    }

                    AllManagerOperatorUsage.Add(operatorUsage);
                });
                if (AllManagerOperatorUsage?.Count > 0)
                {
                    //SendEmail.OperatorUsageReport(AllManagerOperatorUsage, _logger);
                    string callbackUrl = ConfigurationManager.AppSettings["LoginURL"];
                    foreach (var x in AllManagerOperatorUsage)
                    {
                        List<string> sitetablerows = new List<string>();
                        x.SiteWiseReports.ForEach(y =>
                        {
                            if (y.operatorWiseReports?.Count > 0)
                            {
                                y.operatorWiseReports.ForEach(op =>
                                {
                                    if (op.OperatorUsage?.Count > 0)
                                    {
                                        var count = 0;
                                        op.OperatorUsage.ForEach(ins =>
                                        {
                                            if (count == 0)
                                            {
                                                ins.isFirst = true;
                                                ins.totalCount = op.OperatorUsage.Count;
                                            }
                                            count++;
                                        });
                                    }
                                });
                            }
                        });
                        string subject = "Operator Usage Report Email - From " + x.fromdate + " To " + x.todate + "";
                        if (!String.IsNullOrEmpty(x.email_id))
                        {
                            x.callbackUrl = callbackUrl;
                            var templateID = ConfigurationManager.AppSettings["Operator_Usage_Template_ID"];
                            var response = await SendEmail.SendGridEmailWithTemplate(x.email_id, subject, x, templateID);
                            EmailNotificationStatusUpdate emailNotification = new EmailNotificationStatusUpdate();
                            emailNotification.from = ConfigurationManager.AppSettings["SendGrid_Email"];
                            emailNotification.to = x.email_id;
                            emailNotification.submitted_on = DateTime.UtcNow;
                            emailNotification.subject = subject;
                            emailNotification.status = response;
                            var inserted = await _UoW.BaseGenericRepository<EmailNotificationStatusUpdate>().Insert(emailNotification);
                            if (inserted)
                            {
                                _UoW.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task PendingInspection(List<ManagerPendingInspectionResponseModel> users)
        {
            try
            {
                foreach (var x in users)
                {
                    string subject = x.inspections.Count == 1 ? "1 Pending Inspection Review" : x.inspections.Count.ToString() + " Pending Inspection Reviews";
                    if (!String.IsNullOrEmpty(x.email))
                    {
                        //var FileStream = ExcelCreation.WriteExcelFile(x.inspections);
                        var templateID = ConfigurationManager.AppSettings["Pending_Inspections_Template_ID"];
                        //var fileName = "Pending-Inspection-Reviews_" + DateTime.Now.ToShortDateString() + ".xlsx";
                        var response = await SendEmail.SendGridEmailWithTemplate(x.email, subject, x, templateID);
                        EmailNotificationStatusUpdate emailNotification = new EmailNotificationStatusUpdate();
                        emailNotification.from = ConfigurationManager.AppSettings["SendGrid_Email"];
                        emailNotification.to = x.email;
                        emailNotification.submitted_on = DateTime.UtcNow;
                        emailNotification.subject = subject;
                        emailNotification.status = response;
                        var inserted = await _UoW.BaseGenericRepository<EmailNotificationStatusUpdate>().Insert(emailNotification);
                        if (inserted)
                        {
                            _UoW.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ApiException apiException = new ApiException(e);
                var jsonString = JsonConvert.SerializeObject(apiException);
                _logger.LogWarning("Exception in Prepare Email: " + jsonString);
            }
        }

        public async Task GetExecutiveDailyReport()
        {
            List<ExecutiveDailyNewIssuesRequestModel> AllExecutiveReport = new List<ExecutiveDailyNewIssuesRequestModel>();
            try
            {
                List<User> executives = _UoW.UserRepository.GetAllExecutiveForDailyReport();
                DateTime startingDate = DateTime.Today;
                DateTime start = startingDate.AddDays(-1);
                DateTime end = start.AddDays(1);
                DateTime startTime = start;
                DateTime endTime = end;
                //DateTime startTime = TimeZoneInfo.ConvertTime(start, TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles"));
                //DateTime endTime = TimeZoneInfo.ConvertTime(end, TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles"));
                DateTime startPSTTime = startTime.ToUniversalTime();
                DateTime endPSTTime = endTime.ToUniversalTime();

                _logger.LogInformation("Start Time and End time is " + startPSTTime + " " + endPSTTime);
                foreach (var executive in executives)
                {
                    ExecutiveDailyNewIssuesRequestModel executiveDaily = new ExecutiveDailyNewIssuesRequestModel();
                    executiveDaily.firstname = executive.firstname;
                    executiveDaily.lastname = executive.lastname;
                    executiveDaily.username = executive.username;
                    executiveDaily.email = executive.email;
                    executiveDaily.user_id = executive.uuid;
                    List<IssueResponseModel> issues = new List<IssueResponseModel>();
                    if (executive.Usersites?.Count > 0)
                    {
                        var usersites = executive.Usersites?.Where(x => x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (usersites?.Count > 0)
                        {
                            var allissues = _UoW.IssueRepository.GetIssuesForDailyReport(usersites, startPSTTime, endPSTTime);
                            if (allissues?.Count > 0)
                            {
                                issues = _mapper.Map<List<IssueResponseModel>>(allissues);
                            }
                        }
                    }
                    executiveDaily.issues = issues;
                    AllExecutiveReport.Add(executiveDaily);
                }
                SendExecutiveDailyNewIssues(AllExecutiveReport);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task SendExecutiveDailyNewIssues(List<ExecutiveDailyNewIssuesRequestModel> users)
        {
            try
            {
                foreach (var x in users)
                {
                    string subject = "Issues - Progress Report";
                    if (!String.IsNullOrEmpty(x.email))
                    {
                        var templateID = ConfigurationManager.AppSettings["Executive_DailyReport_Template_ID"];
                        var response = await SendEmail.SendGridEmailWithTemplate(x.email, subject, x, templateID);
                        EmailNotificationStatusUpdate emailNotification = new EmailNotificationStatusUpdate();
                        emailNotification.from = ConfigurationManager.AppSettings["SendGrid_Email"];
                        emailNotification.to = x.email;
                        emailNotification.submitted_on = DateTime.UtcNow;
                        emailNotification.subject = subject;
                        emailNotification.status = response;
                        var inserted = await _UoW.BaseGenericRepository<EmailNotificationStatusUpdate>().Insert(emailNotification);
                        if (inserted)
                        {
                            _UoW.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ApiException apiException = new ApiException(e);
                var jsonString = JsonConvert.SerializeObject(apiException);
                _logger.LogWarning("Exception in Prepare Email: " + jsonString);
            }
        }

        public async Task GetExecutiveWeeklyReport()
        {
            List<ExecutiveWeeklyReportRequestModel> AllExecutiveReport = new List<ExecutiveWeeklyReportRequestModel>();
            try
            {
                List<User> executives = _UoW.UserRepository.GetAllExecutiveForWeeklyReport();
                DayOfWeek weekStart = DayOfWeek.Sunday; // or Sunday, or whenever
                DateTime startingDate = DateTime.Today;
                while (startingDate.DayOfWeek != weekStart)
                    startingDate = startingDate.AddDays(-1);

                DateTime start = startingDate.AddDays(-7);
                DateTime end = startingDate.AddDays(-1);
                DateTime startPSTTime = start.ToUniversalTime();
                DateTime endPSTTime = end.ToUniversalTime();

                _logger.LogInformation("Start Time and End time is " + startPSTTime + " " + endPSTTime);
                foreach (var executive in executives)
                {
                    ExecutiveWeeklyReportRequestModel executiveWeekly = new ExecutiveWeeklyReportRequestModel();
                    executiveWeekly.firstname = executive.firstname;
                    executiveWeekly.lastname = executive.lastname;
                    executiveWeekly.username = executive.username;
                    executiveWeekly.email = executive.email;
                    executiveWeekly.fromdate = startPSTTime.ToString("MM/dd/yyyy hh:mm tt");
                    executiveWeekly.todate = endPSTTime.ToString("MM/dd/yyyy hh:mm tt");
                    executiveWeekly.user_id = executive.uuid;
                    executiveWeekly.callbackUrl = executive.Usersites.Select(x => x.Sites.Company).FirstOrDefault()?.domain_name;
                    List<AssetWeeklyReport> assets = new List<AssetWeeklyReport>();
                    // Get Inspections count by Asset and start and end meter hours
                    var totalasset = await _UoW.AssetRepository.GetAssetsWithInspectionForWeeklyEmail(executive.uuid.ToString());
                    if (totalasset.Count > 0)
                    {

                        assets = (from a in totalasset
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
                                      begin_hours = b.Select(x => x.Inspection?.Where(y => y.asset_id == b.Key.asset_id && y.created_at >= startPSTTime && y.created_at < endPSTTime).OrderByDescending(x => x.created_at)?.LastOrDefault()?.meter_hours.ToString())?.LastOrDefault(),
                                      end_hours = b.Select(x => x.Inspection?.Where(y => y.asset_id == b.Key.asset_id && y.created_at >= startPSTTime && y.created_at < endPSTTime).OrderByDescending(x => x.created_at)?.FirstOrDefault()?.meter_hours.ToString())?.FirstOrDefault(),
                                      totalinspection = b.ToList().Select(x => x.Inspection.Where(y => y.created_at >= startPSTTime && y.created_at < endPSTTime).Count()).Sum()
                                  }).ToList();

                        assets = assets.Where(x => x.totalinspection > 0).ToList();

                        assets.ForEach(x =>
                        {
                            var asset = _UoW.AssetRepository.GetAssetByIDFromInternalCall(x.yard);
                            if (asset != null)
                            {
                                x.yard = asset.Sites.site_name;
                            }
                        });

                    }
                    executiveWeekly.assets = assets;
                    List<IssueResponseModel> issues = new List<IssueResponseModel>();
                    if (executive.Usersites?.Count > 0)
                    {
                        var usersites = executive.Usersites?.Where(x => x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (usersites?.Count > 0)
                        {
                            var allissues = _UoW.IssueRepository.GetIssuesForDailyReport(usersites, startPSTTime, endPSTTime);
                            if (allissues?.Count > 0)
                            {
                                issues = _mapper.Map<List<IssueResponseModel>>(allissues);
                            }
                        }
                    }
                    executiveWeekly.issues = issues;

                    // prepare the data for Inspection Pending Reviews
                    List<Inspection> inspections = await _UoW.InspectionRepository.FindAllPeningInspection();
                    var sitewiseinspection = inspections.GroupBy(x => x.Asset.site_id);

                    var exusersites = executive.Usersites?.Where(x => x.status == (int)Status.Active).ToList();
                    if (exusersites?.Count > 0)
                    {
                        exusersites.ForEach(x =>
                        {
                            sitewiseinspection.ToList().ForEach(y =>
                            {
                                if (x.site_id == y.Key)
                                {
                                    List<InspectionDetails> inspection = _mapper.Map<List<InspectionDetails>>(y.ToList());

                                    if (executiveWeekly.inspections == null)
                                    {
                                        executiveWeekly.inspections = inspection;
                                    }
                                    else
                                    {
                                        executiveWeekly.inspections.AddRange(inspection);
                                    }

                                    PendingInspectionsSummary pendingInspectionsSummary = new PendingInspectionsSummary();
                                    pendingInspectionsSummary.site_name = x.Sites.site_name;
                                    pendingInspectionsSummary.pending_reviews = inspection.Count;
                                    pendingInspectionsSummary.time_elapsed = DateTimeUtil.GetBeforetimeText(y.OrderBy(x => x.created_at).Select(x => x.created_at).FirstOrDefault());
                                    executiveWeekly.pendingInspectionsSummary.Add(pendingInspectionsSummary);
                                }
                            });
                        });
                    }
                    // Get pending Inspection Review
                    AllExecutiveReport.Add(executiveWeekly);
                }
                SendExecutiveWeeklyNewIssues(AllExecutiveReport);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task SendExecutiveWeeklyNewIssues(List<ExecutiveWeeklyReportRequestModel> users)
        {
            try
            {
                foreach (var x in users)
                {
                    string subject = "Weekly - Progress Report";
                    if (!String.IsNullOrEmpty(x.email))
                    {
                        var response = String.Empty;
                        var templateID = ConfigurationManager.AppSettings["Executive_WeeklyReport_Template_ID"];
                        if (x.inspections?.Count > 0)
                        {
                            var FileStream = ExcelCreation.WriteExcelFile(x.inspections);
                            var fileName = "Pending-Inspection-Reviews_" + DateTime.Now.ToShortDateString() + ".xlsx";
                            response = await SendEmail.SendGridEmailWithTemplate(x.email, subject, x, templateID, FileStream, fileName);
                        }
                        else
                        {
                            response = await SendEmail.SendGridEmailWithTemplate(x.email, subject, x, templateID);
                        }
                        EmailNotificationStatusUpdate emailNotification = new EmailNotificationStatusUpdate();
                        emailNotification.from = ConfigurationManager.AppSettings["SendGrid_Email"];
                        emailNotification.to = x.email;
                        emailNotification.submitted_on = DateTime.UtcNow;
                        emailNotification.subject = subject;
                        emailNotification.status = response;
                        var inserted = await _UoW.BaseGenericRepository<EmailNotificationStatusUpdate>().Insert(emailNotification);
                        if (inserted)
                        {
                            _UoW.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ApiException apiException = new ApiException(e);
                var jsonString = JsonConvert.SerializeObject(apiException);
                _logger.LogWarning("Exception in Prepare Email: " + jsonString);
            }
        }

        public async Task<ExistingThumbnailResponseModel> GetAllInspectionImages(string awsAccessKey, string awsSecretKey, string bucketName, string folderPathName, int height, int width)
        {
            ListViewModel<ImagesListObjectViewModel> responseModel = new ListViewModel<ImagesListObjectViewModel>();
            ExistingThumbnailResponseModel thumbnailImages = new ExistingThumbnailResponseModel();
            try
            {
                var response = _UoW.InspectionRepository.GetAllInspectionsImages();
                if (response?.Count > 0)
                {
                    responseModel.list = _mapper.Map<List<ImagesListObjectViewModel>>(response);
                    foreach (var imageList in responseModel.list)
                    {
                        if (imageList?.image_names?.Count > 0)
                        {
                            thumbnailImages.originalImages.list.AddRange(imageList.image_names);
                            if (s3BucketService == null)
                            {
                                s3BucketService = new S3BucketService();
                            }
                            var uploadedlist = await s3BucketService.UploadThumbNailImagesForExisting(imageList.image_names, awsAccessKey, awsSecretKey, bucketName, folderPathName, height, width);
                            thumbnailImages.thumbnailImages.list.AddRange(uploadedlist);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return thumbnailImages;
        }
    }
}
