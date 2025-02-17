using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MobileInspectionResponseModel
    {
        public Guid inspection_id { get; set; }
        public Guid asset_id { get; set; }
        public string internal_asset_id { get; set; }
        public string asset_name { get; set; }
        public List<InspectionAttributesJsonObjectViewModel> new_notok_attributes { get; set; }
        public Nullable<DateTime> datetime_requested { get; set; }
        public string operator_firstname { get; set; }
        public string operator_lastname { get; set; }
        public List<InspectionAttributesJsonObjectViewModel> attribute_values { get; set; }
        public virtual List<IssueViewModel> Issues { get; set; }
        public string timezone { get; set; }
    }
}
