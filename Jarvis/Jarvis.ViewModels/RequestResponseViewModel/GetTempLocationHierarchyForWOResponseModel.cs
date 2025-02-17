using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetTempLocationHierarchyForWOResponseModel
    {
        public List<temp_WOlineBuildings> temp_buildings { get; set; }
    }
    public class temp_WOlineBuildings
    {
        public Guid temp_formiobuilding_id { get; set; }
        public string temp_formio_building_name { get; set; }
        public List<temp_WOlineFloors> temp_floors { get; set; } = new List<temp_WOlineFloors>();
    }
    public class temp_WOlineFloors
    {
        public Guid temp_formiofloor_id { get; set; }
        public string temp_formio_floor_name { get; set; }
        public Guid temp_formiobuilding_id { get; set; }
        public string temp_formio_building_name { get; set; }
        public List<temp_WOlineRooms> temp_rooms { get; set; } = new List<temp_WOlineRooms>();
    }
    public class temp_WOlineRooms
    {
        public Guid temp_formioroom_id { get; set; }
        public string temp_formio_room_name { get; set; }
        public Guid temp_formiofloor_id { get; set; }
        public string temp_formio_floor_name { get; set; }
        public string temp_formio_building_name { get; set; }
    }
}
