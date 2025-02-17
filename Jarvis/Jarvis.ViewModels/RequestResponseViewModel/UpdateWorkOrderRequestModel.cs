using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class UpdateIssueRequestModel
    {
        /// <summary>
        /// Enter Work Order UUID
        /// </summary>
        public Guid issue_uuid { get; set; }

        /// <summary>
        /// Enter Work Order Title
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Enter Work Order Notes
        /// </summary>
        public string notes { get; set; }

        /// <summary>
        /// Enter Work Order Priority
        /// </summary>
        public int priority { get; set; }
        
        /// <summary>
        /// Enter Work Order Status
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// Enter Login User UserID
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// Enter Updated By UserID
        /// </summary>
        public string updated_at { get; set; }

        public List<CommentJsonObjectViewModel> comment { get; set; }
    }
}
