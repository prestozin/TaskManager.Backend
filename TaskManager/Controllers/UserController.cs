using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Application.DTOs.User.Request;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }



    [Authorize]
    [HttpGet("GetUser")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _userService.GetUser();

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPut("EditUser")]
    public async Task<IActionResult> EditUser([FromBody] EditUserRequest request)
    {
        var result = await _userService.EditUser(request);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

}
