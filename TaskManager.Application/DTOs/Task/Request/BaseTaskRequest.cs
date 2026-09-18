namespace TaskManager.Application.DTOs.Task.Request;

public class BaseTaskRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? StatusId { get; set; }
    public int? PriorityId { get; set; }
}
