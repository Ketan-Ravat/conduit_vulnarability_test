using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Shared.Helper
{
    public class FormioDynamicDataobj
    {
        public class Root
        {
            public string display { get; set; }

            [JsonProperty("components")]
            public object components { get; set; }

            [JsonProperty("data")]
            public object data { get; set; }
        }
    }
}
