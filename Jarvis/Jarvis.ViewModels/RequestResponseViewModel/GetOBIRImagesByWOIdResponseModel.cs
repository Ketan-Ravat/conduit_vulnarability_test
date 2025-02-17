using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetOBIRImagesByWOIdResponseModel
    {
        public string asset_name { get; set; }
        public string asset_class_name { get; set; }
        public string asset_class_code { get; set; }
        public List<View_OBIRImage_label> ob_ir_Image_label_list { get; set; }
    }    
}
