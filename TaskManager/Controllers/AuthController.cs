using TaskManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Auth.Response;
using TaskManager.Application.DTOs.Auth.Request;

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
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginResponse dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }
}
