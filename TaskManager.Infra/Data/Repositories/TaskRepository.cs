using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Shared;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Infra.Data.Repositories;

public class TaskRepository : BaseRepository<Task>, ITaskRepository
{
    private readonly ApplicationDbContext _context;
    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskEntity>> GetTaskByTitle(string title, Guid userId)
    {
       return await _context.Tasks
            .Where(t => t.Title != null && t.Title.Contains(title))
            .Where(u => u.UserId == userId)
            .ToListAsync();
    }

    public async Task<TaskEntity?> GetTaskById(Guid? taskId, Guid userId)
    {
        return await _context.Tasks
            .Include(t => t.TaskStatus)
            .Include(t => t.TaskPriority)
            .SingleOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
    }

    public async Task AddTaskAsync(TaskEntity task)
    {
        await _context.AddAsync(task);
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<TaskEntity> tasks, int totalCount)> GetAllTasks(Guid userId, PagedParamsDto pagedParams)
    {
        var query = _context.Tasks.Where(t => t.UserId == userId);

        int totalCount = await query.CountAsync();

        var tasks = await ApplySort(query, pagedParams.Sort, pagedParams.Order)
                            .Include(t => t.TaskStatus)
                            .Include(t => t.TaskPriority)
                            .Skip((pagedParams.PageNumber - 1) * pagedParams.PageSize)
                            .Take(pagedParams.PageSize)
                            .ToListAsync();

        return (tasks, totalCount);
    }

    public async Task EditTaskAsync(TaskEntity task)
    {
       _context.Update(task);
       await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteTaskAsync(TaskEntity task) 
    {
       var taskToDelete =  _context.Remove(task);

       await _context.SaveChangesAsync();

       return taskToDelete != null;
    }
}
