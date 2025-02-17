using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateWOOfflineSQSRequestmodel
    {
        public object data { get; set; } 
        public Guid requested_by { get; set; }
        public string file_key { get; set; }
        public Guid trackmobilesyncoffline_id { get; set; }
    }
}
