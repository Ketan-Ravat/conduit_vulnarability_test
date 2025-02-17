using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetSubcomponentsForFedByMappingResponseModel
    {
        public List<TopLevelAssetData> toplevel_main_assets { get; set; }
        public List<SubLevelAssetData> subcomponent_main_assets { get; set; }
        public List<TopLevelAssetData> toplevel_obwo_assets { get; set; }
        public List<SubLevelAssetData> subcomponent_obwo_assets { get; set; }
    }

    public class TopLevelAssetData
    {
        public string asset_name { get; set; }    
        public string asset_id { get; set; }
        public string woonboardingassets_id { get; set; }
    }
    public class SubLevelAssetData
    {
        public string asset_name { get; set; }
        public string asset_id { get; set; }
        public string woonboardingassets_id { get; set; }
        public Guid toplevelcomponent_asset_id { get; set; }
    }
}
