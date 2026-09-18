using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core.Entities;

public class TaskPriority
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}