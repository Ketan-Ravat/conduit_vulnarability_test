using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class PMTriggers
    {
        [Key]
        public Guid pm_trigger_id { get; set; }

        [ForeignKey("AssetPMs")]
        public Guid asset_pm_id { get; set; }

        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }

        [ForeignKey("StatusMaster")]
        public int status { get; set; }

        [ForeignKey("AssetPMStatusMaster")]
        public Nullable<int> asset_pm_status { get; set; }

        public Nullable<DateTime> due_datetime { get; set; }
        
        public Nullable<int> due_meter_hours { get; set; }

        public Nullable<int> estimated_due_date_meter_hour { get; set; }
        
        public Nullable<int> total_est_time_minutes { get; set; }

        public Nullable<int> total_est_time_hours { get; set; }
        
        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public Nullable<DateTime> modified_at { get; set; }
        public Nullable<DateTime> datetime_when_pm_due { get; set; }
        public Nullable<long> meter_hours_when_pm_due { get; set; }

        public virtual Asset Asset { get; set; }

        public virtual AssetPMs AssetPMs { get; set; }
        
        public virtual StatusMaster StatusMaster { get; set; }

        public virtual ICollection<PMTriggersTasks> PMTriggersTasks { get; set; }
        public virtual PMTriggersRemarks PMTriggersRemarks { get; set; }
        public virtual StatusMaster AssetPMStatusMaster { get; set; }

    }
}
