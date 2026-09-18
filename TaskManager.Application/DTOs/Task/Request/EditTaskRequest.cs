namespace TaskManager.Application.DTOs.Task.Request;

public class EditTaskRequest : BaseTaskRequest
{
    public Guid? Id { get; set; }
}
