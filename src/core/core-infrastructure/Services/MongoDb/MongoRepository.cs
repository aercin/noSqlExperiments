using core_application.Abstractions;
using core_domain.Abstractions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace core_infrastructure.Services.MongoDb
{
    public abstract class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : IDocument
    {
        private readonly IMongoCollection<TDocument> _collection;

        public MongoRepository(IConfigurationSection config, IMongoClient client)
        {
            var database = client.GetDatabase(config.GetValue<string>("DatabaseName"));
            _collection = database.GetCollection<TDocument>(config.GetValue<string>($"{typeof(TDocument).Name}CollectionName"));
        }

        public async Task<List<TDocument>> FilterByAsync(Expression<Func<TDocument, bool>> filterExpression, Action<FilterOptions> options, IDisposable? session = null)
        {
            List<TDocument> result;

            var filterOptions = new FilterOptions();
            options(filterOptions);

            if (session != null)
                result = await _collection.Find((IClientSessionHandle)session, filterExpression)
                                          .Skip((filterOptions.PageNumber - 1) * filterOptions.PageSize)
                                          .Limit(filterOptions.PageSize)
                                          .ToListAsync();
            else
                result = await _collection.Find(filterExpression)
                                          .Skip((filterOptions.PageNumber - 1) * filterOptions.PageSize)
                                          .Limit(filterOptions.PageSize)
                                          .ToListAsync();
            return result;
        }

        public async Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression, IDisposable? session = null)
        {
            TDocument result;

            if (session != null)
                result = await _collection.Find((IClientSessionHandle)session, filterExpression).FirstOrDefaultAsync();
            else
                result = await _collection.Find(filterExpression).FirstOrDefaultAsync();

            return result;
        }

        public async Task InsertOneAsync(TDocument document, IDisposable? session = null)
        {
            if (session != null)
                await _collection.InsertOneAsync((IClientSessionHandle)session, document);
            else
                await _collection.InsertOneAsync(document);
        }

        public async Task ReplaceOneAsync(TDocument document, IDisposable? session = null)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            if (session != null)
                await _collection.FindOneAndReplaceAsync((IClientSessionHandle)session, filter, document);
            else
                await _collection.FindOneAndReplaceAsync(filter, document);
        }

        public async Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression, IDisposable? session = null)
        {
            if (session != null)
                await _collection.FindOneAndDeleteAsync((IClientSessionHandle)session, filterExpression);
            else
                await _collection.FindOneAndDeleteAsync(filterExpression);
        }
    }
}
