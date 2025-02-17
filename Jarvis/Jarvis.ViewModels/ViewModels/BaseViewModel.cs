using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jarvis.ViewModels
{
    public class BaseViewModel
    {
        public long result { get; set; }

        public bool success { get; set; }

        public string message { get; set; }
    }
}
