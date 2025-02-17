using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterWorkOrderOptionsRequestModel 
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        /// <summary>
        /// Enter Work Order Number
        /// </summary>
        public int wo_number { get; set; }
        /// <summary>
        /// Enter Work Order title
        /// </summary>
        public string wo_id { get; set; }
        /// <summary>
        /// Enter priority 
        /// Low = 45,
        /// Medium = 46,
        /// High = 47
        /// </summary>
        public int priority { get; set; }
        /// <summary>
        /// Enter Status of Inspection 
        /// WOOpen = 54,
        /// WOInProgress = 55,
        /// WOCompleted = 56,
        /// WOCancelled = 57,
        /// WOReOpen = 58
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// Enter Search String
        /// </summary>
        public string search_string { get; set; }
        /// <summary>
        /// Enter Search String
        /// </summary>
        public string option_search_string { get; set; }
        /// <summary>
        /// Enter work order type
        /// </summary>
        public int? wo_type { get; set; }
    }
}
