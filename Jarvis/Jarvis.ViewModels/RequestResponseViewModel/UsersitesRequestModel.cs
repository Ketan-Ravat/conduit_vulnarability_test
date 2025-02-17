using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class UsersitesRequestModel
    {
        public Guid usersite_id { get; set; }

        public Guid user_id { get; set; }

        //public string roleid { get; set; }

        public Guid company_id { get; set; }

        public Guid site_id { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public Nullable<int> status { get; set; }

        //public virtual SitesViewModel Sites { get; set; }

        //public virtual UserViewModel User { get; set; }
    }
}
