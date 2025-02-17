using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterAttributesEquipmentResponseModel
    {
        public List<string> equipment_number { get; set; }
        public List<string> manufacturer { get; set; }
        public List<string> model_number { get; set; }
        public List<int?> calibration_status { get; set; }
    }
}
