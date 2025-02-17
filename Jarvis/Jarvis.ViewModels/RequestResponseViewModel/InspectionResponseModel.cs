using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class InspectionResponseModel
    {
        public Guid inspection_id { get; set; }
        public Guid asset_id { get; set; }
        public string internal_asset_id { get; set; }
        public string asset_name { get; set; }
        public string time_elapsed { get; set; }
        public Guid operator_id { get; set; }
        public string operator_name { get; set; }
        public string operator_firstname { get; set; }
        public string operator_lastname { get; set; }
        public string manager_id { get; set; }
        public string manager_name { get; set; }
        public int status { get; set; }
        public bool is_comment_important { get; set; }
        public string status_name { get; set; }
        public string operator_notes { get; set; }
        //public AssetsValueJsonObjectViewModel[] attribute_values { get; set; }
        //public string company_id { get; set; }
        //public Guid site_id { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string created_by { get; set; }
        public string modified_by { get; set; }
        public long meter_hours { get; set; }
        public int shift { get; set; }
        public string site_name { get; set; }
        public ImagesListObjectViewModel image_list { get; set; }
        public string manager_notes { get; set; }
        public Nullable<DateTime> datetime_requested { get; set; }
        public virtual InspectionSiteViewModel Sites { get; set; }
        public virtual AssetViewModel Asset { get; set; }
        public virtual List<IssueViewModel> Issues { get; set; }
        public List<CategoryWiseAttributesInsepction> attributes { get; set; }
        public List<CategoryWiseAttributesInsepction> new_notok_attributes_by_category { get; set; }
        public List<InspectionAttributesJsonObjectViewModel> new_notok_attributes { get; set; }

        public List<InspectionAttributesJsonObjectViewModel> attribute_values { get; set; }

        public Nullable<DateTime> approval_date { get; set; }
        //public string timezone { get; set; }
    }

        public class CategoryWiseAttributesInsepction
    {
        public int category_id { get; set; }
        public string name { get; set; }
        public string category_spanish_name { get; set; }
        public List<InspectionAttributesJsonObjectViewModel> attribute_values { get; set; }
    }

    public class InspectionAttributesJsonObjectViewModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string value_spanish_name { get; set; }
        public string value { get; set; }
        public int values_type { get; set; }
        public string attribute_spanish_name { get; set; }
        public int category_id { get; set; }
    }
}
