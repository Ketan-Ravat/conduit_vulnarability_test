using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class PMPlansRequestModel
    {
        public Guid pm_plan_id { get; set; }

        public string plan_name { get; set; }

        public Guid pm_category_id { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public Nullable<int> status { get; set; }
        public bool is_default_pm_plan { get; set; }
    }
}
