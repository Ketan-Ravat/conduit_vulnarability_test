using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MobileOBWOAssetImagesResponsemodel
    {
        public Guid? woonboardingassetsimagesmapping_id { get; set; }
        public string asset_photo { get; set; }
        public string asset_thumbnail_photo { get; set; }
        public bool is_deleted { get; set; }
        public int asset_photo_type { get; set; }// 1 for profile and 2 for nameplate images
        public int? image_duration_type_id { get; set; }  // 1 - before , 2 - after
        public Guid woonboardingassets_id { get; set; }
    }
}
