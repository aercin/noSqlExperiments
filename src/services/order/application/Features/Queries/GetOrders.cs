using application.Abstractions;
using AutoMapper;
using core_application.Common;
using domain.Entities;
using domain.Enumerations;
using MediatR;

namespace application.Features.Queries
{
    public static class GetOrders
    {
        #region Query
        public class Query : QueryPaginationBase<Result>
        {
            public Guid UserId { get; set; }
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
                var result = await this._uow.OrderRepo.FilterByAsync(x => x.UserId == request.UserId.ToString(), opt =>
                {
                    opt.PageNumber = 1;
                    opt.PageSize = 10;
                });

                return Result<List<OrderDto>>.Success(this._mapper.Map<List<OrderDto>>(result));
            }
        }
        #endregion

        #region Mapping Profile
        public class GetOrdersProfile : Profile
        {
            public GetOrdersProfile()
            {
                CreateMap<Order, OrderDto>().ForMember(dest => dest.OrderNo, opt => opt.MapFrom(src => Guid.Parse(src.Id)));
            }
        }

        #endregion

        #region Response
        public class OrderDto
        {
            public Guid OrderNo { get; set; }
            public OrderStatus Status { get; set; }
        }
        #endregion
    }
}
