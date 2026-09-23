using TaskManager.Application.DTOs.Task.Report;

namespace TaskManager.Application.DTOs.Task.Response;

public class TaskReportResponse
{
    public int TotalTasks { get; set; }
    public List<ReportCategoryResponse> Status { get; set; } = [];
    public List<ReportCategoryResponse> Priority { get; set; } = [];
}
