using FluentValidation;
using TaskManager.Application.DTOs;
using TaskManager.Core.Constants;
using TaskManager.Core.Enums;

namespace TaskManager.Application.Validators.Task;

public class BaseTaskValidator<T> : AbstractValidator<T> where T : BaseTaskDto
{
    protected void SetupCommonRules()
    {
        RuleFor(x => x.Title)
         .NotEmpty()
             .WithMessage(Messages.TASK_TITLE_REQUIRED)
         .Length(Constants.TASK_TITLE_MIN_LENGTH, Constants.TASK_TITLE_MAX_LENGTH)
             .WithMessage(Messages.TASK_TITLE_LENGTH);

        RuleFor(x => x.Description)
            .MaximumLength(Constants.TASK_DESCRIPTION_MAX_LENGTH)
                  .WithMessage(Messages.TASK_DESCRIPTION_MAX_LENGTH);

        RuleFor(x => x.StatusId)
         .Must(ValidateStatus)
             .WithMessage(Messages.TASK_STATUS_INVALID);

        RuleFor(x => x.PriorityId)
        .Must(ValidatePriority)
            .WithMessage(Messages.TASK_PRIORITY_INVALID);
    }

    public BaseTaskValidator()
    {
        SetupCommonRules();
    }

    private bool ValidateStatus(int? statusId)
    {
        if (!statusId.HasValue)
            return true;

        return Enum.IsDefined(typeof(ETaskStatus), statusId.Value);
    }

    private bool ValidatePriority(int? priorityId)
    {
        if (!priorityId.HasValue)
            return true;

        return Enum.IsDefined(typeof(ETaskPriority), priorityId.Value);
    }
}
