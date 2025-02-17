using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class Vendors
    {
        [Key]
        public Guid vendor_id { get; set; }
        public string vendor_name { get; set; }
        public string vendor_email { get; set; }
        public string vendor_phone_number { get; set; }
        public string vendor_category { get; set; }
        public int? vendor_category_id { get; set; }
        public string vendor_address { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }

        [ForeignKey("Company")]
        public Guid company_id { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<WorkordersVendorContactsMapping> WorkordersVendorContactsMapping { get; set; }
        public virtual ICollection<Contacts> Contacts { get; set; }
    }
}
