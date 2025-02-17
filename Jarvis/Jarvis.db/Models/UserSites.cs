using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public partial class UserSites
    {
        [Key]
        public Guid usersite_id { get; set; }

        [ForeignKey("User")]
        public Guid user_id { get; set; }

        //public string roleid { get; set; }
        public Guid company_id { get; set; }

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        [ForeignKey("StatusMaster")]
        public Nullable<int> status { get; set; }

        public virtual Sites Sites { get; set; }

        public virtual User User { get; set; }

        public virtual StatusMaster StatusMaster { get; set; }
    }
}
