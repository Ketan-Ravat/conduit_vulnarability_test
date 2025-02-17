using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllOnboardingAssetsByWOIdResponseModel
    {
        public Guid woonboardingassets_id { get; set; }
        public Guid asset_id { get; set; }
        public string asset_name { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public Guid site_id { get; set; }
        public string site_name { get; set; }
        public int status { get; set; }
        public int? component_level_type_id { get; set; } // 1 for top_level , 2- subcomponant
        public Guid? toplevelcomponent_asset_id { get; set; }
        public string status_name { get; set; }
        public string QR_code { get; set; }
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public string asset_class_type { get; set; }
        public string form_nameplate_info { get; set; }
        public int? arc_flash_label_valid { get; set; }
        public int? maintenance_index_type { get; set; }
        public int temp_issues_count { get; set; }
        public string asset_profile { get; set; }
        public List<string> issues_title_list { get; set; }
    }
}
