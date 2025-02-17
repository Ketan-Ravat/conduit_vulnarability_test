using Jarvis.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IFormAttributesService
    {
        Task<bool> AddFormAttributes(FormAttributesViewModel request);
        List<FormAttributesViewModel> GetAllInspectionAttributes();

        List<InspectionAttributeCategoryViewModel> GetAllInspectionAttributsCategory();
    }
}
