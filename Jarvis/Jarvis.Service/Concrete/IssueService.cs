using AutoMapper;
using Carbon.Json;
using DocumentFormat.OpenXml.Office.CustomUI;
using Jarvis.db.Models;
using Jarvis.Service.Abstract;
using Jarvis.Service.Notification;
using Jarvis.Service.Resources;
using Jarvis.Service.Services;
using Jarvis.Shared;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Xml.Linq;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Jarvis.Service.Concrete
{
    public class IssueService : BaseService, IIssueService
    {
        public readonly IMapper _mapper;


        NotificationService notificationService = null;
        WorkOrderService WOservice = null;
        SyncRecord record = null;
        private readonly IS3BucketService s3BucketService;
        public IssueService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            this.s3BucketService = new S3BucketService();
            this.WOservice = new WorkOrderService(mapper);
        }
 
        public List<IssueResponseModel> GetWorkOrderByAsset(string asset_id, int pagesize, int pageindex)
        {
            List<IssueResponseModel> responseModel = new List<IssueResponseModel>();
            try
            {
                var response = _UoW.IssueRepository.GetIssueByAssetId(asset_id, pagesize, pageindex);
                if (response.Count > 0)
                {
                    responseModel = _mapper.Map<List<IssueResponseModel>>(response);
                }
            }
            catch (Exception e)
            {
                Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public async Task<CreateIssueResponseModel> CreateIssue(IssueRequestModel requestModel)
        {
            CreateIssueResponseModel responseModel = new CreateIssueResponseModel();
            try
            {
                _UoW.BeginTransaction();
                if (requestModel.internal_asset_id != null && requestModel.internal_asset_id != string.Empty)
                {
                    if (requestModel.attachtoinspection)
                    {
                        var inspection = await _UoW.InspectionRepository.GetLastInspectionByInternalAssetId(requestModel.internal_asset_id);
                        if (inspection != null && inspection.inspection_id != null && inspection.inspection_id != Guid.Empty)
                        {
                            requestModel.inspection_id = inspection.inspection_id;
                            requestModel.asset_id = inspection.asset_id;
                            var responsemapper = _mapper.Map<List<AssetsValueJsonObjectViewModel>>(inspection.attribute_values);
                            requestModel.attributes_value = responsemapper.ToArray();
                            requestModel.site_id = inspection.site_id.ToString();
                            requestModel.company_id = inspection.company_id;
                        }
                        else
                        {
                            // not inspection there
                            responseModel.success = false;
                            responseModel.status = (int)ResponseStatusNumber.Error;
                            responseModel.message = ResourceMessages.InspectionNotFound;
                            return responseModel;
                        }
                    }
                    else
                    {
                        var asset = _UoW.AssetRepository.GetAssetsByInternalAssetID(requestModel.internal_asset_id);
                        if (asset != null && asset.asset_id != null && asset.asset_id != Guid.Empty)
                        {
                            requestModel.asset_id = asset.asset_id;
                            requestModel.site_id = asset.site_id.ToString();
                            requestModel.company_id = asset.company_id;
                        }
                        else
                        {
                            // asset are not there
                            responseModel.status = (int)ResponseStatusNumber.Error;
                            responseModel.success = false;
                            responseModel.message = ResourceMessages.AssetNotFound;
                            return responseModel;
                        }
                    }
                }
                //var comment = requestModel.comments.ToList();
                //comment.ForEach(x => x.created_at = DateTime.UtcNow);
                var workordermodel = _mapper.Map<Issue>(requestModel);
                workordermodel.created_at = DateTime.UtcNow;
                workordermodel.modified_at = DateTime.UtcNow;
                workordermodel.created_by = requestModel.userid;
                var response = _UoW.IssueRepository.CreateIssue(workordermodel);
                if (response > 0)
                {
                    _UoW.SaveChanges();
                    if (requestModel.inspection_id != null && requestModel.inspection_id != Guid.Empty)
                    {
                        var asset = _UoW.InspectionRepository.GetInspectionById(requestModel.inspection_id.ToString(), requestModel.userid);
                        if (asset != null && asset.inspection_id != null && asset.inspection_id != Guid.Empty)
                        {
                            var assets = _UoW.AssetRepository.GetAssetByAssetID(requestModel.asset_id.ToString());
                            if (requestModel.isapprove)
                            {
                                assets.status = (int)Status.AssetActive;
                                assets.asset_requested_on = requestModel.created_at;
                                assets.asset_request_status = requestModel.status;
                                assets.asset_approved_by = requestModel.userid;
                                assets.asset_approved_on = DateTime.UtcNow;
                                assets.meter_hours = asset.meter_hours;
                            }
                            else
                            {
                                assets.status = (int)Status.InMaintenanace;
                            }
                            assets.modified_at = DateTime.UtcNow;
                            assets.lastinspection_attribute_values = asset.attribute_values;
                            var updateAsset = await _UoW.AssetRepository.Update(assets);

                            AssetTransactionHistory assetTransactionHistory = new AssetTransactionHistory();
                            assetTransactionHistory.asset_id = asset.asset_id;
                            assetTransactionHistory.created_at = DateTime.UtcNow;
                            assetTransactionHistory.inspection_id = requestModel.inspection_id.ToString();
                            assetTransactionHistory.operator_id = asset.operator_id.ToString();
                            assetTransactionHistory.manager_id = requestModel.userid;
                            assetTransactionHistory.comapny_id = asset.company_id;
                            assetTransactionHistory.site_id = asset.site_id.ToString();
                            assetTransactionHistory.attributeValues = asset.attribute_values;
                            assetTransactionHistory.inspection_form_id = asset.Asset.inspectionform_id.ToString();
                            assetTransactionHistory.meter_hours = asset.meter_hours;
                            assetTransactionHistory.shift = asset.shift;
                            assetTransactionHistory.status = asset.status;
                            var txnHistoryResult = _UoW.AssetRepository.AddAssetTransactionHistory(assetTransactionHistory);

                            if (txnHistoryResult)
                            {
                                //_UoW.CommitTransaction();
                                var inspectionstatusresposne = await _UoW.InspectionRepository.UpdateInspectionStatus(requestModel.inspection_id.Value, requestModel.status, requestModel.isapprove, requestModel.userid);
                                if (inspectionstatusresposne > 0)
                                {
                                    _UoW.SaveChanges();
                                    _UoW.CommitTransaction();
                                    responseModel.success = true;
                                    responseModel.status = (int)ResponseStatusNumber.Success;
                                    responseModel.issueguid = workordermodel.issue_uuid.ToString();

                                    if (notificationService == null)
                                    {
                                        notificationService = new NotificationService(_mapper);
                                    }
                                    if (requestModel.isapprove)
                                    {
                                        await notificationService.SendNotification((int)NotificationStatus.NewWorkOrderWithApprovedAsset, requestModel.inspection_id.ToString(), requestModel.userid);
                                    }
                                    else
                                    {
                                        await notificationService.SendNotification((int)NotificationStatus.NewWorkOrderForInspection, requestModel.inspection_id.ToString(), requestModel.userid);
                                    }
                                }
                                else
                                {
                                    responseModel.success = false;
                                    responseModel.status = (int)ResponseStatusNumber.Error;
                                    responseModel.message = ResourceMessages.Errorinupdateinspectionstatus;
                                    responseModel.issueguid = inspectionstatusresposne.ToString();
                                    _UoW.RollbackTransaction();
                                }
                            }
                            else
                            {
                                responseModel.status = (int)ResponseStatusNumber.Error;
                                responseModel.success = false;
                                responseModel.message = ResourceMessages.ErrorinTranHistory;
                                responseModel.issueguid = response.ToString();
                                _UoW.RollbackTransaction();
                            }
                        }
                        else
                        {
                            responseModel.status = (int)ResponseStatusNumber.NotFound;
                            responseModel.success = false;
                            responseModel.message = ResourceMessages.InspectionNotFound;
                            responseModel.issueguid = response.ToString();
                            _UoW.RollbackTransaction();
                        }
                    }
                    else
                    {
                        _UoW.CommitTransaction();
                        responseModel.status = (int)ResponseStatusNumber.Success;
                        responseModel.success = true;
                        responseModel.issueguid = workordermodel.issue_uuid.ToString();
                        //workorderguid = workordermodel.work_order_uuid.ToString();
                    }
                }
                else
                {
                    responseModel.status = (int)ResponseStatusNumber.Error;
                    responseModel.success = false;
                    responseModel.message = ResourceMessages.Errorincreateissue;
                    responseModel.issueguid = response.ToString();
                    _UoW.RollbackTransaction();
                }
            }
            catch (Exception e)
            {
                responseModel.status = (int)ResponseStatusNumber.Error;
                _UoW.RollbackTransaction();
                throw e;
            }
            return responseModel;
        }

        public async Task<int> UpdateIssueByManager(UpdateIssueRequestModel requestModel)
        {
            try
            {
                int result;
                DateTime updated_datetime = new DateTime();
                try
                {
                    updated_datetime = DateTime.ParseExact(requestModel.updated_at, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }
                catch
                {
                    // do nothing;
                    result = (int)ResponseStatusNumber.DateRequired;
                    return result;
                }
                List<CommentJsonObject> comments = new List<CommentJsonObject>();
                if (requestModel.comment?.Count > 0)
                {
                    requestModel.comment.ForEach(x =>
                    {
                        CommentJsonObject comment = new CommentJsonObject();
                        x.created_by = requestModel.userid;
                        x.created_at = DateTime.UtcNow;
                        comment = _mapper.Map<CommentJsonObject>(x);
                        comments.Add(comment);
                    });
                }
                _UoW.BeginTransaction();
                var workorder = _UoW.IssueRepository.GetIssueByIssueId(requestModel.issue_uuid);
                if (workorder != null && workorder.issue_uuid != null && workorder.issue_uuid != Guid.Empty)
                {
                    //bool updatestatus = false;
                    workorder.name = requestModel.title;
                    workorder.notes = requestModel.notes;
                    workorder.priority = requestModel.priority;
                    workorder.modified_at = DateTime.UtcNow;
                    workorder.modified_by = requestModel.userid;
                    workorder.updated_at = updated_datetime;
                    workorder.Inspection.modified_at = DateTime.UtcNow;
                    workorder.Inspection.modified_by = requestModel.userid;
                    workorder.Asset.modified_at = DateTime.UtcNow;
                    workorder.Asset.modified_by = requestModel.userid;
                    if (workorder.status > 0 && workorder.status != requestModel.status)
                    {
                        workorder.status = requestModel.status;
                        //updatestatus = true;
                        workorder.IssueRecord.status = requestModel.status;
                        if (requestModel.status == (int)Status.Completed)
                        {
                            workorder.IssueRecord.fixed_datetime = DateTime.UtcNow;
                            workorder.IssueRecord.fixed_by = requestModel.userid;
                        }
                    }
                    if (comments.Count > 0)
                    {
                        if (workorder.comments == null)
                        {
                            workorder.comments = comments;
                        }
                        else
                        {
                            workorder.comments.AddRange(comments);
                        }
                    }
                    //_UoW.BaseGenericRepository<>().Update()
                    bool updateresult = await _UoW.IssueRepository.Update(workorder);
                    if (updateresult)
                    {
                        IssueStatus workOrderStatus = new IssueStatus();
                        workOrderStatus.issue_id = workorder.issue_uuid;
                        workOrderStatus.status = workorder.status;
                        workOrderStatus.modified_at = DateTime.UtcNow;
                        workOrderStatus.modified_by = requestModel.userid;

                        var response = _UoW.IssueRepository.CreateIssueStatus(workOrderStatus);
                        if (response > 0)
                        {
                            _UoW.SaveChanges();
                            if (requestModel.status == (int)Status.Completed)
                            {
                                var activityLogs = NotificationGenerator.IssueResolved(workorder.Asset.name, workorder.Asset.meter_hours.Value.ToString(), workorder.name);
                                activityLogs.asset_id = workorder.asset_id;
                                activityLogs.created_at = DateTime.UtcNow;
                                activityLogs.ref_id = workorder.issue_uuid.ToString();
                                activityLogs.updated_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                activityLogs.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                                var res = await _UoW.BaseGenericRepository<AssetActivityLogs>().Update(activityLogs);
                                if (res == true)
                                {
                                    _UoW.SaveChanges();
                                }
                            }
                            _UoW.CommitTransaction();
                            //if (notificationService == null)
                            //{
                            //    notificationService = new NotificationService(_mapper);
                            //}
                            //if (updatestatus)
                            //{
                            //    await notificationService.SendNotification((int)NotificationStatus.UpdateWorkOrderStatus, requestModel.work_order_uuid.ToString(), requestModel.userid);
                            //}
                            result = (int)ResponseStatusNumber.Success;
                        }
                        else
                        {
                            _UoW.RollbackTransaction();
                            result = (int)ResponseStatusNumber.Error;
                        }
                    }
                    else
                    {
                        _UoW.RollbackTransaction();
                        result = (int)ResponseStatusNumber.Error;
                    }
                }
                else
                {
                    result = (int)ResponseStatusNumber.NotFound;
                }
                return result;
            }
            catch (Exception e)
            {
                _UoW.CommitTransaction();
                throw e;
            }
        }

        public async Task<int> UpdateIssueByMaintenance(UpdateIssueByMaintenanceRequestModel requestModel)
        {
            try
            {
                int result;
                DateTime updated_datetime = new DateTime();
                try
                {
                    updated_datetime = DateTime.ParseExact(requestModel.updated_at, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }
                catch
                {
                    // do nothing;
                    result = (int)ResponseStatusNumber.DateRequired;
                    return result;
                }
                List<CommentJsonObject> comments = new List<CommentJsonObject>();
                _UoW.BeginTransaction();
                var workorder = _UoW.IssueRepository.GetIssueByIssueId(requestModel.issue_uuid);

                if (requestModel.comment.Count > 0)
                {
                    requestModel.comment.ForEach(x =>
                    {
                        CommentJsonObject comment = new CommentJsonObject();
                        x.created_by = requestModel.userid;
                        x.created_at = DateTime.UtcNow;
                        comment = _mapper.Map<CommentJsonObject>(x);
                        comments.Add(comment);
                    });
                }

                if (workorder != null && workorder.issue_uuid != null && workorder.issue_uuid != Guid.Empty)
                {
                    if (workorder.status != (int)Status.Completed)
                    {
                        bool updatestatus = false;
                        if (workorder.status != requestModel.status)
                        {
                            updatestatus = true;
                            workorder.status = requestModel.status;
                            workorder.IssueRecord.status = requestModel.status;
                            if (requestModel.status == (int)Status.Completed)
                            {
                                workorder.IssueRecord.fixed_datetime = DateTime.UtcNow;
                                workorder.IssueRecord.fixed_by = requestModel.userid;

                                var activityLogs = NotificationGenerator.IssueResolved(workorder.Asset.name, workorder.Asset.meter_hours.Value.ToString(), workorder.name);
                                activityLogs.asset_id = workorder.asset_id;
                                activityLogs.created_at = DateTime.UtcNow;
                                activityLogs.ref_id = workorder.issue_uuid.ToString();
                                activityLogs.updated_by = requestModel.userid;
                                activityLogs.site_id = workorder.Asset.site_id;
                                var res = await _UoW.BaseGenericRepository<AssetActivityLogs>().Update(activityLogs);
                            }
                        }
                        workorder.modified_at = DateTime.UtcNow;
                        workorder.modified_by = requestModel.userid;
                        workorder.updated_at = updated_datetime;
                        workorder.Asset.modified_at = DateTime.UtcNow;
                        workorder.Asset.modified_by = requestModel.userid;
                        workorder.Inspection.modified_at = DateTime.UtcNow;
                        workorder.Inspection.modified_by = requestModel.userid;
                        workorder.maintainence_staff_id = requestModel.userid;
                        if (comments.Count > 0)
                        {
                            if(workorder.comments == null)
                            {
                                workorder.comments = comments;
                            }
                            else
                            {
                                workorder.comments.AddRange(comments);
                            }
                        }
                        //else
                        //{
                        //    List<CommentJsonObject> comments = new List<CommentJsonObject>();
                        //    comments.Add(comment);
                        //    workorder.comments = comments;
                        //}

                        IssueStatus workOrderStatus = new IssueStatus();
                        workOrderStatus.issue_id = workorder.issue_uuid;
                        workOrderStatus.status = workorder.status;
                        workOrderStatus.modified_at = DateTime.UtcNow;
                        workOrderStatus.modified_by = requestModel.userid;

                        var response = _UoW.IssueRepository.CreateIssueStatus(workOrderStatus);
                        if (response > 0)
                        {
                            _UoW.SaveChanges();

                            if(record == null && requestModel.is_sync)
                            {
                                record = new SyncRecord(_mapper);
                            }

                            if (requestModel.is_sync)
                            {
                                var device_info = _UoW.DeviceRepository.GetDeviceInfoByUUId(UpdatedGenericRequestmodel.CurrentUser.device_uuid);
                                if(device_info != null)
                                {
                                    InsertSyncRecordRequestModel syncrecord = new InsertSyncRecordRequestModel();
                                    syncrecord.device_battery_percentage = UpdatedGenericRequestmodel.CurrentUser.device_battery_percentage;
                                    syncrecord.device_latitude = UpdatedGenericRequestmodel.CurrentUser.device_latitude;
                                    syncrecord.device_longitude = UpdatedGenericRequestmodel.CurrentUser.device_longitude;
                                    syncrecord.device_info_id = device_info.device_info_id;
                                    syncrecord.device_uuid = UpdatedGenericRequestmodel.CurrentUser.device_uuid;
                                    syncrecord.is_workorder = true;
                                    syncrecord.mac_address = UpdatedGenericRequestmodel.CurrentUser.mac_address;
                                    syncrecord.requested_by = UpdatedGenericRequestmodel.CurrentUser.requested_by;
                                    syncrecord.request_id = UpdatedGenericRequestmodel.CurrentUser.request_id;
                                    string requeststring = Newtonsoft.Json.JsonConvert.SerializeObject(requestModel);
                                    syncrecord.request_model = requeststring;
                                    bool recordresult = await record.Insert(syncrecord);
                                    if (!recordresult)
                                    {
                                        _UoW.RollbackTransaction();
                                        result = (int)ResponseStatusNumber.Error;
                                        return result;
                                    }
                                }
                                else
                                {
                                    _UoW.RollbackTransaction();
                                    result = (int)ResponseStatusNumber.DeviceRecordNotFound;
                                    return result;
                                }

                            }
                            _UoW.CommitTransaction();
                            bool workorderresult = await _UoW.IssueRepository.Update(workorder);
                            if (workorderresult)
                            {
                                if (notificationService == null)
                                {
                                    notificationService = new NotificationService(_mapper);
                                }
                                //_UoW.CommitTransaction();
                                if (updatestatus)
                                {
                                    await notificationService.SendNotification((int)NotificationStatus.UpdateWorkOrderStatus, requestModel.issue_uuid.ToString(), requestModel.userid);
                                }
                                result = (int)ResponseStatusNumber.Success;
                            }
                            else
                            {
                                _UoW.RollbackTransaction();
                                result = (int)ResponseStatusNumber.Error;
                            }
                        }
                        else
                        {
                            _UoW.RollbackTransaction();
                            result = (int)ResponseStatusNumber.Error;
                        }
                    }
                    else
                    {
                        result = (int)ResponseStatusNumber.Exceeded;
                    }
                }
                else
                {
                    result = (int)ResponseStatusNumber.NotFound;
                }
                return result;
            }
            catch (Exception e)
            {
                _UoW.CommitTransaction();
                throw e;
            }
        }

        public ListViewModel<IssueResponseModel> GetAllIssues(int status, int pagesize, int pageindex)
        {
            ListViewModel<IssueResponseModel> responseModel = new ListViewModel<IssueResponseModel>();
            try
            {
                var response = _UoW.IssueRepository.GetIssues(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), status, pagesize, pageindex);
                if (response.Count > 0)
                {
                    int totalworkorder = response.Count;
                    if (pageindex > 0 && pagesize > 0)
                    {
                        response = response.OrderByDescending(g => g.created_at.Value).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    }

                    responseModel.list = _mapper.Map<List<IssueResponseModel>>(response);

                    foreach (var workorder in responseModel.list)
                    {
                        var assetname = _UoW.AssetRepository.GetAssetByAssetID(workorder.asset_id.ToString());
                        if (assetname != null && assetname.asset_id != null && assetname.asset_id != Guid.Empty)
                        {
                            workorder.asset_name = assetname.name;
                        }
                    }
                    responseModel.listsize = totalworkorder;
                    responseModel.pageIndex = pageindex;
                    responseModel.pageSize = pagesize;
                }
                responseModel.result = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public ListViewModel<IssueResponseModel> FilterIssues(FilterIssueRequestModel requestModel)
        {
            ListViewModel<IssueResponseModel> responseModel = new ListViewModel<IssueResponseModel>();
            try
            {
                var response = _UoW.IssueRepository.FilterIssues(requestModel);
                if (response.Count > 0)
                {
                    int totalworkorder = response.Count;
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pageindex = 1;
                        requestModel.pagesize = 20;
                    }
                    response = response.OrderByDescending(g => g.created_at.Value).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                    responseModel.list = _mapper.Map<List<IssueResponseModel>>(response);

                    foreach (var workorder in responseModel.list)
                    {
                        var assetname = _UoW.AssetRepository.GetAssetByAssetID(workorder.asset_id.ToString());
                        if (assetname != null && assetname.asset_id != null && assetname.asset_id != Guid.Empty)
                        {
                            workorder.asset_name = assetname.name;
                        }
                    }
                    responseModel.listsize = totalworkorder;
                    responseModel.pageIndex = requestModel.pageindex;
                    responseModel.pageSize = requestModel.pagesize;
                }
                responseModel.result = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                Logger.Log("Error in Get All WorkOrders " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public ListViewModel<IssuesNameResponseModel> FilterIssuesTitleOption(FilterIssueRequestModel requestModel)
        {
            ListViewModel<IssuesNameResponseModel> responseModel = new ListViewModel<IssuesNameResponseModel>();
            try
            {
                var response = _UoW.IssueRepository.FilterIssuesTitleOption(requestModel);
                if (response.Count > 0)
                {
                    int totalworkorder = response.Count;
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pageindex = 1;
                        requestModel.pagesize = 20;
                    }
                    response = response.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                    responseModel.list = _mapper.Map<List<IssuesNameResponseModel>>(response);
                    responseModel.listsize = totalworkorder;
                    responseModel.pageIndex = requestModel.pageindex;
                    responseModel.pageSize = requestModel.pagesize;
                }
                responseModel.result = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                Logger.Log("Error in Get All WorkOrders " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public ListViewModel<AssetListResponseModel> FilterIssuesAssetOption(FilterIssueRequestModel requestModel)
        {
            ListViewModel<AssetListResponseModel> responseModel = new ListViewModel<AssetListResponseModel>();
            try
            {
                var response = _UoW.IssueRepository.FilterIssuesAssetOption(requestModel);
                if (response.Count > 0)
                {
                    var assets = response.Select(x => x.Asset).Distinct().ToList();
                    if(assets.Count > 0)
                    {
                        responseModel.listsize = assets.Count;
                        if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                        {
                            requestModel.pageindex = 1;
                            requestModel.pagesize = 20;
                        }
                        assets = assets.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                        responseModel.list = _mapper.Map<List<AssetListResponseModel>>(assets);
                        responseModel.pageIndex = requestModel.pageindex;
                        responseModel.pageSize = requestModel.pagesize;
                    }
                }
                responseModel.result = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                Logger.Log("Error in Get All WorkOrders " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public ListViewModel<SitesViewModel> FilterIssuesSitesOption(FilterIssueRequestModel requestModel)
        {
            ListViewModel<SitesViewModel> responseModel = new ListViewModel<SitesViewModel>();
            try
            {
                var response = _UoW.IssueRepository.FilterIssuesSitesOption(requestModel);
                if (response.Count > 0)
                {
                    var sites = response.Select(x => x.Sites).Distinct().ToList();
                    if (sites.Count > 0)
                    {
                        responseModel.listsize = sites.Count;
                        if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                        {
                            requestModel.pageindex = 1;
                            requestModel.pagesize = 20;
                        }
                        sites = sites.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                        responseModel.list = _mapper.Map<List<SitesViewModel>>(sites);
                        responseModel.pageIndex = requestModel.pageindex;
                        responseModel.pageSize = requestModel.pagesize;
                    }
                }
                responseModel.result = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                Logger.Log("Error in Get All WorkOrders " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public ListViewModel<IssuesNameResponseModel> GetAllIssuesTitle(string siteid)
        {
            ListViewModel<IssuesNameResponseModel> responseModel = new ListViewModel<IssuesNameResponseModel>();
            try
            {
                var response = _UoW.IssueRepository.GetIssuesTitle(siteid);
                if (response.Count > 0)
                {
                    int totalworkorder = response.Count;
                    response = response.OrderByDescending(g => g.created_at.Value).ToList();
                    
                    responseModel.list = _mapper.Map<List<IssuesNameResponseModel>>(response);

                    responseModel.listsize = totalworkorder;
                }
                responseModel.result = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public async Task<IssueResponseModel> GetIssueByID(string workOrderId)
        {
            IssueResponseModel workOrder = new IssueResponseModel();
            try
            {
                var response = _UoW.IssueRepository.GetIssueById(workOrderId, UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (response != null)
                {
                    workOrder = _mapper.Map<IssueResponseModel>(response);

                    //foreach (var workOrder in responseModel)
                    //{

                    // New Comment
                    if (workOrder.asset_id != null && workOrder.asset_id != Guid.Empty)
                    {
                        var asset = _UoW.AssetRepository.GetAssetByAssetID(workOrder.asset_id.ToString());
                        var assetresponse = _mapper.Map<AssetsResponseModel>(asset);

                        //if (assetresponse != null)
                        //{
                        //    //var workOrders = _UoW.WorkOrderRepository.GetWorkOrderByAssetId(assetresponse.asset_id.ToString(), pagesize,  pageindex);
                        //    //if (workOrders != null)
                        //    //{
                        //    //    var workOrderResponse = _mapper.Map<List<WorkOrderResponseModel>>(workOrders);
                        //    //    assetresponse.WorkOrders = workOrderResponse;
                        //    //}
                        //}
                        workOrder.assets = assetresponse;
                        workOrder.asset_name = assetresponse.name;
                    }

                    if (workOrder.inspection_id != null && workOrder.inspection_id != Guid.Empty)
                    {
                        var inspection = _UoW.InspectionRepository.GetInspectionById(workOrder.inspection_id.ToString(),UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                        var inspectionResponse = _mapper.Map<InspectionResponseModel>(inspection);
                        workOrder.inspections = inspectionResponse;
                    }


                    /// get created by name who created comment
                    if (workOrder.comments != null && workOrder.comments.Count > 0)
                    {
                        foreach (var comment in workOrder.comments)
                        {
                            if (comment.created_by != string.Empty && comment.created_by != null)
                            {
                                var createduser = await _UoW.UserRepository.GetUserByID(comment.created_by);
                                if (createduser != null)
                                {
                                    comment.created_by_name = createduser.firstname + " " + createduser.lastname;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return workOrder;
        }

        public ListViewModel<IssueResponseModel> GetTodayNewIssues(int pagesize, int pageindex)
        {
            ListViewModel<IssueResponseModel> responseModel = new ListViewModel<IssueResponseModel>();
            try
            {
                var response = _UoW.IssueRepository.GetTodayNewIssues(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), pagesize, pageindex);
                if (response.Count > 0)
                {
                    responseModel.list = _mapper.Map<List<IssueResponseModel>>(response);
                    foreach (var workorder in responseModel.list)
                    {
                        var assetname = _UoW.AssetRepository.GetAssetByAssetID(workorder.asset_id.ToString());
                        if (assetname != null && assetname.asset_id != null && assetname.asset_id != Guid.Empty)
                        {
                            workorder.asset_name = assetname.name;
                        }
                    }
                    responseModel.pageIndex = pageindex;
                    responseModel.pageSize = pagesize;
                }
                responseModel.result = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                Logger.Log("Error in Get All Assets by today date " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public ListViewModel<IssueResponseModel> GetIssueByAssetId(string assetid, int pagesize, int pageindex)
        {
            ListViewModel<IssueResponseModel> responseModel = new ListViewModel<IssueResponseModel>();
            try
            {
                var response = _UoW.IssueRepository.GetIssueByAssetId(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), assetid, pagesize, pageindex);
                if (response.Count > 0)
                {
                    int totalcount = response.Count;
                    if (pageindex > 0 && pagesize > 0)
                    {
                        response = response.OrderByDescending(x => x.created_at).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    }
                    responseModel.list = _mapper.Map<List<IssueResponseModel>>(response);
                    responseModel.listsize = totalcount;
                    //responseModel.list = _mapper.Map<List<WorkOrderResponseModel>>(response);

                    responseModel.list.ForEach(x =>
                    {
                        if (x.status == (int)Status.New)
                        {
                            x.status_name = "New";
                        }
                        else if (x.status == (int)Status.InProgress)
                        {
                            x.status_name = "InProgress";
                        }
                        else if (x.status == (int)Status.Waiting)
                        {
                            x.status_name = "Waiting";
                        }
                        else if (x.status == (int)Status.Completed)
                        {
                            x.status_name = "Completed";
                        }
                    });
                    //foreach (var workorder in responseModel.list)
                    //{
                    //    var assetname = _UoW.AssetRepository.GetAssetByAssetID(workorder.asset_id.ToString());
                    //    if (assetname != null && assetname.asset_id != null && assetname.asset_id != Guid.Empty)
                    //    {
                    //        workorder.asset_name = assetname.name;
                    //    }
                    //}
                    responseModel.pageIndex = pageindex;
                    responseModel.pageSize = pagesize;
                }
                responseModel.result = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                Logger.Log("Error in Get All Assets by today date " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public ListViewModel<IssueResponseModel> SearchIssues(string searchstring, string timezone, int pagesize, int pageindex)
        {
            ListViewModel<IssueResponseModel> responseModel = new ListViewModel<IssueResponseModel>();
            try
            {
                var response = _UoW.IssueRepository.SearchIssues(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), searchstring, timezone, pagesize, pageindex);
                if (response.Count > 0)
                {
                    int totalworkorder = response.Count;
                    if (pagesize > 0 && pageindex > 0)
                    {
                        response = response.OrderByDescending(g => g.created_at.Value).Skip((pageindex - 1) * pagesize).Take(pagesize).OrderByDescending(x => x.created_at).ToList();
                    }
                    responseModel.list = _mapper.Map<List<IssueResponseModel>>(response);
                    responseModel.listsize = totalworkorder;
                    foreach (var workorder in responseModel.list)
                    {
                        var assetname = _UoW.AssetRepository.GetAssetByAssetID(workorder.asset_id.ToString());
                        if (assetname != null && assetname.asset_id != null && assetname.asset_id != Guid.Empty)
                        {
                            workorder.asset_name = assetname.name;
                        }
                        else
                        {
                            workorder.asset_name = string.Empty;
                            workorder.internal_asset_id = string.Empty;
                        }
                    }

                    //var searchStringLower = searchstring.ToLower();

                    //if ("high".Contains(searchStringLower))
                    //{
                    //    searchStringLower = "2";
                    //}
                    //if ("veryhigh".Contains(searchStringLower) || "very high".Contains(searchStringLower))
                    //{
                    //    searchStringLower = "1";
                    //}
                    //else if ("low".Contains(searchStringLower))
                    //{
                    //    searchStringLower = "4";
                    //}
                    //else if ("medium".Contains(searchStringLower))
                    //{
                    //    searchStringLower = "3";
                    //}

                    //var responseList = responseModel.list;

                    ////responseModel.list = responseList.Where(x => x.asset_name.ToLower().Contains(searchStringLower) || x.name.ToLower().Contains(searchStringLower)
                    ////                    || x.status_name.ToLower().Contains(searchStringLower) || x.priority.ToString() == searchStringLower
                    ////                    ).ToList();

                    //if(pageindex > 0)
                    //{
                    //    responseModel.list = responseList.OrderByDescending(g => g.created_at.Value).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    //}

                    responseModel.pageIndex = pageindex;
                    responseModel.pageSize = pagesize;
                }
                responseModel.result = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public ListViewModel<IssueResponseModel> SearchIssueByAssetId(string assetid, string searchstring, int pagesize, int pageindex)
        {
            ListViewModel<IssueResponseModel> responseModel = new ListViewModel<IssueResponseModel>();
            try
            {
                var response = _UoW.IssueRepository.SearchIssueByAssetId(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), assetid, searchstring, pagesize, pageindex);
                if (response.Count > 0)
                {
                    int totalworkorder = response.Count;

                    if (pageindex > 0 && pagesize > 0)
                    {
                        response = response.OrderByDescending(g => g.created_at.Value).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    }
                    responseModel.list = _mapper.Map<List<IssueResponseModel>>(response);
                 
                    responseModel.pageIndex = pageindex;
                    responseModel.pageSize = pagesize;
                    responseModel.listsize = totalworkorder;
                }
                responseModel.result = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                Logger.Log("Error in Get All Assets by today date " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public ListViewModel<AssetIssueResponseModel> GetAssetsIssue(int pagesize, int pageindex)
        {
            try
            {
                ListViewModel<AssetIssueResponseModel> responseModel = new ListViewModel<AssetIssueResponseModel>();
                List<Asset> response = _UoW.IssueRepository.GetAssetsIssue(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), pagesize, pageindex, (int)Status.Active);
                //List<Asset> assetresponse = _UoW.WorkOrderRepository.GetAssetsWorkOrder(userid, pagesize, pageindex, (int)Status.Completed);
                if (response.Count > 0)
                {
                    int totalworkorder = response.Count;
                    responseModel.list = _mapper.Map<List<AssetIssueResponseModel>>(response);
                    List<IssueResponseModel> workorders = new List<IssueResponseModel>();
                    responseModel.list.ForEach(x =>
                    {
                        workorders = x.Issues.ToList();
                        x.Completed_Issues = workorders.Where(x => x.status == (int)Status.Completed).ToList();
                        if (pageindex > 0 && pagesize > 0)
                        {
                            x.Completed_Issues = x.Completed_Issues.OrderByDescending(x => x.modified_at).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                        }
                        x.Issues = workorders.Where(x => x.status != (int)Status.Completed).ToList();

                        x.Issues.ForEach(x =>
                        {
                            List<CategoryWiseAttributesInsepction> categoryWiseAttributes = new List<CategoryWiseAttributesInsepction>();
                            x.inspections.attribute_values.GroupBy(x => x.category_id);

                            var results = x.inspections.attribute_values.GroupBy(
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
                                categoryWiseAttribute.attribute_values = _mapper.Map<List<InspectionAttributesJsonObjectViewModel>>(item.Attributes);

                                categoryWiseAttributes.Add(categoryWiseAttribute);
                            }

                            /// get created by name who created comment
                            if (x.comments != null && x.comments.Count > 0)
                            {
                                foreach (var comment in x.comments)
                                {
                                    if (comment.created_by != string.Empty && comment.created_by != null)
                                    {
                                        var createduser = _UoW.UserRepository.GetUserByID(comment.created_by);
                                        if (createduser.Result != null)
                                        {
                                            comment.created_by_name = createduser.Result.firstname + " " + createduser.Result.lastname;
                                        }
                                    }
                                }
                            }

                            x.inspections.attributes = categoryWiseAttributes;
                        });

                        x.Completed_Issues.ForEach(x =>
                        {
                            List<CategoryWiseAttributesInsepction> categoryWiseAttributes = new List<CategoryWiseAttributesInsepction>();
                            x.inspections.attribute_values.GroupBy(x => x.category_id);

                            var results = x.inspections.attribute_values.GroupBy(
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
                                categoryWiseAttribute.attribute_values = _mapper.Map<List<InspectionAttributesJsonObjectViewModel>>(item.Attributes);

                                categoryWiseAttributes.Add(categoryWiseAttribute);
                            }

                            /// get created by name who created comment
                            if (x.comments != null && x.comments.Count > 0)
                            {
                                foreach (var comment in x.comments)
                                {
                                    if (comment.created_by != string.Empty && comment.created_by != null)
                                    {
                                        var createduser = _UoW.UserRepository.GetUserByID(comment.created_by);
                                        if (createduser.Result != null)
                                        {
                                            comment.created_by_name = createduser.Result.firstname + " " + createduser.Result.lastname;
                                        }
                                    }
                                }
                            }

                            x.inspections.attributes = categoryWiseAttributes;
                        });

                    });

                    //var assets = _mapper.Map<List<AssetWorkOrderResponseModel>>(assetresponse);
                    //responseModel.list.ForEach(x => x.completeworkorder = assets);
                    responseModel.listsize = totalworkorder;
                    responseModel.pageIndex = pageindex;
                    responseModel.pageSize = pagesize;
                }
                responseModel.result = (int)ResponseStatusNumber.Success;
                return responseModel;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Asset> FindCompletedWorkOrder(List<Asset> assets)
        {
            List<Asset> toalassets = assets;
            foreach (var asset in toalassets)
            {
                var assetdetaiils = asset;
                List<Issue> workOrders = new List<Issue>();
                foreach (var compeledworkorder in assetdetaiils.Issues)
                {
                    Issue workorder = compeledworkorder;
                    if (workorder.status == (int)Status.Completed)
                    {
                        workOrders.Add(workorder);
                    }
                }
                if (workOrders.Count > 0)
                {
                    assetdetaiils.Issues = workOrders;
                }
            }
            return toalassets;
        }

        public List<Asset> FindNotCompletedWorkOrder(List<Asset> assets)
        {
            foreach (var asset in assets)
            {
                List<Issue> workOrders = new List<Issue>();
                foreach (var compeledworkorder in asset.Issues)
                {
                    if (compeledworkorder.status != (int)Status.Completed)
                    {
                        workOrders.Add(compeledworkorder);
                    }
                }
                if (workOrders.Count > 0)
                {
                    asset.Issues = workOrders;
                }
            }
            return assets;
        }

        public ListViewModel<IssueResponseModel> GetAllIssue(string timestamp, int pagesize, int pageindex)
        {
            try
            {
                ListViewModel<IssueResponseModel> workorder = new ListViewModel<IssueResponseModel>();
                List<Issue> workorders = _UoW.IssueRepository.GetAllIssues(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), timestamp);
                workorder.listsize = workorders.Count();
                if (workorders.Count > 0)
                {
                    if (pageindex > 0 && pagesize > 0)
                    {
                        workorders = workorders.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    }
                    workorder.list = _mapper.Map<List<IssueResponseModel>>(workorders);
                    workorder.pageIndex = pageindex;
                    workorder.pageSize = pagesize;
                }
                return workorder;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int> CreateIssuesFromWorkOrder()
        {
            var response = (int)ResponseStatusNumber.Error;
            try
            {
                _UoW.BeginTransaction();
                response = await _UoW.IssueRepository.CreateIssueFromIssue();
                if (response > 0)
                {
                    _UoW.SaveChanges();
                    _UoW.CommitTransaction();
                }
                return response;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int> CreateIssueStatus()
        {
            var response = (int)ResponseStatusNumber.Error;
            try
            {
                _UoW.BeginTransaction();
                response = await _UoW.IssueRepository.CreateIssueStatus();
                if (response > 0)
                {
                    _UoW.SaveChanges();
                    _UoW.CommitTransaction();
                }
                return response;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int> CreateIssueRecords()
        {
            var response = (int)ResponseStatusNumber.Error;
            try
            {
                _UoW.BeginTransaction();
                response = await _UoW.IssueRepository.CreateIssueRecords();
                if (response > 0)
                {
                    _UoW.SaveChanges();
                    _UoW.CommitTransaction();
                }
                return response;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public async Task<LinkIssueToWOFromIssueListTabResponsemodel> LinkIssueToWOFromIssueListTab(LinkIssueToWOFromIssueListTabRequestModel requestmodel)
        {
            int response = (int)ResponseStatusNumber.Error;
            LinkIssueToWOFromIssueListTabResponsemodel responsemodel = new LinkIssueToWOFromIssueListTabResponsemodel();
            string empty_asset_class_list = "";
            responsemodel.success = (int)ResponseStatusNumber.Error;

            try
            {
                if (requestmodel.link_issue_list != null)
                {
                    foreach (var requested_asset_issue in requestmodel.link_issue_list)
                    {

                        var get_asset_issue = _UoW.WorkOrderRepository.GetAssetIssueByAssetIssueIdRepo(requested_asset_issue.asset_issue_id);
                        WorkOrderService workOrderService = new WorkOrderService(_mapper);

                        if (requested_asset_issue.wo_type == (int)MWO_inspection_wo_type.Inspection)
                        {
                            AssignMultipleAssetClasstoWORequestmodel AssignMultipleAssetClasstoWO1 = new AssignMultipleAssetClasstoWORequestmodel();
                            AssignMultipleAssetClasstoWO1.assign_inspection_asset_class_list = new List<AssignAssetClasstoWOList>();
                            AssignMultipleAssetClasstoWO1.wo_id = requested_asset_issue.wo_id;

                            var class_form = get_asset_issue.Asset.InspectionTemplateAssetClass.AssetClassFormIOMapping.Where(x => x.wo_type == (int)Status.Maintenance_WO).FirstOrDefault();
                            Guid? form_id = null;
                            // check if form is assigned in asset class
                            if (class_form != null && class_form.form_id != null)
                            {
                                form_id = class_form.form_id;
                            }
                            else
                            {
                                // check for AT form 
                                class_form = get_asset_issue.Asset.InspectionTemplateAssetClass.AssetClassFormIOMapping.Where(x => x.wo_type == (int)Status.Acceptance_Test_WO).FirstOrDefault();
                                if (class_form != null && class_form.form_id != null)
                                {
                                    form_id = class_form.form_id;
                                }
                                else
                                {
                                    empty_asset_class_list = empty_asset_class_list + " , " + get_asset_issue.Asset.InspectionTemplateAssetClass.asset_class_name;
                                    responsemodel.empty_asset_class_list = empty_asset_class_list;
                                }
                            }

                            if (form_id != null)
                            {
                                AssignAssetClasstoWOList AssignAssetClasstoWOList = new AssignAssetClasstoWOList();
                                AssignAssetClasstoWOList.asset_id = get_asset_issue.asset_id;
                                AssignAssetClasstoWOList.form_id = form_id;
                                AssignAssetClasstoWOList.inspection_type = 2;
                                AssignAssetClasstoWOList.inspectiontemplate_asset_class_id = get_asset_issue.Asset.inspectiontemplate_asset_class_id;
                                AssignAssetClasstoWOList.wo_id = requested_asset_issue.wo_id;
                                AssignAssetClasstoWOList.wo_type = 67;

                                AssignMultipleAssetClasstoWO1.assign_inspection_asset_class_list.Add(AssignAssetClasstoWOList);
                                var insert_WO_lines = await workOrderService.AssignMultipleAssetClasstoWO(AssignMultipleAssetClasstoWO1);

                                if (insert_WO_lines.success == (int)ResponseStatusNumber.Success)
                                {
                                    //Link Issue with WOLine
                                    LinkIssueToWOLineRequestmodel linkIssueToWOLineReqmodel = new LinkIssueToWOLineRequestmodel();

                                    linkIssueToWOLineReqmodel.asset_issue_id = new List<Guid>();
                                    linkIssueToWOLineReqmodel.asset_issue_id.Add(requested_asset_issue.asset_issue_id);
                                    linkIssueToWOLineReqmodel.wo_id = requested_asset_issue.wo_id;
                                    linkIssueToWOLineReqmodel.asset_form_id = insert_WO_lines.asset_form_id;

                                    var res = await workOrderService.LinkIssueToWOLine(linkIssueToWOLineReqmodel);

                                    if (res == (int)ResponseStatusNumber.Success)
                                    {
                                        response = (int)ResponseStatusNumber.Success;
                                        responsemodel.success = (int)ResponseStatusNumber.Success;
                                    }
                                }
                            }
                           
                        }
                        else
                        {
                            AddIssuesDirectlyToMaintenanceWORequestModel addIssuesDirectlyToMaintenanceWORequestModel = new AddIssuesDirectlyToMaintenanceWORequestModel();
                            addIssuesDirectlyToMaintenanceWORequestModel.asset_issue_id = requested_asset_issue.asset_issue_id;
                            addIssuesDirectlyToMaintenanceWORequestModel.wo_id = requested_asset_issue.wo_id;
                            addIssuesDirectlyToMaintenanceWORequestModel.inspection_type = requested_asset_issue.wo_type;

                            var res = await workOrderService.AddIssuesDirectlyToMaintenanceWOServiceBySteps(addIssuesDirectlyToMaintenanceWORequestModel);

                            if (res!=null &&  res.success == (int)ResponseStatusNumber.Success)
                            {
                                response = (int)ResponseStatusNumber.Success;
                                responsemodel.success = (int)ResponseStatusNumber.Success;
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Log("Error in Get All WorkOrders " + e.Message);
                throw e;
            }
            return responsemodel;
        }

        public async Task<LinkIssueToWOFromIssueListTabResponsemodel> LinkIssueToWOFromIssueListTabForSteps(LinkIssueToWOFromIssueListTabRequestModel requestmodel)
        {
            int response = (int)ResponseStatusNumber.Error;
            LinkIssueToWOFromIssueListTabResponsemodel responsemodel = new LinkIssueToWOFromIssueListTabResponsemodel();
            string empty_asset_class_list = "";
            responsemodel.success = (int)ResponseStatusNumber.Error;
             
            try
            {
                if (requestmodel.link_issue_list != null)
                {
                    foreach (var requested_asset_issue in requestmodel.link_issue_list)
                    {

                        var get_asset_issue = _UoW.WorkOrderRepository.GetAssetIssueByAssetIssueIdRepo(requested_asset_issue.asset_issue_id);
                        WorkOrderService workOrderService = new WorkOrderService(_mapper);

                        if (requested_asset_issue.wo_type == (int)MWO_inspection_wo_type.Inspection)
                        {
                            AssignMultipleAssetClasstoWORequestmodel AssignMultipleAssetClasstoWO1 = new AssignMultipleAssetClasstoWORequestmodel();
                            AssignMultipleAssetClasstoWO1.assign_inspection_asset_class_list = new List<AssignAssetClasstoWOList>();
                            AssignMultipleAssetClasstoWO1.wo_id = requested_asset_issue.wo_id;

                            var class_form = get_asset_issue.Asset.InspectionTemplateAssetClass.AssetClassFormIOMapping.Where(x => x.wo_type == (int)Status.Maintenance_WO).FirstOrDefault();
                            Guid? form_id = null;
                            // check if form is assigned in asset class
                            if (class_form != null && class_form.form_id != null)
                            {
                                form_id = class_form.form_id;
                            }
                            else
                            {
                                // check for AT form 
                                class_form = get_asset_issue.Asset.InspectionTemplateAssetClass.AssetClassFormIOMapping.Where(x => x.wo_type == (int)Status.Acceptance_Test_WO).FirstOrDefault();
                                if (class_form != null && class_form.form_id != null)
                                {
                                    form_id = class_form.form_id;
                                }
                                else
                                {
                                    empty_asset_class_list = empty_asset_class_list + " , " + get_asset_issue.Asset.InspectionTemplateAssetClass.asset_class_name;
                                    responsemodel.empty_asset_class_list = empty_asset_class_list;
                                }
                            }

                            if (form_id != null)
                            {
                                AssignAssetClasstoWOList AssignAssetClasstoWOList = new AssignAssetClasstoWOList();
                                AssignAssetClasstoWOList.asset_id = get_asset_issue.asset_id;
                                AssignAssetClasstoWOList.form_id = form_id;
                                AssignAssetClasstoWOList.inspection_type = 2;
                                AssignAssetClasstoWOList.inspectiontemplate_asset_class_id = get_asset_issue.Asset.inspectiontemplate_asset_class_id;
                                AssignAssetClasstoWOList.wo_id = requested_asset_issue.wo_id;
                                AssignAssetClasstoWOList.wo_type = 67;

                                AssignMultipleAssetClasstoWO1.assign_inspection_asset_class_list.Add(AssignAssetClasstoWOList);
                                var insert_WO_lines = await workOrderService.AssignMultipleAssetClasstoWO(AssignMultipleAssetClasstoWO1);

                                if (insert_WO_lines.success == (int)ResponseStatusNumber.Success)
                                {
                                    //Link Issue with WOLine
                                    LinkIssueToWOLineRequestmodel linkIssueToWOLineReqmodel = new LinkIssueToWOLineRequestmodel();

                                    linkIssueToWOLineReqmodel.asset_issue_id = new List<Guid>();
                                    linkIssueToWOLineReqmodel.asset_issue_id.Add(requested_asset_issue.asset_issue_id);
                                    linkIssueToWOLineReqmodel.wo_id = requested_asset_issue.wo_id;
                                    linkIssueToWOLineReqmodel.asset_form_id = insert_WO_lines.asset_form_id;

                                    var res = await workOrderService.LinkIssueToWOLine(linkIssueToWOLineReqmodel);

                                    if (res == (int)ResponseStatusNumber.Success)
                                    {
                                        response = (int)ResponseStatusNumber.Success;
                                        responsemodel.success = (int)ResponseStatusNumber.Success;
                                    }
                                }
                            }

                        }
                        else
                        {
                            // for new flow we are using existing functions which are used in WO
                            AddIssueByStepsRequestmodel request = new AddIssueByStepsRequestmodel();

                            // add issue details
                            var get_main_issue = _UoW.IssueRepository.GetMainIssue(requested_asset_issue.asset_issue_id);
                            request.issue_details = new AddIssueDetailsForSteps();
                            request.issue_details.issue_title = get_main_issue.issue_title;
                            request.issue_details.problem_description = get_main_issue.issue_description;
                            request.issue_details.priority = get_main_issue.priority;
                            request.issue_details.wo_id = requested_asset_issue.wo_id;
                           
                            request.issue_details.asset_issue_id = get_main_issue.asset_issue_id;
                            request.issue_details.issue_type = get_main_issue.issue_type;
                            request.issue_details.inspection_type = requested_asset_issue.wo_type;
                            request.issue_details.selected_asset_id = get_main_issue.asset_id;
                            request.issue_details.is_selected_asset_id_main = true;
                            request.issue_details.issue_creation_type = (int)IssueCreationtype.existing_issue;

                            request.issue_details.issue_images = new List<IssueImagesMapping>();

                            if(get_main_issue.AssetIssueImagesMapping!=null && get_main_issue.AssetIssueImagesMapping.Count > 0)
                            {
                                var issue_imgs = get_main_issue.AssetIssueImagesMapping.Where(x => !x.is_deleted).ToList();
                                foreach(var img in issue_imgs)
                                {
                                    IssueImagesMapping IssueImagesMapping = new IssueImagesMapping();
                                    IssueImagesMapping.asset_issue_image_mapping_id = img.asset_issue_image_mapping_id;
                                    IssueImagesMapping.image_file_name = img.image_file_name;
                                    IssueImagesMapping.image_thumbnail_file_name = img.image_thumbnail_file_name;
                                    IssueImagesMapping.image_duration_type_id = img.image_duration_type_id.Value;

                                    request.issue_details.issue_images.Add(IssueImagesMapping);
                                }

                            }


                            // now add woline details
                            // 
                            AssetService woservice = new AssetService(_mapper);
                            GetAssetDetailsByIdForTempAsset GetAssetDetailsByIdForTempAsset = new GetAssetDetailsByIdForTempAsset();
                            GetAssetDetailsByIdForTempAsset.asset_id = get_main_issue.asset_id;
                            GetAssetDetailsByIdForTempAsset.wo_id = requested_asset_issue.wo_id;
                            var get_woline_details = woservice.GetAssetDetailsByIdForTempAsset(GetAssetDetailsByIdForTempAsset);

                            var map = _mapper.Map<UpdateOBWOAssetDetailsRequestmodel>(get_woline_details);

                            request.install_woline_details = map;

                            var insert_issue =  await AddIssueBySteps(request);
                            if(insert_issue > (int)ResponseStatusNumber.Success)
                            {
                                responsemodel.success = (int)ResponseStatusNumber.Success;
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Log("Error in Get All WorkOrders " + e.Message);
                throw e;
            }
            return responsemodel;
        }

        public async Task<int> AddIssueBySteps(AddIssueByStepsRequestmodel requestmodel)
        {
            var serailize = Newtonsoft.Json.JsonConvert.SerializeObject(requestmodel);
            
            WorkOrderService wo_service = new WorkOrderService(_mapper);
            if(requestmodel.issue_details.issue_creation_type == (int)IssueCreationtype.existing_issue) // for existing issue
            {

                Guid temp_asset_id;
                //check if temp asset is exist or not 
                var get_temp_asset = _UoW.IssueRepository.GetTempIssueByAssetId(requestmodel.issue_details.selected_asset_id.Value , requestmodel.issue_details.wo_id);
                if(get_temp_asset == null) // if temp asset is not created then create new temp asset
                {
                    // create temp asset
                    get_temp_asset = new TempAsset();
                    temp_asset_id = await CreateUpdateTempAsset(requestmodel.install_woline_details , get_temp_asset);
                }
                else
                {
                    temp_asset_id = get_temp_asset.tempasset_id;
                }

                // get temp Assetdetails detals 
                get_temp_asset = _UoW.IssueRepository.GetTempAssetbyId(temp_asset_id);

                // check if temp asset having install woline or not if not exist then insert
                var install_woline = get_temp_asset.WOOnboardingAssets.Where(x => x.inspection_type == (int)MWO_inspection_wo_type.OnBoarding 
                                                                              && !x.is_deleted).FirstOrDefault();

                if (install_woline == null) /// if temp asset is not available then install/add woline will also not created in wo so create new install/add woline
                {
                    // create install woline
                    var add_install_woline =await CreateInstallWoline(requestmodel.install_woline_details);
                    // update temp asset_id in install woline 
                    var get_install_woline = _UoW.IssueRepository.GetWolneById(add_install_woline.woonboardingassets_id);
                    get_install_woline.tempasset_id = temp_asset_id;
                    var update = await _UoW.BaseGenericRepository<WOOnboardingAssets>().Update(get_install_woline);
                }
                // create repair/replace/other woline and attach issue to that woline
                await CreateIssueWOline(requestmodel, get_temp_asset);
                await wo_service.updateOBWOStatusForStatusManagement(requestmodel.issue_details.wo_id);
            }
            else if(requestmodel.issue_details.issue_creation_type == (int)IssueCreationtype.new_issue) // create new issue
            {
                // create temp issue 
                WOLineIssue temp_issue = new WOLineIssue();
                temp_issue.issue_title = requestmodel.issue_details.issue_title;
                temp_issue.issue_type = requestmodel.issue_details.issue_type;
                temp_issue.issue_status = (int)Status.InProgress;
               
                temp_issue.original_wo_id = requestmodel.issue_details.wo_id;
                
                temp_issue.issue_description = requestmodel.issue_details.problem_description;
                temp_issue.is_issue_linked_for_fix = false;
                if(requestmodel.issue_details.resolution_type == 1)
                {
                    temp_issue.is_issue_linked_for_fix = true;
                }
                temp_issue.site_id =Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                temp_issue.wo_id = requestmodel.issue_details.wo_id;
                temp_issue.created_at = DateTime.UtcNow;
                temp_issue.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                if (requestmodel.issue_details.new_issue_asset_type != (int)NewIssueAssettype.VerifyOnField) // if new issue is not for verify on field
                {
                    temp_issue.form_retrived_asset_name = requestmodel.install_woline_details.asset_name;

                    // create temp asset first based on selected asset
                    // check if temp asset is exist or not based on selected asset if exist then update its data
                    bool is_temp_asset_exist = false;
                    TempAsset get_temp_asset = null;
                    if(requestmodel.issue_details.new_issue_asset_type == (int)NewIssueAssettype.NewAsset) // if requested is for new asset then add temp asset
                    {
                        is_temp_asset_exist = false;
                    }
                    else if(requestmodel.issue_details.new_issue_asset_type == (int)NewIssueAssettype.ExistingAsset) /// if selected assset is existing 
                    {
                        if (requestmodel.issue_details.is_selected_asset_id_main) // if selected asset id is from main then check from main asset
                        {
                            get_temp_asset = _UoW.IssueRepository.GetTempIssueByAssetId(requestmodel.issue_details.selected_asset_id.Value, requestmodel.issue_details.wo_id);
                            if(get_temp_asset != null)
                            {
                                is_temp_asset_exist = true;
                            }
                        }
                        else if (!requestmodel.issue_details.is_selected_asset_id_main) // if selected asset id is from temp then check from woline
                        {
                            var get_woline = _UoW.IssueRepository.GetWolneById(requestmodel.issue_details.selected_asset_id.Value);
                            get_temp_asset = get_woline.TempAsset;
                            if (get_temp_asset != null)
                            {
                                is_temp_asset_exist = true;
                            }
                        }
                    }
                    if (get_temp_asset == null)
                        get_temp_asset = new TempAsset();

                    // create or update temp asset
                    var temp_asset_id =  await CreateUpdateTempAsset(requestmodel.install_woline_details, get_temp_asset);

                    // get temp asset by id 
                    get_temp_asset = _UoW.IssueRepository.GetTempAssetbyId(temp_asset_id);

                    // check if temp asset having install woline or not if not exist then insert
                    var install_woline = get_temp_asset.WOOnboardingAssets.Where(x => x.inspection_type == (int)MWO_inspection_wo_type.OnBoarding
                                                                                  && !x.is_deleted).FirstOrDefault();

                    if (install_woline == null) /// if temp asset is not available then install/add woline will also not created in wo so create new install/add woline
                    {
                        // create install woline
                        var add_install_woline = await CreateInstallWoline(requestmodel.install_woline_details);
                        // update temp asset_id in install woline 
                        install_woline = _UoW.IssueRepository.GetWolneById(add_install_woline.woonboardingassets_id);
                        install_woline.tempasset_id = temp_asset_id;
                        var update = await _UoW.BaseGenericRepository<WOOnboardingAssets>().Update(install_woline);
                    }

                    // create repair/replace/other woline and attach issue to that woline
                    var isseu_woline = await CreateIssueWOline(requestmodel, get_temp_asset);
                  
                    // update selected asset id for issue , it can be main asset or temp asset.
                    if (requestmodel.issue_details.new_issue_asset_type == (int)NewIssueAssettype.NewAsset)
                    {
                        isseu_woline.issues_temp_asset_id = install_woline.woonboardingassets_id;
                    }
                    else if(requestmodel.issue_details.new_issue_asset_type == (int)NewIssueAssettype.ExistingAsset)
                    {
                        if (requestmodel.issue_details.is_selected_asset_id_main) // if selected asset id is from main asset
                        {
                            isseu_woline.asset_id = requestmodel.issue_details.selected_asset_id;
                        }
                        else if (!requestmodel.issue_details.is_selected_asset_id_main) // if selected asset id is from temp asset
                        {
                            isseu_woline.issues_temp_asset_id = requestmodel.issue_details.selected_asset_id;
                        }
                    }
                    await _UoW.BaseGenericRepository<WOOnboardingAssets>().Update(isseu_woline);

                    //update temp issues woline and origin woline details
                    temp_issue.woonboardingassets_id = isseu_woline.woonboardingassets_id;
                    temp_issue.original_woonboardingassets_id = install_woline.woonboardingassets_id;
                    if (requestmodel.issue_details.is_selected_asset_id_main == true && requestmodel.issue_details.selected_asset_id != null) // if selected asset is main then assign main asset id 
                    {
                        temp_issue.original_asset_id = requestmodel.issue_details.selected_asset_id;
                    }
                    
                }
                else // verify on field
                {
                    // create only issue woline without any assset details
                    var isseu_woline = await CreateIssueWOline(requestmodel, null);
                    temp_issue.woonboardingassets_id = isseu_woline.woonboardingassets_id;
                }

                var insert_temp_issue = await _UoW.BaseGenericRepository<WOLineIssue>().Insert(temp_issue);
                _UoW.SaveChanges();

                await wo_service.updateOBWOStatusForStatusManagement(requestmodel.issue_details.wo_id);
            }

            return 1;
        }

        public async Task<int> UpdateIssueBySteps(UpdateIssueByStepsRequestmodel requestmodel)
        {
            int response = (int)ResponseStatusNumber.Error;
            // update for exisitng/main issue
            if (requestmodel.issue_details.issue_creation_type == (int)IssueCreationtype.existing_issue) // for existing issue
            {
                Guid temp_asset_id;
                //update TempAsset details
                var get_temp_asset = _UoW.IssueRepository.GetTempIssueByAssetIdAsnotracking(requestmodel.issue_details.selected_asset_id.Value, requestmodel.issue_details.wo_id);
                temp_asset_id = await CreateUpdateTempAsset(requestmodel.install_woline_details, get_temp_asset);
                
                // get temp Assetdetails detals 
                get_temp_asset = _UoW.IssueRepository.GetTempAssetbyId(temp_asset_id);

                // update install woline
                var update_install_woline = await CreateInstallWoline(requestmodel.install_woline_details);

                // update repair/replace/other woline
                // prepare request model to update issue woline
                UpdateOBWOAssetDetailsRequestmodel issue_woline_request = new UpdateOBWOAssetDetailsRequestmodel();
                issue_woline_request.woonboardingassets_id = requestmodel.issue_woline_details.woonboardingassets_id;
                issue_woline_request.wo_id = requestmodel.issue_details.wo_id;
                issue_woline_request.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                issue_woline_request.asset_id = get_temp_asset.asset_id;
                issue_woline_request.asset_name = get_temp_asset.asset_name;
                issue_woline_request.inspection_type = requestmodel.issue_details.inspection_type;
                issue_woline_request.issue_title = requestmodel.issue_details.issue_title;
                issue_woline_request.issue_priority = requestmodel.issue_details.priority;
                issue_woline_request.problem_description = requestmodel.issue_details.problem_description;
                issue_woline_request.solution_description = requestmodel.issue_details.resolution_description;
                issue_woline_request.inspection_further_details = requestmodel.issue_details.inspection_further_description;
                issue_woline_request.is_replaced_asset_id_is_main = requestmodel.issue_details.is_replaced_asset_id_is_main;
                issue_woline_request.replaced_asset_id = requestmodel.issue_details.replaced_asset_id;
                issue_woline_request.status = requestmodel.issue_woline_details.status;
                issue_woline_request.repair_resolution = null;
                issue_woline_request.replacement_resolution = null;
                issue_woline_request.general_issue_resolution = null;
                if (requestmodel.issue_details.inspection_type == (int)MWO_inspection_wo_type.Repair)
                    issue_woline_request.repair_resolution = requestmodel.issue_details.resolution_type;
                if (requestmodel.issue_details.inspection_type == (int)MWO_inspection_wo_type.Replace)
                    issue_woline_request.replacement_resolution = requestmodel.issue_details.resolution_type;
                if (requestmodel.issue_details.inspection_type == (int)MWO_inspection_wo_type.Trouble_Call_Check)
                    issue_woline_request.general_issue_resolution = requestmodel.issue_details.resolution_type;

                issue_woline_request.asset_issue_id = new List<Guid> { requestmodel.issue_details.asset_issue_id.Value };

                if(requestmodel.issue_details.issue_images != null && requestmodel.issue_details.issue_images.Count > 0) // add or update images 
                {
                    issue_woline_request.asset_image_list = new List<OBWOAssetImages>();
                    foreach (var img in requestmodel.issue_details.issue_images)
                    {
                        OBWOAssetImages OBWOAssetImages = new OBWOAssetImages();
                        OBWOAssetImages.woonboardingassetsimagesmapping_id = img.woonboardingassetsimagesmapping_id;
                        OBWOAssetImages.asset_photo = img.image_file_name;
                        OBWOAssetImages.asset_thumbnail_photo = img.image_thumbnail_file_name;
                        OBWOAssetImages.is_deleted = img.is_deleted;
                        OBWOAssetImages.asset_photo_type = (int)AssetPhotoType.Asset_Profile;
                        OBWOAssetImages.woonboardingassets_id = issue_woline_request.woonboardingassets_id;
                        OBWOAssetImages.image_duration_type_id = img.image_duration_type_id;


                        issue_woline_request.asset_image_list.Add(OBWOAssetImages);
                    }
                }

                await UpdateIssueWoline(issue_woline_request);
                response = (int)ResponseStatusNumber.Success;
            }
            else if (requestmodel.issue_details.issue_creation_type == (int)IssueCreationtype.new_issue) // create new issue
            {
                if (requestmodel.issue_details.new_issue_asset_type != (int)NewIssueAssettype.VerifyOnField) // if new issue is not for verify on field
                {
                    // create temp asset first based on selected asset
                    // check if temp asset is exist or not based on selected asset if exist then update its data
                    bool is_temp_asset_exist = false;
                    TempAsset get_temp_asset = null;
                    if (requestmodel.issue_details.new_issue_asset_type == (int)NewIssueAssettype.NewAsset) // if requested is for new asset then add temp asset
                    {
                        is_temp_asset_exist = false;
                    }
                    else if (requestmodel.issue_details.new_issue_asset_type == (int)NewIssueAssettype.ExistingAsset) /// if selected assset is existing 
                    {
                        if (requestmodel.issue_details.is_selected_asset_id_main) // if selected asset id is from main then check from main asset
                        {
                            get_temp_asset = _UoW.IssueRepository.GetTempIssueByAssetId(requestmodel.issue_details.selected_asset_id.Value, requestmodel.issue_details.wo_id);
                            if (get_temp_asset != null)
                            {
                                is_temp_asset_exist = true;
                            }
                        }
                        else if (!requestmodel.issue_details.is_selected_asset_id_main) // if selected asset id is from temp then check from woline
                        {
                            get_temp_asset = _UoW.IssueRepository.GetWolneTempAssetById(requestmodel.issue_details.selected_asset_id.Value);
                            //get_temp_asset = get_woline.TempAsset;
                            if (get_temp_asset != null)
                            {
                                is_temp_asset_exist = true;
                            }
                        }
                    }
                    if (get_temp_asset == null)
                        get_temp_asset = new TempAsset();

                    // create or update temp asset
                    var temp_asset_id = await CreateUpdateTempAsset(requestmodel.install_woline_details, get_temp_asset);

                    // get temp asset by id 
                    get_temp_asset = _UoW.IssueRepository.GetTempAssetbyId(temp_asset_id);


                    // create or update woline 
                    requestmodel.install_woline_details.tempasset_id = get_temp_asset.tempasset_id;
                    var add_install_woline = await CreateInstallWoline(requestmodel.install_woline_details);
                   
                    // update repair/replace/other woline
                    // prepare request model to update issue woline
                    UpdateOBWOAssetDetailsRequestmodel issue_woline_request = new UpdateOBWOAssetDetailsRequestmodel();
                    issue_woline_request.woonboardingassets_id = requestmodel.issue_woline_details.woonboardingassets_id;
                    issue_woline_request.status = requestmodel.issue_woline_details.status;
                    issue_woline_request.wo_id = requestmodel.issue_details.wo_id;
                    issue_woline_request.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                    if (!requestmodel.issue_details.is_selected_asset_id_main) // if selected asset is from temp asset
                    {
                        issue_woline_request.issues_temp_asset_id = add_install_woline.woonboardingassets_id;
                        issue_woline_request.asset_id = null;
                    }
                    else // if selected asset is from main asset
                    {
                        issue_woline_request.issues_temp_asset_id = null;
                        issue_woline_request.asset_id = get_temp_asset.asset_id;
                    }
                    issue_woline_request.asset_name = get_temp_asset.asset_name;
                    issue_woline_request.inspection_type = requestmodel.issue_details.inspection_type;
                    issue_woline_request.issue_title = requestmodel.issue_details.issue_title;
                    issue_woline_request.issue_priority = requestmodel.issue_details.priority;
                    issue_woline_request.problem_description = requestmodel.issue_details.problem_description;
                    issue_woline_request.solution_description = requestmodel.issue_details.resolution_description;
                    issue_woline_request.inspection_further_details = requestmodel.issue_details.inspection_further_description;
                    issue_woline_request.replaced_asset_id = requestmodel.issue_details.replaced_asset_id;
                    issue_woline_request.is_replaced_asset_id_is_main = requestmodel.issue_details.is_replaced_asset_id_is_main;
                    issue_woline_request.new_issue_asset_type = (int)NewIssueAssettype.ExistingAsset;

                    issue_woline_request.repair_resolution = null;
                    issue_woline_request.replacement_resolution = null;
                    issue_woline_request.general_issue_resolution = null;

                    if (requestmodel.issue_details.inspection_type == (int)MWO_inspection_wo_type.Repair)
                        issue_woline_request.repair_resolution = requestmodel.issue_details.resolution_type;
                    if (requestmodel.issue_details.inspection_type == (int)MWO_inspection_wo_type.Replace)
                        issue_woline_request.replacement_resolution = requestmodel.issue_details.resolution_type;
                    if (requestmodel.issue_details.inspection_type == (int)MWO_inspection_wo_type.Trouble_Call_Check)
                        issue_woline_request.general_issue_resolution = requestmodel.issue_details.resolution_type;
                    if (requestmodel.issue_details.asset_issue_id != null)
                    {
                        issue_woline_request.asset_issue_id = new List<Guid> { requestmodel.issue_details.asset_issue_id.Value };
                    }

                    if (requestmodel.issue_details.issue_images != null && requestmodel.issue_details.issue_images.Count > 0) // add or update images 
                    {
                        issue_woline_request.asset_image_list = new List<OBWOAssetImages>();
                        foreach (var img in requestmodel.issue_details.issue_images)
                        {
                            OBWOAssetImages OBWOAssetImages = new OBWOAssetImages();
                            OBWOAssetImages.woonboardingassetsimagesmapping_id = img.woonboardingassetsimagesmapping_id;
                            OBWOAssetImages.asset_photo = img.image_file_name;
                            OBWOAssetImages.asset_thumbnail_photo = img.image_thumbnail_file_name;
                            OBWOAssetImages.is_deleted = img.is_deleted;
                            OBWOAssetImages.asset_photo_type = (int)AssetPhotoType.Asset_Profile;
                            OBWOAssetImages.woonboardingassets_id = issue_woline_request.woonboardingassets_id;
                            OBWOAssetImages.image_duration_type_id = img.image_duration_type_id;

                            issue_woline_request.asset_image_list.Add(OBWOAssetImages);
                        }
                    }
                    issue_woline_request.tempasset_id = temp_asset_id;
                    // update selected asset id for issue , it can be main asset or temp asset.
                    if (requestmodel.issue_details.new_issue_asset_type == (int)NewIssueAssettype.NewAsset)
                    {
                        issue_woline_request.issues_temp_asset_id = add_install_woline.woonboardingassets_id;
                    }
                    else if (requestmodel.issue_details.new_issue_asset_type == (int)NewIssueAssettype.ExistingAsset)
                    {
                        if (requestmodel.issue_details.is_selected_asset_id_main) // if selected asset id is from main asset
                        {
                            issue_woline_request.asset_id = requestmodel.issue_details.selected_asset_id;
                        }
                        else if (!requestmodel.issue_details.is_selected_asset_id_main) // if selected asset id is from temp asset
                        {
                            issue_woline_request.issues_temp_asset_id = requestmodel.issue_details.selected_asset_id;
                        }
                    }


                    var update_issue_woline = await UpdateIssueWoline(issue_woline_request);
                    

                   /* // update temp asset id in issue woline
                    var get_issue_woline = _UoW.IssueRepository.GetWolneByIdAsnotracking(update_issue_woline.woonboardingassets_id);
                    get_issue_woline.tempasset_id = temp_asset_id;
                    // update selected asset id for issue , it can be main asset or temp asset.
                    if (requestmodel.issue_details.new_issue_asset_type == (int)NewIssueAssettype.NewAsset)
                    {
                        get_issue_woline.issues_temp_asset_id = get_install_woline.woonboardingassets_id;
                    }
                    else if (requestmodel.issue_details.new_issue_asset_type == (int)NewIssueAssettype.ExistingAsset)
                    {
                        if (requestmodel.issue_details.is_selected_asset_id_main) // if selected asset id is from main asset
                        {
                            get_issue_woline.asset_id = requestmodel.issue_details.selected_asset_id;
                        }
                        else if (!requestmodel.issue_details.is_selected_asset_id_main) // if selected asset id is from temp asset
                        {
                            get_issue_woline.issues_temp_asset_id = requestmodel.issue_details.selected_asset_id;
                        }
                    }
                    try
                    {
                        await _UoW.BaseGenericRepository<WOOnboardingAssets>().Update(get_issue_woline);
                    }
                    catch(Exception ex)
                    {

                    }*/
                    // update wolineissue and update asset_ids and woline ids 
                    var get_temp_issue = _UoW.IssueRepository.GetwolineissueById(requestmodel.issue_details.wo_line_issue_id.Value);
                    get_temp_issue.original_woonboardingassets_id = add_install_woline.woonboardingassets_id;
                    get_temp_issue.original_asset_id = null;
                    get_temp_issue.modified_at = DateTime.UtcNow;
                    get_temp_issue.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    // update temp issue status based on woline status
                    get_temp_issue.issue_status = (int)Status.InProgress;
                    if(requestmodel.issue_details.resolution_type == 1)
                        get_temp_issue.is_issue_linked_for_fix = true;
                    else
                        get_temp_issue.is_issue_linked_for_fix = false;

                    await _UoW.BaseGenericRepository<WOLineIssue>().Update(get_temp_issue);

                }
                else // verify on field 
                {
                    // if verify on field then remove linked asset and its details from issue and woline;

                    //get repair/replace/other woline and update its data 
                    var get_issue_woline = _UoW.IssueRepository.GetWolneById(requestmodel.issue_woline_details.woonboardingassets_id.Value);
                    get_issue_woline.new_issue_asset_type = (int)NewIssueAssettype.VerifyOnField;
                    get_issue_woline.tempasset_id = null;
                    get_issue_woline.wo_id = requestmodel.issue_details.wo_id;
                    get_issue_woline.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                    get_issue_woline.asset_id = null;
                    get_issue_woline.asset_name =null;
                    get_issue_woline.inspection_type = requestmodel.issue_details.inspection_type;
                    get_issue_woline.problem_description = requestmodel.issue_details.problem_description;
                    get_issue_woline.issue_title = requestmodel.issue_details.issue_title;
                    get_issue_woline.issue_priority = requestmodel.issue_details.priority;
                    get_issue_woline.solution_description = requestmodel.issue_details.resolution_description;
                    get_issue_woline.inspection_further_details = requestmodel.issue_details.inspection_further_description;
                    get_issue_woline.replaced_asset_id = null;
                    get_issue_woline.issues_temp_asset_id = null;
                    get_issue_woline.status = requestmodel.issue_woline_details.status;
                    get_issue_woline.modified_at = DateTime.UtcNow;
                    get_issue_woline.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    /// add/update images in issue woline 
                    /// 
                    if (requestmodel.issue_details.issue_images != null && requestmodel.issue_details.issue_images.Count>0)
                    {
                        var new_images = requestmodel.issue_details.issue_images.Where(x => x.woonboardingassetsimagesmapping_id == null && !x.is_deleted).ToList();
                        foreach(var img in new_images)
                        {
                            WOOnboardingAssetsImagesMapping WOOnboardingAssetsImagesMapping = new WOOnboardingAssetsImagesMapping();
                            WOOnboardingAssetsImagesMapping.asset_photo = img.image_file_name;
                            WOOnboardingAssetsImagesMapping.asset_thumbnail_photo = img.image_thumbnail_file_name;
                            WOOnboardingAssetsImagesMapping.created_at =DateTime.UtcNow;
                            WOOnboardingAssetsImagesMapping.asset_photo_type = (int)AssetPhotoType.Asset_Profile;
                            WOOnboardingAssetsImagesMapping.image_duration_type_id = img.image_duration_type_id;

                            get_issue_woline.WOOnboardingAssetsImagesMapping.Add(WOOnboardingAssetsImagesMapping);
                        }

                        var deleted_images = requestmodel.issue_details.issue_images.Where(x => x.woonboardingassetsimagesmapping_id!= null && x.is_deleted).Select(x=>x.woonboardingassetsimagesmapping_id).ToList();
                        var db_images = get_issue_woline.WOOnboardingAssetsImagesMapping.Where(x => deleted_images.Contains(x.woonboardingassetsimagesmapping_id)).ToList();
                        foreach(var img in db_images)
                        {
                            img.is_deleted = true;
                            img.modified_at = DateTime.UtcNow;
                        }
                    }

                    var update_issue_woline = await _UoW.BaseGenericRepository<WOOnboardingAssets>().Update(get_issue_woline);

                    /// update wolineissue and remove asset ids 
                    /// 
                    var get_temp_issue = _UoW.IssueRepository.GetwolineissueById(requestmodel.issue_details.wo_line_issue_id.Value);
                    get_temp_issue.original_woonboardingassets_id = null;
                    get_temp_issue.original_asset_id = null;
                    get_temp_issue.issue_title = requestmodel.issue_details.issue_title;
                    get_temp_issue.issue_description = requestmodel.issue_details.problem_description;
                    get_temp_issue.original_asset_id = null;
                    get_temp_issue.modified_at = DateTime.UtcNow;
                    get_temp_issue.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    await _UoW.BaseGenericRepository<WOLineIssue>().Update(get_temp_issue);


                }

                response = (int)ResponseStatusNumber.Success;
            }

            return response;
        }

        public async Task<WOOnboardingAssets> CreateIssueWOline(AddIssueByStepsRequestmodel requestmodel , TempAsset temp_asset)
        {
            WOOnboardingAssets issue_woline = new WOOnboardingAssets();
            try
            {
                WorkOrderService workorderservice = new WorkOrderService(_mapper);
                if (temp_asset != null)
                {
                    issue_woline.asset_id = temp_asset.asset_id;
                    issue_woline.asset_name = temp_asset.asset_name;
                    issue_woline.asset_class_name = temp_asset.InspectionTemplateAssetClass.asset_class_name;
                    issue_woline.asset_class_code = temp_asset.InspectionTemplateAssetClass.asset_class_code;
                    issue_woline.tempasset_id = temp_asset.tempasset_id;
                                        
                    // insert building data
                    issue_woline.building = temp_asset.TempFormIOBuildings.temp_formio_building_name;
                    issue_woline.floor = temp_asset.TempFormIOFloors.temp_formio_floor_name;
                    issue_woline.room = temp_asset.TempFormIORooms.temp_formio_room_name;
                    issue_woline.section = temp_asset.TempFormIOSections.temp_formio_section_name;
                    issue_woline.WOOBAssetTempFormIOBuildingMapping = new WOOBAssetTempFormIOBuildingMapping();
                    issue_woline.WOOBAssetTempFormIOBuildingMapping.temp_formiobuilding_id = temp_asset.temp_formiobuilding_id;
                    issue_woline.WOOBAssetTempFormIOBuildingMapping.temp_formiofloor_id = temp_asset.temp_formiofloor_id;
                    issue_woline.WOOBAssetTempFormIOBuildingMapping.temp_formioroom_id = temp_asset.temp_formioroom_id;
                    issue_woline.WOOBAssetTempFormIOBuildingMapping.temp_formiosection_id = temp_asset.temp_formiosection_id;
                    issue_woline.WOOBAssetTempFormIOBuildingMapping.created_at = DateTime.UtcNow;
                }
                issue_woline.created_at = DateTime.UtcNow;
                issue_woline.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                issue_woline.inspection_type = requestmodel.issue_details.inspection_type;
                issue_woline.new_issue_asset_type = requestmodel.issue_details.new_issue_asset_type;
                if (requestmodel.issue_details.asset_issue_id != null)
                {
                    issue_woline.is_wo_line_for_exisiting_asset = true;
                }
                if(requestmodel.issue_details.issue_woline_status == (int)Status.Ready_for_review)
                {
                    issue_woline.technician_user_id = UpdatedGenericRequestmodel.CurrentUser.requested_by;
                }

                issue_woline.issue_priority = requestmodel.issue_details.priority;
                issue_woline.issue_title = requestmodel.issue_details.issue_title;
                issue_woline.problem_description = requestmodel.issue_details.problem_description;
                issue_woline.solution_description = requestmodel.issue_details.resolution_description;
                issue_woline.inspection_further_details = requestmodel.issue_details.inspection_further_details;
                issue_woline.replaced_asset_id = requestmodel.issue_details.replaced_asset_id;
                issue_woline.is_replaced_asset_id_is_main = requestmodel.issue_details.is_replaced_asset_id_is_main;

                issue_woline.repair_resolution = null;
                issue_woline.replacement_resolution = null;
                issue_woline.general_issue_resolution = null;

                if (requestmodel.issue_details.inspection_type == (int)MWO_inspection_wo_type.Repair)
                    issue_woline.repair_resolution = requestmodel.issue_details.resolution_type;
                if (requestmodel.issue_details.inspection_type == (int)MWO_inspection_wo_type.Replace)
                    issue_woline.replacement_resolution = requestmodel.issue_details.resolution_type;
                if (requestmodel.issue_details.inspection_type == (int)MWO_inspection_wo_type.Trouble_Call_Check)
                    issue_woline.general_issue_resolution = requestmodel.issue_details.resolution_type;

                issue_woline.status = requestmodel.issue_details.issue_woline_status;
                issue_woline.wo_id = requestmodel.issue_details.wo_id;
                #region map images from main issue to woline 

                issue_woline.WOOnboardingAssetsImagesMapping = new List<WOOnboardingAssetsImagesMapping>();
                // first map images which are from main issue and copy from issue bucket to woline bucket
                if (requestmodel.issue_details.issue_images != null)
                {
                    var get_main_issue_images = requestmodel.issue_details.issue_images.Where(x => x.asset_issue_image_mapping_id != null).ToList();
                    foreach (var image in get_main_issue_images)
                    {
                        WOOnboardingAssetsImagesMapping WOOnboardingAssetsImagesMapping = new WOOnboardingAssetsImagesMapping();
                        WOOnboardingAssetsImagesMapping.asset_photo = image.image_file_name;
                        WOOnboardingAssetsImagesMapping.asset_thumbnail_photo = image.image_thumbnail_file_name;
                        WOOnboardingAssetsImagesMapping.created_at = DateTime.UtcNow;
                        WOOnboardingAssetsImagesMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        WOOnboardingAssetsImagesMapping.asset_photo_type = (int)AssetPhotoType.Asset_Profile;
                        WOOnboardingAssetsImagesMapping.image_duration_type_id = image.image_duration_type_id;
                        WOOnboardingAssetsImagesMapping.is_deleted = image.is_deleted;
                        issue_woline.WOOnboardingAssetsImagesMapping.Add(WOOnboardingAssetsImagesMapping);



                        try
                        {
                            // copy image from Issue bucket to conduit-dev-assetimages
                            s3BucketService.CopyImagesToAnotherBucket(ConfigurationManager.AppSettings["AWS_ACCESS_KEY"], ConfigurationManager.AppSettings["AWS_SECRET_KEY"], ConfigurationManager.AppSettings["issue_photos_bucket"], ConfigurationManager.AppSettings["asset_bucket_name"], image.image_file_name);
                            s3BucketService.CopyImagesToAnotherBucket(ConfigurationManager.AppSettings["AWS_ACCESS_KEY"], ConfigurationManager.AppSettings["AWS_SECRET_KEY"], ConfigurationManager.AppSettings["issue_photos_bucket"], ConfigurationManager.AppSettings["asset_bucket_name"], image.image_thumbnail_file_name);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    // now add images which are not in main issue but added additionalyy when creating woline so those images will be in woline bucket so no need to move 
                    var get_additional_images = requestmodel.issue_details.issue_images.Where(x => x.asset_issue_image_mapping_id == null && !x.is_deleted).ToList();
                    foreach (var image in get_additional_images)
                    {
                        WOOnboardingAssetsImagesMapping WOOnboardingAssetsImagesMapping = new WOOnboardingAssetsImagesMapping();
                        WOOnboardingAssetsImagesMapping.asset_photo = image.image_file_name;
                        WOOnboardingAssetsImagesMapping.asset_thumbnail_photo = image.image_thumbnail_file_name;
                        WOOnboardingAssetsImagesMapping.created_at = DateTime.UtcNow;
                        WOOnboardingAssetsImagesMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        WOOnboardingAssetsImagesMapping.asset_photo_type = (int)AssetPhotoType.Asset_Profile;
                        WOOnboardingAssetsImagesMapping.image_duration_type_id = image.image_duration_type_id;
                        issue_woline.WOOnboardingAssetsImagesMapping.Add(WOOnboardingAssetsImagesMapping);
                    }
                    


                }
               

                #endregion map images from main issue to woline

                //insert issue woline
                var insert = await _UoW.BaseGenericRepository<WOOnboardingAssets>().Insert(issue_woline);
                _UoW.SaveChanges();

                //update issue woline in main issue table 
                if (requestmodel.issue_details.asset_issue_id != null)
                {
                    var get_asset_issue = _UoW.IssueRepository.GetMainIssue(requestmodel.issue_details.asset_issue_id.Value);
                    get_asset_issue.wo_id = issue_woline.wo_id;
                    get_asset_issue.is_issue_linked = true;
                    get_asset_issue.woonboardingassets_id = issue_woline.woonboardingassets_id;
                    get_asset_issue.modified_at = DateTime.UtcNow;
                    get_asset_issue.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();


                    if (get_asset_issue.issue_type == (int)WOLine_Temp_Issue_Type.Replace) //ReplaceMent 
                    {
                        get_asset_issue.Asset.asset_operating_condition_state = (int)AssetOperatingConduitionState.Replacement_Scheduled;
                    }
                    else  // Repair / Thermal / compliance
                    {
                        if (get_asset_issue.Asset.asset_operating_condition_state != (int)AssetOperatingConduitionState.Replacement_Needed
                            && get_asset_issue.Asset.asset_operating_condition_state != (int)AssetOperatingConduitionState.Replace_Inprogress
                            && get_asset_issue.Asset.asset_operating_condition_state != (int)AssetOperatingConduitionState.Replacement_Scheduled
                        ) //  check asset condition is not replace
                        {
                            get_asset_issue.Asset.asset_operating_condition_state = (int)AssetOperatingConduitionState.Repair_Scheduled;
                        }
                    }

                    var update = await _UoW.BaseGenericRepository<AssetIssue>().Update(get_asset_issue);
                    _UoW.SaveChanges();
                    // update Issue condition based on WO line 
                    await workorderservice.UpdateIssueStatusBasedonWOline(get_asset_issue.asset_issue_id);
                    // update asset condition based on issue condition 
                    //await workorderservice.ChangeAssetConditionBasedOnIssue(get_asset_issue.asset_issue_id);
                }
                

            }
            catch (Exception ex)
            {

            }
            
            return issue_woline;
        }

        public async Task<UploadAssettoOBWOResponsemodel> UpdateIssueWoline(UpdateOBWOAssetDetailsRequestmodel requestmodel)
        {
            WorkOrderService woservice = new WorkOrderService(_mapper);
            requestmodel.is_request_from_issue_service = true;
            var create_woline = await woservice.UpdateOBWOAssetDetails(requestmodel);

            return create_woline;
        }

        public async Task<UploadAssettoOBWOResponsemodel> CreateInstallWoline(UpdateOBWOAssetDetailsRequestmodel requestmodel)
        {
            WorkOrderService woservice = new WorkOrderService(_mapper);
            requestmodel.is_request_from_issue_service = true;
            requestmodel.is_woline_from_other_inspection = true;// if install woline is from issue/PM then this will be true and do not show this in wo datails screen
            var create_woline = await woservice.UpdateOBWOAssetDetails(requestmodel);

            return create_woline;
        }

        public async Task<Guid> CreateUpdateTempAsset(UpdateOBWOAssetDetailsRequestmodel requestmodel , TempAsset temp_asset)
        {
            WorkOrderService woservice = new WorkOrderService(_mapper);
            var temp_asset_id = await WOservice.AddTempAssetData(requestmodel, temp_asset);
           // var temp_asset_id =await AddTempAssetData(requestmodel, temp_asset);
            return temp_asset_id;
        }


        public ListViewModel<GetAllAssetIssuesOptimizedResponsemodel> GetAllAssetIssuesOptimized(GetAllAssetIssuesRequestmodel requestmodel)
        {
            ListViewModel<GetAllAssetIssuesOptimizedResponsemodel> IssueList = new ListViewModel<GetAllAssetIssuesOptimizedResponsemodel>();
            var issueList = _UoW.IssueRepository.GetAllAssetIssuesOptimized(requestmodel);
            if (issueList.Item1 != null && issueList.Item1.Count > 0)
            {
                IssueList.list = issueList.Item1;
                IssueList.pageSize = requestmodel.page_size;
                IssueList.pageIndex = requestmodel.page_index;
                IssueList.listsize = issueList.Item2;
            }
            return IssueList;
        }

        public GetAllIssueByWOidResponsemodel GetAllIssueByWOidOptimized(GetAllIssueByWOidRequestmodel requestmodel)
        {
            GetAllIssueByWOidResponsemodel response = new GetAllIssueByWOidResponsemodel();

            // temp issue
            var issueList = _UoW.IssueRepository.GetAlltempIssuebyWOidOptimized(requestmodel);
            if (issueList != null && issueList.Count > 0)
            {
                response.temp_issue_list = issueList;
            }
            //main issue
            var main_issue_list = _UoW.IssueRepository.GetAllmainIssuebyWOidOptimized(requestmodel);
            if (main_issue_list != null)
            {
                response.main_issue_list = main_issue_list;
            }
            return response;
        }

        public async Task<int> UpdateIRVisualImageLabel(UpdateIRVisualImageLabelRequestModel requestmodel)
        {
            int res = (int)ResponseStatusNumber.Error;
            try
            {
                var get_IRWOImageLabelMapping = _UoW.IssueRepository.GetIRWOImageLabelMappingById(requestmodel.irwoimagelabelmapping_id);
                if (get_IRWOImageLabelMapping != null)
                {
                    get_IRWOImageLabelMapping.ir_image_label = requestmodel.ir_image_label;
                    get_IRWOImageLabelMapping.visual_image_label = requestmodel.visual_image_label;
                    get_IRWOImageLabelMapping.updated_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    get_IRWOImageLabelMapping.updated_at = DateTime.UtcNow;

                    var update = await _UoW.BaseGenericRepository<IRWOImagesLabelMapping>().Update(get_IRWOImageLabelMapping);
                    if (update)
                    {
                        res = (int)ResponseStatusNumber.Success;
                        _UoW.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return res;
        }


        public async Task<GenerateOnboardingWOReportResponseModel> GenerateSiteIssuesReport(string report_type)
        {
            WorkOrderService workOrderService = new WorkOrderService(_mapper);
            GenerateOnboardingWOReportResponseModel responseModel = new GenerateOnboardingWOReportResponseModel();
            try
            {
                HttpClient client = new HttpClient();

                string base_url = ConfigurationManager.AppSettings["eg_ai_nameplate_info_url"] + "site-reporting/issues-report";

                GenerateSiteIssuesReportRequestModel req_model = new GenerateSiteIssuesReportRequestModel();
                req_model.reportType = report_type;
                req_model.companyId = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                req_model.siteId = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                var getuser = _UoW.UserRepository.GetUserByUserID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                req_model.userEmail = getuser != null ? getuser.email : null;

                // Set a timeout for 1 minute
                var timeout = TimeSpan.FromSeconds(45);
                var startTime = DateTime.UtcNow;
                var content = new StringContent(JsonConvert.SerializeObject(req_model), Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(base_url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseData = response.Content.ReadAsStringAsync().Result.ToString();
                    RootObj_GenerateReport_API json = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObj_GenerateReport_API>(responseData);
                    var task_id = json.data.taskId;
                    if (task_id > 0)
                    {
                        responseModel.status = (int)Status.Completed;
                    }
                }
                /*
                while (DateTime.UtcNow - startTime < timeout)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = response.Content.ReadAsStringAsync().Result.ToString();
                        RootObj_GenerateReport_API json = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObj_GenerateReport_API>(responseData);
                        var task_id = json.data.taskId;
                        var response_getapi = await CallPythonAPI_TaskStatus(client, task_id);
                        responseModel.status = response_getapi.status;

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
                }*/

                // Check if the loop ended due to timeout
                //if (DateTime.UtcNow - startTime >= timeout)
                //{
                //    responseModel.status = (int)ResponseStatusNumber.TimedOut;
                //}
            }
            catch (Exception e)
            {
            }
            return responseModel;
        }
        public async Task<GenerateOnboardingWOReportResponseModel> CallPythonAPI_TaskStatus(HttpClient client, int task_id)
        {
            GenerateOnboardingWOReportResponseModel responsemodel = new GenerateOnboardingWOReportResponseModel();
            int res = (int)ResponseStatusNumber.Error;
            string report_url = null;
            try
            {
                string base_url = ConfigurationManager.AppSettings["eg_ai_nameplate_info_url"] + "task/status/" + task_id;

                HttpResponseMessage response = await client.GetAsync(base_url);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    RootObj2_Status_API json = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObj2_Status_API>(responseData);

                    if (json.data.status == "completed")
                    {
                        res = (int)Status.Completed;
                        report_url = json.data.result.result;//Newtonsoft.Json.JsonConvert.SerializeObject();
                    }
                    else if (json.data.status == "running")
                    {
                        res = (int)Status.running;
                    }
                    else if (json.data.status == "failed")
                    {
                        res = (int)Status.ReportFailed;
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
            responsemodel.report_url = report_url;
            return responsemodel;
        }
    }
}
