using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class WorkOrder
    {
        [Key]
        public Guid work_order_uuid { get; set; }

        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }

        [ForeignKey("Attributes")]
        public Guid attribute_id { get; set; }

        [ForeignKey("Inspection")]
        public Guid inspection_id { get; set; }

        public string internal_asset_id { get; set; }

        public string name { get; set; }

        public long? work_order_number { get; set; }

        public string description { get; set; }

        public string notes { get; set; }

        [Column(TypeName = "jsonb")]
        public AssetsValueJsonObject[] attributes_value { get; set; }

        [ForeignKey("StatusMaster")]
        public int status { get; set; }

        [Column(TypeName = "jsonb")]
        public List<CommentJsonObject> comments { get; set; }

        public int priority { get; set; }

        public DateTime checkout_datetime { get; set; }

        public DateTime requested_datetime { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string modified_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public Nullable<DateTime> updated_at { get; set; }

        //public virtual Asset Asset { get; set; }
        [ForeignKey("Sites")]
        public Guid? site_id { get; set; }

        public string company_id { get; set; }

        public virtual StatusMaster StatusMaster { get; set; }

        public string maintainence_staff_id { get; set; }

        public ICollection<WorkOrderStatus> WorkOrderStatus { get; set; }

        public virtual WorkOrderRecord WorkOrderRecord { get; set; }

        public virtual Sites Sites { get; set; }

        public virtual Inspection Inspection { get; set; }

        public virtual InspectionFormAttributes Attributes { get; set; }

        public virtual Asset Asset { get; set; }
    }
}