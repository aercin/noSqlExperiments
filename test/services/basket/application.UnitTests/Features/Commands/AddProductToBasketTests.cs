using application.Abstractions;
using application.Features.Commands;
using AutoMapper;
using domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq; 
using NUnit.Framework;
using System.Text.Json;

namespace application.UnitTests.Features.Commands
{
    [TestFixture]
    public class AddProductToBasketTests
    {
        [SetUp] public void SetUp() { }
        [TearDown] public void TearDown() { }

        [Test]
        public async Task Handle_CheckAddingExistedProductToBasket_ReturnTrue()
        {
            //Arrange 
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var mockBasket = new Mock<Basket>();
            mockBasket.Object.UserId = userId;
            mockBasket.Object.Items = new List<BasketItem>
            {
                new BasketItem
                {
                    ProductId =productId,
                    Price = It.IsAny<decimal>(),
                    Quantity = 1
                }
            };
            var mockRedisRepo = new Mock<IRedisRepository>();
            mockRedisRepo.Setup(x => x.IsKeyExistAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
            mockRedisRepo.Setup(x => x.GetAsync<Basket>(It.IsAny<string>())).Returns(Task.FromResult(mockBasket.Object));

            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<AddProductToBasket.CommandHandler>>();

            var mockConfigSection = new Mock<IConfigurationSection>();
            mockConfigSection.Setup(x => x.Value).Returns("10");

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("Basket:Expiration")).Returns(mockConfigSection.Object);

            var mockCommand = new Mock<AddProductToBasket.Command>();
            mockCommand.Object.UserId = It.IsAny<Guid>();
            mockCommand.Object.ProductId = productId;
            mockCommand.Object.Quantity = 4;

            var objCommandHandler = new AddProductToBasket.CommandHandler(mockRedisRepo.Object, mockMapper.Object, mockLogger.Object, mockConfig.Object);

            //Act

            var res = await objCommandHandler.Handle(mockCommand.Object, CancellationToken.None);

            //Assert 

            Assert.IsTrue(mockBasket.Object.Items.Single(x => x.ProductId == productId).Quantity == 5);
        }


        [Test]
        public async Task Handle_CheckAddingProductToBasketWhichIsNotExistBefore_ReturnTrue()
        {
            //Arrange 
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var mockBasket = new Mock<Basket>();
            mockBasket.Object.UserId = userId;

            var mockRedisRepo = new Mock<IRedisRepository>();
            mockRedisRepo.Setup(x => x.IsKeyExistAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
            mockRedisRepo.Setup(x => x.GetAsync<Basket>(It.IsAny<string>())).Returns(Task.FromResult(mockBasket.Object));

            var mockCommand = new Mock<AddProductToBasket.Command>();
            mockCommand.Object.UserId = It.IsAny<Guid>();
            mockCommand.Object.ProductId = productId;
            mockCommand.Object.Quantity = 5;
            
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<BasketItem>(It.IsAny<AddProductToBasket.Command>())).Returns(JsonSerializer.Deserialize<BasketItem>(JsonSerializer.Serialize(mockCommand.Object)));

            var mockLogger = new Mock<ILogger<AddProductToBasket.CommandHandler>>();

            var mockConfigSection = new Mock<IConfigurationSection>();
            mockConfigSection.Setup(x => x.Value).Returns("10");

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("Basket:Expiration")).Returns(mockConfigSection.Object);

            var objCommandHandler = new AddProductToBasket.CommandHandler(mockRedisRepo.Object, mockMapper.Object, mockLogger.Object, mockConfig.Object);

            //Act

            var res = await objCommandHandler.Handle(mockCommand.Object, CancellationToken.None);

            //Assert 

            Assert.IsTrue(mockBasket.Object.Items.Single(x => x.ProductId == productId).Quantity == 5);
        }
    }
}
