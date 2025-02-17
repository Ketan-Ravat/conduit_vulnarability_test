using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class TempAssetPMs
    {
        [Key]
        public Guid temp_asset_pm_id { get; set; }

        [ForeignKey("WOOnboardingAssets")]
        public Guid woonboardingassets_id { get; set; } // this will act as Main Asset fro this table
       
        [ForeignKey("PMs")]
        public Nullable<Guid> pm_id { get; set; }

        [ForeignKey("StatusMaster")]
        public int status { get; set; }
        public bool is_Asset_PM_fixed { get; set; }//bool use in mobile app as PM is done or not
        public Nullable<DateTime> created_at { get; set; }
        public bool is_archive { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public virtual WOOnboardingAssets WOOnboardingAssets { get; set; }
        public virtual PMs PMs { get; set; }
        public List<TempActiveAssetPMWOlineMapping> TempActiveAssetPMWOlineMapping { get; set; }
    }
}
