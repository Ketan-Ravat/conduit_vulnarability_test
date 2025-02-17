using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllSiteDocumentResponsemodel
    {
        public Guid sitedocument_id { get; set; }
        public string file_name { get; set; }
        public string file_url { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }

    }
}
