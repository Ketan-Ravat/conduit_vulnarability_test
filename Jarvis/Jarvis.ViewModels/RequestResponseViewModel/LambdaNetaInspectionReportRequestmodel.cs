using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class LambdaNetaInspectionReportRequestmodel
    {
        public string tracking_id { get; set; }
        public List<string> asset_list { get; set; }
    }
}
