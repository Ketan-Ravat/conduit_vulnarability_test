using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IAssetPMService
    {
        Task<PMPlansResponseModel> AddAssetPM(AssignPMToAsset AssignPM);
        Task<AssetPMResponseModel> GetAssetPMByID(Guid id);
        Task<ListViewModel<AssetPMResponseModel>> GetAssetPMByAssetID(Guid asset_id, int filter_type);
        Task<AssetPMResponseModel> UpdateAssetPM(UpdateAssetPMRequestModel pmRequest);
        Task<AssetPMResponseModel> DuplicateAssetPM(UpdateAssetPMRequestModel pmRequest);
        Task<int> DeleteAssetPM(Guid id);
        Task<int> RemoveAssetPMPlan(Guid id);
        Task<int> MarkCompletedPM(PMMarkCompletedRequestModel pmRequest);
        Task<int> UpdateTriggerStatus();
        Task<int> UpdateTaskStatus(PMTriggerTaskRequestModel requestModel);
        Task<ListViewModel<DashboardPendingPMItems>> DashboardPendingPMItems(DashboardPendingPMItemsRequestModel requestModel);
        Task<ListViewModel<AssetListResponseModel>> FilterPendingPMItemsAssetIds(FilterPendingPMItemsOptionsRequestModel requestModel);
        Task<ListViewModel<PMPlansResponseModel>> FilterPendingPMItemsPMPlans(FilterPendingPMItemsOptionsRequestModel requestModel);
        Task<ListViewModel<AssetPMResponseModel>> FilterPendingPMItemsPMItems(FilterPendingPMItemsOptionsRequestModel requestModel);
        Task<ListViewModel<SitesViewModel>> FilterPendingPMItemsSites(FilterPendingPMItemsOptionsRequestModel requestModel);
        Task<List<UpComingPMs>> UpComingPMItems(int filter_type);
        Task<UpComingPMs> UpComingPMItemsWeekly();
        Task SendOverDuePMReportToExecutive();
        Task<DashboardPMMetricsResponseModel> DashboardPMMetrics();
        Task<ListViewModel<ServiceDealerViewModel>> GetAllServiceDealers(string searchstring, int pageindex, int pagesize);
        Task<ListViewModel<AssetMeterHourHistoryResponseModel>> GetAssetMeterHourHistory(AssetMeterHourHistoryRequestModel requestModel);
        List<GetPMPlansByClassIdResponsemodel> GetPMPlansByClassId(Guid inspectiontemplate_asset_class_id);
        int LinkPMToWOLine(LinkPMToWOLineRequestmodel requestmodel);
        ListViewModel<GetAssetPMListResponsemodel> GetAssetPMList(GetAssetPMListRequestmodel requestmodel);
        ListViewModel<GetAssetPMListOptimizedResponsemodel> GetAssetPMListOptimized(GetAssetPMListRequestmodel requestmodel);
        Task<int> MarkPMcompletedNewflow(MarkPMcompletedNewflowRequestmodel requestmodel);

        AssetPMCountResponsemodel AssetPMCount();
        Task<int> assignpmtoassetcecco();
        Task<int> AddUpdatePMItemsMasterData(AddUpdatePMItemsMasterDataRequestModel requestModel);
        ListViewModel<GetAssetPMListAssetWiseResponsemodel> GetAssetPMListAssetWise(GetAssetPMListRequestmodel requestmodel);
        GetFilterDropdownAssetPMListResponseModel GetFilterDropdownAssetPMList(GetAssetPMListRequestmodel requestmodel);
        ListViewModel<PMLastCompletedDateReportResponsemodel> PMLastCompletedDateReport(PMLastCompletedDateReportRequestmodel requestmodel);

        Task<int> BulkCreatePMWOline(BulkCreatePMWOlineRequestmodel requestmodel);
        Task<int> BulkUpdatePMLastcompleted(BulkupdatePMLastcompletedRequestModel requestmodel);

        Task<int> AssignPMtoCeccoSiteScript();
        Task<int> AddPMbySteps(AddPMbyStepsRequestmodel request);
        GetPMWOlineByIDStepsResponsemodel GetPMWOlineByIDSteps(GetPMWOlineByIDStepsRequestmodel request);
        Task<int> ScriptForToUpdateAssetPMsDueDateDueInDueFlag();
        Task<int> AutomateScriptForToUpdateAssetPMsDueDateDueInDueFlag();
        Task<int> BulkCreateIRAssetPMsAssetInIRWO(BulkCreatePMWOlineRequestmodel requestmodel);
        Task<int> EnableDisableAssetPMsStatus(EnableDisableAssetPMsStatusRequestModel requestModel);
        Task<int> UpdateWorkStartDateOnEditOpenWOLine(Guid woonboardingassets_id);
        Task<int> ScriptToAdd1YearAssetPMs();
    }
} 
