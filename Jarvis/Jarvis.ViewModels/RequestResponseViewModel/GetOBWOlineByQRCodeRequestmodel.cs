using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetOBWOlineByQRCodeRequestmodel
    {
        public Guid wo_id { get; set; }
        public string search_string { get; set; }
    }
}
