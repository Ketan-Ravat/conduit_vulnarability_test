using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssetFormIOResponseModel
    {
        public int response_status { get; set; }

        public Guid asset_form_id { get; set; }

        public Guid asset_id { get; set; }

        public Nullable<Guid> form_id { get; set; }

        public string asset_form_name { get; set; }

        public string asset_form_type { get; set; }

        public string asset_form_description { get; set; }

        // Removed as per Anmol comments
        //public string asset_form_data { get; set; }

        public string requested_by { get; set; }

        public int status { get; set; }
        public string status_name { get; set; }

        public string timezone { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }
        public string asset_name { get; set; }
        public string modified_by { get; set; }
        public string accepted_by { get; set; }
        public DateTime? inspected_at { get; set; }
        public DateTime? accepted_at { get; set; }
        public int pdf_report_status_id { get; set; }
        public string pdf_report_status_name { get; set; }
        public string pdf_report_url { get; set; }
        public string wo_number { get; set; }
        public int wo_type { get; set; }
        public string manual_wo_number { get; set; }
        public Guid WOcategorytoTaskMapping_id { get; set; }
        public int? inspection_verdict { get; set; }
        public string inspection_verdict_name { get; set; }
        public string form_retrived_workOrderType { get; set; }
        public string form_retrived_location { get; set; }
        public string form_retrived_asset_id { get; set; }
        public string work_procedure { get; set; }
        public string form_category_name { get; set; }
        public DateTime? intial_form_filled_date { get; set; } // first time form field date 

        //We only workOrderStatus from WorkOrder object : Anmol
        //public WorkOrderResponseModel WorkOrder { get; set; }

        public Nullable<Guid> wo_Id { get; set; }
        public int workOrderStatus { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public string asset_class_name { get; set; }
        public string asset_class_code { get; set; }
    }
}
