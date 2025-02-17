using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllTempMasterLocationForWOResponseModel
    {
        public List<temp_master_building_class> temp_master_buildings { get; set; }
    }
    public class temp_master_building_class
    {
        public int? formiobuilding_id { get; set; }
        public Guid temp_master_building_id { get; set; }
        public string building_name { get; set; }
        public List<temp_master_floor_class> temp_master_floor { get; set; } = new List<temp_master_floor_class>();
    }
    public class temp_master_floor_class
    {
        public int? formiofloor_id { get; set; }
        public Guid temp_master_floor_id { get; set; }
        public string floor_name { get; set; }
        public string building_name { get; set; }
        public List<temp_master_room_class> temp_master_rooms { get; set; } = new List<temp_master_room_class>();
    }
    public class temp_master_room_class
    {
        public int? formioroom_id { get; set; }
        public Guid temp_master_room_id { get; set; }
        public string room_name { get; set; }
        public string floor_name { get; set; }
        public string building_name { get; set; }

        public int? room_assets_count { get; set; }
    }
}
