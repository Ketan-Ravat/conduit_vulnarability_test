using AutoMapper;
using Carbon.Json;
using Jarvis.db.Migrations;
using Jarvis.db.Models;
using Jarvis.Service.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using MongoDB.Bson;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ZXing;

namespace Jarvis.Service.Concrete
{
    public class PMService : BaseService, IPMService
    {
        public readonly IMapper _mapper;
        private Logger _logger;

        public PMService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            _logger = Logger.GetInstance<PMService>();
        }

        public async Task<PMResponseModel> AddUpdatePM(AddPMRequestModel pmRequest)
        {
            PMResponseModel pmResponse = new PMResponseModel();
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    if (pmRequest.pm_id != null && pmRequest.pm_id != Guid.Empty)
                    {
                        // Add code to update PM
                        var pmDetails = await _UoW.PMRepository.GetPMsById(pmRequest.pm_id.Value);
                        if (pmDetails != null)
                        {
                            pmDetails.title = pmRequest.title;
                            pmDetails.description = pmRequest.description;
                            pmDetails.status = pmRequest.status;
                            pmDetails.pm_trigger_by = pmRequest.pm_trigger_by;
                            pmDetails.pm_trigger_type = pmRequest.pm_trigger_type;
                            pmDetails.is_trigger_on_starting_at = pmRequest.is_trigger_on_starting_at;
                            pmDetails.estimation_time = pmRequest.estimation_time;
                            pmDetails.work_procedure_type = pmRequest.work_procedure_type;
                            pmDetails.form_id = pmRequest.form_id;
                            pmDetails.modified_at = DateTime.UtcNow;
                            pmDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                            #region in conduit we are not using tasks
                            /*
                            foreach (var tasks in pmRequest.PMTasks)
                            {
                                PMTasks pmTasks = new PMTasks();
                                if (tasks.pm_task_id == null || tasks.pm_task_id == Guid.Empty)
                                {
                                    var alreadyexist = pmDetails.PMTasks.Where(x => x.task_id == tasks.task_id && x.pm_id == pmDetails.pm_id).FirstOrDefault();
                                    if (alreadyexist != null && alreadyexist.pm_task_id != null && alreadyexist.pm_task_id != Guid.Empty)
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
                                        pmTasks.task_id = tasks.task_id;
                                        pmTasks.pm_plan_id = pmDetails.pm_plan_id;
                                        pmTasks.created_at = DateTime.UtcNow;
                                        pmTasks.is_archive = false;
                                        pmTasks.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                        pmTasks.modified_at = DateTime.UtcNow;
                                        pmDetails.PMTasks.Add(pmTasks);
                                    }
                                }
                            }

                            var pmtasks = pmDetails.PMTasks.Where(x => x.pm_id == pmRequest.pm_id).ToList();

                            var removeTasks = pmtasks.Where(p => !pmRequest.PMTasks.Any(p2 => p2.task_id == p.task_id) && p.is_archive == false).ToList();

                            if (removeTasks.Count > 0)
                            {
                                removeTasks.ForEach(x => x.is_archive = true);
                            }
                            */
                            #endregion in conduit we are not using tasks

                            #region PM Attachments

                            if (pmRequest.pm_attachments != null)
                            {
                                foreach (var attachments in pmRequest.pm_attachments)
                                {
                                    PMAttachments pmAttachments = new PMAttachments();
                                    if (attachments.pm_attachment_id == null || attachments.pm_attachment_id == Guid.Empty)
                                    {
                                        var alreadyexist = pmDetails.PMAttachments.Where(x => x.pm_id == pmDetails.pm_id && x.user_uploaded_name == attachments.user_uploaded_name).FirstOrDefault();
                                        if (alreadyexist != null && alreadyexist.pm_attachment_id != null && alreadyexist.pm_attachment_id != Guid.Empty)
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
                                            pmAttachments.pm_plan_id = pmDetails.pm_plan_id;
                                            pmAttachments.filename = attachments.filename;
                                            pmAttachments.user_uploaded_name = attachments.user_uploaded_name;
                                            pmAttachments.created_at = DateTime.UtcNow;
                                            pmAttachments.is_archive = false;
                                            pmAttachments.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                            pmAttachments.modified_at = DateTime.UtcNow;
                                            pmDetails.PMAttachments.Add(pmAttachments);
                                        }
                                    }
                                }

                                var pmattachments = pmDetails.PMAttachments.Where(x => x.pm_id == pmRequest.pm_id).ToList();

                                var removeAttachments = pmattachments.Where(p => !pmRequest.pm_attachments.Any(p2 => p2.user_uploaded_name == p.user_uploaded_name) && p.is_archive == false).ToList();

                                if (removeAttachments.Count > 0)
                                {
                                    removeAttachments.ForEach(x => x.is_archive = true);
                                }
                            }

                            #endregion

                            #region PMTriggerCondition
                            pmDetails.PMsTriggerConditionMapping.ToList().ForEach(tmtriggercondition =>
                            {
                                var tmtriggerconditionrequest = pmRequest.pm_trigger_condition_mapping_request_model.Where(x => x.pm_trigger_condition_mapping_id == tmtriggercondition.pm_trigger_condition_mapping_id).FirstOrDefault();
                                tmtriggercondition.datetime_repeates_every = tmtriggerconditionrequest.datetime_repeates_every;
                                tmtriggercondition.datetime_repeat_time_period_type = tmtriggerconditionrequest.datetime_repeat_time_period_type;
                                tmtriggercondition.modified_at = DateTime.UtcNow;
                                tmtriggercondition.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            });
                            #endregion PMTriggerCondition

                            result = _UoW.PMRepository.Update(pmDetails).Result;

                            if (result > 0)
                            {
                                _UoW.SaveChanges();
                                _dbtransaction.Commit();
                                pmResponse = _mapper.Map<PMResponseModel>(pmDetails);
                                pmResponse.response_status = result;
                            }
                        }
                    }
                    else
                    {
                        var addPM = _mapper.Map<PMs>(pmRequest);
                        addPM.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        addPM.created_at = DateTime.UtcNow;
                        if(!String.IsNullOrEmpty(addPM.title) &&  addPM.title.ToLower() == "infrared thermography")
                        {
                            addPM.pm_inspection_type_id = 1;
                        }
                        /*addPM.PMTasks.ToList().ForEach(x =>
                        {
                            x.created_at = DateTime.UtcNow;
                            x.modified_at = DateTime.UtcNow;
                            x.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            x.pm_plan_id = addPM.pm_plan_id;
                        });*/
                        if (addPM.PMAttachments != null)
                        {
                            addPM.PMAttachments.ToList().ForEach(x =>
                            {
                                x.created_at = DateTime.UtcNow;
                                x.modified_at = DateTime.UtcNow;
                                x.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                x.pm_plan_id = addPM.pm_plan_id;
                            });
                        }
                        addPM.PMsTriggerConditionMapping.ToList().ForEach(x =>
                        {
                            x.created_at = DateTime.UtcNow;
                            x.modified_at = DateTime.UtcNow;
                            x.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            //x.pm_plan_id = addPM.pm_plan_id;
                        });
                        result = await _UoW.PMRepository.Insert(addPM);
                        if (result > 0)
                        {
                            _UoW.SaveChanges();
                            _dbtransaction.Commit();
                            pmResponse = _mapper.Map<PMResponseModel>(addPM);
                            pmResponse.response_status = result;
                        }
                        else
                        {
                            pmResponse.response_status = result;
                        }
                    }

                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    pmResponse.response_status = (int)ResponseStatusNumber.Error;
                }
            }

            return pmResponse;
        }

        public async Task<ListViewModel<GetAllPMsByPlanIdResponsemodel>> GetAllPMsByPlanId(Guid pm_plan_id)
        {
            ListViewModel<GetAllPMsByPlanIdResponsemodel> pMsResponse = new ListViewModel<GetAllPMsByPlanIdResponsemodel>();
            try
            {
                var pmDetails = await _UoW.PMRepository.GetAllPMsByPlan(pm_plan_id);
                if (pmDetails?.Count > 0)
                {
                    pMsResponse.list = _mapper.Map<List<GetAllPMsByPlanIdResponsemodel>>(pmDetails);
                    /*pMsResponse.list.ForEach(x =>
                    {
                        x.PMTasks = x.PMTasks.Where(x => !x.is_archive).ToList();
                    });*/
                    /*pMsResponse.list.ForEach(x =>
                    {
                        x.PMAttachments = x.PMAttachments.Where(x => !x.is_archive).ToList();
                    });*/
                    pMsResponse.listsize = pMsResponse.list.Count;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return pMsResponse;
        }

        public async Task<PMResponseModel> GetPMByID(Guid id)
        {
            PMResponseModel pmResponse = new PMResponseModel();
            try
            {
                var pmDetails = await _UoW.PMRepository.GetPMsById(id);
                if (pmDetails?.pm_id != null)
                {
                    pmDetails.PMTasks = pmDetails.PMTasks.Where(x => !x.is_archive).ToList();
                    pmDetails.PMAttachments = pmDetails.PMAttachments.Where(x => !x.is_archive).ToList();
                    pmResponse = _mapper.Map<PMResponseModel>(pmDetails);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return pmResponse;
        }

        public async Task<int> DeletePMByID(Guid id)
        {
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var pmDetails = await _UoW.PMRepository.GetPMsById(id);
                    if (pmDetails?.pm_id != null)
                    {
                        pmDetails.is_archive = true;
                        pmDetails.modified_at = DateTime.UtcNow;
                        pmDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        pmDetails.PMTasks.ToList().ForEach(x =>
                        {
                            x.is_archive = true;
                            x.modified_at = DateTime.UtcNow;
                            x.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        });
                        pmDetails.PMAttachments.ToList().ForEach(x =>
                        {
                            x.is_archive = true;
                            x.modified_at = DateTime.UtcNow;
                            x.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        });
                        result = _UoW.PMRepository.Update(pmDetails).Result;
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

        public async Task<PMResponseModel> MovePM(MovePMRequestModel pmRequest)
        {
            PMResponseModel pmResponse = new PMResponseModel();
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var pmDetails = await _UoW.PMRepository.GetPMsById(pmRequest.pm_id);
                    if (pmDetails != null)
                    {
                        pmDetails.pm_plan_id = pmRequest.new_pm_plan_id;
                        pmDetails.modified_at = DateTime.UtcNow;
                        pmDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();


                        foreach (var tasks in pmDetails.PMTasks)
                        {
                            tasks.pm_plan_id = pmRequest.new_pm_plan_id;
                            tasks.modified_at = DateTime.UtcNow;
                            tasks.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        }

                        foreach (var attachments in pmDetails.PMAttachments)
                        {
                            attachments.pm_plan_id = pmRequest.new_pm_plan_id;
                            attachments.modified_at = DateTime.UtcNow;
                            attachments.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        }

                        result = _UoW.PMRepository.Update(pmDetails).Result;

                        if (result > 0)
                        {
                            _UoW.SaveChanges();
                            _dbtransaction.Commit();
                            pmResponse = _mapper.Map<PMResponseModel>(pmDetails);
                            pmResponse.response_status = result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _dbtransaction.Rollback();
                    pmResponse.response_status = (int)ResponseStatusNumber.Error;
                }
            }

            return pmResponse;
        }

        public async Task<int> createPMforOldAssetClass()
        {


            var get_assets = _UoW.PMRepository.Getallassetstovalidatejson();
            foreach(var asset in get_assets)
            {
                try
                {
                    JsonDocument doc = JsonDocument.Parse(asset.form_retrived_nameplate_info);
                    Console.WriteLine("JSON is valid!");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Invalid JSON: {ex.Message}");
                }
            }

            /* #region create PM for old class
             GetAllAssetClassRequestmodel request = new GetAllAssetClassRequestmodel();
             request.company_id = Guid.Parse("0b2b3b98-f141-40f1-88b9-fa8de7224c0f");
             var get_all_asset_class = _UoW.FormIOAssetClassRepository.GetAllAssetClass(request);
             var list = get_all_asset_class.Item1;
             foreach(var item in list)
             {
                 PMCategory PMCategory = new PMCategory();
                 PMCategory.company_id = item.company_id;
                 PMCategory.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                 PMCategory.created_at = DateTime.UtcNow;
                 var category_code = GenerateRandomString.RandomString(8);
                 PMCategory.category_code = category_code;
                 PMCategory.status = (int)Status.Active;
                 PMCategory.category_name = item.asset_class_name;
                 PMCategory.inspectiontemplate_asset_class_id = item.inspectiontemplate_asset_class_id;
                 var insert_pm = await _UoW.BaseGenericRepository<PMCategory>().Insert(PMCategory);

                 _UoW.SaveChanges();
             }

             #endregion create PM for old class*/
            return 1;
        }

        public List<GetPMsListByAssetClassIdResponseModel> GetPMsListByAssetClassId(GetPMsListByAssetClassIdRequestModel requestModel)
        {
            List<GetPMsListByAssetClassIdResponseModel> response = new List<GetPMsListByAssetClassIdResponseModel>();
            try
            {
                var get_pm_list = _UoW.PMRepository.GetPMsListByAssetClassId(requestModel);

                if (get_pm_list != null)
                {
                    response = _mapper.Map<List<GetPMsListByAssetClassIdResponseModel>>(get_pm_list);
                    
                    string get_asset_class_code = _UoW.PMRepository.GetAssetClassCodeByAssetId(requestModel);

                    foreach(var pm in response)
                    {
                        if(get_asset_class_code != null && get_asset_class_code != "")
                        {
                            var get_pmitem_master_form = _UoW.WorkOrderRepository.GetPMMasterFormByAssetpm(get_asset_class_code, pm.plan_name, pm.title);
                            if (get_pmitem_master_form != null)
                            {
                                pm.form_name = get_pmitem_master_form.form_name;
                            }else if(pm.pm_inspection_type_id == (int)PMInspectionTypeId.IRThermography)
                            {
                                pm.form_name = "Energized";
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            return response;
        }

        public async Task<ManuallyAssignAnyPMtoWOResponseModel> ManuallyAssignAnyPMtoWO(AssignPMsToAssetRequestModel requestModel)
        {
            ManuallyAssignAnyPMtoWOResponseModel response = new ManuallyAssignAnyPMtoWOResponseModel();

            AddAssetPMWolineRequestmodel AddAssetPMWolineRequestmodel = new AddAssetPMWolineRequestmodel();
            AddAssetPMWolineRequestmodel.wo_id = requestModel.wo_id;

            AddAssetPMWolineRequestmodel.asset_pm_id = new List<Guid>();
            try
            {
                var get_plan_list = _UoW.PMRepository.GetPMPlansListByPMIds(requestModel.pm_ids);
                
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
                        if (requestModel.pm_ids.Contains(pm.pm_id))
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

                            AddAssetPMWolineRequestmodel.asset_pm_id.Add(assetPMs.asset_pm_id);
                        }
                        
                    }
                }

                WorkOrderService workOrderService = new WorkOrderService(_mapper);
                response.pm_woline_list = await workOrderService.AddAssetPMWoline(AddAssetPMWolineRequestmodel);

            }
            catch (Exception e)
            {
            }
            return response;
        }
    }
}
