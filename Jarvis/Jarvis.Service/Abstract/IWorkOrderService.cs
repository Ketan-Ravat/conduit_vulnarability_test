using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.db.Models;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IWorkOrderService
    {
        Task<WorkOrderResponseModel> AddUpdateWorkOrder(AddWorkOrderRequestModel pmRequest);
        Task<ListViewModel<WorkOrderResponseModel>> GetAllWorkOrder(GetAllWorkOrderRequestModel requestModel);
        Task<ListViewModel<IssueResponseModel>> GetNewIssuesListByAssetId(FilterWorkOrderIssueRequestModel filter_type);
        Task<ListViewModel<AssetActivityLogsViewModel>> WorkOrderStatusHistory(Guid wo_id);
        Task<int> DeleteWorkOrder(Guid wo_id);
        ListViewModel<WorkOrderResponseModel> FilterWorkOrderTitleOption(FilterWorkOrderOptionsRequestModel requestModel);
        ListViewModel<WorkOrderResponseModel> FilterWorkOrderNumberOption(FilterWorkOrderOptionsRequestModel requestModel);
        Task<WorkOrderResponseModel> GetWorkOrderById(Guid wo_id);

        //-----------------------------------  new requirement workorder implementation ------------------------------------//
        ListViewModel<NewFlowWorkorderListResponseModel> GetAllWorkOrdersNewflow(NewFlowWorkorderListRequestModel requestModel);
        ListViewModel<GetAllWorkOrdersNewflowOptimizedResponsemodel> GetAllWorkOrdersNewflowOptimized(NewFlowWorkorderListRequestModel requestModel);
        public Task<CreateWorkorderNewflowResponsemodel> CreateWorkorderNewflow(NewFlowCreateWORequestModel requestModel , string S3_aws_access_key , string S3_aws_secret_key);
        ListViewModel<GetAllCatagoryForWOResponseModel> GetAllCatagoryForWO(GetAllCatagoryForWORequestModel requestModel);
        public Task<int> AssignCategorytoWO(AssignCategorytoWORequestmodel requestModel);
         Task<AssignMultipleAssetClasstoWOResponsemodel> AssignMultipleAssetClasstoWO(AssignMultipleAssetClasstoWORequestmodel requestModel);
         Task<AssignAssetClasstoWOResponsemodel> AssignAssetClasstoWO(AssignAssetClasstoWORequestmodel requestModel);
        ViewWorkOrderDetailsByIdResponsemodel ViewWorkOrderDetailsById(string wo_id);
        ViewOBWODetailsByIdResponsemodel ViewOBWODetailsById(string wo_id);
        GetOBWOAssetDetailsByIdResponsemodel GetOBWOAssetDetailsById(string woonboardingassets_id);
        GetOBWOAssetDetailsByIdResponsemodel GetOBWOAssetDetailsById_V2(string woonboardingassets_id);
        ListViewModel<GetAllTechnicianResponsemodel> GetAllTechnician(GetAllTechnicianRequestModel requestmodel);
        List<GetWOcategoryTaskByCategoryIDListResponsemodel> GetWOcategoryTaskByCategoryID(string wo_inspectionsTemplateFormIOAssignment_id);
        GetWOGridView GetWOGridView(string wo_inspectionsTemplateFormIOAssignment_id);
        GetFormByWOTaskIDResponsemodel GetFormByWOTaskID(string wo_inspectionsTemplateFormIOAssignment_id);
        Task<int> AssignTechniciantoWOcategory(AssignTechniciantoWOcategoryRequestmodel request_model);
        Task<int> AssignAssettoWOcategory(AssignAssettoWOcategoryRequestModel request_model);
        Task<int> UpdateWOCategoryTask(UpdateWOCategoryTaskRequestmodel request_model, string aws_access_key, string aws_secret_key , string formio_pdf_bucket);
        Task<int> UpdateMultiWOCategoryTaskStatus(UpdateMultiWOCategoryTaskStatusRequestmodel request_model, string aws_access_key, string aws_secret_key);

        Task<int> UpdateWOCategoryStatus(UpdateWOCategoryRequestmodel request_model);
        Task<UpdateWOStatusResponseModel> UpdateWOStatus(UpdateWOStatusRequestModel request_model); 
        Task<UpdateOBWOStatusResponsemodel> UpdateOBWOStatus(UpdateOBWOStatusRequestmodel request_model); 
        Task<int> DeleteWOAttachment(DeleteWOAttachmentRequestModel request_model);
        Task<int> DeleteWOCategory(DeleteWOCategoryRequestModel request_model);
        Task<int> DeleteWOCategoryTask(DeleteWOCategoryTaskRequestModel request_model);
        Task<int> MultiCopyWOTask(MultiCopyWOTaskRequestModel request_model);
        Task<int> MapWOAttachmenttoWO(MapWOAttachmenttoWORequestModel request_model);
        List<GetWOCalenderEventsResponseModel> GetWOCalenderEvents(GetWOCalenderEventsRequestModel requestModel);
        List<GetWOcategoryTaskByCategoryIDListResponsemodel> GetAllWOCategoryTaskByWOid(string wo_id,int status);
        GetWOBacklogCardListResponsemodel GetWOBacklogCardList(string search_string, List<string>? site_id);
        Task<UploadQuoteResponsemodel> UploadQuote(UploadQuoteRequestmodel requestmodel);
        Task< GetWOsForOfflineResponsemodel> GetWOsForOffline(string userid, bool want_to_remove_nfpa_issue);
        WorkOrderDetailsByIdForExportPDFResponsemodel WorkOrderDetailsByIdForExportPDF(string wo_id);
        GetAssetFormDataForBulkImportResponsemodel GetAssetFormDataForBulkImport(string wo_id);
        Task<int> CopyFieldsFromForm(CopyFieldsFromFormRequestmodel requestmodel);
        Task<UpdateWOOfflineResponsemodel> UpdateWOOffline(UpdateWOOfflineRequestModel requestmodel , string sqs_aws_access_key , string sqs_aws_secret_key , string offline_sync_bucket, string S3_aws_access_key , string S3_aws_secret_key);
        GetOfflineSyncLambdaStatusResponsemodel GetOfflineSyncLambdaStatus(GetOfflineSyncLambdaStatusRequestmodel requestmodel);
        Task<int> UpdateWOOfflineAfterLambdaExecution(UpdateWOOfflineRequestModel requestmodel , string sqs_aws_access_key , string sqs_aws_secret_key);
        GetAssetBuildingHierarchyResponsemodel GetAssetBuildingHierarchy();
        GetAssetBuildingHierarchyResponsemodel GetAssetBuildingHierarchyByWorkorder(string wo_id);
        Task<int> DeleteWO(DeleteWORequestModel requestmodel);
       // Task<int> AssignAssetToMWO(AssignAssetToMWORequestmodel requestmodel);

        List<GetAssetsToAssignResponsemodel> GetAssetsToAssign(Guid form_id);
        List<GetAssetsToAssignResponsemodel> GetAssetsToAssigninWO(int  wo_type_id);
        GetAssetsToAssigninMWOInspectionResponsemodel GetAssetsToAssigninMWOInspection(GetAssetsToAssigninMWOInspectionRequestmodel requestmodel);
        List<GetAssetclassFormToAddcategoryResponsemodel> GetAssetclassFormToAddcategory(GetAssetclassFormToAddcategoryRequestmodel requestmodel);

        Task<UploadAssettoOBWOResponsemodel>  UploadAssettoOBWO(UploadAssettoOBWORequestModel requestmodel);
        Task<UploadAssettoOBWOResponsemodel> UpdateOBWOAssetDetails(UpdateOBWOAssetDetailsRequestmodel requestmodel);
        Task<int> DeleteOBWOAsset(DeleteOBWOAssetRequestmodel requestmodel);
        Task<UpdateOBWOAssetStatusResponsemodel> UpdateOBWOAssetStatus(UpdateOBWOAssetStatusRequestmodel requestmodel);
        ExportCompletedAssetsByWOResponsemodel ExportCompletedAssetsByWO(ExportCompletedAssetsByWORequestmodel requestmodel);
        Task<int> UpdateHierarchyandLevelForPythonscript(UpdateHierarchyandLevelForPythonscriptRequestmodel requestmodel);
        Task<AddOBFedByAssetResponsemodel> AddOBFedByAsset(AddOBFedByAssetRequestmodel request_model);
        GetOBFedByAssetListResponsemodel GetOBFedByAssetList(GetOBFedByAssetListRequestmodel requestmodel);

        Task<int> AddIRImage(List<IFormFile> images, string manual_order_number, string wo_id , string site_id);

        List<GetIRScanImagesFilesResponsemodel> GetIRScanImagesFiles(GetIRScanImagesFilesRequestmodel requestmodel);
        Task<GenerateAssetInspectionFormioReportResponsemodel> GenerateAssetInspectionFormioReport(GenerateAssetInspectionFormioReportRequestmodel requestModel, string aws_access_key, string aws_secret_key , string bucketname);
        Task<GenerateAssetInspectionFormioReportResponsemodel> GenerateIRWOAssetReport(GenerateIRWOAssetReportRequestmodel requestModel, string aws_access_key, string aws_secret_key , string bucketname);

        GetFormJsonForLambdaResponsemodel GetFormJsonForLambda(string asset_form_id);

        Task<int> UpdateWOlinePDFurlfromLambda(UpdateWOlinePDFurlfromLambdaRequestmodel requestmodel);
        int UpdateWOCategoryGroupString(UpdateWOCategoryGroupStringRequestmodel requestmodel);

        Task<IRWOAssetReportStatusResponsemodel> IRWOAssetReportStatus(string wo_id);
        BulkImportStatusResponsemodel BulkImportAssetFormIOStatus(string wo_id);

        ListViewModel<GetAllWOLineTempIssuesResponsemodel> GetAllWOLineTempIssues(GetAllWOLineTempIssuesRequestmodel requestmodel);
        ListViewModel<GetAllAssetIssuesResponsemodel> GetAllAssetIssues(GetAllAssetIssuesRequestmodel requestmodel);
        ListViewModel<GetAllAssetIssueCommentsResponsemodel> GetAllAssetIssueComments(GetAllAssetIssueCommentsRequestmodel requestmodel);
        Task<int> AddUpdateIssueComment(AddUpdateIssueCommentRequestmodel requestmodel);
        Task<int> AddUpdateAssetIssue(AddUpdateAssetIssueRequestmodel requestmodel);
        Task<int> LinkIssueToWOLine(LinkIssueToWOLineRequestmodel requestmodel);
        Task<int> DeleteAssetIssue(DeleteAssetIssueRequestmodel requestmodel);

        ViewAssetIssueDetailsByIdResponsemodel ViewAssetIssueDetailsById(string asset_issue_id);
        IssueListtoLinkWOlineResponsemodel IssueListtoLinkWOline(IssueListtoLinkWOlineRequestmodel requestmodel);
        GetWOLinkedIssueResponsemodel GetWOLinkedIssue(GetWOLinkedIssueRequestmodel requestmodel);
        Task<int> UnlinkIssueFromWO(UnlinkIssueFromWORequestmodel requestmodel);
        Task<LinkAssetPMToWOresponsemodel> LinkAssetPMToWO(LinkAssetPMToWORequestmodel requestmodel);
        Task<int> AddAssetLocationData(AddAssetLocationDataRequestmodel requestmodel);

        GetWOCompletedThreadStatusResponsemodel GetWOCompletedThreadStatus(Guid wo_id);

        GetLocationHierarchyForWOResponsemodel GetLocationHierarchyForWO(GetLocationHierarchyForWORequestmodel request);
        GetLocationHierarchyForWOResponsemodel GetLocationHierarchyForWO_Version2(GetLocationHierarchyForWORequestmodel request);
        GetWOlinesByLocationResponsemodel GetWOlinesByLocation(GetWOlinesByLocationRequestmodel requestmodel);
        Task<int> BulkImportAssetFormIO(BulkImportAssetFormIORequestmodel requestModel, string aws_access_key, string aws_secret_key, string bucketName);

        Task<int> ScriptforWOlinelocation();
        Task<int> Scriptforformandclass();

        (GetOBWOAssetDetailsByIdResponsemodel,int) GetOBWOlineByQRCode(GetOBWOlineByQRCodeRequestmodel requestmodel);

        GetAllIssueByWOidResponsemodel GetAllIssueByWOid(GetAllIssueByWOidRequestmodel requestmodel);

        Task<int> AssignExistingAssettoOBWO(AssignExistingAssettoOBWORequestmodel requestmodel);

        ListViewModel<GetAssetsToAssignOBWOResponsemodel> GetAssetsToAssignOBWO(GetAssetsToAssignOBWORequestmodel requestmodel);
        GetComponantLevelAssetsResponsemodel GetComponantLevelAssets(GetComponantLevelAssetsRequestmodel requestmodel);
        List<GetAssetPMConditionDataForExportResponsemodel> GetAssetPMConditionDataForExport(GetAssetPMConditionDataForExportRequestmodel requestmodel);
        Task<AddIssuesDirectlyToMaintenanceWOServiceResponsemodel> AddIssuesDirectlyToMaintenanceWOService(AddIssuesDirectlyToMaintenanceWORequestModel request);
        Task<AddIssuesDirectlyToMaintenanceWOServiceResponsemodel> AddIssuesDirectlyToMaintenanceWOServiceBySteps(AddIssuesDirectlyToMaintenanceWORequestModel request);

        Task<int> SetIssueNumberInAssetIssues();
        int DeleteLinkOfAssetPMWithWOLine(DeleteLinkOfAssetPMWithWOLineRequestmodel requestmodel);

        Task<List<AddAssetPMWolineResponsemodel>> AddAssetPMWoline(AddAssetPMWolineRequestmodel requestmodel);

        GetAssetPMFormByIdResponsemodel GetAssetPMFormById(GetAssetPMFormByIdRequestmodel requestmodel);
        GetPMMasterFormByPMidRequestmodel GetPMMasterFormByPMid(Guid pm_id);
        ListViewModel<GetAssetsbyLocationHierarchyResponsemodel> GetAssetsbyLocationHierarchy(GetAssetsbyLocationHierarchyRequestmodel requestmodel);

        Task<int> AddDataToEquipment();
        Task<int> ChangeCalibrationStatusScript();
        Task<int> SubmitPMFormJson(SubmitPMFormJsonRequestmodel requestmodel);

        Task<int> AddTempLocationData(AddTempLocationDataRequestModel requestmodel);
        Task<int> AddTempLocationDataV2(AddTempLocationDataRequestModel requestmodel);
        Task<int> AddExistingtoTempLocation(AddExistingtoTempLocationRequestmodel requestmodel);
        GetTempLocationHierarchyForWOResponseModel GetTempLocationHierarchyForWO(Guid wo_id, string search_string);
        GetTempLocationHierarchyForWOResponseModel GetTempLocationHierarchyForWOV2(Guid wo_id, string search_string);
        GetAllTempMasterLocationForWOResponseModel GetAllTempMasterLocationDropdownList(GetAllTempMasterLocationForWORequestModel requestModel);
        GetTempLocationHierarchyForWOResponseModel GetTempLocationHierarchyForWO_V3(Guid wo_id, string search_string);
        GetTempLocationHierarchyForWOResponseModel GetActiveLocationByWO(Guid wo_id, string search_string);
        GetAllTempMasterLocationForWOResponseModel GetAllTempMasterLocationsListForWO(GetAllTempMasterLocationForWORequestModel requestModel);
        ListViewModel<GetWOOBAssetsbyLocationHierarchyResponseModel> GetWOOBAssetsbyLocationHierarchy(GetWOOBAssetsbyLocationHierarchyRequestModel requestmodel);
        ListViewModel<GetWOOBAssetsbyLocationHierarchyResponseModel> GetWOOBAssetsbyTempMasterLocationHierarchy(GetWOOBAssetsbyLocationHierarchyRequestModel requestmodel);
        Task<int> DeleteTempLocationDetails(DeleteTempLocationDetailsRequestModel requestmodel);
        Task<int> updatesubassetlocationscript();
        List<GetOBIRImagesByWOIdResponseModel> GetIRImagesByWOId(Guid wo_id);
        ListViewModel<GetOBIRImagesByWOIdResponseModel> GetIRImagesByWOId_V2(GetOBIRImagesByWOId_V2RequestModel requestModel);
        Task<AddPMtoNewWolineResponsemodel> AddPMtoNewWoline(AddPMtoNewWolineRequestmodel requst);
        Task<List<GetWODetilsForReportResponsemodel>> GetWODetilsForReport();
        Task<List<GetIssueDetailsForReportResponsemodel>> GetIssueDetailsForReport();
        GetWOTypeWiseSubmittedAssetsCountResponseModel GetWOTypeWiseSubmittedAssetsCount();

        Task<CreateTempIssueResponsemodel> CreateTempIssue(CreateTempIssueRequestmodel requestmodel);
        GetAssetListForIssueResponsemodel GetAssetListForIssue(GetAssetListForIssueRequestmodel request);
        ListViewModel<GetAllCalanderWorkordersResponseModel> GetAllCalendarWorkorders(GetAllCalanderWorkordersRequestModel requestModel);

        GetIssueWOlineDetailsByIdResponsemodel GetIssueWOlineDetailsById(string woonboardingassets_id);
        Task<int> SendNotificationsForDueOverdueWorkorders();
        Task<int> WorkorderTempAssetScriptForInstallWOline();
        Task<int> WorkorderTempAssetScriptForIssueWOline();
        Task<int> AddUpdateTimeMaterial(AddUpdateTimeMaterialRequestModel requestModel);
        GetAllTimeMaterialsForWOResponseModel GetAllTimeMaterialsForWO(GetAllTimeMaterialsForWORequestModel requestModel);
        Task<int> BulkCreateTimeMaterialsForWO(BulkCreateTimeMaterialsForWORequestmodel requestmodel);

        GetImageInfoByTextRactServiceResponsemodel GetImageInfoByTextRact(GetImageInfoByTextRactRequestmodel requestmodel);

        Task<int> ChangeIRPhotosExtention(List<string> file_url, string wo_id);
        Task<int> ChangeIRPMPhotosExtention(List<string> file_url, string wo_id);
        Task<UpdateMultiOBWOAssetsStatusResponseModel> UpdateMultiOBWOAssetsStatus(UpdateMultiOBWOAssetsStatusRequestModel requestModel);
        GetAllResponsiblePartyListResponseModel GetAllResponsiblePartyList();
        Task<int> ChangeQuoteStatus(ChangeQuoteStatusRequestModel requestModel);
        GetAllOBAssetsWithQRCodeByWOIdResponseModel GetAllOBAssetsWithQRCodeByWOId(string wo_id);

        List<GetIRImageFilePathExclude> GetImagesFilePaths(List<string> file_names, string wo_id);
        (int?, int?) GetIRWOCameraTypeFlags(Guid wo_id);
        Task<(AddExistingAssetToWorkorderByQRCodeResponsemodel, int)> AddExistingAssetToWorkorderByQRCode(AddExistingAssetToWorkorderByQRCodeRequestModel requestModel);
        GetQuoteListStatusWiseResponsemodel GetQuoteListStatusWise(string search_string, List<string>? site_id);
        GetAllTempAssetDataForWOResponseModel GetAllTempAssetDataForWO(Guid wo_id);
        Task<GenerateOnboardingWOReportResponseModel> GenerateOnboardingWOReport(GenerateOnboardingWOReportRequestModel_2 requestModel);
        Task<GenerateOnboardingWOReportResponseModel> GenerateMaintenanceWOReport(GenerateOnboardingWOReportRequestModel_2 requestModel);
        Task<CreateQuoteFromEstimatorResponsemodel> CreateQuoteFromEstimator(CreateQuoteFromEstimatorRequestmodel requestmodel);
        Task<int> AddNewTempMasterLocationData(AddTempMasterLocationDataRequestModel requestModel);
        Task<int> AddExistingTempMasterLocation(AddExistingtoTempLocationRequestmodel requestmodel);

        GetPMEstimationResponseModel GetPMEstimation(GetPMEstimationRequestModel requestModel);
    }
}
