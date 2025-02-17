using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllAssetClassRequestmodel
    {

        public Guid company_id { get; set; }
        public int page_size { get; set; }
        public int page_index { get; set; }
        public string search_string { get; set;}

    }
}
