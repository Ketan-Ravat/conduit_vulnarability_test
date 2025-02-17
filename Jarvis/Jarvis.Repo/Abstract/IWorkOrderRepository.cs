using Jarvis.db.DBResponseModel;
using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.db.Models;
using Jarvis.ViewModels;
using Jarvis.ViewModels.EmailRequestViewModel;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Jarvis.Repo.Abstract
{
    public interface IWorkOrderRepository
    {
        Task<int> Insert(WorkOrders entity);
        Task<int> Update(WorkOrders entity);
        WorkOrders GetWorkOrderById(Guid wo_id);
        WorkOrders GetWOByidforUpdate(Guid wo_id);
        WorkOrders GetWOByidforUpdateOffline(Guid wo_id);
        (int, int) GetAssetformioThreadcount(Guid wo_id);
        (int, int) GetOBWOLineThreadcount(Guid wo_id);
        Task<ListViewModel<WorkOrders>> GetAllWorkOrders(GetAllWorkOrderRequestModel requestModel);
        List<Issue> GetNewIssuesListByAssetId(string asset_id, string mr_id, string searchstring);
        List<AssetActivityLogs> WorkOrderStatusHistory(Guid wo_id);
        (List<WorkOrders>, int) FilterWorkOrderTitleOption(FilterWorkOrderOptionsRequestModel requestModel);
        (List<WorkOrders>, int) FilterWorkOrderNumberOption(FilterWorkOrderOptionsRequestModel requestModel);

        WorkOrderTasks GetWOTaskById(Guid wo_task_id);
        (List<WorkOrders>, int total_list_count) GetAllWorkOrdersNewflow(string userid, NewFlowWorkorderListRequestModel requestModel);
        (List<InspectionsTemplateFormIO>, int total_list_count) GetAllCatagoryForWO(string userid, GetAllCatagoryForWORequestModel requestModel);
        (List<Asset>, int total_list_count) GetAssetsToAssignOBWO(GetAssetsToAssignOBWORequestmodel requestmodel);
        List<Asset> GetComponantLevelMainAssets(GetComponantLevelAssetsRequestmodel requestmodel);
        List<WOOnboardingAssets> GetComponantLevelTempAssets(GetComponantLevelAssetsRequestmodel requestmodel);
        List<WOInspectionsTemplateFormIOAssignment> GetAllCatagoryOFWO(Guid wo_id);
        WorkOrders ViewWorkOrderDetailsById(Guid wo_id);
        WorkOrders ViewOBWODetailsById(Guid wo_id);
        List<WOInspectionsTemplateFormIOAssignment> GetWOFormiomapping(Guid wo_id);
        List<WOInspectionsTemplateFormIOAssignment> GetWOFormiomappingForBulkImport(Guid wo_id);
        List<WOInspectionsTemplateFormIOAssignment> GetWOcategorymapping(Guid wo_id);
        List<WOcategorytoTaskMapping> GetWoCategoryToTaskToviewWO(Guid wo_inspectionsTemplateFormIOAssignment_id);
        List<FormIOFormsExcludedProprties> GetExcludedFormIOFormsByIds(List<Guid> form_ids);
        List<Tasks> GetTaskByFormID(Guid form_id);
        Tasks GetTaskByID(Guid task_id);
        (List<User>, int total_list_count) GetAllTechnician(GetAllTechnicianRequestModel requestModel);

        List<WOcategorytoTaskMapping> GetWOcategoryTaskByCategoryID(string wo_inspectionsTemplateFormIOAssignment_id);
        //   List<WOcategorytoTaskMapping> GetWOcategoryTaskByWoid(Guid wo_id);
        List<WOcategorytoTaskMapping> GetAllWOCategoryTaskByWOid(Guid eo_id);
        List<WOcategorytoTaskMapping> GetAllWOCategoryTaskByWOidForTask(Guid eo_id, int status);

        AssetFormIO GetFormByTaskID(Guid WOcategorytoTaskMapping_id);
        AssetFormIO GetFormByTaskIDForBulkImport(Guid WOcategorytoTaskMapping_id);

        WOcategorytoTaskMapping AssignTechniciantoWOcategoryTask(Guid WOcategorytoTaskMapping_id);
        WOcategorytoTaskMapping AssignAssettoWOcategoryTask(Guid WOcategorytoTaskMapping_id);
        List<WOcategorytoTaskMapping> UpdateMultiWOCategoryTaskStatus(List<Guid> WOcategorytoTaskMapping_id);

        WOcategorytoTaskMapping GetWOcategoryTaskByTaskID(Guid WOcategorytoTaskMapping_id);
        WOcategorytoTaskMapping GetWOcategoryTaskByTaskIDForUpdateStatus(Guid WOcategorytoTaskMapping_id);

        WOcategorytoTaskMapping GetWOcategoryTaskByTaskIDMobile(Guid WOcategorytoTaskMapping_id);
        List<WOcategorytoTaskMapping> GetWOcategoryTaskByTaskIDforCopyfields(List<Guid> WOcategorytoTaskMapping_id);
        WOInspectionsTemplateFormIOAssignment GetWOcategoryID(Guid wo_inspectionsTemplateFormIOAssignment_id);
        WOInspectionsTemplateFormIOAssignment GetWOcategoryIDFormobile(Guid wo_inspectionsTemplateFormIOAssignment_id);
        WOInspectionsTemplateFormIOAssignment GetWOcategoryWOID(Guid wo_id);

        WorkOrderAttachments GetWOAttachmentById(Guid wo_attachment_id);
        List<string> GetWOAttachmentsById(Guid wo_attachment_id);
        int GetAssetscountBySite(string company_id);
        User GetUserByID(Guid user_id);
        List<User> GetUsersByIDs(List<Guid> user_id);
        List<User> GetUserByIDs(List<Guid> user_ids);
        Asset GetAssetByLocation(string location);
        Asset GetAssetByAssetnames(List<string> location);
        Asset GetAssetByInternalID(string internal_id);
        Asset GetAssetByID(Guid asset_id);
        WOOnboardingAssets GetFedByOBAssetByID(Guid asset_id);
        InspectionsTemplateFormIO GetFormIOByName(Guid form_id);

        InspectionForms GetinspectionformbyFormType(int formiotype, Guid company_id);
        (List<WorkOrders>, int total_list_count) GetWOsForOffline(string userid, DateTime? sync_time);
        WorkOrders WorkOrderDetailsByIdForExportPDF(Guid wo_id);
        List<WOcategorytoTaskMapping> GetWOcompletedTask(Guid wo_inspectionsTemplateFormIOAssignment_id);
        List<WOcategorytoTaskMapping> GetWOcompletedTaskForBulkImport(Guid wo_inspectionsTemplateFormIOAssignment_id);
        ListViewModel<Asset> OfflineAssetData(DateTime? sync_time);
        List<AssetClassFormIOMapping> GetAssetClassFormIOMappingOffline(DateTime? sync_time);
        List<AssetProfileImages> GetAssetProfileImagesForoffline(DateTime? sync_time);
        List<InspectionTemplateAssetClass> GetMasterClassForOffline(DateTime? sync_time);
        List<AssetClassFormIOMapping> GetMasterClassFormMappingForOffline(DateTime? sync_time);
        List<WOOnboardingAssets> GetOBAssetdetailsOffline(DateTime? sync_time);
        List<WOLineBuildingMapping> GetWOLineBuildingMappingOffline(DateTime? sync_time);
        List<FormIOBuildings> GetFormIOBuildingsOffline(DateTime? sync_time);
        List<FormIOFloors> GetFormIOFloorsOffline(DateTime? sync_time);
        List<FormIORooms> GetFormIORoomsOffline(DateTime? sync_time);
        List<FormIOSections> GetFormIOSectionOffline(DateTime? sync_time);
        List<WOOBAssetFedByMapping> GetOBFedByAssetMappingOffline(DateTime? sync_time);
        List<WOOnboardingAssetsImagesMapping> GetOBAssetdetailsImagesOffline(DateTime? sync_time);
        List<IRWOImagesLabelMapping> GetOBAssetdetailsImagesLabelsOffline(DateTime? sync_time);
        List<AssetParentHierarchyMapping> GetAssetParentMappingOffline(DateTime? sync_time);
        List<WOInspectionsTemplateFormIOAssignment> GetAllCatagoryForWOOffline(DateTime? sync_time);
        List<WOInspectionsTemplateFormIOAssignment> GetCatagoryForWOOfflineByCategoryids(List<Guid> wo_inspection_template_formio_ids);
        List<WOcategorytoTaskMapping> GetAllWOCategoryTaskByWOidOffline(DateTime? sync_time);
        List<AssetFormIOExclude> GetAllAssetFormByWOIDOffline(List<Guid> wo_id, DateTime? sync_time);
        List<Equipment> GetFormIOEquipmentOffline(DateTime? sync_time);

        List<AssetFormIOExclude> GetAssetFormBycategorytaskid(List<Guid> WOcategorytoTaskMapping_id, DateTime? sync_time);
        List<WOcategorytoTaskMapping> Getcategorytaskbycategoryids(List<Guid> WOcategory_id, DateTime? sync_time);

        FormIOBuildings GetFormIOBuildingByName(string building_name);
        FormIOBuildings GetFormIOBuildingByNameTemp(string building_name, Guid siteId);

        FormIOBuildings ScriptGetFormIOBuildingByName(string building_name, Guid site_id, Guid company_id);
        FormIOFloors GetFormIOFloorByName(string floor_name, int building_id);
        FormIOFloors GetFormIOFloorByNameTemp(string floor_name, int building_id, Guid siteId);
        FormIOFloors ScriptGetFormIOFloorByName(string floor_name, int building_id, Guid site_id, Guid company_id);
        FormIORooms GetFormIORoomByName(string room_name, int floor_id);
        //FormIOFloors GetFormIOFloorByNameTemp(string floor_name, int building_id, Guid siteId);
        FormIORooms ScriptGetFormIORoomByName(string room_name, int floor_id, Guid site_id, Guid company_id);
        FormIOSections GetFormIOSectionByName(string section_name, int room_id);
        FormIORooms GetFormIORoomByNameTemp(string room_name, int floor_id, Guid siteId);
        FormIOSections GetFormIOSectionByNameTemp(string section_name, int room_id, Guid siteId);
        FormIOSections ScriptGetFormIOSectionByName(string section_name, int room_id, Guid site_id, Guid company_id);
        FormIOLocationNotes GetFormIONotesBySection(int section_id);
        InspectionTemplateAssetClass GetInspectionTemplateAssetClass(Guid inspectiontemplate_asset_class_id);
        List<FormIOBuildings> GetAssetBuildingHierarchy();
        List<AssetFormIO> getallformsbywos(Guid wo_id);

        WorkOrders GetWOByidforDelete(Guid wo_id);
        WorkOrders GetWOByidforUpdateStatus(Guid wo_id);
        WOInspectionsTemplateFormIOAssignment GetWOCategoryStatusforStatusmanage(Guid wo_inspectionsTemplateFormIOAssignment_id);
        List<AssetFormIOStatusExcluded> GetWOAssetFormIOStatusforStatusmanage(List<Guid> WOcategorytoTaskMapping_id);
        List<WorkOrderAttachments> GetWOsAttachmentsForOffline(DateTime? sync_time);

        List<Guid> GetAssetClassByForm(Guid Form_id);
        List<Guid> GetAssetClassBywotype(int wo_type_id);

        List<Asset> GetAssetsByAssetClass(List<Guid> asset_class_id);
        List<WorkOrders> GetWOs();

        bool IsWONumberValid(Guid? wo_id, string wo_number);
        bool IsWONumberValidFromEstimator(Guid? wo_id, string wo_number, Guid site_id);

        List<AssetClassFormIOMappingExcludeProperty> GetAssetclassFormToAddcategory(GetAssetclassFormToAddcategoryRequestmodel requestmodel);

        InspectionTemplateAssetClass GetAssetclassByCode(string asset_class_code);
        InspectionTemplateAssetClass GetAssetclassByCodeandCompanyId(string asset_class_code, Guid company_id);
        InspectionTemplateAssetClass GetAssetclassByCodeForDuplicate(string asset_class_code, Guid? inspectiontemplate_asset_class_id);
        InspectionTemplateAssetClass GetAssetclassByID(Guid inspectiontemplate_asset_class_id);

        WOOnboardingAssets GetOBWOAssetDetailsById(Guid woonboardingassets_id);
        WOOnboardingAssets GetOBWOAssetDetailsByIdForMWO(Guid woonboardingassets_id);
        WOOnboardingAssets GetWOLineByQRcode(string qr_code, Guid woonboardingassets_id);
        WorkOrders GetOBWOByID(Guid wo_id);
        WOOnboardingAssets GetOBWOAssetByID(Guid woonboardingassets_id);
        WorkOrders GetOBWOByIDForComplete(Guid wo_id);
        List<ExportCompletedAssetsByWOExcludedProperty> ExportCompletedAssetsByWO(ExportCompletedAssetsByWORequestmodel requestmodel);
        List<WOOnboardingAssets> GetOBAssetsListByWOId(ExportCompletedAssetsByWORequestmodel requestmodel);
        List<InspectionsTemplateFormIO> GetMasterFormsByFormIDs(List<Guid> form_ids);
        List<AssetFormIO> GetAssetFormIOByWOid(Guid wo_id);

        List<WOInspectionsTemplateFormIOAssignment> GetWOCategoriesByWOid(Guid wo_id);
        List<WOcategorytoTaskMapping> GetWOCategorytaskByWOid(Guid wo_id);
        List<InspectionTemplateFormIoExclude> GetMasterFormExcluded(List<Guid> form_ids);
        WorkOrders GetWorkOrderByIdForComplete(Guid wo_id);
        WorkOrders GetWorkOrderByIdForMoveIRImages(Guid wo_id);
        List<WOOnboardingAssets> GetOBAssetForMWO(Guid wo_id);
        Asset GetAssetByInternalIDCount(string internal_id, Guid requested_asset_id);
        Asset GetAssetByQRCode(List<string> qr_code);
        Asset GetAssetByQRCodeForOBWO(string qr_code,Guid? asset_id);
        Asset GetAssetByQRCodeExist(List<string> qr_code, List<Guid> existing_asset_id);

        InspectionTemplateAssetClass GetAssetClassById(Guid inspectiontemplate_asset_class_id);

        List<WOOnboardingAssets> GetOBWOAssetsByWOid(GetOBFedByAssetListRequestmodel requestmodel);
        List<WOOnboardingAssets> GetOBWOAssetsByWOidForMWO(GetAssetsToAssigninMWOInspectionRequestmodel requestmodel);

        Asset GetAsssetByInternalID(string internal_id, Guid site_id);
        Asset GetAssetByFedBy(Guid asset_id);
        Asset GetAssetByAssetIdActive(Guid asset_id);
        List<Asset> GetAssetsForHierarchy();
        List<Asset> GetAssetsByInternalid(List<string> internal_id, Guid site_id);
        List<Asset> GetAssetsByFedByIDs(List<Guid> asset_ids);
        WOOBAssetFedByMapping GetWOLineByFedby(Guid woonboardingassets_id);
        WOlineSubLevelcomponentMapping Getsublevelcomponenttocheck(Guid woonboardingassets_id);
        WOlineTopLevelcomponentMapping Gettoplevelcomponenttocheck(Guid woonboardingassets_id);
        IRScanWOImageFileMapping GetIRScanImageFileByNAme(string img_file_name, string wo_id);
        List<IRWOImagesLabelMapping> GetIRImageLabelsByWOID(string wo_id);

        List<IRScanWOImageFileMapping> GetImageNameMappingByID(Guid wo_id);

        bool IsOtherInspectionTypeINWO(Guid wo_id);
        FormIOAuthToken GetFormIOToken();

        WOOnboardingAssets GetAssetFromWOline(string asset_id);
        WOOnboardingAssets GetwooblineforMWObyid(Guid woonboardingassets_id);

        AssetFormIO GetLastInspectionByAssetID(Guid asset_id);

        AssetFormIO GetAssetformdataByID(Guid asset_form_id);
        List<WOInspectionsTemplateFormIOAssignment> GetWOCategoryByIds(List<Guid> wo_inspectionsTemplateFormIOAssignment_id);
        WorkOrders GetWObyIdforIRlambdareport(Guid wo_id);

        WOLineIssue GetWOLineIssueById(Guid wo_line_issue_id);
        WOOnboardingAssets GetWOlineByOBAssetId(Guid woonboardingassets_id);
        WOOnboardingAssets GetWOlineByIdForLinkIssue(Guid woonboardingassets_id);
        (List<WOLineIssue>, int) GetAllWOLineTempIssues(GetAllWOLineTempIssuesRequestmodel requestmodel);
        List<WOLineIssue> GetAlltempIssuebyWOid(GetAllIssueByWOidRequestmodel requestmodel);
        List<AssetIssue> GetAllmainIssuebyWOid(GetAllIssueByWOidRequestmodel requestmodel);
        (List<AssetIssue>, int) GetAllAssetIssues(GetAllAssetIssuesRequestmodel requestmodel);
        (List<AssetIssueComments>, int) GetAllAssetIssueComments(GetAllAssetIssueCommentsRequestmodel requestmodel);

        AssetIssueComments GetIssueCommentById(Guid asset_issue_comments_id);
        AssetIssue GetAssetIssueById(Guid asset_issue_id);
        List<AssetIssue> GetAssetIssueByMultiId(List<Guid> asset_issue_id);
        AssetIssue ViewAssetIssueDetailsById(Guid asset_issue_id);
        List<AssetIssue> GetMainIssueByAssetId(Guid asset_id);
        List<WOLineIssue> GetIssuesIssueByAssetId(IssueListtoLinkWOlineRequestmodel requestmodel);
        List<WOLineIssue> GetFloatingTempIssueByAssetId(IssueListtoLinkWOlineRequestmodel requestmodel, List<Guid> already_got_temp_issues);

        List<AssetIssue> GetMainIssueByAssetformid(GetWOLinkedIssueRequestmodel requestmodel);
        List<WOLineIssue> GetTempIssueByAssetformid(GetWOLinkedIssueRequestmodel requestmodel);

        List<AssetIssue> GetMainIssueBywoobassetid(GetWOLinkedIssueRequestmodel requestmodel);
        List<WOLineIssue> GetTempIssueBywoobassetid(GetWOLinkedIssueRequestmodel requestmodel);

        List<WOLineIssue> GetTempIssueByMultiId(List<Guid> wo_line_issue_id);

        AssetIssue GetAssetIssueByIdForAssetupdate(Guid asset_issue_id);
        List<Guid> GetAssetformIdsbyWO(Guid wo_id);
        List<Asset> GetAssetwhichhavenoIssue(Guid wo_id);
        List<Asset> GetWOlineAssetwhichhavenoIssue(Guid wo_id);
        List<Guid> GetWOObWOlineIds(Guid wo_id);
        List<AssetIssue> GetAssetIssuebyAssetformId(List<Guid> asset_form_id);
        List<AssetIssue> GetAssetIssuebyOBWOline(List<Guid> woonboardingassets_id);
        List<AssetIssue> GetAssetIssuebyIds(List<Guid> asset_issue_id);
        List<AssetIssue> GetIssueByWOidtoUnlink(Guid wo_id);
        List<AssetIssue> GetnotCompletedIssueByWOidtoUnlink(Guid wo_id);
        List<AssetPMs> GetAssetpmByWOidtoUnlink(Guid wo_id);
        List<AssetPMs> GetAssetpmByWOidtoUnlinkUpdated(Guid wo_id);
        List<WOLineIssue> GetWOLineIssuebyIds(List<Guid> wo_line_issue_id);
        List<AssetPMs> GetAssetPMsbyIds(List<Guid> asset_pm_id);
        AssetPMs GetAssetPMsbyId(Guid asset_pm_id);
        TempAssetPMs GetTempAssetPMsbyIdForsubmit(Guid asset_pm_id);
        TempAssetPMs GetTempAssetPMsbyId(Guid temp_asset_pm_id);
        PMs GetPMById(Guid pm_id);

        List<FormIOBuildings> GetLocationHierarchyForWO(GetLocationHierarchyForWORequestmodel request);

        List<WOOnboardingAssets> GetWOlinesByLocation(GetWOlinesByLocationRequestmodel requestmodel);
        List<AssetFormIO> GetOpenAssetformio(Guid wo_d);
        List<WOOnboardingAssets> GetOpenwoobline(Guid wo_d);
        List<WOOnboardingAssets> ScriptforWOlinelocation();

        List<InspectionTemplateAssetClass> Scriptforformandclass();
        List<InspectionsTemplateFormIO> Scriptforformioform();
        string Scriptforformandclass_get_WPnyid(Guid form_id);
        Guid Scriptforformandclass_get_idbyWP(string WP);

        WOOnboardingAssets GetOBWOlineByQRCode(GetOBWOlineByQRCodeRequestmodel requestmodel);

        Asset GetAssetidForPMattch(Guid asset_id);
        List<Guid> ExistingWolineForLinkPM(List<Guid> asset_id, Guid wo_id);
        Guid GetAssetformIOtoLinkPM(Guid asset_id, Guid wo_id);

        List<AssetPMs> GetAssetPmsForOffline(DateTime? sync_time);
        AssetPMs GetAssetPMtoUpdateOffline(Guid asset_pm_id);

        List<AssetIssue> GetAssetMainIssueForOffline(DateTime? sync_time);
        List<WOLineIssue> GetAssetWoloineIssueForOffline(DateTime? sync_time);
        List<AssetIssueImagesMapping> GetAssetIssueImagesForOffline(DateTime? sync_time);
        List<AssetIRWOImagesLabelMapping> GetAssetIRVisualImageMappingOffline(DateTime? sync_time);
        List<AssetAttachmentMapping> GetAssetAttachmentsMappingOffline(DateTime? sync_time);
        List<WOlineTopLevelcomponentMapping> GetWolineToplevelAssetMappingOffline(DateTime? sync_time);
        List<WOlineSubLevelcomponentMapping> GetWolineSublevelAssetMappingOffline(DateTime? sync_time);

        List<AssetTopLevelcomponentMapping> GetAssetToplevelAssetMappingOffline(DateTime? sync_time);
        List<AssetSubLevelcomponentMapping> GetAssetSublevelAssetMappingOffline(DateTime? sync_time);

        AssetIssue GetAssetIssuebyIdforOffline(Guid asset_issue_id);

        WOOnboardingAssets GetWOlineforUnlinkissue(Guid woonboardingassets_id);

        Asset GetAssetforFedby(Guid asset_id);
        List<WorkOrders> GetwobyIdsforIssueList(List<Guid> wo_ids);

        Asset GetAssetByIdforExisting(Guid asset_id);
        Asset GetAssetByIdforExistingScript(Guid asset_id);
        WOOnboardingAssets GetWolineforUpdateComponant(Guid woonboardingassets_id);

        Asset Getsublevelcomponent(Guid asset_id);

        AssetSubLevelcomponentMapping CheckSubcomponent(Guid requested_assset_id, Guid subcomponent_asset_id);

        AssetTopLevelcomponentMapping CheckToplevelAssetofSubcomponent(Guid requested_asset_id, Guid toplevel_asset_id);

        List<InspectionTemplateAssetClass> GetAllAssetClassForExport();
        List<Asset> GetAssetsByAssetClassId(Guid inspectiontemplate_asset_class_id);
        string AddIssuesDirectlyToMaintenanceWORepo(AddIssuesDirectlyToMaintenanceWORequestModel request);
        AssetIssue GetAssetIssueByAssetIssueIdRepo(Guid asset_issue_id);

        WOlineTopLevelcomponentMapping GetWolinetoplevelforOfflineupdate(Guid woline_toplevelcomponent_mapping_id);
        WOlineSubLevelcomponentMapping GetWolinesublevelforOfflineupdate(Guid woline_sublevelcomponent_mapping_id);

        AssetTopLevelcomponentMapping GetAssettoplevelforOfflineupdate(Guid asset_toplevelcomponent_mapping_id);
        AssetSubLevelcomponentMapping GetAssetsublevelforOfflineupdate(Guid asset_sublevelcomponent_mapping_id);

        WOlineTopLevelcomponentMapping GettoplevelmappingforDelete(Guid sublevelcomponent_asset_id);
        WOlineSubLevelcomponentMapping GetsublevelmappingforDelete(Guid woline_id, Guid sublevel_component_asset_id);
        int GetTotalNumberOfIssues(Guid siteId);
        string GetSiteCodeById(Guid siteId);

        (List<AssetIssue>, int) GetIssuesForIssueNumber(Guid siteId);
        List<Sites> GetAllSitesIds();
        List<Guid> GetAllSites();
        List<Equipment> GetAllEquipment(Guid site_id);

        Asset GetTobeReplaceAsset(Guid asset_id);

        List<AssetPMs> GetAssetpmToaddWOline(List<Guid> asset_pm_ids);

        PMs GetpmPmid(Guid pm_id);

        PMItemMasterForms GetPMMasterFormByAssetpm(string asset_class_code, string plan_name, string title);
        (List<Asset>, int total_list_count) GetAssetsbyLocationHierarchy(GetAssetsbyLocationHierarchyRequestmodel requestmodel);
        Asset GetSubLevelAssetById(Guid sublevelcomponent_asset_id);

        TempFormIOBuildings GetTempFormIOBuildingByName(string building_name, Guid wo_id);
        FormIOBuildings GetMainFormIOBuildingByName(string building_name);
        TempFormIOBuildings GetTempFormIOBuildingByNameV2(string building_name);
        TempFormIOFloors GetTempFormIOFloorByName(string floor_name, Guid temp_building_id, Guid wo_id);
        TempFormIOFloors GetTempFormIOFloorByNameV2(string floor_name, string building_name);
        FormIOFloors GetMainFormIOFloorByName(string floor_name, string building_name);
        TempFormIORooms GetTempFormIORoomByName(string room_name, Guid temp_floor_id, Guid wo_id);
        TempFormIORooms GetTempFormIORoomByNameV2(string room_name, string temp_floor_name, string temp_building_name);
        FormIORooms GetMainFormIORoomByName(string room_name, string temp_floor_name, string temp_building_name);
        TempFormIOSections GetTempFormIOSectionByName(string section_name, Guid temp_room_id, Guid wo_id);
        List<TempFormIOBuildings> GetTempLocationHierarchyForWO(Guid wo_id, string search_string);
        (List<WOOnboardingAssets>, int total_list_count) GetWOOBAssetsbyLocationHierarchy(GetWOOBAssetsbyLocationHierarchyRequestModel requestmodel);
        (List<WOOnboardingAssets>, int total_list_count) GetWOOBAssetsbyLocationHierarchyV2(GetWOOBAssetsbyLocationHierarchyRequestModel requestmodel);

        int GetAssetCount();
        List<PMItemMasterForms> GetPMMasterFormForOffline(DateTime? sync_time);
        List<PMs> GetPMMasterForOffline(DateTime? sync_time);
        List<PMPlans> GetPMPlanMasterForOffline(DateTime? sync_time);
        List<PMCategory> GetPMCategoryMasterForOffline(DateTime? sync_time);
        List<ActiveAssetPMWOlineMapping> GetPMSubmittedData(DateTime? sync_time);
        AssetFormIOBuildingMappings GetAssetLocationDataById(Guid asset_id);

        Guid GetAssetPMsbyWOLineId(Guid woonboardingassets_id);

        Asset GetAssetbyIDForCondition(Guid asset_id);

        TempFormIOBuildings IsExistTempBuildingToAdd(Guid wo_id, int formiobuilding_id);
        TempFormIOFloors IsExistTempFloorToAdd(Guid wo_id, int formiobuilding_id);

        TempFormIORooms IsExistTempRoomsToAdd(Guid wo_id, int formioroom_id);

        TempFormIOBuildings GetTempBuildingForDelete(Guid temp_formiobuilding_id);
        TempFormIOFloors GetTempFloorForDelete(Guid temp_formiofloor_id);
        TempFormIORooms GetTempRoomForDelete(Guid temp_formioroom_id);

        TempFormIOSections GetTempSectionByName(string section, Guid temp_formioroom_id);

        ActiveAssetPMWOlineMapping GetPMSubmittedDataForOffline(Guid active_asset_pm_woline_mapping_id);
        List<Asset> CountForBuildingAssets(List<int> formiobuilding_id);
        Guid GetTopLevelWOOBAsset(Guid woonboardingassets_id);

        List<AssetPMsTriggerConditionMapping> GetAssetPMTriggerConditionMappingOffline(DateTime? sync_time);
        List<WOOnboardingAssets> GetWOOBAssetsByWOId(Guid wo_id);
        (List<GetOBIRImagesByWOIdResponseModel>, int) GetWOOBAssetsByWOId_v2(GetOBIRImagesByWOId_V2RequestModel requestModel);
        List<AssetPMs> GetOpenAssetpms(Guid asset_id);
        List<AssetPMs> GetCurrentAssetPMsByAssetId(Guid asset_id);
        bool CheckForAssetPMIsActiveOrNot(Guid asset_pm_id);
        List<TempFormIOBuildings> GetTempBuildingforOffline(DateTime? sync_time);
        List<TempFormIOFloors> GetTempFloorforOffline(DateTime? sync_time);
        List<TempFormIORooms> GetTempRoomforOffline(DateTime? sync_time);
        List<TempFormIOSections> GetTempSectionforOffline(DateTime? sync_time);
        List<WOOBAssetTempFormIOBuildingMapping> GetTemplocationwolineMappingoffline(DateTime? sync_time);
        List<TempAssetPMs> GetTempassetpmsoffline(DateTime? sync_time);
        List<TempActiveAssetPMWOlineMapping> GetTempActiveassetpmsoffline(DateTime? sync_time);
        List<TempAsset> GetTempassetoffline(DateTime? sync_time);

        TempFormIOBuildings GetTempBuildingforOfflineUpdate(Guid temp_formiobuilding_id);
        TempFormIOFloors GetTempFloorforOfflineUpdate(Guid temp_formiofloor_id);
        TempFormIORooms GetTempRoomforOfflineUpdate(Guid temp_formioroom_id);
        WOOBAssetTempFormIOBuildingMapping GetTemplocationwolinemappingforOfflineUpdate(Guid wo_ob_asset_temp_formiobuilding_id);

        TempFormIOSections GetTempSectionformOfflineUpdate(string section_name, Guid? temp_formioroom_id);

        WOOnboardingAssets GetMainAssetWOlineforPM(Guid woonboardingassets_id);
        AssetPMs GetAssetpmByidForInspectionlist(Guid asset_pm_id);



        List<Asset> Getalltoplevelforscript();

        WOOnboardingAssets GetSublevelWolinefornameplateImage(Guid woonboardingassets_id);

        Asset AssetGettoplevelmainAsset(Guid asset_id);
        WOOnboardingAssets GetOBWOAssetforLocation(Guid woonboardingassets_id);
        WorkOrderTechnicianMapping GetWorkOrderTechnicianMappingById(Guid wo_technician_mapping_id);

        AssetPMPlans GetAssetPMPlanforMapping(Guid pm_plan_id);

        WOOBAssetTempFormIOBuildingMapping GetTempLocationOfOBWOAssetByOBAssetID(Guid woonboardingassets_id);
        int GetOBWOAssetCountByStatus(Guid wo_id, int status_code);
        int GetAssetFormIOCountByStatus(Guid wo_id, int status_code);
        int GetAssetFormIOCountByWOType(int wo_type);
        int GetOBWOAssetCountByWOType(int wo_type);

        //AssetPMs GetAssetpmByidForInspectionlist(Guid asset_pm_id);

        List<WorkOrders> GetWODetilsForReport();
        List<WorkOrders> GetCompletedWODetilsForReport();

        List<AssetIssue> GetIssueDetailsForReport(Guid site_id);
        List<Sites> GetsitesForReport();

        Asset GetMainAssetforTempIssue(Guid asset_id);

        WOOnboardingAssets GetwolineForTempIssue(Guid woonboardingassets_id);

        List<Asset> GetMainAssetstoAddIssue(GetAssetListForIssueRequestmodel request);
        List<WOOnboardingAssets> GetTempAssetstoAddissue(GetAssetListForIssueRequestmodel request);

        WOOnboardingAssets GetWOlineByIdForissueupdate(Guid woline_id);
        Asset GetMainAssetByIdForissueupdate(Guid asset_id);

        TempAssetPMs GetTempAssetPMsbyIdOfflineUpdate(Guid temp_asset_pm_id);
        TempActiveAssetPMWOlineMapping GetTempActiveAssetPMwolinemappingbyIdOfflineUpdate(Guid temp_active_asset_pm_woline_mapping_id);
        List<WorkOrders> GetAllCalendarWorkorders(GetAllCalanderWorkordersRequestModel requestModel);
        List<WOlineIssueImagesMapping> GetWOlineIssueImages(List<Guid> requested_deleted_image_mapping_ids);

        TempFormIOBuildings GetTempBuilding(string building, Guid wo_id);
        TempFormIOBuildings GetTempBuildingAsnotracking(string building, Guid wo_id);
        TempFormIOFloors GetTempFloor(string floor, Guid temp_formiobuilding_id, Guid wo_id);
        TempFormIOFloors GetTempFloorAsnotracking(string floor, Guid temp_formiobuilding_id, Guid wo_id);
        TempFormIORooms GetTempRoom(string room, Guid temp_formiofloor_id, Guid wo_id);
        TempFormIORooms GetTempRoomAsnotracking(string room, Guid temp_formiofloor_id, Guid wo_id);
        TempFormIOSections GetTempSection(string section, Guid temp_formioroom_id, Guid wo_id);
        TempFormIOSections GetTempSectionAsnotracking(string section, Guid temp_formioroom_id, Guid wo_id);
        int GetTempRoomCountForWorkOrder(Guid wo_id);
        WOOnboardingAssets GetInstallWOlinefromTempAssetId(Guid tempasset_id);
        List<WOOnboardingAssets> GetIssueInstallWoline(Guid wo_id);
        WOOnboardingAssets GetCompletedIssueWOlinebyTempAsset(Guid tempasset_id);
        TempAsset GetTempAssetbyId(Guid tempasset_id);
        TempAsset GetTempAssetByMainAssetId(Guid asset_id, Guid wo_id);
        TempAsset GetTempAssetBywoline(Guid woonboardingassets_id);
        TempAsset GetTempAssetForDelete(Guid tempasset_id);
        WOOnboardingAssets GetWolineForTempAssetdelete(Guid woonboardingassets_id);
        List<WOOnboardingAssets> GetWOSublevelByIds(List<Guid> woonboardingassets_id);

        WOOnboardingAssets IsInstallWOlineExist(Guid wo_id, Guid asset_id);
        List<string> GetWorkorderWatcherByWOId(Guid wo_id);
        bool CheckUserIsWatcherOrNot(Guid wo_id, Guid user_id);
        List<WorkOrders> GetAllWorkordersForScheduler();
        int GetAssetIssueCountBySite();
        int GetWorkorderCountBySite();
        int GetEquipmentCount();
        List<TempAsset> GetAllTempAssetsByWOId(Guid wo_id);
        WOLineIssue GetTempIssueforOfflineUpdate(Guid wo_line_issue_id);
        List<WorkOrders> GetAllWOforTempassetscript();
        List<WOOnboardingAssets> GetAllwolinesforScript(Guid wo_id);
        InspectionTemplateAssetClass GetAssetclassbycodefroscript(string class_code, Guid company_id);

        TempAsset GetTempAssetbyAssetid(Guid asset_id, Guid wo_id);
        WOOnboardingAssets GetInstallwOlinebyassetid(Guid asset_id, Guid wo_id);

        WOOnboardingAssets GetMainIssueWOlineForScript(Guid woonboardingassets_id);
        List<WOOnboardingAssets> GetAllissuewolinesforScript(Guid wo_id);
        TempFormIOBuildings GetTempFormIOBuildingByNameForScript(string building_name, Guid wo_id);
        TempFormIOFloors GetTempFormIOFloorByNameForScript(string floor_name, Guid temp_building_id, Guid wo_id);
        TempFormIORooms GetTempFormIORoomByNameForScript(string room_name, Guid temp_floor_id, Guid wo_id);
        TempFormIOSections GetTempFormIOSectionByNameForScript(string section_name, Guid temp_room_id, Guid wo_id);
        WOlineSubLevelcomponentMapping GetSublevelMappping(Guid woonboardingassets_id, Guid sublevel_woonboardingassets_id);
        WOLineIssue GetTempIssueByWOline(Guid woonboardingassets_id);
        AssetPMs GetIRAssetPMsByAssetId(Guid asset_id);
        AssetPMs GetVisualAssetPMsByAssetId(Guid asset_id);
        TimeMaterials GetTimeMaterialById(Guid time_material_id);
        (List<TimeMaterials>, int, double) GetAllTimeMaterialsForWO(GetAllTimeMaterialsForWORequestModel requestModel);
        int GetTimeMaterialCountByType(Guid wo_id, int time_material_category_type);
        int GetAllTimeMaterialCountByWOId(Guid wo_id);
        List<User> GetAllWatcherOfWorkorder(Guid wo_id);
        WOOnboardingAssetsImagesMapping GetWOOBImageById(Guid woonboardingassetsimagesmapping_id);

        WorkOrders GetWObyIDForOfflineIssue(Guid wo_id);

        string GetAssetClassTypeById(Guid inspectiontemplate_asset_class_id);

        List<IRWOImagesLabelMapping> GetIRImageByname(string image_name, Guid wo_id);

        List<WOOnboardingAssets> GetAllIRPMWOline(Guid wo_id);
        WOOnboardingAssets GetOBWOAssetByName(string wo_id, string name);
        WOlineSubLevelcomponentMapping GetWolineSublevelAssetMappingById(Guid sublevelcomponent_asset_id, Guid woonboardingassets_id);
        List<WOOnboardingAssetsImagesMapping> GetWOOBAssetImagesById(Guid woonboardingassets_id);
        List<AssetProfileImages> GetMainAssetImagesById(Guid asset_id);
        List<AssetProfileImages> GetMainAssetImagesByWOLineId(Guid woonboardingassets_id);
        InspectionTemplateAssetClass GetAssetClassByClasscode(string class_code);

        List<WOlineIssueImagesMapping> TempIssueImagesMappingOffline(DateTime? sync_time);

        List<SitewalkthroughTempPmEstimation> GetSiteWalkthroughTempPMEstimationOffline(DateTime? sync_time);

        WOlineIssueImagesMapping GetTempIssueImageforOfflineUpdate(Guid woline_issue_image_mapping_id);
        string GetOBWOAssetNameByID(Guid woonboardingassets_id);
        Guid? GetOBWOAssetByNameClass(string wo_id, string name, string class_code);

        WOOnboardingAssets GetWOlinebyIdforOperatingcondition(Guid woonboardingassets_id);
        WOOnboardingAssets GetOBSublevelAssetById(Guid woonboardingassets_id);
        bool CheckIsClassAvailableOrNot(List<string> class_code_list);
        AssetSubLevelcomponentMapping GetAssetSublevelMappingBySubId(Guid sublevelcomponent_asset_id);

        DeviceInfo GetdeviceInfoById(Guid device_uuid);

        TrackMobileSyncOffline GetOfflineRequestTrackData(Guid trackmobilesyncoffline_id);

        WOOnboardingAssets GetWOlineForcompletestatus(Guid woonboardingassets_id);
        List<WOOnboardingAssets> GetOBWOAssetsByIDs(List<Guid> woonboardingassets_id_list);
        List<ResponsibleParty> GetAllResponsiblePartyList();
        List<WOOnboardingAssets> GetAllOBAssetsWithQRCodeByWOId(string wo_id);
        (List<WOlistExcludeProperties>, int total_list_count) GetAllWorkOrdersNewflowOptimized(string userid, NewFlowWorkorderListRequestModel requestModel);
        int GetAssetsCountByLocation(int building_id, int floor_id, int room_id);
        WorkOrderBackOfficeUserMapping GetWorkOrderBackOfficeMappingById(Guid wo_backoffice_user_mapping_id);
        Sites GetcompanyURLbySiteId(Guid site_id);

        User GetUserFirstnameById(Guid user_id);

        List<string> GetWOAssignedUserNamesbyId(List<Guid> user_ids);
        Guid GetcompantBySiteId(Guid site_id);
        WorkOrders GetWODetailsForUserAssignmentEmail(Guid wo_id);
        List<GetIRImageFilePathExclude> GetImagesFilePaths(List<string> file_names, string wo_id);

        List<TempFormIOBuildings> GetTempBuildingv2();
        List<TempMasterBuilding> GetTempMasterBuilding(GetAllTempMasterLocationForWORequestModel requestModel);
        List<TempFormIOFloors> GetTempFloorsv2(string building_name);
        List<TempMasterFloor> GetTempMasterFloor(GetAllTempMasterLocationForWORequestModel requestModel, string building_name);
        List<TempFormIORooms> GetTempRoomsv2(string floor_name, string building_name);
        List<TempMasterRoom> GetTempMasterRoom(GetAllTempMasterLocationForWORequestModel requestModel, string floor_name, string building_name);
        List<FormIOBuildings> GetMainBuildingv2(List<string> temp_building_names);
        List<FormIOFloors> GetMainFloorsv2(string building_name, List<string> temp_floor_names);
        List<FormIORooms> GetMainRoomsv2(string floor_name, string building_name, List<string> temp_floor_names);
        bool CheckWOLineExistOrNot(Guid wo_id, Guid asset_id);
        string GetWOLineByAssetIdQRCodeWOId(Guid wo_id, Guid asset_id, string qr_code);

        List<TempAsset> GetActiveWOlineLocations(Guid wo_id, string search_string);

        List<TempAsset> GetTempAssetsWithTempMasterLocations(GetAllTempMasterLocationForWORequestModel requestModel);
        string GettempBuildingNameById(Guid temp_formiobuilding_id);
        string GettempFloorNameById(Guid temp_formiofloor_id);
        string GettempRoomNameById(Guid temp_formioroom_id);
        string GetTempMasterRoomNameById(Guid temp_master_room_id);
        Guid GetNewCreatedAssetID(Guid woonboardingassets_id);

        List<WorkOrderTechnicianMapping> GetWOAlreadyAssignedTech(List<Guid> already_sent_mail_user, Guid wo_id);
        List<WorkOrderBackOfficeUserMapping> GetWOAlreadyAssignedBackoffice(List<Guid> already_sent_mail_user, Guid wo_id);
        TempFormIORooms GetDefaultTempLocation();
        WOOnboardingAssetsDateTimeTracking GetWOOnboardingAssetsDateTimeTrackingById(Guid woonboardingassets_id);
        List<AssetPMs> GetSchedulePMsByAssetIds(List<Guid> asset_id);
        List<AssetPMs> GetSchedulePMsByWOIdAssetIds(List<Guid> asset_id, Guid wo_id);
        List<feeding_circuit_list_class> GetOBAssetFeedingCircuitList(Guid woonboardingassets_id);
        List<WOlineTopLevelcomponentMapping> GetWOLineToplevelMappingsById(Guid woonboardingassets_id);
        List<WOlineSubLevelcomponentMapping> GetWOLineSublevelMappingsById(Guid woonboardingassets_id);
        List<WOOBAssetFedByMapping> GetWOLineFedByMappingsById(Guid woonboardingassets_id);
        int GetIssuesCountByOBWOAssetId(Guid woonboardingassets_id);
        TempMasterBuilding GetTempMasterBuildingByName(string building_name);
        TempMasterBuilding GetTempMasterBuildingByName_V2Script(string building_name, Guid site_id);
        TempMasterFloor GetTempMasterFloorByName(string floor_name, string building_name);
        TempMasterFloor GetTempMasterFloorByName_V2Script(string floor_name, string building_name, Guid site_id);
        TempMasterRoom GetTempMasterRoomByName(string room_name, string temp_floor_name, string temp_building_name);
        TempMasterRoom GetTempMasterRoomByName_V2Script(string room_name, string temp_floor_name, string temp_building_name, Guid site_id);
        TempMasterBuildingWOMapping GetTempMasterBuildingWOMappingById(Guid temp_master_building_id, Guid wo_id);
        TempMasterFloorWOMapping GetTempMasterFloorWOMappingById(Guid temp_master_floor_id, Guid wo_id);
        TempMasterRoomWOMapping GetTempMasterRoomWOMappingById(Guid temp_master_room_id, Guid wo_id);
        IRWOImagesLabelMapping GetIRWOImageLabelMappingByName(Guid woonboardingassets_id, string ir_image, string visual_image);
        List<WOlineIssueImagesMapping> GetIssueImagesByIRVisualId(Guid irwoimagelabelmapping_id);
        List<temp_asset_data> GetAllWOOnboardingAssetsByWOId(Guid wo_id);
        List<assets_fedby_mappings_class> GetWOOBFedByMappingsByWOId(Guid wo_id);
        List<asset_subcomponents_mappings_class> GetTopSubComponentMappingsByWOId(Guid wo_id);
        List<AssetPMs> GetVisualSchedulePMsByAssetIds(List<Guid> asset_id);
        int GetWorkOrderTypeById(Guid wo_id);
        bool IsAssetisAssigned(Guid asset_id);
        bool IsAssetisAssignedToOtherWOs(Guid asset_id, Guid wo_id);
        Guid GetCCFromSiteId(Guid site_id);
        Guid GetCompanyIdFromSiteId(Guid site_id);
        List<TempMasterBuilding> GetTempMasterBuildingforOffline(DateTime? sync_time);
        List<TempMasterFloor> GetTempMasterFloorforOffline(DateTime? sync_time);
        List<TempMasterRoom> GetTempMasterRoomforOffline(DateTime? sync_time);
        List<TempMasterBuildingWOMapping> GetTempMasterBuildingWOMappingforOffline(DateTime? sync_time);
        List<TempMasterFloorWOMapping> GetTempMasterFloorWOMappingforOffline(DateTime? sync_time);
        List<TempMasterRoomWOMapping> GetTempMasterRoomWOMappingforOffline(DateTime? sync_time);
        List<TempMasterBuilding> GetAllTempMasterBuildingsListForWO(GetAllTempMasterLocationForWORequestModel requestModel);
        List<TempMasterFloor> GetAllTempMasterFloorsListForWO(GetAllTempMasterLocationForWORequestModel requestModel);
        List<TempMasterRoom> GetAllTempMasterRoomsListForWO(GetAllTempMasterLocationForWORequestModel requestModel);
        (List<WOOnboardingAssets>, int total_list_count) GetWOOBAssetsbyLocationHierarchyV3(GetWOOBAssetsbyLocationHierarchyRequestModel requestmodel);
        int GetTempMasterLocationCount(Guid wo_id);
        List<WorkOrderTechnicianMapping> GetWOTechnicanMappingForOffline(DateTime? sync_time);
        WorkordersVendorContactsMapping GetWOContactMappingById(Guid workorders_vendor_contacts_mapping_id);
        List<WorkordersVendorContactsMapping> GetWOContactMappingsByVendorId(Guid vendor_id, Guid wo_id);
        string GetAssetProfileImage(Guid woonboardingassets_id);
        List<WO_Vendor_Contacts_Mapping_View_Class> GetWOContactMappingsByWOId(Guid wo_id);
        List<WorkordersVendorContactsMapping> GetWOVendorMappingByVendorId(Guid vendor_id);
        List<WorkordersVendorContactsMapping> GetWOContactMappingByContactId(Guid contact_id);
        List<WorkordersVendorContactsMapping> GetVendorContactMappings(List<Guid> contacts_id, Guid vendor_id, Guid wo_id);
        List<Guid> CheckIfContactsAreAlreadyAdded(Guid wo_id, List<Guid> req_contact_ids);
        List<string> GetContactsEmailsByWOId(Guid wo_id);
        (int, int) GetAcceptRejectCountForWO(Guid wo_id);

        List<Guid> CheckForUserEmailFlag(List<Guid> list);
        GetPMEstimationResponseModel GetPMEstimation(GetPMEstimationRequestModel requestModel);
        SitewalkthroughTempPmEstimation GetSiteWalkThroughPMEstimationbyID(Guid sitewalkthrough_temp_pm_estimation_id);
        int GetIRWOImagesCount(Guid wo_id);
        List<SitewalkthroughTempPmEstimation> GetPMEstimationbyWOlineid(Guid woonboardingassets_id);

        AssetPMs GetCurrentAssetPM(Guid asset_id, Guid pm_id);



        List<Guid> GetPMIdList(Guid asset_id);
        List<AssetPMs> GetAssetpmsbyAssetId(Guid asset_id);

        (List<WOOnboardingAssets>, int total_list_count) GetTotalOBWOAssetRoomWise(Guid wo_id, string room_name);

        SitewalkthroughTempPmEstimation GetSiteWalkThroughTempPMEstimationByID(Guid sitewalkthrough_temp_pm_estimation_id);

        GetPMEstimationResponseModel GetPMEstimationByClassId(Guid class_id);
        public Guid? GetTempAssetIdByWoOnboardingAssetId(Guid woonboardingassets_id);
        List<vendor_details_class> GetVedorContactsDetailsForWO(Guid wo_id);
        Guid GetPMPlanIdByPMId(Guid pm_id);
    }
}
