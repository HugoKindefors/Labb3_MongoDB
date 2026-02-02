using MongoDB.Driver;

namespace Labb3.Data
{
    public sealed class MongoContext
    {
        public IMongoDatabase Database { get; }

        public MongoContext()
        {
            var client = new MongoClient("mongodb://localhost:27017");

            Database = client.GetDatabase("VendelaMagnusson");
        }
    }
}
