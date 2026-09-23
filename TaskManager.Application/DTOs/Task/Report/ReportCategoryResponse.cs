namespace TaskManager.Application.DTOs.Task.Report;

public class ReportCategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}
