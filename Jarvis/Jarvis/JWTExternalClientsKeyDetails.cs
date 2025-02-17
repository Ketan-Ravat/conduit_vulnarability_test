using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jarvis
{
    public class JWTExternalClientsKeyDetails
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string ReferralId { get; set; }
        public string RsaPrivateKey { get; set; }
        public string RsaPublicKey { get; set; }
    }
}
