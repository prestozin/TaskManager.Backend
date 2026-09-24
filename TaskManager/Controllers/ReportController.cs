using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Auth.Request;
using TaskManager.Application.DTOs.Auth.Response;
using TaskManager.Application.DTOs.Task.Report;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }


    [Authorize]
    [HttpGet("GetReport")]
    public async Task<IActionResult> GetReportAsync([FromQuery] ReportPagedParams reportParams)
    {
        var result = await _reportService.GetReportAsync(reportParams);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }
}
