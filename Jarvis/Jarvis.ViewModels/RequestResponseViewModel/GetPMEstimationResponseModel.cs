using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetPMEstimationResponseModel
    {
        public Guid? pm_plan_id { get; set; }

        public string? plan_name { get; set; }

        public List<PMEstimation> pm_estimation_list { get; set; }
    }

    public class PMEstimation
    {
        public Guid pm_id { get; set; }

        public string title { get; set; }

        public Nullable<int> estimation_time { get; set; }

        public Guid? sitewalkthrough_temp_pm_estimation_id { get; set; }
    }
}
