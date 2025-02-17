using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IMaintenanceRequestService
    {
        Task<MRResponseModel> AddUpdateMaintenanceReq(AddMRRequestModel pmRequest);
        Task<ListViewModel<MRResponseModel>> GetAllMaintenanceRequest(GetAllMRRequestModel requestModel);
        Task<GetOpenMRCount> MaintenanceRequestOpenStatusCount();
        Task<int> CancelMaintencanceRequest(MRCancelRequestModel cancelModel);
        Task<ListViewModel<WorkOrderResponseModel>> GetAllWorkOrderWithNoMR(string assetid, string searchstring, int pageindex, int pagesize);
        Task<MRResponseModel> GetMaintenanceRequestById(Guid mr_id);
        Task<int> ResolveMaintenanceRequest(ResolveMaintenanceRequestModel requestModel);
        //Task<ListViewModel<MRResponseModel>> FilterTypeOptions(FilterMaintenanceRequestModel requestModel);
        Task<ListViewModel<MRResponseModel>> FilterRequestedByOptions(FilterMaintenanceRequestModel requestModel);
    }
}
