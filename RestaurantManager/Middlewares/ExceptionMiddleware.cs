using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RestaurantManager.Middlewares
{
    public sealed class ExceptionMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _next = next;

            _logger = loggerFactory
                .CreateLogger<ExceptionMiddleware>();
        }

        public async Task InvokeAsync(
            HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(
                    context,
                    e);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            _logger
                .LogInformation(
                    "{0}: {1}",
                    exception
                        .InnerException
                        ?.Source ??
                    exception.Source,
                    exception
                        .InnerException
                        ?.Message ??
                    exception.Message);

            context
                .Response
                .StatusCode = (int) HttpStatusCode.BadRequest;

            context
                .Response
                .ContentType = "text/plain";

            await context
                .Response
                .WriteAsync(
                    exception
                        .Message);
        }
    }
}
