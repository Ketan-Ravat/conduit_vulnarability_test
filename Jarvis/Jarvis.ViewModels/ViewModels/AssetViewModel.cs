using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class AssetViewModel
    {
        public string asset_id { get; set; }

        public string asset_photo { get; set; }

        public string internal_asset_id { get; set; }

        public string company_id { get; set; }

        public Guid site_id { get; set; }
        public string site_code { get; set; }
        public string site_logo_img { get; set; }

        public Nullable<int> status { get; set; }

        public string status_name { get; set; }

        public Guid inspectionform_id { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string notes { get; set; }

        public Nullable<int> asset_request_status { get; set; }

        public string asset_requested_by { get; set; }

        public Nullable<DateTime> asset_requested_on { get; set; }

        public string asset_approved_by { get; set; }

        public Nullable<DateTime> asset_approved_on { get; set; }

        //public AssetsValueJsonObject[] lastinspection_attribute_values { get; set; }
        public List<AssetsValueJsonObjectViewModel> lastinspection_attribute_values { get; set; }

        public Nullable<int> usage { get; set; }

        public Nullable<long> meter_hours { get; set; }

        public string name { get; set; }

        public string asset_type { get; set; }

        public string product_name { get; set; }

        public string model_name { get; set; }

        public string asset_serial_number { get; set; }

        public string model_year { get; set; }

        public string site_location { get; set; }

        public string current_stage { get; set; }

        public string parent { get; set; }

        public string children { get; set; }

        //public virtual Sites Sites { get; set; }

        //public virtual InspectionForms InspectionForms { get; set; }
        public virtual ICollection<AssetTransactionHistoryViewModel> AssetTransactionHistory { get; set; }
        //public virtual ICollection<Inspection> Inspection { get; set; }
        public List<IssueViewModel> Issues { get; set; }
        public int pmPlanCount { get; set; }
    }
}
