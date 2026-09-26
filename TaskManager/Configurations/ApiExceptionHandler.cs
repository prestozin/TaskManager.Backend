using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using TaskManager.Application.DTOs;
using TaskManager.Core.Constants;

namespace TaskManager.Api.Configurations;

public class ApiExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ApiExceptionHandler> _logger;

    public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,CancellationToken cancellationToken)
    {
        ResultResponse<object> response;

        switch (exception)
        {
            case ValidationException validationException:

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                response = ResultResponse<object>.Failure(validationException.Errors);

                break;

            case UnauthorizedAccessException:

                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

                response = ResultResponse<object>.Failure(Messages.UNAUTHORIZED);

                break;

            default:

                _logger.LogError(exception, "Unhandled exception while processing request.");

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                response = ResultResponse<object>.Failure(Messages.UNEXPECTED_ERROR);

                break;
        }

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
