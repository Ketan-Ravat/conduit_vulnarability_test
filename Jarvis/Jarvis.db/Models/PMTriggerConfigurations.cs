using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class PMTriggerConfigurations
    {
        [Key]
        public Guid config_id { get; set; }

        [ForeignKey("Company")]
        public Guid company_id { get; set; }

        public int days_before_due_date_to_due_trigger { get; set; }
        
        public int meterhours_before_due_meter_to_due_trigger { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public Nullable<DateTime> modified_at { get; set; }
        
        public string created_by { get; set; }
        
        public string modified_by { get; set; }

        public virtual Company Company { get; set; }

    }
}
