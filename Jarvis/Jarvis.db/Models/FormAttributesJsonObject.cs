using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class FormAttributesJsonObject
    {
        [Key]
        public Guid attributes_id { get; set; }
        public string name { get; set; }
        public int values_type { get; set; }
        public string company_id { get; set; }
        public int category_id { get; set; }
        public string site_id { get; set; }
        [Column(TypeName = "jsonb")]
        public AttributeValueJsonObject[] value_parameters { get; set; }
    }
}
