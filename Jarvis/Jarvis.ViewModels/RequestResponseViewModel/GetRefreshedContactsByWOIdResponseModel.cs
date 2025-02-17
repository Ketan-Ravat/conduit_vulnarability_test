using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetRefreshedContactsByWOIdResponseModel
    {
        public int total_contacts_count { get; set; }
        public int accepted_contacts_count { get; set; }
        public List<Contact_Status_Model> contacts_status_list { get; set; }
    }
    public class Contact_Status_Model
    {
        public string contact_name { get; set; }
        public int? contact_invite_status { get; set; }
    }
}
