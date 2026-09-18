using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Task.Request;
using TaskManager.Application.DTOs.Task.Response;
using TaskManager.Core.Entities;
using TaskManager.Core.Shared;

namespace TaskManager.Application.Interfaces;

public interface ITaskService
{
    Task<ResultDto<TaskResponse>> GetTaskAsync(Guid taskId, Guid userId);
    Task<ResultDto<string>> AddTaskAsync(CreateTaskRequest task, Guid userId);
    Task<ResultDto<PagedResultDto<TaskResponse>>> GetPagedAsync(Guid userId, TaskPagedParams pagedParams);
    Task<ResultDto<string>> EditTaskAsync(EditTaskRequest dto, Guid userId);
    Task<ResultDto<string>> DeleteTaskAsync(DeleteTaskRequest request, Guid userId);
    Task<ResultDto<TaskSelectablesResponse>> GetSelectablesAsync();
}
