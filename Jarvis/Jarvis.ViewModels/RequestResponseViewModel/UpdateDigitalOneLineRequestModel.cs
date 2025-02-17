using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateDigitalOneLineRequestModel
    {
        public string file_name { get; set; }
        public int? upload_status { get; set; }

        public Guid site_id { get; set;}
    }
}
