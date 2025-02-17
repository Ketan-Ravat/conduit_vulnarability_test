using Amazon.Runtime.Internal.Util;
using Jarvis.db.DBResponseModel;
using Jarvis.db.ExcludePropertiesfromDBHelper;
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
    public interface IAssetRepository
    {
        void AddAssets(List<Asset> assets);

        void AddAsset(Asset asset);

        Task<int> Insert(Asset entity);
        Task<int> Update(Asset entity);

        Asset GetAssetsByInternalAssetID(string internal_asset_id);

        Asset GetCompanyAssetsByInternalAssetID(string internal_asset_id, string company_id);

        List<Asset> GetAllAssets(string userid, int status, int pagesize, int pageindex);
        ListViewModel<Asset> GetSubAssetsByAssetID(string asset_id, int pagesize, int pageindex);
        ListViewModel<Asset> GetChildrenByAssetID(string asset_id, int pagesize, int pageindex);

        (ListViewModel<Asset>, List<AssetFormIOBuildingMappings>) FilterAssets(FilterAssetsRequestModel requestModel);

        List<Asset> FilterAssetNameOptions(FilterAssetsOptionsRequestModel requestModel);

        List<string> FilterAssetModelOptions(FilterAssetsOptionsRequestModel requestModel);

        List<string> FilterAssetModelYearOptions(FilterAssetsOptionsRequestModel requestModel);

        List<int> FilterAssetStatusOptions(FilterAssetsOptionsRequestModel requestModel);

        List<Sites> FilterAssetSitesOptions(FilterAssetsOptionsRequestModel requestModel);

        List<Company> FilterAssetCompanyOptions(FilterAssetsOptionsRequestModel requestModel);

        Asset GetAsset(int pagesize, int pageindex, GetAssetsByIdRequestModel requestModel);

        List<Inspection> GetAllInspections(string userid, int pagesize = 0, int pageindex = 0);

        Task<List<Inspection>> GetAllCheckedOutAssets(string userid, int pagesize, int pageindex);

        bool AddAssetTransactionHistory(AssetTransactionHistory assetTxnHistory);

        Asset GetAssetByID(string userid, string asset_id);
        Asset GetAssetByIDWithoutStatusCheck(string userid, string asset_id);
        Asset GetAssetByIDFromInternalCall(string asset_id);
        Asset GetAssetByInternalID(string userid, string internal_asset_id);

        Asset GetAssetByAssetID(string asset_id);
        Asset GetduplicateAssetbyQR(string qr_code, Guid asset_id);
        Asset GetduplicateAssetbyQR_CreateAsset(string qr_code);
        Asset GetAssetByQRcode(string QR_code);

        BaseViewModel CheckAssetIntoInspectionByAssetID(string barcode_id, string userid);

        BaseViewModel CheckAssetIntoInspectionByInternalAssetId(string internal_asset_id, string userid);

        Task<List<Asset>> SearchAssets(string userid, string searchstring, int status, int pagesize, int pageindex);

        Task<int> UploadAssetPhoto(Asset asset);

        Task<List<Asset>> GetAllAssetsWithInspectionForm(string userid, string timestamp);

        Task<List<Asset>> GetAssetsWithInspection(string userid);

        Task<List<DashboardOutstandingIssues>> DashboardOutstandingIssues(string userid);

        Task<List<Inspection>> GetInspections(string userid);

        GetSyncDataResponseModel GetSyncData(string userid, DateTime? timestamp);

        AssetInspectionListResponseModel GetAssetInspectionReport(string internal_asset_id, int pagesize, int pageindex);

        List<AssetInspectionReport> GetAssetInspectionReportByID(List<Guid> reports_id);

        AssetInspectionReport GetAssetInspectionReport(Guid asset_id, DateTime from_date, DateTime to_date);

        List<Inspection> GetAllOperatorInspectionReport(string userid);
        List<User> GetAllOperatorBySiteList(string siteid);
        List<Inspection> GetAllOperatorInspectionReportByOperator(string userid);
        Inspection GetInspectionsByINSId(string inspection_id);
        Task<List<Asset>> GetAssetsWithInspectionForWeeklyEmail(string userid);
        List<Asset> GetAssetsBySiteID(Guid siteid);
        Task<List<AssetType>> GetAllAssetTypes(string searchstring);
        Task<AssetType> GetAssetTypeById(int id);
        Task<List<AssetActivityLogs>> GetActivityLogs(string asset_id, string userid, int filter_type);
        bool AddAssetMeterHoursHistory(AssetMeterHourHistory assetMeterHourHistory);

        ListViewModel<Asset> GetParentAssets(FilterAssetsRequestModel requestModel);
        ListViewModel<Asset> GetChildAssets(string parentId);
        List<Asset> GetAllRawHierarchyAssets();
        List<Asset> GetAllAssetsForCluster(String wo_id);
        List<Asset> GetAllRawHierarchyAssetsForOffline(DateTime? sync_time);

        Asset GetAssetByIDForhierarchychange(Guid asset_id);

        Asset GetAssetByInternalIDForhierarchychange(string asset_internal_id);

        List<Asset> GetAssetByIDs(List<Guid> asset_ids);
        List<Asset> GetAssetByAssetNames(List<string> asset_names);
        List<WOOnboardingAssets> GetOBWOAssetByAssetNames(List<string> asset_names, Guid? woonboardingassets_id);
        List<AssetProfileImages> GetAssetImages(List<Guid> asset_profile_images_id);
        AssetProfileImages GetAssetImagebyID(Guid asset_profile_images_id);
        List<WOOnboardingAssetsImagesMapping> GetOBAssetImages(List<Guid> asset_OB_images_id);
        WOOnboardingAssetsImagesMapping GetOBAssetImagebyID(Guid asset_OB_images_id);
        WOOBAssetFedByMapping GetOBAssetFedByID(Guid? wo_ob_asset_fed_by_id);
        IRWOImagesLabelMapping GetIRImageLabelMappingByID(Guid? irwoimagelabelmapping_id);

        User Getuserbyid(Guid user_id);
        (List<AssetNotes>, int) GetAssetNotes(GetAssetNotesRequestmodel requestModel);
        (List<AssetAttachmentMapping>, int) GetAssetAttachments(GetAssetAttachmentsRequestmodel requestModel);
        AssetAttachmentMapping GetAssetAttachmentById(Guid assetatachmentmapping_id);

        AssetNotes GetAssetnoteByID(Guid asset_notes_id);

        List<FormIOBuildings> GetBuildingsBySite();
        List<FormIOFloors> GetFloorsBySite();
        List<FormIORooms> GetRoomsBySite();
        List<FormIOSections> GetSectionBySite();

        int FilterAssetHierarchyLevelOptions();

        List<AssetChildrenHierarchyMapping> GetChildByAssetId(Guid asset_id);

        List<Asset> GetAssetforcecco();

        List<Guid> GetAssetParentsByIDs(List<Guid> asset_parent_ids);
        AssetIssueImagesMapping GetOBAssetIssueImagebyID(Guid asset_issue_image_mapping_id);

        (List<AssetSubLevelcomponentMapping>, int) GetSubcomponentsByAssetId(GetSubcomponentsByAssetIdRequestmodel requestModel);

        Asset GetsubcomonentAssetDetail(Guid sublevelcomponent_asset_id);

        AssetSubLevelcomponentMapping UpdateCircuitForAssetSubcomponent(Guid asset_sublevelcomponent_mapping_id);
        AssetSubLevelcomponentMapping DeleteAssetSubcomponent(Guid asset_sublevelcomponent_mapping_id);

        AssetTopLevelcomponentMapping GetToplevelmappingofSubcomponent(Guid asset_id);

        List<Asset> GetSubcomponentAssetsToAddinAsset();

        Asset GetAssetByIdForNewSubcomponent(Guid asset_id);

        List<AssetParentHierarchyMapping> GetParentAssetByAssetId(Guid asset_id);
        List<AssetChildrenHierarchyMapping> GetChildrenAssetByAssetId(Guid asset_id);

        AssetParentHierarchyMapping UpdateAssetFedByCircuit(Guid asset_id);
        AssetChildrenHierarchyMapping UpdateAssetFeedingCircuit(Guid asset_id);

        AssetSubLevelcomponentMapping GetAssetsSubcomponentbyid(Guid asset_id, Guid subcomponent_asset_id);

        ClusterDiagramPDFSiteMapping GetClusterDiagramPDFSiteMappingBySiteId(Guid siteId);
        (List<Asset>, int) GetAssetsLocationDetails(GetAssetsLocationDetailsRequestModel request);
        (List<Asset>, int) GetTopLevelAssetsData(GetTopLevelAssetsRequestModel request);
        string GetAssetNameByAssetId(Guid assetId);

        List<AssetParentHierarchyMapping> GetfedbyviaSubcomponent(Guid sublevelcomponent_asset_id);
        List<AssetChildrenHierarchyMapping> GetfeedingviaSubcomponent(Guid sublevelcomponent_asset_id);
        InspectionsTemplateFormIO InsertFormIOTemplate(Guid form_id);
        List<Asset> GetAssetsByListOfAssetIds(List<Guid> asset_id);

        FormIOBuildings GetBuildingForDelete(int formiobuilding_id);
        FormIOFloors GetFloorForDelete(int formiofloor_id);
        FormIORooms GetRoomForDelete(int formioroom_id);

        FormIOBuildings GetBuildingById(int formiobuilding_id);
        FormIOFloors GetFloorById(int formiofloor_id);
        FormIORooms GetRoomById(int formioroom_id);
        FormIOSections GetSectionById(int formiosection_id);
        List<AssetFormIOBuildingMappings> GetAssetsFromLocations(UpdateLocationDetailsRequestModel request);

        bool CheckForLocationNameIsExist(UpdateLocationDetailsRequestModel request);

        Asset GetSubLevelAssetById(Guid sublevelcomponent_asset_id);
        (List<WOOnboardingAssets>, int) GetOBWOAssetsOfRequestedAsset(GetOBWOAssetsOfRequestedAssetRequestModel request);
        (List<GetAllWOOBAssetsByAssetIdResponseModel>, int) GetAllWOOBAssetsByAssetId(GetOBWOAssetsOfRequestedAssetRequestModel request);
        List<WorkOrders> GetAllWOBySiteId(Guid site_id);
        TempFormIOBuildings GetTempFormIOBuildingByName(string building_name, Guid site_id, Guid wo_id);
        TempFormIOFloors GetTempFormIFloorByName(string floor_name, Guid site_id, Guid wo_id, Guid temp_buildingId);
        TempFormIORooms GetTempFormIORoomByName(string room_name, Guid site_id, Guid wo_id, Guid temp_floorId);
        TempFormIOSections GetTempFormIOSectionByName(string section_name, Guid site_id, Guid wo_id, Guid temp_roomId);
        WOOBAssetTempFormIOBuildingMapping GetWOOBAssetTempLocationMapping(Guid woonboardingassets_id);

        List<AssetChildrenHierarchyMapping> GetAssetChildren(Guid asset_id);
        List<WOOBAssetFedByMapping> GetTempAssetChildren(Guid woonboardingassets_id);

        Asset GetAssetbyIdforFeedingscircuit(Guid asset_id);
        Asset GetAssetDatabyId(string asset_id);
        List<Sites> GetAllSitesForScript();
        AssetParentHierarchyMapping GetAssetparentMapping(Guid asset_id, Guid parent_asset_id);
        List<AssetProfileImages> GetAssetProfileImagesByAssetId(Guid asset_id);
        List<IRWOImagesLabelMapping> GetIRWOImagesByAssetId(Guid asset_id);
        WOOnboardingAssets GetOBWOAssetsByAssetId(Guid asset_id, Guid wo_id);
        WOOnboardingAssets GetinstallWOlineFromTempAsset(Guid asset_id, Guid wo_id);
        List<Asset> GetTopLevelFedByAssetList();
        List<WOOnboardingAssets> GetTopLevelFedByWOOBAssetList(GetOBTopLevelFedByAssetListRequestModel requestmodel , List<Guid> child_asset);

        List<AssetIssueImagesMapping> GetAssetIssueImages(Guid asset_id);
        List<Asset> GetAllTopLevelAssetsList(Guid? wo_id);
        List<Asset> GetAllSubLevelAssetsMappingList(Guid? wo_id);
        List<WOOnboardingAssets> GetAllTopLevelOBWOAssetsList(Guid wo_id);
        List<WOOnboardingAssets> GetAllSubLevelOBWOAssetsMappingList(Guid wo_id);
        List<Asset> GetToplevelFebbyAssetlist(List<Guid> asset_id);
        string GetAssetImageByTypeAndId(Guid asset_id, int type);

        List<WOOBAssetFedByMapping> GetWOlinefedbbyMapping(Guid wo_id);

        List<WOOnboardingAssets> GetallToplevelWolines(Guid wo_id);

        List<WOOnboardingAssets> GetSublevelWOlines(List<Guid> sublevel_list);
        Asset GetAssetByNameClassCode(string asset_name, string class_code);

        List<WOOnboardingAssets> GetExistingAssetWOline(Guid wo_id);
        Asset GetAssetById(Guid asset_id);
        AssetProfileImages GetAssetProfileImageById(Guid asset_profile_images_id);
        AssetIssueImagesMapping GetAssetIssueImageById(Guid asset_issue_image_mapping_id);
        AssetIRWOImagesLabelMapping GetIRWOImagesLabelById(Guid assetirwoimageslabelmapping_id);
        List<AssetIRWOImagesLabelMapping> GetAssetIRWOImagesByAssetId(Guid asset_id);

        AssetParentHierarchyMapping GetParentMapping(Guid child_asset_id, Guid parent_asset_id);

        AssetChildrenHierarchyMapping GetChildMapping(Guid parent_asset_id, Guid child_asset_id);

        List<AssetChildrenHierarchyMapping> GetAssetChildrenByOCP(Guid sublevelcomponent_asset_id);

        AssetSubLevelcomponentMapping GetSubcomponentMapping(Guid subcomponent_asset_id);

        List<Guid> GetAssetsLinkedSubcomponents(Guid assset_id, Guid asset_children_hierrachy_id);

        List<WOOnboardingAssets> GetNewlyCreatedAssetsbyWO(Guid wo_id);

        (List<AssetListExclude>, int, List<AssetlocationHierarchyExclude>) FilterAssetOptimized(FilterAssetsRequestModel requestModel);
        (List<GetAllBuildingLocationsData>,int) GetAllBuildingLocations(int pagesize, int pageindex);
        (List<GetAllFloorsByBuildingData>, int) GetAllFloorsByBuilding(int formiobuilding_id, int pagesize, int pageindex);
        (List<GetAllRoomsByFloorData>, int) GetAllRoomsByFloor(int formiofloor_id, int pagesize, int pageindex);

        string GetViasubcomponenentCircuit(Guid via_subcomponent_asset_id);
        List<AssetParentHierarchyMapping> GetParentMappingByParentAssetId(Guid parent_asset_id);
        List<AssetTopLevelcomponentMapping> GetToplevelMappingsListByTopLevelAssetId(Guid asset_id);
        List<AssetChildrenHierarchyMapping> GetChildrenMappingByChildAssetId(Guid asset_id);
        List<AssetParentHierarchyMapping> GetParentMappingsByOcpOcpMainId(Guid asset_id);
        WOOnboardingAssetsImagesMapping GetLatestNameplateImageById(Guid woonboardingassets_id);
        AssetProfileImages GetLatestNameplateImageByAssetId(Guid asset_id);
        AssetGroup GetAssetGroupById(Guid asset_group_id);
        List<AssetGroup> AssetGroupsDropdownList();

        List<Asset> AssetListDropdownForAssetGroup(Guid asset_group_id);
        (List<GetAllAssetGroupsList_Class>,int) GetAllAssetGroupsList(GetAllAssetGroupsListRequestModel requestModel);
        (List<GetAllOnboardingAssetsByWOIdResponseModel>,int) GetAllOnboardingAssetsByWOId(GetAllOnboardingAssetsByWOIdRequestModel requestModel);
        List<asset_node_main_class> GetAllAssetsListForReactFlow();
        List<initialEdges_class> GetAllAssetsConnectionsForReactFlow();
        List<Asset> GetAllAssetsListByIds(List<Guid> asset_ids);
        List<Asset> GetAllAssetsByAssetGroupId(Guid assetGroupId);
        List<TempAsset> GetAllTempAssetsByAssetGroupId(Guid assetGroupId);
        List<Asset> GetAssetsByAssetId(List<Guid> assetIds);
        List<AssetParentHierarchyMapping> GetAssetParentHirerachyList(List<string> parentHirerachyIds);
        AssetChildrenHierarchyMapping GetAssetChildrenMapping(Guid parent_id, Guid child_id);
    }
}
