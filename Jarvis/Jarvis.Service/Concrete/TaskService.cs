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
    public class TaskService : BaseService, ITaskService
    {
        public readonly IMapper _mapper;
        private Logger _logger;

        public TaskService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            _logger = Logger.GetInstance<TaskService>();
        }

        public async Task<TaskResponseModel> AddUpdateTask(TaskRequestModel taskRequest)
        {
            TaskResponseModel taskResponse = new TaskResponseModel();
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    if (taskRequest.task_id != null && taskRequest.task_id != Guid.Empty)
                    {
                        var taskDetails = await _UoW.TaskRepository.GetTaskById(taskRequest.task_id);
                        if(taskDetails != null)
                        {
                            taskDetails.modified_by = GenericRequestModel.requested_by.ToString();
                            taskDetails.modified_at = DateTime.UtcNow;
                            taskDetails.task_title = taskRequest.task_title;
                            taskDetails.task_est_hours = taskRequest.task_est_hours;
                            taskDetails.task_est_minutes = taskRequest.task_est_minutes;
                            taskDetails.task_est_display = String.Concat(
                                taskDetails.task_est_hours > 0 && taskDetails.task_est_hours > 1 ? taskDetails.task_est_hours.ToString() + " hours " : ""
                                , taskDetails.task_est_hours > 0 && taskDetails.task_est_hours == 1 ? taskDetails.task_est_hours.ToString() + " hour " : ""
                                , taskDetails.task_est_minutes > 0 && taskDetails.task_est_minutes > 1 ? taskDetails.task_est_minutes.ToString() + " minutes " : ""
                                , taskDetails.task_est_minutes > 0 && taskDetails.task_est_minutes == 1 ? taskDetails.task_est_minutes.ToString() + " minute " : "");
                            taskRequest.task_est_display = taskRequest.task_est_display?.Trim();
                            taskDetails.hourly_rate = taskRequest.hourly_rate;
                            taskDetails.description = taskRequest.description;
                            taskDetails.notes = taskRequest.notes;
                            taskDetails.form_id = taskRequest.form_id;
                            #region Asset Tasks

                          /*  foreach (var tasks in taskRequest.AssetTasks)
                            {
                                AssetTasks assetTasks = new AssetTasks();
                                if (tasks.asset_task_id == null || tasks.asset_task_id == Guid.Empty)
                                {
                                    var alreadyexist = taskDetails.AssetTasks.Where(x => x.task_id == tasks.task_id && x.asset_id == tasks.asset_id).FirstOrDefault();
                                    if (alreadyexist != null && alreadyexist.asset_task_id != null && alreadyexist.asset_task_id != Guid.Empty)
                                    {
                                        alreadyexist.status = (int)Status.Active; 
                                        
                                        if (alreadyexist.is_archive == true)
                                        {
                                            alreadyexist.modified_at = DateTime.UtcNow;
                                            alreadyexist.modified_by = GenericRequestModel.requested_by.ToString();
                                            alreadyexist.is_archive = false;
                                        }
                                    }
                                    else
                                    {
                                        assetTasks.asset_id = tasks.asset_id;
                                        assetTasks.status = (int)Status.Active;
                                        assetTasks.created_at = DateTime.UtcNow;
                                        assetTasks.is_archive = false;
                                        assetTasks.created_by = GenericRequestModel.requested_by.ToString();
                                        assetTasks.modified_at = DateTime.UtcNow;
                                        taskDetails.AssetTasks.Add(assetTasks);
                                    }
                                }
                            }

                            var assettasks = taskDetails.AssetTasks.Where(x => x.task_id == taskRequest.task_id).ToList();

                            var removeTasks = assettasks.Where(p => !taskRequest.AssetTasks.Any(p2 => p2.task_id == p.task_id) && p.is_archive == false).ToList();

                            if (removeTasks.Count > 0)
                            {
                                removeTasks.ForEach(x => x.is_archive = true);
                            }

                            taskDetails.asset_id = taskRequest.AssetTasks.FirstOrDefault().asset_id;
                            */
                            #endregion

                            result = _UoW.TaskRepository.Update(taskDetails).Result;
                            if (result > 0)
                            {
                                _dbtransaction.Commit();
                                taskResponse = _mapper.Map<TaskResponseModel>(taskDetails);
                            }
                            else
                            {
                                _dbtransaction.Rollback();
                            }
                        }
                        taskResponse.response_status = result;
                    }
                    else
                    {
                        taskRequest.created_by = GenericRequestModel.requested_by.ToString();
                        taskRequest.created_at = DateTime.UtcNow;
                        taskRequest.company_id = Guid.Parse(GenericRequestModel.company_id);
                        taskRequest.task_est_display = String.Concat(
                                taskRequest.task_est_hours > 0 && taskRequest.task_est_hours > 1 ? taskRequest.task_est_hours.ToString() + " hours " : ""
                                , taskRequest.task_est_hours > 0 && taskRequest.task_est_hours == 1 ? taskRequest.task_est_hours.ToString() + " hour " : ""
                                , taskRequest.task_est_minutes > 0 && taskRequest.task_est_minutes > 1 ? taskRequest.task_est_minutes.ToString() + " minutes " : ""
                                , taskRequest.task_est_minutes > 0 && taskRequest.task_est_minutes == 1 ? taskRequest.task_est_minutes.ToString() + " minute " : "");
                        taskRequest.task_est_display = taskRequest.task_est_display?.Trim();

                        var task = _mapper.Map<Tasks>(taskRequest);
                        task.created_at = DateTime.UtcNow;
                        /*
                        taskRequest.AssetTasks.ToList().ForEach(x =>
                        {
                            x.is_archive = false;
                            x.status = (int)Status.Active;
                            x.created_at = DateTime.UtcNow;
                            x.modified_at = DateTime.UtcNow;
                            x.created_by = GenericRequestModel.requested_by.ToString();
                        });



                        task.asset_id = taskRequest.AssetTasks.FirstOrDefault().asset_id;
                         */

                        task.form_id = taskRequest.form_id;
                        result = await _UoW.TaskRepository.Insert(task);
                        if (result > 0)
                        {
                            _UoW.SaveChanges();
                            _dbtransaction.Commit();
                            taskResponse = _mapper.Map<TaskResponseModel>(task);
                            taskResponse.response_status = result;
                        }
                        else
                        {
                            taskResponse.response_status = result;
                        }
                    }

                }
                catch (Exception ex)
                {
                    _dbtransaction.Rollback();
                    taskResponse.response_status = (int)ResponseStatusNumber.Error;
                }
            }

            return taskResponse;
        }

        public  ListViewModel<TaskResponseModel> GetAllTasks(int pageindex, int pagesize, string searchstring)
        {
            ListViewModel<TaskResponseModel> taskResponse = new ListViewModel<TaskResponseModel>();
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var taskDetails =  _UoW.TaskRepository.GetAllTasks(searchstring);
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
                        taskResponse.list = _mapper.Map<List<TaskResponseModel>>(taskDetails);
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

        public async Task<TaskResponseModel> GetTaskByID(Guid id)
        {
            TaskResponseModel taskResponse = new TaskResponseModel();
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var taskDetails = await _UoW.TaskRepository.GetTaskById(id);
                    if (taskDetails?.task_id != null)
                    {
                        taskResponse = _mapper.Map<TaskResponseModel>(taskDetails);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return taskResponse;
        }

        public async Task<int> DeleteTaskByID(Guid id)
        {
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var existTasksLinkedCount = await _UoW.TaskRepository.GetTaskLinkedCount(id);
                    if (existTasksLinkedCount == 0)
                    {
                        var taskDetails = await _UoW.TaskRepository.GetTaskById(id);
                        if (taskDetails?.task_id != null)
                        {
                            taskDetails.isArchive = true;
                            taskDetails.modified_at = DateTime.UtcNow;
                            taskDetails.modified_by = GenericRequestModel.requested_by.ToString();
                            taskDetails.AssetTasks.ToList().ForEach(x =>
                            {
                                x.is_archive = true;
                                x.status = (int)Status.Active;
                                x.modified_at = DateTime.UtcNow;
                                x.modified_by = GenericRequestModel.requested_by.ToString();
                            });
                            result = _UoW.TaskRepository.Update(taskDetails).Result;
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
                    else
                    {
                        result = (int)ResponseStatusNumber.TaskInUse;
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
    }
}
