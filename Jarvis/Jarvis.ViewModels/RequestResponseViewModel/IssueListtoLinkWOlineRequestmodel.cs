using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class IssueListtoLinkWOlineRequestmodel
    {
        public Guid? asset_id { get; set; }
        public Guid? wo_id { get; set; }
        public Guid? issues_temp_asset_id { get; set; }

    }
}
