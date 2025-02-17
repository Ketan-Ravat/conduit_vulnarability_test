using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class WOcategorytoTaskMapping
    {
        [Key]
        public Guid WOcategorytoTaskMapping_id { get; set; }

        [ForeignKey("WOInspectionsTemplateFormIOAssignment")]
        public Guid wo_inspectionsTemplateFormIOAssignment_id { get; set; }
        
        [ForeignKey("Tasks")]
        public Guid? task_id { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid? wo_id { get; set; }

      //  [ForeignKey("User")]
      //  public Guid? technician_user_id { get; set; }
        public bool is_parent_task { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public Guid? created_by { get; set; }
        public Guid? updated_by { get; set; }
        public bool is_archived { get; set; }
        public bool is_newly_asset_created { get; set; }
        public string task_rejected_notes { get; set; }
        public int serial_number { get; set; }
        public int? inspection_type { get; set; }
        public Guid? woonboardingassets_id { get; set; }// ob asset id is for in MWO---> insppction tab ---> new asset so new asset will create as OB asset in MWO
        [ForeignKey("_assigned_asset")]
        public Guid? assigned_asset { get; set; }

        [ForeignKey("Asset")]
        public Guid? newly_created_asset_id { get; set; }
        public virtual Asset Asset { get; set; }
        public virtual Asset _assigned_asset { get; set; }
        public virtual Tasks Tasks { get; set; }
        public virtual WOInspectionsTemplateFormIOAssignment WOInspectionsTemplateFormIOAssignment { get; set; }
        public virtual AssetFormIO AssetFormIO { get; set; }
        public virtual WorkOrders WorkOrders { get; set; }
       //public virtual User User { get; set; }
    }
}
