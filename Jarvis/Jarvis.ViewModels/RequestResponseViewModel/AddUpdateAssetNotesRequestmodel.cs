using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddUpdateAssetNotesRequestmodel
    {
        public Guid? asset_notes_id { get; set; }
        public Guid asset_id { get; set; }
        public string asset_note { get; set; }
        public bool is_deleted { get; set; }
    }
}
