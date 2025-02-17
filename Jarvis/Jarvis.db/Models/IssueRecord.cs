using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class IssueRecord
    {
        [Key]
        public Guid issue_record_uuid { get; set; }
        [ForeignKey("Issue")]
        public Guid issue_uuid { get; set; }
        [ForeignKey("Attributes")]
        public Guid attrubute_id { get; set; }
        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }
        [ForeignKey("Inspection")]
        public Guid inspection_id { get; set; }
        [ForeignKey("StatusMaster")]
        public int status { get; set; }
        public DateTime requested_datetime { get; set; }
        public DateTime checkout_datetime { get; set; }
        public DateTime created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? fixed_datetime { get; set; }
        public string? fixed_by { get; set; }

        public virtual Inspection Inspection { get; set; }
        public virtual Asset Asset { get; set; }
        public virtual InspectionFormAttributes Attributes { get; set; }
        public virtual Issue Issue { get; set; }
        public virtual StatusMaster StatusMaster { get; set; }
    }
}
