using FluentValidation;
using TaskManager.Application.DTOs.Task.Request;
using TaskManager.Core.Constants;

namespace TaskManager.Application.Validators.Task
{
    public class DeleteTaskValidator : AbstractValidator<DeleteTaskRequest>
    {
        public DeleteTaskValidator()
        {
            RuleFor(x => x.TaskId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
                .WithMessage(string.Format(Messages.FIELD_REQUIRED, "tarefas"))
            .NotEmpty()
                .WithMessage(string.Format(Messages.AT_LEAST_ONE_ITEM, "tarefas"))
            .Must(taskIds => taskIds!.All(taskId => taskId != Guid.Empty))
                .WithMessage(string.Format(Messages.FIELD_INVALID, "id da tarefa"))
            .Must(taskIds => taskIds!.Distinct().Count() == taskIds.Count)
                .WithMessage(string.Format(Messages.DUPLICATE_ITEMS_NOT_ALLOWED, "tarefas"))
            .Must(taskIds => taskIds!.Count <= Constants.MAX_TASK_DELETE_BATCH_SIZE)
                .WithMessage(string.Format(Messages.MAXIMUM_ITEMS, "tarefas", Constants.MAX_TASK_DELETE_BATCH_SIZE));
        }
    }
}
