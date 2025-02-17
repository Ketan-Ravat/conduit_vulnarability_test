using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllFloorsByBuildingResponseModel
    {
        public List<GetAllFloorsByBuildingData> floor_list {  get; set; }
        public int listsize { get; set; }
    }
    public class GetAllFloorsByBuildingData
    {
        public int room_count { get; set; }
        public int formiofloor_id { get; set; }
        public int? formiobuilding_id { get; set; }
        public string formio_floor_name { get; set; }
    }
}
