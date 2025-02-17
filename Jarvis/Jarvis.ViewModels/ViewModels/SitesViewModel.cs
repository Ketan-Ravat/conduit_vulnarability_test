using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class SitesViewModel
    {
        public Guid site_id { get; set; }

        public string company_id { get; set; }

        public string comapny_name { get; set; }

        public string site_name { get; set; }

        public string site_code { get; set; }

        public string location { get; set; }

        public string status { get; set; }

        public string status_name { get; set; }

        public string timezone { get; set; }


        //public CompanyViewModel Company { get; set; }

        //public virtual ICollection<UserSites> Usersites { get; set; }
    }
}
