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
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _userService.GetUser();

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPatch("UpdateProfile")]
    public async Task<IActionResult> UpdateProfile([FromBody] EditUserRequest request)
    {
        var result = await _userService.UpdateUser(request);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

}
