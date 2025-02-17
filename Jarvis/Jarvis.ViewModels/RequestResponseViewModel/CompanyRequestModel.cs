using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.ViewModels
{
    public class CompanyRequestModel
    {
        [Key]
        public Guid company_id { get; set; }

        public string company_name { get; set; }

        public string company_code { get; set; }

        public DateTime created_at { get; set; }

        public string created_by { get; set; }

        public DateTime modified_at { get; set; }

        public string modified_by { get; set; }

        public int status { get; set; }

        //public virtual ICollection<SitesViewModel> Sites { get; set; }
    }
}
