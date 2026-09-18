namespace TaskManager.Application.DTOs.Task.Response;

public class TaskResponse
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } 
    public string? Status { get; set; }
    public string? Priority { get; set; }
}
