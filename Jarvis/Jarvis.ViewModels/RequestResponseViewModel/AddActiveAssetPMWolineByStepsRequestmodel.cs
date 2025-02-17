using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddActiveAssetPMWolineByStepsRequestmodel
    {
        public Guid wo_id { get; set; }
        public List<active_pm_data> pm_list { get; set; }
        public WOOnboardingAssets install_woline { get; set; }

    }

    public class active_pm_data
    {
        public Guid asset_pm_id { get; set; }
        public string pm_form_output_data { get; set; }
        public int woline_status { get; set; }
    }
}
