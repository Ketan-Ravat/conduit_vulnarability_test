using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetClassFormIOMapping
    {
        [Key]
        public Guid asset_class_formio_mapping_id { get; set; }

        [ForeignKey("InspectionTemplateAssetClass")]
        public Guid inspectiontemplate_asset_class_id { get; set; }
        
        [ForeignKey("InspectionsTemplateFormIO")]
        public Guid? form_id { get; set; }
        public bool isarchive { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
       
        [ForeignKey("WOTypeStatusMaster")]
        public int? wo_type { get; set; }
        public virtual StatusMaster WOTypeStatusMaster { get; set; }
        public InspectionTemplateAssetClass InspectionTemplateAssetClass { get; set; }
        public InspectionsTemplateFormIO InspectionsTemplateFormIO { get; set; }
    }
}
