using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class Tasks
    {
        [Key]
        public Guid task_id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long task_code { get; set; }

        public Guid company_id { get; set; }

        public string task_title { get; set; }

        public int task_est_minutes { get; set; }

        public int task_est_hours { get; set; }

        public string task_est_display { get; set; }

        public double? hourly_rate { get; set; }

        [ForeignKey("FormIO")]
        public Guid? form_id { get; set; }

        [ForeignKey("Asset")]
        public Guid? asset_id { get; set; }
        public string description { get; set; }

        public string notes { get; set; }

        public bool isArchive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public virtual ICollection<PMTasks> PMTasks { get; set; }

        public virtual ICollection<AssetTasks> AssetTasks { get; set; }

        public virtual InspectionsTemplateFormIO FormIO { get; set; }
        public virtual Asset Asset { get; set; }
        public virtual ICollection<WOcategorytoTaskMapping> WOcategorytoTaskMapping { get; set; }
    }
}
