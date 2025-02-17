using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class SiteListResponseModel
    {
        public Guid site_id { get; set; }
        public Guid company_id { get; set; }
        public Guid client_company_id { get; set; }
        public string site_name { get; set; }
        public string site_code { get; set; }
        public string location { get; set; }
        public Nullable<int> status { get; set; }
        public string customer { get; set; }
        public string customer_address { get; set; }
        public bool isAutoApprove { get; set; }
        public bool showHideApprove { get; set; }
        public bool isManagerNotes { get; set; }
        public string timezone { get; set; }
    }
}
