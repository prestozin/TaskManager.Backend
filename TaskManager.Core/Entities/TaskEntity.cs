using TaskManager.Core.Enums;

namespace TaskManager.Core.Entities
{
    public class TaskEntity
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public int StatusId { get; set; } = (int)ETaskStatus.Pending;
        public int PriorityId { get; set; } = (int)ETaskPriority.Media;
        public TaskStatus? TaskStatus { get; set; }
        public TaskPriority? TaskPriority { get; set; }
    }
}
