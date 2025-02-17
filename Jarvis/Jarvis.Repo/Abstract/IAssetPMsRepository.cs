using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.db.Models;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IAssetPMsRepository
    {
        Task<int> Insert(AssetPMs entity);
        Task<int> InsertList(IEnumerable<AssetPMs> entity);
        Task<int> Update(AssetPMs entity);
        Task<AssetPMPlans> GetAssetPMPlanById(Guid asset_pm_plan_id);
        Task<AssetPMs> GetAssetPMById(Guid asset_pm_id);
        Task<AssetPMs> GetAssetPMByIdForView(Guid asset_pm_id);
        Task<AssetPMs> GetAssetPMByIdForUpdate(Guid asset_pm_id);
        Task<List<AssetPMs>> GetAssetPMByAssetId(Guid asset_id, int filter_type);
        Task<ListViewModel<PMTriggers>> GetPendingPMItems(DashboardPendingPMItemsRequestModel requestModel);
        Task<ListViewModel<Asset>> FilterPendingPMItemsAssetIds(FilterPendingPMItemsOptionsRequestModel requestModel);
        Task<ListViewModel<AssetPMPlans>> FilterPendingPMItemsPMPlans(FilterPendingPMItemsOptionsRequestModel requestModel);
        Task<ListViewModel<AssetPMs>> FilterPendingPMItemsPMItems(FilterPendingPMItemsOptionsRequestModel requestModel);
        Task<ListViewModel<Sites>> FilterPendingPMItemsSites(FilterPendingPMItemsOptionsRequestModel requestModel);
        Task<List<PMTriggers>> GetUpComingPMs();
        Task<List<PMTriggers>> GetDueAssetPMs();
        Task<DashboardPMMetricsResponseModel> DashboardPMMetrics();
        Task<List<ServiceDealers>> GetAllServiceDealers(string searchstring);
        int GetAssetPMCountByAssetId(Guid asset_id);
        Task<List<AssetMeterHourHistory>> GetAssetMeterHourHistory(AssetMeterHourHistoryRequestModel requestModel);

        List<PMPlans> GetPMPlansByClassId(Guid inspectiontemplate_asset_class_id);
        List<AssetPMs> GetAssetPMbyIDs(List<Guid> asset_pm_id);
        (List<AssetPMs>, int) GetAssetPMList(GetAssetPMListRequestmodel requestmodel);
        (List<AssetPMListExcludeProperties>, int) GetAssetPMListOptimized(GetAssetPMListRequestmodel requestmodel);
        List<AssetPMs> GetActiveAssetPM();
        int GetCompletedAssetPM();
        List<Asset> GetStaffAssers();
        PMPlans GetStaffPMplamns(Guid inspectiontemplate_asset_class_id);

        InspectionTemplateAssetClass GetAssetclassbycode(string code);
        int GetWOLineStatusByAssetFormId(Guid asset_form_id);
        (List<AssetPMs>, int) ReturnAllOverdueAssetPM(GetAssetPMListRequestmodel requestmodel);
        string GetSiteNameById(Guid site_id);

        List<Asset> GetAllAssetForPMReport(Guid site);
        List<AssetPMs> IsAnyPMAlreadyLinked(List<Guid> asset_pm_ids);

        List<Asset> GetceccoAssets();
        PMCategory Gepmdetailforceccoscript(Guid class_id);
        List<Guid> ReturnOnlyOpenAssetPMIds(List<Guid> asset_pms_ids);

        int GetAssetPMCountBySite();

        WOOnboardingAssets GetPMWOlineById(Guid woonboardingassets_id);
        WOOnboardingAssets GetPMWOlineForDetails(Guid woonboardingassets_id);
        List<AssetPMs> GetAllAssetPMsForDueDateScript();
        bool CheckIfAssetPMIsFromOtherSite(List<Guid> asset_pms_ids);
        List<Guid> ReturnAllOpenAssetPMsAssetIds(List<Guid> asset_pms_ids);
        List<Asset> GetAllAssetsForAssetPMsDueDateScript();
        List<AssetPMs> GetAssetPMsByAssetId(Guid asset_id);
        int GetAssetPMsCountByAssetIdPMId(Guid asset_id, Guid pm_id);
        List<Guid> GetAssetPMsIdsByAssetIdPMId(Guid asset_id, Guid pm_id);
        DateTime? GetLastAssetPMStartDateByAssetIdPMId(Guid asset_id, Guid pm_id);
        List<AssetPMs> GetAssetPMsByAssetIdPMId(Guid asset_id, Guid pm_id,Guid asset_pm_id);
        List<AssetPMs> GetAllAssetPMsToAdd1YrPMs();
        bool IsThisCurrentAssetPM(Guid asset_id, Guid pm_id, Guid asset_pm_id);
        //string GetLastCompletedAssetPMFormJson(Guid pm_id);
        string GetLastCompletedAssetPMFormJson(Guid pm_id,Guid asset_id);
        int GetAssetPMCountByYear(DateTime datetime_starting_at, Guid asset_id, Guid pm_id);
        List<AssetPMs> GetCurrentAssetPMsByAssetId_2(Guid asset_id);
        string GetAssetPMCountByDueDate(DateTime due_date, Guid asset_id, Guid pm_id);
        List<Guid> GetAssetsListPMScript();
        User GetRequestedUser(Guid user_id);
        string GetRequestedSite(Guid site_id);


    }
}
