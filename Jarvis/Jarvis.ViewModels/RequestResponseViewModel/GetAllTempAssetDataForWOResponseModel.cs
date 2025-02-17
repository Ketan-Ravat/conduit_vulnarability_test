using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllTempAssetDataForWOResponseModel
    {
        public List<temp_asset_data> asset_data { get; set; }
        public List<assets_fedby_mappings_class>? assets_fedby_mappings { get; set; }
        public List<asset_subcomponents_mappings_class>? asset_subcomponents_mappings { get; set; }
    }
    public class temp_asset_data
    {
        public Guid? asset_id { get; set; }
        public Guid woonboardingassets_id { get; set; }
        public string asset_name { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public int? location { get; set; }
        public string QR_code { get; set; }
        public int? condition_index_type { get; set; }
        public int? criticality_index_type { get; set; }
        public int? asset_operating_condition_state { get; set; }
    }
}
