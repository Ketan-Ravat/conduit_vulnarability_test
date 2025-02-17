using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetsValueJsonObject
    {
        //public Guid id { get; set; }

        //public string name { get; set; }

        //public string value { get; set; }

        public Guid id { get; set; }

        public string name { get; set; }

        public string value { get; set; }
        public int values_type { get; set; }

        public DateTime? date_time { get; set; }

        public int category_id { get; set; }

        //public string site_id { get; set; }

        //public AttributeValueJsonObject[] value_parameters { get; set; }
    }
}
