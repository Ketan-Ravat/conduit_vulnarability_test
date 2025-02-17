using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddPMtoTempAssetRequestmodel
    {
        public Guid woonboardingassets_id { get; set; }
        public List<TempAssetPMlist> pm_list { get; set; }
        public Guid wo_id { get; set; }
        public WOOnboardingAssets install_woline { get; set; }
    }

    public class TempAssetPMlist
    {
        public Guid pm_id { get; set; }
        public string pm_form_output_data { get; set; }
    }
}
