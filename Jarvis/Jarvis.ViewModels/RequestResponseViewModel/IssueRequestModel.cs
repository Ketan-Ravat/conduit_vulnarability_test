using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Jarvis.ViewModels
{
    public class IssueRequestModel
    {
        [JsonIgnore]
        public Guid uuid { get; set; }

        /// <summary>
        /// Enter Login User UserID
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// Enter Work Order UUID
        /// </summary>
        public Guid issue_uuid { get; set; }

        /// <summary>
        /// Enter Asset ID
        /// </summary>
        public Guid asset_id { get; set; }

        /// <summary>
        /// Enter Inspection ID
        /// </summary>
        public Guid? inspection_id { get; set; }

        /// <summary>
        /// Enter Work Order Name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Enter Work Order Description
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Enter Work Order Notes
        /// </summary>
        public string notes { get; set; }

        public AssetsValueJsonObjectViewModel[] attributes_value { get; set; }

        /// <summary>
        /// Enter Work Order Status
        /// </summary>
        public int status { get; set; }

        public CommentJsonObjectViewModel[] comments { get; set; }

        /// <summary>
        /// Enter Work Order Priority
        /// </summary>
        public int priority { get; set; }

        [JsonIgnore]
        public string created_by { get; set; }

        [JsonIgnore]
        public Nullable<DateTime> created_at { get; set; }

        [JsonIgnore]
        public string modified_by { get; set; }

        [JsonIgnore]
        public Nullable<DateTime> modified_at { get; set; }

        /// <summary>
        /// Enter Site ID
        /// </summary>
        public string site_id { get; set; }

        /// <summary>
        /// Enter Company ID
        /// </summary>
        public string company_id { get; set; }

        /// <summary>
        /// Is Approved
        /// </summary>
        public bool isapprove { get; set; }

        /// <summary>
        /// Enter Internal Asset ID
        /// </summary>
        public string internal_asset_id { get; set; }

        /// <summary>
        /// Attach With Inspection 
        /// </summary>
        public bool attachtoinspection { get; set; }

        //public virtual Asset Asset { get; set; }

        //public virtual StatusMaster StatusMaster { get; set; }

        //public string maintainence_staff_id { get; set; }

        //public IEnumerable<WorkOrderStatus> WorkOrderStatus { get; set; }

        public Guid mr_id { get; set; }
    }
}
