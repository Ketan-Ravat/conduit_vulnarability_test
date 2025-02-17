using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class S3UploadAssetAttachmentResponsemodel
    {
        public string s3_file_name { get; set; }
        public string user_uploaded_file_name { get; set; }
    }
}
