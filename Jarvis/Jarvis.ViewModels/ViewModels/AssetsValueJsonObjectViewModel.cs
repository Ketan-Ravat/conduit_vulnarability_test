using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class AssetsValueJsonObjectViewModel
    {
        public Guid id { get; set; }

        public string name { get; set; }

        public string value { get; set; }
        public int values_type { get; set; }

        public int category_id { get; set; }

        public DateTime? date_time { get; set; }
    }
}
