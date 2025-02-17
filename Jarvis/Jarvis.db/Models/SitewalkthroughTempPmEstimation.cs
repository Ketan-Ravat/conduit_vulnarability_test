using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class SitewalkthroughTempPmEstimation
    {
        [Key]
        public Guid sitewalkthrough_temp_pm_estimation_id { get; set; }

        [ForeignKey("TempAsset")]
        public Guid tempasset_id { get; set; }

        [ForeignKey("PMPlans")]

        public Guid pm_plan_id { get; set; }

        [ForeignKey("PMs")]

        public Guid pm_id { get; set; }

        [ForeignKey("WOOnboardingAssets")]
        public Guid woonboardingassets_id { get; set; }

        public int? estimation_time { get; set; }

        [ForeignKey("InspectionTemplateAssetClass")]

        public Guid inspectiontemplate_asset_class_id { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public bool is_deleted { get; set; }

        public virtual TempAsset TempAsset { get; set; }

        public virtual PMPlans PMPlans { get; set; }

        public virtual PMs PMs { get; set; }

        public virtual WOOnboardingAssets WOOnboardingAssets { get; set; }

        public virtual InspectionTemplateAssetClass InspectionTemplateAssetClass { get; set; }





    }
}
