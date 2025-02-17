using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class LambdaAssetFornioInspectionReportRequestmodel
    {
        public string company_code { get; set; }
        public string asset_form_id { get; set; }
        public string pdf_file_name { get; set; }
        public string manual_wo_number { get; set; }
        public string aws_access_key { get; set; }
        public string aws_secret_key { get; set; }
        public string s3_bucket_name { get; set; }
        public double aspectRatio { get; set; }
        public string siteName { get; set; }
        public string clientCompany { get; set; }
        public string company { get; set; }
        public string logoUrl { get; set; }
        public string pdf_report_template_url { get; set; }
       /* public string wo_number { get; set; }
        public string wo_type { get; set; }
        public string inspected_by { get; set; }
        public string accepted_by { get; set; }
        public string Requested_datetime { get; set; }
        public string Accepted_datetime { get; set; }
        public string company_name { get; set; }
        public string site_name { get; set; }
        public string client_company_name { get; set; }
        public Guid asset_form_id { get; set; }*/
    }
}
