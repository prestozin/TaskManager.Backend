using TaskManager.Core.Entities;
using TaskManager.Core.Shared;

namespace TaskManager.Core.Interfaces;

public interface ITaskRepository
{
    Task<List<TaskEntity>> GetTaskByTitle(string title, Guid userId);
    Task AddTaskAsync(TaskEntity task);
    Task<(IEnumerable<TaskEntity> tasks, int totalCount)> GetAllTasks(Guid userId, PagedParamsDto pagedParams);
    Task<TaskEntity> GetTaskById(Guid? taskId, Guid userId);
    Task EditTaskAsync(TaskEntity task);
    Task<bool> DeleteTaskAsync(TaskEntity task);
}
