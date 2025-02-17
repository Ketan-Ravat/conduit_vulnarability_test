using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class InspectionTemplateAssetClass
    {
        [Key]
        public Guid inspectiontemplate_asset_class_id { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public int? asset_expected_usefull_life { get; set; }
        public string pdf_report_template_url { get; set; }
        [ForeignKey("FormIOType")]
        public int? form_type_id { get; set; }

        [ForeignKey("Company")]
        public Guid company_id { get; set; }
        public bool isarchive { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public string form_nameplate_info { get; set; }
        public  FormIOType FormIOType { get; set; }
        public  Company Company { get; set; }
        public PMCategory PMCategory { get; set; }
        public ICollection<InspectionsTemplateFormIO> InspectionsTemplateFormIO { get; set; }
        public ICollection<AssetClassFormIOMapping> AssetClassFormIOMapping { get; set; }
        public ICollection<WOInspectionsTemplateFormIOAssignment> WOInspectionsTemplateFormIOAssignment { get; set; }
        public ICollection<Asset> Asset { get; set; }
        public ICollection<TempAsset> TempAsset { get; set; }

    }
}
