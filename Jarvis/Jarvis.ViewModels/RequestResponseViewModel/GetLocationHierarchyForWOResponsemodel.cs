using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetLocationHierarchyForWOResponsemodel
    {
        public List<WOlineBuilding> buildings { get; set; }
    }
    public class WOlineBuilding
    {
        public int formiobuilding_id { get; set; }
        public string formio_building_name { get; set; }
        public int floor_count { get; set; }
        public int room_count { get; set; }
        public int asset_count { get; set; }
        public List<WOlineFloors> floors { get; set; }
        
    }
    public class WOlineFloors
    {
        public int formiofloor_id { get; set; }
        public string formio_floor_name { get; set; }
        public int formiobuilding_id { get; set; }
        public int room_count { get; set; }
        public int asset_count { get; set; }
        public List<WOlineRooms> rooms { get; set; }
    }
    public class WOlineRooms
    {
        public int formioroom_id { get; set; }
        public string formio_room_name { get; set; }
        public int formiofloor_id { get; set; }
        public int asset_count { get; set; }

    }

}
