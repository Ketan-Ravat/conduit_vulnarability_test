using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface INotificationService
    {
        Task<bool> SendNotification(int NotificationStatusID, string ref_id, string userid);
        Task<bool> SendNotificationGenericNewFlow(int notification_type, List<string>? ref_id_list, List<string>? userid_list);
    }
}
