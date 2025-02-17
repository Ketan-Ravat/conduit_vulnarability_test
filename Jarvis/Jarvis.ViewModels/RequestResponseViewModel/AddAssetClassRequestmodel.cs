using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddAssetClassRequestmodel
    {
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public int form_type_id { get; set; }
        public int? asset_expected_usefull_life { get; set; }         
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public Guid? set_default_pm_plan_id { get; set; }
        public bool want_to_remove_default_pmplan { get; set; }
    }
}
