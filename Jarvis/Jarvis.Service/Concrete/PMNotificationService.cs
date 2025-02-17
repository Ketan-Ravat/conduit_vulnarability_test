using AutoMapper;
using Jarvis.db.Models;
using Jarvis.Service.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Concrete {
    public class PMNotificationService : BaseService, IPMNotificationService {
        public readonly IMapper _mapper;
        private Logger _logger;
        NotificationService notificationService = null;

        public PMNotificationService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            _logger = Logger.GetInstance<PMNotificationService>();
        }

        public async Task<CompanyPMNotificationResponseModel> AddUpdatePMNotification(CompanyPMNotificationRequestModel request)
        {
            CompanyPMNotificationResponseModel notiResponse = new CompanyPMNotificationResponseModel();
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {

                    var notificationDetail = await _UoW.PMNotificationRepository.GetByCompanyId(request.company_id);
                    if (notificationDetail != null)
                    {
                        notificationDetail.modified_at = DateTime.UtcNow;
                        notificationDetail.modified_by = GenericRequestModel.requested_by.ToString();
                        notificationDetail.first_reminder_before_on = request.first_reminder_before_on;
                        notificationDetail.first_reminder_before_on_type = request.first_reminder_before_on_type;
                        notificationDetail.first_reminder_before_on_status = request.first_reminder_before_on_status;
                        notificationDetail.second_reminder_before_on = request.second_reminder_before_on;
                        notificationDetail.second_reminder_before_on_status = request.second_reminder_before_on_status;
                        notificationDetail.second_reminder_before_on_type = request.second_reminder_before_on_type;
                        notificationDetail.due_at_reminder_status = request.due_at_reminder_status;
                        notificationDetail.first_reminder_before_on_meter_hours = request.first_reminder_before_on_meter_hours;
                        notificationDetail.first_reminder_before_on_meter_hours_status = request.first_reminder_before_on_meter_hours_status;
                        notificationDetail.second_reminder_before_on_meter_hours = request.second_reminder_before_on_meter_hours;
                        notificationDetail.second_reminder_before_on_meter_hours_status = request.second_reminder_before_on_meter_hours_status;
                        notificationDetail.due_at_reminder_meter_hours_status = request.due_at_reminder_meter_hours_status;
                        result = await _UoW.PMNotificationRepository.Update(notificationDetail);
                        if (result > 0)
                        {
                            _UoW.SaveChanges();
                            _dbtransaction.Commit();
                            notiResponse.response_status = result;
                        }
                        else
                        {
                            notiResponse.response_status = result;
                        }
                    }
                    else
                    {
                        var addPMNotification = _mapper.Map<CompanyPMNotificationConfigurations>(request);
                        addPMNotification.created_by = GenericRequestModel.requested_by.ToString();
                        addPMNotification.created_at = DateTime.UtcNow;
                        addPMNotification.status = (int)Status.Active;
                        result = await _UoW.PMNotificationRepository.Insert(addPMNotification);
                        if (result > 0)
                        {
                            _UoW.SaveChanges();
                            _dbtransaction.Commit();
                            notiResponse = _mapper.Map<CompanyPMNotificationResponseModel>(addPMNotification);
                            notiResponse.response_status = result;
                        }
                        else
                        {
                            notiResponse.response_status = result;
                        }
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    notiResponse.response_status = (int)ResponseStatusNumber.Error;
                    _logger.LogError(e.Message);
                }
            }

            return notiResponse;
        }

        public async Task<AssetPMNotificationResponseModel> AddUpdateAssetPMNotification(AssetPMNotificationRequestModel request)
        {
            AssetPMNotificationResponseModel notiResponse = new AssetPMNotificationResponseModel();
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {

                    var notificationDetail = await _UoW.PMNotificationRepository.GetByAssetId(request.asset_id);
                    if (notificationDetail != null)
                    {
                        notificationDetail.modified_at = DateTime.UtcNow;
                        notificationDetail.modified_by = GenericRequestModel.requested_by.ToString();
                        notificationDetail.first_reminder_before_on = request.first_reminder_before_on;
                        notificationDetail.first_reminder_before_on_type = request.first_reminder_before_on_type;
                        notificationDetail.first_reminder_before_on_status = request.first_reminder_before_on_status;
                        notificationDetail.second_reminder_before_on = request.second_reminder_before_on;
                        notificationDetail.second_reminder_before_on_status = request.second_reminder_before_on_status;
                        notificationDetail.second_reminder_before_on_type = request.second_reminder_before_on_type;
                        notificationDetail.due_at_reminder_status = request.due_at_reminder_status;
                        notificationDetail.first_reminder_before_on_meter_hours = request.first_reminder_before_on_meter_hours;
                        notificationDetail.first_reminder_before_on_meter_hours_status = request.first_reminder_before_on_meter_hours_status;
                        notificationDetail.second_reminder_before_on_meter_hours = request.second_reminder_before_on_meter_hours;
                        notificationDetail.second_reminder_before_on_meter_hours_status = request.second_reminder_before_on_meter_hours_status;
                        notificationDetail.due_at_reminder_meter_hours_status = request.due_at_reminder_meter_hours_status;
                        bool response = await _UoW.BaseGenericRepository<AssetPMNotificationConfigurations>().Update(notificationDetail);
                        if (response)
                        {
                            result = (int)ResponseStatusNumber.Success;
                            _UoW.SaveChanges();
                            _dbtransaction.Commit();
                            notiResponse.response_status = result;
                        }
                        else
                        {
                            result = (int)ResponseStatusNumber.Error;
                            notiResponse.response_status = result;
                        }
                    }
                    else
                    {
                        var addPMNotification = _mapper.Map<AssetPMNotificationConfigurations>(request);
                        addPMNotification.created_by = GenericRequestModel.requested_by.ToString();
                        addPMNotification.created_at = DateTime.UtcNow;
                        addPMNotification.status = (int)Status.Active;
                        bool response = await _UoW.BaseGenericRepository<AssetPMNotificationConfigurations>().Insert(addPMNotification);
                        if (response)
                        {
                            result = (int)ResponseStatusNumber.Success;
                            _UoW.SaveChanges();
                            _dbtransaction.Commit();
                            notiResponse = _mapper.Map<AssetPMNotificationResponseModel>(addPMNotification);
                            notiResponse.response_status = result;
                        }
                        else
                        {
                            result = (int)ResponseStatusNumber.Error;
                            notiResponse.response_status = result;
                        }
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    notiResponse.response_status = (int)ResponseStatusNumber.Error;
                    _logger.LogError(e.Message);
                }
            }

            return notiResponse;
        }

        public async Task<CompanyPMNotificationResponseModel> GetPMNotification(Guid company_id)
        {
            CompanyPMNotificationResponseModel notiResponse = new CompanyPMNotificationResponseModel();
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {

                    var notificationDetail = await _UoW.PMNotificationRepository.GetByCompanyId(company_id);
                    if (notificationDetail != null)
                    {
                        notiResponse = _mapper.Map<CompanyPMNotificationResponseModel>(notificationDetail);
                        notiResponse.response_status = (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        notiResponse.response_status = (int)ResponseStatusNumber.NotFound;
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    notiResponse.response_status = (int)ResponseStatusNumber.Error;
                    _logger.LogError(e.Message);
                }
            }

            return notiResponse;
        }

        public async Task<AssetPMNotificationResponseModel> GetAssetPMNotification(Guid asset_id)
        {
            AssetPMNotificationResponseModel notiResponse = new AssetPMNotificationResponseModel();
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {

                    var notificationDetail = await _UoW.PMNotificationRepository.GetByAssetId(asset_id);
                    if (notificationDetail != null)
                    {
                        notiResponse = _mapper.Map<AssetPMNotificationResponseModel>(notificationDetail);
                        notiResponse.response_status = (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        var asset = _UoW.AssetRepository.GetAssetByAssetID(asset_id.ToString());
                        if (asset != null)
                        {
                            var companyNotificationDetail = await _UoW.PMNotificationRepository.GetByCompanyId(asset.Sites.company_id);
                            if (companyNotificationDetail != null)
                            {
                                notiResponse = _mapper.Map<AssetPMNotificationResponseModel>(companyNotificationDetail);
                                notiResponse.response_status = (int)ResponseStatusNumber.Success;
                            }
                            else
                            {
                                notiResponse.response_status = (int)ResponseStatusNumber.NotFound;
                            }
                        }
                        else
                        {
                            notiResponse.response_status = (int)ResponseStatusNumber.NotFound;
                        }
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    notiResponse.response_status = (int)ResponseStatusNumber.Error;
                    _logger.LogError(e.Message);
                }
            }

            return notiResponse;
        }

        public async Task<int> ExecutePMNotification()
        {
            int result = (int)ResponseStatusNumber.Error;
            if (notificationService == null)
            {
                notificationService = new NotificationService(_mapper);
            }
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var notificationDetail = await _UoW.PMNotificationRepository.GetAllPMNotificationConfiguration();
                    if (notificationDetail?.Count > 0)
                    {
                        foreach (var companyNotification in notificationDetail)
                        {
                            List<UserSites> managers = _UoW.UserRepository.GetAllManagerForPMNotifications(companyNotification.company_id);
                            var sitewisemanager = managers.GroupBy(x => x.user_id).ToList();
                            foreach (var manager in sitewisemanager)
                            {
                                foreach (var userSite in manager)
                                {
                                    var user = userSite.User;
                                    if (user != null)
                                    {
                                        if (user.manager_pm_notification_status)
                                        {
                                            var assets = _UoW.AssetRepository.GetAssetsBySiteID(userSite.site_id);
                                            if (assets?.Count > 0)
                                            {
                                                foreach (var asset in assets)
                                                {
                                                    if (asset.AssetPMNotificationConfigurations != null)
                                                    {
                                                        companyNotification.first_reminder_before_on = asset.AssetPMNotificationConfigurations.first_reminder_before_on;
                                                        companyNotification.first_reminder_before_on_type = asset.AssetPMNotificationConfigurations.first_reminder_before_on_type;
                                                        companyNotification.first_reminder_before_on_status = asset.AssetPMNotificationConfigurations.first_reminder_before_on_status;
                                                        companyNotification.second_reminder_before_on = asset.AssetPMNotificationConfigurations.second_reminder_before_on;
                                                        companyNotification.second_reminder_before_on_type = asset.AssetPMNotificationConfigurations.second_reminder_before_on_type;
                                                        companyNotification.second_reminder_before_on_status = asset.AssetPMNotificationConfigurations.second_reminder_before_on_status;
                                                        companyNotification.due_at_reminder_status = asset.AssetPMNotificationConfigurations.due_at_reminder_status;
                                                        companyNotification.first_reminder_before_on_meter_hours = asset.AssetPMNotificationConfigurations.first_reminder_before_on_meter_hours;
                                                        companyNotification.first_reminder_before_on_meter_hours_status = asset.AssetPMNotificationConfigurations.first_reminder_before_on_meter_hours_status;
                                                        companyNotification.second_reminder_before_on_meter_hours = asset.AssetPMNotificationConfigurations.second_reminder_before_on_meter_hours;
                                                        companyNotification.second_reminder_before_on_meter_hours_status = asset.AssetPMNotificationConfigurations.second_reminder_before_on_meter_hours_status;
                                                        companyNotification.due_at_reminder_meter_hours_status = asset.AssetPMNotificationConfigurations.due_at_reminder_meter_hours_status;
                                                        companyNotification.status = asset.AssetPMNotificationConfigurations.status;
                                                    }
                                                    ListViewModel<AssetPMResponseModel> pmResponse = new ListViewModel<AssetPMResponseModel>();
                                                    var pmDetails = asset.AssetPMs.Where(x => !x.is_archive && x.status != (int)Status.Completed).ToList();
                                                    if (pmDetails?.Count > 0)
                                                    {
                                                        foreach (var pm in pmDetails)
                                                        {
                                                            pm.PMTriggers = pm.PMTriggers.Where(x => !x.is_archive && x.status != (int)Status.TriggerCompleted).ToList();
                                                            foreach (var trigger in pm.PMTriggers)
                                                            {
                                                                // trigger is Time based
                                                                if (trigger.due_datetime != null)
                                                                {
                                                                    // check if first reminder is enabled
                                                                    if (companyNotification.first_reminder_before_on_status == (int)Status.Active)
                                                                    {
                                                                        var notificationSendDate = PMTriggersUtil.GetDateFromDueDate(companyNotification.first_reminder_before_on, companyNotification.first_reminder_before_on_type, trigger.due_datetime.Value);
                                                                        if (notificationSendDate == DateTime.UtcNow.Date)
                                                                        {
                                                                            // send Notification
                                                                            await notificationService.SendNotification((int)NotificationStatus.FirstDueDateReminder, trigger.asset_pm_id.ToString(), manager.Key.ToString());
                                                                        }
                                                                    }
                                                                    if (companyNotification.second_reminder_before_on_status == (int)Status.Active)
                                                                    {
                                                                        var notificationSendDate = PMTriggersUtil.GetDateFromDueDate(companyNotification.second_reminder_before_on, companyNotification.second_reminder_before_on_type, trigger.due_datetime.Value);
                                                                        if (notificationSendDate == DateTime.UtcNow.Date)
                                                                        {
                                                                            await notificationService.SendNotification((int)NotificationStatus.SecondDueDateReminder, trigger.asset_pm_id.ToString(), manager.Key.ToString());
                                                                        }
                                                                    }
                                                                    if (companyNotification.due_at_reminder_status == (int)Status.Active)
                                                                    {
                                                                        if (DateTime.UtcNow.Date == trigger.due_datetime.Value.Date)
                                                                        {
                                                                            await notificationService.SendNotification((int)NotificationStatus.OnDueDateReminder, trigger.asset_pm_id.ToString(), manager.Key.ToString());
                                                                        }
                                                                    }
                                                                }
                                                                // trigger is on Meter Hours
                                                                if (trigger.due_meter_hours != null && trigger.due_meter_hours > 0)
                                                                {
                                                                    var diffintInMeterHours = trigger.due_meter_hours - asset.meter_hours;
                                                                    if (companyNotification.first_reminder_before_on_meter_hours_status == (int)Status.Active)
                                                                    {
                                                                        if (diffintInMeterHours < companyNotification.first_reminder_before_on_meter_hours)
                                                                        {
                                                                            if (companyNotification.second_reminder_before_on_meter_hours_status == (int)Status.Active)
                                                                            {
                                                                                if (diffintInMeterHours > companyNotification.second_reminder_before_on_meter_hours)
                                                                                {
                                                                                    // send 1st Notification Meter hours reminder
                                                                                    await notificationService.SendNotification((int)NotificationStatus.FirstMeterHoursDueReminder, trigger.asset_pm_id.ToString(), manager.Key.ToString());
                                                                                }
                                                                                else if (diffintInMeterHours < companyNotification.second_reminder_before_on_meter_hours)
                                                                                {
                                                                                    if (companyNotification.due_at_reminder_meter_hours_status == (int)Status.Active)
                                                                                    {
                                                                                        if (diffintInMeterHours > 0)
                                                                                        {
                                                                                            // send 2nd Notification meter hours reminder
                                                                                            await notificationService.SendNotification((int)NotificationStatus.SecondMeterHoursDueReminder, trigger.asset_pm_id.ToString(), manager.Key.ToString());
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            // send Due meter hours notification reminder
                                                                                            await notificationService.SendNotification((int)NotificationStatus.OnMeterHoursDueReminder, trigger.asset_pm_id.ToString(), manager.Key.ToString());
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        // send 2nd Notification Meter hours reminder
                                                                                        await notificationService.SendNotification((int)NotificationStatus.SecondMeterHoursDueReminder, trigger.asset_pm_id.ToString(), manager.Key.ToString());
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                // send 1st notification meter hours reminder
                                                                                await notificationService.SendNotification((int)NotificationStatus.FirstMeterHoursDueReminder, trigger.asset_pm_id.ToString(), manager.Key.ToString());
                                                                            }
                                                                        }
                                                                    }
                                                                    else if (companyNotification.second_reminder_before_on_meter_hours_status == (int)Status.Active)
                                                                    {
                                                                        if (diffintInMeterHours < companyNotification.second_reminder_before_on_meter_hours)
                                                                        {
                                                                            if (companyNotification.due_at_reminder_meter_hours_status == (int)Status.Active)
                                                                            {
                                                                                if (diffintInMeterHours > 0)
                                                                                {
                                                                                    // send 2nd Notification meter hours reminder
                                                                                    await notificationService.SendNotification((int)NotificationStatus.SecondMeterHoursDueReminder, trigger.asset_pm_id.ToString(), manager.Key.ToString());
                                                                                }
                                                                                else
                                                                                {
                                                                                    // send Due meter hours notification reminder
                                                                                    await notificationService.SendNotification((int)NotificationStatus.OnMeterHoursDueReminder, trigger.asset_pm_id.ToString(), manager.Key.ToString());
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                // send 2nd Notification Meter hours reminder
                                                                                await notificationService.SendNotification((int)NotificationStatus.SecondMeterHoursDueReminder, trigger.asset_pm_id.ToString(), manager.Key.ToString());
                                                                            }
                                                                        }
                                                                    }
                                                                    else if (companyNotification.due_at_reminder_meter_hours_status == (int)Status.Active)
                                                                    {
                                                                        if (diffintInMeterHours <= 0)
                                                                        {
                                                                            // send on due reminder
                                                                            await notificationService.SendNotification((int)NotificationStatus.OnMeterHoursDueReminder, trigger.asset_pm_id.ToString(), manager.Key.ToString());
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        pmResponse.result = (int)ResponseStatusNumber.NotFound;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        result = (int)ResponseStatusNumber.NotFound;
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    result = (int)ResponseStatusNumber.Error;
                    _logger.LogError(e.Message);
                }
            }

            return result;
        }

        public async Task<int> TriggerPMItemNotification(Guid trigger_id, bool is_disabled)
        {
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var response = await _UoW.PMNotificationRepository.GetPMItemNotificationConfig(GenericRequestModel.requested_by, trigger_id);
                    if (response != null)
                    {
                        response.is_disabled = is_disabled;
                        var updated = await _UoW.BaseGenericRepository<ManagerPMNotificationConfiguration>().Update(response);
                        if (updated == true)
                        {
                            _dbtransaction.Commit();
                            result = (int)ResponseStatusNumber.Success;
                        }
                    }
                    else
                    {
                        ManagerPMNotificationConfiguration managerPMNotification = new ManagerPMNotificationConfiguration();
                        managerPMNotification.user_id = GenericRequestModel.requested_by;
                        managerPMNotification.pm_trigger_id = trigger_id;
                        managerPMNotification.is_disabled = is_disabled;
                        var inserted = await _UoW.BaseGenericRepository<ManagerPMNotificationConfiguration>().Insert(managerPMNotification);
                        if (inserted == true)
                        {
                            _UoW.SaveChanges();
                            _dbtransaction.Commit();
                            result = (int)ResponseStatusNumber.Success;
                        }
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    result = (int)ResponseStatusNumber.Error;
                    _logger.LogError(e.Message);
                }
            }

            return result;
        }


        public async Task<int> ExecutePMNotificationForVendors()
        {
            int result = (int)ResponseStatusNumber.Error;
            if (notificationService == null)
            {
                notificationService = new NotificationService(_mapper);
            }
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var notificationDetail = await _UoW.PMNotificationRepository.GetAllPMNotificationConfiguration();
                    if (notificationDetail?.Count > 0)
                    {
                        List<VendorPMEmailNotification> vendorPMEmailNotifications = new List<VendorPMEmailNotification>();
                        foreach (var companyNotification in notificationDetail)
                        {
                            List<ToSendVendorEmailExcelDetails> toSendVendors = new List<ToSendVendorEmailExcelDetails>();
                            List<Sites> AllActiveSites = await _UoW.SiteRepository.GetActiveSitesByCompanyID(companyNotification.company_id);
                            foreach (var site in AllActiveSites)
                            {
                                var assets = _UoW.AssetRepository.GetAssetsBySiteID(site.site_id);
                                if (assets?.Count > 0)
                                {
                                    foreach (var asset in assets)
                                    {
                                        if (asset.AssetPMNotificationConfigurations != null)
                                        {
                                            companyNotification.first_reminder_before_on = asset.AssetPMNotificationConfigurations.first_reminder_before_on;
                                            companyNotification.first_reminder_before_on_type = asset.AssetPMNotificationConfigurations.first_reminder_before_on_type;
                                            companyNotification.first_reminder_before_on_status = asset.AssetPMNotificationConfigurations.first_reminder_before_on_status;
                                            companyNotification.second_reminder_before_on = asset.AssetPMNotificationConfigurations.second_reminder_before_on;
                                            companyNotification.second_reminder_before_on_type = asset.AssetPMNotificationConfigurations.second_reminder_before_on_type;
                                            companyNotification.second_reminder_before_on_status = asset.AssetPMNotificationConfigurations.second_reminder_before_on_status;
                                            companyNotification.due_at_reminder_status = asset.AssetPMNotificationConfigurations.due_at_reminder_status;
                                            companyNotification.first_reminder_before_on_meter_hours = asset.AssetPMNotificationConfigurations.first_reminder_before_on_meter_hours;
                                            companyNotification.first_reminder_before_on_meter_hours_status = asset.AssetPMNotificationConfigurations.first_reminder_before_on_meter_hours_status;
                                            companyNotification.second_reminder_before_on_meter_hours = asset.AssetPMNotificationConfigurations.second_reminder_before_on_meter_hours;
                                            companyNotification.second_reminder_before_on_meter_hours_status = asset.AssetPMNotificationConfigurations.second_reminder_before_on_meter_hours_status;
                                            companyNotification.due_at_reminder_meter_hours_status = asset.AssetPMNotificationConfigurations.due_at_reminder_meter_hours_status;
                                            companyNotification.status = asset.AssetPMNotificationConfigurations.status;
                                        }
                                        ListViewModel<AssetPMResponseModel> pmResponse = new ListViewModel<AssetPMResponseModel>();
                                        var pmDetails = asset.AssetPMs.Where(x => !x.is_archive && x.service_dealer_id != null).ToList();
                                        if (pmDetails?.Count > 0)
                                        {
                                            foreach (var pm in pmDetails)
                                            {
                                                pm.PMTriggers = pm.PMTriggers.Where(x => !x.is_archive && x.status != (int)Status.TriggerCompleted).ToList();
                                                foreach (var trigger in pm.PMTriggers)
                                                {
                                                    // trigger is Time based
                                                    if (trigger.due_datetime != null)
                                                    {
                                                        // check if first reminder is enabled
                                                        if (companyNotification.first_reminder_before_on_status == (int)Status.Active)
                                                        {
                                                            var notificationSendDate = PMTriggersUtil.GetDateFromDueDate(companyNotification.first_reminder_before_on, companyNotification.first_reminder_before_on_type, trigger.due_datetime.Value);
                                                            if (notificationSendDate == DateTime.UtcNow.Date)
                                                            {
                                                                // send Notification
                                                                ToSendVendorEmailExcelDetails toSendVendorEmailExcelDetails = new ToSendVendorEmailExcelDetails();
                                                                toSendVendorEmailExcelDetails.notification_type = GlobalConstants.FirstDueDateReminder;
                                                                toSendVendorEmailExcelDetails.notification_type_id = (int)NotificationStatus.FirstDueDateReminder;
                                                                toSendVendorEmailExcelDetails.service_dealer_id = trigger.AssetPMs.service_dealer_id.Value;
                                                                toSendVendorEmailExcelDetails.vendor_name = trigger.AssetPMs.ServiceDealers.name;
                                                                toSendVendorEmailExcelDetails.vendor_email = trigger.AssetPMs.ServiceDealers.email;
                                                                toSendVendorEmailExcelDetails.asset_id = asset.asset_id;
                                                                toSendVendorEmailExcelDetails.asset_name = asset.name;
                                                                toSendVendorEmailExcelDetails.asset_location = site.location;
                                                                toSendVendorEmailExcelDetails.company_id = companyNotification.company_id;
                                                                toSendVendorEmailExcelDetails.company_name = companyNotification.Company.company_name;
                                                                toSendVendorEmailExcelDetails.internal_asset_id = asset.internal_asset_id;
                                                                toSendVendorEmailExcelDetails.trigger_id = trigger.pm_trigger_id;
                                                                toSendVendorEmailExcelDetails.status = trigger.status;
                                                                toSendVendorEmailExcelDetails.pm_title = trigger.AssetPMs.title;
                                                                toSendVendorEmailExcelDetails.current_meter = asset.meter_hours;
                                                                toSendVendorEmailExcelDetails.due_at = trigger.due_meter_hours;
                                                                toSendVendorEmailExcelDetails.due_in = trigger.due_datetime != null ? DateTimeUtil.GetDueIn(trigger.due_datetime.Value) : null;
                                                                toSendVendors.Add(toSendVendorEmailExcelDetails);
                                                            }
                                                        }
                                                        if (companyNotification.second_reminder_before_on_status == (int)Status.Active)
                                                        {
                                                            var notificationSendDate = PMTriggersUtil.GetDateFromDueDate(companyNotification.second_reminder_before_on, companyNotification.second_reminder_before_on_type, trigger.due_datetime.Value);
                                                            if (notificationSendDate == DateTime.UtcNow.Date)
                                                            {
                                                                ToSendVendorEmailExcelDetails toSendVendorEmailExcelDetails = new ToSendVendorEmailExcelDetails();
                                                                toSendVendorEmailExcelDetails.notification_type = GlobalConstants.SecondDueDateReminder;
                                                                toSendVendorEmailExcelDetails.notification_type_id = (int)NotificationStatus.SecondDueDateReminder;
                                                                toSendVendorEmailExcelDetails.service_dealer_id = trigger.AssetPMs.service_dealer_id.Value;
                                                                toSendVendorEmailExcelDetails.vendor_name = trigger.AssetPMs.ServiceDealers.name;
                                                                toSendVendorEmailExcelDetails.vendor_email = trigger.AssetPMs.ServiceDealers.email;
                                                                toSendVendorEmailExcelDetails.asset_id = asset.asset_id;
                                                                toSendVendorEmailExcelDetails.asset_name = asset.name;
                                                                toSendVendorEmailExcelDetails.asset_location = site.location;
                                                                toSendVendorEmailExcelDetails.company_id = companyNotification.company_id;
                                                                toSendVendorEmailExcelDetails.company_name = companyNotification.Company.company_name;
                                                                toSendVendorEmailExcelDetails.internal_asset_id = asset.internal_asset_id;
                                                                toSendVendorEmailExcelDetails.trigger_id = trigger.pm_trigger_id;
                                                                toSendVendorEmailExcelDetails.status = trigger.status;
                                                                toSendVendorEmailExcelDetails.pm_title = trigger.AssetPMs.title;
                                                                toSendVendorEmailExcelDetails.current_meter = asset.meter_hours;
                                                                toSendVendorEmailExcelDetails.due_at = trigger.due_meter_hours;
                                                                toSendVendorEmailExcelDetails.due_in = trigger.due_datetime != null ? DateTimeUtil.GetDueIn(trigger.due_datetime.Value) : null;
                                                                toSendVendors.Add(toSendVendorEmailExcelDetails);
                                                            }
                                                        }
                                                        if (companyNotification.due_at_reminder_status == (int)Status.Active)
                                                        {
                                                            if (DateTime.UtcNow.Date == trigger.due_datetime.Value.Date)
                                                            {
                                                                ToSendVendorEmailExcelDetails toSendVendorEmailExcelDetails = new ToSendVendorEmailExcelDetails();
                                                                toSendVendorEmailExcelDetails.notification_type = GlobalConstants.OnDueDateReminder;
                                                                toSendVendorEmailExcelDetails.notification_type_id = (int)NotificationStatus.OnDueDateReminder;
                                                                toSendVendorEmailExcelDetails.service_dealer_id = trigger.AssetPMs.service_dealer_id.Value;
                                                                toSendVendorEmailExcelDetails.vendor_name = trigger.AssetPMs.ServiceDealers.name;
                                                                toSendVendorEmailExcelDetails.vendor_email = trigger.AssetPMs.ServiceDealers.email;
                                                                toSendVendorEmailExcelDetails.asset_id = asset.asset_id;
                                                                toSendVendorEmailExcelDetails.asset_name = asset.name;
                                                                toSendVendorEmailExcelDetails.asset_location = site.location;
                                                                toSendVendorEmailExcelDetails.company_id = companyNotification.company_id;
                                                                toSendVendorEmailExcelDetails.company_name = companyNotification.Company.company_name;
                                                                toSendVendorEmailExcelDetails.internal_asset_id = asset.internal_asset_id;
                                                                toSendVendorEmailExcelDetails.trigger_id = trigger.pm_trigger_id;
                                                                toSendVendorEmailExcelDetails.status = trigger.status;
                                                                toSendVendorEmailExcelDetails.pm_title = trigger.AssetPMs.title;
                                                                toSendVendorEmailExcelDetails.current_meter = asset.meter_hours;
                                                                toSendVendorEmailExcelDetails.due_at = trigger.meter_hours_when_pm_due;
                                                                toSendVendorEmailExcelDetails.due_in = trigger.due_datetime != null ? DateTimeUtil.GetDueIn(trigger.due_datetime.Value) : null;
                                                                toSendVendors.Add(toSendVendorEmailExcelDetails);
                                                            }
                                                        }
                                                    }
                                                    // trigger is on Meter Hours
                                                    if (trigger.due_meter_hours != null && trigger.due_meter_hours > 0)
                                                    {
                                                        var diffintInMeterHours = trigger.due_meter_hours - asset.meter_hours;
                                                        if (companyNotification.first_reminder_before_on_meter_hours_status == (int)Status.Active)
                                                        {
                                                            if (diffintInMeterHours < companyNotification.first_reminder_before_on_meter_hours)
                                                            {
                                                                if (companyNotification.second_reminder_before_on_meter_hours_status == (int)Status.Active)
                                                                {
                                                                    if (diffintInMeterHours > companyNotification.second_reminder_before_on_meter_hours)
                                                                    {
                                                                        // send 1st Notification Meter hours reminder
                                                                        ToSendVendorEmailExcelDetails toSendVendorEmailExcelDetails = new ToSendVendorEmailExcelDetails();
                                                                        toSendVendorEmailExcelDetails.notification_type = GlobalConstants.FirstMeterHoursDueReminder;
                                                                        toSendVendorEmailExcelDetails.notification_type_id = (int)NotificationStatus.FirstMeterHoursDueReminder;
                                                                        toSendVendorEmailExcelDetails.service_dealer_id = trigger.AssetPMs.service_dealer_id.Value;
                                                                        toSendVendorEmailExcelDetails.vendor_name = trigger.AssetPMs.ServiceDealers.name;
                                                                        toSendVendorEmailExcelDetails.vendor_email = trigger.AssetPMs.ServiceDealers.email;
                                                                        toSendVendorEmailExcelDetails.asset_id = asset.asset_id;
                                                                        toSendVendorEmailExcelDetails.asset_name = asset.name;
                                                                        toSendVendorEmailExcelDetails.asset_location = site.location;
                                                                        toSendVendorEmailExcelDetails.company_id = companyNotification.company_id;
                                                                        toSendVendorEmailExcelDetails.company_name = companyNotification.Company.company_name;
                                                                        toSendVendorEmailExcelDetails.internal_asset_id = asset.internal_asset_id;
                                                                        toSendVendorEmailExcelDetails.trigger_id = trigger.pm_trigger_id;
                                                                        toSendVendorEmailExcelDetails.status = trigger.status;
                                                                        toSendVendorEmailExcelDetails.pm_title = trigger.AssetPMs.title;
                                                                        toSendVendorEmailExcelDetails.current_meter = asset.meter_hours;
                                                                        toSendVendorEmailExcelDetails.due_at = trigger.due_meter_hours;
                                                                        toSendVendorEmailExcelDetails.due_in = trigger.due_datetime != null ? DateTimeUtil.GetDueIn(trigger.due_datetime.Value) : null;
                                                                        toSendVendors.Add(toSendVendorEmailExcelDetails);
                                                                    }
                                                                    //else if (diffintInMeterHours < companyNotification.second_reminder_before_on_meter_hours && diffintInMeterHours > 0)
                                                                    else if (diffintInMeterHours < companyNotification.second_reminder_before_on_meter_hours)
                                                                    {
                                                                        if (companyNotification.due_at_reminder_meter_hours_status == (int)Status.Active)
                                                                        {
                                                                            if (diffintInMeterHours > 0)
                                                                            {
                                                                                // send 2nd Notification meter hours reminder
                                                                                ToSendVendorEmailExcelDetails toSendVendorEmailExcelDetails = new ToSendVendorEmailExcelDetails();
                                                                                toSendVendorEmailExcelDetails.notification_type = GlobalConstants.SecondMeterHoursDueReminder;
                                                                                toSendVendorEmailExcelDetails.notification_type_id = (int)NotificationStatus.SecondMeterHoursDueReminder;
                                                                                toSendVendorEmailExcelDetails.service_dealer_id = trigger.AssetPMs.service_dealer_id.Value;
                                                                                toSendVendorEmailExcelDetails.vendor_name = trigger.AssetPMs.ServiceDealers.name;
                                                                                toSendVendorEmailExcelDetails.vendor_email = trigger.AssetPMs.ServiceDealers.email;
                                                                                toSendVendorEmailExcelDetails.asset_id = asset.asset_id;
                                                                                toSendVendorEmailExcelDetails.asset_name = asset.name;
                                                                                toSendVendorEmailExcelDetails.asset_location = site.location;
                                                                                toSendVendorEmailExcelDetails.company_id = companyNotification.company_id;
                                                                                toSendVendorEmailExcelDetails.company_name = companyNotification.Company.company_name;
                                                                                toSendVendorEmailExcelDetails.internal_asset_id = asset.internal_asset_id;
                                                                                toSendVendorEmailExcelDetails.trigger_id = trigger.pm_trigger_id;
                                                                                toSendVendorEmailExcelDetails.status = trigger.status;
                                                                                toSendVendorEmailExcelDetails.pm_title = trigger.AssetPMs.title;
                                                                                toSendVendorEmailExcelDetails.current_meter = asset.meter_hours;
                                                                                toSendVendorEmailExcelDetails.due_at = trigger.due_meter_hours;
                                                                                toSendVendorEmailExcelDetails.due_in = trigger.due_datetime != null ? DateTimeUtil.GetDueIn(trigger.due_datetime.Value) : null;
                                                                                toSendVendors.Add(toSendVendorEmailExcelDetails);
                                                                            }
                                                                            else
                                                                            {
                                                                                // send Due meter hours notification reminder
                                                                                ToSendVendorEmailExcelDetails toSendVendorEmailExcelDetails = new ToSendVendorEmailExcelDetails();
                                                                                toSendVendorEmailExcelDetails.notification_type = GlobalConstants.OnMeterHoursDueReminder;
                                                                                toSendVendorEmailExcelDetails.notification_type_id = (int)NotificationStatus.OnMeterHoursDueReminder;
                                                                                toSendVendorEmailExcelDetails.service_dealer_id = trigger.AssetPMs.service_dealer_id.Value;
                                                                                toSendVendorEmailExcelDetails.vendor_name = trigger.AssetPMs.ServiceDealers.name;
                                                                                toSendVendorEmailExcelDetails.vendor_email = trigger.AssetPMs.ServiceDealers.email;
                                                                                toSendVendorEmailExcelDetails.asset_id = asset.asset_id;
                                                                                toSendVendorEmailExcelDetails.asset_name = asset.name;
                                                                                toSendVendorEmailExcelDetails.asset_location = site.location;
                                                                                toSendVendorEmailExcelDetails.company_id = companyNotification.company_id;
                                                                                toSendVendorEmailExcelDetails.company_name = companyNotification.Company.company_name;
                                                                                toSendVendorEmailExcelDetails.internal_asset_id = asset.internal_asset_id;
                                                                                toSendVendorEmailExcelDetails.trigger_id = trigger.pm_trigger_id;
                                                                                toSendVendorEmailExcelDetails.status = trigger.status;
                                                                                toSendVendorEmailExcelDetails.pm_title = trigger.AssetPMs.title;
                                                                                toSendVendorEmailExcelDetails.current_meter = asset.meter_hours;
                                                                                toSendVendorEmailExcelDetails.due_at = trigger.due_meter_hours;
                                                                                toSendVendorEmailExcelDetails.due_in = trigger.datetime_when_pm_due != null ? DateTimeUtil.GetDueIn(trigger.datetime_when_pm_due.Value) : null;
                                                                                toSendVendors.Add(toSendVendorEmailExcelDetails);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            // send 2nd Notification Meter hours reminder
                                                                            ToSendVendorEmailExcelDetails toSendVendorEmailExcelDetails = new ToSendVendorEmailExcelDetails();
                                                                            toSendVendorEmailExcelDetails.notification_type = GlobalConstants.SecondMeterHoursDueReminder;
                                                                            toSendVendorEmailExcelDetails.notification_type_id = (int)NotificationStatus.SecondMeterHoursDueReminder;
                                                                            toSendVendorEmailExcelDetails.service_dealer_id = trigger.AssetPMs.service_dealer_id.Value;
                                                                            toSendVendorEmailExcelDetails.vendor_name = trigger.AssetPMs.ServiceDealers.name;
                                                                            toSendVendorEmailExcelDetails.vendor_email = trigger.AssetPMs.ServiceDealers.email;
                                                                            toSendVendorEmailExcelDetails.asset_id = asset.asset_id;
                                                                            toSendVendorEmailExcelDetails.asset_name = asset.name;
                                                                            toSendVendorEmailExcelDetails.asset_location = site.location;
                                                                            toSendVendorEmailExcelDetails.company_id = companyNotification.company_id;
                                                                            toSendVendorEmailExcelDetails.company_name = companyNotification.Company.company_name;
                                                                            toSendVendorEmailExcelDetails.internal_asset_id = asset.internal_asset_id;
                                                                            toSendVendorEmailExcelDetails.trigger_id = trigger.pm_trigger_id;
                                                                            toSendVendorEmailExcelDetails.status = trigger.status;
                                                                            toSendVendorEmailExcelDetails.pm_title = trigger.AssetPMs.title;
                                                                            toSendVendorEmailExcelDetails.current_meter = asset.meter_hours;
                                                                            toSendVendorEmailExcelDetails.due_at = trigger.due_meter_hours;
                                                                            toSendVendorEmailExcelDetails.due_in = trigger.due_datetime != null ? DateTimeUtil.GetDueIn(trigger.due_datetime.Value) : null;
                                                                            toSendVendors.Add(toSendVendorEmailExcelDetails);
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    // send 1st notification meter hours reminder
                                                                    ToSendVendorEmailExcelDetails toSendVendorEmailExcelDetails = new ToSendVendorEmailExcelDetails();
                                                                    toSendVendorEmailExcelDetails.notification_type = GlobalConstants.FirstMeterHoursDueReminder;
                                                                    toSendVendorEmailExcelDetails.notification_type_id = (int)NotificationStatus.FirstMeterHoursDueReminder;
                                                                    toSendVendorEmailExcelDetails.service_dealer_id = trigger.AssetPMs.service_dealer_id.Value;
                                                                    toSendVendorEmailExcelDetails.vendor_name = trigger.AssetPMs.ServiceDealers.name;
                                                                    toSendVendorEmailExcelDetails.vendor_email = trigger.AssetPMs.ServiceDealers.email;
                                                                    toSendVendorEmailExcelDetails.asset_id = asset.asset_id;
                                                                    toSendVendorEmailExcelDetails.asset_name = asset.name;
                                                                    toSendVendorEmailExcelDetails.asset_location = site.location;
                                                                    toSendVendorEmailExcelDetails.company_id = companyNotification.company_id;
                                                                    toSendVendorEmailExcelDetails.company_name = companyNotification.Company.company_name;
                                                                    toSendVendorEmailExcelDetails.internal_asset_id = asset.internal_asset_id;
                                                                    toSendVendorEmailExcelDetails.trigger_id = trigger.pm_trigger_id;
                                                                    toSendVendorEmailExcelDetails.status = trigger.status;
                                                                    toSendVendorEmailExcelDetails.pm_title = trigger.AssetPMs.title;
                                                                    toSendVendorEmailExcelDetails.current_meter = asset.meter_hours;
                                                                    toSendVendorEmailExcelDetails.due_at = trigger.due_meter_hours;
                                                                    toSendVendorEmailExcelDetails.due_in = trigger.due_datetime != null ? DateTimeUtil.GetDueIn(trigger.due_datetime.Value) : null;
                                                                    toSendVendors.Add(toSendVendorEmailExcelDetails);
                                                                }
                                                            }
                                                        }
                                                        else if (companyNotification.second_reminder_before_on_meter_hours_status == (int)Status.Active)
                                                        {
                                                            //if (diffintInMeterHours < companyNotification.second_reminder_before_on_meter_hours && diffintInMeterHours > 0)
                                                            if (diffintInMeterHours < companyNotification.second_reminder_before_on_meter_hours)
                                                            {
                                                                if (companyNotification.due_at_reminder_meter_hours_status == (int)Status.Active)
                                                                {
                                                                    if (diffintInMeterHours > 0)
                                                                    {
                                                                        // send 2nd Notification meter hours reminder
                                                                        ToSendVendorEmailExcelDetails toSendVendorEmailExcelDetails = new ToSendVendorEmailExcelDetails();
                                                                        toSendVendorEmailExcelDetails.notification_type = GlobalConstants.SecondMeterHoursDueReminder;
                                                                        toSendVendorEmailExcelDetails.notification_type_id = (int)NotificationStatus.SecondMeterHoursDueReminder;
                                                                        toSendVendorEmailExcelDetails.service_dealer_id = trigger.AssetPMs.service_dealer_id.Value;
                                                                        toSendVendorEmailExcelDetails.vendor_name = trigger.AssetPMs.ServiceDealers.name;
                                                                        toSendVendorEmailExcelDetails.vendor_email = trigger.AssetPMs.ServiceDealers.email;
                                                                        toSendVendorEmailExcelDetails.asset_id = asset.asset_id;
                                                                        toSendVendorEmailExcelDetails.asset_name = asset.name;
                                                                        toSendVendorEmailExcelDetails.asset_location = site.location;
                                                                        toSendVendorEmailExcelDetails.company_id = companyNotification.company_id;
                                                                        toSendVendorEmailExcelDetails.company_name = companyNotification.Company.company_name;
                                                                        toSendVendorEmailExcelDetails.internal_asset_id = asset.internal_asset_id;
                                                                        toSendVendorEmailExcelDetails.trigger_id = trigger.pm_trigger_id;
                                                                        toSendVendorEmailExcelDetails.status = trigger.status;
                                                                        toSendVendorEmailExcelDetails.pm_title = trigger.AssetPMs.title;
                                                                        toSendVendorEmailExcelDetails.current_meter = asset.meter_hours;
                                                                        toSendVendorEmailExcelDetails.due_at = trigger.due_meter_hours;
                                                                        toSendVendorEmailExcelDetails.due_in = trigger.due_datetime != null ? DateTimeUtil.GetDueIn(trigger.due_datetime.Value) : null;
                                                                        toSendVendors.Add(toSendVendorEmailExcelDetails);
                                                                    }
                                                                    else
                                                                    {
                                                                        // send Due meter hours notification reminder
                                                                        ToSendVendorEmailExcelDetails toSendVendorEmailExcelDetails = new ToSendVendorEmailExcelDetails();
                                                                        toSendVendorEmailExcelDetails.notification_type = GlobalConstants.OnMeterHoursDueReminder;
                                                                        toSendVendorEmailExcelDetails.notification_type_id = (int)NotificationStatus.OnMeterHoursDueReminder;
                                                                        toSendVendorEmailExcelDetails.service_dealer_id = trigger.AssetPMs.service_dealer_id.Value;
                                                                        toSendVendorEmailExcelDetails.vendor_name = trigger.AssetPMs.ServiceDealers.name;
                                                                        toSendVendorEmailExcelDetails.vendor_email = trigger.AssetPMs.ServiceDealers.email;
                                                                        toSendVendorEmailExcelDetails.asset_id = asset.asset_id;
                                                                        toSendVendorEmailExcelDetails.asset_name = asset.name;
                                                                        toSendVendorEmailExcelDetails.asset_location = site.location;
                                                                        toSendVendorEmailExcelDetails.company_id = companyNotification.company_id;
                                                                        toSendVendorEmailExcelDetails.company_name = companyNotification.Company.company_name;
                                                                        toSendVendorEmailExcelDetails.internal_asset_id = asset.internal_asset_id;
                                                                        toSendVendorEmailExcelDetails.trigger_id = trigger.pm_trigger_id;
                                                                        toSendVendorEmailExcelDetails.status = trigger.status;
                                                                        toSendVendorEmailExcelDetails.pm_title = trigger.AssetPMs.title;
                                                                        toSendVendorEmailExcelDetails.current_meter = asset.meter_hours;
                                                                        toSendVendorEmailExcelDetails.due_at = trigger.due_meter_hours;
                                                                        toSendVendorEmailExcelDetails.due_in = trigger.datetime_when_pm_due != null ? DateTimeUtil.GetDueIn(trigger.datetime_when_pm_due.Value) : null;
                                                                        toSendVendors.Add(toSendVendorEmailExcelDetails);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    // send 2nd Notification Meter hours reminder
                                                                    ToSendVendorEmailExcelDetails toSendVendorEmailExcelDetails = new ToSendVendorEmailExcelDetails();
                                                                    toSendVendorEmailExcelDetails.notification_type = GlobalConstants.SecondMeterHoursDueReminder;
                                                                    toSendVendorEmailExcelDetails.notification_type_id = (int)NotificationStatus.SecondMeterHoursDueReminder;
                                                                    toSendVendorEmailExcelDetails.service_dealer_id = trigger.AssetPMs.service_dealer_id.Value;
                                                                    toSendVendorEmailExcelDetails.vendor_name = trigger.AssetPMs.ServiceDealers.name;
                                                                    toSendVendorEmailExcelDetails.vendor_email = trigger.AssetPMs.ServiceDealers.email;
                                                                    toSendVendorEmailExcelDetails.asset_id = asset.asset_id;
                                                                    toSendVendorEmailExcelDetails.asset_name = asset.name;
                                                                    toSendVendorEmailExcelDetails.asset_location = site.location;
                                                                    toSendVendorEmailExcelDetails.company_id = companyNotification.company_id;
                                                                    toSendVendorEmailExcelDetails.company_name = companyNotification.Company.company_name;
                                                                    toSendVendorEmailExcelDetails.internal_asset_id = asset.internal_asset_id;
                                                                    toSendVendorEmailExcelDetails.trigger_id = trigger.pm_trigger_id;
                                                                    toSendVendorEmailExcelDetails.status = trigger.status;
                                                                    toSendVendorEmailExcelDetails.pm_title = trigger.AssetPMs.title;
                                                                    toSendVendorEmailExcelDetails.current_meter = asset.meter_hours;
                                                                    toSendVendorEmailExcelDetails.due_at = trigger.due_meter_hours;
                                                                    toSendVendorEmailExcelDetails.due_in = trigger.due_datetime != null ? DateTimeUtil.GetDueIn(trigger.due_datetime.Value) : null;
                                                                    toSendVendors.Add(toSendVendorEmailExcelDetails);
                                                                }
                                                            }
                                                        }
                                                        else if (companyNotification.due_at_reminder_meter_hours_status == (int)Status.Active)
                                                        {
                                                            if (diffintInMeterHours <= 0)
                                                            {
                                                                // send on due reminder
                                                                ToSendVendorEmailExcelDetails toSendVendorEmailExcelDetails = new ToSendVendorEmailExcelDetails();
                                                                toSendVendorEmailExcelDetails.notification_type = GlobalConstants.OnMeterHoursDueReminder;
                                                                toSendVendorEmailExcelDetails.notification_type_id = (int)NotificationStatus.OnMeterHoursDueReminder;
                                                                toSendVendorEmailExcelDetails.service_dealer_id = trigger.AssetPMs.service_dealer_id.Value;
                                                                toSendVendorEmailExcelDetails.vendor_name = trigger.AssetPMs.ServiceDealers.name;
                                                                toSendVendorEmailExcelDetails.vendor_email = trigger.AssetPMs.ServiceDealers.email;
                                                                toSendVendorEmailExcelDetails.asset_id = asset.asset_id;
                                                                toSendVendorEmailExcelDetails.company_id = companyNotification.company_id;
                                                                toSendVendorEmailExcelDetails.company_name = companyNotification.Company.company_name;
                                                                toSendVendorEmailExcelDetails.internal_asset_id = asset.internal_asset_id;
                                                                toSendVendorEmailExcelDetails.trigger_id = trigger.pm_trigger_id;
                                                                toSendVendorEmailExcelDetails.status = trigger.status;
                                                                toSendVendorEmailExcelDetails.pm_title = trigger.AssetPMs.title;
                                                                toSendVendorEmailExcelDetails.current_meter = asset.meter_hours;
                                                                toSendVendorEmailExcelDetails.due_at = trigger.due_meter_hours;
                                                                toSendVendorEmailExcelDetails.due_in = trigger.datetime_when_pm_due != null ? DateTimeUtil.GetDueIn(trigger.datetime_when_pm_due.Value) : null;
                                                                toSendVendors.Add(toSendVendorEmailExcelDetails);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            pmResponse.result = (int)ResponseStatusNumber.NotFound;
                                        }
                                    }
                                }
                            }

                            //for single company prepare the object for each vendors

                            if (toSendVendors.Count > 0)
                            {
                                var ServiceDealerPMs = toSendVendors.GroupBy(x => x.service_dealer_id);
                                foreach (var serviceDealer in ServiceDealerPMs)
                                {
                                    VendorPMEmailNotification vendorPM = new VendorPMEmailNotification();
                                    vendorPM.vendor_name = serviceDealer.First().vendor_name;
                                    vendorPM.vendor_email = serviceDealer.First().vendor_email;
                                    vendorPM.company_name = serviceDealer.First().company_name;
                                    vendorPM.service_dealer_id = serviceDealer.Key;
                                    foreach (var item in serviceDealer)
                                    {
                                        var SentNotiDetails = await _UoW.PMNotificationRepository.GetSentPMNotification(serviceDealer.Key, item.trigger_id, item.notification_type_id);
                                        if (SentNotiDetails != null)
                                        {
                                            continue;
                                        }
                                        vendorPM.overdue_pms = item.status == (int)Status.OverDue || item.status == (int)Status.Due ? vendorPM.overdue_pms + 1 : vendorPM.overdue_pms;
                                        vendorPM.total_upcoming_pms = item.status != (int)Status.OverDue && item.status != (int)Status.Due ? vendorPM.total_upcoming_pms + 1 : vendorPM.total_upcoming_pms;
                                        vendorPM.vendorExcelDetails.Add(_mapper.Map<VendorEmailExcelDetails>(item));
                                    }
                                    vendorPMEmailNotifications.Add(vendorPM);
                                }
                            }
                        }
                        if (vendorPMEmailNotifications?.Count > 0)
                        {
                            SendPMNotificationToVendors(vendorPMEmailNotifications);
                        }
                    }
                    else
                    {
                        result = (int)ResponseStatusNumber.NotFound;
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    result = (int)ResponseStatusNumber.Error;
                    _logger.LogError(e.Message);
                }
            }

            return result;
        }

        public async Task SendPMNotificationToVendors(List<VendorPMEmailNotification> vendors)
        {
            try
            {
                foreach (var x in vendors)
                {
                    string subject = x.company_name + " Upcoming PM Report";
                    if (!String.IsNullOrEmpty(x.vendor_email) && x.vendorExcelDetails?.Count > 0)
                    {
                        var FileStream = ExcelCreation.WriteExcelFile(x.vendorExcelDetails);
                        var templateID = ConfigurationManager.AppSettings["Vendor_PMDueReport_Template_ID"];
                        var response = await SendEmail.SendGridEmailWithTemplate(x.vendor_email, subject, x, templateID, FileStream, "PMDetails.xlsx");
                        EmailNotificationStatusUpdate emailNotification = new EmailNotificationStatusUpdate();
                        emailNotification.from = ConfigurationManager.AppSettings["SendGrid_Email"];
                        emailNotification.to = x.vendor_email;
                        emailNotification.submitted_on = DateTime.UtcNow;
                        emailNotification.subject = subject;
                        emailNotification.status = response;
                        var inserted = await _UoW.BaseGenericRepository<EmailNotificationStatusUpdate>().Insert(emailNotification);
                        if (inserted)
                        {
                            foreach (var item in x.vendorExcelDetails)
                            {
                                SentPMNotification pmNotification = new SentPMNotification();
                                pmNotification.manager_id = x.service_dealer_id;
                                pmNotification.notification_type = item.notification_type_id;
                                pmNotification.trigger_id = item.trigger_id;
                                await _UoW.BaseGenericRepository<SentPMNotification>().Update(pmNotification);
                            }
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

    }
}
