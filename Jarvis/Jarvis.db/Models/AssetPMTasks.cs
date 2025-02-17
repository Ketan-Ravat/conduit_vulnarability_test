using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetPMTasks
    {
        [Key]
        public Guid asset_pm_task_id { get; set; }

        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }

        public Nullable<Guid> pm_task_id { get; set; }

        [ForeignKey("Tasks")]
        public Guid task_id { get; set; }

        [ForeignKey("AssetPMs")]
        public Guid asset_pm_id { get; set; }

        [ForeignKey("AssetPMPlans")]
        public Guid asset_pm_plan_id { get; set; }

        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public virtual Asset Asset { get; set; }

        public virtual Tasks Tasks { get; set; }

        public virtual AssetPMs AssetPMs { get; set; }
        
        public virtual AssetPMPlans AssetPMPlans { get; set; }
    }
}
