using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetImageInfoByTextRactRequestmodel
    {
        public string bucket_name { get; set; }
        public string image_name { get; set;}
        public Guid? inspectiontemplate_asset_class_id { get; set; }
    }
}
