using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllClientCompanyWithSitesResponseModel
    {
        public Guid client_company_id { get; set; }
        public string client_company_name { get; set; }
        public string client_company_code { get; set; }
        public Guid? parent_company_id { get; set; }
        public string owner { get; set; }
        public string owner_address { get; set; }
        public int status { get; set; }
        public virtual List<Site_Data> list_of_site { get; set; }

    }
    public class Site_Data
    {
        public Guid site_id { get; set; }
        public Guid company_id { get; set; }
        public string site_name { get; set; }
        public string site_code { get; set; }
        public string customer { get; set; }
        public bool is_add_asset_class_enabled { get; set; }
        public string customer_address { get; set; }
        public string customer_address_2 { get; set; }
        public Guid? sitecontact_id { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip {  get; set; }
        public int status { get; set; }
        public string profile_image { get; set; }
        public List<SiteProjectManagerMapping_View_Class> site_projectmanager_list { get;set; }
    }
    public class SiteProjectManagerMapping_View_Class
    {
        public Guid site_projectmanager_mapping_id { get; set; }
        public Guid user_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }

}
