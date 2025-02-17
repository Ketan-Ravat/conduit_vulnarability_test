using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.EmailRequestViewModel
{
    public class WOTechRemoveEmailRequestmodel
    {
        public string company_logo_url { get; set; }
        public string technician_name { get; set; }
        public string client_company_name { get; set; }
        public string site_name { get; set; }
        public string manual_wo_number { get; set; }
        public string wo_description { get; set; }
        public string wo_start_date { get; set; }
        public string due_at { get; set; }
        public string email_subject { get; set; }
    }
}
