using Mapster;
using Moq;
using TaskManager.Application.DTOs.Task.Request;
using TaskManager.Application.Mappings;
using TaskManager.Application.Services;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Shared;
using TaskStatusEntity = TaskManager.Core.Entities.TaskStatus;

namespace TaskManager.Tests.Services;

public class TaskServiceTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Mock<ITaskRepository> _taskRepository = new Mock<ITaskRepository>();
    private readonly Mock<ICurrentUserContext> _currentUserContext = new Mock<ICurrentUserContext>();

    static TaskServiceTests()
    {
        TypeAdapterConfig.GlobalSettings.Scan(typeof(TaskMapping).Assembly);
    }

    public TaskServiceTests()
    {
        _currentUserContext
            .Setup(context => context.UserId)
            .Returns(_userId);
    }

    [Fact]
    public async Task ShouldReturnFailure_WhenTaskIdIsEmpty()
    {
        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        var result = await service.GetTaskByIdAsync(Guid.Empty);

        Assert.False(result.IsSuccess);
        _taskRepository.Verify(
            repository => repository.GetTaskByIdAsync(It.IsAny<Guid?>(), It.IsAny<Guid>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ShouldReturnFailure_WhenTaskDoesNotExist()
    {
        Guid taskId = Guid.NewGuid();

        _taskRepository
            .Setup(repository => repository.GetTaskByIdAsync(taskId, _userId))
            .ReturnsAsync((TaskEntity?)null);

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        var result = await service.GetTaskByIdAsync(taskId);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ShouldReturnTask_WhenTaskExists()
    {
        Guid taskId = Guid.NewGuid();

        TaskEntity task = CreateTask(taskId, _userId, 1, 2);

        _taskRepository
            .Setup(repository => repository.GetTaskByIdAsync(taskId, _userId))
            .ReturnsAsync(task);

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        var result = await service.GetTaskByIdAsync(taskId);

        Assert.True(result.IsSuccess);
        Assert.Equal(taskId, result.Data!.Id);
        Assert.Equal("Pendente", result.Data.Status);
        Assert.Equal("Média", result.Data.Priority);
    }

    [Fact]
    public async Task ShouldUseCurrentUserAndNormalizeFields_WhenCreateTaskSucceeds()
    {
        SetupSelectables();

        TaskEntity? createdTask = null;

        _taskRepository
            .Setup(repository => repository.CreateTaskAsync(It.IsAny<TaskEntity>()))
            .Callback<TaskEntity>(task => createdTask = task)
            .Returns(Task.CompletedTask);

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        var result = await service.CreateTaskAsync(new CreateTaskRequest
        {
            Title = "  Task title  ",
            Description = "   ",
            StatusId = 1,
            PriorityId = 2
        });

        Assert.True(result.IsSuccess);
        Assert.NotNull(createdTask);
        Assert.Equal(_userId, createdTask!.UserId);
        Assert.Equal("Task title", createdTask.Title);
        Assert.Null(createdTask.Description);
    }

    [Fact]
    public async Task ShouldUpdateExistingTask_WhenEditTaskSucceeds()
    {
        SetupSelectables();

        Guid taskId = Guid.NewGuid();
        TaskEntity task = CreateTask(taskId, _userId, 1, 1);

        _taskRepository
            .Setup(repository => repository.GetTaskByIdAsync(taskId, _userId))
            .ReturnsAsync(task);

        _taskRepository
            .Setup(repository => repository.EditTaskAsync(task))
            .Returns(Task.CompletedTask);

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        var result = await service.EditTaskAsync(new EditTaskRequest
        {
            Id = taskId,
            Title = "  Updated title  ",
            Description = " Updated description ",
            StatusId = 2,
            PriorityId = 3
        });

        Assert.True(result.IsSuccess);
        Assert.Equal(taskId, task.Id);
        Assert.Equal("Updated title", task.Title);
        Assert.Equal("Updated description", task.Description);
        Assert.Equal(2, task.StatusId);
        Assert.Equal(3, task.PriorityId);
        _taskRepository.Verify(repository => repository.EditTaskAsync(task), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnFailure_WhenNoTaskIsFoundForDelete()
    {
        Guid taskId = Guid.NewGuid();

        _taskRepository
            .Setup(repository => repository.GetTasksByIdsAsync(It.IsAny<List<Guid>>(), _userId))
            .ReturnsAsync([]);

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        var result = await service.DeleteTaskAsync(new DeleteTaskRequest
        {
            TaskId = [taskId]
        });

        Assert.False(result.IsSuccess);
        _taskRepository.Verify(repository => repository.DeleteTasksAsync(It.IsAny<List<TaskEntity>>()), Times.Never);
    }

    [Fact]
    public async Task ShouldReturnPartialSuccess_WhenSomeTasksAreNotFoundForDelete()
    {
        Guid existingId = Guid.NewGuid();
        Guid missingId = Guid.NewGuid();

        List<TaskEntity> foundTasks =
        [
            CreateTask(existingId, _userId, 1, 1)
        ];

        _taskRepository
            .Setup(repository => repository.GetTasksByIdsAsync(It.IsAny<List<Guid>>(), _userId))
            .ReturnsAsync(foundTasks);

        _taskRepository
            .Setup(repository => repository.DeleteTasksAsync(foundTasks))
            .Returns(Task.CompletedTask);

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        var result = await service.DeleteTaskAsync(new DeleteTaskRequest
        {
            TaskId = [existingId, missingId]
        });

        Assert.True(result.IsSuccess);
        Assert.Contains("1 tarefa(s) excluída(s)", result.Message);
        Assert.Contains("1 tarefa(s) não encontrada(s)", result.Message);
    }

    [Fact]
    public async Task ShouldReturnPagedTasks_WhenPagedQuerySucceeds()
    {
        TaskEntity task = CreateTask(Guid.NewGuid(), _userId, 1, 1);

        _taskRepository
            .Setup(repository => repository.GetPagedAsync(_userId, It.IsAny<TaskPagedParams>()))
            .ReturnsAsync((new List<TaskEntity> { task }.AsEnumerable(), 12));

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        var result = await service.GetPagedAsync(new TaskPagedParams
        {
            PageNumber = 2,
            PageSize = 10
        });

        Assert.True(result.IsSuccess);
        Assert.Equal(12, result.Data!.TotalCount);
        Assert.Equal(2, result.Data.PageNumber);
        Assert.Equal(2, result.Data.TotalPages);
        Assert.Single(result.Data.Items!);
    }

    [Fact]
    public async Task ShouldBuildReportCategories_WhenReportContainsTasks()
    {
        TaskEntity first = CreateTask(Guid.NewGuid(), _userId, 1, 1);
        TaskEntity second = CreateTask(Guid.NewGuid(), _userId, 1, 2);
        TaskEntity third = CreateTask(Guid.NewGuid(), _userId, 2, 2);

        List<TaskEntity> tasks = [first, second, third];

        _taskRepository
            .Setup(repository => repository.GetReportAsync(_userId, It.IsAny<ReportPagedParams>()))
            .ReturnsAsync(tasks);

        _taskRepository
            .Setup(repository => repository.GetPagedAsync(_userId, It.IsAny<TaskPagedParams>()))
            .ReturnsAsync((tasks.AsEnumerable(), tasks.Count));

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        var result = await service.GetReportAsync(new ReportPagedParams
        {
            PageNumber = 1,
            PageSize = 10
        });

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Data!.TotalTasks);

        var pending = Assert.Single(result.Data.Status, item => item.Id == 1);
        Assert.Equal(2, pending.Count);
        Assert.Equal(66.67m, pending.Percentage);

        var medium = Assert.Single(result.Data.Priority, item => item.Id == 2);
        Assert.Equal(2, medium.Count);
        Assert.Equal(66.67m, medium.Percentage);
    }

    [Fact]
    public async Task ShouldReturnSelectables_WhenStatusesAndPrioritiesExist()
    {
        SetupSelectables();

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        var result = await service.GetSelectablesAsync();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.Status!.Count);
        Assert.Equal(3, result.Data.Priority!.Count);
    }

    [Fact]
    public async Task ShouldReturnZeroPercentage_WhenReportHasNoTasks()
    {
        _taskRepository
            .Setup(repository => repository.GetReportAsync(_userId, It.IsAny<ReportPagedParams>()))
            .ReturnsAsync([]);

        _taskRepository
            .Setup(repository => repository.GetPagedAsync(_userId, It.IsAny<TaskPagedParams>()))
            .ReturnsAsync((Enumerable.Empty<TaskEntity>(), 0));

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        var result = await service.GetReportAsync(new ReportPagedParams
        {
            PageNumber = 1,
            PageSize = 10
        });

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Data!.TotalTasks);
        Assert.Empty(result.Data.Status);
        Assert.Empty(result.Data.Priority);
    }

    [Fact]
    public async Task ShouldReturnFailure_WhenTaskDoesNotExistForEdit()
    {
        SetupSelectables();

        Guid taskId = Guid.NewGuid();

        _taskRepository
            .Setup(repository => repository.GetTaskByIdAsync(taskId, _userId))
            .ReturnsAsync((TaskEntity?)null);

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        var result = await service.EditTaskAsync(new EditTaskRequest
        {
            Id = taskId,
            Title = "Task",
            StatusId = 1,
            PriorityId = 1
        });

        Assert.False(result.IsSuccess);
        _taskRepository.Verify(repository => repository.EditTaskAsync(It.IsAny<TaskEntity>()), Times.Never);
    }

    private void SetupSelectables()
    {
        _taskRepository
            .Setup(repository => repository.GetTaskStatusesAsync())
            .ReturnsAsync(
            [
                new TaskStatusEntity { Id = 1, Name = "Pendente" },
                new TaskStatusEntity { Id = 2, Name = "Em Progresso" }
            ]);

        _taskRepository
            .Setup(repository => repository.GetTaskPrioritiesAsync())
            .ReturnsAsync(
            [
                new TaskPriority { Id = 1, Name = "Baixa" },
                new TaskPriority { Id = 2, Name = "Média" },
                new TaskPriority { Id = 3, Name = "Alta" }
            ]);
    }

    private static TaskEntity CreateTask(Guid id, Guid userId, int statusId, int priorityId)
    {
        string statusName = statusId == 1 ? "Pendente" : "Em Progresso";
        string priorityName = priorityId switch
        {
            1 => "Baixa",
            2 => "Média",
            _ => "Alta"
        };

        return new TaskEntity
        {
            Id = id,
            Title = "Task",
            UserId = userId,
            StatusId = statusId,
            PriorityId = priorityId,
            TaskStatus = new TaskStatusEntity { Id = statusId, Name = statusName },
            TaskPriority = new TaskPriority { Id = priorityId, Name = priorityName }
        };
    }
}
