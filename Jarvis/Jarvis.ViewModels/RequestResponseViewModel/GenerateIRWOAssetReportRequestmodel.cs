using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GenerateIRWOAssetReportRequestmodel
    {
        public string user_id { get; set; }
        public Guid wo_id { get; set; }

        //public bool is_requested_to_regenerate_report { get; set; }
    }
}
