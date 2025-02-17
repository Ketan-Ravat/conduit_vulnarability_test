using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class CreateUpdateContactRequestModel
    {
        public Guid? contact_id { get; set; }
        public string name { get; set; }
        public int category_id { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string notes { get; set; }
        public bool mark_as_primary { get; set; }//true = primary , false = not primary
        public Guid vendor_id { get; set; }
        public bool is_deleted { get; set; }
       
    }
}
