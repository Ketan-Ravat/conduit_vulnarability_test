using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class DashboardPendingPMItems
    {
        public Guid pm_trigger_id { get; set; }
        public string title { get; set; }
        public string internal_asset_id { get; set; }
        public string asset_name { get; set; }
        public string pm_plan_name { get; set; }
        public string site_name { get; set; }
        public string due_in { get; set; }
        public int total_est_time_hours { get; set; }
        public int total_est_time_minutes { get; set; }
        public Guid asset_id { get; set; }
        public Guid asset_pm_id { get; set; }
        public int status { get; set; }
        public string status_name { get; set; }
        public int asset_pm_status { get; set; }
        public string asset_pm_status_name { get; set; }
        public AssetPMResponseModel AssetPMs { get; set; }
        public List<PMTriggersTasksResponseModel> PMTriggersTasks { get; set; }
    }

    public class UpComingPMs
    {
        public DateTime date { get; set; }
        public int pmCount { get; set; }
        public List<UpcomingPMsWeekly> upcomingPMs { get; set; }
        public UpComingPMs()
        {
            upcomingPMs = new List<UpcomingPMsWeekly>();
        }
    }

    public class UpcomingPMsWeekly {
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public int pmCount { get; set; }
    }

}
