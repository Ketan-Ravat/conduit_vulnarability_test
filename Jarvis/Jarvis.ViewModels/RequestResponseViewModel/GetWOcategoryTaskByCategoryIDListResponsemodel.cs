using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetWOcategoryTaskByCategoryIDListResponsemodel
    {
      //  public long task_code { get; set; }
        public string description { get; set; } //
        public string asset_id { get; set; }
        public string asset_name { get; set; }
        public string assigned_asset_id { get; set; }
        public string woonboardingassets_id { get; set; }
        public int? component_level_type_id { get; set; } // 1 for top_level , 2- subcomponant
        public Guid? toplevelcomponent_asset_id { get; set; }
        public string assigned_asset_name { get; set; }
        public Guid WOcategorytoTaskMapping_id { get; set; }
        public Guid wo_inspectionsTemplateFormIOAssignment_id { get; set; }
        public int status_id { get; set; }
        public string status_name { get; set; }
        public bool is_parent_task { get; set; }
        public string WP { get; set; } //
        public string technician_name { get; set; }
        public Guid? technician_id { get; set; }
        public int serial_number { get; set; }
        public string form_name { get; set; }
        public string form_type { get; set; } //
        public string location { get; set; }
        public string task_rejected_notes { get; set; }
        public Guid wo_id { get; set; }
        public bool is_archived { get; set; }
        public Guid? task_id { get; set; }
        public Guid form_id { get; set; }
        public bool? defects { get; set; }
        public string asset_class_name { get; set; }
        public int? inspection_type { get; set; }
        public int? inspection_verdict { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_type { get; set; }
        public Guid asset_form_id { get; set; }
        //  public List<DynamicFields> dynamic_field { get; set; }
        public int wo_type { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public Guid site_id { get; set; }
        public string site_name { get; set; }
    }

}
