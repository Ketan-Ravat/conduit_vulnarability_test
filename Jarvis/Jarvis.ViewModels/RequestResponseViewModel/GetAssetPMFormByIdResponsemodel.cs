using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetPMFormByIdResponsemodel
    {
        public Guid? asset_pm_id { get; set; }
        public Guid? temp_asset_pm_id { get; set; }
        public string form_json { get; set; }
        public string pm_form_output_data { get; set; }

        public int success { get; set; }
    }
}
