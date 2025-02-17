using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class CreateQuoteFromEstimatorRequestmodel
    {
        public string quote_number { get; set; }
        public string quote_description { get; set; }
        public DateTime? start_date { get; set; }
        public int quote_type { get; set; } //Onboarding_WO - 76 , IR_Scan_WO - 78 , Maintenance_WO - 67
        public Guid site_id { get; set; }
        public List<quote_line_items> quote_Line_Items { get; set; }
    }
    public class quote_line_items
    {
        public Guid asset_id { get; set; }
        public Guid? asset_pm_id { get; set; }
    }
}
