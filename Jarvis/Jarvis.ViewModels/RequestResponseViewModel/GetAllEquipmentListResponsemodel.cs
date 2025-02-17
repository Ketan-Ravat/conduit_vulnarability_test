using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public  class GetAllEquipmentListResponsemodel
    {
        public Guid equipment_id { get; set; }

        public string equipment_number { get; set; }

        public Guid site_id { get; set; }

        public string equipment_name { get; set; }

        public string manufacturer { get; set; }

        public string model_number { get; set; }
        public string serial_number { get; set; }

        public int calibration_interval { get; set; }

        public DateTime calibration_date { get; set; }

        public int? calibration_status { get; set; }
    }
}
