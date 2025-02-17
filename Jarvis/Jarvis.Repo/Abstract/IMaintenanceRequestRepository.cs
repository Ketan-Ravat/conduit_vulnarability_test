using Jarvis.db.DBResponseModel;
using Jarvis.db.Models;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IMaintenanceRequestRepository
    {
        Task<int> Insert(MaintenanceRequests entity);
        Task<int> Update(MaintenanceRequests entity);
        Task<MaintenanceRequests> GetMRsById(Guid mr_id);
        Task<ListViewModel<MaintenanceRequests>> GetAllMaintenanceRequest(GetAllMRRequestModel requestModel);
        Task<List<MaintenanceRequests>> MaintenanceRequestOpenStatusCount(string userid);
        Inspection GetInspectionIdByIssueId(Guid issueid);
        Task<List<WorkOrders>> GetAllWorkOrderWithNoMR(string assetid, string searchstring);
        Task<List<MaintenanceRequests>> GetMRsByWorkOrderId(Guid wo_id);
        //Task<ListViewModel<MaintenanceRequests>> FilterTypeOptions(FilterMaintenanceRequestModel requestModel);
        Task<ListViewModel<MaintenanceRequests>> FilterRequestedByOptions(FilterMaintenanceRequestModel requestModel);
    }
}
