using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class GetAllCompanyResponseModel
    {
        public Guid company_id { get; set; }

        public string company_name { get; set; }

        public string company_code { get; set; }
        public string domain_name { get; set; }

        public DateTime created_at { get; set; }

        public string created_by { get; set; }

        public DateTime modified_at { get; set; }

        public string modified_by { get; set; }

        public int status { get; set; }

        public string status_name { get; set; }

        public virtual ICollection<GetCompanySitesViewModel> Sites { get; set; }
    }
    public class GetCompanySitesViewModel
    {
        public Guid site_id { get; set; }

        public string site_name { get; set; }

        public string site_code { get; set; }

        public string location { get; set; }

        public int status { get; set; }

        public string status_name { get; set; }

        public DateTime created_at { get; set; }

        public string created_by { get; set; }

        public DateTime modified_at { get; set; }

        public string modified_by { get; set; }
    }
}
