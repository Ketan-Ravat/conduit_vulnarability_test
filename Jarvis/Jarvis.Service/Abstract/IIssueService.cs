using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IIssueService
    {
        Task<CreateIssueResponseModel> CreateIssue(IssueRequestModel requestModel);

        ListViewModel<IssueResponseModel> GetAllIssues(int status, int pagesize, int pageindex);

        ListViewModel<IssueResponseModel> FilterIssues(FilterIssueRequestModel requestModel);
        ListViewModel<IssuesNameResponseModel> FilterIssuesTitleOption(FilterIssueRequestModel requestModel);
        ListViewModel<AssetListResponseModel> FilterIssuesAssetOption(FilterIssueRequestModel requestModel);
        ListViewModel<SitesViewModel> FilterIssuesSitesOption(FilterIssueRequestModel requestModel);
        ListViewModel<IssuesNameResponseModel> GetAllIssuesTitle(string siteid);

        Task<IssueResponseModel> GetIssueByID(string issueId);

        Task<int> UpdateIssueByManager(UpdateIssueRequestModel requestModel);

        ListViewModel<IssueResponseModel> GetTodayNewIssues(int pagesize, int pageindex);

        Task<int> UpdateIssueByMaintenance(UpdateIssueByMaintenanceRequestModel requestModel);

        ListViewModel<IssueResponseModel> GetIssueByAssetId(string assetid,int pagesize,int pageindex);

        ListViewModel<IssueResponseModel> SearchIssueByAssetId(string assetid,string searchstring, int pagesize, int pageindex);

        ListViewModel<IssueResponseModel> SearchIssues(string searchstring,string timezone, int pagesize, int pageindex);

        ListViewModel<AssetIssueResponseModel> GetAssetsIssue(int pagesize, int pageindex);

        ListViewModel<IssueResponseModel> GetAllIssue(string timestamp, int pagesize, int pageindex);

        Task<int> CreateIssuesFromWorkOrder();
        Task<int> CreateIssueStatus();
        Task<int> CreateIssueRecords();

        Task<LinkIssueToWOFromIssueListTabResponsemodel> LinkIssueToWOFromIssueListTab(LinkIssueToWOFromIssueListTabRequestModel requestmodel);
        Task<LinkIssueToWOFromIssueListTabResponsemodel> LinkIssueToWOFromIssueListTabForSteps(LinkIssueToWOFromIssueListTabRequestModel requestmodel);

        Task<int> AddIssueBySteps(AddIssueByStepsRequestmodel requestmodel);
        Task<int> UpdateIssueBySteps(UpdateIssueByStepsRequestmodel requestmodel);
        ListViewModel<GetAllAssetIssuesOptimizedResponsemodel> GetAllAssetIssuesOptimized(GetAllAssetIssuesRequestmodel requestmodel);
        GetAllIssueByWOidResponsemodel GetAllIssueByWOidOptimized(GetAllIssueByWOidRequestmodel requestmodel);
        Task<int> UpdateIRVisualImageLabel(UpdateIRVisualImageLabelRequestModel requestmodel);
        Task<GenerateOnboardingWOReportResponseModel> GenerateSiteIssuesReport(string report_type);
    }
}
