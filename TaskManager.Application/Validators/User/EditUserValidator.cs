using FluentValidation;
using TaskManager.Application.DTOs.User.Request;
using TaskManager.Core.Constants;

namespace TaskManager.Application.Validators.User;

public class EditUserValidator : AbstractValidator<EditUserRequest>
{
    public EditUserValidator()
    {
        RuleFor(x => x.Name)
           .Cascade(CascadeMode.Stop)
           .Must(name => !string.IsNullOrWhiteSpace(name))
               .WithMessage(string.Format(Messages.FIELD_REQUIRED,"nome"))
           .Length(Constants.NAME_MIN_LENGTH, Constants.NAME_MAX_LENGTH)
               .WithMessage(string.Format(Messages.FIELD_LENGTH,"nome", Constants.NAME_MIN_LENGTH, Constants.NAME_MAX_LENGTH));

        RuleFor(x => x.Role)
            .MaximumLength(Constants.ROLE_MAX_LENGTH)
                .WithMessage(string.Format(Messages.FIELD_MAX_LENGTH, "cargo", Constants.ROLE_MAX_LENGTH));

        RuleFor(x => x.Area)
            .MaximumLength(Constants.AREA_MAX_LENGTH)
                .WithMessage(string.Format(Messages.FIELD_MAX_LENGTH, "área de atuação",Constants.AREA_MAX_LENGTH));

        RuleFor(x => x.About)
            .MaximumLength(Constants.ABOUT_MAX_LENGTH)
                .WithMessage(string.Format(Messages.FIELD_MAX_LENGTH, "sobre mim",Constants.ABOUT_MAX_LENGTH));
    }
}
