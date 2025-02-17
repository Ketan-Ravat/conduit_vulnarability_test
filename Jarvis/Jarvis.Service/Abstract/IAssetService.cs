using Jarvis.db.Models;
using Jarvis.Shared;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Jarvis.Service.Concrete.AssetService;

namespace Jarvis.Service.Abstract
{
    public interface IAssetService
    {
        Task<List<string>> AddAssetsFromExcelAsync(AddAssetRequestModel request);
        //bool AddAssetsFromExcel(AddAssetRequestModel request);

        ListViewModel<AssetsResponseModel> GetAllAssets(int status, int pagesize, int pageindex);
        ListViewModel<AssetsResponseModel> GetSubAssetsByAssetID(string asset_id, int pagesize, int pageindex);
        ListViewModel<GetChildrenByAssetIDResponsemodel> GetChildrenByAssetID(string asset_id, int pagesize, int pageindex);

        FilterAssetsResponseview<AssetsResponseModel> FilterAssets(FilterAssetsRequestModel requestModel);

        ListViewModel<AssetListResponseModel> FilterAssetsNameOptions(FilterAssetsOptionsRequestModel requestModel);

        ListViewModel<string> FilterAssetsModelOptions(FilterAssetsOptionsRequestModel requestModel);

        ListViewModel<string> FilterAssetsModelYearOptions(FilterAssetsOptionsRequestModel requestModel);

        ListViewModel<int> FilterAssetsStatusOptions(FilterAssetsOptionsRequestModel requestModel);

        ListViewModel<SitesViewModel> FilterAssetsSitesOptions(FilterAssetsOptionsRequestModel requestModel);

        ListViewModel<CompanyViewModel> FilterAssetsCompanyOptions(FilterAssetsOptionsRequestModel requestModel);

        Task<AssetsResponseModel> GetAsset(int pagesize, int pageindex, GetAssetsByIdRequestModel requestModel);

        ListViewModel<InspectionResponseModel> GetAllInspections(string userid, int pagesize, int pageindex);

        Task<ListViewModel<PendingAndCheckoutInspViewModel>> GetAllCheckedOutAssets(int pagesize, int pageindex);

        int ValidateInternalAssetID(string internal_asset_id);

        Asset GetAssetDetailsByID(string asset_id);
        Asset GetAssetDetailsByQRcode(string QR_code);

        AssetViewModel GetAssetByAssetID(string asset_id);
        AssetViewModel GetAssetByIneternalID(string asset_internal_id);

        Task<ListViewModel<AssetsResponseModel>> SearchAssets(string searchstring, int status, int pagesize, int pageindex);

        Task<int> UploadAssetPhoto(UploadAssetPhotoRequestModel requestModel);
        Task<int> UploadAssetAttachments(UploadAssetAttachmentsRequestmodel requestModel);
        Task<int> UploadAssetimage(UploadAssetPhotoRequestModel requestModel);

        Task<List<AssetsResponseModel>> GetAllAssetsWithInspectionForm(string timestamp);

        Task<ListViewModel<MonthlyInspection>> GetAssetInspectionReportMonthly(DateTime start, DateTime end);

        Task<ListViewModel<WeeklyInspection>> GetAssetInspectionReportWeekly(DateTime start, DateTime end);

        Task<ListViewModel<AssetWeeklyReport>> GetAssetReportWeekly(DateTime start, DateTime end);

        Task<DashboardOutstandingIssuesResponseModel> DashboardOutstandingIssues();

        Task<ListViewModel<GetLatestMeterHoursReportResponseModel>> GetLatestMeterHoursReport();

        Task<int> UpdateMeterHours(UpdateMeterHoursRequestModel request);

        Task<SyncDataResponseModel> GetSyncData(string userid);

        ListViewModel<AssetInspectionReportResponseModel> GetAssetInspectionReport(string internal_asset_id, int pagesize, int pageindex);

        Task<AssetInspectionReportResponseModel> GenerateAssetInspectionReport(AssetInspectionReportRequestModel requestModel, DateTime from_date, DateTime to_date, string aws_access_key, string aws_secret_key);

