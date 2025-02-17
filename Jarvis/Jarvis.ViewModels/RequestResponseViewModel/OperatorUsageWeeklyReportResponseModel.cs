using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class OperatorUsageWeeklyReportResponseModel
    {
        public string manager_id { get; set; }
        public string manager_name { get; set; }
        public string email_id { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string callbackUrl { get; set; }
        public List<SiteWiseReport> SiteWiseReports { get; set; }
    }
    public class SiteWiseReport
    {
        public string site_id { get; set; }
        public string site_name { get; set; }
        public List<OperatorWiseReport> operatorWiseReports { get; set; }
        public List<OperatorWithoutInspection> OperatorWithoutInspectionList { get; set; }
        public OperatorWithoutInspection OperatorWithoutInspection { get; set; }
    }
    public class AssetOperatorUsageWeeklyReport
    {
        public bool isFirst { get; set; }
        public int totalCount { get; set; }
        public string yard { get; set; }

        public string internal_asset_id { get; set; }

        public string asset_name { get; set; }

        public int totalinspection { get; set; }
        public string lastinspectiondate { get; set; }
        public string operator_id { get; set; }
        public string operator_name { get; set; }
    }
    public class OperatorWithoutInspection
    {
        public string operatorsNameList { get; set; }
        public string operator_fname { get; set; }
        public string operator_lname { get; set; }
        public string operator_id { get; set; }
    }
    public class OperatorWiseReport
    {
        public string operator_fname { get; set; }
        public string operator_lname { get; set; }
        public string operator_id { get; set; }
        public List<AssetOperatorUsageWeeklyReport> OperatorUsage { get; set; }
        public int totalCount { get; set; }
    }
}
