using Jarvis.ViewModels;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IInspectionFormService
    {
        Task<bool> AddInspectionForm(InspectionFormRequestViewModel request);

        Task<bool> AddInspectionFormAttributes(InspectionFormAttributesRequestModel request);

        InspectionAttributeCategoryViewModel GetAttributesCategoryByID(int category_id);

        Task<ListViewModel<InspectionFormDataViewModel>> GetAllInspectionFormByCompanyId(Guid company_id);

    }
}
