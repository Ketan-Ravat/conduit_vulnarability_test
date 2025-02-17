using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetWOBacklogCardList_V2RequestModel
    {
        public string search_string { get; set; }
        public List<string>? site_id { get; set; }
    }
}
