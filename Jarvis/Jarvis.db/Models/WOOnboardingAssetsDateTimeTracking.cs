using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class WOOnboardingAssetsDateTimeTracking
    {
        [Key]
        public Guid woonboardingassets_datetime_tracking_id { get; set; }

        [ForeignKey("WOOnboardingAssets")]
        public Guid woonboardingassets_id { get; set; }

        public DateTime? saved_date { get; set; }
        public DateTime? submitted_date { get; set; }
        public DateTime? accepted_date { get; set; }
        public DateTime? rejected_date { get; set; }
        public DateTime? hold_date { get; set; }
        public DateTime? deleted_date { get; set; }
        public DateTime? work_start_date { get; set; }
        public int? work_time_spend { get; set; } // number of seconds spend 
        public string modified_by { get; set; }
        public virtual WOOnboardingAssets WOOnboardingAssets { get; set; }
    }
}
