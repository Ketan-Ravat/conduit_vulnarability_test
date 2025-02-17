using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class InspectionSiteViewModel
    {
        public Guid site_id { get; set; }

        public string site_name { get; set; }

        public string site_code { get; set; }

        public string location { get; set; }

        public string status { get; set; }

        public DateTime created_at { get; set; }

        public string created_by { get; set; }

        public DateTime modified_at { get; set; }

        public string modified_by { get; set; }

        public string company_id { get; set; }

        public string company_name { get; set; }

        public string company_code { get; set; }
        //public CompanyViewModel Company { get; set; }

        public bool isAutoApprove { get; set; }

        public bool showHideApprove { get; set; }

        public bool isManagerNotes { get; set; }

        public string timezone { get; set; }
    }
}
