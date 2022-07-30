namespace User.API.Infrastructure.Filters
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net;
    using User.API.Infrastructure.ActionResults;

    /// <summary>
    /// Class for handling Exception
    /// </summary>
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// Defines the _environment.
        /// </summary>
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Defines the _logger.
        /// </summary>
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpGlobalExceptionFilter"/> class.
        /// </summary>
        /// <param name="environment">The environment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{HttpGlobalExceptionFilter}"/>.</param>
        public HttpGlobalExceptionFilter(IWebHostEnvironment environment, ILogger<HttpGlobalExceptionFilter> logger)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The OnException.
        /// </summary>
        /// <param name="context">The context<see cref="ExceptionContext"/>.</param>
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
