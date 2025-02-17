using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.ExcludePropertiesfromDBHelper
{
    public class AssetlocationHierarchyExclude
    {
        public string room_name { get; set; }
        public int room_id { get; set; }
        public string floor_name { get; set; }
        public string building_name { get; set; }
        public int building_id { get; set; }
        public int floor_id { get; set; }
    }
}
