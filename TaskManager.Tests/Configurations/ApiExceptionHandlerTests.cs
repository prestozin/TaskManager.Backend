using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Api.Configurations;

namespace TaskManager.Tests.Configurations;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class ApiExceptionHandlerTests
{
    private readonly Mock<ILogger<ApiExceptionHandler>> _logger = new Mock<ILogger<ApiExceptionHandler>>();

    [Test]
    public async Task ShouldReturnBadRequestAndValidationErrors_WhenValidationExceptionOccurs()
    {
        // Arrange
        ApiExceptionHandler handler = new ApiExceptionHandler(_logger.Object);
        DefaultHttpContext context = CreateContext();

        ValidationException exception = new ValidationException(
        [
            new ValidationFailure("Name", "Required"),
            new ValidationFailure("Email", "Invalid"),
            new ValidationFailure("Email", "Invalid")
        ]);

        // Act
        bool handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        using JsonDocument response = await ReadResponse(context);
        JsonElement errors = response.RootElement.GetProperty("errors");

        // Assert
        Assert.That(handled, Is.True);
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(errors.GetArrayLength(), Is.EqualTo(2));
        Assert.That(errors[0].GetString(), Is.EqualTo("Required"));
        Assert.That(errors[1].GetString(), Is.EqualTo("Invalid"));
    }

    [Test]
    public async Task ShouldReturnUnauthorized_WhenUnauthorizedExceptionOccurs()
    {
        // Arrange
        ApiExceptionHandler handler = new ApiExceptionHandler(_logger.Object);
        DefaultHttpContext context = CreateContext();

        // Act
        bool handled = await handler.TryHandleAsync(
            context,
            new UnauthorizedAccessException(),
            CancellationToken.None
        );

        using JsonDocument response = await ReadResponse(context);

        // Assert
        Assert.That(handled, Is.True);
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
        Assert.That(response.RootElement.GetProperty("isSuccess").GetBoolean(), Is.False);
    }

    [Test]
    public async Task ShouldReturnInternalServerError_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        ApiExceptionHandler handler = new ApiExceptionHandler(_logger.Object);
        DefaultHttpContext context = CreateContext();

        // Act
        bool handled = await handler.TryHandleAsync(
            context,
            new InvalidOperationException("unexpected"),
            CancellationToken.None
        );

        using JsonDocument response = await ReadResponse(context);

        // Assert
        Assert.That(handled, Is.True);
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        Assert.That(response.RootElement.GetProperty("isSuccess").GetBoolean(), Is.False);
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
