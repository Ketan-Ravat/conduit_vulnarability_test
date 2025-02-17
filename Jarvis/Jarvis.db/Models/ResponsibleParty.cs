using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class ResponsibleParty
    {
        [Key]
        public Guid responsible_party_id { get; set; }
        public string responsible_party_name { get; set; }
        public bool is_deleted { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
    }
}
