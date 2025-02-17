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
    public interface IMobileWorkorderRepository
    {
        (List<WorkOrders>, int total_list_count) GetAllWorkOrdersNewflow(string userid, NewFlowWorkorderListRequestModel requestModel);
        WorkOrders ViewWorkOrderDetailsById(Guid wo_id);
        List<Tasks> GetAllTasks(string searchstring);
        List<WOcategorytoTaskMapping> GetWOcategoryTaskByCategoryID(string wo_inspectionsTemplateFormIOAssignment_id);
        (List<AssetFormIO>, int total_size) GetAllAssetTemplateList(string assetid, int pagesize, int pageindex);
        ListViewModel<Asset> GetSubAssetsByAssetID(string asset_id, int pagesize, int pageindex);
        public List<Issue> FilterIssues(FilterIssueRequestModel requestModel);
        ListViewModel<Asset> FilterAssets(FilterAssetsRequestModel requestModel);
        Issue GetIssueById(string issueId, string userid);
        Task<AssetFormIO> GetAssetFormIOByAssetFormId(Guid asset_form_id);

        (List<AssetPMs>, int) GetAssetPMList(GetAssetPMListMobileRequestmodel requestModel);

        AssetPMs GetAssetpmById(Guid asset_pm_id);

        DeviceInfo GetDeviceInfoByuuid(Guid UUID);
    }
}
