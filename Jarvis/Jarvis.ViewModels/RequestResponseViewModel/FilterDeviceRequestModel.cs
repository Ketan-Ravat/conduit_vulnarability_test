using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterDeviceRequestModel : FilterBaseRequestModel
    {
        /// <summary>
        /// Enter Status of Device 
        /// 1	Approved
        /// 2	Not Approved
        /// 0   All
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// Enter BRAND
        /// </summary>
        public List<string> brand { get; set; }
        /// <summary>
        /// Enter Model
        /// </summary>
        public List<string> model { get; set; }
        /// <summary>
        /// Enter OS
        /// </summary>
        public List<string> os { get; set; }
        /// <summary>
        /// Enter Search String
        /// </summary>
        public string search_string { get; set; }
        /// <summary>
        /// Enter Search String
        /// </summary>
        public string option_search_string { get; set; }
        /// <summary>
        /// Enter BRAND
        /// </summary>
        public List<string> type { get; set; }
    }
}
