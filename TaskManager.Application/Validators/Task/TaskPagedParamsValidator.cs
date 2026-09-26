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

        RuleFor(x => x.TaskStatusId)
            .Must(IsValidStatus)
                .WithMessage(string.Format(Messages.FIELD_INVALID, "status"));

        RuleFor(x => x.TaskPriorityId)
            .Must(IsValidPriority)
                .WithMessage(string.Format(Messages.FIELD_INVALID, "prioridade"));

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

        return Enum.TryParse<ETaskSort>(sort, true, out _);
    }

    private static bool IsValidOrder(string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return false;

        return Enum.TryParse<ESortOrder>(order, true, out _);
    }

    private static bool IsValidStatus(int? statusId)
    {
        if (!statusId.HasValue)
            return true;

        return Enum.IsDefined(typeof(ETaskStatus), statusId.Value);
    }

    private static bool IsValidPriority(int? priorityId)
    {
        if (!priorityId.HasValue)
            return true;

        return Enum.IsDefined(typeof(ETaskPriority), priorityId.Value);
    }

    private static bool HasValidDateRange(TaskPagedParams request)
    {
        if (!request.StartDate.HasValue || !request.EndDate.HasValue)
            return true;

        return request.StartDate.Value <= request.EndDate.Value;
    }
}
