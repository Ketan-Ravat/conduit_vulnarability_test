using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetLocationDataByAssetIdResponseModel
    {
        public int? formiobuilding_id { get; set; }
        public int? formiofloor_id { get; set; }
        public int? formioroom_id { get; set; }
        public int? formiosection_id { get; set; }
        public string formio_building_name { get; set; }
        public string formio_floor_name { get; set; }
        public string formio_room_name { get; set; }
        public string formio_section_name { get; set; }
    }
}
