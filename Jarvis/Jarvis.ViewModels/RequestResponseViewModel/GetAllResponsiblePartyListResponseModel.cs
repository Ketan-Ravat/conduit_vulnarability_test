using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllResponsiblePartyListResponseModel
    {
        public List<GetAllResponsibleParty_Data> list { get; set; }
    }
    public class GetAllResponsibleParty_Data
    {
        public Guid responsible_party_id { get; set; }
        public string responsible_party_name { get; set; }
    }
}
