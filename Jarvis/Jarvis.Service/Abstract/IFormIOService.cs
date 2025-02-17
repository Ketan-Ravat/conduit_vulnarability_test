using Jarvis.db.Models;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IFormIOService
    {
        public ListViewModel<GetAllFormIOFormResponsemodel> GetAllForms(int page_size, int page_index , string search_string);
        Task<FormIOResponseModel> AddUpdateFormIO(AddFormIORequestModel formRequest);

        FormIOPIChartCountResponseModel DashboardPIchartcount();
        DashboardPropertiescountsResponseModel DashboardPropertiescounts();
        ListViewModel<FormIOResponseModel> GetAllFormNames(int page_size, int page_index);
        Task<ListViewModel<FormTypeResponseModel>> GetAllFormTypes(int pageindex, int pagesize, string searchstring);
        Task<int> DeleteForm(DeleteFormRequestmodel requestmodel);

        GetFormDataTemplateByFormIdResponsemodel GetFormDataTemplateByFormId(Guid form_id);
    }
}
