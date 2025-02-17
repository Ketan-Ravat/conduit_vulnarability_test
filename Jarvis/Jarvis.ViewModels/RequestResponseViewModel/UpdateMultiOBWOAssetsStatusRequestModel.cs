using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateMultiOBWOAssetsStatusRequestModel
    {
        public List<Guid> woonboardingassets_id_list { get; set; }
        public int? status { get; set; }
        public bool is_requested_for_delete { get; set; }
    }
}
