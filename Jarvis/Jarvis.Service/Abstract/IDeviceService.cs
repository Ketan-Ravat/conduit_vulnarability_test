using Jarvis.db.Models;
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
    public interface IDeviceService
    {
        Task<DeviceRegisterResponseModel> Register(DeviceInfoRequestModel requestModel);
        int GetDeviceAuthinticationStatus(Guid device_uuid);
        Task<int> Update(int device_info_id, DeviceInfoRequestModel requestModel);
        Task<int> UpdateDeviceStatus(int device_info_id, UpdateDeviceStatusRequestModel requestModel);
        Task RemoveSyncRecord();
        Task<ListViewModel<DeviceInfoViewModel>> FilterDevice(FilterDeviceRequestModel requestModel);
        Task<ListViewModel<string>> FilterDeviceTypeOptions(FilterDeviceRequestModel requestModel);
        Task<ListViewModel<string>> FilterDeviceBrandOptions(FilterDeviceRequestModel requestModel);
        Task<ListViewModel<string>> FilterDeviceModelOptions(FilterDeviceRequestModel requestModel);
        Task<ListViewModel<string>> FilterDeviceOSOptions(FilterDeviceRequestModel requestModel);
        Task<ListViewModel<SitesViewModel>> FilterDeviceSitesOptions(FilterDeviceRequestModel requestModel);
        Task<ListViewModel<CompanyViewModel>> FilterDeviceCompanyOptions(FilterDeviceRequestModel requestModel);
        List<string> GetAllDeviceModelList();
        List<string> GetAllDeviceOSList();
        List<string> GetAllDeviceBrandList();
        List<string> GetAllDeviceTypesList();
        Task<int> UpdateDeviceAppVersion(Guid device_uuid, string app_version);
        DeviceInfo GetdevicecodeByDeviceId(Guid device_uuid);
        Task<int> UpdateDeviceGeoLocation(UpdateDeviceGeoLocationRequestModel requestModel);
    }
}
