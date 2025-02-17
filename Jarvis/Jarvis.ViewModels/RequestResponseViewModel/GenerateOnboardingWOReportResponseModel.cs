using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GenerateOnboardingWOReportResponseModel
    {
        public int status { get; set; }
        public string report_url { get; set; }
    }
    public class RootObj_Status_API
    {
        public DataObj_Status_API? data { get; set; }
        public int status_code { get; set; }
    }
    public class DataObj_Status_API
    {
        public string status { get; set; }
        public string result { get; set; }
    }
    public class RootObj_GenerateReport_API
    {
        public DataObj_GenerateReport_API? data { get; set; }
        public int status_code { get; set; }
    }
    public class DataObj_GenerateReport_API
    {
        public string work_order_id { get; set; }
        public int taskId { get; set; }
        public string taskStatusUrl { get; set; }
    }
    public class RootObj2_Status_API
    {
        public DataObj2_Status_API? data { get; set; }
        public int status_code { get; set; }
    }
    public class DataObj2_Status_API
    {
        public string status { get; set; }
        public ResultObj2_Status_API result { get; set; }
    }
    public class ResultObj2_Status_API
    {
        public string message { get; set; }
        public string result { get; set; }
    }
}
