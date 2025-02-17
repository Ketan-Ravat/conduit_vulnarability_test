using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetTopLevelAssetsRequestModel
    {
        public Guid site_id { get; set; }
        public int pageindex { get; set; }  
        public int pagesize { get; set; }                                     
        public string search_string { get; set; }                                     
    }
}
