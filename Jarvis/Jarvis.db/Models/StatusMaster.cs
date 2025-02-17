using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class StatusMaster
    {
        [Key]
        public int status_id { get; set; }
        [ForeignKey("StatusTypes")]
        public int status_type_id { get; set; }
        public string status_name { get; set; }
        public virtual StatusTypes StatusTypes { get; set; }

        public virtual ICollection<Issue> Issue { get; set; }

        [InverseProperty("StatusMaster")]
        public virtual ICollection<Asset> Asset { get; set; }

        public virtual ICollection<Company> Company { get; set; }

        public virtual ICollection<Inspection> Inspection { get; set; }

        public virtual ICollection<InspectionForms> InspectionForms { get; set; }

        public virtual ICollection<Sites> Sites { get; set; }

        [InverseProperty("StatusMaster")]
        public virtual ICollection<User> User { get; set; }
        [InverseProperty("Report_StatusMaster")]
        public virtual ICollection<User> ReportUser { get; set; }

        [InverseProperty("ConditionMaster")]
        public virtual ICollection<Asset> FormIOAssetCondition { get; set; }
      //  [InverseProperty("StatusMaster")]
        //public virtual ICollection<Asset> FormIOAsset { get; set; }

        public virtual ICollection<UserSites> UserSites { get; set; }

        public virtual ICollection<IssueRecord> IssueRecord { get; set; }

        [InverseProperty("StatusMaster")]
        public virtual ICollection<AssetFormIO> InspectionsTemplateFormIO { get; set; }

        [InverseProperty("PDFReportStatusMaster")]
        public virtual ICollection<AssetFormIO> InspectionsTemplateFormIOPDF { get; set; }
    }
}
