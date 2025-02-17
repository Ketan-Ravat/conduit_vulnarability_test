using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetPMWOlineByIDStepsResponsemodel
    {
        public GetOBWOAssetDetailsByIdResponsemodel installl_woline { get; set; }

        public List<WOActivePMDetails> linked_active_pm_list { get; set; }
        public List<WOTempPMDetails> linked_temp_active_pm_list { get; set; }
    }

    public class WOActivePMDetails
    {
        public Guid asset_pm_id { get; set; }
        public Guid pm_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string due_in { get; set; }
        public string pm_plan_name { get; set; }
        public Guid woonboardingassets_id { get; set; }
    }
    public class WOTempPMDetails
    {
        public Guid? temp_asset_pm_id { get; set; }
        public Guid? asset_pm_id { get; set; }
        public Guid pm_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string pm_plan_name { get; set; }
        public Guid woonboardingassets_id { get; set; }
    }
}
