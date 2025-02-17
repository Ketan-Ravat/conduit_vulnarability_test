using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel {
    public class DashboardPMMetricsResponseModel {
        public int overdueCount { get; set; }
        public int inProgressCount { get; set; }
        public int waitingCount { get; set; }
        public int openCount { get; set; }
    }
}