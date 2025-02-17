using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetActiveUserSitesAndRolesResponseModel
    {
        public List<ActiveUserSites_Data> usersites { get; set; }
        public List<ActiveUserRoles_Data> userroles { get; set; }
        public List<ActiveClintCompany_Data> client_company { get; set; }
        public Guid? active_site_id { get; set; }
        public Guid? default_site_id { get; set; }
        public Guid? default_client_company_id { get; set; }
        public Guid? active_client_company_id { get; set; }
        public Guid? active_role_id_web { get; set; }
        public Guid? default_role_id_web { get; set; }
        public string active_site_name { get; set; }
        public string default_site_name { get; set; }
        public string default_client_company_name { get; set; }
        public string active_client_company_name { get; set; }
        public string active_role_name_web { get; set; }
        public string default_role_name_web { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }

    public class ActiveUserSites_Data
    {
        public Guid site_id { get; set; }
        public string site_name { get; set; }
        public string site_code { get; set; }
        public Nullable<int> status { get; set; }
        public Guid? client_company_id { get; set; }
        public string client_company_name { get; set; }
        public Guid company_id { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string customer_address { get; set; }
        public string customer_address_2 { get; set; }
        public Site_Contact_Details_Obj? site_contact_details { get; set; }
    }
    public class ActiveUserRoles_Data
    {
        public Guid role_id { get; set; }
        public string role_name { get; set; }
    }
    public class ActiveClintCompany_Data
    {
        public Guid client_company_id { get; set; }
        public string client_company_name { get; set; }
        public List<ClientCompany_Site_Data> client_company_Usersites { get; set; }
    }
    public class ClientCompany_Site_Data
    {
        public Guid site_id { get; set; }
        public string site_name { get; set; }
    }
}
