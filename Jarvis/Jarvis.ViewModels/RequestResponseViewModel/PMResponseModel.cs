﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class PMResponseModel
    {
        public int response_status { get; set; }
        public Guid pm_id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public int status { get; set; }

        public Guid pm_plan_id { get; set; }

        public int pm_trigger_type { get; set; }

        public int pm_trigger_by { get; set; }

        public int? work_procedure_type { get; set;}

        public Guid? form_id { get; set; }
        public Nullable<int> estimation_time { get; set; } // number of minutes 

        public Nullable<DateTime> created_at { get; set; }

        public Nullable<bool> is_trigger_on_starting_at { get; set; }
        public bool is_archive { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public string status_name { get; set; }

        public string pm_type_status_name { get; set; }

        public string pm_by_status_name { get; set; }

        public string pm_datetime_repeat_type_status_name { get; set; }
       // public List<PMTasksResponseModel> PMTasks { get; set; }

        public List<PMAttachmentsResponseModel> pm_attachments { get; set; }
       // public ServiceDealerViewModel ServiceDealers { get; set; }

        public List<PMsTriggerConditionMappingResponsemodel> pm_trigger_condition_mapping_response_model { get; set; }
    }
    public class PMsTriggerConditionMappingResponsemodel
    {
        public Guid pm_trigger_condition_mapping_id { get; set; }
        public Nullable<int> datetime_repeates_every { get; set; } // total years or month count to repeat
        public Nullable<int> datetime_repeat_time_period_type { get; set; } //29 - Month, 30 - Year, 39 - Week, 40 - Day
        public int condition_type_id { get; set; } // 1 - condition-1 , 2 - condition-2 ,3 - condition-3
        public Guid pm_id { get; set; }
        public Guid site_id { get; set; }
        public bool is_archive { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
    }
}
