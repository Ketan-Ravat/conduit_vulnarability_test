using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class FormIOType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int form_type_id { get; set; }
        public string form_type_name { get; set; }
        public Guid company_id { get; set; }
        public bool isarchive { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public virtual ICollection<InspectionForms> InspectionForms { get; set; }
        public virtual ICollection<InspectionTemplateAssetClass> InspectionTemplateAssetClass { get; set; }
    }
}
