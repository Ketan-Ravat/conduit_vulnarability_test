using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateDefaultRoleRequestModel
    {
        //public Guid requested_by { get; set; }

        //public Guid user_id { get; set; }

        public int platform { get; set; }

        public Guid role_id { get; set; }

        public bool ti_role { get; set; }
    }
}
