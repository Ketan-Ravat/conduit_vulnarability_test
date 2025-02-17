using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel {
    public class VendorPMEmailNotification {
        public string company_name { get; set; }
        public string vendor_name { get; set; }
        public Guid service_dealer_id { get; set; }
        public string vendor_email { get; set; }
        public int total_upcoming_pms { get; set; }
        public int overdue_pms { get; set; }
        public List<VendorEmailExcelDetails> vendorExcelDetails { get; set; }
        public VendorPMEmailNotification()
        {
            vendorExcelDetails = new List<VendorEmailExcelDetails>();
        }
    }
    public class VendorEmailExcelDetails {
        public string company_name { get; set; }
        public string notification_type { get; set; }
        public int notification_type_id { get; set; }
        public Guid trigger_id { get; set; }
        public string internal_asset_id { get; set; }
        public string asset_name { get; set; }
        public string asset_location { get; set; }
        public string pm_title { get; set; }
        public long? current_meter { get; set; }
        public int? due_at { get; set; }
        public string due_in { get; set; }
    }

    public class ToSendVendorEmailExcelDetails {
        public Guid company_id { get; set; }
        public string company_name { get; set; }
        public int notification_type_id { get; set; }
        public string notification_type { get; set; }
        public Guid asset_id { get; set; }
        public string internal_asset_id { get; set; }
        public string asset_name { get; set; }
        public string asset_location { get; set; }
        public Guid trigger_id { get; set; }
        public string pm_title { get; set; }
        public string vendor_name { get; set; }
        public string vendor_email { get; set; }
        public Guid service_dealer_id { get; set; }
        public long? current_meter { get; set; }
        public long? due_at { get; set; }
        public string due_in { get; set; }
        public int status { get; set; }
    }
}
