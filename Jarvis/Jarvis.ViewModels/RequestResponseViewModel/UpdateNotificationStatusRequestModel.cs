using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateNotificationStatusRequestModel
    {
        public Guid notification_id { get; set; }
        /// <summary>
        /// 1 = New
        /// 2 = Read
        /// </summary>
        public int status { get; set; }
    }
}
