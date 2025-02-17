using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UploadAssetImageResponsemodel
    {
        public string original_imege_file { get; set; }

        public string thumbnail_image_file { get; set; }
        public string user_uploaded_filename { get; set; }
    }
}
