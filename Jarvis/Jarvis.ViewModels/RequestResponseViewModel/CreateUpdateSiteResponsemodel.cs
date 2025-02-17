using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class CreateUpdateSiteResponsemodel
    {
        public int success { get; set;}
        public string user_emails { get; set;}
        public Guid site_id { get; set;}
        public string site_name { get; set;}
        public Guid? client_company_id { get; set; }
    }
}
