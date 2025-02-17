using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class InspectionFormTypes
    {
        //public InspectionFormTypes()
        //{
        //    InspectionForms = new HashSet<InspectionForms>();
        //}

        [Key]
        public Guid inspection_form_type_id { get; set; }
        public string name { get; set; }
        [ForeignKey("Company")]
        public Guid company_id { get; set; }
        public string site_id { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public virtual Company Company { get; set; }
        //public virtual ICollection<InspectionForms> InspectionForms { get; set; }
    }
}
