using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class GetUserDetailsResponseModel
    {
        public Guid uuid { get; set; }

        public Guid barcode_id { get; set; }

        public Guid role_id { get; set; }
        public string role_name { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public string username { get; set; }

        public string firstname { get; set; }

        public string lastname { get; set; }

        public Nullable<int> status { get; set; }

        public string status_name { get; set; }

        public string os { get; set; }

        public int prefer_language_id { get; set; }

        public string prefer_language_name { get; set; }

        public string default_rolename_app { get; set; }

        public string default_rolename_app_name { get; set; }

        public string default_rolename_web { get; set; }

        public string default_rolename_web_name { get; set; }

        //public int? default_app { get; set; }

        //public string default_app_name { get; set; }

        public Guid? default_site_id { get; set; }
        public string default_site_name { get; set; }

        public virtual ICollection<UserSitesViewModel> Usersites { get; set; }

        public virtual ICollection<UserRole> userroles { get; set; }
    }
}
