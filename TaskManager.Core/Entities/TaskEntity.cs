using System.ComponentModel.DataAnnotations;
using TaskManager.Core.Enums;

namespace TaskManager.Core.Entities;

public class TaskEntity
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }

    public User? User { get; set; }

    public int StatusId { get; set; } 

    public TaskStatus TaskStatus { get; set; } = null!;

    public int PriorityId { get; set; }

    public TaskPriority TaskPriority { get; set; } = null!;
}