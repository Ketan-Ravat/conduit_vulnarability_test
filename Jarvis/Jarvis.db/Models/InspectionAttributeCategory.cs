using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class InspectionAttributeCategory
    {
        [Key]
        public int category_id { get; set; }
        public string name { get; set; }
        public virtual ICollection<InspectionFormAttributes> InspectionFormAttributes { get; set; }
    }
}
