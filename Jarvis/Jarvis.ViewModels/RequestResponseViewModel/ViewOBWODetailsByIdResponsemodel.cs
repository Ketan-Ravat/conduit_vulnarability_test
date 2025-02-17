using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ViewOBWODetailsByIdResponsemodel
    {
        public Guid wo_id { get; set; }
        public string wo_number { get; set; }
        public int issues_count { get; set; }
        public int location_room_count { get; set; }
        public int wo_type { get; set; }
        public string manual_wo_number { get; set; }
        public DateTime? due_date { get; set; }
        public int? wo_due_overdue_flag { get; set; }
        public string due_in { get; set; }
        public string wo_type_name { get; set; }
        public string description { get; set; }
        public Guid site_id { get; set; }
        public string site_name { get; set; }
        public DateTime start_date { get; set; }
        public int wo_status_id { get; set; }
        public string wo_status { get; set; }
        public int time_materials_count { get; set; }
        public Nullable<DateTime> completed_date { get; set; }
        public Guid? client_company_id { get; set; }
        public string client_company_name { get; set; }
        public string ir_wo_pdf_report { get; set; }
        public int? ir_wo_pdf_report_status { get; set; }
        public bool is_watcher { get; set; }
        public string po_number { get; set; }
        public Guid? responsible_party_id { get; set; }
        public string responsible_party_name { get; set; }
        public int? quote_status_id { get; set; } // accepted , rejected, Defered
        public string quote_status_name { get; set; } // accepted , rejected, Defered
        public int? ir_visual_camera_type { get; set; }
        public int? ir_visual_image_type { get; set; }
        public int new_location_count { get; set; }
        public bool is_reminder_required { get; set; } //if true then send email reminder
        public string reminders_frequency_json { get; set; }
        public string calendarId { get; set; }
        public string wo_vendor_notes_json { get; set; }
        public int total_contacts_count { get; set; }
        public int accepted_contacts_count { get; set; }
        public int ir_image_count { get; set; }
        public List<OBWOAssetDetails> asset_details { get; set; }
        public List<OBWOAssetDetails> asset_details_v2 { get; set; }
        public List<WorkOrder_TechnicianUser_Details_Class> technician_mapping_list { get; set; }
        public List<WorkOrder_BOUser_Details_Class> backoffice_mapping_list { get; set; }
        public List<WO_Vendor_Contacts_Mapping_View_Class> workorder_vendor_contacts_list { get; set; }
        public status_wise_asset_count_obj status_wise_asset_count_obj { get; set; }
        public List<WorkOrderAttachmentsResponseModel> WorkOrderAttachments { get; set; }
        
    }
    
    public class OBWOAssetDetails
    {
        public Guid woonboardingassets_id { get; set; }
        public Guid asset_id { get; set; }
        public string asset_name { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public string temp_master_building { get; set; }
        public string temp_master_floor { get; set; }
        public string temp_master_room { get; set; }
        public string temp_master_section { get; set; }
        public Guid site_id { get; set; }
        public string site_name { get; set; }
        public Guid? temp_formiobuilding_id { get; set; }
        public Guid? temp_formiofloor_id { get; set; }
        public Guid? temp_formioroom_id { get; set; }
        public Guid? temp_formiosection_id { get; set; }
        public int status { get; set; }
        public int component_level_type_id { get; set; } // 1 for top_level , 2- subcomponant
        public Guid? toplevelcomponent_asset_id { get; set; }
        public string  status_name { get; set; }
        public string QR_code { get; set; }
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public string asset_class_type { get; set; }
        public string form_nameplate_info { get; set; }
        public int? arc_flash_label_valid { get; set; }
        public int? maintenance_index_type { get; set; }
        public int temp_issues_count { get; set; }
        public string asset_profile { get; set; }
        public List<string> issues_title_list { get; set; }
    }

    public class WO_Vendor_Contacts_Mapping_View_Class
    {
        public Guid? vendor_id { get; set; }
        public string vendor_name { get; set; }
        public string vendor_email { get; set; }
        public List<Contacts_Data_View_Obj_Class> contacts_list { get; set; }
    }
    public class Contacts_Data_View_Obj_Class
    {
        public Guid? workorders_vendor_contacts_mapping_id { get; set; }
        public Guid contact_id { get; set; }
        public Guid vendor_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public int? contact_invite_status { get; set; }
    }
    
}
