using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllTechniciansLocationByWOIdResponseModel
    {
        public List<UserLocation_Data_Model> list { get; set; }
    }
    public class UserLocation_Data_Model
    {
        public Guid user_id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string address { get; set; }
        public bool is_location_active { get; set; }
    }
}
