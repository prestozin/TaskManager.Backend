using TaskManager.Core.Entities;

namespace TaskManager.Application.DTOs.Task.Response;

public class TaskSelectablesResponse
{
    public List<Core.Entities.TaskStatus>? Status { get; set; }
    public List<TaskPriority>? Priority { get; set; }
}
