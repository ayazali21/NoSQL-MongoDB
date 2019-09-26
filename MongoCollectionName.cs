using System;

namespace Platform.NoSql.MongoDB
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class MongoCollectionName : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the CollectionName class attribute with the desired name.
        /// </summary>
        /// <param name="value">Name of the collection.</param>
        public MongoCollectionName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Empty collectionname not allowed", "value");
            Name = value;
        }
        public virtual string Name { get; private set; }
    }
}
