using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MapWOAttachmenttoWORequestModel
    {
        public Guid wo_id { get; set; }
        public string file_name { get; set; }
        public string user_uploaded_name { get; set; }
    }
}
