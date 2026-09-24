using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Task.Report;
using TaskManager.Application.DTOs.Task.Response;
using TaskManager.Application.Interfaces;
using TaskManager.Core.Constants;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;

public class ReportService : IReportService
{

    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private Guid UserId => _currentUserContext.UserId;


    public ReportService(ITaskRepository taskRepository, ICurrentUserContext currentUserContext)
    {
        _taskRepository = taskRepository;
        _currentUserContext = currentUserContext;
    }
    public async Task<ResultResponse<TaskReportResponse>> GetReportAsync(ReportPagedParams reportParams)
    {
        if (UserId == Guid.Empty || reportParams == null)
            return ResultResponse<TaskReportResponse>.Failure(Messages.TASK_NOT_FOUND);

        var tasks = (await _taskRepository.GetReportAsync(UserId, reportParams)).ToList();


        var report = new TaskReportResponse
        {
            TotalTasks = tasks.Count,
            Status = BuildReport(tasks, tasks.Count, true),
            Priority = BuildReport(tasks, tasks.Count, false)
        };

        return ResultResponse<TaskReportResponse>.Success(report);
    }

    private List<ReportCategoryResponse> BuildReport(IEnumerable<TaskEntity> tasks, int totalTasks, bool isStatus)
    {
        var statusReport = tasks
            .Select(task => new { Id = task.StatusId, Name = task.TaskStatus.Name });

        var priorityReport = tasks
            .Select(task => new { Id = task.PriorityId, Name = task.TaskPriority.Name });

        var report = isStatus ? statusReport : priorityReport;

        return report
            .GroupBy(item => new { item.Id, item.Name })
            .Select(group => BuildReportCategory(group.Key.Id, group.Key.Name, group.Count(), totalTasks))
            .ToList();
    }

    private ReportCategoryResponse BuildReportCategory(int id, string name, int count, int totalTasks)
    {
        return new ReportCategoryResponse
        {
            Id = id,
            Name = name,
            Count = count,
            Percentage = totalTasks == 0 ? 0 : Math.Round((decimal)count / totalTasks * 100, 2)
        };
    }
}
