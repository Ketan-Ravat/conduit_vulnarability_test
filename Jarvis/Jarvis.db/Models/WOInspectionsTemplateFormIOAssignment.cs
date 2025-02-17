using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class WOInspectionsTemplateFormIOAssignment
    {
        [Key]
        public Guid wo_inspectionsTemplateFormIOAssignment_id { get; set; }
        
        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }
        
        [ForeignKey("InspectionsTemplateFormIO")]
        public Guid form_id { get; set; }

        [ForeignKey("Tasks")]
        public Guid? task_id { get; set; }

        [ForeignKey("User")]
        public Guid? technician_user_id { get; set; }

        [ForeignKey("Parent_Asset")]
        public Guid? asset_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public Guid? created_by { get; set; }
        public Guid? updated_by { get; set; }
        public bool is_archived { get; set; }
        
        [ForeignKey("InspectionTemplateAssetClass")]
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        
        [ForeignKey("StatusMaster")]
        public int status_id { get; set; }
        public string group_string { get;set; }
        public virtual StatusMaster StatusMaster { get; set; }
        public virtual Asset Parent_Asset { get; set; }
        public virtual InspectionsTemplateFormIO InspectionsTemplateFormIO { get; set; }
        public virtual WorkOrders WorkOrders { get; set; }
        public virtual User User { get; set; }
        public virtual Tasks Tasks { get; set; }
        public virtual InspectionTemplateAssetClass InspectionTemplateAssetClass { get; set; }
        public virtual ICollection<WOcategorytoTaskMapping> WOcategorytoTaskMapping { get; set; }

    }
}
