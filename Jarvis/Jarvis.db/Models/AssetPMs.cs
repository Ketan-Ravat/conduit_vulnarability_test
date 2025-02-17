using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetPMs
    {
        [Key]
        public Guid asset_pm_id { get; set; }

        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }

        public Nullable<Guid> pm_id { get; set; }

        public string title { get; set; }

        public string description { get; set; }
        public int? work_procedure_type { get; set; }
        public bool is_pm_inspection_manual { get; set; } // this key is for pm added in WO which are not active

        [ForeignKey("StatusMaster")]
        public int status { get; set; }
        
        [ForeignKey("AssetFormIO")]
        public Guid? asset_form_id { get; set; }

        [ForeignKey("WOOnboardingAssets")]
        public Guid? woonboardingassets_id { get; set; } // current active woline

        [ForeignKey("WorkOrders")]
        public Guid? wo_id { get; set; } // current active wo

        [ForeignKey("AssetPMPlans")]
        public Guid asset_pm_plan_id { get; set; }

        [ForeignKey("PMTypeStatus")]
        public int pm_trigger_type { get; set; }

        [ForeignKey("PMByStatus")]
        public int pm_trigger_by { get; set; }

        [ForeignKey("ServiceDealers")]
        public Guid? service_dealer_id { get; set; }

        [ForeignKey("InspectionsTemplateFormIO")]

        public Guid? form_id { get; set; }
        public bool is_Asset_PM_fixed { get; set; }//bool use in mobile app as PM is done or not
        public string pm_form_output_data { get; set; } // this key is now deprected , we are using ActiveAssetPMWOlineMapping for pm submitted form json
        public Nullable<DateTime> due_date { get; set; }
        public Nullable<int> pm_due_overdue_flag { get; set; }
        public string pm_due_time_duration { get; set; }
        public bool is_assetpm_enabled { get; set; } = true;  // true = Enabled , false = Disabled
        public Nullable<DateTime> datetime_due_at { get; set; }
        public Nullable<int> estimation_time { get; set; } // number of minutes 
        public Nullable<int> meter_hours_due_at { get; set; }

        public Nullable<int> datetime_repeates_every { get; set; }

        public Nullable<DateTime> datetime_starting_at { get; set; }

        [ForeignKey("PMDateTimeRepeatTypeStatus")]
        public Nullable<int> datetime_repeat_time_period_type { get; set; } 

        public Nullable<int> meter_hours_starting_at { get; set; }

        public Nullable<int> meter_hours_repeates_every { get; set; }
        
        public Nullable<bool> is_trigger_on_starting_at { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public bool is_archive { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }
        public string completed_notes { get; set; }
        public DateTime? asset_pm_completed_date { get; set; }
        public virtual Asset Asset { get; set; }

        public virtual StatusMaster StatusMaster { get; set; }
        
        public virtual AssetPMPlans AssetPMPlans { get; set; }

        public virtual StatusMaster PMTypeStatus { get; set; }
        
        public virtual StatusMaster PMByStatus { get; set; }
        
        public virtual StatusMaster PMDateTimeRepeatTypeStatus { get; set; }

        public virtual ServiceDealers ServiceDealers { get; set; }
        public virtual AssetFormIO AssetFormIO { get; set; }
        public virtual WOOnboardingAssets WOOnboardingAssets { get; set; }
        public virtual WorkOrders WorkOrders { get; set; }

        public virtual InspectionsTemplateFormIO InspectionsTemplateFormIO { get; set; }
        public virtual ICollection<AssetPMTasks> AssetPMTasks { get; set; }

        public virtual ICollection<PMTriggers> PMTriggers { get; set; }

        public virtual ICollection<AssetPMAttachments> AssetPMAttachments { get; set; }
        public virtual ICollection<AssetPMsTriggerConditionMapping> AssetPMsTriggerConditionMapping { get; set; }

        public List<ActiveAssetPMWOlineMapping> ActiveAssetPMWOlineMapping { get; set; }


    }
}
