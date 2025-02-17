using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddTempLocationDataRequestModel
    {
        public Guid wo_id { get; set; }

        public Guid? temp_formiobuilding_id { get; set; }
        public Guid? temp_formiofloor_id { get; set; }

        public string location_name { get; set; }
        public string building_name { get; set; }
        public string floor_name { get; set; }
        public string room_name { get; set; }
        public int location_type { get; set; }  // 1 - building , 2 - Floor , 3 - Room, 4 - Section

    }
}
