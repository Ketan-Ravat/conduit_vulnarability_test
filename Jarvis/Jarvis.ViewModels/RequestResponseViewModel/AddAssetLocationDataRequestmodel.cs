using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddAssetLocationDataRequestmodel
    {
        public int? formiobuilding_id { get; set; }
        public int? formiofloor_id { get; set; }
        public string location_name { get; set; }
        public string building_name { get; set; }
        public string floor_name { get; set; }
        public string room_name { get; set; }
        public int location_type { get; set; }// 1 - building , 2 - Floor , 3 - Room

        public int? room_conditions { get; set; }

        public string access_notes { get; set; }

        public string issue { get; set; }


    }
}
