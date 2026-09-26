using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Shared;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.DTOs.Task.Report;

namespace TaskManager.Infra.Data.Repositories;

public class TaskRepository : BaseRepository<Task>, ITaskRepository
{
    private readonly ApplicationDbContext _context;
    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaskEntity?> GetTaskByIdAsync(Guid? taskId, Guid? userId)
    {
        return await _context.Tasks
            .Include(t => t.TaskStatus)
            .Include(t => t.TaskPriority)
            .SingleOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
    }

    public async Task CreateTaskAsync(TaskEntity task)
    {
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<TaskEntity> tasks, int totalCount)> GetPagedAsync(Guid userId, TaskPagedParams pagedParams)
    {
        IQueryable<TaskEntity> query = _context.Tasks
             .AsNoTracking()
             .Where(task => task.UserId == userId);

        query = ApplyFilters(query, pagedParams);

        int totalCount = await query.CountAsync();

        var tasks = await ApplySort(query, pagedParams.Sort, pagedParams.Order)
                            .Include(t => t.TaskStatus)
                            .Include(t => t.TaskPriority)
                            .Skip((pagedParams.PageNumber - 1) * pagedParams.PageSize)
                            .Take(pagedParams.PageSize)
                            .ToListAsync();

        return (tasks, totalCount);
    }

    private IQueryable<TaskEntity> ApplyFilters(IQueryable<TaskEntity> query, TaskPagedParams pagedParams)
    {
        if (pagedParams.StartDate.HasValue)
            query = query.Where(task => task.CreatedAt >= pagedParams.StartDate.Value);
        

        if (pagedParams.EndDate.HasValue)        
            query = query.Where(task => task.CreatedAt < pagedParams.EndDate.Value);
        

        if (!string.IsNullOrWhiteSpace(pagedParams.Search))        
            query = query.Where(task => task.Title.Contains(pagedParams.Search) || 
            (task.Description != null &&task.Description.Contains(pagedParams.Search)));
        

        if (pagedParams.TaskStatusId.HasValue)        
            query = query.Where(t => t.StatusId == pagedParams.TaskStatusId.Value);
        

        if (pagedParams.TaskPriorityId.HasValue)       
            query = query.Where(t => t.PriorityId == pagedParams.TaskPriorityId.Value);
        
        return query;
    }

    public async Task EditTaskAsync(TaskEntity task)
    {
       _context.Tasks.Update(task);
       await _context.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(TaskEntity task) 
    {
       _context.Tasks.Remove(task);
       await _context.SaveChangesAsync(); 
    }

    public async Task<List<Core.Entities.TaskStatus>> GetTaskStatusesAsync()
    {
        return await _context.Status
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<TaskPriority>> GetTaskPrioritiesAsync()
    {
        return await _context.Priorities
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskEntity>> GetReportAsync(Guid userId, ReportPagedParams reportParams)
    {
        var query = _context.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        query = ApplyReportFilters(query, reportParams);

        return await query
            .Include(t => t.TaskStatus)
            .Include(t => t.TaskPriority)
            .ToListAsync();
    }

    private IQueryable<TaskEntity> ApplyReportFilters(IQueryable<TaskEntity> query, ReportPagedParams reportParams)
    {
        if (reportParams.StartDate.HasValue)
            query = query.Where(task => task.CreatedAt >= reportParams.StartDate.Value);
        

        if (reportParams.EndDate.HasValue)
            query = query.Where(task =>task.CreatedAt < reportParams.EndDate.Value);
        
        return query;
    }
}
