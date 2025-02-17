using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ManagerMobileDashboardDataCountResponseModel
    {
        public int PendingReview { get; set; }
        public int CheckOutAssets { get; set; }
        public int OutStandingIssues { get; set; }
        public int OverDuePMCount { get; set; }
        public int UpcomingPMCount { get; set; }
        public int AssetsCount { get; set; }
        public int WorkOrdersCount { get; set; }
        public int WorkorderLinesCount { get; set; }
        public DateTime? last_sync_date { get; set; }
    }
}
