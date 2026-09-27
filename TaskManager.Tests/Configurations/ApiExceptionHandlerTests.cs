using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Api.Configurations;

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

        using JsonDocument response = await ReadResponse(context);
        JsonElement errors = response.RootElement.GetProperty("errors");

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal(2, errors.GetArrayLength());
        Assert.Equal("Required", errors[0].GetString());
        Assert.Equal("Invalid", errors[1].GetString());
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

        using JsonDocument response = await ReadResponse(context);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.False(response.RootElement.GetProperty("isSuccess").GetBoolean());
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

        using JsonDocument response = await ReadResponse(context);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.False(response.RootElement.GetProperty("isSuccess").GetBoolean());
    }

    private static DefaultHttpContext CreateContext()
    {
        DefaultHttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        return context;
    }

    private static async Task<JsonDocument> ReadResponse(DefaultHttpContext context)
    {
        context.Response.Body.Position = 0;

        return await JsonDocument.ParseAsync(context.Response.Body);
    }
}
