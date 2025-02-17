using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.EmailRequestViewModel
{
    public class WOLeadRemoveEmailRequestmodel
    {
        public string company_logo_url { get; set; }
        public string lead_name { get; set; }
        public string client_company_name { get; set; }
        public string site_name { get; set; }
        public string manual_wo_number { get; set; }
        public string wo_description { get; set; }
        public string wo_start_date { get; set; }
        public string due_at { get; set; }
        public string email_subject { get; set; }
    }
}
