using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Service.Abstract
{
    public interface IJwtHandler
    {
        bool ValidateToken(string _publicKey, string token);
    }
}
