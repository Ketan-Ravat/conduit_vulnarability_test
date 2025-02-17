using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.MongoDB
{
    public interface IMongoDBContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
