using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class PMs
    {
        [Key]
        public Guid pm_id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        [ForeignKey("StatusMaster")]
        public int status { get; set; }

        [ForeignKey("PMPlans")]
        public Guid pm_plan_id { get; set; }

        [ForeignKey("PMTypeStatus")]
        public int pm_trigger_type { get; set; } // Fixed One Time -25 , Recurring - 26 

        [ForeignKey("PMByStatus")]
        public int pm_trigger_by { get; set; } //Time - 26 , Meter Hours - 27 , Time / Meter Hours  - 28

        [ForeignKey("ServiceDealers")]
        public Guid? service_dealer_id { get; set; }

        [ForeignKey("InspectionsTemplateFormIO")]

        public Guid? form_id { get; set; }

        public InspectionsTemplateFormIO InspectionsTemplateFormIO { get; set; }

        public int? work_procedure_type { get; set; }

        public Nullable<DateTime> datetime_due_at { get; set; }

        public Nullable<int> meter_hours_due_at { get; set; }
        public Nullable<int> pm_inspection_type_id { get; set; } // 1 = Infrared ThermoGraphy

        public Nullable<int> datetime_repeates_every { get; set; } // total years ormonth count to repeat
        public Nullable<int> estimation_time { get; set; } // number of minutes 

        public Nullable<DateTime> datetime_starting_at { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        [ForeignKey("PMDateTimeRepeatTypeStatus")]
        public Nullable<int> datetime_repeat_time_period_type { get; set; } //29 - Month, 30 - Year, 39 - Week, 40 - Day

        public Nullable<int> meter_hours_starting_at { get; set; }

        public Nullable<int> meter_hours_repeates_every { get; set; }
        
        public Nullable<bool> is_trigger_on_starting_at { get; set; }

        public bool is_archive { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public virtual StatusMaster StatusMaster { get; set; }
        
        public virtual PMPlans PMPlans { get; set; }

        public virtual StatusMaster PMTypeStatus { get; set; }
        
        public virtual StatusMaster PMByStatus { get; set; }
        
        public virtual StatusMaster PMDateTimeRepeatTypeStatus { get; set; }
        public virtual ServiceDealers ServiceDealers { get; set; }

        public virtual ICollection<PMTasks> PMTasks { get; set; }

        public virtual ICollection<PMAttachments> PMAttachments { get; set; }
        public virtual ICollection<PMsTriggerConditionMapping> PMsTriggerConditionMapping { get; set; }
        public virtual ICollection<TempAssetPMs> TempAssetPMs { get; set; }
    }
}
