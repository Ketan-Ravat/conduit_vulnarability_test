using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GenerateBulkNetaReportRequestModel
    {
        public List<string> asset_form_ids { get; set; }
        public int report_inspection_type { get; set; }
    }
}
