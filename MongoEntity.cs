using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Platform.NoSql.MongoDB
{
    /// <summary>
    /// Abstract Entity for all the Mongo Entities.
    /// </summary>
    [Serializable]
    [BsonIgnoreExtraElements(Inherited = true)]
    public class MongoEntity
    {
        public MongoEntity()
        {
            CreatedOn = DateTime.Now;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string ObjectId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedOn { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedOn { get; set; }

        private bool _isDeleted;
        public bool IsDeleted { get => (bool?)_isDeleted ?? false; set => _isDeleted = value; }

    }
}