        Task<ListViewModel<InspectionResponseModel>> GetAssetInspectionForReportView(AssetInspectionReportRequestModel requestModel);

        Task<List<AssetInspectionReportResponseModel>> GetReportStatus(ReportStatusRequestModel requestModel);

        List<AssetListResponseModel> GetAllAssetsList();

        List<string> GetAllAssetsModelsList();
        List<string> GetAllAssetsModelYearsList();

        Task<int> UpdateAssetStatus(UpdateAssetStatusRequestModel requestModel);

        Task<AssetsResponseModel> InsertUpdateAssetDetails(AssetRequestModel asset);
        Task<AssetTypeResponseModel> AddUpdateAssetType(AssetTypeRequestModel taskRequest);
        Task<ListViewModel<AssetTypeResponseModel>> GetAllAssetTypes(int pageindex, int pagesize, string searchstring);
        Task<AssetTypeResponseModel> GetAssetTypeByID(int id);
        Task<ListViewModel<AssetActivityLogsViewModel>> GetActivityLogs(string userid, int pagesize, int pageindex, string asset_id, int filter_type);

        ListViewModel<AssetsResponseModel> GetAllHierarchyAssets(FilterAssetsRequestModel requestModel);
        List<GetAllHierarchyAssetsResponseModel> GetAllRawHierarchyAssets();
        List<GetAllAssetsForClusterResponsemodel> GetAllAssetsForCluster(string wo_id);

        ListViewModel<string> GetAssetLevelOptions();
        GetNameplateInfoByAssetidResponsemodel GetNameplateInfoByAssetid(string assetid);

        Task<int> ChangeAssetHierarchy(ChangeAssetHierarchyRequestmodel requestModel);
        Task<int> UploadAssetExcelTest();
        Task<int> DeleteAssetImage(DeleteAssetImageRequestmodel requestModel);
        Task<EditAssetDetailsResponseModel> EditAssetDetails(EditAssetDetailsRequestmodel requestModel);

        int GenertaeExcel(string assetlist);

        Task<int> AddUpdateAssetNotes(AddUpdateAssetNotesRequestmodel requestmodel);

        ListViewModel<GetAssetNotesResponsemodel> GetAssetNotes(GetAssetNotesRequestmodel requestModel);

        FilterAssetBuildingLocationOptions FilterAssetBuildingLocationOptions();
        FilterAssetHierarchyLevelOptionsResponsemodel FilterAssetHierarchyLevelOptions();

        ListViewModel<GetAssetAttachmentsResponsemodel> GetAssetAttachments(GetAssetAttachmentsRequestmodel requestModel);

        Task<int> DeleteAssetAttachments(DeleteAssetAttachmentsRequestmodel requestmodel);

        ListViewModel<GetSubcomponentsByAssetIdResponsemodel> GetSubcomponentsByAssetId(GetSubcomponentsByAssetIdRequestmodel requestmodel);

        Task<int> UpdateCircuitForAssetSubcomponent(UpdateCircuitForAssetSubcomponentRequestmodel requestmodel);
        Task<int> DeleteAssetSubcomponent(DeleteAssetSubcomponentRequestmodel requestmodel);
        ListViewModel<GetSubcomponentAssetsToAddinAssetResponsemodel> GetSubcomponentAssetsToAddinAsset();

        Task<int> AddNewSubComponent(AddNewSubComponentRequestmodel requestmodel);

        GetAssetCircuitDetailsResponsemodel GetAssetCircuitDetails(GetAssetCircuitDetailsRequestmodel requestmodel);

        Task<int> UpdateAssetFedByCircuit(UpdateAssetFedByCircuitRequestmodel requestmodel);
        Task<int> UpdateAssetFeedingCircuit(UpdateAssetFeedingCircuitRequestmodel requestmodel);

        Task<int> UpdateDigitalOneLine(UpdateDigitalOneLineRequestModel requestmodel);

        GetUploadedOneLinePdfResponseModel GetUploadedOneLinePdfDataBySiteIdService(Guid siteId);
        ListViewModel<GetAssetsLocationDetailsResponseModel> GetAssetsLocationDetailsService(GetAssetsLocationDetailsRequestModel requestModel);
        ListViewModel<GetTopLevelAssetsResponseModel> GetTopLevelAssetsService(GetTopLevelAssetsRequestModel requestModel);
        Task<int> InsertFormIOTemplate();

