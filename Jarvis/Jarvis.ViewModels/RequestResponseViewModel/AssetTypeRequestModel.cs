using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssetTypeRequestModel
    {
        public int asset_type_id { get; set; }
        public string name { get; set; }
        public Guid company_id { get; set; }
        public bool isArchive { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
    }
}
