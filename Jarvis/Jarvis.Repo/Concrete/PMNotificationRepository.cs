using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete {
    public class PMNotificationRepository : BaseGenericRepository<CompanyPMNotificationConfigurations>, IPMNotificationRepository {
        public PMNotificationRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }
        public virtual async Task<int> Insert(CompanyPMNotificationConfigurations entity)
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
                    Add(entity);
                    IsSuccess = (int)ResponseStatusNumber.Success;
                }
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public virtual async Task<int> Update(CompanyPMNotificationConfigurations entity)
        {
            int IsSuccess = 0;
            try
            {
                dbSet.Update(entity);
                IsSuccess = await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public async Task<CompanyPMNotificationConfigurations> GetByCompanyId(Guid company_id)
        {
            return await context.CompanyPMNotificationConfigurations.Where(u => u.company_id == company_id && u.status == (int)Status.Active).FirstOrDefaultAsync();
        }

        public async Task<AssetPMNotificationConfigurations> GetByAssetId(Guid asset_id)
        {
            return await context.AssetPMNotificationConfigurations.Where(u => u.asset_id == asset_id && u.status == (int)Status.Active).Include(x => x.Asset).FirstOrDefaultAsync();
        }

        public async Task<List<CompanyPMNotificationConfigurations>> GetAllPMNotificationConfiguration()
        {
            return await context.CompanyPMNotificationConfigurations.Where(u => u.status == (int)Status.Active).Include(x => x.Company).ToListAsync();
        }

        public async Task<SentPMNotification> GetSentPMNotification(Guid userid, Guid triggerid, int notification_type)
        {
            return await context.SentPMNotification.Where(u => u.manager_id == userid && u.trigger_id == triggerid && u.notification_type == notification_type).FirstOrDefaultAsync();
        }

        public async Task<ManagerPMNotificationConfiguration> GetPMItemNotificationConfig(Guid userid, Guid triggerid)
        {
            return await context.ManagerPMNotificationConfiguration.Where(u => u.user_id == userid && u.pm_trigger_id == triggerid).FirstOrDefaultAsync();
        }

        public async Task<bool> GetFirstSentNotification(Guid userid, Guid triggerid)
        {
            var response = await context.SentPMNotification.Where(u => u.manager_id == userid && u.trigger_id == triggerid &&
            (u.notification_type == (int)NotificationStatus.FirstDueDateReminder || u.notification_type == (int)NotificationStatus.FirstMeterHoursDueReminder)).FirstOrDefaultAsync();
            if (response != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> GetSecondSentNotification(Guid userid, Guid triggerid)
        {
            var response = await context.SentPMNotification.Where(u => u.manager_id == userid && u.trigger_id == triggerid &&
            (u.notification_type == (int)NotificationStatus.SecondDueDateReminder || u.notification_type == (int)NotificationStatus.SecondMeterHoursDueReminder)).FirstOrDefaultAsync();
            if (response != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> GetPMDueSentNotification(Guid userid, Guid triggerid)
        {
            var response = await context.SentPMNotification.Where(u => u.manager_id == userid && u.trigger_id == triggerid &&
            (u.notification_type == (int)NotificationStatus.OnDueDateReminder || u.notification_type == (int)NotificationStatus.OnMeterHoursDueReminder)).FirstOrDefaultAsync();
            if (response != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
