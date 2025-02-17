using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class PMCategoryResponseModel
    {
        public int response_status { get; set; }
        public string response_message { get; set; }
        public Guid pm_category_id { get; set; }

        public string category_name { get; set; }

        public string category_code { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public Nullable<int> status { get; set; }
       // public List<PMPlansResponseModel> PMPlans { get; set; }
        public int pmPlansCount { get; set; }
    }
}
