using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterUsersOptimizedResponsemodel
    {
        public Guid uuid { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public Nullable<int> status { get; set; }
        public bool is_registration_succeed { get; set; }
        public bool is_email_verified { get; set; }
        public string default_rolename_app_name { get; set; }
        public virtual ICollection<UserSitesViewModel> usersites { get; set; }
        public virtual ICollection<UserRole> userroles { get; set; }
    }
}
