using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetById([FromRoute] Guid userId)
    {
        var result = await _userService.GetUserById(userId);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

}
