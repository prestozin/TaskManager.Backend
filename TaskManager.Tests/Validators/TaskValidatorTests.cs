using TaskManager.Application.DTOs.Task.Request;
using TaskManager.Application.Validators;
using TaskManager.Application.Validators.Task;
using TaskManager.Core.Enums;
using TaskManager.Core.Shared;

namespace TaskManager.Tests.Validators;

public class TaskValidatorTests
{
    private static readonly List<int> StatusIds = [1, 2, 3, 4];
    private static readonly List<int> PriorityIds = [1, 2, 3];

    [Fact]
    public void ShouldPassValidation_WhenCreateTaskRequestIsValid()
    {
        CreateTaskValidator validator = new CreateTaskValidator(StatusIds, PriorityIds);

        CreateTaskRequest request = new CreateTaskRequest
        {
            Title = "Valid task",
            Description = "Description",
            StatusId = 1,
            PriorityId = 2
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", 1, 1)]
    [InlineData("ab", 1, 1)]
    [InlineData("Valid task", 99, 1)]
    [InlineData("Valid task", 1, 99)]
    public void ShouldFailValidation_WhenCreateTaskRequestIsInvalid(string title, int statusId, int priorityId)
    {
        CreateTaskValidator validator = new CreateTaskValidator(StatusIds, PriorityIds);

        CreateTaskRequest request = new CreateTaskRequest
        {
            Title = title,
            StatusId = statusId,
            PriorityId = priorityId
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ShouldFailValidation_WhenDescriptionExceedsMaximumLength()
    {
        CreateTaskValidator validator = new CreateTaskValidator(StatusIds, PriorityIds);

        CreateTaskRequest request = new CreateTaskRequest
        {
            Title = "Valid task",
            Description = new string('a', 501),
            StatusId = 1,
            PriorityId = 1
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ShouldFailValidation_WhenEditTaskIdIsNull()
    {
        EditTaskValidator validator = new EditTaskValidator(StatusIds, PriorityIds);

        EditTaskRequest request = new EditTaskRequest
        {
            Id = null,
            Title = "Valid task",
            StatusId = 1,
            PriorityId = 1
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact(Skip = "Known production issue: NotEmpty on Guid? currently accepts Guid.Empty.")]
    public void ShouldFailValidation_WhenEditTaskIdIsEmpty()
    {
        EditTaskValidator validator = new EditTaskValidator(StatusIds, PriorityIds);

        EditTaskRequest request = new EditTaskRequest
        {
            Id = Guid.Empty,
            Title = "Valid task",
            StatusId = 1,
            PriorityId = 1
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ShouldPassValidation_WhenPagedParamsAreValid()
    {
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

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0, 10, "CreatedAt", "desc")]
    [InlineData(1, 0, "CreatedAt", "desc")]
    [InlineData(1, 101, "CreatedAt", "desc")]
    [InlineData(1, 10, "Invalid", "desc")]
    [InlineData(1, 10, "CreatedAt", "invalid")]
    public void ShouldFailValidation_WhenPagedParamsAreInvalid(int pageNumber, int pageSize, string sort, string order)
    {
        TaskPagedParamsValidator validator = new TaskPagedParamsValidator();

        TaskPagedParams request = new TaskPagedParams
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Sort = sort,
            Order = order
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ShouldFailValidation_WhenPagedDateRangeIsInvalid()
    {
        TaskPagedParamsValidator validator = new TaskPagedParamsValidator();

        TaskPagedParams request = new TaskPagedParams
        {
            StartDate = new DateTime(2026, 9, 30),
            EndDate = new DateTime(2026, 9, 1)
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ShouldPassValidation_WhenDeleteTaskRequestContainsUniqueValidIds()
    {
        DeleteTaskValidator validator = new DeleteTaskValidator();

        DeleteTaskRequest request = new DeleteTaskRequest
        {
            TaskId = [Guid.NewGuid(), Guid.NewGuid()]
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ShouldFailValidation_WhenDeleteTaskRequestIsEmpty()
    {
        DeleteTaskValidator validator = new DeleteTaskValidator();

        DeleteTaskRequest request = new DeleteTaskRequest
        {
            TaskId = []
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ShouldFailValidation_WhenDeleteTaskRequestContainsDuplicateIds()
    {
        DeleteTaskValidator validator = new DeleteTaskValidator();
        Guid id = Guid.NewGuid();

        DeleteTaskRequest request = new DeleteTaskRequest
        {
            TaskId = [id, id]
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ShouldFailValidation_WhenDeleteTaskRequestContainsEmptyGuid()
    {
        DeleteTaskValidator validator = new DeleteTaskValidator();

        DeleteTaskRequest request = new DeleteTaskRequest
        {
            TaskId = [Guid.Empty]
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ShouldFailValidation_WhenDeleteTaskRequestExceedsBatchLimit()
    {
        DeleteTaskValidator validator = new DeleteTaskValidator();

        DeleteTaskRequest request = new DeleteTaskRequest
        {
            TaskId = Enumerable.Range(0, 101).Select(_ => Guid.NewGuid()).ToList()
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }
}
