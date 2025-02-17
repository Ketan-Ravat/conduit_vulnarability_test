using AutoMapper;
using Jarvis.db.Models;
using Jarvis.Service.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Jarvis.Service.Concrete
{
    public class DeviceService : BaseService, IDeviceService
    {
        public readonly IMapper _mapper;

        private Shared.Utility.Logger _logger;

        public DeviceService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            _logger = Shared.Utility.Logger.GetInstance<DeviceService>();
        }

        public async Task<DeviceRegisterResponseModel> Register(DeviceInfoRequestModel requestModel)
        {
            DeviceRegisterResponseModel responseModel = new DeviceRegisterResponseModel();
            try
            {
                DeviceInfo deviceInfo = _UoW.DeviceRepository.GetDeviceInfo(requestModel.device_uuid, requestModel.device_code);
                if (deviceInfo == null)
                {
                    if (!String.IsNullOrEmpty(requestModel.company_code))
                    {
                        var company = await _UoW.CompanyRepository<Company>().GetCompanyByCode(requestModel.company_code);
                        if (company != null && company.company_id != null)
                        {
                            requestModel.company_id = company.company_id;
                            deviceInfo = _mapper.Map<DeviceInfo>(requestModel);
                            deviceInfo.is_approved = true;
                            deviceInfo.created_at = DateTime.UtcNow;
                            var insert = await _UoW.BaseGenericRepository<DeviceInfo>().Insert(deviceInfo);
                            if (insert)
                            {
                                _UoW.SaveChanges();
                                responseModel.status = (int)ResponseStatusNumber.Success;
                                responseModel.company_code = company.company_code;
                                responseModel.domain_name = company.domain_name;
                                responseModel.company_name = company.company_name;
                                responseModel.company_id = company.company_id.ToString();
                                responseModel.device_code = deviceInfo.device_code;
                                responseModel.device_uuid = deviceInfo.device_uuid.ToString();
                                responseModel.identity_pool_id = company.identity_pool_id;
                                responseModel.user_pool_id = company.user_pool_id;
                                responseModel.region = company.region;
                                responseModel.company_logo = company.company_logo;
                                responseModel.company_thumbnail_logo = company.company_thumbnail_logo;
                                responseModel.user_pool_web_client_id = company.user_pool_web_client_id;
                                responseModel.cognito_mfa_timer = company.cognito_mfa_timer;
                            }
                            else
                            {
                                responseModel.status = (int)ResponseStatusNumber.Error;
                            }
                        }
                        else
                        {
                            responseModel.status = (int)ResponseStatusNumber.NotFoundCompanyCode;
                        }
                    }
                    else
                    {
                        deviceInfo = _mapper.Map<DeviceInfo>(requestModel);
                        deviceInfo.created_at = DateTime.UtcNow;
                        var insert = await _UoW.BaseGenericRepository<DeviceInfo>().Insert(deviceInfo);
                        if (insert)
                        {
                            _UoW.SaveChanges();
                            responseModel.status = (int)ResponseStatusNumber.Success;
                            responseModel.device_code = deviceInfo.device_code;
                            responseModel.device_uuid = deviceInfo.device_uuid.ToString();
                        }
                        else
                        {
                            responseModel.status = (int)ResponseStatusNumber.Error;
                        }
                    }
                }
                else
                {
                    if (deviceInfo?.is_approved == true)
                    {
                        if (!String.IsNullOrEmpty(requestModel.company_code))
                        {
                            var company = await _UoW.CompanyRepository<Company>().GetCompanyByCode(requestModel.company_code);
                            if (company != null && company.company_id != null)
                            {
                                requestModel.company_id = company.company_id;
                                //if (deviceInfo.company_id == requestModel.company_id)
                                //{
                                deviceInfo.company_id = requestModel.company_id;
                                deviceInfo.modified_at = DateTime.UtcNow;
                                var updated = await _UoW.BaseGenericRepository<DeviceInfo>().Update(deviceInfo);
                                if (updated)
                                {
                                    _UoW.SaveChanges();
                                    responseModel.status = (int)ResponseStatusNumber.Success;
                                    responseModel.company_code = company.company_code;
                                    responseModel.company_name = company.company_name;
                                    responseModel.company_id = company.company_id.ToString();
                                    responseModel.device_code = deviceInfo.device_code;
                                    responseModel.device_uuid = deviceInfo.device_uuid.ToString();
                                    responseModel.company_logo = company.company_logo;
                                    responseModel.company_thumbnail_logo = company.company_thumbnail_logo;
                                    responseModel.domain_name = company.domain_name;
                                    responseModel.identity_pool_id = company.identity_pool_id;
                                    responseModel.user_pool_id = company.user_pool_id;
                                    responseModel.region = company.region;
                                    responseModel.user_pool_web_client_id = company.user_pool_web_client_id;
                                }
                                else
                                {
                                    responseModel.status = (int)ResponseStatusNumber.Error;
                                }
                                /*}
                                else
                                {
                                    responseModel.status = (int)ResponseStatusNumber.DeviceUnAuthorized;
                                }*/
                            }
                            else
                            {
                                responseModel.status = (int)ResponseStatusNumber.NotFoundCompanyCode;
                            }
                        }
                        else
                        {
                            responseModel.status = (int)ResponseStatusNumber.DeviceUnAuthorized;
                        }
                    }
                    else
                    {
                        responseModel.status = (int)ResponseStatusNumber.DeviceUnAuthorized;
                    }
                    //return (int)ResponseStatusNumber.AlreadyExists;
                }
                return responseModel;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int GetDeviceAuthinticationStatus(Guid device_uuid)
        {
            try
            {
                DeviceInfo deviceInfo = _UoW.DeviceRepository.GetDeviceInfoByUUId(device_uuid);
                if (deviceInfo != null)
                {
                    if (deviceInfo.is_approved)
                    {
                        return (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        return (int)ResponseStatusNumber.False;
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

        public async Task<int> Update(int device_info_id, DeviceInfoRequestModel requestModel)
        {
            try
            {
                DeviceInfo deviceInfo = _UoW.DeviceRepository.GetDeviceInfoById(device_info_id);
                if (deviceInfo != null)
                {
                    deviceInfo = _mapper.Map(requestModel, deviceInfo);
                    deviceInfo.modified_at = DateTime.UtcNow;
                    var update = await _UoW.BaseGenericRepository<DeviceInfo>().Update(deviceInfo);
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
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int> UpdateDeviceStatus(int device_info_id, UpdateDeviceStatusRequestModel requestModel)
        {
            try
            {
                DeviceInfo deviceInfo = _UoW.DeviceRepository.GetDeviceInfoById(device_info_id);
                if (deviceInfo != null)
                {
                    deviceInfo.modified_at = DateTime.UtcNow;
                    deviceInfo.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    deviceInfo.is_approved = requestModel.status;
                    var update = await _UoW.BaseGenericRepository<DeviceInfo>().Update(deviceInfo);
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
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task RemoveSyncRecord()
        {
            try
            {
                List<RecordSyncInformation> records = _UoW.DeviceRepository.FindSyncRecord();
                if (records.Count > 0)
                {
                    bool delete = await _UoW.BaseGenericRepository<RecordSyncInformation>().Delete(records);
                    if (!delete)
                    {
                        // Nothing to Do
                        _logger.LogError("delete sync record using scheduler: ", "Something went wrong");
                    }
                }
            }
            catch (Exception e)
            {
                // Nothing to Do
                _logger.LogError("Error in delete sync record using scheduler: ", e.ToString());
            }
        }

        public async Task<ListViewModel<DeviceInfoViewModel>> FilterDevice(FilterDeviceRequestModel requestModel)
        {
            try
            {
                FilterDeviceResponseModel filterResponse = new FilterDeviceResponseModel();
                ListViewModel<DeviceInfoViewModel> devices = new ListViewModel<DeviceInfoViewModel>();
                var devicelist = await _UoW.DeviceRepository.FilterDevice(requestModel);

                if (devicelist.Count > 0)
                {
                    filterResponse.brands = devicelist.Where(x => !string.IsNullOrEmpty(x.brand)).Select(x => x.brand).Distinct().ToList();
                    filterResponse.types = devicelist.Where(x => !string.IsNullOrEmpty(x.type)).Select(x => x.type).Distinct().ToList();
                    filterResponse.model = devicelist.Where(x => !string.IsNullOrEmpty(x.model)).Select(x => x.model).Distinct().ToList();
                    filterResponse.os = devicelist.Where(x => !string.IsNullOrEmpty(x.os)).Select(x => x.os).Distinct().ToList();
                    var siteIds = devicelist.Where(x => x.last_sync_site_id != null).Select(x => x.last_sync_site_id.Value).Distinct().ToList();
                    if(siteIds?.Count > 0)
                    {
                        filterResponse.sites = new List<SitesViewModel>();
                        foreach (var item in siteIds)
                        {
                            var siteDetails = _UoW.SiteRepository.GetSiteById(item.ToString());
                            if (siteDetails != null)
                            {
                                var singleSite = _mapper.Map<SitesViewModel>(siteDetails);
                                singleSite.comapny_name = siteDetails.Company?.company_name;
                                filterResponse.sites.Add(singleSite);
                            }
                        }
                    }
                    int totaldevice = devicelist.Count;
                    if (requestModel.pageindex > 0 && requestModel.pagesize > 0)
                    {
                        devicelist = devicelist.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                    }
                    devices.list = _mapper.Map<List<DeviceInfoViewModel>>(devicelist);
                    if (devices.list?.Count > 0)
                    {
                        devices.list.ForEach(x =>
                        {
                            if (x.last_sync_site_id != null)
                            {
                                var sitedetail = _UoW.SiteRepository.GetSiteById(x.last_sync_site_id.Value.ToString());
                                if (sitedetail != null)
                                {
                                    x.last_sync_site_name = sitedetail.site_name;
                                }
                            }
                            if (x.company_id != null)
                            {
                                var companydetail = _UoW.CompanyRepository<Company>().GetCompanyByID(x.company_id.Value.ToString());
                                if (companydetail != null)
                                {
                                    x.company_name = companydetail.company_name;
                                }
                            }
                        });
                    }
                    devices.listsize = totaldevice;
                    filterResponse.filterData = devices;
                }
                devices.pageSize = requestModel.pagesize;
                devices.pageIndex = requestModel.pageindex;
                return devices;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ListViewModel<string>> FilterDeviceTypeOptions(FilterDeviceRequestModel requestModel)
        {
            try
            {
                ListViewModel<string> devices = new ListViewModel<string>();
                var devicelist = await _UoW.DeviceRepository.FilterDeviceTypeOptions(requestModel);

                if (devicelist?.Count > 0)
                {
                    devices.list = devicelist.Where(x => !string.IsNullOrEmpty(x.type)).Select(x => x.type).Distinct().ToList();
                    devices.listsize = devices.list.Count;
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    devices.list = devices.list.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                }
                devices.pageSize = requestModel.pagesize;
                devices.pageIndex = requestModel.pageindex;
                return devices;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ListViewModel<string>> FilterDeviceBrandOptions(FilterDeviceRequestModel requestModel)
        {
            try
            {
                ListViewModel<string> devices = new ListViewModel<string>();
                var devicelist = await _UoW.DeviceRepository.FilterDeviceBrandOptions(requestModel);

                if (devicelist?.Count > 0)
                {
                    devices.list = devicelist.Where(x => !string.IsNullOrEmpty(x.brand)).Select(x => x.brand).Distinct().ToList();
                    devices.listsize = devices.list.Count;
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    devices.list = devices.list.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                }
                devices.pageSize = requestModel.pagesize;
                devices.pageIndex = requestModel.pageindex;
                return devices;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ListViewModel<string>> FilterDeviceModelOptions(FilterDeviceRequestModel requestModel)
        {
            try
            {
                ListViewModel<string> devices = new ListViewModel<string>();
                var devicelist = await _UoW.DeviceRepository.FilterDeviceModelOptions(requestModel);

                if (devicelist?.Count > 0)
                {
                    devices.list = devicelist.Where(x => !string.IsNullOrEmpty(x.model)).Select(x => x.model).Distinct().ToList();
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    devices.listsize = devices.list.Count;
                    devices.list = devices.list.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                }
                devices.pageSize = requestModel.pagesize;
                devices.pageIndex = requestModel.pageindex;
                return devices;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ListViewModel<string>> FilterDeviceOSOptions(FilterDeviceRequestModel requestModel)
        {
            try
            {
                ListViewModel<string> devices = new ListViewModel<string>();
                var devicelist = await _UoW.DeviceRepository.FilterDeviceOSOptions(requestModel);

                if (devicelist?.Count > 0)
                {
                    devices.list = devicelist.Where(x => !string.IsNullOrEmpty(x.os)).Select(x => x.os).Distinct().ToList();
                    devices.listsize = devices.list.Count;
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    devices.list = devices.list.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                }
                devices.pageSize = requestModel.pagesize;
                devices.pageIndex = requestModel.pageindex;
                return devices;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ListViewModel<SitesViewModel>> FilterDeviceSitesOptions(FilterDeviceRequestModel requestModel)
        {
            try
            {
                ListViewModel<SitesViewModel> devices = new ListViewModel<SitesViewModel>();
                var devicelist = await _UoW.DeviceRepository.FilterDevice(requestModel);

                if (devicelist?.Count > 0)
                {
                    var siteIds = devicelist.Where(x => x.last_sync_site_id != null).Select(x => x.last_sync_site_id.Value).Distinct().ToList();
                    if (siteIds?.Count > 0)
                    {
                        List<SitesViewModel> listSites = new List<SitesViewModel>();
                        foreach (var item in siteIds)
                        {
                            var siteDetails = _UoW.SiteRepository.GetSiteById(item.ToString());
                            if (siteDetails != null)
                            {
                                var singleSite = _mapper.Map<SitesViewModel>(siteDetails);
                                singleSite.comapny_name = siteDetails.Company?.company_name;
                                listSites.Add(singleSite);
                            }
                        }
                        devices.listsize = listSites.Count;
                        devices.list = listSites;
                        // search string
                        if (!string.IsNullOrEmpty(requestModel.option_search_string))
                        {
                            var searchstring = requestModel.option_search_string.ToLower().ToString();
                            devices.list = devices.list.Where(x => x.site_name.ToLower().Contains(searchstring)
                            || x.site_code.ToLower().Contains(searchstring)).ToList();
                        }
                    }
                    if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                    {
                        requestModel.pagesize = 20;
                        requestModel.pageindex = 1;
                    }
                    devices.list = devices.list.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                }
                devices.pageSize = requestModel.pagesize;
                devices.pageIndex = requestModel.pageindex;
                return devices;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ListViewModel<CompanyViewModel>> FilterDeviceCompanyOptions(FilterDeviceRequestModel requestModel)
        {
            try
            {
                ListViewModel<CompanyViewModel> devices = new ListViewModel<CompanyViewModel>();
                var devicelist = await _UoW.DeviceRepository.FilterDevice(requestModel);

                if (devicelist?.Count > 0)
                {
                    int totaldevice = devicelist.Count;
                    var companyIDs = devicelist.Where(x => x.company_id != null).Select(x => x.company_id.Value).Distinct().ToList();
                    if (companyIDs?.Count > 0)
                    {
                        List<CompanyViewModel> listCompany = new List<CompanyViewModel>();
                        foreach (var item in companyIDs)
                        {
                            var companyDetails = _UoW.CompanyRepository<Company>().GetCompanyByID(item.ToString());
                            if (companyDetails != null)
                            {
                                var singleComp = _mapper.Map<CompanyViewModel>(companyDetails);
                                listCompany.Add(singleComp);
                            }
                        }
                        devices.listsize = listCompany.Count;
                        devices.list = listCompany;
                        if (requestModel.pageindex == 0 || requestModel.pagesize == 0)
                        {
                            requestModel.pagesize = 20;
                            requestModel.pageindex = 1;
                        }
                        // search string
                        if (!string.IsNullOrEmpty(requestModel.option_search_string))
                        {
                            var searchstring = requestModel.option_search_string.ToLower().ToString();
                            devices.list = devices.list.Where(x => x.company_name.ToLower().Contains(searchstring) 
                            || x.company_code.ToLower().Contains(searchstring)).ToList();
                        }
                    }
                    devices.list = devices.list.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize).ToList();
                }
                devices.pageSize = requestModel.pagesize;
                devices.pageIndex = requestModel.pageindex;
                return devices;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<string> GetAllDeviceModelList()
        {
            try
            {
                List<string> responseModels = new List<string>();

                List<DeviceInfo> deviceInfos = _UoW.DeviceRepository.GetAllDevices(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());

                if (deviceInfos.Count() > 0)
                {
                    responseModels = deviceInfos.Where(x => !string.IsNullOrEmpty(x.model)).Select(x => x.model).Distinct().ToList();
                }

                return responseModels;
            }
            catch { throw; }
        }

        public List<string> GetAllDeviceOSList()
        {
            try
            {
                List<string> responseModels = new List<string>();

                List<DeviceInfo> deviceInfos = _UoW.DeviceRepository.GetAllDevices(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());

                if (deviceInfos.Count() > 0)
                {
                    responseModels = deviceInfos.Where(x => !string.IsNullOrEmpty(x.os)).Select(x => x.os).Distinct().ToList();
                }

                return responseModels;
            }
            catch { throw; }
        }

        public List<string> GetAllDeviceBrandList()
        {
            try
            {
                List<string> responseModels = new List<string>();

                List<DeviceInfo> deviceInfos = _UoW.DeviceRepository.GetAllDevices(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());

                if (deviceInfos.Count() > 0)
                {
                    responseModels = deviceInfos.Where(x => !string.IsNullOrEmpty(x.brand)).Select(x => x.brand).Distinct().ToList();
                }

                return responseModels;
            }
            catch { throw; }
        }

        public List<string> GetAllDeviceTypesList()
        {
            try
            {
                List<string> responseModels = new List<string>();

                List<DeviceInfo> deviceInfos = _UoW.DeviceRepository.GetAllDevices(UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());

                if (deviceInfos.Count() > 0)
                {
                    responseModels = deviceInfos.Where(x => !string.IsNullOrEmpty(x.type)).Select(x => x.type).Distinct().ToList();
                }

                return responseModels;
            }
            catch { throw; }
        }

        public async Task<int> UpdateDeviceAppVersion(Guid device_uuid, string app_version)
        {
            try
            {
                DeviceInfo deviceInfo = _UoW.DeviceRepository.GetDeviceInfoByUUId(device_uuid);
                if (deviceInfo != null)
                {
                    deviceInfo.modified_at = DateTime.UtcNow;
                    if (!string.IsNullOrEmpty(app_version))
                    {
                        deviceInfo.app_version = app_version;
                    }
                    else
                    {
                        return (int)ResponseStatusNumber.InvalidData;
                    }
                    var update = await _UoW.BaseGenericRepository<DeviceInfo>().Update(deviceInfo);
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
            catch (Exception e)
            {
                throw e;
            }
        }

        public DeviceInfo GetdevicecodeByDeviceId(Guid device_uuid)
        {
            return _UoW.DeviceRepository.GetDeviceInfoByUUId(device_uuid);
        }
        public async Task<int> UpdateDeviceGeoLocation(UpdateDeviceGeoLocationRequestModel requestModel)
        {
            int res = (int)ResponseStatusNumber.Error;
            try
            {
                var get_device_info = _UoW.DeviceRepository.GetDeviceInfoByUUId(requestModel.device_uuid);
                if(get_device_info != null)
                {
                    get_device_info.is_location_enabled = requestModel.is_location_enabled;
                    get_device_info.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    get_device_info.modified_at = DateTime.UtcNow;
                    var update = await _UoW.BaseGenericRepository<DeviceInfo>().Update(get_device_info);
                    if (update)
                    {
                        _UoW.SaveChanges();
                        res = (int)ResponseStatusNumber.Success;
                    }
                }
            }
            catch(Exception e)
            {
            }
            return res;
        }
    }
}
