using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class ClientCompany
    {
        [Key]
        public Guid client_company_id { get; set; }
        public string client_company_name { get; set;}
        [ForeignKey("StatusMaster")]
        public Nullable<int> status { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public string created_by { get; set; }
        public string clientcompany_code { get; set; }
        public string owner { get; set; }
        public string owner_address { get; set; }

        [ForeignKey("parent_company")]
        public Guid? parent_company_id { get; set; }
        public string logo_url { get; set;}
        public virtual StatusMaster StatusMaster { get; set; }
        public virtual Company parent_company { get; set; }
        public virtual ICollection<Sites> Sites { get; set; }
        
        public virtual ICollection<SiteContact> SiteContacts { get; set; }
    }
}
