using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllBuildingLocationsResponseModel
    {
        public List<GetAllBuildingLocationsData> building_list { get; set; }
        public int listsize { get; set; }
    }
    public class GetAllBuildingLocationsData
    {
        public int formiobuilding_id { get; set; }
        public string formio_building_name { get; set; }
        public int floor_count { get; set; }
    }
}
