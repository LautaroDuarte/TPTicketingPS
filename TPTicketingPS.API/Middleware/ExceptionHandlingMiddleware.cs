using System.Net;
using System.Text.Json;
using TPTicketingPS.Application.Common.Exceptions;

// Para entrega 2

namespace TPTicketingPS.Api.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var (status, payload) = exception switch
        {
            NotFoundException ex => (
                HttpStatusCode.NotFound,
                (object)new { error = "NotFound", message = ex.Message }),

            ConflictException ex => (
                HttpStatusCode.Conflict,
                new { error = "Conflict", message = ex.Message }),

            ValidationException ex => (
                HttpStatusCode.BadRequest,
                new { error = "ValidationError", message = ex.Message, details = ex.Errors }),

            FluentValidation.ValidationException ex => (
                HttpStatusCode.BadRequest,
                new
                {
                    error = "ValidationError",
                    message = "Hay errores de validación en la petición.",
                    details = ex.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                }),

            _ => (
                HttpStatusCode.InternalServerError,
                new { error = "InternalServerError", message = "Ocurrió un error inesperado." })
        };

        if (status == HttpStatusCode.InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception");
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}