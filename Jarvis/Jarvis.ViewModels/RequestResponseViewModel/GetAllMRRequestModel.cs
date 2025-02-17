using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllMRRequestModel
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
        public string requested_by { get; set; }
        public string search_string { get; set; }

        /// <summary>
        /// All = 0
        /// Open = 48
        /// Cancelled = 49
        /// Completed = 50
        /// Work Order Created = 51
        /// </summary>
        public int mr_filter_type { get; set; }
    }

    public class GetOpenMRCount
    {
        public int openMRCount { get; set; }
    }
}
