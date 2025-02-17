using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetNotesRequestmodel
    {
        public Guid? asset_id { get; set; }
        public int page_size { get; set; }
        public int page_index { get; set; }
    }
}
