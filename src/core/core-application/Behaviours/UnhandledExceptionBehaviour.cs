using core_application.Common;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Runtime.Serialization;

namespace core_application.Behaviours
{
    public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
                                                                                                           where TResponse : Result
    {
        private readonly ILogger _logger;

        public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
        {
            this._logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var hasIgnoreBehavioursAttr = typeof(TRequest).GetCustomAttributes(typeof(IgnoreBehavioursAttribute), true).Any();
            if (hasIgnoreBehavioursAttr)
                return await next();

            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                string errorMessage = "Sistemde bekleyen bir durum oluştu.";
                if (ex.GetType() == typeof(ValidationException))
                {
                    errorMessage = "Yanlış veya eksik bilgi gönderimi söz konusudur.";
                }

                this._logger.LogError(ex, errorMessage);

                return (TResponse)Result.Failure(new List<string> { errorMessage });
            }
        }
    }
}
