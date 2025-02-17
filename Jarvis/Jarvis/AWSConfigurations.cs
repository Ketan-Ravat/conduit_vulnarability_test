using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jarvis
{
    public class AWSConfigurations
    {
        public string aws_access_key { get; set; }
        public string aws_secret_key { get; set; }
        public string inspection_bucket_name { get; set; }
        public string thumbnail_folder_name { get; set; }
        public int thumbnail_height { get; set; }
        public int thumbnail_width { get; set; }
        public string asset_bucket_name { get; set; }
        public string barcodes_bucket_name { get; set; }
        public string account_id { get; set; }
        public string manager_dashboard_id { get; set; }
        public string executive_dashboard_id { get; set; }
        public string role_arn { get; set; }
        public string pmattachment_bucket_name { get; set; }
        public string workorderattachment_bucket_name { get; set; }
        public string LVCB_Form_id { get; set; }
        public string IR_photos_bucket_name { get; set; }
        public string formio_pdf_bucket { get; set; }
        public string issue_photos_bucket { get; set; }
        public string asset_attachment { get; set; }
        public string S3_aws_access_key { get; set; }
        public string S3_aws_secret_key { get; set; }
        public string textract_aws_access_key { get; set; }
        public string textract_aws_secret_key { get; set; }
        public string sqs_aws_access_key { get; set; }
        public string sqs_aws_secret_key { get; set; }
        public string quicksight_aws_access_key { get; set; }
        public string quicksight_aws_secret_key { get; set; }
        public string cognito_resend_aws_access_key { get; set; }
        public string cognito_resend_aws_secret_key { get; set; }

        public string conduit_dev_digital_pdf_bucket { get; set; }
        public string conduit_dev_userprofile_bucket { get; set; }
        public string conduit_siteprofile_bucket { get; set; }
        public string pm_overview_dashboard_id { get; set; }
        public string business_overview_dashboard_id { get; set; }
        public string asset_overview_dashboard_id { get; set; }
        public string nfpa_70b_compliance_dashboard_id { get; set; }
        public string nfpa_70e_compliance_dashboard_id { get; set; }
        public string offline_sync_bucket { get; set; }
        public string mobile_logs_bucket { get; set; }
        public string jinja_report_template_bucket { get; set; }
        public string sitedocument_bucket { get; set; }

    }
}
