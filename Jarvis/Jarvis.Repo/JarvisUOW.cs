using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Repo.Concrete;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;
using Jarvis.db.MongoDB;

namespace Jarvis.Repo
{
    public class JarvisUOW : IJarvisUOW , IDisposable
    {
        private readonly DBContextFactory _context;

        private readonly MongoDBContext _mongodbcontext;
        //IUserRepository _userRepository;
        IBaseGenericRepository<User> userRepository;

        private IDbContextTransaction _transaction;
        public Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        //public JarvisUOW(DBContextFactory context,IBaseGenericRepository<User> userRepository)
        //{
        //    this.context = context;
        //    this.userRepository = userRepository;
        //}


        IInspectionFormRepository _InspectionFormRepository;

        IUserRepository _userRepository;

        IAssetRepository _assetRepository;

        ISiteRepository _siteRepository;

        IInspectionRepository _inspectionRepository;

        IIssueRepository _issueRepository;

        IDeviceRepository _deviceRepository;

        IPMCategoryRepository _pmCategoryRepository;

        IPMPlansRepository _pmPlansRepository;

        ITaskRepository _taskRepository;

        IPMRepository _pmRepository;

        IAssetPMPlansRepository _assetPMPlansRepository;
        
        IAssetPMsRepository _assetPMsRepository;

        IPMTriggersRepository _pmTriggersRepository;

        IPMTriggersRemarksRepository _pmTriggersRemarksRepository;

        ICompletedPMTriggerRepository _completedPMTriggerRepository;

        IPMTriggersTasksRepository _pmTriggersTasksRepository;
        IPMNotificationRepository _pmNotificationRepository;

        IMaintenanceRequestRepository _mrRepository;
        IWorkOrderRepository _workOrderRepository;

        IFormIORepository _formIORepository;

        IAssetFormIORepository _assetFormIORepository;

        IMobileWorkorderRepository _mobileWorkorderRepository;
        IFormIOAssetClassRepository _formioassetclassRepository;
        IDashboardRepository _dashboardRepository;
        public JarvisUOW()
        {
            this._context = new DBContextFactory();
            //this.userRepository = userRepository;
        }


        public IBaseGenericRepository<T> BaseGenericRepository<T>() where T : class
        {
            if (repositories.Keys.Contains(typeof(T)) == true)
            {
                return repositories[typeof(T)] as IBaseGenericRepository<T>;
            }
            IBaseGenericRepository<T> repo = new BaseGenericRepository<T>(_context);
            repositories.Add(typeof(T), repo);
            return repo;
        }

        public ICompanyRepository<T> CompanyRepository<T>() where T : class
        {
            if (repositories.Keys.Contains(typeof(T)) == true)
            {
                return repositories[typeof(T)] as ICompanyRepository<T>;
            }
            ICompanyRepository<T> repo = new CompanyRepository<T>(_context);
            repositories.Add(typeof(T), repo);
            return repo;
        }

        public IFormAttributesRepository<T> FormAttributesRepository<T>() where T : class
        {
            if (repositories.Keys.Contains(typeof(T)) == true)
            {
                return repositories[typeof(T)] as IFormAttributesRepository<T>;
            }
            IFormAttributesRepository<T> repo = new FormAttributesRepository<T>(_context);
            repositories.Add(typeof(T), repo);
            return repo;
        }

        public IInspectionFormRepository InspectionFormRepository
        {
            get
            {
                if (_InspectionFormRepository == null)
                {
                    _InspectionFormRepository = new InspectionFormRepository(_context);
                }
                return _InspectionFormRepository;
            }
        }

