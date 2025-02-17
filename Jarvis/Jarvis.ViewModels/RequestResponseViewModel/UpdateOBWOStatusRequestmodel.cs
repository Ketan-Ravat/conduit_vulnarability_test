using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateOBWOStatusRequestmodel
    {
        public Guid wo_id { get; set; }
        public int status { get; set; }
        public bool is_from_MWO { get; set; } = false;// this is for BE internal purpose
    }
}
