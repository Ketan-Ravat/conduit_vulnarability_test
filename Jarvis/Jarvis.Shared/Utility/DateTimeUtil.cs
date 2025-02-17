using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.WebPages;

namespace Jarvis.Shared.Utility {
    public class DateTimeUtil {
        /// <summary>
        /// Gets the UTC datetime.
        /// </summary>
        /// <returns></returns>
        public static DateTime GetUTCDatetime()
        {
            DateTime currentDate = DateTime.UtcNow;
            return currentDate;
        }

        /// <summary>
        /// Gets the beforetime text.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        public static string GetBeforetimeText(DateTime datetime)
        {
            //string res = string.Empty;
            //DateTime current = DateTime.UtcNow;
            //var diff = (current - datetime);
            //int totaldays = Convert.ToInt32(diff.TotalDays);
            //if (totaldays == 0)
            //{
            //    if (Math.Floor(diff.TotalHours) > 0)
            //    {
            //        res = Convert.ToInt32(diff.TotalHours) + " hours ago";
            //    }
            //    else
            //    {
            //        res = Convert.ToInt32(diff.TotalMinutes) + " minutes ago";
            //    }
            //}
            //else if (totaldays > 0)
            //{
            //    if ((totaldays / 7) > 0)
            //    {
            //        res = (totaldays / 7) + " weeks ago";
            //    }
            //    else
            //    {
            //        res = totaldays + " days ago";
            //    }
            //}

            //return res;



            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - datetime.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "1 second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "1 minute ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "1 hour ago";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "1 month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "1 year ago" : years + " years ago";
            }

        }

        /// <summary>
        /// Gets the month year string.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        //public static List<MonthYearModel> GetMonthYearString(int year)
        //{
        //    List<MonthYearModel> mylist = new List<MonthYearModel>();
        //    for (int i = 1; i <= 12; i++)
        //    {
        //        mylist.Add(new MonthYearModel()
        //        {
        //            monthname = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
        //            monthno = i,
        //            year = year
        //        });
        //    }
        //    return mylist;
        //}

        /// <summary>
        /// Dates the format.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        public static string DateFormat(DateTime datetime)
        {
            return datetime.ToString("dd MMM yyyy hh:mm tt");
        }

        /// <summary>
        /// Dates the format.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        public static string DateFormatToShortDate(DateTime datetime)
        {
            return datetime.ToString("dd MMM yyyy");
        }

        /// <summary>
        /// Get Upcoming Due in in words
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        public static string GetDueIn(DateTime dueDate)
        {
            DateTime currentDate = DateTime.UtcNow.Date;
            if (dueDate >= currentDate)
            {
                var totalDays = (dueDate - currentDate).TotalDays;
                var totalYears = Math.Truncate(totalDays / 365);
                var totalMonths = Math.Truncate((totalDays % 365) / 30);
                var remainingDays = Math.Truncate((totalDays % 365) % 30);
                var totalWeeks = Math.Truncate((remainingDays) / 7);
                remainingDays = remainingDays - (totalWeeks * 7);

                var dueInWord = "";
                if (totalYears != 0)
                {
                    dueInWord = totalYears + (totalYears == 1 ? " year " : " years ");
                }
                if (totalMonths != 0)
                {
                    dueInWord = dueInWord + totalMonths + (totalMonths == 1 ? " month " : " months ");
                }
                if (totalWeeks != 0)
                {
                    dueInWord = dueInWord + totalWeeks + (totalWeeks == 1 ? " week " : " weeks ");
                }
                if (remainingDays > 0)
                {
                    dueInWord = dueInWord + remainingDays + (remainingDays == 1 ? " day " : " days ");
                }
                else if(remainingDays == 0 && dueInWord == string.Empty)
                {
                    dueInWord = "today";
                }
                Console.WriteLine("Due Day = " + dueInWord);
                return dueInWord;
            }
            else
            {
                const int SECOND = 1;
                const int MINUTE = 60 * SECOND;
                const int HOUR = 60 * MINUTE;
                const int DAY = 24 * HOUR;
                const int MONTH = 30 * DAY;

                var ts = new TimeSpan(DateTime.UtcNow.Ticks - dueDate.Ticks);
                double delta = Math.Abs(ts.TotalSeconds);

                if (delta < 1 * MINUTE)
                    return ts.Seconds == 1 ? "1 second ago" : ts.Seconds + " seconds ago";

                if (delta < 2 * MINUTE)
                    return "1 minute ago";

                if (delta < 45 * MINUTE)
                    return ts.Minutes + " minutes ago";

                if (delta < 90 * MINUTE)
                    return "1 hour ago";

                if (delta < 24 * HOUR)
                    return ts.Hours + " hours ago";

                if (delta < 48 * HOUR)
                    return "yesterday";

                if (delta < 30 * DAY)
                    return ts.Days + " days ago";

                if (delta < 12 * MONTH)
                {
                    int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                    return months <= 1 ? "1 month ago" : months + " months ago";
                }
                else
                {
                    int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                    return years <= 1 ? "1 year ago" : years + " years ago";
                }
            }
        }

