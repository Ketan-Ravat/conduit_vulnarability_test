using Jarvis.Repo;
using Jarvis.Service.Abstract;
using Jarvis.Shared.StatusEnums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Services
{
    public class ValidateUser
    {
        public readonly IJarvisUOW _UoW;

        private Shared.Utility.Logger _logger;

        public ValidateUser()
        {
            _UoW = new JarvisUOW();
        }

        public int User(string userid, Guid device_uu_id)
        {
            int response = _UoW.UserRepository.CheckUserValid(userid, device_uu_id);
            return response;
        }
    }
}
