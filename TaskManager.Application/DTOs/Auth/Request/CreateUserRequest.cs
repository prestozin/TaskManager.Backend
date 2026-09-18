using TaskManager.Application.DTOs.Auth.Response;

namespace TaskManager.Application.DTOs.Auth.Request;

public class CreateUserRequest : AuthBaseResponse
{
    public string Name { get; set; } = string.Empty;
}
