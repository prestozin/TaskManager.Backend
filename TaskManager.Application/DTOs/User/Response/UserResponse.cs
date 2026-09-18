namespace TaskManager.Application.DTOs.User.Response;

public class UserResponse
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Role { get; set; }
    public string? Area { get; set; }
    public string? About { get; set; }
    public DateTime CreatedAt { get; set; }
}
