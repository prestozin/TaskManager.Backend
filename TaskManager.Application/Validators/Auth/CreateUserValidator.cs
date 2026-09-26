using FluentValidation;
using TaskManager.Application.DTOs.Auth.Request;
using TaskManager.Core.Constants;

namespace TaskManager.Application.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage(string.Format(Messages.FIELD_REQUIRED, "nome"))
            .Length(Constants.NAME_MIN_LENGTH, Constants.NAME_MAX_LENGTH)
                .WithMessage(string.Format(Messages.FIELD_REQUIRED, "nome", Constants.NAME_MIN_LENGTH, Constants.NAME_MAX_LENGTH));

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage(string.Format(Messages.FIELD_REQUIRED, "e-mail"))
            .EmailAddress()
                .WithMessage(string.Format(Messages.FIELD_INVALID, "e-mail"))
            .MaximumLength(Constants.EMAIL_MAX_LENGTH)
                .WithMessage(string.Format(Messages.FIELD_MAX_LENGTH, "e-mail", Constants.EMAIL_MAX_LENGTH));

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage(string.Format(Messages.FIELD_REQUIRED, "senha"))
            .Matches(Constants.PASSWORD_REGEX)
                .WithMessage(Messages.PASSWORD_RULES);
    }
}
