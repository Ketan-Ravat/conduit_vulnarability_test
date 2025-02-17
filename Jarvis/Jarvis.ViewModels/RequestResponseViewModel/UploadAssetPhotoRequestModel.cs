using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class UploadAssetPhotoRequestModel
    {
        public string asset_id { get; set; }

        public string asset_photo { get; set; }
        public string thumbnail_photo { get; set; }
        public int asset_photo_type { get; set; }

    }
}
