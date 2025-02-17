using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.ExcludePropertiesfromDBHelper
{
    public class FormIOFormsExcludedProprties
    {
        public Guid form_id { get; set; }
        public string form_name { get; set; }
        public string form_type { get; set; }
        public string form_description { get; set; }
        public Nullable<int> status { get; set; }
        public string status_name { get; set; }
        public Nullable<int> form_type_id { get; set; }
        public string work_procedure { get; set; }
    }
}
