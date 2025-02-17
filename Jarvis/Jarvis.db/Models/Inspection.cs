using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public partial class Inspection
    {
        [Key]
        public Guid inspection_id { get; set; }

        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }

        [ForeignKey("User")]
        public Guid? operator_id { get; set; }

        public string manager_id { get; set; }

        [ForeignKey("StatusMaster")]
        public int status { get; set; }

        public bool is_comment_important { get; set; }

        public string operator_notes { get; set; }

        [Column(TypeName ="jsonb")]
        public List<AssetsValueJsonObject> attribute_values { get; set; }
        public string company_id { get; set; }
        [ForeignKey("Sites")]
        public Guid site_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime modified_at { get; set; }
        public string created_by { get; set; }
        public string modified_by { get; set; }
        public long meter_hours { get; set; }
        public int shift { get; set; }
        [Column(TypeName = "jsonb")]
        public ImagesListObject image_list { get; set; }
        public string manager_notes { get; set; }
        public Nullable<DateTime> datetime_requested { get; set; }

        public virtual User User { get; set; }
        public virtual Sites Sites { get; set; }
        public virtual Asset Asset { get; set; }

        public virtual StatusMaster StatusMaster { get; set; }
        
        public virtual ICollection<Issue> Issues { get; set; }
        public virtual ICollection<IssueRecord> IssueRecords { get; set; }
    }
}
