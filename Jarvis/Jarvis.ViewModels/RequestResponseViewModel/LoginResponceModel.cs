using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels
{
    public class LoginResponceModel
    {
        public int response_status { get; set; }
        public string response_message { get; set; }

        public Guid uuid { get; set; }

        public string email { get; set; }

        public string username { get; set; }
        public string mobile_number { get; set; }
        public string firstname { get; set; }
        
        public string lastname { get; set; }

        public int status { get; set; }

        public string status_name { get; set; }

        public string profile_picture_url { get; set; }
        public string profile_picture_name { get; set; }

        public string job_title { get; set; }
        public string signature_url { get; set; }
        public bool email_notification_status { get; set; }
        public bool operator_usage_report_email_not_status { get; set; }
        public bool manager_pm_notification_status { get; set; }
        public int? executive_report_status { get; set; }

        public string rolename { get; set; }
        public string default_rolename_app { get; set; }

        public string default_rolename_app_name { get; set; }

        public string default_rolename_web { get; set; }

        public string default_rolename_web_name { get; set; }

        public string active_rolename_app { get; set; }

        public string active_rolename_app_name { get; set; }

        public string active_rolename_web { get; set; }

        public string active_rolename_web_name { get; set; }

        //public string ti_default_role { get; set; }

        //public string ti_default_site { get; set; }

        public string default_site_id { get; set; }

        public string active_site_id { get; set; }
        public string default_company_id { get; set; }

        public string active_company_id { get; set; }

        public string ac_default_site { get; set; }

        public string ac_default_role_app { get; set; }

        public string ac_default_role_web { get; set; }

        public string default_site_name { get; set; }
        public string active_site_name { get; set; }
        public string default_company_name { get; set; }
        public string active_company_name { get; set; }
        public int active_site_status { get; set; }
        public int default_site_status { get; set; }
        public int active_company_status { get; set; }
        public int default_company_status { get; set; }

        public int? prefer_language_id { get; set; }

        public string prefer_language_name { get; set; }

        public List<UserSite> Usersites { get; set; }
       
        public ExecutivePMDueEmailConfigResponseModel UserEmailNotificationConfigurationSettings { get; set; }

        public List<UserRole> Userroles { get; set; }
        public List<UserSite> default_client_company_Usersites { get; set; }// only default company sites
        public List<Client_Company> client_company { get; set; }
        public Guid? ac_default_client_company { get; set; }
        public Guid? ac_active_client_company { get; set; }
        public string ac_default_client_company_name { get; set; }
        public string ac_active_client_company_name { get; set; }
        public Guid user_session_id { get; set; }
        public bool is_egalvanic_ai_required { get; set; }
        public bool is_estimator_feature_required { get; set; }
        public bool is_allowed_to_update_formio { get; set; }
        public bool is_mfa_enabled { get; set; }
        public bool is_reactflow_required { get; set; }
        public bool is_retool_bulk_operation_required { get; set; }
        public bool is_required_maintenance_command_center { get; set; }
    }

    public class Client_Company 
    {
        public Guid client_company_id { get; set; }
        public string client_company_name { get; set; }
        public string owner { get; set; }
        public string owner_address { get; set; }
        public string logo_url { get; set; }
        public List<UserSite> client_company_Usersites { get; set; }
    }

    public class UserSite
    {
        public Guid site_id { get; set; }

        public string site_name { get; set; }

        public string company_id { get; set; }

        public string company_name { get; set; }
        public Guid? client_company_id { get; set; }

        //public Guid? sitecontact_id { get; set; }

        //public string sitecontact_name { get; set; }

        public string site_code { get; set; }

        public string location { get; set; }

        public int status { get; set; }

        public string city { get; set; }

        public string state { get; set; }

        public string zip {  get; set; }
        public string customer { get; set; }
        public string customer_address { get; set; }

        public string customer_address_2 { get; set; }

        public int main_site_status { get; set; }
        public Site_Contact_Details_Obj? site_contact_details { get; set; }

        //public DateTime created_at { get; set; }

        //public string created_by { get; set; }

        //public DateTime modified_at { get; set; }

        //public string modified_by { get; set; }

        //public CompanyViewModel Company { get; set; }
    }

    public class UserRole
    {
        public Guid role_id { get; set; }
        public string role_name { get; set; }
        public int status { get; set; }

    }

    public class UserAccessApps
    {
        public int app_id { get; set; }

        public string app_name { get; set; }

        public int status { get; set; }
    }
    public class Site_Contact_Details_Obj
    {
        public Guid? sitecontact_id { get; set; }
        public string sitecontact_title { get; set; }
        public string sitecontact_name { get; set; }
        public string sitecontact_email { get; set; }
        public string sitecontact_phone { get; set; }
    }
}
