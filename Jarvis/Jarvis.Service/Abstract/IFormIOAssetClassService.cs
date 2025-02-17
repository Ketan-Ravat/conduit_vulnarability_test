using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IFormIOAssetClassService
    {
        public GetAllAssetClassListViewModel<GetAllAssetClassResponsemodel> GetAllAssetClass(GetAllAssetClassRequestmodel request);
        public List<GetFormsByAssetclassIDResponsemodel> GetFormsByAssetclassID(string guid);
        GetFormIOFormByIdResponsemodel GetFormIOFormById(Guid form_id);
        List<GetFormPropertiesByAssetclassIDResponsemodel> GetFormPropertiesByAssetclassID(Guid inspectiontemplate_asset_class_id);
        List<GetFormIOFormByIdResponsemodel> GetFormListtoAddByAssetclassID(Guid inspectiontemplate_asset_class_id);

        List<GetFormIOFormByIdResponsemodel> GetFormListtoAddByAssetclassID_V2(GetFormListtoAddByAssetclassIDRequestModel requestModel);

        GetFormNameplateInfobyClassIdResponsemodel GetFormNameplateInfobyClassId(Guid inspectiontemplate_asset_class_id);
        Task<int> AddFormInAssetClass(AddFormInAssetClassRequestmodel request);
         Task<AddAssetClassResponsemodel> AddAssetClass(AddAssetClassRequestmodel request);
        Task<int> DeleteFormFromAssetClass( DeleteFormFromAssetClassRequestmodel request);
        Task<int> DeleteAssetClass(DeleteAssetClassRequestmodel request);
        List<GetAllAssetClassCodesResponsemodel> GetAllAssetClassCodes();
        Task<int> UpdateNamePlateinfo(UpdateNamePlateinfoRequestmodel request);
    }
}
