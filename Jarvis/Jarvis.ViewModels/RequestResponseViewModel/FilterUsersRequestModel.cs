using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterUsersRequestModel : FilterBaseRequestModel
    {
        /// <summary>
        /// Enter Status of Asset 
        /// 0   All
        /// 1	Active
        /// 2	Inactive
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// Enter Role ID
        /// </summary>
        public List<string> role_id { get; set; }
        /// <summary>
        /// Enter Search String
        /// </summary>
        public string search_string { get; set; }
        /// <summary>
        /// Enter Option Search String
        /// </summary>
        public string option_search_string { get; set; }
    }
}
