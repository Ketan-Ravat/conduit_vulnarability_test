using Jarvis.Shared.StatusEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Shared.Utility
{
    public static class PMTriggersUtil
    {
        public static int ConvertHoursToMinutes(int hours)
        {
            return hours * 60;
        }

        public static int ConvertMinutesToHours(int mins)
        {
            int hours = (mins - mins % 60) / 60;
            return hours;
            
        }

        public static int ConvertMinutesToHoursOfMinutes(int mins)
        {
            int hours = (mins - mins % 60) / 60;
            return (mins - hours * 60);
        }

        public static DateTime GetDateFromDueDate(int before_on, int before_on_type, DateTime due_date)
        {
            DateTime calculatedDateTime = due_date;
            if(before_on_type == (int)Status.Week)
            {
                var totaldays = before_on * 7;
                calculatedDateTime = calculatedDateTime.AddDays(-totaldays);
            }
            else if (before_on_type == (int)Status.Day)
            {
                calculatedDateTime = calculatedDateTime.AddDays(-before_on);
            }
            else if (before_on_type == (int)Status.Month)
            {
                var totaldays = before_on * 30;
                calculatedDateTime = calculatedDateTime.AddDays(-totaldays);
            }
            return calculatedDateTime;
        }
    }
}
