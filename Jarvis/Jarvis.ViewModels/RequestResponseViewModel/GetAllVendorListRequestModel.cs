using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllVendorListRequestModel
    {
        public string search_string { get; set; }
        public int pagesize { get; set; }
        public int pageindex { get; set; }
        public List<int> vendor_category_ids { get; set; }
    }
}
