using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class PMCategoryRequestModel
    {
        public Guid pm_category_id { get; set; }

        public string category_name { get; set; }

        public string category_code { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public Nullable<int> status { get; set; }

        public string company_id { get; set; }
    }
}
