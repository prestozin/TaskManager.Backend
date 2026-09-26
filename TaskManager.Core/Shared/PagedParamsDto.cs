using Microsoft.VisualBasic;
using TaskManager.Core.Constants;

namespace TaskManager.Core.Shared;

public class PagedParamsDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Sort { get; set; } 
    public string? Order { get; set; } 
}
