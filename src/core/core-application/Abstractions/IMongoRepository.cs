using core_domain.Abstractions;
using System.Linq.Expressions;

namespace core_application.Abstractions
{
    public interface IMongoRepository<TDocument> where TDocument : IDocument
    {
        Task<List<TDocument>> FilterByAsync(Expression<Func<TDocument, bool>> filterExpression, Action<FilterOptions> options, IDisposable? session = null);

        Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression, IDisposable? session = null);

        Task InsertOneAsync(TDocument document, IDisposable? session = null);

        Task ReplaceOneAsync(TDocument document, IDisposable? session = null);

        Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression, IDisposable? session = null);
    }

    public class FilterOptions
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
