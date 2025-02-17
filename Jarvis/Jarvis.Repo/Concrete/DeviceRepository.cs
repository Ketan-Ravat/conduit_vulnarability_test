using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels.RequestResponseViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete
{
    public class DeviceRepository : BaseGenericRepository<Inspection>, IDeviceRepository
    {
        public DeviceRepository(DBContextFactory dataContext) : base(dataContext)
        {

        }

        public DeviceInfo GetDeviceInfoByUUId(Guid device_uuid)
        {
            return context.DeviceInfo.Where(x => x.device_uuid == device_uuid).FirstOrDefault();
        }

        public DeviceInfo GetDeviceInfo(Guid device_uuid, string device_code)
        {
            return context.DeviceInfo.Where(x => x.device_uuid == device_uuid || x.device_code == device_code).FirstOrDefault();
        }
        public DeviceInfo GetDeviceInfoById(int device_info_id)
        {
            return context.DeviceInfo.Where(x => x.device_info_id == device_info_id).FirstOrDefault();
        }

        public List<RecordSyncInformation> FindSyncRecord()
        {
            int days = Convert.ToInt32(ConfigurationManager.AppSettings["RemoveRecordDays"] == null ? "0" : ConfigurationManager.AppSettings["RemoveRecordDays"].ToString());
            return context.RecordSyncInformation.Where(x => x.created_at.Value.Date < DateTime.UtcNow.AddDays(-days ).Date).ToList();
        }

        public async Task<List<DeviceInfo>> FilterDevice(FilterDeviceRequestModel requestModel)
        {
            List<DeviceInfo> deviceInfos = new List<DeviceInfo>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
                {
                    rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(rolename))
                {
                    IQueryable<DeviceInfo> query = context.DeviceInfo;

                    if (rolename == GlobalConstants.Admin)
                    {
                        if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                            {
                                //query = query.Where(x => x.last_sync_site_id != null && companysites.Select(x => x.site_id).ToList().Contains(x.last_sync_site_id.Value));
                                query = query.Where(x => x.company_id != null && x.company_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id);
                            }
                            else
                            {
                                query = query.Where(x => x.last_sync_site_id != null && companysites.Select(x => x.site_id).ToList().Contains(x.last_sync_site_id.Value) && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                            }
                        }
                        else if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.AllCompanyType)
                        {
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status != (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.last_sync_site_id != null && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                            }
                        }
                    }
                    else if (rolename == GlobalConstants.CompanyAdmin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Where(x => x.company_id != null && x.company_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id);
                            //query = query.Where(x => x.last_sync_site_id != null && usersites.Contains(x.last_sync_site_id.Value));
                        }
                        else
                        {
                            query = query.Where(x => x.last_sync_site_id != null && usersites.Contains(x.last_sync_site_id.Value) && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                        }
                    }

                    if (requestModel.status > 0)
                    {
                        if (requestModel.status == (int)Status.Active)
                        {
                            query = query.Where(x => x.is_approved == true);
                        }
                        else if (requestModel.status == (int)Status.Deactive)
                        {
                            query = query.Where(x => x.is_approved == false);
                        }
                    }

                    if (requestModel.site_id?.Count > 0)
                    {
                        query = query.Where(x => x.last_sync_site_id != null && requestModel.site_id.Contains(x.last_sync_site_id.Value.ToString()));
                    }

                    if (requestModel.company_id?.Count > 0)
                    {
                        //query = query.Where(x => x.last_sync_site_id != null && requestModel.company_id.Contains(x.company_id.ToString()));
                        query = query.Where(x => requestModel.company_id.Contains(x.company_id.ToString()));
                    }


                    // add model name Filter
                    if (requestModel.model?.Count > 0)
                    {
                        query = query.Where(x => requestModel.model.Contains(x.model));
                    }

                    // add brand Filter
                    if (requestModel.brand?.Count > 0)
                    {
                        query = query.Where(x => requestModel.brand.Contains(x.brand));
                    }

                    // add os Filter
                    if (requestModel.os?.Count > 0)
                    {
                        query = query.Where(x => requestModel.os.Contains(x.os));
                    }

                    // add type Filter
                    if (requestModel.type?.Count > 0)
                    {
                        query = query.Where(x => requestModel.type.Contains(x.type));
                    }

                    // search string
                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        var searchstring = requestModel.search_string.ToLower().ToString();
                        query = query.Where(x => (x.name.ToLower().Contains(searchstring) || x.model.ToLower().Contains(searchstring) || x.mac_address.ToLower().Contains(searchstring) ||
                        x.brand.ToLower().Contains(searchstring) || x.device_code.ToLower() == searchstring || x.device_uuid.ToString() == searchstring
                        || x.os.ToLower().Contains(searchstring) || x.version.Contains(searchstring) || x.type.Contains(searchstring)));
                    }

                    deviceInfos = query.OrderByDescending(x=>x.created_at).ToList();
                }
            }
            return deviceInfos;
        }

        public async Task<List<DeviceInfo>> FilterDeviceTypeOptions(FilterDeviceRequestModel requestModel)
        {
            List<DeviceInfo> deviceInfos = new List<DeviceInfo>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
                {
                    rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(rolename))
                {
                    IQueryable<DeviceInfo> query = context.DeviceInfo;

                    if (rolename == GlobalConstants.Admin)
                    {
                        if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                            {
                                //query = query.Where(x => x.last_sync_site_id != null && companysites.Select(x => x.site_id).ToList().Contains(x.last_sync_site_id.Value));
                                query = query.Where(x => x.company_id != null && x.company_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id);
                            }
                            else
                            {
                                query = query.Where(x => x.last_sync_site_id != null && companysites.Select(x => x.site_id).ToList().Contains(x.last_sync_site_id.Value) && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                            }
                        }
                        else if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.AllCompanyType)
                        {
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status != (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.last_sync_site_id != null && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                            }
                        }
                    }
                    else if (rolename == GlobalConstants.CompanyAdmin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Where(x => x.company_id != null && x.company_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id);
                            //query = query.Where(x => x.last_sync_site_id != null && usersites.Contains(x.last_sync_site_id.Value));
                        }
                        else
                        {
                            query = query.Where(x => x.last_sync_site_id != null && usersites.Contains(x.last_sync_site_id.Value) && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                        }
                    }

                    if (requestModel.status > 0)
                    {
                        if (requestModel.status == (int)Status.Active)
                        {
                            query = query.Where(x => x.is_approved == true);
                        }
                        else if (requestModel.status == (int)Status.Deactive)
                        {
                            query = query.Where(x => x.is_approved == false);
                        }
                    }

                    if (requestModel.site_id?.Count > 0)
                    {
                        query = query.Where(x => x.last_sync_site_id != null && requestModel.site_id.Contains(x.last_sync_site_id.Value.ToString()));
                    }

                    if (requestModel.company_id?.Count > 0)
                    {
                        //query = query.Where(x => x.last_sync_site_id != null && requestModel.company_id.Contains(x.company_id.ToString()));
                        query = query.Where(x => requestModel.company_id.Contains(x.company_id.ToString()));
                    }


                    // add model name Filter
                    if (requestModel.model?.Count > 0)
                    {
                        query = query.Where(x => requestModel.model.Contains(x.model));
                    }

                    // add brand Filter
                    if (requestModel.brand?.Count > 0)
                    {
                        query = query.Where(x => requestModel.brand.Contains(x.brand));
                    }

                    // add os Filter
                    if (requestModel.os?.Count > 0)
                    {
                        query = query.Where(x => requestModel.os.Contains(x.os));
                    }

                    // add type Filter
                    if (requestModel.type?.Count > 0)
                    {
                        query = query.Where(x => requestModel.type.Contains(x.type));
                    }

                    // search string
                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        var searchstring = requestModel.search_string.ToLower().ToString();
                        query = query.Where(x => (x.name.ToLower().Contains(searchstring) || x.model.ToLower().Contains(searchstring) || x.mac_address.ToLower().Contains(searchstring) ||
                        x.brand.ToLower().Contains(searchstring) || x.device_code.ToLower() == searchstring || x.device_uuid.ToString() == searchstring
                        || x.os.ToLower().Contains(searchstring) || x.version.Contains(searchstring) || x.type.Contains(searchstring)));
                    }

                    // search string
                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        var searchstring = requestModel.option_search_string.ToLower().ToString();
                        query = query.Where(x => x.type.ToLower().Contains(searchstring));
                    }

                    deviceInfos = query.OrderByDescending(x => x.created_at).ToList();
                }
            }
            return deviceInfos;
        }

        public async Task<List<DeviceInfo>> FilterDeviceBrandOptions(FilterDeviceRequestModel requestModel)
        {
            List<DeviceInfo> deviceInfos = new List<DeviceInfo>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
                {
                    rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(rolename))
                {
                    IQueryable<DeviceInfo> query = context.DeviceInfo;

                    if (rolename == GlobalConstants.Admin)
                    {
                        if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                            {
                                //query = query.Where(x => x.last_sync_site_id != null && companysites.Select(x => x.site_id).ToList().Contains(x.last_sync_site_id.Value));
                                query = query.Where(x => x.company_id != null && x.company_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id);
                            }
                            else
                            {
                                query = query.Where(x => x.last_sync_site_id != null && companysites.Select(x => x.site_id).ToList().Contains(x.last_sync_site_id.Value) && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                            }
                        }
                        else if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.AllCompanyType)
                        {
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status != (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.last_sync_site_id != null && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                            }
                        }
                    }
                    else if (rolename == GlobalConstants.CompanyAdmin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Where(x => x.company_id != null && x.company_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id);
                            //query = query.Where(x => x.last_sync_site_id != null && usersites.Contains(x.last_sync_site_id.Value));
                        }
                        else
                        {
                            query = query.Where(x => x.last_sync_site_id != null && usersites.Contains(x.last_sync_site_id.Value) && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                        }
                    }

                    if (requestModel.status > 0)
                    {
                        if (requestModel.status == (int)Status.Active)
                        {
                            query = query.Where(x => x.is_approved == true);
                        }
                        else if (requestModel.status == (int)Status.Deactive)
                        {
                            query = query.Where(x => x.is_approved == false);
                        }
                    }

                    if (requestModel.site_id?.Count > 0)
                    {
                        query = query.Where(x => x.last_sync_site_id != null && requestModel.site_id.Contains(x.last_sync_site_id.Value.ToString()));
                    }

                    if (requestModel.company_id?.Count > 0)
                    {
                        //query = query.Where(x => x.last_sync_site_id != null && requestModel.company_id.Contains(x.company_id.ToString()));
                        query = query.Where(x => requestModel.company_id.Contains(x.company_id.ToString()));
                    }


                    // add model name Filter
                    if (requestModel.model?.Count > 0)
                    {
                        query = query.Where(x => requestModel.model.Contains(x.model));
                    }

                    // add brand Filter
                    if (requestModel.brand?.Count > 0)
                    {
                        query = query.Where(x => requestModel.brand.Contains(x.brand));
                    }

                    // add os Filter
                    if (requestModel.os?.Count > 0)
                    {
                        query = query.Where(x => requestModel.os.Contains(x.os));
                    }

                    // add type Filter
                    if (requestModel.type?.Count > 0)
                    {
                        query = query.Where(x => requestModel.type.Contains(x.type));
                    }

                    // search string
                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        var searchstring = requestModel.search_string.ToLower().ToString();
                        query = query.Where(x => (x.name.ToLower().Contains(searchstring) || x.model.ToLower().Contains(searchstring) || x.mac_address.ToLower().Contains(searchstring) ||
                        x.brand.ToLower().Contains(searchstring) || x.device_code.ToLower() == searchstring || x.device_uuid.ToString() == searchstring
                        || x.os.ToLower().Contains(searchstring) || x.version.Contains(searchstring) || x.type.Contains(searchstring)));
                    }

                    // search string
                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        var searchstring = requestModel.option_search_string.ToLower().ToString();
                        query = query.Where(x => x.brand.ToLower().Contains(searchstring));
                    }

                    deviceInfos = query.OrderByDescending(x => x.created_at).ToList();
                }
            }
            return deviceInfos;
        }

        public async Task<List<DeviceInfo>> FilterDeviceModelOptions(FilterDeviceRequestModel requestModel)
        {
            List<DeviceInfo> deviceInfos = new List<DeviceInfo>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
                {
                    rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(rolename))
                {
                    IQueryable<DeviceInfo> query = context.DeviceInfo;

                    if (rolename == GlobalConstants.Admin)
                    {
                        if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                            {
                                //query = query.Where(x => x.last_sync_site_id != null && companysites.Select(x => x.site_id).ToList().Contains(x.last_sync_site_id.Value));
                                query = query.Where(x => x.company_id != null && x.company_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id);
                            }
                            else
                            {
                                query = query.Where(x => x.last_sync_site_id != null && companysites.Select(x => x.site_id).ToList().Contains(x.last_sync_site_id.Value) && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                            }
                        }
                        else if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.AllCompanyType)
                        {
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status != (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.last_sync_site_id != null && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                            }
                        }
                    }
                    else if (rolename == GlobalConstants.CompanyAdmin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Where(x => x.company_id != null && x.company_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id);
                            //query = query.Where(x => x.last_sync_site_id != null && usersites.Contains(x.last_sync_site_id.Value));
                        }
                        else
                        {
                            query = query.Where(x => x.last_sync_site_id != null && usersites.Contains(x.last_sync_site_id.Value) && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                        }
                    }

                    if (requestModel.status > 0)
                    {
                        if (requestModel.status == (int)Status.Active)
                        {
                            query = query.Where(x => x.is_approved == true);
                        }
                        else if (requestModel.status == (int)Status.Deactive)
                        {
                            query = query.Where(x => x.is_approved == false);
                        }
                    }

                    if (requestModel.site_id?.Count > 0)
                    {
                        query = query.Where(x => x.last_sync_site_id != null && requestModel.site_id.Contains(x.last_sync_site_id.Value.ToString()));
                    }

                    if (requestModel.company_id?.Count > 0)
                    {
                        //query = query.Where(x => x.last_sync_site_id != null && requestModel.company_id.Contains(x.company_id.ToString()));
                        query = query.Where(x => requestModel.company_id.Contains(x.company_id.ToString()));
                    }


                    // add model name Filter
                    if (requestModel.model?.Count > 0)
                    {
                        query = query.Where(x => requestModel.model.Contains(x.model));
                    }

                    // add brand Filter
                    if (requestModel.brand?.Count > 0)
                    {
                        query = query.Where(x => requestModel.brand.Contains(x.brand));
                    }

                    // add os Filter
                    if (requestModel.os?.Count > 0)
                    {
                        query = query.Where(x => requestModel.os.Contains(x.os));
                    }

                    // add type Filter
                    if (requestModel.type?.Count > 0)
                    {
                        query = query.Where(x => requestModel.type.Contains(x.type));
                    }

                    // search string
                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        var searchstring = requestModel.search_string.ToLower().ToString();
                        query = query.Where(x => (x.name.ToLower().Contains(searchstring) || x.model.ToLower().Contains(searchstring) || x.mac_address.ToLower().Contains(searchstring) ||
                        x.brand.ToLower().Contains(searchstring) || x.device_code.ToLower() == searchstring || x.device_uuid.ToString() == searchstring
                        || x.os.ToLower().Contains(searchstring) || x.version.Contains(searchstring) || x.type.ToLower().Contains(searchstring)));
                    }

                    // search string
                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        var searchstring = requestModel.option_search_string.ToLower().ToString();
                        query = query.Where(x => x.model.ToLower().Contains(searchstring));
                    }

                    deviceInfos = query.OrderByDescending(x => x.created_at).ToList();
                }
            }
            return deviceInfos;
        }

        public async Task<List<DeviceInfo>> FilterDeviceOSOptions(FilterDeviceRequestModel requestModel)
        {
            List<DeviceInfo> deviceInfos = new List<DeviceInfo>();
            if (UpdatedGenericRequestmodel.CurrentUser.requested_by != null && UpdatedGenericRequestmodel.CurrentUser.requested_by != Guid.Empty)
            {
                string rolename = string.Empty;
                //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
                {
                    rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(rolename))
                {
                    IQueryable<DeviceInfo> query = context.DeviceInfo;

                    if (rolename == GlobalConstants.Admin)
                    {
                        if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                            {
                                //query = query.Where(x => x.last_sync_site_id != null && companysites.Select(x => x.site_id).ToList().Contains(x.last_sync_site_id.Value));
                                query = query.Where(x => x.company_id != null && x.company_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id);
                            }
                            else
                            {
                                query = query.Where(x => x.last_sync_site_id != null && companysites.Select(x => x.site_id).ToList().Contains(x.last_sync_site_id.Value) && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                            }
                        }
                        else if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.AllCompanyType)
                        {
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status != (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.last_sync_site_id != null && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                            }
                        }
                    }
                    else if (rolename == GlobalConstants.CompanyAdmin)
                    {
                        var usersites = context.UserSites.Where(x => x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                        if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                        {
                            query = query.Where(x => x.company_id != null && x.company_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id);
                            //query = query.Where(x => x.last_sync_site_id != null && usersites.Contains(x.last_sync_site_id.Value));
                        }
                        else
                        {
                            query = query.Where(x => x.last_sync_site_id != null && usersites.Contains(x.last_sync_site_id.Value) && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                        }
                    }

                    if (requestModel.status > 0)
                    {
                        if (requestModel.status == (int)Status.Active)
                        {
                            query = query.Where(x => x.is_approved == true);
                        }
                        else if (requestModel.status == (int)Status.Deactive)
                        {
                            query = query.Where(x => x.is_approved == false);
                        }
                    }

                    if (requestModel.site_id?.Count > 0)
                    {
                        query = query.Where(x => x.last_sync_site_id != null && requestModel.site_id.Contains(x.last_sync_site_id.Value.ToString()));
                    }

                    if (requestModel.company_id?.Count > 0)
                    {
                        //query = query.Where(x => x.last_sync_site_id != null && requestModel.company_id.Contains(x.company_id.ToString()));
                        query = query.Where(x => requestModel.company_id.Contains(x.company_id.ToString()));
                    }


                    // add model name Filter
                    if (requestModel.model?.Count > 0)
                    {
                        query = query.Where(x => requestModel.model.Contains(x.model));
                    }

                    // add brand Filter
                    if (requestModel.brand?.Count > 0)
                    {
                        query = query.Where(x => requestModel.brand.Contains(x.brand));
                    }

                    // add os Filter
                    if (requestModel.os?.Count > 0)
                    {
                        query = query.Where(x => requestModel.os.Contains(x.os));
                    }

                    // add type Filter
                    if (requestModel.type?.Count > 0)
                    {
                        query = query.Where(x => requestModel.type.Contains(x.type));
                    }

                    // search string
                    if (!string.IsNullOrEmpty(requestModel.search_string))
                    {
                        var searchstring = requestModel.search_string.ToLower().ToString();
                        query = query.Where(x => (x.name.ToLower().Contains(searchstring) || x.model.ToLower().Contains(searchstring) || x.mac_address.ToLower().Contains(searchstring) ||
                        x.brand.ToLower().Contains(searchstring) || x.device_code.ToLower() == searchstring || x.device_uuid.ToString() == searchstring
                        || x.os.ToLower().Contains(searchstring) || x.version.Contains(searchstring) || x.type.ToLower().Contains(searchstring)));
                    }

                    // search string
                    if (!string.IsNullOrEmpty(requestModel.option_search_string))
                    {
                        var searchstring = requestModel.option_search_string.ToLower().ToString();
                        query = query.Where(x => x.os.ToLower().Contains(searchstring));
                    }

                    deviceInfos = query.OrderByDescending(x => x.created_at).ToList();
                }
            }
            return deviceInfos;
        }

        public List<DeviceInfo> GetAllDevices(string userid)
        {
            List<DeviceInfo> deviceInfos = new List<DeviceInfo>();
            if (userid != null)
            {
                string rolename = string.Empty;
                //var role = context.User.Include(x => x.Role_App).Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).FirstOrDefault();
                if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
                {
                    rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(rolename))
                {
                    IQueryable<DeviceInfo> query = context.DeviceInfo;

                    if (rolename == GlobalConstants.Admin)
                    {
                        if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.Active)
                        {
                            var companysites = context.Sites.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id && x.status == (int)Status.Active).Include(x => x.Company).ToList();
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.last_sync_site_id != null && companysites.Select(x => x.site_id).ToList().Contains(x.last_sync_site_id.Value));
                            }
                            else
                            {
                                query = query.Where(x => x.last_sync_site_id != null && companysites.Select(x => x.site_id).ToList().Contains(x.last_sync_site_id.Value) && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                            }
                        }
                        else if (UpdatedGenericRequestmodel.CurrentUser.company_status == (int)Status.AllCompanyType)
                        {
                            if (UpdatedGenericRequestmodel.CurrentUser.site_status != (int)Status.AllSiteType)
                            {
                                query = query.Where(x => x.last_sync_site_id != null && x.last_sync_site_id.Value.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id);
                            }
                        }
                    }

                    deviceInfos = query.ToList();
                }
            }
            return deviceInfos.OrderByDescending(x => x.created_at).ToList();
        }
    }
}
