using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddUpdateSiteContactRequestModel
    {
        public Guid? sitecontact_id { get; set; }

        public Guid client_company_id { get; set; }
        public string sitecontact_title { get; set; }

        public string sitecontact_name { get; set; }

        public string sitecontact_email { get; set; }

        public string sitecontact_phone { get; set; }

    }
}
