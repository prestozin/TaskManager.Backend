using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Shared;
using TaskManager.Infra.Data;
using TaskManager.Infra.Data.Repositories;
using TaskStatusEntity = TaskManager.Core.Entities.TaskStatus;

namespace TaskManager.Tests.Repositories;

public class TaskRepositoryTests
{
    [Fact]
    public async Task ShouldReturnTask_WhenTaskBelongsToUser()
    {
        Guid userId = Guid.NewGuid();
        Guid otherUserId = Guid.NewGuid();
        Guid taskId = Guid.NewGuid();

        await using ApplicationDbContext context = CreateContext();

        SeedSelectables(context);

        context.Tasks.AddRange(
            CreateTask(taskId, userId, "Owned task", 1, 1),
            CreateTask(Guid.NewGuid(), otherUserId, "Other task", 1, 1)
        );

        await context.SaveChangesAsync();

        TaskRepository repository = new TaskRepository(context);

        var result = await repository.GetTaskByIdAsync(taskId, userId);

        Assert.NotNull(result);
        Assert.Equal(taskId, result!.Id);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task ShouldReturnNull_WhenTaskBelongsToAnotherUser()
    {
        Guid ownerId = Guid.NewGuid();
        Guid requesterId = Guid.NewGuid();
        Guid taskId = Guid.NewGuid();

        await using ApplicationDbContext context = CreateContext();

        SeedSelectables(context);
        context.Tasks.Add(CreateTask(taskId, ownerId, "Private task", 1, 1));
        await context.SaveChangesAsync();

        TaskRepository repository = new TaskRepository(context);

        var result = await repository.GetTaskByIdAsync(taskId, requesterId);

        Assert.Null(result);
    }

    [Fact]
    public async Task ShouldFilterAndPaginateTasks_WhenPagedQueryIsProvided()
    {
        Guid userId = Guid.NewGuid();
        Guid otherUserId = Guid.NewGuid();

        await using ApplicationDbContext context = CreateContext();

        SeedSelectables(context);

        context.Tasks.AddRange(
            CreateTask(Guid.NewGuid(), userId, "Alpha task", 1, 3, new DateTime(2026, 9, 10)),
            CreateTask(Guid.NewGuid(), userId, "Beta task", 1, 3, new DateTime(2026, 9, 11)),
            CreateTask(Guid.NewGuid(), userId, "Ignored priority", 1, 1, new DateTime(2026, 9, 12)),
            CreateTask(Guid.NewGuid(), otherUserId, "Alpha other user", 1, 3, new DateTime(2026, 9, 13))
        );

        await context.SaveChangesAsync();

        TaskRepository repository = new TaskRepository(context);

        TaskPagedParams request = new TaskPagedParams
        {
            PageNumber = 1,
            PageSize = 1,
            Sort = ETaskSort.CreatedAt.ToString(),
            Order = ESortOrder.Asc.ToString(),
            TaskStatusId = 1,
            TaskPriorityId = 3,
            Search = "task",
            StartDate = new DateTime(2026, 9, 1),
            EndDate = new DateTime(2026, 10, 1)
        };

        var (tasks, totalCount) = await repository.GetPagedAsync(userId, request);
        List<TaskEntity> result = tasks.ToList();

        Assert.Equal(2, totalCount);
        Assert.Single(result);
        Assert.Equal("Alpha task", result[0].Title);
    }

    [Fact]
    public async Task ShouldDeleteOnlyProvidedTasks_WhenDeleteTasksIsCalled()
    {
        Guid userId = Guid.NewGuid();

        await using ApplicationDbContext context = CreateContext();

        SeedSelectables(context);

        TaskEntity first = CreateTask(Guid.NewGuid(), userId, "First", 1, 1);
        TaskEntity second = CreateTask(Guid.NewGuid(), userId, "Second", 1, 1);

        context.Tasks.AddRange(first, second);
        await context.SaveChangesAsync();

        TaskRepository repository = new TaskRepository(context);

        await repository.DeleteTasksAsync([first]);

        Assert.False(await context.Tasks.AnyAsync(task => task.Id == first.Id));
        Assert.True(await context.Tasks.AnyAsync(task => task.Id == second.Id));
    }

    [Fact]
    public async Task ShouldCreateTask_WhenCreateTaskIsCalled()
    {
        Guid userId = Guid.NewGuid();

        await using ApplicationDbContext context = CreateContext();
        SeedSelectables(context);
        await context.SaveChangesAsync();

        TaskRepository repository = new TaskRepository(context);
        TaskEntity task = CreateTask(Guid.NewGuid(), userId, "Created", 1, 2);

        await repository.CreateTaskAsync(task);

        Assert.True(await context.Tasks.AnyAsync(item => item.Id == task.Id));
    }

    [Fact]
    public async Task ShouldReturnOnlyOwnedRequestedTasks_WhenIdsAreProvided()
    {
        Guid userId = Guid.NewGuid();
        Guid otherUserId = Guid.NewGuid();

        await using ApplicationDbContext context = CreateContext();
        SeedSelectables(context);

        TaskEntity owned = CreateTask(Guid.NewGuid(), userId, "Owned", 1, 1);
        TaskEntity other = CreateTask(Guid.NewGuid(), otherUserId, "Other", 1, 1);

        context.Tasks.AddRange(owned, other);
        await context.SaveChangesAsync();

        TaskRepository repository = new TaskRepository(context);

        List<TaskEntity> result = await repository.GetTasksByIdsAsync(
            [owned.Id, other.Id],
            userId
        );

        Assert.Single(result);
        Assert.Equal(owned.Id, result[0].Id);
    }

    [Fact]
    public async Task ShouldPersistTaskChanges_WhenEditTaskIsCalled()
    {
        Guid userId = Guid.NewGuid();

        await using ApplicationDbContext context = CreateContext();
        SeedSelectables(context);

        TaskEntity task = CreateTask(Guid.NewGuid(), userId, "Old title", 1, 1);
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        TaskRepository repository = new TaskRepository(context);

        task.Title = "New title";
        await repository.EditTaskAsync(task);

        Assert.Equal(
            "New title",
            (await context.Tasks.SingleAsync(item => item.Id == task.Id)).Title
        );
    }

    [Fact]
    public async Task ShouldReturnStatusesAndPriorities_WhenSelectablesAreRequested()
    {
        await using ApplicationDbContext context = CreateContext();
        SeedSelectables(context);
        await context.SaveChangesAsync();

        TaskRepository repository = new TaskRepository(context);

        var statuses = await repository.GetTaskStatusesAsync();
        var priorities = await repository.GetTaskPrioritiesAsync();

        Assert.Equal(2, statuses.Count);
        Assert.Equal(3, priorities.Count);
    }

    [Fact]
    public async Task ShouldReturnFilteredOwnedTasks_WhenReportIsRequested()
    {
        Guid userId = Guid.NewGuid();
        Guid otherUserId = Guid.NewGuid();

        await using ApplicationDbContext context = CreateContext();
        SeedSelectables(context);

        context.Tasks.AddRange(
            CreateTask(Guid.NewGuid(), userId, "Inside", 1, 1, new DateTime(2026, 9, 10)),
            CreateTask(Guid.NewGuid(), userId, "Outside", 1, 1, new DateTime(2026, 8, 10)),
            CreateTask(Guid.NewGuid(), otherUserId, "Other user", 1, 1, new DateTime(2026, 9, 10))
        );

        await context.SaveChangesAsync();

        TaskRepository repository = new TaskRepository(context);

        var result = (await repository.GetReportAsync(
            userId,
            new ReportPagedParams
            {
                StartDate = new DateTime(2026, 9, 1),
                EndDate = new DateTime(2026, 10, 1)
            }
        )).ToList();

        Assert.Single(result);
        Assert.Equal("Inside", result[0].Title);
    }

    [Fact]
    public async Task ShouldSearchDescription_WhenTitleDoesNotMatch()
    {
        Guid userId = Guid.NewGuid();

        await using ApplicationDbContext context = CreateContext();
        SeedSelectables(context);

        TaskEntity task = CreateTask(Guid.NewGuid(), userId, "Different title", 1, 1);
        task.Description = "contains special term";

        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        TaskRepository repository = new TaskRepository(context);

        var (tasks, totalCount) = await repository.GetPagedAsync(
            userId,
            new TaskPagedParams { Search = "special" }
        );

        Assert.Equal(1, totalCount);
        Assert.Single(tasks);
    }

    private static ApplicationDbContext CreateContext()
    {
        DbContextOptions<ApplicationDbContext> options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new ApplicationDbContext(options);
    }

    private static void SeedSelectables(ApplicationDbContext context)
    {
        context.Status.AddRange(
            new TaskStatusEntity { Id = 1, Name = "Pendente" },
            new TaskStatusEntity { Id = 2, Name = "Em Progresso" }
        );

        context.Priorities.AddRange(
            new TaskPriority { Id = 1, Name = "Baixa" },
            new TaskPriority { Id = 2, Name = "Média" },
            new TaskPriority { Id = 3, Name = "Alta" }
        );
    }

    private static TaskEntity CreateTask(
        Guid id,
        Guid userId,
        string title,
        int statusId,
        int priorityId,
        DateTime? createdAt = null)
    {
        return new TaskEntity
        {
            Id = id,
            UserId = userId,
            Title = title,
            Description = $"{title} description",
            StatusId = statusId,
            PriorityId = priorityId,
            CreatedAt = createdAt ?? DateTime.UtcNow
        };
    }
}
