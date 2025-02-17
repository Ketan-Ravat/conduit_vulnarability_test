using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class BulkupdatePMLastcompletedRequestModel
    {
        public List<asset_pm_id_with_last_completed_date> asset_pm_id_with_last_completed_date_list { get; set; }
    }

    public class asset_pm_id_with_last_completed_date
    {
        public Guid asset_pm_id { get; set; }
        public DateTime last_completed_date { get; set; }
        public bool is_assetpm_enabled { get; set; } // false = disabled , true = enabled
    }
}
