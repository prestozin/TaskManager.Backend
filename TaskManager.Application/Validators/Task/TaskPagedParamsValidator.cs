using FluentValidation;
using TaskManager.Core.Constants;
using TaskManager.Core.Enums;
using TaskManager.Core.Shared;

namespace TaskManager.Application.Validators;

public class TaskPagedParamsValidator : AbstractValidator<TaskPagedParams>
{
    public TaskPagedParamsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
                .WithMessage(string.Format(Messages.FIELD_MINIMUM_VALUE, "página", 1));

        RuleFor(x => x.PageSize)
            .InclusiveBetween(Constants.MIN_PAGE_SIZE, Constants.MAX_PAGE_SIZE)
                .WithMessage(string.Format(Messages.FIELD_RANGE, "quantidade por página", Constants.MIN_PAGE_SIZE, Constants.MAX_PAGE_SIZE));

        RuleFor(x => x.Sort)
            .Must(IsValidSort)
                .WithMessage(string.Format(Messages.FIELD_INVALID, "ordenação"));

        RuleFor(x => x.Order)
            .Must(IsValidOrder)
                .WithMessage(string.Format(Messages.FIELD_INVALID, "direção da ordenação"));

        //RuleFor(x => x.TaskStatusId)
        //    .Must(statusId => !statusId.HasValue || statusIds.Contains(statusId.Value))
        //        .WithMessage(string.Format(Messages.FIELD_INVALID, "status"));

        //RuleFor(x => x.TaskPriorityId)
        //    .Must(priorityId => !priorityId.HasValue || priorityIds.Contains(priorityId.Value))
        //        .WithMessage(string.Format(Messages.FIELD_INVALID, "prioridade"));

        RuleFor(x => x.Search)
            .MaximumLength(Constants.TASK_SEARCH_MAX_LENGTH)
                .WithMessage(string.Format(Messages.FIELD_MAX_LENGTH, "pesquisa", Constants.TASK_SEARCH_MAX_LENGTH));

        RuleFor(x => x)
            .Must(HasValidDateRange)
                .WithMessage(Messages.INVALID_DATE_RANGE);
    }

    private static bool IsValidSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
            return false;

        return Enum.TryParse(sort,true, out ETaskSort taskSort) && Enum.IsDefined(taskSort);
    }

    private static bool IsValidOrder(string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return false;

        return Enum.TryParse(order, true, out ESortOrder sortOrder) && Enum.IsDefined(sortOrder);
    }

    private static bool HasValidDateRange(TaskPagedParams request)
    {
        if (!request.StartDate.HasValue || !request.EndDate.HasValue)
            return true;

        return request.StartDate.Value <= request.EndDate.Value;
    }
}
