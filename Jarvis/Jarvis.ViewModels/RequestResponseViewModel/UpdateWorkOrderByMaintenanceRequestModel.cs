using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class UpdateIssueByMaintenanceRequestModel : GenericRequestModel
    {
        /// <summary>
        /// Enter Login UserID
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// Work Order UUID
        /// </summary>
        public Guid issue_uuid { get; set; }

        /// <summary>
        /// Work Order UUID for Old APP
        /// </summary>
        public Guid work_order_uuid { get; set; }
        /// <summary>
        /// Enter Status
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// Enter Updated DateTime
        /// </summary>
        public string updated_at { get; set; }

        /// <summary>
        /// Enter Sync Or Not
        /// </summary>
        public bool is_sync { get; set; }

        public List<CommentJsonObjectViewModel> comment { get; set; }
    }
}
