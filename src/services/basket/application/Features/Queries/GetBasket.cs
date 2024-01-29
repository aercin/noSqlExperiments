using application.Abstractions;
using AutoMapper;
using core_application.Common;
using domain.Entities;
using MediatR;

namespace application.Features.Queries
{
    public static class GetBasket
    {
        #region Query
        public class Query : IRequest<Result>
        {
            public Guid UserId { get; set; }
        }
        #endregion

        #region Query Handler 
        public class QueryHandler : IRequestHandler<Query, Result>
        {
            private readonly IRedisRepository _redisRepository;
            private readonly IMapper _mapper;

            public QueryHandler(IRedisRepository redisRepository, IMapper mapper)
            {
                this._redisRepository = redisRepository;
                this._mapper = mapper;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var result = await this._redisRepository.GetAsync<Basket>(request.UserId.ToString());

                return Result<List<BasketItemDto>>.Success(this._mapper.Map<List<BasketItemDto>>(result.Items));
            }
        }
        #endregion

        #region Mapping Profile
        public class GetBasketProfile : Profile
        {
            public GetBasketProfile()
            {
                CreateMap<BasketItem, BasketItemDto>();
            }
        }

        #endregion

        #region Response
        public class BasketItemDto
        {
            public Guid ProductId { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }
        #endregion 
    }
}
