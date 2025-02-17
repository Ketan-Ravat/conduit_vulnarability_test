using Jarvis.db.Models;
using Jarvis.Service.Resources;
using Jarvis.Shared.StatusEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Service.Notification
{
    public class NotificationGenerator
    {
        public static Notification AutoApproedInspection(string asset_name, string operator_name)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.AutoApproveInspectionHeader;
            notification.message = NotificationMessages.AutoApproveInspection;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{Operator_Name}", operator_name);
            return notification;
        }

        public static Notification AutoApproedInspectionOperator(string asset_name)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.AutoApprovedInspectionOperatorHeader;
            notification.message = NotificationMessages.AutoApprovedInspectionOperator;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            return notification;
        }

        public static Notification PendingNewInspection(string asset_name, string operator_name)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.PendingNewInspectionHeader;
            notification.message = NotificationMessages.PendingNewInspection;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{Operator_Name}", operator_name);
            return notification;
        }

        public static Notification ManagerApproveInspection(string asset_name, string manager_name)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.ManagerApproveInspectionHeader;
            notification.message = NotificationMessages.ManagerApproveInspection;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{Manager_Name}", manager_name);
            return notification;
        }

        public static Notification NewWorkOrderForInspection(string asset_name, string manager_name)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.NewIssueForInspectionHeader;
            notification.message = NotificationMessages.NewIssueForInspection;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{Manager_Name}", manager_name);
            return notification;
        }

        public static Notification WorkOrderCreated(string asset_name, string manager_name, string attribute_name)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.CreateIssueHeader;
            notification.message = NotificationMessages.CreateIssue;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{Manager_Name}", manager_name);
            notification.message = notification.message.Replace("{Attribute_Name}", attribute_name);
            return notification;
        }


        public static Notification RejecteInspection(string asset_name, string manager_name)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.RejectInspectionHeader;
            notification.message = NotificationMessages.RejectInspection;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{Manager_Name}", manager_name);
            return notification;
        }

        public static Notification NewWorkOrderWithAutoApprovedAsset(string asset_name, string manager_name)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.CreateIssueWithApprovedAssetHeader;
            notification.message = NotificationMessages.CreateIssueWithApprovedAsset;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{Manager_Name}", manager_name);
            return notification;
        }

        public static Notification UpdateWorkOrderStatus(string asset_name, string Maintenancestaff_Name, string Status, string attribute_name)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.UpdateIssueStatusHeader;
            notification.message = NotificationMessages.UpdateIssueStatus;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{Maintenancestaff_Name}", Maintenancestaff_Name);
            notification.message = notification.message.Replace("{WorkOrder_Status}", Status);
            notification.message = notification.message.Replace("{Attribute_Name}", attribute_name);
            return notification;
        }

        public static Notification UpdateWorkOrderPriority(string asset_name, string manager_name, string priority)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.UpdateIssuePriorityHeader;
            notification.message = NotificationMessages.UpdateIssuePriority;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{Manager_Name}", manager_name);
            notification.message = notification.message.Replace("{Priority}", priority);
            return notification;
        }

        public static Notification PMNotificationDueDateReminder(string asset_name, string pm_name, string time)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.PMNotificationFinalReminderHeader;
            notification.heading = notification.heading.Replace("{Asset_Name}", asset_name);
            notification.message = NotificationMessages.PMNotificationDueDateReminder;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{pm_name}", pm_name);
            notification.message = notification.message.Replace("{time}", time);
            return notification;
        }

        public static Notification PMNotificationFirstReminder(string asset_name, string time, string pm_name)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.PMNotificationFirstReminderHeader;
            notification.heading = notification.heading.Replace("{Asset_Name}", asset_name);
            notification.message = NotificationMessages.PMNotificationDueDateReminder;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{time}", time);
            notification.message = notification.message.Replace("{pm_name}", pm_name);
            return notification;
        }

        public static Notification PMNotificationSecondReminder(string asset_name, string time, string pm_name)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.PMNotificationSecondReminderHeader;
            notification.heading = notification.heading.Replace("{Asset_Name}", asset_name);
            notification.message = NotificationMessages.PMNotificationDueDateReminder;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{time}", time);
            notification.message = notification.message.Replace("{pm_name}", pm_name);
            return notification;
        }
        public static Notification PMNotificationFinalMeterHoursReminder(string asset_name, string meter_hours, string pm_name, string due_meter_hours)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.PMNotificationFinalReminderHeader;
            notification.heading = notification.heading.Replace("{Asset_Name}", asset_name);
            notification.message = NotificationMessages.PMNotificationMeterHoursReminder;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{meter_hours}", meter_hours);
            notification.message = notification.message.Replace("{due_meter_hours}", due_meter_hours);
            notification.message = notification.message.Replace("{pm_name}", pm_name);
            return notification;
        }

        public static Notification PMNotificationFirstMeterHoursReminder(string asset_name, string meter_hours, string pm_name, string due_meter_hours)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.PMNotificationFirstReminderHeader;
            notification.heading = notification.heading.Replace("{Asset_Name}", asset_name);
            notification.message = NotificationMessages.PMNotificationMeterHoursReminder;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{meter_hours}", meter_hours);
            notification.message = notification.message.Replace("{due_meter_hours}", due_meter_hours);
            notification.message = notification.message.Replace("{pm_name}", pm_name);
            return notification;
        }

        public static Notification PMNotificationSecondMeterHoursReminder(string asset_name, string meter_hours, string pm_name, string due_meter_hours)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.PMNotificationSecondReminderHeader;
            notification.message = NotificationMessages.PMNotificationMeterHoursReminder;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{meter_hours}", meter_hours);
            notification.message = notification.message.Replace("{due_meter_hours}", due_meter_hours);
            notification.message = notification.message.Replace("{pm_name}", pm_name);
            return notification;
        }

        public static Notification PMNotificationonDue(string asset_name, string pm_name)
        {
            Notification notification = new Notification();
            notification.heading = NotificationMessages.PMNotificationOnDueHeader;
            notification.message = NotificationMessages.PMNotificationOnDue;
            notification.message = notification.message.Replace("{Asset_Name}", asset_name);
            notification.message = notification.message.Replace("{pm_name}", pm_name);
            return notification;
        }

        public static AssetActivityLogs ActivityPMDue(string asset_name, string pm_name, string meter_hours)
        {
            AssetActivityLogs activityLogs = new AssetActivityLogs();
            activityLogs.activity_header = NotificationMessages.ActivityPMDueHeader;
            activityLogs.activity_message = NotificationMessages.ActivityPMDueMeterHoursMessage;
            activityLogs.activity_message = activityLogs.activity_message.Replace("{Asset_Name}", asset_name);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{pm_name}", pm_name);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{meter_hours}", meter_hours);
            activityLogs.activity_type = (int)ActivityTypes.PMDue;
            activityLogs.status = (int)Notification_Status.New;
            return activityLogs;
        }

        public static AssetActivityLogs SendPMNotificationLog(string asset_name, string pm_name, string meter_hours, int notification_type, string time ="", string due_meter_hours ="")
        {
            AssetActivityLogs activityLogs = new AssetActivityLogs();
            if(notification_type == (int)NotificationStatus.FirstDueDateReminder)
            {
                activityLogs.activity_header = NotificationMessages.PMNotificationFirstReminderHeader;
                activityLogs.activity_header = activityLogs.activity_header.Replace("{Asset_Name}", asset_name);
                activityLogs.activity_message = NotificationMessages.PMNotificationDueDateReminder;
                activityLogs.activity_message = activityLogs.activity_message.Replace("{Asset_Name}", asset_name);
                activityLogs.activity_message = activityLogs.activity_message.Replace("{time}", time);
                activityLogs.activity_message = activityLogs.activity_message.Replace("{pm_name}", pm_name);
            }
            else if (notification_type == (int)NotificationStatus.SecondDueDateReminder)
            {
                activityLogs.activity_header = NotificationMessages.PMNotificationSecondReminderHeader;
                activityLogs.activity_header = activityLogs.activity_header.Replace("{Asset_Name}", asset_name);
                activityLogs.activity_message = NotificationMessages.PMNotificationDueDateReminder;
                activityLogs.activity_message = activityLogs.activity_message.Replace("{Asset_Name}", asset_name);
                activityLogs.activity_message = activityLogs.activity_message.Replace("{time}", time);
                activityLogs.activity_message = activityLogs.activity_message.Replace("{pm_name}", pm_name);
            }
            else if (notification_type == (int)NotificationStatus.FirstMeterHoursDueReminder)
            {
                activityLogs.activity_header = NotificationMessages.PMNotificationFirstReminderHeader;
                activityLogs.activity_message = NotificationMessages.PMNotificationMeterHoursReminder;
                activityLogs.activity_message = activityLogs.activity_message.Replace("{Asset_Name}", asset_name);
                activityLogs.activity_message = activityLogs.activity_message.Replace("{meter_hours}", meter_hours);
                activityLogs.activity_message = activityLogs.activity_message.Replace("{due_meter_hours}", due_meter_hours);
                activityLogs.activity_message = activityLogs.activity_message.Replace("{pm_name}", pm_name);
            }
            else if (notification_type == (int)NotificationStatus.SecondMeterHoursDueReminder)
            {
                activityLogs.activity_header = NotificationMessages.PMNotificationSecondReminderHeader;
                activityLogs.activity_message = NotificationMessages.PMNotificationMeterHoursReminder;
                activityLogs.activity_message = activityLogs.activity_message.Replace("{Asset_Name}", asset_name);
                activityLogs.activity_message = activityLogs.activity_message.Replace("{meter_hours}", meter_hours);
                activityLogs.activity_message = activityLogs.activity_message.Replace("{due_meter_hours}", due_meter_hours);
                activityLogs.activity_message = activityLogs.activity_message.Replace("{pm_name}", pm_name);
            }
            else if (notification_type == (int)NotificationStatus.OnDueDateReminder)
            {
                activityLogs.activity_header = NotificationMessages.PMNotificationFinalReminderHeader;
                activityLogs.activity_header = activityLogs.activity_header.Replace("{Asset_Name}", asset_name);
                activityLogs.activity_message = NotificationMessages.PMNotificationDueDateReminder;
                activityLogs.activity_message = activityLogs.activity_message.Replace("{Asset_Name}", asset_name);
                activityLogs.activity_message = activityLogs.activity_message.Replace("{pm_name}", pm_name);
                activityLogs.activity_message = activityLogs.activity_message.Replace("{time}", time);
            }
            else if (notification_type == (int)NotificationStatus.OnMeterHoursDueReminder)
            {
                activityLogs.activity_header = NotificationMessages.PMNotificationFinalReminderHeader;
                activityLogs.activity_header = activityLogs.activity_header.Replace("{Asset_Name}", asset_name);
                activityLogs.activity_message = NotificationMessages.PMNotificationMeterHoursReminder;
                activityLogs.activity_message = activityLogs.activity_message.Replace("{Asset_Name}", asset_name);
                activityLogs.activity_message = activityLogs.activity_message.Replace("{meter_hours}", meter_hours);
                activityLogs.activity_message = activityLogs.activity_message.Replace("{due_meter_hours}", due_meter_hours);
                activityLogs.activity_message = activityLogs.activity_message.Replace("{pm_name}", pm_name);
            }
            activityLogs.activity_type = (int)ActivityTypes.PMNotificationLog;
            activityLogs.status = (int)Notification_Status.New;
            return activityLogs;
        }

        public static AssetActivityLogs AssetAutoApprove(string asset_name, string meter_hours, Guid site_id)
        {
            AssetActivityLogs activityLogs = new AssetActivityLogs();
            activityLogs.activity_header = NotificationMessages.InspectionCompletedHeader;
            activityLogs.activity_message = NotificationMessages.InspectionCompletedMessage;
            activityLogs.activity_message = activityLogs.activity_message.Replace("{Asset_Name}", asset_name);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{meter_hours}", meter_hours);
            activityLogs.activity_type = (int)ActivityTypes.AssetAutoApprove;
            activityLogs.site_id = site_id;
            activityLogs.status = (int)Notification_Status.New;
            return activityLogs;
        }

        public static AssetActivityLogs AssetNewInspection(string asset_name, string meter_hours, string operator_name, Guid site_id)
        {
            AssetActivityLogs activityLogs = new AssetActivityLogs();
            activityLogs.activity_header = NotificationMessages.InspectionCreatedHeader;
            activityLogs.activity_message = NotificationMessages.InspectionCreatedMessage;
            activityLogs.activity_message = activityLogs.activity_message.Replace("{Asset_Name}", asset_name);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{operator_name}", operator_name);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{meter_hours}", meter_hours);
            activityLogs.activity_type = (int)ActivityTypes.OperatorRequestForAsset;
            activityLogs.status = (int)Notification_Status.New;
            return activityLogs;
        }

        public static AssetActivityLogs NewIssueCreated(string asset_name, string meter_hours, string issue_name)
        {
            AssetActivityLogs activityLogs = new AssetActivityLogs();
            activityLogs.activity_header = NotificationMessages.IssueCreatedHeader;
            activityLogs.activity_message = NotificationMessages.IssueCreatedMessage;
            activityLogs.activity_message = activityLogs.activity_message.Replace("{Asset_Name}", asset_name);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{meter_hours}", meter_hours);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{issue_name}", issue_name);
            activityLogs.activity_type = (int)ActivityTypes.NewIssueCreated;
            activityLogs.status = (int)Notification_Status.New;
            return activityLogs;
        }

        public static AssetActivityLogs IssueResolved(string asset_name, string meter_hours, string issue_name)
        {
            AssetActivityLogs activityLogs = new AssetActivityLogs();
            activityLogs.activity_header = NotificationMessages.IssueResolvedHeader;
            activityLogs.activity_message = NotificationMessages.IssueResolvedMessage;
            activityLogs.activity_message = activityLogs.activity_message.Replace("{Asset_Name}", asset_name);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{meter_hours}", meter_hours);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{issue_name}", issue_name);
            activityLogs.activity_type = (int)ActivityTypes.IssueResolved;
            activityLogs.status = (int)Notification_Status.New;
            return activityLogs;
        }

        public static AssetActivityLogs WorkOrdersCreated(string asset_name, string title, string workorder_type)
        {
            AssetActivityLogs activityLogs = new AssetActivityLogs();
            activityLogs.activity_header = NotificationMessages.WorkOrderCreatedHeader;
            activityLogs.activity_message = NotificationMessages.WorkOrderCreatedMessage;
            activityLogs.activity_message = activityLogs.activity_message.Replace("{Asset_Name}", asset_name);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{title}", title);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{workorder_type}", workorder_type);
            activityLogs.activity_type = (int)ActivityTypes.WorkOrderCreated;
            activityLogs.status = (int)Notification_Status.New;
            return activityLogs;
        }

        public static AssetActivityLogs WorkOrderUpdated(string asset_name, string title, string status_value)
        {
            AssetActivityLogs activityLogs = new AssetActivityLogs();
            activityLogs.activity_header = NotificationMessages.WorkOrderUpdatedHeader;
            activityLogs.activity_message = NotificationMessages.WorkOrderUpdatedMessage;
            activityLogs.activity_message = activityLogs.activity_message.Replace("{Asset_Name}", asset_name);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{title}", title);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{status_value}", status_value);
            activityLogs.activity_type = (int)ActivityTypes.WorkOrderUpdated;
            activityLogs.status = (int)Notification_Status.New;
            return activityLogs;
        }

        public static AssetActivityLogs WorkOrderDeleted(string asset_name, string title, string status_value)
        {
            AssetActivityLogs activityLogs = new AssetActivityLogs();
            activityLogs.activity_header = NotificationMessages.WorkOrderDeletedHeader;
            activityLogs.activity_message = NotificationMessages.WorkOrderDeletedMessage;
            activityLogs.activity_message = activityLogs.activity_message.Replace("{Asset_Name}", asset_name);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{title}", title);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{status_value}", status_value);
            activityLogs.activity_type = (int)ActivityTypes.WorkOrderDeleted;
            activityLogs.status = (int)Notification_Status.New;
            return activityLogs;
        }

        public static AssetActivityLogs WorkOrderIssueLink(string issue_number, string title)
        {
            AssetActivityLogs activityLogs = new AssetActivityLogs();
            activityLogs.activity_header = NotificationMessages.WorkOrderIssueLinkHeader;
            activityLogs.activity_message = NotificationMessages.WorkOrderIssueLinkMessage;
            activityLogs.activity_message = activityLogs.activity_message.Replace("{issue_number}", issue_number);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{title}", title);
            //activityLogs.activity_message = activityLogs.activity_message.Replace("{workorder_type}", workorder_type);
            activityLogs.activity_type = (int)ActivityTypes.WorkOrderIssueLink;
            activityLogs.status = (int)Notification_Status.New;
            return activityLogs;
        }

        public static AssetActivityLogs WorkOrderIssueUnLink(string issue_number, string title)
        {
            AssetActivityLogs activityLogs = new AssetActivityLogs();
            activityLogs.activity_header = NotificationMessages.WorkOrderIssueUnLinkHeader;
            activityLogs.activity_message = NotificationMessages.WorkOrderIssueUnLinkMessage;
            activityLogs.activity_message = activityLogs.activity_message.Replace("{issue_number}", issue_number);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{title}", title);
            //activityLogs.activity_message = activityLogs.activity_message.Replace("{workorder_type}", workorder_type);
            activityLogs.activity_type = (int)ActivityTypes.WorkOrderIssueUnlink;
            activityLogs.status = (int)Notification_Status.New;
            return activityLogs;
        }

        public static AssetActivityLogs WorkOrderDateUpdated(string asset_name, string title, string date_value)
        {
            AssetActivityLogs activityLogs = new AssetActivityLogs();
            activityLogs.activity_header = NotificationMessages.WorkOrderUpdatedHeader;
            activityLogs.activity_message = NotificationMessages.WorkOrderDateUpdatedMessage;
            activityLogs.activity_message = activityLogs.activity_message.Replace("{Asset_Name}", asset_name);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{title}", title);
            activityLogs.activity_message = activityLogs.activity_message.Replace("{date_value}", date_value);
            activityLogs.activity_type = (int)ActivityTypes.WorkOrderUpdated;
            activityLogs.status = (int)Notification_Status.New;
            return activityLogs;
        }
    }
}
