using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IPMNotificationService
    {
        Task<CompanyPMNotificationResponseModel> AddUpdatePMNotification(CompanyPMNotificationRequestModel request);
        Task<AssetPMNotificationResponseModel> AddUpdateAssetPMNotification(AssetPMNotificationRequestModel request);
        Task<CompanyPMNotificationResponseModel> GetPMNotification(Guid company_id);
        Task<AssetPMNotificationResponseModel> GetAssetPMNotification(Guid asset_id);
        Task<int> ExecutePMNotification();
        Task<int> TriggerPMItemNotification(Guid trigger_id, bool is_disabled);
        Task<int> ExecutePMNotificationForVendors();
    }
}
