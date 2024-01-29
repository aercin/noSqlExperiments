using core_application.Behaviours;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace application.UnitTests.Features.Behaviours
{
    [TestFixture]
    public class UnhandledExceptionBehaviourTests
    {
        [SetUp]
        public void SetUp() { }

        [TearDown] public void TearDown() { }

        [Test]
        public async Task Handle_IsErrorLogWrittenWhenAnyExceptionalSituation_ReturnTrue()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<IRequest<core_application.Common.Result>>>();
            var mockRequestHandlerDelegate = new Mock<RequestHandlerDelegate<core_application.Common.Result>>();
            mockRequestHandlerDelegate.Setup(x => x()).Throws<Exception>();

            var objUnhandledExceptionBehaviour = new UnhandledExceptionBehaviour<IRequest<core_application.Common.Result>, core_application.Common.Result>(mockLogger.Object);

            //Act
            await objUnhandledExceptionBehaviour.Handle(It.IsAny<IRequest<core_application.Common.Result>>(), mockRequestHandlerDelegate.Object, CancellationToken.None);

            //Assert
            mockLogger.Verify(m => m.Log(
                 It.Is<LogLevel>(x => x == LogLevel.Error),
                 It.IsAny<EventId>(),
                 It.Is<It.IsAnyType>((v, t) => true),
                 It.IsAny<Exception>(),
                 It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }
    }
}
