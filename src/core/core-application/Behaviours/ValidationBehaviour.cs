﻿using core_application.Common;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace core_application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : class, IRequest<TResponse>
                                                                                                   where TResponse : Result
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationBehaviour(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var hasIgnoreBehavioursAttr = typeof(TRequest).GetCustomAttributes(typeof(IgnoreBehavioursAttribute), true).Any();
            if (hasIgnoreBehavioursAttr)
                return await next();

            var validator = this._serviceProvider.GetService<IValidator<TRequest>>();
            if (validator != null)
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResult = validator.Validate(context);

                if (!validationResult.IsValid)
                {
                    throw new ValidationException($"{validationResult.ToString()}");
                }
            }

            return await next();
        }
    }
}
