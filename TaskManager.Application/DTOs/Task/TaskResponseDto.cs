namespace TaskManager.Application.DTOs;

public class TaskResponseDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
    public string? Status { get; set; }
    public string? Priority { get; set; }
}
