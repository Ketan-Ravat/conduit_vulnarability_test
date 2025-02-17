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
using Jarvis.Service.Notification;
using System.Text.Json;
using Jarvis.Shared.Helper;
using TimeZoneConverter;
using System.Dynamic;

namespace Jarvis.Service.Concrete
{
    public class MobileWorkorderService : BaseService, IMobileWorkOrderService, IDisposable
    {
        public readonly IMapper _mapper;
        private Logger _logger;

        public MobileWorkorderService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            _logger = Logger.GetInstance<WorkOrderService>();
        }
        public ListViewModel<MobileNewFlowWorkorderListResponseModel> GetAllWorkOrdersNewflow(NewFlowWorkorderListRequestModel requestModel)
        {
            ListViewModel<MobileNewFlowWorkorderListResponseModel> response = new ListViewModel<MobileNewFlowWorkorderListResponseModel>();

            var WO = _UoW.MobileWorkOrderRepository.GetAllWorkOrdersNewflow(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), requestModel);

            var mappedlist = _mapper.Map<List<MobileNewFlowWorkorderListResponseModel>>(WO.Item1);

            if (mappedlist != null && mappedlist.Count > 0)
            {
                response.list = mappedlist;
                response.listsize = WO.Item2;
                response.list.ForEach(s =>
                {
                    if (s.wo_type == (int)Status.Acceptance_Test_WO)
                    {
                        s.wo_number = "AT" + s.wo_number;
                    }
                    else if (s.wo_type == (int)Status.Maintenance_WO)
                    {
                        s.wo_number = "WO" + s.wo_number;
                    }
                });
            }
            return response;
        }

        public MobileViewWorkOrderDetailsByIdResponsemodel ViewWorkOrderDetailsById(string wo_id)
        {
            MobileViewWorkOrderDetailsByIdResponsemodel response = new MobileViewWorkOrderDetailsByIdResponsemodel();
            var wo_details = _UoW.MobileWorkOrderRepository.ViewWorkOrderDetailsById(Guid.Parse(wo_id));
            if (wo_details != null)
            {
                wo_details.WorkOrderAttachments = wo_details.WorkOrderAttachments.Where(q => !q.is_archive).ToList();
                response = _mapper.Map<MobileViewWorkOrderDetailsByIdResponsemodel>(wo_details);
                if (response.wo_type == (int)Status.Acceptance_Test_WO)
                {
                    response.wo_number = "AT" + response.wo_number;
                }
                else if (response.wo_type == (int)Status.Maintenance_WO)
                {
                    response.wo_number = "WO" + response.wo_number;
                }

                /// asset form ios
                /// 

                var get_asset_form = _UoW.WorkOrderRepository.GetAllAssetFormByWOIDOffline(new List<Guid>() { Guid.Parse(wo_id) }, null);
                List<AssetFormIO> WO_Assetformios = _mapper.Map<List<AssetFormIO>>(get_asset_form);
                List<AssetFormIO> test = null;

                var get_wo_form_mapping = _UoW.WorkOrderRepository.GetWOcategorymapping(Guid.Parse(wo_id));
                response.form_category_list = new List<mobile_form_categoty_list>();
                if (get_wo_form_mapping != null && get_wo_form_mapping.Count > 0)
                {
                    response.form_category_list = new List<mobile_form_categoty_list>();
                    get_wo_form_mapping.ForEach(q =>
                    {

                        mobile_form_categoty_list form_categoty_list = new mobile_form_categoty_list();
                        form_categoty_list.wo_inspectionsTemplateFormIOAssignment_id = q.wo_inspectionsTemplateFormIOAssignment_id;
                        form_categoty_list.form_category_name = q.InspectionsTemplateFormIO.FormIOType.form_type_name;
                        form_categoty_list.form_name = q.InspectionsTemplateFormIO.form_name;
                        form_categoty_list.form_data = q.InspectionsTemplateFormIO.form_data;
                       // form_categoty_list.form_description = q.Tasks != null ? q.Tasks.description : null;
                        form_categoty_list.WP = q.InspectionsTemplateFormIO.work_procedure;
                        if (q.Parent_Asset != null)
                        {
                            form_categoty_list.parent_asset_name = q.Parent_Asset.name;
                         //   form_categoty_list.parent_asset_id = q.asset_id;
                        }
                        if (q.User != null)
                        {
                            //  form_categoty_list.technician_name = q.User.firstname + q.User.lastname;
                            //    form_categoty_list.technician_id = q.User.uuid;
                         //   form_categoty_list.parent_asset_id = q.asset_id;
                        }
                        form_categoty_list.status_id = q.status_id;
                        form_categoty_list.status_name = q.StatusMaster.status_name;
                        if (q.WOcategorytoTaskMapping != null && q.WOcategorytoTaskMapping.Count > 0)
                        {
                            form_categoty_list.progress_total = q.WOcategorytoTaskMapping.Where(t => !t.is_archived).Count(); // total task in category
                            var wo_category_task_mapping_ids = q.WOcategorytoTaskMapping.Where(t => !t.is_archived).Select(w => w.WOcategorytoTaskMapping_id).ToList();
                            test = WO_Assetformios.Where(w => wo_category_task_mapping_ids.Contains(w.WOcategorytoTaskMapping_id.Value)).ToList();
                            if (test != null && test.Count > 0)
                            {
                                form_categoty_list.progress_completed = test.Where(x => x.status == (int)Status.Completed).Count();
                            }
                            //form_categoty_list.progress_completed = q.WOcategorytoTaskMapping.Where(t => !t.is_archived).Select(x => x.AssetFormIO.status == (int)Status.Completed).Count();
                        }
                        response.form_category_list.Add(form_categoty_list);
                        Dispose();
                    });
                }
                var wo_tasks = GetAllWOCategoryTaskByWOid(wo_details.wo_id.ToString());
                response.wo_all_tasks = new List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel>();
                if (wo_tasks != null)
                {
                    response.wo_all_tasks = wo_tasks;
                }

            }
            else
            {
                /// not found
                response = null;
            }
            return response;
        }
        public List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel> GetAllWOCategoryTaskByWOid(string wo_id)
        {
            List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel> response = new List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel>();
            try
            {
                var task_lists = _UoW.WorkOrderRepository.GetAllWOCategoryTaskByWOidForTask(Guid.Parse(wo_id),0);

                if (task_lists != null && task_lists.Count > 0)
                {
                    /* var get_asset_form = _UoW.WorkOrderRepository.GetAllAssetFormByWOIDOffline(new List<Guid>() { Guid.Parse(wo_id) });
                     List<AssetFormIO> WO_Assetformios = _mapper.Map<List<AssetFormIO>>(get_asset_form);
                     task_lists.ForEach(s =>
                     {
                         s.AssetFormIO = WO_Assetformios.Where(q => q.WOcategorytoTaskMapping_id == s.WOcategorytoTaskMapping_id).FirstOrDefault();
                     });
                    */
                    //int serial_no = 0;
                    response = _mapper.Map<List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel>>(task_lists);
                    response.ForEach(x =>
                    {
                        // serial_no++;
                        // x.serial_number = serial_no;
                        if (x.technician_id != null && x.technician_id != Guid.Empty)
                        {
                            var technician_user = _UoW.WorkOrderRepository.GetUserByID(x.technician_id.Value);
                            x.technician_name = technician_user.firstname + " " + technician_user.lastname;
                        }

                    });
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public ListViewModel<MobileTaskResponseModel> GetAllTasks(int pageindex, int pagesize, string searchstring)
        {
            ListViewModel<MobileTaskResponseModel> taskResponse = new ListViewModel<MobileTaskResponseModel>();
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var taskDetails = _UoW.MobileWorkOrderRepository.GetAllTasks(searchstring);
                    if (taskDetails?.Count > 0)
                    {
                        if (pageindex == 0 || pagesize == 0)
                        {
                            pagesize = 20;
                            pageindex = 1;
                        }
                        taskResponse.listsize = taskDetails.Count;
                        taskDetails = taskDetails.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                        taskDetails.ForEach(x =>
                        {
                            x.AssetTasks = x.AssetTasks.Where(q => !q.is_archive).ToList();
                        });
                        taskResponse.list = _mapper.Map<List<MobileTaskResponseModel>>(taskDetails);
                        //var assetresponse = _mapper.Map<AssetsResponseModel>(asset);
                        taskResponse.pageIndex = pageindex;
                        taskResponse.pageSize = pagesize;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return taskResponse;
        }
        public List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel> GetWOcategoryTaskByCategoryID(string wo_inspectionsTemplateFormIOAssignment_id)
        {
            List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel> response = new List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel>();
            try
            {
                var task_lists = _UoW.MobileWorkOrderRepository.GetWOcategoryTaskByCategoryID(wo_inspectionsTemplateFormIOAssignment_id);
                if (task_lists != null && task_lists.Count > 0)
                {
                    task_lists = task_lists.OrderBy(x => x.serial_number).ToList();
                    // int serial_no = 0;
                    response = _mapper.Map<List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel>>(task_lists);
                    response.ForEach(x =>
                    {

                        // serial_no++;
                        // x.serial_number = serial_no;
                        if (x.technician_id != null && x.technician_id != Guid.Empty)
                        {
                            var technician_user = _UoW.WorkOrderRepository.GetUserByID(x.technician_id.Value);
                            x.technician_name = technician_user.firstname + " " + technician_user.lastname;
                        }
                    });
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public async Task<MobileGetWOsForOfflineResponsemodel> GetWOsForOffline(string userid)
        {
            MobileGetWOsForOfflineResponsemodel response = new MobileGetWOsForOfflineResponsemodel();

            if (UpdatedGenericRequestmodel.CurrentUser.device_uuid != null && UpdatedGenericRequestmodel.CurrentUser.device_uuid != Guid.Empty)
            {
                bool force_to_reset = false;
                DateTime? sync_time = null;
                DateTime? old_sync_time = null;
                DateTime? master_form_sync_time = null;
                var device_info = _UoW.DeviceRepository.GetDeviceInfoByUUId(UpdatedGenericRequestmodel.CurrentUser.device_uuid);

                var user_site_id = await _UoW.UserRepository.GetUserByID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                old_sync_time = device_info.last_sync_time;
                if (device_info.last_sync_time != null)
                {
                    master_form_sync_time = old_sync_time;
                }
                if (user_site_id.ac_active_site == device_info.last_sync_site_id)
                {
                    force_to_reset = false;
                    sync_time = device_info.last_sync_time;
                }
                else
                {
                    force_to_reset = true;
                }
                device_info.last_sync_site_id = user_site_id.ac_active_site;
                device_info.last_sync_time = DateTime.UtcNow;
                device_info.modified_at = DateTime.UtcNow;
                device_info.modified_by = userid;
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.app_version))
                {
                    device_info.app_version = UpdatedGenericRequestmodel.CurrentUser.app_version;
                }
                bool update_device_info = await _UoW.BaseGenericRepository<DeviceInfo>().Update(device_info);


                #region WO_data
                var wos = _UoW.WorkOrderRepository.GetWOsForOffline(userid, sync_time);
                var womapper = _mapper.Map<List<MobileNewFlowWorkorderListResponseModel>>(wos.Item1);
                womapper.ForEach(s =>
                {
                    if (s.wo_type == (int)Status.Acceptance_Test_WO)
                    {
                        s.wo_number = "AT" + s.wo_number;
                    }
                    else if (s.wo_type == (int)Status.Maintenance_WO)
                    {
                        s.wo_number = "WO" + s.wo_number;
                    }
                });
                response.workorders = womapper;
                #endregion WO_data

                #region MasterFormData  // master formio data
                ListViewModel<MobileFormIOResponseModel> response1 = new ListViewModel<MobileFormIOResponseModel>();
                var formios = _UoW.formIORepository.GetAllFormsforOffline(master_form_sync_time);
                response1.list = _mapper.Map<List<MobileFormIOResponseModel>>(formios.Item1);
                response.formio_master_forms = response1.list;
                #endregion MasterFormData

                var wo_ids = wos.Item1.Select(x => x.wo_id).ToList();

                #region WOTaskstoFormMappingData // Task forms
                List<MobileGetFormByWOTaskIDResponsemodel> formresponse = null;
                var get_asset_form = _UoW.WorkOrderRepository.GetAllAssetFormByWOIDOffline(new List<Guid>(), sync_time);
                if (get_asset_form != null)
                {
                    formresponse = _mapper.Map<List<MobileGetFormByWOTaskIDResponsemodel>>(get_asset_form);
                    formresponse.ForEach(x =>
                    {
                        x.asset_form_data = get_asset_form.Where(q => q.asset_form_id == x.asset_form_id).Select(q => q.asset_form_data).FirstOrDefault();
                        /*if (x.asset_form_data != null)
                        {
                            var dynamic_form_obj = JsonSerializer.Deserialize<FormioDynamicDataobj.Root>(x.asset_form_data);
                            x.asset_form_data = JsonSerializer.Serialize(dynamic_form_obj.data);
                        }*/
                    });
                }
                response.wo_task_forms_list = formresponse;
                #endregion WOTaskstoFormMappingData

                List<AssetFormIO> WO_Assetformios = _mapper.Map<List<AssetFormIO>>(get_asset_form);
                var categories = _UoW.WorkOrderRepository.GetAllCatagoryForWOOffline(sync_time);

                #region WOCategoriestoTasksMappingData // Tasks

                List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel> task_mapping = new List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel>();
                var task_lists = _UoW.WorkOrderRepository.GetAllWOCategoryTaskByWOidOffline(sync_time);
                try
                {
                    //if sync time is not null then get asset form data based on WOcategorytoTaskMapping_id
                    List<db.ExcludePropertiesfromDBHelper.AssetFormIOExclude> asset_form_io_by_task_id = new List<db.ExcludePropertiesfromDBHelper.AssetFormIOExclude>();
                    if (sync_time != null)
                    {
                        asset_form_io_by_task_id = _UoW.WorkOrderRepository.GetAssetFormBycategorytaskid(task_lists.Select(x => x.WOcategorytoTaskMapping_id).ToList(), sync_time);
                        categories = _UoW.WorkOrderRepository.GetCatagoryForWOOfflineByCategoryids(task_lists.Select(x => x.wo_inspectionsTemplateFormIOAssignment_id).ToList());
                        WO_Assetformios = _mapper.Map<List<AssetFormIO>>(asset_form_io_by_task_id);
                    }
                    else
                    {
                        asset_form_io_by_task_id = get_asset_form; // if first time then all data will be mapped
                        WO_Assetformios = _mapper.Map<List<AssetFormIO>>(asset_form_io_by_task_id);
                    }
                    task_lists.ForEach(x =>
                    {
                        x.AssetFormIO = WO_Assetformios.Where(q => q.WOcategorytoTaskMapping_id == x.WOcategorytoTaskMapping_id).FirstOrDefault();
                        x.WOInspectionsTemplateFormIOAssignment = categories.Where(q => q.wo_inspectionsTemplateFormIOAssignment_id == x.wo_inspectionsTemplateFormIOAssignment_id).FirstOrDefault();
                    });
                    if (task_lists != null && task_lists.Count > 0)
                    {
                        // int serial_no = 0;
                        task_mapping = _mapper.Map<List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel>>(task_lists);
                        var technician_ids = task_mapping.Where(x => x.technician_id != null).Select(x => x.technician_id.Value).Distinct().ToList();
                        var technicians = _UoW.WorkOrderRepository.GetUserByIDs(technician_ids);
                        task_mapping.ForEach(x =>
                        {
                            // serial_no++;ne s
                            //  x.serial_number = serial_no;
                            if (x.technician_id != null && x.technician_id != Guid.Empty)
                            {
                                var technician_user = technicians.Where(e => e.uuid == x.technician_id).FirstOrDefault();
                                if (technician_user != null)
                                {
                                    x.technician_name = technician_user.firstname + " " + technician_user.lastname;
                                }
                            }
                            var form = asset_form_io_by_task_id.Where(q => q.WOcategorytoTaskMapping_id == x.WOcategorytoTaskMapping_id).FirstOrDefault();
                            if (form != null)
                            {
                                x.status_name = form.status_name;
                            }

                        });
                    }
                    response.wo_category_task_list = task_mapping;
                }
                catch (Exception ex)
                {

                }
                #endregion WOCategoriestoTasksMappingData

                #region WOCategoriesMappingData  // categories

                response.form_category_list = new List<mobile_form_categoty_list>();
                //if sync time is not null then get categorytaskmapping by categoryids based on WOcategorytoTaskMapping_id
                List<WOcategorytoTaskMapping> wocategorytaskbycategoryids = new List<WOcategorytoTaskMapping>();
                // if (sync_time != null)
                // {
                wocategorytaskbycategoryids = _UoW.WorkOrderRepository.Getcategorytaskbycategoryids(categories.Select(x => x.wo_inspectionsTemplateFormIOAssignment_id).ToList(), sync_time);
                // asset form io
                var asset_form_io_by_task_id1 = _UoW.WorkOrderRepository.GetAssetFormBycategorytaskid(wocategorytaskbycategoryids.Select(x => x.WOcategorytoTaskMapping_id).ToList(), sync_time);
                WO_Assetformios = _mapper.Map<List<AssetFormIO>>(asset_form_io_by_task_id1);

                wocategorytaskbycategoryids.ForEach(x =>
                {
                    x.AssetFormIO = WO_Assetformios.Where(q => q.WOcategorytoTaskMapping_id == x.WOcategorytoTaskMapping_id).FirstOrDefault();
                    x.WOInspectionsTemplateFormIOAssignment = categories.Where(q => q.wo_inspectionsTemplateFormIOAssignment_id == x.wo_inspectionsTemplateFormIOAssignment_id).FirstOrDefault();
                });
                //}

                categories.ForEach(q =>
                {
                    q.WOcategorytoTaskMapping = wocategorytaskbycategoryids.Where(w => w.wo_inspectionsTemplateFormIOAssignment_id == q.wo_inspectionsTemplateFormIOAssignment_id).ToList();
                    mobile_form_categoty_list form_categoty_list = new mobile_form_categoty_list();
                    form_categoty_list.wo_inspectionsTemplateFormIOAssignment_id = q.wo_inspectionsTemplateFormIOAssignment_id;
                    form_categoty_list.form_category_name = q.InspectionsTemplateFormIO.FormIOType.form_type_name;
                    form_categoty_list.form_name = q.InspectionsTemplateFormIO.form_name;
                    ///form_categoty_list.form_data = q.InspectionsTemplateFormIO.form_data;
                   // form_categoty_list.form_description = q.Tasks != null ? q.Tasks.description : null;
                    form_categoty_list.WP = q.InspectionsTemplateFormIO.work_procedure;
                  //  form_categoty_list.wo_id = q.wo_id;
                    form_categoty_list.is_archived = q.is_archived;
                    if (q.Parent_Asset != null)
                    {
                        form_categoty_list.parent_asset_name = q.Parent_Asset.name;
                     //   form_categoty_list.parent_asset_id = q.asset_id;
                    }
                    if (q.User != null)
                    {
                        //  form_categoty_list.technician_name = q.User.firstname + q.User.lastname;
                        //    form_categoty_list.technician_id = q.User.uuid;
                       // form_categoty_list.parent_asset_id = q.asset_id;
                    }
                    form_categoty_list.status_id = q.status_id;
                    form_categoty_list.status_name = q.StatusMaster.status_name;
                    if (q.WOcategorytoTaskMapping != null && q.WOcategorytoTaskMapping.Count > 0)
                    {
                        form_categoty_list.progress_total = q.WOcategorytoTaskMapping.Where(t => !t.is_archived).Count(); // total task in category
                        var category_to_tasks = q.WOcategorytoTaskMapping.Where(t => !t.is_archived).Select(x => x.WOcategorytoTaskMapping_id).ToList();
                        var asset_form_io = asset_form_io_by_task_id1.Where(q => q.WOcategorytoTaskMapping_id != null && category_to_tasks.Contains(q.WOcategorytoTaskMapping_id.Value)).ToList();
                        if (asset_form_io != null && asset_form_io.Count > 0)
                        {
                            form_categoty_list.progress_completed = asset_form_io.Where(x => x.status == (int)Status.Completed).Count();
                        }
                        //form_categoty_list.progress_completed = q.WOcategorytoTaskMapping.Where(t => !t.is_archived).Select(x => x.AssetFormIO.status == (int)Status.Completed).Count();
                    }
                    response.form_category_list.Add(form_categoty_list);
                });
                #endregion WOCategoriesMappingData

                #region AssetData
                //FilterAssetsRequestModel requestModel = new FilterAssetsRequestModel();
                ListViewModel<AssetsResponseModel> responseModel = new ListViewModel<AssetsResponseModel>();
                //requestModel.status = (int)Status.Active;
                var assetes = _UoW.WorkOrderRepository.OfflineAssetData(sync_time);
                if (assetes?.list?.Count > 0)
                {
                    responseModel.list = _mapper.Map<List<AssetsResponseModel>>(assetes.list);
                    responseModel.list.ForEach(x =>
                    {
                        if (!String.IsNullOrEmpty(x.children))
                        {
                            x.is_child_available = true;
                        }
                    });
                }
                response.asset_list = responseModel.list;
                #endregion AssetData


                /// uncomment this when impletemtn starts
                 #region MasterTasks
                ListViewModel<MobileTaskResponseModel> taskResponse = new ListViewModel<MobileTaskResponseModel>();
                var taskDetails = _UoW.TaskRepository.GetAllTasksForWOOffline(master_form_sync_time);
                if (taskDetails?.Count > 0)
                {
                    taskResponse.listsize = taskDetails.Count;
                    taskDetails = taskDetails.ToList();
                    taskDetails.ForEach(x =>
                    {
                        x.AssetTasks = x.AssetTasks.Where(q => !q.is_archive).ToList();
                    });
                    taskResponse.list = _mapper.Map<List<MobileTaskResponseModel>>(taskDetails);
                    //var assetresponse = _mapper.Map<AssetsResponseModel>(asset);
                    response.MasterTasks = taskResponse.list;
                }

                #endregion MasterTasks


                response.force_to_reset = force_to_reset;
            }
            return response;
        }
        public ListViewModel<MobileAssetFormIOResponseModel> GetAllAssetTemplateList(string assetid, int pagesize, int pageindex)
        {
            ListViewModel<MobileAssetFormIOResponseModel> templateModel = new ListViewModel<MobileAssetFormIOResponseModel>();
            try
            {
                var templateRequests = _UoW.MobileWorkOrderRepository.GetAllAssetTemplateList(assetid, pagesize, pageindex);
                var list_form_ids = templateRequests.Item1.Select(x => x.form_id.Value).Distinct().ToList();
              //  var forms = _UoW.formIORepository.GetFormsByIds(list_form_ids);
                if (templateRequests.Item1?.Count > 0)
                {
                    templateModel.list = _mapper.Map<List<MobileAssetFormIOResponseModel>>(templateRequests.Item1);

                    foreach (var temp in templateModel.list)
                    {
                        /* try
                         {
                             var form = forms.Where(x => x.form_id == temp.form_id).FirstOrDefault();
                             if (form != null)
                             {
                                 // if(temp.asset_form_id.ToString() == "acd6eb93-3d06-4c62-9d53-c9a8d7f227c6")
                                 //{

                                 //}
                                 var inspection_form_data = JsonSerializer.Deserialize<FormioDynamicDataobj.Root>(temp.asset_form_data);
                                 //Newtonsoft.Json.JsonConvert.DeserializeObject(temp.asset_form_data);
                                 var master_form_ = JsonSerializer.Deserialize<FormioDynamicDataobj.Root>(form.form_data);
                                 //Newtonsoft.Json.JsonConvert.DeserializeObject(form.form_data);
                                 //master_form_.data = new object();
                                 master_form_.data = inspection_form_data.data;
                                 string str = JsonSerializer.Serialize(master_form_);
                                 temp.asset_form_data = str;
                             }
                         }
                         catch (Exception ex)
                         {

                         }
                        */
                        if (temp.requested_by != null && temp.requested_by != "")
                        {
                            var userDetails = _UoW.UserRepository.GetUserByID(temp.requested_by).Result;
                            if (userDetails != null)
                            {
                                temp.requested_by = userDetails.firstname + " " + userDetails.lastname;
                            }
                        }
                      /*  if (temp.accepted_by != null && temp.accepted_by != "")
                        {
                            var AcceptedbyuserDetails = _UoW.UserRepository.GetUserByID(temp.accepted_by).Result;
                            if (AcceptedbyuserDetails != null)
                            {
                                temp.accepted_by = AcceptedbyuserDetails.firstname + " " + AcceptedbyuserDetails.lastname;
                            }
                        }
                        if (temp.WorkOrder != null)
                        {
                            if (temp.WorkOrder.wo_type == (int)Status.Acceptance_Test_WO)
                            {
                                temp.wo_number = "AT" + temp.wo_number;
                            }
                            else if (temp.WorkOrder.wo_type == (int)Status.Maintenance_WO)
                            {
                                temp.wo_number = "WO" + temp.wo_number;
                            }
                        }*/

                    }
                    templateModel.listsize = templateRequests.Item2;
                    templateModel.pageIndex = pageindex;
                    templateModel.pageSize = pagesize;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return templateModel;
        }

        public ListViewModel<MobileAssetsResponseModel> GetSubAssetsByAssetID(string asset_id, int pagesize, int pageindex)
        {
            ListViewModel<MobileAssetsResponseModel> responseModel = new ListViewModel<MobileAssetsResponseModel>();
            try
            {
                var response = _UoW.MobileWorkOrderRepository.GetSubAssetsByAssetID(asset_id, pagesize, pageindex);
                if (response?.list?.Count > 0)
                {
                    responseModel.list = _mapper.Map<List<MobileAssetsResponseModel>>(response.list);
                    if (responseModel.list?.Count > 0)
                    {
                        responseModel.list.ForEach(x =>
                        {
                            
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

        public ListViewModel<MobileIssueResponseModel> FilterIssues(FilterIssueRequestModel requestModel)
        {
            ListViewModel<MobileIssueResponseModel> responseModel = new ListViewModel<MobileIssueResponseModel>();
            try
            {
                var response = _UoW.MobileWorkOrderRepository.FilterIssues(requestModel);
                if (response.Count > 0)
                {
                    int totalworkorder = response.Count;
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pageindex = 1;
                        requestModel.pagesize = 20;
                    }
                    response = response.OrderByDescending(g => g.created_at.Value).Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                    responseModel.list = _mapper.Map<List<MobileIssueResponseModel>>(response);

                    /*foreach (var workorder in responseModel.list)
                    {
                        var assetname = _UoW.AssetRepository.GetAssetByAssetID(workorder.asset_id.ToString());
                        if (assetname != null && assetname.asset_id != null && assetname.asset_id != Guid.Empty)
                        {
                            workorder.asset_name = assetname.name;
                        }
                    }*/
                    responseModel.listsize = totalworkorder;
                    responseModel.pageIndex = requestModel.pageindex;
                    responseModel.pageSize = requestModel.pagesize;
                }
                responseModel.result = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                Jarvis.Shared.Logger.Log("Error in Get All WorkOrders " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public ListViewModel<MobileInspectionResponseModel> FilterInspections(FilterInspectionsRequestModel requestModel)
        {
            ListViewModel<MobileInspectionResponseModel> responseModel = new ListViewModel<MobileInspectionResponseModel>();
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
                    responseModel.list = _mapper.Map<List<MobileInspectionResponseModel>>(response.list);
                   
                    responseModel.listsize = response.listsize;
                    responseModel.pageIndex = response.pageIndex;
                    responseModel.pageSize = response.pageSize;
                    foreach (var inspection in responseModel.list)
                    {
                        List<CategoryWiseAttributesInsepction> categoryWiseAttributesInsepctions = new List<CategoryWiseAttributesInsepction>();
                        List<CategoryWiseAttributesInsepction> newcategoryWiseAttributesInsepctions = new List<CategoryWiseAttributesInsepction>();
                        List<InspectionAttributesJsonObjectViewModel> newNotOkAttributes = new List<InspectionAttributesJsonObjectViewModel>();
                     //   var name = _UoW.InspectionRepository.FindUserNameById(inspection.operator_id);
                     //   inspection.operator_name = name;
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
        public ListViewModel<MobileAssetsResponseModel> FilterAssets(FilterAssetsRequestModel requestModel)
        {
            ListViewModel<MobileAssetsResponseModel> responseModel = new ListViewModel<MobileAssetsResponseModel>();
            try
            {
                var response = _UoW.MobileWorkOrderRepository.FilterAssets(requestModel);
                if (response?.list?.Count > 0)
                {
                    responseModel.list = _mapper.Map<List<MobileAssetsResponseModel>>(response.list);
                    responseModel.listsize = response.listsize;
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
        public async Task<MobileIssueResponseModel> GetIssueByID(string workOrderId)
        {
            MobileIssueResponseModel workOrder = new MobileIssueResponseModel();
            try
            {
                var response = _UoW.MobileWorkOrderRepository.GetIssueById(workOrderId, UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (response != null)
                {
                    workOrder = _mapper.Map<MobileIssueResponseModel>(response);

                    //foreach (var workOrder in responseModel)
                    //{

                    // New Comment
                   /* if (workOrder.asset_id != null && workOrder.asset_id != Guid.Empty)
                    {
                        var asset = _UoW.AssetRepository.GetAssetByAssetID(workOrder.asset_id.ToString());
                        var assetresponse = _mapper.Map<AssetsResponseModel>(asset);

                        workOrder.assets = assetresponse;
                        workOrder.asset_name = assetresponse.name;
                    }

                    if (workOrder.inspection_id != null && workOrder.inspection_id != Guid.Empty)
                    {
                        var inspection = _UoW.InspectionRepository.GetInspectionById(workOrder.inspection_id.ToString(), UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                        var inspectionResponse = _mapper.Map<InspectionResponseModel>(inspection);
                        workOrder.inspections = inspectionResponse;
                    }
                    */

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
                Jarvis.Shared.Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return workOrder;
        }
        public async Task<GetAssetFormIOByAssetFormIdResponsemodel> GetAssetFormIOByAssetFormId(Guid asset_form_id)
        {
            GetAssetFormIOByAssetFormIdResponsemodel formResponse = new GetAssetFormIOByAssetFormIdResponsemodel();
            try
            {
                var formDetails = await _UoW.MobileWorkOrderRepository.GetAssetFormIOByAssetFormId(asset_form_id);
                if (formDetails != null)
                {
                    var form = _UoW.formIORepository.GetFormsByIds(new List<Guid> { formDetails.form_id.Value }).FirstOrDefault();

                    formResponse = _mapper.Map<GetAssetFormIOByAssetFormIdResponsemodel>(formDetails);

                    var inspection_form_data = JsonSerializer.Deserialize<FormioDynamicDataobj.Root>(formResponse.asset_form_data);
                    //Newtonsoft.Json.JsonConvert.DeserializeObject(temp.asset_form_data);
                    var master_form_ = JsonSerializer.Deserialize<FormioDynamicDataobj.Root>(form.form_data);
                    //Newtonsoft.Json.JsonConvert.DeserializeObject(form.form_data);
                    //master_form_.data = new object();
                    master_form_.data = inspection_form_data.data;
                    string str = JsonSerializer.Serialize(master_form_);
                    formResponse.asset_form_data = str;

                    if (formDetails.WorkOrders != null)
                    {
                        if (formDetails.WorkOrders.wo_type == (int)Status.Acceptance_Test_WO)
                        {
                            formResponse.wo_number = "AT" + formDetails.WorkOrders.wo_number;
                            formResponse.manual_wo_number = "AT" + formDetails.WorkOrders.manual_wo_number;
                        }
                        else if (formDetails.WorkOrders.wo_type == (int)Status.Maintenance_WO)
                        {
                            formResponse.wo_number = "WO" + formDetails.WorkOrders.wo_number;
                            formResponse.manual_wo_number = "WO" + formDetails.WorkOrders.manual_wo_number;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw e;
            }
            return formResponse;
        }

        public ListViewModel<GetAssetPMListMobileResponsemodel> GetAssetPMList(GetAssetPMListMobileRequestmodel requestModel)
        {
            ListViewModel<GetAssetPMListMobileResponsemodel> response = new ListViewModel<GetAssetPMListMobileResponsemodel>();

            var get_asset_pm = _UoW.MobileWorkOrderRepository.GetAssetPMList(requestModel);
            if(get_asset_pm.Item1!=null && get_asset_pm.Item1.Count > 0)
            {
                response.listsize = get_asset_pm.Item2;
                response.list = _mapper.Map<List<GetAssetPMListMobileResponsemodel>>(get_asset_pm.Item1);
            }
            response.pageIndex = requestModel.pageindex;
            response.pageSize = requestModel.pagesize;
            return response;
        }

        public async Task<int> UpdateAssetPMFixStatus(UpdateAssetPMFixStatusRequestmodel requestModel)
        {
            int respnse = (int)ResponseStatusNumber.Error;
            try
            {
              
                foreach (var item in requestModel.asset_pm_list)
                {
                    var get_pm = _UoW.MobileWorkOrderRepository.GetAssetpmById(item.asset_pm_id);
                    get_pm.is_Asset_PM_fixed = item.is_Asset_PM_fixed;
                    get_pm.modified_at = DateTime.UtcNow;
                    var update = await _UoW.BaseGenericRepository<AssetPMs>().Update(get_pm);
                    _UoW.SaveChanges();
                }
                respnse = (int)ResponseStatusNumber.Success;
            }
            catch(Exception ex)
            {
                _UoW.RollbackTransaction();
            }

           
            return respnse;
        }
        public void Dispose()
        {
           
        }
    }
}
