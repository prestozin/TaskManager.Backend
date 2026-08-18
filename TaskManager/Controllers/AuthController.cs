using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Register(CreateUserDto dto)
    {
       var result = await _authService.RegisterAsync(dto);

        if(!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }
}
