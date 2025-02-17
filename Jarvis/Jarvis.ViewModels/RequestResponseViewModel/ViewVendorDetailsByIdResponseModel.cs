using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ViewVendorDetailsByIdResponseModel
    {
        public string vendor_name { get; set; }
        public string vendor_email { get; set; }
        public string vendor_phone_number { get; set; }
        public string vendor_category { get; set; }
        public int? vendor_category_id { get; set; }
        public string vendor_address { get; set; }
        public List<Contacts_Data_Model> contacts_list { get; set; }
        public List<Workorders_Data_Model> workorders_list { get; set; }
    }
    public class Contacts_Data_Model
    {
        public Guid contact_id { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public int? category_id { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string notes { get; set; }
        public bool mark_as_primary { get; set; }
    }
    public class Workorders_Data_Model
    {
        public Guid wo_id { get; set; }
        public string manual_wo_number { get; set; } 
        public string site_name { get; set; } 
        public int wo_type { get; set; }
        public int? total_count { get; set; }
        public int? accepted_count { get; set; }
        public DateTime due_at { get; set; }
        public DateTime start_date { get; set; }
        public List<contacts_ivite_status_class> contacts_invite_status { get; set; }
    }
    public class contacts_ivite_status_class
    {
        public string name { get; set; }
        public int? contact_invite_status { get; set; }
    }
}
