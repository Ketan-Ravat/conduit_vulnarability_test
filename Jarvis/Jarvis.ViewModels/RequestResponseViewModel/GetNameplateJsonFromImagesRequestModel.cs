using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetNameplateJsonFromImagesRequestModel
    {
        public string assetClassCode { get; set; }
        public List<string> assetNameplateImagePaths { get; set; }
        public Guid? assetId { get; set; }
        public Guid? woonboardingassetsId { get; set; }
    }


    public class GetNameplateJsonFromImagesResponseModel
    {
        public int status { get; set; }
        public string nameplate_json { get; set; }
    }


    public class Data_Anayse_API
    {
        public string assetClassCode { get; set; }
        public List<string> assetNameplateImagePaths { get; set; }
        public string taskStatus { get; set; }
        public int taskId { get; set; }
        public string taskStatusUrl { get; set; }
    }

    public class Root_Anayse_API
    {
        public Data_Anayse_API data { get; set; }
        public int status_code { get; set; }
    }

    public class Data_Status_API
    {
        public string status { get; set; }
        public Result_Status_API? result { get; set; }
        public int id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? modified_at { get; set; }
    }

    public class Result_Status_API
    {
        public string assetClassCode { get; set; }
        public List<string> assetNameplateImagePaths { get; set; }
        public int taskId { get; set; }
        public string taskStatusUrl { get; set; }
        public string error { get; set; }
        public object? nameplateJson { get; set; }
    }

    public class Root_Status_API
    {
        public Data_Status_API? data { get; set; }
        public int status_code { get; set; }
    }

}
