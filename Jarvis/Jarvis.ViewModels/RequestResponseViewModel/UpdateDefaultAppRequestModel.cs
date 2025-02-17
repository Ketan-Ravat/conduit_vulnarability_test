using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateDefaultAppRequestModel
    {
        public Guid user_id { get; set; }

        //public Guid requested_by { get; set; }

        public int app_id { get; set; }
    }
}
