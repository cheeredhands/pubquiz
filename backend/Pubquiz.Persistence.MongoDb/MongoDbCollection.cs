using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Pubquiz.Persistence.Helpers;

namespace Pubquiz.Persistence.MongoDb
{
    /// <inheritdoc />
    public class MongoDbCollection<T> : ICollection<T> where T : Model, new()
    {
        protected readonly IMongoCollection<T> Collection;
        protected readonly ILogger Logger;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="mongoDatabase"></param>
        public MongoDbCollection(ILoggerFactory loggerFactory, IMongoDatabase mongoDatabase)
        {
            Logger = loggerFactory.CreateLogger(GetType());
            try
            {
                Collection = mongoDatabase.GetCollection<T>(typeof(T).Name);
                EnsureIndexes();
            }
            catch (Exception exception)
            {
                Logger.LogCritical(
                    $"Error while getting collection {typeof(T).Name} connecting to {mongoDatabase.Client.Settings.Server.Host}.",
                    exception);
                throw;
            }
        }

        /// <inheritdoc />
        public IQueryable<T> AsQueryable() => Collection.AsQueryable();

        /// <inheritdoc />
        public Task<T> GetAsync(string id) => Collection.FindAsync(o => o.Id == id).Result?.SingleOrDefaultAsync();

        /// <inheritdoc />
        public Task<IEnumerable<T>> GetAsync(params string[] ids) =>
            Task.FromResult(Collection.FindAsync(o => ids.Contains(o.Id)).Result?.ToEnumerable());
        
        /// <inheritdoc />
        public async Task<bool> UpdateAsync(T document) =>
            await Collection.FindOneAndReplaceAsync(i => i.Id == document.Id, document) != null;

        /// <inheritdoc />
        public async Task<T> AddAsync(T document)
        {
            await Collection.InsertOneAsync(document);
            return document;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(string id) =>
            await Collection.FindOneAndDeleteAsync(i => i.Id == id) != null;
        
        /// <inheritdoc />
        public async Task<long> GetCountAsync() =>
            await Collection.CountDocumentsAsync(i => true);

        /// <inheritdoc />
        public async Task<long> GetCountAsync(Expression<Func<T, bool>> filter) =>
            await Collection.CountDocumentsAsync(filter);

        /// <inheritdoc />
        public async Task<bool> AnyAsync() => await GetCountAsync() > 0;

        /// <inheritdoc />
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter) =>
            await GetCountAsync(filter) > 0;

        /// <inheritdoc />
        public async Task<T> FirstOrDefaultAsync() =>
            await Collection.AsQueryable().FirstOrDefaultAsync();

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter) => Task.Run(() =>
            Collection.AsQueryable().FirstOrDefault(filter)
        );

        private void EnsureIndexes()
        {
            // We can only index a collection if there's at least one element, otherwise it does nothing
            if (Collection.CountDocuments(i => true) <= 0) return;

            // If there are more than one indexes present (the default on Id is created by mongo), do nothing. This means that if indexes
            // need to change, the collection needs to be reinitialized (or manage the indexes from Robomongo)
            if (Collection.Indexes.List().ToList().Count > 1) return;

            var theClass = typeof(T);

            // Walk the members of the class to see if there are any directly attached index directives
            foreach (var m in theClass.GetRuntimeProperties())
            {
                var attribute = m.GetCustomAttributes()
                    .FirstOrDefault(a => a.GetType() == typeof(EnsureIndexAttribute));
                if (attribute != null)
                    EnsureIndexesAsDeclared((EnsureIndexAttribute) attribute, m.Name);
            }
        }

        private void EnsureIndexesAsDeclared(EnsureIndexAttribute attribute, string indexFieldName)
        {
            var builder = new IndexKeysDefinitionBuilder<T>();
            var indexKeysDefinition = attribute.Descending
                ? builder.Descending(indexFieldName)
                : builder.Ascending(indexFieldName);
            var createIndexOptions = new CreateIndexOptions
            {
                Unique = attribute.Unique,
                Sparse = attribute.Sparse
            };
            var createIndexModel = new CreateIndexModel<T>(indexKeysDefinition, createIndexOptions);
            Logger.LogDebug($"Adding index on field {indexFieldName} to collection {typeof(T).Name}");
            Collection.Indexes.CreateOne(createIndexModel);
        }
    }
}