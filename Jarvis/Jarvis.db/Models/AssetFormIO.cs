using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetFormIO
    {
        [Key]
        public Guid asset_form_id { get; set; }

        [ForeignKey("Asset")]
        public Guid? asset_id { get; set; } // when Asset will create in AT WO then we will store its asset id in this coloumn

        [ForeignKey("Sites")]
        public Guid? site_id { get; set; }

        public Nullable<Guid> form_id { get; set; }

        public string asset_form_name { get; set; }
        public string asset_form_type { get; set; }

        public string asset_form_description { get; set; }

        public string asset_form_data { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }

        public Nullable<Guid> requested_by { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }
        public string accepted_by { get; set; }

        [ForeignKey("StatusMaster")]
        public int status { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid? wo_id { get; set; }

        [ForeignKey("WOcategorytoTaskMapping")]
        public Guid? WOcategorytoTaskMapping_id { get; set; }

        [ForeignKey("PDFReportStatusMaster")]
        public int? pdf_report_status { get; set; }
        public string pdf_report_url { get; set; }
        public string report_lambda_logs { get; set; }
        public string form_retrived_asset_name { get; set; }
        public string form_retrived_asset_id { get; set; }
        public string form_retrived_location { get; set; }
        public string form_retrived_data { get; set; }
        public DateTime? intial_form_filled_date { get; set; } // first time form field date 
        public string form_retrived_nameplate_info { get; set; }
        public string form_retrived_workOrderType { get; set; }
        public DateTime? inspected_at { get; set; }
        public DateTime? accepted_at { get; set; }
        public DateTime? export_pdf_at { get; set; }
        public int? inspection_verdict { get; set; }
        public bool? defects { get; set; }
        public bool is_main_asset_created { get; set; }
        public virtual Asset Asset { get; set; }
        public virtual Sites Sites { get; set; }
        public virtual StatusMaster StatusMaster { get; set; }
        public virtual WorkOrders WorkOrders { get; set; }
        public virtual StatusMaster PDFReportStatusMaster { get; set; }
        public virtual WOcategorytoTaskMapping WOcategorytoTaskMapping { get; set; }
        public virtual FormIOInsulationResistanceTestMapping FormIOInsulationResistanceTestMapping { get; set; }
        public virtual ICollection<WOLineIssue> WOLineIssue { get; set; }
        public virtual ICollection<AssetIssue> AssetIssue { get; set; }
        public virtual ICollection<AssetPMs> AssetPMs { get; set; }
    }
}
