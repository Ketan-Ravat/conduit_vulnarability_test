using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetWOTypeWiseSubmittedAssetsCountResponseModel
    {
        public int acceptance_wo_submitted_assets_count { get; set; }
        public int maintenance_wo_submitted_assets_count { get; set; }
        public int ob_wo_submitted_assets_count { get; set; }
        public int ir_wo_submitted_assets_count { get; set; }
        public int maintenance_wo_neta_inspection_count { get; set; }
        public int maintenance_wo_other_inspection_count { get; set; }
        public int mwo_ob_ir_wo_total_submitted_count { get; set; }

        public int asset_count { get; set; }
        public int equipment_count { get; set; }
        public int asset_pm_count { get; set; }
        public int asset_issue_count { get; set; }
        public int workorder_count { get; set; }
    }
}
