using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetTechniciansListWithSubmittedLinesCountResponseModel
    {
        public string technician_user_name { get; set;}
        public Guid user_id { get; set;}
        public string site_name { get; set; }
        public Guid site_id { get; set; }
        public int submitted_line_items_count { get; set; }

    }
}
