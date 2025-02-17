using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddUpdateSiteRequestModel
    {
        public Guid site_id { get; set; }
        public Guid company_id { get; set; }
        public Guid client_company_id { get; set; }
        public string site_name { get; set; }
        public string site_code { get; set; }
        public string location { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public bool isAutoApprove { get; set; } = false;
        public bool showHideApprove { get; set; } = false;
        public bool isManagerNotes { get; set; } = false;
    }
}
