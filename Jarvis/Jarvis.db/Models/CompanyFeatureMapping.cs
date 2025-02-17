using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class CompanyFeatureMapping
    {
        [Key]
        public Guid company_feature_id { get; set; }

        [ForeignKey("Company")]
        public Guid? company_id { get; set; }

        [ForeignKey("Features")]
        public Guid? feature_id { get; set; }

        [ForeignKey("User")]
        public Guid? user_id { get; set; }

        public bool is_required { get; set; }
        public DateTime created_at { get; set; }

        public Features Features { get; set; }
        public Company Company { get; set; }

        public User User { get; set; }
    }
}
