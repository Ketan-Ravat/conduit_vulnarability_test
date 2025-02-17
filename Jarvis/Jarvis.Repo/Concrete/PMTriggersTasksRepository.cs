using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete
{
    public class PMTriggersTasksRepository : BaseGenericRepository<PMTriggersTasks>, IPMTriggersTasksRepository
    {
        public PMTriggersTasksRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }
        public virtual async Task<int> Update(PMTriggersTasks entity)
        {
            int IsSuccess = 0;
            try
            {
                dbSet.Update(entity);
                IsSuccess = await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public async Task<PMTriggersTasks> GetTaskByID(Guid trigger_task_id)
        {
            return await context.PMTriggersTasks.Where(x => x.trigger_task_id == trigger_task_id).FirstOrDefaultAsync();
        }
    }
}
