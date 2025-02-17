using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetComponantLevelAssetsResponsemodel
    {
       public List<MainAssets> main_assets_list { get; set; }
       public List<TempAssets> woline_assets { get; set; }

    }
    public class MainAssets
    {
        public string asset_name { get; set; }
        public Guid asset_id {get; set; }
        public string building_name { get; set; }
        public string floor_name { get; set; }
        public string room_name { get; set; }
    }
    public class TempAssets
    {
        public Guid woonboardingassets_id { get; set; }
        public string asset_name { get; set; }
        public string building_name { get; set; }
        public string floor_name { get; set; }
        public string room_name { get; set; }
    }
}
