namespace TaskManager.Application.DTOs.User.Request;

public class ChangeUserPasswordRequest
{
    public string? OldPassword { get; set; }
    public string? NewPassword { get; set; }
}
