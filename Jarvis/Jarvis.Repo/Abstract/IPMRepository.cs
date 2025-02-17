using Jarvis.db.Models;
using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IPMRepository
    {
        Task<int> Insert(PMs entity);
        Task<int> InsertList(IEnumerable<PMs> entity);
        Task<int> Update(PMs entity);
        Task<PMs> GetPMsById(Guid pm_id);
        Task<List<PMs>> GetAllPMsByPlan(Guid pm_plan_id);
        List<Issue> get_cvs_issue();
        List<Asset> Getallassetstovalidatejson();
        List<PMs> GetPMsListByAssetClassId(GetPMsListByAssetClassIdRequestModel requestModel);
        List<PMPlans> GetPMPlansListByPMIds(List<Guid> pm_ids);
        AssetPMPlans CheckForPMPlanIsAny(Guid pm_plan_id,Guid asset_id);
        string GetAssetClassCodeByAssetId(GetPMsListByAssetClassIdRequestModel requestModel);
    }
}
