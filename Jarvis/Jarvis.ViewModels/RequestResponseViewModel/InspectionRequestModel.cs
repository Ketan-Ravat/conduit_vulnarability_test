using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class InspectionRequestModel
    {
        /// <summary>
        /// Enter User ID
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// Enter Inspection ID
        /// </summary>
        public Guid inspection_id { get; set; }

        /// <summary>
        /// Enter Asset ID
        /// </summary>
        public Guid asset_id { get; set; }

        /// <summary>
        /// Enter Operator ID
        /// </summary>
        public string operator_id { get; set; }

        /// <summary>
        /// Enter Manager ID
        /// </summary>
        public string manager_id { get; set; }

        /// <summary>
        /// Enter Inspection Status
        /// </summary>
        public int status { get; set; }
        public bool is_comment_important { get; set; }

        /// <summary>
        /// Enter Operator Notes 
        /// </summary>
        public string operator_notes { get; set; }

        public AssetsValueJsonObjectViewModel[] attribute_values { get; set; }

        /// <summary>
        /// Enter Company ID
        /// </summary>
        public string company_id { get; set; }

        /// <summary>
        /// Enter Site ID
        /// </summary>
        public Guid site_id { get; set; }

        /// <summary>
        /// Enter Created Date Time
        /// </summary>
        public Nullable<DateTime> created_at { get; set; }

        /// <summary>
        /// Enter Modified Date Time
        /// </summary>
        public Nullable<DateTime> modified_at { get; set; }

        public string created_by { get; set; }

        //public string modified_by { get; set; }

        /// <summary>
        /// Enter Meter Hours
        /// </summary>
        public long meter_hours { get; set; }

        /// <summary>
        /// Enter Shift
        /// </summary>
        public int shift { get; set; }

        //public ImagesListObjectViewModel image_list { get; set; }
        /// <summary>
        /// Enter Images
        /// </summary>
        public ImagesListObjectViewModel image_list { get; set; }

        /// <summary>
        /// Enter Manager Notes
        /// </summary>
        public string manager_notes { get; set; }

        /// <summary>
        /// Enter Requested Date Time
        /// </summary>
        public DateTime datetime_requested { get; set; }
    }
}
