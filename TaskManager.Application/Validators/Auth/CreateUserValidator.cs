using FluentValidation;
using TaskManager.Application.DTOs;
using TaskManager.Core.Constants;

namespace TaskManager.Application.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name)
          .NotEmpty()
                .WithMessage(Messages.NAME_REQUIRED)
          .Length(Constants.NAME_MIN_LENGTH, Constants.NAME_MAX_LENGTH)
                .WithMessage(Messages.NAME_LENGTH);

        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage(Messages.EMAIL_REQUIRED)
            .EmailAddress()
                .WithMessage(Messages.EMAIL_INVALID)
            .MaximumLength(Constants.EMAIL_MAX_LENGTH)
                .WithMessage(Messages.EMAIL_MAX_LENGTH);

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage(Messages.PASSWORD_REQUIRED)
           .Matches(Constants.PASSWORD_REGEX)
                .WithMessage(Messages.PASSWORD_RULES);
    }
}
