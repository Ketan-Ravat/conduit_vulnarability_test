using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetTopLevelAssetsResponseModel
    {
        public Guid asset_id { get; set; }
        public string name { get; set; }

        public string formio_building_name { get; set; }
        public string formio_floor_name { get; set; }
        public string formio_room_name { get; set; }
        public string formio_section_name { get; set; }

        public virtual List<SubLevelComponentAssets> subLevel_components { get; set; }

    }

    public class SubLevelComponentAssets
    {
        public Guid asset_sublevelcomponent_mapping_id { get; set; }
        public Guid asset_id { get; set; }
        public Guid sublevelcomponent_asset_id { get; set; }
        public int? formiosection_id { get; set; }
        public string formio_section_name { get; set; }

        public string sublevelcomponent_asset_name { get; set; }
        public string circuit { get; set; }
        public string image_name { get; set; }
        public string image_name_url { get; set; }
        public string asset_class_type { get; set; }

    }
}
