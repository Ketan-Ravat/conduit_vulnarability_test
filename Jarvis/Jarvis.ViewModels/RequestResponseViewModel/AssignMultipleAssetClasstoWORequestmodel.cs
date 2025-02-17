using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssignMultipleAssetClasstoWORequestmodel
    {
        public List<AssignAssetClasstoWOList> assign_inspection_asset_class_list { get; set; }

        public Guid wo_id { get; set; }
    }
    public class AssignAssetClasstoWOList
    {
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public Guid? form_id { get; set; }
        public Guid wo_id { get; set; }
        public int wo_type { get; set; }
        public Guid? asset_id { get; set; }// if asset is selected from main asset list
        public Guid? wo_ob_asset_id { get; set; }// if asset is selected from OB/repair
        public int? inspection_type { get; set; }
        public int? status { get; set; }
        public string new_create_asset_name { get; set; } // if WO line is for new create asset
        public Guid? asset_pm_id { get; set; } // for internal BE use only
    }
}
