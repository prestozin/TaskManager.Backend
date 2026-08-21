namespace TaskManager.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string HashPassword { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ICollection<TaskEntity>? Tasks { get; set; } = [];
    }
}
