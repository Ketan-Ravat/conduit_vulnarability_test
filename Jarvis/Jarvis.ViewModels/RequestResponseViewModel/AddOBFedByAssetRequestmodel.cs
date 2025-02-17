using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddOBFedByAssetRequestmodel
    {
        public string asset_name { get; set; }
        public Guid inspectiontemplate_asset_class_id { get; set; }
        public Guid wo_id { get; set; }
        public bool is_woline_from_other_inspection { get; set; }// if install woline is from issue/PM then this will be true and do not show this in wo datails screen
        public List<SubComponent_AssetDetails_Class>? subcomponents_list { get; set; }

        //public string formio_building_name { get; set; }  //-- Deprecated , Now we are assigning Default Locations to Fed By
        //public string formio_floor_name { get; set; }
        //public string formio_room_name { get; set; }
        //public string formio_section_name { get; set; }

        //public int? inspection_type { get; set; }
    }

    public class SubComponent_AssetDetails_Class
    {
        public string asset_name { get; set; }
        public Guid inspectiontemplate_asset_class_id { get; set; }
    }
}
