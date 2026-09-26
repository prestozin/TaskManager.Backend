using FluentValidation;
using TaskManager.Application.DTOs.User.Request;
using TaskManager.Core.Constants;

namespace TaskManager.Application.Validators.User;

public class DeleteUserValidator : AbstractValidator<DeleteUserRequest>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty() 
                .WithMessage(string.Format(Messages.FIELD_REQUIRED, "senha"))
            .MaximumLength(Constants.PASSWORD_MAX_LENGTH)
                .WithMessage(string.Format(Messages.FIELD_MAX_LENGTH, "senha", Constants.PASSWORD_MAX_LENGTH));
    }
}
