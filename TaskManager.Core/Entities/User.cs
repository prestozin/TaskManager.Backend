using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string HashPassword { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(250)]
        public string? About { get; set; }

        [MaxLength(100)]
        public string? Area { get; set; }

        [MaxLength(100)]
        public string? Role { get; set; }

        public ICollection<TaskEntity> Tasks { get; set; } = [];
    }
}