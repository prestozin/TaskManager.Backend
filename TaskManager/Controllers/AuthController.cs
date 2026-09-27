using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TaskManager.Application.DTOs.Auth.Request;
using TaskManager.Application.Interfaces;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


    [HttpPost("Register")]
    public async Task<IActionResult> CreateUserAsync(CreateUserRequest dto)
    {
       var result = await _authService.CreateUserAsync(dto);

        if(!result.IsSuccess)
            return Conflict(result);

        return Ok(result);
    }

    [EnableRateLimiting("Auth")]
    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (!result.IsSuccess)
            return Unauthorized(result);

        return Ok(result);
    }
}
