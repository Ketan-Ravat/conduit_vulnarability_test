using Jarvis.db.Migrations;
using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.DBResponseModel
{
    public class GetSyncDataResponseModel
    {
        public List<Guid> active_assets { get; set; }
        public List<Asset> assets { get; set; }

        public List<InspectionForms> inspectionforms { get; set; }

        public List<Issue> issues { get; set; }

        public List<Inspection> inspections { get; set; }

        public List<User> users { get; set; }

        public MasterData masterdata { get; set; }

        public int asset_list { get; set; }

        public int inspection_form_list { get; set; }

        public int inspections_list { get; set; }

        public int issue_list { get; set; }

        public int user_list { get; set; }
    }
}
