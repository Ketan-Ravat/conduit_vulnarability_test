using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using Jarvis.Repo.Abstract;
using Jarvis.ViewModels;
using System.Threading.Tasks;

namespace Jarvis.Repo
{
    public interface IJarvisUOW
    {
        IUserRepository UserRepository { get; }

        ICompanyRepository<TEntity> CompanyRepository<TEntity>() where TEntity : class;

        IBaseGenericRepository<TEntity> BaseGenericRepository<TEntity>() where TEntity : class;

        IFormAttributesRepository<TEntity> FormAttributesRepository<TEntity>() where TEntity : class;

        IInspectionFormRepository InspectionFormRepository{ get; }

        IAssetRepository AssetRepository { get; }

        IDeviceRepository DeviceRepository { get; }

        ISiteRepository SiteRepository { get; }

        IInspectionRepository InspectionRepository { get; }

        IIssueRepository IssueRepository { get; }

        IPMCategoryRepository PMCategoryRepository { get; }

        IPMPlansRepository PMPlansRepository { get; }

        ITaskRepository TaskRepository { get; }
        IPMRepository PMRepository { get; }
        IAssetPMPlansRepository AssetPMPlansRepository { get; }
        IAssetPMsRepository AssetPMsRepository { get; }
        IPMTriggersRepository PMTriggersRepository { get; }
        IPMTriggersRemarksRepository PMTriggersRemarksRepository { get; }
        ICompletedPMTriggerRepository CompletedPMTriggerRepository { get; }
        IPMTriggersTasksRepository PMTriggersTasksRepository { get; }
        IPMNotificationRepository PMNotificationRepository { get; }
        IMaintenanceRequestRepository MRRepository { get; }
        IWorkOrderRepository WorkOrderRepository { get; }
        IMobileWorkorderRepository MobileWorkOrderRepository { get; }
        IFormIORepository formIORepository { get; }
        IAssetFormIORepository AssetFormIORepository { get; }
        IFormIOAssetClassRepository FormIOAssetClassRepository { get; }
        IDashboardRepository DashboardRepository { get; }
        IDbContextTransaction BeginTransaction();
        Int64 SaveChanges();
        void RollbackTransaction();
        void CommitTransaction();
    }
}
