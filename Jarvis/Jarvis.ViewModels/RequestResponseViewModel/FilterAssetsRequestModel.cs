using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterAssetsRequestModel : FilterBaseRequestModel
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
        /// Enter Levels for filter
        /// </summary>
        public List<string>? Levels { get; set; }

        /// <summary>
        /// for BE use only
        /// </summary>
        public string form_io_form_id { get; set; }
        public List<int>? criticality_index_type { get; set; }
        public List<int>? condition_index_type { get; set; }
        public List<Guid>? inspectiontemplate_asset_class_id { get; set; }
        public List<int>? formiobuilding_id { get; set; }
        public List<int>? formiofloor_id { get; set; }
        public List<int>? formioroom_id { get; set; }
        public List<int>? formiosection_id { get; set; }
        public List<int>? thermal_classification_id { get; set; }
        public List<int>? asset_operating_condition_state { get; set; }
        public List<int>? code_compliance { get; set; }
    }
}
