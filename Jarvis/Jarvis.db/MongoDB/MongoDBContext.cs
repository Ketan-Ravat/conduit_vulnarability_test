using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Jarvis.db.MongoDB
{
    public class MongoDBContext 
    {
        private readonly IMongoDatabase _db;

        public MongoDBContext(IMongoClient client, string dbName)
        {
            _db = client.GetDatabase("form-portal");
        }

        public IMongoCollection<tbl_mongo_form_io> tbl_mongo_form_io => _db.GetCollection<tbl_mongo_form_io>("tbl_mongo_form_io");
    }
}
