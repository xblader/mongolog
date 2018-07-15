using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogMongoDB
{
    public class LogErrorEntry
    {
        [BsonId]
        public ObjectId MyKey { get; set; }
        public string ID { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}