using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public partial class Company
    {

        public Company()
        {
            Sites = new HashSet<Sites>();
            InspectionForms = new HashSet<InspectionForms>();
        }

        [Key]
        public Guid company_id { get; set; }

        public string company_name { get; set; }

        public string company_code { get; set; }
        public string domain_name { get; set; }

        public string identity_pool_id { get; set; }

        public string region { get; set; }

        public string user_pool_id { get; set; }

        public string user_pool_web_client_id { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }
        public string company_logo { get; set; }
        public string company_thumbnail_logo { get; set; }
        public string modified_by { get; set; }

        #region for autodesk project keys
        public string project_id { get; set; }
        public string autodesk_app_name { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string redirect_uri { get; set; }

        #endregion for autodesk project keys

        [ForeignKey("StatusMaster")]
        public Nullable<int> status { get; set; }
        public bool isCalibrationDateEnabled { get; set; }
        public int? cognito_mfa_timer { get; set; }
        public virtual ICollection<Sites> Sites { get; set; }
        public virtual ICollection<InspectionForms> InspectionForms { get; set; }
        public virtual ICollection<InspectionFormTypes> InspectionFormTypes { get; set; }
        public virtual StatusMaster StatusMaster { get; set; }
        public virtual ICollection<ClientCompany> ClientCompany { get; set; }
        public virtual ICollection<RateSheet> RateSheet { get; set; }
        public virtual ICollection<CompanyFeatureMapping> CompanyFeatureMapping { get; set; }

        public virtual ICollection<SiteContact> SiteContact { get; set; }
    }
}
