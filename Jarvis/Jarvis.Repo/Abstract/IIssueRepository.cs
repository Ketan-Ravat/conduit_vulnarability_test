using Jarvis.db.Models;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IIssueRepository
    {
        Task<bool> Update(Issue entity); 

        List<Issue> GetIssueByAssetId(string asset_id,int pagesize, int pageindex);

        List<Issue> GetIssueByInternalAssetId(string internal_asset_id, int pagesize, int pageindex);

        int CreateIssue(Issue requestModel);
        List<Issue> GetIssues(string user_id, int status, int pagesize, int pageindex);

        List<Issue> FilterIssues(FilterIssueRequestModel requestModel);
        List<Issue> FilterIssuesTitleOption(FilterIssueRequestModel requestModel);
        List<Issue> FilterIssuesAssetOption(FilterIssueRequestModel requestModel);
        List<Issue> FilterIssuesSitesOption(FilterIssueRequestModel requestModel);

        List<Issue> GetIssuesTitle(string siteid);
        Issue GetIssueByIssueId(Guid work_order_uuid);

        int UpdateIssue(Issue issue);
        //int UpdateWorkOrder(requestModel);

        int CreateIssueStatus(IssueStatus entity);
        Issue GetIssueById(string issueId, string userid);

        int ValidateInternalAssetID(string userid, string internal_asset_id);

        List<Issue> GetTodayNewIssues(string userid, int pagesize, int pageindex);

        List<Issue> GetIssueByAssetId(string userid, string assetid, int pagesize, int pageindex);

        List<Issue> SearchIssueByAssetId(string userid, string assetid,string searchstring, int pagesize, int pageindex);

        List<Issue> SearchIssues(string userid, string searchstring, string timezone, int pagesize, int pageindex);
        //int AddWorkOrderStatus(WorkOrderStatus workOrderStatus);

        Issue GetIssueByInspectionId(string inspectionid);
        List<Issue> GetIssuesByInspectionId(string inspectionid);

        Issue GetAttributeIssueByBarcodeId(string attributes_id, string barcode_id);

        Issue GetAttributeIssueByInspectionId(string attributes_id, string inspection_id);
        Issue GetAttributeIssueByInternalAssetId(string attributes_id, string internal_asset_id);

        bool HaveAlreadyIssue(Guid attribute_id, Guid asset_id, DateTime date_time, DateTime inspection_created_at);

        long GetMaxIssueNumber(Guid site_id);
        List<Asset> GetAssetsIssue(string userid, int pagesize, int pageindex,int status);
        List<Issue> GetAllIssues(string userid, string timestamp);
        Task<List<Issue>> GetPendingIssues(string userid);
        Task<int> CreateIssueFromIssue();
        Task<int> CreateIssueStatus();
        Task<int> CreateIssueRecords();
        List<Issue> GetIssuesForDailyReport(List<Guid> siteid, DateTime startdate, DateTime enddate);
        Issue GetIssueByMrId(Guid mr_uuid);

        TempAsset GetTempIssueByAssetId(Guid asset_id , Guid wo_id);
        TempAsset GetTempIssueByAssetIdAsnotracking(Guid asset_id , Guid wo_id);

        WOOnboardingAssets GetWolneById(Guid woonboardingassets_id);
        TempAsset GetWolneTempAssetById(Guid woonboardingassets_id);
        WOOnboardingAssets GetWolneByIdAsnotracking(Guid woonboardingassets_id);

        TempAsset GetTempAssetbyId(Guid tempasset_id);

        AssetIssue GetMainIssue(Guid asset_issue_id);

        WOLineIssue GetwolineissueById(Guid wo_line_issue_id);
        (List<GetAllAssetIssuesOptimizedResponsemodel>, int) GetAllAssetIssuesOptimized(GetAllAssetIssuesRequestmodel requestmodel);
        List<GetAllWOLineTempIssuesResponsemodel> GetAlltempIssuebyWOidOptimized(GetAllIssueByWOidRequestmodel requestmodel);
        List<link_main_issue_list> GetAllmainIssuebyWOidOptimized(GetAllIssueByWOidRequestmodel requestmodel);
        IRWOImagesLabelMapping GetIRWOImageLabelMappingById(Guid irwoimagelabelmapping_id);
    }
}
