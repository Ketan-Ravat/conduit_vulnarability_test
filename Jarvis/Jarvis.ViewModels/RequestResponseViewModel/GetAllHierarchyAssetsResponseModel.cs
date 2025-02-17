using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllHierarchyAssetsResponseModel
    {
        public Guid asset_id { get; set; }
        public string internal_asset_id { get; set; }
        public string name { get; set; }
        public string QR_code { get; set; }
        //    public string site_location { get; set; }
        public string parent_internal_asset_id { get; set; }
        public Guid site_id { get; set; }
        public string site_name { get; set; }
        public string site_code { get; set; }
        //    public Guid company_id { get; set; }
        //    public string company_name { get; set; }
        public double? condition_index { get; set; }
        public string children { get; set; }
        public string levels { get; set; }
        public bool is_child_available { get; set; }
        public int? condition_index_type { get; set; }
        public string condition_index_type_name { get; set; }
    }
}
