using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Task.Report;
using TaskManager.Application.DTOs.Task.Response;

namespace TaskManager.Application.Interfaces;

public interface IReportService
{
    Task<ResultResponse<TaskReportResponse>> GetReportAsync(ReportPagedParams pagedParams);
}
