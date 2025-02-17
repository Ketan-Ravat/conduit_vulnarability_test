using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class IssueDetailRequestModel
    {
        public string userid { get; set; }

        public Guid issue_uuid { get; set; }

        public Guid asset_id { get; set; }

        public Guid? inspection_id { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string notes { get; set; }

        public AssetsValueJsonObjectViewModel[] attributes_value { get; set; }

        public int status { get; set; }

        public CommentJsonObjectViewModel[] comments { get; set; }

        public int priority { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string modified_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string site_id { get; set; }

        public string company_id { get; set; }

        public bool isapprove { get; set; }

        public string internal_asset_id { get; set; }

        public bool attachtoinspection { get; set; }

        //public virtual Asset Asset { get; set; }

        //public virtual StatusMaster StatusMaster { get; set; }

        //public string maintainence_staff_id { get; set; }

        //public IEnumerable<WorkOrderStatus> WorkOrderStatus { get; set; }
    }
}
