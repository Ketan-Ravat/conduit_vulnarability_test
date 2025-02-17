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
    public class PMTriggersRepository : BaseGenericRepository<PMTriggers>, IPMTriggersRepository
    {
        public PMTriggersRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }
        public virtual async Task<int> Insert(PMTriggers entity)
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

        public virtual async Task<int> InsertList(IEnumerable<PMTriggers> entity)
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
                    AddRange(entity);
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

        public virtual async Task<int> Update(PMTriggers entity)
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

        public async Task<List<PMTriggers>> GetAssetPMTriggers(Guid asset_pm_id)
        {
            return await context.PMTriggers.Where(u => u.asset_pm_id == asset_pm_id)
                .Include(x => x.PMTriggersTasks).Include(x => x.StatusMaster).ToListAsync();
        }

        public async Task<PMTriggers> GetActiveAssetPMTriggers(Guid asset_pm_id)
        {
            return await context.PMTriggers.Where(u => u.asset_pm_id == asset_pm_id && !u.is_archive && u.status != (int)Status.TriggerCompleted)
                .Include(x => x.PMTriggersTasks).Include(x => x.StatusMaster).FirstOrDefaultAsync();
        }

        public async Task<PMTriggers> GetTriggerByID(Guid trigger_id)
        {
            return await context.PMTriggers.Where(u => u.pm_trigger_id == trigger_id)
                .Include(x => x.PMTriggersTasks).Include(x => x.PMTriggersRemarks).Include(x => x.AssetPMs).ThenInclude(x => x.Asset).Include(x => x.AssetPMs.AssetPMTasks).Include(x => x.StatusMaster).FirstOrDefaultAsync();
        }

        public async Task<List<PMTriggers>> GetAllTriggers()
        {
            return await context.PMTriggers.Where(x => !x.is_archive && x.status != (int)Status.TriggerCompleted).Include(x => x.PMTriggersTasks).Include(x => x.AssetPMs)
                .ThenInclude(x => x.Asset).Include(x => x.AssetPMs.AssetPMTasks).Include(x => x.StatusMaster).ToListAsync();
        }
    }
}
