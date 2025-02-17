using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IAssetPMPlansRepository
    {
        Task<int> Insert(AssetPMPlans entity);
        Task<int> Update(AssetPMPlans entity);
        Task<AssetPMPlans> GetAssetPMPlanById(Guid asset_pm_plan_id);
    }
}
