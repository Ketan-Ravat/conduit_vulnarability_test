using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllAssetIssuesRequestmodel
    {
        public int page_size { get; set; }
        public int page_index { get; set; }
        public string search_string { get; set; }
        public  List<int>? status { get; set; }
        public Guid? asset_id { get; set; }

    }
}
