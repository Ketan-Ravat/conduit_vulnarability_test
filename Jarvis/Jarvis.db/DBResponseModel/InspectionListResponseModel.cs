using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.DBResponseModel
{
    public class InspectionListResponseModel
    {
        public InspectionListResponseModel()
        {
            Inspection = new List<Inspection>();
        }

        public List<Inspection> Inspection { get; set; }

        public int list_size { get; set; }
    }
}
