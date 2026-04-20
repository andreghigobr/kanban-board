using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kanban.Api.Common.Errors;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        var (statusCode, message) = exception switch
        {
            TaskNotFoundException => (404, exception.Message),
            TaskValidationException => (400, exception.Message),
            ArgumentException => (400, exception.Message),
            KeyNotFoundException => (404, exception.Message),
            _ => (500, "An unexpected error occurred")
        };

        _logger.LogError(exception, "Exception occurred: {Message} (Status Code: {StatusCode})", message, statusCode);

        context.Result = new ObjectResult(new { message })
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}
