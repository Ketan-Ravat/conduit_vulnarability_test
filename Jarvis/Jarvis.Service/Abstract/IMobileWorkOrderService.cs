using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IMobileWorkOrderService
    {
        ListViewModel<MobileNewFlowWorkorderListResponseModel> GetAllWorkOrdersNewflow(NewFlowWorkorderListRequestModel requestModel);
        MobileViewWorkOrderDetailsByIdResponsemodel ViewWorkOrderDetailsById(string wo_id);
        ListViewModel<MobileTaskResponseModel> GetAllTasks(int pageindex, int pagesize, string searchstring);
        List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel> GetWOcategoryTaskByCategoryID(string wo_inspectionsTemplateFormIOAssignment_id);
        List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel> GetAllWOCategoryTaskByWOid(string wo_id);
        Task<MobileGetWOsForOfflineResponsemodel> GetWOsForOffline(string userid);
        ListViewModel<MobileAssetFormIOResponseModel> GetAllAssetTemplateList(string assetid, int pagesize, int pageindex);
        ListViewModel<MobileAssetsResponseModel> GetSubAssetsByAssetID(string asset_id, int pagesize, int pageindex);
        ListViewModel<MobileIssueResponseModel> FilterIssues(FilterIssueRequestModel requestModel);
        ListViewModel<MobileInspectionResponseModel> FilterInspections(FilterInspectionsRequestModel requestModel);
        ListViewModel<MobileAssetsResponseModel> FilterAssets(FilterAssetsRequestModel requestModel);
        Task<MobileIssueResponseModel> GetIssueByID(string issueId);
        Task<GetAssetFormIOByAssetFormIdResponsemodel> GetAssetFormIOByAssetFormId(Guid asset_form_id);
        ListViewModel<GetAssetPMListMobileResponsemodel> GetAssetPMList(GetAssetPMListMobileRequestmodel requestModel);
        Task<int> UpdateAssetPMFixStatus(UpdateAssetPMFixStatusRequestmodel requestModel);
    }
}
