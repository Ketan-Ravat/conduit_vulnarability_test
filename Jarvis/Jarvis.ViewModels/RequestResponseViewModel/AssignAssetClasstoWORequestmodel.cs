using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssignAssetClasstoWORequestmodel
    {
        public Guid inspectiontemplate_asset_class_id { get; set; }
        public Guid form_id { get; set; }
        public Guid wo_id { get; set; }
        public int wo_type { get; set; }
        public Guid? asset_id { get; set; }// if asset is selected from main asset list
        public int inspection_type { get; set; }
        public string asset_name { get; set; }// this is used in MWO when adding new wo line in inspection type for new asset
        public string group_string { get; set; }
    }
}
