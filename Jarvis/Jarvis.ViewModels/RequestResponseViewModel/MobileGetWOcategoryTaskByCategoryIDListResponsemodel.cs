using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MobileGetWOcategoryTaskByCategoryIDListResponsemodel
    {
      //  public long task_code { get; set; }
       // public string description { get; set; } //
        public string asset_id { get; set; }
        public string asset_name { get; set; }
        public string assigned_asset_id { get; set; }
        public string assigned_asset_name { get; set; }
        public Guid WOcategorytoTaskMapping_id { get; set; }
        public Guid wo_inspectionsTemplateFormIOAssignment_id { get; set; }
        public int status_id { get; set; }
        public string status_name { get; set; }
       // public double field1 { get; set; }
      //  public double field2 { get; set; }
      //  public double field3 { get; set; }
      //  public double field4 { get; set; }
        public bool is_parent_task { get; set; }
       // public string WP { get; set; } //
        public string technician_name { get; set; }
        public Guid? technician_id { get; set; }
        public int serial_number { get; set; }
      //  public string form_name { get; set; }
      //  public string form_type { get; set; } //
        public string location { get; set; }
        public string task_rejected_notes { get; set; }
        public Guid wo_id { get; set; }
        public bool is_archived { get; set; }
       // public Guid? task_id { get; set; }
        public Guid form_id { get; set; }

    }
}
