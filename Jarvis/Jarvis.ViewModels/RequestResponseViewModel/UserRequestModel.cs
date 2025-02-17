using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class UserRequestModel
    {
        public string requestby { get; set; }

        public Guid uuid { get; set; }

        //public Guid roleid { get; set; }

        public string email { get; set; }
        public string mobile_number { get; set; }

        public string password { get; set; }

        public string username { get; set; }

        public string firstname { get; set; }

        public string lastname { get; set; }

        public bool is_required { get; set; }
        public string? profile_picture_name { get; set; }
        public string job_title { get; set; }
        public string signature_url { get; set; }
        public Guid? ac_default_role_app { get; set; }

        public Guid? ac_default_role_web { get; set; }

        public Guid? ac_default_site { get; set; }
        public Guid? ac_default_company { get; set; }
        public Guid? ac_active_company { get; set; }

        public Guid? ac_active_role_app { get; set; }

        public Guid? ac_active_role_web { get; set; }

        public Guid? ac_active_site { get; set; }

        //public string ti_default_role { get; set; }

        //public string ti_default_site { get; set; }

        public int? prefer_language_id { get; set; }

        //public int default_app { get; set; }

        public Nullable<int> status { get; set; }
        
        public virtual ICollection<RoleViewModel> Userroles { get; set; }

        public virtual ICollection<UsersitesRequestModel> Usersites { get; set; }

        public virtual ICollection<UserAccessAppViewModel> UserAccessApps { get; set; }

    }
}
