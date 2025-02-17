using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class PendingInspectionCheckoutAssetsManagerResponseModel
    {
        public List<PendingAndCheckoutInspViewModel> PendingInspection { get; set; }
        public List<PendingAndCheckoutInspViewModel> CheckOutAssets { get; set; }
        public int OutStandingIssuesCount { get; set; }
    }
}
