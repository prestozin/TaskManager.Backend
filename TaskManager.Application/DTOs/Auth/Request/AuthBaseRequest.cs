namespace TaskManager.Application.DTOs.Auth.Request;

public class AuthBaseRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
