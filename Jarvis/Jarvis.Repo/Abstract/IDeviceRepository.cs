using Jarvis.db.Models;
using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IDeviceRepository
    {
        DeviceInfo GetDeviceInfoByUUId(Guid device_uuid);
        DeviceInfo GetDeviceInfo(Guid device_uuid, string device_code);
        DeviceInfo GetDeviceInfoById(int device_info_id);
        List<RecordSyncInformation> FindSyncRecord();
        Task<List<DeviceInfo>> FilterDevice(FilterDeviceRequestModel requestModel);
        Task<List<DeviceInfo>> FilterDeviceTypeOptions(FilterDeviceRequestModel requestModel);
        Task<List<DeviceInfo>> FilterDeviceBrandOptions(FilterDeviceRequestModel requestModel);
        Task<List<DeviceInfo>> FilterDeviceModelOptions(FilterDeviceRequestModel requestModel);
        Task<List<DeviceInfo>> FilterDeviceOSOptions(FilterDeviceRequestModel requestModel);
        List<DeviceInfo> GetAllDevices(string userid);
    }
}
