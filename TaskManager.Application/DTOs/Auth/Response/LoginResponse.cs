namespace TaskManager.Application.DTOs.Auth.Response;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Name {get; set;} = string.Empty;
}
