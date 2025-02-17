using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jarvis
{
    public class MongoDBDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        //public string BooksCollectionName { get; set; } = null!;
    }
}
