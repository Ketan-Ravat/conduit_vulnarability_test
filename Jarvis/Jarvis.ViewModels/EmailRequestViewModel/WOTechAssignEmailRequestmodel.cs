using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.EmailRequestViewModel
{
    public class WOTechAssignEmailRequestmodel
    {
        public string company_logo_url { get; set; }
        public string technician_name { get; set; }
        public string client_company_name { get; set; }
        public string site_name { get; set; }
        public string manual_wo_number { get; set; }
        public string wo_description { get; set; }
        public string wo_start_date { get; set; }
        public string due_at { get; set; }
        public string lead_names { get; set; }
        public string technicians_names { get; set; }
        public string email_subject { get; set; }
        public bool wo_update { get; set; }
        public List<tech_details> technicians { get; set; }
        public List<pm_details_class> pms { get; set; }
        public List<vendor_details_class> vendors { get; set; }
    }
    public class tech_details
    {
        public string technician_name { get; set; }
        public string technician_email { get; set; }
        public string technician_phone { get; set; }
    }
    public class pm_details_class
    {
        public string pm_name { get; set; }
        public string pm_email { get; set; }
        public string pm_phone { get; set; }
    }
    public class vendor_details_class
    {
        public string vendor_name { get; set;}
        public List<vendor_contact_details_class> vendor_contacts { get; set; }
    }
    public class vendor_contact_details_class
    {
        public string vendor_contact_name { get;set;}
        public string vendor_phone { get;set;}
        public string vendor_email { get;set;}
    }
}
