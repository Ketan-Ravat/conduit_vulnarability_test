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
    public class AssetPMPlansRepository : BaseGenericRepository<AssetPMPlans>, IAssetPMPlansRepository
    {
        public AssetPMPlansRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }
        public virtual async Task<int> Insert(AssetPMPlans entity)
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

        public virtual async Task<int> Update(AssetPMPlans entity)
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

        public async Task<AssetPMPlans> GetAssetPMPlanById(Guid asset_pm_plan_id)
        {
            return await context.AssetPMPlans.Where(u => u.asset_pm_plan_id == asset_pm_plan_id)
                .Include(x => x.AssetPMs).ThenInclude(x => x.AssetPMTasks).Include(x => x.StatusMaster).OrderBy(x => x.created_at).FirstOrDefaultAsync();
        }
    }
}
