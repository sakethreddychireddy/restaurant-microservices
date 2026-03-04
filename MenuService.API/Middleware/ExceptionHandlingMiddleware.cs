using MenuService.Domain.Exceptions;
using System.Text.Json;
using FluentValidation;

namespace MenuService.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (MenuItemNotFoundException ex)
            {
                await WriteErrorAsync(context, StatusCodes.Status404NotFound, ex.Message);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "Validation failed.", errors);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                await WriteErrorAsync(context, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception.");
                await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
        private static async Task WriteErrorAsync(HttpContext context, int statusCode,
            string message, object? errors = null)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new { statusCode, message, errors };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
