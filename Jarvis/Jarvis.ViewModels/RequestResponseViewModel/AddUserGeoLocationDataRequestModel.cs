using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddUserGeoLocationDataRequestModel
    {
        public Guid user_id { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public Guid device_id { get; set; }
        public bool is_location_active { get; set; }
    }
}
