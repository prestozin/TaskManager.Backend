using TaskManager.Core.Entities;
using TaskManager.Core.Shared;

namespace TaskManager.Core.Interfaces;

public interface ITaskRepository
{
    Task CreateTaskAsync(TaskEntity task);
    Task<(IEnumerable<TaskEntity> tasks, int totalCount)> GetPagedAsync(Guid userId, TaskPagedParams pagedParams);
    Task<TaskEntity?> GetTaskByIdAsync(Guid? taskId, Guid? userId);
    Task EditTaskAsync(TaskEntity task);
    Task DeleteTasksAsync(List<TaskEntity> tasks);
    Task<List<Entities.TaskStatus>> GetTaskStatusesAsync();
    Task<List<TaskPriority>> GetTaskPrioritiesAsync();
    Task<IEnumerable<TaskEntity>> GetReportAsync(Guid userId, ReportPagedParams pagedParams);
    Task<List<TaskEntity>> GetTasksByIdsAsync(List<Guid> taskIds, Guid userId);
}
