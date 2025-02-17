using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class PMTriggersRemarks
    {
        [Key]
        public Guid pm_triggers_remark_id { get; set; }

        [ForeignKey("PMTriggers")]
        public Guid pm_trigger_id { get; set; }

        public string comments { get; set; }

        public DateTime completed_on { get; set; }

        public int completed_at_meter_hours { get; set; }

        public int completed_in_hours { get; set; }
        
        public int completed_in_minutes { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public Nullable<DateTime> modified_at { get; set; }
        
        public string modified_by { get; set; }

        public string created_by { get; set; }

        public virtual PMTriggers PMTriggers { get; set; }
    }
}
