using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class InspectionForms
    {
        [Key]
        public Guid inspection_form_id { get; set; }
        public string name { get; set; }
        [ForeignKey("Company")]
        public Guid company_id { get; set; }
        public string site_id { get; set; }
        //[ForeignKey("InspectionFormTypes")]
        //public Guid inspection_form_type_id { get; set; }
        [ForeignKey("StatusMaster")]
        public Nullable<int> status { get; set; }
        [Column(TypeName = "jsonb")]
        public FormAttributesJsonObject[] form_attributes { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        [ForeignKey("FormIOType")]
        public Nullable<int> form_type_id { get; set; }
        public virtual FormIOType FormIOType { get; set; }
        public virtual Company Company{ get; set; }
        //public virtual InspectionFormTypes InspectionFormTypes { get; set; }

        public ICollection<Asset> Asset { get; set; }

        public StatusMaster StatusMaster { get; set; }

    }
}
