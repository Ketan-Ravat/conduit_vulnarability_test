using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class PMTriggersTasks
    {
        [Key]
        public Guid trigger_task_id { get; set; }

        [ForeignKey("PMTriggers")]
        public Guid pm_trigger_id { get; set; }

        [ForeignKey("AssetPMs")]
        public Guid asset_pm_id { get; set; }

        [ForeignKey("AssetPMTasks")]
        public Guid asset_pm_task_id { get; set; }

        [ForeignKey("Tasks")]
        public Guid task_id { get; set; }

        [ForeignKey("StatusMaster")]
        public int status { get; set; }

        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }

        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string created_by { get; set; }

        public string modified_by { get; set; }

        public virtual AssetPMs AssetPMs { get; set; }

        public virtual PMTriggers PMTriggers { get; set; }

        public virtual AssetPMTasks AssetPMTasks { get; set; }

        public virtual Tasks Tasks { get; set; }

        public virtual StatusMaster StatusMaster { get; set; }

        public virtual Asset Asset { get; set; }
    }
}