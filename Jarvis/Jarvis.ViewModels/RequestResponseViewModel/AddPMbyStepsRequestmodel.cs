using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddPMbyStepsRequestmodel
    {
        public UpdateOBWOAssetDetailsRequestmodel install_woline_details { get; set; }
        public List<PMListttoAdd> pm_list { get; set; }
        public Guid wo_id { get; set; }
        public int asset_type { get; set; } // 1 - existing asset , 2 - temp asset , 3 - new asset
        public Guid? selected_asset_id { get; set; }
    }

    public class PMListttoAdd
    {
        public Guid? asset_pm_id { get; set; }
        public Guid? pm_id { get; set; }
        public int woline_status { get; set; }
        public string pm_form_output_data { get; set; }
    }
}
