using System.Net;
using System.Text.Json;
using TPTicketingPS.Application.Common.Exceptions;
using AppValidationException = TPTicketingPS.Application.Common.Exceptions.ValidationException;
using FluentValidationException = FluentValidation.ValidationException;

namespace TPTicketingPS.API.Middleware;

/// <summary>
/// Captura excepciones de la capa Application y las traduce a respuestas HTTP.
/// Mantiene a los controllers libres de try/catch y a los use cases agnósticos de HTTP.
/// </summary>
public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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
        var (status, payload) = MapException(exception);

        // Solo logueamos el stack trace para errores 500 (los otros son esperables).
        if (status == HttpStatusCode.InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception en {Path}", context.Request.Path);
        }
        else
        {
            logger.LogWarning("Excepción manejada: {Type} - {Message}",
                exception.GetType().Name, exception.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }

    private static (HttpStatusCode Status, object Payload) MapException(Exception exception) =>
        exception switch
        {
            NotFoundException ex => (
                HttpStatusCode.NotFound,
                new
                {
                    error = "NotFound",
                    message = ex.Message
                }),

            ConflictException ex => (
                HttpStatusCode.Conflict,
                new
                {
                    error = "Conflict",
                    message = ex.Message
                }),

            // Validación manual desde un use case
            AppValidationException ex => (
                HttpStatusCode.BadRequest,
                new
                {
                    error = "ValidationError",
                    message = ex.Message,
                    details = ex.Errors
                }),

            // Validación automática de FluentValidation (desde validator)
            FluentValidationException ex => (
                HttpStatusCode.BadRequest,
                new
                {
                    error = "ValidationError",
                    message = "Hay errores de validación en la petición.",
                    details = ex.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray())
                }),

            // Cualquier otra cosa: 500 con mensaje genérico
            _ => (
                HttpStatusCode.InternalServerError,
                new
                {
                    error = "InternalServerError",
                    message = "Ocurrió un error inesperado al procesar la solicitud."
                })
        };
}