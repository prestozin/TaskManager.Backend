
namespace TaskManager.Application.DTOs;

public class TaskBaseDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? StatusId { get; set; }
    public int? PriorityId { get; set; }
}
