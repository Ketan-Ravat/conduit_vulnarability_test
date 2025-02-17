using AutoMapper;
using Jarvis.db.Models;
using Jarvis.Repo;
using Jarvis.Repo.Abstract;
using Jarvis.Service.Abstract;
using Jarvis.ViewModels;
using Jarvis.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jarvis.Shared.StatusEnums;
using System.Linq;
using Jarvis.ViewModels.ViewModels;
using Jarvis.Service.Notification;
using MimeKit;
using MailKit.Net.Smtp;
using Org.BouncyCastle.Ocsp;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.Utility;
using Jarvis.Shared.Utility;
using Jarvis.Shared.Helper;
using System.Threading;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon;
using System.Security.Policy;
using static Jarvis.Service.Concrete.AssetService;
using Jarvis.Service.Resources;
using DocumentFormat.OpenXml.Spreadsheet;
using Jarvis.Repo.Concrete;
using DocumentFormat.OpenXml.Math;
using SendGrid;
using Amazon.QuickSight.Model.Internal.MarshallTransformations;
using Google.Api.Gax;
using static Google.Apis.Requests.BatchRequest;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Web.Helpers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Calendar.v3.Data;
using static Google.Apis.Calendar.v3.EventsResource.InsertRequest;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Jarvis.Service.Concrete
{
    public class UserService : BaseService, IUserService
    {
        private readonly IMapper _mapper;
        private Shared.Utility.Logger _logger;
        public UserService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            _logger = Shared.Utility.Logger.GetInstance<UserService>();
        }

        public ListViewModel<GetUserResponseModel> GetUsers(int status, int pageindex, int pagesize)
        {
            try
            {
                ListViewModel<GetUserResponseModel> userlist = new ListViewModel<GetUserResponseModel>();
                var users = _UoW.UserRepository.GetUsers(status, pageindex, pagesize);
                if (users.Count > 0)
                {
                    int totalusers = users.Count;
                    if (pagesize > 0 && pageindex > 0)
                    {
                        users = users.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    }

                    userlist.list = _mapper.Map<List<GetUserResponseModel>>(users);
                    Sites sites = new Sites();
                    userlist.list.ForEach(x =>
                    {
                        x.usersites = x.usersites.Where(x => x.status == (int)Status.Active).ToList();
                        x.userroles = x.userroles.Where(x => x.status == (int)Status.Active).ToList();
                        var cognitoUserRoles = x.userroles.Where(x => (x.role_name == GlobalConstants.Admin || x.role_name == GlobalConstants.Executive || x.role_name == GlobalConstants.Manager) && x.status == (int)Status.Active).ToList();
                        if (cognitoUserRoles?.Count > 0)
                        {
                            if (x.is_email_verified == true)
                            {
                                x.is_registration_succeed = true;
                            }
                            else
                            {
                                x.is_registration_succeed = false;
                            }
                        }
                        else
                        {
                            x.is_registration_succeed = true;
                        }
                    });
                    //Sites sites = new Sites();
                    //userlist.list.ForEach(x =>
                    //    x.usersites.ToList().ForEach(y =>
                    //    {
                    //        sites = _UoW.UserRepository.GetSiteBySiteId(y.site_id);
                    //        //y = _mapper.Map<UserSitesViewModel>(sites);
                    //        y.comapny_name = sites.Company.company_name;
                    //        y.company_id = sites.company_id.ToString();
                    //        y.site_name = sites.site_name;
                    //        y.site_code = sites.site_code;
                    //        y.location = sites.location;
                    //    }
                    //));
                    userlist.listsize = totalusers;
                }
                return userlist;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ListViewModel<GetUserResponseModel> FilterUsers(FilterUsersRequestModel requestModel)
        {
            try
            {
                ListViewModel<GetUserResponseModel> userlist = new ListViewModel<GetUserResponseModel>();
                var users = _UoW.UserRepository.FilterUsers(requestModel);
                if (users.Count > 0)
                {
                    int totalusers = users.Count;
                    if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
                    {
                        users = users.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                    }

                    userlist.list = _mapper.Map<List<GetUserResponseModel>>(users);
                    Sites sites = new Sites();
                    userlist.list.ForEach(x =>
                    {
                        x.usersites = x.usersites.Where(x => x.status == (int)Status.Active).ToList();
                        x.userroles = x.userroles.Where(x => x.status == (int)Status.Active).ToList();
                        var cognitoUserRoles = x.userroles.Where(x => (x.role_name == GlobalConstants.Admin || x.role_name == GlobalConstants.Executive || x.role_name == GlobalConstants.Manager || x.role_name == GlobalConstants.CompanyAdmin || x.role_name == GlobalConstants.Technician) && x.status == (int)Status.Active).ToList();
                        if (cognitoUserRoles?.Count > 0)
                        {
                            if (x.is_email_verified == true)
                            {
                                x.is_registration_succeed = true;
                            }
                            else
                            {
                                x.is_registration_succeed = false;
                            }
                        }
                        else
                        {
                            x.is_registration_succeed = true;
                        }
                    });
                    userlist.listsize = totalusers;
                }
                return userlist;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ListViewModel<FilterUsersOptimizedResponsemodel> FilterUsersOptimized(FilterUsersRequestModel requestModel)
        {
            try
            {
                ListViewModel<FilterUsersOptimizedResponsemodel> userlist = new ListViewModel<FilterUsersOptimizedResponsemodel>();
                var users = _UoW.UserRepository.FilterUsers(requestModel);
                if (users.Count > 0)
                {
                    int totalusers = users.Count;
                    if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
                    {
                        users = users.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                    }

                    userlist.list = _mapper.Map<List<FilterUsersOptimizedResponsemodel>>(users);
                    Sites sites = new Sites();
                    userlist.list.ForEach(x =>
                    {
                        x.usersites = x.usersites.Where(x => x.status == (int)Status.Active).ToList();
                        if (UpdatedGenericRequestmodel.CurrentUser.role_id != "ff52a40a-b130-4388-bb1c-4237f8dae72e")
                        {
                            x.usersites = null;
                        }
                        x.userroles = x.userroles.Where(x => x.status == (int)Status.Active).ToList();
                        var cognitoUserRoles = x.userroles.Where(x => (x.role_name == GlobalConstants.Admin || x.role_name == GlobalConstants.Executive || x.role_name == GlobalConstants.Manager || x.role_name == GlobalConstants.CompanyAdmin || x.role_name == GlobalConstants.Technician) && x.status == (int)Status.Active).ToList();
                        if (cognitoUserRoles?.Count > 0)
                        {
                            if (x.is_email_verified == true)
                            {
                                x.is_registration_succeed = true;
                            }
                            else
                            {
                                x.is_registration_succeed = false;
                            }
                        }
                        else
                        {
                            x.is_registration_succeed = true;
                        }
                    });
                    userlist.listsize = totalusers;
                }
                return userlist;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public GetAllTechniciansListResponseModel GetAllTechniciansList(string site_id)
        {
            GetAllTechniciansListResponseModel response = new GetAllTechniciansListResponseModel();
            try
            {
                if (!String.IsNullOrEmpty(site_id))
                {
                    var users = _UoW.UserRepository.GetAllTechniciansListBySiteId(Guid.Parse(site_id));
                    if (users != null)
                    {
                        response = users;
                    }
                }
                else
                {
                    var users = _UoW.UserRepository.GetAllTechniciansList();
                    if (users != null)
                    {
                        response = users;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return response;
        }

        public GetAllTechniciansListResponseModel GetAllBackOfficeUsersList(string site_id)
        {
            GetAllTechniciansListResponseModel response = new GetAllTechniciansListResponseModel();
            try
            {
                if (!String.IsNullOrEmpty(site_id))
                {
                    var users = _UoW.UserRepository.GetAllBackOfficeUsersListBySiteId(Guid.Parse(site_id));
                    if (users != null)
                    {
                        response = users;
                    }
                }
                else
                {
                    var users = _UoW.UserRepository.GetAllBackOfficeUsersList();
                    if (users != null)
                    {
                        response = users;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return response;
        }

        public ListViewModel<GetRolesResponseModel> FilterUsersRoleOptions(FilterUsersRequestModel requestModel)
        {
            try
            {
                ListViewModel<GetRolesResponseModel> userRoleList = new ListViewModel<GetRolesResponseModel>();
                var users = _UoW.UserRepository.FilterUsersRoleOptions(requestModel);
                if (users.Count > 0)
                {
                    var userroles = users.Select(x => x.Userroles).ToList();
                    if (userroles?.Count > 0)
                    {
                        List<Role> allroles = new List<Role>();
                        foreach (var eachroles in userroles)
                        {
                            allroles.AddRange(eachroles.Where(x => x.status == (int)Status.Active).Select(x => x.Role).ToList());
                        }
                        allroles = allroles.Distinct().ToList();
                        userRoleList.list = _mapper.Map<List<GetRolesResponseModel>>(allroles);
                        userRoleList.listsize = allroles.Count;
                    }
                    int totalusers = users.Count;
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    userRoleList.pageIndex = requestModel.pageindex;
                    userRoleList.pageSize = requestModel.pagesize;
                }
                return userRoleList;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ListViewModel<SitesViewModel> FilterUsersSitesOptions(FilterUsersRequestModel requestModel)
        {
            try
            {
                ListViewModel<SitesViewModel> userSiteList = new ListViewModel<SitesViewModel>();
                var users = _UoW.UserRepository.FilterUsersSitesOptions(requestModel);
                if (users.Count > 0)
                {
                    var usersites = users.Select(x => x.Usersites).ToList();
                    if (usersites?.Count > 0)
                    {
                        List<Sites> allsites = new List<Sites>();
                        foreach (var eachsites in usersites)
                        {
                            allsites.AddRange(eachsites.Where(x => x.status == (int)Status.Active).Select(x => x.Sites).ToList());
                        }
                        allsites = allsites.Where(x => x.status == (int)Status.Active).ToList();
                        allsites = allsites.Distinct().ToList();
                        if (allsites.Count > 0)
                        {
                            userSiteList.listsize = allsites.Count;
                            if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                            {
                                requestModel.pagesize = 20;
                                requestModel.pageindex = 1;
                            }
                            userSiteList.pageIndex = requestModel.pageindex;
                            userSiteList.pageSize = requestModel.pagesize;
                            allsites = allsites.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                            userSiteList.list = _mapper.Map<List<SitesViewModel>>(allsites);
                            userSiteList.list.ForEach(x =>
                            {
                                var sitedetails = allsites.Where(y => y.site_id == x.site_id).FirstOrDefault();
                                if (sitedetails != null)
                                {
                                    x.comapny_name = sitedetails.Company?.company_name;
                                }
                            });
                        }
                    }
                }
                return userSiteList;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ListViewModel<CompanyViewModel> FilterUsersCompanyOptions(FilterUsersRequestModel requestModel)
        {
            try
            {
                ListViewModel<CompanyViewModel> userCompList = new ListViewModel<CompanyViewModel>();
                var users = _UoW.UserRepository.FilterUsersCompanyOptions(requestModel);
                if (users.Count > 0)
                {
                    var usersites = users.Select(x => x.Usersites).ToList();
                    if (usersites?.Count > 0)
                    {
                        List<Company> allCompany = new List<Company>();
                        foreach (var eachsites in usersites)
                        {
                            allCompany.AddRange(eachsites.Where(x => x.status == (int)Status.Active).Select(x => x.Sites.Company).ToList());
                        }
                        allCompany = allCompany.Distinct().ToList();
                        if (allCompany.Count > 0)
                        {
                            userCompList.listsize = allCompany.Count;
                            if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                            {
                                requestModel.pagesize = 20;
                                requestModel.pageindex = 1;
                            }
                            userCompList.pageIndex = requestModel.pageindex;
                            userCompList.pageSize = requestModel.pagesize;
                            allCompany = allCompany.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                            userCompList.list = _mapper.Map<List<CompanyViewModel>>(allCompany);
                        }
                    }
                }
                return userCompList;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<User> GetUserByID(string userid)
        {
            return await _UoW.UserRepository.GetUserByID(userid);
        }

        public async Task<LoginResponceModel> UserLogin(LoginRequestModel request)
        {
            LoginResponceModel responseModel = new LoginResponceModel();
            try
            {
                User response = new User();
                if (request.password != null)
                {
                    request.password = Shared.Utility.PasswordHashUtil.CreateHashPassword(request.password);
                }
                response = await _UoW.UserRepository.UserLogin(request);
                if (response.uuid != Guid.Empty)
                {
                    int validuser = (int)ResponseStatusNumber.Success;
                    if (UpdatedGenericRequestmodel.CurrentUser.device_uuid != Guid.Empty)
                    {
                        validuser = _UoW.UserRepository.CheckUserValid(response.uuid.ToString(), UpdatedGenericRequestmodel.CurrentUser.device_uuid);
                    }
                    if (validuser > 0)
                    {
                        // checking for "ALL" site data for client company

                        /* if(response.Company!=null && response.Company.ClientCompany!=null && response.Company.ClientCompany.Count > 0)
                         {
                             response.Company.ClientCompany.ToList().ForEach(x =>
                             {
                                if( x.Sites.Where(x => x.status == (int)Status.Active).ToList()?.Count > 1)
                                 {
                                     var AllSitematser = _UoW.UserRepository.GetAllSiteMaster();
                                     x.Sites.Add(AllSitematser);
                                 }
                             });
                         }*/


                        responseModel = _mapper.Map<LoginResponceModel>(response);
                        //var userlist = responseModel.Usersites;
                        responseModel.Userroles = responseModel.Userroles.Where(x => x.status == (int)Status.Active).ToList();
                        responseModel.Usersites = responseModel.Usersites.Where(x => (x.status == (int)Status.Active || x.status == (int)Status.AllSiteType) && x.main_site_status == (int)Status.Active).ToList();

                        responseModel.Usersites.ForEach(x =>
                        {
                            if (x.status == (int)Status.Active || x.status == (int)Status.AllSiteType)
                            {
                                var sites = _UoW.UserRepository.GetSiteBySiteId(x.site_id);
                                if (sites.site_id != null && sites.site_id != Guid.Empty)
                                {
                                    x.site_name = sites.site_name;
                                    x.site_code = sites.site_code;
                                    x.location = sites.location;
                                    x.client_company_id = sites.client_company_id;
                                    //x.sitecontact_id = sites.sitecontact_id;
                                    //x.sitecontact_name = sites.SiteContact!=null ? sites.SiteContact.sitecontact_name:null;
                                    x.city = sites.city;
                                    x.state = sites.state;
                                    x.zip = sites.zip;
                                    x.customer_address = sites.customer_address;
                                    x.customer_address_2 = sites.customer_address_2;
                                    if (sites.SiteContact != null)
                                    {
                                        Site_Contact_Details_Obj site_Contact_Details_Obj = new Site_Contact_Details_Obj();
                                        site_Contact_Details_Obj.sitecontact_title = sites.SiteContact.sitecontact_title;
                                        site_Contact_Details_Obj.sitecontact_name = sites.SiteContact.sitecontact_name;
                                        site_Contact_Details_Obj.sitecontact_email = sites.SiteContact.sitecontact_email;
                                        site_Contact_Details_Obj.sitecontact_phone = sites.SiteContact.sitecontact_phone;
                                        site_Contact_Details_Obj.sitecontact_id = sites.SiteContact.sitecontact_id;
                                        x.site_contact_details = site_Contact_Details_Obj;
                                    }
                                }
                            }
                        });
                        if (responseModel.client_company != null)
                        {
                            List<Client_Company> client_compmany = new List<Client_Company>();
                            /// give only client company which sites are assigned
                            /// 

                            var accesible_sites = responseModel.Usersites.Select(x => x.site_id).ToList();
                            responseModel.client_company.ForEach(c =>
                            {
                                c.client_company_Usersites = c.client_company_Usersites.Where(q => accesible_sites.Contains(q.site_id)).ToList();
                            });
                            responseModel.client_company = responseModel.client_company.Where(x => x.client_company_Usersites.Count > 0).ToList();
                            responseModel.client_company.ForEach(x =>
                            {
                                if (x.client_company_Usersites != null)
                                {
                                    x.client_company_Usersites.ForEach(q =>
                                    {
                                        if (q.status == (int)Status.AllSiteType)
                                        {
                                            q.company_id = x.client_company_id.ToString();
                                            q.company_name = x.client_company_name;
                                        }
                                    });
                                }
                            });
                        }
                        if (responseModel.default_client_company_Usersites != null)
                        {
                            responseModel.default_client_company_Usersites.ForEach(x =>
                            {
                                if (x.status == (int)Status.AllSiteType)
                                {
                                    x.company_id = responseModel.ac_default_client_company.ToString();
                                    x.company_name = responseModel.ac_default_client_company_name;
                                }
                            });
                        }

                        _logger.LogWarning("UserLogin Service : User Session Code Started... " + UpdatedGenericRequestmodel.CurrentUser.role_id);

                        //if (UpdatedGenericRequestmodel.CurrentUser.role_id != GlobalConstants.Technician_Role_id)
                        //{
                        //}

                        //Create new UserSession and DeActive previous UserSessions
                        _logger.LogWarning("UserLogin Service : Inside Session IF condition");
                        _logger.LogWarning("UserLogin Service : request_by= " + UpdatedGenericRequestmodel.CurrentUser.requested_by);
                        _logger.LogWarning("UserLogin Service : AC_default_role_web= " + responseModel.ac_default_role_web);

                        var get_active_session_list = _UoW.UserRepository.GetActiveUserSessionByUUId(response.uuid);
                        foreach (var get_active_session in get_active_session_list)
                        {
                            get_active_session.status = (int)Status.Deactive;
                            get_active_session.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            get_active_session.modified_at = DateTime.UtcNow;
                            var update_usersession = await _UoW.BaseGenericRepository<UserSession>().Update(get_active_session);
                            //_UoW.SaveChanges();
                        }

                        UserSession userSession = new UserSession();
                        userSession.user_id = response.uuid;
                        userSession.status = (int)Status.Active;
                        userSession.device_id = Guid.Empty;//UpdatedGenericRequestmodel.CurrentUser.device_uuid;
                        userSession.role_id = Guid.Parse(responseModel.ac_default_role_web);//Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.role_id);

                        if (UpdatedGenericRequestmodel.CurrentUser.platform_type.ToLower() == AuthenticationConstants.MobilePlatform)
                        {
                            userSession.role_id = Guid.Parse(responseModel.ac_default_role_app);//Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.role_id);
                        }
                        userSession.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        userSession.created_at = DateTime.UtcNow;
                        var insert_usersession = await _UoW.BaseGenericRepository<UserSession>().Insert(userSession);
                        responseModel.user_session_id = userSession.user_session_id;
                        _UoW.SaveChanges();

                        responseModel.is_estimator_feature_required = _UoW.UserRepository.IsFeatureRequiredOrNotForCompany(Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id),
                                 Guid.Parse(GlobalConstants.estimator_feature));

                        responseModel.is_allowed_to_update_formio = _UoW.UserRepository.IsFeatureRequiredOrNotForCompany(Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id),
                                 Guid.Parse(GlobalConstants.allowed_to_update_formio_feature_id));

                        responseModel.is_egalvanic_ai_required = _UoW.UserRepository.IsFeatureRequiredOrNotForCompany(Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id),
                            Guid.Parse(GlobalConstants.egalvanic_ai));

                        responseModel.is_mfa_enabled = _UoW.UserRepository.IsFeatureRequiredOrNotForCompany(Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id),
                            Guid.Parse(GlobalConstants.is_mfa_enabled_feature_id));

                        responseModel.is_retool_bulk_operation_required = _UoW.UserRepository.IsFeatureRequiredOrNotForCompany(Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id),
                            Guid.Parse(GlobalConstants.is_retool_bulk_operation_required_id));

                        responseModel.is_required_maintenance_command_center = _UoW.UserRepository.IsFeatureRequiredOrNotForCompany(Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id),
                           Guid.Parse(GlobalConstants.is_required_maintenance_command_center));

                        responseModel.is_reactflow_required = _UoW.UserRepository.IsFeatureRequiredOrNotForCompany(Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id),
                            Guid.Parse(GlobalConstants.is_reactflow_required));

                        if (!response.is_email_verified)
                        {
                            response.is_email_verified = true;
                            response.modified_at = DateTime.UtcNow;
                            var update_user = await _UoW.BaseGenericRepository<User>().Update(response);
                            _UoW.SaveChanges();
                        }
                    }
                    else
                    {
                        responseModel.response_status = validuser;
                    }
                    //responseModel.Usersites.ForEach(x => x.status = (int)Status.Active);
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning("UserLogin Service : " + e.Message.ToString());
                throw;
            }
            return responseModel;
        }

        public async Task<LoginResponceModel> InsertUpdateUserDetails(UserRequestModel user, string awsAccessKey, string awsSecretKey)
        {
            _logger.LogDebug("aws access key ::  ", awsAccessKey);
            _logger.LogDebug("aws awsSecret Key  ::  ", awsSecretKey);
            int result;
            LoginResponceModel response = new LoginResponceModel();
            NotificationService notificationService = new NotificationService(_mapper);
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    if (!String.IsNullOrEmpty(user.email))
                        user.email = user.email.ToLower().Trim();

                    if (!String.IsNullOrEmpty(user.username))
                        user.username = user.username.ToLower().Trim();

                    var usermodel = _mapper.Map<User>(user);
                    if (usermodel.uuid == Guid.Empty)
                    {
                        usermodel.created_at = DateTime.UtcNow;
                        usermodel.modified_at = DateTime.UtcNow;
                        usermodel.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        if (!String.IsNullOrEmpty(usermodel.password))
                        {
                            usermodel.password = Shared.Utility.PasswordHashUtil.CreateHashPassword(user.password);
                        }
                        usermodel.barcode_id = Guid.NewGuid();
                        usermodel.Userroles.ToList().ForEach(X =>
                        {
                            X.created_at = DateTime.UtcNow;
                            X.modified_at = DateTime.UtcNow;
                            X.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            X.status = (int)Status.Active;
                        });

                        var AllRoles = await _UoW.UserRepository.GetAllRoles();
                        if (AllRoles?.Count > 0)
                        {
                            var operatorRoleId = AllRoles.Where(x => x.name == GlobalConstants.Operator).Select(x => x.role_id).FirstOrDefault();
                            var msRoleId = AllRoles.Where(x => x.name == GlobalConstants.MS).Select(x => x.role_id).FirstOrDefault();
                            var managerRoleId = AllRoles.Where(x => x.name == GlobalConstants.Manager).Select(x => x.role_id).FirstOrDefault();
                            var executiveRoleId = AllRoles.Where(x => x.name == GlobalConstants.Executive).Select(x => x.role_id).FirstOrDefault();
                            var operatorRole = usermodel.Userroles.Where(x => x.role_id == operatorRoleId).ToList();
                            if (operatorRole?.Count > 0)
                            {
                                usermodel.ac_default_role_app = operatorRoleId;
                                usermodel.ac_default_role_web = null;
                            }
                            else
                            {
                                var msRole = usermodel.Userroles.Where(x => x.role_id == msRoleId).ToList();
                                if (msRole?.Count > 0)
                                {
                                    usermodel.ac_default_role_app = msRoleId;
                                    usermodel.ac_default_role_web = null;
                                }
                                else
                                {
                                    // check for web
                                    var webAppWithMultipleRole = usermodel.Userroles.Where(x => x.role_id == managerRoleId).ToList();
                                    if (webAppWithMultipleRole?.Count > 0)
                                    {
                                        usermodel.ac_default_role_app = managerRoleId;
                                    }
                                }
                            }
                        }

                        usermodel.ac_active_role_app = usermodel.ac_default_role_app;
                        usermodel.ac_active_role_web = usermodel.ac_default_role_web;
                        usermodel.ac_active_site = usermodel.ac_default_site;

                        usermodel.profile_picture_name = user.profile_picture_name;
                        usermodel.signature_url = user.signature_url;
                        usermodel.job_title = user.job_title;

                        /// assign client company to user as per defaut site
                        if (usermodel.ac_default_site != null && usermodel.ac_default_site.Value != Guid.Empty)
                        {
                            var site = _UoW.UserRepository.GetSiteBySiteId(usermodel.ac_default_site.Value);
                            if (site != null && site.status == (int)Status.AllSiteType)
                            {
                                var client_company_from_parent_company = _UoW.UserRepository.GetClientCompanyByParentCompany(user.ac_active_company.Value);
                                if (client_company_from_parent_company != null && client_company_from_parent_company.Count > 0)
                                {
                                    usermodel.ac_active_client_company = client_company_from_parent_company.FirstOrDefault().client_company_id;
                                    usermodel.ac_default_client_company = client_company_from_parent_company.FirstOrDefault().client_company_id;
                                }
                            }
                            else
                            {
                                var client_company = _UoW.UserRepository.GetClientCompanyBySiteid(usermodel.ac_default_site.Value);
                                if (client_company != null)
                                {
                                    usermodel.ac_active_client_company = client_company.client_company_id;
                                    usermodel.ac_default_client_company = client_company.client_company_id;
                                }
                            }
                        }
                        var siteId = usermodel.Usersites.Where(x => x.status == (int)Status.Active).Select(x => x.site_id).FirstOrDefault();

                        result = await _UoW.UserRepository.Insert(usermodel);
                        if (result > 0)
                        {
                            response = _mapper.Map<LoginResponceModel>(usermodel);
                            var siteDetails = await _UoW.CompanyRepository<Company>().GetSiteDetails(siteId.ToString());
                            if (siteDetails != null)
                            {
                                var AllRoleIds = await _UoW.UserRepository.GetAllRoles();
                                var CognitoUserRoleIds = AllRoleIds.Where(x => x.name != GlobalConstants.Operator && x.name != GlobalConstants.MS).Select(x => x.role_id).ToList();
                                var isCognitoUser = usermodel.Userroles.Where(x => CognitoUserRoleIds.Contains(x.role_id)).ToList();
                                if (isCognitoUser?.Count > 0)
                                {
                                    string tempp_password = GenerateRandomString.GenerateRandomPassword();
                                    var resultCog = await CreateCognitoUser(usermodel, siteDetails.Company.user_pool_web_client_id, siteDetails.Company.user_pool_id, awsAccessKey, awsSecretKey, tempp_password);
                                    if (resultCog.success > 0)
                                    {
                                        // send mail from sendgrid
                                        /*var templateID = ConfigurationManager.AppSettings["User_Signup_mail"];
                                        usersignupemail usersignupemail = new usersignupemail();
                                        usersignupemail.username = usermodel.email;
                                        usersignupemail.temp_code = tempp_password;
                                        usersignupemail.env_domain = siteDetails.Company.company_code + ConfigurationManager.AppSettings["WebAppDomainForMail"];

                                        var email_response = await SendEmail.SendGridEmailWithTemplate(usermodel.email, "Welcome to Conduit", usersignupemail, templateID);
                                       */
                                        _UoW.SaveChanges();
                                    }
                                    else
                                    {
                                        result = (int)ResponseStatusNumber.Error;
                                        response.response_message = resultCog.message;
                                    }
                                }
                                else
                                {
                                    _UoW.SaveChanges();
                                }
                            }

                            // Send Multiple Notifications to User for every Assigned Site 
                            var get_user_id = new List<string> { usermodel.uuid.ToString() };
                            var new_assigned_ref_sites_id = user.Usersites.Where(x => x.status == (int)Status.Active).Select(x => x.site_id.ToString()).ToList();
                            await notificationService.SendNotificationGenericNewFlow((int)NotificationType_Version_2.SiteAssignedToUser, new_assigned_ref_sites_id, get_user_id);
                        }
                    }
                    else
                    {
                       
                        var userdetail = await _UoW.UserRepository.FindUserDetails(usermodel.uuid);
                        {
                            var userId = user.uuid;
                            var isRequired = user.is_required;

                            //Retrieve the relevant CompanyFeatureMapping record from the database
                            //var companyFeatureMapping = _UoW.UserRepository.GetUserCompanyFeatureRecord(userId);
                            //if (companyFeatureMapping != null)
                            //{
                            //    companyFeatureMapping.is_required = isRequired;
                            //    var companyFeatureUpdate = await _UoW.BaseGenericRepository<CompanyFeatureMapping>().Update(companyFeatureMapping);
                            //    if (companyFeatureUpdate == true)
                            //    {
                            //        _UoW.SaveChanges();
                            //    }
                            //}

                            userdetail.modified_at = DateTime.UtcNow;
                            userdetail.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            userdetail.status = user.status;
                            
                            //userdetail.email = user.email;
                            userdetail.firstname = user.firstname;
                            userdetail.mobile_number = user.mobile_number;
                            userdetail.lastname = user.lastname;
                            //userdetail.phone_number = user.mobile_number;
                            userdetail.prefer_language_id = user.prefer_language_id;
                            //userdetail.ac_default_role_app = user.ac_default_role_app;
                            userdetail.ac_default_role_web = user.ac_default_role_web;
                            userdetail.ac_default_site = user.ac_default_site;
                            userdetail.ac_default_company = user.ac_default_company != null ? user.ac_default_company : userdetail.ac_default_company;
                            userdetail.ac_active_company = user.ac_active_company != null ? user.ac_active_company : userdetail.ac_active_company;

                            //update default client company based on default site
                            var Sitedetails = _UoW.SiteRepository.GetSiteById(user.ac_default_site.Value.ToString());
                            userdetail.ac_default_client_company = Sitedetails.client_company_id;

                            //userdetail.ac_active_role_app = user.ac_active_role_app;
                            //userdetail.ac_active_role_web = user.ac_active_role_web;
                            //userdetail.ac_active_site = user.ac_active_site;

                            userdetail.profile_picture_name = user.profile_picture_name;

                            userdetail.signature_url = user.signature_url;
                            userdetail.job_title = user.job_title;

                            //update phone number in cognito if its updated
                            var resultCog = await UpdateCognitoUserPhoneNumber(usermodel, Sitedetails.Company.user_pool_web_client_id, Sitedetails.Company.user_pool_id, awsAccessKey, awsSecretKey);

                            List<string> new_added_user_sites = new List<string>();
                            foreach (var sites in user.Usersites)
                            {
                                UserSites userSite = new UserSites();
                                if (sites.usersite_id == null || sites.usersite_id == Guid.Empty)
                                {
                                    var alreadyexist = userdetail.Usersites.Where(x => x.site_id == sites.site_id && x.user_id == userdetail.uuid).FirstOrDefault();
                                    if (alreadyexist != null && alreadyexist.usersite_id != null && alreadyexist.usersite_id != Guid.Empty)
                                    {

                                        if (alreadyexist.status == (int)Status.Deactive)
                                        {
                                            alreadyexist.modified_at = DateTime.UtcNow;
                                            alreadyexist.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                            alreadyexist.status = (int)Status.Active;
                                            new_added_user_sites.Add(sites.site_id.ToString());
                                        }
                                    }
                                    else
                                    {
                                        new_added_user_sites.Add(sites.site_id.ToString());

                                        userSite.site_id = sites.site_id;
                                        userSite.created_at = DateTime.UtcNow;
                                        userSite.status = (int)Status.Active;
                                        userSite.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                        userSite.modified_at = DateTime.UtcNow;
                                        userdetail.Usersites.Add(userSite);
                                    }
                                }
                            }

                            foreach (var role in user.Userroles)
                            {
                                UserRoles userRole = new UserRoles();
                                if (role.userrole_id == null || role.userrole_id == Guid.Empty)
                                {
                                    var alreadyexist = userdetail.Userroles.Where(x => x.role_id == role.role_id && x.user_id == userdetail.uuid).FirstOrDefault();
                                    if (alreadyexist != null && alreadyexist.userrole_id != null && alreadyexist.userrole_id != Guid.Empty)
                                    {
                                        if (alreadyexist.status == (int)Status.Deactive)
                                        {
                                            alreadyexist.modified_at = DateTime.UtcNow;
                                            alreadyexist.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                            alreadyexist.status = (int)Status.Active;
                                        }
                                    }
                                    else
                                    {
                                        userRole.role_id = role.role_id;
                                        userRole.user_id = userdetail.uuid;
                                        userRole.created_at = DateTime.UtcNow;
                                        userRole.status = (int)Status.Active;
                                        userRole.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                        userRole.modified_at = DateTime.UtcNow;
                                        userdetail.Userroles.Add(userRole);
                                    }
                                }
                            }

                            var usersites = userdetail.Usersites.Where(x => x.user_id == usermodel.uuid).ToList();

                            var userroles = userdetail.Userroles.Where(x => x.user_id == usermodel.uuid).ToList();

                            var removesites = usersites.Where(p => !usermodel.Usersites.Any(p2 => p2.site_id == p.site_id) && p.status == (int)Status.Active).ToList();

                            // check if user have any active WO in remove site then return error that site can not be removed
                            // 
                            var remove_site_id = removesites.Where(x => x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                            var active_wo = _UoW.UserRepository.GetUserActiveWOForInactiveSite(remove_site_id, userdetail.uuid);
                            if (active_wo != null && active_wo.Count > 0)
                            {
                                string message = "";
                                var site_groupby = active_wo.GroupBy(x => x.site_id).ToList();
                                foreach (var site in site_groupby)
                                {
                                    foreach (var item in site)
                                    {
                                        if (message == "")
                                            message = item.WorkOrders.Sites.site_name + " - WO# " + item.WorkOrders.manual_wo_number;
                                        else
                                            message = message + "," + item.WorkOrders.Sites.site_name + " - WO# " + item.WorkOrders.manual_wo_number;
                                    }
                                }
                                string response_message = "Access removal is currently restricted as {0} have active work orders assigned across multiple sites. The following work order numbers are assigned to {0} for the respective sites: {1}. Please contact your administrator for further assistance";
                                response.response_message = String.Format(response_message, userdetail.firstname, message);
                                return response;
                            }

                            var removeroles = userroles.Where(p => !usermodel.Userroles.Any(p2 => p2.role_id == p.role_id) && p.status == (int)Status.Active).ToList();

                            if (removesites.Count > 0)
                            {
                                removesites.ForEach(x => x.status = (int)Status.Deactive);
                            }

                            if (removeroles.Count > 0)
                            {
                                removeroles.ForEach(x => x.status = (int)Status.Deactive);
                            }

                            if (removeroles.Count > 0)
                            {
                                var AllRoles = await _UoW.UserRepository.GetAllRoles();
                                if (AllRoles?.Count > 0)
                                {
                                    var operatorRoleId = AllRoles.Where(x => x.name == GlobalConstants.Operator).Select(x => x.role_id).FirstOrDefault();
                                    var msRoleId = AllRoles.Where(x => x.name == GlobalConstants.MS).Select(x => x.role_id).FirstOrDefault();
                                    var managerRoleId = AllRoles.Where(x => x.name == GlobalConstants.Manager).Select(x => x.role_id).FirstOrDefault();
                                    var executiveRoleId = AllRoles.Where(x => x.name == GlobalConstants.Executive).Select(x => x.role_id).FirstOrDefault();
                                    var operatorRole = userdetail.Userroles.Where(x => x.role_id == operatorRoleId && x.status == (int)Status.Active).ToList();
                                    if (operatorRole?.Count > 0)
                                    {
                                        userdetail.ac_default_role_app = operatorRoleId;
                                        userdetail.ac_active_role_app = operatorRoleId;
                                        userdetail.ac_default_role_web = null;
                                    }
                                    else
                                    {
                                        var msRole = userdetail.Userroles.Where(x => x.role_id == msRoleId && x.status == (int)Status.Active).ToList();
                                        if (msRole?.Count > 0)
                                        {
                                            userdetail.ac_default_role_app = msRoleId;
                                            userdetail.ac_active_role_app = msRoleId;
                                            userdetail.ac_default_role_web = null;
                                            userdetail.ac_active_role_web = null;
                                        }
                                        else
                                        {
                                            // check for web
                                            var webAppWithMultipleRole = userdetail.Userroles.Where(x => x.role_id == managerRoleId && x.status == (int)Status.Active).ToList();
                                            if (webAppWithMultipleRole?.Count > 0)
                                            {
                                                userdetail.ac_default_role_app = managerRoleId;
                                                userdetail.ac_active_role_app = managerRoleId;
                                            }
                                        }
                                    }
                                }
                            }

                            result = _UoW.UserRepository.Update(userdetail).Result;

                            if (result > 0)
                            {

                                // Send Multiple Notifications to User for every Assigned Site 
                                var get_user_id = new List<string> { usermodel.uuid.ToString() };
                                //var new_assigned_ref_sites_id = new_added_user_sites;
                                await notificationService.SendNotificationGenericNewFlow((int)NotificationType_Version_2.SiteAssignedToUser, new_added_user_sites, get_user_id);

                                response = _mapper.Map<LoginResponceModel>(userdetail);
                            }
                        }
                    }
                    response.response_status = result;
                    if (result > 0)
                    {
                        _dbtransaction.Commit();
                        //var userdetails = await _UoW.UserRepository.FindUserDetails(uid);
                        //response = _mapper.Map<LoginResponceModel>(userdetails);
                    }
                    else
                    {
                        _dbtransaction.Rollback();
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    throw;
                }
                return response;
            }
        }
        public class usersignupemail
        {
            public string username { get; set; }

            public string temp_code { get; set; }
            public string env_domain { get; set; }
        }
        public async Task<bool> AddRole(RoleRequestModel role)
        {
            bool result = false;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var AddRoleRequest = _mapper.Map<Role>(role);
                    result = await _UoW.BaseGenericRepository<Role>().Insert(AddRoleRequest);
                    if (result)
                    {
                        _UoW.SaveChanges();
                        _dbtransaction.Commit();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogWarning("Exception to add attributes ", e.Message);
                    _dbtransaction.Rollback();
                    throw;
                }
            }
            return result;
        }

        public async Task<int> SendNotification(SendNotificationRequest request)
        {
            NotificationService notificationService = new NotificationService(_mapper);


            //var operatordetails = _UoW.UserRepository.GetUserSiteById(request.token.ToString());
            //var userlists = _mapper.Map<List<UserSitesNotificationModel>(operatordetails);
            //var token = userlists.User.notification_token;

            //var notificationmessage = NotificationGenerator.AutoApproedInspectionOperator("ABC");


            //RootRequestObj operatorReq = new RootRequestObj()
            //{
            //    //registration_ids = token,
            //    priority = "high",
            //    data = new RequestData()
            //    {
            //        title = notificationmessage.heading,
            //        body = notificationmessage.message,
            //        type = 1,
            //        ref_id = "1",
            //        custom = 1,
            //    },
            //    notification = new Notifications()
            //    {
            //        title = notificationmessage.heading,
            //        body = notificationmessage.message
            //    }
            //};
            var response = await notificationService.SendNotification(1, "49f96548-9403-49b1-b0e9-e66aae770a0b", request.token);
            if (response)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }


        public NotificationListViewModel<NotificationViewModel> GetNotification(string userid, string role_id, int pagesize, int pageindex)
        {
            NotificationListViewModel<NotificationViewModel> responseModel = new NotificationListViewModel<NotificationViewModel>();
            try
            {
                string notification_user_role = null;
                if (role_id == GlobalConstants.BackOffice_Role_Id) { notification_user_role = ((int)NotificationUserRoleType.BackOffice_User).ToString(); }
                else if (role_id == GlobalConstants.Technician_Role_id) { notification_user_role = ((int)NotificationUserRoleType.Technician_User).ToString(); }

                var get_notifications = _UoW.UserRepository.GetNotificationsDataByUserId(userid, notification_user_role, pagesize, pageindex);
                int totalnotification = get_notifications.Item2;

                responseModel.list = _mapper.Map<List<NotificationViewModel>>(get_notifications.Item1);

                responseModel.list.ForEach(item =>
                {
                    if (!String.IsNullOrEmpty(item.ref_id))
                    {
                        var get_wo_type = _UoW.UserRepository.GetWOTypeById(Guid.Parse(item.ref_id));

                        if (get_wo_type != null && get_wo_type > 0)
                            item.wo_type = get_wo_type;
                    }
                });
                responseModel.notification_count = GetNotificationsCount((int)Notification_Status.New);
                responseModel.listsize = totalnotification;
            }
            catch (Exception e)
            {
                _logger.LogError("Exception to get Notifications: ", e.Message);
                throw;
            }

            return responseModel;
        }

        public class NotificationListViewModel<T> : BaseViewModel
        {
            public List<T> list { get; set; }
            public int pageSize { get; set; }
            public int pageIndex { get; set; }
            public int listsize { get; set; }
            public int notification_count { get; set; }
            public NotificationListViewModel()
            {
                list = new List<T>();
            }
        }

        /*
        public ListViewModel<NotificationViewModel> GetNotification(string userid, string role_id , int pagesize, int pageindex)
        {
            ListViewModel<NotificationViewModel> responseModel = new ListViewModel<NotificationViewModel>();
            try
            {
                var response = _UoW.BaseGenericRepository<NotificationData>().GetAll().ToList();
                int totalnotification = response.Count();
                if (pageindex > 0)
                {
                    string notification_user_role = null;
                    if (role_id == GlobalConstants.BackOffice_Role_Id)  { notification_user_role = NotificationUserRoleType.BackOffice_User.ToString(); }
                    else if(role_id == GlobalConstants.Technician_Role_id) { notification_user_role = NotificationUserRoleType.Technician_User.ToString(); }
                    
                    response = response.Where(x => x.user_id.ToString() == userid 
                    && notification_user_role.Contains(x.notification_user_role)).OrderByDescending(g => g.createdDate).ToList();
                    
                    totalnotification = response.Count();
                    response = response.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                }
                else
                {
                    response = response.Where(x => x.user_id.ToString() == userid).OrderByDescending(g => g.createdDate).ToList();
                    totalnotification = response.Count();
                }
                responseModel.list = _mapper.Map<List<NotificationViewModel>>(response);
                responseModel.listsize = totalnotification;
            }
            catch (Exception e)
            {
                _logger.LogError("Exception to get Notifications: ", e.Message);
                throw;
            }

            return responseModel;
        } 

         */

        public int GetNotificationsCount(int? notification_status)
        {
            int totalnotification = 0;
            try
            {
                string notification_user_role = null;
                if (UpdatedGenericRequestmodel.CurrentUser.role_id == GlobalConstants.BackOffice_Role_Id) { notification_user_role = ((int)NotificationUserRoleType.BackOffice_User).ToString(); }
                else if (UpdatedGenericRequestmodel.CurrentUser.role_id == GlobalConstants.Technician_Role_id) { notification_user_role = ((int)NotificationUserRoleType.Technician_User).ToString(); }

                var get_count = _UoW.UserRepository.GetNotificationCountByUserId(UpdatedGenericRequestmodel.CurrentUser.requested_by, notification_user_role, notification_status);
                totalnotification = get_count;
            }
            catch (Exception e)
            {
                _logger.LogError("Exception to get Notifications: ", e.Message);
                throw e;
            }

            return totalnotification;
        }
        /*var response = _UoW.BaseGenericRepository<NotificationData>().GetAll().ToList();
                if(status == (int)Notification_Status.New)
                {
                    totalnotification = response.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && (x.notification_status == status || x.notification_status == 0)).Count();
                }
                else
                {
                    totalnotification = response.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.notification_status == status).Count();
                }*/

        public async Task<int> UpdateNotificationStatus(UpdateNotificationStatusRequestModel requestModel)
        {
            int responseModel = (int)ResponseStatusNumber.Error;
            try
            {
                var response = await _UoW.UserRepository.GetNotificationByID(requestModel.notification_id);
                if (response != null && (requestModel.status == (int)Notification_Status.New || requestModel.status == (int)Notification_Status.Read))
                {
                    response.notification_status = requestModel.status;
                    var updateRes = await _UoW.BaseGenericRepository<NotificationData>().Update(response);
                    if (updateRes)
                    {
                        responseModel = (int)ResponseStatusNumber.Success;
                    }
                }
                else
                {
                    responseModel = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Exception to Update Notification status: ", e.Message);
                throw;
            }

            return responseModel;
        }

        public async Task<int> MarkAllNotificationStatus(int notification_status, Guid user_id)
        {
            int responseModel = (int)ResponseStatusNumber.Error;
            try
            {
                string notification_user_role = null;
                if (UpdatedGenericRequestmodel.CurrentUser.role_id == GlobalConstants.BackOffice_Role_Id) { notification_user_role = ((int)NotificationUserRoleType.BackOffice_User).ToString(); }
                else if (UpdatedGenericRequestmodel.CurrentUser.role_id == GlobalConstants.Technician_Role_id) { notification_user_role = ((int)NotificationUserRoleType.Technician_User).ToString(); }

                var get_notifications = _UoW.UserRepository.GetNotificationsDataByUserId(user_id.ToString(), notification_user_role, 0, 0);

                if (get_notifications.Item1 != null && get_notifications.Item1.Count > 0)
                {
                    get_notifications.Item1.ForEach(x =>
                    {
                        x.notification_status = notification_status;
                    });
                    var updateRes = _UoW.BaseGenericRepository<NotificationData>().UpdateList(get_notifications.Item1);
                    if (updateRes)
                    {
                        responseModel = (int)ResponseStatusNumber.Success;
                    }
                }
                else
                {
                    responseModel = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Exception to Update Notification status: ", e.Message);
                throw;
            }

            return responseModel;
        }

        public bool Logout()
        {
            try
            {
                _UoW.BeginTransaction();
                var response = _UoW.UserRepository.Logout();
                if (response)
                {
                    _UoW.CommitTransaction();
                    return true;
                }
                else
                {
                    _UoW.RollbackTransaction();
                    return false;
                }
            }
            catch (Exception e)
            {
                _UoW.RollbackTransaction();
                throw;
            }
        }

        public GetUserResponseModel GetUserByIDFromParent(string uuid)
        {
            try
            {
                GetUserResponseModel user = new GetUserResponseModel();
                var response = _UoW.UserRepository.GetUserByIDFromParent(uuid);
                if (response != null && response.uuid != null && response.uuid != Guid.Empty)
                {
                    user = _mapper.Map<GetUserResponseModel>(response);
                    user.userroles = user.userroles.Where(x => x.status == (int)Status.Active).ToList();
                    user.usersites = user.usersites.Where(x => x.status == (int)Status.Active).ToList();
                    Sites sites = new Sites();
                    user.usersites.ToList().ForEach(x =>
                    {
                        sites = _UoW.UserRepository.GetSiteBySiteId(x.site_id);
                        x.comapny_name = sites.Company.company_name;
                        x.company_id = sites.company_id.ToString();
                        x.site_name = sites.site_name;
                        x.site_code = sites.site_code;
                        x.location = sites.location;
                    });
                }
                return user;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<BaseViewModel> ResetPassword(string userid, string uuid)
        {
            try
            {
                BaseViewModel response = new BaseViewModel();
                var user = _UoW.UserRepository.GetUserByIDFromParent(uuid);

                if (user != null && user.uuid != null && user.uuid != Guid.Empty)
                {
                    string password = CreateRandomPassword(8);
                    user.modified_at = DateTime.UtcNow;
                    user.modified_by = userid;
                    user.password = Shared.Utility.PasswordHashUtil.CreateHashPassword(password);
                    var updateresponse = await _UoW.UserRepository.Update(user);
                    if (updateresponse > 0)
                    {
                        MimeMessage message = new MimeMessage();
                        MailboxAddress from = new MailboxAddress("Admin", "kaushaljayswal.sculptsoft@gmail.com");
                        message.From.Add(from);

                        MailboxAddress to = new MailboxAddress("User", "jayswalkaushal2@gmail.com");
                        message.To.Add(to);

                        BodyBuilder bodyBuilder = new BodyBuilder();
                        bodyBuilder.HtmlBody = "<h1>Hello World!</h1>";
                        bodyBuilder.TextBody = "Hello World!";


                        message.Subject = "This is email subject";
                        message.Body = bodyBuilder.ToMessageBody();


                        SmtpClient client = new SmtpClient();
                        client.Connect("smtp.gmail.com", 587, false);
                        client.Authenticate("kaushaljayswal.sculptsoft@gmail.com", "Kaushal@123");
                        client.Send(message);
                        client.Disconnect(true);
                        client.Dispose();

                        _UoW.CommitTransaction();
                        response.result = updateresponse;
                        response.message = password;
                    }
                    else
                    {
                        response.result = updateresponse;
                        _UoW.RollbackTransaction();
                    }
                }
                else
                {
                    response.result = (int)ResponseStatusNumber.NotFound;
                }
                return response;

            }
            catch (Exception e)
            {
                throw;
            }
        }

        private static string CreateRandomPassword(int length)
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            Random random = new Random();

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }

        public Guid GetBarcodeIdById(string userid)
        {
            try
            {
                return _UoW.UserRepository.GetBarcodeIdById(userid);
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public async Task<int> ResetUserBarcode(string userid, string uuid)
        {
            try
            {
                var user = _UoW.UserRepository.GetUserByIDFromParent(uuid);
                if (user != null && user.uuid != null && user.uuid != Guid.Empty)
                {
                    _UoW.BeginTransaction();
                    user.barcode_id = Guid.NewGuid();
                    user.modified_by = userid;
                    user.modified_at = DateTime.UtcNow;
                    var response = await _UoW.UserRepository.Update(user);
                    if (response > 0)
                    {
                        _UoW.CommitTransaction();
                        return response;
                    }
                    else
                    {
                        _UoW.RollbackTransaction();
                        return response;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _UoW.RollbackTransaction();
                throw;
            }
        }

        public async Task<ListViewModel<GetRolesResponseModel>> GetRoles()
        {
            ListViewModel<GetRolesResponseModel> response = new ListViewModel<GetRolesResponseModel>();
            try
            {
                var roles = await _UoW.UserRepository.GetRoles();
                if (roles.Count > 0)
                {
                    response.list = _mapper.Map<List<GetRolesResponseModel>>(roles);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return response;
        }

        public async Task<int> UpdateUserStatus(UpdateUserStatusRequestModel requestModel)
        {
            int response = (int)ResponseStatusNumber.Error;
            try
            {
                var user = await _UoW.UserRepository.GetUserByID(requestModel.userid);
                if (user != null && user.uuid != null && user.uuid != Guid.Empty)
                {
                    _UoW.BeginTransaction();
                    user.status = requestModel.status;
                    user.modified_at = DateTime.UtcNow;
                    user.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    var result = await _UoW.UserRepository.Update(user);
                    if (result > 0)
                    {
                        _UoW.CommitTransaction();
                    }
                    else
                    {
                        _UoW.RollbackTransaction();
                    }
                    response = result;
                }
                else
                {
                    response = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _UoW.RollbackTransaction();
                throw;
            }
            return response;
        }

        public ListViewModel<GetUserDetailsResponseModel> GetUserDetails(string timestamp, int pageindex, int pagesize)
        {
            try
            {
                ListViewModel<GetUserDetailsResponseModel> response = new ListViewModel<GetUserDetailsResponseModel>();

                var responsemodel = _UoW.UserRepository.GetUserDetails(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString(), timestamp, pageindex, pagesize);
                if (responsemodel.Count > 0)
                {

                    responsemodel.ForEach(x => x.Usersites = x.Usersites.Where(y => y.status == (int)Status.Active).ToList());
                    response.list = _mapper.Map<List<GetUserDetailsResponseModel>>(responsemodel);
                    response.list.ForEach(x =>
                    {
                        if (x.Usersites != null)
                        {
                            x.Usersites.ToList().ForEach(y =>
                            {
                                if (y.site_id != null)
                                {
                                    var sites = _UoW.UserRepository.GetSiteBySiteId(y.site_id);
                                    if (sites != null)
                                    {
                                        y.site_name = sites.site_name;
                                        y.location = sites.location;
                                        y.site_code = sites.site_code;
                                        y.company_id = sites.company_id.ToString();
                                        y.comapny_name = sites.Company.company_name;
                                    }
                                }
                            });
                        }
                    });
                    response.success = true;
                    response.pageIndex = pageindex;
                    response.pageSize = pagesize;
                }
                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<int> UpdateUserToken(UpdateUserTokenRequestModel request)
        {
            try
            {
                int response = (int)ResponseStatusNumber.Error;

                var sametokenusers = await _UoW.UserRepository.GetUsersByToken(request.notification_token);
                if (sametokenusers.Count > 0)
                {
                    sametokenusers.ForEach(x => x.notification_token = string.Empty);
                    foreach (var sametokenuser in sametokenusers)
                    {
                        _UoW.BeginTransaction();
                        int result = await _UoW.UserRepository.Update(sametokenuser);
                        if (result > 0)
                        {
                            _UoW.CommitTransaction();
                        }
                        else
                        {
                            _UoW.RollbackTransaction();
                        }
                    }
                }

                var user = await _UoW.UserRepository.GetUserByID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (user != null && user.uuid != null && user.uuid != Guid.Empty)
                {
                    _UoW.BeginTransaction();
                    user.notification_token = request.notification_token;
                    user.modified_at = DateTime.UtcNow;
                    user.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    int result = await _UoW.UserRepository.Update(user);
                    if (result > 0)
                    {
                        response = result;
                        _UoW.CommitTransaction();
                    }
                    else
                    {
                        _UoW.RollbackTransaction();
                    }
                }
                return response;
            }
            catch (Exception e)
            {
                _UoW.RollbackTransaction();
                throw;
            }
        }

        public async Task<ListViewModel<GetUserResponseModel>> SearchUser(string searchstring, int pageindex, int pagesize)
        {
            ListViewModel<GetUserResponseModel> users = new ListViewModel<GetUserResponseModel>();
            try
            {
                int response = (int)ResponseStatusNumber.Error;
                var user = await _UoW.UserRepository.SearchUser(searchstring, pageindex, pagesize);
                if (user.Count > 0)
                {
                    int totalusers = user.Count;
                    if (pagesize > 0 && pageindex > 0)
                    {
                        user = user.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                    }
                    user.ForEach(x =>
                    {
                        x.Usersites = x.Usersites.Where(x => x.status == (int)Status.Active).ToList();
                        x.Userroles = x.Userroles.Where(x => x.status == (int)Status.Active).ToList();
                    });
                    users.list = _mapper.Map<List<GetUserResponseModel>>(user);
                    users.listsize = totalusers;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return users;
        }

        public async Task<int> TriggerEmailNotificationStatus(bool status)
        {
            try
            {
                User user = await _UoW.UserRepository.GetUserByID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (user != null)
                {
                    user.email_notification_status = status;
                    user.modified_at = DateTime.UtcNow;
                    user.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    _UoW.BeginTransaction();
                    int updateuser = await _UoW.UserRepository.Update(user);
                    if (updateuser > 0)
                    {
                        _UoW.CommitTransaction();
                        return (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        _UoW.RollbackTransaction();
                        return (int)ResponseStatusNumber.Error;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _UoW.RollbackTransaction();
                throw;
            }
        }

        public async Task<int> TriggerPMNotificationStatus(bool status)
        {
            try
            {
                User user = await _UoW.UserRepository.GetUserByID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (user != null)
                {
                    user.manager_pm_notification_status = status;
                    user.modified_at = DateTime.UtcNow;
                    user.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    _UoW.BeginTransaction();
                    int updateuser = await _UoW.UserRepository.Update(user);
                    if (updateuser > 0)
                    {
                        _UoW.CommitTransaction();
                        return (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        _UoW.RollbackTransaction();
                        return (int)ResponseStatusNumber.Error;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _UoW.RollbackTransaction();
                throw;
            }
        }

        public async Task AddUserRoles(CancellationToken cancellationToken)
        {
            try
            {
                List<UserRoles> roles = new List<UserRoles>();
                List<User> users = _UoW.UserRepository.GetAllUsers();

                users.ForEach(x =>
                {
                    if (x.role_id != null || x.role_id != Guid.Empty)
                    {
                        bool isalreadyadded = _UoW.UserRepository.RoleAlreadyAdded(x.uuid, x.role_id.Value);
                        if (!isalreadyadded)
                        {
                            UserRoles role = new UserRoles();
                            role.created_at = x.created_at;
                            role.created_by = x.created_by;
                            role.modified_at = x.modified_at;
                            role.role_id = x.role_id.Value;
                            role.status = x.status;
                            role.user_id = x.uuid;
                            roles.Add(role);
                        }

                        if (x.Role.name == GlobalConstants.Manager || x.Role.name == GlobalConstants.Admin)
                        {
                            x.ac_default_role_web = x.role_id;
                        }
                        else
                        {
                            x.ac_default_role_app = x.role_id;
                        }
                        //x.default_app = 1;
                    }
                });
                bool update = _UoW.BaseGenericRepository<UserRoles>().UpdateList(roles);

            }
            catch
            {
                throw;
            }
        }

        public async Task<int> UpdateDefaultRole(UpdateDefaultRoleRequestModel requestModel)
        {
            try
            {
                User user = await _UoW.UserRepository.GetUserByID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (user != null)
                {
                    if (user.Userroles.Any(x => (x.role_id == requestModel.role_id && x.status == (int)Status.Active)))
                    {
                        if (requestModel.platform == (int)PlatForm.App)
                        {
                            user.ac_default_role_app = requestModel.role_id;
                        }
                        else if (requestModel.platform == (int)PlatForm.Web)
                        {
                            //if (requestModel.ti_role)
                            //{
                            //    user.ti_default_role = requestModel.role_id.ToString();
                            //}
                            //else
                            //{
                            user.ac_default_role_web = requestModel.role_id;
                            //}
                        }
                        user.modified_at = DateTime.UtcNow;
                        user.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        bool update = await _UoW.BaseGenericRepository<User>().Update(user);

                        if (update)
                        {
                            return (int)ResponseStatusNumber.Success;
                        }
                        else
                        {
                            return (int)ResponseStatusNumber.Error;
                        }
                    }
                    else
                    {
                        return (int)ResponseStatusNumber.NotExists;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> UpdateDefaultSite(UpdateDefaultSiteRequestModel requestModel)
        {
            try
            {
                User user = await _UoW.UserRepository.GetUserByID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (user != null)
                {
                    string userrole = null;
                    if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id) && UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
                    {
                        var roles = await _UoW.UserRepository.GetAllRoles();
                        if (roles?.Count > 0)
                        {
                            userrole = roles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.name).FirstOrDefault();
                        }
                    }
                    var Sitedetails = _UoW.SiteRepository.GetSiteById(requestModel.site_id.ToString());
                    if (user.Usersites.Any(x => (x.site_id == requestModel.site_id && x.status == (int)Status.Active)) || userrole == GlobalConstants.Admin || Sitedetails?.status == (int)Status.AllSiteType)
                    {
                        user.ac_default_site = requestModel.site_id;
                        user.ac_default_client_company = Sitedetails.client_company_id;
                        user.ac_default_company = Sitedetails.company_id;
                        user.modified_at = DateTime.UtcNow;
                        user.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        bool update = await _UoW.BaseGenericRepository<User>().Update(user);

                        if (update)
                        {
                            return (int)ResponseStatusNumber.Success;
                        }
                        else
                        {
                            return (int)ResponseStatusNumber.Error;
                        }
                    }
                    else
                    {
                        return (int)ResponseStatusNumber.NotExists;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch
            {
                throw;
            }
        }

        //public async Task<int> ResetUserPassword(UpdateUserPasswordRequestModel requestModel)
        //{
        //    try
        //    {
        //        User user = _UoW.UserRepository.GetUserByEmailID(requestModel.email_id);
        //        if (user != null)
        //        {
        //            string hashpassword = Shared.Utility.PasswordHashUtil.CreateHashPassword(requestModel.password);
        //            user.password = hashpassword;
        //            user.modified_at = DateTime.UtcNow;
        //            bool update = await _UoW.BaseGenericRepository<User>().Update(user);

        //            if (update)
        //            {
        //                return (int)ResponseStatusNumber.Success;
        //            }
        //            else
        //            {
        //                return (int)ResponseStatusNumber.Error;
        //            }
        //        }
        //        else
        //        {
        //            return (int)ResponseStatusNumber.NotFound;
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public async Task<int> TriggerOperatorUsageEmailNotificationStatus(string userid, bool status)
        {
            try
            {
                User user = await _UoW.UserRepository.GetUserByID(userid);
                if (user != null)
                {
                    user.operator_usage_report_email_not_status = status;
                    user.modified_at = DateTime.UtcNow;
                    user.modified_by = userid;
                    _UoW.BeginTransaction();
                    int updateuser = await _UoW.UserRepository.Update(user);
                    if (updateuser > 0)
                    {
                        _UoW.CommitTransaction();
                        return (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        _UoW.RollbackTransaction();
                        return (int)ResponseStatusNumber.Error;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _UoW.RollbackTransaction();
                throw;
            }
        }

        public async Task<int> ResetUserPassword(UpdateUserPasswordRequestModel requestModel)
        {
            try
            {
                ResetPasswordToken token = _UoW.UserRepository.GetUserTokenByTokenID(requestModel.token);

                if (token != null && token.is_used)
                {
                    return (int)ResponseStatusNumber.AlreadyUsedToken;
                }

                if (token != null && token.created_at.Value.AddMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["Token_Expiry_Time"])) < DateTime.UtcNow)
                {
                    return (int)ResponseStatusNumber.TokenExpired;
                }

                if (token != null)
                {
                    User user = await _UoW.UserRepository.GetUserByID(token.user_id.ToString());
                    if (user != null)
                    {
                        token.is_used = true;
                        token.used_at = DateTime.UtcNow;
                        token.modified_at = DateTime.UtcNow;

                        string hashpassword = Shared.Utility.PasswordHashUtil.CreateHashPassword(requestModel.password);
                        if (user.password == hashpassword)
                        {
                            return (int)ResponseStatusNumber.NewPasswordMustBeDifferent;
                        }

                        bool updatetoken = await _UoW.BaseGenericRepository<ResetPasswordToken>().Update(token);
                        if (updatetoken)
                        {
                            user.password = hashpassword;
                            user.modified_at = DateTime.UtcNow;
                            bool update = await _UoW.BaseGenericRepository<User>().Update(user);
                            if (update)
                            {
                                return (int)ResponseStatusNumber.Success;
                            }
                            else
                            {
                                return (int)ResponseStatusNumber.Error;
                            }
                        }
                        else
                        {
                            return (int)ResponseStatusNumber.Error;
                        }
                    }
                    else
                    {
                        return (int)ResponseStatusNumber.NotFound;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.InvalidToken;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> ResetPasswordEmail(string email)
        {
            try
            {
                int response = (int)ResponseStatusNumber.False;
                var user = _UoW.UserRepository.GetUserByEmailID(email);
                if (user != null)
                {
                    if ((int)Status.Active == user.status)
                    {
                        _UoW.BeginTransaction();
                        ResetPasswordToken Resetpassword = new ResetPasswordToken();
                        string token = GenerateRandomString.RandomString(50);
                        Resetpassword.token = token;
                        Resetpassword.created_at = DateTime.UtcNow;
                        Resetpassword.modified_at = DateTime.UtcNow;
                        Resetpassword.email = email;
                        Resetpassword.user_id = user.uuid;
                        Resetpassword.is_used = false;
                        var result = await _UoW.BaseGenericRepository<ResetPasswordToken>().Insert(Resetpassword);
                        if (result)
                        {
                            _UoW.SaveChanges();
                            _UoW.CommitTransaction();
                            List<string> toemail = new List<string> { email };
                            string callbackUrl = UrlGenerator.GetFEChangePasswordUrl(token);
                            Thread th1 = new Thread(() => SendEmail.SendReserPasswordEmail(toemail, callbackUrl, _logger));
                            th1.Start();
                            response = (int)ResponseStatusNumber.Success;
                        }
                        else
                        {
                            _UoW.RollbackTransaction();
                            response = (int)ResponseStatusNumber.Error;
                        }
                    }
                    else
                    {
                        response = (int)ResponseStatusNumber.DeActiveRecord;
                    }
                }
                else
                {
                    response = (int)ResponseStatusNumber.NotFound;
                }
                return response;
            }
            catch
            {
                _UoW.RollbackTransaction();
                throw;
            }
        }

        public async Task<string> GetQuickSightEmbedUrlAsync()
        {
            try
            {
                var response = await _UoW.UserRepository.GetUserRole();
                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }


        public async Task<JwtResponse> UserBarcodeLogin(AuthenticateTokenRequestModel request, JWTTokenRequestModel JWTTokenDetails)
        {
            JwtResponse responseModel = new JwtResponse();
            try
            {
                _logger.LogError("UserAuthenticate Service : Service Begin " + UpdatedGenericRequestmodel.CurrentUser.device_uuid);
                User response = new User();
                response = await _UoW.UserRepository.UserBarcodeLogin(request);
                if (response.uuid != Guid.Empty)
                {
                    int validuser = (int)ResponseStatusNumber.Success;
                    //if (UpdatedGenericRequestmodel.CurrentUser.device_uuid != Guid.Empty)
                    //{
                    //    validuser = _UoW.UserRepository.CheckUserValid(response.uuid.ToString(), UpdatedGenericRequestmodel.CurrentUser.device_uuid);
                    //}
                    if (validuser > 0)
                    {
                        using RSA rsa = RSA.Create();
                        rsa.ImportRSAPrivateKey(Convert.FromBase64String(JWTTokenDetails.privateKey), out _);

                        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
                        {
                            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
                        };

                        var now = DateTime.UtcNow;
                        var unixTimeSeconds = new DateTimeOffset(now).ToUnixTimeSeconds();
                        var afterexpire = now.AddMinutes(60);
                        var afterunixTimeSeconds = new DateTimeOffset(afterexpire).ToUnixTimeSeconds();

                        var jwt = new JwtSecurityToken(
                            audience: JWTTokenDetails.Audiance,
                            issuer: JWTTokenDetails.Issuer,
                            claims: new Claim[] {
                    new Claim(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("user_id", response.uuid.ToString())
                            },
                            notBefore: now,
                            expires: afterexpire,
                            signingCredentials: signingCredentials
                        );

                        string token = new JwtSecurityTokenHandler().WriteToken(jwt);
                        responseModel.Token = token;
                        responseModel.ExpiresAt = afterunixTimeSeconds;
                        responseModel.response_status = validuser;
                    }
                    else
                    {
                        _logger.LogError("UserAuthenticate Service : Invalid User");
                        responseModel.response_status = validuser;
                    }
                }
                else
                {
                    _logger.LogWarning("UserAuthenticate Service : Response UUID is empty");
                    responseModel.response_status = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("UserAuthenticate Service : " + e.Message.ToString());
                throw e;
            }
            return responseModel;
        }

        public LoginResponceModel GetUserSessionByUserIDAsync(string userid)
        {
            LoginResponceModel responseModel = new LoginResponceModel();
            var response = _UoW.UserRepository.GetUserSessionDetails(userid);
            if (response != null)
            {
                responseModel = _mapper.Map<LoginResponceModel>(response);
                responseModel.Userroles = responseModel.Userroles.Where(x => x.status == (int)Status.Active).ToList();
                responseModel.Usersites = responseModel.Usersites.Where(x => x.status == (int)Status.Active).ToList();
                responseModel.Usersites.ForEach(x =>
                {
                    if (x.status == (int)Status.Active)
                    {
                        var sites = _UoW.UserRepository.GetSiteBySiteId(x.site_id);
                        if (sites.site_id != null && sites.site_id != Guid.Empty)
                        {
                            x.site_name = sites.site_name;
                            x.site_code = sites.site_code;
                            x.location = sites.location;
                        }
                    }
                });
            }
            return responseModel;
        }

        public LoginResponceModel GetUserSessionByUserIDAsyncOptimized(string userid)
        {
            LoginResponceModel responseModel = new LoginResponceModel();
            var response = _UoW.UserRepository.GetUserSessionDetailsOptimized(userid);
            if (response != null)
            {
                responseModel = _mapper.Map<LoginResponceModel>(response);
                /*responseModel.Userroles = responseModel.Userroles.Where(x => x.status == (int)Status.Active).ToList();
                responseModel.Usersites = responseModel.Usersites.Where(x => x.status == (int)Status.Active).ToList();
                responseModel.Usersites.ForEach(x =>
                {
                    if (x.status == (int)Status.Active)
                    {
                        var sites = _UoW.UserRepository.GetSiteBySiteId(x.site_id);
                        if (sites.site_id != null && sites.site_id != Guid.Empty)
                        {
                            x.site_name = sites.site_name;
                            x.site_code = sites.site_code;
                            x.location = sites.location;
                        }
                    }
                });*/
            }
            return responseModel;
        }

        public Guid GetSiteCompanyId(string site_id)
        {
            return _UoW.UserRepository.GetSiteCompanyId(site_id);
        }
        public UserRoles activeRoleCheck(Guid uuid, string active_rolename_web)
        {
            return _UoW.UserRepository.activeRoleCheck(uuid, active_rolename_web);
        }
        public UserSites activeSiteCheck(Guid uuid, string active_site_id)
        {
            return _UoW.UserRepository.activeSiteCheck(uuid, active_site_id);
        }
        public async Task<int> UpdateActiveRole(UpdateActiveRoleRequestModel requestModel)
        {
            try
            {
                User user = await _UoW.UserRepository.GetUserByID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (user != null)
                {
                    if (user.Userroles.Any(x => (x.role_id == requestModel.role_id && x.status == (int)Status.Active)))
                    {
                        if (requestModel.platform == (int)PlatForm.App)
                        {
                            user.ac_active_role_app = requestModel.role_id;
                        }
                        else if (requestModel.platform == (int)PlatForm.Web)
                        {
                            user.ac_active_role_web = requestModel.role_id;
                        }
                        user.modified_at = DateTime.UtcNow;
                        user.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        bool update = await _UoW.BaseGenericRepository<User>().Update(user);

                        if (update)
                        {
                            return (int)ResponseStatusNumber.Success;
                        }
                        else
                        {
                            return (int)ResponseStatusNumber.Error;
                        }
                    }
                    else
                    {
                        return (int)ResponseStatusNumber.NotExists;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch
            {
                throw;
            }
        }

        //public async Task<int> ValidateResetPasswordToken(string token)
        //{
        //    try
        //    {
        //        ResetPasswordToken responseToken = _UoW.UserRepository.GetUserTokenByTokenID(token);

        //        if (responseToken != null && responseToken.is_used)
        //        {
        //            return (int)ResponseStatusNumber.AlreadyUsedToken;
        //        }

        //        if (responseToken != null && responseToken.created_at.Value.AddMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["Token_Expiry_Time"])) < DateTime.UtcNow)
        //        {
        //            return (int)ResponseStatusNumber.TokenExpired;
        //        }

        //        if (responseToken != null)
        //        {
        //            return (int)ResponseStatusNumber.Success;
        //        }
        //        else
        //        {
        //            return (int)ResponseStatusNumber.InvalidToken;
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public async Task<ListViewModel<GetUserResponseModel>> GetAllOperatorsList()
        {
            ListViewModel<GetUserResponseModel> users = new ListViewModel<GetUserResponseModel>();
            try
            {
                int response = (int)ResponseStatusNumber.Error;
                var user = await _UoW.UserRepository.GetAllOperatorsList();
                if (user.Count > 0)
                {
                    int totalusers = user.Count;
                    users.list = _mapper.Map<List<GetUserResponseModel>>(user);
                    users.listsize = users.list.Count;
                    users.result = (int)ResponseStatusNumber.Success;
                }
                else
                {
                    users.result = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return users;
        }

        public async Task<int> UpdateActiveSite(UpdateActiveSiteRequestModel requestModel)
        {
            try
            {
                User user = await _UoW.UserRepository.GetUserByID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (user != null)
                {
                    string userrole = null;
                    if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id) && UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
                    {
                        var roles = await _UoW.UserRepository.GetAllRoles();
                        if (roles?.Count > 0)
                        {
                            userrole = roles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.name).FirstOrDefault();
                        }
                    }
                    var Sitedetails = _UoW.SiteRepository.GetSiteById(requestModel.site_id.ToString());
                    if (user.Usersites.Any(x => (x.site_id == requestModel.site_id && x.status == (int)Status.Active)) || Sitedetails?.status == (int)Status.AllSiteType || userrole == GlobalConstants.Admin)
                    {
                        user.ac_active_site = requestModel.site_id;
                        user.modified_at = DateTime.UtcNow;
                        user.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        bool update = await _UoW.BaseGenericRepository<User>().Update(user);

                        if (update)
                        {
                            return (int)ResponseStatusNumber.Success;
                        }
                        else
                        {
                            return (int)ResponseStatusNumber.Error;
                        }
                    }
                    else
                    {
                        return (int)ResponseStatusNumber.NotExists;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Response_Message> CreateCognitoUser(User user, string user_pool_web_client_id, string user_pool_id, string awsAccessKey, string awsSecretKey, string temp_password)
        {
            Response_Message response = new Response_Message();
            try
            {
                var check_mfa_enabled = _UoW.UserRepository.IsMFAEnabled(user_pool_id);

                AmazonCognitoIdentityProviderClient _client = new AmazonCognitoIdentityProviderClient(awsAccessKey, awsSecretKey, RegionEndpoint.USEast2);
                //var userDetails = await _UoW.UserRepository.GetUserByID(user.uuid.ToString());
                string _clientId = user_pool_web_client_id;
                string _poolId = user_pool_id;
                AttributeType attributeType = new AttributeType
                {
                    Name = "email",
                    Value = user.email
                };
                AttributeType isemailVerified = new AttributeType
                {
                    Name = "email_verified",
                    Value = "true"
                };
                AttributeType phone_number = new AttributeType
                {
                    Name = "phone_number",
                    Value = user.mobile_number
                };

                /*AttributeType is_phone_number_verified = new AttributeType
                {
                    Name = "phone_number_verified",
                    Value = "true"
                };*/
                AdminCreateUserRequest signUpRequest = new AdminCreateUserRequest()
                {
                    UserPoolId = _poolId,
                    TemporaryPassword = temp_password,
                    Username = user.username
                };
                signUpRequest.UserAttributes.Add(attributeType);
                signUpRequest.UserAttributes.Add(isemailVerified);
                if (check_mfa_enabled)
                {
                    signUpRequest.UserAttributes.Add(phone_number);
                   // signUpRequest.UserAttributes.Add(is_phone_number_verified);
                }
                var signUpResult = await _client.AdminCreateUserAsync(signUpRequest);
                //await UpdateCognitoUserPhoneNumberAfterSignup(user, user_pool_web_client_id, user_pool_id, awsAccessKey, awsSecretKey);
                response.success = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                response.success = (int)ResponseStatusNumber.Error;
                response.message = e.Message;
            }
            return response;
        }

        public async Task<Response_Message> UpdateCognitoUserPhoneNumberAfterSignup(User user, string user_pool_web_client_id, string user_pool_id, string awsAccessKey, string awsSecretKey)
        {
            Response_Message response = new Response_Message();
            try
            {
                AmazonCognitoIdentityProviderClient _client = new AmazonCognitoIdentityProviderClient(awsAccessKey, awsSecretKey, RegionEndpoint.USEast2);
                //var userDetails = await _UoW.UserRepository.GetUserByID(user.uuid.ToString());
                string _clientId = user_pool_web_client_id;
                string _poolId = user_pool_id;
                var request = new AdminUpdateUserAttributesRequest
                {
                    UserPoolId = user_pool_web_client_id,
                    Username = user.email,  // Use email here if it is configured as an alias
                    UserAttributes = new List<AttributeType>
                    {
                        new AttributeType
                        {
                            Name = "phone_number",
                            Value = user.mobile_number
                        },
                        new AttributeType
                        {
                            Name = "phone_number_verified",
                            Value = "true"
                        }
                    }
                };

                var signUpResult = await _client.AdminUpdateUserAttributesAsync(request);
                response.success = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                response.success = (int)ResponseStatusNumber.Error;
                response.message = e.Message;
            }
            return response;
        }

        public async Task<Response_Message> UpdateCognitoUserPhoneNumber(User user, string user_pool_web_client_id, string user_pool_id, string awsAccessKey, string awsSecretKey)
        {
            Response_Message response = new Response_Message();
            try
            {
                AmazonCognitoIdentityProviderClient _client = new AmazonCognitoIdentityProviderClient(awsAccessKey, awsSecretKey, RegionEndpoint.USEast2);
                //var userDetails = await _UoW.UserRepository.GetUserByID(user.uuid.ToString());
                string _clientId = user_pool_web_client_id;
                string _poolId = user_pool_id;
                var request = new AdminUpdateUserAttributesRequest
                {
                    UserPoolId = _poolId,
                    Username = user.email,  // Use email here if it is configured as an alias
                    UserAttributes = new List<AttributeType>
                    {
                        new AttributeType
                        {
                            Name = "phone_number",
                            Value = user.mobile_number
                        },
                        /*new AttributeType
                        {
                            Name = "phone_number_verified",
                            Value = "true"
                        }*/
                    }
                };
                
                var signUpResult = await _client.AdminUpdateUserAttributesAsync(request);
                response.success = (int)ResponseStatusNumber.Success;
            }
            catch (Exception e)
            {
                response.success = (int)ResponseStatusNumber.Error;
                response.message = e.Message;
            }
            return response;
        }
        public async Task<int> ResendCognitoUser(string uuid, string awsAccessKey, string awsSecretKey)
        {
            try
            {
                var userDetails = await _UoW.UserRepository.GetUserByID(uuid);
                if (userDetails != null)
                {
                    AmazonCognitoIdentityProviderClient _client = new AmazonCognitoIdentityProviderClient(awsAccessKey, awsSecretKey, RegionEndpoint.USEast2);
                    var usersites = userDetails.Usersites.FirstOrDefault();
                    string _clientId = usersites.Sites.Company.user_pool_web_client_id;
                    string _poolId = usersites.Sites.Company.user_pool_id;

                    AdminCreateUserRequest signUpRequest = new AdminCreateUserRequest()
                    {
                        UserPoolId = _poolId,
                        TemporaryPassword = GenerateRandomString.GenerateRandomPassword(),
                        Username = userDetails.username,
                        MessageAction = MessageActionType.RESEND
                    };
                    var signUpResult = await _client.AdminCreateUserAsync(signUpRequest);
                    return (int)ResponseStatusNumber.Success;
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int> UpdateEmailVerified(bool status)
        {
            try
            {
                User user = await _UoW.UserRepository.GetUserByID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (user != null)
                {
                    user.is_email_verified = status;
                    user.modified_at = DateTime.UtcNow;
                    user.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    _UoW.BeginTransaction();
                    int updateuser = await _UoW.UserRepository.Update(user);
                    if (updateuser > 0)
                    {
                        _UoW.CommitTransaction();
                        return (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        _UoW.RollbackTransaction();
                        return (int)ResponseStatusNumber.Error;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _UoW.RollbackTransaction();
                throw;
            }
        }

        public async Task<int> UpdateEmailVerifiedV2(string email, string company_id, string domain_name)
        {
            try
            {
                if (String.IsNullOrEmpty(company_id))
                {
                    company_id = _UoW.UserRepository.GetComapnyIdFromDomain(domain_name);
                }
                User user = await _UoW.UserRepository.GetUserByIDForverify(email, company_id);
                if (user != null)
                {
                    user.is_email_verified = true;
                    user.modified_at = DateTime.UtcNow;
                    //user.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    _UoW.BeginTransaction();
                    int updateuser = await _UoW.UserRepository.Update(user);
                    if (updateuser > 0)
                    {
                        _UoW.CommitTransaction();
                        return (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        _UoW.RollbackTransaction();
                        return (int)ResponseStatusNumber.Error;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _UoW.RollbackTransaction();
                throw;
            }
        }

        public async Task<int> UpdateDefaultCompany(UpdateDefaultCompanyRequestModel requestModel)
        {
            try
            {
                User user = await _UoW.UserRepository.GetUserByID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (user != null)
                {
                    user.ac_default_company = requestModel.company_id;
                    if (Guid.Empty != requestModel.site_id && requestModel.site_id != null)
                    {
                        user.ac_default_site = requestModel.site_id;
                    }
                    user.modified_at = DateTime.UtcNow;
                    user.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    bool update = await _UoW.BaseGenericRepository<User>().Update(user);

                    if (update)
                    {
                        return (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        return (int)ResponseStatusNumber.Error;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch
            {
                throw;
            }
        }
        public async Task<int> UpdateDefaultClientCompany(UpdateDefaultCompanyRequestModel requestModel)
        {
            try
            {
                User user = await _UoW.UserRepository.GetUserByID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (user != null)
                {
                    user.ac_default_client_company = requestModel.company_id;
                    if (Guid.Empty != requestModel.site_id && requestModel.site_id != null)
                    {
                        user.ac_default_site = requestModel.site_id;
                    }
                    user.modified_at = DateTime.UtcNow;
                    user.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    bool update = await _UoW.BaseGenericRepository<User>().Update(user);

                    if (update)
                    {
                        return (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        return (int)ResponseStatusNumber.Error;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> UpdateActiveCompany(UpdateDefaultCompanyRequestModel requestModel)
        {
            try
            {
                User user = await _UoW.UserRepository.GetUserByID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (user != null)
                {
                    user.ac_active_company = requestModel.company_id;
                    if (Guid.Empty != requestModel.site_id && requestModel.site_id != null)
                    {
                        user.ac_active_site = requestModel.site_id;
                    }
                    user.modified_at = DateTime.UtcNow;
                    user.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    bool update = await _UoW.BaseGenericRepository<User>().Update(user);

                    if (update)
                    {
                        return (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        return (int)ResponseStatusNumber.Error;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch
            {
                throw;
            }
        }
        public async Task<int> UpdateActiveClientCompany(UpdateDefaultCompanyRequestModel requestModel)
        {
            try
            {
                User user = await _UoW.UserRepository.GetUserByID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (user != null)
                {
                    user.ac_active_client_company = requestModel.company_id;

                    user.modified_at = DateTime.UtcNow;
                    user.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    bool update = await _UoW.BaseGenericRepository<User>().Update(user);

                    if (update)
                    {
                        return (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        return (int)ResponseStatusNumber.Error;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> UpdateExecutiveEmailNotificationStatus(int status)
        {
            try
            {
                User user = await _UoW.UserRepository.GetUserByID(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
                if (user != null)
                {
                    if (status > 0)
                    {
                        user.executive_report_status = status;
                    }
                    else
                    {
                        user.executive_report_status = null;
                    }

                    user.modified_at = DateTime.UtcNow;
                    user.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    _UoW.BeginTransaction();
                    int updateuser = await _UoW.UserRepository.Update(user);
                    if (updateuser > 0)
                    {
                        _UoW.CommitTransaction();
                        return (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        _UoW.RollbackTransaction();
                        return (int)ResponseStatusNumber.Error;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _UoW.RollbackTransaction();
                throw;
            }
        }

        public async Task<int> UpdateExecutivePMDueReportEmailStatus(ExecutivePMDueEmailConfigRequestModel requestModel)
        {
            try
            {
                UserEmailNotificationConfigurationSettings userEmailConfig = await _UoW.UserRepository.GetExecutiveRolePMDueReportConfig(UpdatedGenericRequestmodel.CurrentUser.requested_by);
                if (userEmailConfig != null)
                {
                    userEmailConfig.modified_at = DateTime.UtcNow;
                    _UoW.BeginTransaction();
                    if (!requestModel.executive_pm_due_not_resolved_email_notification)
                    {
                        userEmailConfig.disable_till = requestModel.disable_till;
                        userEmailConfig.disable_till_by = requestModel.disable_till_by;
                        userEmailConfig.setup_on = DateTime.UtcNow;
                        userEmailConfig.executive_pm_due_not_resolved_email_notification = requestModel.executive_pm_due_not_resolved_email_notification;
                        if (requestModel.disable_till_by == (int)Status.Month)
                        {
                            userEmailConfig.disabled_till_date = DateTime.UtcNow.Date.AddMonths(requestModel.disable_till.Value);
                        }
                        else if (requestModel.disable_till_by == (int)Status.Week)
                        {
                            userEmailConfig.disabled_till_date = DateTime.UtcNow.Date.AddDays(requestModel.disable_till.Value * 7);
                        }
                        else if (requestModel.disable_till_by == (int)Status.Day)
                        {
                            userEmailConfig.disabled_till_date = DateTime.UtcNow.Date.AddDays(requestModel.disable_till.Value);
                        }
                        else
                        {
                            return (int)ResponseStatusNumber.InvalidData;
                        }
                    }
                    else
                    {
                        userEmailConfig.executive_pm_due_not_resolved_email_notification = requestModel.executive_pm_due_not_resolved_email_notification;
                    }
                    var updateuser = await _UoW.BaseGenericRepository<UserEmailNotificationConfigurationSettings>().Update(userEmailConfig);
                    if (updateuser)
                    {
                        _UoW.CommitTransaction();
                        return (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        _UoW.RollbackTransaction();
                        return (int)ResponseStatusNumber.Error;
                    }
                }
                else
                {
                    if (!requestModel.executive_pm_due_not_resolved_email_notification)
                    {
                        if (requestModel.disable_till_by == (int)Status.Month)
                        {
                            requestModel.disabled_till_date = DateTime.UtcNow.Date.AddMonths(requestModel.disable_till.Value);
                        }
                        else if (requestModel.disable_till_by == (int)Status.Week)
                        {
                            requestModel.disabled_till_date = DateTime.UtcNow.Date.AddDays(requestModel.disable_till.Value * 7);
                        }
                        else if (requestModel.disable_till_by == (int)Status.Day)
                        {
                            requestModel.disabled_till_date = DateTime.UtcNow.Date.AddDays(requestModel.disable_till.Value);
                        }
                        else
                        {
                            return (int)ResponseStatusNumber.InvalidData;
                        }
                    }
                    UserEmailNotificationConfigurationSettings userEmail = _mapper.Map<UserEmailNotificationConfigurationSettings>(requestModel);
                    userEmail.status = (int)Status.Active;
                    userEmail.created_at = DateTime.UtcNow;
                    userEmail.setup_on = DateTime.UtcNow;
                    userEmail.user_id = UpdatedGenericRequestmodel.CurrentUser.requested_by;
                    var insert = await _UoW.BaseGenericRepository<UserEmailNotificationConfigurationSettings>().Insert(userEmail);
                    if (insert)
                    {
                        _UoW.SaveChanges();
                        _UoW.CommitTransaction();
                        return (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        _UoW.RollbackTransaction();
                        return (int)ResponseStatusNumber.Error;
                    }
                }
            }
            catch (Exception e)
            {
                _UoW.RollbackTransaction();
                throw e;
            }
        }
        public List<UserRoles> GetUserRolesByEmail(string email_id)
        {
            List<UserRoles> response = new List<UserRoles>();

            response = _UoW.UserRepository.GetUserRolesByEmail(email_id);

            return response;
        }
        public MobileAppVersion MobileAppVersion(int device_brand)
        {
            var get_version = _UoW.UserRepository.MobileAppVersion(device_brand);
            /*MobileAppVersion get_version = new MobileAppVersion();
            get_version.device_brand = 1;
            get_version.force_to_update_app_version = 0.8;
            get_version.store_app_version = 0.9;*/
            return get_version;
        }

        public async Task<CreateUpdateSiteResponsemodel> CreateClientCompany(CreateClientCompanyRequestModel requestModel)
        {
            CreateUpdateSiteResponsemodel CreateUpdateSiteResponsemodel = new CreateUpdateSiteResponsemodel();
            CreateUpdateSiteResponsemodel.success = (int)ResponseStatusNumber.Error;
            try
            {             //  Update
                if (requestModel.client_company_id != null && requestModel.client_company_id != Guid.Empty)
                {
                    var get_client_company = _UoW.UserRepository.GetClientCompanyById(requestModel.client_company_id.Value);
                    if (get_client_company != null)
                    {
                        // check if request CC update is for inactive and any user is only assign to to this CC or not
                        if (requestModel.status == (int)Status.Deactive && get_client_company.status == (int)Status.Active)
                        {
                            var get_user_only_this_site = _UoW.UserRepository.GetUserWithSingleClientComapany(requestModel.client_company_id.Value, get_client_company.parent_company_id.Value);
                            if (get_user_only_this_site.Count > 0)
                            {
                                CreateUpdateSiteResponsemodel.success = (int)ResponseStatusNumber.user_exist_with_single_site;
                                CreateUpdateSiteResponsemodel.user_emails = String.Join(", ", get_user_only_this_site);
                                return CreateUpdateSiteResponsemodel;
                            }
                        }

                        get_client_company.client_company_name = requestModel.client_company_name;
                        get_client_company.clientcompany_code = requestModel.client_company_code;
                        get_client_company.owner = requestModel.owner;
                        get_client_company.owner_address = requestModel.owner_address;
                        get_client_company.status = requestModel.status;
                        get_client_company.modified_at = DateTime.UtcNow;
                        get_client_company.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        var update = await _UoW.BaseGenericRepository<ClientCompany>().Update(get_client_company);
                        _UoW.SaveChanges();

                        if (update)
                        {
                            if (requestModel.status == (int)Status.Deactive)
                            {
                                var get_sites = _UoW.UserRepository.GetSitesByClientCompanyId(requestModel.client_company_id.Value);

                                foreach (var site in get_sites)
                                {
                                    site.status = (int)Status.Deactive;
                                    site.modified_at = DateTime.UtcNow;
                                    site.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                                    var update_site = await _UoW.BaseGenericRepository<Sites>().Update(site);
                                    _UoW.SaveChanges();
                                }
                            }
                            CreateUpdateSiteResponsemodel.success = (int)ResponseStatusNumber.Success;
                        }
                    }
                }
                else    // Insert
                {
                    ClientCompany clientCompany = new ClientCompany();
                    clientCompany.client_company_name = requestModel.client_company_name;
                    clientCompany.clientcompany_code = requestModel.client_company_code;
                    clientCompany.owner = requestModel.owner;
                    clientCompany.owner_address = requestModel.owner_address;
                    clientCompany.status = requestModel.status;
                    clientCompany.created_at = DateTime.UtcNow;
                    clientCompany.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    clientCompany.parent_company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                    var insert = await _UoW.BaseGenericRepository<ClientCompany>().Insert(clientCompany);
                    _UoW.SaveChanges();

                    if (insert)
                    {
                        requestModel.client_company_id = clientCompany.client_company_id;
                        CreateUpdateSiteResponsemodel.success = (int)ResponseStatusNumber.Success;
                    }
                }
                CreateUpdateSiteResponsemodel.client_company_id = requestModel.client_company_id;
            }
            catch (Exception e) { }

            return CreateUpdateSiteResponsemodel;
        }


        public async Task<CreateUpdateSiteResponsemodel> CreateUpdateSite(CreateUpdateSiteRequestModel requestModel)
        {
            CreateUpdateSiteResponsemodel CreateUpdateSiteResponsemodel = new CreateUpdateSiteResponsemodel();
            CreateUpdateSiteResponsemodel.success = (int)ResponseStatusNumber.Error;
            try
            {   //  Update
                if (requestModel.site_id != null && requestModel.site_id != Guid.Empty)
                {
                    var get_site = _UoW.UserRepository.GetSiteBySiteId(requestModel.site_id.Value);
                    if (get_site != null)
                    {
                        // check if request site update is for inactive and any user is only assign to to this site or not
                        if(requestModel.status == (int)Status.Deactive && get_site.status == (int)Status.Active)
                        {
                            var get_user_only_this_site = _UoW.UserRepository.GetUserWithSingleSite(requestModel.site_id.Value , get_site.company_id);
                            if (get_user_only_this_site.Count > 0)
                            {
                                CreateUpdateSiteResponsemodel.success = (int)ResponseStatusNumber.user_exist_with_single_site;
                                CreateUpdateSiteResponsemodel.user_emails = String.Join(", ", get_user_only_this_site);

                                return CreateUpdateSiteResponsemodel;
                            }
                        }
                        get_site.site_name = requestModel.site_name;
                        get_site.site_code = requestModel.site_code;
                        get_site.isAddAssetClassEnabled = requestModel.is_add_asset_class_enabled;
                        get_site.customer = requestModel.customer;
                        get_site.status = requestModel.status;
                        get_site.city = requestModel.city;
                        get_site.state = requestModel.state;
                        get_site.zip = requestModel.zip;
                        if (requestModel.sitecontact_id != null)
                        {
                            get_site.sitecontact_id = requestModel.sitecontact_id.Value;
                        }
                        get_site.customer_address = requestModel.customer_address;
                        get_site.customer_address_2 = requestModel.customer_address_2;
                        get_site.profile_image = requestModel.profile_image;
                        get_site.modified_at = DateTime.UtcNow;
                        get_site.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        var update = await _UoW.BaseGenericRepository<Sites>().Update(get_site);
                        _UoW.SaveChanges();

                        if(requestModel.sitecontact_id == null)
                        {
                            if(get_site.sitecontact_id != null)
                            {
                                var siteContact =  _UoW.UserRepository.GetSiteContactById(get_site.sitecontact_id.Value);
                                if (siteContact != null)
                                {
                                    siteContact.is_deleted = true;
                                    siteContact.modified_at = DateTime.UtcNow;
                                    siteContact.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                                    var update2 = await _UoW.BaseGenericRepository<SiteContact>().Update(siteContact);
                                    _UoW.SaveChanges();
                                }
                            }
                        }

                        //Add SiteProjectManagerMapping
                        if (requestModel.site_projectmanager_list != null && requestModel.site_projectmanager_list.Count > 0)
                        {
                            foreach (var item in requestModel.site_projectmanager_list)
                            {
                                if (item.site_projectmanager_mapping_id == null)
                                {
                                    SiteProjectManagerMapping _SiteProjectManagerMapping = new SiteProjectManagerMapping();
                                    _SiteProjectManagerMapping.user_id = item.user_id;
                                    _SiteProjectManagerMapping.site_id = get_site.site_id;

                                    _SiteProjectManagerMapping.created_at = DateTime.UtcNow;
                                    _SiteProjectManagerMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                    _SiteProjectManagerMapping.is_deleted = false;

                                    var insert_sitepm = await _UoW.BaseGenericRepository<SiteProjectManagerMapping>().Insert(_SiteProjectManagerMapping);
                                    _UoW.SaveChanges();
                                }
                                else if (item.is_deleted && item.site_projectmanager_mapping_id != null && item.site_projectmanager_mapping_id != Guid.Empty)
                                {
                                    var get_SiteProjectManagerMapping = _UoW.UserRepository.GetSiteProjectManagerMappingById(item.site_projectmanager_mapping_id.Value);

                                    if (get_SiteProjectManagerMapping != null)
                                    {
                                        get_SiteProjectManagerMapping.is_deleted = true;
                                        get_SiteProjectManagerMapping.modified_at = DateTime.UtcNow;
                                        get_SiteProjectManagerMapping.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                                        var update_ = await _UoW.BaseGenericRepository<SiteProjectManagerMapping>().Update(get_SiteProjectManagerMapping);
                                        _UoW.SaveChanges();
                                    }
                                }
                            }
                        }

                        if (update)
                        {
                            CreateUpdateSiteResponsemodel.success =  (int)ResponseStatusNumber.Success;
                        }

                        CreateUpdateSiteResponsemodel.site_id = get_site.site_id;
                        CreateUpdateSiteResponsemodel.site_name = get_site.site_name;
                    }
                }
                else    // Insert
                {
                    if (requestModel.client_company_id == null || requestModel.client_company_id == Guid.Empty)
                    {
                        var get_default_client_com = _UoW.UserRepository.GetDefaultClientCompany();
                        if(get_default_client_com!= null)
                        {
                            requestModel.client_company_id = get_default_client_com.client_company_id;
                        }
                        else
                        {
                            ClientCompany clientCompany = new ClientCompany();
                            clientCompany.client_company_name= "Default";
                            clientCompany.clientcompany_code = "Default";
                            clientCompany.parent_company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                            clientCompany.status = (int)Status.Active;
                            clientCompany.created_at = DateTime.UtcNow;
                            clientCompany.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                            var insert_client_com = await _UoW.BaseGenericRepository<ClientCompany>().Insert(clientCompany);
                            _UoW.SaveChanges();

                            requestModel.client_company_id = clientCompany.client_company_id;
                        }
                    }

                    Sites sites = new Sites();
                    sites.site_name = requestModel.site_name;
                    sites.site_code = requestModel.site_code;
                    sites.isAddAssetClassEnabled = requestModel.is_add_asset_class_enabled;
                    sites.customer = requestModel.customer;
                    sites.customer_address = requestModel.customer_address;
                    sites.customer_address_2 = requestModel.customer_address_2;
                    sites.status = requestModel.status;
                    sites.city = requestModel.city;
                    sites.state = requestModel.state;
                    sites.zip = requestModel.zip;
                    if (requestModel.sitecontact_id != null)
                    {
                        sites.sitecontact_id = requestModel.sitecontact_id.Value;
                    }
                    sites.timezone = "America/Los_Angeles";
                    sites.company_id = requestModel.company_id;
                    sites.client_company_id = requestModel.client_company_id;
                    sites.profile_image = requestModel.profile_image;
                    sites.created_at = DateTime.UtcNow;
                    sites.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    var insert = await _UoW.BaseGenericRepository<Sites>().Insert(sites);
                    _UoW.SaveChanges();

                    CreateUpdateSiteResponsemodel.site_id = sites.site_id;
                    CreateUpdateSiteResponsemodel.site_name = sites.site_name;
                    var newSiteId = sites.site_id;

                    var all_company_admin = _UoW.UserRepository.GetAllCompanyAdmins(requestModel.company_id);
                    foreach (var admin in all_company_admin)
                    {
                        // Add Site Access to all company admin user by adding it to UserSites Table
                        UserSites userSites = new UserSites();

                        userSites.user_id = admin.uuid;
                        userSites.company_id = requestModel.company_id;
                        userSites.site_id = sites.site_id;
                        userSites.status = (int)Status.Active;
                        userSites.created_at = DateTime.UtcNow;
                        userSites.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        var insert_user_site = await _UoW.BaseGenericRepository<UserSites>().Insert(userSites);
                        _UoW.SaveChanges();
                    }

                    bool company_technician_without_site_access = _UoW.UserRepository.CheckForTechnicianSiteAccess(UpdatedGenericRequestmodel.CurrentUser.requested_by, sites.site_id);
                    if(company_technician_without_site_access)
                    {

                        UserSites userSites = new UserSites();

                        userSites.user_id = UpdatedGenericRequestmodel.CurrentUser.requested_by;
                        userSites.company_id = requestModel.company_id;
                        userSites.site_id = sites.site_id;
                        userSites.status = (int)Status.Active;
                        userSites.created_at = DateTime.UtcNow;
                        userSites.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        var insert_user_site = await _UoW.BaseGenericRepository<UserSites>().Insert(userSites);
                        _UoW.SaveChanges();
                    }

                    //Add Default Locations in this Site
                    await AddDefaultLocationsInSite(sites.site_id);

                    //Add SiteProjectManagerMapping
                    if (requestModel.site_projectmanager_list != null && requestModel.site_projectmanager_list.Count > 0)
                    {
                        foreach (var item in requestModel.site_projectmanager_list)
                        {
                            SiteProjectManagerMapping _SiteProjectManagerMapping = new SiteProjectManagerMapping();
                            _SiteProjectManagerMapping.user_id = item.user_id;
                            _SiteProjectManagerMapping.site_id = sites.site_id;

                            _SiteProjectManagerMapping.created_at = DateTime.UtcNow;
                            _SiteProjectManagerMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            _SiteProjectManagerMapping.is_deleted = false;

                            var insert_sitepm = await _UoW.BaseGenericRepository<SiteProjectManagerMapping>().Insert(_SiteProjectManagerMapping);
                            _UoW.SaveChanges();
                        }
                    }

                    if (insert)
                    {
                        CreateUpdateSiteResponsemodel.success = (int)ResponseStatusNumber.Success;
                    }
                }

            }
            catch (Exception e) { }

            return CreateUpdateSiteResponsemodel;
        }

        public async Task<int> AddDefaultLocationsInSite(Guid site_id)
        {
            //Add Default Building Location
            FormIOBuildings FormIOBuildings = new FormIOBuildings();
            FormIOBuildings.formio_building_name = "Default";
            FormIOBuildings.created_at = DateTime.UtcNow;
            FormIOBuildings.site_id = site_id;
            FormIOBuildings.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);

            var insertbuilding = await _UoW.BaseGenericRepository<FormIOBuildings>().Insert(FormIOBuildings);
            _UoW.SaveChanges();

            //Add Default Floors Location
            FormIOFloors FormIOFloors = new FormIOFloors();
            FormIOFloors.formio_floor_name = "Default";
            FormIOFloors.formiobuilding_id = FormIOBuildings.formiobuilding_id;
            FormIOFloors.created_at = DateTime.UtcNow;
            FormIOFloors.site_id = site_id;
            FormIOFloors.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);

            var insertfloor = await _UoW.BaseGenericRepository<FormIOFloors>().Insert(FormIOFloors);
            _UoW.SaveChanges();

            //Add Default Room Location
            FormIORooms FormIORooms = new FormIORooms();
            FormIORooms.formio_room_name = "Default";
            FormIORooms.formiofloor_id = FormIOFloors.formiofloor_id;
            FormIORooms.created_at = DateTime.UtcNow;
            FormIORooms.site_id = site_id;
            FormIORooms.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);

            var insertroom = await _UoW.BaseGenericRepository<FormIORooms>().Insert(FormIORooms);
            _UoW.SaveChanges();

            return 1;
        }


        public ListViewModel<GetAllClientCompanyWithSitesResponseModel> GetAllClientCompanyWithSites(GetAllClientCompanyWithSitesRequestModel requestModel)
        {
            ListViewModel<GetAllClientCompanyWithSitesResponseModel> responseModel = new ListViewModel<GetAllClientCompanyWithSitesResponseModel>();
            try
            {
                var response = _UoW.UserRepository.GetAllClientCompanyWithSites(requestModel);
                if (response.Item1.Count > 0)
                {
                    int totalassets = response.Item2;
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    responseModel.list = _mapper.Map<List<GetAllClientCompanyWithSitesResponseModel>>(response.Item1);

                    responseModel.listsize = totalassets;
                    responseModel.pageIndex = requestModel.pageindex;
                    responseModel.pageSize = requestModel.pagesize;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }


        public async Task<int> AddUpdateWorkOrderWatcher(AddUpdateWorkOrderWatcherRequestModel requestModel)
        {
            int response = (int)ResponseStatusNumber.Error;
            try
            {
                if (requestModel.user_id != null && requestModel.ref_id != null)
                {
                    //Insert in WorkorderWatcherUserMapping table
                    if (requestModel.is_deleted == false)
                    {
                        WorkOrderWatcherUserMapping workOrderWatcherUserMapping = new WorkOrderWatcherUserMapping();
                        workOrderWatcherUserMapping.user_id = requestModel.user_id;
                        workOrderWatcherUserMapping.user_role_type = (int)WatcherUserRoleType.BackOffice_User;// 1 = Back-Office user
                        workOrderWatcherUserMapping.ref_id = requestModel.ref_id;
                        workOrderWatcherUserMapping.ref_type = (int)WatcherRefType.Workorder; // ref_type --> 1 = Workorder
                        workOrderWatcherUserMapping.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                        workOrderWatcherUserMapping.is_deleted = false;
                        workOrderWatcherUserMapping.created_at = DateTime.UtcNow;
                        workOrderWatcherUserMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.request_id;

                        var insert_wo_watcher = await _UoW.BaseGenericRepository<WorkOrderWatcherUserMapping>().Insert(workOrderWatcherUserMapping);
                        _UoW.SaveChanges();
                        response = (int)ResponseStatusNumber.Success;
                    }
                    else //Update in WorkorderWatcherUserMapping table
                    {
                        var get_wo_watcher = _UoW.UserRepository.GetWorkorderWatcherMappingById(requestModel);
                        if (get_wo_watcher != null)
                        {
                            get_wo_watcher.is_deleted = true;
                            get_wo_watcher.modified_at = DateTime.UtcNow;
                            get_wo_watcher.modified_by = UpdatedGenericRequestmodel.CurrentUser.request_id;

                            var update_wo_watcher = await _UoW.BaseGenericRepository<WorkOrderWatcherUserMapping>().Update(get_wo_watcher);
                            _UoW.SaveChanges();
                            response = (int)ResponseStatusNumber.Success;
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            return response;
        }


        public GetActiveUserSitesAndRolesResponseModel GetActiveUserSitesAndRoles(Guid user_id)
        {
            GetActiveUserSitesAndRolesResponseModel responseModel = new GetActiveUserSitesAndRolesResponseModel();
            try
            {

                var get_user = _UoW.UserRepository.GetUserByIdForSites(user_id);

                responseModel = _mapper.Map<GetActiveUserSitesAndRolesResponseModel>(get_user);

                var get_usersites = _UoW.UserRepository.GetActiveUserSitesById(user_id);

                var get_userroles = _UoW.UserRepository.GetActiveUserRolesById(user_id);

                var get_client_company = _UoW.UserRepository.GetActiveClientCompany(get_usersites.Select(x => x.site_id).ToList());

                var mapped_usersites = _mapper.Map<List<ActiveUserSites_Data>>(get_usersites);
                responseModel.usersites = mapped_usersites;
                responseModel.userroles = _mapper.Map<List<ActiveUserRoles_Data>>(get_userroles);
                //responseModel.client_company = _mapper.Map<List<ActiveClintCompany_Data>>(get_client_company);

                var mapped_cc = mapped_usersites.GroupBy(us => new { us.client_company_id, us.client_company_name })
                    .Select(g => new ActiveClintCompany_Data
                    {
                        client_company_id = g.Key.client_company_id.Value,
                        client_company_name = g.Key.client_company_name,
                        client_company_Usersites = g.Select(x => new ClientCompany_Site_Data
                        {
                            site_id = x.site_id,
                            site_name = x.site_name,
                        }).ToList()
                    }).ToList();

                responseModel.client_company = mapped_cc;
            }
            catch (Exception e)
            {
            }
            return responseModel;
        }

        public GetAllTechniciansListResponseModel GetAllProjectManagersList()
        {
            GetAllTechniciansListResponseModel response = new GetAllTechniciansListResponseModel();
            try
            {
                var users = _UoW.UserRepository.GetAllProjectManagersList();
                if (users != null && users.list.Count > 0)
                {
                    response = users;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return response;
        }

        public GetAllTechniciansLocationByWOIdResponseModel GetAllTechniciansLocationByWOId(Guid wo_id)
        {
            GetAllTechniciansLocationByWOIdResponseModel response = new GetAllTechniciansLocationByWOIdResponseModel();
            try
            {
                var get_technicians = _UoW.UserRepository.GetAllTechniciansLocationByWOId(wo_id);
                if (get_technicians != null && get_technicians.list != null && get_technicians.list.Count > 0)
                {
                    response = get_technicians;
                }
            }
            catch (Exception e)
            {
            }
            return response;
        }

        public async Task<int> AddUserGeoLocationData(AddUserGeoLocationDataRequestModel requestModel)
        {
            int res = (int)ResponseStatusNumber.Error;
            try
            {
                UserLocation userLocation = new UserLocation();
                userLocation.user_id = requestModel.user_id;
                userLocation.latitude = requestModel.latitude;
                userLocation.longitude = requestModel.longitude;
                userLocation.device_id = requestModel.device_id;
                userLocation.is_location_active = requestModel.is_location_active;

                userLocation.created_at = DateTime.UtcNow;
                userLocation.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                var insert = await _UoW.BaseGenericRepository<UserLocation>().Insert(userLocation);
                if (insert)
                {
                    res = (int)ResponseStatusNumber.Success;
                    _UoW.SaveChanges();
                }
            }
            catch (Exception e)
            {
            }
            return res;
        }

        public GetAllTechniciansListResponseModel GetAllTechniciansForCalendar(GetAllCalanderWorkordersRequestModel requestModel)
        {
            GetAllTechniciansListResponseModel response = new GetAllTechniciansListResponseModel();
            try
            {
                var users = _UoW.UserRepository.GetAllTechniciansForCalendar(requestModel, true);//pass TRUE for Technicians
                if (users != null && users.list.Count > 0)
                {
                    response = users;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return response;
        }
        public GetAllTechniciansListResponseModel GetAllLeadsForCalendar(GetAllCalanderWorkordersRequestModel requestModel)
        {
            GetAllTechniciansListResponseModel response = new GetAllTechniciansListResponseModel();
            try
            {
                var users = _UoW.UserRepository.GetAllTechniciansForCalendar(requestModel, false);//pass FALSE for Leads
                if (users != null && users.list.Count > 0)
                {
                    response = users;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return response;
        }

        public string GetRoleNameById(Guid role_id)
        {
            return _UoW.UserRepository.GetRoleNameById(role_id);
        }
        public bool CheckUserSessionIsValidOrNot(Guid user_id, Guid user_session_id, Guid role_id)
        {
            return _UoW.UserRepository.CheckUserSessionIsValidOrNot(user_id, user_session_id, role_id);
        }
        public bool IsUserSessionIsRequiredOrNot(Guid company_id)
        {
            return _UoW.UserRepository.IsFeatureRequiredOrNotForCompany(company_id, Guid.Parse(GlobalConstants.session_management_multi_browser_feature_id));
        }
        public string GetSiteNameById(Guid site_id)
        {
            return _UoW.AssetPMsRepository.GetSiteNameById(site_id);
        }

        public async Task<ViewVendorDetailsByIdResponseModel> ViewVendorDetailsById(Guid vendor_id)
        {
            ViewVendorDetailsByIdResponseModel responseModel = new ViewVendorDetailsByIdResponseModel();
            try
            {
                var wo_ids = _UoW.UserRepository.GetWOIdsByVendorId(vendor_id);
                foreach(var wo_id in wo_ids)
                {
                    await UpdateWOInviteesStatus(wo_id);
                }

                var get_vendor = _UoW.UserRepository.GetVendorDetailsById(vendor_id);

                if (get_vendor != null)
                {
                    responseModel = _mapper.Map<ViewVendorDetailsByIdResponseModel>(get_vendor);

                    var get_wos = _UoW.UserRepository.GetWorkordersByVendorId(vendor_id);
                    responseModel.workorders_list = get_wos;
                }
                else
                {
                    responseModel = null;
                }
            }
            catch (Exception e)
            {
            }
            return responseModel;
        }

        public async Task<int> CreateUpdateVendor(CreateUpdateVendorRequestModel requestModel)
        {
            int res = (int)ResponseStatusNumber.Error;

            try
            {

                if (requestModel.vendor_id == null || requestModel.vendor_id == Guid.Empty)//-- Create Vendor
                {
                    Vendors vendors = new Vendors();

                    vendors.vendor_name = requestModel.vendor_name;
                    vendors.vendor_email = requestModel.vendor_email;
                    vendors.vendor_phone_number = requestModel.vendor_phone_number;
                    vendors.vendor_category_id = requestModel.vendor_category_id;
                    vendors.vendor_category = GlobalConstants.VendorCategoryTypesENUMs(requestModel.vendor_category_id);
                    vendors.vendor_address = requestModel.vendor_address;
                    vendors.is_deleted = false;
                    vendors.created_at = DateTime.UtcNow;
                    vendors.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    vendors.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);

                    var insert = await _UoW.BaseGenericRepository<Vendors>().Insert(vendors);
                    if (insert)
                    {
                        _UoW.SaveChanges();
                        res = (int)ResponseStatusNumber.Success;
                    }
                }
                else
                {
                    var get_vendor = _UoW.UserRepository.GetVendorById(requestModel.vendor_id.Value);

                    if (get_vendor != null)
                    {
                        if (requestModel.is_deleted) //-- Delete Vendor
                        {
                            get_vendor.is_deleted = true;
                            get_vendor.modified_at = DateTime.UtcNow;
                            get_vendor.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                            // delete vendor
                            var get_wo_vendor_mappings = _UoW.WorkOrderRepository.GetWOVendorMappingByVendorId(requestModel.vendor_id.Value);
                            foreach (var item in get_wo_vendor_mappings)
                            {
                                item.is_deleted = true;
                                item.modified_at = DateTime.UtcNow;
                                item.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                var update_map = await _UoW.BaseGenericRepository<WorkordersVendorContactsMapping>().Update(item);
                                _UoW.SaveChanges();
                            }
                        }
                        else //-- Update Vendor
                        {
                            get_vendor.vendor_name = requestModel.vendor_name;
                            get_vendor.vendor_email = requestModel.vendor_email;
                            get_vendor.vendor_phone_number = requestModel.vendor_phone_number;
                            get_vendor.vendor_category_id = requestModel.vendor_category_id;
                            get_vendor.vendor_category = GlobalConstants.VendorCategoryTypesENUMs(requestModel.vendor_category_id);
                            get_vendor.vendor_address = requestModel.vendor_address;
                            get_vendor.modified_at = DateTime.UtcNow;
                            get_vendor.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            get_vendor.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                        }
                        var update = await _UoW.BaseGenericRepository<Vendors>().Update(get_vendor);
                        if (update)
                        {
                            _UoW.SaveChanges();
                            res = (int)ResponseStatusNumber.Success;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                res = (int)ResponseStatusNumber.Error;
                return res;
            }
            return res;
        }

        public async Task<int> CreateUpdateContact(CreateUpdateContactRequestModel requestModel)
        {
            int res = (int)ResponseStatusNumber.Error;

            try
            {
                if (requestModel.contact_id == null || requestModel.contact_id == Guid.Empty)//-- Create Contact
                {
                    Contacts contacts = new Contacts();

                    contacts.name = requestModel.name;
                    contacts.category_id = requestModel.category_id;
                    contacts.category = GlobalConstants.ContactCategoryTypesENUMs(requestModel.category_id);
                    contacts.phone_number = requestModel.phone_number;
                    contacts.email = requestModel.email;
                    contacts.notes = requestModel.notes;
                    contacts.mark_as_primary = requestModel.mark_as_primary;
                    contacts.vendor_id = requestModel.vendor_id;
                    contacts.is_deleted = false;
                    contacts.created_at = DateTime.UtcNow;
                    contacts.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    contacts.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);

                    var insert = await _UoW.BaseGenericRepository<Contacts>().Insert(contacts);
                    if (insert)
                    {
                        _UoW.SaveChanges();
                        res = (int)ResponseStatusNumber.Success;
                    }
                }
                else
                {
                    var get_contact = _UoW.UserRepository.GetContactById(requestModel.contact_id.Value);

                    if (get_contact != null)
                    {
                        if (requestModel.is_deleted) //-- Delete Contact
                        {

                            get_contact.is_deleted = true;
                            get_contact.modified_at = DateTime.UtcNow;
                            get_contact.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                            // delete contact
                            var get_wo_contact_mappings = _UoW.WorkOrderRepository.GetWOContactMappingByContactId(requestModel.contact_id.Value);
                            foreach (var item in get_wo_contact_mappings)
                            {
                                item.is_deleted = true;
                                item.modified_at = DateTime.UtcNow;
                                item.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                var update_map = await _UoW.BaseGenericRepository<WorkordersVendorContactsMapping>().Update(item);
                                _UoW.SaveChanges();
                            }
                        }
                        else //-- Update Contact
                        {
                            get_contact.name = requestModel.name;
                            get_contact.category_id = requestModel.category_id;
                            get_contact.category = GlobalConstants.ContactCategoryTypesENUMs(requestModel.category_id);

                            get_contact.phone_number = requestModel.phone_number;
                            get_contact.email = requestModel.email;
                            get_contact.notes = requestModel.notes;
                            get_contact.mark_as_primary = requestModel.mark_as_primary;
                            get_contact.vendor_id = requestModel.vendor_id;
                            get_contact.modified_at = DateTime.UtcNow;
                            get_contact.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            get_contact.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                        }
                        var update = await _UoW.BaseGenericRepository<Contacts>().Update(get_contact);
                        if (update)
                        {
                            _UoW.SaveChanges();
                            res = (int)ResponseStatusNumber.Success;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                res = (int)ResponseStatusNumber.Error;
                return res;
            }
            return res;
        }

        public GetAllVendorListResponseModel GetAllVendorList(GetAllVendorListRequestModel requestModel)
        {
            GetAllVendorListResponseModel response = new GetAllVendorListResponseModel();
            try
            {
                var get_all_vendors = _UoW.UserRepository.GetAllVendorList(requestModel);

                if (get_all_vendors.Item1 != null && get_all_vendors.Item1.Count > 0)
                {
                    var map = _mapper.Map<List<Vendors_Data>>(get_all_vendors.Item1);

                    response.vendors_list = map;
                    response.listsize = get_all_vendors.Item2;
                }
            }
            catch (Exception e)
            {
            }
            return response;
        }

        public GetAllVendorsContactsForDropdownResponseModel GetAllVendorsContactsForDropdown()
        {
            GetAllVendorsContactsForDropdownResponseModel response = new GetAllVendorsContactsForDropdownResponseModel();
            try
            {
                var get_vendor_contacts = _UoW.UserRepository.GetAllVendorsContactsForDropdown();
                response.workorder_vendor_contacts_list = get_vendor_contacts;
            }
            catch (Exception e)
            {
            }
            return response;
        }

        public (int, string) CreateGoogleCalendarEvent(CreateGoogleCalendarEventRequestModel requestModel)
        {
            int response = (int)ResponseStatusNumber.Error;
            string event_id = null;
            try
            {
                // Path to your service account key file
                string serviceAccountKeyFilePath = "eg-ai-441802-c884af399397.json";
                string calendarId = "primary";//"c_d643ba5cf76fa802d1af55190bef58cd8ee213ccd347ed7f9a5bff8a437b4262@group.calendar.google.com"; // Shared calendar ID

                // Authenticate using the service account
                var credential = GoogleCredential.FromFile(serviceAccountKeyFilePath)
                    .CreateScoped(CalendarService.Scope.Calendar).CreateWithUser("dispatch@egalvanic.com");

                // Create the Calendar API service
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "eg-ai",
                });

                List<EventReminder> reminders_list = new List<EventReminder>();
                if (requestModel.reminders_list != null && requestModel.reminders_list.Count>0)
                {
                    reminders_list = requestModel.reminders_list.Select(reminder => new EventReminder
                    {
                        Method = "email", // 0 => email, 1 => popup
                        Minutes = reminder.duration * 60 // Assuming duration is already in minutes
                    }).ToList();
                }
                else
                {
                    reminders_list = new List<EventReminder>
                    {
                         new EventReminder { Method = "email", Minutes = 60 }, // Email 60 minutes before
                         new EventReminder { Method = "popup", Minutes = 10 }  // Popup 10 minutes before
                    };
                }

                //Create new Calenar Event
                if (requestModel.is_requested_for_add && String.IsNullOrEmpty(requestModel.event_id))
                {
                    // Create a new event
                    var newEvent = new Event()
                    {
                        Summary = requestModel.summary,
                        Location = requestModel.location,
                        Description = requestModel.description,
                        Start = new EventDateTime() { DateTime = requestModel.start_datetime, TimeZone = "America/Los_Angeles", },
                        End = new EventDateTime() { DateTime = requestModel.end_datetime, TimeZone = "America/Los_Angeles", },
                        /*
                        Attendees = new System.Collections.Generic.List<EventAttendee>()
                        {
                            //new eventattendee() { email = "mukul@egalvanic.com" },
                            //new eventattendee() { email = "rahul@egalvanic.com" },
                            //new eventattendee() { email = "fenil.kanjiya@egalvanic.com" },
                            new eventattendee() { email = "shubham.goswami@egalvanic.com" },
                            new eventattendee() { email = "shubham.goswami@sculptsoft.com" },
                            new eventattendee() { email = "goswamishubham241101@gmail.com" },
                        },*/
                        Reminders = new Event.RemindersData
                        {
                            UseDefault = false, // Override default reminders
                            Overrides = reminders_list
                            //Overrides = new[]
                            //{

                            //    new EventReminder { Method = "email", Minutes = 60 }, // Email reminder 1 hour before
                            //    new EventReminder { Method = "popup", Minutes = 10 }  // Popup reminder 10 minutes before
                            //}
                        },
                        Attendees = requestModel.attendees_email_list.ConvertAll(email => new EventAttendee { Email = email,Organizer=false }),
                        GuestsCanInviteOthers=false,
                        GuestsCanModify=false,
                        GuestsCanSeeOtherGuests=true,
                    };
                    /*
                    foreach(var attendee in requestModel.attendees_email_list)
                    {
                        AclRule aclRule = new AclRule
                        {
                            Role = "reader", // Grant read-only access
                            Scope = new AclRule.ScopeData
                            {
                                Type = "user",
                                Value = attendee
                            }
                        }; 
                        var aclInsertRequest = service.Acl.Insert(aclRule, calendarId);
                        var aclResult = aclInsertRequest.Execute();
                    }
                    */
                    //newEvent.GuestsCanModify = false;
                    //newEvent.GuestsCanInviteOthers = false;
                    //newEvent.GuestsCanSeeOtherGuests = true;

                    var request = service.Events.Insert(newEvent, calendarId);
                    //request.SendUpdates = SendUpdatesEnum.ExternalOnly;
                    request.SendUpdates = (SendUpdatesEnum?)EventsResource.UpdateRequest.SendUpdatesEnum.All;
                    var createdEvent = request.Execute();
                    event_id = createdEvent.Id;
                    response = (int)ResponseStatusNumber.Success;
                }
                else//Update Calendar Event
                {
                    // Get the existing event
                    var eventRequest = service.Events.Get(calendarId, requestModel.event_id);
                    var existingEvent = eventRequest.Execute();
                    event_id = requestModel.event_id;

                    if (!requestModel.is_deleted)
                    {
                        existingEvent.Start = new EventDateTime() { DateTime = requestModel.start_datetime, TimeZone = "America/Los_Angeles", };
                        existingEvent.End = new EventDateTime() { DateTime = requestModel.end_datetime, TimeZone = "America/Los_Angeles", };
                        existingEvent.Summary = requestModel.summary;
                        existingEvent.Location = requestModel.location;
                        existingEvent.Description = requestModel.description;
                        var list = existingEvent.Attendees.Select(x => x.Email).ToList();

                        //deleted_list items in list but not in attendees_email_list  
                        var deleted_list = list.Except(requestModel.attendees_email_list).ToList();

                        // Find added_list: items in attendees_email_list but not in list
                        var added_list = requestModel.attendees_email_list.Except(list).ToList();

                        if (deleted_list.Count > 0)
                        {
                            existingEvent.Attendees = existingEvent.Attendees
                                .Where(a => !deleted_list.Contains(a.Email)).ToList();
                        }
                        // Add new attendees
                        existingEvent.Attendees = existingEvent.Attendees ?? new System.Collections.Generic.List<EventAttendee>();
                        foreach (var item in added_list)
                        {
                            existingEvent.Attendees.Add(new EventAttendee() { Email = item,Organizer=false });
                            existingEvent.Reminders = new Event.RemindersData { UseDefault = false, Overrides = reminders_list };
                        }

                        /*
                        foreach (var attendee in added_list)
                        {
                            AclRule aclRule = new AclRule
                            {
                                Role = "reader", // Grant read-only access
                                Scope = new AclRule.ScopeData
                                {
                                    Type = "user",
                                    Value = attendee
                                }
                            };
                            var aclInsertRequest = service.Acl.Insert(aclRule, calendarId);
                            var aclResult = aclInsertRequest.Execute();
                        }
                        */

                        existingEvent.GuestsCanModify = false;
                        existingEvent.GuestsCanInviteOthers = false;
                        //existingEvent.Description = "changes in desc" + DateTime.UtcNow.ToString();
                        // Update the event
                        var updateRequest = service.Events.Update(existingEvent, calendarId, existingEvent.Id);
                        updateRequest.SendUpdates = EventsResource.UpdateRequest.SendUpdatesEnum.All;
                        var updatedEvent = updateRequest.Execute();
                        response = (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        var deleteRequest = service.Events.Delete(calendarId, requestModel.event_id);
                        deleteRequest.Execute();
                        response = (int)ResponseStatusNumber.Success;
                    }
                }
            }
            catch (Exception e)
            {
            }
            return (response, event_id);
        }

        public async Task<int> UpdateWOInviteesStatus(Guid wo_id)
        {
            int res = (int)ResponseStatusNumber.Error;
            string event_id = null;
            try
            {
                var getWO = _UoW.WorkOrderRepository.GetWOByidforUpdateOffline(wo_id);
                if (getWO != null && !String.IsNullOrEmpty(getWO.calendarId))
                {
                    event_id = getWO.calendarId;
                    // Path to your service account key file
                    string serviceAccountKeyFilePath = "eg-ai-441802-c884af399397.json";
                    string calendarId = "c_d643ba5cf76fa802d1af55190bef58cd8ee213ccd347ed7f9a5bff8a437b4262@group.calendar.google.com"; // Shared calendar ID

                    // Authenticate
                    var credential = GoogleCredential.FromFile(serviceAccountKeyFilePath)
                        .CreateScoped(CalendarService.Scope.Calendar);

                    var service = new CalendarService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "eg-ai",
                    });

                    // Get the event
                    var eventRequest = service.Events.Get(calendarId, event_id);
                    Event calendarEvent = eventRequest.Execute();

                    // Check attendee responses
                    if (calendarEvent.Attendees != null)
                    {
                        foreach (var attendee in calendarEvent.Attendees)
                        {
                            string email = attendee.Email;
                            string responseStatus = attendee.ResponseStatus;

                            // Update database based on responseStatus
                            int status = responseStatus switch
                            {
                                "accepted" => 1,
                                "declined" => 2,
                                "needsAction" => 3,
                                "tentative" => 4,
                                _ => 0,
                            };
                            var contact_id = _UoW.UserRepository.GetContactIdByEmail(email,wo_id);
                            if (contact_id != null && contact_id != Guid.Empty)
                            {
                                var con_map = _UoW.UserRepository.GetWOContactMapById(contact_id, wo_id);
                                if (con_map != null)
                                {
                                    con_map.contact_invite_status = status;
                                    con_map.modified_at = DateTime.UtcNow;
                                    con_map.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                    var update = await _UoW.BaseGenericRepository<WorkordersVendorContactsMapping>().Update(con_map);
                                }
                            }
                        }
                    }
                    _UoW.SaveChanges();
                    res = (int)ResponseStatusNumber.Success;
                }
            }
            catch (Exception e)
            {
            }
            return res;
        }

        public async Task<GetRefreshedContactsByWOIdResponseModel> GetRefreshedContactsByWOId(GetRefreshedContactsByWOIdRequestModel requestModel)
        {
            GetRefreshedContactsByWOIdResponseModel res = new GetRefreshedContactsByWOIdResponseModel();
            try
            {
                var getWO = _UoW.WorkOrderRepository.GetWObyIdforIRlambdareport(requestModel.wo_id);
                if (getWO != null && !String.IsNullOrEmpty(getWO.calendarId))
                    await UpdateWOInviteesStatus(requestModel.wo_id);

                var obj = _UoW.UserRepository.GetRefreshedContactsByWOId(requestModel.wo_id);
                res = obj;
            }
            catch(Exception e)
            {
            }
            return res;
        }

        public class CreateGoogleCalendarEventRequestModel
        {
            public bool is_requested_for_add { get; set; }
            public bool is_deleted { get; set; }
            public string event_id { get; set; }
            public string summary { get; set; }
            public string location { get; set; }
            public string description { get; set; }
            public DateTime start_datetime { get; set; }
            public DateTime end_datetime { get; set; }
            public List<string> attendees_email_list { get; set; }
            public List<Reminders_Json_Class> reminders_list { get; set; }
        }
        public class Reminders_Json_Class
        {
            public int duration { get; set; }
            public int type { get; set; }
        }

        public GetSiteUsersDetailsByIdResponseModel GetSiteUsersDetailsById( Guid site_id)
        {
            GetSiteUsersDetailsByIdResponseModel response = new GetSiteUsersDetailsByIdResponseModel();
            try
            {
                response = _UoW.UserRepository.GetSiteUsersDetailsById(site_id);
                
            }
            catch (Exception e)
            {
                throw e;
            }
            
            return response;
        }

        public async Task<int> AddUpdateSiteContact(AddUpdateSiteContactRequestModel requestModel)
        {
            int responseStatusNumber = (int)ResponseStatusNumber.Error;

            try
            {
                if (requestModel.sitecontact_id != null && requestModel.sitecontact_id != Guid.Empty)
                {
                    var getSiteContactDetails = _UoW.UserRepository.GetSiteContactById(requestModel.sitecontact_id.Value);

                    if (getSiteContactDetails != null)
                    {
                        getSiteContactDetails.client_company_id = requestModel.client_company_id;
                        getSiteContactDetails.sitecontact_title = requestModel.sitecontact_title;
                        getSiteContactDetails.sitecontact_name = requestModel.sitecontact_name;
                        getSiteContactDetails.sitecontact_email = requestModel.sitecontact_email;
                        getSiteContactDetails.sitecontact_phone = requestModel.sitecontact_phone;
                        getSiteContactDetails.modified_at = DateTime.UtcNow;
                        getSiteContactDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                        bool update = await _UoW.BaseGenericRepository<SiteContact>().Update(getSiteContactDetails);
                        if (update)
                        {
                            _UoW.SaveChanges();
                            responseStatusNumber = (int)ResponseStatusNumber.Success;
                        }
                    }
                    else
                    {
                        responseStatusNumber = (int)ResponseStatusNumber.NotFound;
                    }

                }
                else
                {

                    SiteContact siteContact = new SiteContact();
                    siteContact.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                    siteContact.client_company_id = requestModel.client_company_id;
                    siteContact.sitecontact_title = requestModel.sitecontact_title;
                    siteContact.sitecontact_name = requestModel.sitecontact_name;
                    siteContact.sitecontact_email = requestModel.sitecontact_email;
                    siteContact.sitecontact_phone = requestModel.sitecontact_phone;
                    siteContact.is_deleted = false;
                    siteContact.created_at = DateTime.UtcNow;
                    siteContact.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                    var result = await _UoW.BaseGenericRepository<SiteContact>().Insert(siteContact);

                    if (result)
                    {
                        _UoW.SaveChanges();
                        responseStatusNumber = (int)ResponseStatusNumber.Success;
                    }
                }

                return responseStatusNumber;
            }
            catch (Exception)
            {
                return responseStatusNumber;
            }
        }

        public async Task<int> DeleteSiteContactById(Guid sitecontact_id)
        {
            int responseStatusNumber = (int)ResponseStatusNumber.Error;
            try
            {
                if (!string.IsNullOrEmpty(sitecontact_id.ToString()))
                {
                    var getSiteContactDetails = _UoW.UserRepository.GetSiteContactById(sitecontact_id);
                    if (getSiteContactDetails != null)
                    {
                        getSiteContactDetails.is_deleted = true;


                        var update = await _UoW.BaseGenericRepository<SiteContact>().Update(getSiteContactDetails);
                        if (update)
                        {
                            _UoW.SaveChanges();
                            responseStatusNumber = (int)ResponseStatusNumber.Success;
                        }
                    }
                    else
                    {
                        responseStatusNumber = (int)ResponseStatusNumber.NotFound;
                    }
                }
                else
                {
                    responseStatusNumber = (int)ResponseStatusNumber.Error;
                }

                return responseStatusNumber;
            }
            catch (Exception)
            {
                return responseStatusNumber;
            }
        }

    }
}