using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Shared.Helper
{
    public class IssueAssetConditionHelper
    {
        public List<IssueStatus1> IssueStatus { get; set; }
    }
    public class AssetConditions
    {
        public int asset_condition_id { get; set; }
        public int thermal_appliance_id { get; set; }
        public int compliance_id { get; set; }
        public int? asset_condition { get; set; }
        public int? thermal_appliance { get; set; }
    }

    public class IssueStatus1
    {
        public int status_id { get; set; }
        public string status_name { get; set; }
        public List<IssueType> issue_type { get; set; }
    }

    public class IssueType
    {
        public int issue_type_id { get; set; }
        public string issue_type_name { get; set; }
        public AssetConditions asset_conditions { get; set; }
    }
}
