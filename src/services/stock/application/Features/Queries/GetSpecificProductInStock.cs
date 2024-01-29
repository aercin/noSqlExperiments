using application.Abstractions;
using AutoMapper;
using core_application.Common;
using domain.Entities;
using MediatR;

namespace application.Features.Queries
{
    public static class GetSpecificProductInStock
    {
        #region Query
        public class Query : QueryPaginationBase<Result>
        {
            public Guid ProductId { get; set; }
        }
        #endregion

        #region Query Handler 
        public class QueryHandler : IRequestHandler<Query, Result>
        {
            private readonly IUnitOfWork _uow;
            private readonly IMapper _mapper;

            public QueryHandler(IUnitOfWork uow, IMapper mapper)
            {
                this._uow = uow;
                this._mapper = mapper;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var result = await this._uow.StockRepo.FindOneAsync(x => x.Products.Any(x => x.Id == request.ProductId.ToString()));

                return Result<List<ProductDto>>.Success(this._mapper.Map<List<ProductDto>>(result.Products));
            }
        }
        #endregion

        #region Mapping Profile
        public class GetStockProfile : Profile
        {
            public GetStockProfile()
            {
                CreateMap<Product, ProductDto>().ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => Guid.Parse(src.Id)));
            }
        }
        #endregion

        #region Response
        public class ProductDto
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }
        #endregion
    }
}
