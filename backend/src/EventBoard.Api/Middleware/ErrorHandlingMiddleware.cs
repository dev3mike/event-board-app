using System.Net;
using System.Text.Json;
using EventBoard.Api.DTOs;
using EventBoard.Domain.Exceptions;

namespace EventBoard.Api.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "application/json";

            var error = new ApiErrorDto { Code = ex.Code, Message = ex.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(error, JsonOptions));
        }
        catch (BusinessRuleException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            context.Response.ContentType = "application/json";

            var error = new ApiErrorDto { Code = ex.Code, Message = ex.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(error, JsonOptions));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled exception. Path={Path}, Method={Method}, TraceId={TraceId}",
                context.Request.Path,
                context.Request.Method,
                context.TraceIdentifier);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var error = new ApiErrorDto { Code = "UnexpectedError", Message = "An unexpected error occurred." };
            await context.Response.WriteAsync(JsonSerializer.Serialize(error, JsonOptions));
        }
    }
}
