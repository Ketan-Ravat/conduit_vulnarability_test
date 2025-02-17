using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllWorkOrderRequestModel
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public long wo_number { get; set; }
        public string wo_id { get; set; }
        public int priority { get; set; }
        public int status { get; set; }
        public string search_string { get; set; }

        public int? wo_type { get; set; }
    }
}
