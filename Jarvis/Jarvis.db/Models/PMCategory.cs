using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class PMCategory
    {
        public PMCategory()
        {
            PMPlans = new HashSet<PMPlans>();
        }
        [Key]
        public Guid pm_category_id { get; set; }

        public string category_name { get; set; }

        public string category_code { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        [ForeignKey("StatusMaster")]
        public Nullable<int> status { get; set; }

        [ForeignKey("Company")]
        public Guid? company_id { get; set; }
        
        [ForeignKey("InspectionTemplateAssetClass")]
        public Guid? inspectiontemplate_asset_class_id { get; set; }

        public virtual StatusMaster StatusMaster { get; set; }
        public virtual ICollection<PMPlans> PMPlans { get; set; }
        public virtual Company Company { get; set; }
        public virtual InspectionTemplateAssetClass InspectionTemplateAssetClass { get; set; }
    }
}
