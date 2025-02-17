using Jarvis.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IBaseService
    {
        Task<UsersitesResponseModel> UserSites(UsersitesRequestModel requestModel);
    }
}
