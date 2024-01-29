using application.Abstractions;
using application.Features.Commands;
using application.Features.Queries.Models;
using AutoMapper;
using domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace application.UnitTests.Features.Commands
{
    [TestFixture]
    public class CreateProductTests
    {
        [SetUp] public void SetUp() { }

        [TearDown] public void TearDown() { }

        [Test]
        public async Task Handle_IsDuplicateCheckAvailable_ReturnTrue()
        {
            //Arrange
            var mockProductRepo = new Mock<IProductRepository>();
            mockProductRepo.Setup(x => x.GetAsync(It.IsAny<Action<ProductQueryOptions>>())).Returns(Task.FromResult(new List<Product> { new Product() }));
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<CreateProduct.CommandHandler>>();
            var objCommandHandler = new CreateProduct.CommandHandler(mockProductRepo.Object, mockMapper.Object, mockLogger.Object);

            //Act
            var res = await objCommandHandler.Handle(It.IsAny<CreateProduct.Command>(), CancellationToken.None);

            //Assert
            Assert.IsTrue(res.IsSuccess == false);
        }

        [Test]
        public async Task Handle_IsFailStatusReturnWhenProductNotCreated_ReturnTrue()
        {
            //Arrange 
            var mockProductRepo = new Mock<IProductRepository>();
            mockProductRepo.Setup(x => x.GetAsync(It.IsAny<Action<ProductQueryOptions>>())).Returns(Task.FromResult(new List<Product>()));
            mockProductRepo.Setup(x => x.AddAsync(It.IsAny<Product>())).Returns(Task.FromResult(Mock.Of<Product>()));
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<CreateProduct.CommandHandler>>();
            var objCommandHandler = new CreateProduct.CommandHandler(mockProductRepo.Object, mockMapper.Object, mockLogger.Object);

            //Act
            var res = await objCommandHandler.Handle(It.IsAny<CreateProduct.Command>(), CancellationToken.None);

            //Assert
            Assert.IsTrue(res.IsSuccess == false);
        }

        [Test]
        public async Task Handle_IsSuccessStatusReturnWhenProductCreated_ReturnTrue()
        {
            //Arrange 
            var mockProductRepo = new Mock<IProductRepository>();
            mockProductRepo.Setup(x => x.GetAsync(It.IsAny<Action<ProductQueryOptions>>())).Returns(Task.FromResult(new List<Product>()));

            var mockProduct = new Mock<Product>();
            mockProduct.Object.Cas = "dummyCasValue";

            mockProductRepo.Setup(x => x.AddAsync(It.IsAny<Product>())).Returns(Task.FromResult(mockProduct.Object));

            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<CreateProduct.CommandHandler>>();
            var objCommandHandler = new CreateProduct.CommandHandler(mockProductRepo.Object, mockMapper.Object, mockLogger.Object);

            //Act
            var res = await objCommandHandler.Handle(It.IsAny<CreateProduct.Command>(), CancellationToken.None);

            //Assert
            Assert.IsTrue(res.IsSuccess);
        }
    }
}