        /// <summary>
        /// Get Due in in words
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        public static string GetDueInNewFlow(DateTime start_date , DateTime end_date)
        {
            DateTime currentDate = DateTime.UtcNow.Date;
            var totalDays = (end_date - start_date).TotalDays;
            var totalYears = Math.Truncate(totalDays / 365);
            var totalMonths = Math.Truncate((totalDays % 365) / 30);
            var remainingDays = Math.Truncate((totalDays % 365) % 30);
            var totalWeeks = Math.Truncate((remainingDays) / 7);
            remainingDays = remainingDays - (totalWeeks * 7);



            // Calculate the time difference
            TimeSpan difference = end_date.Subtract(start_date);

            // Calculate the difference in years, months, and days
            int years = end_date.Year - start_date.Year;
            int months = end_date.Month - start_date.Month;
            int days = end_date.Day - start_date.Day;

            // Adjust for negative values
            if (days < 0)
            {
                months--;
                days += DateTime.DaysInMonth(end_date.Year, start_date.Month);
            }
            if (months < 0)
            {
                years--;
                months += 12;
            }

            // Output the difference
            Console.WriteLine($"Difference: {years} years, {months} months, {days} days");
            totalYears = years;
            totalMonths = months;
            remainingDays = days;
            remainingDays = remainingDays - (totalWeeks * 7);

            var dueInWord = "";
            if (totalYears != 0)
            {
                dueInWord = totalYears + (totalYears == 1 ? " year " : " years ");
            }
            if (totalMonths != 0)
            {
                dueInWord = dueInWord + totalMonths + (totalMonths == 1 ? " month " : " months ");
            }
            if (totalWeeks != 0)
            {
                dueInWord = dueInWord + totalWeeks + (totalWeeks == 1 ? " week " : " weeks ");
            }
            if (remainingDays > 0)
            {
                dueInWord = dueInWord + remainingDays + (remainingDays == 1 ? " day " : " days ");
            }
            else if (remainingDays == 0 && dueInWord == string.Empty)
            {
                dueInWord = "today";
            }
            Console.WriteLine("Due Day = " + dueInWord);
            return dueInWord;
        }

        public static (string, bool) GetDueOverdueTimingByDueDate(DateTime due_date)
        {
            string due_in = "";
            bool is_overdue = false;
            try
            {
                if (due_date != null && due_date != DateTime.MinValue)
                {
                    var due_starting_at = due_date.Date;

                    if (due_date.Date < DateTime.UtcNow.Date)
                    {
                        due_in = "Overdue " + DateTimeUtil.GetDueInNewFlow(due_date.Date, DateTime.UtcNow.Date);
                        is_overdue = true;
                    }
                    else if (due_starting_at.Date == DateTime.UtcNow.Date)
                    {
                        due_in = "Due";
                    }
                    else
                    {
                        due_in = "Due in " + DateTimeUtil.GetDueInNewFlow(DateTime.UtcNow.Date, due_starting_at);
                    }
                }
            }catch (Exception e)
            {
            }

            return (due_in, is_overdue);
        }
    }
}
