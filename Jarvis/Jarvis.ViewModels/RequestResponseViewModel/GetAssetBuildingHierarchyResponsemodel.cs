using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetBuildingHierarchyResponsemodel
    {
        public List<Building> building { get; set; }

    }
    public class Building
    {
        public int formiobuilding_id { get; set; }
        public string formio_building_name { get; set; }
        public List<floor> floor { get; set; }
    }
    public class floor
    {
        public int formiofloor_id { get; set; }
        public string formio_floor_name { get; set; }
        public List<rooms> rooms { get; set; }
    }
    public class rooms {

        public int formioroom_id { get; set; }
        public string formio_room_name { get; set; }
        public List<section> section { get; set; }
    }
    public class section
    {
        public int formiosection_id { get; set; }
        public string formio_section_name { get; set; }
        public List<asset_detail> asset_detail { get; set; }
    }
    public class asset_detail
    {
        public string asset_name { get; set; }
        public string asset_internal_id { get; set; }
    }
}
