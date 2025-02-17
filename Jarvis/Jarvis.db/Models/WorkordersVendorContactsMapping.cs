using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class WorkordersVendorContactsMapping
    {
        [Key]
        public Guid workorders_vendor_contacts_mapping_id { get; set; }

        [ForeignKey("Vendors")]
        public Guid? vendor_id { get; set; }

        [ForeignKey("Contacts")]
        public Guid contact_id { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }

        public int? contact_invite_status { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public virtual WorkOrders WorkOrders { get; set; }
        public virtual Contacts Contacts { get; set; }
        public virtual Vendors Vendors { get; set; }
    }
}
