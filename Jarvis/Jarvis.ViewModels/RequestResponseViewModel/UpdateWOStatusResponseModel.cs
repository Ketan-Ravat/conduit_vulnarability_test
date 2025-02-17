using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateWOStatusResponseModel
    {
        public int task_id { get; set; }
        public int success { get; set; }
        public string asset_id { get; set; }
    }
}
