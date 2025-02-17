using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class Res_ErrorMessage
    {
        public Res_ErrorMessage()
        {

        }
        public Res_ErrorMessage(string title, string error)
        {
            Error = error;
            Title = title;
        }
        public String Error { get; set; }

        public String Title { get; set; }
    }
}
