using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IPMService
    {
        Task<PMResponseModel> AddUpdatePM(AddPMRequestModel pmRequest);
        Task<ListViewModel<GetAllPMsByPlanIdResponsemodel>> GetAllPMsByPlanId(Guid pm_plan_id);
        Task<PMResponseModel> GetPMByID(Guid id);
        Task<int> DeletePMByID(Guid id);
        Task<int> createPMforOldAssetClass();
        Task<PMResponseModel> MovePM(MovePMRequestModel pmRequest);
        List<GetPMsListByAssetClassIdResponseModel> GetPMsListByAssetClassId(GetPMsListByAssetClassIdRequestModel requestModel);
        Task<ManuallyAssignAnyPMtoWOResponseModel> ManuallyAssignAnyPMtoWO(AssignPMsToAssetRequestModel requestModel);
    }
}
