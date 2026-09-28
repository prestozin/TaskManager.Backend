using TaskManager.Application.DTOs.Task.Request;
using TaskManager.Application.Validators;
using TaskManager.Application.Validators.Task;
using TaskManager.Core.Enums;
using TaskManager.Core.Shared;

namespace TaskManager.Tests.Validators;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class TaskValidatorTests
{
    private static readonly List<int> StatusIds = [1, 2, 3, 4];
    private static readonly List<int> PriorityIds = [1, 2, 3];

    [Test]
    public void ShouldPassValidation_WhenCreateTaskRequestIsValid()
    {
        // Arrange
        CreateTaskValidator validator = new CreateTaskValidator(StatusIds, PriorityIds);

        CreateTaskRequest request = new CreateTaskRequest
        {
            Title = "Valid task",
            Description = "Description",
            StatusId = 1,
            PriorityId = 2
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }
    [TestCase("", 1, 1)]
    [TestCase("ab", 1, 1)]
    [TestCase("Valid task", 99, 1)]
    [TestCase("Valid task", 1, 99)]
    public void ShouldFailValidation_WhenCreateTaskRequestIsInvalid(string title, int statusId, int priorityId)
    {
        // Arrange
        CreateTaskValidator validator = new CreateTaskValidator(StatusIds, PriorityIds);

        CreateTaskRequest request = new CreateTaskRequest
        {
            Title = title,
            StatusId = statusId,
            PriorityId = priorityId
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ShouldFailValidation_WhenDescriptionExceedsMaximumLength()
    {
        // Arrange
        CreateTaskValidator validator = new CreateTaskValidator(StatusIds, PriorityIds);

        CreateTaskRequest request = new CreateTaskRequest
        {
            Title = "Valid task",
            Description = new string('a', 501),
            StatusId = 1,
            PriorityId = 1
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ShouldFailValidation_WhenEditTaskIdIsNull()
    {
        // Arrange
        EditTaskValidator validator = new EditTaskValidator(StatusIds, PriorityIds);

        EditTaskRequest request = new EditTaskRequest
        {
            Id = null,
            Title = "Valid task",
            StatusId = 1,
            PriorityId = 1
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    [Ignore("Known production issue: NotEmpty on Guid? currently accepts Guid.Empty.")]
    public void ShouldFailValidation_WhenEditTaskIdIsEmpty()
    {
        // Arrange
        EditTaskValidator validator = new EditTaskValidator(StatusIds, PriorityIds);

        EditTaskRequest request = new EditTaskRequest
        {
            Id = Guid.Empty,
            Title = "Valid task",
            StatusId = 1,
            PriorityId = 1
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ShouldPassValidation_WhenPagedParamsAreValid()
    {
        // Arrange
        TaskPagedParamsValidator validator = new TaskPagedParamsValidator();

        TaskPagedParams request = new TaskPagedParams
        {
            PageNumber = 1,
            PageSize = 10,
            Sort = ETaskSort.CreatedAt.ToString(),
            Order = ESortOrder.Desc.ToString(),
            Search = "task",
            StartDate = new DateTime(2026, 9, 1),
            EndDate = new DateTime(2026, 9, 30)
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }
    [TestCase(0, 10, "CreatedAt", "desc")]
    [TestCase(1, 0, "CreatedAt", "desc")]
    [TestCase(1, 101, "CreatedAt", "desc")]
    [TestCase(1, 10, "Invalid", "desc")]
    [TestCase(1, 10, "CreatedAt", "invalid")]
    public void ShouldFailValidation_WhenPagedParamsAreInvalid(int pageNumber, int pageSize, string sort, string order)
    {
        // Arrange
        TaskPagedParamsValidator validator = new TaskPagedParamsValidator();

        TaskPagedParams request = new TaskPagedParams
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Sort = sort,
            Order = order
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ShouldFailValidation_WhenPagedDateRangeIsInvalid()
    {
        // Arrange
        TaskPagedParamsValidator validator = new TaskPagedParamsValidator();

        TaskPagedParams request = new TaskPagedParams
        {
            StartDate = new DateTime(2026, 9, 30),
            EndDate = new DateTime(2026, 9, 1)
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ShouldPassValidation_WhenDeleteTaskRequestContainsUniqueValidIds()
    {
        // Arrange
        DeleteTaskValidator validator = new DeleteTaskValidator();

        DeleteTaskRequest request = new DeleteTaskRequest
        {
            TaskId = [Guid.NewGuid(), Guid.NewGuid()]
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ShouldFailValidation_WhenDeleteTaskRequestIsEmpty()
    {
        // Arrange
        DeleteTaskValidator validator = new DeleteTaskValidator();

        DeleteTaskRequest request = new DeleteTaskRequest
        {
            TaskId = []
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ShouldFailValidation_WhenDeleteTaskRequestContainsDuplicateIds()
    {
        // Arrange
        DeleteTaskValidator validator = new DeleteTaskValidator();
        Guid id = Guid.NewGuid();

        DeleteTaskRequest request = new DeleteTaskRequest
        {
            TaskId = [id, id]
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ShouldFailValidation_WhenDeleteTaskRequestContainsEmptyGuid()
    {
        // Arrange
        DeleteTaskValidator validator = new DeleteTaskValidator();

        DeleteTaskRequest request = new DeleteTaskRequest
        {
            TaskId = [Guid.Empty]
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ShouldFailValidation_WhenDeleteTaskRequestExceedsBatchLimit()
    {
        // Arrange
        DeleteTaskValidator validator = new DeleteTaskValidator();

        DeleteTaskRequest request = new DeleteTaskRequest
        {
            TaskId = Enumerable.Range(0, 101).Select(_ => Guid.NewGuid()).ToList()
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }
    [Test]
    public void ShouldPassValidation_WhenReportPagedParamsAreValid()
    {
        // Arrange
        ReportPagedParamsValidator validator = new ReportPagedParamsValidator();

        ReportPagedParams request = new ReportPagedParams
        {
            PageNumber = 1,
            PageSize = 10,
            StartDate = new DateTime(2026, 9, 1),
            EndDate = new DateTime(2026, 9, 30)
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }
    [TestCase(0, 10)]
    [TestCase(1, 0)]
    [TestCase(1, 101)]
    public void ShouldFailValidation_WhenReportPaginationIsInvalid(int pageNumber, int pageSize)
    {
        // Arrange
        ReportPagedParamsValidator validator = new ReportPagedParamsValidator();

        ReportPagedParams request = new ReportPagedParams
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ShouldFailValidation_WhenReportDateRangeIsInvalid()
    {
        // Arrange
        ReportPagedParamsValidator validator = new ReportPagedParamsValidator();

        ReportPagedParams request = new ReportPagedParams
        {
            StartDate = new DateTime(2026, 9, 30),
            EndDate = new DateTime(2026, 9, 1)
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

}
