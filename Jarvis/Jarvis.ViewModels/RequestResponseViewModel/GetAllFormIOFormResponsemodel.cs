using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllFormIOFormResponsemodel
    {
        public int response_status { get; set; }
        public Guid form_id { get; set; }
        public string form_name { get; set; }
        public string form_type { get; set; }
        public int status { get; set; }
        public string status_name { get; set; }
        public string form_description { get; set; }
        public string work_procedure { get; set; }
        public int? form_type_id { get; set; }

        public int? inpsection_form_type { get; set; }
    }
}
