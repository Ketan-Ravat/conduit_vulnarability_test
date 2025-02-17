using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssignCategorytoWORequestmodel
    {
        public Guid form_id { get; set; }
        public Guid task_id { get; set; }
        public Guid wo_id { get; set; }
        public Guid? technician_user_id { get; set; }
    }
}
