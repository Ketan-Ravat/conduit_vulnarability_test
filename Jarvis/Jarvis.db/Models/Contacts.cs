using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class Contacts
    {
        [Key]
        public Guid contact_id { get; set; }

        public string name { get; set; }
        public string category { get; set; }
        public int? category_id { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string notes { get; set; }
        public bool mark_as_primary { get; set; }//true = primary , false = not primary

        [ForeignKey("Vendors")]
        public Guid vendor_id { get; set; }

        [ForeignKey("Company")]
        public Guid? company_id { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public virtual Vendors Vendors { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<WorkordersVendorContactsMapping> WorkordersVendorContactsMapping { get; set; }
    }
}
