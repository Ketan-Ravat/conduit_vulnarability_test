using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class InspectionsTemplateFormIO
    {
        [Key]
        public Guid form_id { get; set; }

        public string form_name { get; set; }

        public string form_type { get; set; }


        public string form_data { get; set; }
        public string form_output_data_template { get; set; }

        public Guid company_id { get; set; }
        public string form_description { get; set; }
        public int? inpsection_form_type { get; set; }
      
       // [ForeignKey("Sites")]
//        public Guid site_id { get; set; }

  //      [ForeignKey("Asset")]
    //    public Guid? asset_id { get; set; }

        [ForeignKey("StatusMaster")]
        public Nullable<int> status { get; set; }

        [ForeignKey("FormIOType")]
        public Nullable<int> form_type_id { get; set; }

        [ForeignKey("InspectionTemplateAssetClass")]
        public Guid? inspectiontemplate_asset_class_id { get; set; }

        public string work_procedure { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public virtual ICollection<Asset> Asset { get; set; }
        //
        //   public virtual Sites Sites { get; set; }
        public string dynamic_fields { get; set; } 
        public string dynamic_nameplate_fields { get; set; } 
        public string asset_class_form_properties { get; set; } 
        public bool is_master_form { get; set; }
        public virtual StatusMaster StatusMaster { get; set; }
        public virtual InspectionTemplateAssetClass InspectionTemplateAssetClass { get; set; }
        public virtual FormIOType FormIOType { get; set; }
      //  public ICollection<Asset> Asset { get; set; }
        public virtual List<Tasks> Tasks { get; set; }
        public virtual ICollection<WOInspectionsTemplateFormIOAssignment> WOInspectionsTemplateFormIOAssignment { get; set; }
        public virtual ICollection<AssetClassFormIOMapping> AssetClassFormIOMapping { get; set; }
      

    }
}
