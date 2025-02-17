using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterAssetsOptionsRequestModel : FilterBaseRequestModel
    {
        /// <summary>
        /// Enter Status of Asset 
        /// 1	Active
        /// 2	Inactive
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
        /// Enter Model Name
        /// </summary>
        public List<string> model_name { get; set; }
        /// <summary>
        /// Enter Model year
        /// </summary>
        public List<string> model_year { get; set; }
        /// <summary>
        /// Enter 0 To get all else 1 to show only open issues asset
        /// </summary>
        public int show_open_issues { get; set; }
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
