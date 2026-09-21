using TaskManager.Core.Entities;
using TaskManager.Core.Shared;

namespace TaskManager.Core.Interfaces;

public interface ITaskRepository
{
    Task CreateTaskAsync(TaskEntity task);
    Task<(IEnumerable<TaskEntity> tasks, int totalCount)> GetPaged(Guid userId, TaskPagedParams pagedParams);
    Task<TaskEntity?> GetTaskByIdAsync(Guid? taskId, Guid? userId);
    Task EditTaskAsync(TaskEntity task);
    Task DeleteTaskAsync(TaskEntity task);
    Task<List<Entities.TaskStatus>> GetTaskStatusesAsync();
    Task<List<TaskPriority>> GetTaskPrioritiesAsync();
}
