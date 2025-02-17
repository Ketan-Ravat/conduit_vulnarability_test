using Jarvis.Shared.StatusEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class NewFlowCreateWORequestModel
    {
        public Guid? wo_id { get; set; } // to update WO
        public Guid client_company_id { get; set; } /// Enterprice
        public string Description { get; set; }
        public Guid site_id { get; set; }
        public Guid? technician_user_id { get; set; }   
        public DateTime start_date { get; set;  }
        public int wo_type { get; set; }
        public int wo_status { get; set; } = (int)Status.open;
        public int? quote_status { get; set; }
        public string manual_wo_number { get; set; }
        public DateTime due_date { get; set; }
        public string po_number { get; set; }
        public Guid? responsible_party_id { get; set; }
        public int? ir_visual_camera_type { get; set; }
        public int? ir_visual_image_type { get; set; }
        public bool is_reminder_required { get; set; } //if true then send email reminder
        public string reminders_frequency_json { get; set; }
        //public string calendarId { get; set; }
        public string wo_vendor_notes_json { get; set; }// json to store notes vendor wise
        public List<WO_TechnicianUser_Mapping_Class> workorder_technician_list { get; set; }
        public List<WO_BackOfficeUser_Mapping_Class> workorder_backoffice_list { get; set; }
        public List<WO_Vendor_Contacts_Mapping_Class> workorder_vendor_contacts_list { get; set; }
        public bool is_required_to_send_email { get; set; }
    }
    public class WO_Vendor_Contacts_Mapping_Class
    {
        public bool is_deleted { get; set; }
        public Guid? vendor_id { get; set; }
        public List<Contacts_Data_Obj_Class> contacts_list { get; set; }
    }
    public class Contacts_Data_Obj_Class
    {
        public Guid? workorders_vendor_contacts_mapping_id { get; set; }
        public Guid contact_id { get; set; }
        public bool is_deleted { get; set; }
    }
    public class WO_TechnicianUser_Mapping_Class
    {
        public Guid? wo_technician_mapping_id { get; set; }
        public Guid user_id { get; set; }
        public bool is_deleted { get; set; }
        public bool is_curr_site_user { get; set; }
    }
    public class WO_BackOfficeUser_Mapping_Class
    {
        public Guid? wo_backoffice_user_mapping_id { get; set; }
        public Guid user_id { get; set; }
        public bool is_deleted { get; set; }
        public bool is_curr_site_user { get; set; }
    }
}
