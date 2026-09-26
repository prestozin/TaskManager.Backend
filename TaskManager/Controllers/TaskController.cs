using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Task.Report;
using TaskManager.Application.DTOs.Task.Request;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
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
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid taskId)
    {
        var result = await _taskService.GetTaskByIdAsync(taskId);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("GetPaged")]
    public async Task<IActionResult> GetPagedAsync([FromQuery] TaskPagedParams request)
    {
        var result = await _taskService.GetPagedAsync(request);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("CreateTask")]
    public async Task<IActionResult> CreateTaskAsync(CreateTaskRequest request)
    {
        var result = await _taskService.CreateTaskAsync(request);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPut("EditTask")]
    public async Task<IActionResult> EditTaskAsync(EditTaskRequest request)
    {
        var result = await _taskService.EditTaskAsync(request);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("DeleteTask")]
    public async Task<IActionResult> DeleteTaskAsync([FromBody] DeleteTaskRequest request)
    {
        var result = await _taskService.DeleteTaskAsync(request);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("GetSelectables")]
    public async Task<IActionResult> GetSelectablesAsync()
    {
        var result = await _taskService.GetSelectablesAsync();

        return Ok(result);
    }


    [Authorize]
    [HttpGet("GetReport")]
    public async Task<IActionResult> GetReportAsync([FromQuery] ReportPagedParams reportParams)
    {
        var result = await _taskService.GetReportAsync(reportParams);

        return Ok(result);
    }
}

