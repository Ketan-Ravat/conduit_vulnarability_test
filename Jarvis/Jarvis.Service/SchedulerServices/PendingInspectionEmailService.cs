using AutoMapper;
using Jarvis.Repo;
using Jarvis.Service.Abstract;
using Jarvis.ViewModels.ViewModels;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jarvis.Service.SchedulerServices
{
    public class PendingInspectionEmailService : BackgroundService
    {
        public readonly IInspectionService inspectionservices;

        public readonly IJarvisUOW _UoW;

        private Shared.Utility.Logger _logger;

        public PendingInspectionEmailService(IInspectionService _inspectionservices)
        {
            _UoW = new JarvisUOW();
            this.inspectionservices = _inspectionservices;
            _logger = Shared.Utility.Logger.GetInstance<PendingInspectionEmailService>();
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    //Time when method needs to be called
                    var DailyTime = "06:00:00";
                    var timeParts = DailyTime.Split(new char[1] { ':' });

                    DateTime dateNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles")); ;
                    var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day,
                               int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]));
                    TimeSpan ts;
                    if (date > dateNow)
                        ts = date - dateNow;
                    else
                    {
                        date = date.AddDays(1);
                        ts = date - dateNow;
                    }

                    _logger.LogError("Send Pending Email DayOfWeek: ", dateNow.DayOfWeek.ToString() + ", Date Of Today: " + dateNow.ToString());
                    if (((dateNow.DayOfWeek != DayOfWeek.Saturday) || (dateNow.DayOfWeek != DayOfWeek.Sunday)) && (dateNow.Hour == 6 && dateNow.Minute == 00))
                    {
                        _logger.LogError("Date of sending email: ", dateNow.ToString());
                        //inspectionservices.SendEmailNotificationForPendingInspection(cancellationToken);
                    }
                    await Task.Delay(ts, cancellationToken);
                }
            }
            catch (Exception e)
            {
                ApiException apiException = new ApiException(e);
                var jsonString = JsonConvert.SerializeObject(apiException);
                _logger.LogWarning("Exception in Execute Async Seduler Pending Email Service : " + jsonString);
            }
        }
    }
}
