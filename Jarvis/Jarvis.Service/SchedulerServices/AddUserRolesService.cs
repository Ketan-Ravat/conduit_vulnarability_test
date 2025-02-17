using Jarvis.Repo;
using Jarvis.Service.Abstract;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jarvis.Service.SchedulerServices
{
    public class AddUserRolesService : BackgroundService
    {
        public readonly IUserService userservice;

        public readonly IJarvisUOW _UoW;

        private Shared.Utility.Logger _logger;

        public AddUserRolesService(IUserService _userservice)
        {
            _UoW = new JarvisUOW();
            this.userservice = _userservice;
            _logger = Shared.Utility.Logger.GetInstance<AddUserRolesService>();
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await userservice.AddUserRoles(cancellationToken);
                break;
            }
        }
    }
}
