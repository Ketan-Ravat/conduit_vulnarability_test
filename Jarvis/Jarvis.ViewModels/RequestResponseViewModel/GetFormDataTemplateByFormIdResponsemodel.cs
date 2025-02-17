using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetFormDataTemplateByFormIdResponsemodel
    {
        public Guid form_id { get; set; }
        public string form_name { get; set; }
        public string form_output_data_template { get; set; }
    }
}
