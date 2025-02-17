using System;
using System.Collections.Generic;
using System.Text;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System.Threading.Tasks;
using Jarvis.ViewModels;
using Jarvis.db.Models;

namespace Jarvis.Service.Abstract
{
    public interface IAssetFormIOService
    {
        Task<AssetFormIOResponseModel> GetAssetFormIOByAssetId(Guid asset_id);
        Task<AssetFormIOResponseModel> AddUpdateAssetFormIO(AssetFormIORequestModel formRequest,string LVCB_Form_id);
        Task<AddUpdateAssetFormIOOfflineResponsemodel> AddUpdateAssetFormIOOffline(AddUpdateAssetFormIOOfflineRequestModel formRequest, string offline_sync_bucket, string sqs_aws_access_key, string sqs_aws_secret_key);
        Task<int> UpdateOnlyAssetFormIO(UpdateOnlyAssetFormIORequestmodel request);
        ListViewModel<AssetFormIOResponseModel> GetAllAssetTemplateList(GetAllAssetInspectionListByAssetIdRequestModel request);
        
        Task<AssetFormIOReportStatusResponsemodel> GetReportStatus(string asset_form_id);

        Task<int> ExractandStoreOnlydatafromOldForm();
        Task<int> UpdateAssetInfo(UpdateAssetInfoRequestmodel request);

        ListViewModel<FormIOInsulationResistanceTestResponseModel> GetFormIOInsulationResistanceTest(FormIOInsulationResistanceTestRequestModel request);
        Task<int> changeassetformiostatus(changeassetformiostatusRequestmodel request);
        Task<int> ChangeAssetFormIOStatusFormultiple(ChangeAssetFormIOStatusFormultipleRequestmodel request);
        List<GetAssetsForSubmittedFilterOptionsResponsemodel> GetAssetsForSubmittedFilterOptions();
        List<GetWorkOrdersForSubmittedFilterOptionsResponsemodel> GetWorkOrdersForSubmittedFilterOptions();
        List<GetInspectedForSubmittedFilterOptionsResponsemodel> GetInspectedForSubmittedFilterOptions();
        List<GeApprovedForSubmittedFilterOptionsResponsemodel> GetApprovedForSubmittedFilterOptions();
        GetAssetFormJsonbyIdResponsemodel GetAssetFormJsonbyId(GetAssetFormJsonbyIdRequestmodel request);

        Task<int> updateformjson();

        //ListViewModel<Asset> ReplaceAssetformIOJson(Guid siteId, Guid companyId);
        Task<int> ReplaceAssetformIOJson(Guid siteId, Guid companyId);
        GetAssetformByIDResponsemodel GetAssetformByID(string asset_form_id);
        GetAssetformByIDForBulkReportResponsemodel GetAssetformByIDForBulkReport(GetAssetformByIDForBulkReportRequestmodel requestmodel);
        GetAssetformEquipmentListResponsemodel GetAssetformEquipmentList(GetAssetformEquipmentListRequestmodel requestmodel);
        ListViewModel<GetAllEquipmentListResponsemodel> GetAllEquipmentListService(GetAllEquipmentListRequestmodel requestmodel);
        Task<AddUpdateEquipmentResponseModel> AddUpdateEquipmentService(AddUpdateEquipmentRequestModel request);
        Task<int> DeleteEquipmentService(Guid equipmentId); 
        Task<FilterAttributesEquipmentResponseModel> FilterAttributesEquipmentService();
        GetAssetFormIOByAssetIdResponseModel GetAssetFormIOByAssetID(Guid asset_id);
        Task<int> ScriptForAddNFPA70BToAssets();
        Task<int> scriptforgehealtcare();
        List<GetAssetsForSubmittedFilterOptionsResponsemodel> GetAssetsForSubmittedFilterOptionsByStatus(GetAssetsForSubmittedFilterOptionsByStatusRequestModel requestmodel);
        List<GetWorkOrdersForSubmittedFilterOptionsResponsemodel> GetWorkOrdersForSubmittedFilterOptionsByStatus(GetAssetsForSubmittedFilterOptionsByStatusRequestModel requestmodel);
        Task<int> UpdateEquipmentCalibrationStatusByDateAndInterval();
        Task<int> GenerateBulkNetaReport(GenerateBulkNetaReportRequestModel requestModel, string aws_access_key, string aws_secret_key);
         Task<ListViewModel<GetAllNetaInspectionBulkReportTrackingListResponseModel>> GetAllNetaInspectionBulkReportTrackingList(GetAllNetaInspectionBulkReportTrackingListRequestModel requestModel);
    }
}
