using FluentValidation;
using TaskManager.Application.DTOs.Task.Request;
using TaskManager.Application.Validators.Task;
using TaskManager.Core.Constants;
using TaskManager.Core.Enums;

namespace TaskManager.Application.Validators;

public class EditTaskValidator : BaseTaskValidator<EditTaskRequest>
{
    public EditTaskValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
                .WithMessage(Messages.TASK_ID_REQUIRED);
    }
}
