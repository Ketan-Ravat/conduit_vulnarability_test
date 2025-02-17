using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateIRVisualImageLabelRequestModel
    {
        public Guid irwoimagelabelmapping_id { get; set; }
        public string ir_image_label { get; set; }
        public string visual_image_label { get; set; }
    }
}
