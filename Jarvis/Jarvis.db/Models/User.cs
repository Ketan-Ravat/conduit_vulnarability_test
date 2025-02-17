using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public partial class User
    {
        public User()
        {
            Usersites = new List<UserSites>();
        }

        [Key]
        public Guid uuid { get; set; }

        public Guid barcode_id { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public string username { get; set; }

        public string firstname { get; set; }

        public string lastname { get; set; }
        public string mobile_number { get; set; } 
        public string profile_picture_name { get; set; }
        public string job_title { get; set; }
        public string signature_url { get; set; }

        [ForeignKey("Role")]
        public Guid? role_id { get; set; }

        [ForeignKey("Role_App")]
        public Guid? ac_default_role_app { get; set; }

        [ForeignKey("Role_Web")]
        public Guid? ac_default_role_web { get; set; }

        [ForeignKey("Active_Role_App")]
        public Guid? ac_active_role_app { get; set; }

        [ForeignKey("Active_Role_Web")]
        public Guid? ac_active_role_web { get; set; }

        [ForeignKey("Site")]
        public Guid? ac_default_site { get; set; }

        [ForeignKey("Active_Site")]
        public Guid? ac_active_site { get; set; }
        [ForeignKey("Company")]
        public Guid? ac_default_company { get; set; }

        [ForeignKey("Active_Company")]
        public Guid? ac_active_company { get; set; }

        //public string ti_default_role { get; set; }

        //public string ti_default_site { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        [ForeignKey("StatusMaster")]
        public Nullable<int> status { get; set; }

        [ForeignKey("Report_StatusMaster")]
        public Nullable<int> executive_report_status { get; set; }

        public string notification_token { get; set; }

        public string os { get; set; }

        public bool email_notification_status { get; set; }
        public bool operator_usage_report_email_not_status { get; set; }
        public bool is_email_verified { get; set; }
        public bool manager_pm_notification_status { get; set; }

        [ForeignKey("LanguageMaster")]
        public int? prefer_language_id { get; set; }

        //[ForeignKey("AppMaster")]
        //public int? default_app { get; set; }

        //public AppMaster AppMaster { get; set; }

        [ForeignKey("default_client_Company")]
        public Guid? ac_default_client_company { get; set; }

        [ForeignKey("Active_client_Company")]
        public Guid? ac_active_client_company { get; set; }
        public string phone_number { get; set; }// deprected
        public virtual ClientCompany default_client_Company { get; set; }
        public virtual ClientCompany Active_client_Company { get; set; }

        public virtual List<UserRoles> Userroles { get; set; }

        public virtual Role Role_App { get; set; }

        public virtual Role Role { get; set; }

        public virtual Role Role_Web { get; set; }
        public virtual Role Active_Role_App { get; set; }
        public virtual Role Active_Role_Web { get; set; }

        public virtual Sites Site { get; set; }
        public virtual Sites Active_Site { get; set; }
        public virtual Company Company { get; set; }
        public virtual Company Active_Company { get; set; }

        public virtual List<UserSites> Usersites { get; set; }

        public virtual ICollection<Inspection> Inspection { get; set; }

        public virtual LanguageMaster LanguageMaster { get; set; }

        public virtual StatusMaster StatusMaster { get; set; }
        public virtual StatusMaster Report_StatusMaster { get; set; }
        public virtual UserEmailNotificationConfigurationSettings UserEmailNotificationConfigurationSettings { get; set; }

        public ICollection<AssetIssueComments> AssetIssueComments { get; set; }

        public virtual ICollection<WorkOrderTechnicianMapping> WorkOrderTechnicianMapping { get; set; }
        public virtual ICollection<WorkOrderBackOfficeUserMapping> WorkOrderBackOfficeUserMapping { get; set; }
        public virtual ICollection<SiteProjectManagerMapping> SiteProjectManagerMapping { get; set; }
        public virtual ICollection<UserSession> UserSession { get; set; }
        public virtual ICollection<UserLocation> UserLocation { get; set; }


        //  public virtual WOcategorytoTaskMapping WOcategorytoTaskMapping { get; set; }

        //public virtual ICollection<UserAccessApp> UserAccessApps { get; set; }
    }
}
