using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.db.Models;
using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IFormIOAssetClassRepository
    {
        (List<InspectionTemplateAssetClass> , int) GetAllAssetClass(GetAllAssetClassRequestmodel request);
        List<InspectionTemplateFormIoExclude> GetFormsByAssetclassID(Guid inspectiontemplate_asset_class_id);
        InspectionsTemplateFormIO GetFormIOFormById(Guid form_id);
        List<InspectionTemplateFormIoExclude> GetFormPropertiesByAssetclassID(Guid inspectiontemplate_asset_class_id);
        List<InspectionTemplateFormIoExclude> GetFormListtoAddByAssetclassID(Guid inspectiontemplate_asset_class_id);

        List<InspectionTemplateFormIoExclude> GetFormListtoAddByAssetclassID_V2(GetFormListtoAddByAssetclassIDRequestModel requestModel);

        List<InspectionTemplateAssetClass> GetAssetclassByAssetclassCodes(List<string> assetclass_codes);
        List<InspectionTemplateAssetClass> GetAllAssetClassForList();

        InspectionTemplateAssetClass GetAssetclassbyID(DeleteAssetClassRequestmodel request);
        InspectionTemplateAssetClass GetAssetclassbyIDForNameplateinfo(Guid inspectiontemplate_asset_class_id);

        WOOnboardingAssets GetWolinebyAssetclassCode(string asset_class_code);
        bool GetIsAssetClassEnableFlagBySiteId();
        public PMPlans GetDefaultPmPlan(Guid class_id);
    }
}
