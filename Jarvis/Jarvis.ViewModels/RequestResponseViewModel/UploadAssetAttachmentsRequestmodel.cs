using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UploadAssetAttachmentsRequestmodel
    {
        public string asset_id { get; set; }
        public string site_id { get; set; }
        public List<s3uploaded_files> S3_files { get; set; }
    }
    public class s3uploaded_files
    {
        public string user_uploaded_file_name { get; set; }
        public string s3_file_name { get; set; }
    }
}
