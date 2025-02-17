using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class SiteContact
    {
        [Key]
        public Guid sitecontact_id { get; set; }

        [ForeignKey("Company")]

        public Guid company_id { get; set; }

        [ForeignKey("ClientCompany")]
        public Guid? client_company_id { get; set; }
        public string sitecontact_title { get; set; }

        public string sitecontact_name { get; set; }

        public string sitecontact_email { get; set; }

        public string sitecontact_phone { get; set; }

        public bool is_deleted { get; set; }

        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }

        public virtual Company Company { get; set; }

        public virtual ClientCompany ClientCompany { get; set; }

        public virtual ICollection<Sites> Sites { get; set; }
    }
}
