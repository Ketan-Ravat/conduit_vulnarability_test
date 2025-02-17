using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetPMListRequestmodel
    {
        public int pagesize { get; set; }
        public int pageindex { get; set; }
        public string search_string { get; set; }
        public Guid? asset_id { get; set; }
        public List<int> status { get; set; }
        public bool is_requested_for_assign { get; set; } = false; // if this is true then return asset pm which are left to link in any WO and it is not completed status
        public Guid? asset_form_id { get; set; } // Already Linked PM
        public bool is_requested_for_overdue_pm { get; set; } = false; // if true then return all overdue PMs list
        public List<Guid>? inspectiontemplate_asset_class_id { get; set; }
        public List<string>? title { get; set; }
        public DateTime? end_due_date { get; set; }
        public bool is_request_for_pm_report { get; set; }
        public bool want_to_remove_overdue_pms { get; set; }
        public bool is_requested_for_current_assetpms { get; set; }
    }
}
