using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssignExistingAssettoOBWORequestmodel
    {
        public List<Guid> asset_id { get; set; }
        public Guid wo_id { get; set; }
        public Guid site_id { get; set; }
        public Guid requested_by { get; set; }
        public int? status { get; set; }
        public bool is_woline_for_issue { get; set; }
        public Guid? temp_formiobuilding_id { get; set; }
        public Guid? temp_formiofloor_id { get; set; }
        public Guid? temp_formioroom_id { get; set; }
        //public bool is_woline_from_other_inspection { get; set; }
    }
}
