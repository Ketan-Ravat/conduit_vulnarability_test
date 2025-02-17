using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MobileAssetClassResponsemodel
    {
        public Guid inspectiontemplate_asset_class_id { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public int form_type_id { get; set; }
        public Guid company_id { get; set; }
        public bool isarchive { get; set; }
        public string form_nameplate_info { get; set; }
        public string classType { get; set; }
    }
}
