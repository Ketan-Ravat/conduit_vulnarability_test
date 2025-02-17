using Jarvis.db.DBResponseModel;
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
    public interface IInspectionRepository
    {
        Task<int> Insert(Inspection entity);

        List<Inspection> GetAllInspections();

        Inspection GetInspectionById(string inspectionid, string userid);
        Inspection GetInspectionByIdForOperator(string inspectionid, string userid);

        string FindUserNameById(Guid operator_id);

        InspectionFormAttributes GetAttributesCategoryFromId(Guid attributes_id);

        InspectionFormAttributes GetAttributesFromName(string attributes_name);

        Task<List<Inspection>> PendingInspection(string userid);

        Task<List<Inspection>> PendingInspectionByOperator(string userid);

        Task<List<Inspection>> CheckOutAssets(string userid);

        Task<List<Inspection>> CheckOutAssetsByOperator(string userid);

        Task<int> UpdateInspectionStatus(Guid inspection_id, int status, bool isapproved,string userid);
        //Task<bool> AddInspection(InspectionRequestModel requestModel);

        Task<Inspection> ApproveInspectionStatus(ApproveInspectionRequestModel requestModel);

        Task<Inspection> GetLastInspectionByInternalAssetId(string internal_asset_id);

        string GetUserNameById(Guid user_id);

        Task<List<Inspection>> GetInspectionByAssetId(string userid, string assetid, int pagesize, int pageindex);

        Task<List<Inspection>> SearchInspections(string userid, string searchstring, string timezone, int pagesize, int pageindex);

        Task<List<Inspection>> SearchInspectionsByAsset(string userid,string assetId, string searchstring, string timezone, int pagesize, int pageindex);

        Task<List<Inspection>> GetAllInspections(string userid,DateTime start,DateTime end);

        int CheckPendingInspection(string assetGuId, string operatorId);

        int CheckAssetInspectionByTime(string assetGuid, string operatorId, string requested_datetime);

        int GetPendingInspection(string inspection_id);

        MasterData GetMasterData();

        Task<List<Inspection>> FindAllPeningInspection();

        InspectionListResponseModel GetInspections(string internal_asset_id, Nullable<DateTime> from_date = null, Nullable<DateTime> to_date = null);

        ListViewModel<Inspection> FilterInspections(FilterInspectionsRequestModel requestModel);

        ListViewModel<Inspection> FilterInspectionAssetNameOptions(FilterInspectionOptionsRequestModel requestModel);

        List<Inspection> FilterInspectionStatusOptions(FilterInspectionOptionsRequestModel requestModel);

        ListViewModel<Inspection> FilterInspectionShiftNumberOptions(FilterInspectionOptionsRequestModel requestModel);

        ListViewModel<Inspection> FilterInspectionOperatorsOptions(FilterInspectionOptionsRequestModel requestModel);

        ListViewModel<Inspection> FilterInspectionSupervisorOptions(FilterInspectionOptionsRequestModel requestModel);

        ListViewModel<Inspection> FilterInspectionSitesOptions(FilterInspectionOptionsRequestModel requestModel);

        ListViewModel<Inspection> FilterInspectionCompanyOptions(FilterInspectionOptionsRequestModel requestModel);

        List<Inspection> GetInspectionsForWeeklyReport(List<Guid> siteid, DateTime startdate, DateTime enddate);

        List<ImagesListObject> GetAllInspectionsImages();
    }
}
