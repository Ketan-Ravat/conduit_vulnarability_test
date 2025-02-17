using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public partial class Sites
    {
        public Sites()
        {
            UserSites = new HashSet<UserSites>();
        }

        [Key]
        public Guid site_id { get; set; }

        [ForeignKey("Company")]
        public Guid company_id { get; set; }

        public string site_name { get; set; }

        public string site_code { get; set; }
         
        public string location { get; set; }

        [ForeignKey("StatusMaster")]
        public Nullable<int> status { get; set; }

        [ForeignKey("SiteContact")]

        public Guid? sitecontact_id { get; set; }

        public bool isAutoApprove { get; set; }
        public bool isAddAssetClassEnabled { get; set; }

        public bool showHideApprove { get; set; }

        public bool isManagerNotes { get; set; }

        public string timezone { get; set; }

        public string city { get; set; }

        public string state {  get; set; }

        public string zip { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public string customer { get; set; }
        public string customer_address { get; set; }

        public string customer_address_2 { get; set; }
        public string profile_image { get; set; }

        [ForeignKey("ClientCompany")]
        public Guid? client_company_id { get; set; }
        public virtual ClientCompany ClientCompany { get; set; }
        public virtual Company Company  { get; set; }

        public virtual SiteContact SiteContact { get; set; }
        
        public virtual ICollection<UserSites> UserSites { get; set; }

        public virtual ICollection<Asset> Asset { get; set; }

        public virtual ICollection<Inspection> Inspection { get; set; }
        public virtual ICollection<Issue> Issue { get; set; }
        public virtual ICollection<AssetFormIO> AssetFormIO { get; set; }
        public virtual ICollection<SiteProjectManagerMapping> SiteProjectManagerMapping { get; set; }
        public virtual StatusMaster StatusMaster { get; set; }

        public virtual ClusterDiagramPDFSiteMapping ClusterDiagramPDFSiteMapping { get; set; }

    }
}
