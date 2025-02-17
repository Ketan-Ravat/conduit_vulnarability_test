using Amazon.S3.Model.Internal.MarshallTransformations;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete
{
    public class UserRepository : BaseGenericRepository<User>, IUserRepository
    {
        //private IBaseGenericRepository<User> userRepository;
        private readonly DBContextFactory context;
        //private DbSet<T> dbSet;

        public UserRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
            //this.userRepository = userRepository;
            //dbSet = context.Set<User>();
        }

        public virtual async Task<int> Insert(User entity)
        {
            int IsSuccess;
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                else
                {
                    entity.Usersites.ToList().ForEach(x => x.created_at = DateTime.UtcNow);
                    entity.Usersites.ToList().ForEach(x => x.modified_at = DateTime.UtcNow);
                    entity.Usersites.ToList().ForEach(x => x.status = 1);
                    var alreadyregister = context.User.Include(x=>x.Active_Company)
                        .Where(x => x.username == entity.username 
                        && x.Active_Company.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id))
                        .FirstOrDefault();

                    if (alreadyregister != null)
                    {
                        if (alreadyregister.uuid == null || alreadyregister.uuid == Guid.Empty)
                        {
                            Add(entity);
                            IsSuccess = (int)ResponseStatusNumber.Success;
                        }
                        else
                        {
                            IsSuccess = (int)ResponseStatusNumber.AlreadyExists;
                        }
                    }
                    else
                    {
                        Add(entity);
                        IsSuccess = (int)ResponseStatusNumber.Success;
                    }
                }
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
            //catch (DbEntityValidationException dbEx)
            //{
            //    var msg = string.Empty;

            //    foreach (var validationErrors in dbEx.EntityValidationErrors)
            //    {
            //        foreach (var validationError in validationErrors.ValidationErrors)
            //        {
            //            msg += string.Format("Property: {0} Error: {1}",
            //            validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
            //        }
            //    }
            //    var fail = new Exception(msg, dbEx);
            //    throw fail;
            //}
        }

        public virtual async Task<int> Update(User entity)
        {
            int IsSuccess = 0;
            try
            {
                var isregister = context.User.Where(x => x.uuid == entity.uuid).FirstOrDefault();
                if (isregister.uuid != Guid.Empty || isregister.uuid != null)
                {
                    dbSet.Update(entity);
                    IsSuccess = await context.SaveChangesAsync();
                }
                else
                {
                    IsSuccess = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public List<User> GetUsers(int status, int pageindex, int pagesize)
        {
            List<User> users = new List<User>();
            //return context.User.Include(x => x.Usersites).ToList();
            string userrole = null;
            if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id) && UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                userrole = context.UserRoles.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();
            }
            // if user active role is Admin then show all role users 
            if (userrole == GlobalConstants.Admin)
            {
                if (status > 0)
                {
                    users = context.Set<User>().Where(x => x.status == status || x.uuid.ToString() != UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString()).Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company)
                            .Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
                else
                {
                    users = context.Set<User>().Where(x => x.uuid.ToString() != UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString()).Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster)
                        .Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }

            }
            else if (!string.IsNullOrEmpty(userrole))
            {
                List<Guid> userlist = new List<Guid>();

                // if user active role is Manager then show Manager, Operator and MS Role users
                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (usersite.Count > 0)
                {
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) && x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive)) && (x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive)) && (x.Role.name != GlobalConstants.Executive || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Deactive))))
                        .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }

                if (userlist.Count > 0 && status > 0)
                {
                    users = context.User.Where(x => x.status == status && userlist.Contains(x.uuid))
                        .Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
                else if (userlist.Count > 0)
                {
                    users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
            }
            return users.OrderByDescending(x => x.created_at).ToList();
        }

        public List<User> FilterUsers(FilterUsersRequestModel requestModel)
        {
            List<User> users = new List<User>();
            //return context.User.Include(x => x.Usersites).ToList();
            string userrole = null;
            if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id) && UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                userrole = context.UserRoles.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();
            }
            // if user active role is Admin then show all role users 
            if (userrole == GlobalConstants.Admin)
            {
                List<Guid> userlist = new List<Guid>();
                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.Active)
                {
                    var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                    if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                    {
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id)
                        && x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))))
                       .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                    }
                    else
                    {
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && //x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                        x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))))
                       .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                    }
                    if (userlist.Count > 0)
                    {
                        users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                                .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                    }
                }
                else if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.AllCompanyType)
                {
                    if (UpdatedGenericRequestmodel.CurrentUser.site_status != (int)Status.AllSiteType)
                    {
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                        x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))))
                       .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                        users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                    }
                    else
                    {
                        users = context.User.Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                    }
                }
                else
                {
                    users = context.User.Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
                if (requestModel.status > 0)
                {
                    users = users.Where(x => x.status == requestModel.status).ToList();
                }
            }
            else if (userrole == GlobalConstants.CompanyAdmin)
            {
                List<Guid> userlist = new List<Guid>();
                var usersite = new List<Guid>();

                var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id ).Include(x => x.Company).ToList();//&& x.status == (int)Status.Active
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    // Company admin can get data for users excluding Company Admin
                    // userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) &&
                    // x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    // && (x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive))))
                    //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                    // Company admin can get data for users including Company Admin
                    userlist = context.UserSites.Where(x =>  companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && //x.status == (int)Status.Active &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                    .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }
                else
                {
                    // Company admin can get data for users excluding Company Admin
                    // userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                    // x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    // && (x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive))))
                    //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                    // Company admin can get data for users including Company Admin
                    //userlist = context.UserSites.Where(x=>  companysites.Select(x => x.site_id).ToList().Contains(x.site_id)).Include(x => x.User).Select(x => x.user_id).Distinct().ToList();
                    userlist = context.UserSites.Where(x =>  companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && // x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id && x.status == (int)Status.Active &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                   .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }
                if (userlist.Count > 0)
                {
                    //Inactive Operator will not show in User List
                    users = context.User.Where(x => (x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator)) ||
                         (x.status == (int)Status.Active && x.Userroles.All(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active)))) && userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                           .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();

                    //users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                    //        .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }

                if (requestModel.status > 0)
                {
                    //Inactive Operator will not show in User List
                    //if (requestModel.status == (int)Status.Active)
                    //{
                    //    users = users.Where(x => ((x.status == requestModel.status && x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator && x.status == (int)Status.Active))) ||
                    //     (x.status == (int)Status.Active && x.Userroles.All(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active))))).ToList();
                    //}
                    //else
                    //{
                    //    users = users.Where(x => (x.status == requestModel.status && x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator && x.status == (int)Status.Active)))).ToList();
                    //}

                    users = users.Where(x => x.status == requestModel.status).ToList();
                }
            }
            else if (userrole == GlobalConstants.Manager)
            {
                List<Guid> userlist = new List<Guid>();

                // if user active role is Manager then show Manager, Operator and MS Role users
                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (requestModel.site_id?.Count > 0)
                    {
                        usersite = usersite.Where(x => requestModel.site_id.Contains(x.ToString())).ToList();
                    }
                }
                else
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (usersite.Count > 0)
                {
                    //Manager can view the data of Manager, Operator and MS
                    //userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) &&
                    //x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive)) &&
                    //(x.Role.name != GlobalConstants.Executive || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Deactive)) &&
                    //(x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive))))
                    //    .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                    //Manager can get the data of Company Admin, Executive, Manager, Operator and MS
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                        .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }

                if (userlist.Count > 0 && requestModel.status > 0)
                {
                    //Inactive Operator will not show in User List
                    //if (requestModel.status == (int)Status.Active)
                    //{
                    //    users = context.User.Where(x => ((x.status == requestModel.status && x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator && x.status == (int)Status.Active))) ||
                    //     (x.status == (int)Status.Active && x.Userroles.All(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active)))) && userlist.Contains(x.uuid))
                    //    .Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).ToList();
                    //}
                    //else
                    //{
                    //    users = context.User.Where(x => (x.status == requestModel.status && x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator && x.status == (int)Status.Active))) && userlist.Contains(x.uuid))
                    //    .Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).ToList();
                    //}

                    users = context.User.Where(x => x.status == requestModel.status && userlist.Contains(x.uuid))
                        .Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).ToList();
                }
                else if (userlist.Count > 0)
                {
                    //Inactive Operator will not show in User List
                    users = context.User.Where(x => 
                         (
                          x.Userroles.Any(x => (x.status == (int)Status.Active)) ||
                         (x.status == (int)Status.Active && x.Userroles.Any(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active)))

                         //x.Userroles.Any(x=>x.Role.name == GlobalConstants.Operator && x.status != (int)Status.Active)
                         )
                         && 
                         userlist.Contains(x.uuid))
                        .Include(x => x.Userroles).ThenInclude(x => x.Role)
                        .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();

                    //users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                    //        .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
            }
            else if (userrole == GlobalConstants.Executive)
            {
                List<Guid> userlist = new List<Guid>();

                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (requestModel.site_id?.Count > 0)
                    {
                        usersite = usersite.Where(x => requestModel.site_id.Contains(x.ToString())).ToList();
                    }
                }
                else
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (usersite.Count > 0)
                {
                    //Executive can view the data of Company Admin, Executive, Manager, Operator and MS
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                        .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }

                if (userlist.Count > 0 && requestModel.status > 0)
                {
                    //Inactive Operator will not show in User List
                    //if (requestModel.status == (int)Status.Active)
                    //{
                    //    users = context.User.Where(x => ((x.status == requestModel.status && x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator && x.status == (int)Status.Active))) ||
                    //     (x.status == (int)Status.Active && x.Userroles.All(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active)))) && userlist.Contains(x.uuid))
                    //    .Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).ToList();
                    //}
                    //else
                    //{
                    //    users = context.User.Where(x => (x.status == requestModel.status && x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator && x.status == (int)Status.Active))) && userlist.Contains(x.uuid))
                    //    .Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).ToList();
                    //}

                    users = context.User.Where(x => x.status == requestModel.status && userlist.Contains(x.uuid))
                       .Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).ToList();
                }
                else if (userlist.Count > 0)
                {
                    //Inactive Operator will not show in User List
                    users = context.User.Where(x => (x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator && x.status == (int)Status.Active)) ||
                         (x.status == (int)Status.Active && x.Userroles.All(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active)))) && userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();

                    //users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                   //       .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
            }

            var com_map = context.CompanyFeatureMappings
                .Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id
                && x.feature_id.ToString() == GlobalConstants.hide_egalvanic_users_feature_id).FirstOrDefault();

            if (userrole != GlobalConstants.Admin && com_map != null && com_map.is_required)
                users = users.Where(x => !String.IsNullOrEmpty(x.email) && !x.email.Contains("egalvanic")).ToList();
            
            if (requestModel.role_id != null && requestModel.role_id.Count > 0)
            {
                users = users.Where(x => x.Userroles.Any(x => requestModel.role_id.Contains(x.role_id.ToString()) && x.status == (int)Status.Active) && x.is_email_verified).ToList();
            }

            if (requestModel.site_id?.Count > 0)
            {
                if (userrole == GlobalConstants.Admin || userrole == GlobalConstants.CompanyAdmin)
                {
                    users = users.Where(x => x.Usersites.Any(x => x.status == (int)Status.Active && requestModel.site_id.Contains(x.site_id.ToString()))).ToList();
                }
            }

            if (requestModel.company_id?.Count > 0)
            {
                if (userrole == GlobalConstants.Admin)
                {
                    users = users.Where(x => x.Usersites.Any(x => x.status == (int)Status.Active && requestModel.company_id.Contains(x.Sites.company_id.ToString()))).ToList();
                }
            }


            if (!String.IsNullOrEmpty(requestModel.search_string))
            {
                requestModel.search_string = requestModel.search_string.ToLower();
                users = users.Where(x => (x.StatusMaster.status_name.ToLower().Contains(requestModel.search_string) ||
                        x.Userroles.Any(x => x.Role.name.ToLower().Contains(requestModel.search_string) && x.status == (int)Status.Active) || (!String.IsNullOrEmpty(x.email) ? x.email.ToLower().Contains(requestModel.search_string) : false) || x.username.ToLower().Contains(requestModel.search_string))).ToList();
            }
            users = users.Where(x => x.status == (int)Status.Active || x.status == (int)Status.Deactive).ToList();
            return users.OrderBy(x => x.username).ToList();
        }



        public List<User> FilterUsersOptimized(FilterUsersRequestModel requestModel)
        {
            List<User> users = new List<User>();
            //return context.User.Include(x => x.Usersites).ToList();
            string userrole = null;
            if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id) && UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                userrole = context.UserRoles.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();
            }
            // if user active role is Admin then show all role users 
            if (userrole == GlobalConstants.Admin)
            {
                List<Guid> userlist = new List<Guid>();
                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.Active)
                {
                    var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                    if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                    {
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id)
                        && x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))))
                       //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster)
                       .Select(x => x.user_id).Distinct().ToList();
                    }
                    else
                    {
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                        x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))))
                       //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster)
                       .Select(x => x.user_id).Distinct().ToList();
                    }
                    if (userlist.Count > 0)
                    {
                        users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                                //.Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company)
                                .Include(x => x.Userroles).Include(x => x.Usersites).ToList();
                    }
                }
                else if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.AllCompanyType)
                {
                    if (UpdatedGenericRequestmodel.CurrentUser.site_status != (int)Status.AllSiteType)
                    {
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                        x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))))
                       //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster)
                       .Select(x => x.user_id).Distinct().ToList();
                        users = context.User.Where(x => userlist.Contains(x.uuid))
                            //.Include(x => x.Userroles).ThenInclude(x => x.Role)
                            //.Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company)
                            .ToList();
                    }
                    else
                    {
                        users = context.User.Include(x => x.Userroles).ThenInclude(x => x.Role)
                            //.Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company)
                            .ToList();
                    }
                }
                else
                {
                    users = context.User.ToList();
                    //.Include(x => x.Userroles).ThenInclude(x => x.Role)
                    //.Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company)

                }
                if (requestModel.status > 0)
                {
                    users = users.Where(x => x.status == requestModel.status).ToList();
                }
            }
            else if (userrole == GlobalConstants.CompanyAdmin)
            {
                List<Guid> userlist = new List<Guid>();
                var usersite = new List<Guid>();

                var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id).Include(x => x.Company).ToList();//&& x.status == (int)Status.Active
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    // Company admin can get data for users excluding Company Admin
                    // userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) &&
                    // x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    // && (x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive))))
                    //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                    // Company admin can get data for users including Company Admin
                    userlist = context.UserSites.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && //x.status == (int)Status.Active &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                    //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster)
                    .Select(x => x.user_id).Distinct().ToList();
                }
                else
                {
                    // Company admin can get data for users excluding Company Admin
                    // userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                    // x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    // && (x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive))))
                    //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                    // Company admin can get data for users including Company Admin
                    //userlist = context.UserSites.Where(x=>  companysites.Select(x => x.site_id).ToList().Contains(x.site_id)).Include(x => x.User).Select(x => x.user_id).Distinct().ToList();
                    userlist = context.UserSites.Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && // x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id && x.status == (int)Status.Active &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                   //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster)
                   .Select(x => x.user_id).Distinct().ToList();
                }
                if (userlist.Count > 0)
                {
                    //Inactive Operator will not show in User List
                    users = context.User.Where(x => (x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator)) ||
                         (x.status == (int)Status.Active && x.Userroles.All(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active)))) && userlist.Contains(x.uuid))
                           //.Include(x => x.Userroles).ThenInclude(x => x.Role)
                           //.Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company)
                           .Include(x => x.Userroles).Include(x => x.Usersites).ToList();

                    //users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                    //        .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }

                if (requestModel.status > 0)
                {
                    //Inactive Operator will not show in User List
                    if (requestModel.status == (int)Status.Active)
                    {
                        users = users.Where(x => ((x.status == requestModel.status && x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator && x.status == (int)Status.Active))) ||
                         (x.status == (int)Status.Active && x.Userroles.All(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active))))).ToList();
                    }
                    else
                    {
                        users = users.Where(x => (x.status == requestModel.status && x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator && x.status == (int)Status.Active)))).ToList();
                    }

                    //users = users.Where(x => x.status == requestModel.status).ToList();
                }
            }
            else if (userrole == GlobalConstants.Manager)
            {
                List<Guid> userlist = new List<Guid>();

                // if user active role is Manager then show Manager, Operator and MS Role users
                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (requestModel.site_id?.Count > 0)
                    {
                        usersite = usersite.Where(x => requestModel.site_id.Contains(x.ToString())).ToList();
                    }
                }
                else
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (usersite.Count > 0)
                {
                    //Manager can view the data of Manager, Operator and MS
                    //userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) &&
                    //x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive)) &&
                    //(x.Role.name != GlobalConstants.Executive || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Deactive)) &&
                    //(x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive))))
                    //    .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                    //Manager can get the data of Company Admin, Executive, Manager, Operator and MS
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                        //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster)
                        .Select(x => x.user_id).Distinct().ToList();
                }

                if (userlist.Count > 0 && requestModel.status > 0)
                {
                    //Inactive Operator will not show in User List
                    if (requestModel.status == (int)Status.Active)
                    {
                        users = context.User.Where(x => ((x.status == requestModel.status && x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator && x.status == (int)Status.Active))) ||
                         (x.status == (int)Status.Active && x.Userroles.All(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active)))) && userlist.Contains(x.uuid))
                        .Include(x=>x.Userroles).Include(x=>x.Usersites)
                            //.Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site)
                        .ToList();
                    }
                    else
                    {
                        users = context.User.Where(x => (x.status == requestModel.status && x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator && x.status == (int)Status.Active))) && userlist.Contains(x.uuid))
                        //.Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site)
                        .ToList();
                    }

                    //users = context.User.Where(x => x.status == requestModel.status && userlist.Contains(x.uuid))
                    //    .Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).ToList();
                }
                else if (userlist.Count > 0)
                {
                    //Inactive Operator will not show in User List
                    users = context.User.Where(x =>
                         (
                          x.Userroles.Any(x => (x.status == (int)Status.Active)) ||
                         (x.status == (int)Status.Active && x.Userroles.Any(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active)))

                         //x.Userroles.Any(x=>x.Role.name == GlobalConstants.Operator && x.status != (int)Status.Active)
                         )
                         &&
                         userlist.Contains(x.uuid))
                        //.Include(x => x.Userroles).ThenInclude(x => x.Role)
                        //.Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company)
                        .ToList();

                    //users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                    //        .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
            }
            else if (userrole == GlobalConstants.Executive)
            {
                List<Guid> userlist = new List<Guid>();

                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (requestModel.site_id?.Count > 0)
                    {
                        usersite = usersite.Where(x => requestModel.site_id.Contains(x.ToString())).ToList();
                    }
                }
                else
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (usersite.Count > 0)
                {
                    //Executive can view the data of Company Admin, Executive, Manager, Operator and MS
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                        //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster)
                        .Select(x => x.user_id).Distinct().ToList();
                }

                if (userlist.Count > 0 && requestModel.status > 0)
                {
                    //Inactive Operator will not show in User List
                    if (requestModel.status == (int)Status.Active)
                    {
                        users = context.User.Where(x => ((x.status == requestModel.status && x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator && x.status == (int)Status.Active))) ||
                         (x.status == (int)Status.Active && x.Userroles.All(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active)))) && userlist.Contains(x.uuid))
                        //.Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site)
                        .ToList();
                    }
                    else
                    {
                        users = context.User.Where(x => (x.status == requestModel.status && x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator && x.status == (int)Status.Active))) && userlist.Contains(x.uuid))
                        //.Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site)
                        .ToList();
                    }

                    //users = context.User.Where(x => x.status == requestModel.status && userlist.Contains(x.uuid))
                    //    .Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).ToList();
                }
                else if (userlist.Count > 0)
                {
                    //Inactive Operator will not show in User List
                    users = context.User.Where(x => (x.Userroles.All(x => (x.Role.name != GlobalConstants.Operator && x.status == (int)Status.Active)) ||
                         (x.status == (int)Status.Active && x.Userroles.All(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active)))) && userlist.Contains(x.uuid))
                            //.Include(x => x.Userroles).ThenInclude(x => x.Role)
                            //.Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company)
                            .Include(x => x.Userroles).Include(x => x.Usersites).ToList();

                    //users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                    //       .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
            }

            var com_map = context.CompanyFeatureMappings
                .Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id
                && x.feature_id.ToString() == GlobalConstants.hide_egalvanic_users_feature_id).FirstOrDefault();

            if (userrole != GlobalConstants.Admin && com_map != null && com_map.is_required)
                users = users.Where(x => !x.email.Contains("egalvanic")).ToList();

            if (requestModel.role_id != null && requestModel.role_id.Count > 0)
            {
                users = users.Where(x => x.Userroles.Any(x => requestModel.role_id.Contains(x.role_id.ToString()) && x.status == (int)Status.Active) && x.is_email_verified).ToList();
            }

            if (requestModel.site_id?.Count > 0)
            {
                if (userrole == GlobalConstants.Admin || userrole == GlobalConstants.CompanyAdmin)
                {
                    users = users.Where(x => x.Usersites.Any(x => x.status == (int)Status.Active && requestModel.site_id.Contains(x.site_id.ToString()))).ToList();
                }
            }

            if (requestModel.company_id?.Count > 0)
            {
                if (userrole == GlobalConstants.Admin)
                {
                    users = users.Where(x => x.Usersites.Any(x => x.status == (int)Status.Active && requestModel.company_id.Contains(x.Sites.company_id.ToString()))).ToList();
                }
            }


            if (!String.IsNullOrEmpty(requestModel.search_string))
            {
                requestModel.search_string = requestModel.search_string.ToLower();
                users = users.Where(x => (x.StatusMaster.status_name.ToLower().Contains(requestModel.search_string) ||
                        x.Userroles.Any(x => x.Role.name.ToLower().Contains(requestModel.search_string) && x.status == (int)Status.Active) || (!String.IsNullOrEmpty(x.email) ? x.email.ToLower().Contains(requestModel.search_string) : false) || x.username.ToLower().Contains(requestModel.search_string))).ToList();
            }
            users = users.Where(x => x.status == (int)Status.Active || x.status == (int)Status.Deactive).ToList();
            return users.OrderBy(x => x.username).ToList();
        }


        public GetAllTechniciansListResponseModel GetAllTechniciansList()
        {
            GetAllTechniciansListResponseModel response = new GetAllTechniciansListResponseModel();

            var userid_list = context.UserSites.Where(x=>x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) 
                    && x.status == (int)Status.Active)
                        .Select(x=>x.user_id).Distinct().ToList();

            var tech_userid_list = context.UserRoles.Where(x => x.role_id == Guid.Parse(GlobalConstants.Technician_Role_id)
                && userid_list.Contains(x.user_id) && x.status == (int)Status.Active)
                    .Select(x=>x.user_id).Distinct().ToList();

            IQueryable<User> tech_users = context.User.Where(x => tech_userid_list.Contains(x.uuid) && x.is_email_verified && x.status == (int)Status.Active);

            response.list = tech_users
              .Select(x => new GetAllUserData_Class
              {
                  uuid = x.uuid,
                  firstname = x.firstname,
                  lastname = x.lastname,
                  email = x.email
              }).ToList();

            return response;
        }

        public GetAllTechniciansListResponseModel GetAllTechniciansListBySiteId(Guid site_id)
        {
            GetAllTechniciansListResponseModel response = new GetAllTechniciansListResponseModel();

            var site = context.Sites.Where(x=>x.site_id==site_id).FirstOrDefault();
            var site_id_list = context.Sites.Where(x=>x.company_id == site.company_id && x.status==(int)Status.Active).Select(x => x.site_id).ToList();
            
            var userid_list = context.UserSites.Where(x => site_id_list.Contains(x.site_id) && x.status == (int)Status.Active)
                        .Select(x => x.user_id).Distinct().ToList();

            var tech_userid_list = context.UserRoles.Where(x => x.role_id == Guid.Parse(GlobalConstants.Technician_Role_id)
                && userid_list.Contains(x.user_id) && x.status == (int)Status.Active)
                    .Select(x => x.user_id).Distinct().ToList();

            IQueryable<User> tech_users = context.User.Where(x => tech_userid_list.Contains(x.uuid) && x.is_email_verified && x.status == (int)Status.Active);

            response.list = tech_users
              .Select(x => new GetAllUserData_Class
              {
                  uuid = x.uuid,
                  firstname = x.firstname,
                  lastname = x.lastname,
                  email = x.email,
                  is_curr_site_user = context.UserSites.Where(z=>z.site_id==site_id && z.user_id==x.uuid && z.status == (int)Status.Active).FirstOrDefault() != null ? true : false
              }).ToList();

            return response;
        }

        public GetAllTechniciansListResponseModel GetAllBackOfficeUsersList()
        {
            GetAllTechniciansListResponseModel response = new GetAllTechniciansListResponseModel();

            var userid_list = context.UserSites.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)
                    && x.status == (int)Status.Active)
                        .Select(x => x.user_id).Distinct().ToList();

            var bo_userid_list = context.UserRoles.Where(x => x.role_id == Guid.Parse(GlobalConstants.BackOffice_Role_Id)
                && userid_list.Contains(x.user_id) && x.status == (int)Status.Active)
                    .Select(x => x.user_id).Distinct().ToList();

            IQueryable<User> bo_users = context.User.Where(x => bo_userid_list.Contains(x.uuid) && x.is_email_verified && x.status == (int)Status.Active);

            response.list = bo_users
              .Select(x => new GetAllUserData_Class
              {
                  uuid = x.uuid,
                  firstname = x.firstname,
                  lastname = x.lastname,
                  email = x.email
              }).ToList();

            return response;
        }
        public GetAllTechniciansListResponseModel GetAllBackOfficeUsersListBySiteId(Guid site_id)
        {
            GetAllTechniciansListResponseModel response = new GetAllTechniciansListResponseModel();

            var site = context.Sites.Where(x => x.site_id == site_id).FirstOrDefault();
            var site_id_list = context.Sites.Where(x => x.company_id == site.company_id && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

            var userid_list = context.UserSites.Where(x => site_id_list.Contains(x.site_id) && x.status == (int)Status.Active)
                        .Select(x => x.user_id).Distinct().ToList();

            var bo_userid_list = context.UserRoles.Where(x => x.role_id == Guid.Parse(GlobalConstants.BackOffice_Role_Id)
                && userid_list.Contains(x.user_id) && x.status == (int)Status.Active)
                    .Select(x => x.user_id).Distinct().ToList();

            IQueryable<User> bo_users = context.User.Where(x => bo_userid_list.Contains(x.uuid) && x.is_email_verified && x.status == (int)Status.Active);

            response.list = bo_users
              .Select(x => new GetAllUserData_Class
              {
                  uuid = x.uuid,
                  firstname = x.firstname,
                  lastname = x.lastname,
                  email = x.email,
                  is_curr_site_user = context.UserSites.Where(z => z.site_id == site_id && z.user_id == x.uuid && z.status == (int)Status.Active).FirstOrDefault() != null ? true : false
              }).ToList();

            return response;
        }

        public List<User> FilterUsersRoleOptions(FilterUsersRequestModel requestModel)
        {
            List<User> users = new List<User>();
            //return context.User.Include(x => x.Usersites).ToList();
            string userrole = null;
            if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id) && UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                userrole = context.UserRoles.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();
            }
            // if user active role is Admin then show all role users 
            if (userrole == GlobalConstants.Admin)
            {
                List<Guid> userlist = new List<Guid>();
                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.Active)
                {
                    var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                    if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                    {
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id)
                        && x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))))
                       .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                    }
                    else
                    {
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                        x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))))
                       .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                    }
                    if (userlist.Count > 0)
                    {
                        users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                                .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                    }
                }
                else if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.AllCompanyType)
                {
                    if (UpdatedGenericRequestmodel.CurrentUser.site_status != (int)Status.AllSiteType)
                    {
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                        x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))))
                       .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                        users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                    }
                    else
                    {
                        users = context.User.Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                    }
                }
                else
                {
                    users = context.User.Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
                if (requestModel.status > 0)
                {
                    users = users.Where(x => x.status == requestModel.status).ToList();
                }
            }
            else if (userrole == GlobalConstants.CompanyAdmin)
            {
                List<Guid> userlist = new List<Guid>();
                var usersite = new List<Guid>();

                var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    // Company admin can get data for users excluding Company Admin
                    // userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) &&
                    // x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    // && (x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive))))
                    //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                    // Company admin can get data for users including Company Admin
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                    .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }
                else
                {
                    // Company admin can get data for users excluding Company Admin
                    // userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                    // x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    // && (x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive))))
                    //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                    // Company admin can get data for users including Company Admin
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) &&  //&& x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                   .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }
                if (userlist.Count > 0)
                {
                    users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }

                if (requestModel.status > 0)
                {
                    users = users.Where(x => x.status == requestModel.status).ToList();
                }
            }
            else if (userrole == GlobalConstants.Manager)
            {
                List<Guid> userlist = new List<Guid>();

                // if user active role is Manager then show Manager, Operator and MS Role users
                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (requestModel.site_id?.Count > 0)
                    {
                        usersite = usersite.Where(x => requestModel.site_id.Contains(x.ToString())).ToList();
                    }
                }
                else
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (usersite.Count > 0)
                {
                    //Manager can only view the data of Manager, Operator and MS
                    //userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) &&
                    //x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive)) &&
                    //(x.Role.name != GlobalConstants.Executive || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Deactive)) &&
                    //(x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive))))
                    //    .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                    //Manager can view the data of Company Admin, Executive, Manager, Operator and MS
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                        .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }

                if (userlist.Count > 0 && requestModel.status > 0)
                {
                    users = context.User.Where(x => x.status == requestModel.status && userlist.Contains(x.uuid))
                        .Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).ToList();
                }
                else if (userlist.Count > 0)
                {
                    users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
            }
            else if (userrole == GlobalConstants.Executive)
            {
                List<Guid> userlist = new List<Guid>();
                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (requestModel.site_id?.Count > 0)
                    {
                        usersite = usersite.Where(x => requestModel.site_id.Contains(x.ToString())).ToList();
                    }
                }
                else
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (usersite.Count > 0)
                {
                    //Executive can view the data of Company Admin, Executive, Manager, Operator and MS
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                        .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }

                if (userlist.Count > 0 && requestModel.status > 0)
                {
                    users = context.User.Where(x => x.status == requestModel.status && userlist.Contains(x.uuid))
                        .Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).ToList();
                }
                else if (userlist.Count > 0)
                {
                    users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
            }

            if (requestModel.role_id != null && requestModel.role_id.Count > 0)
            {
                users = users.Where(x => x.Userroles.Any(x => requestModel.role_id.Contains(x.role_id.ToString()) && x.status == (int)Status.Active)).ToList();
            }

            if (requestModel.site_id?.Count > 0)
            {
                if (userrole == GlobalConstants.Admin || userrole == GlobalConstants.CompanyAdmin)
                {
                    users = users.Where(x => x.Usersites.Any(x => x.status == (int)Status.Active && requestModel.site_id.Contains(x.site_id.ToString()))).ToList();
                }
            }

            if (requestModel.company_id?.Count > 0)
            {
                if (userrole == GlobalConstants.Admin)
                {
                    users = users.Where(x => x.Usersites.Any(x => x.status == (int)Status.Active && requestModel.company_id.Contains(x.Sites.company_id.ToString()))).ToList();
                }
            }


            if (!String.IsNullOrEmpty(requestModel.search_string))
            {
                requestModel.search_string = requestModel.search_string.ToLower();
                users = users.Where(x => (x.StatusMaster.status_name.ToLower().Contains(requestModel.search_string) ||
                        x.Userroles.Any(x => x.Role.name.ToLower().Contains(requestModel.search_string) && x.status == (int)Status.Active) || (!String.IsNullOrEmpty(x.email) ? x.email.ToLower().Contains(requestModel.search_string) : false) || x.username.ToLower().Contains(requestModel.search_string))).ToList();
            }

            if (!String.IsNullOrEmpty(requestModel.option_search_string))
            {
                requestModel.search_string = requestModel.option_search_string.ToLower();
                users = users.Where(x => x.Userroles.Any(x => x.Role.name.ToLower().Contains(requestModel.search_string) && x.status == (int)Status.Active)).ToList();
            }

            return users.OrderBy(x => x.username).ToList();
        }

        public List<User> FilterUsersSitesOptions(FilterUsersRequestModel requestModel)
        {
            List<User> users = new List<User>();
            //return context.User.Include(x => x.Usersites).ToList();
            string userrole = null;
            if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id) && UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                userrole = context.UserRoles.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();
            }
            // if user active role is Admin then show all role users 
            if (userrole == GlobalConstants.Admin)
            {
                List<Guid> userlist = new List<Guid>();
                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.Active)
                {
                    var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                    if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                    {
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id)
                        && x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))))
                       .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                    }
                    else
                    {
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                        x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))))
                       .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                    }
                    if (userlist.Count > 0)
                    {
                        users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                                .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                    }
                }
                else if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.AllCompanyType)
                {
                    if (UpdatedGenericRequestmodel.CurrentUser.site_status != (int)Status.AllSiteType)
                    {
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                        x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))))
                       .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                        users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                    }
                    else
                    {
                        users = context.User.Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                    }
                }
                else
                {
                    users = context.User.Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
                if (requestModel.status > 0)
                {
                    users = users.Where(x => x.status == requestModel.status).ToList();
                }
            }
            else if (userrole == GlobalConstants.CompanyAdmin)
            {
                List<Guid> userlist = new List<Guid>();
                var usersite = new List<Guid>();

                var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    // Company admin can get data for users excluding Company Admin
                    // userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) &&
                    // x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    // && (x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive))))
                    //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                    // Company admin can get data for users including Company Admin
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                   .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }
                else
                {
                    // Company admin can get data for users excluding Company Admin
                    // userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                    // x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    // && (x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive))))
                    //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                    // Company admin can get data for users including Company Admin
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                   .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }
                if (userlist.Count > 0)
                {
                    users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }

                if (requestModel.status > 0)
                {
                    users = users.Where(x => x.status == requestModel.status).ToList();
                }
            }
            else if (userrole == GlobalConstants.Manager)
            {
                List<Guid> userlist = new List<Guid>();

                // if user active role is Manager then show Manager, Operator and MS Role users
                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (requestModel.site_id?.Count > 0)
                    {
                        usersite = usersite.Where(x => requestModel.site_id.Contains(x.ToString())).ToList();
                    }
                }
                else
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (usersite.Count > 0)
                {
                    //Manager can view the data of Manager, Operator and MS
                    //userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) &&
                    //x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive)) &&
                    //(x.Role.name != GlobalConstants.Executive || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Deactive)) &&
                    //(x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive))))
                    //    .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                    //Manager can view the data of Company Admin, Executive, Manager, Operator and MS
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                        .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }

                if (userlist.Count > 0 && requestModel.status > 0)
                {
                    users = context.User.Where(x => x.status == requestModel.status && userlist.Contains(x.uuid))
                        .Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).ToList();
                }
                else if (userlist.Count > 0)
                {
                    users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
            }
            else if (userrole == GlobalConstants.Executive)
            {
                List<Guid> userlist = new List<Guid>();
                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (requestModel.site_id?.Count > 0)
                    {
                        usersite = usersite.Where(x => requestModel.site_id.Contains(x.ToString())).ToList();
                    }
                }
                else
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (usersite.Count > 0)
                {
                    //Executive can view the data of Company Admin, Executive, Manager, Operator and MS
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                    .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }

                if (userlist.Count > 0 && requestModel.status > 0)
                {
                    users = context.User.Where(x => x.status == requestModel.status && userlist.Contains(x.uuid))
                        .Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).ToList();
                }
                else if (userlist.Count > 0)
                {
                    users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
            }

            if (requestModel.role_id != null && requestModel.role_id.Count > 0)
            {
                users = users.Where(x => x.Userroles.Any(x => requestModel.role_id.Contains(x.role_id.ToString()) && x.status == (int)Status.Active)).ToList();
            }

            if (requestModel.site_id?.Count > 0)
            {
                if (userrole == GlobalConstants.Admin || userrole == GlobalConstants.CompanyAdmin)
                {
                    users = users.Where(x => x.Usersites.Any(x => x.status == (int)Status.Active && requestModel.site_id.Contains(x.site_id.ToString()))).ToList();
                }
            }

            if (requestModel.company_id?.Count > 0)
            {
                if (userrole == GlobalConstants.Admin)
                {
                    users = users.Where(x => x.Usersites.Any(x => x.status == (int)Status.Active && requestModel.company_id.Contains(x.Sites.company_id.ToString()))).ToList();
                }
            }


            if (!String.IsNullOrEmpty(requestModel.search_string))
            {
                requestModel.search_string = requestModel.search_string.ToLower();
                users = users.Where(x => (x.StatusMaster.status_name.ToLower().Contains(requestModel.search_string) ||
                        x.Userroles.Any(x => x.Role.name.ToLower().Contains(requestModel.search_string) && x.status == (int)Status.Active) || (!String.IsNullOrEmpty(x.email) ? x.email.ToLower().Contains(requestModel.search_string) : false) || x.username.ToLower().Contains(requestModel.search_string))).ToList();
            }

            if (!String.IsNullOrEmpty(requestModel.option_search_string))
            {
                requestModel.search_string = requestModel.option_search_string.ToLower();
                users = users.Where(x => x.Usersites.Any(x => x.Sites.site_name.ToLower().Contains(requestModel.search_string) && x.status == (int)Status.Active) ||
                x.Usersites.Any(x => x.Sites.site_code.ToLower().Contains(requestModel.search_string) && x.status == (int)Status.Active)).ToList();
            }

            return users.OrderBy(x => x.username).ToList();
        }

        public List<User> FilterUsersCompanyOptions(FilterUsersRequestModel requestModel)
        {
            List<User> users = new List<User>();
            //return context.User.Include(x => x.Usersites).ToList();
            string userrole = null;
            if (!String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id) && UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                userrole = context.UserRoles.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();
            }
            // if user active role is Admin then show all role users 
            if (userrole == GlobalConstants.Admin)
            {
                List<Guid> userlist = new List<Guid>();
                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.Active)
                {
                    var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                    if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                    {
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id)
                        && x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))))
                       .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                    }
                    else
                    {
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                        x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))))
                       .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                    }
                    if (userlist.Count > 0)
                    {
                        users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                                .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                    }
                }
                else if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.AllCompanyType)
                {
                    if (UpdatedGenericRequestmodel.CurrentUser.site_status != (int)Status.AllSiteType)
                    {
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                        x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))))
                       .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                        users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                    }
                    else
                    {
                        users = context.User.Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                    }
                }
                else
                {
                    users = context.User.Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
                if (requestModel.status > 0)
                {
                    users = users.Where(x => x.status == requestModel.status).ToList();
                }
            }
            else if (userrole == GlobalConstants.CompanyAdmin)
            {
                List<Guid> userlist = new List<Guid>();
                var usersite = new List<Guid>();

                var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    // Company admin can get data for users excluding Company Admin
                    // userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) &&
                    // x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    // && (x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive))))
                    //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                    // Company admin can get data for users including Company Admin
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                   .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }
                else
                {
                    // Company admin can get data for users excluding Company Admin
                    // userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                    // x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    // && (x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive))))
                    //.Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                    // Company admin can get data for users including Company Admin
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                   .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }
                if (userlist.Count > 0)
                {
                    users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }

                if (requestModel.status > 0)
                {
                    users = users.Where(x => x.status == requestModel.status).ToList();
                }
            }
            else if (userrole == GlobalConstants.Manager)
            {
                List<Guid> userlist = new List<Guid>();

                // if user active role is Manager then show Manager, Operator and MS Role users
                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (requestModel.site_id?.Count > 0)
                    {
                        usersite = usersite.Where(x => requestModel.site_id.Contains(x.ToString())).ToList();
                    }
                }
                else
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (usersite.Count > 0)
                {
                    //Manager can only view the data of Company Manager, Operator and MS
                    //userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) &&
                    //x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive)) &&
                    //(x.Role.name != GlobalConstants.Executive || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Deactive)) &&
                    //(x.Role.name != GlobalConstants.CompanyAdmin || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Deactive))))
                    //    .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                    //Manager can view the data of Company Admin, Executive, Manager, Operator and MS
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                        .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }

                if (userlist.Count > 0 && requestModel.status > 0)
                {
                    users = context.User.Where(x => x.status == requestModel.status && userlist.Contains(x.uuid))
                        .Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).ToList();
                }
                else if (userlist.Count > 0)
                {
                    users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
            }
            else if (userrole == GlobalConstants.Executive)
            {
                List<Guid> userlist = new List<Guid>();
                var usersite = new List<Guid>();
                if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (requestModel.site_id?.Count > 0)
                    {
                        usersite = usersite.Where(x => requestModel.site_id.Contains(x.ToString())).ToList();
                    }
                }
                else
                {
                    usersite = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                if (usersite.Count > 0)
                {
                    //Executive can view the data of Company Admin, Executive, Manager, Operator and MS
                    userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersite.Contains(x.site_id) &&
                    x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive))
                    || (x.Role.name == GlobalConstants.CompanyAdmin && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)
                    || (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)))
                        .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();
                }

                if (userlist.Count > 0 && requestModel.status > 0)
                {
                    users = context.User.Where(x => x.status == requestModel.status && userlist.Contains(x.uuid))
                        .Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).ToList();
                }
                else if (userlist.Count > 0)
                {
                    users = context.User.Where(x => userlist.Contains(x.uuid)).Include(x => x.Userroles).ThenInclude(x => x.Role)
                            .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.StatusMaster).Include(x => x.Role_App).Include(x => x.Role_Web).Include(x => x.Site).Include(x => x.Company).Include(x => x.Active_Company).ToList();
                }
            }

            if (requestModel.role_id != null && requestModel.role_id.Count > 0)
            {
                users = users.Where(x => x.Userroles.Any(x => requestModel.role_id.Contains(x.role_id.ToString()) && x.status == (int)Status.Active)).ToList();
            }

            if (requestModel.site_id?.Count > 0)
            {
                if (userrole == GlobalConstants.Admin || userrole == GlobalConstants.CompanyAdmin)
                {
                    users = users.Where(x => x.Usersites.Any(x => x.status == (int)Status.Active && requestModel.site_id.Contains(x.site_id.ToString()))).ToList();
                }
            }

            if (requestModel.company_id?.Count > 0)
            {
                if (userrole == GlobalConstants.Admin)
                {
                    users = users.Where(x => x.Usersites.Any(x => x.status == (int)Status.Active && requestModel.company_id.Contains(x.Sites.company_id.ToString()))).ToList();
                }
            }


            if (!String.IsNullOrEmpty(requestModel.search_string))
            {
                requestModel.search_string = requestModel.search_string.ToLower();
                users = users.Where(x => (x.StatusMaster.status_name.ToLower().Contains(requestModel.search_string) ||
                        x.Userroles.Any(x => x.Role.name.ToLower().Contains(requestModel.search_string) && x.status == (int)Status.Active) || (!String.IsNullOrEmpty(x.email) ? x.email.ToLower().Contains(requestModel.search_string) : false) || x.username.ToLower().Contains(requestModel.search_string))).ToList();
            }

            if (!String.IsNullOrEmpty(requestModel.option_search_string))
            {
                requestModel.search_string = requestModel.option_search_string.ToLower();
                users = users.Where(x => x.Usersites.Any(x => x.Sites.Company.company_name.ToLower().Contains(requestModel.search_string) && x.status == (int)Status.Active) ||
                x.Usersites.Any(x => x.Sites.Company.company_code.ToLower().Contains(requestModel.search_string) && x.status == (int)Status.Active)).ToList();
            }

            return users.OrderBy(x => x.username).ToList();
        }

        public async Task<User> GetUserByID(string uuid)
        {
            var usersites = context.UserSites.Where(x => x.user_id.ToString() == uuid && x.status == (int)Status.Active).Include(x => x.Sites).ToList();
            var User = await context.User.Where(x => x.uuid.ToString() == uuid).Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.UserEmailNotificationConfigurationSettings).FirstOrDefaultAsync();
            if (User != null)
            {
                User.Usersites = usersites;

                User.Usersites.ToList().ForEach(x => x.Sites = context.Sites.Where(y => y.site_id == x.site_id).Include(z => z.Company).FirstOrDefault());
            }
            return User;
        }
        public async Task<User> GetUserByIDForverify(string email,string company_id)
        {
            var User = await context.User.Where(x => x.email == email && x.ac_active_company == Guid.Parse(company_id)).FirstOrDefaultAsync();
            return User;
        }
        public async Task<User> UserLogin(LoginRequestModel request)
        {
            try
            {
                User response = new User();
                User user = null;
                if (request.barcodeId == String.Empty || request.barcodeId == null)
                {
                    if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.token))
                    {
                        user = context.User.Where(x => x.username.ToLower() == request.username.ToLower() && x.status == (int)Status.Active && x.Site.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)).FirstOrDefault();
                    }
                    else
                    {
                        user = context.User.Where(x => x.username.ToLower() == request.username.ToLower() && x.password == request.password && x.status == (int)Status.Active && x.ac_active_company == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)).FirstOrDefault();
                    }
                }
                else
                {
                    Guid guidResult = Guid.Parse(request.barcodeId);
                    var barcodeuserlist = context.UserRoles.Include(x => x.Role).Where(x => x.status == (int)Status.Active && (x.Role.name == GlobalConstants.MS || x.Role.name == GlobalConstants.Operator)).Select(x => x.user_id).ToList();
                    user = context.User.Where(x => x.barcode_id == guidResult && barcodeuserlist.Contains(x.uuid) && x.status == (int)Status.Active && x.ac_active_company == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)).FirstOrDefault();
                }
                if (user != null)
                {
                    response = context.User.Include(x => x.Usersites).ThenInclude(x => x.Sites).ThenInclude(x => x.Company)
                        .Where(z => z.uuid == user.uuid )
                                .Include(x => x.Userroles).ThenInclude(x => x.Role)
                                .Include(x => x.LanguageMaster)
                                .Include(x => x.Site)
                                .Include(x => x.UserEmailNotificationConfigurationSettings)
                                .Include(x => x.Company).Include(x => x.Active_Company)
                                .Include(x=>x.Company.ClientCompany)
                                .ThenInclude(x=>x.Sites)
                                .FirstOrDefault();
                    if ((response.uuid != null || response.uuid == Guid.Empty))
                    {
                        user.uuid = response.uuid;
                        if ((request.notification_token != null && request.notification_token != String.Empty))
                        {
                            user.notification_token = request.notification_token;
                        }
                        user.os = request.os;
                        user.ac_active_role_app = user.ac_default_role_app;
                        user.ac_active_role_web = user.ac_default_role_web;
                        user.ac_active_site = user.ac_default_site;
                        user.ac_active_company = user.ac_default_company;
                        user.ac_active_client_company = user.ac_default_client_company;
                        context.Update(user);
                        await context.SaveChangesAsync();
                        /*if (response.Usersites.Where(x => x.status == (int)Status.Active).ToList()?.Count > 1 || response.Userroles.All(x => x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Active))
                        {
                            var allsiteId = context.Sites.Where(x => x.status == (int)Status.AllSiteType).FirstOrDefault();
                            if (allsiteId != null && allsiteId.site_id != null && Guid.Empty != allsiteId.site_id)
                            {
                                if (response.Usersites == null)
                                {
                                    response.Usersites = new List<UserSites>();
                                }
                                UserSites usersite = new UserSites();
                                usersite.site_id = allsiteId.site_id;
                                usersite.Sites = allsiteId;
                                usersite.status = allsiteId.status.Value;
                                usersite.company_id = response.Usersites.Select(x => x.company_id).FirstOrDefault();
                                usersite.user_id = response.uuid;
                                response.Usersites.Add(usersite);
                            }
                        }*/
                    }
                }
                return response;
            }
            catch (Exception e)
            {
                Logger.Log("UserLogin Respository : " + e.Message.ToString());
                throw e;
            }
        }

        public User GetUserSessionDetails(string uuid)
        {
            try
            {
                User response = new User();
                User user = null;
                user = context.User.Where(x => x.uuid.ToString() == uuid && x.status == (int)Status.Active).FirstOrDefault();
                if (user != null)
                {
                    response = context.User.Where(z => z.uuid == user.uuid)
                                .Include(x => x.Userroles).ThenInclude(x => x.Role)
                                .Include(x => x.Usersites).ThenInclude(x=>x.Sites)
                                .Include(x => x.LanguageMaster)
                                .Include(x => x.Active_Role_App).Include(x => x.Active_Role_Web)
                                .Include(x => x.Active_Site)
                                .Include(x => x.Site)
                                .Include(x => x.Company)
                                .Include(x => x.Company.ClientCompany)
                                .Include(x => x.Active_Company).FirstOrDefault();
                }
                return response;
            }
            catch (Exception e)
            {
                Logger.Log("UserSession Respository : " + e.Message.ToString());
                throw e;
            }
        }

        public User GetUserSessionDetailsOptimized(string uuid)
        {
            try
            {
                User response = new User();
                User user = null;
                user = context.User.Where(x => x.uuid.ToString() == uuid && x.status == (int)Status.Active).FirstOrDefault();
                if (user != null)
                {
                    response = context.User.Where(z => z.uuid == user.uuid)
                                //.Include(x => x.Userroles).ThenInclude(x => x.Role)
                                //.Include(x => x.Usersites).ThenInclude(x => x.Sites)
                                //.Include(x => x.LanguageMaster)
                                .Include(x => x.Active_Role_App).Include(x => x.Active_Role_Web).Include(x=>x.Role_App)    
                                .Include(x => x.Active_Site)
                                .Include(x => x.Site)
                                .Include(x => x.Company)
                                //.Include(x => x.Company.ClientCompany)
                                .Include(x => x.Active_Company).FirstOrDefault();
                }
                return response;
            }
            catch (Exception e)
            {
                Logger.Log("UserSession Respository : " + e.Message.ToString());
                throw e;
            }
        }
        public UserRoles activeRoleCheck(Guid uuid, string active_rolename_web)
        {
            return context.UserRoles.Where(x=>x.user_id == uuid && x.role_id.ToString() == active_rolename_web && x.status == (int)Status.Active).FirstOrDefault();
        }
        public UserSites activeSiteCheck(Guid uuid, string active_site_id)
        {
            return context.UserSites.Where(x=>x.user_id == uuid && x.site_id.ToString() == active_site_id && x.status == (int)Status.Active).FirstOrDefault();
        }
        public async Task<User> UserBarcodeLogin(AuthenticateTokenRequestModel request)
        {
            try
            {
                User user = null;
                Guid guidResult = Guid.Parse(request.barcodeId);
                user = context.User.Where(x => x.barcode_id == guidResult && x.status == (int)Status.Active).FirstOrDefault();
                return user;
            }
            catch (Exception e)
            {
                Logger.Log("UserLogin Respository : " + e.Message.ToString());
                throw e;
            }
        }

        public async Task<User> FindUserDetails(Guid uid)
        {
            return await context.User.Where(u => u.uuid == uid)
                    .Include(x => x.Userroles).ThenInclude(x => x.Role)
                    .Include(x => x.Usersites).Include(x => x.LanguageMaster).FirstOrDefaultAsync();
        }


        public async Task<UserSites> FindUserSitesDetails(Guid usid)
        {
            return await context.UserSites.Include(x => x.Sites).Include(x => x.User).Include(x => x.Sites.Company)
                    .Where(u => u.usersite_id == usid).FirstOrDefaultAsync();
        }

        public async Task<string> GetUserRoleFromId(string userid)
        {
            string rolename = null;
            if (string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
            {
                rolename = await context.UserRoles.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(y => y.Role.name).FirstOrDefaultAsync();
            }
            return rolename;
        }

        public Sites GetSiteBySiteId(Guid site_id)
        {
            return context.Sites.Include(s => s.SiteContact).Include(x => x.Company).Where(x => x.site_id == site_id).FirstOrDefault();
        }

        public List<UserSites> GetAllManager(string ref_id)
        {
            List<UserSites> userSites = new List<UserSites>();

            var site = context.Inspection.Where(x => x.inspection_id.ToString() == ref_id).Select(x => x.site_id).FirstOrDefault();

            if (site != null || site != Guid.Empty)
            {
                userSites = context.UserSites.Include(x => x.User).Where(x => x.site_id == site && x.User.Userroles.Any(x => (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)) && x.status == (int)Status.Active).ToList();
            }
            return userSites;
        }


        public List<UserSites> GetUserSiteById(string userid)
        {
            List<UserSites> userSites = new List<UserSites>();

            userSites = context.UserSites.Include(x => x.User).Where(x => x.user_id.ToString() == userid).Take(1).ToList();

            return userSites;
        }

        public List<UserSites> GetMaintenanceStaffByIssueID(string issue_id)
        {
            var user = context.Issue.Where(x => x.issue_uuid.ToString() == issue_id).Select(x => x.modified_by).FirstOrDefault();

            return context.UserSites.Include(x => x.User).Where(x => x.user_id.ToString() == user && x.User.Userroles.Any(x => (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)) && x.User.status == (int)Status.Active).ToList();
        }

        public List<UserSites> GetAllMaintenanceStaffByIssueID(string ref_id)
        {
            List<UserSites> userSites = new List<UserSites>();

            var site = context.Issue.Where(x => x.issue_uuid.ToString() == ref_id).Select(x => x.site_id).FirstOrDefault();

            if (site != null || site != Guid.Empty)
            {
                userSites = context.UserSites.Include(x => x.User).Where(x => x.site_id == site && x.User.Userroles.Any(x => (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)) && x.status == (int)Status.Active).ToList();
            }
            return userSites;
        }

        public List<UserSites> GetOperatorForNotification(string inspection_id)
        {
            List<UserSites> userSites = new List<UserSites>();

            var operatorid = context.Inspection.Where(x => x.inspection_id.ToString() == inspection_id).Select(x => x.operator_id).FirstOrDefault();

            if (operatorid != null || operatorid != Guid.Empty)
            {
                userSites = context.UserSites.Include(x => x.User).Where(x => x.user_id == operatorid && x.status == (int)Status.Active).Take(1).ToList();
            }
            return userSites;
        }

        public List<UserSites> GetAllManagerByIssueID(string ref_id)
        {
            List<UserSites> userSites = new List<UserSites>();

            var site = context.Issue.Where(x => x.issue_uuid.ToString() == ref_id).FirstOrDefault();

            if (site != null || site.site_id != Guid.Empty)
            {
                //userSites = context.UserSites.Include(x => x.User).Include(x => x.User.Role).Where(x => x.site_id == site.site_id && x.user_id.ToString() == site.created_by && x.status == (int)Status.Active).Take(1).ToList();
                userSites = context.UserSites.Include(x => x.User).Where(x => x.site_id == site.site_id && x.User.Userroles.Any(x => (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)) && x.status == (int)Status.Active).ToList();
            }
            return userSites;
        }

        public List<UserSites> GetAllManagerByAssetPMID(string ref_id)
        {
            List<UserSites> userSites = new List<UserSites>();

            var site = context.AssetPMs.Where(x => x.asset_pm_id.ToString() == ref_id).Include(x => x.Asset.Sites).Select(x => x.Asset.Sites).FirstOrDefault();

            if (site != null || site.site_id != Guid.Empty)
            {
                //userSites = context.UserSites.Include(x => x.User).Include(x => x.User.Role).Where(x => x.site_id == site.site_id && x.user_id.ToString() == site.created_by && x.status == (int)Status.Active).Take(1).ToList();
                userSites = context.UserSites.Include(x => x.User).Where(x => x.site_id == site.site_id && x.User.Userroles.Any(x => (x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active)) && x.status == (int)Status.Active).ToList();
            }
            return userSites;
        }

        public List<UserSites> GetAllOperator(string ref_id)
        {
            List<UserSites> userSites = new List<UserSites>();

            var site = context.Inspection.Where(x => x.inspection_id.ToString() == ref_id).Select(x => x.site_id).FirstOrDefault();

            if (site != null || site != Guid.Empty)
            {
                userSites = context.UserSites.Include(x => x.User).Where(x => x.site_id == site && x.User.Userroles.Any(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active)) && x.status == (int)Status.Active).ToList();

            }
            return userSites;
        }

        public List<UserSites> GetAllOperatorByIssueID(string ref_id)
        {
            List<UserSites> userSites = new List<UserSites>();

            var site = context.Issue.Where(x => x.issue_uuid.ToString() == ref_id).Select(x => x.site_id).FirstOrDefault();

            if (site != null || site != Guid.Empty)
            {
                userSites = context.UserSites.Include(x => x.User).Where(x => x.site_id == site && x.User.Userroles.Any(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active)) && x.status == (int)Status.Active).ToList();
            }
            return userSites;
        }

        public String GetUserFromUserId(string user_id)
        {
            return context.User.Where(x => x.uuid.ToString() == user_id).Select(x => x.username).FirstOrDefault();
        }

        public List<UserSites> GetAllOperatorAndMaintenceStaff(string ref_id)
        {
            List<UserSites> userSites = new List<UserSites>();

            var site = context.Inspection.Where(x => x.inspection_id.ToString() == ref_id).Select(x => x.site_id).FirstOrDefault();

            if (site != null || site != Guid.Empty)
            {
                userSites = context.UserSites.Include(x => x.User).Where(x => x.site_id == site && (x.User.Userroles.Any(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active)) && x.status == (int)Status.Active)).ToList();
            }
            return userSites;
        }

        public bool Logout()
        {
            var user = context.User.Where(x => x.uuid.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString()).FirstOrDefault();

            if (user.uuid != null && user.uuid != Guid.Empty)
            {
                user.notification_token = string.Empty;
                user.os = string.Empty;
                context.Update(user);
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public User GetUserByID(string userid, string uuid)
        {
            User user = new User();
            var userrole = context.User.Include(x => x.Role).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).Select(x => x.Role.name).FirstOrDefault();
            if (userrole == GlobalConstants.Admin)
            {
                user = context.User.Include(x => x.Usersites).Include(x => x.Role).Include(x => x.StatusMaster).Include(x => x.LanguageMaster).Where(x => x.uuid.ToString() == uuid).FirstOrDefault();
            }
            else
            {
                var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Include(x => x.Sites).ToList();
                if (usersites.Count > 0)
                {
                    user = context.User.Include(x => x.Usersites).Include(x => x.Role).Include(x => x.StatusMaster).Include(x => x.LanguageMaster).Where(x => x.uuid.ToString() == uuid).FirstOrDefault();
                }
            }
            return user;
        }

        public User GetUserByIDFromParent(string uuid)
        {
            //User user = null;
            if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id) && UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                var userrole = context.UserRoles.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();
                if (userrole == GlobalConstants.Admin)
                {
                    return context.User.Where(x => x.uuid.ToString() == uuid).Include(x => x.Usersites).Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.StatusMaster).Include(x => x.LanguageMaster).FirstOrDefault();
                }
                else
                {
                    var usersites = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active).Include(x => x.Sites).ToList();
                    if (usersites.Count > 0)
                    {
                        return context.User.Where(x => x.uuid.ToString() == uuid).Include(x => x.Usersites).Include(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.StatusMaster).Include(x => x.LanguageMaster).FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public Guid GetBarcodeIdById(string userid)
        {
            return context.User.Where(x => x.uuid.ToString() == userid).Select(x => x.barcode_id).FirstOrDefault();
        }

        public async Task<List<Role>> GetRoles()
        {
            List<Role> roles = new List<Role>();
            if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id) && UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                var userrole = context.UserRoles.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();
                // if user's role have company admin then keep it as comapny admin.
                var does_user_have_CA = context.UserRoles.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.role_id.ToString() == "972e1960-947b-42b0-a8ea-5e23a68e9632").Select(x => x.Role.name).FirstOrDefault();
                if (!String.IsNullOrEmpty(does_user_have_CA))
                {
                    userrole = does_user_have_CA;
                }
                if (userrole == GlobalConstants.Manager)
                {
                    roles = await context.Set<Role>().Where(x => x.name != GlobalConstants.Admin && x.name != GlobalConstants.Executive && x.name != GlobalConstants.CompanyAdmin).ToListAsync();
                }
                else if (userrole == GlobalConstants.Admin)
                {
                    roles = await context.Set<Role>().ToListAsync();
                }
                else if (userrole == GlobalConstants.Executive)
                {
                    roles = await context.Set<Role>().Where(x => x.name == GlobalConstants.Executive).ToListAsync();
                }
                else if (userrole == GlobalConstants.Technician)
                {
                    roles = await context.Set<Role>().Where(x => x.name == GlobalConstants.Technician).ToListAsync();
                }
                else if (userrole == GlobalConstants.CompanyAdmin)
                {
                    //roles = await context.Set<Role>().Where(x => x.name != GlobalConstants.Admin && x.name != GlobalConstants.CompanyAdmin).ToListAsync();
                    roles = await context.Set<Role>().Where(x => x.name != GlobalConstants.Admin && (x.name == GlobalConstants.CompanyAdmin || x.name == GlobalConstants.Manager
                    || x.name == GlobalConstants.Executive || x.name == GlobalConstants.Operator || x.name == GlobalConstants.MS || x.name == GlobalConstants.Technician)).ToListAsync();
                }
            }
            return roles;
        }

        public List<User> GetUserDetails(string userid, string timestamp, int pageindex, int pagesize)
        {
            List<User> users = new List<User>();
            var usersites = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
            bool date = false;
            string dateString = timestamp;
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime dateTime = new DateTime();
            // It throws Argument null exception  
            try
            {
                dateTime = DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm:ss", provider);
                date = true;
            }
            catch
            {
                date = false;
            }

            if (usersites.Count > 0)
            {

                List<Guid> userlist = new List<Guid>();
                //var usersite = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersites.Contains(x.site_id) &&
                        x.User.Userroles.Any(x => (x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active) || (x.Role.name == GlobalConstants.MS && x.status == (int)Status.Active))).Select(x => x.user_id).Distinct().ToList();

                if (userlist.Count > 0 && date)
                {
                    users = context.User.Include(x => x.Userroles).ThenInclude(x => x.Role)
                        .Include(x => x.Usersites).Include(x => x.StatusMaster).Include(x => x.LanguageMaster).Include(x => x.Site)
                        .Where(x => (x.modified_at >= dateTime || x.Usersites.Any(y => y.modified_at >= dateTime || y.modified_at.ToString() == null) || x.Userroles.Any(y => y.modified_at >= dateTime || y.modified_at.ToString() == null)) && userlist.Contains(x.uuid)).ToList();
                }
                else if (userlist.Count > 0)
                {
                    users = context.User.Include(x => x.Userroles).ThenInclude(x => x.Role)
                        .Include(x => x.Usersites).Include(x => x.StatusMaster).Include(x => x.LanguageMaster).Include(x => x.Site)
                        .Where(x => userlist.Contains(x.uuid)).ToList();
                }
            }

            if (users.Count > 0 && pageindex > 0 && pagesize > 0)
            {
                users = users.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
            }
            return users;
        }

        public async Task<List<User>> SearchUser(string searchstring, int pageindex, int pagesize)
        {
            List<User> user = new List<User>();
            List<Role> roles = new List<Role>();
            searchstring = searchstring.ToLower();
            if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id) && UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string userrole = context.UserRoles.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();
                if (userrole == GlobalConstants.Admin)
                {
                    user = await context.User.Include(x => x.Usersites).Include(x => x.Userroles).Include(x => x.StatusMaster).Where(x => x.uuid.ToString() != UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() &&
                    (x.StatusMaster.status_name.ToLower().Contains(searchstring) || x.Userroles.Any(x => x.Role.name.ToLower().Contains(searchstring)) || x.email.ToLower().Contains(searchstring) || x.username.ToLower().Contains(searchstring))
                    ).ToListAsync();

                }
                else
                {
                    var usersites = context.UserSites.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                    if (usersites.Count > 0)
                    {
                        List<Guid> userlist = new List<Guid>();

                        //userlist = context.UserSites.Include(x => x.User).Include(x => x.User.Role).Include(x => x.StatusMaster).Where(x => x.status == (int)Status.Active && x.user_id.ToString() != userid && usersites.Contains(x.site_id) && x.User.Role.name != GlobalConstants.Admin).Select(x => x.user_id).Distinct().ToList();
                        userlist = context.UserSites.Where(x => x.status == (int)Status.Active && usersites.Contains(x.site_id) && x.User.Userroles.All(x => (x.Role.name != GlobalConstants.Admin) || (x.Role.name == GlobalConstants.Admin && x.status == (int)Status.Deactive)))
                                    .Include(x => x.User).Include(x => x.User.Userroles).Include(x => x.StatusMaster).Select(x => x.user_id).Distinct().ToList();

                        if (userlist.Count > 0)
                        {
                            //user = context.User.Include(x => x.Role).Include(x => x.Usersites).Include(x => x.StatusMaster).Where(x => x.status == status && userlist.Contains(x.uuid) && x.Role.name != GlobalConstants.Admin).ToList();

                            user = await context.User.Where(x => userlist.Contains(x.uuid) && (x.StatusMaster.status_name.ToLower().Contains(searchstring) ||
                                        x.Userroles.Any(x => x.Role.name.ToLower().Contains(searchstring)) || x.email.ToLower().Contains(searchstring) || x.username.ToLower().Contains(searchstring)))
                                        .Include(x => x.Usersites).Include(x => x.Userroles).Include(x => x.StatusMaster).ToListAsync();

                        }
                    }
                }
            }
            return user;
        }

        public async Task<List<User>> GetUsersByToken(string notification_token)
        {
            List<User> users = new List<User>();
            if (notification_token != null && notification_token != string.Empty)
            {
                users = await context.User.Where(x => x.notification_token == notification_token).ToListAsync();
            }
            return users;
        }

        public List<UserSites> GetAllManager()
        {
            var managerRoleUsers = context.UserRoles.Include(x => x.Role).Where(x => x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active).Select(x => x.user_id).Distinct().ToList();

            return context.UserSites.
                Where(x => x.User.status == (int)Status.Active && managerRoleUsers.Contains(x.User.uuid) && x.status == (int)Status.Active && x.User.email_notification_status == true && x.User.is_email_verified == true)
                .Include(x => x.User).Include(x => x.Sites).ThenInclude(x => x.Company).ToList();
        }

        public int CheckUserValid(string userid, Guid device_uuid)
        {
            List<Guid> company_id = context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).Include(x => x.Sites)
                .Select(x => x.Sites.company_id).Distinct().ToList();

            if (company_id.Count > 0)
            {
                var device_info = context.DeviceInfo.Where(x => x.device_uuid == device_uuid && x.is_approved).FirstOrDefault();
                if (device_info != null)
                {
                    if (device_info.company_id != null && company_id.Contains(device_info.company_id.Value))
                    {
                        return (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        return (int)ResponseStatusNumber.DeviceNotAssignToUserCompany;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.UnauthorizedDevice;
                }
            }
            else
            {
                return (int)ResponseStatusNumber.False;
            }
        }

        public Guid? GetUserSite(string userid)
        {
            return context.UserSites.Where(x => x.user_id.ToString() == userid && x.status == (int)Status.Active).OrderBy(x => x.created_at).Select(x => x.site_id).FirstOrDefault();
        }

        public string GetUserNameByID(string user_id)
        {
            return context.User.Where(x => x.uuid.ToString() == user_id).Select(x => x.firstname + " " + x.lastname).FirstOrDefault();
        }
        public User GetUserByEmailID(string email_id)
        {
            return context.User.Where(x => x.email.ToLower() == email_id.ToLower()).FirstOrDefault();
        }

        public ResetPasswordToken GetUserTokenByTokenID(string token)
        {
            return context.ResetPasswordToken.Where(x => x.token == token).FirstOrDefault();
        }

        public List<User> GetAllUsers()
        {
            return context.User.Include(x => x.Role).ToList();
        }

        public bool RoleAlreadyAdded(Guid uuid, Guid role_id)
        {
            return context.UserRoles.Where(x => x.user_id == uuid && x.role_id == x.role_id).Select(x => (x.userrole_id != Guid.Empty || x.userrole_id != null)).FirstOrDefault();
        }
        public async Task<string> GetUserRole()
        {
            var userrole = "";
            if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id) && UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                userrole = context.UserRoles.Where(x => x.user_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString() && x.status == (int)Status.Active && x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id).Select(x => x.Role.name).FirstOrDefault();
            }
            return userrole;
        }

        public async Task<List<Role>> GetAllRoles()
        {
            return context.Role.ToList();
        }

        public List<UserSites> GetAllManagerForOperatorUsageReport()
        {
            var managerRoleUsers = context.UserRoles.Include(x => x.Role).Where(x => x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active).Select(x => x.user_id).Distinct().ToList();

            return context.UserSites.
                Where(x => x.User.status == (int)Status.Active && managerRoleUsers.Contains(x.User.uuid) && x.status == (int)Status.Active && x.User.operator_usage_report_email_not_status == true && x.User.is_email_verified == true)
                .Include(x => x.User).Include(x => x.Sites).ThenInclude(x => x.Company).ToList();
        }

        public async Task<List<User>> GetAllOperatorsList()
        {
            List<User> users = new List<User>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                var userrole = await GetUserRole();
                if (!string.IsNullOrEmpty(userrole))
                {
                    if (userrole == GlobalConstants.Admin)
                    {
                        if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                            {
                                var operatorRoleUsers = context.UserRoles.Include(x => x.Role).Where(x => x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active).Select(x => x.user_id).ToList();
                                users = context.UserSites.Include(x => x.User).Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && operatorRoleUsers.Contains(x.User.uuid)).Select(x => x.User).ToList();
                            }
                            else
                            {
                                var operatorRoleUsers = context.UserRoles.Include(x => x.Role).Where(x => x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active).Select(x => x.user_id).ToList();
                                users = context.UserSites.Include(x => x.User).Where(x => companysites.Select(x => x.site_id).ToList().Contains(x.site_id) && operatorRoleUsers.Contains(x.User.uuid) && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.User).ToList();
                            }
                        }
                        else if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.AllCompanyType)
                        {
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status != (int)Status.AllSiteType)
                            {
                                var operatorRoleUsers = context.UserRoles.Include(x => x.Role).Where(x => x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active).Select(x => x.user_id).ToList();
                                users = context.UserSites.Where(x => x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Include(x => x.User).Where(x => operatorRoleUsers.Contains(x.User.uuid)).Select(x => x.User).ToList();
                            }
                            else
                            {
                                var operatorRoleUsers = context.UserRoles.Include(x => x.Role).Where(x => x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active).Select(x => x.user_id).ToList();
                                users = context.UserSites.Include(x => x.User).Where(x => operatorRoleUsers.Contains(x.User.uuid)).Select(x => x.User).ToList();
                            }
                        }
                        else
                        {
                            var operatorRoleUsers = context.UserRoles.Include(x => x.Role).Where(x => x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active).Select(x => x.user_id).ToList();
                            users = context.UserSites.Include(x => x.User).Where(x => operatorRoleUsers.Contains(x.User.uuid)).Select(x => x.User).ToList();
                        }
                    }
                    else
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        var operatorRoleUsers = context.UserRoles.Include(x => x.Role).Where(x => x.Role.name == GlobalConstants.Operator && x.status == (int)Status.Active).Select(x => x.user_id).ToList();

                        users = context.UserSites.Include(x => x.User).Where(x => usersites.Contains(x.site_id) && operatorRoleUsers.Contains(x.User.uuid)).Select(x => x.User).ToList();
                    }
                }
            }
            return users;
        }
        public List<User> GetAllExecutiveForDailyReport()
        {
            var executiveRoleUsers = context.UserRoles.Include(x => x.Role).Where(x => x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active).Select(x => x.user_id).Distinct().ToList();
            return context.User.Where(x => executiveRoleUsers.Contains(x.uuid) && x.executive_report_status == (int)Status.DailyReport && x.is_email_verified == true && x.status == (int)Status.Active)
                .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.Userroles).ToList();
        }

        public List<User> GetAllExecutiveForWeeklyReport()
        {
            var executiveRoleUsers = context.UserRoles.Include(x => x.Role).Where(x => x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active).Select(x => x.user_id).Distinct().ToList();
            return context.User.Where(x => executiveRoleUsers.Contains(x.uuid) && x.executive_report_status == (int)Status.WeeklyReport && x.is_email_verified == true && x.status == (int)Status.Active)
                .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.Userroles).ToList();
        }

        public List<UserSites> GetAllExecutiveForPendingInspectionEmail()
        {
            var executiveRoleUsers = context.UserRoles.Include(x => x.Role).Where(x => x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active).Select(x => x.user_id).Distinct().ToList();

            return context.UserSites.
                Where(x => x.User.status == (int)Status.Active && executiveRoleUsers.Contains(x.User.uuid) && x.status == (int)Status.Active && x.User.executive_report_status == (int)Status.WeeklyReport && x.User.is_email_verified == true)
                .Include(x => x.User).Include(x => x.Sites).ThenInclude(x => x.Company).ToList();
            
        }

        public List<User> GetAllManagerForPMNotification()
        {
            var managerRoleUsers = context.UserRoles.Include(x => x.Role).Where(x => x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active).Select(x => x.user_id).Distinct().ToList();

            return context.User.
                Where(x => x.status == (int)Status.Active && managerRoleUsers.Contains(x.uuid) && x.status == (int)Status.Active).ToList();
        }

        public async Task<NotificationData> GetNotificationByID(Guid notification_id)
        {
            return await context.NotificationData.Where(u => u.notification_id == notification_id).FirstOrDefaultAsync();
        }

        public List<UserSites> GetAllManagerForPMNotifications(Guid company_id)
        {
            var managerRoleUsers = context.UserRoles.Include(x => x.Role).Where(x => x.Role.name == GlobalConstants.Manager && x.status == (int)Status.Active).Select(x => x.user_id).Distinct().ToList();
            var sites = context.Sites.Where(x => x.status == (int)Status.Active && x.company_id == company_id).Select(x => x.site_id).ToList();
            return context.UserSites.
                Where(x => sites.Contains(x.site_id) && x.User.status == (int)Status.Active && managerRoleUsers.Contains(x.User.uuid) && x.status == (int)Status.Active && x.User.manager_pm_notification_status == true)
                .Include(x => x.User).Include(x => x.Sites).ThenInclude(x => x.Company).ToList();
        }

        public List<User> GetAllExecutiveForPMDueReport()
        {
            var executiveRoleUsers = context.UserRoles.Include(x => x.Role).Where(x => x.Role.name == GlobalConstants.Executive && x.status == (int)Status.Active).Select(x => x.user_id).Distinct().ToList();
            return context.User.Where(x => executiveRoleUsers.Contains(x.uuid) && x.UserEmailNotificationConfigurationSettings != null
            && (x.UserEmailNotificationConfigurationSettings.executive_pm_due_not_resolved_email_notification == true ||
            (x.UserEmailNotificationConfigurationSettings.executive_pm_due_not_resolved_email_notification == false && x.UserEmailNotificationConfigurationSettings.disabled_till_date.Value.Date < DateTime.UtcNow.Date))
            && x.is_email_verified == true && x.status == (int)Status.Active)
                .Include(x => x.Usersites).ThenInclude(x => x.Sites.Company).Include(x => x.Userroles).ToList();
        }

        public async Task<UserEmailNotificationConfigurationSettings> GetExecutiveRolePMDueReportConfig(Guid uuid)
        {
            return await context.UserEmailNotificationConfigurationSettings.Where(x => x.user_id == uuid && x.status == (int)Status.Active).FirstOrDefaultAsync();
        }

        public async Task<User> GetInternalUserByID(string uuid)
        {
            return await context.User.Where(x => x.uuid.ToString() == uuid).FirstOrDefaultAsync();
        }
        public User GetUserByUserID(string user_id)
        {
            return context.User.Where(x => x.uuid.ToString() == user_id).FirstOrDefault();
        }

        public Sites GetAllSiteMaster()
        {
            return context.Sites.Where(x => x.status == (int)Status.AllSiteType).Include(x=>x.ClientCompany).FirstOrDefault();
        }
        public ClientCompany GetClientCompanyBySiteid(Guid SiteID)
        {
            return context.Sites.Where(x => x.site_id == SiteID).Select(x => x.ClientCompany).FirstOrDefault();
        }
        public List<ClientCompany> GetClientCompanyByParentCompany(Guid CompanyID)
        {
            return context.ClientCompany.Where(x => x.parent_company_id == CompanyID && x.status == (int)Status.Active).ToList();
        }
        public List<UserRoles> GetUserRolesByEmail(String email_id)
        {
            return context.UserRoles.Where(x => x.User.email.ToLower().Trim() == email_id && x.status == (int)Status.Active).Include(x=>x.Role).ToList();
        }
        public MobileAppVersion MobileAppVersion(int device_brand)
        {
            return context.MobileAppVersion.Where(x=>x.device_brand == device_brand).FirstOrDefault();
        }
        public ClientCompany GetClientCompanyById(Guid client_company)
        {
            return context.ClientCompany.Where(x => x.client_company_id == client_company).FirstOrDefault();
        }

        public (List<ClientCompany>, int) GetAllClientCompanyWithSites(GetAllClientCompanyWithSitesRequestModel request)
        {
            IQueryable<ClientCompany> query = context.ClientCompany.Where(x => x.parent_company_id == request.company_id);

            
            // search string
            if (!string.IsNullOrEmpty(request.search_string))
            {

                var searchstring = request.search_string.ToLower().ToString();
                query = query.Where(x => (x.client_company_name.ToLower().Contains(searchstring) || x.owner.ToLower().Contains(searchstring) || 
                x.owner_address.ToLower().Contains(searchstring) || x.clientcompany_code.ToLower().Contains(searchstring)));
            }

            int total_count = query.Count();

            if (request.pageindex > 0 && request.pagesize > 0)
            {
                query = query.OrderBy(x => x.client_company_name).Skip((request.pageindex - 1) * request.pagesize).Take(request.pagesize);
            }
            else
            {
                query = query.OrderBy(x => x.client_company_name);
            }

            return (query.Include(x=>x.Sites).ThenInclude(x=>x.SiteProjectManagerMapping).ThenInclude(x=>x.User).ToList(), total_count);
        }

        public List<User> GetAllCompanyAdmins(Guid company_id)
        {
            var ca_userid_list = context.UserRoles.Where(x => x.role_id == Guid.Parse("972e1960-947b-42b0-a8ea-5e23a68e9632") && x.status == (int)Status.Active).Select(x => x.user_id).Distinct().ToList();

            return context.User.Where(x => ca_userid_list.Contains(x.uuid) && x.ac_default_company == company_id && x.status == (int)Status.Active ).ToList();
        }

        public bool CheckForTechnicianSiteAccess(Guid requested_by, Guid site_id)
        {
            //Check if the requested_by user is a technician
            var isRequestedByTechnician = context.UserRoles
                .Any(x => x.user_id == requested_by
                          && x.role_id == Guid.Parse("b22217c2-a932-498c-8ab2-11fc2628104b") // Technician role ID
                          && x.status == (int)Status.Active);

            //Check if the technician does not have an entry in the UserSites table for the given site_id
            var technicianHasNoSiteAccess = !context.UserSites
                .Any(us => us.user_id == requested_by
                           && us.site_id == site_id
                           && us.status == (int)Status.Active); 

            if (isRequestedByTechnician && technicianHasNoSiteAccess)
            {
                return true;
            }

            return false;
        }

        

        public WorkOrderWatcherUserMapping GetWorkorderWatcherMappingById(AddUpdateWorkOrderWatcherRequestModel requestModel)
        {
            return context.WorkOrderWatcherUserMapping.Where(x=>x.user_id == requestModel.user_id 
                    && x.ref_id == requestModel.ref_id && !x.is_deleted).FirstOrDefault();
        }

        public List<string> GetUserRolesById(Guid user_id)
        {
            return context.UserRoles.Where(x => x.user_id == user_id && x.status == (int)Status.Active)
                .Select(x=>x.role_id.ToString()).ToList();
        }

        public List<Guid> GetAllBackOfficeUsersBySiteId(Guid site_id)
        {
            return context.UserSites.Include(x=>x.User).ThenInclude(x=>x.Userroles).Where(x=>x.site_id == site_id 
                && x.status == (int)Status.Active && x.User.status == (int)Status.Active 
                && x.User.Userroles.Select(x=>x.role_id).Contains(Guid.Parse(GlobalConstants.BackOffice_Role_Id))
                && x.User.Userroles.Select(x=>x.status).Contains((int)Status.Active) ) 
                .Select(x=>x.user_id).ToList();
        }

        public (List<NotificationData>,int) GetNotificationsDataByUserId(string userid, string notification_user_role, int pagesize, int pageindex)
        {
            IQueryable<NotificationData> query = context.NotificationData.Where(x => x.user_id == Guid.Parse(userid)
            && x.notification_user_role.Contains(notification_user_role)
            && x.status == (int)Status.Active && x.is_visible);

            int total_count = query.Count();

            if (pageindex > 0 && pagesize > 0)
            {
                query = query.OrderByDescending(g => g.createdDate).Skip((pageindex - 1) * pagesize).Take(pagesize);
            }
            else
            {
                query = query.OrderByDescending(x => x.createdDate);
            }

            return (query.ToList(), total_count);
        }

        public int GetNotificationCountByUserId(Guid user_id, string notification_user_role , int? notification_status)
        {
            if (notification_status == null)
            {
                return context.NotificationData.Where(x => x.user_id == user_id && x.notification_user_role.Contains(notification_user_role)
                && x.status == (int)Status.Active).Count();
            }
            else
            {
                return context.NotificationData.Where(x => x.user_id == user_id && x.notification_user_role.Contains(notification_user_role)
                && x.notification_status == notification_status && x.status == (int)Status.Active).Count();
            }
        }
        public List<WorkOrderTechnicianMapping> GetUserActiveWOForInactiveSite(List<Guid> remove_site, Guid user_id)
        {
            return context.WorkOrderTechnicianMapping.Where(x => x.user_id == user_id && remove_site.Contains(x.WorkOrders.site_id) && !x.WorkOrders.is_archive && x.WorkOrders.status != (int)Status.Completed
                                                             && !x.is_deleted)
                .Include(x=>x.WorkOrders).ThenInclude(x=>x.Sites)
                .ToList();
        }
        public List<WorkOrderTechnicianMapping> GetUserActiveWOForInactiveRole(Guid user_id)
        {
            return context.WorkOrderTechnicianMapping.Where(x => x.user_id == user_id && !x.WorkOrders.is_archive && x.WorkOrders.status != (int)Status.Completed
                                                             && !x.is_deleted)
                .Include(x=>x.WorkOrders).ThenInclude(x=>x.Sites)
                .ToList();
        }
        public int? GetWOTypeById(Guid wo_id)
        {
            return context.WorkOrders.Where(x => x.wo_id == wo_id).Select(x => x.wo_type).FirstOrDefault();
        }
        public User GetUserByIdForSites(Guid user_id)
        {
            return context.User.Where(x => x.uuid == user_id)
                .Include(x => x.Company.ClientCompany)
                .Include(x => x.Site)
                .Include(x => x.Active_Site)
                .Include(x => x.Role_Web)
                .Include(x => x.Active_Role_Web)
                .FirstOrDefault();
        }
        public List<Sites> GetActiveUserSitesById(Guid user_id)
        {
            var sites = context.UserSites.Where(x => x.user_id == user_id && x.status == (int)Status.Active).Select(x => x.site_id).ToList();

            return context.Sites.Where(x => sites.Contains(x.site_id) && x.status == (int)Status.Active)
                //.Include(x => x.Company)
                .Include(x => x.ClientCompany)
                .Include(x => x.SiteContact)
                .ToList();
        }
        public List<Role> GetActiveUserRolesById(Guid user_id)
        {
            var roles = context.UserRoles.Where(x => x.user_id == user_id && x.status == (int)Status.Active).Select(x => x.role_id).ToList();

            return context.Role.Where(x => roles.Contains(x.role_id)).ToList();
        }
        public List<ClientCompany> GetActiveClientCompany(List<Guid> sites)
        {
            var cc_id_list = context.Sites.Where(x => sites.Contains(x.site_id) && x.status == (int)Status.Active)
                .Select(x => x.client_company_id).Distinct().ToList();

            return context.ClientCompany.Where(x => cc_id_list.Contains(x.client_company_id) && x.status == (int)Status.Active)
                .Include(x => x.Sites).ToList();
        }
        public string GetRoleNameById(Guid role_id)
        {
            return context.Role.Where(x => x.role_id == role_id).Select(x => x.name).FirstOrDefault();
        }
        public  Guid GetSiteCompanyId(string site_id)
        {
            return context.Sites.Where(x => x.site_id == Guid.Parse(site_id)).Select(x => x.company_id).FirstOrDefault();
        }
        public SiteProjectManagerMapping GetSiteProjectManagerMappingById(Guid site_projectmanager_mapping_id)
        {
            return context.SiteProjectManagerMapping.Where(x=>x.site_projectmanager_mapping_id == site_projectmanager_mapping_id).FirstOrDefault();
        }
        public GetAllTechniciansListResponseModel GetAllProjectManagersList()
        {
            GetAllTechniciansListResponseModel response = new GetAllTechniciansListResponseModel();

            var userid_list = context.UserSites.Include(x=>x.Sites).Where(x => x.Sites.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)
                    && x.status == (int)Status.Active)
                        .Select(x => x.user_id).Distinct().ToList();

            var pm_userid_list = context.UserRoles.Where(x => 
                (x.role_id == Guid.Parse(GlobalConstants.BackOffice_Role_Id) || x.role_id == Guid.Parse(GlobalConstants.CompanyAdmin_Role_id))
                && userid_list.Contains(x.user_id) && x.status == (int)Status.Active)
                    .Select(x => x.user_id).Distinct().ToList();

            IQueryable<User> pm_users = context.User.Where(x => pm_userid_list.Contains(x.uuid) && x.is_email_verified && x.status == (int)Status.Active);
            
            response.list = pm_users
              .Select(x => new GetAllUserData_Class
              {
                  uuid = x.uuid,
                  firstname = x.firstname,
                  lastname = x.lastname,
                  email = x.email
              }).ToList();

            return response;
        }

        public List<UserSession> GetActiveUserSessionByUUId(Guid user_id)
        {
            if (UpdatedGenericRequestmodel.CurrentUser.platform_type.ToLower() == AuthenticationConstants.MobilePlatform)
            {
                return context.UserSession.Where(x => x.user_id == user_id && x.status == (int)Status.Active && x.role_id == Guid.Parse(GlobalConstants.Technician_Role_id)).ToList();
            }
            else
            {
                return context.UserSession.Where(x => x.user_id == user_id && x.status == (int)Status.Active && x.role_id != Guid.Parse(GlobalConstants.Technician_Role_id)).ToList();
            }
        }
        public bool CheckUserSessionIsValidOrNot(Guid user_id, Guid user_session_id, Guid role_id)
        {
            var session = context.UserSession.Where(x => x.user_session_id == user_session_id).FirstOrDefault();

            if (session != null && session.user_id == user_id && UpdatedGenericRequestmodel.CurrentUser.platform_type.ToLower() == AuthenticationConstants.MobilePlatform && session.role_id == Guid.Parse(GlobalConstants.Technician_Role_id) && session.status == (int)Status.Active)
            {
                return true;
            }
            else if (session != null && session.user_id == user_id 
                && (session.role_id == Guid.Parse(GlobalConstants.BackOffice_Role_Id) || session.role_id == Guid.Parse(GlobalConstants.CompanyAdmin_Role_id) || session.role_id == Guid.Parse(GlobalConstants.Executive_Role_id) || session.role_id == Guid.Parse(GlobalConstants.SuperAdmin_Role_id))
                && session.status == (int)Status.Active)
            {
                return true;
            }
            return false;
        }


        public List<NotificationData> GetAllNewNotificationsByTypeUserId(Guid user_id, int notification_type, string ref_id)
        {
            return context.NotificationData.Where(x => x.user_id == user_id
            && x.notification_type == notification_type //&& x.sendDate.Date < DateTime.UtcNow.AddDays(-7).Date
            && x.ref_id == ref_id 
            && (x.notification_status == (int)Notification_Status.New || x.is_visible)).ToList();
        }

        public GetAllTechniciansLocationByWOIdResponseModel GetAllTechniciansLocationByWOId(Guid wo_id)
        {
            GetAllTechniciansLocationByWOIdResponseModel response = new GetAllTechniciansLocationByWOIdResponseModel();
           
            var tech_ids = context.WorkOrderTechnicianMapping.Where(x => x.wo_id == wo_id && !x.is_deleted).Select(x => x.user_id).ToList();

            var users = context.User.Where(x => tech_ids.Contains(x.uuid) && x.status == (int)Status.Active).Include(x => x.UserLocation).Include(x=>x.Site).ToList();
            
            response.list = users
              .Select(x => new UserLocation_Data_Model
              {
                  user_id = x.uuid,
                  email = x.email,
                  name = x.firstname +" "+ x.lastname,
                  address = x.Site != null ? x.Site.site_name : "New York",// for FE testing purpose only
                  is_location_active = x.UserLocation.OrderByDescending(y => y.created_at).FirstOrDefault()!=null ? x.UserLocation.OrderByDescending(y=>y.created_at).FirstOrDefault().is_location_active:false,
                  latitude = x.UserLocation.OrderByDescending(y => y.created_at).FirstOrDefault()!=null ? x.UserLocation.OrderByDescending(y=>y.created_at).FirstOrDefault().latitude:null,
                  longitude = x.UserLocation.OrderByDescending(y => y.created_at).FirstOrDefault()!=null ? x.UserLocation.OrderByDescending(y => y.created_at).FirstOrDefault().longitude:null

              }).ToList();

            return response;
        }

        public bool CheckIsWOOverdueByWeekOrNot(string wo_id)
        {
            bool res = false;
            var wo = context.WorkOrders.Where(x => x.wo_id.ToString() == wo_id).FirstOrDefault();
            var due_date = wo.due_at.Date;
            var diff = DateTime.UtcNow.Date - due_date;

            if (diff.Days > 7)
                res = true;

            return res;
        }
        
        public GetAllTechniciansListResponseModel GetAllTechniciansForCalendar(GetAllCalanderWorkordersRequestModel requestModel,bool is_request_for_technician)
        {
            GetAllTechniciansListResponseModel response = new GetAllTechniciansListResponseModel();

            var wo_ids = context.WorkOrders.Where(x => ((x.start_date.Date >= requestModel.start_date.Date && x.start_date.Date <= requestModel.end_date.Date)
            || (x.due_at.Date >= requestModel.start_date.Date && x.due_at.Date <= requestModel.end_date.Date))
            && !x.is_archive && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).Select(x=>x.wo_id).ToList();

            var uuids = new List<Guid>();

            if (is_request_for_technician)
                uuids = context.WorkOrderTechnicianMapping.Where(x => wo_ids.Contains(x.wo_id) && !x.is_deleted).Select(x => x.user_id).Distinct().ToList();
            else
                uuids =  context.WorkOrderBackOfficeUserMapping.Where(x => wo_ids.Contains(x.wo_id) && !x.is_deleted).Select(x => x.user_id).Distinct().ToList();

            var users = context.User.Where(x => uuids.Contains(x.uuid)).ToList();
            
            response.list = users.Select(z => new GetAllUserData_Class { uuid = z.uuid, firstname = z.firstname, lastname = z.lastname, email = z.email }).ToList();

            return response;
        }

        public bool IsFeatureRequiredOrNotForCompany(Guid company_id,Guid feature_id)
        {
            return context.CompanyFeatureMappings.Where(x=> x.company_id==company_id 
            && x.feature_id == feature_id
            && x.is_required).Any();
        }

        public GetAllFeaturesFlagsByCompanyResponseModel GetAllFeaturesByCompanyId(Guid company_id)
        {
            GetAllFeaturesFlagsByCompanyResponseModel responseModel = new GetAllFeaturesFlagsByCompanyResponseModel();
            var features = context.CompanyFeatureMappings.Where(x=>x.company_id==company_id).Include(x=>x.Features).ToList();

            responseModel.list = features.Select(x => new GetAllFeaturesFlagsByCompany_Class
            {
                company_feature_id = x.company_feature_id,
                feature_id = x.feature_id,
                is_required = x.is_required,
                feature_name = x.Features!=null ? x.Features.feature_name : null,
                 feature_description = x.Features!=null ? x.Features.feature_description : null
            }).ToList();

            return responseModel;
        }
        public CompanyFeatureMapping GetCompanyFeatureMappingById(Guid company_feature_id)
        {
            return context.CompanyFeatureMappings.Where(x=>x.company_feature_id == company_feature_id).FirstOrDefault();
        }

        public List<Sites> GetSitesByClientCompanyId(Guid client_company_id)
        {
            return context.Sites.Where(x=>x.client_company_id == client_company_id && x.status==(int)Status.Active).ToList();
        }

        public Vendors GetVendorDetailsById(Guid vendor_id)
        {
            return context.Vendors.Where(x=>x.vendor_id == vendor_id).Include(x=>x.Contacts).FirstOrDefault();
        }

        public List<Workorders_Data_Model> GetWorkordersByVendorId(Guid vendor_id)
        {
            List<Workorders_Data_Model> res = new List<Workorders_Data_Model>();
            var wo_ids = context.WorkordersVendorContactsMapping.Where(x => x.vendor_id == vendor_id && !x.is_deleted).Select(x => x.wo_id).ToList();

            var wos = context.WorkOrders.Where(x => wo_ids.Contains(x.wo_id))
                .Include(x => x.Sites).Include(x => x.WorkordersVendorContactsMapping).ThenInclude(x => x.Contacts).ToList();

            res = wos.Select(x => new Workorders_Data_Model
            {
                wo_id = x.wo_id,
                manual_wo_number = x.manual_wo_number,
                due_at = x.due_at,
                start_date = x.start_date,
                wo_type = x.wo_type,
                site_name = x.Sites != null ? x.Sites.site_name : null,
                total_count = x.WorkordersVendorContactsMapping != null ? x.WorkordersVendorContactsMapping.Where(y => !y.is_deleted).Count() : 0,
                accepted_count = x.WorkordersVendorContactsMapping != null ? x.WorkordersVendorContactsMapping.Where(y => !y.is_deleted && y.contact_invite_status == (int)Contact_Invite_Status.Accepted).Count() : 0,

                contacts_invite_status = x.WorkordersVendorContactsMapping.Where(y => !y.is_deleted).Select(y => new contacts_ivite_status_class
                {
                    contact_invite_status = y.contact_invite_status,
                    name = y.Contacts.name,
                }).ToList()

            }).ToList();

            return res;
        }

        public Vendors GetVendorById(Guid vendor_id)
        {
            return context.Vendors.Where(x => x.vendor_id == vendor_id).FirstOrDefault();
        }

        public Contacts GetContactById(Guid contact_id)
        {
            return context.Contacts.Where(x => x.contact_id == contact_id).FirstOrDefault();
        }

        public (List<Vendors>, int) GetAllVendorList(GetAllVendorListRequestModel requestModel)
        {
            IQueryable<Vendors> query = context.Vendors.Where(x => !x.is_deleted && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id));
            
            if (requestModel.vendor_category_ids != null && requestModel.vendor_category_ids.Count > 0)
            {
                query = query.Where(x => requestModel.vendor_category_ids.Contains(x.vendor_category_id.Value));
            }

            if (requestModel.vendor_category_ids != null && requestModel.vendor_category_ids.Count > 0)
            {
                query = query.Where(x => requestModel.vendor_category_ids.Contains(x.vendor_category_id.Value));
            }

            if (!String.IsNullOrEmpty(requestModel.search_string))
            {
                string search_str = requestModel.search_string.ToLower().Trim();
                query = query.Where(x =>
                                    x.vendor_name.ToLower().Trim().Contains(search_str) ||
                                    x.vendor_email.ToLower().Trim().Contains(search_str) ||
                                    x.vendor_phone_number.ToLower().Trim().Contains(search_str) ||
                                    x.vendor_category.ToLower().Trim().Contains(search_str) ||
                                    x.vendor_address.ToLower().Trim().Contains(search_str)
                );
            }

            int listsize = query.Count();
            query = query.OrderByDescending(x => x.created_at);

            if (requestModel.pagesize > 0 && requestModel.pageindex > 0)
            {
                query = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
            }
            return (query.ToList(), listsize);
        }
        public List<WO_Vendor_Contacts_Mapping_View_Class> GetAllVendorsContactsForDropdown()
        {
            List<WO_Vendor_Contacts_Mapping_View_Class> res = new List<WO_Vendor_Contacts_Mapping_View_Class>();
            var list = context.Vendors.Where(x=>x.company_id==Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) 
            && !x.is_deleted).Include(x=>x.Contacts).ToList();

            res = list.Select(x => new WO_Vendor_Contacts_Mapping_View_Class
            {
                vendor_id = x.vendor_id,
                vendor_email = x.vendor_email,
                vendor_name = x.vendor_name,
                contacts_list = x.Contacts.Where(x=>!x.is_deleted).Select(x => new Contacts_Data_View_Obj_Class
                {
                    vendor_id = x.vendor_id,
                    contact_id = x.contact_id,
                    name = x.name,
                    email = x.email
                }).ToList()

            }).ToList();

            return res;
        }
        public bool IsMFAEnabled(string user_pool_id)
        {
            var get_company = context.Company.Where(x => x.user_pool_id == user_pool_id).Select(x => x.company_id).FirstOrDefault();
            var is_mfa_enable = context.CompanyFeatureMappings.Where(x => x.company_id == get_company
                                && x.feature_id == Guid.Parse(GlobalConstants.is_mfa_enabled_feature_id)).Select(x => x.is_required).FirstOrDefault();

            return is_mfa_enable;
        }
        public string GetComapnyIdFromDomain(string domain_name)
        {
            return context.Company.Where(x => x.domain_name == domain_name).FirstOrDefault().company_id.ToString();
        }
        public Guid GetContactIdByEmail(string email,Guid wo_id)
        {
            var ll = context.WorkordersVendorContactsMapping.Where(x => x.wo_id==wo_id && !x.is_deleted).Include(x=>x.Contacts).ToList();

            return ll.Where(x=>x.Contacts.email==email).Select(x=>x.contact_id).FirstOrDefault();

            //return context.Contacts.Where(x => x.email == email && !x.is_deleted
            //    && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)).Select(x => x.contact_id).FirstOrDefault();
        }
        public WorkordersVendorContactsMapping GetWOContactMapById(Guid contact_id, Guid wo_id)
        {
            return context.WorkordersVendorContactsMapping.Where(x => x.contact_id == contact_id && x.wo_id == wo_id
            && !x.is_deleted).FirstOrDefault();
        }

        public GetRefreshedContactsByWOIdResponseModel GetRefreshedContactsByWOId(Guid wo_id)
        {
            GetRefreshedContactsByWOIdResponseModel res = new GetRefreshedContactsByWOIdResponseModel();
            var map = context.WorkordersVendorContactsMapping.Where(x=>x.wo_id==wo_id&&!x.is_deleted).Include(x=>x.Contacts).ToList();

            res.total_contacts_count = map.Count();
            res.accepted_contacts_count = map.Where(y => y.contact_invite_status == (int)Contact_Invite_Status.Accepted).Count();
            
            res.contacts_status_list = map.Select(x => new Contact_Status_Model
            {
                contact_name=x.Contacts.name,
                contact_invite_status = x.contact_invite_status
            }).ToList();

            return res;
        }
        public List<Guid> GetWOIdsByVendorId(Guid vendor_id)
        {
            return context.WorkordersVendorContactsMapping.Where(x=>x.vendor_id==vendor_id&&!x.is_deleted).Select(x=>x.wo_id).Distinct().ToList();
        }

        public List<string> GetUserWithSingleSite(Guid site_id , Guid company_id)
        {
            // user ids with other sites active
            var users_with_other_sites = context.UserSites.Where(x => x.site_id != site_id
                                                && x.status == (int)Status.Active
                                                && x.Sites.company_id == company_id).Select(x => x.user_id).Distinct().ToList();

            var users_with_this_site_only = context.UserSites.Where(x => x.Sites.company_id == company_id && !users_with_other_sites.Contains(x.user_id)
            && x.site_id == site_id && x.status == (int)Status.Active && x.User.status == (int)Status.Active).Select(x => x.User.email).Distinct().ToList();

            return users_with_this_site_only;
        }
        public List<string> GetUserWithSingleClientComapany(Guid clientcompany_id, Guid company_id)
        {
            // user ids with other sites active
            var users_with_other_sites = context.UserSites.Where(x => x.Sites.client_company_id != clientcompany_id
                                                && x.status == (int)Status.Active
                                                && x.Sites.company_id == company_id).Select(x => x.user_id).Distinct().ToList();

            var users_with_this_site_only = context.UserSites.Where(x => x.Sites.company_id == company_id && !users_with_other_sites.Contains(x.user_id)
            && x.Sites.client_company_id == clientcompany_id && x.status == (int)Status.Active && x.User.status == (int)Status.Active).Select(x => x.User.email).Distinct().ToList();

            return users_with_this_site_only;
        }
        public CompanyFeatureMapping GetUserCompanyFeatureRecord(Guid UserId)
        {
            return context.CompanyFeatureMappings.Where(cfm => cfm.user_id == UserId && cfm.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && cfm.feature_id == Guid.Parse(GlobalConstants.hide_email_for_user_feature_id)).FirstOrDefault();
        }

        public GetSiteUsersDetailsByIdResponseModel GetSiteUsersDetailsById(Guid site_id)
        {
            GetSiteUsersDetailsByIdResponseModel response = new GetSiteUsersDetailsByIdResponseModel();

            var result = context.Sites.Where(x => x.site_id == site_id).Include(x => x.UserSites).ThenInclude(x => x.User).ThenInclude(x => x.Userroles).ThenInclude(x => x.Role).Include(x => x.SiteProjectManagerMapping).ThenInclude(x => x.User).FirstOrDefault();

            if (result != null) // Check if a result was found
            {
                response = new GetSiteUsersDetailsByIdResponseModel
                {
                    site_id = result.site_id,
                    company_id = result.company_id,
                    site_name = result.site_name,
                    site_code = result.site_code,
                    customer = result.customer,
                    is_add_asset_class_enabled = result.isAddAssetClassEnabled,
                    customer_address = result.customer_address,
                    status = result.status,
                    profile_image = result.profile_image,
                    site_projectmanager_list = result.SiteProjectManagerMapping.Select(pm => new SiteProjectManagerMapping_View_Class
                    {
                         site_projectmanager_mapping_id = pm.site_projectmanager_mapping_id,
                         user_id = pm.user_id,
                         name = pm.User.firstname + " " + pm.User.lastname,
                         email = pm.User.email,
                    }).ToList(),
                    site_users_list = result.UserSites.Select(x => new Site_Users_List
                    {
                         user_id = x.user_id,
                         user_name = x.User.firstname + " " + x.User.lastname,
                         user_email = x.User.email,
                         roles_list = x.User.Userroles.Select(ur => ur.Role.name).ToList()

                    }).ToList()

                };
            }

            return response;
        }

        public ClientCompany GetDefaultClientCompany()
        {
            return context.ClientCompany.Where(x=> x.client_company_name=="Default" && x.status == (int)Status.Active
            && x.parent_company_id==Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)).FirstOrDefault();
        }

        public SiteContact GetSiteContactById(Guid sitecontact_id)
        {
            return context.SiteContact.Where(x => x.sitecontact_id == sitecontact_id && !x.is_deleted).FirstOrDefault();
        }

        public UserSites GetUserSiteById(Guid uuid)
        {
            return context.UserSites.Where(x=>x.user_id==uuid && x.site_id==Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).FirstOrDefault();
        }
    }
}
