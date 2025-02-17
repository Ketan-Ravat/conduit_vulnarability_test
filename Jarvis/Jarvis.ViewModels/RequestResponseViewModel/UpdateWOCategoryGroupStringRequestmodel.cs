using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateWOCategoryGroupStringRequestmodel
    {
        public string group_string { get; set; }
        public List<Guid> wo_inspectionsTemplateFormIOAssignment_id { get;set; }

    }
}
