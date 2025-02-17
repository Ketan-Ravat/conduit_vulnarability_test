using AutoMapper;
using Jarvis.db.Models;
using Jarvis.Service.Abstract;
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
    public class PMPlansService : BaseService, IPMPlansService
    {
        public readonly IMapper _mapper;
        private Logger _logger;

        public PMPlansService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            _logger = Logger.GetInstance<PMPlansService>();
        }

        public async Task<PMPlansResponseModel> AddUpdatePMPlan(PMPlansRequestModel pmPlanRequest)
        {
            PMPlansResponseModel pmPlanResponse = new PMPlansResponseModel();
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    if (pmPlanRequest.pm_plan_id != null && pmPlanRequest.pm_plan_id != Guid.Empty)
                    {
                        var planDetails = await _UoW.PMPlansRepository.GetPMPlanById(pmPlanRequest.pm_plan_id);
                        if (planDetails != null)
                        {
                            planDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            planDetails.modified_at = DateTime.UtcNow;
                            planDetails.plan_name = pmPlanRequest.plan_name;
                            planDetails.pm_category_id = pmPlanRequest.pm_category_id;
                            if (pmPlanRequest.is_default_pm_plan)
                            { // remove other pm plan if default is active
                                var planDetails_for_remove = _UoW.PMPlansRepository.GetPMPlanByIdtoRemoveDefault(planDetails.pm_category_id);
                                if (planDetails_for_remove != null)
                                {
                                    planDetails_for_remove.is_default_pm_plan = false;

                                    var updtae_defuault = await _UoW.BaseGenericRepository<PMPlans>().Update(planDetails_for_remove);
                                    _UoW.SaveChanges();
                                }
                            }
                            planDetails.is_default_pm_plan = pmPlanRequest.is_default_pm_plan;

                            result = _UoW.PMPlansRepository.Update(planDetails).Result;
                            if (result > 0)
                            {
                                _UoW.SaveChanges();
                                _dbtransaction.Commit();
                                pmPlanResponse = _mapper.Map<PMPlansResponseModel>(planDetails);
                            }
                            else
                            {
                                _dbtransaction.Rollback();
                            }
                        }
                        pmPlanResponse.response_status = result;
                    }
                    else
                    {
                        pmPlanRequest.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        pmPlanRequest.created_at = DateTime.UtcNow;
                        pmPlanRequest.status = (int)Status.Active;
                        var pmPlan = _mapper.Map<PMPlans>(pmPlanRequest);
                        if (pmPlanRequest.is_default_pm_plan)
                        { // remove other pm plan if default is active
                            var planDetails_for_remove = _UoW.PMPlansRepository.GetPMPlanByIdtoRemoveDefault(pmPlanRequest.pm_category_id);
                            if (planDetails_for_remove != null)
                            {
                                planDetails_for_remove.is_default_pm_plan = false;

                                var updtae_defuault = await _UoW.BaseGenericRepository<PMPlans>().Update(planDetails_for_remove);
                                _UoW.SaveChanges();
                            }
                        }
                        pmPlan.is_default_pm_plan = pmPlanRequest.is_default_pm_plan;
                        result = await _UoW.PMPlansRepository.Insert(pmPlan);
                        if (result > 0)
                        {
                            _UoW.SaveChanges();
                            _dbtransaction.Commit();
                            pmPlanResponse = _mapper.Map<PMPlansResponseModel>(pmPlan);
                            pmPlanResponse.response_status = result;
                        }
                        else
                        {
                            pmPlanResponse.response_status = result;
                        }
                    }

                }
                catch (Exception)
                {
                    _dbtransaction.Rollback();
                    pmPlanResponse.response_status = (int)ResponseStatusNumber.Error;
                }
            }

            return pmPlanResponse;
        }

        public async Task<ListViewModel<PMPlansResponseModel>> GetAllPMPlans(Guid pm_category_id)
        {
            ListViewModel<PMPlansResponseModel> pMPlansResponse = new ListViewModel<PMPlansResponseModel>();
            try
            {
                var planDetails = await _UoW.PMPlansRepository.GetAllPMPlans(pm_category_id);
                if (planDetails?.Count > 0)
                {
                    if (UpdatedGenericRequestmodel.CurrentUser.role_id != GlobalConstants.SuperAdmin_Role_id)
                    {
                        planDetails = planDetails.Where(x => x.plan_name != "70B-STANDARD").ToList();
                    }
                    pMPlansResponse.list = _mapper.Map<List<PMPlansResponseModel>>(planDetails);
                    pMPlansResponse.listsize = pMPlansResponse.list.Count;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return pMPlansResponse;
        }

        public async Task<PMPlansResponseModel> GetPMPlanByID(Guid id)
        {
            PMPlansResponseModel pMCategoryResponse = new PMPlansResponseModel();
            try
            {
                var planDetails = await _UoW.PMPlansRepository.GetPMPlanById(id);
                if (planDetails?.pm_plan_id != null)
                {
                    pMCategoryResponse = _mapper.Map<PMPlansResponseModel>(planDetails);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return pMCategoryResponse;
        }

        public async Task<int> DeletePMPlanByID(Guid id)
        {
            int result = (int)ResponseStatusNumber.Error;

            // check if this plan is added to any asset and is any PM is incomplete
            var get_asset_pms = _UoW.PMPlansRepository.GetAssetPlansByMasterPlan(id);
            if (get_asset_pms!=null)
            {
                result = (int)ResponseStatusNumber.PMPlanIsLinked;
                return result;
            }
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var planDetails = await _UoW.PMPlansRepository.GetPMPlanById(id);
                    
                    if (planDetails?.pm_plan_id != null)
                    {
                        planDetails.status = (int)Status.Deactive;
                        planDetails.modified_at = DateTime.UtcNow;
                        planDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        if (planDetails.PMs?.Count > 0)
                        {
                            foreach (var pmitem in planDetails.PMs)
                            {
                                pmitem.is_archive = true;
                                pmitem.modified_at = DateTime.UtcNow;
                                pmitem.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                if (pmitem.PMTasks?.Count > 0)
                                {
                                    foreach (var pmtask in pmitem.PMTasks)
                                    {
                                        pmtask.is_archive = true;
                                        pmtask.modified_at = DateTime.UtcNow;
                                        pmtask.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                    }
                                }
                                if (pmitem.PMAttachments?.Count > 0)
                                {
                                    foreach (var pmattachment in pmitem.PMAttachments)
                                    {
                                        pmattachment.is_archive = true;
                                        pmattachment.modified_at = DateTime.UtcNow;
                                        pmattachment.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                    }
                                }
                            }
                        }
                        result = _UoW.PMPlansRepository.Update(planDetails).Result;
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
                        result = (int)ResponseStatusNumber.NotFound;
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

        public async Task<int> DuplicatePMPlan(DuplicatePMRequestModel request)
        {
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    PMPlans duplicatePlan = new PMPlans();
                    var planDetails = await _UoW.PMPlansRepository.GetPMPlanById(request.pm_plan_id);
                    if (planDetails?.pm_plan_id != null)
                    {
                        duplicatePlan.plan_name = "Duplicate - " + planDetails.plan_name;
                        duplicatePlan.modified_at = DateTime.UtcNow;
                        duplicatePlan.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        duplicatePlan.created_at = DateTime.UtcNow;
                        duplicatePlan.pm_category_id = planDetails.pm_category_id;
                        duplicatePlan.status = planDetails.status;
                        result = _UoW.PMPlansRepository.Insert(duplicatePlan).Result;
                        if (result > 0)
                        {
                            List<PMs> pmList = new List<PMs>();
                            if (planDetails.PMs?.Count > 0)
                            {
                                planDetails.PMs.ToList().ForEach(x =>
                                {
                                    PMs pmRequest = new PMs();
                                    pmRequest.title = x.title;
                                    pmRequest.description = x.description;
                                    pmRequest.status = x.status;
                                    pmRequest.pm_trigger_by = x.pm_trigger_by;
                                    pmRequest.service_dealer_id = x.service_dealer_id;
                                    pmRequest.pm_trigger_type = x.pm_trigger_type;
                                    pmRequest.datetime_due_at = x.datetime_due_at;
                                    pmRequest.meter_hours_due_at = x.meter_hours_due_at;
                                    pmRequest.datetime_repeates_every = x.datetime_repeates_every;
                                    pmRequest.datetime_starting_at = x.datetime_starting_at;
                                    pmRequest.datetime_repeat_time_period_type = x.datetime_repeat_time_period_type;
                                    pmRequest.meter_hours_starting_at = x.meter_hours_starting_at;
                                    pmRequest.meter_hours_repeates_every = x.meter_hours_repeates_every;
                                    pmRequest.is_trigger_on_starting_at = x.is_trigger_on_starting_at;
                                    pmRequest.modified_at = DateTime.UtcNow;
                                    pmRequest.created_at = DateTime.UtcNow;
                                    pmRequest.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                    pmRequest.pm_plan_id = duplicatePlan.pm_plan_id;

                                    if (x.PMTasks?.Count > 0)
                                    {
                                        List<PMTasks> pmTasks = new List<PMTasks>();
                                        foreach (var task in x.PMTasks)
                                        {
                                            PMTasks pmTask = new PMTasks();
                                            pmTask.task_id = task.task_id;
                                            pmTask.modified_at = DateTime.UtcNow;
                                            pmTask.created_at = DateTime.UtcNow;
                                            pmTask.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                            pmTask.pm_plan_id = duplicatePlan.pm_plan_id;
                                            pmTasks.Add(pmTask);
                                        }
                                        pmRequest.PMTasks = pmTasks;
                                    }

                                    if (x.PMAttachments?.Count > 0)
                                    {
                                        List<PMAttachments> pmAttachments = new List<PMAttachments>();
                                        foreach (var attachment in x.PMAttachments)
                                        {
                                            PMAttachments pmAttachment = new PMAttachments();
                                            pmAttachment.filename = attachment.filename;
                                            pmAttachment.user_uploaded_name = attachment.user_uploaded_name;
                                            pmAttachment.modified_at = DateTime.UtcNow;
                                            pmAttachment.created_at = DateTime.UtcNow;
                                            pmAttachment.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                            pmAttachment.pm_plan_id = duplicatePlan.pm_plan_id;
                                            pmAttachments.Add(pmAttachment);
                                        }
                                        pmRequest.PMAttachments = pmAttachments;
                                    }

                                    pmList.Add(pmRequest);
                                });
                                duplicatePlan.PMs = pmList;
                                result = _UoW.PMRepository.InsertList(duplicatePlan.PMs).Result;
                                if (result > 0)
                                {
                                    _UoW.SaveChanges();
                                    _dbtransaction.Commit();
                                    result = (int)ResponseStatusNumber.Success;
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
                        result = (int)ResponseStatusNumber.NotFound;
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

        public async Task<int> MarkDefaultPMPlan(MarkDefaultPMPlanRequestmodel request)
        {
            int result = (int)ResponseStatusNumber.Error;
           
            try
            {
                var planDetails = _UoW.PMPlansRepository.GetPMPlanByIdForDefault(request.pm_plan_id);
                var planDetails_for_remove = _UoW.PMPlansRepository.GetPMPlanByIdtoRemoveDefault(planDetails.pm_category_id);
                if (planDetails_for_remove != null)
                {
                    planDetails_for_remove.is_default_pm_plan = false;

                    var updtae_defuault = await _UoW.BaseGenericRepository<PMPlans>().Update(planDetails_for_remove);
                    _UoW.SaveChanges();
                }
               
                planDetails.is_default_pm_plan = true;

                var updtae = await _UoW.BaseGenericRepository<PMPlans>().Update(planDetails);
                _UoW.SaveChanges();

                if (updtae)
                {
                    _UoW.CommitTransaction();
                    result = (int)ResponseStatusNumber.Success;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
            return result;
        }
    }
}
