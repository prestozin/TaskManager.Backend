using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskManager.Infra.Data;

namespace TaskManager.Tests.Infrastructure;

public class CurrentUserContextTests
{
    [Fact]
    public void ShouldReturnUserId_WhenNameIdentifierClaimIsValid()
    {
        Guid userId = Guid.NewGuid();

        DefaultHttpContext httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, userId.ToString())],
                "TestAuthentication"
            )
        };

        HttpContextAccessor accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        CurrentUserContext context = new CurrentUserContext(accessor);

        Assert.Equal(userId, context.UserId);
    }

    [Fact]
    public void ShouldThrowUnauthorized_WhenNameIdentifierClaimIsMissing()
    {
        HttpContextAccessor accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };

        CurrentUserContext context = new CurrentUserContext(accessor);

        Assert.Throws<UnauthorizedAccessException>(() => context.UserId);
    }

    [Fact]
    public void ShouldThrowUnauthorized_WhenNameIdentifierClaimIsInvalid()
    {
        DefaultHttpContext httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, "invalid-guid")],
                "TestAuthentication"
            )
        };

        HttpContextAccessor accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        CurrentUserContext context = new CurrentUserContext(accessor);

        Assert.Throws<UnauthorizedAccessException>(() => context.UserId);
    }
}
