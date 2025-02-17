using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetSiteUsersDetailsByIdResponseModel
    {
        public Guid site_id { get; set; }
        public Guid company_id { get; set; }
        public string site_name { get; set; }
        public string site_code { get; set; }
        public string customer { get; set; }
        public bool is_add_asset_class_enabled { get; set; }
        public string customer_address { get; set; }
        public int? status { get; set; }
        public string profile_image { get; set; }
        public List<SiteProjectManagerMapping_View_Class> site_projectmanager_list { get; set; }

        public List<Site_Users_List> site_users_list { get; set; }
    }

    public class Site_Users_List
    {
        public Guid user_id { get; set; }
        public string user_name { get; set; }
        public string user_email { get; set; }
        public List<string> roles_list { get; set; }
    }

   

}
