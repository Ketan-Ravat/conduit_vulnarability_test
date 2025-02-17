using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class CompletedPMTriggers
    {
        [Key]
        public Guid completed_trigger_id { get; set; }

        [ForeignKey("AssetPMs")]
        public Guid asset_pm_id { get; set; }

        [ForeignKey("PMTriggers")]
        public Guid pm_trigger_id { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public Nullable<DateTime> modified_at { get; set; }
        
        public string modified_by { get; set; }

        public string created_by { get; set; }

        public virtual AssetPMs AssetPMs { get; set; }
        public virtual PMTriggers PMTriggers { get; set; }
    }
}
