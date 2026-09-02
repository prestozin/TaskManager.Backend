using TaskManager.Core.Entities;

namespace TaskManager.Application.DTOs.Task;

public class TaskSelectablesDto
{
    public List<Core.Entities.TaskStatus>? Status { get; set; }
    public List<TaskPriority>? Priority { get; set; }
}