        public IDeviceRepository DeviceRepository
        {
            get
            {
                if (_deviceRepository == null)
                {
                    _deviceRepository = new DeviceRepository(_context);
                }
                return _deviceRepository;
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_context);
                }
                return _userRepository;
            }
        }

        public IAssetRepository AssetRepository
        {
            get
            {
                if (_assetRepository == null)
                {
                    _assetRepository = new AssetRepository(_context);
                }
                return _assetRepository;
            }
        }

        public ISiteRepository SiteRepository
        {
            get
            {
                if (_siteRepository == null)
                {
                    _siteRepository = new SiteRepository(_context);
                }
                return _siteRepository;
            }
        }

        public IInspectionRepository InspectionRepository
        {
            get
            {
                if (_inspectionRepository == null)
                {
                    _inspectionRepository = new InspectionRepository(_context);
                }
                return _inspectionRepository;
            }
        }

        public IIssueRepository IssueRepository
        {
            get
            {
                if (_issueRepository == null)
                {
                    _issueRepository = new IssueRepository(_context);
                }
                return _issueRepository;
            }
        }

        public IPMCategoryRepository PMCategoryRepository
        {
            get
            {
                if (_pmCategoryRepository == null)
                {
                    _pmCategoryRepository = new PMCategoryRepository(_context);
                }
                return _pmCategoryRepository;
            }
        }

        public IPMPlansRepository PMPlansRepository
        {
            get
            {
                if (_pmPlansRepository == null)
                {
                    _pmPlansRepository = new PMPlansRepository(_context);
                }
                return _pmPlansRepository;
            }
        }

        public ITaskRepository TaskRepository
        {
            get
            {
                if (_taskRepository == null)
                {
                    _taskRepository = new TaskRepository(_context);
                }
                return _taskRepository;
            }
        }

        public IPMRepository PMRepository
        {
            get
            {
                if (_pmRepository == null)
                {
                    _pmRepository = new PMRepository(_context);
                }
                return _pmRepository;
            }
        }

        public IAssetPMPlansRepository AssetPMPlansRepository
        {
            get
            {
                if (_assetPMPlansRepository == null)
                {
                    _assetPMPlansRepository = new AssetPMPlansRepository(_context);
                }
                return _assetPMPlansRepository;
            }
        }

        public IAssetPMsRepository AssetPMsRepository
        {
            get
            {
                if (_assetPMsRepository == null)
                {
                    _assetPMsRepository = new AssetPMsRepository(_context);
                }
                return _assetPMsRepository;
            }
        }

        public IPMTriggersRepository PMTriggersRepository
        {
            get
            {
                if (_pmTriggersRepository == null)
                {
                    _pmTriggersRepository = new PMTriggersRepository(_context);
                }
                return _pmTriggersRepository;
            }
        }


        public IPMTriggersRemarksRepository PMTriggersRemarksRepository
        {
            get
            {
                if (_pmTriggersRemarksRepository == null)
                {
                    _pmTriggersRemarksRepository = new PMTriggersRemarksRepository(_context);
                }
                return _pmTriggersRemarksRepository;
            }
        }

        public ICompletedPMTriggerRepository CompletedPMTriggerRepository
        {
            get
            {
                if (_completedPMTriggerRepository == null)
                {
                    _completedPMTriggerRepository = new CompletedPMTriggerRepository(_context);
                }
                return _completedPMTriggerRepository;
            }
        }

        public IPMTriggersTasksRepository PMTriggersTasksRepository
        {
            get
            {
                if (_pmTriggersTasksRepository == null)
                {
                    _pmTriggersTasksRepository = new PMTriggersTasksRepository(_context);
                }
                return _pmTriggersTasksRepository;
            }
        }

        public IPMNotificationRepository PMNotificationRepository
        {
            get
            {
                if (_pmNotificationRepository == null)
                {
                    _pmNotificationRepository = new PMNotificationRepository(_context);
                }
                return _pmNotificationRepository;
            }
        }

        public IMaintenanceRequestRepository MRRepository
        {
            get
            {
                if (_mrRepository == null)
                {
                    _mrRepository = new MaintenanceRequestRepository(_context);
                }
                return _mrRepository;
            }
        }

        public IMobileWorkorderRepository MobileWorkOrderRepository
        {
            get
            {
                if (_mobileWorkorderRepository == null)
                {
                    _mobileWorkorderRepository = new MobileWorkorderRepository(_context);
                }
                return _mobileWorkorderRepository;
            }
        }

        public IWorkOrderRepository WorkOrderRepository
        {
            get
            {
                if (_workOrderRepository == null)
                {
                    _workOrderRepository = new WorkOrderRepository(_context);
                }
                return _workOrderRepository;
            }
        }

        public IFormIORepository formIORepository
        {
            get
            {
                if (_formIORepository == null)
                {
                    _formIORepository = new FormIORepository(_context);
                }
                return _formIORepository;
            }
        }

        public IAssetFormIORepository AssetFormIORepository
        {
            get
            {
                if (_assetFormIORepository == null)
                {
                    _assetFormIORepository = new AssetFormIORepository(_context);
                }
                return _assetFormIORepository;
            }
        }

        public IFormIOAssetClassRepository FormIOAssetClassRepository
        {
            get
            {
                if (_formioassetclassRepository == null)
                {
                    _formioassetclassRepository = new FormIOAssetClassRepository(_context);
                }
                return _formioassetclassRepository;
            }
        }

        public IDashboardRepository DashboardRepository
        {
            get
            {
                if (_dashboardRepository == null)
                {
                    _dashboardRepository = new DashboardRepository(_context);
                }
                return _dashboardRepository;
            }
        }

        //IMongoDBFormIORepository IJarvisUOW.MongoDBFormIORepository { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IDbContextTransaction BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
            return _transaction;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Int64 SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void StartTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
        }

        /// <summary>
        /// IDisposable implementation
        /// </summary>
        private bool disposed = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        
        //public IUserRepository UserRepository
        //{
        //    get
        //    {
        //        if (_userRepository == null)
        //        {
        //            _userRepository = new UserRepository(_context, userRepository);
        //        }
        //        return _userRepository;
        //    }
        //}


    }
}
