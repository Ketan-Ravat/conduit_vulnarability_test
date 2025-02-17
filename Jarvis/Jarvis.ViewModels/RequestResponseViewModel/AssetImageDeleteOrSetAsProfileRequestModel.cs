using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssetImageDeleteOrSetAsProfileRequestModel
    {
        public Guid asset_id {  get; set; }
        public Guid? asset_profile_images_id { get; set; }
        public Guid? assetirwoimageslabelmapping_id { get; set; }
        public Guid? asset_issue_image_mapping_id { get; set; }
        public string asset_image_url { get; set; }
        public bool is_deleted { get; set;} //if true then delete img else set as profile
    }
}
