using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Platform.NoSql.MongoDB
{
    public interface IMongoRepository<T> where T : MongoEntity
    {
        IMongoCollection<T> DbCollection { get; }
        T Add(T entity);

        void Add(IEnumerable<T> entities);

        long Count();

        long Count(Expression<Func<T, bool>> filter);

        void Delete(string id);

        void Delete(IEnumerable<string> ids);

        void Delete(IEnumerable<T> entities);

        void Delete(T entity);

        void Delete(Expression<Func<T, bool>> criteria);

        void DeleteAll();

        bool Exists(Expression<Func<T, bool>> criteria);

        List<T> Find(Expression<Func<T, bool>> filter);

        List<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, T>> selectClause);

        List<T> Find(Expression<Func<T, bool>> filter, int startIndex, int pageSize);

        List<T> Find(int startIndex, int pageSize, Expression<Func<T, bool>> filter, Expression<Func<T, T>> selectClause);
        List<T> Find(FilterDefinition<T> filter, Expression<Func<T, T>> selectClause);

        List<T> GetAll();

        List<T> GetAll(Expression<Func<T, T>> selectClause);

        List<T> Get(int startIndex, int pageSize, Expression<Func<T, T>> selectClause);
        List<T> Get(int startIndex, int pageSize, FilterDefinition<T> filter, Expression<Func<T, T>> selectClause);
        IQueryable<T> Get(Func<T, bool> filter);

        T GetById(string id);

        T GetSingle(Expression<Func<T, bool>> criteria);

        T Update(T entity);

        T Update(T entity, Expression<Func<T, bool>> filter);

        void Update(IEnumerable<T> entities);

        void Update(IEnumerable<T> entities, bool IsUpsert = true);

        void Update(IEnumerable<T> entities, Expression<Func<T, bool>> filter, bool IsUpsert = true);
    }
}