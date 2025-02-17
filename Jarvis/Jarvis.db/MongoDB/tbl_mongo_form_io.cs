using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.MongoDB
{
    [BsonIgnoreExtraElements]
    public class tbl_mongo_form_io
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string asset_form_name { get; set; }
        public string asset_form_type { get; set; }
        public string asset_form_description { get; set; }
        public BsonDocument asset_form_data { get; set; }
        public string asset_id { get; set; }
        public string site_id { get; set; }
        public DateTime created_Date { get; set; }
        public int status { get; set; }
    }
}
