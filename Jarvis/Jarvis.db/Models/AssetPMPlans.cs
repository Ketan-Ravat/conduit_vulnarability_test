using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetPMPlans
    {
        [Key]
        public Guid asset_pm_plan_id { get; set; }

        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }
        
        public Guid pm_plan_id { get; set; }

        public string plan_name { get; set; }
        public bool is_pm_plan_inspection_manual { get; set; } // this key is for pm plan added in WO which are not active

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        [ForeignKey("StatusMaster")]
        public Nullable<int> status { get; set; }
        public virtual StatusMaster StatusMaster { get; set; }
        public virtual Asset Asset { get; set; }
        public virtual ICollection<AssetPMs> AssetPMs { get; set; }
    }
}
