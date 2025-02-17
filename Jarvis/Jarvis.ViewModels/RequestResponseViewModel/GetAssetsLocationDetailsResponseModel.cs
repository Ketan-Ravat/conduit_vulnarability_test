using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetsLocationDetailsResponseModel
    {
        public Guid asset_id { get; set; }
        public string QR_code { get; set; }
        public string name { get; set; }
        public string formio_building_name { get; set; }
        public string formio_floor_name { get; set; }
        public string formio_room_name { get; set; }
        public string formio_section_name { get; set; }
        public string asset_details_url { get; set; }

        public int? criticality_index_type { get; set; }
        public int? condition_index_type { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string pm_plan_name { get; set; }
    }
}
