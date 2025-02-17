using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class CopyFieldsFromFormRequestmodel
    {
        public Guid copy_from_wOcategorytoTaskMapping_id { get; set; }
        public List<Guid> copy_to_wOcategorytoTaskMapping_id { get; set; }
        public List<string> containers { get; set; }
    }
}
