using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Task.Request;
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
    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetById([FromRoute] Guid taskId)
    {
        var result = await _taskService.GetTaskAsync(taskId);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("GetPaged")]
    public async Task<IActionResult> GetPaged([FromQuery] TaskPagedParams request)
    {
        var result = await _taskService.GetPagedAsync(request);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("AddTask")]
    public async Task<IActionResult> AddTask(CreateTaskRequest request)
    {
        var result = await _taskService.AddTaskAsync(request);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPut("EditTask")]
    public async Task<IActionResult> EditTask(EditTaskRequest request)
    {
        var result = await _taskService.EditTaskAsync(request);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("DeleteTask")]
    public async Task<IActionResult> DeleteTask([FromBody] DeleteTaskRequest request)
    {
        var result = await _taskService.DeleteTaskAsync(request);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("GetSelectables")]
    public async Task<IActionResult> GetSelectables()
    {
        var result = await _taskService.GetSelectablesAsync();

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }
}

