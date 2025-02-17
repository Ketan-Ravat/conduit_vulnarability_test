using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class AssetTasksResponseModel
    {
        public Guid asset_task_id { get; set; }

        public Guid task_id { get; set; }

        public Guid asset_id { get; set; }

        public int status { get; set; }

        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }
        public string internal_asset_id { get; set; }
        public string asset_name { get; set; }
        public AssetsResponseModel Assets { get; set; }
    }
}
