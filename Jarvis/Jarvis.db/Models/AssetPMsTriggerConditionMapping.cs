using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetPMsTriggerConditionMapping
    {
        [Key]
        public Guid asset_pm_trigger_condition_mapping_id { get; set; }

        public Nullable<int> datetime_repeates_every { get; set; } // total years or month count to repeat

        [ForeignKey("PMDateTimeRepeatTypeStatus")]
        public Nullable<int> datetime_repeat_time_period_type { get; set; } //29 - Month, 30 - Year, 39 - Week, 40 - Day

        [ForeignKey("AssetPMs")]
        public Guid asset_pm_id { get; set; } //29 - Month, 30 - Year, 39 - Week, 40 - Day
        public int condition_type_id { get; set; } // 1 - condition-1 , 2 - condition-2 ,3 - condition-3

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }
        public bool is_archive { get; set; }
        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }
        public virtual StatusMaster PMDateTimeRepeatTypeStatus { get; set; }
        public virtual AssetPMs AssetPMs { get; set; }
        public virtual Sites Sites { get; set; }
    }
}
