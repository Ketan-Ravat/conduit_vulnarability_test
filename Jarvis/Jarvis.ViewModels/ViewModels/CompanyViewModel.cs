using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class CompanyViewModel
    {
        public Guid company_id { get; set; }

        public string company_name { get; set; }

        public string company_code { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public Nullable<int> status { get; set; }
    }
}
