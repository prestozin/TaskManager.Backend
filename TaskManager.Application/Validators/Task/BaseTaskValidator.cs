using FluentValidation;
using TaskManager.Application.DTOs.Task.Request;
using TaskManager.Core.Constants;
using TaskManager.Core.Enums;

namespace TaskManager.Application.Validators.Task;

public class BaseTaskValidator<T> : AbstractValidator<T> where T : BaseTaskRequest
{
    protected void SetupCommonRules()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .Must(title => !string.IsNullOrWhiteSpace(title))
                .WithMessage(string.Format(Messages.FIELD_REQUIRED, "título"))
            .Length(Constants.TASK_TITLE_MIN_LENGTH, Constants.TASK_TITLE_MAX_LENGTH)
                .WithMessage(string.Format(Messages.FIELD_LENGTH, "título", Constants.TASK_TITLE_MIN_LENGTH,Constants.TASK_TITLE_MAX_LENGTH));

        RuleFor(x => x.Description)
            .MaximumLength(Constants.TASK_DESCRIPTION_MAX_LENGTH)
                .WithMessage(string.Format(Messages.FIELD_MAX_LENGTH, "descrição", Constants.TASK_DESCRIPTION_MAX_LENGTH));

        RuleFor(x => x.StatusId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
                .WithMessage(string.Format(Messages.FIELD_REQUIRED, "status"))
            .Must(statusId => statusId.HasValue && Enum.IsDefined(typeof(ETaskStatus), statusId.Value))
                .WithMessage(string.Format(Messages.FIELD_REQUIRED, "status"));

        RuleFor(x => x.PriorityId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
                .WithMessage(string.Format(Messages.FIELD_REQUIRED, "prioridade"))
            .Must(priorityId => priorityId.HasValue && Enum.IsDefined(typeof(ETaskPriority), priorityId.Value))
                .WithMessage(string.Format(Messages.FIELD_REQUIRED, "prioridade"));
    }

    public BaseTaskValidator()
    {
        SetupCommonRules();
    }


}
