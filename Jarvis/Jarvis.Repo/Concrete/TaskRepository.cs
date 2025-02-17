using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels.RequestResponseViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete
{
    public class TaskRepository : BaseGenericRepository<Tasks>, ITaskRepository
    {
        public TaskRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }
        public virtual async Task<int> Insert(Tasks entity)
        {
            int IsSuccess;
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                else
                {
                    Add(entity);
                    IsSuccess = (int)ResponseStatusNumber.Success;
                }
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public virtual async Task<int> Update(Tasks entity)
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

        public async Task<Tasks> GetTaskById(Guid pm_task_id)
        {
            return await context.Tasks.Where(u => u.task_id == pm_task_id)
                .Include(x => x.AssetTasks).ThenInclude(x => x.Asset)
                .FirstOrDefaultAsync();
        }

        public List<Tasks> GetAllTasks(string searchstring)
        {
            IQueryable<Tasks> query =  context.Tasks;
            query = query.Where(x => (x.company_id.ToString() == GenericRequestModel.company_id || x.FormIO.is_master_form) && x.FormIO.status == (int)Status.Active).Include(x=>x.Asset);
            if (!String.IsNullOrEmpty(searchstring))
            {
                searchstring = searchstring.ToLower();
                query = query.Where(x => !x.isArchive);

                query = query.Where(x => 
                   x.task_title.ToLower().Contains(searchstring)
                || x.task_code.ToString().Contains(searchstring)
                || x.description.ToLower().Contains(searchstring)
                || x.Asset.name.ToLower().Contains(searchstring)
                || x.Asset.internal_asset_id.ToLower().Contains(searchstring)
                || x.FormIO.form_name.ToLower().Contains(searchstring)                
                );
               
            }
            else
            {
                query = query.Where(x => (!x.isArchive) );
            }
            return query
                .Include(x=>x.Asset)
                .Include(x => x.AssetTasks)
                    .ThenInclude(x => x.Asset)
                .Include(x => x.FormIO)
                  .ThenInclude(x=>x.FormIOType)
                .OrderByDescending(x => x.created_at).ToList();
        }

        public List<Tasks> GetAllTasksForWOOffline(DateTime? sync_date)
        {
            IQueryable<Tasks> query = context.Tasks;
            query = query.Where(x => (x.company_id.ToString() == GenericRequestModel.company_id || x.FormIO.is_master_form) && x.FormIO.status == (int)Status.Active).Include(x => x.Asset);

            if (sync_date == null)
            {
                query = query.Where(x => (!x.isArchive));
            }
            else
            {
                query = query.Where(x => x.created_at.Value >= sync_date.Value || x.modified_at.Value >= sync_date.Value);
            }
            return query
                .Include(x => x.Asset)
                .Include(x => x.AssetTasks)
                    .ThenInclude(x => x.Asset)
                .Include(x => x.FormIO)
                  .ThenInclude(x => x.FormIOType)
                .OrderByDescending(x => x.created_at).ToList();
        }

        public async Task<int> GetTaskLinkedCount(Guid task_id)
        {
            /*IQueryable<PMTriggersTasks> pmTask = context.PMTriggersTasks.Where(x => x.task_id == task_id && !x.is_archive);
            IQueryable<AssetPMTasks> assetPMTask = context.AssetPMTasks.Where(x => x.task_id == task_id && !x.is_archive);
            IQueryable<WorkOrderTasks> workorderPMTask = context.WorkOrderTasks.Where(x => x.task_id == task_id && !x.is_archive);
            if (pmTask.Count() > 0 || assetPMTask.Count() > 0 || workorderPMTask.Count() > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }*/

            var linked_task = context.WOcategorytoTaskMapping.Where(x => x.task_id == task_id).ToList();
            if (linked_task.All(x=>x.is_archived))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        
    }
}
