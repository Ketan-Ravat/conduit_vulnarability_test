using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterInspectionOptionsRequestModel : FilterBaseRequestModel
    {
        /// <summary>
        /// Enter Status of Inspection 
        /// 8	Pending
        /// 9	Cancelled
        /// 16	Rejected
        /// 11	InMaintenance
        /// 10	Accepted
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// Enter Asset ID
        /// </summary>
        public List<string> asset_id { get; set; }
        /// <summary>
        /// Enter Internal Asset ID
        /// </summary>
        public List<string> internal_asset_id { get; set; }
        /// <summary>
        /// Enter Shift Number from 1,2,3
        /// </summary>
        public List<int> shift_number { get; set; }
        /// <summary>
        /// Enter Operators
        /// </summary>
        public List<string> requestor_id { get; set; }
        /// <summary>
        /// Enter 0 To get all Records 1 to get only new not ok record
        /// </summary>
        public int new_not_ok_attribute { get; set; }
        /// <summary>
        /// Enter Search String
        /// </summary>
        public string search_string { get; set; }
        /// <summary>
        /// Enter Search String
        /// </summary>
        public string option_search_string { get; set; }
        /// <summary>
        /// Enter Timezone
        /// </summary>
        public string timezone { get; set; }

        /// <summary>
        /// Enter Manager
        /// </summary>
        public List<string> manager_id { get; set; }
    }
}
