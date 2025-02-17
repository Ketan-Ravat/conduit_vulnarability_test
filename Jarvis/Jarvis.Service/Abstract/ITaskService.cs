using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface ITaskService
    {
        Task<TaskResponseModel> AddUpdateTask(TaskRequestModel taskRequest);
        ListViewModel<TaskResponseModel> GetAllTasks(int pageindex, int pagesize, string searchstring);
        Task<TaskResponseModel> GetTaskByID(Guid id);
        Task<int> DeleteTaskByID(Guid id);
    }
}
