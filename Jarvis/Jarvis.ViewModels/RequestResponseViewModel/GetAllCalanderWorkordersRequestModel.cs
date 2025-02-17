using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllCalanderWorkordersRequestModel
    {
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public List<int>? status { get; set; }
        public List<Guid>? technician_ids { get; set; }
        public List<Guid>? lead_ids { get; set; }
        public List<Guid>? vendor_ids { get; set; }
        public List<Guid>? site_id { get; set; }
    }
}