        Task<int> ChangeSelectedAssetsLocation(ChangeSelectedAssetsLocationRequestModel requestmodel);
        Task<int> DeleteLocationDetails(DeleteLocationDetailsRequestModel requestmodel);
        Task<int> UpdateLocationDetails(UpdateLocationDetailsRequestModel requestmodel);
        ListViewModel<GetOBWOAssetsOfRequestedAssetResponseModel> GetOBWOAssetsOfRequestedAsset(GetOBWOAssetsOfRequestedAssetRequestModel requestModel);
        ListViewModel<GetAllWOOBAssetsByAssetIdResponseModel> GetAllWOOBAssetsByAssetId(GetOBWOAssetsOfRequestedAssetRequestModel requestModel);

        GetAssetFeedingCircuitForReportResponsemodel GetAssetFeedingCircuitForReport(GetAssetFeedingCircuitForReportRequestmdel request);
        Task<int> ScriptForAddTempLocations();
        GetAllImagesForAssetResponseModel GetAllImagesForAsset(Guid asset_id);
        GetLocationDataByAssetIdResponseModel GetLocationDataByAssetId(Guid asset_id);
        GetOBWOAssetDetailsByIdResponsemodel GetAssetDetailsByIdForTempAsset(GetAssetDetailsByIdForTempAsset requestModel);
        GetOBTopLevelFedByAssetListResponseModel GetOBTopLevelFedByAssetList(GetOBTopLevelFedByAssetListRequestModel requestModel);
        GetSubcomponentsForFedByMappingResponseModel GetTopLevelSubLevlComponentHiararchy(Guid? wo_id, string woonboaardingasset_id);
        Task<int> UploadBulkMainAssets(UploadBulkMainAssetsRequestModel requestmodel);
        Task<int> AssetImageDeleteOrSetAsProfile(AssetImageDeleteOrSetAsProfileRequestModel requestModel);
        FilterAssetsOptimizedResponseview<FilterAssetOptimizedResponsemodel> FilterAssetOptimized(FilterAssetsRequestModel requestModel);
        GetAllBuildingLocationsResponseModel GetAllBuildingLocations(int pagesize, int pageindex);
        GetAllFloorsByBuildingResponseModel GetAllFloorsByBuilding(int formiobuilding_id, int pagesize, int pageindex);
        GetAllFloorsByBuildingResponseModel GetAllFloorsByBuilding_Dropdown(int formiobuilding_id);
        GetAllRoomsByFloorResponseModel GetAllRoomsByFloor(int formiofloor_id, int pagesize, int pageindex);
        Task<int> UpdateAssetClassPDFUrl(Guid inspectiontemplate_asset_class_id, string pdf_report_template_url);
        Task<GetNameplateJsonFromImagesResponseModel> GetNameplateJsonFromImages(GetNameplateJsonFromImagesRequestModel requestModel);       
        Task<int> EditBulkMainAssets(EditBulkMainAssetsRequestModel requestModel);
        Task<CreateUpdateAssetGroupResponseModel> CreateUpdateAssetGroup(CreateUpdateAssetGroupRequestModel requestModel);
        AssetGroupsDropdownListResponseModel AssetGroupsDropdownList();
        AssetListDropdownForAssetGroupResponseModel AssetListDropdownForAssetGroup(Guid asset_group_id);
        GetAllAssetGroupsListResponseModel GetAllAssetGroupsList(GetAllAssetGroupsListRequestModel requestModel);
        ListViewModel<GetAllOnboardingAssetsByWOIdResponseModel> GetAllOnboardingAssetsByWOId(GetAllOnboardingAssetsByWOIdRequestModel requestModel);
        GetAllAssetsListForReactFlowResponseModel GetAllAssetsListForReactFlow();
        Task<int> UpdateAssetsPositionForReactFlow(UpdateAssetsPositionForReactFlowRequestModel requestModel);
    }
}
