using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class PMMarkCompletedRequestModel
    {
        public Guid pm_trigger_id { get; set; }

        public Guid asset_pm_id { get; set; }

        public string comments { get; set; }

        public DateTime completed_on { get; set; }

        public int completed_at_meter_hours { get; set; }

        public int completed_in_hours { get; set; }

        public int completed_in_minutes { get; set; }
    }
}
