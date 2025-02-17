using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetIssue
    {
        [Key]
        public Guid asset_issue_id { get; set; }
        public int issue_type { get; set; } //Repair , Replacement , Compliance(NEC,Osha), Thermal

        [ForeignKey("Asset")]
        public Guid? asset_id { get; set; }//asset id to map this Issue
        public string issue_number { get; set; }
        
        [ForeignKey("StatusMaster")]
        public int? issue_status { get; set; }//Open , Scheduled , In Progress , Resolved
        public int? issue_caused_id { get; set; }// Osha , NEC , Thermal etc
        public int? priority { get; set; }// low , medium , high

        [ForeignKey("AssetFormIO")]
        public Guid? asset_form_id { get; set; } // if issue is linked to inspection woline

        [ForeignKey("WOOnboardingAssets")]
        public Guid? woonboardingassets_id { get; set; }  // if issue is linked to obwoline
        public string issue_title { get; set; }
        public string issue_description { get; set; }
        public string field_note { get; set; }
        public string back_office_note { get; set; }
        public string resolve_issue_reason { get; set; }
        public bool is_issue_linked { get; set; }
        [ForeignKey("Sites")]
        public Guid? site_id { get; set; }
        [ForeignKey("WorkOrders")]
        public Guid? wo_id { get; set; }
        [ForeignKey("WOLineIssue")]
        public Guid? wo_line_issue_id { get; set; } // temp issue from which this issue is created
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public Sites Sites { get; set; }
        public AssetFormIO AssetFormIO { get; set; }
        public WorkOrders WorkOrders { get; set; }
        public WOOnboardingAssets WOOnboardingAssets { get; set; }
        public Asset Asset { get; set; }
        public WOLineIssue WOLineIssue { get; set; }
        public StatusMaster StatusMaster { get; set; }
        public ICollection<AssetIssueComments> AssetIssueComments { get; set; }
        public ICollection<AssetIssueImagesMapping> AssetIssueImagesMapping { get; set; }
    }
}
