using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssignAssetClasstoWOResponsemodel
    {
        public Guid form_id { get; set; }    
        public Guid WOcategorytoTaskMapping_id { get;set; }
        public string form_type { get; set; }  
    }
}
