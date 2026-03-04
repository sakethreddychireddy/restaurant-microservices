using System.Net;

namespace AuthService.API.Middleware
{
    public sealed class ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "An unexpected error occurred. Please try again later.",
                    success = false,
                    Service = "AuthService"
                });
            }
        }
    }
}
