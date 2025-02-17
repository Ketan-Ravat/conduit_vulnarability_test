using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static Jarvis.Shared.Helper.FormIOObject;

namespace Jarvis.Shared.Helper
{
    public class FormioDynamicobj
    {
        public class Root
        {
            public string display { get; set; }

            [JsonProperty("components")]
            public object components { get; set; }

            [JsonProperty("data")]
            public Data data { get; set; }
        }
        public class Data
        {
            public object nameplateInformation { get; set; }
            public object visualInspection { get; set; }
            public header header { get; set; } = new header();
            public bool submit { get; set; }
        }
    }
}
