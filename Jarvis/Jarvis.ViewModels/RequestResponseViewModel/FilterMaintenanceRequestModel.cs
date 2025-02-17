using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterMaintenanceRequestModel 
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }

        /// <summary>
        /// Enter Status of Issue 
        /// MROpen = 48,
        /// MRCancelled = 49,
        /// MRCompeleted = 50,
        /// MRWorkOrderCreated = 51,
        /// </summary>
        //public int status { get; set; }
        public List<string> mr_id { get; set; }

        /// <summary>
        /// Enter Maintenance Type
        /// Manual = 52,
        /// Inspection = 53,
        /// </summary>
        public int type { get; set; }
     
        /// /// <summary>
        /// Enter Requested By
        /// </summary>
        public string requested_by { get; set; }

        /// <summary>
        /// search string
        /// </summary>
        public string search_string { get; set; }
        
        /// <summary>
        /// search option string
        /// </summary>
        public string option_search_string { get; set; }
        /// <summary>
        /// All = 0
        /// Open = 48
        /// Cancelled = 49
        /// Completed = 50
        /// Work Order Created = 51
        /// </summary>
        public int mr_filter_type { get; set; }
    }
}
