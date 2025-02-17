using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class UpdateMeterHoursRequestModel
    {
        public string asset_id { get; set; }
        public int meter_hours { get; set; }
        //public string requested_by { get; set; }
    }
}
