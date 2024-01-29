using MediatR;

namespace core_application.Common
{
    public class QueryPaginationBase<T> : IRequest<T> where T : Result
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
