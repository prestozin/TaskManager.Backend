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

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
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

    [Test]
    public async Task ShouldReturnFailure_WhenTaskIdIsEmpty()
    {
        // Arrange
        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        // Act
        var result = await service.GetTaskByIdAsync(Guid.Empty);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        _taskRepository.Verify(
            repository => repository.GetTaskByIdAsync(It.IsAny<Guid?>(), It.IsAny<Guid>()),
            Times.Never
        );
    }

    [Test]
    public async Task ShouldReturnFailure_WhenTaskDoesNotExist()
    {
        // Arrange
        Guid taskId = Guid.NewGuid();

        _taskRepository
            .Setup(repository => repository.GetTaskByIdAsync(taskId, _userId))
            .ReturnsAsync((TaskEntity?)null);

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        // Act
        var result = await service.GetTaskByIdAsync(taskId);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public async Task ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        Guid taskId = Guid.NewGuid();

        TaskEntity task = CreateTask(taskId, _userId, 1, 2);

        _taskRepository
            .Setup(repository => repository.GetTaskByIdAsync(taskId, _userId))
            .ReturnsAsync(task);

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        // Act
        var result = await service.GetTaskByIdAsync(taskId);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data!.Id, Is.EqualTo(taskId));
        Assert.That(result.Data.Status, Is.EqualTo("Pendente"));
        Assert.That(result.Data.Priority, Is.EqualTo("Média"));
    }

    [Test]
    public async Task ShouldUseCurrentUserAndNormalizeFields_WhenCreateTaskSucceeds()
    {
        // Arrange
        SetupSelectables();

        TaskEntity? createdTask = null;

        _taskRepository
            .Setup(repository => repository.CreateTaskAsync(It.IsAny<TaskEntity>()))
            .Callback<TaskEntity>(task => createdTask = task)
            .Returns(Task.CompletedTask);

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        // Act
        var result = await service.CreateTaskAsync(new CreateTaskRequest
        {
            Title = "  Task title  ",
            Description = "   ",
            StatusId = 1,
            PriorityId = 2
        });

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(createdTask, Is.Not.Null);
        Assert.That(createdTask!.UserId, Is.EqualTo(_userId));
        Assert.That(createdTask.Title, Is.EqualTo("Task title"));
        Assert.That(createdTask.Description, Is.Null);
    }

    [Test]
    public async Task ShouldUpdateExistingTask_WhenEditTaskSucceeds()
    {
        // Arrange
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

        // Act
        var result = await service.EditTaskAsync(new EditTaskRequest
        {
            Id = taskId,
            Title = "  Updated title  ",
            Description = " Updated description ",
            StatusId = 2,
            PriorityId = 3
        });

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(task.Id, Is.EqualTo(taskId));
        Assert.That(task.Title, Is.EqualTo("Updated title"));
        Assert.That(task.Description, Is.EqualTo("Updated description"));
        Assert.That(task.StatusId, Is.EqualTo(2));
        Assert.That(task.PriorityId, Is.EqualTo(3));
        _taskRepository.Verify(repository => repository.EditTaskAsync(task), Times.Once);
    }

    [Test]
    public async Task ShouldReturnFailure_WhenNoTaskIsFoundForDelete()
    {
        // Arrange
        Guid taskId = Guid.NewGuid();

        _taskRepository
            .Setup(repository => repository.GetTasksByIdsAsync(It.IsAny<List<Guid>>(), _userId))
            .ReturnsAsync([]);

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        // Act
        var result = await service.DeleteTaskAsync(new DeleteTaskRequest
        {
            TaskId = [taskId]
        });

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        _taskRepository.Verify(repository => repository.DeleteTasksAsync(It.IsAny<List<TaskEntity>>()), Times.Never);
    }

    [Test]
    public async Task ShouldReturnPartialSuccess_WhenSomeTasksAreNotFoundForDelete()
    {
        // Arrange
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

        // Act
        var result = await service.DeleteTaskAsync(new DeleteTaskRequest
        {
            TaskId = [existingId, missingId]
        });

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Message, Does.Contain("1 tarefa(s) excluída(s)"));
        Assert.That(result.Message, Does.Contain("1 tarefa(s) não encontrada(s)"));
    }

    [Test]
    public async Task ShouldReturnPagedTasks_WhenPagedQuerySucceeds()
    {
        // Arrange
        TaskEntity task = CreateTask(Guid.NewGuid(), _userId, 1, 1);

        _taskRepository
            .Setup(repository => repository.GetPagedAsync(_userId, It.IsAny<TaskPagedParams>()))
            .ReturnsAsync((new List<TaskEntity> { task }.AsEnumerable(), 12));

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        // Act
        var result = await service.GetPagedAsync(new TaskPagedParams
        {
            PageNumber = 2,
            PageSize = 10
        });

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data!.TotalCount, Is.EqualTo(12));
        Assert.That(result.Data.PageNumber, Is.EqualTo(2));
        Assert.That(result.Data.TotalPages, Is.EqualTo(2));
        Assert.That(result.Data.Items!.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldBuildReportCategories_WhenReportContainsTasks()
    {
        // Arrange
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

        // Act
        var result = await service.GetReportAsync(new ReportPagedParams
        {
            PageNumber = 1,
            PageSize = 10
        });

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data!.TotalTasks, Is.EqualTo(3));

        var pending = result.Data.Status.Single(item => item.Id == 1);
        Assert.That(pending.Count, Is.EqualTo(2));
        Assert.That(pending.Percentage, Is.EqualTo(66.67m));

        var medium = result.Data.Priority.Single(item => item.Id == 2);
        Assert.That(medium.Count, Is.EqualTo(2));
        Assert.That(medium.Percentage, Is.EqualTo(66.67m));
    }

    [Test]
    public async Task ShouldReturnSelectables_WhenStatusesAndPrioritiesExist()
    {
        // Arrange
        SetupSelectables();

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        // Act
        var result = await service.GetSelectablesAsync();

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Status!.Count, Is.EqualTo(2));
        Assert.That(result.Data.Priority!.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task ShouldReturnZeroPercentage_WhenReportHasNoTasks()
    {
        // Arrange
        _taskRepository
            .Setup(repository => repository.GetReportAsync(_userId, It.IsAny<ReportPagedParams>()))
            .ReturnsAsync([]);

        _taskRepository
            .Setup(repository => repository.GetPagedAsync(_userId, It.IsAny<TaskPagedParams>()))
            .ReturnsAsync((Enumerable.Empty<TaskEntity>(), 0));

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        // Act
        var result = await service.GetReportAsync(new ReportPagedParams
        {
            PageNumber = 1,
            PageSize = 10
        });

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data!.TotalTasks, Is.EqualTo(0));
        Assert.That(result.Data.Status, Is.Empty);
        Assert.That(result.Data.Priority, Is.Empty);
    }

    [Test]
    public async Task ShouldReturnFailure_WhenTaskDoesNotExistForEdit()
    {
        // Arrange
        SetupSelectables();

        Guid taskId = Guid.NewGuid();

        _taskRepository
            .Setup(repository => repository.GetTaskByIdAsync(taskId, _userId))
            .ReturnsAsync((TaskEntity?)null);

        TaskService service = new TaskService(_taskRepository.Object, _currentUserContext.Object);

        // Act
        var result = await service.EditTaskAsync(new EditTaskRequest
        {
            Id = taskId,
            Title = "Task",
            StatusId = 1,
            PriorityId = 1
        });

        // Assert
        Assert.That(result.IsSuccess, Is.False);
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
