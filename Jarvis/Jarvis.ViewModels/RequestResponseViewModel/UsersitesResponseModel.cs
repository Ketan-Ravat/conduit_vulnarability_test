using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class UsersitesResponseModel : BaseViewModel
    {
        public Guid usersite_id { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public Nullable<int> status { get; set; }

        public virtual SitesViewModel Sites { get; set; }

        //public virtual LoginResponceModel User { get; set; }

        public virtual UserViewModel User { get; set; }
    }
}
