using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddUpdateWorkOrderWatcherRequestModel
    {
        public Guid ref_id { get; set; }
        public Guid user_id { get; set; }
        public bool is_deleted { get; set; }

        //public int? ref_type { get; set; }  // 1 = Workorder , 2 = ?
        //public int? user_role_type { get; set; } // 1 = Back-Office , 2 = Technician
    }
}
