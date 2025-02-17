using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class WorkOrderDetailsByIdForExportPDFResponsemodel
    {
        public Guid wo_id { get; set; }
        public string wo_number { get; set; }
        public string manual_wo_number { get; set; }
        public bool isCalibrationDateEnabled { get; set; }

        public int wo_type { get; set; }
        public string wo_type_name { get; set; }
        public string description { get; set; }
        public Guid site_id { get; set; }
        public string site_name { get; set; }
        public DateTime start_date { get; set; }
        public int wo_status_id { get; set; }
        public string wo_status { get; set; }
        // public string description { get; set; }
        // public string technician_name { get; set; }
        // public Guid? technician_id { get; set; }
        public Guid? client_company_id { get; set; }
        public string client_company_name { get; set; }
        public List<form_categoty_list_export> form_category_list { get; set; }
        public List<FormIOMasterForms> master_forms { get; set; }
    }
    public class form_categoty_list_export
    {
        public Guid wo_inspectionsTemplateFormIOAssignment_id { get; set; }
        public string form_category_name { get; set; }
        public Guid form_id { get; set; }
        public string form_description { get; set; }
        public string WP { get; set; }
        public string form_data { get; set; }
        public string parent_asset_name { get; set; }
        public Guid? parent_asset_id { get; set; }
        public int progress_total { get; set; }
        public int progress_completed { get; set; }
        // public string technician_name { get; set; }
        //  public Guid? technician_id { get; set; }
        public int status_id { get; set; }
        public string status_name { get; set; }
        public string form_name { get; set; }
        public List<GetWOcategoryTaskByCategoryIDListResponsemodelExport> task_list { get; set; }
    }

    public class FormIOMasterForms
    {
        public Guid form_id { get; set; }
        public string  form_data { get; set; }
    }
    public class GetWOcategoryTaskByCategoryIDListResponsemodelExport
    {
        public long task_code { get; set; }
        public string description { get; set; }
        public string asset_id { get; set; }
        public string asset_name { get; set; }
        public string assigned_asset_id { get; set; }
        public string assigned_asset_name { get; set; }
        public Guid WOcategorytoTaskMapping_id { get; set; }
        public Guid wo_inspectionsTemplateFormIOAssignment_id { get; set; }
        public int status_id { get; set; }
        public string status_name { get; set; }
        public bool is_parent_task { get; set; }
        public string WP { get; set; }
        public string technician_name { get; set; }
        public Guid? technician_id { get; set; }
        public int serial_number { get; set; }
        public string form_name { get; set; }
        public string form_type { get; set; }
        public string location { get; set; }
        public string task_rejected_notes { get; set; }
        public GetFormByWOTaskIDResponsemodelExport task_form { get; set; }
    }
    public class GetFormByWOTaskIDResponsemodelExport
    {
        public int response_status { get; set; }

        public Guid asset_form_id { get; set; }

        public Guid asset_id { get; set; }

        public Nullable<Guid> form_id { get; set; }

        public string asset_form_name { get; set; }

        public string asset_form_type { get; set; }

        public string asset_form_description { get; set; }

        public string asset_form_data { get; set; }

        public string requested_by { get; set; }

        public int status { get; set; }

        public string timezone { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }
        public string asset_name { get; set; }
        public string modified_by { get; set; }
        public string accepted_by { get; set; }
        public Guid WOcategorytoTaskMapping_id { get; set; }
        public DateTime? intial_form_filled_date { get; set; }
    }
}
