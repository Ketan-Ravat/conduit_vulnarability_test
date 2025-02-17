using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateAssetPMRequestModel
    {
        public Guid asset_pm_id { get; set; }
        
        public Guid asset_id { get; set; }

        public Guid pm_id { get; set; }

        public string title { get; set; }

        public string description { get; set; }
        /// <summary>
        /// Open = 31,
        /// Waiting = 42,
        /// In Progress = 43,
        /// </summary>
        public int status { get; set; }

        public Guid asset_pm_plan_id { get; set; }

        public int? work_procedure_type { get; set; }

        public Guid? form_id { get; set; }

        public int pm_trigger_type { get; set; }
        public Nullable<int> estimation_time { get; set; } // number of minutes 
        public int pm_trigger_by { get; set; }
        public Nullable<bool> is_trigger_on_starting_at { get; set; }
        public DateTime datetime_starting_at { get; set; }

        public List<AssetPMAttachmentsRequestModel> asset_pm_attachments { get; set; }
        public List<AssetPMsTriggerConditionMappingRequestModel> asset_pm_trigger_condition_mapping_request_model { get; set; }
    }
    public class AssetPMsTriggerConditionMappingRequestModel
    {
        public Guid asset_pm_trigger_condition_mapping_id { get; set; }
        public Nullable<int> datetime_repeates_every { get; set; } // total years or month count to repeat
        public Nullable<int> datetime_repeat_time_period_type { get; set; } //29 - Month, 30 - Year, 39 - Week, 40 - Day
    }
}
