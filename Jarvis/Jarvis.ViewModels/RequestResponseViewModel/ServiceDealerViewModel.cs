using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel {
    public class ServiceDealerViewModel {
        public Guid service_dealer_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public int status { get; set; }
        public string status_name { get; set; }
    }
}
