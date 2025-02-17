using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetMeterHourHistory
    {
        [Key]
        public Guid asset_meter_hour_id { get; set; }

        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }
        public Nullable<long> meter_hours { get; set; }
        public string requested_by { get; set; }
        public Nullable<int> status { get; set; }
        public string company_id { get; set; }
        public string site_id { get; set; }
        public Nullable<DateTime> updated_at { get; set; }
        public virtual Asset Asset { get; set; }
    }
}
