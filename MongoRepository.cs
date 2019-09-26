using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Platform.NoSql.MongoDB
{
    /// <summary>
    /// Deals with entities in MongoDb.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    public abstract class MongoRepository<T> : IMongoRepository<T> where T : MongoEntity
    {
        protected IMongoCollection<T> _dbCollection;
        private readonly string _ConnectionString;

        public IMongoCollection<T> DbCollection
        {
            get
            {
                if (_dbCollection == null)
                {
                    _dbCollection = Util.GetCollectionFromConnectionString<T>(_ConnectionString);
                }
                return _dbCollection;
            }
        }

        public MongoRepository(string connectionString)
        {
            _ConnectionString = connectionString;
            this._dbCollection = Util.GetCollectionFromUrl<T>(new MongoUrl(_ConnectionString));
        }

        public MongoRepository(string connectionString, string collectionName)
        {
            _ConnectionString = connectionString;
            this._dbCollection = Util.GetCollectionFromConnectionString<T>(_ConnectionString, collectionName);
        }


        public virtual T Add(T entity)
        {
            this._dbCollection.InsertOne(entity);
            return entity;
        }

        public virtual void Add(IEnumerable<T> entities)
        {
            this._dbCollection.InsertMany(entities);
        }

        public virtual long Count()
        {
            return _dbCollection.CountDocuments(Builders<T>.Filter.Empty);
        }

        public virtual long Count(Expression<Func<T, bool>> criteria)
        {
            return this._dbCollection.AsQueryable().Count(criteria);
        }

        public virtual void Delete(string id)
        {
            this._dbCollection.DeleteOne(Builders<T>.Filter.Eq("_id", ObjectId.Parse(id)));
        }

        public virtual void Delete(T entity)
        {
            this.Delete(entity.ObjectId);
        }

        public virtual void Delete(IEnumerable<string> ids)
        {
            var objectIds = ids.Select(id => new ObjectId(id));
            this._dbCollection.DeleteMany(Builders<T>.Filter.In("_id", new BsonArray(objectIds)));
        }

        public virtual void Delete(IEnumerable<T> entities)
        {
            Delete(entities.Select(p => p.ObjectId));
        }

        public virtual void Delete(Expression<Func<T, bool>> criteria)
        {
            var collectionToRemove = this._dbCollection.AsQueryable().Where(criteria);

            if (collectionToRemove.Any())
            {
                this.Delete(collectionToRemove);
            }
        }

        public virtual void DeleteAll()
        {
            this._dbCollection.DeleteMany(Builders<T>.Filter.Empty);
        }

        public virtual bool Exists(Expression<Func<T, bool>> criteria)
        {
            return this._dbCollection.AsQueryable().Any(criteria);
        }

        public virtual List<T> Find(Expression<Func<T, bool>> filter)
        {
            return _dbCollection.AsQueryable()
                                .Where(filter)
                                .ToList();
        }

        public virtual List<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, T>> selectClause)
        {
            return
                this._dbCollection.Find(filter)
                                    .Project(selectClause)
                                    .ToList();
        }

        public virtual List<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, T>> selectClause, SortDefinition<T> sortBy)
        {
            return
                this._dbCollection.Find(filter)
                .Sort(sortBy)
                                    .Project(selectClause)
                                    .ToList();
        }

        public virtual List<T> Find(Expression<Func<T, bool>> filter, int startIndex, int pageSize)
        {
            return _dbCollection.AsQueryable()
                                .Where(filter)
                                .OrderByDescending(i => i.CreatedOn)
                                .Skip(startIndex)
                                .Take(pageSize)
                                .ToList();
        }

        public virtual List<T> Find(int startIndex, int pageSize, Expression<Func<T, bool>> filter, Expression<Func<T, T>> selectClause)
        {
            return
                this._dbCollection.Find(filter)
                                    .Skip(startIndex)
                                    .Limit(pageSize)
                                    //.SortByDescending(x => x.CreatedOn)
                                    //.ThenBy(x => x.Age)
                                    .Project(selectClause)
                                    .ToList();
        }

        public virtual List<T> Find(FilterDefinition<T> filter, Expression<Func<T, T>> selectClause)
        {
            return
                this._dbCollection.Find(filter)
                                    //.SortByDescending(x => x.CreatedOn)
                                    //.ThenBy(x => x.Age)
                                    .Project(selectClause)
                                    .ToList();
        }

        public virtual List<T> Get(int startIndex, int pageSize, Expression<Func<T, T>> selectClause)
        {
            return
                this._dbCollection.Find(FilterDefinition<T>.Empty)
                                    .Skip(startIndex)
                                    .Limit(pageSize)
                                    //.SortByDescending(x => x.CreatedOn)
                                    //.ThenBy(x => x.Age)
                                    .Project(selectClause)
                                    .ToList();
        }
        public List<T> Get(int startIndex, int pageSize, FilterDefinition<T> filter, Expression<Func<T, T>> selectClause)
        {
            return
               this._dbCollection.Find(filter)
                                   .Skip(startIndex)
                                   .Limit(pageSize)
                                   //.SortByDescending(x => x.CreatedOn)
                                   //.ThenBy(x => x.Age)
                                   .Project(selectClause)
                                   .ToList();
        }

        public IQueryable<T> Get(Func<T, bool> filter)
        {
            return
               this._dbCollection.AsQueryable().Where(filter).AsQueryable();
                                   //.SortByDescending(x => x.CreatedOn)
                                   //.ThenBy(x => x.Age)
                                   //.Project(selectClause)
                                   //.as
        }

        public virtual List<T> GetAll(Expression<Func<T, T>> selectClause)
        {
            return this._dbCollection
                .Find(Builders<T>.Filter.Empty)
                .Project(selectClause)
                .ToList();
        }

        public virtual List<T> GetAll()
        {
            return this._dbCollection.Find(Builders<T>.Filter.Empty).ToList();
        }

        public T GetById(string id)
        {
            return this._dbCollection.Find(Builders<T>.Filter.Eq("_id", ObjectId.Parse(id))).FirstOrDefault();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var result = await this._dbCollection.FindAsync(Builders<T>.Filter.Eq("_id", ObjectId.Parse(id)), new FindOptions<T> { Limit = 1 });
            var requiredEntities = await result.ToListAsync();
            return requiredEntities.FirstOrDefault();
        }

        public T GetSingle(Expression<Func<T, bool>> criteria)
        {
            return _dbCollection.AsQueryable().FirstOrDefault(criteria);
        }

        public virtual T Update(T entity)
        {
            this._dbCollection.ReplaceOne(Builders<T>.Filter.Eq("_id", ObjectId.Parse(entity.ObjectId)), entity, new UpdateOptions { IsUpsert = true });
            return entity;
        }

        public virtual T Update(T entity, Expression<Func<T, bool>> filter)
        {
            entity.ObjectId = null;
            this._dbCollection.ReplaceOne(filter, entity, new UpdateOptions { IsUpsert = true });
            return entity;
        }

        public virtual void Update(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                this._dbCollection.ReplaceOne(Builders<T>.Filter.Eq("_id", ObjectId.Parse(entity.ObjectId)), entity, new UpdateOptions { IsUpsert = true });
        }

        public virtual void Update(IEnumerable<T> entities, bool IsUpsert = true)
        {
            var bulkOps = new List<WriteModel<T>>();
            foreach (var record in entities)
            {
                var upsertOne = new ReplaceOneModel<T>(
                    Builders<T>.Filter.Where(x => x.ObjectId == record.ObjectId),
                    record)
                { IsUpsert = IsUpsert };
                bulkOps.Add(upsertOne);
            }
            this._dbCollection.BulkWrite(bulkOps);
        }

        public virtual void Update(IEnumerable<T> entities, Expression<Func<T, bool>> filter, bool IsUpsert = true)
        {
            var bulkOps = new List<WriteModel<T>>();
            foreach (var record in entities)
            {
                var upsertOne = new ReplaceOneModel<T>(
                    Builders<T>.Filter.Where(filter),
                    record)
                { IsUpsert = IsUpsert };
                bulkOps.Add(upsertOne);
            }
            this._dbCollection.BulkWrite(bulkOps);
        }
    }
}