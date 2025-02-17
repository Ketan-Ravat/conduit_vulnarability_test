using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.db.Models;
using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IAssetFormIORepository
    {
        Task<int> Insert(AssetFormIO entity);
        Task<int> Update(AssetFormIO entity);
        Task<AssetFormIO> GetAssetFormIOById(Guid form_id);
        AssetFormIO GetAssetFormIOByIdMobile(Guid form_id);
        Task<AssetFormIO> GetAssetFormIOByAssetId(Guid asset_id);
        AssetFormIO GetAssetFormIOBytId(Guid asset_form_id);
        (List<AssetFormIO> , int total_size) GetAllAssetTemplateList(GetAllAssetInspectionListByAssetIdRequestModel request);

        (List<AssetFormIOExcludeNew>, int total_size) GetAllAssetTemplateListNew(GetAllAssetInspectionListByAssetIdRequestModel request);


        List<AssetFormIO> ExractandStoreOnlydatafromOldForm(int skip, int take);
        int ExractandStoreOnlydatafromOldFormcount();

        Asset GetAssetForUpdate(Guid asset_id);
        FormIOInsulationResistanceTestMapping GetFormIOInsulationResistanceTestMappingByAssetFormID(Guid asset_form_id);

        (List<FormIOInsulationResistanceTestMapping>,int) GetFormIOInsulationResistanceTest(FormIOInsulationResistanceTestRequestModel request);
        AssetFormIO GetAssetFormIOByIdForStatusShange(Guid asset_form_id);
        List<AssetFormIO> GetAssetFormIOByIdForStatusShangeFormultiple(List<Guid> asset_form_id);
        bool IsWOCompleted(List<Guid> asset_form_id);
        bool is_form_completed(List<Guid> asset_form_id);

        List<string> GetAssetsForSubmittedFilterOptions(GetAssetsForSubmittedFilterOptionsByStatusRequestModel requestmodel);

        List<WorkOrders> GetWorkOrdersForSubmittedFilterOptions(GetAssetsForSubmittedFilterOptionsByStatusRequestModel requestmodel);

        List<Guid> GetInspectedForSubmittedFilterOptions();
        List<string> GetApprovedForSubmittedFilterOptions();
        List<AssetFormIO> Getallassetformios();

        List<AssetFormIO> GetFormtoUpdate();

        InspectionsTemplateFormIO GetMasterFormByformID(Guid form_id);

        List<AssetFormIO> GetAssetformsByIds(List<Guid> asset_form_ids);
        List<InspectionsTemplateFormIO> GetMasterFormDataByIds(List<Guid> form_ids);
        AssetFormIO GetAssetFormIOByIdForTempIssue(Guid form_id);

        List<WOLineIssue> GetNECTempIssueByAssetFormid(Guid asset_form_id);
        List<WOLineIssue> GetOSHATempIssueByAssetFormid(Guid asset_form_id);

        (List<Equipment>, int) GetAllEquipmentListData(GetAllEquipmentListRequestmodel request);

        Equipment GetEquipmentDataByID(Guid equipmentId);
        List<string> GetDropdownEquipmentNumber(); 
        List<string> GetDropdownEquipmentManufacturer();
        List<string> GetDropdownEquipmentModelNumber();
        List<int?> GetDropdownEquipmentCalibrationStatus();
        bool CheckForDuplicateEquipmentNumber(string equipmentNumber);
        bool CheckForDuplicateEquipmentNumberForUpdate(string equipmentNumber, Guid equipment_id);

        Company GetCompanyById(Guid company_id);
        AssetFormIO GetAssetFormIOByAssetID(Guid asset_id);
        List<Asset> GetAllAssetsToAddPMs();
        List<Asset> GetAllAssetsToAddPMs2(List<string> list);
        List<AssetPMPlans> GetAssetPMPlanByAssetId(Guid asset_id);
        Guid GetassetclassbyClassCode(string code, string name);
        Guid GetassetclassbyClassCode(string code);
        List<Equipment> GetAllEquipmentListForUpdateCalStatus();
        FormIOBuildings GetFormIOBuildingByName(string building, Guid site_id);
        ClientCompany GetCCbyname(string cc_name);
        Sites Getsitebyname(string site_name, Guid cc_id);
        FormIOFloors GetFormIOFloorByName(string floor_name, int building_id, Guid site_id);
        FormIORooms GetFormIORoomByName(string room_name, int floor_id, Guid site_id);
        FormIOSections GetFormIOSectionByName(string section_name, int room_id, Guid site_id);
        (List<NetaInspectionBulkReportTracking>, int) GetAllNetaInspectionsReportList(GetAllNetaInspectionBulkReportTrackingListRequestModel requestModel);
        List<string> GetAssetNamesByAssetFormIds(List<string> asset_form_ids_list);
        int GetNetareportCountBySite();
        List<TempAsset> GetAllTempAssetsForScript(Guid site_id);
        List<Guid> GetAllSitesByCompany();
    }
}
