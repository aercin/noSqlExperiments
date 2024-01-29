using application.Abstractions;
using application.Features.Commands;
using domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace application.UnitTests.Features.Commands
{
    [TestFixture]
    public class DeleteProductTests
    {
        [SetUp] public void SetUp() { }
        [TearDown] public void TearDown() { }

        [Test]
        public async Task Handle_IsFailStatusReturnWhenProductNotExist_ReturnTrue()
        {
            //Arrange
            var mockUpdateProductCommand = new Mock<UpdateProduct.Command>();
            var mockProductRepo = new Mock<IProductRepository>();
            mockProductRepo.Setup(x => x.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult((Product?)null));

            var mockLogger = new Mock<ILogger<UpdateProduct.CommandHandler>>();

            var objCommandHandler = new UpdateProduct.CommandHandler(mockProductRepo.Object, mockLogger.Object);

            //Act
            var res = await objCommandHandler.Handle(mockUpdateProductCommand.Object, CancellationToken.None);

            //Assert
            Assert.IsTrue(res.IsSuccess == false);
        }
    }
}
