using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class CreateUpdateVendorRequestModel
    {
        public Guid? vendor_id { get; set; }
        public string vendor_name { get; set; }
        public string vendor_email { get; set; }
        public string vendor_phone_number { get; set; }
        public int vendor_category_id { get; set; }
        public string vendor_address { get; set; }

        public bool is_deleted { get; set; }
    }
}
