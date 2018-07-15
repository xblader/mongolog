using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace LogMongoDB
{
    public class MongoAdapter
    {
        protected virtual string GetConnectionString(string ConnectionStringName = null, string ConnectionString = null)
        {
            ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            return connectionStringSetting != null ? connectionStringSetting.ConnectionString : ConnectionString;
        }

        public virtual IMongoCollection<LogErrorEntry> GetCollection(string CollectionName)
        {
            IMongoDatabase db = GetDatabase();
            IMongoCollection<LogErrorEntry> collection = db.GetCollection<LogErrorEntry>(CollectionName ?? "logs");
            return collection;
        }

        protected virtual IMongoDatabase GetDatabase()
        {
            string connectionString = GetConnectionString(ConnectionStringName: "logerror");

            MongoUrl url = MongoUrl.Create(connectionString);
            MongoClient client = new MongoClient(url);

            IMongoDatabase db = client.GetDatabase(url.DatabaseName ?? "log4net");
            return db;
        }
    }
}