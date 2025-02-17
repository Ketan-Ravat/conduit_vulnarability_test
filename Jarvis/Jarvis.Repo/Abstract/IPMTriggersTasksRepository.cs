using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IPMTriggersTasksRepository
    {
        Task<int> Update(PMTriggersTasks entity);
        Task<PMTriggersTasks> GetTaskByID(Guid trigger_task_id);
    }
}
