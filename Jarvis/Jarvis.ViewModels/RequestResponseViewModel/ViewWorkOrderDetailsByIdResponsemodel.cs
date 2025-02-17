using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ViewWorkOrderDetailsByIdResponsemodel
    {
        public Guid wo_id { get; set; }
        public string wo_number { get; set; }
        public int wo_type { get; set; }
        public string manual_wo_number { get; set; }
        public int issues_count { get; set; }
        public int time_materials_count { get; set; }
        public string wo_type_name { get; set; }
        public string description { get; set; }
        public Guid site_id { get; set; }
        public string site_name { get; set; }
        public DateTime start_date { get; set; }
        public int wo_status_id { get; set; }
        public string wo_status { get; set; }
        public Nullable<DateTime> completed_date { get; set; }

        // public string description { get; set; }
        // public string technician_name { get; set; }
        // public Guid? technician_id { get; set; }
        public Guid? client_company_id { get; set; }
        public string client_company_name { get; set; }
        public string po_number { get; set; }
        public Guid? responsible_party_id { get; set; }
        public string responsible_party_name { get; set; }
        public int? quote_status_id { get; set; } // accepted , rejected, Defered
        public string quote_status_name { get; set; } // accepted , rejected, Defered
        public List<form_categoty_list> form_category_list { get; set; }
        public List<WorkOrderAttachmentsResponseModel> WorkOrderAttachments { get; set; }
        public List<GetWOcategoryTaskByCategoryIDListResponsemodel> wo_all_tasks { get; set; }
        //public List<mwo_ob_assets> mwo_ob_assets { get; set; }
        public List<mwo_ob_assets> mwo_ob_assets_v2 { get; set; }
        public string ir_wo_pdf_report { get; set; }
        public int? ir_wo_pdf_report_status { get; set; }
        public DateTime? due_date { get; set; }
        public int? wo_due_overdue_flag { get; set; }
        public string due_in { get; set; }
        public bool is_watcher { get; set; }
        public bool is_reminder_required { get; set; } //if true then send email reminder
        public string reminders_frequency_json { get; set; }
        public string calendarId { get; set; }
        public string wo_vendor_notes_json { get; set; }
        public int total_contacts_count { get; set; }
        public int accepted_contacts_count { get; set; }
        public List<WorkOrder_TechnicianUser_Details_Class> technician_mapping_list { get; set; }
        public List<WorkOrder_BOUser_Details_Class> backoffice_mapping_list { get; set; }
        public List<WO_Vendor_Contacts_Mapping_View_Class> workorder_vendor_contacts_list { get; set; }
        public status_wise_asset_count_obj status_wise_asset_count_obj { get; set; }
    }
    public class mwo_ob_assets
    {
        public Guid woonboardingassets_id { get; set; }
        public int? inspection_type { get; set; }
        public int status { get; set; }
        public string  status_name { get; set; }
        public Guid? asset_id { get; set; }
        public string asset_name { get; set; }
        public int? component_level_type_id { get; set; } // 1 for top_level , 2- subcomponant
        public Guid? toplevelcomponent_asset_id { get; set; }
        public Guid? technician_user_id { get; set; }
        public string technician_name { get; set; }
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string asset_class_type { get; set; }
        public Guid? asset_pm_id { get; set; }
        public Guid? temp_asset_pm_id { get; set; }
        public string asset_pm_title { get; set; }
        public int? pm_inspection_type_id { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public string temp_master_building { get; set; }
        public string temp_master_floor { get; set; }
        public string temp_master_room { get; set; }
        public string temp_master_section { get; set; }
        public bool is_pm_main_woline { get; set; } // this key is used to desable asset class change if woline is for PM main woline
        public Guid site_id { get; set; }
        public string site_name { get; set; }
        public Guid? tempasset_id { get; set; }
        public string form_nameplate_info { get; set; }
        public int? arc_flash_label_valid { get; set; }
        public int? maintenance_index_type { get; set; }
        public int temp_issues_count { get; set; }
        public string QR_code { get; set; }
        public List<string> issues_title_list { get; set; }
        public string asset_profile { get; set; }
    }
    public class form_categoty_list
    {
        public Guid wo_inspectionsTemplateFormIOAssignment_id { get; set; }
        public string form_category_name { get; set; }
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
        public Guid wo_id { get; set; }
        public Guid form_id { get; set; }
        public bool is_archived { get; set; }  
        public string asset_class_name { get; set; }
        public string asset_class_type { get; set; }
        public string group_string { get; set; }

    }

    public class WorkOrder_TechnicianUser_Details_Class
    {
        public Guid wo_technician_mapping_id { get; set; }
        public Guid user_id { get; set; }
        public bool is_deleted { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }
    public class WorkOrder_BOUser_Details_Class
    {
        public Guid wo_backoffice_user_mapping_id { get; set; }
        public Guid user_id { get; set; }
        public bool is_deleted { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }
}
