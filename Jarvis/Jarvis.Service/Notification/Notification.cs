using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Service.Notification
{
    [BsonIgnoreExtraElements]
    public class Notification
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfNull(true)]
        public string Id { get; set; }
        public long uid { get; set; }
        [BsonRepresentation(BsonType.Int32)]
        [BsonIgnoreIfNull(true)]
        public long? refId { get; set; }
        public string heading { get; set; }
        public string message { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        [BsonIgnoreIfNull(true)]
        public DateTime? createdDate { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        [BsonIgnoreIfNull(true)]
        public DateTime? sendDate { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        [BsonIgnoreIfNull(true)]
        public DateTime? executionDate { get; set; }
        public bool isPersist { get; set; }
        public int status { get; set; }
        public int userType { get; set; }
        public int notificationType { get; set; }
        //public List<Targets> targets { get; set; }
        public Custom custom { get; set; }
       
    }
}
