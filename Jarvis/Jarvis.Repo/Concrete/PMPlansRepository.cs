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
    public class PMPlansRepository : BaseGenericRepository<PMPlans>, IPMPlansRepository
    {
        public PMPlansRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }
        public virtual async Task<int> Insert(PMPlans entity)
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

        public virtual async Task<int> Update(PMPlans entity)
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

        public async Task<PMPlans> GetPMPlanById(Guid pm_plan_id)
        {
            return await context.PMPlans.Where(u => u.pm_plan_id == pm_plan_id)
                .Include(x => x.PMCategory).Include(x => x.PMs).ThenInclude(x=>x.PMTasks)
                .Include(x => x.PMs).ThenInclude(x => x.PMAttachments)
                .Include(x => x.StatusMaster).FirstOrDefaultAsync();
        }
        public PMPlans GetPMPlanByIdForDefault(Guid pm_plan_id)
        {
            return  context.PMPlans.Where(u => u.pm_plan_id == pm_plan_id)
                .FirstOrDefault();
        }

        public PMPlans GetPMPlanByIdtoRemoveDefault(Guid pm_category_id)
        {
            return context.PMPlans.Where(u => u.pm_category_id == pm_category_id && u.is_default_pm_plan)
                .FirstOrDefault();
        }

        public async Task<PMPlans> GetPMPlanByIdToAddinAsset(Guid pm_plan_id)
        {
            return await context.PMPlans.Where(u => u.pm_plan_id == pm_plan_id)
                .Include(x => x.PMCategory)
                .Include(x => x.PMs)
                .Include(x => x.PMs).ThenInclude(x => x.PMAttachments)
                .Include(x => x.PMs).ThenInclude(x => x.PMsTriggerConditionMapping)
                .Include(x => x.StatusMaster).FirstOrDefaultAsync();
        }

        public async Task<List<PMPlans>> GetAllPMPlans(Guid pm_category_id)
        {
            return await context.PMPlans.Where(u => u.pm_category_id == pm_category_id && u.status == (int)Status.Active)
                //.Include(x => x.PMCategory)
                .Include(x => x.PMs).OrderBy(x=>x.created_at).ToListAsync();
        }

        public AssetPMPlans GetAssetPlansByMasterPlan(Guid id)
        {
            return context.AssetPMPlans
                .Include(x=>x.AssetPMs)
                .Where(x => x.pm_plan_id == id && x.AssetPMs.Any(w => w.status != (int)Status.Completed && !w.is_archive))
                .FirstOrDefault();
        }
    }
}
