using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IInspectionService
    {
        ListViewModel<InspectionResponseModel> GetAllInspections(int pagesize, int pageindex);
        ListViewModel<InspectionResponseModel> FilterInspections(FilterInspectionsRequestModel requestModel);

        ListViewModel<AssetListResponseModel> FilterInspectionAssetNameOption(FilterInspectionOptionsRequestModel requestModel);

        ListViewModel<int> FilterInspectionStatusOption(FilterInspectionOptionsRequestModel requestModel);

        ListViewModel<int> FilterInspectionShiftNumberOption(FilterInspectionOptionsRequestModel requestModel);

        ListViewModel<OperatorsListResponseModel> FilterInspectionOperatorsOption(FilterInspectionOptionsRequestModel requestModel);

        ListViewModel<OperatorsListResponseModel> FilterInspectionSupervisorOption(FilterInspectionOptionsRequestModel requestModel);

        ListViewModel<SitesViewModel> FilterInspectionSitesOption(FilterInspectionOptionsRequestModel requestModel);

        ListViewModel<CompanyViewModel> FilterInspectionCompanyOption(FilterInspectionOptionsRequestModel requestModel);

        Task<bool> AddInspection(InspectionRequestModel requestModel);

        Task<InspectionResponseModel> GetInspectionById(string inspectionid);
        Task<InspectionResponseModel> GetInspectionByIdForOperator(string inspectionid, string userid);

        List<InspectionResponseModel> GetPendingInspectionByManager();

        Task<PendingInspectionCheckoutAssetsManagerResponseModel> PendingInspectionCheckoutAssetsManager();
        Task<ManagerMobileDashboardDataCountResponseModel> ManagerMobileDashboardDataCount();

        Task<List<PendingAndCheckoutInspViewModel>> PendingInspectionCheckoutAssetsByOperator();

        Task<int> ApproveInspection(ApproveInspectionRequestModel requestModel);

        Task<ListViewModel<InspectionResponseModel>> GetInspectionByAssetId(string assetid,int pagesize,int pageindex);

        Task<ListViewModel<InspectionResponseModel>> SearchInspections(string searchstring,string timezone,int pagesize,int pageindex);

        Task<ListViewModel<InspectionResponseModel>> SearchInspectionsByAsset(string assetId, string searchstring, string timezone, int pagesize, int pageindex);

        Task<int> CreateInpsectionOffline(InspectionRequestModel inspectionRequestModel);

        Task<List<string>> UploadbulkInspection(UploadInspectionRequestModel requestModel);

        int CheckPendingInspection(string assetGuid, string operatorId);

        int CheckAssetInspectionByTime(string assetGuid, string operatorId, string requested_datetime);

        MasterDataResponseModel GetMasterData();

        //Task SendEmailNotificationForPendingInspection(CancellationToken cancellationToken);
        Task SendPendingInspectionEmail();

        Task<InspectionIssueOfflineResponseModel> InspectionIssueOffline(OfflineSyncDataRequestModel requestModel);

        Task GetOperatorUsageReport();
        Task GetExecutiveDailyReport();
        Task GetExecutiveWeeklyReport();

        Task<ExistingThumbnailResponseModel> GetAllInspectionImages(string awsAccessKey, string awsSecretKey, string bucketName, string folderPathName, int height, int width);
    }
}
