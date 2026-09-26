namespace TaskManager.Application.DTOs.Auth.Request;

public class CreateUserRequest : AuthBaseRequest
{
    public string Name { get; set; } = string.Empty;
}
