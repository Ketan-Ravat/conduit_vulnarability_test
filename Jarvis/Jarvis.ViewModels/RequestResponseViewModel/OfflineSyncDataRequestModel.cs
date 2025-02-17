using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class OfflineSyncDataRequestModel
    {
        public List<OfflineIssueRequestModel> issues { get; set; }
        public List<OfflineIssueRequestModel> work_orders  { get; set; }
        public List<OfflineInspectionRequestModel> inspections { get; set; }
    }
}
