using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateAssetInfoRequestmodel
    {
        public string form_retrived_nameplate_info { get; set; }
        public int? criticality_index { get; set; }
        public double? condition_index { get; set; }
        public int? criticality_index_type { get; set; }
        public int? condition_index_type { get; set; }
        public Guid asset_id { get; set; }
        public Guid? formio_id { get; set; }
        public List<AssetsProfileImageListRequest> asset_namespate_images { get; set; }

    }
}
