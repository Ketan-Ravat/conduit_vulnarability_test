using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class CreateWorkorderNewflowResponsemodel
    {
        public Guid wo_id { get; set; }
        public string wo_number { get; set; }
        public string manual_wo_number { get; set; }
        public int response_status { get; set; }
    }
}
