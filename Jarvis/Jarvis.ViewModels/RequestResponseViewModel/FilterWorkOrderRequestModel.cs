using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterIssueRequestModel : FilterBaseRequestModel
    {
        /// <summary>
        /// Enter Status of Issue 
        /// New = 12,
        /// InProgress = 13,
        /// Waiting = 14,
        /// Completed = 15,
        /// </summary>
        public List<int> status { get; set; }
        /// <summary>
        /// Enter Asset ID
        /// </summary>
        public List<string> asset_id { get; set; }
        /// <summary>
        /// search string
        /// </summary>
        public string search_string { get; set; }
        /// <summary>
        /// Enter Priority
        /// Very_High = 1,
        /// High = 2,
        /// Medium = 3,
        /// Low = 4
        /// </summary>
        public List<int> priority { get; set; }
        /// <summary>
        /// Enter Issue Title
        /// </summary>
        public List<string> issue_title { get; set; }
        /// <summary>
        /// Enter Timezone
        /// </summary>
        public string timezone { get; set; }
        /// <summary>
        /// search option string
        /// </summary>
        public string option_search_string { get; set; }
    }
}
