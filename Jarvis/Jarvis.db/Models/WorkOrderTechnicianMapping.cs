using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class WorkOrderTechnicianMapping
    {
        [Key]
        public Guid wo_technician_mapping_id { get; set; }


        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }


        [ForeignKey("TechnicianUser")]
        public Guid user_id { get; set; }


        [ForeignKey("Sites")]
        public Guid site_id { get; set; }

        public bool is_deleted { get; set; }

        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }

        public virtual User TechnicianUser { get; set; }
        public virtual WorkOrders WorkOrders { get; set; }
        public virtual Sites Sites { get; set; }
    }
}
