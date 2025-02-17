using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetAttachmentsResponsemodel
    {
        public Guid assetatachmentmapping_id { get; set; }
        public string file_name { get; set; }
        public string file_url { get; set; }
        public string user_uploaded_file_name { get; set; }
        public Guid asset_id { get; set; }
    }
}
