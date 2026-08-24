using Microsoft.VisualBasic;
using TaskManager.Core.Constants;

namespace TaskManager.Core.Shared;

public class PagedParamsDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Sort { get; set; } = Constants.Constants.DEFAULT_TASK_SORT_VALUE;
    public string? Order { get; set; } = Constants.Constants.DEFAULT_ORDER_VALUE;

    private bool ValidateOrder(string? order)
    {
        var acceptedValues = Constants.Constants.ORDER_ACCEPTED_VALUES;

        if (string.IsNullOrWhiteSpace(order) || !acceptedValues.Contains(order))
            return false;

        return true;
    }

    private bool ValidateSort(string? sort)
    {
        var acceptedValues = Constants.Constants.SORT_ACCEPTED_VALUES;

        if (string.IsNullOrWhiteSpace(sort) || !acceptedValues.Contains(sort))
            return false;

        return true;
    }
}
