using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllPMsByPlanIdResponsemodel
    {
        public Guid pm_id { get; set; }
        public string title { get; set; }
        public int pm_inspection_type_id { get; set; }
        public int pm_trigger_type { get; set; }

        public int? work_procedure_type { get; set; }

        public Guid? form_id { get; set; }
        public Nullable<int> estimation_time { get; set; } // number of minutes 
        public List<PMConditionmapping> pm_trigger_condition_mapping_response_model { get; set; }
    }
    public class PMConditionmapping
    {
        public Guid pm_trigger_condition_mapping_id { get; set; }
        public Nullable<int> datetime_repeates_every { get; set; } // total years or month count to repeat
        public Nullable<int> datetime_repeat_time_period_type { get; set; } //29 - Month, 30 - Year, 39 - Week, 40 - Day
        public int condition_type_id { get; set; } // 1 - condition-1 , 2 - condition-2 ,3 - condition-3
    }
}
