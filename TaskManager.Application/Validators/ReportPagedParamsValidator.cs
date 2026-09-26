using FluentValidation;
using TaskManager.Application.DTOs.Task.Report;
using TaskManager.Core.Constants;

namespace TaskManager.Application.Validators;

public class ReportPagedParamsValidator : AbstractValidator<ReportPagedParams>
{
    public ReportPagedParamsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
                .WithMessage(string.Format(Messages.FIELD_MIN_LENGTH, "página", "1"));

        RuleFor(x => x.PageSize)
            .InclusiveBetween(Constants.MIN_PAGE_SIZE, Constants.MAX_PAGE_SIZE)
                .WithMessage(string.Format(Messages.FIELD_RANGE, "quantidade por página",Constants.MIN_PAGE_SIZE,Constants.MAX_PAGE_SIZE));

        RuleFor(x => x)
            .Must(HasValidDateRange)
                .WithMessage(Messages.INVALID_DATE_RANGE);
    }

    private static bool HasValidDateRange(ReportPagedParams request)
    {
        if (!request.StartDate.HasValue || !request.EndDate.HasValue)
            return true;

        return request.StartDate.Value <= request.EndDate.Value;
    }
}
