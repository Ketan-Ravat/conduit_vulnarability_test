using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IPMPlansService
    {
        Task<PMPlansResponseModel> AddUpdatePMPlan(PMPlansRequestModel pmCategoryRequest);
        Task<ListViewModel<PMPlansResponseModel>> GetAllPMPlans(Guid pm_category_id);
        Task<PMPlansResponseModel> GetPMPlanByID(Guid id);
        Task<int> DeletePMPlanByID(Guid id);
        Task<int> DuplicatePMPlan(DuplicatePMRequestModel request);
        Task<int> MarkDefaultPMPlan(MarkDefaultPMPlanRequestmodel request);
    }
}
