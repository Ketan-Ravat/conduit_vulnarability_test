using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class UpdateUserPasswordRequestModel
    {
        /// <summary>
        /// Enter Token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// Enter New Password 
        /// </summary>
        public string password { get; set; }
    }
}
