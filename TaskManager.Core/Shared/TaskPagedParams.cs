
namespace TaskManager.Core.Shared;

public class TaskPagedParams : PagedParamsDto
{
    public int? TaskStatusId { get; set; }
    public int? TaskPriorityId { get; set; }
    public TaskPagedParams()
    {
        Sort  = Constants.Constants.DEFAULT_TASK_SORT_VALUE;
        Order  = Constants.Constants.DEFAULT_ORDER_VALUE;
    }
}
