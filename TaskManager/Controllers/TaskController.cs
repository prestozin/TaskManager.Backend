using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Core.Shared;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [Authorize]
    [HttpGet("GetById")]
    public async Task<IActionResult> GetById(Guid taskId)
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _taskService.GetTaskById(taskId, userId);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("GetByTitle")]
    public async Task<IActionResult> GetByTitle(string title)
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _taskService.GetTasksByTitle(title, userId);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("GetPaged")]
    public async Task<IActionResult> GetPaged([FromQuery] TaskPagedParams pagedParams)
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _taskService.GetPaged(userId, pagedParams);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("AddTask")]
    public async Task<IActionResult> AddTask(CreateTaskDto task)
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _taskService.AddTaskAsync(task, userId);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPut("EditTask")]
    public async Task<IActionResult> EditTask(EditTaskDto dto)
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _taskService.EditTaskAsync(dto, userId);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("DeleteTask")]
    public async Task<IActionResult> DeleteTask(Guid taskId)
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _taskService.DeleteTaskAsync(taskId, userId);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("GetSelectables")]
    public async Task<IActionResult> GetSelectables()
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _taskService.GetSelectablesAsync();

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }
}

