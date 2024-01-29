using application.Abstractions;
using AutoMapper;
using core_application.Common;
using domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace application.Features.Commands
{
    public static class CreateProduct
    {
        #region Command
        public class Command : IRequest<Result>
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int CategoryId { get; set; }
            public decimal Price { get; set; }
        }
        #endregion

        #region Command Handler
        public class CommandHandler : IRequestHandler<Command, Result>
        {
            private readonly IProductRepository _productRepository; 
            private readonly IMapper _mapper;
            private readonly ILogger _logger;
            public CommandHandler(IProductRepository productRepository,
                                  IMapper mapper,
                                  ILogger<CommandHandler> logger)
            {
                this._productRepository = productRepository;
                this._mapper = mapper;
                this._logger = logger;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                this._logger.LogInformation("Sisteme yeni bir ürün tanımlaması yapılmaktadır");

                #region Mükerrer ürün tanımı söz konusu mu
                var storedProducts = await this._productRepository.GetAsync(opt =>
                {
                    opt.Code = request.Code;
                });
                if (storedProducts.Any())
                {
                    return Result.Failure(new List<string> { "Ürün sistemde zaten tanımlıdır." });
                }
                #endregion

                var newProduct = this._mapper.Map<Product>(request);

                var addedProduct = await this._productRepository.AddAsync(newProduct);

                Result result;

                if (!string.IsNullOrEmpty(addedProduct.Cas))
                {
                    result = Result<Response>.Success(new Response { ProductId = addedProduct.Id });
                }
                else
                {
                    result = Result.Failure(new List<string>());
                }

                return result;
            }
        }
        #endregion

        #region Mapping Profile
        public class CreateProductProfile : Profile
        {
            public CreateProductProfile()
            {
                CreateMap<Command, Product>();
            }
        }
        #endregion

        #region Data Validation
        public class CreateProductValidator : AbstractValidator<Command>
        {
            public CreateProductValidator()
            {
                RuleFor(c => c.Code).NotEmpty().WithMessage("Code alanı boş geçilemez");
                RuleFor(c => c.Name).NotEmpty().WithMessage("Name alanı boş geçilemez");
                RuleFor(c => c.Description).NotEmpty().WithMessage("Description alanı boş geçilemez");
                RuleFor(c => c.CategoryId).GreaterThan(0).WithMessage("Categoryid alanı boş geçilemez");
                RuleFor(c => c.Price).GreaterThan(0).WithMessage("Price alanı boş geçilemez");
            }
        }
        #endregion

        #region Response 
        public class Response
        {
            public Guid ProductId { get; set; }
        }
        #endregion
    }
}
