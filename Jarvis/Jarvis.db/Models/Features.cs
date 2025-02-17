using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class Features
    {
        [Key]
        public Guid feature_id { get; set; }

        public int? feature_type { get; set; } // company or user

        public string feature_name { get; set; }
        public string feature_description { get; set; }
        public DateTime created_at { get; set; }
        public virtual ICollection<CompanyFeatureMapping> CompanyFeatureMapping { get; set; }
    }
}
