using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ExecutiveWeeklyReportRequestModel
    {
        public Guid user_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string callbackUrl { get; set; }

        public List<IssueResponseModel> issues { get; set; }
        public List<AssetWeeklyReport> assets { get; set; }
        public List<PendingInspectionsSummary> pendingInspectionsSummary { get; set; }
        public List<InspectionDetails> inspections { get; set; }
        public ExecutiveWeeklyReportRequestModel()
        {
            pendingInspectionsSummary = new List<PendingInspectionsSummary>();
        }
    }
}
