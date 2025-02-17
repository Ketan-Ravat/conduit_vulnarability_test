using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetPMPlansByClassIdResponsemodel
    {
        public Guid pm_plan_id { get; set; }
        public string plan_name { get; set; }
        public Guid pm_category_id { get; set; }
        public int pm_count { get; set;  }
    }
}
