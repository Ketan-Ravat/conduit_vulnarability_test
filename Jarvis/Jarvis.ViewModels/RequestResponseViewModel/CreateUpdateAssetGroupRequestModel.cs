using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class CreateUpdateAssetGroupRequestModel
    {
        public Guid? asset_group_id { get; set; }
        public string asset_group_name { get; set; }
        public string asset_group_description { get; set; }
        public int criticality_index_type { get; set; }
        public bool is_deleted { get; set; }
        public List<AssetsObjects> assets_list { get; set; }
    }

    public class AssetsObjects
    {
        public Guid asset_id { get; set; }
        public bool is_deleted { get; set; }
    }
}
