using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllEquipmentListRequestmodel
    {
        public Guid site_id { get; set; }
        public int page_size { get; set; }
        public int page_index { get; set; }
        public string search_string { get; set; }
        public List<string> equipment_number { get; set; }
        public List<string> manufacturer { get; set; }
        public List<string> model_number { get; set; }
        public List<int?> calibration_status { get; set; }
    }
}
