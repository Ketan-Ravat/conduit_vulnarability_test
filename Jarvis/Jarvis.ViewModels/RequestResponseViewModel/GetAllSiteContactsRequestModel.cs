using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllSiteContactsRequestModel
    {
        public string search_string { get; set; }
        public int pagesize { get; set; }
        public int pageindex { get; set; }

        public Guid client_company_id { get; set; }
    }
}
