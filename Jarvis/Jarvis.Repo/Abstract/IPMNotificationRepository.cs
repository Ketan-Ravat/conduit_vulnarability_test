using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IPMNotificationRepository
    {
        Task<int> Insert(CompanyPMNotificationConfigurations entity);
        Task<int> Update(CompanyPMNotificationConfigurations entity);
        Task<CompanyPMNotificationConfigurations> GetByCompanyId(Guid company_id);
        Task<AssetPMNotificationConfigurations> GetByAssetId(Guid asset_id);
        Task<List<CompanyPMNotificationConfigurations>> GetAllPMNotificationConfiguration();
        Task<SentPMNotification> GetSentPMNotification(Guid userid, Guid triggerid, int notification_type);
        Task<ManagerPMNotificationConfiguration> GetPMItemNotificationConfig(Guid userid, Guid triggerid);
        Task<bool> GetFirstSentNotification(Guid userid, Guid triggerid);
        Task<bool> GetSecondSentNotification(Guid userid, Guid triggerid);
        Task<bool> GetPMDueSentNotification(Guid userid, Guid triggerid);
    }
}
