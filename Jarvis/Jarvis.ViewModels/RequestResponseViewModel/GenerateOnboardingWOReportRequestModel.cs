using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GenerateOnboardingWOReportRequestModel
    {
        public Guid workOrderId { get; set; }
        public Guid siteId { get; set; }
        public Guid companyId { get; set; }
        public string reportType { get; set; }
        public string userEmail { get; set; }
        public List<string> attachments { get; set; }
    }
    public class GenerateOnboardingWOReportRequestModel_2
    {
        public Guid wo_id { get; set; }
        public string report_type { get; set; }
        public bool is_requested_for_wo { get; set; } // only use for MT type reporting
        public List<string> attachments { get; set; }
    }
}
