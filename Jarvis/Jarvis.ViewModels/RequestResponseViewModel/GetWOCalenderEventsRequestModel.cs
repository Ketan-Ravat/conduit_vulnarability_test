using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetWOCalenderEventsRequestModel
    {
        public Guid? technician_user_id { get; set; }
        public DateTime from_date { get; set; }
        public DateTime to_date { get; set; }
    }
}
