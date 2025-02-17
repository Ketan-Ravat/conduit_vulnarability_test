using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class ManagerPendingInspectionResponseModel
    {
        public Guid user_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string callbackUrl { get; set; }

        public List<InspectionDetails> inspections { get; set; }
    }
    public class InspectionDetails {
        public string site_name { get; set; }
        public string internal_asset_id { get; set; }
        public string asset_name { get; set; }
        public long meter_hours_at_inspection { get; set; }
        public string time_elapsed { get; set; }
        public string operator_name { get; set; }
    }

    public class PendingInspectionsSummary {
        public string site_name { get; set; }
        public int pending_reviews { get; set; }
        public string time_elapsed { get; set; }
    }
}