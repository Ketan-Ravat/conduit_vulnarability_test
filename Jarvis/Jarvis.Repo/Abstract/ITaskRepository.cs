using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface ITaskRepository
    {
        Task<int> Insert(Tasks entity);
        Task<int> Update(Tasks entity);
        Task<Tasks> GetTaskById(Guid pm_task_id);
        List<Tasks> GetAllTasks(string searchstring);
        List<Tasks> GetAllTasksForWOOffline(DateTime? sync_date);
        Task<int> GetTaskLinkedCount(Guid task_id);
       
    }
}
