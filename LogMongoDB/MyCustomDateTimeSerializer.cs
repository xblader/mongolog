using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogMongoDB
{
    public class MyCustomDateTimeSerializer : StructSerializerBase<DateTime>, IRepresentationConfigurable<MyCustomDateTimeSerializer>
    {
        // private constants
        private static class Flags
        {
            public const long DateTime = 1;
            public const long Ticks = 2;
        }

        // private static fields
        private static readonly MyCustomDateTimeSerializer __dateOnlyInstance = new MyCustomDateTimeSerializer(true);
        private static readonly MyCustomDateTimeSerializer __localInstance = new MyCustomDateTimeSerializer(DateTimeKind.Local);
        private static readonly MyCustomDateTimeSerializer __utcInstance = new MyCustomDateTimeSerializer(DateTimeKind.Utc);

        // private fields
        private readonly bool _dateOnly;
        private readonly SerializerHelper _helper;
        private readonly Int64Serializer _int64Serializer = new Int64Serializer();
        private readonly Int32Serializer _int32Serializer = new Int32Serializer();
        private readonly DateTimeKind _kind;
        private readonly BsonType _representation;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeSerializer"/> class.
        /// </summary>
        public MyCustomDateTimeSerializer()
            : this(DateTimeKind.Utc, BsonType.DateTime)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeSerializer"/> class.
        /// </summary>
        /// <param name="dateOnly">if set to <c>true</c> [date only].</param>
        public MyCustomDateTimeSerializer(bool dateOnly)
            : this(dateOnly, BsonType.DateTime)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeSerializer"/> class.
        /// </summary>
        /// <param name="dateOnly">if set to <c>true</c> [date only].</param>
        /// <param name="representation">The representation.</param>
        public MyCustomDateTimeSerializer(bool dateOnly, BsonType representation)
            : this(dateOnly, DateTimeKind.Utc, representation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeSerializer"/> class.
        /// </summary>
        /// <param name="representation">The representation.</param>
        public MyCustomDateTimeSerializer(BsonType representation)
            : this(DateTimeKind.Utc, representation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeSerializer"/> class.
        /// </summary>
        /// <param name="kind">The kind.</param>
        public MyCustomDateTimeSerializer(DateTimeKind kind)
            : this(kind, BsonType.DateTime)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeSerializer"/> class.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="representation">The representation.</param>
        public MyCustomDateTimeSerializer(DateTimeKind kind, BsonType representation)
            : this(false, kind, representation)
        {
        }

        private MyCustomDateTimeSerializer(bool dateOnly, DateTimeKind kind, BsonType representation)
        {
            switch (representation)
            {
                case BsonType.DateTime:
                case BsonType.Document:
                case BsonType.Int64:
                case BsonType.String:
                    break;

                default:
                    var message = string.Format("{0} is not a valid representation for a DateTimeSerializer.", representation);
                    throw new ArgumentException(message);
            }

            _dateOnly = dateOnly;
            _kind = kind;
            _representation = representation;

            _helper = new SerializerHelper
            (
                new SerializerHelper.Member("DateTime", Flags.DateTime),
                new SerializerHelper.Member("Ticks", Flags.Ticks)
            );
        }

        // public static properties
        /// <summary>
        /// Gets an instance of DateTimeSerializer with DateOnly=true.
        /// </summary>
        public static MyCustomDateTimeSerializer DateOnlyInstance
        {
            get { return __dateOnlyInstance; }
        }

        /// <summary>
        /// Gets an instance of DateTimeSerializer with Kind=Local.
        /// </summary>
        public static MyCustomDateTimeSerializer LocalInstance
        {
            get { return __localInstance; }
        }

        /// <summary>
        /// Gets an instance of DateTimeSerializer with Kind=Utc.
        /// </summary>
        public static MyCustomDateTimeSerializer UtcInstance
        {
            get { return __utcInstance; }
        }

        // public properties
        /// <summary>
        /// Gets whether this DateTime consists of a Date only.
        /// </summary>
        public bool DateOnly
        {
            get { return _dateOnly; }
        }

        /// <summary>
        /// Gets the DateTimeKind (Local, Unspecified or Utc).
        /// </summary>
        public DateTimeKind Kind
        {
            get { return _kind; }
        }

        /// <summary>
        /// Gets the external representation.
        /// </summary>
        /// <value>
        /// The representation.
        /// </value>
        public BsonType Representation
        {
            get { return _representation; }
        }

        BsonType IRepresentationConfigurable.Representation => throw new NotImplementedException();

        // public methods
        /// <summary>
        /// Deserializes a value.
        /// </summary>
        /// <param name="context">The deserialization context.</param>
        /// <param name="args">The deserialization args.</param>
        /// <returns>A deserialized value.</returns>
        public override DateTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;

            bsonReader.ReadStartDocument();
            DateTime value = new BsonDateTime(bsonReader.ReadDateTime("DateTime")).ToLocalTime();
            
            //int day = bsonReader.ReadInt32("Day");
            //int month = bsonReader.ReadInt32("Month");
            //int year = bsonReader.ReadInt32("Year");
            
            bsonReader.ReadEndDocument();

            return value;
        }

        /// <summary>
        /// Serializes a value.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="args">The serialization args.</param>
        /// <param name="value">The object.</param>
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime value)
        {
            var bsonWriter = context.Writer;

            var millisecondsSinceEpoch = BsonUtils.ToMillisecondsSinceEpoch(value.ToLocalTime());

            bsonWriter.WriteStartDocument();
            bsonWriter.WriteDateTime("DateTime", millisecondsSinceEpoch);
            bsonWriter.WriteInt32("Day", value.Day);
            bsonWriter.WriteInt32("Month", value.Month);
            bsonWriter.WriteInt32("Year", value.Year);
            bsonWriter.WriteEndDocument();
        }

        /// <summary>
        /// Returns a serializer that has been reconfigured with the specified dateOnly value.
        /// </summary>
        /// <param name="dateOnly">if set to <c>true</c> the values will be required to be Date's only (zero time component).</param>
        /// <returns>
        /// The reconfigured serializer.
        /// </returns>
        public MyCustomDateTimeSerializer WithDateOnly(bool dateOnly)
        {
            if (dateOnly == _dateOnly)
            {
                return this;
            }
            else
            {
                return new MyCustomDateTimeSerializer(dateOnly, _representation);
            }
        }

        /// <summary>
        /// Returns a serializer that has been reconfigured with the specified dateOnly value and representation.
        /// </summary>
        /// <param name="dateOnly">if set to <c>true</c> the values will be required to be Date's only (zero time component).</param>
        /// <param name="representation">The representation.</param>
        /// <returns>
        /// The reconfigured serializer.
        /// </returns>
        public MyCustomDateTimeSerializer WithDateOnly(bool dateOnly, BsonType representation)
        {
            if (dateOnly == _dateOnly && representation == _representation)
            {
                return this;
            }
            else
            {
                return new MyCustomDateTimeSerializer(dateOnly, representation);
            }
        }

        /// <summary>
        /// Returns a serializer that has been reconfigured with the specified DateTimeKind value.
        /// </summary>
        /// <param name="kind">The DateTimeKind.</param>
        /// <returns>
        /// The reconfigured serializer.
        /// </returns>
        public MyCustomDateTimeSerializer WithKind(DateTimeKind kind)
        {
            if (kind == _kind && _dateOnly == false)
            {
                return this;
            }
            else
            {
                return new MyCustomDateTimeSerializer(kind, _representation);
            }
        }

        /// <summary>
        /// Returns a serializer that has been reconfigured with the specified DateTimeKind value and representation.
        /// </summary>
        /// <param name="kind">The DateTimeKind.</param>
        /// <param name="representation">The representation.</param>
        /// <returns>
        /// The reconfigured serializer.
        /// </returns>
        public MyCustomDateTimeSerializer WithKind(DateTimeKind kind, BsonType representation)
        {
            if (kind == _kind && representation == _representation && _dateOnly == false)
            {
                return this;
            }
            else
            {
                return new MyCustomDateTimeSerializer(kind, representation);
            }
        }

        /// <summary>
        /// Returns a serializer that has been reconfigured with the specified representation.
        /// </summary>
        /// <param name="representation">The representation.</param>
        /// <returns>The reconfigured serializer.</returns>
        public MyCustomDateTimeSerializer WithRepresentation(BsonType representation)
        {
            if (representation == _representation)
            {
                return this;
            }
            else
            {
                if (_dateOnly)
                {
                    return new MyCustomDateTimeSerializer(_dateOnly, representation);
                }
                else
                {
                    return new MyCustomDateTimeSerializer(_kind, representation);
                }
            }
        }

        IBsonSerializer IRepresentationConfigurable.WithRepresentation(BsonType representation)
        {
            throw new NotImplementedException();
        }
    }
}