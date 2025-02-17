using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddMainAssetInactivePMinWORequestmodel
    {
        public List<main_asset_inactive_pm> in_active_pm { get; set; }
        public Guid asset_id { get; set; }
        public Guid wo_id { get; set; }
        public WOOnboardingAssets install_woline { get; set; }
    }

    public class main_asset_inactive_pm
    {
        public Guid pm_id { get; set; }
        public string pm_form_output_data { get; set; }
        public int woline_status { get; set; }
    }
}
