using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace core_infrastructure.Middlewares
{
    public class CorrelationMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            const string correlationHeaderKeyName = "x-correlation-id";

            context.Request.Headers.TryGetValue(correlationHeaderKeyName, out StringValues headerValue);

            if (!context.Request.Headers.ContainsKey(correlationHeaderKeyName))
            {
                context.Request.Headers.Add(correlationHeaderKeyName, Guid.NewGuid().ToString("N"));
            }

            await _next(context);
        }
    }
}
