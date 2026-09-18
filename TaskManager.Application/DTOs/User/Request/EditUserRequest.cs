namespace TaskManager.Application.DTOs.User.Request;

public class EditUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Role { get; set; }
    public string? Area { get; set; }
    public string? About { get; set; }
}
