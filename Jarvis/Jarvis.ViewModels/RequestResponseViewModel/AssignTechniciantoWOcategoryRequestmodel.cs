using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssignTechniciantoWOcategoryRequestmodel
    {
        //public Guid wo_inspectionsTemplateFormIOAssignment_id { get; set; }
        public Guid technician_user_id { get; set; }
        public Guid WOcategorytoTaskMapping_id { get; set; }
    }
}
