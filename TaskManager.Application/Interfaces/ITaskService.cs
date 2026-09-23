using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Task.Report;
using TaskManager.Application.DTOs.Task.Request;
using TaskManager.Application.DTOs.Task.Response;
using TaskManager.Core.Entities;
using TaskManager.Core.Shared;

namespace TaskManager.Application.Interfaces;

public interface ITaskService
{
    Task<ResultResponse<TaskResponse>> GetTaskByIdAsync(Guid taskId);
    Task<ResultResponse<string>> CreateTaskAsync(CreateTaskRequest task);
    Task<ResultResponse<PagedResultDto<TaskResponse>>> GetPagedAsync(TaskPagedParams pagedParams);
    Task<ResultResponse<string>> EditTaskAsync(EditTaskRequest dto);
    Task<ResultResponse<string>> DeleteTaskAsync(DeleteTaskRequest request);
    Task<ResultResponse<TaskSelectablesResponse>> GetSelectablesAsync();

    Task<ResultResponse<TaskReportResponse>> GetReportAsync(ReportPagedParams pagedParams);
}
