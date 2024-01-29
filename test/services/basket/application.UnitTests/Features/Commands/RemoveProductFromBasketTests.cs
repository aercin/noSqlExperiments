using application.Abstractions;
using application.Features.Commands;
using domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace application.UnitTests.Features.Commands
{
    [TestFixture]
    public class RemoveProductFromBasketTests
    {
        [SetUp] public void SetUp() { }

        [TearDown] public void TearDown() { }

        [Test]
        public async Task Handle_IsFailureResultReturnWhenBasketIsNotExist_ReturnTrue()
        {
            //Arrange
            var mockRedisRepo = new Mock<IRedisRepository>();
            mockRedisRepo.Setup(x => x.IsKeyExistAsync(It.IsAny<string>())).Returns(Task.FromResult(false));

            var mockLogger = new Mock<ILogger<RemoveProductFromBasket.CommandHandler>>();
            var mockConfig = new Mock<IConfiguration>();
            var mockCommand = new Mock<RemoveProductFromBasket.Command>();
            mockCommand.Object.UserId = Guid.NewGuid();

            var objCommandHandler = new RemoveProductFromBasket.CommandHandler(mockRedisRepo.Object, mockLogger.Object, mockConfig.Object);

            //Act
            var res = await objCommandHandler.Handle(mockCommand.Object, CancellationToken.None);

            //Assert
            Assert.IsTrue(res.IsSuccess == false);
        }

        [Test]
        public async Task Handle_IsFailureResultReturnWhenProductToBeRemovedIsNotExist_ReturnTrue()
        {
            //Arrange 
            var mockRedisRepo = new Mock<IRedisRepository>();
            mockRedisRepo.Setup(x => x.IsKeyExistAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
            mockRedisRepo.Setup(x => x.GetAsync<Basket>(It.IsAny<string>())).Returns(Task.FromResult(Mock.Of<Basket>()));

            var mockLogger = new Mock<ILogger<RemoveProductFromBasket.CommandHandler>>();
            var mockConfig = new Mock<IConfiguration>();

            var mockCommand = new Mock<RemoveProductFromBasket.Command>();
            mockCommand.Object.UserId = Guid.NewGuid();

            var objCommandHandler = new RemoveProductFromBasket.CommandHandler(mockRedisRepo.Object, mockLogger.Object, mockConfig.Object);

            //Act
            var res = await objCommandHandler.Handle(mockCommand.Object, CancellationToken.None);

            //Assert
            Assert.IsTrue(res.IsSuccess == false);
        }

        [Test]
        public async Task Handle_IsFailureResultReturnWhenNotEnoughProductToBeRemovedInBasket_ReturnTrue()
        {
            //Arrange 
            var mockRedisRepo = new Mock<IRedisRepository>();

            mockRedisRepo.Setup(x => x.IsKeyExistAsync(It.IsAny<string>())).Returns(Task.FromResult(true));

            var productId = Guid.NewGuid();
            var mockBasket = new Mock<Basket>();
            mockBasket.Object.Items = new List<BasketItem>
            {
                new BasketItem
                {
                    ProductId = productId,
                    Price = It.IsAny<decimal>(),
                    Quantity = 1
                }
            };
            mockRedisRepo.Setup(x => x.GetAsync<Basket>(It.IsAny<string>())).Returns(Task.FromResult(mockBasket.Object));

            var mockLogger = new Mock<ILogger<RemoveProductFromBasket.CommandHandler>>();
            var mockConfig = new Mock<IConfiguration>();

            var mockCommand = new Mock<RemoveProductFromBasket.Command>();
            mockCommand.Object.UserId = Guid.NewGuid();
            mockCommand.Object.ProductId = productId;
            mockCommand.Object.Quantity = 10;

            var objCommandHandler = new RemoveProductFromBasket.CommandHandler(mockRedisRepo.Object, mockLogger.Object, mockConfig.Object);

            //Act
            var res = await objCommandHandler.Handle(mockCommand.Object, CancellationToken.None);

            //Assert
            Assert.IsTrue(res.IsSuccess == false);
        }

        [Test]
        public async Task Handle_IsSuccessResultReturnWhenQuantitiesMatched_ReturnTrue()
        {
            //Arrange 
            var mockRedisRepo = new Mock<IRedisRepository>();
            mockRedisRepo.Setup(x => x.IsKeyExistAsync(It.IsAny<string>())).Returns(Task.FromResult(true)); 

            var productId = Guid.NewGuid();
            var mockBasket = new Mock<Basket>();
            mockBasket.Object.Items = new List<BasketItem>
            {
                new BasketItem
                {
                    ProductId = productId,
                    Price = It.IsAny<decimal>(),
                    Quantity = 10
                }
            };
            mockRedisRepo.Setup(x => x.GetAsync<Basket>(It.IsAny<string>())).Returns(Task.FromResult(mockBasket.Object));

            var mockLogger = new Mock<ILogger<RemoveProductFromBasket.CommandHandler>>();

            var mockConfigSection = new Mock<IConfigurationSection>();
            mockConfigSection.Setup(x => x.Value).Returns("10");

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("Basket:Expiration")).Returns(mockConfigSection.Object);

            var mockCommand = new Mock<RemoveProductFromBasket.Command>();
            mockCommand.Object.UserId = Guid.NewGuid();
            mockCommand.Object.ProductId = productId;
            mockCommand.Object.Quantity = 10;

            var objCommandHandler = new RemoveProductFromBasket.CommandHandler(mockRedisRepo.Object, mockLogger.Object, mockConfig.Object);

            //Act
            var res = await objCommandHandler.Handle(mockCommand.Object, CancellationToken.None);

            //Assert 
            Assert.IsTrue(mockBasket.Object.Items.Count() == 0);
        }

        [Test]
        public async Task Handle_IsSuccessResultReturnWhenQuantityToBeRemovedLowerThanQuantityOfProduct_ReturnTrue()
        {
            //Arrange 
            var mockRedisRepo = new Mock<IRedisRepository>();
            mockRedisRepo.Setup(x => x.IsKeyExistAsync(It.IsAny<string>())).Returns(Task.FromResult(true));

            var productId = Guid.NewGuid();
            var mockBasket = new Mock<Basket>();
            mockBasket.Object.Items = new List<BasketItem>
            {
                new BasketItem
                {
                    ProductId = productId,
                    Price = It.IsAny<decimal>(),
                    Quantity = 10
                }
            };
            mockRedisRepo.Setup(x => x.GetAsync<Basket>(It.IsAny<string>())).Returns(Task.FromResult(mockBasket.Object));

            var mockLogger = new Mock<ILogger<RemoveProductFromBasket.CommandHandler>>();

            var mockConfigSection = new Mock<IConfigurationSection>();
            mockConfigSection.Setup(x => x.Value).Returns("10");

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("Basket:Expiration")).Returns(mockConfigSection.Object);

            var mockCommand = new Mock<RemoveProductFromBasket.Command>();
            mockCommand.Object.UserId = Guid.NewGuid();
            mockCommand.Object.ProductId = productId;
            mockCommand.Object.Quantity = 9;

            var objCommandHandler = new RemoveProductFromBasket.CommandHandler(mockRedisRepo.Object, mockLogger.Object, mockConfig.Object);

            //Act
            var res = await objCommandHandler.Handle(mockCommand.Object, CancellationToken.None);

            //Assert 
            Assert.IsTrue(mockBasket.Object.Items.Single(x => x.ProductId == productId).Quantity == 1);
        }
    }
}
