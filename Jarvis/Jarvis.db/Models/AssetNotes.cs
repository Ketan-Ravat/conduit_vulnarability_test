using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetNotes
    {
        [Key]
        public Guid asset_notes_id { get; set; }

        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }
        public string asset_note { get; set; }
        public Guid asset_note_added_by_userid { get; set; }
        public string  asset_note_added_by_user { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public Guid? created_by { get; set; }
        public Guid? updated_by { get; set; }
        public bool is_deleted { get; set; }
        
        [ForeignKey("AssetFormIO")]
        public Guid? asset_form_id { get; set; }

        [ForeignKey("WOOnboardingAssets")]
        public Guid? woonboardingassets_id { get; set; }
        public Asset Asset { get; set; }
        public Sites Sites { get; set; }
        public AssetFormIO AssetFormIO { get; set; }
        public WOOnboardingAssets WOOnboardingAssets { get; set; }
       

    }
}
