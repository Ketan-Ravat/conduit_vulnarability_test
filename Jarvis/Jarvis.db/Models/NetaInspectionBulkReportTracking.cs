using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class NetaInspectionBulkReportTracking
    {
        [Key]
        public Guid netainspectionbulkreporttracking_id { get; set; }
        public string asset_form_ids { get; set; }
        public int report_status { get; set; }
        public string report_id_number { get; set; }
        public int? report_inspection_type { get; set; }
        public string report_url { get; set; }
        public string report_lambda_logs { get; set; }
        public bool is_deleted { get; set; }
        public Guid? created_by { get; set; }
        public Nullable<DateTime> reprt_completed_date { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public Guid? modified_by { get; set; }

        [ForeignKey("Sites")]
        public Guid? site_id { get; set; }
        public virtual Sites Sites { get; set; }

    }
}
