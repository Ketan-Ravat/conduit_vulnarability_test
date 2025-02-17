using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllImagesForAssetResponseModel
    {
        public List<AssetImageDetails> asset_image_list { get; set; }        
    }

    public class AssetImageDetails
    {
        public Guid? asset_profile_images_id { get; set; }
        public Guid? assetirwoimageslabelmapping_id { get; set; }
        public Guid? asset_issue_image_mapping_id { get; set; }

        public string asset_image_name { get; set; }
        public DateTime created_at { get; set; }
        public int asset_photo_type { get; set; }
        public string asset_image_url { get; set; }
        public bool is_deleted { get; set; }
    }
}
