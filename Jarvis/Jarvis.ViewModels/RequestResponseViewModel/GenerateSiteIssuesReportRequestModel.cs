using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GenerateSiteIssuesReportRequestModel
    {
        public Guid siteId { get; set; }
        public Guid companyId { get; set; }
        public string reportType { get; set; }
        public string userEmail { get; set; }
    }
}
