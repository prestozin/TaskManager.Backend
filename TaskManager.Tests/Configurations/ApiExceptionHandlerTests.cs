using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Api.Configurations;
using TaskManager.Application.DTOs;

namespace TaskManager.Tests.Configurations;

public class ApiExceptionHandlerTests
{
    private readonly Mock<ILogger<ApiExceptionHandler>> _logger = new Mock<ILogger<ApiExceptionHandler>>();

    [Fact]
    public async Task ShouldReturnBadRequestAndValidationErrors_WhenValidationExceptionOccurs()
    {
        ApiExceptionHandler handler = new ApiExceptionHandler(_logger.Object);
        DefaultHttpContext context = CreateContext();

        ValidationException exception = new ValidationException(
        [
            new ValidationFailure("Name", "Required"),
            new ValidationFailure("Email", "Invalid"),
            new ValidationFailure("Email", "Invalid")
        ]);

        bool handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        ResultResponse<object>? response = await ReadResponse(context);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal(["Required", "Invalid"], response!.Errors);
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUnauthorizedExceptionOccurs()
    {
        ApiExceptionHandler handler = new ApiExceptionHandler(_logger.Object);
        DefaultHttpContext context = CreateContext();

        bool handled = await handler.TryHandleAsync(
            context,
            new UnauthorizedAccessException(),
            CancellationToken.None
        );

        ResultResponse<object>? response = await ReadResponse(context);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.False(response!.IsSuccess);
    }

    [Fact]
    public async Task ShouldReturnInternalServerError_WhenUnexpectedExceptionOccurs()
    {
        ApiExceptionHandler handler = new ApiExceptionHandler(_logger.Object);
        DefaultHttpContext context = CreateContext();

        bool handled = await handler.TryHandleAsync(
            context,
            new InvalidOperationException("unexpected"),
            CancellationToken.None
        );

        ResultResponse<object>? response = await ReadResponse(context);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.False(response!.IsSuccess);
    }

    private static DefaultHttpContext CreateContext()
    {
        DefaultHttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        return context;
    }

    private static async Task<ResultResponse<object>?> ReadResponse(DefaultHttpContext context)
    {
        context.Response.Body.Position = 0;

        return await JsonSerializer.DeserializeAsync<ResultResponse<object>>(
            context.Response.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
    }
}
