using FluentValidation;
using TaskManager.Application.DTOs;
using TaskManager.Application.Validators.Task;
using TaskManager.Core.Constants;
using TaskManager.Core.Enums;

namespace TaskManager.Application.Validators;

public class EditTaskValidator : BaseTaskValidator<EditTaskDto>
{
    public EditTaskValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
                .WithMessage(Messages.TaskIdRequired);

        RuleFor(x => x.StatusId)
            .Must(ValidateStatusId)
                .WithMessage(Messages.TaskStatusInvalid);
    }

    private bool ValidateStatusId(int? statusId)
    {
        if (!statusId.HasValue)
            return true; 

        return Enum.IsDefined(typeof(ETaskStatus), statusId.Value);
    }
}
