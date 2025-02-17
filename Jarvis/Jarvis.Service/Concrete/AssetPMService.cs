using AutoMapper;
using Jarvis.db.Migrations;
using Jarvis.db.Models;
using Jarvis.Service.Abstract;
using Jarvis.Service.Notification;
using Jarvis.Service.Resources;
using Jarvis.Shared.Helper;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using SendGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Jarvis.Service.Concrete
{
    public class AssetPMService : BaseService, IAssetPMService
    {
        public readonly IMapper _mapper;
        private Logger _logger;

        public AssetPMService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            _logger = Logger.GetInstance<AssetPMService>();
        }

        public async Task<PMPlansResponseModel> AddAssetPM(AssignPMToAsset AssignPM)
        {
            PMPlansResponseModel pmPlanResponse = new PMPlansResponseModel();
            int result = (int)ResponseStatusNumber.Error;
          
                try
                {
                    var pmPlan = await _UoW.PMPlansRepository.GetPMPlanByIdToAddinAsset(AssignPM.pm_plan_id);
                    if (pmPlan != null)
                    {
                        AssetPMPlans assetPMPlan = new AssetPMPlans();
                        assetPMPlan = _mapper.Map<AssetPMPlans>(pmPlan);
                        assetPMPlan.asset_id = AssignPM.asset_id;
                        assetPMPlan.created_at = DateTime.UtcNow;
                        assetPMPlan.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        result = await _UoW.AssetPMPlansRepository.Insert(assetPMPlan);
                        if (result > 0)
                        {
                            
                            pmPlan.PMs = pmPlan.PMs.ToList().Where(x => !x.is_archive).ToList();
                            /*pmPlan.PMs.ToList().ForEach(x =>
                            {
                                x.PMTasks = x.PMTasks.Where(y => !y.is_archive).ToList();
                            });*/
                            pmPlan.PMs.ToList().ForEach(x =>
                            {
                                x.PMAttachments = x.PMAttachments.Where(y => !y.is_archive).ToList();
                            });
                            assetPMPlan.AssetPMs = _mapper.Map<List<AssetPMs>>(pmPlan.PMs);
                            assetPMPlan.AssetPMs.ToList().ForEach(async x =>
                            {
                                x.title = x.title +" - "+ DateTime.UtcNow.Year + " - 1";
                                x.asset_pm_plan_id = assetPMPlan.asset_pm_plan_id;
                                x.asset_id = AssignPM.asset_id;
                                x.created_at = DateTime.UtcNow;
                                x.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                x.status = (int)Status.open;
                                x.datetime_starting_at = DateTime.UtcNow;
                                x.asset_pm_completed_date = x.datetime_starting_at;

                                /*x.AssetPMTasks.ToList().ForEach(y =>
                                {
                                    y.asset_pm_plan_id = assetPMPlan.asset_pm_plan_id;
                                    y.asset_pm_id = x.asset_pm_id;
                                    y.asset_id = AssignPM.asset_id;
                                    y.created_at = DateTime.UtcNow;
                                    y.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                });*/
                                x.AssetPMAttachments.ToList().ForEach(y =>
                                {
                                    y.asset_pm_plan_id = assetPMPlan.asset_pm_plan_id;
                                    y.asset_pm_id = x.asset_pm_id;
                                    y.asset_id = AssignPM.asset_id;
                                    y.created_at = DateTime.UtcNow;
                                    y.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                });
                                x.AssetPMsTriggerConditionMapping.ToList().ForEach(y =>
                                {
                                    y.asset_pm_id = x.asset_pm_id;
                                    y.created_at = DateTime.UtcNow;
                                    y.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                                    y.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                });
                            });
                            result = await _UoW.AssetPMsRepository.InsertList(assetPMPlan.AssetPMs);
                            if (result > 0)
                            {
                                _UoW.SaveChanges();
                                pmPlanResponse.response_status = result;

                                // function to Add Upcoming 5 Years AssetPMs
                                await AddAssetPMsForNext5Yrs(AssignPM.asset_id);

                                // For Updating AssetPMs Due_Date, Due_flag and Due_In based on Asset cricality condition
                                await UpdateDueDateDueInDueFlagForAssetPMsByAssetId(assetPMPlan.asset_id);

                                // Create Triggers for all Asset PM items
                                /*  var pmTriggers = await CreatePMTrigger(assetPMPlan.AssetPMs.ToList());
                                  if (pmTriggers?.Count > 0)
                                  {
                                      result = await _UoW.PMTriggersRepository.InsertList(pmTriggers);
                                      if (result > 0)
                                      {
                                          _UoW.SaveChanges();
                                          _dbtransaction.Commit();
                                          pmPlanResponse = _mapper.Map<PMPlansResponseModel>(pmPlan);
                                          pmPlanResponse.response_status = result;
                                      }
                                      else
                                      {
                                          _dbtransaction.Rollback();
                                      }
                                  }
                                  else
                                  {
                                      _dbtransaction.Rollback();
                                  }*/
                            }
                            else
                            {
                               
                            }
                        }
                        else
                        {
                           
                            pmPlanResponse.response_status = result;
                        }
                    }
                    else
                    {
                        pmPlanResponse.response_status = (int)ResponseStatusNumber.NotFound;
                    }
                }
                catch (Exception e)
                {
                    
                    _logger.LogError(e.Message);
                    pmPlanResponse.response_status = (int)ResponseStatusNumber.Error;
                }
            

            return pmPlanResponse;
        }

        public async Task<int> AddAssetPMsForNext5Yrs(Guid asset_id)
        {
            try
            {
                var get_asset = _UoW.AssetRepository.GetAssetByAssetID(asset_id.ToString());

                var get_assetpms = _UoW.AssetPMsRepository.GetAssetPMsByAssetId(asset_id);

                foreach (var assetpm in get_assetpms)
                {
                    var max_condition = GetConditionTypeForPM(get_asset);
                    if (max_condition > 0)
                    {
                        int count = 0;
                        var pm_condition_trigger = assetpm.AssetPMsTriggerConditionMapping.Where(x => x.condition_type_id == max_condition).FirstOrDefault();
                        if (pm_condition_trigger != null)
                        {
                            int? frequency = pm_condition_trigger.datetime_repeates_every.Value;
                            int? frequency_type = pm_condition_trigger.datetime_repeat_time_period_type.Value;

                            var months_to_add_pms = ConfigurationManager.AppSettings["MonthstoAddAssetPMs"];
                            var years_to_add_pms = ConfigurationManager.AppSettings["YearstoAddAssetPMs"];

                            if (pm_condition_trigger != null && frequency != null && frequency > 0 && frequency_type != null && frequency_type > 0)
                            {
                                if (frequency_type == (int)Status.Month)
                                    count = int.Parse(months_to_add_pms) / frequency.Value;
                                else if (frequency_type == (int)Status.Year)
                                    count = int.Parse(years_to_add_pms) / frequency.Value;
                            }

                            if (count > 0)
                            {
                                DateTime? datetime_starting_at = assetpm.datetime_starting_at.Value;

                                for (int i = 1; i < count; i++)
                                {
                                    AssetPMs assetPMs_ = new AssetPMs();
                                    assetPMs_ = _mapper.Map<AssetPMs>(assetpm);

                                    if (frequency_type == (int)Status.Month)
                                         datetime_starting_at = datetime_starting_at.Value.AddMonths(frequency.Value);
                                    else if(frequency_type == (int)Status.Year)
                                        datetime_starting_at = datetime_starting_at.Value.AddYears(frequency.Value);

                                    var this_yr_count = _UoW.AssetPMsRepository.GetAssetPMCountByYear(datetime_starting_at.Value,assetpm.asset_id,assetpm.pm_id.Value);
                                    int lastSpaceIndex = assetPMs_.title.LastIndexOf(' ');

                                    assetPMs_.title = assetPMs_.title.Substring(0, lastSpaceIndex - 7) + " " + datetime_starting_at.Value.Year+" - " + (this_yr_count);
                                    
                                    assetPMs_.datetime_starting_at = datetime_starting_at;
                                    assetPMs_.asset_pm_completed_date = datetime_starting_at;
                                    assetPMs_.created_at = DateTime.UtcNow;
                                    assetPMs_.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                                    assetPMs_.AssetPMAttachments.ToList().ForEach(y =>
                                    {
                                        y.asset_pm_plan_id = assetPMs_.asset_pm_plan_id;
                                        y.asset_id = assetPMs_.asset_id;
                                        y.created_at = DateTime.UtcNow;
                                        y.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                    });
                                    assetPMs_.AssetPMsTriggerConditionMapping.ToList().ForEach(y =>
                                    {
                                        y.created_at = DateTime.UtcNow;
                                        y.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                                        y.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                    });

                                    var insert = await _UoW.BaseGenericRepository<AssetPMs>().Insert(assetPMs_);
                                    _UoW.SaveChanges();
                                }
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

        public async Task<int> RemoveAssetPMPlan2_forScript(Guid id)
        {
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var pmDetails = await _UoW.AssetPMsRepository.GetAssetPMPlanById(id);
                    if (pmDetails?.asset_pm_plan_id != null)
                    {

                        pmDetails.status = (int)Status.Deactive;
                        pmDetails.modified_at = DateTime.UtcNow;
                        pmDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        pmDetails.AssetPMs.ToList().ForEach(x =>
                        {
                            if (x.status != (int)Status.Completed)
                            {
                                x.is_archive = true;
                                x.modified_at = DateTime.UtcNow;
                                x.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                x.AssetPMAttachments.ToList().ForEach(task =>
                                {
                                    task.is_archive = true;
                                    task.modified_at = DateTime.UtcNow;
                                    task.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                });
                            }
                        });
                        result = await _UoW.AssetPMPlansRepository.Update(pmDetails);
                        _UoW.CommitTransaction();
                        if (result > 0)
                        {
                            result = (int)ResponseStatusNumber.Success;

                            /* if (pmDetails.AssetPMs?.Count > 0)
                             {
                                 foreach (var x in pmDetails.AssetPMs)
                                 {
                                     var pmTriggers = await _UoW.PMTriggersRepository.GetAssetPMTriggers(x.asset_pm_id);
                                     if (pmTriggers?.Count > 0)
                                     {
                                         foreach (var trigger in pmTriggers)
                                         {
                                             trigger.is_archive = true;
                                             trigger.modified_at = DateTime.UtcNow;
                                             trigger.PMTriggersTasks.ToList().ForEach(triggerTask =>
                                             {
                                                 triggerTask.is_archive = true;
                                                 triggerTask.modified_at = DateTime.UtcNow;
                                             });
                                             result = await _UoW.PMTriggersRepository.Update(trigger);
                                         }
                                     }
                                 }
                                 if (result > 0)
                                 {
                                     _dbtransaction.Commit();
                                     result = (int)ResponseStatusNumber.Success;
                                 }
                                 else
                                 {
                                     _dbtransaction.Rollback();
                                 }
                             }
                             else
                             {
                                 _dbtransaction.Commit();
                                 result = (int)ResponseStatusNumber.Success;
                             }*/
                        }
                        else
                        {
                            _dbtransaction.Rollback();
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
                    _logger.LogError("AssetRemovePMAssetID-" + id + "-" + e.Message);
                    throw e;
                }
            }

            return result;
        }


        public async Task<int> RemoveAssetPMPlan(Guid id)
        {
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var pmDetails = await _UoW.AssetPMsRepository.GetAssetPMPlanById(id);
                    if (pmDetails?.asset_pm_plan_id != null)
                    {

                        pmDetails.status = (int)Status.Deactive;
                        pmDetails.modified_at = DateTime.UtcNow;
                        pmDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        pmDetails.AssetPMs.ToList().ForEach(x =>
                        {
                            if (x.status != (int)Status.Completed)
                            {
                                x.is_archive = true;
                                x.modified_at = DateTime.UtcNow;
                                x.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                x.AssetPMAttachments.ToList().ForEach(task =>
                                {
                                    task.is_archive = true;
                                    task.modified_at = DateTime.UtcNow;
                                    task.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                });
                            }
                        });
                        result = await _UoW.AssetPMPlansRepository.Update(pmDetails);
                        _UoW.CommitTransaction();
                        if (result > 0)
                        {
                            result = (int)ResponseStatusNumber.Success;

                            /* if (pmDetails.AssetPMs?.Count > 0)
                             {
                                 foreach (var x in pmDetails.AssetPMs)
                                 {
                                     var pmTriggers = await _UoW.PMTriggersRepository.GetAssetPMTriggers(x.asset_pm_id);
                                     if (pmTriggers?.Count > 0)
                                     {
                                         foreach (var trigger in pmTriggers)
                                         {
                                             trigger.is_archive = true;
                                             trigger.modified_at = DateTime.UtcNow;
                                             trigger.PMTriggersTasks.ToList().ForEach(triggerTask =>
                                             {
                                                 triggerTask.is_archive = true;
                                                 triggerTask.modified_at = DateTime.UtcNow;
                                             });
                                             result = await _UoW.PMTriggersRepository.Update(trigger);
                                         }
                                     }
                                 }
                                 if (result > 0)
                                 {
                                     _dbtransaction.Commit();
                                     result = (int)ResponseStatusNumber.Success;
                                 }
                                 else
                                 {
                                     _dbtransaction.Rollback();
                                 }
                             }
                             else
                             {
                                 _dbtransaction.Commit();
                                 result = (int)ResponseStatusNumber.Success;
                             }*/
                        }
                        else
                        {
                            _dbtransaction.Rollback();
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
                    _logger.LogError(e.Message);
                    throw e;
                }
            }

            return result;
        }
        
        public async Task<AssetPMResponseModel> GetAssetPMByID(Guid id)
        {
            AssetPMResponseModel pmResponse = new AssetPMResponseModel();
            try
            {
                var pmDetails = await _UoW.AssetPMsRepository.GetAssetPMByIdForView(id);
                if (pmDetails?.asset_pm_id != null)
                {
                    var max_condition = GetConditionTypeForPM(pmDetails.Asset);
                    pmDetails.AssetPMAttachments = pmDetails.AssetPMAttachments.Where(x => !x.is_archive).ToList();
                    pmResponse = _mapper.Map<AssetPMResponseModel>(pmDetails);
                    pmResponse.active_condition_type_id = max_condition;
                }
                else
                {
                    pmResponse.status = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw e;
            }
            return pmResponse;
        }

        public async Task<ListViewModel<AssetPMResponseModel>> GetAssetPMByAssetID(Guid asset_id, int filter_type)
        {
            ListViewModel<AssetPMResponseModel> pmResponse = new ListViewModel<AssetPMResponseModel>();
            try
            {
                var pmDetails = await _UoW.AssetPMsRepository.GetAssetPMByAssetId(asset_id, filter_type);
                if (pmDetails?.Count > 0)
                {
                    foreach (var pm in pmDetails)
                    {
                        pm.AssetPMTasks = pm.AssetPMTasks.Where(x => !x.is_archive).ToList();
                        pm.AssetPMAttachments = pm.AssetPMAttachments.Where(x => !x.is_archive).ToList();
                        pm.PMTriggers = pm.PMTriggers.Where(x => !x.is_archive && x.status != (int)Status.TriggerCompleted).ToList();
                        foreach (var trigger in pm.PMTriggers)
                        {
                            trigger.PMTriggersTasks = trigger.PMTriggersTasks.Where(x => !x.is_archive).ToList();
                        }
                    }
                    pmResponse.list = _mapper.Map<List<AssetPMResponseModel>>(pmDetails);
                    pmResponse.listsize = pmResponse.list.Count;
                }
                else
                {
                    pmResponse.result = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw e;
            }
            return pmResponse;
        }

        public async Task<AssetPMResponseModel> UpdateAssetPM(UpdateAssetPMRequestModel pmRequest)
        {
            AssetPMResponseModel pmResponse = new AssetPMResponseModel();
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    DateTime? datetime_starting_at = null; bool is_frequency_changed = false;
                    var is_curr_assetpm = _UoW.AssetPMsRepository.IsThisCurrentAssetPM(pmRequest.asset_id, pmRequest.pm_id, pmRequest.asset_pm_id);
                    if (!is_curr_assetpm)
                    {
                        pmResponse.response_status = (int)ResponseStatusNumber.can_not_update_future_assetpm;
                        return pmResponse;
                    }
                    else if (pmRequest.asset_pm_id != null && pmRequest.asset_pm_id != Guid.Empty)
                    {
                        // Add code to update PM
                        var pmDetails = await _UoW.AssetPMsRepository.GetAssetPMByIdForUpdate(pmRequest.asset_pm_id);
                        if (pmDetails != null)
                        {
                            pmDetails.title = pmRequest.title;
                            pmDetails.description = pmRequest.description;
                            pmDetails.status = pmRequest.status;
                            pmDetails.pm_trigger_by = pmRequest.pm_trigger_by;
                            pmDetails.pm_trigger_type = pmRequest.pm_trigger_type;
                            pmDetails.is_trigger_on_starting_at = pmRequest.is_trigger_on_starting_at;
                            datetime_starting_at = pmDetails.datetime_starting_at;
                            pmDetails.datetime_starting_at = pmRequest.datetime_starting_at;
                            pmDetails.work_procedure_type = pmRequest.work_procedure_type;
                            pmDetails.form_id = pmRequest.form_id;
                            pmDetails.modified_at = DateTime.UtcNow;
                            pmDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            pmDetails.asset_pm_completed_date = pmRequest.datetime_starting_at;
                            pmDetails.estimation_time = pmRequest.estimation_time;

                            #region PM Attachments

                            if (pmRequest.asset_pm_attachments != null)
                            {
                                foreach (var attachments in pmRequest.asset_pm_attachments)
                                {
                                    AssetPMAttachments assetpmAttachments = new AssetPMAttachments();
                                    if (attachments.asset_pm_attachment_id == null || attachments.asset_pm_attachment_id == Guid.Empty)
                                    {
                                        var alreadyexist = pmDetails.AssetPMAttachments.Where(x => x.user_uploaded_name == attachments.user_uploaded_name && x.asset_pm_id == pmDetails.asset_pm_id).FirstOrDefault();
                                        if (alreadyexist != null && alreadyexist.asset_pm_attachment_id != null && alreadyexist.asset_pm_attachment_id != Guid.Empty)
                                        {
                                            if (alreadyexist.is_archive == true)
                                            {
                                                alreadyexist.modified_at = DateTime.UtcNow;
                                                alreadyexist.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                                alreadyexist.is_archive = false;
                                                alreadyexist.filename = attachments.filename;
                                            }
                                        }
                                        else
                                        {
                                            assetpmAttachments.asset_id = pmDetails.asset_id;
                                            assetpmAttachments.asset_pm_id = pmDetails.asset_pm_id;
                                            assetpmAttachments.asset_pm_plan_id = pmDetails.asset_pm_plan_id;
                                            assetpmAttachments.filename = attachments.filename;
                                            assetpmAttachments.user_uploaded_name = attachments.user_uploaded_name;
                                            assetpmAttachments.created_at = DateTime.UtcNow;
                                            assetpmAttachments.is_archive = false;
                                            assetpmAttachments.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                            assetpmAttachments.modified_at = DateTime.UtcNow;
                                            pmDetails.AssetPMAttachments.Add(assetpmAttachments);
                                        }
                                    }
                                }

                                #endregion

                                var assetpmattachments = pmDetails.AssetPMAttachments.Where(x => x.asset_pm_id == pmRequest.asset_pm_id).ToList();

                                var removeAttachments = assetpmattachments.Where(p => !pmRequest.asset_pm_attachments.Any(p2 => p2.user_uploaded_name == p.user_uploaded_name) && p.is_archive == false).ToList();

                                if (removeAttachments.Count > 0)
                                {
                                    removeAttachments.ForEach(x => x.is_archive = true);
                                }
                            }
                            if (pmDetails.AssetPMsTriggerConditionMapping != null && pmDetails.AssetPMsTriggerConditionMapping.Count > 0)
                            {
                                pmDetails.AssetPMsTriggerConditionMapping.ToList().ForEach(x =>
                                {
                                    var requested_trigger = pmRequest.asset_pm_trigger_condition_mapping_request_model.Where(q => q.asset_pm_trigger_condition_mapping_id == x.asset_pm_trigger_condition_mapping_id).FirstOrDefault();
                                    if (requested_trigger != null)
                                    {
                                        int? fre = null; int? fre_type = null;
                                        fre = x.datetime_repeates_every; fre_type = x.datetime_repeat_time_period_type;
                                        
                                        if (fre != null && fre_type!=null &&  (fre!=requested_trigger.datetime_repeates_every 
                                        || fre_type!=requested_trigger.datetime_repeat_time_period_type))
                                        {
                                            is_frequency_changed = true;
                                        }

                                        x.datetime_repeates_every = requested_trigger.datetime_repeates_every;
                                        x.datetime_repeat_time_period_type = requested_trigger.datetime_repeat_time_period_type;
                                        x.modified_at = DateTime.UtcNow;
                                        x.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                    }
                                });
                            }

                            // Remove/Update Old Triggers related to this Asset PM ID
                            result = _UoW.AssetPMsRepository.Update(pmDetails).Result;

                            if (result > 0)
                            {
                                _UoW.SaveChanges();
                                _dbtransaction.Commit();
                                pmResponse.response_status = (int)ResponseStatusNumber.Success;

                                if (datetime_starting_at != null && (datetime_starting_at.Value.Date != pmRequest.datetime_starting_at.Date || is_frequency_changed))
                                {
                                    int count_should_be = await CalculateActualAssetPMsCountByFrequency(pmDetails);
                                    var get_pm_count = _UoW.AssetPMsRepository.GetAssetPMsCountByAssetIdPMId(pmDetails.asset_id, pmDetails.pm_id.Value);
                                    // Remove AssetPMs due to frequency change
                                    if (get_pm_count > count_should_be)
                                    {
                                        int remove_count = get_pm_count - count_should_be;
                                        await AddRemoveAssetPMsByFrequency(false, remove_count, pmDetails);
                                    }
                                    // Add new AssetPMs due to frequency change
                                    else if (get_pm_count < count_should_be)
                                    {
                                        int add_count = count_should_be - get_pm_count;
                                        await AddRemoveAssetPMsByFrequency(true, add_count, pmDetails);
                                    }
                                    // Update Start Date for other Duplicate AssetPMs
                                    await UpdateStartDateOfAllDuplicateAssetPMs(pmDetails);
                                }

                                // For Updating AssetPMs Due_Date, Due_flag and Due_In based on Asset cricality condition
                                await UpdateDueDateDueInDueFlagForAssetPMsByAssetId(pmDetails.asset_id);

                                /*if (pmDetails.PMTriggers?.Count > 0)
                                {
                                    var activeTrigger = pmDetails.PMTriggers.Where(x => !x.is_archive && x.status != (int)Status.TriggerCompleted).OrderByDescending(x => x.created_at).FirstOrDefault();
                                    if (activeTrigger != null)
                                    {
                                        var oldSttaus = activeTrigger.status;
                                        var requestedStatus = pmRequest.status;

                                        var triggerTasks = activeTrigger.PMTriggersTasks.ToList();

                                        foreach (var tasks in pmDetails.AssetPMTasks)
                                        {
                                            PMTriggersTasks pmTasks = new PMTriggersTasks();
                                            //if (tasks.asset_pm_task_id == null || tasks.asset_pm_task_id == Guid.Empty)
                                            //{
                                            var alreadyexist = triggerTasks.Where(x => x.task_id == tasks.task_id && x.pm_trigger_id == activeTrigger.pm_trigger_id).FirstOrDefault();
                                            if (alreadyexist != null && alreadyexist.trigger_task_id != null && alreadyexist.trigger_task_id != Guid.Empty)
                                            {
                                                if (alreadyexist.is_archive == true)
                                                {
                                                    alreadyexist.modified_at = DateTime.UtcNow;
                                                    alreadyexist.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                                    alreadyexist.is_archive = false;
                                                }
                                            }
                                            else
                                            {
                                                pmTasks.pm_trigger_id = activeTrigger.pm_trigger_id;
                                                pmTasks.pm_trigger_id = activeTrigger.pm_trigger_id;
                                                pmTasks.asset_id = activeTrigger.asset_id;
                                                pmTasks.asset_pm_id = activeTrigger.asset_pm_id;
                                                pmTasks.asset_pm_task_id = tasks.asset_pm_task_id;
                                                pmTasks.task_id = tasks.task_id;
                                                pmTasks.status = (int)Status.TriggetTaskNew;
                                                pmTasks.created_at = DateTime.UtcNow;
                                                pmTasks.is_archive = false;
                                                pmTasks.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                                pmTasks.modified_at = DateTime.UtcNow;
                                                activeTrigger.PMTriggersTasks.Add(pmTasks);
                                            }
                                            //}
                                        }

                                        var triggerpmtasks = triggerTasks.Where(x => x.asset_pm_id == pmDetails.asset_pm_id).ToList();

                                        var triggerremoveTasks = triggerpmtasks.Where(p => !pmDetails.AssetPMTasks.Any(p2 => p2.task_id == p.task_id && p2.is_archive == false) && p.is_archive == false).ToList();

                                        if (triggerremoveTasks.Count > 0)
                                        {
                                            triggerremoveTasks.ForEach(x => x.is_archive = true);
                                        }

                                        // update the current trigger value
                                        if (pmDetails.pm_trigger_type == (int)Status.FixedOneTime)
                                        {
                                            // check the trigger by time/meter hours/time+meter hours
                                            if (pmDetails.pm_trigger_by == (int)Status.Time)
                                            {
                                                var currentDate = DateTime.UtcNow;
                                                var dueDate = pmDetails.datetime_due_at.Value;
                                                // case 1 currentDate = 25/8/2021 and DueDate = 30/8/2021 or DueDate = 25/8/2021
                                                if (dueDate >= currentDate)
                                                {
                                                    // check the Trigger Configuration
                                                    // check date diff in days
                                                    double diff2 = (dueDate - currentDate).TotalDays;
                                                    if (diff2 > GlobalConstants.DateDiffForDueStatus)
                                                    {
                                                        // set the status new
                                                        activeTrigger.status = (int)Status.TriggerNew;
                                                    }
                                                    else
                                                    {
                                                        // set the status Due
                                                        activeTrigger.datetime_when_pm_due = dueDate;
                                                        activeTrigger.meter_hours_when_pm_due = pmDetails.Asset.meter_hours;
                                                        activeTrigger.status = (int)Status.Due;
                                                    }

                                                }
                                                // case 1 currentDate = 25/8/2021 and DueDate = 20/8/2021
                                                else if (dueDate < currentDate)
                                                {
                                                    // set the status is overdue
                                                    activeTrigger.datetime_when_pm_due = dueDate;
                                                    activeTrigger.meter_hours_when_pm_due = pmDetails.Asset.meter_hours;
                                                    activeTrigger.status = (int)Status.OverDue;
                                                }

                                                activeTrigger.due_datetime = dueDate;
                                                activeTrigger.created_at = DateTime.UtcNow;
                                            }
                                            else if (pmDetails.pm_trigger_by == (int)Status.MeterHours)
                                            {
                                                var dueMeterHours = pmDetails.meter_hours_due_at.Value;
                                                // get current Asset Meter Hours
                                                if (pmDetails.Asset == null)
                                                {
                                                    pmDetails.Asset = _UoW.AssetRepository.GetAssetByAssetID(pmDetails.asset_id.ToString());
                                                }
                                                // case 1 current Meter hours(200) < due meter hours(500)
                                                if (dueMeterHours >= pmDetails.Asset.meter_hours)
                                                {
                                                    // set the status new
                                                    activeTrigger.status = (int)Status.TriggerNew;
                                                }
                                                // case 2 current Meter hours = 500 and DueDate = 200
                                                else if (dueMeterHours < pmDetails.Asset.meter_hours)
                                                {
                                                    // set the status is overdue
                                                    activeTrigger.datetime_when_pm_due = DateTime.UtcNow;
                                                    activeTrigger.meter_hours_when_pm_due = dueMeterHours;
                                                    activeTrigger.status = (int)Status.OverDue;
                                                }
                                                activeTrigger.due_meter_hours = dueMeterHours;
                                                activeTrigger.modified_at = DateTime.UtcNow;
                                            }
                                            else if (pmDetails.pm_trigger_by == (int)Status.TimeMeterHours)
                                            {
                                                var dueMeterHours = pmDetails.meter_hours_due_at.Value;
                                                // get current Asset Meter Hours
                                                var currentDate = DateTime.UtcNow;
                                                var dueDate = pmDetails.datetime_due_at.Value;
                                                // case 1 currentDate = 25/8/2021 and DueDate = 30/8/2021 or DueDate = 25/8/2021
                                                if (dueDate >= currentDate)
                                                {
                                                    // check the Trigger Configuration
                                                    // check date diff in days
                                                    double diff2 = (dueDate - currentDate).TotalDays;
                                                    if (diff2 > GlobalConstants.DateDiffForDueStatus)
                                                    {
                                                        // set the status new
                                                        activeTrigger.status = (int)Status.TriggerNew;
                                                    }
                                                    else
                                                    {
                                                        // set the status activeTrigger.datetime_when_pm_due = DateTime.UtcNow;
                                                        activeTrigger.meter_hours_when_pm_due = pmDetails.Asset.meter_hours;
                                                        activeTrigger.datetime_when_pm_due = dueDate;
                                                        activeTrigger.status = (int)Status.Due;
                                                    }
                                                }
                                                // case 1 currentDate = 25/8/2021 and DueDate = 20/8/2021
                                                else if (dueDate < currentDate)
                                                {
                                                    // set the status is overdue
                                                    activeTrigger.meter_hours_when_pm_due = pmDetails.Asset.meter_hours;
                                                    activeTrigger.datetime_when_pm_due = dueDate;
                                                    activeTrigger.status = (int)Status.OverDue;
                                                }

                                                if (activeTrigger.status == (int)Status.TriggerNew || activeTrigger.status == (int)Status.Due)
                                                {
                                                    if (pmDetails.Asset == null)
                                                    {
                                                        pmDetails.Asset = _UoW.AssetRepository.GetAssetByAssetID(pmDetails.asset_id.ToString());
                                                    }
                                                    // case 1 current Meter hours(200) < due meter hours(500)
                                                    if (dueMeterHours >= pmDetails.Asset.meter_hours)
                                                    {
                                                        // set the status new
                                                        if (activeTrigger.status != (int)Status.Due)
                                                        {
                                                            activeTrigger.status = (int)Status.TriggerNew;
                                                        }
                                                    }
                                                    // case 2 current Meter hours = 500 and DueDate = 200
                                                    else if (dueMeterHours < pmDetails.Asset.meter_hours)
                                                    {
                                                        // set the status is overdue
                                                        activeTrigger.meter_hours_when_pm_due = dueMeterHours;
                                                        activeTrigger.datetime_when_pm_due = DateTime.UtcNow;
                                                        activeTrigger.status = (int)Status.OverDue;
                                                    }
                                                }
                                                activeTrigger.due_meter_hours = dueMeterHours;
                                                activeTrigger.due_datetime = dueDate;
                                                activeTrigger.modified_at = DateTime.UtcNow;
                                            }
                                        }
                                        else if (pmDetails.pm_trigger_type == (int)Status.Recurring)
                                        {
                                            var lastTrigger = pmDetails.PMTriggers.Where(x => x.pm_trigger_id != activeTrigger.pm_trigger_id && !x.is_archive && x.status == (int)Status.TriggerCompleted).OrderByDescending(x => x.modified_at).FirstOrDefault();
                                            if (pmDetails.pm_trigger_by == (int)Status.Time)
                                            {
                                                var currentDate = DateTime.UtcNow;
                                                var dueDate = pmDetails.datetime_starting_at.Value;

                                                if (lastTrigger != null)
                                                {
                                                    var triggerDetail = await _UoW.PMTriggersRepository.GetTriggerByID(lastTrigger.pm_trigger_id);
                                                    if (triggerDetail != null)
                                                    {
                                                        if (pmDetails.datetime_repeat_time_period_type == (int)Status.Month)
                                                        {
                                                            dueDate = triggerDetail.PMTriggersRemarks.completed_on.AddMonths(pmDetails.datetime_repeates_every.Value);
                                                        }
                                                        else if (pmDetails.datetime_repeat_time_period_type == (int)Status.Year)
                                                        {
                                                            dueDate = triggerDetail.PMTriggersRemarks.completed_on.AddYears(pmDetails.datetime_repeates_every.Value);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (pmDetails.is_trigger_on_starting_at == true)
                                                        {
                                                            dueDate = pmDetails.datetime_starting_at.Value;
                                                        }
                                                        else
                                                        {
                                                            if (pmDetails.datetime_repeat_time_period_type == (int)Status.Month)
                                                            {
                                                                dueDate = pmDetails.datetime_starting_at.Value.AddMonths(pmDetails.datetime_repeates_every.Value);
                                                            }
                                                            else if (pmDetails.datetime_repeat_time_period_type == (int)Status.Year)
                                                            {
                                                                dueDate = pmDetails.datetime_starting_at.Value.AddYears(pmDetails.datetime_repeates_every.Value);
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (pmDetails.is_trigger_on_starting_at == true)
                                                    {
                                                        dueDate = pmDetails.datetime_starting_at.Value;
                                                    }
                                                    else
                                                    {
                                                        if (pmDetails.datetime_repeat_time_period_type == (int)Status.Month)
                                                        {
                                                            dueDate = pmDetails.datetime_starting_at.Value.AddMonths(pmDetails.datetime_repeates_every.Value);
                                                        }
                                                        else if (pmDetails.datetime_repeat_time_period_type == (int)Status.Year)
                                                        {
                                                            dueDate = pmDetails.datetime_starting_at.Value.AddYears(pmDetails.datetime_repeates_every.Value);
                                                        }
                                                    }
                                                }
                                                // case 1 currentDate = 25/8/2021 and DueDate = 30/8/2021 or DueDate = 25/8/2021
                                                if (dueDate >= currentDate)
                                                {
                                                    // check the Trigger Configuration
                                                    // check date diff in days
                                                    double diff2 = (dueDate - currentDate).TotalDays;
                                                    if (diff2 > GlobalConstants.DateDiffForDueStatus)
                                                    {
                                                        // set the status new
                                                        activeTrigger.status = (int)Status.TriggerNew;
                                                    }
                                                    else
                                                    {
                                                        // set the status Due
                                                        activeTrigger.meter_hours_when_pm_due = pmDetails.Asset.meter_hours;
                                                        activeTrigger.datetime_when_pm_due = dueDate;
                                                        activeTrigger.status = (int)Status.Due;
                                                    }

                                                }
                                                // case 1 currentDate = 25/8/2021 and DueDate = 20/8/2021
                                                else if (dueDate < currentDate)
                                                {
                                                    // set the status is overdue
                                                    activeTrigger.meter_hours_when_pm_due = pmDetails.Asset.meter_hours;
                                                    activeTrigger.datetime_when_pm_due = dueDate;
                                                    activeTrigger.status = (int)Status.OverDue;
                                                }
                                                activeTrigger.due_datetime = dueDate;
                                                activeTrigger.created_at = DateTime.UtcNow;
                                            }
                                            else if (pmDetails.pm_trigger_by == (int)Status.MeterHours)
                                            {
                                                var dueMeterHours = pmDetails.meter_hours_starting_at.Value;
                                                if (lastTrigger != null)
                                                {
                                                    var triggerDetail = await _UoW.PMTriggersRepository.GetTriggerByID(lastTrigger.pm_trigger_id);
                                                    if (triggerDetail != null)
                                                    {
                                                        dueMeterHours = triggerDetail.PMTriggersRemarks.completed_at_meter_hours + pmDetails.meter_hours_repeates_every.Value;
                                                    }
                                                    else
                                                    {
                                                        if (pmDetails.is_trigger_on_starting_at == true)
                                                        {
                                                            dueMeterHours = pmDetails.meter_hours_starting_at.Value;
                                                        }
                                                        else
                                                        {
                                                            dueMeterHours = pmDetails.meter_hours_starting_at.Value + pmDetails.meter_hours_repeates_every.Value;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (pmDetails.is_trigger_on_starting_at == true)
                                                    {
                                                        dueMeterHours = pmDetails.meter_hours_starting_at.Value;
                                                    }
                                                    else
                                                    {
                                                        dueMeterHours = pmDetails.meter_hours_starting_at.Value + pmDetails.meter_hours_repeates_every.Value;
                                                    }
                                                }

                                                if (pmDetails.Asset == null)
                                                {
                                                    pmDetails.Asset = _UoW.AssetRepository.GetAssetByAssetID(pmDetails.asset_id.ToString());
                                                }

                                                // case 1 current Meter hours(200) < due meter hours(500)
                                                if (dueMeterHours >= pmDetails.Asset.meter_hours)
                                                {
                                                    // set the status new
                                                    activeTrigger.status = (int)Status.TriggerNew;
                                                }
                                                // case 2 current Meter hours = 500 and DueDate = 200
                                                else if (dueMeterHours < pmDetails.Asset.meter_hours)
                                                {
                                                    // set the status is overdue
                                                    activeTrigger.meter_hours_when_pm_due = dueMeterHours;
                                                    activeTrigger.datetime_when_pm_due = DateTime.UtcNow;
                                                    activeTrigger.status = (int)Status.OverDue;
                                                }
                                                activeTrigger.due_meter_hours = dueMeterHours;
                                                activeTrigger.created_at = DateTime.UtcNow;
                                            }
                                            else if (pmDetails.pm_trigger_by == (int)Status.TimeMeterHours)
                                            {
                                                var dueMeterHours = pmDetails.meter_hours_starting_at.Value;
                                                // get current Asset Meter Hours
                                                var currentDate = DateTime.UtcNow;
                                                pmDetails.datetime_starting_at = DateTime.UtcNow;
                                                var dueDate = pmDetails.datetime_starting_at.Value;

                                                if (lastTrigger != null)
                                                {
                                                    var triggerDetail = await _UoW.PMTriggersRepository.GetTriggerByID(lastTrigger.pm_trigger_id);
                                                    if (triggerDetail != null)
                                                    {
                                                        if (pmDetails.datetime_repeat_time_period_type == (int)Status.Month)
                                                        {
                                                            dueDate = triggerDetail.PMTriggersRemarks.completed_on.AddMonths(pmDetails.datetime_repeates_every.Value);
                                                        }
                                                        else if (pmDetails.datetime_repeat_time_period_type == (int)Status.Year)
                                                        {
                                                            dueDate = triggerDetail.PMTriggersRemarks.completed_on.AddYears(pmDetails.datetime_repeates_every.Value);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (pmDetails.is_trigger_on_starting_at == true)
                                                        {
                                                            dueDate = pmDetails.datetime_starting_at.Value;
                                                        }
                                                        else
                                                        {
                                                            if (pmDetails.datetime_repeat_time_period_type == (int)Status.Month)
                                                            {
                                                                dueDate = pmDetails.datetime_starting_at.Value.AddMonths(pmDetails.datetime_repeates_every.Value);
                                                            }
                                                            else if (pmDetails.datetime_repeat_time_period_type == (int)Status.Year)
                                                            {
                                                                dueDate = pmDetails.datetime_starting_at.Value.AddYears(pmDetails.datetime_repeates_every.Value);
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (pmDetails.is_trigger_on_starting_at == true)
                                                    {
                                                        dueDate = pmDetails.datetime_starting_at.Value;
                                                    }
                                                    else
                                                    {
                                                        if (pmDetails.datetime_repeat_time_period_type == (int)Status.Month)
                                                        {
                                                            dueDate = pmDetails.datetime_starting_at.Value.AddMonths(pmDetails.datetime_repeates_every.Value);
                                                        }
                                                        else if (pmDetails.datetime_repeat_time_period_type == (int)Status.Year)
                                                        {
                                                            dueDate = pmDetails.datetime_starting_at.Value.AddYears(pmDetails.datetime_repeates_every.Value);
                                                        }
                                                    }
                                                }
                                                // case 1 currentDate = 25/8/2021 and DueDate = 30/8/2021 or DueDate = 25/8/2021
                                                if (dueDate >= currentDate)
                                                {
                                                    // check the Trigger Configuration
                                                    // check date diff in days
                                                    double diff2 = (dueDate - currentDate).TotalDays;
                                                    if (diff2 > GlobalConstants.DateDiffForDueStatus)
                                                    {
                                                        // set the status new
                                                        activeTrigger.status = (int)Status.TriggerNew;
                                                    }
                                                    else
                                                    {
                                                        // set the status Due
                                                        activeTrigger.meter_hours_when_pm_due = pmDetails.Asset.meter_hours;
                                                        activeTrigger.datetime_when_pm_due = dueDate;
                                                        activeTrigger.status = (int)Status.Due;
                                                    }
                                                }
                                                // case 1 currentDate = 25/8/2021 and DueDate = 20/8/2021
                                                else if (dueDate < currentDate)
                                                {
                                                    // set the status is overdue
                                                    activeTrigger.meter_hours_when_pm_due = pmDetails.Asset.meter_hours;
                                                    activeTrigger.datetime_when_pm_due = dueDate;
                                                    activeTrigger.status = (int)Status.OverDue;
                                                }

                                                if (lastTrigger != null)
                                                {
                                                    var triggerDetail = await _UoW.PMTriggersRepository.GetTriggerByID(lastTrigger.pm_trigger_id);
                                                    if (triggerDetail != null)
                                                    {
                                                        dueMeterHours = triggerDetail.PMTriggersRemarks.completed_at_meter_hours + pmDetails.meter_hours_repeates_every.Value;
                                                    }
                                                    else
                                                    {
                                                        // get current Asset Meter Hours
                                                        if (pmDetails.is_trigger_on_starting_at == true)
                                                        {
                                                            dueMeterHours = pmDetails.meter_hours_starting_at.Value;
                                                        }
                                                        else
                                                        {
                                                            dueMeterHours = pmDetails.meter_hours_starting_at.Value + pmDetails.meter_hours_repeates_every.Value;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    // get current Asset Meter Hours
                                                    if (pmDetails.is_trigger_on_starting_at == true)
                                                    {
                                                        dueMeterHours = pmDetails.meter_hours_starting_at.Value;
                                                    }
                                                    else
                                                    {
                                                        dueMeterHours = pmDetails.meter_hours_starting_at.Value + pmDetails.meter_hours_repeates_every.Value;
                                                    }
                                                }

                                                if (activeTrigger.status == (int)Status.TriggerNew || activeTrigger.status == (int)Status.Due)
                                                {
                                                    if (pmDetails.Asset == null)
                                                    {
                                                        pmDetails.Asset = _UoW.AssetRepository.GetAssetByAssetID(pmDetails.asset_id.ToString());
                                                    }
                                                    // case 1 current Meter hours(200) < due meter hours(500)
                                                    if (dueMeterHours >= pmDetails.Asset.meter_hours)
                                                    {
                                                        if (activeTrigger.status != (int)Status.Due)
                                                        {
                                                            // set the status new
                                                            activeTrigger.status = (int)Status.TriggerNew;
                                                        }
                                                    }
                                                    // case 2 current Meter hours = 500 and DueDate = 200
                                                    else if (dueMeterHours < pmDetails.Asset.meter_hours)
                                                    {
                                                        // set the status is overdue
                                                        activeTrigger.meter_hours_when_pm_due = dueMeterHours;
                                                        activeTrigger.datetime_when_pm_due = DateTime.UtcNow;
                                                        activeTrigger.status = (int)Status.OverDue;
                                                    }
                                                }

                                                activeTrigger.due_meter_hours = dueMeterHours;
                                                activeTrigger.due_datetime = dueDate;
                                                activeTrigger.created_at = DateTime.UtcNow;
                                            }
                                        }

                                        if (requestedStatus == (int)Status.PMWaiting || requestedStatus == (int)Status.PMInProgress)
                                        {
                                            activeTrigger.status = requestedStatus;
                                        }

                                        activeTrigger.asset_pm_status = pmRequest.status;
                                        result = await _UoW.PMTriggersRepository.Update(activeTrigger);
                                        if (result > 0)
                                        {
                                            _UoW.SaveChanges();
                                            _dbtransaction.Commit();
                                            pmResponse = _mapper.Map<AssetPMResponseModel>(pmDetails);
                                            pmResponse.response_status = result;
                                        }
                                        else
                                        {
                                            _dbtransaction.Rollback();
                                        }
                                    }
                                    else
                                    {
                                        _UoW.SaveChanges();
                                        _dbtransaction.Commit();
                                    }
                                }
                                else
                                {
                                    _UoW.SaveChanges();
                                    _dbtransaction.Commit();
                                    result = (int)ResponseStatusNumber.Success;
                                }*/
                            }
                        }
                    }
                    else
                    {
                        pmResponse.response_status = (int)ResponseStatusNumber.NotExists;
                    }

                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    _logger.LogError(e.Message);
                    pmResponse.response_status = (int)ResponseStatusNumber.Error;
                }
            }

            return pmResponse;
        }

        public async Task<int> CalculateActualAssetPMsCountByFrequency(AssetPMs asset_pm)
        {
            int count = 0;
            var max_condition = GetConditionTypeForPM(asset_pm.Asset);
            var pm_condition_trigger = asset_pm.AssetPMsTriggerConditionMapping.Where(x => x.condition_type_id == max_condition).FirstOrDefault();
            if (pm_condition_trigger != null)
            {
                int? frequency = pm_condition_trigger.datetime_repeates_every.Value;
                int? frequency_type = pm_condition_trigger.datetime_repeat_time_period_type.Value;
                var months_to_add_pms = ConfigurationManager.AppSettings["MonthstoAddAssetPMs"];
                var years_to_add_pms = ConfigurationManager.AppSettings["YearstoAddAssetPMs"];

                if (pm_condition_trigger != null && frequency != null && frequency > 0 && frequency_type != null && frequency_type > 0)
                {
                    if (frequency_type == (int)Status.Month)
                        count = int.Parse(months_to_add_pms) / frequency.Value;
                    else if (frequency_type == (int)Status.Year)
                        count = int.Parse(years_to_add_pms) / frequency.Value;
                }
            }
            return count;
        }

        public async Task<int> AddRemoveAssetPMsByFrequency(bool requested_for_add, int count, AssetPMs assetpm)
        {
            try
            {
                //Add AssetPMs based on Frequency
                if (requested_for_add)
                {
                    for (int i = 0; i < count; i++)
                    {
                        AssetPMs assetPMs_ = new AssetPMs();
                        assetPMs_ = _mapper.Map<AssetPMs>(assetpm);

                        assetPMs_.created_at = DateTime.UtcNow;
                        assetPMs_.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        assetPMs_.AssetPMAttachments.ToList().ForEach(y =>
                        {
                            y.asset_pm_plan_id = assetPMs_.asset_pm_plan_id;
                            y.asset_id = assetPMs_.asset_id;
                            y.created_at = DateTime.UtcNow;
                            y.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        });
                        assetPMs_.AssetPMsTriggerConditionMapping.ToList().ForEach(y =>
                        {
                            y.created_at = DateTime.UtcNow;
                            y.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                            y.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        });

                        var insert = await _UoW.BaseGenericRepository<AssetPMs>().Insert(assetPMs_);
                    }
                    _UoW.SaveChanges();
                }
                // Remove AssetPMs based on Frequency
                else
                {
                    var assetpm_ids_to_delete = _UoW.AssetPMsRepository.GetAssetPMsIdsByAssetIdPMId(assetpm.asset_id, assetpm.pm_id.Value)
                        .Take(count);

                    foreach (var assetpm_id in assetpm_ids_to_delete)
                    {
                        await DeleteAssetPM(assetpm_id);
                    }
                }
            }
            catch (Exception e)
            {
            }

            return 1;
        }

        public async Task<int> UpdateStartDateOfAllDuplicateAssetPMs(AssetPMs assetpm)
        {
            try
            {   
                var get_assetpms_list = _UoW.AssetPMsRepository.GetAssetPMsByAssetIdPMId(assetpm.asset_id, assetpm.pm_id.Value,assetpm.asset_pm_id);
                DateTime? datetime_starting_at = assetpm.datetime_starting_at;
                var max_condition = GetConditionTypeForPM(assetpm.Asset);
                var pm_condition_trigger = assetpm.AssetPMsTriggerConditionMapping.Where(x => x.condition_type_id == max_condition).FirstOrDefault();
                int? frequency = pm_condition_trigger.datetime_repeates_every.Value;
                int? frequency_type = pm_condition_trigger.datetime_repeat_time_period_type.Value;

                foreach (var pmDetails in get_assetpms_list)
                {
                    if (pmDetails.AssetPMsTriggerConditionMapping != null && pmDetails.AssetPMsTriggerConditionMapping.Count > 0)
                    {
                        pmDetails.AssetPMsTriggerConditionMapping.ToList().ForEach(x =>
                        {
                            var requested_trigger = assetpm.AssetPMsTriggerConditionMapping.Where(q => q.condition_type_id == x.condition_type_id).FirstOrDefault();
                            if (requested_trigger != null)
                            {
                                x.datetime_repeates_every = requested_trigger.datetime_repeates_every;
                                x.datetime_repeat_time_period_type = requested_trigger.datetime_repeat_time_period_type;
                                x.modified_at = DateTime.UtcNow;
                                x.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            }
                        });
                    }

                    if (frequency_type == (int)Status.Month)
                        datetime_starting_at = datetime_starting_at.Value.AddMonths(frequency.Value);
                    else if (frequency_type == (int)Status.Year)
                        datetime_starting_at = datetime_starting_at.Value.AddYears(frequency.Value);

                    pmDetails.datetime_starting_at = datetime_starting_at;
                    pmDetails.asset_pm_completed_date = datetime_starting_at;
                    pmDetails.modified_at = DateTime.UtcNow;
                    pmDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    var update = await _UoW.BaseGenericRepository<AssetPMs>().Update(pmDetails);
                }
                _UoW.SaveChanges();
                foreach (var pmDetails in get_assetpms_list)
                {
                    var this_yr_count = _UoW.AssetPMsRepository.GetAssetPMCountByYear(pmDetails.datetime_starting_at.Value, assetpm.asset_id, assetpm.pm_id.Value);
                    int lastSpaceIndex = assetpm.title.LastIndexOf(' ');

                    pmDetails.title = assetpm.title.Substring(0, lastSpaceIndex - 7) + " " + pmDetails.datetime_starting_at.Value.Year + " - " + (this_yr_count);
                    var update2 = await _UoW.BaseGenericRepository<AssetPMs>().Update(pmDetails);
                }
                _UoW.SaveChanges();
            }
            catch(Exception e)
            {
            }
            return 1;
        }

        public async Task<int> UpdateDueDateDueInDueFlagForAssetPMsByAssetId(Guid asset_id)
        {
            int res = (int)ResponseStatusNumber.Error;
            try
            {
                var get_asset = _UoW.AssetRepository.GetAssetByAssetID(asset_id.ToString());

                if (get_asset != null && get_asset.AssetPMs != null)
                {
                    var pms = get_asset.AssetPMs.Where(x => !x.is_archive && x.status != (int)Status.Completed && x.datetime_starting_at != null && !x.is_pm_inspection_manual).ToList();

                    foreach (var asset_pm in pms)
                    {
                        var due_flag = 0;
                        var max_condition = GetConditionTypeForPM(get_asset);

                        if (asset_pm.AssetPMsTriggerConditionMapping != null)
                        {
                            var pm_condition_trigger = asset_pm.AssetPMsTriggerConditionMapping.Where(x => x.condition_type_id == max_condition).FirstOrDefault();

                            if (pm_condition_trigger != null && pm_condition_trigger.datetime_repeates_every != null && pm_condition_trigger.datetime_repeat_time_period_type != null)
                            {
                                // get PM end date -- PM_end_date is Due_date
                                var due_date = CalculatePMEndDate(asset_pm.datetime_starting_at.Value, pm_condition_trigger.datetime_repeates_every.Value, pm_condition_trigger.datetime_repeat_time_period_type.Value);

                                var due_in = DateTimeUtil.GetDueOverdueTimingByDueDate(due_date).Item1;

                                if (DateTime.UtcNow.Date == due_date.Date)
                                {
                                    due_flag = (int)pm_due_overdue_flag.PM_Due;
                                }
                                else if (DateTime.UtcNow.Date > due_date.Date)
                                {
                                    due_flag = (int)pm_due_overdue_flag.PM_Overdue;
                                }
                                else
                                {
                                    due_flag = (int)pm_due_overdue_flag.PM_OnTrack;
                                }

                                asset_pm.due_date = due_date;
                                asset_pm.pm_due_overdue_flag = due_flag;
                                asset_pm.pm_due_time_duration = due_in;

                                var update = await _UoW.BaseGenericRepository<AssetPMs>().Update(asset_pm);
                                _UoW.SaveChanges();
                                res = (int)ResponseStatusNumber.Success;
                            }
                        }
                    }

                    foreach (var assetpm in pms.Where(x => x.due_date != null))
                    {
                        var title = _UoW.AssetPMsRepository.GetAssetPMCountByDueDate(assetpm.due_date.Value, assetpm.asset_id, assetpm.pm_id.Value);
                        assetpm.title = title;
                        assetpm.modified_at = DateTime.UtcNow;
                        var update = await _UoW.BaseGenericRepository<AssetPMs>().Update(assetpm);
                        _UoW.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
            }
            return res;
        }

        public async Task<int> ScriptForToUpdateAssetPMsDueDateDueInDueFlag()
        {
            // 

            int count = 0;
            int res = (int)ResponseStatusNumber.Error;
            try
            {
                var get_all_assetPMs = _UoW.AssetPMsRepository.GetAllAssetPMsForDueDateScript();
                _logger.LogInformation("AssetPM script total count " + get_all_assetPMs.Count().ToString());
                foreach (var asset_pm in get_all_assetPMs)
                {
                    var due_flag = 0;
                    var max_condition = GetConditionTypeForPM(asset_pm.Asset);

                    if (asset_pm.AssetPMsTriggerConditionMapping != null)
                    {
                        var pm_condition_trigger = asset_pm.AssetPMsTriggerConditionMapping.Where(x => x.condition_type_id == max_condition).FirstOrDefault();

                        if (pm_condition_trigger != null && pm_condition_trigger.datetime_repeates_every != null && pm_condition_trigger.datetime_repeat_time_period_type != null)
                        {
                            // get PM end date -- PM_end_date is Due_date
                            var due_date = CalculatePMEndDate(asset_pm.datetime_starting_at.Value, pm_condition_trigger.datetime_repeates_every.Value, pm_condition_trigger.datetime_repeat_time_period_type.Value);

                            var due_in = DateTimeUtil.GetDueOverdueTimingByDueDate(due_date).Item1;

                            if (DateTime.UtcNow.Date == due_date.Date)
                            {
                                due_flag = (int)pm_due_overdue_flag.PM_Due;
                            }
                            else if (DateTime.UtcNow.Date > due_date.Date)
                            {
                                due_flag = (int)pm_due_overdue_flag.PM_Overdue;
                            }
                            else
                            {
                                due_flag = (int)pm_due_overdue_flag.PM_OnTrack;
                            }

                            asset_pm.due_date = due_date;
                            asset_pm.pm_due_overdue_flag = due_flag;
                            asset_pm.pm_due_time_duration = due_in;
                            asset_pm.modified_at = DateTime.UtcNow;
                            var update = await _UoW.BaseGenericRepository<AssetPMs>().Update(asset_pm);
                            _UoW.SaveChanges();
                            res = (int)ResponseStatusNumber.Success;
                            count++;
                            _logger.LogInformation("AssetPM script completed count " + count.ToString());
                        }
                    }
                }

            }catch(Exception e)
            {
                _logger.LogError("AssetPM script Exception " + e.Message);
            }
            return res;
        }

        public async Task<int> AutomateScriptForToUpdateAssetPMsDueDateDueInDueFlag()
        {
            for(int i = 0; i < 28; i++)
            {
                await ScriptForToUpdateAssetPMsDueDateDueInDueFlag();
            }
            await ScriptForToUpdateAssetPMsDueDateDueInDueFlag();

            return 1;
        }
        public async Task<AssetPMResponseModel> DuplicateAssetPM(UpdateAssetPMRequestModel pmRequest)
        {
            AssetPMResponseModel pmResponse = new AssetPMResponseModel();
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    AssetPMs assetPM = new AssetPMs();
                    assetPM = _mapper.Map<AssetPMs>(pmRequest);
                    assetPM.status = (int)Status.TriggerNew;
                    foreach (var tasks in assetPM.AssetPMTasks)
                    {
                        tasks.created_at = DateTime.UtcNow;
                        tasks.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        tasks.asset_id = assetPM.asset_id;
                        tasks.asset_pm_plan_id = assetPM.asset_pm_plan_id;
                    }
                    assetPM.AssetPMAttachments.ToList().ForEach(attachment =>
                    {
                        attachment.created_at = DateTime.UtcNow;
                        attachment.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        attachment.asset_id = assetPM.asset_id;
                        attachment.asset_pm_plan_id = assetPM.asset_pm_plan_id;
                    });
                    assetPM.created_at = DateTime.UtcNow;
                    assetPM.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    result = _UoW.AssetPMsRepository.Insert(assetPM).Result;
                    if (result > 0)
                    {
                        // create Triggers related to this Asset PM ID
                        // Create Triggers for all Asset PM items
                        List<AssetPMs> pmlist = new List<AssetPMs>();
                        pmlist.Add(assetPM);
                        var pmTriggers = await CreatePMTrigger(pmlist);
                        if (pmTriggers?.Count > 0)
                        {
                            result = await _UoW.PMTriggersRepository.InsertList(pmTriggers);
                            if (result > 0)
                            {
                                _UoW.SaveChanges();
                                _dbtransaction.Commit();
                                pmResponse = _mapper.Map<AssetPMResponseModel>(assetPM);
                                pmResponse.response_status = result;
                            }
                            else
                            {
                                _dbtransaction.Rollback();
                            }
                        }
                        else
                        {
                            _dbtransaction.Rollback();
                        }
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    _logger.LogError(e.Message);
                    pmResponse.response_status = (int)ResponseStatusNumber.Error;
                }
            }

            return pmResponse;
        }

        public async Task<int> DeleteAssetPM(Guid id)
        {
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var pmDetails = await _UoW.AssetPMsRepository.GetAssetPMById(id);
                    if (pmDetails?.asset_pm_id != null)
                    {
                        pmDetails.is_archive = true;
                        pmDetails.modified_at = DateTime.UtcNow;
                        pmDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        pmDetails.AssetPMTasks.ToList().ForEach(x =>
                        {
                            x.is_archive = true;
                            x.modified_at = DateTime.UtcNow;
                            x.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        });
                        pmDetails.AssetPMAttachments.ToList().ForEach(x =>
                        {
                            x.is_archive = true;
                            x.modified_at = DateTime.UtcNow;
                            x.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        });
                        // remove triggers which are attached with this Asset PM ID
                        result = _UoW.AssetPMsRepository.Update(pmDetails).Result;
                        if (result > 0)
                        {
                            var pmTriggers = await _UoW.PMTriggersRepository.GetAssetPMTriggers(pmDetails.asset_pm_id);
                            if (pmTriggers?.Count > 0)
                            {
                                foreach (var trigger in pmTriggers)
                                {
                                    trigger.is_archive = true;
                                    trigger.modified_at = DateTime.UtcNow;
                                    trigger.PMTriggersTasks.ToList().ForEach(triggerTask =>
                                    {
                                        triggerTask.is_archive = true;
                                        triggerTask.modified_at = DateTime.UtcNow;
                                    });
                                    result = await _UoW.PMTriggersRepository.Update(trigger);
                                }
                                if (result > 0)
                                {
                                    _dbtransaction.Commit();
                                    result = (int)ResponseStatusNumber.Success;
                                }
                                else
                                {
                                    _dbtransaction.Rollback();
                                }
                            }
                            else
                            {
                                _dbtransaction.Commit();
                                result = (int)ResponseStatusNumber.Success;
                            }
                        }
                        else
                        {
                            _dbtransaction.Rollback();
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
                    _logger.LogError(e.Message);
                    throw e;
                }
            }

            return result;
        }

        public async Task<List<PMTriggers>> CreatePMTrigger(List<AssetPMs> assetPMs)
        {
            List<PMTriggers> pmTriggers = new List<PMTriggers>();
            try
            {
                foreach (var assetPM in assetPMs)
                {
                    // check the trigger type
                    // trigger type is fixed on
                    if (assetPM.pm_trigger_type == (int)Status.FixedOneTime)
                    {
                        PMTriggers pMTrigger = new PMTriggers();
                        // get current Asset Meter Hours
                        if (assetPM.Asset == null)
                        {
                            assetPM.Asset = _UoW.AssetRepository.GetAssetByAssetID(assetPM.asset_id.ToString());
                        }
                        // check the trigger by time/meter hours/time+meter hours
                        if (assetPM.pm_trigger_by == (int)Status.Time)
                        {
                            var currentDate = DateTime.UtcNow;
                            var dueDate = assetPM.datetime_due_at.Value;
                            // case 1 currentDate = 25/8/2021 and DueDate = 30/8/2021 or DueDate = 25/8/2021
                            if (dueDate >= currentDate)
                            {
                                // check the Trigger Configuration
                                // check date diff in days
                                double diff2 = (dueDate - currentDate).TotalDays;
                                if (diff2 > GlobalConstants.DateDiffForDueStatus)
                                {
                                    // set the status new
                                    pMTrigger.status = (int)Status.TriggerNew;
                                }
                                else
                                {
                                    // set the status Due
                                    pMTrigger.meter_hours_when_pm_due = assetPM.Asset.meter_hours;
                                    pMTrigger.datetime_when_pm_due = dueDate;
                                    pMTrigger.status = (int)Status.Due;
                                }

                            }
                            // case 1 currentDate = 25/8/2021 and DueDate = 20/8/2021
                            else if (dueDate < currentDate)
                            {
                                // set the status is overdue
                                pMTrigger.meter_hours_when_pm_due = assetPM.Asset.meter_hours;
                                pMTrigger.datetime_when_pm_due = dueDate;
                                pMTrigger.status = (int)Status.OverDue;
                            }

                            pMTrigger.asset_pm_id = assetPM.asset_pm_id;
                            pMTrigger.asset_id = assetPM.asset_id;
                            pMTrigger.due_datetime = dueDate;
                            pMTrigger.created_at = DateTime.UtcNow;
                        }
                        else if (assetPM.pm_trigger_by == (int)Status.MeterHours)
                        {
                            var dueMeterHours = assetPM.meter_hours_due_at.Value;
                            // case 1 current Meter hours(200) < due meter hours(500)
                            if (dueMeterHours >= assetPM.Asset.meter_hours)
                            {
                                // set the status new
                                pMTrigger.status = (int)Status.TriggerNew;
                            }
                            // case 2 current Meter hours = 500 and DueDate = 200
                            else if (dueMeterHours < assetPM.Asset.meter_hours)
                            {
                                // set the status is overdue
                                pMTrigger.meter_hours_when_pm_due = dueMeterHours;
                                pMTrigger.datetime_when_pm_due = DateTime.UtcNow;
                                pMTrigger.status = (int)Status.OverDue;
                            }
                            pMTrigger.asset_pm_id = assetPM.asset_pm_id;
                            pMTrigger.asset_id = assetPM.asset_id;
                            pMTrigger.due_meter_hours = dueMeterHours;
                            pMTrigger.created_at = DateTime.UtcNow;
                        }
                        else if (assetPM.pm_trigger_by == (int)Status.TimeMeterHours)
                        {
                            var dueMeterHours = assetPM.meter_hours_due_at.Value;
                            // get current Asset Meter Hours
                            var currentDate = DateTime.UtcNow;
                            var dueDate = assetPM.datetime_due_at.Value;
                            // case 1 currentDate = 25/8/2021 and DueDate = 30/8/2021 or DueDate = 25/8/2021
                            if (dueDate >= currentDate)
                            {
                                // check the Trigger Configuration
                                // check date diff in days
                                double diff2 = (dueDate - currentDate).TotalDays;
                                if (diff2 > GlobalConstants.DateDiffForDueStatus)
                                {
                                    // set the status new
                                    pMTrigger.status = (int)Status.TriggerNew;
                                }
                                else
                                {
                                    // set the status Due
                                    pMTrigger.meter_hours_when_pm_due = assetPM.Asset.meter_hours;
                                    pMTrigger.datetime_when_pm_due = dueDate;
                                    pMTrigger.status = (int)Status.Due;
                                }
                            }
                            // case 1 currentDate = 25/8/2021 and DueDate = 20/8/2021
                            else if (dueDate < currentDate)
                            {
                                // set the status is overdue
                                pMTrigger.meter_hours_when_pm_due = assetPM.Asset.meter_hours;
                                pMTrigger.datetime_when_pm_due = dueDate;
                                pMTrigger.status = (int)Status.OverDue;
                            }

                            if (pMTrigger.status == (int)Status.TriggerNew || pMTrigger.status == (int)Status.Due)
                            {
                                if (assetPM.Asset == null)
                                {
                                    assetPM.Asset = _UoW.AssetRepository.GetAssetByAssetID(assetPM.asset_id.ToString());
                                }
                                // case 1 current Meter hours(200) < due meter hours(500)
                                if (dueMeterHours >= assetPM.Asset.meter_hours)
                                {
                                    // set the status new
                                    if (pMTrigger.status != (int)Status.Due)
                                    {
                                        pMTrigger.status = (int)Status.TriggerNew;
                                    }
                                }
                                // case 2 current Meter hours = 500 and DueDate = 200
                                else if (dueMeterHours < assetPM.Asset.meter_hours)
                                {
                                    // set the status is overdue
                                    pMTrigger.meter_hours_when_pm_due = dueMeterHours;
                                    pMTrigger.datetime_when_pm_due = DateTime.UtcNow;
                                    pMTrigger.status = (int)Status.OverDue;
                                }
                            }

                            pMTrigger.asset_pm_id = assetPM.asset_pm_id;
                            pMTrigger.asset_id = assetPM.asset_id;
                            pMTrigger.due_meter_hours = dueMeterHours;
                            pMTrigger.due_datetime = dueDate;
                            pMTrigger.created_at = DateTime.UtcNow;
                        }


                        List<PMTriggersTasks> pmTriggersTasks = new List<PMTriggersTasks>();
                        var pmTasks = assetPM.AssetPMTasks.ToList().Where(x => !x.is_archive).ToList();
                        if (pmTasks?.Count > 0)
                        {
                            foreach (var x in pmTasks)
                            {
                                var taskdetails = await _UoW.TaskRepository.GetTaskById(x.task_id);
                                var hours = taskdetails.task_est_hours;
                                var minutes = taskdetails.task_est_minutes;
                                var HoursMinutes = PMTriggersUtil.ConvertHoursToMinutes(hours);
                                HoursMinutes += minutes;
                                pMTrigger.total_est_time_hours = pMTrigger.total_est_time_hours == null ? 0 : pMTrigger.total_est_time_hours;
                                pMTrigger.total_est_time_minutes = pMTrigger.total_est_time_minutes == null ? 0 : pMTrigger.total_est_time_minutes;
                                pMTrigger.total_est_time_hours += PMTriggersUtil.ConvertMinutesToHours(HoursMinutes);
                                pMTrigger.total_est_time_minutes += PMTriggersUtil.ConvertMinutesToHoursOfMinutes(HoursMinutes);
                                PMTriggersTasks pmTriggersTask = new PMTriggersTasks();
                                pmTriggersTask.asset_id = pMTrigger.asset_id;
                                pmTriggersTask.asset_pm_id = pMTrigger.asset_pm_id;
                                pmTriggersTask.asset_pm_task_id = x.asset_pm_task_id;
                                pmTriggersTask.task_id = x.task_id;
                                pmTriggersTask.status = (int)Status.TriggetTaskNew;
                                pmTriggersTask.created_at = DateTime.UtcNow;
                                pmTriggersTask.modified_at = DateTime.UtcNow;
                                pmTriggersTask.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                pmTriggersTask.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                pmTriggersTasks.Add(pmTriggersTask);
                            }
                            pMTrigger.PMTriggersTasks = pmTriggersTasks;
                        }

                        pMTrigger.asset_pm_status = (int)Status.TriggerNew;
                        pmTriggers.Add(pMTrigger);
                    }
                    // trigger type if recurring
                    else if (assetPM.pm_trigger_type == (int)Status.Recurring)
                    {
                        PMTriggers pMTrigger = new PMTriggers();
                        if (assetPM.Asset == null)
                        {
                            assetPM.Asset = _UoW.AssetRepository.GetAssetByAssetID(assetPM.asset_id.ToString());
                        }
                        if (assetPM.pm_trigger_by == (int)Status.Time)
                        {
                            var currentDate = DateTime.UtcNow;
                            var dueDate = assetPM.datetime_starting_at.Value;
                            if (assetPM.is_trigger_on_starting_at == true)
                            {
                                dueDate = assetPM.datetime_starting_at.Value;
                            }
                            else
                            {
                                if (assetPM.datetime_repeat_time_period_type == (int)Status.Month)
                                {
                                    dueDate = assetPM.datetime_starting_at.Value.AddMonths(assetPM.datetime_repeates_every.Value);
                                }
                                else if (assetPM.datetime_repeat_time_period_type == (int)Status.Year)
                                {
                                    dueDate = assetPM.datetime_starting_at.Value.AddYears(assetPM.datetime_repeates_every.Value);
                                }
                            }

                            // case 1 currentDate = 25/8/2021 and DueDate = 30/8/2021 or DueDate = 25/8/2021
                            if (dueDate >= currentDate)
                            {
                                // check the Trigger Configuration
                                // check date diff in days
                                double diff2 = (dueDate - currentDate).TotalDays;
                                if (diff2 > GlobalConstants.DateDiffForDueStatus)
                                {
                                    // set the status new
                                    pMTrigger.status = (int)Status.TriggerNew;
                                }
                                else
                                {
                                    // set the status Due
                                    pMTrigger.meter_hours_when_pm_due = assetPM.Asset.meter_hours;
                                    pMTrigger.datetime_when_pm_due = dueDate;
                                    pMTrigger.status = (int)Status.Due;
                                }

                            }
                            // case 1 currentDate = 25/8/2021 and DueDate = 20/8/2021
                            else if (dueDate < currentDate)
                            {
                                // set the status is overdue
                                pMTrigger.meter_hours_when_pm_due = assetPM.Asset.meter_hours;
                                pMTrigger.datetime_when_pm_due = dueDate;
                                pMTrigger.status = (int)Status.OverDue;
                            }
                            pMTrigger.asset_pm_id = assetPM.asset_pm_id;
                            pMTrigger.asset_id = assetPM.asset_id;
                            pMTrigger.due_datetime = dueDate;
                            pMTrigger.created_at = DateTime.UtcNow;
                        }
                        else if (assetPM.pm_trigger_by == (int)Status.MeterHours)
                        {
                            var dueMeterHours = assetPM.meter_hours_starting_at.Value;
                            // get current Asset Meter Hours
                            if (assetPM.is_trigger_on_starting_at == true)
                            {
                                dueMeterHours = assetPM.meter_hours_starting_at.Value;
                            }
                            else
                            {
                                dueMeterHours = assetPM.meter_hours_starting_at.Value + assetPM.meter_hours_repeates_every.Value;
                            }
                            if (assetPM.Asset == null)
                            {
                                assetPM.Asset = _UoW.AssetRepository.GetAssetByAssetID(assetPM.asset_id.ToString());
                            }
                            // case 1 current Meter hours(200) < due meter hours(500)
                            if (dueMeterHours >= assetPM.Asset.meter_hours)
                            {
                                // set the status new
                                pMTrigger.status = (int)Status.TriggerNew;
                            }
                            // case 2 current Meter hours = 500 and DueDate = 200
                            else if (dueMeterHours < assetPM.Asset.meter_hours)
                            {
                                // set the status is overdue
                                pMTrigger.meter_hours_when_pm_due = dueMeterHours;
                                pMTrigger.datetime_when_pm_due = DateTime.UtcNow;
                                pMTrigger.status = (int)Status.OverDue;
                            }
                            pMTrigger.asset_pm_id = assetPM.asset_pm_id;
                            pMTrigger.asset_id = assetPM.asset_id;
                            pMTrigger.due_meter_hours = dueMeterHours;
                            pMTrigger.created_at = DateTime.UtcNow;
                        }
                        else if (assetPM.pm_trigger_by == (int)Status.TimeMeterHours)
                        {
                            var dueMeterHours = assetPM.meter_hours_starting_at.Value;
                            // get current Asset Meter Hours
                            var currentDate = DateTime.UtcNow;
                            //var dueDate = assetPM.datetime_starting_at.Value;
                            assetPM.datetime_starting_at = DateTime.UtcNow;
                            var dueDate = assetPM.datetime_starting_at.Value;
                            if (assetPM.is_trigger_on_starting_at == true)
                            {
                                dueDate = assetPM.datetime_starting_at.Value;
                            }
                            else
                            {
                                if (assetPM.datetime_repeat_time_period_type == (int)Status.Month)
                                {
                                    dueDate = assetPM.datetime_starting_at.Value.AddMonths(assetPM.datetime_repeates_every.Value);
                                }
                                else if (assetPM.datetime_repeat_time_period_type == (int)Status.Year)
                                {
                                    dueDate = assetPM.datetime_starting_at.Value.AddYears(assetPM.datetime_repeates_every.Value);
                                }
                            }
                            // case 1 currentDate = 25/8/2021 and DueDate = 30/8/2021 or DueDate = 25/8/2021
                            if (dueDate >= currentDate)
                            {
                                // check the Trigger Configuration
                                // check date diff in days
                                double diff2 = (dueDate - currentDate).TotalDays;
                                if (diff2 > GlobalConstants.DateDiffForDueStatus)
                                {
                                    // set the status new
                                    pMTrigger.status = (int)Status.TriggerNew;
                                }
                                else
                                {
                                    // set the status Due
                                    pMTrigger.meter_hours_when_pm_due = assetPM.Asset.meter_hours;
                                    pMTrigger.datetime_when_pm_due = dueDate;
                                    pMTrigger.status = (int)Status.Due;
                                }
                            }
                            // case 1 currentDate = 25/8/2021 and DueDate = 20/8/2021
                            else if (dueDate < currentDate)
                            {
                                // set the status is overdue
                                pMTrigger.meter_hours_when_pm_due = assetPM.Asset.meter_hours;
                                pMTrigger.datetime_when_pm_due = dueDate;
                                pMTrigger.status = (int)Status.OverDue;
                            }

                            // get current Asset Meter Hours
                            if (assetPM.is_trigger_on_starting_at == true)
                            {
                                dueMeterHours = assetPM.meter_hours_starting_at.Value;
                            }
                            else
                            {
                                dueMeterHours = assetPM.meter_hours_starting_at.Value + assetPM.meter_hours_repeates_every.Value;
                            }

                            if (pMTrigger.status == (int)Status.TriggerNew || pMTrigger.status == (int)Status.Due)
                            {
                                if (assetPM.Asset == null)
                                {
                                    assetPM.Asset = _UoW.AssetRepository.GetAssetByAssetID(assetPM.asset_id.ToString());
                                }
                                // case 1 current Meter hours(200) < due meter hours(500)
                                if (dueMeterHours >= assetPM.Asset.meter_hours)
                                {
                                    if (pMTrigger.status != (int)Status.Due)
                                    {
                                        // set the status new
                                        pMTrigger.status = (int)Status.TriggerNew;
                                    }
                                }
                                // case 2 current Meter hours = 500 and DueDate = 200
                                else if (dueMeterHours < assetPM.Asset.meter_hours)
                                {
                                    // set the status is overdue
                                    pMTrigger.meter_hours_when_pm_due = dueMeterHours;
                                    pMTrigger.datetime_when_pm_due = DateTime.UtcNow;
                                    pMTrigger.status = (int)Status.OverDue;
                                }
                            }

                            pMTrigger.asset_pm_id = assetPM.asset_pm_id;
                            pMTrigger.asset_id = assetPM.asset_id;
                            pMTrigger.due_meter_hours = dueMeterHours;
                            pMTrigger.due_datetime = dueDate;
                            pMTrigger.created_at = DateTime.UtcNow;
                        }

                        List<PMTriggersTasks> pmTriggersTasks = new List<PMTriggersTasks>();
                        var pmTasks = assetPM.AssetPMTasks.ToList().Where(x => !x.is_archive).ToList();
                        if (pmTasks?.Count > 0)
                        {
                            foreach (var x in pmTasks)
                            {
                                var taskdetails = await _UoW.TaskRepository.GetTaskById(x.task_id);
                                var hours = taskdetails.task_est_hours;
                                var minutes = taskdetails.task_est_minutes;
                                var HoursMinutes = PMTriggersUtil.ConvertHoursToMinutes(hours);
                                HoursMinutes += minutes;
                                pMTrigger.total_est_time_hours = pMTrigger.total_est_time_hours == null ? 0 : pMTrigger.total_est_time_hours;
                                pMTrigger.total_est_time_minutes = pMTrigger.total_est_time_minutes == null ? 0 : pMTrigger.total_est_time_minutes;
                                pMTrigger.total_est_time_hours += PMTriggersUtil.ConvertMinutesToHours(HoursMinutes);
                                pMTrigger.total_est_time_minutes += PMTriggersUtil.ConvertMinutesToHoursOfMinutes(HoursMinutes);
                                PMTriggersTasks pmTriggersTask = new PMTriggersTasks();
                                pmTriggersTask.asset_id = pMTrigger.asset_id;
                                pmTriggersTask.asset_pm_id = pMTrigger.asset_pm_id;
                                pmTriggersTask.asset_pm_task_id = x.asset_pm_task_id;
                                pmTriggersTask.task_id = x.task_id;
                                pmTriggersTask.status = (int)Status.TriggetTaskNew;
                                pmTriggersTask.created_at = DateTime.UtcNow;
                                pmTriggersTask.modified_at = DateTime.UtcNow;
                                pmTriggersTask.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                pmTriggersTask.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                pmTriggersTasks.Add(pmTriggersTask);
                            }
                            pMTrigger.PMTriggersTasks = pmTriggersTasks;
                        }

                        pMTrigger.asset_pm_status = (int)Status.TriggerNew;
                        pmTriggers.Add(pMTrigger);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Create PM trigger error " + e.Message);
                throw e;
            }
            return pmTriggers;
        }

        public async Task<int> MarkCompletedPM(PMMarkCompletedRequestModel pmRequest)
        {
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var pmTriggersRemarks = _mapper.Map<PMTriggersRemarks>(pmRequest);
                    pmTriggersRemarks.created_at = DateTime.UtcNow;
                    pmTriggersRemarks.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    result = _UoW.PMTriggersRemarksRepository.Insert(pmTriggersRemarks).Result;
                    if (result > 0)
                    {
                        var Trigger = await _UoW.PMTriggersRepository.GetTriggerByID(pmRequest.pm_trigger_id);
                        if (Trigger != null)
                        {
                            Trigger.asset_pm_status = (int)Status.TriggerCompleted;
                            Trigger.status = (int)Status.TriggerCompleted;
                            Trigger.modified_at = DateTime.UtcNow;
                            Trigger.PMTriggersTasks.ToList().ForEach(x =>
                            {
                                x.status = (int)Status.TriggetTaskCompleted;
                                x.modified_at = DateTime.UtcNow;
                                x.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            });
                            result = await _UoW.PMTriggersRepository.Update(Trigger);
                            if (result > 0)
                            {
                                CompletedPMTriggers completedPMTriggers = new CompletedPMTriggers();
                                completedPMTriggers.asset_pm_id = Trigger.asset_pm_id;
                                completedPMTriggers.pm_trigger_id = Trigger.pm_trigger_id;
                                completedPMTriggers.created_at = DateTime.UtcNow;
                                completedPMTriggers.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                result = await _UoW.CompletedPMTriggerRepository.Insert(completedPMTriggers);
                                var assetPM = Trigger.AssetPMs;
                                assetPM.status = (int)Status.PMCompleted;
                                assetPM.modified_at = DateTime.UtcNow;
                                assetPM.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                var response = await _UoW.AssetPMsRepository.Update(assetPM);
                                if (response > 0)
                                {
                                    // Create New Scheduler if this Trigger is recursive
                                    if (Trigger.AssetPMs.pm_trigger_type == (int)Status.Recurring)
                                    {
                                        var PMTriggers = await CreateRecurringTriggers(Trigger.AssetPMs, pmRequest.completed_on, pmRequest.completed_at_meter_hours);
                                        if (PMTriggers != null)
                                        {
                                            result = await _UoW.PMTriggersRepository.Insert(PMTriggers);
                                            if (result > 0)
                                            {
                                                _UoW.SaveChanges();
                                                _dbtransaction.Commit();
                                                result = (int)ResponseStatusNumber.Success;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _UoW.SaveChanges();
                                        _dbtransaction.Commit();
                                        result = (int)ResponseStatusNumber.Success;
                                    }
                                }
                                else
                                {
                                    _dbtransaction.Rollback();
                                }
                            }
                            else
                            {
                                _dbtransaction.Rollback();
                            }
                        }
                        else
                        {
                            result = (int)ResponseStatusNumber.NotFound;
                        }
                    }
                    else
                    {
                        _dbtransaction.Rollback();
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    _logger.LogError(e.Message);
                    result = (int)ResponseStatusNumber.Error;
                }
            }
            return result;
        }

        public async Task<PMTriggers> CreateRecurringTriggers(AssetPMs assetPM, DateTime completedOn, int completedAtMeterHours)
        {
            PMTriggers pMTrigger = new PMTriggers();
            try
            {
                if (assetPM.pm_trigger_by == (int)Status.Time)
                {
                    var currentDate = DateTime.UtcNow;
                    var dueDate = DateTime.UtcNow;
                    if (assetPM.datetime_repeat_time_period_type == (int)Status.Month)
                    {
                        dueDate = completedOn.AddMonths(assetPM.datetime_repeates_every.Value);
                    }
                    else if (assetPM.datetime_repeat_time_period_type == (int)Status.Year)
                    {
                        dueDate = completedOn.AddYears(assetPM.datetime_repeates_every.Value);
                    }


                    // case 1 currentDate = 25/8/2021 and DueDate = 30/8/2021 or DueDate = 25/8/2021
                    if (dueDate >= currentDate)
                    {
                        // check the Trigger Configuration
                        // check date diff in days
                        double diff2 = (dueDate - currentDate).TotalDays;
                        if (diff2 > GlobalConstants.DateDiffForDueStatus)
                        {
                            // set the status new
                            pMTrigger.status = (int)Status.TriggerNew;
                        }
                        else
                        {
                            // set the status Due
                            pMTrigger.meter_hours_when_pm_due = assetPM.Asset.meter_hours;
                            pMTrigger.datetime_when_pm_due = dueDate;
                            pMTrigger.status = (int)Status.Due;
                        }

                    }
                    // case 1 currentDate = 25/8/2021 and DueDate = 20/8/2021
                    else if (dueDate < currentDate)
                    {
                        // set the status is overdue
                        pMTrigger.meter_hours_when_pm_due = assetPM.Asset.meter_hours;
                        pMTrigger.datetime_when_pm_due = dueDate;
                        pMTrigger.status = (int)Status.OverDue;
                    }
                    pMTrigger.asset_pm_id = assetPM.asset_pm_id;
                    pMTrigger.asset_id = assetPM.asset_id;
                    pMTrigger.due_datetime = dueDate;
                    pMTrigger.created_at = DateTime.UtcNow;
                    pMTrigger.asset_pm_status = (int)Status.TriggerCompleted;
                    List<PMTriggersTasks> pmTriggersTasks = new List<PMTriggersTasks>();
                    var pmTasks = assetPM.AssetPMTasks.ToList().Where(x => !x.is_archive).ToList();
                    if (pmTasks?.Count > 0)
                    {
                        foreach (var x in pmTasks)
                        {
                            var taskdetails = await _UoW.TaskRepository.GetTaskById(x.task_id);
                            var hours = taskdetails.task_est_hours;
                            var minutes = taskdetails.task_est_minutes;
                            var HoursMinutes = PMTriggersUtil.ConvertHoursToMinutes(hours);
                            HoursMinutes += minutes;
                            pMTrigger.total_est_time_hours = PMTriggersUtil.ConvertMinutesToHours(HoursMinutes);
                            pMTrigger.total_est_time_minutes = PMTriggersUtil.ConvertMinutesToHoursOfMinutes(HoursMinutes);
                            PMTriggersTasks pmTriggersTask = new PMTriggersTasks();
                            pmTriggersTask.asset_id = pMTrigger.asset_id;
                            pmTriggersTask.asset_pm_id = pMTrigger.asset_pm_id;
                            pmTriggersTask.asset_pm_task_id = x.asset_pm_task_id;
                            pmTriggersTask.task_id = x.task_id;
                            pmTriggersTask.status = (int)Status.TriggetTaskNew;
                            pmTriggersTask.created_at = DateTime.UtcNow;
                            pmTriggersTask.modified_at = DateTime.UtcNow;
                            pmTriggersTask.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            pmTriggersTask.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            pmTriggersTasks.Add(pmTriggersTask);
                        }
                        pMTrigger.PMTriggersTasks = pmTriggersTasks;
                    }
                }
                else if (assetPM.pm_trigger_by == (int)Status.MeterHours)
                {
                    var dueMeterHours = assetPM.meter_hours_starting_at.Value;
                    // get current Asset Meter Hours
                    dueMeterHours = completedAtMeterHours + assetPM.meter_hours_repeates_every.Value;

                    // case 1 current Meter hours(200) < due meter hours(500)
                    if (dueMeterHours >= assetPM.Asset.meter_hours)
                    {
                        // set the status new
                        pMTrigger.status = (int)Status.TriggerNew;
                    }
                    // case 2 current Meter hours = 500 and DueDate = 200
                    else if (dueMeterHours < assetPM.Asset.meter_hours)
                    {
                        // set the status is overdue
                        pMTrigger.meter_hours_when_pm_due = dueMeterHours;
                        pMTrigger.datetime_when_pm_due = DateTime.UtcNow;
                        pMTrigger.status = (int)Status.OverDue;
                    }
                    pMTrigger.asset_pm_id = assetPM.asset_pm_id;
                    pMTrigger.asset_id = assetPM.asset_id;
                    pMTrigger.due_meter_hours = dueMeterHours;
                    pMTrigger.created_at = DateTime.UtcNow;
                    pMTrigger.asset_pm_status = (int)Status.TriggerNew;
                    List<PMTriggersTasks> pmTriggersTasks = new List<PMTriggersTasks>();
                    var pmTasks = assetPM.AssetPMTasks.ToList().Where(x => !x.is_archive).ToList();
                    if (pmTasks?.Count > 0)
                    {
                        foreach (var x in pmTasks)
                        {
                            var taskdetails = await _UoW.TaskRepository.GetTaskById(x.task_id);
                            var hours = taskdetails.task_est_hours;
                            var minutes = taskdetails.task_est_minutes;
                            var HoursMinutes = PMTriggersUtil.ConvertHoursToMinutes(hours);
                            HoursMinutes += minutes;
                            pMTrigger.total_est_time_hours = PMTriggersUtil.ConvertMinutesToHours(HoursMinutes);
                            pMTrigger.total_est_time_minutes = PMTriggersUtil.ConvertMinutesToHoursOfMinutes(HoursMinutes);
                            PMTriggersTasks pmTriggersTask = new PMTriggersTasks();
                            pmTriggersTask.asset_id = pMTrigger.asset_id;
                            pmTriggersTask.asset_pm_id = pMTrigger.asset_pm_id;
                            pmTriggersTask.asset_pm_task_id = x.asset_pm_task_id;
                            pmTriggersTask.task_id = x.task_id;
                            pmTriggersTask.status = (int)Status.TriggetTaskNew;
                            pmTriggersTask.created_at = DateTime.UtcNow;
                            pmTriggersTask.modified_at = DateTime.UtcNow;
                            pmTriggersTask.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            pmTriggersTask.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            pmTriggersTasks.Add(pmTriggersTask);
                        }
                        pMTrigger.PMTriggersTasks = pmTriggersTasks;
                    }
                }
                else if (assetPM.pm_trigger_by == (int)Status.TimeMeterHours)
                {
                    var dueMeterHours = assetPM.meter_hours_starting_at.Value;
                    // get current Asset Meter Hours
                    var currentDate = DateTime.UtcNow;
                    var dueDate = completedOn;
                    if (assetPM.datetime_repeat_time_period_type == (int)Status.Month)
                    {
                        dueDate = completedOn.AddMonths(assetPM.datetime_repeates_every.Value);
                    }
                    else if (assetPM.datetime_repeat_time_period_type == (int)Status.Year)
                    {
                        dueDate = completedOn.AddYears(assetPM.datetime_repeates_every.Value);
                    }

                    // case 1 currentDate = 25/8/2021 and DueDate = 30/8/2021 or DueDate = 25/8/2021
                    if (dueDate >= currentDate)
                    {
                        // check the Trigger Configuration
                        // check date diff in days
                        double diff2 = (dueDate - currentDate).TotalDays;
                        if (diff2 > GlobalConstants.DateDiffForDueStatus)
                        {
                            // set the status new
                            pMTrigger.status = (int)Status.TriggerNew;
                        }
                        else
                        {
                            // set the status Due
                            pMTrigger.meter_hours_when_pm_due = assetPM.Asset.meter_hours;
                            pMTrigger.datetime_when_pm_due = dueDate;
                            pMTrigger.status = (int)Status.Due;
                        }
                    }
                    // case 1 currentDate = 25/8/2021 and DueDate = 20/8/2021
                    else if (dueDate < currentDate)
                    {
                        // set the status is overdue
                        pMTrigger.meter_hours_when_pm_due = assetPM.Asset.meter_hours;
                        pMTrigger.datetime_when_pm_due = dueDate;
                        pMTrigger.status = (int)Status.OverDue;
                    }

                    // get current Asset Meter Hours
                    dueMeterHours = completedAtMeterHours + assetPM.meter_hours_repeates_every.Value;

                    if (pMTrigger.status == (int)Status.TriggerNew)
                    {
                        // case 1 current Meter hours(200) < due meter hours(500)
                        if (dueMeterHours >= assetPM.Asset.meter_hours)
                        {
                            // set the status new
                            pMTrigger.status = (int)Status.TriggerNew;
                        }
                        // case 2 current Meter hours = 500 and DueDate = 200
                        else if (dueMeterHours < assetPM.Asset.meter_hours)
                        {
                            // set the status is overdue
                            pMTrigger.meter_hours_when_pm_due = dueMeterHours;
                            pMTrigger.datetime_when_pm_due = DateTime.UtcNow;
                            pMTrigger.status = (int)Status.OverDue;
                        }
                    }

                    pMTrigger.asset_pm_id = assetPM.asset_pm_id;
                    pMTrigger.asset_id = assetPM.asset_id;
                    pMTrigger.due_meter_hours = dueMeterHours;
                    pMTrigger.due_datetime = dueDate;
                    pMTrigger.created_at = DateTime.UtcNow;
                    pMTrigger.asset_pm_status = (int)Status.TriggerNew;
                    List<PMTriggersTasks> pmTriggersTasks = new List<PMTriggersTasks>();
                    var pmTasks = assetPM.AssetPMTasks.ToList().Where(x => !x.is_archive).ToList();
                    if (pmTasks?.Count > 0)
                    {
                        foreach (var x in pmTasks)
                        {
                            var taskdetails = await _UoW.TaskRepository.GetTaskById(x.task_id);
                            var hours = taskdetails.task_est_hours;
                            var minutes = taskdetails.task_est_minutes;
                            var HoursMinutes = PMTriggersUtil.ConvertHoursToMinutes(hours);
                            HoursMinutes += minutes;
                            pMTrigger.total_est_time_hours = PMTriggersUtil.ConvertMinutesToHours(HoursMinutes);
                            pMTrigger.total_est_time_minutes = PMTriggersUtil.ConvertMinutesToHoursOfMinutes(HoursMinutes);
                            PMTriggersTasks pmTriggersTask = new PMTriggersTasks();
                            pmTriggersTask.asset_id = pMTrigger.asset_id;
                            pmTriggersTask.asset_pm_id = pMTrigger.asset_pm_id;
                            pmTriggersTask.asset_pm_task_id = x.asset_pm_task_id;
                            pmTriggersTask.task_id = x.task_id;
                            pmTriggersTask.status = (int)Status.TriggetTaskNew;
                            pmTriggersTask.created_at = DateTime.UtcNow;
                            pmTriggersTask.modified_at = DateTime.UtcNow;
                            pmTriggersTask.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            pmTriggersTask.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            pmTriggersTasks.Add(pmTriggersTask);
                        }
                        pMTrigger.PMTriggersTasks = pmTriggersTasks;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Create PM trigger error " + e.Message);
                throw e;
            }
            return pMTrigger;
        }

        public async Task<int> UpdateTriggerStatus()
        {
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    // get all triggers
                    var triggers = await _UoW.PMTriggersRepository.GetAllTriggers();
                    if (triggers?.Count > 0)
                    {
                        foreach (var trigger in triggers)
                        {
                            var oldTriggerStatus = trigger.status;
                            if (trigger.AssetPMs.pm_trigger_by == (int)Status.Time)
                            {
                                var currentDate = DateTime.UtcNow;
                                var dueDate = trigger.due_datetime.Value;

                                // case 1 currentDate = 25/8/2021 and DueDate = 30/8/2021 or DueDate = 25/8/2021
                                if (dueDate >= currentDate)
                                {
                                    // check the Trigger Configuration
                                    // check date diff in days
                                    double diff2 = (dueDate - currentDate).TotalDays;
                                    if (diff2 > GlobalConstants.DateDiffForDueStatus)
                                    {
                                        // set the status new
                                        continue;
                                    }
                                    else
                                    {
                                        if (trigger.status != (int)Status.PMWaiting && trigger.status != (int)Status.PMInProgress
                                            && trigger.status != (int)Status.Due)
                                        {
                                            // set the status Due
                                            trigger.datetime_when_pm_due = DateTime.UtcNow;
                                            trigger.meter_hours_when_pm_due = trigger.AssetPMs.Asset.meter_hours;
                                            trigger.status = (int)Status.Due;
                                        }
                                    }

                                }
                                // case 1 currentDate = 25/8/2021 and DueDate = 20/8/2021
                                else if (dueDate < currentDate)
                                {
                                    if (trigger.status != (int)Status.PMWaiting && trigger.status != (int)Status.PMInProgress)
                                    {
                                        // set the status is overdue
                                        trigger.status = (int)Status.OverDue;
                                    }
                                }
                            }
                            else if (trigger.AssetPMs.pm_trigger_by == (int)Status.MeterHours)
                            {
                                var currentMeterHours = trigger.AssetPMs.Asset.meter_hours;
                                var dueMeterHours = trigger.due_meter_hours.Value;

                                // case 1 current Meter hours(200) < due meter hours(500)
                                if (dueMeterHours >= currentMeterHours)
                                {
                                    // set the status new
                                    continue;
                                }
                                // case 2 current Meter hours = 500 and DueDate = 200
                                else if (dueMeterHours < currentMeterHours)
                                {
                                    if (trigger.status != (int)Status.PMWaiting && trigger.status != (int)Status.PMInProgress
                                        && trigger.status != (int)Status.OverDue)
                                    {
                                        // set the status is overdue
                                        trigger.datetime_when_pm_due = DateTime.UtcNow;
                                        trigger.meter_hours_when_pm_due = dueMeterHours;
                                        trigger.status = (int)Status.OverDue;
                                    }
                                }
                            }
                            else if (trigger.AssetPMs.pm_trigger_by == (int)Status.TimeMeterHours)
                            {
                                var currentMeterHours = trigger.AssetPMs.Asset.meter_hours;
                                var dueMeterHours = trigger.due_meter_hours.Value;
                                // get current Asset Meter Hours
                                var currentDate = DateTime.UtcNow;
                                var dueDate = trigger.due_datetime.Value;

                                // case 1 currentDate = 25/8/2021 and DueDate = 30/8/2021 or DueDate = 25/8/2021
                                if (dueDate >= currentDate)
                                {
                                    // check the Trigger Configuration
                                    // check date diff in days
                                    double diff2 = (dueDate - currentDate).TotalDays;
                                    if (diff2 > GlobalConstants.DateDiffForDueStatus)
                                    {
                                        if (trigger.status != (int)Status.PMWaiting && trigger.status != (int)Status.PMInProgress)
                                        {
                                            // set the status new
                                            trigger.status = (int)Status.TriggerNew;
                                        }
                                    }
                                    else
                                    {
                                        if (trigger.status != (int)Status.PMWaiting && trigger.status != (int)Status.PMInProgress
                                            && trigger.status != (int)Status.Due)
                                        {
                                            // set the status Due
                                            trigger.datetime_when_pm_due = DateTime.UtcNow;
                                            trigger.meter_hours_when_pm_due = trigger.AssetPMs.Asset.meter_hours;
                                            trigger.status = (int)Status.Due;
                                        }
                                    }
                                }
                                // case 1 currentDate = 25/8/2021 and DueDate = 20/8/2021
                                else if (dueDate < currentDate)
                                {
                                    if (trigger.status != (int)Status.PMWaiting && trigger.status != (int)Status.PMInProgress)
                                    {
                                        // set the status is overdue
                                        trigger.status = (int)Status.OverDue;
                                    }
                                }

                                if (trigger.status == (int)Status.TriggerNew || trigger.status == (int)Status.Due)
                                {
                                    // case 1 current Meter hours(200) < due meter hours(500)
                                    if (dueMeterHours >= trigger.AssetPMs.Asset.meter_hours)
                                    {
                                        if (trigger.status != (int)Status.Due)
                                        {
                                            // set the status new
                                            continue;
                                        }
                                    }
                                    // case 2 current Meter hours = 500 and DueDate = 200
                                    else if (dueMeterHours < trigger.AssetPMs.Asset.meter_hours)
                                    {
                                        if (trigger.status != (int)Status.PMWaiting && trigger.status != (int)Status.PMInProgress
                                            && trigger.status != (int)Status.OverDue)
                                        {
                                            // set the status is overdue
                                            trigger.datetime_when_pm_due = DateTime.UtcNow;
                                            trigger.meter_hours_when_pm_due = dueMeterHours;
                                            trigger.status = (int)Status.OverDue;
                                        }
                                    }
                                }
                            }

                            trigger.modified_at = DateTime.UtcNow;
                            if (oldTriggerStatus != trigger.status)
                            {
                                result = await _UoW.PMTriggersRepository.Update(trigger);
                                if (result > 0)
                                {
                                    if (trigger.status == (int)Status.Due)
                                    {
                                        var activityLogs = NotificationGenerator.ActivityPMDue(trigger.Asset.name, trigger.AssetPMs.title, trigger.Asset.meter_hours.ToString());
                                        activityLogs.asset_id = trigger.asset_id;
                                        activityLogs.created_at = DateTime.UtcNow;
                                        activityLogs.ref_id = trigger.pm_trigger_id.ToString();
                                        activityLogs.site_id = trigger.Asset.site_id;
                                        var response = await _UoW.BaseGenericRepository<AssetActivityLogs>().Update(activityLogs);
                                        result = response == true ? (int)ResponseStatusNumber.Success : result;
                                    }
                                    if (result > 0) _UoW.SaveChanges();
                                }
                                else
                                {
                                    _dbtransaction.Rollback();
                                }
                            }
                        }

                        if (result > 0) _dbtransaction.Commit();
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    _logger.LogError(e.Message);
                    result = (int)ResponseStatusNumber.Error;
                }
            }
            return result;
        }

        public async Task<int> UpdateTaskStatus(PMTriggerTaskRequestModel requestModel)
        {
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    // get all triggers
                    var tasks = await _UoW.PMTriggersTasksRepository.GetTaskByID(requestModel.trigger_task_id);
                    if (tasks != null)
                    {
                        tasks.modified_at = DateTime.UtcNow;
                        if (requestModel.status == (int)Status.TriggetTaskCompleted || requestModel.status == (int)Status.TriggetTaskInProgress || requestModel.status == (int)Status.TriggetTaskNew || requestModel.status == (int)Status.TriggetTaskWaiting)
                        {
                            tasks.status = requestModel.status;
                            result = await _UoW.PMTriggersTasksRepository.Update(tasks);
                            if (result > 0)
                            {
                                _UoW.SaveChanges();
                                _dbtransaction.Commit();
                                result = (int)ResponseStatusNumber.Success;
                            }
                            else
                            {
                                _dbtransaction.Rollback();
                            }
                        }
                        else
                        {
                            _dbtransaction.Rollback();
                            result = (int)ResponseStatusNumber.NotFound;
                        }
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    _logger.LogError(e.Message);
                    result = (int)ResponseStatusNumber.Error;
                }
            }
            return result;
        }

        public async Task<ListViewModel<DashboardPendingPMItems>> DashboardPendingPMItems(DashboardPendingPMItemsRequestModel requestModel)
        {
            ListViewModel<DashboardPendingPMItems> PMList = new ListViewModel<DashboardPendingPMItems>();
            try
            {
                // get all pending items by site
                var pmItems = await _UoW.AssetPMsRepository.GetPendingPMItems(requestModel);
                if (pmItems?.list?.Count > 0)
                {
                    PMList.list = _mapper.Map<List<DashboardPendingPMItems>>(pmItems.list);
                    PMList.listsize = pmItems.listsize;
                    PMList.pageSize = pmItems.pageSize;
                    PMList.pageIndex = pmItems.pageIndex;
                    PMList.result = (int)ResponseStatusNumber.Success;
                }
                else
                {
                    PMList.result = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                PMList.result = (int)ResponseStatusNumber.Error;
            }
            return PMList;
        }

        public async Task<ListViewModel<AssetListResponseModel>> FilterPendingPMItemsAssetIds(FilterPendingPMItemsOptionsRequestModel requestModel)
        {
            ListViewModel<AssetListResponseModel> filterResponse = new ListViewModel<AssetListResponseModel>();
            try
            {
                var response = await _UoW.AssetPMsRepository.FilterPendingPMItemsAssetIds(requestModel);
                if (response?.list?.Count > 0)
                {
                    filterResponse.list = _mapper.Map<List<AssetListResponseModel>>(response.list);
                    filterResponse.listsize = response.listsize;
                    filterResponse.pageIndex = response.pageIndex;
                    filterResponse.pageSize = response.pageSize;
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return filterResponse;
        }

        public async Task<ListViewModel<PMPlansResponseModel>> FilterPendingPMItemsPMPlans(FilterPendingPMItemsOptionsRequestModel requestModel)
        {
            ListViewModel<PMPlansResponseModel> filterResponse = new ListViewModel<PMPlansResponseModel>();
            try
            {
                var response = await _UoW.AssetPMsRepository.FilterPendingPMItemsPMPlans(requestModel);
                if (response?.list?.Count > 0)
                {
                    filterResponse.list = _mapper.Map<List<PMPlansResponseModel>>(response.list);
                    filterResponse.listsize = response.listsize;
                    filterResponse.pageIndex = response.pageIndex;
                    filterResponse.pageSize = response.pageSize;
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return filterResponse;
        }

        public async Task<ListViewModel<AssetPMResponseModel>> FilterPendingPMItemsPMItems(FilterPendingPMItemsOptionsRequestModel requestModel)
        {
            ListViewModel<AssetPMResponseModel> filterResponse = new ListViewModel<AssetPMResponseModel>();
            try
            {
                var response = await _UoW.AssetPMsRepository.FilterPendingPMItemsPMItems(requestModel);
                if (response?.list?.Count > 0)
                {
                    filterResponse.list = _mapper.Map<List<AssetPMResponseModel>>(response.list);
                    filterResponse.listsize = response.listsize;
                    filterResponse.pageIndex = response.pageIndex;
                    filterResponse.pageSize = response.pageSize;
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return filterResponse;
        }

        public async Task<ListViewModel<SitesViewModel>> FilterPendingPMItemsSites(FilterPendingPMItemsOptionsRequestModel requestModel)
        {
            ListViewModel<SitesViewModel> filterResponse = new ListViewModel<SitesViewModel>();
            try
            {
                var response = await _UoW.AssetPMsRepository.FilterPendingPMItemsSites(requestModel);
                if (response?.list?.Count > 0)
                {
                    filterResponse.list = _mapper.Map<List<SitesViewModel>>(response.list);
                    filterResponse.listsize = response.listsize;
                    filterResponse.pageIndex = response.pageIndex;
                    filterResponse.pageSize = response.pageSize;
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return filterResponse;
        }

        public async Task<List<UpComingPMs>> UpComingPMItems(int filter_type)
        {
            List<UpComingPMs> upComingPMs = new List<UpComingPMs>();
            try
            {
                // get all pending items by site
                var pmItems = await _UoW.AssetPMsRepository.GetUpComingPMs();
                if (pmItems?.Count > 0)
                {
                    if (filter_type == (int)PMDashboardFilterType.Weekly)
                    {
                        var startDate = DateTime.UtcNow.Date; // 10/13/2021
                        var lastWeekDate = DateTime.UtcNow.Date.AddDays(7); // 10/20/2021
                        List<DateTime> listWeek = Enumerable.Range(0, 1 + lastWeekDate.Subtract(startDate).Days).Select(offset => startDate.AddDays(offset)).ToList();
                        foreach (var date in listWeek)
                        {
                            UpComingPMs upComingPM = new UpComingPMs();
                            upComingPM.date = date;
                            upComingPM.pmCount = pmItems.Where(x => x.due_datetime.Value.Date == date).Count();
                            upComingPMs.Add(upComingPM);
                        }
                    }
                    else if (filter_type == (int)PMDashboardFilterType.Monthly)
                    {
                        var startDate = DateTime.UtcNow.Date; // 10/13/2021
                        var lastWeekDate = DateTime.UtcNow.Date.AddMonths(1); // 11/13/2021
                        List<DateTime> listWeek = Enumerable.Range(0, 1 + lastWeekDate.Subtract(startDate).Days).Select(offset => startDate.AddDays(offset)).ToList();
                        foreach (var date in listWeek)
                        {
                            UpComingPMs upComingPM = new UpComingPMs();
                            upComingPM.date = date;
                            upComingPM.pmCount = pmItems.Where(x => x.due_datetime.Value.Date == date).Count();
                            upComingPMs.Add(upComingPM);
                        }
                    }
                    else if (filter_type == (int)PMDashboardFilterType.Quarterly)
                    {
                        var startDate = DateTime.UtcNow.Date; // 10/13/2021
                        var lastWeekDate = DateTime.UtcNow.Date.AddMonths(3); // 1/13/2022
                        List<DateTime> listWeek = Enumerable.Range(0, 1 + lastWeekDate.Subtract(startDate).Days).Select(offset => startDate.AddDays(offset)).ToList();
                        foreach (var date in listWeek)
                        {
                            UpComingPMs upComingPM = new UpComingPMs();
                            upComingPM.date = date;
                            upComingPM.pmCount = pmItems.Where(x => x.due_datetime.Value.Date == date).Count();
                            upComingPMs.Add(upComingPM);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("UpComing PM Items Error " + e.Message);
                throw e;
            }
            return upComingPMs;
        }

        public async Task<UpComingPMs> UpComingPMItemsWeekly()
        {
            UpComingPMs upComingPMs = new UpComingPMs();
            try
            {
                // get all pending items by site
                var pmItems = await _UoW.AssetPMsRepository.GetUpComingPMs();
                if (pmItems?.Count > 0)
                {
                    var startDate = DateTime.UtcNow.Date; // 10/13/2021
                    for (int i = 0; i < 5; i++)
                    {
                        var lastWeekDate = startDate.AddDays(7); // 10/20/2021
                        UpcomingPMsWeekly upComingPM = new UpcomingPMsWeekly();
                        upComingPM.start_date = startDate;
                        upComingPM.end_date = lastWeekDate;
                        upComingPM.pmCount = pmItems.Where(x =>
                        {
                            var due_datetime = x.due_datetime.Value.Date;
                            return due_datetime >= startDate && due_datetime < lastWeekDate;
                        }).Count();
                        upComingPMs.upcomingPMs.Add(upComingPM);
                        startDate = lastWeekDate;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("UpComing PM Items Error " + e.Message);
                throw e;
            }
            return upComingPMs;
        }

        public async Task SendExecutivePMDueReport(List<ExecutivePMDueResponseModel> users)
        {
            try
            {
                foreach (var x in users)
                {
                    string subject = "PM - Due Report";
                    if (!String.IsNullOrEmpty(x.email))
                    {
                        var FileStream = ExcelCreation.WriteExcelFile(x.pmItems);
                        var templateID = ConfigurationManager.AppSettings["Executive_PMDueReport_Template_ID"];
                        var response = await SendEmail.SendGridEmailWithTemplate(x.email, subject, x, templateID, FileStream, "PMDueReport.xlsx");
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

        public async Task SendOverDuePMReportToExecutive()
        {
            List<ExecutivePMDueResponseModel> AllExecutiveReport = new List<ExecutivePMDueResponseModel>();
            try
            {

                List<User> executives = _UoW.UserRepository.GetAllExecutiveForPMDueReport();
                foreach (var executive in executives)
                {
                    ExecutivePMDueResponseModel executiveDaily = new ExecutivePMDueResponseModel();
                    executiveDaily.firstname = executive.firstname;
                    executiveDaily.lastname = executive.lastname;
                    executiveDaily.username = executive.username;
                    executiveDaily.email = executive.email;
                    executiveDaily.user_id = executive.uuid;
                    List<PMDueReportEmailResponse> pmItem = new List<PMDueReportEmailResponse>();
                    List<ExecutiveSiteWiseReport> sitePMReport = new List<ExecutiveSiteWiseReport>();
                    if (executive.Usersites?.Count > 0)
                    {
                        var usersites = executive.Usersites?.Where(x => x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (usersites?.Count > 0)
                        {
                            var allPMItems = await _UoW.AssetPMsRepository.GetDueAssetPMs();
                            if (allPMItems?.Count > 0)
                            {
                                var SitePMs = allPMItems.Where(x => usersites.Contains(x.Asset.Sites.site_id)).ToList();
                                if (SitePMs?.Count > 0)
                                {
                                    pmItem = _mapper.Map<List<PMDueReportEmailResponse>>(SitePMs);
                                    foreach (var pm in pmItem)
                                    {
                                        if (pm.service_dealer_id != Guid.Empty)
                                        {
                                            pm.sent_first_notifcation = await _UoW.PMNotificationRepository.GetFirstSentNotification(pm.service_dealer_id, pm.pm_trigger_id);
                                            pm.sent_second_notifcation = await _UoW.PMNotificationRepository.GetSecondSentNotification(pm.service_dealer_id, pm.pm_trigger_id);
                                            pm.sent_pm_due_notifcation = await _UoW.PMNotificationRepository.GetPMDueSentNotification(pm.service_dealer_id, pm.pm_trigger_id);
                                        }
                                    }
                                    var result = SitePMs.GroupBy(x => x.Asset.site_id).ToList();
                                    foreach (var res in result)
                                    {
                                        ExecutiveSiteWiseReport siteDetail = new ExecutiveSiteWiseReport();
                                        siteDetail.site_name = res.FirstOrDefault().Asset.Sites.site_name;
                                        siteDetail.total_pm_items_overdue = res.Count();
                                        siteDetail.time_elapsed = DateTimeUtil.GetBeforetimeText(res.Where(x => x.due_datetime != null).OrderBy(x => x.due_datetime).Select(x => x.due_datetime.Value).FirstOrDefault());
                                        sitePMReport.Add(siteDetail);
                                    }
                                }
                            }
                        }
                    }
                    executiveDaily.pmItems = pmItem;
                    executiveDaily.siteWiseReport = sitePMReport;
                    AllExecutiveReport.Add(executiveDaily);
                }
                SendExecutivePMDueReport(AllExecutiveReport);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<DashboardPMMetricsResponseModel> DashboardPMMetrics()
        {
            try
            {
                // get all pending items by site
                return await _UoW.AssetPMsRepository.DashboardPMMetrics();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw e;
            }
        }

        public async Task<ListViewModel<ServiceDealerViewModel>> GetAllServiceDealers(string searchstring, int pageindex, int pagesize)
        {
            ListViewModel<ServiceDealerViewModel> servicerDealers = new ListViewModel<ServiceDealerViewModel>();
            try
            {
                // get all pending items by site
                var response = await _UoW.AssetPMsRepository.GetAllServiceDealers(searchstring);
                if (response?.Count > 0)
                {
                    if (pageindex > 0 && pagesize > 0)
                    {
                        servicerDealers.listsize = response.Count();
                        response = response.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    }
                    servicerDealers.list = _mapper.Map<List<ServiceDealerViewModel>>(response);
                    servicerDealers.listsize = response.Count;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw e;
            }
            return servicerDealers;
        }

        public async Task<ListViewModel<AssetMeterHourHistoryResponseModel>> GetAssetMeterHourHistory(AssetMeterHourHistoryRequestModel requestModel)
        {
            ListViewModel<AssetMeterHourHistoryResponseModel> trxnHistoryResponse = new ListViewModel<AssetMeterHourHistoryResponseModel>();
            try
            {
                var historyDetails = await _UoW.AssetPMsRepository.GetAssetMeterHourHistory(requestModel);
                if (historyDetails?.Count > 0)
                {
                    int totalAssetHistory = historyDetails.Count;
                    if (requestModel.pageindex > 0 && requestModel.pagesize > 0)
                    {
                        historyDetails = historyDetails.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                    }
                    trxnHistoryResponse.list = _mapper.Map<List<AssetMeterHourHistoryResponseModel>>(historyDetails);
                    trxnHistoryResponse.pageSize = requestModel.pagesize;
                    trxnHistoryResponse.pageIndex = requestModel.pageindex;
                    trxnHistoryResponse.listsize = totalAssetHistory;
                }
                else
                {
                    trxnHistoryResponse.result = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw e;
            }
            return trxnHistoryResponse;
        }

        public List<GetPMPlansByClassIdResponsemodel> GetPMPlansByClassId(Guid inspectiontemplate_asset_class_id)
        {
            List<GetPMPlansByClassIdResponsemodel> response = new List<GetPMPlansByClassIdResponsemodel>();

            var db_get_pm_plans = _UoW.AssetPMsRepository.GetPMPlansByClassId(inspectiontemplate_asset_class_id);
            if (UpdatedGenericRequestmodel.CurrentUser.role_id != GlobalConstants.SuperAdmin_Role_id)
            {
                db_get_pm_plans = db_get_pm_plans.Where(x => x.plan_name != "70B-STANDARD").ToList();
            }

            response = _mapper.Map<List<GetPMPlansByClassIdResponsemodel>>(db_get_pm_plans);

            return response;
        }
        public int LinkPMToWOLine(LinkPMToWOLineRequestmodel requestmodel)
        {
            int response = (int)ResponseStatusNumber.Error;
            try
            {
                var get_asset_pms = _UoW.AssetPMsRepository.GetAssetPMbyIDs(requestmodel.asset_pm_id);
                var get_woLine_status = _UoW.AssetPMsRepository.GetWOLineStatusByAssetFormId(requestmodel.asset_form_id);

                var updatedPMStatus = (int)Status.open;

                if (get_woLine_status == (int)Status.open) //open
                {
                    updatedPMStatus = (int)Status.Schedule; //Schedule
                }
                else if (get_woLine_status == (int)Status.InProgress)//InProgress
                {
                    updatedPMStatus = (int)Status.InProgress;
                }
                foreach (var asset_pm in get_asset_pms)
                {
                    asset_pm.asset_form_id = requestmodel.asset_form_id;
                    asset_pm.wo_id = requestmodel.wo_id;
                    asset_pm.modified_at = DateTime.UtcNow;
                    asset_pm.is_Asset_PM_fixed = true;
                    if(asset_pm.status != (int)Status.Completed)
                    {
                        asset_pm.status = updatedPMStatus;
                    }
                    asset_pm.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    asset_pm.status = updatedPMStatus;
                }
                var update = _UoW.BaseGenericRepository<AssetPMs>().UpdateList(get_asset_pms);
                if (update)
                {
                    response = (int)ResponseStatusNumber.Success;
                }
            }
            catch (Exception ex)
            {
                response = (int)ResponseStatusNumber.Error;
            }

            return response;
        }

        public ListViewModel<GetAssetPMListResponsemodel> ReturnAllOverdueAssetPM(GetAssetPMListRequestmodel requestmodel)
        {
            ListViewModel<GetAssetPMListResponsemodel> response = new ListViewModel<GetAssetPMListResponsemodel>();
            List<GetAssetPMListResponsemodel> list = new List<GetAssetPMListResponsemodel>();

            var get_active_asset_pm = _UoW.AssetPMsRepository.ReturnAllOverdueAssetPM(requestmodel);
            foreach (var pm in get_active_asset_pm.Item1)
            {
                if (pm.datetime_starting_at != null)
                {
                    var max_condition = GetConditionTypeForPM(pm.Asset);
                    if (pm.AssetPMsTriggerConditionMapping != null)
                    {
                        var pm_condition_trigger = pm.AssetPMsTriggerConditionMapping.Where(x => x.condition_type_id == max_condition).FirstOrDefault();

                        if (pm_condition_trigger != null && pm_condition_trigger.datetime_repeates_every != null && pm_condition_trigger.datetime_repeat_time_period_type != null)
                        {
                            // get PM end date 
                            var PM_end_date = CalculatePMEndDate(pm.datetime_starting_at.Value, pm_condition_trigger.datetime_repeates_every.Value, pm_condition_trigger.datetime_repeat_time_period_type.Value);
                            if (PM_end_date < DateTime.UtcNow)   // over Due
                            {
                                GetAssetPMListResponsemodel GetAssetPMListResponsemodel = new GetAssetPMListResponsemodel();
                                GetAssetPMListResponsemodel.asset_pm_id = pm.asset_pm_id;
                                GetAssetPMListResponsemodel.asset_id = pm.asset_id;
                                GetAssetPMListResponsemodel.pm_id = pm.pm_id;
                                GetAssetPMListResponsemodel.title = pm.title;
                                GetAssetPMListResponsemodel.description = pm.description;
                                GetAssetPMListResponsemodel.asset_name = pm.Asset.name;
                                GetAssetPMListResponsemodel.asset_class_name = pm.Asset.InspectionTemplateAssetClass != null ? pm.Asset.InspectionTemplateAssetClass.asset_class_name : null;
                                GetAssetPMListResponsemodel.asset_class_type = pm.Asset.InspectionTemplateAssetClass != null ? pm.Asset.InspectionTemplateAssetClass.FormIOType.form_type_name : null;
                                GetAssetPMListResponsemodel.status = pm.status;
                                GetAssetPMListResponsemodel.status_name = pm.StatusMaster.status_name;
                                GetAssetPMListResponsemodel.asset_pm_plan_id = pm.AssetPMPlans.asset_pm_plan_id;
                                GetAssetPMListResponsemodel.asset_plan_name = pm.AssetPMPlans.plan_name;
                                GetAssetPMListResponsemodel.last_completed_date = pm.asset_pm_completed_date;
                                GetAssetPMListResponsemodel.is_assetpm_enabled = pm.is_assetpm_enabled;
                                list.Add(GetAssetPMListResponsemodel);
                            }
                        }
                    }
                }
            }
            
            response.list = list;

            return response;
        }

        public ListViewModel<GetAssetPMListResponsemodel> GetAssetPMList(GetAssetPMListRequestmodel requestmodel)
        {
            
            ListViewModel<GetAssetPMListResponsemodel> response = new ListViewModel<GetAssetPMListResponsemodel>();
            /*if(requestmodel.is_requested_for_overdue_pm)
            {
                var query = ReturnAllOverdueAssetPM(requestmodel);
            }*/

            string get_site_name = _UoW.AssetPMsRepository.GetSiteNameById(Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            
            var get_asset_pms = _UoW.AssetPMsRepository.GetAssetPMList(requestmodel);
            List<GetAssetPMListResponsemodel> list = new List<GetAssetPMListResponsemodel>();
            response.listsize = get_asset_pms.Item2;
            get_asset_pms.Item1.ForEach(asset_pm =>
            {
                GetAssetPMListResponsemodel GetAssetPMListResponsemodel = new GetAssetPMListResponsemodel();
                GetAssetPMListResponsemodel.asset_pm_id = asset_pm.asset_pm_id;
                GetAssetPMListResponsemodel.asset_id = asset_pm.asset_id;
                GetAssetPMListResponsemodel.pm_id = asset_pm.pm_id;
                GetAssetPMListResponsemodel.title = asset_pm.title;
                GetAssetPMListResponsemodel.description = asset_pm.description;
                GetAssetPMListResponsemodel.asset_name = asset_pm.Asset.name;
                GetAssetPMListResponsemodel.asset_class_name = asset_pm.Asset.InspectionTemplateAssetClass!=null? asset_pm.Asset.InspectionTemplateAssetClass.asset_class_name:null;
                GetAssetPMListResponsemodel.asset_class_code = asset_pm.Asset.InspectionTemplateAssetClass!=null? asset_pm.Asset.InspectionTemplateAssetClass.asset_class_code:null;
                GetAssetPMListResponsemodel.asset_class_type = asset_pm.Asset.InspectionTemplateAssetClass!=null? asset_pm.Asset.InspectionTemplateAssetClass.FormIOType.form_type_name:null;
                GetAssetPMListResponsemodel.status = asset_pm.status;
                GetAssetPMListResponsemodel.status_name = asset_pm.StatusMaster.status_name;
                GetAssetPMListResponsemodel.asset_pm_plan_id = asset_pm.AssetPMPlans.asset_pm_plan_id;
                GetAssetPMListResponsemodel.asset_plan_name = asset_pm.AssetPMPlans.plan_name;
                GetAssetPMListResponsemodel.last_completed_date = asset_pm.asset_pm_completed_date;
                GetAssetPMListResponsemodel.pm_starting_date = asset_pm.datetime_starting_at;
                GetAssetPMListResponsemodel.facility_name = get_site_name;
                GetAssetPMListResponsemodel.is_assetpm_enabled = asset_pm.is_assetpm_enabled;

                if(asset_pm.Asset.AssetFormIOBuildingMappings != null)
                {
                    GetAssetPMListResponsemodel.building = asset_pm.Asset.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name;
                    GetAssetPMListResponsemodel.floor = asset_pm.Asset.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name;
                    GetAssetPMListResponsemodel.room = asset_pm.Asset.AssetFormIOBuildingMappings.FormIORooms.formio_room_name;
                    if(asset_pm.Asset.AssetFormIOBuildingMappings.FormIOSections!=null)
                        GetAssetPMListResponsemodel.section = asset_pm.Asset.AssetFormIOBuildingMappings.FormIOSections.formio_section_name;
                }
                GetAssetPMListResponsemodel.internal_asset_id = asset_pm.Asset.internal_asset_id;
                GetAssetPMListResponsemodel.manual_wo_number = asset_pm.WorkOrders != null ? asset_pm.WorkOrders.manual_wo_number:null;
                GetAssetPMListResponsemodel.criticality_index_type = asset_pm.Asset.criticality_index_type;
                GetAssetPMListResponsemodel.asset_operating_condition_state = asset_pm.Asset.asset_operating_condition_state;
                
                GetAssetPMListResponsemodel.due_date = asset_pm.due_date;
                if (asset_pm.due_date != null)
                {
                    GetAssetPMListResponsemodel.due_in = DateTimeUtil.GetDueOverdueTimingByDueDate(asset_pm.due_date.Value).Item1;// asset_pm.pm_due_time_duration;
                    if (DateTime.UtcNow.Date == asset_pm.due_date.Value.Date)
                    {
                        GetAssetPMListResponsemodel.pm_due_overdue_flag = (int)pm_due_overdue_flag.PM_Due;
                    }
                    else if (DateTime.UtcNow.Date > asset_pm.due_date.Value.Date)
                    {
                        GetAssetPMListResponsemodel.pm_due_overdue_flag = (int)pm_due_overdue_flag.PM_Overdue;
                    }
                    else
                    {
                        GetAssetPMListResponsemodel.pm_due_overdue_flag = (int)pm_due_overdue_flag.PM_OnTrack;
                    }
                }


                string class_code = asset_pm.Asset.InspectionTemplateAssetClass.asset_class_code;
                var get_pm_master_form = _UoW.WorkOrderRepository.GetPMMasterFormByAssetpm(class_code,asset_pm.title, GetAssetPMListResponsemodel.asset_plan_name);
                
                PMs get_pm = null;
                if(asset_pm.pm_id != null)
                {
                    get_pm = _UoW.WorkOrderRepository.GetPMById(asset_pm.pm_id.Value);

                    GetAssetPMListResponsemodel.is_current_assetpm = _UoW.AssetPMsRepository.IsThisCurrentAssetPM(asset_pm.asset_id, asset_pm.pm_id.Value, asset_pm.asset_pm_id);
                }

                if (asset_pm.Asset.component_level_type_id == (int)ComponentLevelTypes.SublevelComponent)
                {
                    if (asset_pm.Asset.AssetTopLevelcomponentMapping != null)
                    {
                        var toplevelcomponent_asset_id = asset_pm.Asset.AssetTopLevelcomponentMapping.Where(x=>!x.is_deleted).Select(x => x.toplevelcomponent_asset_id).FirstOrDefault();
                        var asset_name = _UoW.AssetRepository.GetAssetNameByAssetId(toplevelcomponent_asset_id);
                        GetAssetPMListResponsemodel.top_level_asset_name = asset_name;
                    }
                }

                if (get_pm_master_form != null)
                {
                    GetAssetPMListResponsemodel.form_name = get_pm_master_form.form_name ;
                
                }else if(get_pm != null && get_pm.pm_inspection_type_id == (int)PMInspectionTypeId.IRThermography)
                {
                    GetAssetPMListResponsemodel.form_name = "Energized";
                }

                if (asset_pm.datetime_starting_at != null)  // decide condition type 
                {   
                    var max_condition = GetConditionTypeForPM(asset_pm.Asset);
                    var pm_condition_trigger = asset_pm.AssetPMsTriggerConditionMapping.Where(x => x.condition_type_id == max_condition).FirstOrDefault();
                    if (pm_condition_trigger != null)
                    {
                        GetAssetPMListResponsemodel.frequency = pm_condition_trigger.datetime_repeates_every.ToString() + " " + pm_condition_trigger.PMDateTimeRepeatTypeStatus.status_name;
                    }
                }

                if(GetAssetPMListResponsemodel.status == (int)Status.Completed)
                {
                    GetAssetPMListResponsemodel.due_date = null;
                }

                list.Add(GetAssetPMListResponsemodel);
            });
           
            

            response.list = list;
            response.pageSize = requestmodel.pagesize;
            response.pageIndex = requestmodel.pageindex;

            return response;
        }


        public ListViewModel<GetAssetPMListOptimizedResponsemodel> GetAssetPMListOptimized(GetAssetPMListRequestmodel requestmodel)
        {
            ListViewModel<GetAssetPMListOptimizedResponsemodel> response = new ListViewModel<GetAssetPMListOptimizedResponsemodel>();
            try
            {
                var get_asset_pms = _UoW.AssetPMsRepository.GetAssetPMListOptimized(requestmodel);
                if (get_asset_pms.Item1 != null && get_asset_pms.Item1.Count>0)
                {
                    var mappedlist = _mapper.Map<List<GetAssetPMListOptimizedResponsemodel>>(get_asset_pms.Item1);
                    response.list = mappedlist;
                    response.listsize = get_asset_pms.Item2;
                    response.pageIndex = requestmodel.pageindex;
                    response.pageSize = requestmodel.pagesize;
                }
            }
            catch (Exception e)
            {
            }
            return response;
        }


        public async Task<int> MarkPMcompletedNewflow(MarkPMcompletedNewflowRequestmodel requestmodel)
        {
            var response = (int)ResponseStatusNumber.Error;
            var item = await _UoW.AssetPMsRepository.GetAssetPMByIdForUpdate(requestmodel.asset_pm_id);

            item.status = (int)Status.Completed;
            item.modified_at = DateTime.UtcNow;
            item.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
            //item.asset_pm_completed_date = DateTime.UtcNow;
            item.completed_notes = requestmodel.completed_notes;

            var update = await _UoW.BaseGenericRepository<AssetPMs>().Update(item);
            if (update)
            {
                _UoW.SaveChanges();
                response = (int)ResponseStatusNumber.Success;
            }
            /** In new Requirements We are not Creating New AssetPMs even if they are Reccuring
             * 
            // create new PM if schedular is reoccure
            if (item.pm_trigger_type == (int)Status.Recurring)
            {
                var get_master_pm = _UoW.WorkOrderRepository.GetPMById(item.pm_id.Value);
                get_master_pm.PMAttachments = get_master_pm.PMAttachments.Where(y => !y.is_archive).ToList();

                AssetPMs assetpm = new AssetPMs();
                assetpm = _mapper.Map<AssetPMs>(get_master_pm);
                assetpm.asset_id = item.asset_id;
                assetpm.created_at = DateTime.UtcNow;
                assetpm.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                assetpm.status = (int)Status.Active;
                assetpm.asset_pm_plan_id = item.asset_pm_plan_id;
                assetpm.datetime_starting_at = DateTime.UtcNow;
                assetpm.asset_pm_completed_date = DateTime.UtcNow;
                assetpm.AssetPMAttachments.ToList().ForEach(y =>
                {
                    y.asset_pm_plan_id = item.asset_pm_plan_id;
                    y.asset_pm_id = assetpm.asset_pm_id;
                    y.asset_id = item.asset_id;
                    y.created_at = DateTime.UtcNow;
                    y.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                });
                assetpm.AssetPMsTriggerConditionMapping.ToList().ForEach(y =>
                {
                    y.asset_pm_id = assetpm.asset_pm_id;
                    y.created_at = DateTime.UtcNow;
                    y.site_id = item.Asset.site_id;
                    y.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                });
                var insert_pm = await _UoW.BaseGenericRepository<AssetPMs>().Insert(assetpm);
                _UoW.SaveChanges();
                response = (int)ResponseStatusNumber.Success;
            }
            else // if PM reocure is fixed then dont do anything
            {
                response = (int)ResponseStatusNumber.Success;
            }
            */
            return response;
        }

        public int GetConditionTypeForPM(Asset asset)
        {
            int asset_criticality = asset.criticality_index_type != null ? asset.criticality_index_type.Value : 0;
            int asset_operating_condition = asset.condition_index_type != null ? asset.condition_index_type.Value : 0;

            if(asset_operating_condition == (int)condition_index_type.Good)
            {
                asset_operating_condition = 1;
            }
            else if(asset_operating_condition == (int)condition_index_type.Average)
            {
                asset_operating_condition = 2;
            }
            else if(asset_operating_condition == (int)condition_index_type.Poor_Corrosive || asset_operating_condition == (int)condition_index_type.Poor_Dusty)
            {
                asset_operating_condition = 3;
            }
            int asset_pec = 1;

            /*if (asset.AssetIssue != null)
            {
                var asset_issues = asset.AssetIssue.ToList();
                var non_compliance_issue_count = asset_issues.Where(x => x.issue_type == (int)WOLine_Temp_Issue_Type.Compliance && x.issue_status != (int)Status.Completed && !x.is_deleted).Count();
                if(non_compliance_issue_count == 1)
                {
                    asset_pec = 2;
                }
                else
                {
                    asset_pec = 3;
                }
            }*/
            int max = Math.Max(asset_criticality, Math.Max(asset_operating_condition, asset_pec));
            return max;
        }

        public DateTime CalculatePMEndDate(DateTime start_date, int datetime_repeates_every, int datetime_repeat_time_period_type)
        {
            if (datetime_repeat_time_period_type == (int)Status.Month)
            {
                return start_date.AddMonths(datetime_repeates_every);
            }
            else if (datetime_repeat_time_period_type == (int)Status.Year)
            {
                return start_date.AddYears(datetime_repeates_every);
            }
            return start_date;
        }

        public AssetPMCountResponsemodel AssetPMCount()
        {
            AssetPMCountResponsemodel response = new AssetPMCountResponsemodel();

            var get_active_asset_pm = _UoW.AssetPMsRepository.GetActiveAssetPM();
            var get_completed_asset_pm = _UoW.AssetPMsRepository.GetCompletedAssetPM();
            response.completed_asset_pm_count = get_completed_asset_pm;

            int due_count = 0;
            int over_due_count = 0;
            int open_count = 0;
            int in_progress_count = 0;
            int schedule_count = 0;

            get_active_asset_pm.ForEach(asset_pm =>
            {
                if (asset_pm.datetime_starting_at != null)
                {
                    // decide condition type 
                    var max_condition = GetConditionTypeForPM(asset_pm.Asset);
                    if (asset_pm.AssetPMsTriggerConditionMapping != null)
                    {
                        var pm_condition_trigger = asset_pm.AssetPMsTriggerConditionMapping.Where(x => x.condition_type_id == max_condition).FirstOrDefault();

                        if (pm_condition_trigger != null && pm_condition_trigger.datetime_repeates_every != null && pm_condition_trigger.datetime_repeat_time_period_type != null)
                        {
                            // get PM end date 
                            var PM_end_date = CalculatePMEndDate(asset_pm.datetime_starting_at.Value, pm_condition_trigger.datetime_repeates_every.Value, pm_condition_trigger.datetime_repeat_time_period_type.Value);
                            var PM_due_date = PM_end_date.AddDays(-15);

                            if (PM_end_date < DateTime.UtcNow) // over Due
                            {
                                over_due_count++;
                                if (asset_pm.status == (int)Status.Active)
                                {
                                    open_count++;
                                }
                                if (asset_pm.status == (int)Status.InProgress)
                                {
                                    in_progress_count++;
                                }
                                if (asset_pm.status == (int)Status.Schedule)
                                {
                                    schedule_count++;
                                }
                            }
                            else if (PM_due_date < DateTime.UtcNow) //  due
                            {
                                due_count++;
                                if (asset_pm.status == (int)Status.Active)
                                {
                                    open_count++;
                                }
                                if (asset_pm.status == (int)Status.InProgress)
                                {
                                    in_progress_count++;
                                }
                                if (asset_pm.status == (int)Status.Schedule)
                                {
                                    schedule_count++;
                                }
                            }
                            else // normal / on-time 
                            {
                                if (asset_pm.status == (int)Status.Active)
                                {
                                    open_count++;
                                }
                                if (asset_pm.status == (int)Status.InProgress)
                                {
                                    in_progress_count++;
                                }
                                if (asset_pm.status == (int)Status.Schedule)
                                {
                                    schedule_count++;
                                }
                            }
                        }
                        else
                        {
                            if (asset_pm.status == (int)Status.Active)
                            {
                                open_count++;
                            }
                            if (asset_pm.status == (int)Status.InProgress)
                            {
                                in_progress_count++;
                            }
                            if (asset_pm.status == (int)Status.Schedule)
                            {
                                schedule_count++;
                            }
                        }
                    }
                    else
                    {
                        if (asset_pm.status == (int)Status.Active)
                        {
                            open_count++;
                        }
                        if (asset_pm.status == (int)Status.InProgress)
                        {
                            in_progress_count++;
                        }
                        if (asset_pm.status == (int)Status.Schedule)
                        {
                            schedule_count++;
                        }
                    }
                }
                else
                {
                    if (asset_pm.status == (int)Status.Active)
                    {
                        open_count++;
                    }
                    if (asset_pm.status == (int)Status.InProgress)
                    {
                        in_progress_count++;
                    }
                    if (asset_pm.status == (int)Status.Schedule)
                    {
                        schedule_count++;
                    }
                }
            });
            response.due_asset_pm_count = due_count;
            response.overdue_asset_pm_count = over_due_count;
            response.open_asset_pm_count = open_count;
            response.scheduled_asset_pm_count= schedule_count;
            return response;
        }

        public async Task<int> assignpmtoassetcecco()
        {
            string json = "{\"ATSW\":[{\"AssetName\":\"N-ATS-E5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-SWBD-5A1\",\"Fed-ByAsset(2)\":\"N-SWBD-E5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-SWITCHES\"},{\"AssetName\":\"N-ATS-LR5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-SWBD-5A1\",\"Fed-ByAsset(2)\":\"N-SWBD-E5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-SWITCHES\"},{\"AssetName\":\"N-ATS-LR5A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-SWBD-5A2\",\"Fed-ByAsset(2)\":\"N-SWBD-E5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-SWITCHES\"},{\"AssetName\":\"N-ATS-SB5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-SWBD-5A1\",\"Fed-ByAsset(2)\":\"N-SWBD-E5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-SWITCHES\"},{\"AssetName\":\"N-ATS-SB5A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-SWBD-5A2\",\"Fed-ByAsset(2)\":\"N-SWBD-E5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-SWITCHES\"}],\"CITR\":[{\"AssetName\":\"CURRENTTRANSFORMER1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-MVS-BA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"MVROOMN0412\",\"AssetSection\":\"N-MVS-BA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"N\\/A\"},{\"AssetName\":\"CURRENTTRANSFORMER2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-MVS-BA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"MVROOMN0412\",\"AssetSection\":\"N-MVS-BA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"N\\/A\"},{\"AssetName\":\"CURRENTTRANSFORMER3\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-MVS-BA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"MVROOMN0412\",\"AssetSection\":\"N-MVS-BA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"N\\/A\"}],\"DISC-L-F\":[{\"AssetName\":\"N-DIS-FIRE-PUMP\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-MVS-BA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"MVROOMN0412\",\"AssetSection\":\"N-MVS-BA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-SWITCHES\",\"nameplateInformation.manufacturer\":\"S&C\",\"nameplateInformation.model\":\"Mini-Rupter\",\"nameplateInformation.voltageRating\":\"15kV\",\"nameplateInformation.ampereRating\":\"600A\"},{\"AssetName\":\"N-DIS-SWBD-5A2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-MVS-BA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"MVROOMN0412\",\"AssetSection\":\"N-MVS-BA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-SWITCHES\",\"nameplateInformation.manufacturer\":\"S&C\",\"nameplateInformation.model\":\"Mini-Rupter\",\"nameplateInformation.voltageRating\":\"15kV\",\"nameplateInformation.ampereRating\":\"600A\"},{\"AssetName\":\"N-DIS-SWBD-5A1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-MVS-BA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"MVROOMN0412\",\"AssetSection\":\"N-MVS-BA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-SWITCHES\",\"nameplateInformation.manufacturer\":\"S&C\",\"nameplateInformation.model\":\"Mini-Rupter\",\"nameplateInformation.voltageRating\":\"15kV\",\"nameplateInformation.ampereRating\":\"600A\"}],\"DISC-L\":[{\"AssetName\":\"MAINSWITCH1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-MVS-BA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"MVROOMN0412\",\"AssetSection\":\"N-MVS-BA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-SWITCHES\",\"nameplateInformation.manufacturer\":\"S&C\",\"nameplateInformation.model\":\"Mini-Rupter\",\"nameplateInformation.voltageRating\":\"15kV\",\"nameplateInformation.ampereRating\":\"600A\"},{\"AssetName\":\"MAINSWITCH2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-MVS-BA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"MVROOMN0412\",\"AssetSection\":\"N-MVS-BA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-SWITCHES\",\"nameplateInformation.manufacturer\":\"S&C\",\"nameplateInformation.model\":\"Mini-Rupter\",\"nameplateInformation.voltageRating\":\"15kV\",\"nameplateInformation.ampereRating\":\"600A\"}],\"DPNL\":[{\"AssetName\":\"N-DPEH5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-MTS-DPEH5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\"},{\"AssetName\":\"N-DPH5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-SWBD-5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":20,\"nameplateInformation.ampereInterruptingCapacity\":65,\"nameplateInformation.availableFaultCurrent\":53990,\"nameplateInformation.mainsRating\":600,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-DPH5A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-SWBD-5A2\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":20,\"nameplateInformation.ampereInterruptingCapacity\":65,\"nameplateInformation.availableFaultCurrent\":53504,\"nameplateInformation.mainsRating\":400,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-DPH5A3\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-SWBD-5A2\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":20,\"nameplateInformation.ampereInterruptingCapacity\":65,\"nameplateInformation.availableFaultCurrent\":56871,\"nameplateInformation.mainsRating\":1200,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-DPHBA1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-SWBD-5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"UTILITYENTRANCEN0311\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\"},{\"AssetName\":\"N-DPL1A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-T150-DPL1A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"EMERGN1674\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":8449,\"nameplateInformation.mainsRating\":600,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-DPL2A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-T150-DPL2A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"EMERGN2654\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":8487,\"nameplateInformation.mainsRating\":600,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-DPL3A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-T150-DPL3A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"EMERGN3654\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":8508,\"nameplateInformation.mainsRating\":600,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-DPL4A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-T150-DPL4A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"EMERGN4654\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":8537,\"nameplateInformation.mainsRating\":600,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-DPL5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-T75-DPL5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":6116,\"nameplateInformation.mainsRating\":400,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-DPLBA1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-T75-DPLBA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"ELECN0411\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":5830,\"nameplateInformation.mainsRating\":400,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-DPLRH5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-ATS-E5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":25,\"nameplateInformation.availableFaultCurrent\":17848,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-DPLRH5A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-ATS-SB5A2\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":50,\"nameplateInformation.availableFaultCurrent\":48970,\"nameplateInformation.mainsRating\":400,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-DPSBH5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-ATS-SB5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P3W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":65,\"nameplateInformation.mainsRating\":1200,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-DPSBH5A4\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-ATS-LR5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MECHANICALROOMN5332\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":50,\"nameplateInformation.availableFaultCurrent\":45870,\"nameplateInformation.mainsRating\":400,\"nameplateInformation.mainsType\":\"MLO\"}],\"DFTR-L\":[{\"AssetName\":\"TR-SWBD-5A1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"OlsunElectrics\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"A75589\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.kva\":3000,\"nameplateInformation.frequency\":60,\"nameplateInformation.primaryVoltage\":13200,\"nameplateInformation.secondaryVoltage\":\"480Y\\/277\",\"nameplateInformation.temperatureRise\":150,\"nameplateInformation.class\":\"AA\\/FFA\",\"nameplateInformation.bil\":\"95kv\\/30kv\",\"nameplateInformation.primaryAmperes\":131.2,\"nameplateInformation.secondaryAmperes\":3608.6,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":\"1-2\",\"nameplateInformation.tapPosition2\":\"2-3\",\"nameplateInformation.tapPosition3\":\"1-4\",\"nameplateInformation.tapPosition4\":\"3-4\",\"nameplateInformation.tapPosition5\":\"4-5\",\"nameplateInformation.voltage1\":13866,\"nameplateInformation.voltage2\":13533,\"nameplateInformation.voltage3\":13200,\"nameplateInformation.voltage4\":12867,\"nameplateInformation.voltage5\":12535},{\"AssetName\":\"TR-SWBD-5A2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A2\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A2\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"OlsunElectrics\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"A75589\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.kva\":3000,\"nameplateInformation.frequency\":60,\"nameplateInformation.primaryVoltage\":13200,\"nameplateInformation.secondaryVoltage\":\"480Y\\/277\",\"nameplateInformation.temperatureRise\":150,\"nameplateInformation.class\":\"AA\\/FFA\",\"nameplateInformation.bil\":\"95kv\\/30kv\",\"nameplateInformation.primaryAmperes\":131.2,\"nameplateInformation.secondaryAmperes\":3608.6,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":\"1-2\",\"nameplateInformation.tapPosition2\":\"2-3\",\"nameplateInformation.tapPosition3\":\"1-4\",\"nameplateInformation.tapPosition4\":\"3-4\",\"nameplateInformation.tapPosition5\":\"4-5\",\"nameplateInformation.voltage1\":13866,\"nameplateInformation.voltage2\":13533,\"nameplateInformation.voltage3\":13200,\"nameplateInformation.voltage4\":12867,\"nameplateInformation.voltage5\":12535}],\"DFTR-S\":[{\"AssetName\":\"N-T225-FIRE-PUMP\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-MVS-BA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"FIREPUMPROOMN0641\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"OlsunElectrics\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"A75557\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":5.75,\"nameplateInformation.kva\":225,\"nameplateInformation.frequency\":60,\"nameplateInformation.primaryVoltage\":13200,\"nameplateInformation.secondaryVoltage\":\"480Y\\/277\",\"nameplateInformation.temperatureRise\":150,\"nameplateInformation.class\":\"AA\\/FFA\",\"nameplateInformation.bil\":\"95kv\\/30kv\",\"nameplateInformation.primaryAmperes\":9.9,\"nameplateInformation.secondaryAmperes\":271,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":\"1-2\",\"nameplateInformation.tapPosition2\":\"2-3\",\"nameplateInformation.tapPosition3\":\"1-4\",\"nameplateInformation.tapPosition4\":\"3-4\",\"nameplateInformation.tapPosition5\":\"4-5\",\"nameplateInformation.voltage1\":13866,\"nameplateInformation.voltage2\":13533,\"nameplateInformation.voltage3\":13200,\"nameplateInformation.voltage4\":12867,\"nameplateInformation.voltage5\":12535},{\"AssetName\":\"N-T150-DPL1A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-H1A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"ELECN1673\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F4916\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":3.97,\"nameplateInformation.kva\":150,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T150-DPL2A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-H2A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"ELECN2653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F4916\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":3.97,\"nameplateInformation.kva\":150,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T150-DPL3A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-H3A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"ELECN3653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F4916\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":3.97,\"nameplateInformation.kva\":150,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T150-DPL4A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-H4A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"ELECN4653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F4916\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":3.97,\"nameplateInformation.kva\":150,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T150-SBL2A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-DPSBH5A2\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"EMERGN2654\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F7516\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":3.82,\"nameplateInformation.kva\":150,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T150-SBL3A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-DPSBH5A3\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"EMERGN3654\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F7516\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":3.82,\"nameplateInformation.kva\":150,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T150-SBL4A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-DPSBH5A3\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"EMERGN4654\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F7516\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":3.82,\"nameplateInformation.kva\":150,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T150-SBLBA1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-DPSBH5A2\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"ELECN0411\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F7516\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":3.82,\"nameplateInformation.kva\":150,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T30-EL5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-DPEH5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F3016\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":2.21,\"nameplateInformation.kva\":30,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T30-SBL5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-DPSBH5A4\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F3016\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":2.21,\"nameplateInformation.kva\":30,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T30-SBLBA6\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-SBHBA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F3016\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":2.21,\"nameplateInformation.kva\":30,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T45-L5A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-DPH5A3\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F4516\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":3.81,\"nameplateInformation.kva\":45,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T45-LBA5\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-DPHBA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"UTILITYENTRANCEN0311\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F4516\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":3.81,\"nameplateInformation.kva\":45,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T45-SBL5B1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-DPSBH5A3\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EXHAUSTFANSN5212\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F3016\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":2.21,\"nameplateInformation.kva\":45,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T75-DPL5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-H5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F7516\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":3.82,\"nameplateInformation.kva\":75,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T75-DPLBA1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-HBA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"ELECN0411\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F7516\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":3.82,\"nameplateInformation.kva\":75,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432},{\"AssetName\":\"N-T75-SBL1A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-DPSBH5A2\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"EMERGN1674\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-POWER-AND-DISTRIBUTION-TRANSFORMERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.phase\":3,\"nameplateInformation.model\":\"V48M28F7516\",\"nameplateInformation.primaryVector\":\"Delta\",\"nameplateInformation.secondaryVector\":\"Wye\",\"nameplateInformation.percentImpedance\":3.82,\"nameplateInformation.kva\":75,\"nameplateInformation.frequency\":60,\"nameplateInformation.type\":\"GeneralPurposeVented\",\"nameplateInformation.primaryVoltage\":480,\"nameplateInformation.secondaryVoltage\":\"208Y\\/120\",\"nameplateInformation.temperatureRise\":115,\"nameplateInformation.sampleType\":\"Dy1\",\"nameplateInformation.tapPosition1\":1,\"nameplateInformation.tapPosition2\":2,\"nameplateInformation.tapPosition3\":3,\"nameplateInformation.tapPosition4\":4,\"nameplateInformation.tapPosition5\":5,\"nameplateInformation.tapPosition6\":6,\"nameplateInformation.tapPosition7\":7,\"nameplateInformation.voltage1\":504,\"nameplateInformation.voltage2\":492,\"nameplateInformation.voltage3\":480,\"nameplateInformation.voltage4\":468,\"nameplateInformation.voltage5\":456,\"nameplateInformation.voltage6\":444,\"nameplateInformation.voltage7\":432}],\"GENR\":[{\"AssetName\":\"GENERATOR\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"GENERATORROOMN565\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"N\\/A\"},{\"AssetName\":\"1600AGENERATORDOCKINGSTATION\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"GENERATOR\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"OUTSIDE\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Average\",\"Criticality\":\"Medium\",\"PMPlan\":\"N\\/A\",\"nameplateInformation.ampereRating\":1600}],\"LAAR\":[{\"AssetName\":\"MAINSWITCH1LightningArresters\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-MVS-BA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"MVROOMN0412\",\"AssetSection\":\"N-MVS-BA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"N\\/A\"},{\"AssetName\":\"MAINSWITCH2LightningArresters\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-MVS-BA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"MVROOMN0412\",\"AssetSection\":\"N-MVS-BA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"N\\/A\"}],\"LVCB\":[{\"AssetName\":\"B-GENERATOR\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"GENERATOR\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"GENERATORROOMN565\",\"AssetSection\":\"GENERATOR\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\"},{\"AssetName\":\"B-N-SWBD-5A1-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"MagnumSBN-C4N\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":\"4000A\",\"tripUnitInformation.tripModuleAmpereRating\":\"4000A\",\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"Digitrip520MCLSIG\",\"tripUnitSettings.longtimePickUpRanges\":\"0.4–1.0x(In)\",\"tripUnitSettings.longtimeDelayRanges\":\"2–24seconds\",\"tripUnitSettings.shorttimePickUpRanges\":\"200–1000%x(Ir)andM1\",\"tripUnitSettings.shorttimeDelayRanges\":\"100–500ms\",\"tripUnitSettings.groundFaultPickUpRanges\":\"25–100%x(In)\",\"tripUnitSettings.groundFaultDelayRanges\":\"100–500ms\",\"tripUnitSettings.instantaneousPickUpRanges\":\"200–1000%x(In)andM1\"},{\"AssetName\":\"B-TIEBREAKER\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"MagnumSBN-C4N\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":\"4000A\",\"tripUnitInformation.tripModuleAmpereRating\":\"4000A\",\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"Digitrip520MCLSIG\",\"tripUnitSettings.longtimePickUpRanges\":\"0.4–1.0x(In)\",\"tripUnitSettings.longtimeDelayRanges\":\"2–24seconds\",\"tripUnitSettings.shorttimePickUpRanges\":\"200–1000%x(Ir)andM1\",\"tripUnitSettings.shorttimeDelayRanges\":\"100–500ms\",\"tripUnitSettings.groundFaultPickUpRanges\":\"25–100%x(In)\",\"tripUnitSettings.groundFaultDelayRanges\":\"100–500ms\",\"tripUnitSettings.instantaneousPickUpRanges\":\"200–1000%x(In)andM1\"},{\"AssetName\":\"B-N-SWBD-5A2-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A2\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A2\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"MagnumSBN-C4N\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":\"4000A\",\"tripUnitInformation.tripModuleAmpereRating\":\"4000A\",\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"Digitrip520MCLSIG\",\"tripUnitSettings.longtimePickUpRanges\":\"0.4–1.0x(In)\",\"tripUnitSettings.longtimeDelayRanges\":\"2–24seconds\",\"tripUnitSettings.shorttimePickUpRanges\":\"200–1000%x(Ir)andM1\",\"tripUnitSettings.shorttimeDelayRanges\":\"100–500ms\",\"tripUnitSettings.groundFaultPickUpRanges\":\"25–100%x(In)\",\"tripUnitSettings.groundFaultDelayRanges\":\"100–500ms\",\"tripUnitSettings.instantaneousPickUpRanges\":\"200–1000%x(In)andM1\"}],\"MCCB-L\":[{\"AssetName\":\"B-Unspecified\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPHBA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"UTILITYENTRANCEN0311\",\"AssetSection\":\"N-DPHBA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":250,\"nameplateInformation.interruptingRating\":50,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-N-DPL1A1-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPL1A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"EMERGN1674\",\"AssetSection\":\"N-DPL1A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":600,\"tripUnitInformation.tripModuleAmpereRating\":600,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-DPL2A1-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPL2A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"EMERGN2654\",\"AssetSection\":\"N-DPL2A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":600,\"tripUnitInformation.tripModuleAmpereRating\":600,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-NDPL5A1-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPL5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N-DPL5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":250,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-N-DPLBA1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPLBA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"ELECN0411\",\"AssetSection\":\"N-DPLBA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":225,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-DHRC-1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPSBH5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-DPSBH5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":300,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20\"},{\"AssetName\":\"B-N-DPSB5A2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPSBH5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-DPSBH5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG5\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":800,\"tripUnitInformation.tripModuleAmpereRating\":800,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20\"},{\"AssetName\":\"B-N-DPSBH5A1-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPSBH5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-DPSBH5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG5\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":1200,\"tripUnitInformation.tripModuleAmpereRating\":1200,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20LSIGw\\/ARMS\"},{\"AssetName\":\"B-N-DPSBH5A3\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPSBH5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-DPSBH5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG5\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":800,\"tripUnitInformation.tripModuleAmpereRating\":800,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20\"},{\"AssetName\":\"B-N-SWBD-5A2-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPSBH5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-DPSBH5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG5\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":1200,\"tripUnitInformation.tripModuleAmpereRating\":1200,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20\"},{\"AssetName\":\"B-N-SBL2A1-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SBL2A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"ELECN2653\",\"AssetSection\":\"N-SBL2A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":400,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-SBL3A1-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SBL3A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"ELECN3653\",\"AssetSection\":\"N-SBL3A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":400,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-SBL4A1-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SBL4A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"ELECN4653\",\"AssetSection\":\"N-SBL4A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":400,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-SBLBA1-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SBLBA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"ELECN0411\",\"AssetSection\":\"N-SBLBA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":600,\"tripUnitInformation.tripModuleAmpereRating\":600,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-DPH5A1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":800,\"tripUnitInformation.tripModuleAmpereRating\":600,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-ATS-E5A1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":150,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-ATS-LR5A1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":225,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-ATS-SB5A1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG5\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":1200,\"tripUnitInformation.tripModuleAmpereRating\":1200,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSIw\\/ARMS\"},{\"AssetName\":\"B-NDPHBA1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":800,\"tripUnitInformation.tripModuleAmpereRating\":600,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-SPARE-1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG5\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":800,\"tripUnitInformation.tripModuleAmpereRating\":800,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-SPARE-2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG5\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":800,\"tripUnitInformation.tripModuleAmpereRating\":800,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-SPARE-3\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG5\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":800,\"tripUnitInformation.tripModuleAmpereRating\":800,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-ATS-LR5A2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A2\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A2\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":400,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-ATS-SB5A2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A2\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A2\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":400,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-NDPH5A2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A2\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A2\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG5\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":800,\"tripUnitInformation.tripModuleAmpereRating\":800,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-NDPH5A3\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A2\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A2\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG5\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":800,\"tripUnitInformation.tripModuleAmpereRating\":600,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-NDPSBH5A1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A2\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A2\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG5\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":1200,\"tripUnitInformation.tripModuleAmpereRating\":1200,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-SPARE-1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A2\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A2\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG5\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":800,\"tripUnitInformation.tripModuleAmpereRating\":800,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-SPARE-2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A2\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A2\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG5\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":1200,\"tripUnitInformation.tripModuleAmpereRating\":1200,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSIw\\/ARMS\"},{\"AssetName\":\"B-SPARE-3\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A2\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A2\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG5\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":800,\"tripUnitInformation.tripModuleAmpereRating\":800,\"nameplateInformation.interruptingRating\":100,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-ATS-E5A1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-E5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-SWBD-E5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":150,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-ATS-LR5A1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-E5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-SWBD-E5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":225,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-ATS-LR5A2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-E5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-SWBD-E5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":400,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-ATS-SB5A1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-E5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-SWBD-E5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG5\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":1200,\"tripUnitInformation.tripModuleAmpereRating\":1200,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20LSIw\\/ARMS\"},{\"AssetName\":\"B-N-ATS-SB5A2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-E5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-SWBD-E5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":400,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-CTRL-FIRE-PUMP\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-E5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-SWBD-E5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":400,\"tripUnitInformation.tripModuleAmpereRating\":225,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-SWBD-E5A1-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-E5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-SWBD-E5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG6\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":1600,\"tripUnitInformation.tripModuleAmpereRating\":1600,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20LSIGw\\/ARMS\"}],\"MCCB-S\":[{\"AssetName\":\"B-N-EH1A1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPEH5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-DPEH5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":100,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-EH4A1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPEH5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-DPEH5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":100,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-T30-EL5A1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPEH5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-DPEH5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":60,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-AHU-01(RETURN)\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPH5A3\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N-DPH5A3\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":70,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-AHU-01(SUPPLY)\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPH5A3\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N-DPH5A3\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":90,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-AHU-02(RETURN)\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPH5A3\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N-DPH5A3\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":70,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-AHU-02(SUPPLY)\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPH5A3\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N-DPH5A3\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":90,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-AHU-03(RETURN)1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPH5A3\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N-DPH5A3\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":100,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-AHU-03(RETURN)2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPH5A3\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N-DPH5A3\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":100,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-AHU-03(SUPPLY)1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPH5A3\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N-DPH5A3\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":250,\"tripUnitInformation.tripModuleAmpereRating\":150,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-AHU-03(SUPPLY)2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPH5A3\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N-DPH5A3\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":250,\"tripUnitInformation.tripModuleAmpereRating\":150,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-N-L5A2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPH5A3\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N-DPH5A3\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG3\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":70,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-CRB-RO-01\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPHBA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"UTILITYENTRANCEN0311\",\"AssetSection\":\"N-DPHBA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":20,\"nameplateInformation.interruptingRating\":50,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-JP-1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPHBA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"UTILITYENTRANCEN0311\",\"AssetSection\":\"N-DPHBA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":15,\"nameplateInformation.interruptingRating\":50,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-N-T45-LBA5\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPHBA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"UTILITYENTRANCEN0311\",\"AssetSection\":\"N-DPHBA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":70,\"nameplateInformation.interruptingRating\":50,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-RHWP-2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPHBA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"UTILITYENTRANCEN0311\",\"AssetSection\":\"N-DPHBA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":50,\"nameplateInformation.interruptingRating\":50,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-WR\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPHBA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"UTILITYENTRANCEN0311\",\"AssetSection\":\"N-DPHBA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":30,\"nameplateInformation.interruptingRating\":50,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-AHU-01(RETURN)\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPSBH5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-DPSBH5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":70,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"N\\/A\"},{\"AssetName\":\"B-AHU-01(SUPPLY)\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPSBH5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-DPSBH5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":70,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"N\\/A\"},{\"AssetName\":\"B-AHU-02(RETURN)\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPSBH5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-DPSBH5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":70,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"N\\/A\"},{\"AssetName\":\"B-AHU-02(SUPPLY)\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPSBH5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-DPSBH5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":70,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"N\\/A\"},{\"AssetName\":\"B-LC-CRB-L5-01\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPSBH5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-DPSBH5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":250,\"tripUnitInformation.tripModuleAmpereRating\":125,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20\"},{\"AssetName\":\"B-N-UPS10\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-DPSBH5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-DPSBH5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"480Y\\/277V\",\"nameplateInformation.frameAmpereRating\":250,\"tripUnitInformation.tripModuleAmpereRating\":125,\"nameplateInformation.interruptingRating\":65,\"tripUnitInformation.tripModel\":\"PXR20\"},{\"AssetName\":\"B-N-EL5A1-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-EL5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-EL5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":100,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"PXR20\"},{\"AssetName\":\"B-N-L5A2-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-L5A2\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N-L5A2\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":225,\"tripUnitInformation.tripModuleAmpereRating\":225,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"N\\/A\"},{\"AssetName\":\"B-N-LBA5-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-LBA5\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"UTILITYENTRANCEN0311\",\"AssetSection\":\"N-LBA5\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":225,\"tripUnitInformation.tripModuleAmpereRating\":150,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"N\\/A\"},{\"AssetName\":\"B-N-SBL1A1-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SBL1A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"ELECN1673\",\"AssetSection\":\"N-SBL1A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":225,\"tripUnitInformation.tripModuleAmpereRating\":225,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-N-SBL5A1-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SBL5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-SBL5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":100,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"PXR20\"},{\"AssetName\":\"B-N-SBL5B1-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SBL5B1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EXHAUSTFANSN5212\",\"AssetSection\":\"N-SBL5B1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":225,\"tripUnitInformation.tripModuleAmpereRating\":150,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-SBLBA6-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SBLBA6\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"ELECN0411\",\"AssetSection\":\"N-SBLBA6\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":100,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"PXR20LSI\"},{\"AssetName\":\"B-N-UPS10-L2B-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-UPS10-L2B\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"IDFN2324\",\"AssetSection\":\"N-UPS10-L2B\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":250,\"tripUnitInformation.tripModuleAmpereRating\":250,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"TMTU\"},{\"AssetName\":\"B-N-UPS11-L5A1-MAIN\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-UPS11-L5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N-UPS11-L5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"#REF!\",\"PMPlan\":\"70B-BREAKERS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"PDG2\",\"nameplateInformation.voltageRating\":\"208Y\\/120V\",\"nameplateInformation.frameAmpereRating\":100,\"tripUnitInformation.tripModuleAmpereRating\":100,\"nameplateInformation.interruptingRating\":10,\"tripUnitInformation.tripModel\":\"PXR20LSI\"}],\"MMME\":[{\"AssetName\":\"UTILITYMETERING\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-MVS-BA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"MVROOMN0412\",\"AssetSection\":\"N-MVS-BA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"N\\/A\"},{\"AssetName\":\"M-SWBD-5A1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"N\\/A\"},{\"AssetName\":\"M-SWBD-5A2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-SWBD-5A2\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N-SWBD-5A2\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"N\\/A\"}],\"MTSW\":[{\"AssetName\":\"N-CTRL-FIRE-PUMP\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-T225-FIRE-PUMP\",\"Fed-ByAsset(2)\":\"N-SWBD-E5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"FIREPUMPROOMN0641\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-SWITCHES\"},{\"AssetName\":\"N-MTS-DPEH5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-ATS-E5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-SWITCHES\"},{\"AssetName\":\"N-MTS-UPS11\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-DPLRH5A2\",\"Fed-ByAsset(2)\":\"N-DPH5A2\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-SWITCHES\"}],\"PANL\":[{\"AssetName\":\"N-DPSBH5A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPSBH5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P3W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":65,\"nameplateInformation.availableFaultCurrent\":54727,\"nameplateInformation.mainsRating\":800,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-DPSBH5A3\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPSBH5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":65,\"nameplateInformation.availableFaultCurrent\":54242,\"nameplateInformation.mainsRating\":800,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-EH1A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPEH5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"ELECN1673\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":14,\"nameplateInformation.availableFaultCurrent\":12654,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-EH4A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPEH5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"ELECN4653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":25,\"nameplateInformation.availableFaultCurrent\":15946,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-EL1A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-EL5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"ELECN1673\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":1442,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-EL5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-T30-EL5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":1827,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-H1A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPH5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"ELECN1673\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":50,\"nameplateInformation.availableFaultCurrent\":45492,\"nameplateInformation.mainsRating\":400,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-H1A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL1A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"ELECN1673\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":50,\"nameplateInformation.availableFaultCurrent\":44789,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-H2A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPH5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"ELECN2653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":50,\"nameplateInformation.availableFaultCurrent\":47955,\"nameplateInformation.mainsRating\":400,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-H2A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL2A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"ELECN2653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":50,\"nameplateInformation.availableFaultCurrent\":47221,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-H3A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPH5A2\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"ELECN3653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":50,\"nameplateInformation.availableFaultCurrent\":48932,\"nameplateInformation.mainsRating\":400,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-H3A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL3A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"ELECN3653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":50,\"nameplateInformation.availableFaultCurrent\":48200,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-H4A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPH5A2\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"ELECN4653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":65,\"nameplateInformation.availableFaultCurrent\":51180,\"nameplateInformation.mainsRating\":400,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-H4A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL4A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"ELECN4653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":65,\"nameplateInformation.availableFaultCurrent\":50443,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-H5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPH5A2\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":65,\"nameplateInformation.availableFaultCurrent\":52639,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-H5A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-H5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":65,\"nameplateInformation.availableFaultCurrent\":50779,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-H5B1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-H5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EXHAUSTFANSN5212\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":65,\"nameplateInformation.availableFaultCurrent\":52278,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-HBA1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPH5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"ELECN0411\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":50,\"nameplateInformation.availableFaultCurrent\":28432,\"nameplateInformation.mainsRating\":250,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-HBA2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-HBA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"ELECN0411\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":50,\"nameplateInformation.availableFaultCurrent\":28079,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L1A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL1A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"ELECN1673\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":7536,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L1A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL1A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"ELECN1673\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":7393,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L1B1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL1A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"METABOLOMICSN1431\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4312,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L1B2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL1A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"METABOLOMICSN1431\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4252,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L1B3\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL1A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"CORRIDORN1230\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4137,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L1B4\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL1A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"CORRIDORN1230\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4081,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L2A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL2A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"EMERGN654\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":8392,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L2A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL2A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"ELECN2653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":7424,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L2A3\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL2A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"CORRIDORN2002\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":6750,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L2A4\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL2A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"CORRIDORN2002\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":6625,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L2B1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL2A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"CIRCULATIONN2336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4650,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L2B2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL2A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"CIRCULATIONN2336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4581,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L2B3\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL2A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"CIRCULATIONN2336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4515,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L3A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL3A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"EMERGN3654\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":7568,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L3A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL3A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"ELECN3653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":7442,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L3B1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL3A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"CIRCULATIONN3336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4589,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L3B2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL3A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"CIRCULATIONN3336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4658,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L3B3\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL3A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"CIRCULATIONN3336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4729,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L3B4\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL3A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"CIRCULATIONN3336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":3520,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L3B5\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL3A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"CIRCULATIONN3336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":3562,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L4A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL4A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"EMERGN4654\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":7465,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L4A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL4A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"ELECN4653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":7323,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L4A3\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL4A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"CIRCULATIONN3472\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":6535,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L4B1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL4A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"CIRCULATIONN4336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4668,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L4B2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL4A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"CIRCULATIONN4336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4739,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L4B3\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL4A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"CIRCULATIONN4336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":3568,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L4B4\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL4A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"CIRCULATIONN4336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":3526,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":6002,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L5A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-T45-L5A2\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"ELECN5552\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":3589,\"nameplateInformation.mainsRating\":150,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-L5B1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EXHAUSTFANSN5212\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":2817,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-L5B2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EXHAUSTFANSN5212\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":2844,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-L5B3\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPL5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EXHAUSTFANSN5212\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":6093,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-LBA1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPLBA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"ELECN0411\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":5787,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-LBA2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPLBA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"STORAGEN0424\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":5787,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-LBA3\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPLBA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"STORAGEN0424\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4229,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-LBA4\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPLBA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"TISSUEBANK&TISSUEANALYTICSN0421\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4229,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-LBA5\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-T45-LBA5\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"UTILITYENTRANCEN0311\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":3600,\"nameplateInformation.mainsRating\":150,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-SBHBA1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-DPSBH5A4\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"UTILITYENTRANCEN0311\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"480V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":25,\"nameplateInformation.availableFaultCurrent\":22594,\"nameplateInformation.mainsRating\":400,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBL1A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-T75-SBL1A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"ELECN1673\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":5464,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-SBL1B1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBL1A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"METABOLOMICSN1431\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":3277,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBL1B2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBL1A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"CORRIDORN1230\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":3184,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBL2A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-T150-SBL2A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"ELECN2653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":8349,\"nameplateInformation.mainsRating\":400,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-SBL2A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBL2A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"ELECN2653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":8252,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBL2A3\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBL2A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"CORRIDORN2002\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":7120,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBL2B1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBL2A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"CIRCULATIONN2336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":5589,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBL2B2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBL2A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"CIRCULATIONN2336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":5538,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBL3A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-T150-SBL3A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"ELECN3653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":8314,\"nameplateInformation.mainsRating\":400,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-SBL3A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBL3A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"ELECN3653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":8029,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBL3A3\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBL3A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"Space225\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":7094,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBL3B1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBL3A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"CIRCULATIONN3336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":5841,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBL3B2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBL3A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"Space251\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4808,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBL4A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-T150-SBL4A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"ELECN4653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":8401,\"nameplateInformation.mainsRating\":400,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-SBL4A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBL4A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"ELECN4653\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":8303,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBL4A3\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBL4A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"CIRCULATIONN3472\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":7241,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBL4B1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBL4A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"CIRCULATIONN4336\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":5832,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBL4B2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBL4A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"Space525\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4801,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBL5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-T30-SBL5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":1901,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-SBL5B1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-T45-SBL5B1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EXHAUSTFANSN5212\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":3183,\"nameplateInformation.mainsRating\":150,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-SBL5B2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBL5B1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EXHAUSTFANSN5212\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":2961,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBLBA1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBLBA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"ELECN0411\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":7854,\"nameplateInformation.mainsRating\":600,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-SBLBA2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBLBA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"STORAGEN0424\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":6399,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBLBA3\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBLBA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"BIOREPOSITORY(TISSUEBANK)N0422\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":6209,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBLBA4\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBLBA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"BIOREPOSITORY(TISSUEBANK)N0422\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":6148,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBLBA5\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-SBLBA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"LABSTORAGE\\/FUTUREFREEZERSN0642\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Recessed\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":72,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":5640,\"nameplateInformation.mainsRating\":225,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-SBLBA6\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-T30-SBLBA6\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"ELECN0411\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Low\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":1854,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-UPS10-L2B\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-UPS10\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"IDFN2324\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":6295,\"nameplateInformation.mainsRating\":250,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-UPS11-L1A\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-UPS11-L5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level01\",\"AssetRoom\":\"EMERGN1674\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":2703,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-UPS11-L2A\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-UPS11-L5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level02\",\"AssetRoom\":\"EMERGN2654\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":3079,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-UPS11-L3A\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-UPS11-L5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level03\",\"AssetRoom\":\"EMERGN3654\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":3575,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-UPS11-L4A\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-UPS11-L5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level04\",\"AssetRoom\":\"EMERGN4654\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":4086,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-UPS11-L5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-UPS11\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":50,\"nameplateInformation.availableFaultCurrent\":28715,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MCB\"},{\"AssetName\":\"N-UPS11-L5A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-UPS11-L5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":25,\"nameplateInformation.availableFaultCurrent\":22195,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"},{\"AssetName\":\"N-UPS11-LBA\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset\":\"N-UPS11-L5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"UTILITYENTRANCEN0311\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.mountingType\":\"Surface\",\"nameplateInformation.enclosureType\":\"Type1\",\"nameplateInformation.voltageRating\":\"120V\",\"nameplateInformation.configuration\":\"3P\\/4W\",\"nameplateInformation.numberOfPoles\":42,\"nameplateInformation.ampereInterruptingCapacity\":10,\"nameplateInformation.availableFaultCurrent\":1560,\"nameplateInformation.mainsRating\":100,\"nameplateInformation.mainsType\":\"MLO\"}],\"PITR\":[{\"AssetName\":\"VOLTAGETRANSFORMER1\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-MVS-BA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"MVROOMN0412\",\"AssetSection\":\"N-MVS-BA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"N\\/A\"},{\"AssetName\":\"VOLTAGETRANSFORMER2\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-MVS-BA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"MVROOMN0412\",\"AssetSection\":\"N-MVS-BA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"N\\/A\"},{\"AssetName\":\"VOLTAGETRANSFORMER3\",\"AssetType\":\"Sub-component\",\"Top-LevelAsset\":\"N-MVS-BA1\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"MVROOMN0412\",\"AssetSection\":\"N-MVS-BA1\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"N\\/A\"}],\"SWBD\":[{\"AssetName\":\"N-SWBD-5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-MVS-BA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"Pow-R-LineXpert(PRLX)\",\"nameplateInformation.voltageRating\":\"480\\/277Wye\",\"nameplateInformation.phaseAmpereRating\":4000,\"nameplateInformation.faultWithstandRating\":100,\"nameplateInformation.mainSeparate\":\"Yes\"},{\"AssetName\":\"N-SWBD-5A2\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-MVS-BA1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"MAINELECN5553\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"Pow-R-LineXpert(PRLX)\",\"nameplateInformation.voltageRating\":\"480\\/277Wye\",\"nameplateInformation.phaseAmpereRating\":4000,\"nameplateInformation.faultWithstandRating\":100,\"nameplateInformation.mainSeparate\":\"Yes\"},{\"AssetName\":\"N-SWBD-E5A1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"1600AGENERATORDOCKINGSTATION\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-PANELBOARDS-AND-SWITCHBOARDS\",\"nameplateInformation.manufacturer\":\"Eaton\",\"nameplateInformation.model\":\"Pow-R-LineXpert(PRLX)\",\"nameplateInformation.voltageRating\":\"480\\/277Wye\",\"nameplateInformation.phaseAmpereRating\":1600,\"nameplateInformation.faultWithstandRating\":65,\"nameplateInformation.mainSeparate\":\"Yes\"}],\"SWGR\":[{\"AssetName\":\"N-MVS-BA1\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N\\/A\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"LowerLevel\",\"AssetRoom\":\"MVROOMN0412\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"High\",\"PMPlan\":\"70B-SWITCHGEAR-AND-SUBSTATIONS\"}],\"UPSX\":[{\"AssetName\":\"N-UPS10\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-DPSBH5A1\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"N\\/A\"},{\"AssetName\":\"N-UPS11\",\"AssetType\":\"Top-levelAsset\",\"Top-LevelAsset\":\"N\\/A\",\"Fed-ByAsset(1)\":\"N-MTS-UPS11\",\"AssetBuilding\":\"N\",\"AssetFloor\":\"Level05\",\"AssetRoom\":\"EMERGENCYELECTRICALN5551\",\"AssetSection\":\"N\\/A\",\"AssetCondition\":\"OperatingNormally\",\"OperatingConditions\":\"Good\",\"Criticality\":\"Medium\",\"PMPlan\":\"N\\/A\"}]}";
            var get_json_obj = System.Text.Json.JsonSerializer.Deserialize<pmlanjsonobj>(json);

            var get_assets = _UoW.AssetPMsRepository.GetStaffAssers();

            var remaian_asset = get_assets.Where(x => x.AssetPMs != null && x.AssetPMs.Count == 0).ToList();

          /*  foreach(var assets in get_assets)
            {
                var get_asset_class = _UoW.AssetPMsRepository.GetAssetclassbycode(assets.InspectionTemplateAssetClass.asset_class_code);

                assets.inspectiontemplate_asset_class_id = get_asset_class.inspectiontemplate_asset_class_id;

                var updtae = await _UoW.BaseGenericRepository<Asset>().Update(assets);
            }*/




            foreach (var asset in remaian_asset)
            {
                if (asset.inspectiontemplate_asset_class_id != null)
                {
                    var get_pmplan = _UoW.AssetPMsRepository.GetStaffPMplamns(asset.inspectiontemplate_asset_class_id.Value);
                    if (get_pmplan != null)
                    {
                        AssignPMToAsset AssignPMToAsset = new AssignPMToAsset();
                        AssignPMToAsset.asset_id = asset.asset_id;
                        AssignPMToAsset.pm_plan_id = get_pmplan.pm_plan_id;

                        var result = await AddAssetPM(AssignPMToAsset);
                    }
                }
            }

            return 1;
        }


        public ListViewModel<GetAssetPMListAssetWiseResponsemodel> GetAssetPMListAssetWise(GetAssetPMListRequestmodel requestmodel)
        {
            ListViewModel<GetAssetPMListAssetWiseResponsemodel> response = new ListViewModel<GetAssetPMListAssetWiseResponsemodel>();

            try
            {
                GetAssetPMListRequestmodel getAssetPMListRequestmodel = new GetAssetPMListRequestmodel();
                getAssetPMListRequestmodel.pagesize = 0;
                getAssetPMListRequestmodel.pageindex = 0;
                getAssetPMListRequestmodel.search_string = requestmodel.search_string;
                getAssetPMListRequestmodel.status = requestmodel.status;
                getAssetPMListRequestmodel.is_requested_for_overdue_pm = requestmodel.is_requested_for_overdue_pm;

                var list = GetAssetPMListOptimized(getAssetPMListRequestmodel);

                var asset_id_list = list.list.Select(x => x.asset_id).Distinct().ToList();

                foreach (var id in asset_id_list)
                {
                    GetAssetPMListAssetWiseResponsemodel GetAssetPMListAssetWiseResponsemodel = new GetAssetPMListAssetWiseResponsemodel();
                    GetAssetPMListAssetWiseResponsemodel.asset_id = id;
                    GetAssetPMListAssetWiseResponsemodel.name = list.list.Where(x => x.asset_id == id).Select(x => x.asset_name).FirstOrDefault();
                    GetAssetPMListAssetWiseResponsemodel.asset_pms_list = list.list.Where(x => x.asset_id == id).ToList();

                    response.list.Add(GetAssetPMListAssetWiseResponsemodel);
                }

                response.listsize = response.list.Count;
                response.list = response.list.OrderBy(x => x.name).ToList();

                if (requestmodel.pagesize > 0 && requestmodel.pageindex > 0)
                {
                    response.list = response.list.Skip((requestmodel.pageindex - 1) * requestmodel.pagesize).Take(requestmodel.pagesize).ToList();
                }

                response.pageIndex = requestmodel.pageindex;
                response.pageSize = requestmodel.pagesize;
                /*var groupedByAsset = from pm in get_asset_pms
                                     group pm by pm.asset_id into assetGroup
                                     select new
                                     {
                                         asset_id = assetGroup.Key,
                                         pms = assetGroup.ToList()
                                     };*/
            }
            catch (Exception e)
            {
            }
            return response;
        }

        public GetFilterDropdownAssetPMListResponseModel GetFilterDropdownAssetPMList(GetAssetPMListRequestmodel requestmodel)
        {
            GetFilterDropdownAssetPMListResponseModel response = new GetFilterDropdownAssetPMListResponseModel();
            try
            {
                var get_asset_pms = _UoW.AssetPMsRepository.GetAssetPMList(requestmodel);
                var get_asset_class_list = get_asset_pms.Item1.Select(x => x.Asset.InspectionTemplateAssetClass).Distinct().ToList();

                response.pm_title_list = get_asset_pms.Item1.Select(x => x.title).Distinct().ToList();
                response.asset_class_list = _mapper.Map<List<FilterDropdownAssetPMListClassCodeList>>(get_asset_class_list);
            }
            catch (Exception e)
            {
            }
            return response;
        }

        public async Task<int> AddUpdatePMItemsMasterData(AddUpdatePMItemsMasterDataRequestModel requestModel)
        {
            int res = (int)ResponseStatusNumber.NotFound;
            try
            {
                PMItemMasterForms pMItemMasterForms = new PMItemMasterForms();
                pMItemMasterForms.asset_class_code = requestModel.asset_class_code;
                pMItemMasterForms.company_id = requestModel.company_id;
                pMItemMasterForms.form_json = requestModel.form_json;
                pMItemMasterForms.asset_class_name = requestModel.asset_class_name;
                pMItemMasterForms.plan_name = requestModel.plan_name;
                pMItemMasterForms.pm_title = requestModel.pm_title;
                pMItemMasterForms.created_at = DateTime.UtcNow;

                var insert = await _UoW.BaseGenericRepository<PMItemMasterForms>().Insert(pMItemMasterForms);
                _UoW.SaveChanges();
                if (insert)
                {
                    res = (int)(ResponseStatusNumber.Success);
                }
            }
            catch (Exception ex) { }

            return res;
        }
        public async Task<int> AddPMbySteps(AddPMbyStepsRequestmodel request)
        {
            //add or update install woline data
            //
            WorkOrderService woservice = new WorkOrderService(_mapper);
            PMService pmservice = new PMService(_mapper);
            var install_woline =  await woservice.UpdateOBWOAssetDetails(request.install_woline_details);

            // get_install_woline 
            var get_install_woline = _UoW.IssueRepository.GetWolneById(install_woline.woonboardingassets_id);

            // add pm woline data
            if (request.asset_type == (int)PMWOlineAssettype.ExistingAsset) // if asset type is existing then everything will be in main table like pm plan and pm
            {
                // for active pm 
                var get_active_pm = request.pm_list.Where(x => x.asset_pm_id != null).ToList();
                if (get_active_pm.Count > 0)
                {
                    AddActiveAssetPMWolineByStepsRequestmodel AddActiveAssetPMWolineByStepsRequestmodel = new AddActiveAssetPMWolineByStepsRequestmodel();
                    AddActiveAssetPMWolineByStepsRequestmodel.pm_list = new List<active_pm_data>();
                    foreach (var item in get_active_pm)
                    {
                        active_pm_data active_pm_data = new active_pm_data();
                        active_pm_data.asset_pm_id = item.asset_pm_id.Value;
                        active_pm_data.pm_form_output_data = item.pm_form_output_data;
                        active_pm_data.woline_status = item.woline_status;
                        AddActiveAssetPMWolineByStepsRequestmodel.pm_list.Add(active_pm_data);
                    }
                    AddActiveAssetPMWolineByStepsRequestmodel.wo_id = request.wo_id;
                    AddActiveAssetPMWolineByStepsRequestmodel.install_woline = get_install_woline;
                    await AddActiveAssetPMWolineBySteps(AddActiveAssetPMWolineByStepsRequestmodel);
                }

                // for inactive PM
                var get_inactive_pm = request.pm_list.Where(x => x.pm_id != null).ToList();
                if (get_inactive_pm.Count > 0)
                {
                    AddMainAssetInactivePMinWORequestmodel AddMainAssetInactivePMinWORequestmodel = new AddMainAssetInactivePMinWORequestmodel();
                    AddMainAssetInactivePMinWORequestmodel.in_active_pm = new List<main_asset_inactive_pm>();
                    foreach (var item in get_inactive_pm)
                    {
                        main_asset_inactive_pm main_asset_inactive_pm = new main_asset_inactive_pm();
                        main_asset_inactive_pm.pm_id = item.pm_id.Value;
                        main_asset_inactive_pm.pm_form_output_data = item.pm_form_output_data;
                        main_asset_inactive_pm.woline_status = item.woline_status;
                        AddMainAssetInactivePMinWORequestmodel.in_active_pm.Add(main_asset_inactive_pm);
                    }
                    AddMainAssetInactivePMinWORequestmodel.wo_id = request.wo_id;
                    AddMainAssetInactivePMinWORequestmodel.install_woline = get_install_woline;
                    AddMainAssetInactivePMinWORequestmodel.asset_id = request.selected_asset_id.Value;

                    await AddMainAssetInactivePMinWO(AddMainAssetInactivePMinWORequestmodel);
                }
            }
            else if (request.asset_type == (int)PMWOlineAssettype.TempAsset || request.asset_type == (int)PMWOlineAssettype.NewAsset) // if asset type is temp asset or new asset then everything will be in temp table
            {
                AddPMtoTempAssetRequestmodel AddPMtoTempAssetRequestmodel = new AddPMtoTempAssetRequestmodel();
                AddPMtoTempAssetRequestmodel.install_woline = get_install_woline;
                AddPMtoTempAssetRequestmodel.wo_id = request.wo_id;
                AddPMtoTempAssetRequestmodel.woonboardingassets_id = get_install_woline.woonboardingassets_id;
                AddPMtoTempAssetRequestmodel.pm_list = new List<TempAssetPMlist>();
                request.pm_list.ForEach(x =>
                {
                    TempAssetPMlist TempAssetPMlist = new TempAssetPMlist();
                    TempAssetPMlist.pm_id = x.pm_id.Value;
                    TempAssetPMlist.pm_form_output_data = x.pm_form_output_data;

                    AddPMtoTempAssetRequestmodel.pm_list.Add(TempAssetPMlist);
                });

                await AddPMtoTempAsset(AddPMtoTempAssetRequestmodel);
            }
            return 1;
        }

        public async Task<List<AddAssetPMWolineResponsemodel>> AddActiveAssetPMWolineBySteps(AddActiveAssetPMWolineByStepsRequestmodel requestmodel)
        {
            List<AddAssetPMWolineResponsemodel> response = new List<AddAssetPMWolineResponsemodel>();
            foreach (var req_asset_pm in requestmodel.pm_list)
            {
                var asset_pm = _UoW.WorkOrderRepository.GetAssetpmToaddWOline(new List<Guid> { req_asset_pm.asset_pm_id }).FirstOrDefault();

                WOOnboardingAssets WOOnboardingAssets = new WOOnboardingAssets();
                WOOnboardingAssets.asset_class_name = requestmodel.install_woline.asset_class_name;
                WOOnboardingAssets.asset_class_code = requestmodel.install_woline.asset_class_code;
                WOOnboardingAssets.QR_code = requestmodel.install_woline.QR_code;
                WOOnboardingAssets.created_at = DateTime.UtcNow;
                WOOnboardingAssets.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                WOOnboardingAssets.is_deleted = false;
                WOOnboardingAssets.status = (int)Status.open;
                WOOnboardingAssets.site_id = requestmodel.install_woline.site_id;
                WOOnboardingAssets.wo_id = requestmodel.wo_id;
                WOOnboardingAssets.inspection_type = (int)MWO_inspection_wo_type.PM;
                WOOnboardingAssets.asset_id = asset_pm.asset_id;
                WOOnboardingAssets.is_wo_line_for_exisiting_asset = true;
                WOOnboardingAssets.building = requestmodel.install_woline.building;
                WOOnboardingAssets.floor = requestmodel.install_woline.floor;
                WOOnboardingAssets.room = requestmodel.install_woline.room;
                WOOnboardingAssets.section = requestmodel.install_woline.section;
                WOOnboardingAssets.tempasset_id = requestmodel.install_woline.tempasset_id;
                WOOnboardingAssets.status = req_asset_pm.woline_status;

                var insert = await _UoW.BaseGenericRepository<WOOnboardingAssets>().Insert(WOOnboardingAssets);
                _UoW.SaveChanges();

                // update assset pm 
                asset_pm.woonboardingassets_id = WOOnboardingAssets.woonboardingassets_id;
                asset_pm.wo_id = WOOnboardingAssets.wo_id;
                asset_pm.status = (int)Status.Schedule;
                asset_pm.modified_at = DateTime.UtcNow;

                ActiveAssetPMWOlineMapping ActiveAssetPMWOlineMapping = new ActiveAssetPMWOlineMapping();
                ActiveAssetPMWOlineMapping.woonboardingassets_id = WOOnboardingAssets.woonboardingassets_id;
                ActiveAssetPMWOlineMapping.asset_pm_id = asset_pm.asset_pm_id;
                ActiveAssetPMWOlineMapping.is_active = true;
                ActiveAssetPMWOlineMapping.is_deleted = false;
                ActiveAssetPMWOlineMapping.created_at = DateTime.UtcNow;
                ActiveAssetPMWOlineMapping.pm_form_output_data = req_asset_pm.pm_form_output_data;
               
                asset_pm.ActiveAssetPMWOlineMapping.Add(ActiveAssetPMWOlineMapping);

                var update = await _UoW.BaseGenericRepository<AssetPMs>().Update(asset_pm);
                _UoW.SaveChanges();

                // create issue from outout json
                await CreateIssuefromPMOutputJson(WOOnboardingAssets.woonboardingassets_id, asset_pm.asset_pm_id);


                var get_pm = _UoW.WorkOrderRepository.GetPMById(asset_pm.pm_id.Value);

                AddAssetPMWolineResponsemodel addAssetPMWolineResponsemodel = new AddAssetPMWolineResponsemodel();

                addAssetPMWolineResponsemodel.asset_pm_id = asset_pm.asset_pm_id;
                addAssetPMWolineResponsemodel.woonboardingassets_id = WOOnboardingAssets.woonboardingassets_id;
                if (get_pm != null)
                {
                    addAssetPMWolineResponsemodel.pm_inspection_type_id = get_pm.pm_inspection_type_id;
                }

                response.Add(addAssetPMWolineResponsemodel);

            }
            return response;
        }

        public async Task<int> CreateIssuefromPMOutputJson(Guid woonboardingassets_id , Guid asset_pm_id)
        {
            WorkOrderService woservice = new WorkOrderService(_mapper);

            var get_assetpm = _UoW.WorkOrderRepository.GetAssetPMsbyId(asset_pm_id);

            AddUpdateTempIssueFromWORequestmodel tempIssueRequest = woservice.OBWOMakeRequestmodelforWOlineIssue(woonboardingassets_id);
            if (tempIssueRequest.list_temp_issue != null && tempIssueRequest.list_temp_issue.Count > 0)
            {
                await woservice.AddUpdateTempIssueFromWO(tempIssueRequest);
            }

            if (tempIssueRequest.is_pm_clear)
                get_assetpm.is_Asset_PM_fixed = true;
            else
                get_assetpm.is_Asset_PM_fixed = false;

            // store additional images 
            if (tempIssueRequest.list_pm_additional_images != null && tempIssueRequest.list_pm_additional_images.Count > 0)
            {
                var db_additional_images = get_assetpm.ActiveAssetPMWOlineMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id).FirstOrDefault().WOlineAssetPMImagesMapping.ToList();
                var requested_imges = tempIssueRequest.list_pm_additional_images.Select(x => x.image_name).ToList();
                var db_images_names = db_additional_images.Select(x => x.image_name).ToList();

                // insert images if new added
                var new_imgs_names = requested_imges.Where(x => !db_images_names.Contains(x)).ToList();
                var new_images = tempIssueRequest.list_pm_additional_images.Where(x => new_imgs_names.Contains(x.image_name)).ToList();
                foreach (var item in new_images)
                {
                    WOlineAssetPMImagesMapping WOlineAssetPMImagesMapping = new WOlineAssetPMImagesMapping();
                    WOlineAssetPMImagesMapping.image_name = item.image_name;
                    WOlineAssetPMImagesMapping.created_at = DateTime.UtcNow;
                    WOlineAssetPMImagesMapping.image_type = item.image_type;
                    WOlineAssetPMImagesMapping.pm_image_caption = item.pm_image_caption;
                    get_assetpm.ActiveAssetPMWOlineMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id).FirstOrDefault().WOlineAssetPMImagesMapping.Add(WOlineAssetPMImagesMapping);
                }


                // update existing image 
                var exiting_imgs_names = requested_imges.Where(x => db_images_names.Contains(x)).ToList();
                var existing_images = tempIssueRequest.list_pm_additional_images.Where(x => exiting_imgs_names.Contains(x.image_name)).ToList();
                foreach (var item in existing_images)
                {
                    get_assetpm.ActiveAssetPMWOlineMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id).FirstOrDefault().WOlineAssetPMImagesMapping.Where(x => x.image_name == item.image_name).FirstOrDefault().pm_image_caption = item.pm_image_caption;
                    get_assetpm.ActiveAssetPMWOlineMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id).FirstOrDefault().WOlineAssetPMImagesMapping.Where(x => x.image_name == item.image_name).FirstOrDefault().image_type = item.image_type;
                    get_assetpm.ActiveAssetPMWOlineMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id).FirstOrDefault().WOlineAssetPMImagesMapping.Where(x => x.image_name == item.image_name).FirstOrDefault().modified_at = DateTime.UtcNow;
                }


                // delete if any image is exist in db but not in request 
                var delete_imgs_names = db_images_names.Where(x => !requested_imges.Contains(x)).ToList();
                // var delete_images = db_images_names.Where(x => delete_imgs_names.Contains(x.image_name)).ToList();

                foreach (var item in delete_imgs_names)
                {
                    get_assetpm.ActiveAssetPMWOlineMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id).FirstOrDefault().WOlineAssetPMImagesMapping.Where(x => x.image_name == item).FirstOrDefault().is_deleted = true;
                    get_assetpm.ActiveAssetPMWOlineMapping.Where(x => x.woonboardingassets_id == woonboardingassets_id).FirstOrDefault().WOlineAssetPMImagesMapping.Where(x => x.image_name == item).FirstOrDefault().modified_at = DateTime.UtcNow;
                }
            }

            // store ir visual images if any
            if (tempIssueRequest.list_pm_ir_scan_images != null && tempIssueRequest.list_pm_ir_scan_images.Count > 0)
            {
                // delete all image mapping and insert new 
                if (get_assetpm.WOOnboardingAssets.IRWOImagesLabelMapping != null && get_assetpm.WOOnboardingAssets.IRWOImagesLabelMapping.Count > 0)
                {
                    get_assetpm.WOOnboardingAssets.IRWOImagesLabelMapping.Where(x => !x.is_deleted).ToList().ForEach(x => {
                        x.updated_at = DateTime.UtcNow;
                        x.is_deleted = true;
                    });
                }
                foreach (var image in tempIssueRequest.list_pm_ir_scan_images)
                {
                    IRWOImagesLabelMapping IRWOImagesLabelMapping = new IRWOImagesLabelMapping();
                    IRWOImagesLabelMapping.ir_image_label = image.ir_image_label;
                    IRWOImagesLabelMapping.visual_image_label = image.visual_image_label;
                    IRWOImagesLabelMapping.created_at = DateTime.UtcNow;
                    IRWOImagesLabelMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    IRWOImagesLabelMapping.visual_image_label = image.visual_image_label;
                    IRWOImagesLabelMapping.site_id = get_assetpm.WorkOrders.site_id;
                    IRWOImagesLabelMapping.s3_image_folder_name = get_assetpm.WorkOrders.site_id.ToString() + "/" + get_assetpm.WorkOrders.manual_wo_number;

                    get_assetpm.WOOnboardingAssets.IRWOImagesLabelMapping.Add(IRWOImagesLabelMapping);
                }
            }

            var update = await _UoW.BaseGenericRepository<AssetPMs>().Update(get_assetpm);

            return 1;

        }

        public async Task<ManuallyAssignAnyPMtoWOResponseModel> AddMainAssetInactivePMinWO(AddMainAssetInactivePMinWORequestmodel requestModel)
        {
            ManuallyAssignAnyPMtoWOResponseModel response = new ManuallyAssignAnyPMtoWOResponseModel();

            AddActiveAssetPMWolineByStepsRequestmodel AddActiveAssetPMWolineByStepsRequestmodel = new AddActiveAssetPMWolineByStepsRequestmodel();
            AddActiveAssetPMWolineByStepsRequestmodel.wo_id = requestModel.wo_id;

            AddActiveAssetPMWolineByStepsRequestmodel.pm_list = new List<active_pm_data>();
            try
            {
                var get_plan_list = _UoW.PMRepository.GetPMPlansListByPMIds(requestModel.in_active_pm.Select(x=>x.pm_id).ToList());

                foreach (var plan in get_plan_list)
                {
                    AssetPMPlans get_asset_pm_plan = null;

                    get_asset_pm_plan = _UoW.PMRepository.CheckForPMPlanIsAny(plan.pm_plan_id, requestModel.asset_id);

                    if (get_asset_pm_plan == null)
                    {
                        get_asset_pm_plan = new AssetPMPlans();
                        get_asset_pm_plan = _mapper.Map<AssetPMPlans>(plan);
                        get_asset_pm_plan.pm_plan_id = plan.pm_plan_id;
                        get_asset_pm_plan.is_pm_plan_inspection_manual = true;
                        get_asset_pm_plan.asset_id = requestModel.asset_id;
                        get_asset_pm_plan.created_at = DateTime.UtcNow;
                        get_asset_pm_plan.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        var insert_plan = await _UoW.BaseGenericRepository<AssetPMPlans>().Insert(get_asset_pm_plan);
                    }

                    // plan.PMs = plan.PMs.ToList().Where(x => requestModel.pm_ids.Contains(x.pm_id) && !x.is_archive).ToList();

                    plan.PMs.ToList().ForEach(x =>
                    {
                        x.PMAttachments = x.PMAttachments.Where(y => !y.is_archive).ToList();
                    });


                    foreach (var pm in plan.PMs)
                    {
                        if (requestModel.in_active_pm.Select(x=>x.pm_id).Contains(pm.pm_id))
                        {
                            AssetPMs assetPMs = new AssetPMs();
                            assetPMs = _mapper.Map<AssetPMs>(pm);
                            assetPMs.asset_id = requestModel.asset_id;
                            assetPMs.asset_pm_plan_id = get_asset_pm_plan.asset_pm_plan_id;
                            assetPMs.is_pm_inspection_manual = true;
                            assetPMs.created_at = DateTime.UtcNow;
                            assetPMs.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            assetPMs.status = (int)Status.Active;

                            assetPMs.AssetPMAttachments.ToList().ForEach(y =>
                            {
                                y.asset_pm_plan_id = get_asset_pm_plan.asset_pm_plan_id;
                                y.asset_pm_id = assetPMs.asset_pm_id;
                                y.asset_id = requestModel.asset_id;
                                y.created_at = DateTime.UtcNow;
                                y.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            });

                            assetPMs.AssetPMsTriggerConditionMapping.ToList().ForEach(y =>
                            {
                                y.asset_pm_id = assetPMs.asset_pm_id;
                                y.created_at = DateTime.UtcNow;
                                y.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            });

                            var insert_pm = await _UoW.BaseGenericRepository<AssetPMs>().Insert(assetPMs);
                            _UoW.SaveChanges();


                            active_pm_data active_pm_data = new active_pm_data();
                            active_pm_data.asset_pm_id = assetPMs.asset_pm_id;
                            active_pm_data.woline_status = requestModel.in_active_pm.Where(x=>x.pm_id == pm.pm_id).FirstOrDefault().woline_status;
                            active_pm_data.pm_form_output_data = requestModel.in_active_pm.Where(x=>x.pm_id == pm.pm_id).FirstOrDefault().pm_form_output_data;

                            AddActiveAssetPMWolineByStepsRequestmodel.pm_list.Add(active_pm_data);
                        }

                    }
                }
                 await AddActiveAssetPMWolineBySteps(AddActiveAssetPMWolineByStepsRequestmodel);

            }
            catch (Exception e)
            {
            }
            return response;
        }

        public ListViewModel<PMLastCompletedDateReportResponsemodel> PMLastCompletedDateReport(PMLastCompletedDateReportRequestmodel requestmodel)
        {
            ListViewModel<PMLastCompletedDateReportResponsemodel> response = new ListViewModel<PMLastCompletedDateReportResponsemodel>();
            List<PMLastCompletedDateReportResponsemodel> pm_list = new List<PMLastCompletedDateReportResponsemodel>();
            var get_assets = _UoW.AssetPMsRepository.GetAllAssetForPMReport(requestmodel.site_id);

            foreach (var item in get_assets)
            {
                if (item.AssetPMs != null && item.AssetPMs.Count > 0)
                {
                    string toplevel_asset_name = null;
                    if (item.component_level_type_id == (int)ComponentLevelTypes.SublevelComponent)
                    {
                        if (item.AssetTopLevelcomponentMapping != null)
                        {
                            var toplevelcomponent_asset_id = item.AssetTopLevelcomponentMapping.Where(x => !x.is_deleted).Select(x => x.toplevelcomponent_asset_id).FirstOrDefault();
                            toplevel_asset_name = _UoW.AssetRepository.GetAssetNameByAssetId(toplevelcomponent_asset_id);
                        }
                    }

                    foreach (var pm in item.AssetPMs)
                    {
                        // Calculate Due Date for PM
                        var PM_due_date = DateTime.UtcNow;
                        string pm_interval = null;
                        if (pm.datetime_starting_at != null)
                        {
                            // decide condition type 
                            var max_condition = GetConditionTypeForPM(pm.Asset);
                            var pm_condition_trigger = pm.AssetPMsTriggerConditionMapping.Where(x => x.condition_type_id == max_condition).FirstOrDefault();
                            if (pm.status != (int)Status.Completed)
                            {
                                if (pm_condition_trigger != null && pm_condition_trigger.datetime_repeates_every != null && pm_condition_trigger.datetime_repeat_time_period_type != null)
                                {
                                    // get PM end date 
                                    var PM_end_date = CalculatePMEndDate(pm.datetime_starting_at.Value, pm_condition_trigger.datetime_repeates_every.Value, pm_condition_trigger.datetime_repeat_time_period_type.Value);
                                    PM_due_date = PM_end_date.AddDays(-7);
                                }
                            }
                            if (pm_condition_trigger != null)
                            {
                                pm_interval = pm_condition_trigger.datetime_repeates_every.ToString() + " " + pm_condition_trigger.PMDateTimeRepeatTypeStatus.status_name;
                            }
                        }

                        PMLastCompletedDateReportResponsemodel PMLastCompletedDateReportResponsemodel = new PMLastCompletedDateReportResponsemodel();
                        if (!pm.is_archive && pm.status != (int)Status.Completed) // for all pms which are not completed and active
                        {
                            PMLastCompletedDateReportResponsemodel.asset_name = item.name;
                            PMLastCompletedDateReportResponsemodel.asset_class_code = item.InspectionTemplateAssetClass.asset_class_code;
                            PMLastCompletedDateReportResponsemodel.asset_class_name = item.InspectionTemplateAssetClass.asset_class_name;
                            PMLastCompletedDateReportResponsemodel.asset_id = item.asset_id;
                            PMLastCompletedDateReportResponsemodel.internal_asset_id = item.internal_asset_id;
                            PMLastCompletedDateReportResponsemodel.pm_title = pm.title;
                            PMLastCompletedDateReportResponsemodel.pm_plan_title = pm.AssetPMPlans.plan_name;
                            PMLastCompletedDateReportResponsemodel.asset_pm_id = pm.asset_pm_id;
                            PMLastCompletedDateReportResponsemodel.pm_last_completed_date = pm.asset_pm_completed_date != null ? pm.asset_pm_completed_date : pm.datetime_starting_at;
                            PMLastCompletedDateReportResponsemodel.building = item.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name;
                            PMLastCompletedDateReportResponsemodel.floor = item.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name;
                            PMLastCompletedDateReportResponsemodel.room = item.AssetFormIOBuildingMappings.FormIORooms.formio_room_name;
                            PMLastCompletedDateReportResponsemodel.section = item.AssetFormIOBuildingMappings.FormIOSections.formio_section_name;
                            PMLastCompletedDateReportResponsemodel.due_date = PM_due_date;
                            PMLastCompletedDateReportResponsemodel.pm_interval_frequency = pm_interval;
                            PMLastCompletedDateReportResponsemodel.top_level_asset_name = toplevel_asset_name;
                            PMLastCompletedDateReportResponsemodel.is_assetpm_enabled = pm.is_assetpm_enabled;

                            pm_list.Add(PMLastCompletedDateReportResponsemodel);

                        }
                        else if (!pm.is_archive && pm.status == (int)Status.Completed && pm.pm_trigger_type == (int)Status.FixedOneTime) // if pm is completed then take only fixed-one-time pm 
                        {
                            PMLastCompletedDateReportResponsemodel.asset_name = item.name;
                            PMLastCompletedDateReportResponsemodel.asset_class_code = item.InspectionTemplateAssetClass.asset_class_code;
                            PMLastCompletedDateReportResponsemodel.asset_class_name = item.InspectionTemplateAssetClass.asset_class_name;
                            PMLastCompletedDateReportResponsemodel.asset_id = item.asset_id;
                            PMLastCompletedDateReportResponsemodel.internal_asset_id = item.internal_asset_id;
                            PMLastCompletedDateReportResponsemodel.pm_title = pm.title;
                            PMLastCompletedDateReportResponsemodel.pm_plan_title = pm.AssetPMPlans.plan_name;
                            PMLastCompletedDateReportResponsemodel.asset_pm_id = pm.asset_pm_id;
                            PMLastCompletedDateReportResponsemodel.pm_last_completed_date = pm.asset_pm_completed_date != null ? pm.asset_pm_completed_date : pm.datetime_starting_at;

                            PMLastCompletedDateReportResponsemodel.building = item.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name;
                            PMLastCompletedDateReportResponsemodel.floor = item.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name;
                            PMLastCompletedDateReportResponsemodel.room = item.AssetFormIOBuildingMappings.FormIORooms.formio_room_name;
                            PMLastCompletedDateReportResponsemodel.section = item.AssetFormIOBuildingMappings.FormIOSections.formio_section_name;
                            PMLastCompletedDateReportResponsemodel.due_date = PM_due_date;
                            PMLastCompletedDateReportResponsemodel.pm_interval_frequency = pm_interval;
                            PMLastCompletedDateReportResponsemodel.top_level_asset_name = toplevel_asset_name;
                            PMLastCompletedDateReportResponsemodel.is_assetpm_enabled = pm.is_assetpm_enabled;

                            pm_list.Add(PMLastCompletedDateReportResponsemodel);
                        }
                    }

                    response.list = pm_list;
                    response.listsize = pm_list.Count();
                }

            }
            return response;
        }

        public async Task<int> BulkCreatePMWOline(BulkCreatePMWOlineRequestmodel requestmodel)
        {
            int response = (int)ResponseStatusNumber.Error;
            var asset_pms_ids = requestmodel.pm_list.Select(x => x.asset_pm_id).ToList();

            // check if any requested asset pm status is other than open/active or not if it is then return error
            var is_anypm_already_added = _UoW.AssetPMsRepository.IsAnyPMAlreadyLinked(requestmodel.pm_list.Select(x => x.asset_pm_id).ToList());

            //check for if any AssetPM is from other Site
            var isAnyPMfromOtherSite = _UoW.AssetPMsRepository.CheckIfAssetPMIsFromOtherSite(asset_pms_ids);

            if (is_anypm_already_added.Count() > 0)
            {
                response = (int)ResponseStatusNumber.AlreadyExists;
                return response;
            }
            else if (isAnyPMfromOtherSite)  // if any pm is from other site then return error
            {
                response = (int)ResponseStatusNumber.NotExists;
                return response;
            }
            else
            {
                var open_asset_pm_ids = _UoW.AssetPMsRepository.ReturnOnlyOpenAssetPMIds(asset_pms_ids);

                AddAssetPMWolineRequestmodel AddAssetPMWolineRequestmodel = new AddAssetPMWolineRequestmodel();
                AddAssetPMWolineRequestmodel.wo_id = requestmodel.wo_id;
                AddAssetPMWolineRequestmodel.asset_pm_id = open_asset_pm_ids;

                WorkOrderService WOService = new WorkOrderService(_mapper);
                var insert_woline = await WOService.AddAssetPMWoline(AddAssetPMWolineRequestmodel);
                if (insert_woline.Count() > 0)
                {
                    response = (int)ResponseStatusNumber.Success;
                }
            }
            return response;
        }

        public async Task<int> BulkUpdatePMLastcompleted(BulkupdatePMLastcompletedRequestModel requestmodel)
        {
            int response = (int)ResponseStatusNumber.Error;
            
            try
            {
                if (requestmodel.asset_pm_id_with_last_completed_date_list != null && requestmodel.asset_pm_id_with_last_completed_date_list.Count > 0)
                {
                    var asset_pm_id_list = requestmodel.asset_pm_id_with_last_completed_date_list.Select(x => x.asset_pm_id).ToList();

                    //check for if any AssetPM is from other Site
                    var isAnyPMfromOtherSite = _UoW.AssetPMsRepository.CheckIfAssetPMIsFromOtherSite(asset_pm_id_list);
                    List<Guid> asset_ids = new List<Guid>();
                    if (isAnyPMfromOtherSite) // if any pm is from other site then return error
                    {
                        response = (int)ResponseStatusNumber.NotExists;
                        return response;
                    }
                    else
                    {
                        BulkUpdatePMs(requestmodel);
                        response = (int)ResponseStatusNumber.Success;
                    }
                    
                }

            }
            catch (Exception ex)
            {
            }
            return response;
        }
        public async Task<int> BulkUpdatePMs(BulkupdatePMLastcompletedRequestModel requestmodel)
        {
            int response = (int)ResponseStatusNumber.Error;
            try
            {
                List<Guid> asset_ids = new List<Guid>();
                var request_by = _UoW.AssetPMsRepository.GetRequestedUser(UpdatedGenericRequestmodel.CurrentUser.requested_by);
                var site_name = _UoW.AssetPMsRepository.GetRequestedSite(Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
                foreach (var this_asset_pm_id in requestmodel.asset_pm_id_with_last_completed_date_list)
                {
                    var get_asset_pm = await _UoW.AssetPMsRepository.GetAssetPMById(this_asset_pm_id.asset_pm_id);

                    if (get_asset_pm != null)
                    {
                        get_asset_pm.datetime_starting_at = this_asset_pm_id.last_completed_date;
                        get_asset_pm.asset_pm_completed_date = this_asset_pm_id.last_completed_date;
                        get_asset_pm.is_assetpm_enabled = this_asset_pm_id.is_assetpm_enabled;

                        get_asset_pm.modified_at = DateTime.UtcNow;
                        get_asset_pm.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        var update_asset_pm = await _UoW.BaseGenericRepository<AssetPMs>().Update(get_asset_pm);

                        if (update_asset_pm == true)
                        {
                            _UoW.SaveChanges();
                            response = (int)ResponseStatusNumber.Success;
                        }
                        asset_ids.Add(get_asset_pm.asset_id);
                    }
                }
                asset_ids = asset_ids.Distinct().ToList();
                // For Updating AssetPMs Due_Date, Due_flag and Due_In based on Asset cricality condition
                foreach (var item in asset_ids)
                {
                    if (item != null && item != Guid.Empty)
                        await UpdateDueDateDueInDueFlagForAssetPMsByAssetId(item);
                }
                // send email to user when bulk upload successfully uploads 
                BulkUpdateEmailRequest BulkUpdateEmailRequest = new BulkUpdateEmailRequest();
                BulkUpdateEmailRequest.updated_pms_count = requestmodel.asset_pm_id_with_last_completed_date_list.Count().ToString();
                BulkUpdateEmailRequest.company_logo = request_by.Active_Company.company_logo;
                BulkUpdateEmailRequest.site_name = site_name;
                await SendEmail.SendGridEmailWithTemplate(request_by.email, "Bulk PM Update Status", BulkUpdateEmailRequest, ConfigurationManager.AppSettings["bulk_update_pms_email_template_id"]);

            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public class BulkUpdateEmailRequest
        {
            public string company_logo { get; set; }
            public string updated_pms_count { get; set; }
            public string site_name { get; set; }
        }

        public async Task<AddPMtoNewWolineResponsemodel> AddPMtoTempAsset(AddPMtoTempAssetRequestmodel requst)
        {
            AddPMtoNewWolineResponsemodel response = new AddPMtoNewWolineResponsemodel();
            response.WolinePMlist = new List<WOlinePMList>();
            var get_main_asset_woline = _UoW.WorkOrderRepository.GetMainAssetWOlineforPM(requst.woonboardingassets_id);
            // get woline by id 
            foreach (var pm in requst.pm_list)
            {   
                WOlinePMList WOlinePMList = new WOlinePMList();

                TempAssetPMs TempAssetPMs = new TempAssetPMs();
                TempAssetPMs.pm_id = pm.pm_id;
                TempAssetPMs.woonboardingassets_id = requst.woonboardingassets_id; // this will act as main temp asset for pm  
                TempAssetPMs.status = (int)Status.Active;
                TempAssetPMs.created_at = DateTime.UtcNow;
                TempAssetPMs.is_archive = false;

                var insert = await _UoW.BaseGenericRepository<TempAssetPMs>().Insert(TempAssetPMs);
                _UoW.SaveChanges();

                WOOnboardingAssets WOOnboardingAssets = new WOOnboardingAssets();
                WOOnboardingAssets.asset_name = get_main_asset_woline.asset_name;
                WOOnboardingAssets.asset_class_code = get_main_asset_woline.asset_class_code;
                WOOnboardingAssets.asset_class_name = get_main_asset_woline.asset_class_name;
                WOOnboardingAssets.QR_code = get_main_asset_woline.QR_code;
                WOOnboardingAssets.created_at = DateTime.UtcNow;
                WOOnboardingAssets.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                WOOnboardingAssets.is_deleted = false;
                WOOnboardingAssets.status = (int)Status.open;
                WOOnboardingAssets.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                WOOnboardingAssets.wo_id = requst.wo_id;
                WOOnboardingAssets.inspection_type = (int)MWO_inspection_wo_type.PM;
                // WOOnboardingAssets.asset_id = asset_pm.asset_id;
                // WOOnboardingAssets.is_wo_line_for_exisiting_asset = true;
                WOOnboardingAssets.building = get_main_asset_woline.TempAsset.TempFormIOBuildings.temp_formio_building_name;
                WOOnboardingAssets.floor = get_main_asset_woline.TempAsset.TempFormIOFloors.temp_formio_floor_name;
                WOOnboardingAssets.room = get_main_asset_woline.TempAsset.TempFormIORooms.temp_formio_room_name;
                WOOnboardingAssets.section = get_main_asset_woline.TempAsset.TempFormIOSections.temp_formio_section_name;
                WOOnboardingAssets.tempasset_id = requst.install_woline.tempasset_id;

                var insert_pm_woline = await _UoW.BaseGenericRepository<WOOnboardingAssets>().Insert(WOOnboardingAssets);
                _UoW.SaveChanges();

                // update assset pm 
                // asset_pm.woonboardingassets_id = WOOnboardingAssets.woonboardingassets_id;
                // asset_pm.wo_id = WOOnboardingAssets.wo_id;
                //  asset_pm.status = (int)Status.Schedule;
                //  asset_pm.modified_at = DateTime.UtcNow;

                TempActiveAssetPMWOlineMapping TempActiveAssetPMWOlineMapping = new TempActiveAssetPMWOlineMapping();
                TempActiveAssetPMWOlineMapping.woonboardingassets_id = WOOnboardingAssets.woonboardingassets_id;
                TempActiveAssetPMWOlineMapping.temp_asset_pm_id = TempAssetPMs.temp_asset_pm_id;
                TempActiveAssetPMWOlineMapping.is_active = true;
                TempActiveAssetPMWOlineMapping.is_deleted = false;
                TempActiveAssetPMWOlineMapping.created_at = DateTime.UtcNow;
                TempActiveAssetPMWOlineMapping.pm_form_output_data = pm.pm_form_output_data;

                var insert_temp_pm_active_data = await _UoW.BaseGenericRepository<TempActiveAssetPMWOlineMapping>().Insert(TempActiveAssetPMWOlineMapping);
                _UoW.SaveChanges();


                // create issue from outout json
                WorkOrderService woservice = new WorkOrderService(_mapper);
                SubmitPMFormJsonRequestmodel SubmitPMFormJsonRequestmodel = new SubmitPMFormJsonRequestmodel();
                SubmitPMFormJsonRequestmodel.temp_asset_pm_id = TempAssetPMs.temp_asset_pm_id;
                SubmitPMFormJsonRequestmodel.pm_form_output_data = pm.pm_form_output_data;
                SubmitPMFormJsonRequestmodel.woonboardingassets_id = WOOnboardingAssets.woonboardingassets_id;
                await woservice.SubmitTempPMFormJson(SubmitPMFormJsonRequestmodel);


                WOlinePMList.temp_asset_pm_id = TempAssetPMs.temp_asset_pm_id;
                WOlinePMList.woonboardingassets_id = WOOnboardingAssets.woonboardingassets_id;

                // get master pm
                var getpm = _UoW.WorkOrderRepository.GetpmPmid(pm.pm_id);
                WOlinePMList.pm_inspection_type_id = getpm.pm_inspection_type_id;

                response.WolinePMlist.Add(WOlinePMList);
            }
            return response;
        }

        public GetPMWOlineByIDStepsResponsemodel GetPMWOlineByIDSteps(GetPMWOlineByIDStepsRequestmodel requestmodel)
        {
            GetPMWOlineByIDStepsResponsemodel response = new GetPMWOlineByIDStepsResponsemodel();
            var woline_by_id = _UoW.AssetPMsRepository.GetPMWOlineById(requestmodel.woonboardingassets_id);

            // get install woline from temp asset woline table
            var get_install_woline = woline_by_id.TempAsset.WOOnboardingAssets.Where(x => x.inspection_type == (int)MWO_inspection_wo_type.OnBoarding && !x.is_deleted).FirstOrDefault();

            // get install woline details for reponse 
            WorkOrderService woservice = new WorkOrderService(_mapper);
            response.installl_woline = woservice.GetOBWOAssetDetailsById(get_install_woline.woonboardingassets_id.ToString());


            /// get other pm wolines for pm list 
            /// 
            var total_pm_wolines  = woline_by_id.TempAsset.WOOnboardingAssets.Where(x=>x.inspection_type == (int)MWO_inspection_wo_type.PM && !x.is_deleted).ToList();

            response.linked_active_pm_list = new List<WOActivePMDetails>();
            response.linked_temp_active_pm_list = new List<WOTempPMDetails>();

            // get active pm woline 
            var active_assetpm_wolines = total_pm_wolines.Where(x => x.ActiveAssetPMWOlineMapping != null && !x.ActiveAssetPMWOlineMapping.AssetPMs.is_pm_inspection_manual).ToList();
            foreach(var item in active_assetpm_wolines)
            {
                
                // get woline details 
                var get_pm_woline = _UoW.AssetPMsRepository.GetPMWOlineForDetails(item.woonboardingassets_id);

                WOActivePMDetails WOActivePMDetails = new WOActivePMDetails();
                WOActivePMDetails.asset_pm_id = get_pm_woline.ActiveAssetPMWOlineMapping.AssetPMs.asset_pm_id;
                WOActivePMDetails.title = get_pm_woline.ActiveAssetPMWOlineMapping.AssetPMs.title;
                WOActivePMDetails.pm_id = get_pm_woline.ActiveAssetPMWOlineMapping.AssetPMs.pm_id.Value;
                WOActivePMDetails.description = get_pm_woline.ActiveAssetPMWOlineMapping.AssetPMs.description;
                WOActivePMDetails.pm_plan_name = get_pm_woline.ActiveAssetPMWOlineMapping.AssetPMs.AssetPMPlans.plan_name;
                WOActivePMDetails.woonboardingassets_id = get_pm_woline.woonboardingassets_id;

                response.linked_active_pm_list.Add(WOActivePMDetails);
            }

            // get inactive asset pm woline 
            var inactive_assetpm_wolines = total_pm_wolines.Where(x => x.ActiveAssetPMWOlineMapping != null && x.ActiveAssetPMWOlineMapping.AssetPMs.is_pm_inspection_manual).ToList();
            foreach(var get_pm_woline in inactive_assetpm_wolines)
            {
                WOTempPMDetails WOTempPMDetails = new WOTempPMDetails();
                WOTempPMDetails.asset_pm_id = get_pm_woline.ActiveAssetPMWOlineMapping.AssetPMs.asset_pm_id;
                WOTempPMDetails.title = get_pm_woline.ActiveAssetPMWOlineMapping.AssetPMs.title;
                WOTempPMDetails.pm_id = get_pm_woline.ActiveAssetPMWOlineMapping.AssetPMs.pm_id.Value;
                WOTempPMDetails.description = get_pm_woline.ActiveAssetPMWOlineMapping.AssetPMs.description;
                WOTempPMDetails.pm_plan_name = get_pm_woline.ActiveAssetPMWOlineMapping.AssetPMs.AssetPMPlans.plan_name;
                WOTempPMDetails.woonboardingassets_id = get_pm_woline.woonboardingassets_id;
                response.linked_temp_active_pm_list.Add(WOTempPMDetails);
            }

            // get temp pm for temp asset
            var active_tepassetpm_wolines = total_pm_wolines.Where(x => x.TempActiveAssetPMWOlineMapping != null).ToList();
            foreach (var item in active_tepassetpm_wolines)
            {

                // get woline details 
                var get_pm_woline = _UoW.AssetPMsRepository.GetPMWOlineForDetails(item.woonboardingassets_id);

                WOActivePMDetails WOActivePMDetails = new WOActivePMDetails();
                WOActivePMDetails.asset_pm_id = get_pm_woline.TempActiveAssetPMWOlineMapping.TempAssetPMs.temp_asset_pm_id;
                WOActivePMDetails.title = get_pm_woline.TempActiveAssetPMWOlineMapping.TempAssetPMs.PMs.title;
                WOActivePMDetails.pm_id = get_pm_woline.TempActiveAssetPMWOlineMapping.TempAssetPMs.pm_id.Value;
                WOActivePMDetails.description = get_pm_woline.TempActiveAssetPMWOlineMapping.TempAssetPMs.PMs.description;
                WOActivePMDetails.pm_plan_name = get_pm_woline.TempActiveAssetPMWOlineMapping.TempAssetPMs.PMs.PMPlans.plan_name;
                WOActivePMDetails.woonboardingassets_id = get_pm_woline.woonboardingassets_id;

                response.linked_active_pm_list.Add(WOActivePMDetails);
            }


            return response;
        }


        public async Task<int> EnableDisableAssetPMsStatus(EnableDisableAssetPMsStatusRequestModel requestModel)
        {
            int res = (int)ResponseStatusNumber.Error;
            try
            {
                var get_asset_pm = _UoW.WorkOrderRepository.GetAssetpmByidForInspectionlist(requestModel.asset_pm_id);
                if (get_asset_pm != null)
                {
                    get_asset_pm.is_assetpm_enabled = requestModel.is_assetpm_enabled;
                    get_asset_pm.modified_at = DateTime.UtcNow;
                    get_asset_pm.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    var Update = await _UoW.BaseGenericRepository<AssetPMs>().Update(get_asset_pm);
                    if (Update)
                    {
                        res = (int)ResponseStatusNumber.Success;
                        _UoW.SaveChanges();
                    }
                }

            }catch(Exception e)
            {
            }
            return res;
        }

        public async Task<int> AssignPMtoCeccoSiteScript()
        {
            var get_assets = _UoW.AssetPMsRepository.GetceccoAssets();
            int i = 0; int j = 0;
            foreach (var item in get_assets)
            {
                try
                {
                    i++;
                    if (item.inspectiontemplate_asset_class_id != null)
                    {
                        var get_pm = _UoW.AssetPMsRepository.Gepmdetailforceccoscript(item.inspectiontemplate_asset_class_id.Value);
                        if (get_pm != null)
                        {
                            var get_70b_plan = get_pm.PMPlans.Where(x => x.plan_name.Contains("NFPA-70B") && x.status == (int)Status.Active).FirstOrDefault();
                            if (get_70b_plan != null)
                            {
                                j++;
                                AssignPMToAsset AssignPMToAsset = new AssignPMToAsset();
                                AssignPMToAsset.asset_id = item.asset_id;
                                AssignPMToAsset.pm_plan_id = get_70b_plan.pm_plan_id;
                                await AddAssetPM2_forScript(AssignPMToAsset, item.site_id);

                                await UpdateDueDateDueInDueFlagForAssetPMsByAssetId(item.asset_id);

                            }
                        }

                    }

                }
                catch (Exception e)
                {
                    _logger.LogInformation("Assign NFPA-70B AssetPM to Assets Error in:" + "ExceptionMessage=" + e.Message + "--" + e.InnerException.Message + "----AssetName=" + item.name + " SiteId= " + item.site_id + "--" + item.company_id);
                    return -1;
                }

            }
            int z = i + j;
            _logger.LogInformation("Assign NFPA-70B AssetPM Script Completed Successfully!!");
            return 1;
        }
        public async Task<PMPlansResponseModel> AddAssetPM2_forScript(AssignPMToAsset AssignPM, Guid site_id)
        {
            PMPlansResponseModel pmPlanResponse = new PMPlansResponseModel();
            int result = (int)ResponseStatusNumber.Error;

            try
            {
                var pmPlan = await _UoW.PMPlansRepository.GetPMPlanByIdToAddinAsset(AssignPM.pm_plan_id);
                if (pmPlan != null)
                {
                    AssetPMPlans assetPMPlan = new AssetPMPlans();
                    assetPMPlan = _mapper.Map<AssetPMPlans>(pmPlan);
                    assetPMPlan.asset_id = AssignPM.asset_id;
                    assetPMPlan.created_at = DateTime.UtcNow;
                    assetPMPlan.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    result = await _UoW.AssetPMPlansRepository.Insert(assetPMPlan);
                    if (result > 0)
                    {
                        pmPlan.PMs = pmPlan.PMs.ToList().Where(x => !x.is_archive).ToList();
                        /*pmPlan.PMs.ToList().ForEach(x =>
                        {
                            x.PMTasks = x.PMTasks.Where(y => !y.is_archive).ToList();
                        });*/
                        pmPlan.PMs.ToList().ForEach(x =>
                        {
                            x.PMAttachments = x.PMAttachments.Where(y => !y.is_archive).ToList();
                        });
                        assetPMPlan.AssetPMs = _mapper.Map<List<AssetPMs>>(pmPlan.PMs);
                        assetPMPlan.AssetPMs.ToList().ForEach(x =>
                        {
                            x.asset_pm_plan_id = assetPMPlan.asset_pm_plan_id;
                            x.asset_id = AssignPM.asset_id;
                            x.created_at = DateTime.UtcNow;
                            x.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            x.status = (int)Status.open;
                            x.datetime_starting_at = DateTime.UtcNow;
                            x.asset_pm_completed_date = x.datetime_starting_at;

                            x.AssetPMAttachments.ToList().ForEach(y =>
                            {
                                y.asset_pm_plan_id = assetPMPlan.asset_pm_plan_id;
                                y.asset_pm_id = x.asset_pm_id;
                                y.asset_id = AssignPM.asset_id;
                                y.created_at = DateTime.UtcNow;
                                y.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            });
                            x.AssetPMsTriggerConditionMapping.ToList().ForEach(y =>
                            {
                                y.asset_pm_id = x.asset_pm_id;
                                y.created_at = DateTime.UtcNow;
                                y.site_id = site_id;//Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                                y.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            });
                        });
                        result = await _UoW.AssetPMsRepository.InsertList(assetPMPlan.AssetPMs);
                        if (result > 0)
                        {
                            _UoW.SaveChanges();
                            pmPlanResponse.response_status = result;

                        }
                        else
                        {

                        }
                    }
                    else
                    {

                        pmPlanResponse.response_status = result;
                    }
                }
                else
                {
                    pmPlanResponse.response_status = (int)ResponseStatusNumber.NotFound;
                }

            }
            catch (Exception e)
            {
                _logger.LogError("AssetAddPM-" + AssignPM.asset_id + "-" + e.Message);
                pmPlanResponse.response_status = (int)ResponseStatusNumber.Error;
            }

            return pmPlanResponse;
        }
        public async Task<int> BulkCreateIRAssetPMsAssetInIRWO(BulkCreatePMWOlineRequestmodel requestmodel)
        {
            WorkOrderService workOrderService = new WorkOrderService(_mapper);
            int res = (int)ResponseStatusNumber.Error;
            try
            {
                if (requestmodel.pm_list != null && requestmodel.pm_list.Count > 0)
                {
                    var asset_pm_id_list = requestmodel.pm_list.Select(x => x.asset_pm_id).ToList();

                    //check for if any AssetPM is from other Site
                    var isAnyPMfromOtherSite = _UoW.AssetPMsRepository.CheckIfAssetPMIsFromOtherSite(asset_pm_id_list);
                    if (isAnyPMfromOtherSite)
                    {
                        res = (int)ResponseStatusNumber.NotExists;
                        return res;
                    }
                    else
                    {       // return AssetIds of AssetPMs which are in Open/Active status
                        var openPMs_asset_id_list = _UoW.AssetPMsRepository.ReturnAllOpenAssetPMsAssetIds(asset_pm_id_list);

                        // For Adding Existing Assets in IR WO
                        if (openPMs_asset_id_list != null && openPMs_asset_id_list.Count > 0)
                        {
                            AssignExistingAssettoOBWORequestmodel assignExistingAssettoOBWORequestmodel = new AssignExistingAssettoOBWORequestmodel();
                            assignExistingAssettoOBWORequestmodel.asset_id = openPMs_asset_id_list;
                            assignExistingAssettoOBWORequestmodel.wo_id = requestmodel.wo_id;

                            var response = await workOrderService.AssignExistingAssettoOBWO(assignExistingAssettoOBWORequestmodel);
                            if (response == (int)ResponseStatusNumber.Success)
                            {
                                res = (int)ResponseStatusNumber.Success;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return res;
        }

        public async Task<int> UpdateWorkStartDateOnEditOpenWOLine(Guid woonboardingassets_id)
        {
            WorkOrderService woservice = new WorkOrderService(_mapper);
            int response = (int)ResponseStatusNumber.Error;
            try
            {
                response = await woservice.AddUpdateWOOnboardingAssetsDateTimeTracking(woonboardingassets_id,(int)WOLineActionsTypes.WorkStartDate,0);
            }
            catch (Exception e)
            {
            }
            return response;
        }


        public async Task<int> ScriptToAdd1YearAssetPMs()
        {
            try
            {
                /*
                var get_assets = _UoW.AssetPMsRepository.GetAssetsListPMScript();
                int i = 0;
                foreach(var asset_id in  get_assets)
                {
                    try
                    {
                        i++;
                        var get_assetpms = _UoW.AssetPMsRepository.GetCurrentAssetPMsByAssetId_2(asset_id);

                        foreach (var pmDetails in get_assetpms)
                        {
                            try
                            {
                                int count_should_be = await CalculateActualAssetPMsCountByFrequency(pmDetails);
                                var get_pm_count = _UoW.AssetPMsRepository.GetAssetPMsCountByAssetIdPMId(pmDetails.asset_id, pmDetails.pm_id.Value);
                                // Remove AssetPMs due to frequency change
                                if (get_pm_count > count_should_be)
                                {
                                    int remove_count = get_pm_count - count_should_be;
                                    await AddRemoveAssetPMsByFrequency(false, remove_count, pmDetails);

                                    await UpdateStartDateOfAllDuplicateAssetPMs(pmDetails);
                                }
                                // Add new AssetPMs due to frequency change
                                else if (get_pm_count < count_should_be)
                                {
                                    int add_count = count_should_be - get_pm_count;
                                    await AddRemoveAssetPMsByFrequency(true, add_count, pmDetails);

                                    await UpdateStartDateOfAllDuplicateAssetPMs(pmDetails);
                                }
                                // Update Start Date for other Duplicate AssetPMs
                                //await UpdateStartDateOfAllDuplicateAssetPMs(pmDetails);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError("error inside foreach pmDetails asset_pm_id : " + pmDetails.asset_pm_id+"/" + e.Message);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("error inside foreach asset_id  : " + asset_id + "/" + e.Message);
                    }
                }
                i = i + 1;
                */
                

                var get_all_assetpms_list = _UoW.AssetPMsRepository.GetAllAssetPMsToAdd1YrPMs();
                
                foreach(var asset_pm in get_all_assetpms_list)
                {
                    int count = 0;

                    DateTime? datetime_starting_at = _UoW.AssetPMsRepository.GetLastAssetPMStartDateByAssetIdPMId(asset_pm.asset_id, asset_pm.pm_id.Value);
                    var max_condition = GetConditionTypeForPM(asset_pm.Asset);
                    var pm_condition_trigger = asset_pm.AssetPMsTriggerConditionMapping.Where(x => x.condition_type_id == max_condition).FirstOrDefault();
                    int? frequency = pm_condition_trigger.datetime_repeates_every.Value;
                    int? frequency_type = pm_condition_trigger.datetime_repeat_time_period_type.Value;

                    if (pm_condition_trigger != null && frequency != null && frequency > 0 && frequency_type != null && frequency_type > 0)
                    {
                        if (frequency_type == (int)Status.Month)
                            count = 12 / frequency.Value;
                        else if (frequency_type == (int)Status.Year)
                            count = 1 / frequency.Value;
                    }

                    for (int i = 0; i < count; i++)
                    {
                        AssetPMs assetPMs_ = new AssetPMs();
                        assetPMs_ = _mapper.Map<AssetPMs>(asset_pm);

                        if (frequency_type == (int)Status.Month)
                            datetime_starting_at = datetime_starting_at.Value.AddMonths(frequency.Value);
                        else if (frequency_type == (int)Status.Year)
                            datetime_starting_at = datetime_starting_at.Value.AddYears(frequency.Value);

                        assetPMs_.datetime_starting_at = datetime_starting_at;
                        assetPMs_.asset_pm_completed_date = datetime_starting_at;
                        assetPMs_.created_at = DateTime.UtcNow;
                        assetPMs_.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        assetPMs_.AssetPMAttachments.ToList().ForEach(y =>
                        {
                            y.asset_pm_plan_id = assetPMs_.asset_pm_plan_id;
                            y.asset_id = assetPMs_.asset_id;
                            y.created_at = DateTime.UtcNow;
                            y.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        });
                        assetPMs_.AssetPMsTriggerConditionMapping.ToList().ForEach(y =>
                        {
                            y.created_at = DateTime.UtcNow;
                            y.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                            y.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        });

                        var insert = await _UoW.BaseGenericRepository<AssetPMs>().Insert(assetPMs_);
                    }
                    _UoW.SaveChanges();
                }

            }catch(Exception e)
            {
            }
            return 1;
        }

        public async Task<int> AddRemoveAssetPMsOnAssetCondition(Guid asset_id)
        {
            try
            {
                var get_assetpms = _UoW.AssetPMsRepository.GetCurrentAssetPMsByAssetId_2(asset_id);

                foreach (var pmDetails in get_assetpms)
                {
                    try
                    {
                        int count_should_be = await CalculateActualAssetPMsCountByFrequency(pmDetails);
                        var get_pm_count = _UoW.AssetPMsRepository.GetAssetPMsCountByAssetIdPMId(pmDetails.asset_id, pmDetails.pm_id.Value);
                        // Remove AssetPMs due to frequency change
                        if (get_pm_count > count_should_be)
                        {
                            int remove_count = get_pm_count - count_should_be;
                            await AddRemoveAssetPMsByFrequency(false, remove_count, pmDetails);
                        }
                        // Add new AssetPMs due to frequency change
                        else if (get_pm_count < count_should_be)
                        {
                            int add_count = count_should_be - get_pm_count;
                            await AddRemoveAssetPMsByFrequency(true, add_count, pmDetails);
                        }
                        // Update Start Date for other Duplicate AssetPMs
                        await UpdateStartDateOfAllDuplicateAssetPMs(pmDetails);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("error inside foreach pmDetails asset_pm_id : " + pmDetails.asset_pm_id + "/" + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
            }
            return 1;
        }

        public class pmlanjsonobj
        {
            public List<ATSW> ATSW { get; set;   }
            public List<CITR> CITR { get; set;   }
            public List<DFTR> DFTR { get; set;   }
            public List<DFTRL> DFTRL { get; set;   }
            public List<DISCL> DISCL { get; set;   }
            public List<DISCLF> DISCLF { get; set;   }
            public List<DPNL> DPNL { get; set;   }
            public List<GENR> GENR { get; set;   }
            public List<LAAR> LAAR { get; set;   }
            public List<LVCB> LVCB { get; set;   }
            public List<MCCB> MCCB { get; set;   }
            public List<MCCBL> MCCBL { get; set;   }
            public List<MMME> MMME { get; set;   }
            public List<MTSW> MTSW { get; set;   }
            public List<PANL> PANL { get; set;   }
            public List<PITR> PITR { get; set;   }
        }

      
            // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
            public class ATSW
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Fed-By Asset (2)")]
                public string FedByAsset2 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }
            }

            public class CITR
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }
            }

            public class DFTR
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }

                [JsonProperty("nameplateInformation.manufacturer")]
                public string nameplateInformationmanufacturer { get; set; }

                [JsonProperty("nameplateInformation.phase")]
                public int nameplateInformationphase { get; set; }

                [JsonProperty("nameplateInformation.model")]
                public string nameplateInformationmodel { get; set; }

                [JsonProperty("nameplateInformation.primaryVector")]
                public string nameplateInformationprimaryVector { get; set; }

                [JsonProperty("nameplateInformation.secondaryVector")]
                public string nameplateInformationsecondaryVector { get; set; }

                [JsonProperty("nameplateInformation.percentImpedance")]
                public double nameplateInformationpercentImpedance { get; set; }

                [JsonProperty("nameplateInformation.kva")]
                public int nameplateInformationkva { get; set; }

                [JsonProperty("nameplateInformation.frequency")]
                public int nameplateInformationfrequency { get; set; }

                [JsonProperty("nameplateInformation.primaryVoltage")]
                public int nameplateInformationprimaryVoltage { get; set; }

                [JsonProperty("nameplateInformation.secondaryVoltage")]
                public string nameplateInformationsecondaryVoltage { get; set; }

                [JsonProperty("nameplateInformation.temperatureRise")]
                public int nameplateInformationtemperatureRise { get; set; }

                [JsonProperty("nameplateInformation.class")]
                public string nameplateInformationclass { get; set; }

                [JsonProperty("nameplateInformation.bil")]
                public string nameplateInformationbil { get; set; }

                [JsonProperty("nameplateInformation.primaryAmperes")]
                public double nameplateInformationprimaryAmperes { get; set; }

                [JsonProperty("nameplateInformation.secondaryAmperes")]
                public int nameplateInformationsecondaryAmperes { get; set; }

                [JsonProperty("nameplateInformation.sampleType")]
                public string nameplateInformationsampleType { get; set; }

                [JsonProperty("nameplateInformation.tapPosition1")]
                public object nameplateInformationtapPosition1 { get; set; }

                [JsonProperty("nameplateInformation.tapPosition2")]
                public object nameplateInformationtapPosition2 { get; set; }

                [JsonProperty("nameplateInformation.tapPosition3")]
                public object nameplateInformationtapPosition3 { get; set; }

                [JsonProperty("nameplateInformation.tapPosition4")]
                public object nameplateInformationtapPosition4 { get; set; }

                [JsonProperty("nameplateInformation.tapPosition5")]
                public object nameplateInformationtapPosition5 { get; set; }

                [JsonProperty("nameplateInformation.voltage1")]
                public int nameplateInformationvoltage1 { get; set; }

                [JsonProperty("nameplateInformation.voltage2")]
                public int nameplateInformationvoltage2 { get; set; }

                [JsonProperty("nameplateInformation.voltage3")]
                public int nameplateInformationvoltage3 { get; set; }

                [JsonProperty("nameplateInformation.voltage4")]
                public int nameplateInformationvoltage4 { get; set; }

                [JsonProperty("nameplateInformation.voltage5")]
                public int nameplateInformationvoltage5 { get; set; }

                [JsonProperty("nameplateInformation.type")]
                public string nameplateInformationtype { get; set; }

                [JsonProperty("nameplateInformation.tapPosition6")]
                public int? nameplateInformationtapPosition6 { get; set; }

                [JsonProperty("nameplateInformation.tapPosition7")]
                public int? nameplateInformationtapPosition7 { get; set; }

                [JsonProperty("nameplateInformation.voltage6")]
                public int? nameplateInformationvoltage6 { get; set; }

                [JsonProperty("nameplateInformation.voltage7")]
                public int? nameplateInformationvoltage7 { get; set; }
            }

            public class DFTRL
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }

                [JsonProperty("nameplateInformation.manufacturer")]
                public string nameplateInformationmanufacturer { get; set; }

                [JsonProperty("nameplateInformation.phase")]
                public int nameplateInformationphase { get; set; }

                [JsonProperty("nameplateInformation.model")]
                public string nameplateInformationmodel { get; set; }

                [JsonProperty("nameplateInformation.primaryVector")]
                public string nameplateInformationprimaryVector { get; set; }

                [JsonProperty("nameplateInformation.secondaryVector")]
                public string nameplateInformationsecondaryVector { get; set; }

                [JsonProperty("nameplateInformation.kva")]
                public int nameplateInformationkva { get; set; }

                [JsonProperty("nameplateInformation.frequency")]
                public int nameplateInformationfrequency { get; set; }

                [JsonProperty("nameplateInformation.primaryVoltage")]
                public int nameplateInformationprimaryVoltage { get; set; }

                [JsonProperty("nameplateInformation.secondaryVoltage")]
                public string nameplateInformationsecondaryVoltage { get; set; }

                [JsonProperty("nameplateInformation.temperatureRise")]
                public int nameplateInformationtemperatureRise { get; set; }

                [JsonProperty("nameplateInformation.class")]
                public string nameplateInformationclass { get; set; }

                [JsonProperty("nameplateInformation.bil")]
                public string nameplateInformationbil { get; set; }

                [JsonProperty("nameplateInformation.primaryAmperes")]
                public double nameplateInformationprimaryAmperes { get; set; }

                [JsonProperty("nameplateInformation.secondaryAmperes")]
                public double nameplateInformationsecondaryAmperes { get; set; }

                [JsonProperty("nameplateInformation.sampleType")]
                public string nameplateInformationsampleType { get; set; }

                [JsonProperty("nameplateInformation.tapPosition1")]
                public string nameplateInformationtapPosition1 { get; set; }

                [JsonProperty("nameplateInformation.tapPosition2")]
                public string nameplateInformationtapPosition2 { get; set; }

                [JsonProperty("nameplateInformation.tapPosition3")]
                public string nameplateInformationtapPosition3 { get; set; }

                [JsonProperty("nameplateInformation.tapPosition4")]
                public string nameplateInformationtapPosition4 { get; set; }

                [JsonProperty("nameplateInformation.tapPosition5")]
                public string nameplateInformationtapPosition5 { get; set; }

                [JsonProperty("nameplateInformation.voltage1")]
                public int nameplateInformationvoltage1 { get; set; }

                [JsonProperty("nameplateInformation.voltage2")]
                public int nameplateInformationvoltage2 { get; set; }

                [JsonProperty("nameplateInformation.voltage3")]
                public int nameplateInformationvoltage3 { get; set; }

                [JsonProperty("nameplateInformation.voltage4")]
                public int nameplateInformationvoltage4 { get; set; }

                [JsonProperty("nameplateInformation.voltage5")]
                public int nameplateInformationvoltage5 { get; set; }
            }

            public class DISCL
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }

                [JsonProperty("nameplateInformation.manufacturer")]
                public string nameplateInformationmanufacturer { get; set; }

                [JsonProperty("nameplateInformation.model")]
                public string nameplateInformationmodel { get; set; }

                [JsonProperty("nameplateInformation.voltageRating")]
                public string nameplateInformationvoltageRating { get; set; }

                [JsonProperty("nameplateInformation.ampereRating")]
                public string nameplateInformationampereRating { get; set; }
            }

            public class DISCLF
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }

                [JsonProperty("nameplateInformation.manufacturer")]
                public string nameplateInformationmanufacturer { get; set; }

                [JsonProperty("nameplateInformation.model")]
                public string nameplateInformationmodel { get; set; }

                [JsonProperty("nameplateInformation.voltageRating")]
                public string nameplateInformationvoltageRating { get; set; }

                [JsonProperty("nameplateInformation.ampereRating")]
                public string nameplateInformationampereRating { get; set; }
            }

            public class DPNL
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }

                [JsonProperty("nameplateInformation.manufacturer")]
                public string nameplateInformationmanufacturer { get; set; }

                [JsonProperty("nameplateInformation.mountingType")]
                public string nameplateInformationmountingType { get; set; }

                [JsonProperty("nameplateInformation.enclosureType")]
                public string nameplateInformationenclosureType { get; set; }

                [JsonProperty("nameplateInformation.voltageRating")]
                public string nameplateInformationvoltageRating { get; set; }

                [JsonProperty("nameplateInformation.configuration")]
                public string nameplateInformationconfiguration { get; set; }

                [JsonProperty("nameplateInformation.numberOfPoles")]
                public int? nameplateInformationnumberOfPoles { get; set; }

                [JsonProperty("nameplateInformation.ampereInterruptingCapacity")]
                public int? nameplateInformationampereInterruptingCapacity { get; set; }

                [JsonProperty("nameplateInformation.availableFaultCurrent")]
                public int? nameplateInformationavailableFaultCurrent { get; set; }

                [JsonProperty("nameplateInformation.mainsRating")]
                public int? nameplateInformationmainsRating { get; set; }

                [JsonProperty("nameplateInformation.mainsType")]
                public string nameplateInformationmainsType { get; set; }
            }

            public class GENR
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }

                [JsonProperty("nameplateInformation.ampereRating")]
                public int? nameplateInformationampereRating { get; set; }
            }

            public class LAAR
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }
            }

            public class LVCB
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }

                [JsonProperty("nameplateInformation.manufacturer")]
                public string nameplateInformationmanufacturer { get; set; }

                [JsonProperty("nameplateInformation.model")]
                public string nameplateInformationmodel { get; set; }

                [JsonProperty("nameplateInformation.voltageRating")]
                public string nameplateInformationvoltageRating { get; set; }

                [JsonProperty("nameplateInformation.frameAmpereRating")]
                public string nameplateInformationframeAmpereRating { get; set; }

                [JsonProperty("tripUnitInformation.tripModuleAmpereRating")]
                public string tripUnitInformationtripModuleAmpereRating { get; set; }

                [JsonProperty("nameplateInformation.interruptingRating")]
                public int? nameplateInformationinterruptingRating { get; set; }

                [JsonProperty("tripUnitInformation.tripModel")]
                public string tripUnitInformationtripModel { get; set; }

                [JsonProperty("tripUnitSettings.longtimePickUpRanges")]
                public string tripUnitSettingslongtimePickUpRanges { get; set; }

                [JsonProperty("tripUnitSettings.longtimeDelayRanges")]
                public string tripUnitSettingslongtimeDelayRanges { get; set; }

                [JsonProperty("tripUnitSettings.shorttimePickUpRanges")]
                public string tripUnitSettingsshorttimePickUpRanges { get; set; }

                [JsonProperty("tripUnitSettings.shorttimeDelayRanges")]
                public string tripUnitSettingsshorttimeDelayRanges { get; set; }

                [JsonProperty("tripUnitSettings.groundFaultPickUpRanges")]
                public string tripUnitSettingsgroundFaultPickUpRanges { get; set; }

                [JsonProperty("tripUnitSettings.groundFaultDelayRanges")]
                public string tripUnitSettingsgroundFaultDelayRanges { get; set; }

                [JsonProperty("tripUnitSettings.instantaneousPickUpRanges")]
                public string tripUnitSettingsinstantaneousPickUpRanges { get; set; }
            }

            public class MCCB
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }

                [JsonProperty("nameplateInformation.manufacturer")]
                public string nameplateInformationmanufacturer { get; set; }

                [JsonProperty("nameplateInformation.model")]
                public string nameplateInformationmodel { get; set; }

                [JsonProperty("nameplateInformation.voltageRating")]
                public string nameplateInformationvoltageRating { get; set; }

                [JsonProperty("nameplateInformation.frameAmpereRating")]
                public int nameplateInformationframeAmpereRating { get; set; }

                [JsonProperty("tripUnitInformation.tripModuleAmpereRating")]
                public int tripUnitInformationtripModuleAmpereRating { get; set; }

                [JsonProperty("nameplateInformation.interruptingRating")]
                public int nameplateInformationinterruptingRating { get; set; }

                [JsonProperty("tripUnitInformation.tripModel")]
                public string tripUnitInformationtripModel { get; set; }
            }

            public class MCCBL
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }

                [JsonProperty("nameplateInformation.manufacturer")]
                public string nameplateInformationmanufacturer { get; set; }

                [JsonProperty("nameplateInformation.model")]
                public string nameplateInformationmodel { get; set; }

                [JsonProperty("nameplateInformation.voltageRating")]
                public string nameplateInformationvoltageRating { get; set; }

                [JsonProperty("nameplateInformation.frameAmpereRating")]
                public int nameplateInformationframeAmpereRating { get; set; }

                [JsonProperty("tripUnitInformation.tripModuleAmpereRating")]
                public int tripUnitInformationtripModuleAmpereRating { get; set; }

                [JsonProperty("nameplateInformation.interruptingRating")]
                public int nameplateInformationinterruptingRating { get; set; }

                [JsonProperty("tripUnitInformation.tripModel")]
                public string tripUnitInformationtripModel { get; set; }
            }

            public class MMME
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }
            }

            public class MTSW
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Fed-By Asset (2)")]
                public string FedByAsset2 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }
            }

            public class PANL
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset")]
                public string FedByAsset { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }

                [JsonProperty("nameplateInformation.manufacturer")]
                public string nameplateInformationmanufacturer { get; set; }

                [JsonProperty("nameplateInformation.mountingType")]
                public string nameplateInformationmountingType { get; set; }

                [JsonProperty("nameplateInformation.enclosureType")]
                public string nameplateInformationenclosureType { get; set; }

                [JsonProperty("nameplateInformation.voltageRating")]
                public string nameplateInformationvoltageRating { get; set; }

                [JsonProperty("nameplateInformation.configuration")]
                public string nameplateInformationconfiguration { get; set; }

                [JsonProperty("nameplateInformation.numberOfPoles")]
                public int nameplateInformationnumberOfPoles { get; set; }

                [JsonProperty("nameplateInformation.ampereInterruptingCapacity")]
                public int nameplateInformationampereInterruptingCapacity { get; set; }

                [JsonProperty("nameplateInformation.availableFaultCurrent")]
                public int nameplateInformationavailableFaultCurrent { get; set; }

                [JsonProperty("nameplateInformation.mainsRating")]
                public int nameplateInformationmainsRating { get; set; }

                [JsonProperty("nameplateInformation.mainsType")]
                public string nameplateInformationmainsType { get; set; }
            }

            public class PITR
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }
            }

            public class Root
            {
                public List<ATSW> ATSW { get; set; }
                public List<CITR> CITR { get; set; }

                [JsonProperty("DISC-L-F")]
                public List<DISCLF> DISCLF { get; set; }

                [JsonProperty("DISC-L")]
                public List<DISCL> DISCL { get; set; }
                public List<DPNL> DPNL { get; set; }

                [JsonProperty("DFTR-L")]
                public List<DFTRL> DFTRL { get; set; }

                [JsonProperty("DFTR-S")]
                public List<DFTR> DFTRS { get; set; }
                public List<GENR> GENR { get; set; }
                public List<LAAR> LAAR { get; set; }
                public List<LVCB> LVCB { get; set; }

                [JsonProperty("MCCB-L")]
                public List<MCCBL> MCCBL { get; set; }

                [JsonProperty("MCCB-S")]
                public List<MCCB> MCCBS { get; set; }
                public List<MMME> MMME { get; set; }
                public List<MTSW> MTSW { get; set; }
                public List<PANL> PANL { get; set; }
                public List<PITR> PITR { get; set; }
                public List<SWBD> SWBD { get; set; }
                public List<SWGR> SWGR { get; set; }
                public List<UPSX> UPSX { get; set; }
            }

            public class SWBD
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }

                [JsonProperty("nameplateInformation.manufacturer")]
                public string nameplateInformationmanufacturer { get; set; }

                [JsonProperty("nameplateInformation.model")]
                public string nameplateInformationmodel { get; set; }

                [JsonProperty("nameplateInformation.voltageRating")]
                public string nameplateInformationvoltageRating { get; set; }

                [JsonProperty("nameplateInformation.phaseAmpereRating")]
                public int nameplateInformationphaseAmpereRating { get; set; }

                [JsonProperty("nameplateInformation.faultWithstandRating")]
                public int nameplateInformationfaultWithstandRating { get; set; }

                [JsonProperty("nameplateInformation.mainSeparate")]
                public string nameplateInformationmainSeparate { get; set; }
            }

            public class SWGR
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }
            }

            public class UPSX
            {
                [JsonProperty("Asset Name")]
                public string AssetName { get; set; }

                [JsonProperty("Asset Type")]
                public string AssetType { get; set; }

                [JsonProperty("Top-Level Asset")]
                public string TopLevelAsset { get; set; }

                [JsonProperty("Fed-By Asset (1)")]
                public string FedByAsset1 { get; set; }

                [JsonProperty("Asset Building")]
                public string AssetBuilding { get; set; }

                [JsonProperty("Asset Floor")]
                public string AssetFloor { get; set; }

                [JsonProperty("Asset Room")]
                public string AssetRoom { get; set; }

                [JsonProperty("Asset Section")]
                public string AssetSection { get; set; }

                [JsonProperty("Asset Condition")]
                public string AssetCondition { get; set; }

                [JsonProperty("Operating Conditions")]
                public string OperatingConditions { get; set; }
                public string Criticality { get; set; }

                [JsonProperty("PM Plan")]
                public string PMPlan { get; set; }
            }


        
    }
}