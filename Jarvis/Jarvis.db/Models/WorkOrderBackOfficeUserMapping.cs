using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class WorkOrderBackOfficeUserMapping
    {
        [Key]
        public Guid wo_backoffice_user_mapping_id { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }

        [ForeignKey("BackOfficeUser")]
        public Guid user_id { get; set; }

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }

        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }

        public virtual User BackOfficeUser { get; set; }
        public virtual WorkOrders WorkOrders { get; set; }
        public virtual Sites Sites { get; set; }
    }
}
