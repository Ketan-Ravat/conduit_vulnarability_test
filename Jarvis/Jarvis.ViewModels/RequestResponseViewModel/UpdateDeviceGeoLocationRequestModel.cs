using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateDeviceGeoLocationRequestModel
    {
        public bool is_location_enabled { get; set; }
        public Guid device_uuid { get; set; }
    }
}
