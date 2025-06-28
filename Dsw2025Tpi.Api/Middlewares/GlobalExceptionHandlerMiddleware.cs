using Dsw2025Tpi.Application.Exceptions;
using System.Net;
using System.Text.Json;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Se ha producido una excepción no controlada.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var statusCode = HttpStatusCode.InternalServerError; // Default a 500
        var message = "Ocurrió un error inesperado al procesar la solicitud.";

        switch (exception)
        {
            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest; // 400 Bad Request
                message = exception.Message;
                break;

            case EntityNotFoundException:
                statusCode = HttpStatusCode.NotFound; // 404 Not Found
                message = exception.Message;
                break;

            case DuplicatedEntityException:
                statusCode = HttpStatusCode.BadRequest; // 400 Bad Request
                message = exception.Message;
                break;

            case PriceNullException:
                statusCode = HttpStatusCode.BadRequest; // 400 Bad Request
                message = exception.Message;
                break;

            default:
                break;
        }

        context.Response.StatusCode = (int)statusCode;
        var result = JsonSerializer.Serialize(new { error = message });
        return context.Response.WriteAsync(result);
    }
}