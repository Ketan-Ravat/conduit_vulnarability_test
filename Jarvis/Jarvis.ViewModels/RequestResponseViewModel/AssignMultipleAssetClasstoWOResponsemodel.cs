using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssignMultipleAssetClasstoWOResponsemodel
    {
        public int success { get; set; }    
        public string assset_class { get;set; }

        public Guid? asset_form_id { get; set; }
    }
}
