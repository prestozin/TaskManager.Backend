using TaskManager.Application.DTOs;
using TaskManager.Core.Entities;
using TaskManager.Core.Shared;

namespace TaskManager.Application.Interfaces;

public interface ITaskService
{
    Task<ResultDto<List<TaskResponseDto>>> GetTasksByTitle(string title, Guid userId);
    Task<ResultDto<TaskResponseDto>> GetTaskById(Guid taskId, Guid userId);
    Task<ResultDto<TaskEntity>> AddTaskAsync(CreateTaskDto task, Guid userId);
    Task<ResultDto<PagedResultDto<TaskResponseDto>>> GetPaged(Guid userId, TaskPagedParams pagedParams);
    Task<ResultDto<TaskResponseDto>> EditTaskAsync(EditTaskDto dto, Guid userId);
    Task<ResultDto<string>> DeleteTaskAsync(Guid taskId, Guid userId);
}
