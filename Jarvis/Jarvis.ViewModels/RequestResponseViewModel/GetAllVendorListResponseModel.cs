using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllVendorListResponseModel
    {
        public List<Vendors_Data> vendors_list { get; set; }
        public int listsize { get; set; }
    }

    public class Vendors_Data
    {
        public Guid vendor_id { get; set; }
        public string vendor_name { get; set; }
        public string vendor_email { get; set; }
        public string vendor_phone_number { get; set; }
        public string vendor_category { get; set; }
        public int? vendor_category_id { get; set; }
        public string vendor_address { get; set; }


    }
}
