using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.Models
{
    public class ReportJsonData
    {
        public Guid? site_id { get; set; }

        public Guid? asset_id { get; set; }

        public string? site_name { get; set; }

        public string? asset_name { get; set; }

        public Guid? attribute_id { get; set; }

        public DateTime? not_ok_since { get; set; }

        public string? attribute_name { get; set; }

        public string internal_asset_id { get; set; }
    }
}
