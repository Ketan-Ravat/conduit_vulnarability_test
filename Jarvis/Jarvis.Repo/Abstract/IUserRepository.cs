using Jarvis.db.Models;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IUserRepository
    {
        List<User> GetUsers(int status, int pageindex, int pagesize);

        List<User> FilterUsers(FilterUsersRequestModel requestModel);
        List<User> FilterUsersOptimized(FilterUsersRequestModel requestModel);
        GetAllTechniciansListResponseModel GetAllTechniciansList();
        GetAllTechniciansListResponseModel GetAllTechniciansListBySiteId(Guid site_id);
        GetAllTechniciansListResponseModel GetAllBackOfficeUsersList();
        GetAllTechniciansListResponseModel GetAllBackOfficeUsersListBySiteId(Guid site_id);
        List<User> FilterUsersRoleOptions(FilterUsersRequestModel requestModel);
        List<User> FilterUsersSitesOptions(FilterUsersRequestModel requestModel);
        List<User> FilterUsersCompanyOptions(FilterUsersRequestModel requestModel);

        Task<User> GetUserByID(string uuid);
        Task<User> GetUserByIDForverify(string email, string company_id);
        Task<User> UserLogin(LoginRequestModel request);

        User GetUserSessionDetails(string uuid);
        User GetUserSessionDetailsOptimized(string uuid);
        UserRoles activeRoleCheck(Guid uuid, string active_rolename_web);
        UserSites activeSiteCheck(Guid uuid, string active_site_id);

        Task<User> UserBarcodeLogin(AuthenticateTokenRequestModel request);

        Task<int> Insert(User entity);

        Task<int> Update(User entity);
        int CheckUserValid(string userid, Guid device_uu_id);
        Task<User> FindUserDetails(Guid uid);

        Task<UserSites> FindUserSitesDetails(Guid usid);

        Task<string> GetUserRoleFromId(string userid);

        Sites GetSiteBySiteId(Guid site_id);

        List<UserSites> GetAllManager(string ref_id);

        List<UserSites> GetAllManagerByIssueID(string ref_id);

        List<UserSites> GetAllManagerByAssetPMID(string ref_id);

        String GetUserFromUserId(string user_id);

        List<UserSites> GetAllOperator(string ref_id);

        List<UserSites> GetAllOperatorByIssueID(string ref_id);

        List<UserSites> GetAllOperatorAndMaintenceStaff(string ref_id);

        List<UserSites> GetAllMaintenanceStaffByIssueID(string ref_id);

        bool Logout();

        User GetUserByIDFromParent(string uuid);

        Guid GetBarcodeIdById(string userid);

        List<UserSites> GetUserSiteById(string userid);

        List<UserSites> GetOperatorForNotification(string inspection_id);

        List<UserSites> GetMaintenanceStaffByIssueID(string issue_id);

        Task<List<Role>> GetRoles();

        List<User> GetUserDetails(string userid, string timestamp, int pageindex, int pagesize);

        Task<List<User>> SearchUser(string searchstring, int pageindex, int pagesize);

        Task<List<User>> GetUsersByToken(string notification_token);

        List<UserSites> GetAllManager();

        Guid? GetUserSite(string userid);

        string GetUserNameByID(string user_id);
        
        User GetUserByEmailID(string email_id);

        ResetPasswordToken GetUserTokenByTokenID(string token);

        List<User> GetAllUsers();
        bool RoleAlreadyAdded(Guid uuid, Guid role_id);

        Task<string> GetUserRole();

        Task<List<Role>> GetAllRoles();
        List<UserSites> GetAllManagerForOperatorUsageReport();
        Task<List<User>> GetAllOperatorsList();
        List<User> GetAllExecutiveForDailyReport();
        List<User> GetAllExecutiveForWeeklyReport();
        List<UserSites> GetAllExecutiveForPendingInspectionEmail();
        List<User> GetAllManagerForPMNotification();
        Task<NotificationData> GetNotificationByID(Guid notification_id);
        List<UserSites> GetAllManagerForPMNotifications(Guid company_id);
        List<User> GetAllExecutiveForPMDueReport();
        Task<UserEmailNotificationConfigurationSettings> GetExecutiveRolePMDueReportConfig(Guid uuid);
        Task<User> GetInternalUserByID(string uuid);
        User GetUserByUserID(string user_id);
        public  Sites GetAllSiteMaster();
        ClientCompany GetClientCompanyBySiteid(Guid SiteID);
        List<ClientCompany> GetClientCompanyByParentCompany(Guid CompanyID);
        List<UserRoles> GetUserRolesByEmail(String email_id);
        MobileAppVersion MobileAppVersion(int device_brand);
        ClientCompany GetClientCompanyById(Guid client_company);
        (List<ClientCompany>, int) GetAllClientCompanyWithSites(GetAllClientCompanyWithSitesRequestModel request);
        List<User> GetAllCompanyAdmins(Guid company_id);

        bool CheckForTechnicianSiteAccess(Guid requested_by, Guid site_id);
        WorkOrderWatcherUserMapping GetWorkorderWatcherMappingById(AddUpdateWorkOrderWatcherRequestModel requestModel);
        List<string> GetUserRolesById(Guid user_id);
        List<Guid> GetAllBackOfficeUsersBySiteId(Guid site_id);
        (List<NotificationData>, int) GetNotificationsDataByUserId(string userid, string notification_user_role, int pagesize, int pageindex);
        int GetNotificationCountByUserId(Guid user_id, string notification_user_role, int? notification_status);

        List<WorkOrderTechnicianMapping> GetUserActiveWOForInactiveSite(List<Guid> remove_site, Guid user_id);
        List<WorkOrderTechnicianMapping> GetUserActiveWOForInactiveRole(Guid user_id);
        int? GetWOTypeById(Guid wo_id);
        User GetUserByIdForSites(Guid user_id);
        List<Sites> GetActiveUserSitesById(Guid user_id);
        List<Role> GetActiveUserRolesById(Guid user_id);
        List<ClientCompany> GetActiveClientCompany(List<Guid> sites);
        string GetRoleNameById(Guid role_id);
        Guid GetSiteCompanyId(string site_id);
        SiteProjectManagerMapping GetSiteProjectManagerMappingById(Guid site_projectmanager_mapping_id);
        GetAllTechniciansListResponseModel GetAllProjectManagersList();
        List<UserSession> GetActiveUserSessionByUUId(Guid user_id);
        bool CheckUserSessionIsValidOrNot(Guid user_id, Guid user_session_id, Guid role_id);
        GetAllTechniciansLocationByWOIdResponseModel GetAllTechniciansLocationByWOId(Guid wo_id);
        bool CheckIsWOOverdueByWeekOrNot(string wo_id);
        List<NotificationData> GetAllNewNotificationsByTypeUserId(Guid user_id, int notification_type, string ref_id);
        GetAllTechniciansListResponseModel GetAllTechniciansForCalendar(GetAllCalanderWorkordersRequestModel requestModel, bool is_request_for_technician);
        bool IsFeatureRequiredOrNotForCompany(Guid company_id,Guid feature_id);
        GetAllFeaturesFlagsByCompanyResponseModel GetAllFeaturesByCompanyId(Guid company_id);
        CompanyFeatureMapping GetCompanyFeatureMappingById(Guid company_feature_id);
        List<Sites> GetSitesByClientCompanyId(Guid client_company_id);
        List<string> GetUserWithSingleSite(Guid site_id, Guid company_id);
        List<string> GetUserWithSingleClientComapany(Guid clientcompany_id, Guid company_id);
        Vendors GetVendorDetailsById(Guid vendor_id);
        List<Workorders_Data_Model> GetWorkordersByVendorId(Guid vendor_id);
        Vendors GetVendorById(Guid vendor_id);
        Contacts GetContactById(Guid contact_id);
        (List<Vendors>, int) GetAllVendorList(GetAllVendorListRequestModel requestModel);
        List<WO_Vendor_Contacts_Mapping_View_Class> GetAllVendorsContactsForDropdown();
        bool IsMFAEnabled(string user_pool_id);
        string GetComapnyIdFromDomain(string domain_name);
        Guid GetContactIdByEmail(string email,Guid wo_id);
        WorkordersVendorContactsMapping GetWOContactMapById(Guid contact_id, Guid wo_id);
        GetRefreshedContactsByWOIdResponseModel GetRefreshedContactsByWOId(Guid wo_id);
        List<Guid> GetWOIdsByVendorId(Guid vendor_id);

        CompanyFeatureMapping GetUserCompanyFeatureRecord(Guid UserId);
        GetSiteUsersDetailsByIdResponseModel GetSiteUsersDetailsById(Guid site_id);
        ClientCompany GetDefaultClientCompany();
        SiteContact GetSiteContactById(Guid sitecontact_id);
        UserSites GetUserSiteById(Guid uuid);
    }
}
