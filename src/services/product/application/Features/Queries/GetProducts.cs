using application.Abstractions;
using AutoMapper;
using core_application.Common;
using domain.Entities;
using MediatR;

namespace application.Features.Queries
{
    public static class GetProducts
    {
        #region Query
        public class Query : QueryPaginationBase<Result>
        {
            public int CategoryId { get; set; }
            public string Code { get; set; }
        }
        #endregion

        #region Query Handler 
        public class QueryHandler : IRequestHandler<Query, Result>
        {
            private readonly IProductRepository _productRepository;
            private readonly IMapper _mapper;

            public QueryHandler(IProductRepository productRepository, IMapper mapper)
            {
                this._productRepository = productRepository;
                this._mapper = mapper;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var result = await this._productRepository.GetAsync(options =>
                {
                    options.CategoryId = request.CategoryId;
                    options.Code = request.Code;
                    options.PageNumber = request.PageNumber;
                    options.PageSize = request.PageSize;
                });

                return Result<List<ProductDto>>.Success(this._mapper.Map<List<ProductDto>>(result));
            }
        }
        #endregion

        #region Mapping Profile
        public class GetProductsProfile : Profile
        {
            public GetProductsProfile()
            {
                CreateMap<Product, ProductDto>();
            }
        }

        #endregion

        #region Response
        public class ProductDto
        {
            public Guid Id { get; set; }
            public int CategoryId { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
        }
        #endregion
    }
}
