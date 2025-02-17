using Jarvis.db.Models;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Jarvis.Service.Concrete.UserService;

namespace Jarvis.Service.Abstract
{
    public interface IUserService
    {
        ListViewModel<GetUserResponseModel> GetUsers(int status, int pageindex, int pagesize);
        ListViewModel<GetUserResponseModel> FilterUsers(FilterUsersRequestModel requestModel);
        ListViewModel<FilterUsersOptimizedResponsemodel> FilterUsersOptimized(FilterUsersRequestModel requestModel);
        GetAllTechniciansListResponseModel GetAllTechniciansList(string site_id);
        GetAllTechniciansListResponseModel GetAllBackOfficeUsersList(string site_id);
        ListViewModel<GetRolesResponseModel> FilterUsersRoleOptions(FilterUsersRequestModel requestModel);
        ListViewModel<SitesViewModel> FilterUsersSitesOptions(FilterUsersRequestModel requestModel);
        ListViewModel<CompanyViewModel> FilterUsersCompanyOptions(FilterUsersRequestModel requestModel);
        Task<User> GetUserByID(string userid);
        Task<LoginResponceModel> UserLogin(LoginRequestModel request);

        // -------- Insert Update user Details  ------------ //
        Task<LoginResponceModel> InsertUpdateUserDetails(UserRequestModel user, string awsAccessKey, string awsSecretKey);

        Task<bool> AddRole(RoleRequestModel role);

        Task<int> SendNotification(SendNotificationRequest request);
        //Task<UsersitesResponseModel> UserSites(UsersitesRequestModel requestModel);
        NotificationListViewModel<NotificationViewModel> GetNotification(string userid, string role_id, int pagesize, int pageindex);

        int GetNotificationsCount(int? status);

        Task<int> UpdateNotificationStatus(UpdateNotificationStatusRequestModel requestModel);

        Task<int> MarkAllNotificationStatus(int status, Guid user_id);

        bool Logout();

        GetUserResponseModel GetUserByIDFromParent(string uuid);

        Task<BaseViewModel> ResetPassword(string userid, string uuid);

        Guid GetBarcodeIdById(string userid);

        Task<int> ResetUserBarcode(string userid,string uuid);

        Task<ListViewModel<GetRolesResponseModel>> GetRoles();

        Task<int> UpdateUserStatus(UpdateUserStatusRequestModel requestModel);

        ListViewModel<GetUserDetailsResponseModel> GetUserDetails(string timestamp, int pageindex, int pagesize);

        Task<int> UpdateUserToken(UpdateUserTokenRequestModel request);

        Task<ListViewModel<GetUserResponseModel>> SearchUser(string searchstring,int pageindex,int pagesize);

        Task<int> TriggerEmailNotificationStatus(bool status);

        Task<int> TriggerPMNotificationStatus(bool status);

        Task AddUserRoles(CancellationToken cancellationToken);
        Task<int> UpdateDefaultRole(UpdateDefaultRoleRequestModel requestModel);
        Task<int> UpdateDefaultSite(UpdateDefaultSiteRequestModel requestModel);
        Task<int> TriggerOperatorUsageEmailNotificationStatus(string userid, bool status);


        Task<int> ResetUserPassword(UpdateUserPasswordRequestModel requestModel);        
        
        Task<int> ResetPasswordEmail(string email);

        Task<string> GetQuickSightEmbedUrlAsync();

        Task<JwtResponse> UserBarcodeLogin(AuthenticateTokenRequestModel request, JWTTokenRequestModel JWTTokenDetails);

        LoginResponceModel GetUserSessionByUserIDAsync(string userid);
        LoginResponceModel GetUserSessionByUserIDAsyncOptimized(string userid);

        Guid GetSiteCompanyId(string site_id);
        UserRoles activeRoleCheck(Guid uuid, string active_rolename_web);
        UserSites activeSiteCheck(Guid uuid, string active_site_id);

        Task<int> UpdateActiveRole(UpdateActiveRoleRequestModel requestModel);

        Task<int> UpdateActiveSite(UpdateActiveSiteRequestModel requestModel);
        Task<ListViewModel<GetUserResponseModel>> GetAllOperatorsList();
        Task<int> ResendCognitoUser(string uuid, string awsAccessKey, string awsSecretKey);
        Task<int> UpdateEmailVerified(bool status);
        Task<int> UpdateEmailVerifiedV2(string email, string company_id , string domain_name);
        Task<int> UpdateDefaultCompany(UpdateDefaultCompanyRequestModel requestModel);
        Task<int> UpdateDefaultClientCompany(UpdateDefaultCompanyRequestModel requestModel);
        Task<int> UpdateActiveCompany(UpdateDefaultCompanyRequestModel requestModel);
        Task<int> UpdateActiveClientCompany(UpdateDefaultCompanyRequestModel requestModel);
        Task<int> UpdateExecutiveEmailNotificationStatus(int status);
        Task<int> UpdateExecutivePMDueReportEmailStatus(ExecutivePMDueEmailConfigRequestModel status);
        List<UserRoles> GetUserRolesByEmail(string email_id);
        MobileAppVersion MobileAppVersion(int device_brand);
        Task<CreateUpdateSiteResponsemodel> CreateClientCompany(CreateClientCompanyRequestModel requestModel);
        Task<CreateUpdateSiteResponsemodel> CreateUpdateSite(CreateUpdateSiteRequestModel requestModel);
        ListViewModel<GetAllClientCompanyWithSitesResponseModel> GetAllClientCompanyWithSites(GetAllClientCompanyWithSitesRequestModel requestModel);
        Task<int> AddUpdateWorkOrderWatcher(AddUpdateWorkOrderWatcherRequestModel requestModel);
        GetActiveUserSitesAndRolesResponseModel GetActiveUserSitesAndRoles(Guid user_id);
        string GetRoleNameById(Guid role_id);
        GetAllTechniciansListResponseModel GetAllProjectManagersList();
        bool CheckUserSessionIsValidOrNot(Guid user_id, Guid user_session_id, Guid role_id);
        GetAllTechniciansLocationByWOIdResponseModel GetAllTechniciansLocationByWOId(Guid wo_id);
        Task<int> AddUserGeoLocationData(AddUserGeoLocationDataRequestModel requestModel);
        GetAllTechniciansListResponseModel GetAllTechniciansForCalendar(GetAllCalanderWorkordersRequestModel requestModel);
        GetAllTechniciansListResponseModel GetAllLeadsForCalendar(GetAllCalanderWorkordersRequestModel requestModel);
        bool IsUserSessionIsRequiredOrNot(Guid company_id);
        string GetSiteNameById(Guid site_id);
        Task<ViewVendorDetailsByIdResponseModel> ViewVendorDetailsById(Guid vendor_id);
        Task<int> CreateUpdateVendor(CreateUpdateVendorRequestModel requestModel);
        Task<int> CreateUpdateContact(CreateUpdateContactRequestModel requestModel);
        GetAllVendorListResponseModel GetAllVendorList(GetAllVendorListRequestModel requestModel);
        GetAllVendorsContactsForDropdownResponseModel GetAllVendorsContactsForDropdown();
        Task<GetRefreshedContactsByWOIdResponseModel> GetRefreshedContactsByWOId(GetRefreshedContactsByWOIdRequestModel requestModel);

        GetSiteUsersDetailsByIdResponseModel GetSiteUsersDetailsById(Guid site_id);

        Task<int> AddUpdateSiteContact(AddUpdateSiteContactRequestModel requestModel);

        Task<int> DeleteSiteContactById(Guid sitecontact_id);
    }
}
