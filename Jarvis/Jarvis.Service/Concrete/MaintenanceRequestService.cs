using AutoMapper;
using Jarvis.db.Models;
using Jarvis.Service.Abstract;
using Jarvis.Service.Notification;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Concrete
{
    public class MaintenanceRequestService : BaseService, IMaintenanceRequestService
    {
        public readonly IMapper _mapper;
        private Logger _logger;

        public MaintenanceRequestService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            _logger = Logger.GetInstance<MaintenanceRequestService>();
        }

        public async Task<MRResponseModel> AddUpdateMaintenanceReq(AddMRRequestModel mrRequest)
        {
            MRResponseModel mrResponse = new MRResponseModel();
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    if (mrRequest.mr_id != null && mrRequest.mr_id != Guid.Empty)
                    {
                        var mrDetails = await _UoW.MRRepository.GetMRsById(mrRequest.mr_id);
                        if (mrDetails != null)
                        {
                            mrDetails.description = mrRequest.description;
                            mrDetails.priority = mrRequest.priority;
                            mrDetails.wo_id = mrRequest.wo_id;
                            if (mrRequest.wo_id != null)
                            {
                                mrDetails.status = (int)Status.MRWorkOrderCreated;
                            }
                            else if (mrRequest.wo_id == null)
                            {
                                mrDetails.status = (int)Status.MROpen;
                            }
                            mrDetails.modified_at = DateTime.UtcNow;
                            mrDetails.modified_by = GenericRequestModel.requested_by.ToString();

                            result = _UoW.MRRepository.Update(mrDetails).Result;
                            if (result > 0)
                            {
                                _UoW.SaveChanges();
                                _dbtransaction.Commit();
                                mrResponse = _mapper.Map<MRResponseModel>(mrDetails);
                                mrResponse.response_status = result;
                            }
                        }
                    }
                    else
                    {
                        var assetDetails = _UoW.AssetRepository.GetAssetByAssetID(mrRequest.asset_id.ToString());
                        var addMR = _mapper.Map<MaintenanceRequests>(mrRequest);
                        addMR.status = (int)Status.MROpen;
                        addMR.mr_type = (int)Status.Manual;
                        addMR.is_archive = false;
                        addMR.requested_by = GenericRequestModel.requested_by;
                        addMR.site_id = assetDetails.site_id;
                        addMR.created_by = GenericRequestModel.requested_by.ToString();
                        addMR.created_at = DateTime.UtcNow;
                        result = await _UoW.MRRepository.Insert(addMR);
                        if (result > 0)
                        {
                            _UoW.SaveChanges();
                            _dbtransaction.Commit();
                            mrResponse = _mapper.Map<MRResponseModel>(addMR);
                            mrResponse.response_status = result;
                        }
                        else
                        {
                            mrResponse.response_status = result;
                        }
                    }

                }
                catch (Exception ex)
                {
                    _dbtransaction.Rollback();
                    mrResponse.response_status = (int)ResponseStatusNumber.Error;
                }
            }

            return mrResponse;
        }

        public async Task<ListViewModel<MRResponseModel>> GetAllMaintenanceRequest(GetAllMRRequestModel requestModel)
        {
            ListViewModel<MRResponseModel> MRList = new ListViewModel<MRResponseModel>();
            try
            {
                // get all Maintenance Request by site
                var mrItems = await _UoW.MRRepository.GetAllMaintenanceRequest(requestModel);

                if (mrItems?.list?.Count > 0)
                {
                    int totalmr = mrItems.list.Count;
                    if (requestModel.pageSize > 0 && requestModel.pageIndex > 0)
                    {
                        mrItems.list = mrItems.list.Skip((requestModel.pageIndex - 1) * requestModel.pageSize).Take(requestModel.pageSize).ToList();
                    }

                    MRList.list = _mapper.Map<List<MRResponseModel>>(mrItems.list);
                    foreach (var item in MRList.list)
                    {
                        var user = await _UoW.UserRepository.GetUserByID(item.requested_by.ToString());
                        item.requested_by_name = user.firstname + " " + user.lastname;

                        if (item.WorkOrders != null && item.WorkOrders.status != 0)
                        {
                            item.work_order_number = item.WorkOrders.wo_number.ToString();
                            item.work_order_status = item.WorkOrders.status;
                        }
                        else
                        {
                            item.work_order_status = 0;
                        }

                        //When maintenance request is for Inspection Issue
                        if (item.mr_type == (int)Status.Inspection && item.mr_type_id != null)
                        {
                            var inspection = _UoW.MRRepository.GetInspectionIdByIssueId((Guid)item.mr_type_id);
                            item.inspection_id = inspection.inspection_id;
                            item.meter_at_inspection = inspection.meter_hours.ToString();
                        }
                    }

                    MRList.listsize = totalmr;
                    MRList.pageIndex = requestModel.pageIndex;
                    MRList.pageSize = requestModel.pageSize;
                    MRList.result = (int)ResponseStatusNumber.Success;
                }
                else
                {
                    MRList.result = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                MRList.result = (int)ResponseStatusNumber.Error;
            }
            return MRList;
        }

        public async Task<GetOpenMRCount> MaintenanceRequestOpenStatusCount()
        {
            GetOpenMRCount resModel = new GetOpenMRCount();
            try
            {
                var openMaintenanceRequests = await _UoW.MRRepository.MaintenanceRequestOpenStatusCount(GenericRequestModel.requested_by.ToString());

                resModel.openMRCount = 0;

                if (openMaintenanceRequests != null)
                {
                    resModel.openMRCount = openMaintenanceRequests.Count;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return resModel;
        }

        public async Task<int> CancelMaintencanceRequest(MRCancelRequestModel cancelModel)
        {
            bool result = false;
            int responseresult = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    if (cancelModel.mr_id != null)
                    {
                        cancelModel.is_archive = false;
                        cancelModel.created_at = DateTime.UtcNow;
                        cancelModel.modified_at = DateTime.UtcNow;
                        cancelModel.created_by = GenericRequestModel.requested_by.ToString();
                        var cancelRequest = _mapper.Map<MaintenanceReqCancelRequests>(cancelModel);
                        result = await _UoW.BaseGenericRepository<MaintenanceReqCancelRequests>().Insert(cancelRequest);
                        if (result)
                        {
                            _UoW.SaveChanges();
                            var mrDetails = await _UoW.MRRepository.GetMRsById(cancelModel.mr_id);
                            if (mrDetails != null)
                            {
                                mrDetails.status = (int)Status.MRCancelled;
                                int mrresult = await _UoW.MRRepository.Update(mrDetails);
                                if (mrresult > 0)
                                {
                                    _UoW.SaveChanges();
                                    _dbtransaction.Commit();
                                    responseresult = (int)ResponseStatusNumber.Success;
                                }
                                else
                                {
                                    _dbtransaction.Rollback();
                                    responseresult = (int)ResponseStatusNumber.Error;
                                }
                            }
                            else
                            {
                                _dbtransaction.Rollback();
                                responseresult = (int)ResponseStatusNumber.Error;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    _logger.LogError(e.Message);
                    throw e;
                }
            }
            return responseresult;
        }

        public async Task<ListViewModel<WorkOrderResponseModel>> GetAllWorkOrderWithNoMR(string assetid, string searchstring, int pageindex, int pagesize)
        {
            ListViewModel<WorkOrderResponseModel> resModel = new ListViewModel<WorkOrderResponseModel>();
            try
            {
                var woRequests = await _UoW.MRRepository.GetAllWorkOrderWithNoMR(assetid, searchstring);
                if (woRequests?.Count > 0)
                {
                    resModel.listsize = woRequests.Count();
                    if (pageindex > 0 && pagesize > 0)
                    {
                        woRequests = woRequests.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    }
                    resModel.list = _mapper.Map<List<WorkOrderResponseModel>>(woRequests);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return resModel;
        }

        public async Task<MRResponseModel> GetMaintenanceRequestById(Guid mr_id)
        {
            MRResponseModel resModel = new MRResponseModel();
            try
            {
                var mrRequests = await _UoW.MRRepository.GetMRsById(mr_id);
                if (mrRequests != null)
                {
                    resModel = _mapper.Map<MRResponseModel>(mrRequests);
                    if (resModel != null)
                    {
                        resModel.response_status = (int)ResponseStatusNumber.Success;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return resModel;
        }

        public async Task<int> ResolveMaintenanceRequest(ResolveMaintenanceRequestModel requestModel)
        {
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    if (requestModel.mr_id != null && requestModel.mr_id != Guid.Empty)
                    {
                        var mrDetails = await _UoW.MRRepository.GetMRsById(requestModel.mr_id);
                        if (mrDetails != null)
                        {
                            mrDetails.status = (int)Status.MRCompeleted;
                            mrDetails.resolved_at = DateTime.UtcNow;
                            mrDetails.resolve_reason = requestModel.resolve_reason;
                            mrDetails.modified_at = DateTime.UtcNow;
                            mrDetails.modified_by = GenericRequestModel.requested_by.ToString();

                            result = _UoW.MRRepository.Update(mrDetails).Result;
                            if (result > 0)
                            {
                                if (mrDetails.Issue?.Count > 0)
                                {
                                    foreach (var issue in mrDetails.Issue)
                                    {
                                        var issueDetails = _UoW.IssueRepository.GetIssueByIssueId(issue.issue_uuid);
                                        if (issueDetails != null)
                                        {
                                            issueDetails.status = (int)Status.Completed;
                                            issueDetails.modified_at = DateTime.UtcNow;
                                            issueDetails.modified_by = GenericRequestModel.requested_by.ToString();
                                            issueDetails.IssueRecord.status = (int)Status.Completed;
                                            issueDetails.IssueRecord.fixed_datetime = DateTime.UtcNow;
                                            issueDetails.IssueRecord.fixed_by = GenericRequestModel.requested_by.ToString();

                                            bool updateresult = await _UoW.IssueRepository.Update(issueDetails);

                                            if (updateresult)
                                            {
                                                IssueStatus issueStatus = new IssueStatus();
                                                issueStatus.issue_id = issueDetails.issue_uuid;
                                                issueStatus.status = (int)Status.Completed;
                                                issueStatus.modified_at = DateTime.UtcNow;
                                                issueStatus.modified_by = GenericRequestModel.requested_by.ToString();

                                                var response = _UoW.IssueRepository.CreateIssueStatus(issueStatus);

                                                if (response > 0)
                                                {
                                                    _UoW.SaveChanges();

                                                    var activityLogs = NotificationGenerator.IssueResolved(issueDetails.Asset.name, issueDetails.Asset.meter_hours.Value.ToString(), issueDetails.name);
                                                    activityLogs.asset_id = issueDetails.asset_id;
                                                    activityLogs.created_at = DateTime.UtcNow;
                                                    activityLogs.ref_id = issueDetails.issue_uuid.ToString();
                                                    activityLogs.updated_by = GenericRequestModel.requested_by.ToString();
                                                    activityLogs.site_id = Guid.Parse(GenericRequestModel.site_id);
                                                    var res = await _UoW.BaseGenericRepository<AssetActivityLogs>().Update(activityLogs);
                                                    if (res == true)
                                                    {
                                                        _UoW.SaveChanges();
                                                    }
                                                    result = (int)ResponseStatusNumber.Success;
                                                }
                                                else
                                                {
                                                    _dbtransaction.Rollback();
                                                    result = (int)ResponseStatusNumber.Error;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (result > 0)
                                {
                                    _dbtransaction.Commit();
                                }
                            }
                        }
                        else
                        {
                            return (int)ResponseStatusNumber.NotFound;
                        }
                    }
                    else
                    {
                        return (int)ResponseStatusNumber.InvalidData;
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    throw e;
                }
            }
            return result;
        }

        //public async Task<ListViewModel<MRResponseModel>> FilterTypeOptions(FilterMaintenanceRequestModel requestModel)
        //{
        //    ListViewModel<MRResponseModel> filterResponse = new ListViewModel<MRResponseModel>();
        //    try
        //    {
        //        var response = await _UoW.MRRepository.FilterTypeOptions(requestModel);
        //        if (response?.list?.Count > 0)
        //        {
        //            filterResponse.list = _mapper.Map<List<MRResponseModel>>(response.list);
        //            filterResponse.listsize = response.listsize;
        //            filterResponse.pageIndex = requestModel.pageIndex;
        //            filterResponse.pageSize = requestModel.pageSize;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    return filterResponse;
        //}

        public async Task<ListViewModel<MRResponseModel>> FilterRequestedByOptions(FilterMaintenanceRequestModel requestModel)
        {
            ListViewModel<MRResponseModel> filterResponse = new ListViewModel<MRResponseModel>();
            try
            {
                var response = await _UoW.MRRepository.FilterRequestedByOptions(requestModel);
                if (response?.list?.Count > 0)
                {
                    var distinctValue = response.list.Select(x => x.requested_by).ToList().Distinct().ToList();
                    //filterResponse.list = _mapper.Map<List<MRResponseModel>>(response.list);
                    if (requestModel.pageIndex == 0 || requestModel.pageSize == 0)
                    {
                        requestModel.pageSize = 20;
                        requestModel.pageIndex = 1;
                    }
                    response.list = response.list.Skip((requestModel.pageIndex - 1) * requestModel.pageSize).Take(requestModel.pageSize).OrderBy(x => x.requested_by).ToList();
                    foreach (var item in distinctValue)
                    {
                        MRResponseModel model = new MRResponseModel();
                        var user = await _UoW.UserRepository.GetUserByID(item.ToString());
                        model.requested_by = item;
                        model.requested_by_name = user.firstname + " " + user.lastname;
                        filterResponse.list.Add(model);
                    }
                    filterResponse.listsize = response.list.Count;
                    filterResponse.pageIndex = requestModel.pageIndex;
                    filterResponse.pageSize = requestModel.pageSize;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return filterResponse;
        }
    }
}
