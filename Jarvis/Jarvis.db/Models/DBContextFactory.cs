using Jarvis.db.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Jarvis.db.Models
{
    public partial class DBContextFactory : DbContext
    {
        internal DBContextFactory context;

        public DBContextFactory() 
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //IConfigurationRoot builder = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            base.OnConfiguring(optionsBuilder);
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfigurationRoot config = builder.Build();
            optionsBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            
        }

        


        public DBContextFactory(DbContextOptions<DBContextFactory> options)
            : base(options)
        {
        }

        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserSites> UserSites { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<Sites> Sites { get; set; }
        public virtual DbSet<Company> Company { get; set; }

        public virtual DbSet<Asset> Assets { get; set; }

        public virtual DbSet<StatusMaster> StatusMasters { get; set; }

        public virtual DbSet<StatusTypes> StatusTypes { get; set; }

        public virtual DbSet<InspectionForms> InspectionForms { get; set; }

        public virtual DbSet<InspectionFormTypes> InspectionFormTypes { get; set; }

        public virtual DbSet<InspectionFormAttributes> InspectionFormAttributes { get; set; }

        public virtual DbSet<InspectionAttributeCategory> InspectionAttributeCategory { get; set; }

        public virtual DbSet<AssetTransactionHistory> AssetTransactionHistory { get; set; }

        public virtual DbSet<Inspection> Inspection { get; set; }

        public virtual DbSet<Issue> Issue { get; set; }
        //public virtual DbSet<WorkOrder> WorkOrder { get; set; }
        public virtual DbSet<IssueStatus> IssueStatus { get; set; }
        public virtual DbSet<IssueRecord> IssueRecord { get; set; }
        //public virtual DbSet<WorkOrderStatus> WorkOrderStatus { get; set; }
        //public virtual DbSet<WorkOrderRecord> WorkOrderRecord { get; set; }

        public virtual DbSet<NotificationData> NotificationData { get; set; }

        public virtual DbSet<MasterData> MasterData { get; set; }

        public virtual DbSet<DashboardOutstandingIssues> DashboardOutstandingIssues { get; set; }

        public virtual DbSet<DeviceInfo> DeviceInfo { get; set; }
        public virtual DbSet<PMCategory> PMCategory { get; set; }
        public virtual DbSet<PMPlans> PMPlans { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<PMs> PMs { get; set; }
        public virtual DbSet<AssetPMPlans> AssetPMPlans { get; set; }
        public virtual DbSet<AssetPMs> AssetPMs { get; set; }
        public virtual DbSet<AssetPMTasks> AssetPMTasks { get; set; }
        public virtual DbSet<PMTriggers> PMTriggers { get; set; }
        public virtual DbSet<PMTriggersTasks> PMTriggersTasks { get; set; }
        public virtual DbSet<PMTriggersRemarks> PMTriggersRemarks { get; set; }
        public virtual DbSet<CompletedPMTriggers> CompletedPMTriggers { get; set; }
        public virtual DbSet<CompanyPMNotificationConfigurations> CompanyPMNotificationConfigurations { get; set; }
        public virtual DbSet<AssetPMNotificationConfigurations> AssetPMNotificationConfigurations { get; set; }
        public virtual DbSet<SentPMNotification> SentPMNotification { get; set; }

        public virtual DbSet<RecordSyncInformation> RecordSyncInformation { get; set; }

        public virtual DbSet<PreferLanguageMaster> PreferLanguageMaster { get; set; }

        public virtual DbSet<AssetInspectionReport> AssetInspectionReport { get; set; }

        public virtual DbSet<ResetPasswordToken> ResetPasswordToken { get; set; }
        public virtual DbSet<EmailNotificationStatusUpdate> EmailNotificationStatusUpdate { get; set; }
        public virtual DbSet<ManagerPMNotificationConfiguration> ManagerPMNotificationConfiguration { get; set; }
        public virtual DbSet<UserEmailNotificationConfigurationSettings> UserEmailNotificationConfigurationSettings { get; set; }
        public virtual DbSet<AssetActivityLogs> AssetActivityLogs { get; set; }
        public virtual DbSet<ServiceDealers> ServiceDealers { get; set; }
        public virtual DbSet<WOInspectionsTemplateFormIOAssignment> WOInspectionsTemplateFormIOAssignment { get; set; }
        public virtual DbSet<WOcategorytoTaskMapping> WOcategorytoTaskMapping { get; set; }
        public virtual DbSet<FormIOLocationNotes> FormIOLocationNotes { get; set; }
        public virtual DbSet<InspectionTemplateAssetClass> InspectionTemplateAssetClass { get; set; }
        public virtual DbSet<MobileAppVersion> MobileAppVersion { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.ApplyConfiguration(new RecordSyncInformationConfiguration());
        //    //modelBuilder.ApplyConfiguration(new RoleConfiguration());

        //    OnModelCreatingPartial(modelBuilder);
        //}

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


        //public DBModels CreateDBContext(String[] args)
        //{
        //    IConfigurationRoot configuration = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json")
        //        .Build();

        //    var builder = new DbContextOptionsBuilder<DBModels>();

        //    var connectionstring = configuration.GetConnectionString("DefaultConnection");

        //    builder.UseNpgsql(connectionstring);

        //    return new DBModels(builder.Options);
        //} 

        public virtual DbSet<AssetType> AssetTypes { get; set; }

        public virtual DbSet<AssetPMAttachments> AssetPMAttachments { get; set; }
        public virtual DbSet<MaintenanceRequests> MaintenanceRequests { get; set; }
        public virtual DbSet<CancelReasonMaster> CancelReasonMaster { get; set; }
        public virtual DbSet<MaintenanceReqCancelRequests> MaintenanceReqCancelRequests { get; set; }

        public virtual DbSet<WorkOrders> WorkOrders { get; set; }
        public virtual DbSet<WorkOrderTasks> WorkOrderTasks { get; set; }
        public virtual DbSet<WorkOrderAttachments> WorkOrderAttachments { get; set; }
        public virtual DbSet<WorkOrderCancel> WorkOrderCancel { get; set; }
        public virtual DbSet<WorkOrderIssues> WorkOrderIssues { get; set; }
        public virtual DbSet<AssetMeterHourHistory> AssetMeterHourHistory { get; set; }
        public virtual DbSet<InspectionsTemplateFormIO> InspectionsTemplateFormIO { get; set; }
        public virtual DbSet<AssetFormIO> AssetFormIO { get; set; }
        public virtual DbSet<ClientCompany> ClientCompany { get; set; }
        public virtual DbSet<FormIOType> FormIOType { get; set; }
        public virtual DbSet<FormIOBuildings> FormIOBuildings { get; set; }
        public virtual DbSet<FormIOFloors> FormIOFloors { get; set; }
        public virtual DbSet<FormIORooms> FormIORooms { get; set; }
        public virtual DbSet<FormIOSections> FormIOSections { get; set; }
        public virtual DbSet<AssetFormIOBuildingMappings> AssetFormIOBuildingMappings { get; set; }
        public virtual DbSet<FormIOInsulationResistanceTestMapping> FormIOInsulationResistanceTestMapping { get; set; }
        public virtual DbSet<AssetProfileImages> AssetProfileImages { get; set; }
        public virtual DbSet<AssetClassFormIOMapping> AssetClassFormIOMapping { get; set; }
        public virtual DbSet<WOOnboardingAssets> WOOnboardingAssets { get; set; }
        public virtual DbSet<WOOnboardingAssetsImagesMapping> WOOnboardingAssetsImagesMapping { get; set; }
        public virtual DbSet<AssetNotes> AssetNotes { get; set; }
        public virtual DbSet<AssetParentHierarchyMapping> AssetParentHierarchyMapping { get; set; }
        public virtual DbSet<AssetChildrenHierarchyMapping> AssetChildrenHierarchyMapping { get; set; }
        public virtual DbSet<WOOBAssetFedByMapping> WOOBAssetFedByMapping { get; set; }
        public virtual DbSet<IRWOImagesLabelMapping> IRWOImagesLabelMapping { get; set; }
        public virtual DbSet<IRScanWOImageFileMapping> IRScanWOImageFileMapping { get; set; }
        public virtual DbSet<FormIOAuthToken> FormIOAuthToken { get; set; }
        public virtual DbSet<AssetReplacementMapping> AssetReplacementMapping { get; set; }
        public virtual DbSet<WOLineIssue> WOLineIssue { get; set; }
        public virtual DbSet<AssetIssue> AssetIssue { get; set; }
        public virtual DbSet<AssetIssueComments> AssetIssueComments { get; set; }
        public virtual DbSet<AssetIssueImagesMapping> AssetIssueImagesMapping { get; set; }
        public virtual DbSet<PMsTriggerConditionMapping> PMsTriggerConditionMapping { get; set; }
        public virtual DbSet<AssetPMsTriggerConditionMapping> AssetPMsTriggerConditionMapping { get; set; }
        public virtual DbSet<WOLineBuildingMapping> WOLineBuildingMapping { get; set; }
        public virtual DbSet<AssetIRWOImagesLabelMapping> AssetIRWOImagesLabelMapping { get; set; }
        public virtual DbSet<AssetAttachmentMapping> AssetAttachmentMapping { get; set; }
        public virtual DbSet<AssetSubLevelcomponentMapping> AssetSubLevelcomponentMapping { get; set; }
        public virtual DbSet<AssetTopLevelcomponentMapping> AssetTopLevelcomponentMapping { get; set; }
        public virtual DbSet<WOlineTopLevelcomponentMapping> WOlineTopLevelcomponentMapping { get; set; }
        public virtual DbSet<WOlineSubLevelcomponentMapping> WOlineSubLevelcomponentMapping { get; set; }

        public virtual DbSet<ClusterDiagramPDFSiteMapping> ClusterDiagramPDFSiteMapping { get; set; }

        public virtual DbSet<Equipment> Equipment { get; set; }
        public virtual DbSet<PMItemMasterForms> PMItemMasterForms { get; set; }

        public virtual DbSet<TempFormIOBuildings> TempFormIOBuildings { get; set; }
        public virtual DbSet<TempFormIOFloors> TempFormIOFloors { get; set; }
        public virtual DbSet<TempFormIORooms> TempFormIORooms { get; set; }
        public virtual DbSet<TempFormIOSections> TempFormIOSections { get; set; }

        public virtual DbSet<WOOBAssetTempFormIOBuildingMapping> WOOBAssetTempFormIOBuildingMappings { get; set; }
        public virtual DbSet<ActiveAssetPMWOlineMapping> ActiveAssetPMWOlineMapping { get; set; }
        public virtual DbSet<TempAssetPMs> TempAssetPMs { get; set; }
        public virtual DbSet<TempActiveAssetPMWOlineMapping> TempActiveAssetPMWOlineMapping { get; set; }
        public virtual DbSet<WorkOrderTechnicianMapping> WorkOrderTechnicianMapping { get; set; }
        public virtual DbSet<WOlineIssueImagesMapping> WOlineIssueImagesMapping { get; set; }
        public virtual DbSet<TempAsset> TempAsset { get; set; }
        public virtual DbSet<WorkOrderWatcherUserMapping> WorkOrderWatcherUserMapping { get; set; }
        public virtual DbSet<TimeMaterials> TimeMaterials { get; set; }
        public virtual DbSet<TrackMobileSyncOffline> TrackMobileSyncOffline { get; set; }
        public virtual DbSet<ResponsibleParty> ResponsibleParty { get; set; }
        public virtual DbSet<NetaInspectionBulkReportTracking> NetaInspectionBulkReportTracking { get; set; }
        public virtual DbSet<WorkOrderBackOfficeUserMapping> WorkOrderBackOfficeUserMapping { get; set; }
        public virtual DbSet<SiteProjectManagerMapping> SiteProjectManagerMapping { get; set; }
        public virtual DbSet<UserSession> UserSession { get; set; }
        public virtual DbSet<WOOnboardingAssetsDateTimeTracking> WOOnboardingAssetsDateTimeTracking { get; set; }
        public virtual DbSet<UserLocation> UserLocation { get; set; }
        public virtual DbSet<Features> Features { get; set; }
        public virtual DbSet<CompanyFeatureMapping> CompanyFeatureMappings { get; set; }
        public virtual DbSet<SiteDocuments> SiteDocuments { get; set; }
        public virtual DbSet<TempMasterBuilding> TempMasterBuilding { get; set; }
        public virtual DbSet<TempMasterFloor> TempMasterFloor { get; set; }
        public virtual DbSet<TempMasterRoom> TempMasterRoom { get; set; }
        public virtual DbSet<TempMasterBuildingWOMapping> TempMasterBuildingWOMapping { get; set; }
        public virtual DbSet<TempMasterFloorWOMapping> TempMasterFloorWOMapping { get; set; }
        public virtual DbSet<TempMasterRoomWOMapping> TempMasterRoomWOMapping { get; set; }
        public virtual DbSet<Vendors> Vendors { get; set; }
        public virtual DbSet<Contacts> Contacts { get; set; }
        public virtual DbSet<WorkordersVendorContactsMapping> WorkordersVendorContactsMapping { get; set; }
        public virtual DbSet<AssetGroup> AssetGroup { get; set; }

        public virtual DbSet<SiteContact> SiteContact { get; set; }

        public virtual DbSet<SitewalkthroughTempPmEstimation> SitewalkthroughTempPmEstimation { get; set; }

    }
}
