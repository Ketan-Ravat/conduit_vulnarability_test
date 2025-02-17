using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetformEquipmentListResponsemodel
    {
        public List<FormioEquipmentlist> equipment_list { get; set; }
    }
    public class FormioEquipmentlist
    {
        public string label { get; set; }
        public string value { get; set; }
        public string name { get; set; }
        public string serialNumber { get; set; }
        public DateTime calibrationDate { get; set; }
    }
    
}
