using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllCatagoryForWORequestModel
    {
        public int pagesize { get; set; } = 0;
        public int pageindex { get; set; } = 0;
        public Guid wo_id { get; set; }
    }
}
