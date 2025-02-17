using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IPMCategoryService
    {
        Task<PMCategoryResponseModel> AddUpdatePMCategory(PMCategoryRequestModel pmCategoryRequest);
        Task<ListViewModel<PMCategoryResponseModel>> GetAllPMCategory();
        Task<PMCategoryResponseModel> GetPMCategoryByID(Guid id);
        Task<int> DeletePMCategoryByID(Guid id);
    }
}
