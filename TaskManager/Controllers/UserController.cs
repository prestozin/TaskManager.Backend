using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.User.Request;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Validators.User;

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
    public async Task<IActionResult> GetUserAsync()
    {
        var result = await _userService.GetUserAsync();

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPut("EditUser")]
    public async Task<IActionResult> EditUserAsync([FromBody] EditUserRequest request)
    {
        var result = await _userService.EditUserAsync(request);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("DeleteUser")]
    public async Task<IActionResult> DeleteUserAsync(string password)
    {
        var result = await _userService.DeleteUserAsync(password);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPatch("ChangePassword")]
    public async Task<IActionResult> ChangePasswordAsync(ChangeUserPasswordRequest request)
    {
        var result = await _userService.ChangePasswordAsync(request);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

}
