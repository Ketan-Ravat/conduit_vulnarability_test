using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public  class FormIOInsulationResistanceTestMapping
    {
        [Key]
        public Guid FormIOInsulationResistanceTestMapping_id { get; set; }

        public string IRPoletoPoleAsFound1 { get; set; }
        public string IRPoletoPoleAsFound2 { get; set; }
        public string IRPoletoPoleAsFound3 { get; set; }

        public string IRAcrossPoleAsFound1 { get; set; }
        public string IRAcrossPoleAsFound2 { get; set; }
        public string IRAcrossPoleAsFound3 { get; set; }

        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public string updated_by { get; set; }
        public bool is_archived { get; set; }


        [ForeignKey("AssetFormIO")]
        public Nullable<Guid> asset_form_id { get; set; }
        public virtual AssetFormIO AssetFormIO { get; set; }
    }
}
