using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetNotesResponsemodel
    {
        public Guid asset_notes_id { get; set; }
        public Guid asset_id { get; set; }
        public string asset_note { get; set; }
        public Guid asset_note_added_by_userid { get; set; }
        public string asset_note_added_by_user { get; set; }
        public DateTime created_at { get; set; }
        public bool is_deleted { get; set; }

    }
}
