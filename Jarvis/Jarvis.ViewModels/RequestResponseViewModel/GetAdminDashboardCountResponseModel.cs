using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAdminDashboardCountResponseModel
    {
        public int active_sites_count { get; set; }
        public int active_workorders_count { get; set; }
        public int active_technicians_count { get; set; }
        public int neta_inspection_line_items_count { get; set; }
        public int regular_inspection_line_items_count { get; set; }

        public WorkorderStatusWiseCountObject wo_status_wise_count_object { get; set; }
    }
    public class WorkorderStatusWiseCountObject
    {
        public int planned_wo_count { get; set; }
        public int released_open_wo_count { get; set; }
        public int inprogress_wo_count { get; set; }
        public int completed_wo_count { get; set; }
        public int hold_wo_count { get; set; }
        public int due_wo_count { get; set; }
        public int overdue_wo_count { get; set; }
    }
}
