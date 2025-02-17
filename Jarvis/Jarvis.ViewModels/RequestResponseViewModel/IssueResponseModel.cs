using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class IssueResponseModel
    {
        public int totalissue { get; set; }
        public Guid issue_uuid { get; set; }
        public string work_order_uuid { get; set; }
        public Nullable<Guid> asset_id { get; set; }
        public Guid attribute_id { get; set; }
        public string asset_name { get; set; }
        public Nullable<Guid> inspection_id { get; set; }
        public string internal_asset_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string site_id { get; set; }
        public string site_name { get; set; }
        public string notes { get; set; }
        public long? issue_number { get; set; }
        public long? work_order_number { get; set; }
        public List<AssetsValueJsonObjectViewModel> attributes_value { get; set; }
        public int status { get; set; }
        public string status_name { get; set; }
        public List<CommentJsonObjectViewModel> comments { get; set; }
        public int priority { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string modified_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        //public virtual Asset Asset { get; set; }
        public string maintainence_staff_id { get; set; }
        public string timeelapsed { get; set; }
        public Nullable<DateTime> datetime_requested { get; set; }
        public AssetsResponseModel assets{ get; set; }
        public InspectionResponseModel inspections { get; set; }
        public string mr_id { get; set; }
        public string timezone { get; set; }
    }
}
