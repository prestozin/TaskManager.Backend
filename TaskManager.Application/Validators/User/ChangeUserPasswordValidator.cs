using FluentValidation;


namespace TaskManager.Application.Validators.User;

public class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPasswordRequest>
{
    public ChangeUserPasswordValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(6);
    }

}
