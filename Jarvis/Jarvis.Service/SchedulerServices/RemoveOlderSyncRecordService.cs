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
    public class RemoveOlderSyncRecordService : BackgroundService
    {
        public readonly IDeviceService deviceservices;

        public readonly IJarvisUOW _UoW;

        private Shared.Utility.Logger _logger;

        public RemoveOlderSyncRecordService(IDeviceService _deviceservice)
        {
            _UoW = new JarvisUOW();
            this.deviceservices = _deviceservice;
            _logger = Shared.Utility.Logger.GetInstance<RemoveOlderSyncRecordService>();
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

                    DateTime dateNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles"));
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

                    _logger.LogError("Scheduler for Remove Sync Record : ", dateNow.ToString());
                    if (dateNow.Hour == 6 && dateNow.Minute == 00)
                    {
                        _logger.LogError("Remove Sync Record : ", dateNow.ToString());
                        //deviceservices.RemoveSyncRecord(cancellationToken);
                    }
                    await Task.Delay(ts, cancellationToken);
                }
            }catch(Exception e)
            {
                ApiException apiException = new ApiException(e);
                var jsonString = JsonConvert.SerializeObject(apiException);
                _logger.LogWarning("Exception in Execute Async Seduler Remove Older Service : " + jsonString);
            }
        }
    }
}
