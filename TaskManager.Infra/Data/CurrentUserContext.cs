using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TaskManager.Core.Constants;
using TaskManager.Core.Interfaces;

namespace TaskManager.Infra.Data;

public class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var id))
                throw new UnauthorizedAccessException(Messages.UNAUTHORIZED);

            return id;
        }
    }
}
