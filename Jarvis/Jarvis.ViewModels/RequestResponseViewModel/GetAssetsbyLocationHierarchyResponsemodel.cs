using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetsbyLocationHierarchyResponsemodel
    {
        public Guid asset_id { get; set; }
        public string asset_name { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public int formiosection_id { get; set; }

        public Guid inspectiontemplate_asset_class_id { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string asset_class_type { get; set; }

        public bool is_top_level_dummy { get; set; }

        public virtual List<SubLevelComponentAssets> subLevel_components { get; set; }

    }
    //public class SubLevelComponentAssetsDetailsClass
    //{
    //    public Guid asset_sublevelcomponent_mapping_id { get; set; }
    //    public Guid asset_id { get; set; }
    //    public Guid sublevelcomponent_asset_id { get; set; }
    //    public int formiosection_id { get; set; }

    //    public string sublevelcomponent_asset_name { get; set; }
    //    public string circuit { get; set; }
    //    public string image_name { get; set; }
    //    public string image_name_url { get; set; }


    //}
}
