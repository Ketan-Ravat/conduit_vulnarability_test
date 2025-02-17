using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class GetAssetInspectionReportResponseModel
    {
        public List<MonthlyInspection> inspection { get; set; }
    }

    public class MonthlyInspection
    {
        //public int month { get; set; }

        public string yard { get; set; }

        public int totalinspection { get; set; }

        public int approvedinspection { get; set; }
    }

    public class WeeklyInspection
    {
        //public string week { get; set; }

        public string yard { get; set; }

        public string equipment_type { get; set; }

        public int totalinspection { get; set; }

        public double approvedinspection { get; set; }
    }

    public class AssetWeeklyReport
    {
        //public string week { get; set; }

        public string yard { get; set; }

        public string internal_asset_id { get; set; }

        public string asset_name { get; set; }

        public string begin_hours { get; set; }

        public string end_hours { get; set; }

        public int totalinspection { get; set; }

        public double approvedinspection { get; set; }
    }
}
