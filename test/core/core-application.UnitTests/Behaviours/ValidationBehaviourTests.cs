using core_application.Behaviours;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;

namespace application.UnitTests.Features.Behaviours
{
    /// <summary>
    /// Test class name convention is Name Of Class which is tested + "Tests"
    /// Test method name convention is Name Of Method which is tested + "_" + scenario + "_" + expected behaviour
    /// </summary>
    [TestFixture]
    public class ValidationBehaviourTests
    {
        [SetUp]
        public void Setup()
        {
            //It is run once before every test's running
        }

        [TearDown]

        public void TearDown()
        {
            //It is run once after every test's running
        }

        [Test]
        public async Task Handle_IsHandlerDelegateCalledWhenValidatorOfRequestNotExist_ReturnTrue()
        {
            //Arrange
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(It.IsAny<Type>())).Returns(null);

            var mockRequestHandlerDelegate = new Mock<RequestHandlerDelegate<core_application.Common.Result>>();

            var validationBehaviourObj = new ValidationBehaviour<IRequest<core_application.Common.Result>, core_application.Common.Result>(mockServiceProvider.Object);

            //Act
            await validationBehaviourObj.Handle(It.IsAny<IRequest<core_application.Common.Result>>(), mockRequestHandlerDelegate.Object, CancellationToken.None);

            //Assert

            mockRequestHandlerDelegate.Verify(x => x(), Times.Once());
        }

        [Test]
        public async Task Handle_IsValidationExceptionRaisedWhenRequestValidationFailed_ReturnTrue()
        {
            //Arrange
            var mockRequestValidator = new Mock<IValidator<IRequest<core_application.Common.Result>>>();
            mockRequestValidator.Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                                .Returns(new ValidationResult(new List<ValidationFailure> { new ValidationFailure() }));

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(It.IsAny<Type>())).Returns(mockRequestValidator.Object);

            var mockRequestHandlerDelegate = new Mock<RequestHandlerDelegate<core_application.Common.Result>>();

            var validationBehaviourObj = new ValidationBehaviour<IRequest<core_application.Common.Result>, core_application.Common.Result>(mockServiceProvider.Object);

            //Act&Assert

            Assert.ThrowsAsync<ValidationException>(() => validationBehaviourObj.Handle(It.IsAny<IRequest<core_application.Common.Result>>(), mockRequestHandlerDelegate.Object, CancellationToken.None));

            await Task.CompletedTask;
        }

        [Test]
        public async Task Handle_IsHandlerDelegateCalledWhenRequestValidationSuccessed_ReturnTrue()
        {
            //Arrange
            var mockRequestValidator = new Mock<IValidator<IRequest<core_application.Common.Result>>>();
            mockRequestValidator.Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                                .Returns(new ValidationResult());

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(It.IsAny<Type>())).Returns(mockRequestValidator.Object);

            var mockRequestHandlerDelegate = new Mock<RequestHandlerDelegate<core_application.Common.Result>>();

            var validationBehaviourObj = new ValidationBehaviour<IRequest<core_application.Common.Result>, core_application.Common.Result>(mockServiceProvider.Object);

            //Act

            await validationBehaviourObj.Handle(It.IsAny<IRequest<core_application.Common.Result>>(), mockRequestHandlerDelegate.Object, CancellationToken.None);

            //Assert
            mockRequestHandlerDelegate.Verify(x => x(), Times.Once());
        }
    }
}
