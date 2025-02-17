using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class PMItemMasterForms
    {
        [Key]
        public Guid pmitemmasterform_id { get; set; }

        [ForeignKey("Company")]
        public Guid? company_id { get; set; }
        public string form_json { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string plan_name { get; set; }
        public string form_name { get; set; }
        public string pm_title { get; set; }
        public bool is_deleted { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public Company Company { get; set; }

    }
}
