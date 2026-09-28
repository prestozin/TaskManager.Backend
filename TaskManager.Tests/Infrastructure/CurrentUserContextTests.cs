using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskManager.Infra.Data;

namespace TaskManager.Tests.Infrastructure;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class CurrentUserContextTests
{
    [Test]
    public void ShouldReturnUserId_WhenNameIdentifierClaimIsValid()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        DefaultHttpContext httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, userId.ToString())],
                    "TestAuthentication"
                )
            )
        };

        HttpContextAccessor accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        CurrentUserContext context = new CurrentUserContext(accessor);

        // Act
        Guid result = context.UserId;

        // Assert
        Assert.That(result, Is.EqualTo(userId));
    }

    [Test]
    public void ShouldThrowUnauthorized_WhenNameIdentifierClaimIsMissing()
    {
        // Arrange
        HttpContextAccessor accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };

        CurrentUserContext context = new CurrentUserContext(accessor);

        // Act
        TestDelegate action = () => _ = context.UserId;

        // Assert
        Assert.Throws<UnauthorizedAccessException>(action);
    }

    [Test]
    public void ShouldThrowUnauthorized_WhenNameIdentifierClaimIsInvalid()
    {
        // Arrange
        DefaultHttpContext httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, "invalid-guid")],
                    "TestAuthentication"
                )
            )
        };

        HttpContextAccessor accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        CurrentUserContext context = new CurrentUserContext(accessor);

        // Act
        TestDelegate action = () => _ = context.UserId;

        // Assert
        Assert.Throws<UnauthorizedAccessException>(action);
    }
}
