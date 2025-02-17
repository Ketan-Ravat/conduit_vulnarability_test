using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IPMPlansRepository
    {
        Task<int> Insert(PMPlans entity);
        Task<int> Update(PMPlans entity);
        Task<PMPlans> GetPMPlanById(Guid pm_plan_id);
        Task<PMPlans> GetPMPlanByIdToAddinAsset(Guid pm_plan_id);
        Task<List<PMPlans>> GetAllPMPlans(Guid pm_category_id);
        AssetPMPlans GetAssetPlansByMasterPlan(Guid id);

        PMPlans GetPMPlanByIdForDefault(Guid pm_plan_id);
         PMPlans GetPMPlanByIdtoRemoveDefault(Guid pm_category_id);
    }
}
