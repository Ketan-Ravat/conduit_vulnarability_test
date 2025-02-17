using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetTransactionHistory
    {
        [Key]
        public Guid asseet_txn_id { get; set; }
        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }
        public string inspection_id { get; set; }
        public string operator_id { get; set; }
        public string manager_id { get; set; }
        public Nullable<int> status { get; set; }
        public string comapny_id { get; set; }
        public string site_id { get; set; }
        [Column(TypeName = "jsonb")]
        public List<AssetsValueJsonObject> attributeValues { get; set; }
        public string inspection_form_id { get; set; }
        public Nullable<long> meter_hours { get; set; }
        public Nullable<int> shift { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public virtual Asset Asset { get; set; }
    }
}
