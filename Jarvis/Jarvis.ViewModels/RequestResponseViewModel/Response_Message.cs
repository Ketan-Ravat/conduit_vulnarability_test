using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class Response_Message
    {
        public Response_Message()
        {

        }
        public Response_Message(int _success, Object _data)
        {
            success = _success;
            data = _data;
        }

        public long success { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public bool is_optional_update { get; set; } = GenericRequestModel.is_optional_update;
        public string store_app_version { get; set; } = GenericRequestModel.store_app_version;
        //public object requestData { get; set; }
    }
}
