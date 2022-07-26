using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using Tweet.API.Infrastructure.ActionResults;

namespace Tweet.API.Infrastructure.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        public HttpGlobalExceptionFilter(IWebHostEnvironment environment, ILogger<HttpGlobalExceptionFilter> logger)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception, 
                context.Exception.Message);

            var json = new JsonErrorResponse
            {
                Messages = new[] { "An unexpected error occured." }
            };

            if (_environment.IsDevelopment())
            {
                json.DeveloperMessage = context.Exception;
            }

            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Result = new InternalServerErrorObjectResult(json);
            context.ExceptionHandled = true;
        }
    }
}
