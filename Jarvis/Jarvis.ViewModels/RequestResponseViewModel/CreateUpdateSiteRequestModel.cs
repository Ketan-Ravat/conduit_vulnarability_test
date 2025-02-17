using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class CreateUpdateSiteRequestModel
    {
        public Guid? site_id { get; set; }
        public Guid client_company_id { get; set; }
        public Guid company_id { get; set; }
        public string site_name { get; set; }
        public string site_code { get; set; }
        public int status { get; set; }

        public string city { get; set; }

        public string state { get; set; }

        public string zip { get; set;}
        public bool is_add_asset_class_enabled { get; set; }

        public Guid? sitecontact_id { get; set; }        
        public string customer { get; set; }
        public string customer_address { get; set; }

        public string customer_address_2 {  get; set; }
        public string profile_image { get; set; }
        public List<SiteProjectManagerMapping_Class> site_projectmanager_list { get; set; }
    }
    public class SiteProjectManagerMapping_Class
    {
        public Guid? site_projectmanager_mapping_id { get; set; }
        public Guid user_id { get; set; }
        public bool is_deleted { get; set; }
    }
}
