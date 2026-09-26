using FluentValidation;
using TaskManager.Application.DTOs.User.Request;
using TaskManager.Core.Constants;


namespace TaskManager.Application.Validators.User;

public class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPasswordRequest>
{
    public ChangeUserPasswordValidator()
    {
        RuleFor(x => x.OldPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage(string.Format(Messages.FIELD_REQUIRED, "senha atual"))
            .MaximumLength(Constants.PASSWORD_MAX_LENGTH)
                .WithMessage(string.Format(Messages.FIELD_MAX_LENGTH, "senha atual", Constants.PASSWORD_MAX_LENGTH));

        RuleFor(x => x.NewPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage(string.Format(Messages.FIELD_REQUIRED, "nova senha"))
            .Matches(Constants.PASSWORD_REGEX)
                .WithMessage(Messages.PASSWORD_RULES);
    }

}
