using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateLocationDetailsRequestModel
    {
        public int? formiobuilding_id { get; set; }
        public int? formiofloor_id { get; set; }
        public int? formioroom_id { get; set; }
        public int? formiosection_id { get; set; }
        public string location_name { get; set; }
        public string building_name { get; set; }
        public string floor_name { get; set; }
        public int editing_location_flag { get; set; }
    }
}
