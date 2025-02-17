using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetTopWorkordersListByStatusRequestModel
    {
        public int pagesize { get; set;}
        public int pageindex { get; set;}
        public int? status { get; set;}
        public int? wo_due_overdue_flag { get; set;}
        public bool is_requested_for_neta_inspection_forms { get; set;}
    }
}
