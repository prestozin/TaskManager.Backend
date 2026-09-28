using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Task.Request;
using TaskManager.Application.DTOs.Task.Response;
using TaskManager.Application.Interfaces;
using TaskManager.Core.Shared;

namespace TaskManager.Tests.Controllers;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class TaskControllerTests
{
    private readonly Mock<ITaskService> _taskService = new Mock<ITaskService>();

    [Test]
    public async Task ShouldReturnOk_WhenTaskExists()
    {
        // Arrange
        Guid taskId = Guid.NewGuid();

        _taskService
            .Setup(service => service.GetTaskByIdAsync(taskId))
            .ReturnsAsync(ResultResponse<TaskResponse>.Success(new TaskResponse { Id = taskId }));

        TaskController controller = new TaskController(_taskService.Object);

        // Act
        var result = await controller.GetByIdAsync(taskId);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        Guid taskId = Guid.NewGuid();

        _taskService
            .Setup(service => service.GetTaskByIdAsync(taskId))
            .ReturnsAsync(ResultResponse<TaskResponse>.Failure("Not found"));

        TaskController controller = new TaskController(_taskService.Object);

        // Act
        var result = await controller.GetByIdAsync(taskId);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task ShouldReturnOk_WhenPagedRequestIsProcessed()
    {
        // Arrange
        TaskPagedParams request = new TaskPagedParams();

        _taskService
            .Setup(service => service.GetPagedAsync(request))
            .ReturnsAsync(ResultResponse<PagedResultDto<TaskResponse>>.Success(
                new PagedResultDto<TaskResponse>([], 1, 10, 0)
            ));

        TaskController controller = new TaskController(_taskService.Object);

        // Act
        var result = await controller.GetPagedAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ShouldReturnOk_WhenCreateTaskSucceeds()
    {
        // Arrange
        CreateTaskRequest request = new CreateTaskRequest();

        _taskService
            .Setup(service => service.CreateTaskAsync(request))
            .ReturnsAsync(ResultResponse<string>.Success("Created"));

        TaskController controller = new TaskController(_taskService.Object);

        // Act
        var result = await controller.CreateTaskAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ShouldReturnBadRequest_WhenCreateTaskFails()
    {
        // Arrange
        CreateTaskRequest request = new CreateTaskRequest();

        _taskService
            .Setup(service => service.CreateTaskAsync(request))
            .ReturnsAsync(ResultResponse<string>.Failure("Failed"));

        TaskController controller = new TaskController(_taskService.Object);

        // Act
        var result = await controller.CreateTaskAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task ShouldReturnOk_WhenEditTaskSucceeds()
    {
        // Arrange
        EditTaskRequest request = new EditTaskRequest();

        _taskService
            .Setup(service => service.EditTaskAsync(request))
            .ReturnsAsync(ResultResponse<string>.Success("Updated"));

        TaskController controller = new TaskController(_taskService.Object);

        // Act
        var result = await controller.EditTaskAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ShouldReturnNotFound_WhenEditTaskFails()
    {
        // Arrange
        EditTaskRequest request = new EditTaskRequest();

        _taskService
            .Setup(service => service.EditTaskAsync(request))
            .ReturnsAsync(ResultResponse<string>.Failure("Not found"));

        TaskController controller = new TaskController(_taskService.Object);

        // Act
        var result = await controller.EditTaskAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task ShouldReturnOk_WhenDeleteTaskSucceeds()
    {
        // Arrange
        DeleteTaskRequest request = new DeleteTaskRequest { TaskId = [Guid.NewGuid()] };

        _taskService
            .Setup(service => service.DeleteTaskAsync(request))
            .ReturnsAsync(ResultResponse<string>.Success("Deleted"));

        TaskController controller = new TaskController(_taskService.Object);

        // Act
        var result = await controller.DeleteTaskAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ShouldReturnNotFound_WhenDeleteTaskFails()
    {
        // Arrange
        DeleteTaskRequest request = new DeleteTaskRequest { TaskId = [Guid.NewGuid()] };

        _taskService
            .Setup(service => service.DeleteTaskAsync(request))
            .ReturnsAsync(ResultResponse<string>.Failure("Not found"));

        TaskController controller = new TaskController(_taskService.Object);

        // Act
        var result = await controller.DeleteTaskAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task ShouldReturnOk_WhenSelectablesAreRequested()
    {
        // Arrange
        _taskService
            .Setup(service => service.GetSelectablesAsync())
            .ReturnsAsync(ResultResponse<TaskSelectablesResponse>.Success(new TaskSelectablesResponse()));

        TaskController controller = new TaskController(_taskService.Object);

        // Act
        var result = await controller.GetSelectablesAsync();

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ShouldReturnOk_WhenReportIsRequested()
    {
        // Arrange
        ReportPagedParams request = new ReportPagedParams();

        _taskService
            .Setup(service => service.GetReportAsync(request))
            .ReturnsAsync(ResultResponse<TaskReportResponse>.Success(new TaskReportResponse()));

        TaskController controller = new TaskController(_taskService.Object);

        // Act
        var result = await controller.GetReportAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }
}
