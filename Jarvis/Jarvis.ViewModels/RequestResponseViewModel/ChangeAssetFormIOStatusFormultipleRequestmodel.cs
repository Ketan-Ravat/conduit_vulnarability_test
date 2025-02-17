using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ChangeAssetFormIOStatusFormultipleRequestmodel
    {
        public List<Guid> asset_form_id { get; set; }
        public int status { get; set; }

    }
}
