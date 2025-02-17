using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetSiteContactByIdResponseModel
    {
        public string sitecontact_title { get; set; }

        public string sitecontact_name { get; set; }

        public string sitecontact_email { get; set; }

        public string sitecontact_phone { get; set; }
    }
}
