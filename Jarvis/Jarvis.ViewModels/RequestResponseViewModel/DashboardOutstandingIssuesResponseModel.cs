using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class DashboardOutstandingIssuesResponseModel
    {
        //public Guid asset_id { get; set; }
        //public string asset_name { get; set; }
        //public int not_ok_count { get; set; }
        public List<Report> reports { get; set; }
    }

    public class Report
    {
        public Guid? site_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime modified_at { get; set; }
        public List<Asset_Details> asset_details { get; set; }
    }

    public class Asset_Details
    {
        public DateTime? modified_at { get; set; }
        public DateTime? created_at { get; set; }
        public Guid? site_id { get; set; }
        public Guid? asset_id { get; set; }
        public string? site_name { get; set; }
        public string? asset_name { get; set; }
        public string? internal_asset_id { get; set; }
        public List<ReportJsonDatas> asset { get; set; }
        //public DateTime? not_ok_since { get; set; }
        //public string? attribute_name { get; set; }

        public Asset_Details()
        {
            asset = new List<ReportJsonDatas>();
        }
    }

    public class ReportJsonDatas
    {
        public Guid? site_id { get; set; }

        public Guid? asset_id { get; set; }

        public string? internal_asset_id { get; set; }

        public string? site_name { get; set; }

        public string? asset_name { get; set; }

        public Guid? attribute_id { get; set; }

        public DateTime? not_ok_since { get; set; }

        public string? attribute_name { get; set; }

        public string time_elapsed { get; set; }
    }
}
