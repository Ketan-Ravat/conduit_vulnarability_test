using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetSubcomponentsByAssetIdResponsemodel
    {
        public Guid asset_sublevelcomponent_mapping_id { get; set; }
        public Guid sublevelcomponent_asset_id { get; set; }
        public string sublevelcomponent_asset_name { get; set; }
        public Guid sublevelcomponent_asset_class_id { get; set; }
        public string sublevelcomponent_asset_class_name { get; set; }
        public string sublevelcomponent_asset_class_code { get; set; }
        public string circuit { get; set; }
        public string image_name { get; set; }
        public string image_url { get; set; }
        public string rated_amps { get; set; }
    }
}
