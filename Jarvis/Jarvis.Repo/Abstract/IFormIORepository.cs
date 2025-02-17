using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IFormIORepository
    {
        public (List<FormIOFormsExcludedProprties> , int) GetAllForms(int page_size, int page_index, string search_string);
        public (List<InspectionsTemplateFormIO> , int) GetAllFormsforOffline(DateTime? sync_time);
        Task<int> Insert(InspectionsTemplateFormIO entity);
        Task<int> Update(InspectionsTemplateFormIO entity);
        Task<InspectionsTemplateFormIO> GetFormIOById(Guid form_id);
        Task<InspectionsTemplateFormIO> GetFormIOByIdForDelete(Guid form_id);
        List<Asset> GetAssetsByCompanyID(string company_id , string userid);
        List<MaintenanceRequests> GetMRsByCompanyID(string company_id, string userid);
        public (List<InspectionsTemplateFormIO>, int) GetAllFormNames(int page_size, int page_index);
        bool IsValidWorkProcedure(string work_procedure, Guid form_id);
        bool IsValidFormName(string form_name, Guid form_id);
        List<InspectionsTemplateFormIO> GetFormsByIds(List<Guid> form_id);
        Task<List<FormIOType>> GetAllFormTypes(string searchstring);
        bool IsFormMappedWithAssetclass(Guid form_id, Guid inspectiontemplate_asset_class_id);
        AssetClassFormIOMapping GetFormMappedWithAssetclass(Guid form_id, Guid inspectiontemplate_asset_class_id);
        List<FormIOFormsExcludedProprties> GetFormsExcludedByIds(List<Guid> form_ids);
        List<WorkOrders> GetWorkOrdersByIds(List<Guid> workOrderIds);
        AssetClassFormIOMapping GetAssetclassFormIOmappingByID(Guid asset_class_formio_mapping_id);

        AssetFormIO ReplaceAssetformIOJson(Guid assetformid);
        List<AssetFormIO> ReplaceAssetformIOJsonAll(Guid siteId);
        List<Asset> ReplaceAssetLocationScript(Guid siteId);
        int InspectionRFRCount();
        int InspectionCompletedCount();
        InspectionsTemplateFormIO GetFormDataTemplateByFormId(Guid form_id);
        int OpenAssetIssuesCount();

        List<AssetIssue> GetAssetIssuesForDashboardCount();
    }
}
