using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class Equipment
    {
        [Key]
        public Guid equipment_id { get; set; }

        public string equipment_number { get; set; }

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }

        [ForeignKey("Company")]
        public Guid? company_id { get; set; }

        public string equipment_name { get; set; }

        public string manufacturer { get; set; }

        public string model_number { get; set; }
        public string serial_number { get; set;}

        public int calibration_interval { get; set; }

        public DateTime calibration_date { get; set; }

        public int? calibration_status { get; set; }

        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public bool isarchive { get; set; }

        public virtual Sites Sites { get; set; }
        public virtual Company Company { get; set; }

    }
}
