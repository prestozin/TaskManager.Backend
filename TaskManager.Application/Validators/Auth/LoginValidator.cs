
using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;
using TaskManager.Core.Constants;

namespace TaskManager.Application.Validators.Auth;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage(string.Format(Messages.FIELD_REQUIRED, "e-mail"))
            .EmailAddress()
                .WithMessage(string.Format(Messages.FIELD_INVALID, "e-mail"))
            .MaximumLength(Constants.EMAIL_MAX_LENGTH)
                .WithMessage(string.Format(string.Format(Messages.FIELD_MAX_LENGTH, "e-mail",Constants.EMAIL_MAX_LENGTH)));

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage(string.Format(Messages.FIELD_REQUIRED, "senha"))
            .MaximumLength(Constants.PASSWORD_MAX_LENGTH)
                .WithMessage(string.Format(Messages.FIELD_MAX_LENGTH, "senha", Constants.PASSWORD_MAX_LENGTH));
    }
}
