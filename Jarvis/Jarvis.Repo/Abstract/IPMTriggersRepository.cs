using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IPMTriggersRepository
    {
        Task<int> Insert(PMTriggers entity);
        Task<int> InsertList(IEnumerable<PMTriggers> entity);
        Task<int> Update(PMTriggers entity);
        Task<List<PMTriggers>> GetAssetPMTriggers(Guid asset_pm_id);

        Task<PMTriggers> GetActiveAssetPMTriggers(Guid asset_pm_id);

        Task<PMTriggers> GetTriggerByID(Guid trigger_id);
        Task<List<PMTriggers>> GetAllTriggers();
    }
}
