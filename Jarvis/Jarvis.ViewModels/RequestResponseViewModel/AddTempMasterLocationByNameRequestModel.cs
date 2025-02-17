using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddTempMasterLocationByNameRequestModel
    {
        public Guid wo_id { get; set; }

        public Guid? temp_master_building_id { get; set; }
        public Guid? temp_master_floor_id { get; set; }

        public string location_name { get; set; }
        public string building_name { get; set; }
        public string floor_name { get; set; }
        public string room_name { get; set; }
        public int location_type { get; set; }
        public int? formiobuilding_id { get; set; }
        public int? formiofloor_id { get; set; }
        public int? formioroom_id { get; set; }
    }
}
