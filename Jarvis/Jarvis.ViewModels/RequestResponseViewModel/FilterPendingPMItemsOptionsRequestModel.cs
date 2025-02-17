using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel {
    public class FilterPendingPMItemsOptionsRequestModel {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public List<string> pm_id { get; set; }
        public List<string> internal_asset_id { get; set; }
        public List<string> pm_plan_id { get; set; }
        public List<string> site_id { get; set; }
        public string search_string { get; set; }
        public string option_search_string { get; set; }
        /// <summary>
        /// OverDue = 33,
        /// Open = 31,
        /// PMWaiting = 42,
        /// PMInProgress = 43
        /// </summary>
        public int pm_status { get; set; }
        /// <summary>
        /// All = 0
        /// Current/Upcoming = 1
        /// Completed = 2
        /// </summary>
        public int pm_filter_type { get; set; }
    }
}
