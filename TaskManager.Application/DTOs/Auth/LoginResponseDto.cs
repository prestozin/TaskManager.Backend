namespace TaskManager.Application.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Name {get; set;} = string.Empty;
}
