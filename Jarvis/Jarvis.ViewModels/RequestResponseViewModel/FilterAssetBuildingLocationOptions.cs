using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterAssetBuildingLocationOptions
    {
        public List<FilterAssetBuildingLocationOptionsmapping> buildings { get; set; }  
        public List<FilterAssetBuildingLocationOptionsmapping> floors { get; set; }  
        public List<FilterAssetBuildingLocationOptionsmapping> rooms { get; set; }  
        public List<FilterAssetBuildingLocationOptionsmapping> sections { get; set; }
        public List<FilterAssetRoomFloorBuildingLocationOptionsmapping> rooms_with_floor_building { get; set; }

    }
    public class FilterAssetBuildingLocationOptionsmapping
    {
        public string label { get; set; }
        public int value { get; set; }
    }
    public class FilterAssetRoomFloorBuildingLocationOptionsmapping
    {
        public string room_name { get; set; }
        public int room_id { get; set; }
        public string floor_name { get; set; }
        public string building_name { get; set; }
        public int building_id { get; set; }
        public int floor_id { get; set; }
    }
}
