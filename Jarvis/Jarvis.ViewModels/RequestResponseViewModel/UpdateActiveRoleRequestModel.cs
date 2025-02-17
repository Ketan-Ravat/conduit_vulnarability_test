using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateActiveRoleRequestModel
    {
        public int platform { get; set; }

        public Guid role_id { get; set; }

        public bool ti_role { get; set; }
    }
}
