using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetsToAssignOBWORequestmodel
    {
        public Guid? wo_id {  get; set; }
        public int pagesize { get; set; } = 0;
        public int pageindex { get; set; } = 0;
        public int? formiobuilding_id { get; set; } // deprecated
        public int? formiofloor_id { get; set; } // deprecated
        public int? formioroom_id { get; set; } // deprecated

        public string temp_formio_building_name { get; set; }
        public string temp_formio_floor_name { get; set; }
        public string temp_formio_room_name { get; set; }
        public string search_string { get; set; }

    }
}
