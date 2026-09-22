namespace TaskManager.Application.Validators.User;

public class ChangeUserPasswordRequest
{
    public string? OldPassword { get; set; }
    public string? NewPassword { get; set; }
}
