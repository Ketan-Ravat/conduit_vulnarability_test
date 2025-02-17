using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class UpdateUserStatusRequestModel
    {
        public string userid { get; set; }

        public int status { get; set; }

        //public string updatedby { get; set; }
    }
}
