using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddAssetPMWolineRequestmodel
    {
        public List<Guid> asset_pm_id { get; set; }
        public Guid wo_id { get; set; } 
        public bool is_request_from_assetpm_service { get; set; } // if request is from AssetPM service then update tempasset_id and submitted data in woline
        public Guid? tempasset_id { get; set; }
        public string pm_form_output_data { get; set; }
    }
}
