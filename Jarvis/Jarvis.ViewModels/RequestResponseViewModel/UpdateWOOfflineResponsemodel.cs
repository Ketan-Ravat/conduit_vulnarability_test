using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public  class UpdateWOOfflineResponsemodel
    {
        public int success { get; set; }
        public Guid? trackmobilesyncoffline_id { get; set; }
        public bool is_lambda_execution_enable { get; set; }
    }
}
