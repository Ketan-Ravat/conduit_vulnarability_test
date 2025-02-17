using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllWOLineTempIssuesRequestmodel
    {
        public int page_size { get; set; }
        public int page_index { get; set; }
        public Guid? wo_id { get; set; }
        public Guid? asset_form_id { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public string search_string { get; set; }
        
    }
}
