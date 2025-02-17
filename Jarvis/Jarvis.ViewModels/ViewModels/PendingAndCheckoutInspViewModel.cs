using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class PendingAndCheckoutInspViewModel
    {
        public Guid inspection_id { get; set; }
        public Guid asset_id { get; set; }
        public Guid operator_id { get; set; }
        public string operator_name { get; set; }
        public string operator_firstname { get; set; }
        public string operator_lastname { get; set; }
        public int status { get; set; }
        public bool is_comment_important { get; set; }
        public string status_name { get; set; }
        public string operator_notes { get; set; }
        public string company_id { get; set; }
        public string company_name { get; set; }
        public string company_code { get; set; }
        public int company_status { get; set; }
        public string site_id { get; set; }
        public string site_name { get; set; }
        public string site_code { get; set; }
        public string location { get; set; }
        public int site_status { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string created_by { get; set; }
        public string modified_by { get; set; }
        public long meter_hours { get; set; }
        public int shift { get; set; }
        public string manager_notes { get; set; }
        public Nullable<DateTime> datetime_requested { get; set; }
        //public virtual InspectionSiteViewModel Sites { get; set; }
        public virtual AssetViewModel Asset { get; set; }
        //public List<CategoryWiseAttributesInsepction> attributes { get; set; }
        public List<InspectionAttributesJsonObjectViewModel> new_notok_attributes { get; set; }
        public string internal_asset_id { get; set; }
        public int asset_status { get; set; }
        public Guid inspectionform_id { get; set; }
        public string notes { get; set; }
        public Nullable<int> asset_request_status { get; set; }
        public string asset_requested_by { get; set; }
        public Nullable<DateTime> asset_requested_on { get; set; }
        public string asset_approved_by { get; set; }
        public Nullable<DateTime> asset_approved_on { get; set; }
        public Nullable<int> usage { get; set; }
        public string asset_name { get; set; }
        public string asset_photo { get; set; }
        public string asset_type { get; set; }
        public string product_name { get; set; }
        public string model_name { get; set; }
        public string asset_serial_number { get; set; }
        public string model_year { get; set; }
        public string current_stage { get; set; }
        public string timeelapsed { get; set; }
        public List<InspectionAttributesJsonObjectViewModel> attribute_values { get; set; }
        public ImagesListObjectViewModel image_list { get; set; }
        public int openIssuesCount { get; set; }
        public bool isManagerNotes { get; set; }
        public bool showHideApprove { get; set; }
    }
}
