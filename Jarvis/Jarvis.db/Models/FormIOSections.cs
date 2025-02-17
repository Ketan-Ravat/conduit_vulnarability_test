using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class FormIOSections
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int formiosection_id { get; set; }
        public string formio_section_name { get; set; }
        public DateTime? created_at { get; set; }
        public Guid site_id { get; set; }
        public Guid company_id { get; set; }
        [ForeignKey("FormIORooms")]
        public int? formioroom_id { get; set; }
        public virtual ICollection<AssetFormIOBuildingMappings> AssetFormIOBuildingMappings { get; set; }
        public virtual ICollection<WOLineBuildingMapping> WOLineBuildingMapping { get; set; }
        public virtual ICollection<TempFormIOSections> TempFormIOSections { get; set; }
        public virtual FormIORooms FormIORooms { get; set; }
        public virtual FormIOLocationNotes FormIOLocationNotes { get; set; }
    }
}
